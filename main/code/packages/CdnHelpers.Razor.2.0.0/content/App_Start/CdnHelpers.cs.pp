using System;
using CdnHelpers;
using System.Reflection;

[assembly: WebActivator.PostApplicationStartMethod(typeof($rootnamespace$.App_Start.CdnHelpers), "Start")]

namespace $rootnamespace$.App_Start
{
	public static class CdnHelpers
	{
		public static void Start() {

			CdnHelpersContext.Current.Configure(c => {
				c.CdnEndointUrl = "[namespace].vo.msecnd.net";
				//c.EnableBlobStorageBacking(CloudStorageAccount.DevelopmentStorageAccount);
				c.EnableImageOptimizations();
				c.UseCdnForContentFolder();
				c.UseCdnForScriptsFolder();
                c.HashKey = Assembly.GetExecutingAssembly().GetName().Version.ToString();
				c.DebuggingEnabled = () => { return System.Web.HttpContext.Current.Request.IsLocal; };
			});

			CdnHelpersContext.Current.RegisterCombinedJsFiles("core",
				"~/scripts/modernizr-1.7.js"
			);

			CdnHelpersContext.Current.RegisterCombinedCssFiles("site",
				"~/content/Site.css"
			);
		
		}

	}
}