using System.Configuration;

namespace dotObjects.Core.Configuration
{
    [ConfigurationCollection(typeof (Context))]
    public class ContextCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Gets or sets the default context.
        /// </summary>
        [ConfigurationProperty("default", IsRequired = true)]
        public string Default
        {
            get { return (string) base["default"]; }
            set { base["default"] = value; }
        }

        public Context this[int idx]
        {
            get { return this[idx]; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new Context();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((Context) (element)).Name;
        }

        public void Add(Context element)
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

        public Context Find(string name)
        {
            foreach (Context ctx in this)
            {
                if (ctx.Name.Equals(name))
                    return ctx;
            }
            return null;
        }
    }
}