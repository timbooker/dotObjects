using System;
using dotObjects.Core.Processing;

namespace dotObjects.Core.UI
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class NonInteractiveAttribute : Attribute
    {
        public ProcessBehavior Behavior { get; set; }

        public NonInteractiveAttribute()
        {
            Behavior = ProcessBehavior.New | ProcessBehavior.Edit | ProcessBehavior.Delete;
        }

        public NonInteractiveAttribute(ProcessBehavior behavior)
        {
            Behavior = behavior;
        }
    }
}