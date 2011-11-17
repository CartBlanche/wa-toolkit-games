namespace Microsoft.Samples.SocialGames.Web
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Microsoft.ApplicationServer.Http.Activation;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.IdentityModel.Web;
    using Microsoft.IdentityModel.Web.Configuration;
    using Microsoft.Samples.SocialGames.GamePlay.Services;
    using Microsoft.Samples.SocialGames.Repositories;
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

            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            FederatedAuthentication.ServiceConfigurationCreated += this.OnServiceConfigurationCreated;

            // initialize blob and queue resources
            new GameRepository().EnsureExist();
            new UserRepository().EnsureExist();
        }

        private void OnServiceConfigurationCreated(object sender, ServiceConfigurationCreatedEventArgs e)
        {
            var sessionTransforms = new List<CookieTransform>(new CookieTransform[] { new DeflateCookieTransform() });
            var sessionHandler = new SessionSecurityTokenHandler(sessionTransforms.AsReadOnly());

            e.ServiceConfiguration.SecurityTokenHandlers.AddOrReplace(sessionHandler);
        }
    }
}