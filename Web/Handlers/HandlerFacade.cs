using System.Web;
using System.Web.SessionState;
using dotObjects.Core.Processing;
using dotObjects.Web.Rendering.Helpers;
using dotObjects.Security.Web.Handlers;
using dotObjects.Security;
using System;

namespace dotObjects.Web.Handlers
{
    public class HandlerFacade : IHttpHandler, IRequiresSessionState
    {
        private string RequestedUrl { get; set; }
        private string PreviousUrl { get; set; }

        public HandlerFacade(string url, string previousUrl)
        {
            RequestedUrl = url;
            PreviousUrl = !string.IsNullOrEmpty(previousUrl) ? previousUrl : string.Empty;
        }

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            GetHandlerBy(UrlHelper.Decode(Uri.UnescapeDataString(RequestedUrl)), UrlHelper.Decode(Uri.UnescapeDataString(PreviousUrl))).ProcessRequest(context);
        }

        private static IHttpHandler GetHandlerBy(string url, string previousUrl)
        {
            var extension = UrlHelper.AllExtensions.Find(url.EndsWith) ?? UrlHelper.DefaultExtension;
            var requestedUri = ProcessURI.Parse(UrlHelper.FromUrl(url, extension));

            if(!string.IsNullOrEmpty(previousUrl))
            {
                if (!url.Equals(previousUrl))
                    requestedUri.PreviousURI = ProcessURI.Parse(UrlHelper.FromUrl(previousUrl, UrlHelper.ExtensionOf(previousUrl)));
                else
                {
                    var type = AppHelper.GetType(requestedUri.Entity);
                    if (type != null)
                        requestedUri.PreviousURI = AppHelper.GetEntitySchema(type).Query.URI;
                }
            }

            SecurityProvider provider = SecurityProvider.GetInstance();
            if (SecurityHandler.IsAuthorized(requestedUri))
            {
                if (requestedUri.Equals(provider.LogoutURI) || requestedUri.Equals(provider.LoginURI))
                    return new SecurityHandler(ProcessURI.Index, requestedUri);

                requestedUri = SecurityHandler.Evaluate(requestedUri);
                if (extension == UrlHelper.PartialViewExtension) return new PartialViewHandler(requestedUri);
                if (extension == UrlHelper.ViewExtension) return new ViewHandler(requestedUri);
                if (extension == UrlHelper.JsonExtension) return new JsonHandler(requestedUri);
                if (extension == UrlHelper.ImageExtension) return new ImageHandler(requestedUri);
                if (extension == UrlHelper.ExcelInteropExtension) return new ExcelInteropHandler(requestedUri);
                if (extension == UrlHelper.PdfExtension) return new PdfHandler(requestedUri);
            }
            if (extension.Equals(UrlHelper.ViewExtension) && !SecurityHandler.IsAuthenticated)
                return new SecurityHandler(requestedUri, provider.LoginURI);
            return new ForbiddenHandler();
        }
    }
}