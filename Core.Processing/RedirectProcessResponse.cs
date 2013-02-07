using System;

namespace dotObjects.Core.Processing
{
    public class RedirectProcessResponse : IProcessResponse
    {
        public ProcessURI URI { get; set; }

        public RedirectProcessResponse(ProcessURI uri)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");
            URI = uri;
        }
    }
}