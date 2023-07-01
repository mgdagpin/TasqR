using System;
using System.Collections.Generic;
using System.Linq;
using static TasqR.Processing.Enums;

namespace TasqR.Processing
{
    public class ProcessTracker : IProcessTracker
    {
        protected ITasqR m_Processor;
        protected TaskJob m_Job;
        protected List<TaskLog> m_Logs = new List<TaskLog>();

        private bool jobIsAlreadyAttached;
        private int totalProcessed;
        private bool? isAborted;

        public Guid UID { get; private set; }

        public bool IsBatch { get; protected set; }
        public JobStatus JobStatus { get; protected set; }

        public bool? Aborted => isAborted;
        public int TotalProcessed => totalProcessed;

        public virtual void Initialize(ITasqR processor)
        {
            UID = Guid.NewGuid();

            m_Processor = processor;

            JobStatus = JobStatus.Initialized;
        }

        public virtual void AttachJob(TaskJob job)
        {
            if (jobIsAlreadyAttached)
            {
                throw new TasqException("Job is already attached to this request.");
            }

            m_Job = job;

            if (m_Job.JobParameters.GetAs<int>("SpecialGroupID") > 0)
            {
                IsBatch = true;
            }

            jobIsAlreadyAttached = true;

            JobStatus = JobStatus.Queued;
        }

        public virtual void JobStarted()
        {
            JobStatus = JobStatus.Started;
        }

        public virtual void Abort()
        {
            isAborted = true;
        }

        public virtual void IncrementTotalProcessed()
        {
            totalProcessed++;
        }

        public virtual void JobEnded()
        {
            JobStatus = JobStatus.Completed;

            if (m_Logs.Any(a => a.Level == TaskMessageLogLevel.Error))
            {
                JobStatus = JobStatus.CompletedWithErrors;
            }
        }

        public virtual void LogError(Exception exception, object key = null)
        {
            m_Logs.Add(new TaskLog
            {
                CreatedOn = DateTime.UtcNow,
                Data = exception,
                Key = key,
                Level = TaskMessageLogLevel.Error
            });
        }

        public virtual void LogMessage(string message, object key = null)
        {
            m_Logs.Add(new TaskLog
            {
                CreatedOn = DateTime.UtcNow,
                Data = message,
                Key = key,
                Level = TaskMessageLogLevel.Info
            });
        }

        public virtual void LogWarning(string message, object key = null)
        {
            m_Logs.Add(new TaskLog
            {
                CreatedOn = DateTime.UtcNow,
                Data = message,
                Key = key,
                Level = TaskMessageLogLevel.Warning
            });
        }

        public virtual bool TryGetJobParameter<T>(string key, out T result)
        {
            if (m_Job.JobParameters != null && m_Job.JobParameters.Exists(key))
            {
                result = m_Job.JobParameters.GetAs<T>(key);

                return true;
            }

            result = default;

            return false;
        }
    }
}