using System;
using dotObjects.Core.Processing;

namespace dotObjects.Core.Persistence
{
    /// <summary>
    /// The dotObjects Query Between criteria class.
    /// </summary>
    [Serializable]
    public class Between : Criteria
    {
        /// <summary>
        /// Returns a Between criteria instance.
        /// </summary>
        /// <param name="field">The specified field label.</param>
        /// <param name="firstValue">The first instance in range.</param>
        /// <param name="lastValue">The last instance in range.</param>
        public Between(string field, object firstValue, object lastValue)
        {
            if (string.IsNullOrEmpty(field))
                throw new ArgumentNullException("field");
            if (firstValue == null)
                throw new ArgumentNullException("firstValue");
            if (lastValue == null)
                throw new ArgumentNullException("lastValue");
            if (!firstValue.GetType().Equals(lastValue.GetType()))
                throw new ArgumentException(string.Format("The types of firstValue and lastValue"
                                                          +
                                                          "parameters must be the same. The current 'firstValue' type is {0} "
                                                          + "and the 'lastValue' type is {1}.", firstValue.GetType(),
                                                          lastValue.GetType()));

            FirstElement = field;
            SecondElement = firstValue;
            ThirdElement = lastValue;
        }

        /// <summary>
        /// The last instance in range.
        /// </summary>
        public object ThirdElement { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} and {3}", FirstElement, Name, SecondElement, ThirdElement);
        }

        public override string ToURIString()
        {
            return base.ToURIString() + ProcessURI.Separator + ThirdElement;
        }

        protected override string Name
        {
            get { return "Between"; }
        }
    }
}