using System;

namespace dotObjects.Core.Persistence
{
    /// <summary>
    /// The dotObjects Query And criteria class.
    /// </summary>
    [Serializable]
    public class And : Criteria
    {
        /// <summary>
        /// Returns a And criteria instance.
        /// </summary>
        /// <param name="first">The first criteria.</param>
        /// <param name="second">The second criteria.</param>
        public And(Criteria first, Criteria second)
        {
            if (first == null)
                throw new ArgumentNullException("first");
            if (second == null)
                throw new ArgumentNullException("second");
            FirstElement = first;
            SecondElement = second;
        }

        protected override string Name
        {
            get { return "And"; }
        }

        public override string ToString()
        {
            return string.Format("({0} {1} {2})", FirstElement, Name, SecondElement);
        }
    }
}