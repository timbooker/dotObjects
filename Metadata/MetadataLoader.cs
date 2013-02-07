using dotObjects.Core;
using dotObjects.Core.Processing;
using dotObjects.Core.Configuration;

namespace dotObjects.Metadata
{
    public class MetadataLoader: IInterceptor
    {
        #region IInterceptor Members

        public void OnApplicationStart()
        {
            foreach (EntityAssembly assembly in CoreSection.Current.EntityAssemblies)
                MetadataManager.Load(assembly);
        }

        public void OnProcessExecuting(ProcessURI uri, object obj)
        {
            
        }

        public void OnProcessExecuted(ProcessURI uri, object obj)
        {
            
        }

        #endregion
    }
}
