namespace Microsoft.Samples.SocialGames.Web
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Autofac;
    using Autofac.Integration.Mvc;
    using Microsoft.ApplicationServer.Http;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.IdentityModel.Web;
    using Microsoft.IdentityModel.Web.Configuration;
    using Microsoft.Samples.SocialGames.Common.Storage;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.Samples.SocialGames.Extensions;
    using Microsoft.Samples.SocialGames.Repositories;
    using Microsoft.Samples.SocialGames.Web.Services;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.ServiceRuntime;

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapServiceRoute<GameService>("game");
            routes.MapServiceRoute<AuthService>("auth");
            routes.MapServiceRoute<UserService>("user");
            routes.MapServiceRoute<EventService>("event");

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }

        protected void Application_Start()
        {
            CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSetter) =>
            {
                string configuration = RoleEnvironment.IsAvailable ?
                    RoleEnvironment.GetConfigurationSettingValue(configName) :
                    ConfigurationManager.AppSettings[configName];

                configSetter(configuration);
            });

            // Setup AutoFac
            var builder = new ContainerBuilder();
            DependancySetup(builder);
            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            // Setup WCF Web API Config
            var config = new WebApiConfiguration();
            config.EnableTestClient = true;
            config.CreateInstance = ((t, i, h) => DependencyResolver.Current.GetService(t));
            //config.MessageHandlerFactory = () => DependencyResolver.Current.GetService<DelegatingHandler[]>();
            RouteTable.Routes.SetDefaultHttpConfiguration(config);

            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            FederatedAuthentication.ServiceConfigurationCreated += this.OnServiceConfigurationCreated;

            // Call Initializers
            var initializers = DependencyResolver.Current.GetServices<IStorageInitializer>();
            foreach (var initializer in initializers)
            {
                initializer.Initialize();
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
            builder.RegisterBlob<UserProfile>(ConfigurationConstants.GamesContainerName, true /* jsonpSupport */)
                .AsImplementedInterfaces();

            // Repositories
            builder.RegisterType<GameActionNotificationQueue>().AsImplementedInterfaces();
            builder.RegisterType<GameActionStatisticsQueue>().AsImplementedInterfaces();
            builder.RegisterType<GameRepository>().AsImplementedInterfaces();
            builder.RegisterType<IdentityProviderRepository>().AsImplementedInterfaces();
            builder.RegisterType<NotificationRepository>().AsImplementedInterfaces();
            builder.RegisterType<StatisticsRepository>().AsImplementedInterfaces().InstancePerHttpRequest()
                .WithParameter(new NamedParameter("nameOrConnectionString", "StatisticsConnectionString"));
            builder.RegisterType<UserRepository>().AsImplementedInterfaces();

            // Controllers
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            // Services
            builder.RegisterType<AuthService>().AsImplementedInterfaces().AsSelf();
            builder.RegisterType<EventService>().AsImplementedInterfaces().AsSelf();
            builder.RegisterType<GameService>().AsImplementedInterfaces().AsSelf();
            builder.RegisterType<HttpContextUserProvider>().AsImplementedInterfaces().AsSelf();
            builder.RegisterType<UserService>().AsImplementedInterfaces().AsSelf();
        }

        private void OnServiceConfigurationCreated(object sender, ServiceConfigurationCreatedEventArgs e)
        {
            var sessionTransforms = new List<CookieTransform>(new CookieTransform[] { new DeflateCookieTransform() });
            var sessionHandler = new SessionSecurityTokenHandler(sessionTransforms.AsReadOnly());

            e.ServiceConfiguration.SecurityTokenHandlers.AddOrReplace(sessionHandler);
        }
    }
}