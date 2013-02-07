using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dotObjects.Core.UI;
using System.Text.RegularExpressions;

namespace dotObjects.Core.UI.Formulas
{
    internal abstract class Formula
    {
        internal abstract string Parse(string expression, Domain domain);

        protected string GetValue(string expression, Domain domain)
        {
            var matchs = Regex.Matches(expression, @"\{(?<field>(.*?))\}");

            foreach (Match match in matchs)
            {
                var field = match.Groups["field"];

                if (!string.IsNullOrEmpty(field.Value))
                    expression = expression.Replace(match.Value, GetChildDomainValue(field.Value, domain));
            }

            return expression;
        }

        protected string GetChildDomainValue(string id, Domain domain)
        {
            var childDomain = domain.GetChildDomain(id);

            if (childDomain != null)
            {

                if (childDomain.GetType().Name.Contains("EntityDomain"))
                {
                    if (childDomain.ObjectValue != null)
                        return childDomain.ObjectValue.ToString();
                    else
                        return null;
                }

                return (childDomain.Value != null) ? childDomain.Value.ToString() : null;
            }

            return null;
        }
    }
}
