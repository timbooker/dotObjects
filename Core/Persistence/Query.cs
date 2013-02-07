using System;
using System.Collections.Generic;

namespace dotObjects.Core.Persistence
{
    /// <summary>
    /// The Persistence Query class
    /// </summary>
    /// <remarks>
    /// The Query class is responsible to mediate between 
    /// the Processing and Persistence layers. Abstracting 
    /// the logic inside a context.
    /// </remarks>
    public sealed class Query
    {
        /// <summary>
        /// 
        /// </summary>
        public const string SkipKey = "Skip";
        /// <summary>
        /// 
        /// </summary>
        public const string TakeKey = "Take";
        /// <summary>
        /// 
        /// </summary>
        public const string LiveKey = "Live";
        /// <summary>
        /// 
        /// </summary>
        public const string NameKey = "QName";
        /// <summary>
        /// 
        /// </summary>
        public const string ProjectionKey = "Projection";

        /// <summary>
        /// Create a Query instance.
        /// </summary>
        /// <exception cref="ArgumentNullException">If the target parameter is null.</exception>
        /// <exception cref="ArgumentNullException">If the context parameter is null.</exception>
        /// <param name="target">The model type to performs the query.</param>
        /// <param name="context">The associated context to current query.</param>
        public Query(Type target, Context context)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            Target = target;

            if (context == null)
                throw new ArgumentNullException("context");
            Context = context;
        }

        /// <summary>
        /// Gets or sets the quantity to skip in this query.
        /// </summary>
        public int Skip { get; set; }

        /// <summary>
        /// Gets or sets the quantity to take in this query.
        /// </summary>
        public int Take { get; set; }

        /// <summary>
        /// Gets or sets if current query is a live query.
        /// </summary>
        public bool Live { get; set; }

        /// <summary>
        /// Gets or sets the model type target.
        /// </summary>
        public Type Target { get; private set; }

        /// <summary>
        /// Gets or sets the Criteria for the current query.
        /// </summary>
        public Criteria Where { get; set; }

        /// <summary>
        /// Gets or sets the ordering for the current query.
        /// </summary>
        public OrderBy[] OrderBy { get; set; }

        /// <summary>
        /// Gets the associated context to current query.
        /// </summary>
        private Context Context { get; set; }

        /// <summary>
        /// Performs the query in the associated context and returns a list of result.
        /// </summary>
        /// <returns>A List of <see cref="Target">Target</see>'s instances.</returns>
        public List<object> Execute()
        {
            return Context.Execute(this);
        }

        /// <summary>
        /// Performs the query in the associated context and return a single result.
        /// </summary>
        /// <returns>A <see cref="Target">Target</see>'s instance.</returns>
        public object ExecuteSingleResult()
        {
            List<object> items = Execute();
            if (items.Count > 0)
                return items[0];
            return null;
        }
    }
}