namespace TasqR.TestProject.Test2
{
    public class SampleCommandWithReturnHandler : JobProcessHandler<SampleCommandWithReturn, int>
    {
        public override int Run(SampleCommandWithReturn process)
        {
            return process.FirstNumber + 1;
        }
    }
}
