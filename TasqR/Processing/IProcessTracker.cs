using System;
using static TasqR.Processing.Enums;

namespace TasqR.Processing
{
    public interface IProcessTracker
    {
        JobStatus JobStatus { get; }
        int Processed { get; }
        Guid UID { get; }
        bool IsBatch { get; }

        void Initialize(ITasqR processor);
        void IncrementTotalProcessed();
        void AttachJob(TaskJob job);
        void LogMessage(string message, object key = null);
        void LogWarning(string message, object key = null);
        void LogError(Exception exception, object key = null);
        void JobStarted();
        void Abort();
        void JobEnded();

        bool TryGetParameter<T>(string key, out T result);

        void ReThrowErrorsIfAny();
    }
}
