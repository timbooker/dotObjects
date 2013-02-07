using System;

namespace dotObjects.Core.Persistence
{
    /// <summary>
    /// The dotObjects Query IsNull criteria class.
    /// </summary>
    [Serializable]
    public class IsNotNull : Criteria
    {
        /// <summary>
        /// Returns a IsNotNull criteria instance.
        /// </summary>
        /// <param name="field">The specified field label that must be null.</param>
        public IsNotNull(string field)
        {
            if (string.IsNullOrEmpty(field))
                throw new ArgumentNullException("field");
            FirstElement = field;
        }


        protected override string Name
        {
            get { return "IsNotNull"; }
        }
    }
}