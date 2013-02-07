using System;
using dotObjects.Core.UI;

namespace dotObjects.Core.Processing
{
    [Serializable]
    public class ViewProcessResponse : MessageProcessResponse
    {
        internal ViewProcessResponse(Domain domain)
        {
            Domain = domain;           
        }

        public ViewProcessResponse(Domain domain, MessageProcessResponse message)
            : base(message.Message, message.MessageType, domain.Type)
        {
            Domain = domain;
        }

        public Domain Domain { get; set; }
    }
}