using System;
using dotObjects.Core.UI;
namespace dotObjects.Core.Processing
{
    internal class IndexProcess : Process
    {
        public IndexProcess(ProcessURI uri)
            : base(uri)
        {
        }

        public override IProcessResponse Execute(Domain domain, Action<object> before, Action<object> after)
        {
            return new EmptyResponse() { URI = URI };
        }
    }
}