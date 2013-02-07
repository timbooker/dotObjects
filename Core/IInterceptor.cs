using dotObjects.Core.Processing;

namespace dotObjects.Core
{
    /// <summary>
    /// 
    /// </summary>
    public interface IInterceptor
    {
        /// <summary>
        /// Defines the method to be implemented and runs on applications start procedure.
        /// </summary>
        void OnApplicationStart();

        /// <summary>
        /// Defines the method to be implemented and runs before <see cref="Process"/> execution.
        /// </summary>
        /// <param name="obj">Can be an entity object or other relative object received from <see cref="Process"/> to be executed.</param>
        /// <param name="uri">The <see cref="ProcessURI"/> that identifies current <see cref="Process"/>.</param>
        void OnProcessExecuting(ProcessURI uri, object obj);

        /// <summary>
        /// Defines the method to be implemented and runs after <see cref="Process"/> execution.
        /// </summary>
        /// <param name="obj">Can be an entity object or other relative object received from executed <see cref="Process"/>.</param>
        /// <param name="uri">The <see cref="ProcessURI"/> that identifies current <see cref="Process"/>.</param>
        void OnProcessExecuted(ProcessURI uri, object obj);
    }
}
