using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;

namespace dotObjects.Security.Configuration
{
    public class Authorization : ConfigurationElement
    {
        [TypeConverter(typeof (CommaDelimitedStringCollectionConverter))]
        [ConfigurationProperty("roles", IsRequired = false)]
        public StringCollection Roles
        {
            get
            {
                return (CommaDelimitedStringCollection) base["roles"]
                       ?? new CommaDelimitedStringCollection();
            }
        }

        [ConfigurationProperty("uris", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof (AuthorizationURI), AddItemName = "uri")]
        public AuthorizationURICollection URIs
        {
            get { return (AuthorizationURICollection) base["uris"]; }
        }

        [ConfigurationProperty("evaluators", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(AuthorizationKeyEvaluator), AddItemName = "evaluator")]
        public AuthorizationKeyEvaluatorCollection Evaluators
        {
            get { return (AuthorizationKeyEvaluatorCollection)base["evaluators"]; }
        }
    }
}