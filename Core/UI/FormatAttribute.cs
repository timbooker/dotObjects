using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace dotObjects.Core.UI
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class FormatAttribute : Attribute
    {
        internal const string DynamicPatern = @"\{(?<fieldId>(.*?))\}";
        internal const string NamedGroupPatern = @"\(\?\<(?<name>(.*?))\>\((?<expression>(.*?))\)\)";
        internal const string FormulaKey = "=";

        public FormatAttribute(string expression)
        {
            Expression = expression;
        }

        public FormatAttribute(string expression, bool summarize)
            : this(expression)
        {
            Summarize = summarize;
        }

        public FormatAttribute(string expression, params object[] parameters)
            : this(expression)
        {
            Parameters = parameters;
        }

        public FormatAttribute(string expression, bool summarize, params object[] parameters)
            : this(expression, summarize)
        {
            Parameters = parameters;
        }

        public string Expression { get; set; }

        public bool Summarize { get; set; }

        public object[] Parameters { get; set; }

        public object GetEvaluated(Domain domain, string parameterValue)
        {
            var namedGroupMatch = Regex.Match(parameterValue, NamedGroupPatern);

            if (namedGroupMatch.Success)
            {
                return namedGroupMatch;
            }
            else
            {
                var dynamicMatch = Regex.Match(parameterValue, DynamicPatern);

                if (!parameterValue.StartsWith(FormulaKey) && dynamicMatch.Groups["fieldId"].Success)
                {
                    return domain.Parent.GetChildDomain(dynamicMatch.Groups["fieldId"].Value).Value;
                }
                else if (parameterValue.StartsWith(FormulaKey))
                {
                    return Formulas.FormulaParser.Parse(parameterValue, domain.Parent);
                }
                else
                {
                    return parameterValue;
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public object[] GetEvaluatedParameters(Domain domain)
        {
            var valueList = new List<Object>();

            if (Parameters != null)
            {
                for (var index = 0; index < Parameters.Length; index++)
                {
                    var evaluated = GetEvaluated(domain, Parameters[index].ToString());

                    if (!(evaluated is Match))
                    {
                        valueList.Add(evaluated);
                    }
                }
            }

            return valueList.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetNamedGroupEvaluatedParameterDictionary(Domain domain)
        {
            var valueDictionary = new Dictionary<string, object>();

            if (Parameters != null)
            {
                for (var index = 0; index < Parameters.Length; index++)
                {
                    var parameterValue = Parameters[index].ToString();
                    var namedGroupMatch = Regex.Match(parameterValue, NamedGroupPatern);

                    if (namedGroupMatch.Success)
                    {
                        var name = namedGroupMatch.Groups["name"].Value;
                        var expression = namedGroupMatch.Groups["expression"].Value;

                        var evaluated = GetEvaluated(domain, expression);

                        if (!(evaluated is Match))
                        {
                            valueDictionary.Add(name, evaluated);
                        }
                    }
                }
            }

            return valueDictionary;
        }

        public string GetValue(Domain domain)
        {
            return GetValue(domain, true);
        }

        public string GetValue(Domain domain, bool useDomainValue)
        {
            var arrayList = new ArrayList();
            var expression = domain.Format.Expression;

            if (useDomainValue)
                arrayList.Insert(arrayList.Count, domain.Value);

            foreach (var parameter in GetEvaluatedParameters(domain))
                arrayList.Insert(arrayList.Count, parameter);

            foreach (var item in GetNamedGroupEvaluatedParameterDictionary(domain))
                expression = expression.Replace(string.Concat("<", item.Key, ">"), item.Value.ToString().Trim());

            return string.Format(expression, arrayList.ToArray());
        }

        public string GetFirstExpressionFormat(Domain domain)
        {
            var firstExpressionFormat = Expression.Substring(Expression.IndexOf("{0") + 2, Expression.IndexOf("}", Expression.IndexOf("{0")) - Expression.IndexOf("{0") - 2);

            if (!string.IsNullOrEmpty(firstExpressionFormat))
            {
                firstExpressionFormat = firstExpressionFormat.Substring(1);

                foreach (var item in GetNamedGroupEvaluatedParameterDictionary(domain))
                    firstExpressionFormat = firstExpressionFormat.Replace(string.Concat("<", item.Key, ">"), item.Value.ToString().Trim());
            }

            return firstExpressionFormat;
        }
    }
}
