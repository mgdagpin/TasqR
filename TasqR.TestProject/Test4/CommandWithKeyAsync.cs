using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TasqR.TestProject.Test4
{
    public class CommandWithKeyAsync : ITasq<int, bool>
    {

    }

    public class CommandWithKeyAsyncHandler : TasqHandlerAsync<CommandWithKeyAsync, int, bool>
    {
        private readonly List<int> p_Keys = new List<int> { 1, 2, 3 };

        public async override Task<IEnumerable> SelectionCriteriaAsync(CommandWithKeyAsync tasq, CancellationToken cancellationToken = default)
        {
            return new[] { 1, 2, 3 };
        }

        public async override Task<bool> RunAsync(int key, CommandWithKeyAsync process, CancellationToken cancellationToken = default)
        {
            await Task.Delay(1000);

            if (p_Keys.Contains(key))
            {
                p_Keys.Remove(key);

                return true;
            }

            return false;
        }
    }
}