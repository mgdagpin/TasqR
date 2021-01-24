using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasqR
{
    public class ProcessEventArgs : EventArgs
    {
        public ITasqR Processor { get; private set; }

        internal ProcessEventArgs(ITasqR processor)
        {
            Processor = processor;
        }
    }

    public delegate void ProcessEventHandler(object sender, ProcessEventArgs args);
}
