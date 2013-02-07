using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;

namespace dotObjects.Security.Configuration
{
    public class AuthorizationRule : ConfigurationElement
    {
        private const string AnonymousRole = "?";
        private const string EveryoneRole = "*";

        private AuthorizationRuleType type;

        [TypeConverter(typeof (CommaDelimitedStringCollectionConverter))]
        [ConfigurationProperty("roles", IsKey = true, IsRequired = true)]
        public StringCollection Roles
        {
            get
            {
                return (CommaDelimitedStringCollection) base["roles"]
                       ?? new CommaDelimitedStringCollection();
            }
        }

        [ConfigurationProperty("expression", IsRequired = false)]
        public string Expression
        {
            get { return (string) base["expression"]; }
        }

        [ConfigurationProperty("", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(AuthorizationURI), AddItemName = "filter")]
        public AuthorizationRuleFilterCollection Filters
        {
            get { return (AuthorizationRuleFilterCollection)base[""]; }
        }

        public AuthorizationRuleType Type
        {
            get { return type; }
            set { type = value; }
        }

        public bool DenyEveryone
        {
            get { return EveryoneRole.Equals(Roles.ToString()) && Type == AuthorizationRuleType.Deny; }
        }

        public bool AllowEveryone
        {
            get { return EveryoneRole.Equals(Roles.ToString()) && Type == AuthorizationRuleType.Allow; }
        }

        public bool AllowAnonymous
        {
            get { return AnonymousRole.Equals(Roles.ToString()) && Type == AuthorizationRuleType.Allow; }
        }

        public bool DenyAnonymous
        {
            get { return AnonymousRole.Equals(Roles.ToString()) && Type == AuthorizationRuleType.Deny; }
        }
    }
}