using System.Configuration;

namespace dotObjects.Web.Rendering.Configuration
{
    public class RenderingSection : ConfigurationSection
    {
        public const string SectionName = "dotObjects/rendering";

        private static RenderingSection current;

        public static RenderingSection Current
        {
            get { return current ?? (current = (RenderingSection) ConfigurationManager.GetSection(SectionName)); }
        }

        /// <summary>
        /// Gets the <see cref="Renderer"/> type.
        /// </summary>
        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get { return (string) base["type"]; }
        }

        /// <summary>
        /// Gets the relative path (without ~/) of templates folder.
        /// </summary>
        [ConfigurationProperty("path", IsRequired = true)]
        public string Path
        {
            get { return (string) base["path"]; }
        }

        /// <summary>
        /// Gets the label of layout's template.
        /// </summary>
        [ConfigurationProperty("layoutFileName", IsRequired = false)]
        public string LayoutFileName
        {
            get { return (string) base["layoutFileName"]; }
        }

        [ConfigurationProperty("templates", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof (TemplateCollection), AddItemName = "template")]
        public TemplateCollection Templates
        {
            get { return ((TemplateCollection) (base["templates"])); }
        }

        [ConfigurationProperty("customHelpers", IsDefaultCollection = false, IsRequired = false)]
        [ConfigurationCollection(typeof (CustomHelperCollection), AddItemName = "add")]
        public CustomHelperCollection CustomHelpers
        {
            get { return ((CustomHelperCollection) (base["customHelpers"])); }
        }
    }
}