using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasqR.TestProject.Microsoft.DependencyInjection.Test2
{
    public class TestCommandWithMultipleHandler : ITasq<int>
    {
        public TestCommandWithMultipleHandler(int startNumber)
        {
            StartNumber = startNumber;
        }

        public int StartNumber { get; }
    }

    public class TestCommandWithMultipleHandlerHandler : TasqHandler<TestCommandWithMultipleHandler, int>
    {
        private readonly ITasqR p_TasqR;

        protected TestCommandWithMultipleHandlerHandler() { }

        public TestCommandWithMultipleHandlerHandler(ITasqR tasqR)
        {
            p_TasqR = tasqR;
        }

        public override int Run(TestCommandWithMultipleHandler request)
        {
            return request.StartNumber + 10;
        }
    }

    public class TestCommandWithMultipleHandlerHandler2 : TestCommandWithMultipleHandlerHandler
    {
        public override int Run(TestCommandWithMultipleHandler request)
        {
            return request.StartNumber + 20;
        }
    }

    public class TestCommandWithMultipleHandlerHandler3 : TestCommandWithMultipleHandlerHandler2
    {
        public override int Run(TestCommandWithMultipleHandler request)
        {
            return request.StartNumber + 30;
        }
    }
}
