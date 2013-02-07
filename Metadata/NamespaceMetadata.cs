using System;
using System.Collections.Generic;
using dotObjects.Core.Configuration;

namespace dotObjects.Metadata
{
    public class NamespaceMetadata
    {
        private List<NamespaceMetadata> namespaces;
        private List<EntityMetadata> entities;

        public string Name { get; set; }

        public List<NamespaceMetadata> Namespaces
        {
            get { return namespaces ?? (namespaces = new List<NamespaceMetadata>()); }
        }

        public List<EntityMetadata> Entities
        {
            get { return entities ?? (entities = new List<EntityMetadata>()); }
        }

        internal List<EntityMetadata> AllEntities
        {
            get
            {
                var all = new List<EntityMetadata>();
                all.AddRange(Entities);
                foreach (var ns in Namespaces)
                    all.AddRange(ns.AllEntities);
                return all;
            }
        }

        internal NamespaceMetadata(string name, IEnumerable<Type> types)
        {
            Name = name;
            foreach (Type type in types)
            {
                if (IsChildNamespace(type.Namespace) && Namespaces.Find(ns => ns.Name.Equals(type.Namespace)) == null)
                {
                    var child = new NamespaceMetadata(type.Namespace, types);
                    if (child.AllEntities.Count > 0) Namespaces.Add(child);
                }

                if (Name.Equals(type.Namespace) && type.IsPublic && !type.IsNested && !type.IsAbstract && !type.IsEnum && !IsLocalizationType(type))
                    Entities.Add(new EntityMetadata(type));
            }
        }

        private static bool IsLocalizationType(Type type)
        {
            foreach (EntityAssembly asm in CoreSection.Current.EntityAssemblies)
            {
                if (type.FullName != null)
                    if (type.FullName.Equals(asm.LocalizationResource))
                        return true;
            }
            return false;
        }

        private bool IsChildNamespace(string @namespace)
        {
            return @namespace != null && @namespace.StartsWith(Name) &&
                   @namespace.Split('.').Length == Name.Split('.').Length + 1;
        }


    }
}
