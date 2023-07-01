namespace TasqR.Processing
{
    public static class Enums
    {
        public enum JobStatus : byte
        {
            None = 0,
            Initialized = 1,
            Queued = 2,
            Started = 3,
            Failed = 4,
            Completed = 5,
            CompletedWithErrors = 6,
            Aborted = 7
        }

        public enum TaskMessageLogLevel
        {
            Info,
            Warning,
            Error
        }
    }
}
