using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dotObjects.Session
{
    public sealed class SessionManager
    {
        private static object moniker = new object();
        private static ISessionState session;

        private SessionManager() { }

        public static ISessionState Session
        {
            get
            {
                lock (moniker)
                {
                    if (session == null)
                        session = new DefaultSession();
                }
                return session;
            }
            private set { session = value; }
        }

        public static void Define(ISessionState session)
        {
            Session = session;
        }
    }
}
