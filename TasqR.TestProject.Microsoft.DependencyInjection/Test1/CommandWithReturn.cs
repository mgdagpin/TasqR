namespace TasqR.TestProject.Microsoft.DependencyInjection.Test1
{
    public class CommandWithReturn : ITasq<int>
    {
        public CommandWithReturn(int startNumber)
        {
            StartNumber = startNumber;
        }

        public int StartNumber { get; internal set; }
    }

    public class CommandWithReturnHandler : JobProcessHandler<CommandWithReturn, int>
    {
        public override int Run(CommandWithReturn process)
        {
            return process.StartNumber + 1;
        }
    }
}
