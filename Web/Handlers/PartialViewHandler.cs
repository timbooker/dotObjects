using System.Web;
using System.Web.Script.Serialization;
using dotObjects.Core.Processing;
using dotObjects.Core.UI;
using dotObjects.Web.Rendering;
using dotObjects.Web.Rendering.Configuration;
using dotObjects.Web.Serialization.UI;

namespace dotObjects.Web.Handlers
{
    public class PartialViewHandler : ViewHandler
    {
        public const string PartialViewKey = "pvd";

        public PartialViewHandler(ProcessURI requestedURI)
            : base(requestedURI)
        {
        }

        public override void ProcessRequest(HttpContext context)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] {new DomainJavaScriptConverter()});
            Domain domain = serializer.Deserialize<Domain>(context.Request[PartialViewKey]);

            Template template = RenderingSection.Current.Templates.Find(RequestedURI);
            TemplateDomain templateDomain = template.TemplateDomains.Find(domain);

            Renderer renderer = RendererFactory.GetRenderer(RequestedURI, null);
            renderer.ProcessTemplate(context.Response.Output, templateDomain.FileName, null, domain);
        }
    }
}