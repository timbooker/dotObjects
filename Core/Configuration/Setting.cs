using System.Configuration;

namespace dotObjects.Core.Configuration
{
    /// <summary>
    /// Represents a base setting element.
    /// </summary>
    public class Setting : ConfigurationElement
    {
        public Setting()
        {
        }

        public Setting(string name, string value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Gets or sets the setting's name.
        /// </summary>
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string) this["name"]; }
            set { this["name"] = value; }
        }

        /// <summary>
        /// Gets or sets the setting's value.
        /// </summary>
        [ConfigurationProperty("value", IsKey = false, IsRequired = true)]
        public string Value
        {
            get { return (string) this["value"]; }
            set { this["value"] = value; }
        }
    }
}