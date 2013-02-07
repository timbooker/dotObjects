using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dotObjects.Core.UI;
using dotObjects.Core.Processing.Arguments;
using dotObjects.Session;

namespace dotObjects.Core.Processing
{
    public class SessionProcess : Process
    {
        public SessionProcess(ProcessURI uri)
            : base(uri)
        {
        }

        public override IProcessResponse Execute(Domain domain, Action<object> before, Action<object> after)
        {
            SessionArgument argument = URI.Argument as SessionArgument;
            if(argument != null)
            {
                before(null);
                ISessionState session = SessionManager.Session;
                foreach (var set in argument.Sets.Keys)
                {
                    if (session.Contains(set))
                        session[set] = argument.Sets[set];
                    else
                        session.Add(set, argument.Sets[set]);
                }

                foreach (var remove in argument.Removes)
                {
                    if (session.Contains(remove.ToString()))
                        session.Remove(remove.ToString());
                }
                after(null);
            }
            return new EmptyResponse() { URI = URI };
        }
    }
}
