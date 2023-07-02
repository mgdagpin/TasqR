namespace TasqR.Processing
{
    public class TaskJob
    {
        public string Name { get; set; }

        public bool IsBatch { get; set; }

        public TasqProvider TasqProvider { get; set; }

        public ParameterDictionary<Parameter> Parameters { get; set; } = new ParameterDictionary<Parameter>();
    }
}
