using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace dotObjects.Metadata
{
    public class StaticMethodMetadata
    {
        private MethodInfo UnderlineMethod { get; set; }

        internal StaticMethodMetadata(MethodInfo method)
        {
            UnderlineMethod = method;
        }
    }
}
