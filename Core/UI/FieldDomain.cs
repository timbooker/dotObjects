using System;

namespace dotObjects.Core.UI
{
    [Serializable]
    public class FieldDomain : Domain
    {
        public FieldDomain(string label, Type type)
            : base(type)
        {
            if (string.IsNullOrEmpty(label))
                throw new ArgumentNullException("label");
            Label = label;

            if (type == null)
                throw new ArgumentNullException("type");
            Type = type;
            ID = type.Name;
        }

        public FieldDomain(string label, Type type, Domain parent)
            : this(label, type)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");
            Parent = parent;
        }

        public FieldDomain(string label, Type type, Domain parent, object value, string id)
            : this(label, type, parent)
        {
            Value = value;
            ID = id;
        }
    }
}