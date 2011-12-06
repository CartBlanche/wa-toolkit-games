namespace Microsoft.Samples.SocialGames.Worker
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Security.Permissions;
    using System.Threading;
    using Autofac;
    using Microsoft.Samples.SocialGames;
    using Microsoft.Samples.SocialGames.Common.JobEngine;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.Samples.SocialGames.Extensions;
    using Microsoft.Samples.SocialGames.Repositories;
    using Microsoft.Samples.SocialGames.Worker.Commands;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Diagnostics;
    using Microsoft.WindowsAzure.Diagnostics.Management;
    using Microsoft.WindowsAzure.ServiceRuntime;
    using Microsoft.Samples.SocialGames.Common;

    public class WorkerRole : RoleEntryPoint
    {
        private static string wadConnectionString = "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString";

        public static IEnumerable<string> ConfiguredCounters
        {
            get
            {
                yield return @"\Processor(_Total)\% Processor Time";
                yield return @"\Memory\Available MBytes";
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static bool IsDevelopmentEnvironment()
        {
            return !RoleEnvironment.IsAvailable ||
            (RoleEnvironment.IsAvailable && RoleEnvironment.DeploymentId.StartsWith("deployment", StringComparison.OrdinalIgnoreCase));
        }

        public override void Run()
        {
            // Setup AutoFac
            var builder = new ContainerBuilder();
            DependancySetup(builder);
            var container = builder.Build();

            // Call Initializers
            var initializers = container.Resolve<IEnumerable<IInitializer>>();
            foreach (var initializer in initializers)
            {
                initializer.Initialize();
            }

            var account = container.Resolve<CloudStorageAccount>();
            var userRepository = container.Resolve<IUserRepository>();
            var gameRepository = container.Resolve<IGameRepository>();
            var workerContext = container.Resolve<IWorkerContext>();

            // TaskBuilder callback for logging errors
            Action<ICommand, IDictionary<string, object>, Exception> logException = (cmd, context, ex) =>
            {
                Trace.TraceError(ex.ToString());
            };

            // Game Queue for Skirmish game
            Task.TriggeredBy(Message.OfType<SkirmishGameQueueMessage>(account, ConfigurationConstants.SkirmishGameQueue))
                .SetupContext((message, context) =>
                {
                    context.Add("userId", message.UserId);
                })
                .Do(container.Resolve<SkirmishGameQueueCommand>())
                .OnError(logException)
                .Start();

            // Leave game messages
            Task.TriggeredBy(Message.OfType<LeaveGameMessage>(account, ConfigurationConstants.LeaveGameQueue))
                .SetupContext((message, context) =>
                {
                    context.Add("userId", message.UserId);
                    context.Add("gameId", message.GameId);
                })
                .Do(container.Resolve<LeaveGameCommand>())
                .OnError(logException)
                .Start();

            // Game Action for Notification messages
            Task.TriggeredBy(Message.OfType<GameActionNotificationMessage>(account, ConfigurationConstants.GameActionNotificationsQueue))
                .SetupContext((message, context) =>
                {
                    context.Add("gameAction", message.GameAction);
                })
                .Do(container.Resolve<GameActionNotificationCommand>())
                .OnError(logException)
                .Start();

            // Game Action for Statistics messages
            Task.TriggeredBy(Message.OfType<GameActionStatisticsMessage>(account, ConfigurationConstants.GameActionStatisticsQueue))
                .SetupContext((message, context) =>
                {
                    context.Add("gameAction", message.GameAction);
                })
                .Do(container.Resolve<GameActionStatisticsCommand>())
                .OnError(logException)
                .Start();

            // Process for Invite messages
            Task.TriggeredBy(Message.OfType<InviteMessage>(account, ConfigurationConstants.InvitesQueue))
                .SetupContext((message, context) =>
                {
                    context.Add("userId", message.UserId);
                    context.Add("invitedUserId", message.InvitedUserId);
                    context.Add("gameQueueId", message.GameQueueId);
                    context.Add("timestamp", message.Timestamp);
                    context.Add("message", message.Message);
                    context.Add("url", message.Url);
                })
                .Do(container.Resolve<InviteCommand>())
                .OnError(logException)
                .Start();

            // Timeout for game queue
            Task.TriggeredBy(Schedule.Every(5 * 1000))
                .Do(container.Resolve<GameQueueAutoStartCommand>())
                .OnError(logException)
                .Start();

            while (true)
            {
                Thread.Sleep(1000);
            }
        }

        protected void DependancySetup(ContainerBuilder builder)
        {
            // Cloud Storage Account
            builder.RegisterInstance<CloudStorageAccount>(CloudStorageAccount.FromConfigurationSetting("DataConnectionString"));

            // Queues
            builder.RegisterQueue<GameActionStatisticsMessage>(ConfigurationConstants.GameActionStatisticsQueue)
                .AsImplementedInterfaces();
            builder.RegisterQueue<GameActionNotificationMessage>(ConfigurationConstants.GameActionNotificationsQueue)
                .AsImplementedInterfaces();
            builder.RegisterQueue<LeaveGameMessage>(ConfigurationConstants.LeaveGameQueue)
                .AsImplementedInterfaces();
            builder.RegisterQueue<InviteMessage>(ConfigurationConstants.InvitesQueue)
                .AsImplementedInterfaces();
            builder.RegisterQueue<SkirmishGameQueueMessage>(ConfigurationConstants.SkirmishGameQueue)
                .AsImplementedInterfaces();

            // Blobs
            builder.RegisterBlob<UserProfile>(ConfigurationConstants.UsersContainerName, true /* jsonpSupport */)
                .AsImplementedInterfaces();
            builder.RegisterBlob<UserSession>(ConfigurationConstants.UserSessionsContainerName, true /* jsonpSupport */)
                .AsImplementedInterfaces();
            builder.RegisterBlob<Friends>(ConfigurationConstants.FriendsContainerName, true /* jsonpSupport */)
                .AsImplementedInterfaces();
            builder.RegisterBlob<NotificationStatus>(ConfigurationConstants.NotificationsContainerName, true /* jsonpSupport */)
                .AsImplementedInterfaces();
            builder.RegisterBlob<Game>(ConfigurationConstants.GamesContainerName, true /* jsonpSupport */)
                .AsImplementedInterfaces();
            builder.RegisterBlob<GameQueue>(ConfigurationConstants.GamesQueuesContainerName, true /* jsonpSupport */)
                .AsImplementedInterfaces();
            builder.RegisterBlob<SkirmishGameQueueMessage>(ConfigurationConstants.SkirmishGameQueue, true /* jsonpSupport */)
                .AsImplementedInterfaces();
            builder.RegisterBlob<UserProfile>(ConfigurationConstants.GamesContainerName, true /* jsonpSupport */)
                .AsImplementedInterfaces();

            // Repositories
            builder.RegisterType<GameActionNotificationQueue>().AsImplementedInterfaces();
            builder.RegisterType<GameActionStatisticsQueue>().AsImplementedInterfaces();
            builder.RegisterType<GameRepository>().AsImplementedInterfaces();
            builder.RegisterType<IdentityProviderRepository>().AsImplementedInterfaces();
            builder.RegisterType<NotificationRepository>().AsImplementedInterfaces();
            builder.RegisterType<StatisticsRepository>().AsImplementedInterfaces()
                .WithParameter(new NamedParameter("nameOrConnectionString", "StatisticsConnectionString"));
            builder.RegisterType<UserRepository>().AsImplementedInterfaces();

            // Commands
            builder.RegisterType<GameActionCommand>();
            builder.RegisterType<GameActionNotificationCommand>();
            builder.RegisterType<GameActionStatisticsCommand>();
            builder.RegisterType<GameQueueAutoStartCommand>();
            builder.RegisterType<InviteCommand>();
            builder.RegisterType<LeaveGameCommand>();
            builder.RegisterType<SkirmishGameQueueCommand>();

            // Misc
            builder.RegisterType<InMemoryWorkerContext>().AsImplementedInterfaces();
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public override bool OnStart()
        {
            Trace.TraceInformation("Microsoft.Samples.SocialGames.Worker.OnStart");
            ServicePointManager.DefaultConnectionLimit = 12;
            RoleEnvironment.Changing += this.RoleEnvironmentChanging;
            CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSetter) =>
            {
                string configuration = RoleEnvironment.IsAvailable ?
                    RoleEnvironment.GetConfigurationSettingValue(configName) :
                    ConfigurationManager.AppSettings[configName];

                configSetter(configuration);
            });

            if (!IsDevelopmentEnvironment())
            {
                ConfigureDiagnosticMonitor();
            }

            return base.OnStart();
        }

        private static void ConfigureDiagnosticMonitor()
        {
            var storageAccount = CloudStorageAccount.FromConfigurationSetting(wadConnectionString);
            var roleInstanceDiagnosticManager = storageAccount.CreateRoleInstanceDiagnosticManager(RoleEnvironment.DeploymentId, RoleEnvironment.CurrentRoleInstance.Role.Name, RoleEnvironment.CurrentRoleInstance.Id);
            var diagnosticMonitorConfiguration = roleInstanceDiagnosticManager.GetCurrentConfiguration();

            // Performance Counters
            ConfiguredCounters.ToList().ForEach(
                counter =>
                {
                    var counterConfiguration = new PerformanceCounterConfiguration
                    {
                        CounterSpecifier = counter,
                        SampleRate = TimeSpan.FromSeconds(30)
                    };

                    diagnosticMonitorConfiguration.PerformanceCounters.DataSources.Add(counterConfiguration);
                });

            diagnosticMonitorConfiguration.PerformanceCounters.ScheduledTransferPeriod = TimeSpan.FromMinutes(1);

            roleInstanceDiagnosticManager.SetCurrentConfiguration(diagnosticMonitorConfiguration);
        }

        private void RoleEnvironmentChanging(object sender, RoleEnvironmentChangingEventArgs e)
        {
            // If a configuration setting is changing
            if (e.Changes.Any(change => change is RoleEnvironmentConfigurationSettingChange))
            {
                // Set e.Cancel to true to restart this role instance
                e.Cancel = true;
            }
        }
    }
}