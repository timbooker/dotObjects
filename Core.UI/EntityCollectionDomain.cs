using System;
using System.Collections;
using System.Collections.Generic;
using dotObjects.Core.Persistence;
using dotObjects.Core.Processing;
using dotObjects.Core.Serialization;
using dotObjects.Core.Configuration;

namespace dotObjects.Core.UI
{
    [Serializable]
    public class EntityCollectionDomain : EntityDomain
    {
        private Type collectionType;
        private const string FieldPrefixFormat = "{0}[{1}]";

        public EntityCollectionDomain(Type type, Type collectionType, ProcessURI uri, object instance, Domain parent)
            : base(type, uri, null)
        {
            if (collectionType == null) throw new ArgumentNullException("collectionType");
            CollectionType = collectionType;
            Instance = instance;
            Parent = parent;
        }

        [NonSerializable]
        public Type CollectionType
        {
            get { return collectionType; }
            set { collectionType = value; }
        }

        public override object ObjectValue
        {
            get
            {
                var collection = CollectionType.Assembly.CreateInstance(CollectionType.FullName);
                foreach (Domain domain in Domains)
                {
                    EntityDomain entity = domain as EntityDomain;

                    object item = (entity.ObjectValue == null && (entity.Value == null || "".Equals(entity.Value)))
                                      ? Activator.CreateInstance(entity.Type)
                                      : (entity.ObjectValue == null)
                                            ? ContextFactory.GetContext(Type).Get(entity.Value.ToString(), Type)
                                            : entity.ObjectValue;
                    entity.Instance = item;
                    ComplexDomain.SynchronizeEntity(entity);
                    
                    if (item != null)
                        CollectionType.GetMethod("Add").Invoke(collection, new[] {item});
                }
                return collection;
            }
        }

        protected override void CreateChildDomains()
        {
            IEnumerable e = (IEnumerable) Instance;
            if (e != null)
            {
                List<object> items = new List<object>();
                IEnumerator iterator = e.GetEnumerator();
                while (iterator.MoveNext())
                {
                    object id = ContextFactory.GetContext(Type).GetID(iterator.Current);
                    Domains.Add(new EntityDomain(Type, URI, id) {Instance = iterator.Current, Parent = this});
                    items.Add(iterator.Current);
                }
            }
        }

        public override void Fill(Dictionary<string, object> data)
        {
            Domains.Clear();
            if (data.ContainsKey(UniqueID) && data[UniqueID] != null)
            {
                string items = data[UniqueID].ToString();
                foreach (string item in items.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (data.ContainsKey(string.Format(FieldPrefixFormat, String.Concat(UniqueID, item), 0)))
                        FillSubItems(data, item);
                    else
                        Domains.Add(new EntityDomain(Type, URI, item));
                }
            }
            else if (data.ContainsKey(string.Format(FieldPrefixFormat, UniqueID, 0)))
            {
                FillSubItems(data);
            }
        }

        private void FillSubItems(Dictionary<string, object> data)
        {
            FillSubItems(data, string.Empty);
        }

        private void FillSubItems(Dictionary<string, object> data, object value)
        {
            int index = 0;
            do
            {
                string key = string.Format(FieldPrefixFormat, String.Concat(UniqueID, value), index);
                EntityDomain domain;

                if ((data[key] == null) || (string.IsNullOrEmpty(data[key].ToString())))
                    domain = new EntityDomain(Type, new ProcessURI(), data[key]);
                else
                    domain = new EntityDomain(Type, URI, data[key]);

                domain.Domains.ForEach(argument =>
                {
                    string argumentKey = string.Concat(key, argument.ID);
                    /*if (argument is EntityCollectionDomain)
                        argument.Fill(data);
                    else*/
                    if (data.ContainsKey(argumentKey))
                    {
                        argument.Value = data[argumentKey];
                        if (CoreSection.Current.IsEntity(argument.Type))
                            ((EntityDomain)argument).Instance = ContextFactory.GetContext(argument.Type).Get(argument.Value.ToString(), argument.Type);
                    }
                });
                Domains.Add(domain);
                index++;
            } while (data.ContainsKey(string.Format(FieldPrefixFormat, String.Concat(UniqueID, value), index)));
        }
    }
}