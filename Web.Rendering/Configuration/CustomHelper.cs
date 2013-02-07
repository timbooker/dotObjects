using System.Configuration;

namespace dotObjects.Web.Rendering.Configuration
{
    public class CustomHelper : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string) base["name"]; }
        }

        [ConfigurationProperty("typeName", IsKey = false, IsRequired = true)]
        public string TypeName
        {
            get { return (string) base["typeName"]; }
        }
    }
}