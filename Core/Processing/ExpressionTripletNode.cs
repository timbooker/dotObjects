namespace dotObjects.Core.Processing
{
    public class ExpressionTripletNode : ExpressionSingleNode
    {
        public ExpressionSingleNode First { get; set; }
        public ExpressionSingleNode Second { get; set; }

        public ExpressionTripletNode(string name, ExpressionSingleNode first, ExpressionSingleNode second) : base(name)
        {
            First = first;
            Second = second;
        }

        public override string ToString()
        {
            return Name + ProcessURI.Separator + First + ProcessURI.Separator + Second;
        }
    }
}