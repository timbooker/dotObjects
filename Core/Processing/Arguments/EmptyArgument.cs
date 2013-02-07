using System;
using System.Collections.Generic;

namespace dotObjects.Core.Processing.Arguments
{
    [Serializable]
    public class EmptyArgument : ProcessArgument
    {
        public EmptyArgument(List<object> values) : base(values)
        {
        }

        protected override void ExtractValues(List<object> values)
        {
        }
    }
}