using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasqR.Processing
{
    public class TaskJob
    {
        public short JobID { get; set; }

        public string ShortDesc { get; set; }
        public string LongDesc { get; set; }
        public string AltDesc { get; set; }

        public bool IsBatch { get; set; }

        public bool IsEnabled { get; set; }

        public TaskHandlerProvider TaskHandlerProvider { get; set; }

        public IEnumerable<TaskJobParameter> JobParameters { get; set; }
    }
}
