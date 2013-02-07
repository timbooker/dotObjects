using System;

namespace dotObjects.Core.UI
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class DependencyAttribute : Attribute
    {
        public string ID { get; set; }

        public DependencyAttribute(string id)
        {
            ID = id;
        }
    }
}