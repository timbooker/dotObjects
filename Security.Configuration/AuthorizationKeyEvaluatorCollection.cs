using System.Configuration;
using dotObjects.Core.Processing;

namespace dotObjects.Security.Configuration
{
    public class AuthorizationKeyEvaluatorCollection : ConfigurationElementCollection
    {
        protected override string ElementName
        {
            get { return "evaluator"; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new AuthorizationKeyEvaluator();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AuthorizationKeyEvaluator)element).Type;
        }
    }
}