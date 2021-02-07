namespace TasqR.TestProject.Test1
{
    public class SampleCommandWithoutReturnHandler : TasqHandler<SampleCommandWithoutReturn>
    {
        public override void Run(SampleCommandWithoutReturn process)
        {
            process.TestModel.SampleNumber++;
        }
    }
}
