namespace TasqR.TestProject.Test1
{
    public class SampleCommandWithoutReturn : ITasq
    {
        public SampleCommandWithoutReturn(TestModel testModel)
        {
            TestModel = testModel;
        }

        public TestModel TestModel { get; }
    }

    public class SampleCommandWithoutReturnHandler : TasqHandler<SampleCommandWithoutReturn>
    {

        public override void Run(SampleCommandWithoutReturn process)
        {
            process.TestModel.SampleNumber++;
        }
    }
}
