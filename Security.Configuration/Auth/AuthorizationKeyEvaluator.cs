using System.Collections.Generic;
using dotObjects.Core.Processing;
using dotObjects.Core.Processing.Arguments;

namespace dotObjects.Security.Configuration.Auth
{
    public abstract class AuthorizationKeyEvaluator
    {
        private const string KeyPrefix = "{";
        private const string KeySuffix = "}";

        public string FullKey
        {
            get { return KeyPrefix + KeyName + KeySuffix; }
        }

        public abstract string KeyName { get; }
        public abstract object KeyValue { get; }

        public virtual ProcessURI Evaluate(Expression expression, ProcessURI uri)
        {
            var nodes = Evaluate(expression);
            var args = new List<object>();
            foreach (var node in nodes)
                args.AddRange(ExtractValue(node));
            uri.Argument = ProcessArgument.Parse(uri.Behavior, args.ToArray());
            return uri;
        }

        public Expression Evaluate(Expression expression)
        {
            for (int i = 0; i < expression.Count; i++)
                expression[i] = Evaluate(expression[i]);
            return expression;
        }

        private ExpressionSingleNode Evaluate(ExpressionSingleNode node)
        {
            if (node != null)
            {
                if (node is ExpressionTripletNode)
                {
                    var triplet = node as ExpressionTripletNode;
                    triplet.First = Evaluate(triplet.First);
                    triplet.Second = Evaluate(triplet.Second);
                }
                else if (node.Value is ExpressionSingleNode)
                    node.Value = Evaluate((ExpressionSingleNode) node.Value);
                else if (node.Value != null && node.Contains(FullKey))
                    node.Value = KeyValue ?? string.Empty;
            }
            return node;
        }


        public static List<object> ExtractValue(ExpressionSingleNode node)
        {
            List<object> values = new List<object>();
            if (node != null)
            {
                values.Add(node.Name);
                if (node is ExpressionTripletNode)
                {
                    var triplet = node as ExpressionTripletNode;
                    values.AddRange(ExtractValue(triplet.First));
                    values.AddRange(ExtractValue(triplet.Second));
                }
                else if (node.Value is ExpressionSingleNode)
                    values.AddRange(ExtractValue((ExpressionSingleNode) node.Value));
                else if (node.Value != null)
                    values.Add(node.Value);
            }
            return values;
        }
    }
}