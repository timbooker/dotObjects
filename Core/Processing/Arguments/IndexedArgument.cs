using System;
using System.Collections.Generic;

namespace dotObjects.Core.Processing.Arguments
{
    [Serializable]
    public class IndexedArgument : ProcessArgument
    {
        public IndexedArgument(List<object> values)
            : base(values)
        {
        }

        public int Index { get; private set; }

        protected override void ExtractValues(List<object> values)
        {
            if (values.Count > 0)
            {
                Index = int.Parse(values[0].ToString());
                values.RemoveAt(0);
            }
        }

        public override string ToString()
        {
            return Index.ToString();
        }
    }
}