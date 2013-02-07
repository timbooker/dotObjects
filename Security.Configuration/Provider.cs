using System.Configuration;
using dotObjects.Core.Configuration;

namespace dotObjects.Security.Configuration
{
    public class Provider : ConfigurationElement
    {
        [ConfigurationProperty("type", IsKey = true, IsRequired = true)]
        public string Type
        {
            get { return (string) base["type"]; }
            set { base["type"] = value; }
        }

        [ConfigurationProperty("settings", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof (SettingsCollection), AddItemName = "add")]
        public SettingsCollection Settings
        {
            get { return (SettingsCollection) base["settings"]; }
        }
    }
}