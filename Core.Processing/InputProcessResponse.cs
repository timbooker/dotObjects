using System;
using dotObjects.Core.UI;

namespace dotObjects.Core.Processing
{
    [Serializable]
    public class InputProcessResponse : MessageProcessResponse
    {
        public InputProcessResponse(EntityDomain domain)
        {
            Domain = domain;
        }

        internal InputProcessResponse(EntityDomain domain, MessageProcessResponse message)
            : base(message.Message, message.MessageType, null)
        {
            Domain = domain;
        }

        public EntityDomain Domain { get; private set; }
    }
}