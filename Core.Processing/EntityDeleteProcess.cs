using System;
using dotObjects.Core.Persistence;
using dotObjects.Core.UI;
using dotObjects.Core.Processing.Arguments;

namespace dotObjects.Core.Processing
{
    internal class EntityDeleteProcess : EntityProcess
    {
        public EntityDeleteProcess(ProcessURI uri)
            : base(uri)
        {
        }

        public override IProcessResponse Execute(Domain domain, Action<object> before, Action<object> after)
        {
            try
            {
                IdentifiedArgument argument = URI.Argument as IdentifiedArgument;
                if (argument != null)
                {
                    object obj = ContextFactory.GetContext(EntityType).Get(argument.Id, EntityType);
                    if (obj == null)
                        throw new Exception(string.Format("Object of type {0} not found!", EntityType.Name));
                    before(obj);
                    ContextFactory.GetContext(EntityType).Delete(obj);
                    after(obj);
                    return RedirectResponse;
                }
                throw new Exception("Invalid URI argument!");
            }
            catch (Exception ex)
            {
                return CreateFailureResponse(ex, null);
            }
        }
    }
}