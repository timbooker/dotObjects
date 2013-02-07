using System;
using System.Collections.Generic;
using System.Reflection;
using dotObjects.Core.Configuration;
using dotObjects.Core.UI;
using dotObjects.Core.Processing.Arguments;

namespace dotObjects.Core.Processing
{
    internal class EntityProcess : Process
    {
        protected RedirectProcessResponse RedirectResponse
        {
            get
            {
                var uri = new ProcessURI(URI.Entity, ProcessBehavior.Schema);
                var response = ProcessFactory.Execute(uri, null) as SchemaProcessResponse;
                var type = CoreSection.Current.FindType(URI.Entity);
                return new RedirectProcessResponse(response.FindEntity(type).Query.URI);
            }
        }

        public EntityProcess(ProcessURI uri)
            : base(uri)
        {
        }

        public override IProcessResponse Execute(Domain domain, Action<object> before, Action<object> after)
        {
            EntityProcess process = Create(URI);
            return process != null ? process.Execute(domain, before, after) : null;
        }

        private static EntityProcess Create(ProcessURI uri)
        {
            switch (uri.Behavior)
            {
                case ProcessBehavior.New:
                    return new EntityNewProcess(uri);
                case ProcessBehavior.Exec:
                    return new EntityExecProcess(uri);
                case ProcessBehavior.Edit:
                    return new EntityEditProcess(uri);
                case ProcessBehavior.View:
                    return new EntityViewProcess(uri);
                case ProcessBehavior.Delete:
                    return new EntityDeleteProcess(uri);
            }
            return null;
        }

        protected IProcessResponse CreateFailureResponse(Exception ex, EntityDomain domain)
        {
            while (ex.InnerException != null) ex = ex.InnerException;
            MessageProcessResponse message = CreateMessageResponse(ex.Message, MessageProcessResponseType.Failure);
            if (domain == null) return message;
            return new InputProcessResponse(domain, message) { URI = URI };
        }

        private MessageProcessResponse CreateMessageResponse(string defaultMessage, MessageProcessResponseType type,
                                                             params string[] keys)
        {
            keys = keys ?? new string[0];
            List<string> msgKeys = new List<string>(keys) {type.ToString()};
            string message = CoreSection.Current.GetLocalizationString(EntityType, msgKeys.ToArray());
            if (message == string.Join(".", msgKeys.ToArray()))
                message = defaultMessage;
            return new MessageProcessResponse(message, type, EntityType) { URI = URI };
        }

        protected static bool CanProcess(MethodBase method, EntityDomain domain, object domainId)
        {
            ParameterInfo[] parameters = method.GetParameters();
            if (method.IsStatic) return parameters.Length == 0 || domain != null;
            return (parameters.Length == 0 && domainId != null)
                   || (domain != null && domainId != null);
        }
    }
}