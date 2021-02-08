using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TasqR.TestProject.Test4
{
    public class CommandWithKeyAsync : ITasq<int, bool>
    {
        public bool AllAreCorrect { get; set; }
    }

    public class CommandWithKeyAsyncHandler : TasqHandlerAsync<CommandWithKeyAsync, int, bool>
    {
        private readonly List<int> p_Keys = new List<int> { 1, 2, 3 };

        public async override Task<IEnumerable<int>> SelectionCriteriaAsync(CommandWithKeyAsync tasq, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                int[] keys = new[] { 1, 2, 3 };

                return keys;
            });
        }

        public async override Task<bool> RunAsync(int key, CommandWithKeyAsync process, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                if (p_Keys.Contains(key))
                {
                    p_Keys.Remove(key);
                }

                process.AllAreCorrect = !p_Keys.Any();

                return true;
            });
        }
    }
}
