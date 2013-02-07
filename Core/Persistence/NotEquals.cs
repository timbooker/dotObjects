using System;

namespace dotObjects.Core.Persistence
{
    /// <summary>
    /// The dotObjects Query Equals criteria class.
    /// </summary>
    /// <remarks>
    /// It represents the '!=' operator between the
    /// specified field and instance.
    /// </remarks>
    [Serializable]
    public class NotEquals : Criteria
    {
        /// <summary>
        /// Returns a NotEquals criteria instance.
        /// </summary>
        /// <param name="field">The specified field label.</param>
        /// <param name="instance">The instance to be compared with field label.</param>
        public NotEquals(string field, object instance)
        {
            if (string.IsNullOrEmpty(field))
                throw new ArgumentNullException("field");
            FirstElement = field;
            SecondElement = instance;
        }

        protected override string Name
        {
            get { return "NotEquals"; }
        }
    }
}