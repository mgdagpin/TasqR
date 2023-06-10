using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace TasqR
{
    public partial class TasqR
    {
        #region Run (No return)
        public virtual async Task RunAsync(ITasq tasq, CancellationToken cancellationToken = default)
        {
            var resolvedHandler = GetHandlerDetail(tasq);

            // if the tasq is iteration with key
            if (resolvedHandler.Reference.HandlerInterface.IsGenericType
                && resolvedHandler.Reference.HandlerInterface.GetGenericArguments().Length == 3)
            {
                throw new TasqException("Tasq passed is with a key type (ITasq<,>) but being used as ITasq");
                //var argT1 = resolvedHandler.Reference.HandlerInterface.GetGenericArguments()[1];
                //var argT2 = resolvedHandler.Reference.HandlerInterface.GetGenericArguments()[2];

                //var method = this.GetType().GetMethods().Single(a => a.Name == nameof(RunAsync) && a.GetGenericArguments().Length == 2);
                //var methodInfo = method.MakeGenericMethod(argT1, argT2);

                //methodInfo.Invoke(this, parameters: new object[] { tasq, cancellationToken });
            }
            else if (resolvedHandler.Reference.HandlerInterface.IsGenericType
                && resolvedHandler.Reference.HandlerInterface.GetGenericArguments().Length == 2)
            {
                var argT1 = resolvedHandler.Reference.HandlerInterface.GetGenericArguments()[1];

                var method = this.GetType().GetMethods().Single(a => a.Name == nameof(RunAsync) && a.GetGenericArguments().Length == 1);

                await (Task)method.MakeGenericMethod(argT1).Invoke(this, parameters: new object[] { tasq, cancellationToken });
            }
            else
            {
                var tasqHandlerInstance = (TasqHandlerAsync)resolvedHandler.Handler;

                tasqHandlerInstance.p_CancellationToken = cancellationToken;

                await tasqHandlerInstance.InitializeAsync(tasq);
                await tasqHandlerInstance.BeforeRunAsync(tasq);
                await tasqHandlerInstance.XRunAsync(null, tasq);
                await tasqHandlerInstance.AfterRunAsync(tasq);
            }            
        }
        #endregion

        #region Run (with return)
        public virtual async Task<TResponse> RunAsync<TResponse>(ITasq<TResponse> tasq, CancellationToken cancellationToken = default)
        {
            var resolvedHandler = GetHandlerDetail(tasq);

            if (resolvedHandler.Handler is TasqHandler)
            {
                return Run(tasq);
            }
            else
            {
                var tasqHandlerInstance = (TasqHandlerAsync)resolvedHandler.Handler;

                tasqHandlerInstance.p_CancellationToken = cancellationToken;

                await tasqHandlerInstance.InitializeAsync(tasq);
                await tasqHandlerInstance.BeforeRunAsync(tasq);
                var runTask = tasqHandlerInstance.XRunAsync(null, tasq);
                await runTask;

                await tasqHandlerInstance.AfterRunAsync(tasq);

                return GetTaskResult<TResponse>(runTask);
            }
        }
        #endregion

        #region Run (with key)
        public virtual async IAsyncEnumerable<TResponse> RunAsync<TKey, TResponse>(ITasq<TKey, TResponse> tasq, CancellationToken cancellationToken = default)
        {
            var resolvedHandler = GetHandlerDetail(tasq);

            if (resolvedHandler.Handler is TasqHandler)
            {
                foreach (var item in Run(tasq))
                {
                    yield return item;

                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                }
            }
            else
            {
                var tasqHandlerInstance = (TasqHandlerAsync)resolvedHandler.Handler;

                tasqHandlerInstance.p_CancellationToken = cancellationToken;

                await tasqHandlerInstance.InitializeAsync(tasq);
                var selectionCriteria = await tasqHandlerInstance.SelectionCriteriaAsync(tasq);

                foreach (var item in selectionCriteria)
                {
                    await tasqHandlerInstance.BeforeRunAsync(tasq);
                    var runTask = tasqHandlerInstance.XRunAsync(item, tasq);

                    await runTask;

                    await tasqHandlerInstance.AfterRunAsync(tasq);

                    yield return GetTaskResult<TResponse>(runTask);

                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                }
            }
        }

        //public virtual async IAsyncEnumerator<TResponse> RunAsync<TKey, TResponse>(ITasq<TKey, TResponse> tasq, CancellationToken cancellationToken = default)
        //{
        //    var resolvedHandler = GetHandlerDetail(tasq);

        //    if (resolvedHandler.Handler is TasqHandler)
        //    {
        //        foreach (var item in Run(tasq))
        //        {
        //            yield return item;
        //        }
        //    }
        //    else
        //    {
        //        var tasqHandlerInstance = (TasqHandlerAsync)resolvedHandler.Handler;

        //        tasqHandlerInstance.p_CancellationToken = cancellationToken;

        //        await tasqHandlerInstance.InitializeAsync(tasq);
        //        var selectionCriteria = await tasqHandlerInstance.SelectionCriteriaAsync(tasq);

        //        foreach (var item in selectionCriteria)
        //        {
        //            if (cancellationToken.IsCancellationRequested)
        //            {
        //                break;
        //            }

        //            await tasqHandlerInstance.BeforeRunAsync(tasq);
        //            var runTask = tasqHandlerInstance.XRunAsync(item, tasq);

        //            await runTask;

        //            await tasqHandlerInstance.AfterRunAsync(tasq);

        //            yield return GetTaskResult<TResponse>(runTask);
        //        }
        //    }
        //}
        #endregion

        protected TTaskResult GetTaskResult<TTaskResult>(Task task)
        {
            var taskType = task.GetType().GetMember("Result");

            if (taskType != null && taskType.Length > 0)
            {
                var first = taskType[0];
                var result = GetValue(first, task);

                return (TTaskResult)result;
            }

            return default;
        }

        protected object GetValue(MemberInfo memberInfo, object forObject)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)memberInfo).GetValue(forObject);
                case MemberTypes.Property:
                    return ((PropertyInfo)memberInfo).GetValue(forObject);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}