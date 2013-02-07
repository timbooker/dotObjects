using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dotObjects.Session;
using dotObjects.Security.Configuration.Auth;

namespace dotObjects.Web.Evaluators
{
    public class DayKeyEvaluator : AuthorizationKeyEvaluator
    {
        public override string KeyName
        {
            get { return "day"; }
        }

        public override object KeyValue
        {
            get { return SessionManager.Session["day"]; }
        }
    }
}
