using System.Drawing;
using System.Web;
using System.Web.SessionState;
using dotObjects.Core.Processing;
using dotObjects.Core.UI;

namespace dotObjects.Web.Handlers
{
    public class ImageHandler : HttpHandlerBase, IRequiresSessionState
    {
        public ImageHandler(ProcessURI requestedURI) : base(requestedURI)
        {
        }

        public override bool IsReusable
        {
            get { return false; }
        }

        public override void ProcessRequest(HttpContext context)
        {
            context.Response.AddHeader("Cache-Control", "no-cache");
            if (RequestedURI.Argument.Values.Count == 2)
            {
                RenderResponse((ViewProcessResponse)ProcessFactory.Execute(RequestedURI, null), context);
            }
            else
                RenderNotFound(context);
        }

        private void RenderResponse(ViewProcessResponse view, HttpContext context)
        {
            if (view != null)
            {
                Domain imageDomain = view.Domain.Domains.Find(GetImageFinderExpression());
                if (imageDomain != null && imageDomain.Value != null)
                {
                    Image image = imageDomain.Value as Image;
                    if (image != null) image.Save(context.Response.OutputStream, image.RawFormat);
                }
            }
            else
                RenderNotFound(context);
        }

        private static void RenderNotFound(HttpContext context)
        {
            context.Response.StatusCode = 404;
        }

        private System.Predicate<Domain> GetImageFinderExpression()
        {
            return d => d.Type.Equals(typeof (Image)) && d.ID.Equals(RequestedURI.Argument.Values[1]);
        }
    }
}