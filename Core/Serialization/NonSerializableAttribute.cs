using System;

namespace dotObjects.Core.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
    public sealed class NonSerializableAttribute : Attribute
    {
    }
}