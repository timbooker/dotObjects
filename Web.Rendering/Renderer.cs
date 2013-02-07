using System.Collections.Generic;
using System.IO;
using dotObjects.Core.Processing;
using dotObjects.Core.UI;
using dotObjects.Web.Rendering.Configuration;
using dotObjects.Web.Rendering.Helpers;
using System;

namespace dotObjects.Web.Rendering
{
    public abstract class Renderer
    {
        [NonSerialized]
        private static Dictionary<IProcessResponse, Template> templates;

        private List<RendererHelper> helpers;
        
        public string Path { get; set; }

        public string LayoutFileName { get; set; }

        public UrlHelper UrlHelper { get; set; }

        public AppHelper AppHelper { get; set; }

        public StateHelper StateHelper { get; set; }

        public AjaxHelper AjaxHelper { get; set; }

        public TypeHelper TypeHelper { get; set; }

        public ProcessURI URI { get; set; }

        public List<RendererHelper> Helpers
        {
            get { return helpers ?? (helpers = new List<RendererHelper>()); }
        }

        public static Dictionary<IProcessResponse, Template> Templates
        {
            get
            {
                var moniker = new object();
                lock (moniker)
                {
                    if (templates == null)
                        templates = new Dictionary<IProcessResponse, Template>();
                }
                return templates;
            }
        }

        public Template GetTemplate(IProcessResponse response)
        {
            var config = RenderingSection.Current;

            if (!Templates.ContainsKey(response))
            {
                var template = config.Templates.Find(string.Concat(response.URI, UrlHelper.CurrentExtension), UrlHelper.CurrentExtension)
                                ?? config.Templates.Find(response.URI)
                                ?? config.Templates.Find(response);

                Templates.Add(response, template);
            }

            return Templates[response];
        }

        protected string GetTemplateName(IProcessResponse response, Domain domain)
        {
            return GetTemplate(response).TemplateDomains.Find(domain).FileName;
        }

        protected string GetTemplateName(ProcessURI uri, Domain domain)
        {
            Template template = RenderingSection.Current.Templates.Find(uri);
            if (template != null)
                return template.TemplateDomains.Find(domain).FileName;
            return string.Empty;
        }

        public virtual void Process(IProcessResponse response, TextWriter writer)
        {
            if (!string.IsNullOrEmpty(LayoutFileName))
                ProcessTemplate(writer, LayoutFileName, response, null);
            else
            {
                Template template = GetTemplate(response);
                if (template != null)
                    ProcessTemplate(writer, template.FileName, response, null);
            }
        }

        public void Process(IProcessResponse response, Domain domain, TextWriter writer)
        {
            ProcessTemplate(writer, GetTemplateName(response, domain), response, domain);
        }

        public abstract void ProcessTemplate(TextWriter writer, string templateName, IProcessResponse response, Domain domain);
    }
}