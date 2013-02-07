using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using dotObjects.Core.Processing;
using dotObjects.Core.UI;
using dotObjects.Web.Rendering.Configuration;

namespace dotObjects.Web.Rendering.Helpers
{
    public class UrlHelper : RendererHelper
    {
        public const string PreviousURLKey = "dotObjectsPreviousUrl";

        public const string ImageExtension = ".image";
        public const string JsonExtension = ".json";
        public const string ViewExtension = ".aspx";
        public const string ExcelHtmlExtension = ".aspxls";
        public const string ExcelInteropExtension = ".aspxlsx";
        public const string PdfExtension = ".aspxpdf";
        public const string PartialViewExtension = ".aspxp";

        private static string baseUrl;

        public UrlHelper(string name)
            : base(name)
        {
        }

        public static List<string> AllExtensions
        {
            get
            {
                return new List<string>
                {
                    ImageExtension,
                    JsonExtension,
                    ViewExtension,
                    ExcelHtmlExtension,
                    ExcelInteropExtension,
                    PdfExtension,
                    PartialViewExtension
                };
            }
        }

        public static string DefaultExtension
        {
            get { return ViewExtension; }
        }

        public static string GetUrl(ProcessURI uri, string extension)
        {
            if (uri.Behavior == ProcessBehavior.Index)
                return BaseUrl;
            return BaseUrl + uri + extension;
        }

        public static string BaseUrl
        {
            get
            {
                if (string.IsNullOrEmpty(baseUrl))
                {
                    var request = HttpContext.Current.Request;

                    var scheme = request.Url.Scheme + "://";
                    var uri = request.Url.AbsoluteUri.Replace(scheme, string.Empty);
                    var path = request.ApplicationPath ?? "/";
                    baseUrl = scheme + uri.Substring(0, uri.IndexOf(path, StringComparison.Ordinal)) + path + (path.Equals(ProcessURI.Separator) ? string.Empty : ProcessURI.Separator);
                }
                return baseUrl;
            }
        }

        public static string GetImageUrl(Domain domain)
        {
            return GetUrl(new ProcessURI(domain.Parent.ID, ProcessBehavior.View, domain.Parent.Value + ProcessURI.Separator + domain.ID), ImageExtension);
        }

        public static string GetViewUrl(ProcessURI uri)
        {
            return GetUrl(uri, ViewExtension);
        }

        public static string GetPartialViewUrl(ProcessURI uri)
        {
            return GetUrl(uri, PartialViewExtension);
        }

        public static string GetJsonUrl(ProcessURI uri)
        {
            return GetUrl(uri, JsonExtension);
        }

        public static string GetAbsolutePath(string relativePath)
        {
            if (relativePath.StartsWith("#/"))
                relativePath = relativePath.Replace("#/", "~/" + RenderingSection.Current.Path + "/");
            return new Control().ResolveUrl(relativePath);
        }

        public static string FromUrl(string path, string extension)
        {
            path = !string.IsNullOrEmpty(BaseUrl) ? path.Replace(BaseUrl, string.Empty) : path;
            path = path.StartsWith(ProcessURI.Separator) ? path.Substring(1) : path;
            if (!string.IsNullOrEmpty(extension) && path.Length > 0 && path.Contains(extension))
                path = path.Replace(extension, string.Empty);
            return path;
        }

        public static string FromUrl(string path)
        {
            if (path.Contains(PartialViewExtension))
                return FromUrl(path, PartialViewExtension);
            if (path.Contains(ViewExtension))
                return FromUrl(path, ViewExtension);
            if (path.Contains(ImageExtension))
                return FromUrl(path, ImageExtension);
            if (path.Contains(JsonExtension))
                return FromUrl(path, JsonExtension);
            if (path.Contains(ExcelHtmlExtension))
                return FromUrl(path, ExcelHtmlExtension);
            if (path.Contains(ExcelInteropExtension))
                return FromUrl(path, ExcelInteropExtension);
            if (path.Contains(PdfExtension))
                return FromUrl(path, PdfExtension);
            throw new ArgumentException(string.Format("The path {0} is invalid!", path));
        }

        public static string CurrentUrl
        {
            get { return HttpContext.Current.Request.Url.AbsolutePath; }
        }

        public static string CurrentExtension
        {
            get { return ExtensionOf(CurrentUrl); }
        }

        public static string RegisterPreviousUrl(ProcessURI uri)
        {
            return string.Format("<input type='hidden' id='{0}' name='{0}' value='{1}' />", PreviousURLKey, uri.PreviousURI);
        }

        public static string ExtensionOf(string url)
        {
            if (url.EndsWith(ExcelHtmlExtension))
                return ExcelHtmlExtension;
            if (url.EndsWith(ExcelInteropExtension))
                return ExcelInteropExtension;
            if (url.EndsWith(PdfExtension))
                return PdfExtension;
            if (url.EndsWith(PartialViewExtension))
                return PartialViewExtension;
            if (url.EndsWith(ViewExtension))
                return ViewExtension;
            if (url.EndsWith(ImageExtension))
                return ImageExtension;
            if (url.EndsWith(JsonExtension))
                return JsonExtension;
            return string.Empty;
        }

        public static string Decode(string url)
        {
            return HttpContext.Current.Server.UrlDecode(url);
        }

        public static string Encode(string url)
        {
            return HttpContext.Current.Server.UrlEncode(url);
        }
    }
}