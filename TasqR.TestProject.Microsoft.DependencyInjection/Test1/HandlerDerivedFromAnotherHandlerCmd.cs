using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasqR.TestProject.Microsoft.DependencyInjection.Test1
{
    public class HandlerDerivedFromAnotherHandlerCmd : ITasq<int>
    {
        public HandlerDerivedFromAnotherHandlerCmd(int startNumber)
        {
            StartNumber = startNumber;
        }

        public int StartNumber { get; }
    }

    public class Handler1 : TasqHandler<HandlerDerivedFromAnotherHandlerCmd, int>
    {
        public override int Run(HandlerDerivedFromAnotherHandlerCmd request)
        {
            return request.StartNumber + 1;
        }
    }

    public class Handler2 : Handler1
    {
        public override int Run(HandlerDerivedFromAnotherHandlerCmd request)
        {
            return request.StartNumber + 2;
        }
    }
}
