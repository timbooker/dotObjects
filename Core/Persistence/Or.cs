using System;

namespace dotObjects.Core.Persistence
{
    /// <summary>
    /// The dotObjects Query Or criteria class.
    /// </summary>
    [Serializable]
    public class Or : Criteria
    {
        /// <summary>
        /// Returns a Or criteria instance.
        /// </summary>
        /// <param name="first">The first criteria.</param>
        /// <param name="second">The second criteria.</param>
        public Or(Criteria first, Criteria second)
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
            get { return "Or"; }
        }

        public override string ToString()
        {
            return string.Format("({0} {1} {2})", FirstElement, Name, SecondElement);
            
        }
    }
}