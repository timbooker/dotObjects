using System.Web;
using System.Web.SessionState;
using dotObjects.Core.Processing;
using dotObjects.Core.UI;
using dotObjects.Security.Configuration.Auth;
using dotObjects.Web.Rendering;
using dotObjects.Web.Rendering.Helpers;

namespace dotObjects.Security.Web.Handlers
{
    public class SecurityHandler : IHttpHandler, IRequiresSessionState
    {
        public SecurityHandler(ProcessURI requestedURI, ProcessURI actionURI)
        {
            RequestedURI = requestedURI;
            ActionURI = actionURI;
        }

        private ProcessURI RequestedURI { get; set; }

        private ProcessURI ActionURI { get; set; }

        public static bool IsAuthenticated
        {
            get { return SecurityProvider.GetInstance().IsAuthenticated; }
        }

        #region IHttpHandler Members

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"></see> interface.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpContext"></see> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests. </param>
        public void ProcessRequest(HttpContext context)
        {
            Domain domain = ViewStateManager.LoadViewState();
            SecurityProvider provider = SecurityProvider.GetInstance();

            IProcessResponse response = ProcessFactory.Execute(ActionURI, domain);

            if (ActionURI.Equals(provider.LoginURI) && provider.IsAuthenticated)
            {
                context.Response.Redirect(!RequestedURI.Equals(provider.LoginURI)
                                              ? UrlHelper.GetViewUrl(RequestedURI)
                                              : UrlHelper.GetViewUrl(new ProcessURI {Behavior = ProcessBehavior.Index}));
            }
            else if (ActionURI.Equals(provider.LogoutURI) && !provider.IsAuthenticated)
                context.Response.Redirect(UrlHelper.GetViewUrl(RequestedURI));
            else
            {
                Renderer renderer = RendererFactory.GetRenderer(provider.LoginURI, response);
                renderer.Process(response, context.Response.Output);
            }
        }

        /// <summary>
        /// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler"></see> instance.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Web.IHttpHandler"></see> instance is reusable; otherwise, false.
        /// </returns>
        public bool IsReusable
        {
            get { return false; }
        }

        #endregion

        public static bool IsAuthorized(ProcessURI uri)
        {
            return AuthorizationManager.IsAuthorized(uri);
        }

        public static ProcessURI Evaluate(ProcessURI uri)
        {
            return AuthorizationManager.Evaluate(uri);
        }

        public static IProcessResponse Evaluate(ProcessURI uri, IProcessResponse response)
        {
            return AuthorizationManager.Evaluate(uri, response);
        }
    }
}