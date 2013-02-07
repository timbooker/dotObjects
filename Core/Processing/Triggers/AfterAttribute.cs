using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dotObjects.Core.Processing.Triggers
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    public class AfterAttribute : Attribute
    {
        public Action<object> Instruction { get; set; }

        public AfterAttribute(Action<object> instruction)
        {
            if(instruction == null)
                throw new ArgumentNullException("instruction");
            Instruction = instruction;
        }
    }
}
