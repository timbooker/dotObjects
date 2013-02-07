namespace dotObjects.Web.Rendering
{
    public abstract class RendererHelper
    {
        public virtual string Name { get; set; }

        protected RendererHelper(string name)
        {
            Name = name;
        }
    }
}