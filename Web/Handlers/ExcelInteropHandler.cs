using System.Web.SessionState;
using System.Web;
using dotObjects.Core.Processing;
using dotObjects.Web.Serialization;
using dotObjects.Core.UI;
using dotObjects.Web.Rendering;
using dotObjects.Web.Rendering.Helpers;
using System;
using System.Linq;
using System.Configuration;
using Microsoft.Office.Interop.Excel;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;
using dotObjects.Core.Processing.Arguments;

namespace dotObjects.Web.Handlers
{
    public class ExcelInteropHandler : HttpHandlerBase, IRequiresSessionState
    {
        public ExcelInteropHandler(ProcessURI requestedURI)
            : base(requestedURI)
        {
        }

        public override bool IsReusable
        {
            get { return false; }
        }

        public override void ProcessRequest(HttpContext context)
        {
            Domain domain = ViewStateManager.LoadViewState();
            Process.CurrentState = ViewStateManager.LoadProcessState();

            IProcessResponse response = ProcessFactory.Execute(RequestedURI, domain);

            if (response is RedirectProcessResponse)
            {
                context.Response.Redirect(UrlHelper.GetJsonUrl(((RedirectProcessResponse)response).URI));
            }
            else
            {
                //TODO:Add Behavior control (to validate when is Query behavior)
                HybridRenderer renderer = RendererFactory.GetRenderer(RequestedURI, response) as HybridRenderer;
                context.Response.AddHeader("Cache-Control", "no-cache");
                context.Response.AddHeader("Pragma", "no-cache");
                context.Response.AddHeader("Expires", DateTime.Now.ToString());
                context.Response.ContentType = "application/vnd.ms-excel";
                renderer.Process(response, context.Response.Output);

                var argument = response.URI.Argument as QueryArgument;
                var name = string.IsNullOrEmpty(argument.Name) ? renderer.FileName : argument.Name;
                context.Response.AddHeader("Content-Disposition", "attachment; filename=" + name + ".xlsx");
            }
        }
    }
}
