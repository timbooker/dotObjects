using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Resources;
using dotObjects.Core.UI;
using System.Threading;

namespace dotObjects.Core.Configuration
{
    /// <summary>
    /// Represents the dotObjects core configuration element.
    /// </summary>
    public class CoreSection : ConfigurationSection
    {
        private const string SectionName = "dotObjects/core";
        private static CoreSection current;
        private static Dictionary<string, ResourceSet> localizationSets;

        /// <summary>
        /// Gets or sets the application name.
        /// </summary>
        [ConfigurationProperty("applicationName", IsRequired = true)]
        public string ApplicationName
        {
            get { return (string) base["applicationName"]; }
            set { base["applicationName"] = value; }
        }

        [ConfigurationProperty("contexts", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof (ContextCollection), AddItemName = "context")]
        public ContextCollection Contexts
        {
            get { return ((ContextCollection) (base["contexts"])); }
        }

        [ConfigurationProperty("entityAssemblies", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof (EntityAssembliesCollection), AddItemName = "assembly")]
        public EntityAssembliesCollection EntityAssemblies
        {
            get { return ((EntityAssembliesCollection)(base["entityAssemblies"])); }
        }

        [ConfigurationProperty("interceptors", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(InterceptorsCollection), AddItemName = "interceptor")]
        public InterceptorsCollection Interceptors
        {
            get { return ((InterceptorsCollection)(base["interceptors"])); }
        }

        /// <summary>
        /// Gets the current dotObjects core configuration.
        /// </summary>
        public static CoreSection Current
        {
            get { return current ?? (current = (CoreSection) ConfigurationManager.GetSection(SectionName)); }
        }

        /// <summary>
        /// Get the available resource managers.
        /// </summary>
        private static Dictionary<string, ResourceSet> LocalizationSets
        {
            get
            {
                if (localizationSets == null)
                {
                    localizationSets = new Dictionary<string, ResourceSet>();
                    foreach (EntityAssembly asm in Current.EntityAssemblies)
                    {
                        var stream = asm.Assembly.GetManifestResourceStream(asm.LocalizationResource + ".resources");
                        if (stream != null)
                        {
                            var set = new ResourceSet(stream);
                            localizationSets.Add(asm.Name, set);
                        }
                    }
                }
                return localizationSets;
            }
        }

        /// <summary>
        /// Finds a entity type by name.
        /// </summary>
        /// <param name="name">The name of entity type.</param>
        /// <returns>A <see cref="System.Type"/> instance if found, null if not.</returns>
        public Type FindType(string name)
        {
            foreach (EntityAssembly assembly in EntityAssemblies)
            {
                Type found = assembly.FindType(name);
                if (found != null)
                    return found;
            }
            return null;
        }

        /// <summary>
        /// Gets all entity types in EntityAssemblies collection.
        /// </summary>
        /// <returns>A <see cref="Array"/> of <see cref="System.Type"/> instances.</returns>
        public Type[] GetTypes()
        {
            List<Type> types = new List<Type>();
            foreach (EntityAssembly elem in EntityAssemblies)
            {
                foreach (Type type in elem.GetTypes())
                    if (!type.IsEnum)
                        types.Add(type);
            }
            return types.ToArray();
        }

        public bool IsEntity(Type type)
        {
            if (type != null)
            {
                foreach (EntityAssembly asm in EntityAssemblies)
                    if (asm.IsEntityType(type))
                        return true;
            }
            return false;
        }

        public static bool IsExcludedEntity(Type type)
        {
            string asmName = type.Assembly.GetName().Name;
            EntityAssembly foundAsm = null;
            foreach (EntityAssembly asm in Current.EntityAssemblies)
            {
                if (asm.Name.Equals(asmName))
                {
                    foundAsm = asm;
                    break;
                }
            }
            if (foundAsm == null) return false;
            if (foundAsm.ExcludeAllTypes) return true;
            foreach (ExcludedType excludeType in foundAsm.ExcludedTypes)
                if (type.FullName != null)
                    if (type.FullName.Equals(excludeType.TypeName))
                        return true;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        private static ResourceSet GetLocalizationSet(Assembly assembly)
        {
            return LocalizationSets.ContainsKey(assembly.GetName().Name)
                       ? LocalizationSets[assembly.GetName().Name]
                       : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public string GetLocalizationString(Type type, params string[] keys)
        {
            ResourceSet set = GetLocalizationSet(type.Assembly);
            string allKeys = string.Join(Domain.TypeSeparator, keys).ToLower();

            if (set != null)
            {
                string fullKey = type.FullName.ToLower();
                if (keys != null && keys.Length > 0)
                    fullKey = (fullKey + Domain.TypeSeparator + allKeys).ToLower();

                var enumerator = set.GetEnumerator();
                while(enumerator.MoveNext())
                {
                    var key = enumerator.Key.ToString();
                    if (key.Equals(fullKey) || key.Equals(allKeys))
                        return enumerator.Value.ToString();
                    if (string.IsNullOrEmpty(allKeys) && key.Equals(type.Name.ToLower()))
                        return enumerator.Value.ToString();
                }
            }
            return string.IsNullOrEmpty(allKeys) ? type.Name : string.Join(Domain.TypeSeparator, keys);
        }

        public static bool IsSystemType(Type type)
        {
            return Array.Exists(Enum.GetNames(typeof (TypeCode)), name => name.Equals(type.Name));
        }

        public static string GetTypeName(Type type)
        {
            if (type != null)
            {
                string asmName = type.Assembly.GetName().Name;
                EntityAssembly asm = CoreSection.Current.EntityAssemblies[asmName];
                if (type.FullName != null)
                    return type.FullName.Replace(asm.RootNamespace + Domain.TypeSeparator, string.Empty);
            }
            return string.Empty;
        }

        public Type GetType(string typeName)
        {
            foreach (EntityAssembly asm in EntityAssemblies)
            {
                var type = asm.FindType(typeName);
                if (type != null)
                    return type;
            }
            return null;
        }

        public EntityAssembly GetAssemblyByType(string typeName)
        {
            foreach (EntityAssembly asm in EntityAssemblies)
            {
                var type = asm.FindType(typeName);
                if (type != null)
                    return asm;
            }
            return null;
        }

        public List<IInterceptor> GetInteceptors()
        {
            var interceptors = new List<IInterceptor>();
            foreach (Interceptor interceptor in Interceptors)
            {
                var instance = Activator.CreateInstance(Type.GetType(interceptor.TypeName));
                interceptors.Add((IInterceptor) instance);
            }
            return interceptors;
        }
    }
}