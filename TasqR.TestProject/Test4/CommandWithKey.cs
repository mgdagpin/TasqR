using System.Collections.Generic;
using System.Linq;

namespace TasqR.TestProject.Test4
{
    public class CommandWithKey : ITasq<int, bool>
    {
        public bool AllAreCorrect { get; set; }
    }

    public class CommandWithKeyHandler : TasqHandler<CommandWithKey, int, bool>
    {
        private readonly List<int> p_Keys = new List<int> { 1, 2, 3 };


        public override bool Run(int key, CommandWithKey process)
        {
            if (p_Keys.Contains(key))
            {
                p_Keys.Remove(key);
            }

            process.AllAreCorrect = !p_Keys.Any();

            return true;
        }

        public override IEnumerable<int> SelectionCriteria(CommandWithKey tasq)
        {
            int[] keys = new[] { 1, 2, 3 };

            return keys;
        }
    }
}
