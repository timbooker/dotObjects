using System.Configuration;

namespace dotObjects.Core.Configuration
{
    [ConfigurationCollection(typeof (ExcludedType))]
    public class ExcludedTypeCollection : ConfigurationElementCollection
    {
        public ExcludedType this[int idx]
        {
            get { return this[idx]; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ExcludedType();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ExcludedType) (element)).TypeName;
        }

        public void Add(ExcludedType element)
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