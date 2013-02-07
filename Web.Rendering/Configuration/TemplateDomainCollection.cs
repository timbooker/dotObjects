using System;
using System.Configuration;
using dotObjects.Core.Configuration;
using dotObjects.Core.UI;

namespace dotObjects.Web.Rendering.Configuration
{
    [ConfigurationCollection(typeof (TemplateDomain))]
    public class TemplateDomainCollection : ConfigurationElementCollection
    {
        public TemplateDomainCollection()
        {
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new TemplateDomain();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            TemplateDomain domain = (TemplateDomain) (element);
            return !String.IsNullOrEmpty(domain.TypeName) ? domain.TypeName : domain.Attributes.ToString();
        }

        public TemplateDomain Find(Domain domain)
        {
            if (domain != null)
            {
                string domainID = domain.ID;
                foreach (TemplateDomain template in this)
                {
                    if (template.Attributes.Contains(domainID))
                        return template;
                }

                Type domainType = domain.Type;
                while (domainType != null)
                {
                    foreach (TemplateDomain template in this)
                        if (IsValid(domain, domainType, template))
                            return template;
                    domainType = domainType.BaseType;
                }

                const string message = "The domain for type {0} or id {1} was not found! Please check the rendering configuration.";
                throw new Exception(string.Format(message, domain.ID, domain.Type));
            }
            return null;
        }

        private static bool IsValid(Domain domain, Type type, TemplateDomain element)
        {
            string typeName = element.TypeName;
            return typeName.Equals(type.FullName) ||
                   (
                       type.IsGenericType &&
                       type.Name.Equals("Nullable`1") &&
                       typeName.Equals(type.GetGenericArguments()[0].FullName)
                   ) ||
                   (
                       CoreSection.Current.IsEntity(type) &&
                       (
                           (domain is EntityCollectionDomain &&
                            typeName.Equals(((EntityCollectionDomain) domain).CollectionType.FullName)) ||
                           (domain is EntityCollectionDomain && typeName.Equals(TemplateDomain.CollectionKey)) ||
                           (domain is EntityDomain && typeName.Equals(TemplateDomain.SingleKey))
                       )
                   );
        }

        internal void Add(TemplateDomain domain)
        {
            base.BaseAdd(domain);
        }

        public override bool IsReadOnly()
        {
            return false;
        }
    }
}