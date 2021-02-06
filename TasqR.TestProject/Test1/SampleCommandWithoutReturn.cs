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
}
