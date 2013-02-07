using System;
using System.Collections.Generic;

namespace dotObjects.Core.Processing.Arguments
{
    [Serializable]
    public class IdentifiedArgument : ProcessArgument
    {
        public IdentifiedArgument(List<object> values) : base(values)
        {
        }

        public object Id { get; private set; }

        protected override void ExtractValues(List<object> values)
        {
            if (values.Count == 1)
                Id = values[0];
        }
    }
}