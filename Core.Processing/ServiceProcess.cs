using dotObjects.Core.UI;
using System;

namespace dotObjects.Core.Processing
{
    internal class ServiceProcess : Process
    {
        public ServiceProcess(ProcessURI uri) : base(uri)
        {
        }

        public override IProcessResponse Execute(Domain domain, Action<object> before, Action<object> after)
        {
            return null;
        }
    }
}