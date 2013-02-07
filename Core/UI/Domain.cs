using System;
using System.Linq;
using System.Collections.Generic;
using dotObjects.Core.Serialization;
using dotObjects.Core.Configuration;
using dotObjects.Core.Processing.Arguments;
using dotObjects.Core.Processing;
using dotObjects.Core.Persistence.LinqToSQL;

namespace dotObjects.Core.UI
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class Domain
    {
        internal const string IDSeparator = "_";
        public const string TypeSeparator = ".";
        private DomainCollection domains;

        public Domain()
        {
        }

        protected Domain(Type type)
            : this()
        {
            TypeName = type.Name;

            if (type.Name.StartsWith("Nullable"))
                TypeName = type.GetGenericArguments()[0].Name;
        }

        private string title;

        public string ID { get; set; }

        public Type Type { get; set; }

        public string TypeName { get; set; }

        public bool IsNumeric
        {
            get { return IsInteger || IsFraction; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsInteger
        {
            get
            {
                switch (TypeName)
                {
                    case "Byte":
                    case "Short":
                    case "Int16":
                    case "Int32":
                    case "Int64":
                        return true;

                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsFraction
        {
            get
            {
                switch (TypeName)
                {
                    case "Double":
                    case "double":
                    case "Decimal":
                    case "decimal":
                        return true;

                    default:
                        return false;
                }
            }
        }

        public string Label { get; set; }

        [NonSerializable]
        public Domain Parent { get; set; }

        public virtual object Value { get; set; }

        public IDomainFilter Filter { get; set; }

        public FormatAttribute Format { get; set; }

        public bool IsQueryable { get; set; }

        public string DependsOf { get; set; }

        [NonSerializable]
        public Domain Dependency
        {
            get
            {
                if (!string.IsNullOrEmpty(DependsOf))
                    return Parent.Domains.Find(d => d.ID.Equals(DependsOf));
                return null;
            }
        }

        public bool IsDependency
        {
            get { return Parent != null && Parent.Domains.FindAll(d => ID.Equals(d.DependsOf)).Count > 0; }
        }

        public virtual object ObjectValue
        {
            get { return Value; }
        }

        public string Title
        {
            get
            {
                if (string.IsNullOrEmpty(title))
                    return (Type != null) ? Type.Name : string.Empty;
                return title;
            }
            set { title = value; }
        }

        public string UniqueID
        {
            get
            {
                if (Parent != null)
                    return Parent.UniqueID + IDSeparator + ID;
                return ID;
            }
        }

        public bool HasValue
        {
            get { return Value != null && !string.IsNullOrEmpty(Value.ToString()); }
        }

        public bool IsEntity
        {
            get { return CoreSection.Current.IsEntity(Type); }
        }

        public DomainCollection Domains
        {
            get
            {
                if (domains == null)
                {
                    domains = new DomainCollection();
                    CreateChildDomains();
                    foreach (Domain d in domains)
                        d.Parent = this;
                }
                return domains;
            }
        }

        public bool CanRead { get; set; }

        public bool CanWrite { get; set; }

        public bool Visible { get; set; }

        protected virtual void CreateChildDomains()
        {
        }

        public virtual void Fill(Dictionary<string, object> data)
        {
            if (data.ContainsKey(UniqueID))
                Value = data[UniqueID];
            foreach (Domain childDomain in Domains)
                childDomain.Fill(data);
        }

        public Domain GetChildDomain(string id)
        {
            var domain = Domains.Find(x => x.ID.Equals(id));

            if (domain == default(Domain) && !id.IndexOf('.').Equals(-1))
            {
                var idSplit = id.Split('.');

                domain = GetChildDomain(idSplit[0]);

                if (domain != default(Domain))
                {
                    domain = domain.GetChildDomain(string.Join(".", idSplit, 1, idSplit.Length - 1));
                }
            }

            return domain;
        }

        public object GetChildDomainValue(string id)
        {
            var domain = GetChildDomain(id);

            if (domain != null)
                return domain.Value;

            return null;
        }

        public bool Match(string where)
        {
            if (!string.IsNullOrEmpty(where))
            {
                var criterias = where.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                var queryArgument = (QueryArgument)ProcessArgument.Parse(ProcessBehavior.Query, criterias);
                var expression = dotObjects.Core.Persistence.Context.GetExpression(Type, queryArgument.Where);

                var values = Array.CreateInstance(Type, 1);
                values.SetValue(ObjectValue, 0);

                return values.AsQueryable().Where(expression).Count() == 1;
            }

            return true;
        }
    }
}