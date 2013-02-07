using System;

namespace dotObjects.Core.Persistence
{
    /// <summary>
    /// The dotObjects Query Contains criteria class.
    /// </summary>
    [Serializable]
    public class Contains : Criteria
    {
        /// <summary>
        /// Returns a Contains criteria instance.
        /// </summary>
        /// <param name="field">The specified field label.</param>
        /// <param name="instance">The instance to be compared with field label.</param>
        public Contains(string field, object instance)
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
            get { return "Contains"; }
        }
    }
}