using System;

namespace dotObjects.Core.Persistence
{
    /// <summary>
    /// The dotObjects Query GreaterThanOrEqual criteria class.
    /// </summary>
    [Serializable]
    public class GreaterThanOrEqual : Criteria
    {
        /// <summary>
        /// Returns a GreaterThanOrEqual criteria instance.
        /// </summary>
        /// <param name="field">The specified field label.</param>
        /// <param name="instance">The instance to be compared with field label.</param>
        public GreaterThanOrEqual(string field, object instance)
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
            get { return "GreaterThanOrEqual"; }
        }
    }
}