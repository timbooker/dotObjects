using System.Configuration;

namespace dotObjects.Core.Configuration
{
    /// <summary>
    /// Represents a base setting configuration elements collection.
    /// </summary>
    [ConfigurationCollection(typeof (Setting))]
    public class SettingsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new Setting();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((Setting) (element)).Name;
        }

        public void Add(Setting element)
        {
            BaseAdd(element);
        }

        public void Remove(string key)
        {
            BaseRemove(key);
        }

        public void Clear()
        {
            BaseClear();
        }
    }
}