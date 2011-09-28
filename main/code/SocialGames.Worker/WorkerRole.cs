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
    using Microsoft.Samples.SocialGames;
    using Microsoft.Samples.SocialGames.Common.JobEngine;
    using Microsoft.Samples.SocialGames.Repositories;
    using Microsoft.Samples.SocialGames.Worker.Commands;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Diagnostics;
    using Microsoft.WindowsAzure.Diagnostics.Management;
    using Microsoft.WindowsAzure.ServiceRuntime;

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
            var account = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");
            var userRepository = new UserRepository(account);
            var gameRepository = new GameRepository(account);
            var workerContext = new InMemoryWorkerContext();

            // TaskBuilder callback for logging errors
            Action<ICommand, IDictionary<string, object>, Exception> logException = (cmd, context, ex) =>
            {
                Trace.TraceError(ex.ToString());
            };

            // Game Queue for Skirmish game
            Task.TriggeredBy(Message.OfType<SkirmishGameQueueMessage>())
                .SetupContext((message, context) =>
                {
                    context.Add("userId", message.UserId);
                })
                .Do(
                    new SkirmishGameQueueCommand(userRepository, gameRepository, workerContext))
                .OnError(logException)
                .Start();

            // Leave game messages
            Task.TriggeredBy(Message.OfType<LeaveGameMessage>())
                .SetupContext((message, context) =>
                {
                    context.Add("userId", message.UserId);
                    context.Add("gameId", message.GameId);
                })
                .Do(
                    new LeaveGameCommand(userRepository, gameRepository))
                .OnError(logException)
                .Start();

            // Game Action for Notification messages
            Task.TriggeredBy(Message.OfType<GameActionMessage>(CloudStorageAccount.FromConfigurationSetting("DataConnectionString"), ConfigurationConstants.GameActionNotificationsQueue))
                .SetupContext((message, context) =>
                {
                    context.Add("gameAction", message.GameAction);
                })
                .Do(
                    new GameActionNotificationCommand())
                .OnError(logException)
                .Start();

            // Game Action for Statistics messages
            Task.TriggeredBy(Message.OfType<GameActionMessage>(CloudStorageAccount.FromConfigurationSetting("DataConnectionString"), ConfigurationConstants.GameActionStatisticsQueue))
                .SetupContext((message, context) =>
                {
                    context.Add("gameAction", message.GameAction);
                })
                .Do(
                    new GameActionStatisticsCommand(new StatisticsRepository("StatisticsConnectionString")))
                .OnError(logException)
                .Start();

            // Process for Invite messages
            Task.TriggeredBy(Message.OfType<InviteMessage>(CloudStorageAccount.FromConfigurationSetting("DataConnectionString"), ConfigurationConstants.InvitesQueue))
                .SetupContext((message, context) =>
                {
                    context.Add("userId", message.UserId);
                    context.Add("invitedUserId", message.InvitedUserId);
                    context.Add("gameQueueId", message.GameQueueId);
                    context.Add("timestamp", message.Timestamp);
                    context.Add("message", message.Message);
                    context.Add("url", message.Url);
                })
                .Do(
                    new InviteCommand())
                .OnError(logException)
                .Start();

            // Timeout for game queue
            Task.TriggeredBy(Schedule.Every(5 * 1000))
                .Do(
                    new GameQueueAutoStartCommand(gameRepository, workerContext))
                .OnError(logException)
                .Start();

            while (true)
            {
                Thread.Sleep(1 * 1000);
            }
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