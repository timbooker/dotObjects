using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using dotObjects.Core.UI;
using dotObjects.Core.Processing;
using dotObjects.Session;
using dotObjects.Web.Rendering.Helpers;

namespace dotObjects.Web.Rendering
{
    public static class ViewStateManager
    {
        public const string FieldName = "dotObjectsState";

        public static string Serialize(Domain domain)
        {
            if (domain != null)
            {
                MemoryStream domainStream = new MemoryStream();
                BinaryFormatter fmt = new BinaryFormatter();
                fmt.Serialize(domainStream, domain);
                return Convert.ToBase64String(domainStream.GetBuffer());
            }
            return string.Empty;
        }

        private static Domain Deserialize(string serialized)
        {
            byte[] viewStateData = Convert.FromBase64String(serialized);
            if (viewStateData.Length == 0) return null;
            BinaryFormatter fmt = new BinaryFormatter();
            return (Domain) fmt.Deserialize(new MemoryStream(viewStateData));
        }

        public static Domain LoadViewState()
        {
            HttpContext context = HttpContext.Current;
            if(context.Request.Params[FieldName] != null)
            {
                var key = context.Request.Params[FieldName];
                ISessionState session = SessionManager.Session;
                if (session.Contains(key) && session[key] != null)
                {
                    Domain responseDomain = Deserialize(session[key].ToString());
                    if (responseDomain != null)
                    {
                        responseDomain.Fill(GetRequestParameters(HttpContext.Current.Request));
                        return responseDomain;
                    }
                }
            }
            return null;
        }

        public static ProcessState LoadProcessState()
        {
            HttpContext context = HttpContext.Current;
            if (context.Request.Params[Process.StateFieldName] != null)
            {
                return (ProcessState) Enum.Parse(typeof (ProcessState), context.Request.Params[Process.StateFieldName]);
            }
            return ProcessState.ToExecute;
        }

        private static Dictionary<string, object> GetRequestParameters(HttpRequest request)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            foreach (string key in request.Params.Keys)
                parameters.Add(key, request[key]);
            foreach (string key in request.Files.Keys)
                parameters.Add(key, request.Files[key].InputStream);
            return parameters;
        }
    }
}