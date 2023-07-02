﻿using System;
using static TasqR.Processing.Enums;

namespace TasqR.Processing
{
    public interface IProcessTracker
    {
        JobStatus JobStatus { get; }
        int TotalProcessed { get; }
        Guid UID { get; }
        bool? Aborted { get; }
        bool IsBatch { get; }


        void Initialize(ITasqR processor);
        void IncrementTotalProcessed();
        void AttachJob(TaskJob job);
        void LogMessage(string message, object key = null);
        void LogWarning(string message, object key = null);
        void LogError(Exception exception, object key = null);
        void JobStarted();
        void JobEnded();
        void Abort();

        bool TryGetParameter<T>(string key, out T result);
    }
}
