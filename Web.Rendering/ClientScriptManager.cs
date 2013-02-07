using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;

namespace dotObjects.Web.Rendering
{
    /// <summary>
    /// This class is responsible for registering and rendering of the javascript and css resources need to render
    /// a HTML page.
    /// </summary>
    /// <remarks>
    /// If a render control needs a reference to a css file or a javascript file, it needs to register this file through this class.
    /// At the page rendering process, the Manager creates all the references in the output. If 2 renderers register the same reference,
    /// the manager will create only 1 reference, at the head section of the page.
    /// </remarks>
    internal static class ClientScriptManager
    {
        private static readonly Dictionary<HttpContext, List<string>> registeredScripts =
            new Dictionary<HttpContext, List<string>>();

        private static readonly Dictionary<HttpContext, List<string>> registeredStyles =
            new Dictionary<HttpContext, List<string>>();

        /// <summary>
        /// Register a CSS file for rendering
        /// </summary>
        /// <param name="styleURI">Full Qualified label of the CSS resource. The resource should be placed as an embedded resource of the assembly.</param>
        public static void RegisterStyle(string styleURI)
        {
            Register(styleURI, registeredStyles);
        }

        /// <summary>
        /// Register a javascript file for rendering
        /// </summary>
        /// <param name="scriptUri">Full Qualified label of the script resource. The resource should be placed as an embedded resource of the assembly.</param>
        public static void RegisterScript(string scriptUri)
        {
            Register(scriptUri, registeredScripts);
        }

        private static void Register(string uri, Dictionary<HttpContext, List<string>> map)
        {
            HttpContext current = HttpContext.Current;
            List<string> currentMap;
            if (map.ContainsKey(current))
                currentMap = map[current];
            else
            {
                currentMap = new List<string>();
                map[current] = currentMap;
            }

            if (!currentMap.Contains(uri))
                currentMap.Add(uri);
        }

        public static void Render(HtmlTextWriter writer)
        {
            try
            {
                foreach (string uri in registeredScripts[HttpContext.Current])
                {
                    writer.WriteLine("<script src=\"{0}\"></script>", uri);
                }
                foreach (string uri in registeredStyles[HttpContext.Current])
                {
                    writer.WriteLine("<link rel=\"stylesheet\" href=\"{0}\" />", uri); //type=\"text/css\" />",uri);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}