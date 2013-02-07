using System;
using System.Collections;
using System.Reflection;
using dotObjects.Core.Configuration;
using dotObjects.Core.Persistence;
using dotObjects.Core.UI;
using System.Collections.Generic;
using dotObjects.Core.Processing.Arguments;
using dotObjects.Logging;

namespace dotObjects.Core.Processing
{
    internal class EntityExecProcess : EntityProcess
    {
        public EntityExecProcess(ProcessURI uri)
            : base(uri)
        {
        }

        public override IProcessResponse Execute(Domain domain, Action<object> before, Action<object> after)
        {

            try
            {
                var argument = URI.Argument as ExecutionArgument;
                if (argument != null)
                {
                    object instance = null;
                    var entity = domain as EntityDomain;
                    if (CurrentState == ProcessState.ToExecute)
                    {
                        var methods = new List<MethodBase>(EntityType.GetMethods());
                        var method = ComplexDomain.GetMethodByName(methods, argument.Name, argument.Index);
                        var context = ContextFactory.GetContext(EntityType);
                        try
                        {
                            instance = (method.IsStatic) ? null : context.Get(argument.Id, EntityType);
                            if (CanProcess(method, entity, argument.Id))
                            {
                                LogManager.Logger.Write(LogType.Info, "Executiong {0}", URI);
                                before(instance);
                                var arguments = ComplexDomain.GetMethodParameters(method, entity);
                                var response = method.Invoke(instance, arguments);
                                if (instance != null) context.Save(instance);
                                context.SaveAll();
                                after(instance);
                                LogManager.Logger.Write(LogType.Info, "Executiong {0}", URI);

                                if (response != null)
                                {
                                    var returnResponse = CreateReturnResponse(method, response);
                                    if (returnResponse != null) return returnResponse;
                                }

                                return URI.PreviousURI != null
                                    ? new RedirectProcessResponse(URI.PreviousURI) 
                                    : RedirectResponse;
                            }
                        }
                        catch (Exception ex)
                        {
                            context.Rollback();
                            LogManager.Logger.Write(LogType.Error, "Exception occurred executing {0}", URI, ex);
                            return CreateFailureResponse(ex, entity);
                        }
                        finally
                        {
                            context.CloseTransaction();
                        }
                    }
                    return new InputProcessResponse(entity ?? new EntityDomain(EntityType, URI, argument.Id, instance)) { URI = URI };
                }
                throw new Exception("Invalid URI!");
            }
            catch (Exception ex)
            {
                return CreateFailureResponse(ex, (EntityDomain)domain ?? new EntityDomain(EntityType, URI));
            }
        }

        private IProcessResponse CreateReturnResponse(MethodBase method, object response)
        {
            if (response is RedirectProcessResponse)
                return (IProcessResponse)response;

            Type responseType = response.GetType();
            if (CoreSection.IsSystemType(responseType))
                return new MessageProcessResponse(response.ToString(), MessageProcessResponseType.Normal, null);

            if (ComplexDomain.IsGenericEntityCollection(responseType))
            {
                Type entityType = ComplexDomain.GetEntityTypeFromCollection(responseType);
                if (entityType != null)
                    return QueryProcess.CreateResponse(entityType, (IEnumerable)response);
            }

            if (CoreSection.Current.IsEntity(responseType))
            {
                var responseURI = new ProcessURI(responseType.FullName, ProcessBehavior.View);
                return new ViewProcessResponse(new EntityDomain(responseType, responseURI, null, response));
            }

            var uri = new ProcessURI(EntityType.FullName, ProcessBehavior.View);
            return new ViewProcessResponse(new ComplexDomain(responseType, uri, response, method.Name));
        }
    }
}