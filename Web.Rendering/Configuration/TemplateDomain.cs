using System.Configuration;
using System.Collections.Specialized;
using System.ComponentModel;

namespace dotObjects.Web.Rendering.Configuration
{
    public class TemplateDomain : ConfigurationElement
    {
        public const string CollectionKey = "dotObjects.Web.Rendering.EntityList";
        public const string SingleKey = "dotObjects.Web.Rendering.Entity";

        [ConfigurationProperty("typeName", IsKey = false, IsRequired = false)]
        public string TypeName
        {
            get { return (string) base["typeName"]; }
        }

        [TypeConverter(typeof (CommaDelimitedStringCollectionConverter))]
        [ConfigurationProperty("attributes", IsKey = false, IsRequired = false)]
        public StringCollection Attributes
        {
            get
            {
                return (CommaDelimitedStringCollection) base["attributes"]
                       ?? new CommaDelimitedStringCollection();
            }
        }

        [ConfigurationProperty("fileName", IsKey = false, IsRequired = true)]
        public string FileName
        {
            get { return (string) base["fileName"]; }
        }
    }
}