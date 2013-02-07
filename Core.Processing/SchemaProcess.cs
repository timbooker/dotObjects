using dotObjects.Core.Configuration;
using dotObjects.Core.Processing.Schema;
using dotObjects.Core.UI;
using System;

namespace dotObjects.Core.Processing
{
    /// <summary>
    /// Class that creates the representational schema of the application domain objects.
    /// </summary>
    /// <remarks>
    /// The schema of the application domain objects can be viewed as the site map of the application objects. 
    /// By that perspective, the schema contains all the application's classes and, for each class, all the operations that
    /// are available through the dotObjects framework, associated with the RESTfull URI that represents a class to that operation.
    /// 
    /// This schema is intended to be used on the menu rendering process, so the rendered don´t have to worry how to obtain
    /// the available operations of the business classes.
    /// 
    /// [TODO: place a link here poiting to RESTFull URIs on the manual.]
    /// </remarks>
    public class SchemaProcess : Process
    {
        public SchemaProcess(ProcessURI uri)
            : base(uri)
        {
        }

        /// <summary>
        /// Execute the command for the current URI REST query.
        /// and the specified Domain's instance.
        /// </summary>
        /// <param name="domain">The specified Domain's instance</param>
        /// <param name="before"> </param>
        /// <param name="after"> </param>
        /// <returns>An IProcessResponse's instance.</returns>
        public override IProcessResponse Execute(Domain domain, Action<object> before, Action<object> after)
        {
            before(null);
            var response = new SchemaProcessResponse { URI = URI };
            if (domain == null || !CoreSection.Current.IsEntity(domain.Type))
            {
                foreach (EntityAssembly asm in CoreSection.Current.EntityAssemblies){
                    response.Namespaces.Add(new NamespaceSchema(asm.RootNamespace, asm.GetTypes()) { IsRoot = true });                }
            }
            else
            {
                response.Namespaces.Add(new NamespaceSchema(domain.Type.Namespace, new[] { (EntityDomain)domain }));
            }
            after(response);
            return response;
        }
    }
}