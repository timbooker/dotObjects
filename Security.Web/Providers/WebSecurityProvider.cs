using System.Web;

namespace dotObjects.Security.Web.Providers
{
    public sealed class WebSecurityProvider : ModelSecurityProvider
    {
        private const string Key = "WebSecurityProviderToken";

        public override object AuthenticationToken
        {
            get { return HttpContext.Current.Session[Key]; }
            protected set { HttpContext.Current.Session[Key] = value; }
        }

        public new static void Login(string username, string password)
        {
            ModelSecurityProvider.Login(username, password);
        }

        public new static void Logout()
        {
            ModelSecurityProvider.Logout();
        }
    }
}