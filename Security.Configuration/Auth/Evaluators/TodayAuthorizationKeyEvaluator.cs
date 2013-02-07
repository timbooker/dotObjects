using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dotObjects.Security.Configuration.Auth.Evaluators
{
    public class TodayAuthorizationKeyEvaluator : AuthorizationKeyEvaluator
    {
        public override string KeyName
        {
            get { return "today"; }
        }

        public override object KeyValue
        {
            get { return DateTime.Today.ToShortDateString(); }
        }
    }
}
