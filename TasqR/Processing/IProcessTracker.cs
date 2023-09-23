using System;
using static TasqR.Processing.Enums;

namespace TasqR.Processing
{
    public interface IProcessTracker
    {
        bool CanTrackExecutionTime { get; set; }

        JobStatus JobStatus { get; }

        /// <summary>
        /// Gets the total of data processed.
        /// </summary>
        int Processed { get; }

        /// <summary>
        /// Gets or sets how many data to process
        /// </summary>
        int Total { get; set; }

        Guid UID { get; }
        bool IsBatch { get; }

        void Initialize(ITasqR processor);
        void IncrementTotalProcessed();
        void AttachJob(TaskJob job);

        void TrackExecutionTime(string tag, object? key = null);


        void LogMessage(string message, object? key = null);
        void LogWarning(string message, object? key = null);
        void LogError(Exception exception, object? key = null);
        void JobStarted();
        void Abort();
        void JobEnded();

        /// <summary>
        /// Try to get parameter value and parsed it to <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns>Returns boolean if parameter value can be parsed or not</returns>
        bool TryGetParameter<T>(string key, out T result);

        /// <summary>
        /// Gets parameter value and parsed it to <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue">The default value to return if no result found</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FormatException"></exception>
        T GetParameter<T>(string key, T? defaultValue = default);

        void ReThrowErrorsIfAny();
    }
}
