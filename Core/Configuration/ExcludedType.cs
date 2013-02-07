using System.Configuration;

namespace dotObjects.Core.Configuration
{
    public class ExcludedType : ConfigurationElement
    {
        [ConfigurationProperty("typeName", IsKey = true, IsRequired = true)]
        public string TypeName
        {
            get { return (string) this["typeName"]; }
        }
    }
}