using System.Configuration;
using System;

namespace dotObjects.Security.Configuration
{
    public class AuthorizationKeyEvaluator : ConfigurationElement
    {
        [ConfigurationProperty("type", IsKey = true, IsRequired = true)]
        public string Type
        {
            get { return (string) base["type"]; }
        }

        public Type UnderlineType
        {
            get { return System.Type.GetType(Type); }
        }
    }
}