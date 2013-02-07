using System.Linq.Expressions;

namespace dotObjects.Core.Persistence.LinqToSQL
{
    internal class DynamicOrdering
    {
        public Expression Selector;
        public bool Ascending;
    }
}