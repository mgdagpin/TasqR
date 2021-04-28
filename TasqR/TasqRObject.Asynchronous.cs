using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TasqR
{
    public partial class TasqRObject
    {
        #region Run (No return)
        public Task RunAsync
            (
                ITasq tasq,
                CancellationToken cancellationToken = default
            )
        {
            var tasqType = tasq.GetType();
            OnLog?.Invoke(this, TasqProcess.Start, new LogEventHandlerEventArgs(tasq));

            var resolvedHandler = ForcedHandlerDetail != null
                ? ForcedHandlerDetail
                : p_TasqHandlerResolver.ResolveHandler(tasqType);

            ForcedHandlerDetail = null;

            OnLog?.Invoke(this, TasqProcess.Start, new LogEventHandlerEventArgs(resolvedHandler.Handler));

            if (resolvedHandler.Reference.HandlerInterface.IsGenericType
                && resolvedHandler.Reference.HandlerInterface.GetGenericArguments().Length == 3)
            {
                var argT1 = resolvedHandler.Reference.HandlerInterface.GetGenericArguments()[1];
                var argT2 = resolvedHandler.Reference.HandlerInterface.GetGenericArguments()[2];

                var method = this.GetType().GetMethods().Single(a => a.Name == nameof(RunAsync) && a.GetGenericArguments().Length == 2);

                return
                    ((Task)method.MakeGenericMethod(argT1, argT2)
                    .Invoke(this, parameters: new object[] { tasq, cancellationToken })
                    )
                    .ContinueWith(a =>
                    {
                        if (a.IsFaulted)
                        {
                            throw a.Exception;
                        }
                    });
            }
            else if (resolvedHandler.Reference.HandlerInterface.IsGenericType
                && resolvedHandler.Reference.HandlerInterface.GetGenericArguments().Length == 2)
            {
                var argT1 = resolvedHandler.Reference.HandlerInterface.GetGenericArguments()[1];

                var method = this.GetType().GetMethods().Single(a => a.Name == nameof(RunAsync) && a.GetGenericArguments().Length == 1);

                return (Task)method.MakeGenericMethod(argT1).Invoke(this, parameters: new object[] { tasq, cancellationToken });
            }

            var tasqHandlerInstance = (TasqHandlerAsync)resolvedHandler.Handler;


            return tasqHandlerInstance.InitializeAsync(tasq)
                .ContinueWith(res1 =>
                {
                    if (res1.IsFaulted) throw new TasqException(res1.Exception);

                    return tasqHandlerInstance.BeforeRunAsync(tasq)
                        .ContinueWith(res2 =>
                        {
                            if (res2.IsFaulted) throw new TasqException(res2.Exception);

                            return tasqHandlerInstance.XRunAsync(null, tasq)
                                .ContinueWith(res3 =>
                                {
                                    if (res3.IsFaulted) throw new TasqException(res3.Exception);

                                    return tasqHandlerInstance.AfterRunAsync(tasq)
                                        .ContinueWith(res4 =>
                                        {
                                            if (res4.IsFaulted) throw new TasqException(res4.Exception);

                                            return res3;
                                        }).Unwrap();
                                }).Unwrap();
                        }).Unwrap();
                }).Unwrap();
        }
        #endregion

        #region Run (with return)
        public Task<TResponse> RunAsync<TResponse>
            (
                ITasq<TResponse> tasq,
                CancellationToken cancellationToken = default
            )
        {
            Task<TResponse> retVal = null;
            var tasqType = tasq.GetType();
            OnLog?.Invoke(this, TasqProcess.Start, new LogEventHandlerEventArgs(tasq));

            var resolvedHandler = ForcedHandlerDetail != null
                ? ForcedHandlerDetail
                : p_TasqHandlerResolver.ResolveHandler(tasqType);

            ForcedHandlerDetail = null;

            OnLog?.Invoke(this, TasqProcess.Start, new LogEventHandlerEventArgs(resolvedHandler.Handler));

            if (resolvedHandler.Handler is TasqHandler)
            {
                retVal = Task.FromResult(Run(tasq));
            }
            else
            {
                TasqHandlerAsync tasqHandlerInstance = (TasqHandlerAsync)resolvedHandler.Handler;

                tasqHandlerInstance.p_CancellationToken = cancellationToken;

                retVal = tasqHandlerInstance.InitializeAsync(tasq)
                    .ContinueWith(res1 =>
                    {
                        if (res1.IsFaulted) throw new TasqException(res1.Exception);

                        return tasqHandlerInstance.BeforeRunAsync(tasq)
                            .ContinueWith(res2 =>
                            {
                                if (res2.IsFaulted) throw new TasqException(res2.Exception);

                                return tasqHandlerInstance.XRunAsync(null, tasq)
                                    .ContinueWith(res3 =>
                                    {
                                        if (res3.IsFaulted) throw new TasqException(res3.Exception);

                                        return tasqHandlerInstance.AfterRunAsync(tasq)
                                            .ContinueWith(res4 =>
                                            {
                                                if (res4.IsFaulted) throw new TasqException(res4.Exception);

                                                return (Task<TResponse>)res3;
                                            }).Unwrap();
                                    }).Unwrap();
                            }).Unwrap();
                    }).Unwrap();
            }

            return retVal;
        }
        #endregion

        #region Run (with key)
        public Task<IEnumerable<TResponse>> RunAsync<TKey, TResponse>
            (
                ITasq<TKey, TResponse> tasq,
                CancellationToken cancellationToken = default
            )
        {
            Task<IEnumerable<TResponse>> retVal = null;
            var tasqType = tasq.GetType();
            OnLog?.Invoke(this, TasqProcess.Start, new LogEventHandlerEventArgs(tasq));

            var resolvedHandler = ForcedHandlerDetail != null
                ? ForcedHandlerDetail
                : p_TasqHandlerResolver.ResolveHandler(tasqType);

            ForcedHandlerDetail = null;

            OnLog?.Invoke(this, TasqProcess.Start, new LogEventHandlerEventArgs(resolvedHandler.Handler));

            if (resolvedHandler.Handler is TasqHandler)
            {
                retVal = Task.FromResult(Run(tasq));
            }
            else
            {
                TasqHandlerAsync tasqHandlerInstance = (TasqHandlerAsync)resolvedHandler.Handler;

                retVal = tasqHandlerInstance.InitializeAsync(tasq)
                    .ContinueWith(async res1 =>
                    {
                        if (res1.IsFaulted) throw new TasqException(res1.Exception);

                        List<TResponse> abRet = new List<TResponse>();

                        var selectionCriteria = await tasqHandlerInstance.SelectionCriteriaAsync(tasq);

                        List<Task<TResponse>> listOfTask = new List<Task<TResponse>>();

                        foreach (var item in selectionCriteria)
                        {
                            var task = tasqHandlerInstance.BeforeRunAsync(tasq)
                                     .ContinueWith(res2 =>
                                     {
                                         if (res2.IsFaulted) throw new TasqException(res2.Exception);

                                         return tasqHandlerInstance.XRunAsync(item, tasq)
                                             .ContinueWith(res3 =>
                                             {
                                                 if (res3.IsFaulted) throw new TasqException(res3.Exception);

                                                 return tasqHandlerInstance.AfterRunAsync(tasq)
                                                     .ContinueWith(res5 =>
                                                     {
                                                         if (res5.IsFaulted) throw new TasqException(res5.Exception);

                                                         var tasR3 = (Task<TResponse>)res3;

                                                         abRet.Add(tasR3.Result);

                                                         return tasR3;
                                                     }).Unwrap();
                                             }, TaskContinuationOptions.AttachedToParent).Unwrap();
                                     }, TaskContinuationOptions.AttachedToParent).Unwrap();

                            listOfTask.Add(task);
                        }

                        var allResult = await Task.WhenAll(listOfTask);

                        return allResult.AsEnumerable();

                    }).Unwrap();
            }

            return retVal;
        }
        #endregion
    }
}