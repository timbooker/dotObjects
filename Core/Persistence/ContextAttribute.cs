using System;

namespace dotObjects.Core.Persistence
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ContextAttribute : Attribute
    {
        public ContextAttribute(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            Name = name;
        }

        public string Name { get; set; }
    }
}