using System;
using System.Collections.Generic;

namespace dotObjects.Core.Processing.Arguments
{
    [Serializable]
    public abstract class ProcessArgument
    {
        protected ProcessArgument(List<object> values)
        {
            Values = values;
            ExtractValues(Values);
        }

        public List<object> Values { get; private set; }

        protected abstract void ExtractValues(List<object> values);

        public static ProcessArgument Parse(ProcessBehavior behavior, string value)
        {
            value = value ?? string.Empty;
            return Parse(behavior, value.Split(new[] {ProcessURI.Separator}, StringSplitOptions.RemoveEmptyEntries));
        }

        public static ProcessArgument Parse(ProcessBehavior behavior, params object[] arguments)
        {
            List<object> values = new List<object>(arguments);
            switch (behavior)
            {
                case ProcessBehavior.New:
                    return new IndexedArgument(values);
                case ProcessBehavior.View:
                case ProcessBehavior.Edit:
                case ProcessBehavior.Delete:
                    return new IdentifiedArgument(values);
                case ProcessBehavior.Exec:
                    return new ExecutionArgument(values);
                case ProcessBehavior.Query:
                    return new QueryArgument(values);
                case ProcessBehavior.Session:
                    return new SessionArgument(values);
            }
            return new EmptyArgument(values);
        }

        public override string ToString()
        {
            return string.Join(ProcessURI.Separator, Values.ToArray());
        }
    }
}