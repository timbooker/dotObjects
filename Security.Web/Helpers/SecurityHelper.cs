using dotObjects.Web.Rendering;
using dotObjects.Core.Processing;
using dotObjects.Security.Web.Handlers;

namespace dotObjects.Security.Web.Helpers
{
    public class SecurityHelper : RendererHelper
    {
        public SecurityHelper(string name) : base(name)
        {
        }

        public static bool IsAuthenticated
        {
            get { return SecurityProvider.GetInstance().IsAuthenticated; }
        }

        public static object AuthenticationToken
        {
            get { return SecurityProvider.GetInstance().AuthenticationToken; }
        }

        public static ProcessURI LoginURI
        {
            get { return SecurityProvider.GetInstance().LoginURI; }
        }

        public static ProcessURI LogoutURI
        {
            get { return SecurityProvider.GetInstance().LogoutURI; }
        }

        public static bool IsAuthorized(ProcessURI uri)
        {
            return SecurityHandler.IsAuthorized(uri);
        }
    }
}