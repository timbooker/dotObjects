using System;

namespace dotObjects.Core.UI
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class NonQueryableAttribute : Attribute
    {
    }
}