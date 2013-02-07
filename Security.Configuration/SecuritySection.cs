using System.Configuration;

namespace dotObjects.Security.Configuration
{
    public class SecuritySection : ConfigurationSection
    {
        internal const string SectionName = "dotObjects/security";

        private static SecuritySection current;

        public static SecuritySection Current
        {
            get { return current ?? (current = (SecuritySection) ConfigurationManager.GetSection(SectionName)); }
        }

        [ConfigurationProperty("provider", IsRequired = false)]
        public Provider Provider
        {
            get { return (Provider) base["provider"]; }
        }

        [ConfigurationProperty("authorization", IsRequired = false, IsDefaultCollection = true)]
        public Authorization Authorization
        {
            get { return (Authorization) base["authorization"]; }
        }
    }
}