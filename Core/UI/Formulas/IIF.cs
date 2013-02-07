using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using dotObjects.Core.UI;

namespace dotObjects.Core.UI.Formulas
{
    internal class IIF : Formula
    {
        internal override string Parse(string expression, Domain domain)
        {
            var matchs = Regex.Matches(expression, @"=IIF\((?<leftSide>(\{|)(?<inLeftSide>(.*?))(\}|)),(?<operator>(.*?)),(?<rightSide>(\{|)(?<inRightSide>(.*?))(\}|)),(?<true>(\{|)(?<inTrue>(.*?))(\}|))(,(?<false>(\{|)(?<inFalse>(.*?))(\}|)|)|)\)");

            foreach (Match match in matchs)
            {
                var @operator = match.Groups["operator"];
                var leftSide = match.Groups["leftSide"];
                var inLeftSide = match.Groups["inLeftSide"];
                var rightSide = match.Groups["rightSide"];
                var inRightSide = match.Groups["inRightSide"];
                var @true = match.Groups["true"];
                var inTrue = match.Groups["inTrue"];
                var @false = match.Groups["false"];
                var inFfalse = match.Groups["inFalse"];

                if (!match.Success || !@operator.Success || !leftSide.Success || !rightSide.Success || !@true.Success)
                    continue;

                var leftSideValue = (leftSide.Value.Equals(inLeftSide.Value)) ? inLeftSide.Value : GetValue(leftSide.Value, domain);
                var rightSideValue = (rightSide.Value.Equals(inRightSide.Value)) ? inRightSide.Value : GetValue(rightSide.Value, domain);
                var trueValue = (@true.Value.Equals(inTrue.Value)) ? inTrue.Value : GetValue(@true.Value, domain);
                var falseValue = (@false.Value.Equals(inFfalse.Value)) ? inFfalse.Value : GetValue(@false.Value, domain);

                switch (@operator.Value)
                {
                    case "Equals":

                        expression = expression.Replace(match.Value, ((leftSideValue == null && rightSideValue == null) || (leftSideValue.Equals(rightSideValue))) ? trueValue : falseValue);

                        break;
                    case "NotEquals":

                        expression = expression.Replace(match.Value, (!(leftSideValue == null && rightSideValue == null) && !(leftSideValue.Equals(rightSideValue))) ? trueValue : falseValue);

                        break;
                }

            }

            return expression;
        }

        private string GetValue(List<Capture> captures, Capture capture, Domain domain)
        {
            var lastCapture = captures.Where(x => x.Index < capture.Index).OrderByDescending(x => x.Index).FirstOrDefault();

            if ((lastCapture != null) && (lastCapture.Value.Equals("{")))
            {
                var childDomain = domain.GetChildDomain(capture.Value);

                if ((childDomain.GetType().Name.Contains("EntityDomain")) && (childDomain.ObjectValue != null))
                {
                    return childDomain.ObjectValue.ToString();
                }

                return (childDomain.Value ?? string.Empty).ToString();
            }

            return capture.Value;
        }
    }
}
