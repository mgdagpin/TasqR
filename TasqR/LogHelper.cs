using System;
using System.Collections.Generic;
using System.Text;

namespace TasqR
{
    internal static class LogHelper
    {        
        internal static void Log(ITasqHandler handler)
        {
            Console.WriteLine($"Handler: {handler.GetType().FullName}");
        }

        internal static void Log(ITasq tasq)
        {
            Console.WriteLine($"Tasq: {tasq.GetType().FullName}");
        }
    }
}
