using System;
using dotObjects.Core.Processing;

namespace dotObjects.Core.UI
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class NonVisibleAttribute : Attribute
    {
        public ProcessBehavior Behavior { get; private set; }

        public NonVisibleAttribute()
        {
            Behavior = ProcessBehavior.Edit | ProcessBehavior.Exec | ProcessBehavior.New | ProcessBehavior.Query |
                       ProcessBehavior.View;
        }

        public NonVisibleAttribute(ProcessBehavior behavior)
        {
            Behavior = behavior;
        }
    }
}