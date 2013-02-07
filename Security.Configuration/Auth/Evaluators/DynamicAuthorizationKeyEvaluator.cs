using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dotObjects.Security.Configuration.Auth.Evaluators
{
    public class DynamicAuthorizationKeyEvaluator : AuthorizationKeyEvaluator
    {
        private string keyName;
        private object value;

        public DynamicAuthorizationKeyEvaluator(string keyName, object value)
        {
            this.keyName = keyName;
            this.value = value;
        }

        public override string KeyName
        {
            get { return keyName; }
        }

        public override object KeyValue
        {
            get { return value; }
        }
    }
}
