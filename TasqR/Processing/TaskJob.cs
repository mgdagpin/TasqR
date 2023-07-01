namespace TasqR.Processing
{
    public class TaskJob
    {
        public string Name { get; set; }

        public bool IsBatch { get; set; }

        public TaskHandlerProvider TaskHandlerProvider { get; set; }

        public ParameterDictionary<Parameter> JobParameters { get; set; }
    }
}
