using System;
using dotObjects.Core.Processing;
using dotObjects.Security.Configuration.Auth;
using dotObjects.Core.Processing.Arguments;
using System.Collections.Generic;
using dotObjects.Security.Configuration.Auth.Evaluators;

namespace dotObjects.Core.UI
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class FilterAttribute : Attribute, IDomainFilter
    {
        private ProcessURI uri;

        public string Expression { get; set; }

        public Domain Domain { get; set; }

        public ProcessURI URI
        {
            get
            {
                if (uri == null)
                {
                    uri = new ProcessURI(Domain.Type.FullName, ProcessBehavior.Query);
                    if (!string.IsNullOrEmpty(Expression))
                    {
                        var expression = Processing.Expression.Parse(Expression, ProcessBehavior.Query);
                        foreach (var evaluator in AuthorizationManager.Evaluators)
                            expression = evaluator.Evaluate(expression);

                        if (Domain.Dependency != null && Domain.Dependency.HasValue)
                        {
                            var dependency = new DynamicAuthorizationKeyEvaluator(Domain.DependsOf, Domain.Dependency.Value);
                            expression = dependency.Evaluate(expression);
                        }

                        var args = new List<object>();
                        foreach (var node in expression)
                            args.AddRange(AuthorizationKeyEvaluator.ExtractValue(node));

                        uri.Argument = ProcessArgument.Parse(uri.Behavior, args.ToArray());
                    }
                }
                return uri;
            }
        }

        public FilterAttribute(string expression)
        {
            Expression = expression;
        }
    }
}