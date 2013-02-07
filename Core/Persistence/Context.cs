using System;
using System.Collections.Generic;
using dotObjects.Core.UI;
using System.Linq;

namespace dotObjects.Core.Persistence
{
    public abstract class Context : IDisposable
    {
        private Dictionary<string, string> settings;

        public Dictionary<string, string> Settings
        {
            get { return settings ?? (settings = new Dictionary<string, string>()); }
        }

        public bool IsReleased { get; set; }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public abstract void Dispose();

        #endregion

        /// <summary>
        /// Get the entity source for specified T type.
        /// </summary>
        /// <typeparam name="T">The type of entity.</typeparam>
        /// <returns>The source of specified entity.</returns>
        public abstract IQueryable<T> GetSource<T>();

        /// <summary>
        /// Begins a Context's transaction.
        /// </summary>
        public abstract void BeginTransaction();

        /// <summary>
        /// Close the current Context's transaction.
        /// </summary>
        public abstract void CloseTransaction();

        /// <summary>
        /// Commits the realized operations in transaction.
        /// </summary>
        public abstract void Commit();

        /// <summary>
        /// Rollback the realized operations in transaction.
        /// </summary>
        public abstract void Rollback();

        /// <summary>
        /// Save (update) the object in Context.
        /// </summary>
        /// <param name="o">A storable object's instance.</param>
        /// <returns>Returns the objectid of the saved instance</returns>
        /// <exception cref="Exception">if some exception occurs during the process.</exception>
        public abstract object Save(object o);

        /// <summary>
        /// 
        /// </summary>
        public abstract void SaveAll();

        /// <summary>
        /// Save (insert) the object in Context.
        /// </summary>
        /// <param name="o">A storable object's instance.</param>
        /// <returns>Returns the objectid of the saved instance</returns>
        /// <exception cref="Exception">if some exception occurs during the process.</exception>
        public abstract object Insert(object o);

        /// <summary>
        /// Delete the object from Context.
        /// </summary>
        /// <param name="o">A storable object's instance.</param>
        /// <returns>Return True if success and False if fail.</returns>
        public abstract bool Delete(object o);

        /// <summary>
        /// Gets the object by its identifier.
        /// </summary>
        /// <param name="id">The unique object identifier.</param>
        /// <param name="target">The target type.</param>
        /// <returns>A storable object's instance.</returns>
        public abstract object Get(object id, Type target);

        /// <summary>
        /// Gets the object's instance in current context.
        /// </summary>
        /// <param name="instance">The entity instance.</param>
        /// <returns>Return the object's instance in current context.</returns>
        public abstract object GetID(object instance);

        /// <summary>
        /// Gets all storable objects that match with 'query' parameter.
        /// </summary>
        /// <param name="query">A Context Query's instance.</param>
        /// <returns>A dictionary whose TKey is the object´s instance and TValue is the object instance.</returns>
        public abstract List<object> Execute(Query query);

        /// <summary>
        /// Execute specific command in the context.
        /// </summary>
        /// <param name="command">The string that contains the command to be executed.</param>
        /// <param name="parameters">The parameters of the command to be executed.</param>
        public abstract void ExecuteCommand(string command, params object[] parameters);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public abstract IEnumerable<T> ExecuteNativeQuery<T>(string query, params object[] parameters);


        /// <summary>
        /// Create a new Context's query for specified target type.
        /// </summary>
        /// <param name="target">The target type.</param>
        /// <returns>A Context Query's instance.</returns>
        public Query CreateQuery(Type target)
        {
            return new Query(target, this) {Skip = -1, Take = -1};
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static string GetExpression(Type target, Criteria criteria)
        {
            var context = ContextFactory.GetContext(target);
            if (context is LinqToSQL.LinqToSqlContext)
            {
                var arguments = new List<object>();
                return ((LinqToSQL.LinqToSqlContext) context).CreateExpression(target, criteria, ref arguments);
            }
            return string.Empty;
        }

        protected static List<string> SplitField(string field)
        {
            return new List<string>(field.Split(new[] {Domain.TypeSeparator}, StringSplitOptions.RemoveEmptyEntries));
        }

        
    }
}