using System;

namespace dotObjects.Core.Processing
{
    [Flags]
    public enum ProcessBehavior
    {
        New = 1,
        View = 2,
        Edit = 4,
        Delete = 8,
        Exec = 16,
        Query = 32,
        Index = 64,
        Schema = 128,
        Session = 1024,

        Service = 256,
        Workflow = 512
    }
}