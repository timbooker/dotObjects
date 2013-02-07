using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using dotObjects.Session;
using System.Web.SessionState;

namespace dotObjects.Web.Session
{
    public class WebSession : ISessionState
    {
        private HttpSessionState State {get { return HttpContext.Current.Session; }} 

        #region ISessionState Members

        public object this[string name]
        {
            get { return State[name]; }
            set { State[name] = value; }
        }

        public void Add(string name, object value)
        {
            State.Add(name, value);
        }

        public void Remove(string name)
        {
            State.Remove(name);
        }

        public bool Contains(string name)
        {
            foreach (var key in State.Keys)
            {
                if (name.Equals(key))
                    return true;
            }
            return false;
        }

        #endregion
    }
}