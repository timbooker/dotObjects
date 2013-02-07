using System;
using System.Collections.Generic;
using dotObjects.Core.UI;

namespace dotObjects.Core.Processing
{
    internal class WorkflowProcess : Process
    {
        private Dictionary<string, Process> steps;

        public WorkflowProcess(ProcessURI uri)
            : base(uri)
        {
        }

        public Dictionary<string, Process> Steps
        {
            get { return steps ?? (steps = new Dictionary<string, Process>()); }
        }

        public override IProcessResponse Execute(Domain domain, Action<object> before, Action<object> after)
        {
            if (Steps.ContainsKey(URI.Entity))
            {
                before(null);
                var response = Steps[URI.Entity].Execute(domain, before, after);
                after(response);
                return response;
            }
            throw new ArgumentException();
        }
    }
}