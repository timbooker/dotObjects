using System;
using dotObjects.Core.Persistence;
using dotObjects.Core.UI;
using dotObjects.Core.Processing.Arguments;

namespace dotObjects.Core.Processing
{
    internal class EntityNewProcess : EntityProcess
    {
        public EntityNewProcess(ProcessURI uri)
            : base(uri)
        {
        }

        public override IProcessResponse Execute(Domain domain, Action<object> before, Action<object> after)
        {
            EntityDomain entity = domain as EntityDomain;
            if (entity != null && CurrentState == ProcessState.ToExecute)
            {
                try
                {
                    IndexedArgument argument = URI.Argument as IndexedArgument;
                    if (argument != null)
                    {
                        object instance = ComplexDomain.CreateEntityInstance(entity, argument.Index);
                        if (instance != null && EntityType.IsInstanceOfType(instance))
                        {
                            before(instance);
                            ContextFactory.GetContext(EntityType).Insert(instance);
                            after(instance);
                            return RedirectResponse;
                        }
                    }
                    else
                        throw new Exception("Invalid URI!");
                }
                catch (Exception ex)
                {
                    return CreateFailureResponse(ex, entity);
                }
            }
            return new InputProcessResponse(entity ?? new EntityDomain(EntityType, URI)) { URI = URI };
        }
    }
}