using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace TasqR.Processing
{
    public class JobProcessor<T> where T : IProcessTracker
    {
        public virtual T InstantiateProcessTracker() => (T)Activator.CreateInstance(typeof(T));

        protected virtual void Queue(T processTracker) { }

        protected virtual Task QueueAsync(T processTracker, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public virtual async IAsyncEnumerable<TResult> RunBatchJobAsync<TKey, TResult>(ITasqR processor, TaskJob job, [EnumeratorCancellation]CancellationToken cancellationToken = default)
        {
            if (job == null)
            {
                throw new ArgumentNullException(nameof(job));
            }

            if (job.TasqProvider == null)
            {
                throw new ArgumentNullException(nameof(job.TasqProvider));
            }

            var jobRequest = InstantiateProcessTracker();

            jobRequest.Initialize(processor);

            jobRequest.AttachJob(job);

            ITasq<TKey, TResult> instance = null;

            try
            {
                jobRequest.JobStarted();

                var handlerProvider = job.TasqProvider;

                var assembly = Assembly.Load(assemblyString: handlerProvider.TaskAssembly);
                var type = assembly.GetType(handlerProvider.TaskClass);

                instance = (ITasq<TKey, TResult>)Activator.CreateInstance(type, args: new object[] { jobRequest });

                if (!handlerProvider.IsDefaultHandler)
                {
                    processor.UsingAsHandler(handlerProvider.NonDefaultHandler);
                }               
            }
            catch (Exception ex)
            {
                jobRequest.LogError(ex);
            }

            if (instance != null)
            {
                await foreach (var result in processor.RunAsync(instance, cancellationToken))
                {
                    yield return result;

                    if (jobRequest.JobStatus == Enums.JobStatus.Aborted)
                    {
                        yield break;
                    }                    
                }
            }

            if (cancellationToken.IsCancellationRequested && jobRequest.JobStatus != Enums.JobStatus.Aborted)
            {
                jobRequest.Abort();
            }

            jobRequest.JobEnded();
        }

        public virtual IProcessTracker RunJob(ITasqR processor, TaskJob job, bool runBatch = false)
        {
            if (job == null)
            {
                throw new ArgumentNullException(nameof(job));
            }

            if (job.TasqProvider == null)
            {
                throw new ArgumentNullException(nameof(job.TasqProvider));
            }

            var jobRequest = InstantiateProcessTracker();

            jobRequest.Initialize(processor);

            jobRequest.AttachJob(job);

            if (jobRequest.IsBatch && !runBatch)
            {
                Queue(jobRequest);

                return jobRequest;
            }

            try
            {
                jobRequest.JobStarted();

                var handlerProvider = job.TasqProvider;

                var assembly = Assembly.Load(assemblyString: handlerProvider.TaskAssembly);
                var type = assembly.GetType(handlerProvider.TaskClass);

                var instance = (ITasq)Activator.CreateInstance(type, args: new object[] { jobRequest });

                if (!handlerProvider.IsDefaultHandler)
                {
                    processor.UsingAsHandler(handlerProvider.NonDefaultHandler);
                }

                processor.Run(instance);
            }
            catch (Exception ex)
            {
                jobRequest.LogError(ex);
            }

            jobRequest.JobEnded();

            return jobRequest;
        }

        public virtual async Task<IProcessTracker> RunJobAsync(ITasqR processor, TaskJob job, bool runBatch = false, CancellationToken cancellationToken = default)
        {
            if (job == null)
            {
                throw new ArgumentNullException(nameof(job));
            }

            if (job.TasqProvider == null)
            {
                throw new ArgumentNullException(nameof(job.TasqProvider));
            }

            var jobRequest = InstantiateProcessTracker();

            jobRequest.Initialize(processor);

            jobRequest.AttachJob(job);

            if (jobRequest.IsBatch && !runBatch)
            {
                await QueueAsync(jobRequest, cancellationToken);

                return jobRequest;
            }

            try
            {
                jobRequest.JobStarted();

                var handlerProvider = job.TasqProvider;

                var assembly = Assembly.Load(assemblyString: handlerProvider.TaskAssembly);
                var type = assembly.GetType(handlerProvider.TaskClass);

                var instance = (ITasq)Activator.CreateInstance(type, args: new object[] { jobRequest });

                if (!handlerProvider.IsDefaultHandler)
                {
                    processor.UsingAsHandler(handlerProvider.NonDefaultHandler);
                }

                await processor.RunAsync(instance, cancellationToken);
            }
            catch (Exception ex)
            {
                jobRequest.LogError(ex);
            }

            if (cancellationToken.IsCancellationRequested && jobRequest.JobStatus != Enums.JobStatus.Aborted)
            {
                jobRequest.Abort();
            }

            jobRequest.JobEnded();

            return jobRequest;
        }
    }
}
