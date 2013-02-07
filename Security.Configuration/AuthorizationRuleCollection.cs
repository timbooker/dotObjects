using System.Collections.Generic;
using System.Configuration;

namespace dotObjects.Security.Configuration
{
    [ConfigurationCollection(typeof (AuthorizationRule), AddItemName = "allow, deny",
        CollectionType = ConfigurationElementCollectionType.BasicMapAlternate)]
    public class AuthorizationRuleCollection : ConfigurationElementCollection
    {
        protected override string ElementName
        {
            get { return string.Empty; }
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMapAlternate; }
        }

        public AuthorizationRule this[int index]
        {
            get { return (AuthorizationRule) BaseGet(index); }
        }

        protected override bool IsElementName(string elementName)
        {
            string name = elementName.ToLowerInvariant();
            return name.Equals("allow") || name.Equals("deny");
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return CreateNewElement(ElementName);
        }

        protected override ConfigurationElement CreateNewElement(string elementName)
        {
            AuthorizationRule rule = new AuthorizationRule();
            if (!string.IsNullOrEmpty(elementName))
            {
                switch (elementName.ToLowerInvariant())
                {
                    case "allow":
                        rule.Type = AuthorizationRuleType.Allow;
                        break;
                    case "deny":
                        rule.Type = AuthorizationRuleType.Deny;
                        break;
                }
                return rule;
            }
            rule.Type = AuthorizationRuleType.Allow;
            return rule;
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AuthorizationRule) element).Roles;
        }

        public List<AuthorizationRule> ToList()
        {
            List<AuthorizationRule> rules = new List<AuthorizationRule>();
            foreach (AuthorizationRule rule in this)
                rules.Add(rule);
            return rules;
        }
    }
}