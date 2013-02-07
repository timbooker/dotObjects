using System;
using System.IO;
using System.Reflection;
using System.Web;
using dotObjects.Core.Processing;
using dotObjects.Web.Rendering.Configuration;
using dotObjects.Web.Rendering.Helpers;

namespace dotObjects.Web.Rendering
{
    public static class RendererFactory
    {
        private static Renderer renderer;

        public static Renderer GetRenderer(ProcessURI uri, IProcessResponse response)
        {
            RenderingSection config = RenderingSection.Current;
            if (renderer == null)
            {
                Type rendererType = Type.GetType(config.Type);
                Assembly asm = rendererType.Assembly;
                renderer = asm.CreateInstance(rendererType.FullName) as Renderer;
                renderer.Path = ParseRendererPath(config);

                //foreach (CustomHelper helper in config.CustomHelpers)
                //    renderer.Helpers.Add(CreateHelperInstance(helper));

                renderer.UrlHelper = new UrlHelper("url");
                renderer.AppHelper = new AppHelper("app");
                renderer.StateHelper = new StateHelper("state");
                renderer.AjaxHelper = new AjaxHelper("ajax");
                renderer.TypeHelper = new TypeHelper("type");
            }

            renderer.URI = uri;
            
            Configuration.Template template = renderer.GetTemplate(response);
            renderer.LayoutFileName = template != null
                                          ? ((string.IsNullOrEmpty(template.LayoutFileName))
                                                 ? config.LayoutFileName
                                                 : template.LayoutFileName)
                                          : config.LayoutFileName;

            return renderer;
        }


        private static string ParseRendererPath(RenderingSection config)
        {
            HttpRequest request = HttpContext.Current.Request;
            return Path.Combine(request.MapPath(request.ApplicationPath), config.Path);
        }
    }
}