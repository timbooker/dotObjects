using System;
using System.Collections.Generic;

namespace dotObjects.Core.UI
{
    [Serializable]
    public class DomainCollection : List<Domain>
    {
        public DomainCollection()
        {
        }

        public DomainCollection(Domain owner)
        {
            Owner = owner;
        }

        private Domain Owner { get; set; }

        public new void Add(Domain item)
        {
            base.Add(item);
            item.Parent = Owner;
        }

        public Domain Get(int index)
        {
            if (index < Count)
                return this[index];
            return null;
        }

        public bool ContainsValue(object value)
        {
            if (value != null)
                foreach (Domain item in this)
                    if (item.HasValue && value.ToString().Equals(item.Value.ToString()))
                        return true;
            return false;
        }
    }
}