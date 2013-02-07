using System.Web;
using System.Web.SessionState;
using dotObjects.Core.Processing;
using dotObjects.Core.UI;
using dotObjects.Web.Rendering;
using dotObjects.Web.Serialization;
using dotObjects.Web.Rendering.Helpers;

namespace dotObjects.Web.Handlers
{
    /// <summary>
    /// Class responsible for handling process requests and render the json output.
    /// </summary>
    public class JsonHandler : HttpHandlerBase, IRequiresSessionState
    {
        public JsonHandler(ProcessURI requestedURI) : base(requestedURI)
        {
        }

        public override bool IsReusable
        {
            get { return false; }
        }

        public override void ProcessRequest(HttpContext context)
        {
            Domain domain = ViewStateManager.LoadViewState();
            Process.CurrentState = ViewStateManager.LoadProcessState();

            IProcessResponse response = ProcessFactory.Execute(RequestedURI, domain);

            if (response is RedirectProcessResponse)
            {
                context.Response.Redirect(UrlHelper.GetJsonUrl(((RedirectProcessResponse) response).URI));
            }
            else
            {
                context.Response.AddHeader("Cache-Control", "no-cache");

                //JavaScriptSerializer serializer = new JavaScriptSerializer();
                //context.Response.Write(serializer.Serialize(response));
                JsonSerializer.SerializeObject(response, context.Response.Output);
            }
        }
    }
}