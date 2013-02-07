using dotObjects.Core.UI;
using dotObjects.Core.Processing;
using dotObjects.Session;
using System;

namespace dotObjects.Web.Rendering.Helpers
{
    public class StateHelper : RendererHelper
    {
        public StateHelper(string name) : base(name)
        {
        }

        public string FieldName
        {
            get { return ViewStateManager.FieldName; }
        }

        public static string Register(Domain domain)
        {
            return Register(domain, ViewStateManager.FieldName);
        }

        public static string Register(Domain domain, string name)
        {
            var key = Guid.NewGuid().ToString("N");
            var session = SessionManager.Session;
            var serialize = ViewStateManager.Serialize(domain);
            session.Add(key, serialize);
            return string.Format("<input type='hidden' id='{0}' name='{0}' value='{1}' />", ViewStateManager.FieldName, key);
        }

        public static object RegisterProcessState()
        {
            return string.Format("<input type='hidden' id='{0}' name='{0}' value='{1}' />", Process.StateFieldName,
                                 ProcessState.ToExecute);
        }
    }
}