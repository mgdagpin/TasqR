using System;
using System.Reflection;

namespace TasqR.Processing
{
    public class TaskHandlerProvider
    {
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
