using System.Configuration;

namespace dotObjects.Core.Configuration
{
    [ConfigurationCollection(typeof (EntityAssembly))]
    public class EntityAssembliesCollection : ConfigurationElementCollection
    {
        public new EntityAssembly this[string name]
        {
            get
            {
                foreach (EntityAssembly asm in this)
                    if (asm.Name.Equals(name))
                        return asm;
                return null;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new EntityAssembly();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((EntityAssembly) (element)).Name;
        }

        public void Add(EntityAssembly element)
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