using System;
using dotObjects.Core.Processing;
using System.IO;
using dotObjects.Core.UI;
using System.Web.UI;
using System.Web;
using dotObjects.Web.Rendering.Configuration;

namespace dotObjects.Web.Rendering.WebForms
{
    public class WebFormsRenderer : Renderer
    {
        private static Page container = new Page();

        public override void ProcessTemplate(TextWriter writer, string templateName, IProcessResponse response,
                                             Domain domain)
        {
            if (!string.IsNullOrEmpty(templateName))
            {
                Page page = new Page();
                page.Controls.Add(LoadControl(templateName, page, response, domain));
                page.RenderControl(new HtmlTextWriter(writer));
            }
        }

        protected Control LoadControl(string templateName, Page page, IProcessResponse response, Domain domain)
        {
            string templatePath = string.Format("~/{0}/{1}", RenderingSection.Current.Path, templateName);
            WebFormsControl control = null;

            try
            {
                control = page.LoadControl(templatePath) as WebFormsControl;
            }
            catch (Exception ex)
            {
#if DEBUG
                throw ex;
#endif
            }

            if (control != null)
            {
                control.URI = URI;
                control.Response = response;
                control.Domain = domain;
                control.Renderer = this;
            }
            return control;
        }

        public void LoadTemplate(IProcessResponse response, Domain domain, Control container)
        {
            Configuration.Template template = GetTemplate(response);
            if (template != null)
            {
                var control = LoadControl(template.FileName, container.Page, response, domain);
                if (control != null)
                    container.Controls.Add(control);
            }
        }

        public void LoadDomainTemplate(IProcessResponse response, Domain domain, Control container)
        {
            string templateName = GetTemplateName(response, domain);
            if (!string.IsNullOrEmpty(templateName))
            {
                var control = LoadControl(templateName, container.Page, response, domain);
                if (control != null)
                    container.Controls.Add(control);
            }
        }
    }
}