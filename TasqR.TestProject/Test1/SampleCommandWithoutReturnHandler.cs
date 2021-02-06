namespace TasqR.TestProject.Test1
{
    public class SampleCommandWithoutReturnHandler : JobProcessHandler<SampleCommandWithoutReturn>
    {
        public override void Run(SampleCommandWithoutReturn process)
        {
            process.TestModel.SampleNumber++;
        }
    }
}
