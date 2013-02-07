using System.Collections.Generic;
using dotObjects.Core.Processing;
using dotObjects.Security.Configuration.Auth.Evaluators;
using System;

namespace dotObjects.Security.Configuration.Auth
{
    public static class AuthorizationManager
    {
        public static readonly List<AuthorizationKeyEvaluator> Evaluators;

        static AuthorizationManager()
        {
            Evaluators = new List<AuthorizationKeyEvaluator>
            {
                new MeAuthorizationKeyEvaluator(),
                new TodayAuthorizationKeyEvaluator()
            };

            foreach (var evalConfiguration in SecuritySection.Current.Authorization.Evaluators)
            {
                var evaluator = (Configuration.AuthorizationKeyEvaluator)evalConfiguration;
                Evaluators.Add((AuthorizationKeyEvaluator)Activator.CreateInstance(evaluator.UnderlineType));
            }
        }

        public static bool IsAuthorized(ProcessURI uri)
        {
            AuthorizationURI authorizationURI = SecuritySection.Current.Authorization.URIs.Find(uri);
            if (authorizationURI != null)
                return IsAuthorized(authorizationURI.Rules.ToList());
            return true;
        }

        private static bool IsAuthorized(List<AuthorizationRule> rules)
        {
            if (rules.Count > 0)
            {
                AuthorizationRule rule = rules[0];
                if (IsAuthorized(rule))
                    return true;
                rules.Remove(rule);
                return IsAuthorized(rules);
            }
            return false;
        }

        private static bool IsAuthorized(AuthorizationRule rule)
        {
            SecurityProvider provider = SecurityProvider.GetInstance();
            if (rule.DenyEveryone)
                return false;
            if (rule.AllowEveryone || rule.AllowAnonymous)
                return true;
            if (provider.IsAuthenticated && rule.DenyAnonymous)
                return true;
            if (provider.IsAuthenticated && provider.IsInRoles(rule.Roles))
                return rule.Type == AuthorizationRuleType.Allow;
            return false;
        }

        public static ProcessURI Evaluate(ProcessURI uri)
        {
            if (IsAuthorized(uri))
            {
                if (uri.Behavior == ProcessBehavior.Query)
                {
                    Expression expression = GetRuleExpression(uri) ??
                                            Expression.Parse(uri.Argument.ToString(), uri.Behavior) ??
                                            Expression.Empty;

                    foreach (AuthorizationKeyEvaluator evaluator in Evaluators)
                        uri = evaluator.Evaluate(expression, uri);
                }
            }
            return uri;
        }

        public static IProcessResponse Evaluate(ProcessURI uri, IProcessResponse response)
        {
            if (IsAuthorized(uri))
            {
                Expression expression = GetRuleResponseExpression(uri) ?? Expression.Empty;
                if (expression != Expression.Empty)
                {
                    var expressionURI = ProcessURI.Parse(expression.ToString());
                    foreach (AuthorizationKeyEvaluator evaluator in Evaluators)
                        expressionURI = evaluator.Evaluate(expression, expressionURI);
                    //TODO: Apply new URI to the response subset
                }
            }
            return response;
        }

        private static Expression GetRuleResponseExpression(ProcessURI uri)
        {
            //TODO: Implement
            return null;
        }

        public static Expression GetRuleExpression(ProcessURI uri)
        {
            var authorizationRule = GetRule(uri);
            if (authorizationRule != null)
                return Expression.Parse(authorizationRule.Expression, uri.Behavior);
            return null;
        }

        public static AuthorizationRule GetRule(ProcessURI uri)
        {
            AuthorizationURI authorizationURI = SecuritySection.Current.Authorization.URIs.Find(uri);
            if (authorizationURI != null)
                return GetRule(authorizationURI);
            return null;
        }

        public static AuthorizationRule GetRule(AuthorizationURI authorizationURI)
        {
            foreach (AuthorizationRule rule in authorizationURI.Rules)
                if (IsAuthorized(rule))
                    return rule;
            return null;
        }
    }
}