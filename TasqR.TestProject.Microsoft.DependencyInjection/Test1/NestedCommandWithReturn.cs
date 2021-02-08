namespace TasqR.TestProject.Microsoft.DependencyInjection.Test1
{
    public class NestedCommandWithReturn : ITasq<int>
    {
        public NestedCommandWithReturn(int startNumber)
        {
            StartNumber = startNumber;
        }

        public int StartNumber { get; }
    }

    public class NestedCommandWithReturnHandler : TasqHandler<NestedCommandWithReturn, int>
    {
        private readonly ITasqR p_TasqR;

        public NestedCommandWithReturnHandler(ITasqR tasqR)
        {
            p_TasqR = tasqR;
        }

        public override int Run(NestedCommandWithReturn process)
        {
            int firstResult = process.StartNumber + 1;

            return p_TasqR.Run(new CommandWithReturn(firstResult));
        }
    }
}
