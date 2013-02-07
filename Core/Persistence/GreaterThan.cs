using System;

namespace dotObjects.Core.Persistence
{
    /// <summary>
    /// The dotObjects Query GreaterThan criteria class.
    /// </summary>
    [Serializable]
    public class GreaterThan : Criteria
    {
        /// <summary>
        /// Returns a GreaterThan criteria instance.
        /// </summary>
        /// <param name="field">The specified field label.</param>
        /// <param name="instance">The instance to be compared with field label.</param>
        public GreaterThan(string field, object instance)
        {
            if (string.IsNullOrEmpty(field))
                throw new ArgumentNullException("field");
            if (instance == null)
                throw new ArgumentNullException("instance");
            FirstElement = field;
            SecondElement = instance;
        }

        protected override string Name
        {
            get { return "GreaterThan"; }
        }
    }
}