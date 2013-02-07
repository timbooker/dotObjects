using System.Configuration;

namespace dotObjects.Security.Configuration
{
    public class AuthorizationRuleFilter : ConfigurationElement
    {
        [ConfigurationProperty("for", IsKey = true, IsRequired = true)]
        public string For
        {
            get { return (string) base["for"]; }
        }

        [ConfigurationProperty("expression", IsRequired = false)]
        public string Expression
        {
            get { return (string)base["expression"]; }
        }
    }
}