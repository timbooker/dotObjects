using System;
using System.Collections.Generic;
using dotObjects.Core.Processing.Schema;

namespace dotObjects.Core.Processing
{
    [Serializable]
    public class SchemaProcessResponse : IProcessResponse
    {
        private List<NamespaceSchema> namespaces;

        public List<NamespaceSchema> Namespaces
        {
            get { return namespaces ?? (namespaces = new List<NamespaceSchema>()); }
        }

        public ProcessURI URI { get; set; }

        public EntitySchema FindEntity(Type type)
        {
            foreach (NamespaceSchema ns in Namespaces)
            {
                EntitySchema entity = ns.FindEntity(type);
                if (entity != null)
                    return entity;
            }
            return null;
        }
    }
}