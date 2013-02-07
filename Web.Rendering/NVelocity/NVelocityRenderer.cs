using System.IO;
using Commons.Collections;
using dotObjects.Core.Processing;
using dotObjects.Core.UI;
using NVelocity;
using NVelocity.App;
using NVelocity.Runtime;

namespace dotObjects.Web.Rendering.NVelocity
{
    public class NVelocityRenderer : Renderer
    {
        private VelocityEngine engine;

        private VelocityEngine Engine
        {
            get
            {
                if (engine == null)
                {
                    ExtendedProperties properties = new ExtendedProperties();
                    properties.SetProperty(RuntimeConstants.FILE_RESOURCE_LOADER_PATH, Path);
                    properties.SetProperty(RuntimeConstants.FILE_RESOURCE_LOADER_CACHE, true);

                    engine = new VelocityEngine(properties);
                    engine.Init();
                }
                return engine;
            }
        }

        public override void ProcessTemplate(TextWriter writer, string templateName, IProcessResponse response,
                                             Domain domain)
        {
            if (!string.IsNullOrEmpty(templateName))
            {
                Template template = Engine.GetTemplate(templateName);
                template.Merge(CreateTemplateContext(URI, response, domain), writer);
            }
        }

        public string ProcessResponse(IProcessResponse response)
        {
            StringWriter writer = new StringWriter();
            Configuration.Template template = GetTemplate(response);
            if (template != null)
                ProcessTemplate(writer, template.FileName, response, null);
            return writer.ToString();
        }

        public string ProcessDomain(IProcessResponse response, Domain domain)
        {
            StringWriter writer = new StringWriter();
            ProcessTemplate(writer, GetTemplateName(response, domain), response, domain);
            return writer.ToString();
        }

        public string ProcessDomain(ProcessURI uri, Domain domain)
        {
            StringWriter writer = new StringWriter();
            ProcessTemplate(writer, GetTemplateName(uri, domain), null, domain);
            return writer.ToString();
        }

        private VelocityContext CreateTemplateContext(ProcessURI uri, IProcessResponse response, Domain domain)
        {
            VelocityContext context = new VelocityContext();
            context.Put("uri", uri);
            context.Put("renderer", this);

            if (response != null) context.Put("response", response);
            if (domain != null) context.Put("domain", domain);

            foreach (RendererHelper helper in Helpers)
                context.Put(helper.Name, helper);

            return context;
        }
    }
}