using System;
using System.Collections.Generic;

namespace dotObjects.Core.Processing.Arguments
{
    [Serializable]
    public class ExecutionArgument : ProcessArgument
    {
        public ExecutionArgument(List<object> values)
            : base(values)
        {
        }

        public object Id { get; private set; }

        public string Name { get; private set; }

        public int Index { get; private set; }

        protected override void ExtractValues(List<object> values)
        {
            if (values.Count == 3 || values.Count == 2)
            {
                Name = values[0].ToString();
                Index = int.Parse(values[1].ToString());
                Id = (values.Count == 3) ? values[2] : null;
            }
        }
    }
}