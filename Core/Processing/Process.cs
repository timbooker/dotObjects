using System;
using dotObjects.Core.Configuration;
using dotObjects.Core.UI;

namespace dotObjects.Core.Processing
{
    /// <summary>
    /// Abstract class for Process definition.
    /// </summary>
    public abstract class Process
    {
        /// <summary>
        /// 
        /// </summary>
        public const string StateFieldName = "dotObjectProcessState";

        private Type entityType;

        protected Process(ProcessURI uri)
        {
            URI = uri;
        }

        protected Type EntityType
        {
            get
            {
                if (entityType == null || !entityType.Name.Equals(URI.Entity))
                    entityType = CoreSection.Current.FindType(URI.Entity);
                if (entityType == null)
                    throw new Exception(string.Format("The type {0} is not a valid entity type!", URI.Entity));
                return entityType;
            }
        }

        protected ProcessURI URI { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public static ProcessState CurrentState { protected get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="before"></param>
        /// <param name="after"></param>
        /// <returns></returns>
        public abstract IProcessResponse Execute(Domain domain, Action<object> before, Action<object> after);

    }
}