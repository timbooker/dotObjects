using System;
using System.Collections.Generic;
using dotObjects.Core.Persistence;
using dotObjects.Core.UI;
using System.Collections;
using dotObjects.Core.Processing.Arguments;
using dotObjects.Core.Configuration;

namespace dotObjects.Core.Processing
{
    public class QueryProcess : Process
    {
        public QueryProcess(ProcessURI uri)
            : base(uri)
        {
        }

        public override IProcessResponse Execute(Domain domain, Action<object> before, Action<object> after)
        {
            try
            {
                var argument = URI.Argument as QueryArgument;
                if (argument != null)
                {
                    before(null);
                    Query query = ContextFactory.GetContext(EntityType).CreateQuery(EntityType);
                    query.Skip = argument.Skip;
                    query.Take = argument.Take;
                    query.Where = argument.Where;
                    query.OrderBy = argument.OrderBy;
                    var items = query.Execute();
                    after(items);
                    return CreateResponse(EntityType, items, URI);
                }
                throw new Exception("Invalid URI!");
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) ex = ex.InnerException;
                return CreateMessageResponse(ex.Message, MessageProcessResponseType.Failure);
            }
        }

        private MessageProcessResponse CreateMessageResponse(string defaultMessage, MessageProcessResponseType type,
                                                             params string[] keys)
        {
            keys = keys ?? new string[0];
            var msgKeys = new List<string>(keys) {type.ToString()};
            var message = CoreSection.Current.GetLocalizationString(EntityType, msgKeys.ToArray());
            if (message == string.Join(".", msgKeys.ToArray()))
                message = defaultMessage;
            return new QueryProcessResponse(EntityType, new List<Domain>())
                       {
                           Message = message,
                           MessageType = type,
                           URI = URI
                       };
        }

        public static QueryProcessResponse CreateResponse(Type type, IEnumerable items)
        {
            return CreateResponse(type, items, new ProcessURI(type.FullName, ProcessBehavior.Query));
        }

        private static QueryProcessResponse CreateResponse(Type type, IEnumerable items, ProcessURI uri)
        {
            var domains = new DomainCollection();
            var enumerator = items.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var instance = enumerator.Current;
                var entity = new EntityDomain(type, uri)
                {
                    Instance = instance,
                    Value = ContextFactory.GetContext(type).GetID(instance),
                };

                domains.Add(entity);
            }
            return new QueryProcessResponse(type, domains) { URI = uri };
        }
    }
}