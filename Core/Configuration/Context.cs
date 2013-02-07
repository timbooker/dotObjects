using System.Configuration;

namespace dotObjects.Core.Configuration
{
    /// <summary>
    /// Represents a context configuration element.
    /// </summary>
    public class Context : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets the context name.
        /// </summary>
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string) base["name"]; }
            set { base["name"] = value; }
        }

        /// <summary>
        /// Gets or sets the context type label.
        /// </summary>
        [ConfigurationProperty("type", IsKey = false, IsRequired = true)]
        public string Type
        {
            get { return (string) this["type"]; }
            set
            {
                System.Type type = System.Type.GetType(value);
                if (type == null)
                    throw new ConfigurationErrorsException(
                        string.Format("The context type must be a Type's full label. Current type: {0}", value));
                if (!type.IsSubclassOf(typeof (Persistence.Context)))
                    throw new ConfigurationErrorsException(string.Format("The type {0} must extends from {1} class.",
                                                                         value, typeof (Persistence.Context).FullName));
                this["type"] = value;
            }
        }

        /// <summary>
        /// Gets the context settings element collection.
        /// </summary>
        [ConfigurationProperty("settings", IsDefaultCollection = false)]
        public SettingsCollection Settings
        {
            get { return ((SettingsCollection) (base["settings"])); }
        }

        internal Persistence.Context GetContext()
        {
            System.Type ctxType = System.Type.GetType(Type);
            Persistence.Context ctx = (Persistence.Context) ctxType.Assembly.CreateInstance(ctxType.FullName);
            if (ctx != null)
            {
                foreach (Setting setting in Settings)
                    ctx.Settings.Add(setting.Name, setting.Value);
            }
            return ctx;
        }
    }
}