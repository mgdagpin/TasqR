using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using static TasqR.Processing.Enums;

namespace TasqR.Processing
{
    public class ProcessTracker : IProcessTracker
    {
        protected ITasqR m_Processor = null!;
        protected TaskJob m_Job = null!;
        protected List<TaskLog> m_Logs = new();

        private Stopwatch p_Watch = new();

        private bool jobIsAlreadyAttached;
        private int totalProcessed;

        public Guid UID { get; private set; }

        public virtual bool IsBatch { get; protected set; }
        public virtual JobStatus JobStatus { get; protected set; }

        
        public virtual int Processed => totalProcessed;

        public virtual int Total { get; set; }
        public bool CanTrackExecutionTime { get; set; }

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

            if (m_Job != null 
                && m_Job.Parameters != null 
                && m_Job.Parameters.GetAs<int>("SpecialGroupID") > 0)
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
            p_Watch.Stop();

            JobStatus = JobStatus.Aborted;
        }

        public virtual void IncrementTotalProcessed()
        {
            totalProcessed++;
        }

        public virtual void JobEnded()
        {
            p_Watch.Stop();

            if (JobStatus == JobStatus.Aborted)
            {
                return;
            }
            
            if (m_Logs.Any(a => a.Level == TaskMessageLogLevel.Error))
            {
                JobStatus = JobStatus.CompletedWithErrors;
            }
            else
            {
                JobStatus = JobStatus.Completed;
            }
        }

        public virtual void TrackExecutionTime(string tag, object? key = null)
        {
            if (CanTrackExecutionTime)
            {
                p_Watch.Stop();

                LogMessage($"{tag}: {p_Watch.ElapsedMilliseconds}", key);

                p_Watch.Restart();
            }            
        }

        public virtual void LogError(Exception exception, object? key = null)
        {
            m_Logs.Add(new TaskLog
            {
                CreatedOn = DateTime.UtcNow,
                Data = exception,
                Key = key,
                Level = TaskMessageLogLevel.Error
            });
        }

        public virtual void LogMessage(string message, object? key = null)
        {
            m_Logs.Add(new TaskLog
            {
                CreatedOn = DateTime.UtcNow,
                Data = message,
                Key = key,
                Level = TaskMessageLogLevel.Info
            });
        }

        public virtual void LogWarning(string message, object? key = null)
        {
            m_Logs.Add(new TaskLog
            {
                CreatedOn = DateTime.UtcNow,
                Data = message,
                Key = key,
                Level = TaskMessageLogLevel.Warning
            });
        }

        public virtual bool TryGetParameter<T>(string key, out T result)
        {
            if (m_Job != null 
                && m_Job.Parameters != null 
                && m_Job.Parameters.Exists(key))
            {
                result = m_Job.Parameters.GetAs<T>(key);

                return true;
            }

            result = default;

            return false;
        }

        public T GetParameter<T>(string key, T? defaultValue = default)
        {
            if (m_Job != null
                && m_Job.Parameters != null
                && m_Job.Parameters.Exists(key))
            {
                return m_Job.Parameters.GetAs<T>(key, throwWhenError: true);
            }

            return defaultValue;
        }

        public void ReThrowErrorsIfAny()
        {
            var errors = m_Logs.Where(a => a.Level == TaskMessageLogLevel.Error);

            if (errors.Any())
            {
                var aggregateExceptions = new AggregateException(errors.Select(a => a.Data as Exception).ToArray());

                ExceptionDispatchInfo.Capture(aggregateExceptions).Throw();
            }
        }
    }
}