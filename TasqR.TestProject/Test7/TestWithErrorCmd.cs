using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TasqR.TestProject.Test7
{
    public class TestWithErrorCmd : ITasq<int, bool>
    {
    }

    public class TestWithErrorCmdHandler : TasqHandlerAsync<TestWithErrorCmd, int, bool>
    {
        public async override Task<IEnumerable> SelectionCriteriaAsync(TestWithErrorCmd request, CancellationToken cancellationToken)
        {
            return new[] { 1 };
        }

        public override Task<bool> RunAsync(int key, TestWithErrorCmd tasq, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        
    }
}
