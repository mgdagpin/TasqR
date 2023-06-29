using System;
using TasqR.Processing.Enums;

namespace TasqR.Processing
{
    public class TaskLog
    {
        public DateTime CreatedOn { get; set; }
        public object Data { get; set; }
        public object Key { get; set; }
        public TaskMessageLogLevel Level { get; set; }
    }
}
