using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dotObjects.Session
{
    public interface ISessionState
    {
        object this[string name] { get; set; }
        void Add(string name, object value);
        void Remove(string name);
        bool Contains(string name);
    }
}
