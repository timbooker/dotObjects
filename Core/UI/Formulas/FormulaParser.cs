using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dotObjects.Core.UI;

namespace dotObjects.Core.UI.Formulas
{
    public static class FormulaParser
    {
        private static List<Formula> formulas = new List<Formula>();

        static FormulaParser()
        {
            formulas.Add(new IIF());
            formulas.Add(new IsNull());
        }

        public static string Parse(string expression, Domain domain)
        {
            foreach (var formula in formulas)
            {
                expression = formula.Parse(expression, domain);
            }
            return expression;
        }
    }
}
