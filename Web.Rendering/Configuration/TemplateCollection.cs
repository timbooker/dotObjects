using System.Configuration;
using dotObjects.Core.Processing;
using dotObjects.Web.Rendering.Helpers;
using System;

namespace dotObjects.Web.Rendering.Configuration
{
    [ConfigurationCollection(typeof (Template))]
    public class TemplateCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new Template();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            Template template = (Template) element;
            return template.Filter + Template.FilterKeySeparator;
        }

        public Template Find(string url, string extension)
        {
            foreach (Template tpl in this)
                if (tpl.IsRegexTemplate(url) || tpl.IsExtensionTemplate(extension))
                    return tpl;
            return null;
        }

        public Template Find(ProcessURI uri)
        {
            foreach (Template tpl in this)
                if (tpl.IsUriTemplate(uri))
                    return tpl;
            return null;
        }

        public Template Find(IProcessResponse response)
        {
            foreach (Template tpl in this)
                if (tpl.IsResponseTemplate(response))
                    return tpl;
            return null;
        }
    }
}