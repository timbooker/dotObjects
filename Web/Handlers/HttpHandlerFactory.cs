using System;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.SessionState;
using System.Configuration;
using System.Web.Configuration;
using dotObjects.Session;
using dotObjects.Web.Rendering.Helpers;

namespace dotObjects.Web.Handlers
{
    public class HttpHandlerFactory : IHttpHandlerFactory, IRequiresSessionState
    {
        private const string PreviousUrlKey = "previousUrl";

        #region IHttpHandlerFactory Members

        /// <summary>
        /// Returns an instance of a class that implements the <see cref="T:System.Web.IHttpHandler"></see> interface.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:System.Web.IHttpHandler"></see> object that processes the request.
        /// </returns>
        /// <param name="pathTranslated">The <see cref="P:System.Web.HttpRequest.PhysicalApplicationPath"></see> to the requested resource. </param>
        /// <param name="url">The <see cref="P:System.Web.HttpRequest.RawUrl"></see> of the requested resource. </param>
        /// <param name="context">An instance of the <see cref="T:System.Web.HttpContext"></see> class that provides references to intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests. </param>
        /// <param name="requestType">The HTTP data transfer method (GET or POST) that the client uses. </param>
        public IHttpHandler GetHandler(HttpContext context, string requestType, string url, string pathTranslated)
        {
            SetupUserCulture();
            
            var previousUrl = context.Request.Params[UrlHelper.PreviousURLKey] != null
                                  ? context.Request.Params[UrlHelper.PreviousURLKey]
                                  : context.Request.UrlReferrer == null
                                              ? null
                                              : context.Request.UrlReferrer.AbsoluteUri;

            return new HandlerFacade(context.Request.Url.AbsoluteUri, previousUrl);
        }

        /// <summary>
        /// Enables a factory to reuse an existing handler instance.
        /// </summary>
        /// <param name="handler">The <see cref="T:System.Web.IHttpHandler"></see> object to reuse. </param>
        public void ReleaseHandler(IHttpHandler handler)
        {
            GC.ReRegisterForFinalize(handler);
        }

        #endregion

        internal static void SetupUserCulture()
        {
            GlobalizationSection globalization =
                ConfigurationManager.GetSection("system.web/globalization") as GlobalizationSection;

            CultureInfo current = (globalization != null && globalization.EnableClientBasedCulture)
                                      ? new CultureInfo(HttpContext.Current.Request.UserLanguages[0])
                                      : new CultureInfo(globalization.Culture);

            Thread.CurrentThread.CurrentCulture = current;
            Thread.CurrentThread.CurrentUICulture = current;
        }
    }
}