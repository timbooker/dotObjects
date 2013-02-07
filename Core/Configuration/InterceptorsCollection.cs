using System.Configuration;

namespace dotObjects.Core.Configuration
{
    [ConfigurationCollection(typeof (Interceptor))]
    public class InterceptorsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new Interceptor();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((Interceptor) (element)).TypeName;
        }

        public void Add(Interceptor element)
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