namespace dotObjects.Core.Processing
{
    public class ExpressionSingleNode
    {
        public string Name { get; private set; }
        public virtual object Value { get; set; }

        public ExpressionSingleNode(string name)
        {
            Name = name;
        }

        public ExpressionSingleNode(string name, object value)
            : this(name)
        {
            Value = value;
        }

        public bool Contains(string value)
        {
            if (Value is ExpressionSingleNode)
                return ((ExpressionSingleNode) Value).Contains(value);
            return Value.ToString().Contains(value);
        }

        public override string ToString()
        {
            return Name + ProcessURI.Separator + Value;
        }
    }
}