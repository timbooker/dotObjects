using System.Web;
using dotObjects.Core.Processing;
using dotObjects.Security.Web.Handlers;

namespace dotObjects.Web.Handlers
{
    public abstract class HttpHandlerBase : IHttpHandler
    {
        private readonly ProcessURI requestedURI;

        protected HttpHandlerBase(ProcessURI requestedURI)
        {
            this.requestedURI = requestedURI;
        }

        public ProcessURI RequestedURI
        {
            get { return requestedURI; }
        }

        #region IHttpHandler Members

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"></see> interface.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpContext"></see> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests. </param>
        public abstract void ProcessRequest(HttpContext context);

        /// <summary>
        /// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler"></see> instance.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Web.IHttpHandler"></see> instance is reusable; otherwise, false.
        /// </returns>
        public abstract bool IsReusable { get; }

        #endregion
    }
}