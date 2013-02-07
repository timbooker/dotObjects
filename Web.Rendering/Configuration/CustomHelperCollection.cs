using System.Configuration;

namespace dotObjects.Web.Rendering.Configuration
{
    [ConfigurationCollection(typeof (CustomHelper))]
    public class CustomHelperCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new CustomHelper();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CustomHelper) (element)).Name;
        }

        public CustomHelper Find(string name)
        {
            foreach (CustomHelper helper in this)
            {
                if (helper.Name.Equals(name))
                    return helper;
            }
            return null;
        }
    }
}