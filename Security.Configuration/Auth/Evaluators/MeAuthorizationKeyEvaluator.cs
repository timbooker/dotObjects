using dotObjects.Core.Configuration;

namespace dotObjects.Security.Configuration.Auth.Evaluators
{
    internal class MeAuthorizationKeyEvaluator : AuthorizationKeyEvaluator
    {
        public override string KeyName
        {
            get { return "me"; }
        }

        public override object KeyValue
        {
            get
            {
                SecurityProvider provider = SecurityProvider.GetInstance();
                object token = provider.AuthenticationToken;
                if (provider.IsAuthenticated && CoreSection.Current.IsEntity(token.GetType()))
                    return token;
                return null;
            }
        }
    }
}