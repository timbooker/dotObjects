using System.Collections.Generic;
using System.Configuration;

namespace dotObjects.Security.Configuration
{
    
    public class AuthorizationRuleFilterCollection : ConfigurationElementCollection
    {
        protected override string ElementName
        {
            get { return "filter"; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new AuthorizationRuleFilter();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AuthorizationRuleFilter)element).For;
        }

        public AuthorizationRuleFilter Find(string @for)
        {
            foreach (AuthorizationRuleFilter filter in this)
                if (filter.For.Equals(@for))
                    return filter;
            return null;
        }
    }
}