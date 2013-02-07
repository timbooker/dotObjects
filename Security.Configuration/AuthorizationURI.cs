using System.Configuration;

namespace dotObjects.Security.Configuration
{
    public class AuthorizationURI : ConfigurationElement
    {
        [ConfigurationProperty("pattern", IsKey = true, IsRequired = true)]
        public string Pattern
        {
            get { return (string) base["pattern"]; }
        }

        [ConfigurationProperty("", IsDefaultCollection = true)]
        public AuthorizationRuleCollection Rules
        {
            get { return (AuthorizationRuleCollection) base[""]; }
        }
    }
}