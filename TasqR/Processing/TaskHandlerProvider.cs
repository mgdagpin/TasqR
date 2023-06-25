using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TasqR.Processing
{
    public class TaskHandlerProvider
    {
        public short ID { get; set; }

        public string TaskAssembly { get; set; }
        public string TaskClass { get; set; }

        public string HandlerAssembly { get; set; }
        public string HandlerClass { get; set; }

        public bool IsDefaultHandler
        {
            get
            {
                return string.IsNullOrWhiteSpace(HandlerAssembly) || string.IsNullOrWhiteSpace(HandlerClass);
            }
        }

        public Type NonDefaultHandler
        {
            get
            {
                if (IsDefaultHandler)
                {
                    return null;
                }

                var assemblyH = Assembly.Load(assemblyString: HandlerAssembly);
                return assemblyH.GetType(name: HandlerClass);
            }
        }
    }
}
