using dotObjects.Core.Processing;
using System;

namespace dotObjects.Core.Persistence
{
    /// <summary>
    /// The dotObjects Query ICriteria base class.
    /// </summary>
    /// <remarks>
    /// The Criteria base class define a contract for all
    /// Criteria classes and allows chaining in Query class.
    /// </remarks>
    [Serializable]
    public abstract class Criteria
    {
        /// <summary>
        /// Creates an And criteria instance.
        /// </summary>
        /// <param name="criteria">The specified criteria.</param>
        /// <returns>An <see cref="And"/> criteria instance.</returns>
        public Criteria And(Criteria criteria)
        {
            return new And(this, criteria);
        }

        /// <summary>
        /// Creates a Between criteria instance.
        /// </summary>
        /// <param name="field">The specified field label.</param>
        /// <param name="firstValue">The first instance in range.</param>
        /// <param name="lastValue">The last instance in range.</param>
        /// <returns>A <see cref="Between"/> criteria instance.</returns>
        public Criteria Between(string field, object firstValue, object lastValue)
        {
            return new Between(field, firstValue, lastValue);
        }

        /// <summary>
        /// Creates a Contains criteria instance.
        /// </summary>
        /// <param name="field">The specified field label.</param>
        /// <param name="value"></param>
        /// <returns>A <see cref="Contains"/> criteria instance.</returns>
        public Criteria Contains(string field, object value)
        {
            return new Contains(field, value);
        }

        /// <summary>
        /// Creates a Equals criteria instance.
        /// </summary>
        /// <param name="field">The specified field label.</param>
        /// <param name="value"></param>
        /// <returns>An <see cref="Equals"/> criteria instance.</returns>
        public Criteria Equals(string field, object value)
        {
            return new Equals(field, value);
        }

        /// <summary>
        /// Creates a GreaterThan criteria instance.
        /// </summary>
        /// <param name="field">The specified field label.</param>
        /// <param name="value"></param>
        /// <returns>A <see cref="GreaterThan"/> criteria instance.</returns>
        public Criteria GreaterThan(string field, object value)
        {
            return new GreaterThan(field, value);
        }

        /// <summary>
        /// Creates a GreaterThanOrEqual criteria instance.
        /// </summary>
        /// <param name="field">The specified field label.</param>
        /// <param name="value"></param>
        /// <returns>A <see cref="GreaterThanOrEqual"/> criteria instance.</returns>
        public Criteria GreaterThanOrEqual(string field, object value)
        {
            return new GreaterThanOrEqual(field, value);
        }

        /// <summary>
        /// Creates an IsNull criteria instance.
        /// </summary>
        /// <param name="field">The specified field label.</param>
        /// <returns>An <see cref="IsNull"/> criteria instance.</returns>
        public Criteria IsNull(string field)
        {
            return new IsNull(field);
        }

        /// <summary>
        /// Creates a LessThan criteria instance.
        /// </summary>
        /// <param name="field">The specified field label.</param>
        /// <param name="value"></param>
        /// <returns>A <see cref="LessThan"/> criteria instance.</returns>
        public Criteria LessThan(string field, object value)
        {
            return new LessThan(field, value);
        }

        /// <summary>
        /// Creates a LessThanOrEqual criteria instance.
        /// </summary>
        /// <param name="field">The specified field label.</param>
        /// <param name="value"></param>
        /// <returns>A <see cref="LessThanOrEqual"/> criteria instance.</returns>
        public Criteria LessThanOrEqual(string field, object value)
        {
            return new LessThanOrEqual(field, value);
        }

        /// <summary>
        /// Creates an Or criteria instance.
        /// </summary>
        /// <param name="criteria">The specified criteria.</param>
        /// <returns>An <see cref="Or"/> criteria instance.</returns>
        public Criteria Or(Criteria criteria)
        {
            return new Or(this, criteria);
        }

        public Criteria In(string field, string values)
        {
            return new In(field, values);
        }

        public object FirstElement { get; set; }
        public object SecondElement { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", FirstElement, Name, SecondElement);
        }

        public virtual string ToURIString()
        {
            var first = FirstElement is Criteria ? ((Criteria) FirstElement).ToURIString() : FirstElement.ToString();
            var second = SecondElement is Criteria ? ((Criteria) SecondElement).ToURIString() : SecondElement.ToString();
            return Name + ProcessURI.Separator + first + ProcessURI.Separator + second;
        }

        protected abstract string Name { get; }
    }
}