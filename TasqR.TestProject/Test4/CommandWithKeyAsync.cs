using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TasqR.TestProject.Test4
{
    public class CommandWithKeyAsync : ITasq<int, bool>
    {
        public List<int> Keys { get; set; } = Enumerable.Range(1, 3).ToList();
    }

    public class CommandWithKeyAsyncHandler : TasqHandlerAsync<CommandWithKeyAsync, int, bool>
    {
        public async override Task<IEnumerable> SelectionCriteriaAsync(CommandWithKeyAsync tasq, CancellationToken cancellationToken = default)
        {
            await Task.Delay(2000);

            return tasq.Keys.ToList();
        }

        public async override Task<bool> RunAsync(int key, CommandWithKeyAsync process, CancellationToken cancellationToken = default)
        {
            await Task.Delay(3000);

            lock(process)
            {
                if (process.Keys.Contains(key))
                {
                    process.Keys.Remove(key);

                    return true;
                }
            }

            return false;
        }
    }
}