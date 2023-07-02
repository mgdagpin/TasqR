using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace TasqR.Processing
{
    public class JobProcessor<T> where T : IProcessTracker
    {
        public virtual T InstantiateProcessTracker() => (T)Activator.CreateInstance(typeof(T));

        protected virtual void Queue(T processTracker) { }

        protected virtual Task QueueAsync(T processTracker, CancellationToken cancellationToken = default) => Task.CompletedTask;

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

            jobRequest.JobEnded();

            return jobRequest;
        }
    }
}
