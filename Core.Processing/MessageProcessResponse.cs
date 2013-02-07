using System;

namespace dotObjects.Core.Processing
{
    [Serializable]
    public class MessageProcessResponse : IProcessResponse
    {
        private string message;
        private MessageProcessResponseType messageType;
        [NonSerialized] private Type entityType;

        internal MessageProcessResponse()
        {
        }

        public MessageProcessResponse(string message, MessageProcessResponseType type, Type entityType)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentNullException(message);
            Message = message;
            MessageType = type;
            EntityType = entityType;
        }

        public MessageProcessResponseType MessageType
        {
            get { return messageType; }
            set { messageType = value; }
        }

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        public Type EntityType
        {
            get { return entityType; }
            private set { entityType = value; }
        }

        public ProcessURI URI { get; set; }
    }
}