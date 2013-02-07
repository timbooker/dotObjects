using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using dotObjects.Core.UI;

namespace dotObjects.Core.Configuration
{
    public class Interceptor : ConfigurationElement
    {
        [ConfigurationProperty("typeName", IsKey = true, IsRequired = true)]
        public string TypeName
        {
            get { return (string)this["typeName"]; }
        }
    }
}