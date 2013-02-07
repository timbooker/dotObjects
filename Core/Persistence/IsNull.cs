using System;

namespace dotObjects.Core.Persistence
{
    /// <summary>
    /// The dotObjects Query IsNull criteria class.
    /// </summary>
    [Serializable]
    public class IsNull : Criteria
    {
        /// <summary>
        /// Returns a IsNull criteria instance.
        /// </summary>
        /// <param name="field">The specified field label that must be null.</param>
        public IsNull(string field)
        {
            if (string.IsNullOrEmpty(field))
                throw new ArgumentNullException("field");
            FirstElement = field;
        }

        protected override string Name
        {
            get { return "isNull"; }
        }
    }
}