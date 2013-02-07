using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using dotObjects.Core.UI;

namespace dotObjects.Core.UI.Formulas
{
    internal class IsNull : Formula
    {
        internal override string Parse(string expression, Domain domain)
        {
            var matchs = Regex.Matches(expression, @"=IsNull\((?<comparator>(\{|)(?<inComparator>(.*?))(\}|)),(?<true>(\{|)(?<inTrue>(.*?))(\}|))\)");

            foreach (Match match in matchs)
            {
                var comparator = match.Groups["comparator"];
                var inComparator = match.Groups["inComparator"];
                var @true = match.Groups["true"];
                var inTrue = match.Groups["inTrue"];

                if (!match.Success || !comparator.Success || !@true.Success)
                    continue;

                var trueValue = string.Empty;
                var comparatorValue = (comparator.Value.Equals(inComparator.Value)) ? inComparator.Value : GetValue(comparator.Value, domain);

                if (string.IsNullOrEmpty(comparatorValue))
                    trueValue = (@true.Value.Equals(inTrue.Value)) ? inTrue.Value : GetValue(@true.Value, domain);

                expression = expression.Replace(match.Value, string.IsNullOrEmpty(comparatorValue) ? trueValue : comparatorValue);
            }

            return expression;
        }
    }
}
