using System;
using System.Collections.Generic;
using System.Linq;
using dotObjects.Core.Configuration;
using dotObjects.Core.UI;

namespace dotObjects.Core.Processing.Schema
{
    public class NamespaceSchema : SchemaBase
    {
        private List<EntitySchema> entities;
        private List<NamespaceSchema> namespaces;

        private NamespaceSchema(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            Name = name;
        }

        public NamespaceSchema(string name, IEnumerable<Type> types) : this(name)
        {
            if (types != null)
            {
                var enumerator = types.GetEnumerator();
                if (enumerator.MoveNext())
                    Label = CoreSection.Current.GetLocalizationString(enumerator.Current, Name);
            }

            if (!string.IsNullOrEmpty(Label) && Label.Equals(Name))
                Label = Label.Substring(Label.LastIndexOf(Domain.TypeSeparator, StringComparison.Ordinal) + 1);

            CreateChildElements(types);
        }

        public NamespaceSchema(string name, EntityDomain[] entityDomains) : this(name)
        {
            var types = new List<Type>();
            Array.ForEach(entityDomains, d => types.Add(d.Type));
            IEnumerator<Type> enumerator = types.GetEnumerator();

            if (enumerator.MoveNext())
                Label = CoreSection.Current.GetLocalizationString(enumerator.Current, Name);

            if (Label.Equals(Name))
                Label = Label.Substring(Label.LastIndexOf(Domain.TypeSeparator, StringComparison.Ordinal) + 1);

            CreateChildElements(entityDomains);
        }


        internal string Name { get; set; }

        public bool IsRoot { get; set; }

        public List<NamespaceSchema> Namespaces
        {
            get { return namespaces ?? (namespaces = new List<NamespaceSchema>()); }
            set { namespaces = value; }
        }

        public List<EntitySchema> Entities
        {
            get { return entities ?? (entities = new List<EntitySchema>()); }
            internal set { entities = value; }
        }

        public override bool Visible
        {
            get
            {
                if (Namespaces.Count == Namespaces.FindAll(ns => !ns.Visible).Count
                    && Entities.Count == Entities.FindAll(m => !m.Visible).Count)
                    return false;
                return base.Visible;
            }
            set { base.Visible = value; }
        }

        private void CreateChildElements(IEnumerable<Type> types)
        {
            foreach (Type type in types)
            {
                if (IsChildNamespace(type.Namespace) && Namespaces.Find(ns => ns.Name.Equals(type.Namespace)) == null)
                {
                    var child = new NamespaceSchema(type.Namespace, types);
                    if (child.Entities.Count > 0) Namespaces.Add(child);
                }
                else if (type.Namespace != null && type.Namespace.Equals(Name) && type.IsPublic && !type.IsNested &&
                         !type.IsAbstract && !type.IsEnum && !IsLocalizationType(type))
                    Entities.Add(new EntitySchema(type, null));
            }
        }

        private void CreateChildElements(EntityDomain[] entityDomains)
        {
            Array.ForEach(entityDomains, entity =>
                                            {
                                                if (entity.Type.Namespace != null && entity.Type.Namespace.Equals(Name) &&
                                                    entity.Type.IsPublic && !entity.Type.IsNested &&
                                                    !entity.Type.IsAbstract && !entity.Type.IsEnum &&
                                                    !IsLocalizationType(entity.Type) &&
                                                    !CoreSection.IsExcludedEntity(entity.Type))
                                                    Entities.Add(new EntitySchema(entity.Type, entity));
                                            });
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

        public EntitySchema FindEntity(Type type)
        {
            return FindInEntities(type) ?? FindInNamespaces(type);
        }

        private EntitySchema FindInNamespaces(Type type)
        {
            return Namespaces.Select(ns => ns.FindEntity(type)).FirstOrDefault(entity => entity != null);
        }

        private EntitySchema FindInEntities(Type type)
        {
            return Entities.FirstOrDefault(entity => entity.EntityType == type);
        }

        private bool IsChildNamespace(string @namespace)
        {
            return @namespace != null && @namespace.StartsWith(Name) &&
                   @namespace.Split('.').Length == Name.Split('.').Length + 1;
        }
    }
}