using System;
using System.Linq;
using System.Collections.Generic;
using dotObjects.Core.Serialization;
using dotObjects.Core.Configuration;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections;

namespace dotObjects.Core.UI
{
    public static class DomainExtensions
    {
        public static IEnumerable<GroupResult> GroupByMany(this IEnumerable<Domain> elements, params string[] groupSelectors)
        {
            var selectors = new List<Func<Domain, object>>(groupSelectors.Length);
            foreach (var selector in groupSelectors)
            {
                LambdaExpression lambdaExpression = dotObjects.Core.Persistence.LinqToSQL.DynamicExpression.ParseLambda(typeof(Domain), typeof(object), string.Format("GetChildDomainValue(\"{0}\")", selector));
                selectors.Add((Func<Domain, object>)lambdaExpression.Compile());
            }
            return elements.GroupByMany(selectors.ToArray());
        }

        public static IEnumerable<GroupResult> GroupByMany(this IEnumerable<Domain> elements, params Func<Domain, object>[] groupSelectors)
        {
            if (groupSelectors.Length > 0)
            {
                var selector = groupSelectors.First();
                var nextSelectors = groupSelectors.Skip(1).ToArray(); //reduce the list recursively until zero
                return
                    elements.GroupBy(selector).Select(
                        g => new GroupResult
                        {
                            Key = g.Key,
                            Count = g.Count(),
                            Items = g,
                            SubGroups = g.GroupByMany(nextSelectors)
                        });
            }
            else
                return null;
        }
    }

    public class GroupResult
    {
        public object Key { get; set; }
        public int Count { get; set; }
        public IEnumerable Items { get; set; }
        public IEnumerable<GroupResult> SubGroups { get; set; }
        public override string ToString() { return string.Format("{0} ({1})", Key, Count); }
    }
}