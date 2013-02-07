using System;

namespace dotObjects.Core.Processing.Schema
{
    public class ProcessSchema : SchemaBase
    {
        private ProcessURI uri;

        internal ProcessSchema()
        {
        }

        public ProcessSchema(string label, ProcessURI uri)
        {
            Label = label;
            URI = uri;
        }

        public ProcessURI URI
        {
            get { return uri; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                uri = value;
            }
        }

        public ProcessSchemaType Type { get; set; }
    }
}