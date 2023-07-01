using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace TasqR.Processing
{
    public class JobProcessor<T> where T : IProcessTracker
    {
        public virtual T InstantiateProcessTracker() => (T)Activator.CreateInstance(typeof(T));

        public virtual IProcessTracker QueueJob(ITasqR processor, TaskJob job)
        {
            var jobRequest = InstantiateProcessTracker();

            jobRequest.Initialize(processor);

            jobRequest.AttachJob(job);

            jobRequest.JobStarted();

            try
            {
                var handlerProvider = job.TaskHandlerProvider;

                var assembly = Assembly.Load(assemblyString: handlerProvider.TaskAssembly);
                var type = assembly.GetType(handlerProvider.TaskClass);

                var instance = (ITasq)Activator.CreateInstance(type, args: new object[] { jobRequest });

                if (!handlerProvider.IsDefaultHandler)
                {
                    processor.UsingAsHandler(handlerProvider.NonDefaultHandler);
                }

                if (!jobRequest.IsBatch)
                {
                    processor.Run(instance);

                    jobRequest.JobEnded();
                }
            }
            catch (Exception ex)
            {
                jobRequest.LogError(ex);

                jobRequest.JobEnded();
            }

            return jobRequest;
        }

        public virtual async Task<IProcessTracker> QueueJobAsync(ITasqR processor, TaskJob job, CancellationToken cancellationToken = default)
        {
            var jobRequest = InstantiateProcessTracker();

            jobRequest.Initialize(processor);

            jobRequest.AttachJob(job);

            jobRequest.JobStarted();

            try
            {
                var handlerProvider = job.TaskHandlerProvider;

                var assembly = Assembly.Load(assemblyString: handlerProvider.TaskAssembly);
                var type = assembly.GetType(handlerProvider.TaskClass);

                var instance = (ITasq)Activator.CreateInstance(type, args: new object[] { jobRequest });

                if (!handlerProvider.IsDefaultHandler)
                {
                    processor.UsingAsHandler(handlerProvider.NonDefaultHandler);
                }

                if (!jobRequest.IsBatch)
                {
                    await processor.RunAsync(instance, cancellationToken);

                    jobRequest.JobEnded();
                }
            }
            catch (Exception ex)
            {
                jobRequest.LogError(ex);

                jobRequest.JobEnded();
            }

            return jobRequest;
        }
    }
}
