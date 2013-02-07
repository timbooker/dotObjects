using System;
using dotObjects.Core.Persistence;
using dotObjects.Core.UI;
using dotObjects.Core.Processing.Arguments;

namespace dotObjects.Core.Processing
{
    internal class EntityViewProcess : EntityProcess
    {
        public EntityViewProcess(ProcessURI uri)
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
                    before(null);
                    object view = ContextFactory.GetContext(EntityType).Get(argument.Id, EntityType);
                    after(view);
                    return new ViewProcessResponse(new EntityDomain(EntityType, URI, argument.Id, view));
                }
                throw new Exception("Invalid URI argument!");
            }
            catch (Exception ex)
            {
                return new ViewProcessResponse(domain ?? new EntityDomain(EntityType, URI, -1),
                                               (MessageProcessResponse) CreateFailureResponse(ex, null));
            }
        }
    }
}