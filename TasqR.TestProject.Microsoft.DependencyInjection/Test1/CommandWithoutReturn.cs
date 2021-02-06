namespace TasqR.TestProject.Microsoft.DependencyInjection.Test1
{
    public class TestModel
    {
        public int SampleNumber { get; set; }
    }

    public class CommandWithoutReturn : ITasq
    {
        public CommandWithoutReturn(TestModel testModel)
        {
            TestModel = testModel;
        }

        public TestModel TestModel { get; }

        public class CommandWithoutReturnHandler : JobProcessHandler<CommandWithoutReturn>
        {
            public override void Run(CommandWithoutReturn process)
            {
                process.TestModel.SampleNumber++;
            }
        }
    }
}
