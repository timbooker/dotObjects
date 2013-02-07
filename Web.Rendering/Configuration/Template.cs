using System.Configuration;
using dotObjects.Core.Processing;
using System.Text.RegularExpressions;
using dotObjects.Security.Configuration.Auth;

namespace dotObjects.Web.Rendering.Configuration
{
    public class Template : ConfigurationElement
    {
        internal const string FilterKeySeparator = ":";
        private const string ResponseFilterKey = "r";
        private const string UriFilterKey = "u";
        private const string ExtensionFilterKey = "e";
        private const string RegexFilterKey = "regex";

        private string filter;
        [ConfigurationProperty("filter", IsKey = true, IsRequired = true)]
        public string Filter
        {
            get
            {
                if (string.IsNullOrEmpty(filter))
                {
                    filter = (string) base["filter"];
                    
                    if (!filter.StartsWith(ResponseFilterKey + FilterKeySeparator) &&
                        !filter.StartsWith(UriFilterKey + FilterKeySeparator) && 
                        !filter.StartsWith(ExtensionFilterKey + FilterKeySeparator) &&
                        !filter.StartsWith(RegexFilterKey + FilterKeySeparator))
                    {
                        const string message = "Template filter must contains a filter identifier!"
                                               + "The available identifiers are {0}{2} for response based filters,"
                                               + " {1}{2} for URI based filters, {3}{2} for extension based filters"
                                               + " and {4}{2} for Regular Expression based filters.";
                        throw new ConfigurationErrorsException(string.Format(message, ResponseFilterKey,
                                                                             UriFilterKey, FilterKeySeparator,
                                                                             ExtensionFilterKey, RegexFilterKey));
                    }
                }
                return filter;
            }
        }

        [ConfigurationProperty("fileName", IsKey = true, IsRequired = false)]
        public string FileName
        {
            get { return (string)base["fileName"]; }
        }

        [ConfigurationProperty("layoutFileName", IsRequired = false)]
        public string LayoutFileName
        {
            get { return (string)base["layoutFileName"]; }
        }

        [ConfigurationProperty("domains", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(TemplateDomainCollection), AddItemName = "add")]
        public TemplateDomainCollection TemplateDomains
        {
            get
            {
                TemplateDomainCollection domains = (TemplateDomainCollection)base["domains"];
                return domains;
            }
        }

        internal bool IsUriTemplate(ProcessURI uri)
        {
            if (uri != null)
            {
                const string filterKey = UriFilterKey + FilterKeySeparator;
                return uri.ToString().Contains(Filter.Replace(filterKey, string.Empty));
            }
            return false;
        }

        internal bool IsResponseTemplate(IProcessResponse response)
        {
            if (response != null)
            {
                string name = response.GetType().Name;
                const string filterKey = ResponseFilterKey + FilterKeySeparator;
                return name.Contains(Filter.Replace(filterKey, string.Empty));
            }
            return false;
        }

        internal bool IsExtensionTemplate(string extension)
        {
            if (!string.IsNullOrEmpty(extension))
            {
                const string filterKey = ExtensionFilterKey + FilterKeySeparator;
                return extension.ToString().EndsWith(Filter.Replace(filterKey, string.Empty));
            }
            return false;
        }

        internal bool IsRegexTemplate(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                const string filterKey = RegexFilterKey + FilterKeySeparator;
                if(Filter.StartsWith(filterKey))
                {
                    var pattern = Filter.Replace(filterKey, string.Empty);
                    //foreach (var evaluator in AuthorizationManager.Evaluators)
                    //    if (evaluator.KeyValue != null)
                    //        pattern = pattern.Replace(evaluator.FullKey, evaluator.KeyValue.ToString());
                    return Regex.IsMatch(value, pattern);
                } 
            }
            return false;
        }
    }
}