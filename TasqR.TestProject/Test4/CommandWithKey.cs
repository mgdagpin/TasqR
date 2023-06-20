using System.Collections.Generic;
using System.Linq;

namespace TasqR.TestProject.Test4
{
    public class CommandWithKey : ITasq<int, bool>
    {
        public Dictionary<int, bool> Data = new Dictionary<int, bool>
        {
            { 1, false },
            { 2, false },
            { 3, false }
        };
    }

    public class CommandWithKeyHandler : TasqHandler<CommandWithKey, int, bool>
    {
        public override bool Run(int key, CommandWithKey process)
        {
            process.Data[key] = true;

            return true;
        }

        public override IEnumerable<int> SelectionCriteria(CommandWithKey tasq)
        {
            return tasq.Data.Keys;
        }
    }
}
