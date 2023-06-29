using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasqR.Processing.Enums
{
    public enum JobStatus : byte
    {
        Initialized = 1,
        Queued = 2,
        Started = 3,
        Failed = 4,
        Completed = 5,
        CompletedWithErrors = 6,
        Aborted = 7
    }
}