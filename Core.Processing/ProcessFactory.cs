using dotObjects.Core.UI;
using dotObjects.Core.Configuration;
using System.Collections.Generic;
namespace dotObjects.Core.Processing
{
    /// <summary>
    /// Responsible for locate process using a REST query.
    /// </summary>
    public static class ProcessFactory
    {

        /// <summary>
        /// Executes the supplied <see cref="uri"/>.
        /// </summary>
        /// <param name="uri">Specifies the <see cref="ProcessURI"/> to be executed.</param>
        /// <param name="domain">Specifies the <see cref="Domain"/> with the information necessary to execute.</param>
        /// <returns>The <see cref="IProcessResponse"/> of the executed process.</returns>
        public static IProcessResponse Execute(ProcessURI uri, Domain domain)
        {
            List<IInterceptor> interceptors = CoreSection.Current.GetInteceptors();
            return GetProcess(uri).Execute(domain, 
                o => interceptors.ForEach(i => i.OnProcessExecuting(uri, o)),
                o => interceptors.ForEach(i => i.OnProcessExecuted(uri, o)));
        }

        /// <summary>
        /// Get the process specified by ProcessURI uri.
        /// </summary>
        /// <param name="uri">A ProcessURI uri.</param>
        /// <returns>A Process' instance.</returns>
        private static Process GetProcess(ProcessURI uri)
        {
            switch (uri.Behavior)
            {
                case ProcessBehavior.Schema:
                    return new SchemaProcess(uri);
                case ProcessBehavior.Service:
                    return new ServiceProcess(uri);
                case ProcessBehavior.Workflow:
                    return new WorkflowProcess(uri);
                case ProcessBehavior.Query:
                    return new QueryProcess(uri);
                case ProcessBehavior.Index:
                    return new IndexProcess(uri);
                case ProcessBehavior.Session:
                    return new SessionProcess(uri);
                default:
                    return new EntityProcess(uri);
            }
        }
    }
}