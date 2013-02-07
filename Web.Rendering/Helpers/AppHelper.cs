using System;
using dotObjects.Core.Configuration;
using dotObjects.Core.Processing;
using dotObjects.Core.Processing.Schema;
using dotObjects.Core.UI;
using dotObjects.Web.Rendering.Configuration;
using System.Threading;
using System.Web;

namespace dotObjects.Web.Rendering.Helpers
{
    public class AppHelper : RendererHelper
    {
        private static SchemaProcessResponse schema;

        public AppHelper(string name)
            : base(name)
        {
        }

        public static SchemaProcessResponse Schema
        {
            get
            {
                var moniker = new object();
                lock (moniker)
                {
                    if (schema == null)
                    {
                        var uri = new ProcessURI("*", ProcessBehavior.Schema);
                        schema = ProcessFactory.Execute(uri, null) as SchemaProcessResponse;
                    }
                }
                return schema;
            }
        }

        public static string ApplicationName
        {
            get { return CoreSection.Current.ApplicationName; }
        }

        public static string RenderingPath
        {
            get { return RenderingSection.Current.Path; }
        }

        public static string CurrentCulture
        {
            get { return Thread.CurrentThread.CurrentUICulture.Name; }
        }

        public static string BaseApplicationPath
        {
            get { return HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath); }
        }

        public static IProcessResponse ExecuteProcess(ProcessURI uri, Domain domain)
        {
            return ProcessFactory.Execute(uri, domain);
        }

        public static bool IsEntity(Type type)
        {
            return CoreSection.Current.IsEntity(type);
        }

        public static Type GetType(string typeName)
        {
            return CoreSection.Current.GetType(typeName);
        }

        public static string GetTypeName(Type type)
        {
            return CoreSection.GetTypeName(type);
        }

        public static EntitySchema GetEntitySchema(EntityDomain domain)
        {
            return Schema.FindEntity(domain.Type);
        }

        public static EntitySchema GetEntitySchema(Type type)
        {
            return GetEntitySchema(CreateDomain(type, new ProcessURI(type.FullName, ProcessBehavior.Query)));
        }

        public static EntityDomain CreateDomain(Type type, ProcessURI uri)
        {
            return new EntityDomain(type, uri);
        }

        public static string GetLocalizationString(Domain domain, params string[] keys)
        {
            if (domain != null)
                return GetLocalizationString(domain.Type, keys);
            return string.Join(Domain.TypeSeparator, keys);
        }

        public static string GetLocalizationString(Type type, params string[] keys)
        {
            if (type != null)
                return CoreSection.Current.GetLocalizationString(type, keys);
            return string.Empty;
        }

        public static bool IsInteractive(Domain domain, ProcessBehavior behavior)
        {
            return ((EntityDomain)domain).IsInteractiveWith(behavior);
        }


    }
}