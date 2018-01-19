using CefSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace MarkdownAnimatorCef.ConfigCef
{
    // LESSON: https://github.com/cefsharp/CefSharp/wiki/General-Usage#scheme-handler
    // TODO: read the whole doc: https://github.com/cefsharp/CefSharp/wiki/General-Usage
    public class SchemeHandlerFactory_FromResource : ISchemeHandlerFactory
    {
        private string resourcePrefix;

        public SchemeHandlerFactory_FromResource(string resourcePrefix)
        {
            this.resourcePrefix = resourcePrefix;
        }

        public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request)
        {
            //Notes:
            // - The 'host' portion is entirely ignored by this scheme handler.
            // - If you register a ISchemeHandlerFactory for http/https schemes you should also specify a domain name
            // - Avoid doing lots of processing in this method as it will affect performance.
            // - Uses the Default ResourceHandler implementation

            var uri = new Uri(request.Url);
            var resourceFileName = uri.AbsolutePath;
            var scriptStream = Application.GetResourceStream(new Uri(resourcePrefix + resourceFileName, UriKind.Relative));
            if (scriptStream != null)
            {
                var mimeType = "text/html";
                var fileExtension = Path.GetExtension(resourceFileName);
                if (fileExtension == ".css")
                    mimeType = "text/css";
                return ResourceHandler.FromStream(scriptStream.Stream, mimeType);
            }

            return null;
        }

    }
}
