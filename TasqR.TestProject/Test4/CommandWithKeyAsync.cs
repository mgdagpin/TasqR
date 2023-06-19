using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TasqR.TestProject.Test4
{
    public class CommandWithKeyAsync : ITasq<int, bool>
    {
        public Dictionary<int, bool> TempData { get; set; } = new Dictionary<int, bool>
        {
            { 1, false },
            { 2, false },
            { 3, false }
        };
    }

    public class CommandWithKeyAsyncHandler : TasqHandlerAsync<CommandWithKeyAsync, int, bool>
    {
        public async override Task<IEnumerable> SelectionCriteriaAsync(CommandWithKeyAsync tasq, CancellationToken cancellationToken = default)
        {
            await Task.Delay(2000);

            return tasq.TempData.Select(a => a.Key).ToList();
        }

        public async override Task<bool> RunAsync(int key, CommandWithKeyAsync process, CancellationToken cancellationToken = default)
        {
            await Task.Delay(3000);

            if (process.TempData.ContainsKey(key))
            {
                process.TempData[key] = true;

                return true;
            }

            return false;
        }
    }
}