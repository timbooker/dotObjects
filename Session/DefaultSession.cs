using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dotObjects.Session
{
    public class DefaultSession : ISessionState
    {
        private Dictionary<string, object> state = new Dictionary<string, object>();

        public object this[string name]
        {
            get { return state[name]; }
            set { state[name] = value; }
        }

        public void Add(string name, object value)
        {
            state.Add(name, value);
        }

        public void Remove(string name)
        {
            state.Remove(name);
        }

        public bool Contains(string name)
        {
            return state.ContainsKey(name);
        }
    }
}
