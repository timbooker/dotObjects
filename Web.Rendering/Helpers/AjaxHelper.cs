using dotObjects.Web.Rendering.Handlers;

namespace dotObjects.Web.Rendering.Helpers
{
    /// <summary>
    /// The AjaxHelper class allows to implement Ajax based calls in dotObjects Rendering layer.
    /// </summary>
    public class AjaxHelper : RendererHelper
    {
        /// <summary>
        /// Create an instance of AjaxHelper class.
        /// </summary>
        public AjaxHelper(string name) : base(name)
        {
        }

        /// <summary>
        /// Register the base JavaScript files for Ajax based calls.
        /// </summary>
        /// <returns>Returns the string with script tag and its script sources.</returns>
        public static string RegisterScripts()
        {
            return string.Format("{0}\n{1}",
                                 RegisterScript("dotObjects.Web.Rendering.Helpers.JavaScript.JQuery.js"),
                                 RegisterAjaxHelperScript());
        }

        /// <summary>
        /// Register the specified resource's script.
        /// </summary>
        /// <param name="resourceName">The resource label to register.</param>
        /// <returns>Returns the string with script tag and the resource script location.</returns>
        private static string RegisterScript(string resourceName)
        {
            string relativePath = ResourceHandler.GetResourceUrl(resourceName);
            string absolutPath = UrlHelper.GetAbsolutePath(relativePath);

            return string.Format("<script src='{0}' type='{1}'></script>", absolutPath, "text/javascript");
        }

        private static string RegisterAjaxHelperScript()
        {
            return string.Format("<script type='{0}'>{1}</script>", "text/javascript",
                                 @"var AjaxHelper = 
                    {
                        Process : function(uri)
                        {
                            var url = """ +
                                 UrlHelper.BaseUrl + @""" + uri + """ + UrlHelper.JsonExtension +
                                 @""";
                            var options = { type: 'POST', url : url, dataType : 'json' } 
                            if(arguments.length == 2)
                                $.ajax(jQuery.extend(options, { success : arguments[1], error : arguments[1]}));
                            else if(arguments.length == 3)
                                $.ajax(jQuery.extend(options, { data : arguments[1], success : arguments[2], error : arguments[2]}));
                        }
                    };");
        }
    }
}