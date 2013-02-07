using System.Configuration;
using dotObjects.Core.Processing;

namespace dotObjects.Security.Configuration
{
    public class AuthorizationURICollection : ConfigurationElementCollection
    {
        protected override string ElementName
        {
            get { return "uri"; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new AuthorizationURI();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AuthorizationURI) element).Pattern;
        }

        public AuthorizationURI Find(ProcessURI uri)
        {
            foreach (AuthorizationURI authorizationURI in this)
                if (uri.ToString().Contains(authorizationURI.Pattern))
                    return authorizationURI;
            return null;
        }
    }
}