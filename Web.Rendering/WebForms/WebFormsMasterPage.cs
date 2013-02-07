using System.Collections.Generic;
using System.Web.UI;
using dotObjects.Core.Processing;
using dotObjects.Core.UI;

namespace dotObjects.Web.Rendering.WebForms
{
    public class WebFormsMasterPage : MasterPage
    {
        private List<RendererHelper> helpers;

        public ProcessURI URI { get; set; }

        public new IProcessResponse Response { get; set; }

        public Domain Domain { get; set; }

        public WebFormsRenderer Renderer { get; set; }

        public List<RendererHelper> Helpers
        {
            get { return helpers ?? (helpers = new List<RendererHelper>()); }
        }
    }
}