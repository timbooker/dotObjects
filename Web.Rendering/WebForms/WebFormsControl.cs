using System.Web.UI;
using dotObjects.Core.Processing;
using dotObjects.Core.UI;

namespace dotObjects.Web.Rendering.WebForms
{
    public class WebFormsControl : UserControl
    {
        public ProcessURI URI { get; set; }

        public new IProcessResponse Response { get; set; }

        public Domain Domain { get; set; }

        public WebFormsRenderer Renderer { get; set; }

        public T GetResponseAs<T>()
        {
            return (T) Response;
        }
    }
}