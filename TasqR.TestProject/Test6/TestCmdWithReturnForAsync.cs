using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TasqR.TestProject.Test6
{
    public class TestCmdWithReturnForAsync : ITasq<int>
    {
        public TestCmdWithReturnForAsync(int startValue)
        {
            StartValue = startValue;
        }

        public int StartValue { get; }
    }

    public class TestCmdWithReturnForAsyncHandler : TasqHandlerAsync<TestCmdWithReturnForAsync, int>
    {
        public override Task<int> RunAsync(TestCmdWithReturnForAsync process, CancellationToken cancellationToken = default)
        {
            int result = process.StartValue + 1;

            return Task.FromResult(result);
        }
    }
}
