using System;
using System.Collections.Generic;
using dotObjects.Core.Persistence;

namespace dotObjects.Core.Processing
{
    public class Expression : List<ExpressionSingleNode>
    {
        public static Expression Empty = new Expression(string.Empty, new List<ExpressionSingleNode>());

        public string Value { get; private set; }

        private Expression(string value, IEnumerable<ExpressionSingleNode> nodes)
            : base(nodes)
        {
            Value = value;
        }

        public static Expression Parse(string value, ProcessBehavior behavior)
        {
            if (string.IsNullOrEmpty(value)) 
                return null;

            List<ExpressionSingleNode> nodes = new List<ExpressionSingleNode>();

            var values = new List<string>(value.Split(new[] {ProcessURI.Separator}, StringSplitOptions.RemoveEmptyEntries));
            while(values.Count > 0)
                nodes.Add(CreateNode(values));

            return new Expression(value, nodes);
        }

        private static ExpressionSingleNode CreateNode(List<string> arguments)
        {
            if (arguments.Count > 0)
            {
                string arg = arguments[0];
                arguments.RemoveAt(0);
                switch (arg)
                {
                    case "Between":
                        throw new NotImplementedException();
                    case "And":
                    case "Or":
                        return new ExpressionTripletNode(arg, CreateNode(arguments), CreateNode(arguments));
                    case "Contains":
                    case "Equals":
                    case "NotEquals":
                    case "GreaterThan":
                    case "GreaterThanOrEqual":
                    case "LessThan":
                    case "LessThanOrEqual":
                    case "In":
                    case OrderBy.Key:
                        var fieldName = arguments[0];
                        var fieldValue = arguments[1];
                        arguments.RemoveRange(0, 2);
                        return new ExpressionSingleNode(arg, new ExpressionSingleNode(fieldName, fieldValue));
                    case Query.ProjectionKey:
                    case Query.NameKey:
                    case Query.TakeKey:
                    case Query.SkipKey:
                    case "IsNull":
                    case "IsNotNull":
                        var value = arguments[0];
                        arguments.RemoveAt(0);
                        return new ExpressionSingleNode(arg, value);
                }
            }
            return null;
        }

        public override string ToString()
        {
            string[] nodes = new string[Count];
            for (int i = 0; i < nodes.Length; i++)
                nodes[i] = this[i].ToString();
            return string.Join(ProcessURI.Separator, nodes);
        }
    }
}