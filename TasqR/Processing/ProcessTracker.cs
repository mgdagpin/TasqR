using System;
using System.Collections.Generic;
using System.Linq;
using TasqR.Processing.Enums;
using TasqR.Processing.Interfaces;

namespace TasqR.Processing
{
    public class ProcessTracker : IProcessTracker
    {
        protected ITasqR m_Processor;
        protected TaskJob m_Job;
        protected ParameterDictionary m_Parameters;
        protected List<TaskLog> m_Logs = new List<TaskLog>();

        private bool jobIsAlreadyAttached;
        private int totalProcessed;
        private bool? isAborted;

        public bool IsBatch { get; private set; }
        public JobStatus JobStatus { get; private set; }
        public Guid UID { get; private set; }

        public bool? Aborted => isAborted;
        public int TotalProcessed => totalProcessed;

        public virtual void Initialize(ITasqR processor)
        {
            UID = Guid.NewGuid();

            m_Processor = processor;

            JobStatus = JobStatus.Initialized;
        }

        public virtual void AttachJob(TaskJob job, ParameterDictionary parameters)
        {
            if (jobIsAlreadyAttached)
            {
                throw new TasqException("Job is already attached to this request.");
            }

            m_Job = job;
            m_Parameters = parameters;

            if (parameters["SpecialGroupID"] != null
                && int.TryParse(parameters["SpecialGroupID"].Value, out int specialGroupId)
                && specialGroupId > 0)
            {
                IsBatch = true;
            }

            jobIsAlreadyAttached = true;
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

        public virtual void ReThrowErrorsIfAny()
        {

        }


        public virtual bool TryGetJobParameter<T>(string key, out T result)
        {
            if (m_Parameters != null && m_Parameters.Exists(key))
            {
                result = m_Parameters.GetAs<T>(key);

                return true;
            }

            result = default;

            return false;
        }
    }
}