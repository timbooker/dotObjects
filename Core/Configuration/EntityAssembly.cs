using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using dotObjects.Core.UI;

namespace dotObjects.Core.Configuration
{
    public class EntityAssembly : ConfigurationElement
    {
        private static Dictionary<string, List<Type>> types = new Dictionary<string, List<Type>>();
        private string name;

        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                    name = (string) this["name"];
                return name;
            }
        }

        /// <summary>
        /// Gets or sets the root namespace of assembly.
        /// </summary>
        [ConfigurationProperty("rootNamespace", IsRequired = true)]
        public string RootNamespace
        {
            get { return (string)base["rootNamespace"]; }
        }

        /// <summary>
        /// Gets or sets the localization resource type for specified model assembly.
        /// </summary>
        [ConfigurationProperty("localizationResource", IsRequired = false)]
        public string LocalizationResource
        {
            get { return (string)base["localizationResource"]; }
            set { base["localizationResource"] = value; }
        }

        /// <summary>
        /// Gets or sets if all types in assembly are excluded.
        /// </summary>
        [ConfigurationProperty("excludeAllTypes", IsRequired = false)]
        public bool ExcludeAllTypes
        {
            get { return (bool)base["excludeAllTypes"]; }
            set { base["excludeAllTypes"] = value; }
        }

        [ConfigurationProperty("excludedTypes", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ExcludedTypeCollection), AddItemName = "excludedType")]
        public ExcludedTypeCollection ExcludedTypes
        {
            get { return ((ExcludedTypeCollection)(base["excludedTypes"])); }
        }


        public Assembly Assembly
        {
            get
            {
                AssemblyName assemblyName = new AssemblyName(Name);
                return AppDomain.CurrentDomain.Load(assemblyName);
            }
        }

        public Type FindType(string name)
        {
            foreach (Type type in GetTypes())
            {
                string fullName = RootNamespace + Domain.TypeSeparator + name;
                if (type.FullName != null)
                {
                    if (type.FullName.Equals(name) || type.FullName.Equals(fullName))
                    {
                        return type;
                    }
                }
            }

            return null;
        }

        public IEnumerable<Type> GetTypes()
        {
            if (!types.ContainsKey(Name))
            {
                List<Type> asmTypes = new List<Type>();
                foreach (Type type in Assembly.GetTypes())
                    if (!type.IsEnum)
                        asmTypes.Add(type);
                types.Add(Name, asmTypes);
            }
            return types[Name];
        }

        public bool IsEntityType(Type type)
        {
            foreach (Type t in GetTypes())
                if (t == type)
                    return true;
            return false;
        }
    }
}