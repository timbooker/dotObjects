using System.Web;
using dotObjects.Logging;

namespace dotObjects.Web.Logging
{
    public class WebLogger : DefaultLogger
    {
        public override void Write(LogType type, string message, params object[] arguments)
        {
            var ip = HttpContext.Current.Request.UserHostAddress;
            var argument = string.Format(message, arguments);
            base.Write(type, "{0}: {1}", ip, argument);
        }
    }
}