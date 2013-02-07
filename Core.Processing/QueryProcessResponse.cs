using System;
using System.Collections.Generic;
using dotObjects.Core.UI;
using dotObjects.Core.Serialization;

namespace dotObjects.Core.Processing
{
    /// <summary>
    /// Class that represents the Response Object for a search request.
    /// </summary>
    /// <remarks>
    /// The search result is represented by the <see cref="QueryProcessResponse.Items"/> property, each 
    /// element of the result collection is a domain object that represents the domain representation of the object response
    /// [TODO: place a link here poiting to the domain representation of objects.]
    /// </remarks>
    [Serializable]
    public class QueryProcessResponse : MessageProcessResponse
    {
        private List<Domain> items;

        public QueryProcessResponse(Type type, List<Domain> items)
        {
            if (items == null)
                throw new ArgumentNullException("items");
            Items = items;
            EntityType = type;
        }

        [NonSerializable]
        public new Type EntityType { get; private set; }

        public List<Domain> Items
        {
            get { return items ?? (items = new List<Domain>()); }
            private set { items = value; }
        }
    }
}