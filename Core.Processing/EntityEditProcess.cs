using System;
using System.Reflection;
using dotObjects.Core.Persistence;
using dotObjects.Core.UI;
using dotObjects.Core.Processing.Arguments;

namespace dotObjects.Core.Processing
{
    internal class EntityEditProcess : EntityProcess
    {
        public EntityEditProcess(ProcessURI uri)
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
                    Context ctx = ContextFactory.GetContext(EntityType);
                    object edit = ctx.Get(argument.Id, EntityType);

                    EntityDomain entity = domain as EntityDomain;
                    if (entity != null)
                    {
                        before(edit);
                        foreach (Domain d in entity.Domains)
                        {
                            if (!d.Visible) continue;
                            ComplexDomain.ConvertValue(d);
                            PropertyInfo p = EntityType.GetProperty(d.ID);
                            p.SetValue(edit, d.ObjectValue, null);
                        }
                        ctx.Save(edit);
                        after(edit);
                        return RedirectResponse;
                    }
                    return new InputProcessResponse(new EntityDomain(EntityType, URI, argument.Id, edit)) { URI = URI };
                }
                throw new Exception("Invalid URI!");
            }
            catch (Exception ex)
            {
                return CreateFailureResponse(ex, (EntityDomain)domain ?? new EntityDomain(EntityType, URI));
            }
        }
    }
}