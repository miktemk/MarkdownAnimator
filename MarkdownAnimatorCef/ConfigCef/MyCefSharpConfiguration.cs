using System;
using System.IO;
using CefSharp;

namespace MarkdownAnimatorCef.ConfigCef
{
    public class MyCefSharpConfiguration
    {
        public static void Configure()
        {
            var settings = new CefSettings()
            {
                //By default CefSharp will use an in-memory cache, you need to specify a Cache Folder to persist data
                CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache"),
                LogFile = "logggg.txt",
                RemoteDebuggingPort = 8088,

            };

            settings.RegisterScheme(new CefCustomScheme()
            {
                SchemeName = "local",
                DomainName = "jasar",
                SchemeHandlerFactory = new SchemeHandlerFactory_FromResource("ResourcesHtml"),
            });

            //Perform dependency check to make sure all relevant resources are in our output directory.
            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
        }
    }
}
