using System;

namespace dotObjects.Core.Persistence
{
    /// <summary>
    /// The dotObjects Query Contains criteria class.
    /// </summary>
    [Serializable]
    public class In : Criteria
    {
        /// <summary>
        /// Returns a In criteria instance.
        /// </summary>
        /// <param name="field">The specified field label.</param>
        /// <param name="value">The value to be compared with field.</param>
        public In(string field, object value)
        {
            if (string.IsNullOrEmpty(field))
                throw new ArgumentNullException("field");
            if (value == null)
                throw new ArgumentNullException("value");
            FirstElement = field;
            SecondElement = value;
        }

        protected override string Name
        {
            get { return "In"; }
        }
    }
}