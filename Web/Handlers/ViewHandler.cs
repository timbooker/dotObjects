using System.Text;
using System.Web;
using System.Web.SessionState;
using dotObjects.Core.Processing;
using dotObjects.Core.UI;
using dotObjects.Logging;
using dotObjects.Web.Rendering;
using dotObjects.Web.Rendering.Helpers;
using dotObjects.Security.Web.Handlers;

namespace dotObjects.Web.Handlers
{
    public class ViewHandler : HttpHandlerBase, IRequiresSessionState
    {
        public ViewHandler(ProcessURI requestedUri)
            : base(requestedUri)
        {
        }

        public override bool IsReusable
        {
            get { return false; }
        }

        public override void ProcessRequest(HttpContext context)
        {
            Renderer.Templates.Clear();

            var sw = System.Diagnostics.Stopwatch.StartNew();
            LogManager.Logger.Write(LogType.Info, "Starting Request: {0}", RequestedURI);

            LogManager.Logger.Write(LogType.Info, "Loading View State");
            
            Domain domain = ViewStateManager.LoadViewState();
            
            StringBuilder sb = new StringBuilder();
            //JsonSerializer.SerializeObject(domain, new StringWriter(sb));

            LogManager.Logger.Write(LogType.Info, "View State loaded: {0}", sb.ToString());

            LogManager.Logger.Write(LogType.Info, "Loading Process State");
            Process.CurrentState = ViewStateManager.LoadProcessState();
            LogManager.Logger.Write(LogType.Info, "Process State loaded");

            IProcessResponse response = ProcessFactory.Execute(RequestedURI, domain);
            LogManager.Logger.Write(LogType.Info, "Starting evaluating process response: {0}", response);
            
            if (response is RedirectProcessResponse)
            {
                var uri = ((RedirectProcessResponse) response).URI;
                var extension = uri.Extension ?? UrlHelper.ViewExtension;
                context.Response.Redirect(UrlHelper.GetUrl(uri, extension));
            }
            else
            {
                response = SecurityHandler.Evaluate(RequestedURI, response);
                Renderer renderer = RendererFactory.GetRenderer(RequestedURI, response);
                renderer.Process(response, context.Response.Output);
            }
            LogManager.Logger.Write(LogType.Info, "Finished evaluating process response: {0}", response);

            sw.Stop();
            LogManager.Logger.Write(LogType.Info, "Finished Request: {0}, Elapsed Time (ms): {1}", RequestedURI, sw.ElapsedMilliseconds);
        }
    }
}