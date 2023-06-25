using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TasqR.Processing.Interfaces;

namespace TasqR.Processing
{
    public abstract class TaskJobBase
    {
        public IProcessTracker ProcessTracker { get; set; }

        protected TaskJobBase(IProcessTracker processTracker)
        {
            ProcessTracker = processTracker;
        }
    }
}
