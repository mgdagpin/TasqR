using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using TasqR.Processing.Interfaces;

namespace TasqR.Processing
{
    public class JobProcessor<T> where T : IProcessTracker, new()
    {
        public IProcessTracker QueueJob(ITasqR processor, TaskJob job, ParameterDictionary parameters)
        {
            var jobRequest = new T();

            jobRequest.Initialize(processor);

            jobRequest.AttachJob(job, parameters);

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

        public async Task<IProcessTracker> QueueJobAsync(ITasqR processor, TaskJob job, ParameterDictionary parameters, CancellationToken cancellationToken = default)
        {
            var jobRequest = new T();

            jobRequest.Initialize(processor);

            jobRequest.AttachJob(job, parameters);

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
                    await processor.RunAsync(instance);

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
