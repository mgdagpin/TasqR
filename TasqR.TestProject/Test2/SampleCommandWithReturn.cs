namespace TasqR.TestProject.Test2
{
    public class SampleCommandWithReturn : ITasq<int>
    {
        public SampleCommandWithReturn(int firstNumber)
        {
            FirstNumber = firstNumber;
        }

        public int FirstNumber { get; }
    }
}
