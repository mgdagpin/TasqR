using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TasqR.Common;

namespace TasqR
{
    public class TasqRObject : ITasqR
    {
        public Guid ID { get; private set; }
        public IEnumerable<TypeTasqReference> RegisteredReferences => p_TasqHandlerResolver.RegisteredReferences;

        #region Events
        public event ProcessEventHandler OnInitializeExecuting;
        public event ProcessEventHandler OnInitializeExecuted;

        public event ProcessEventHandler OnSelectionCriteriaExecuting;
        public event ProcessEventHandler OnSelectionCriteriaExecuted;


        public event ProcessEventHandler OnBeforeRunExecuting;
        public event ProcessEventHandler OnBeforeRunExecuted;

        public event ProcessEventHandler OnRunExecuting;
        public event ProcessEventHandler OnRunExecuted;

        public event ProcessEventHandler OnAfterRunExecuting;
        public event ProcessEventHandler OnAfterRunExecuted;
        #endregion

        private readonly ITasqHandlerResolver p_TasqHandlerResolver;

        public TasqRObject(ITasqHandlerResolver tasqHandlerResolver)
        {
            p_TasqHandlerResolver = tasqHandlerResolver;
            ID = Guid.NewGuid();
        }

        #region Run (No return)
        public void Run
            (
                ITasq tasq
            )
        {
            var tasqType = tasq.GetType();
            LogHelper.Log(tasq);

            var resolvedHandler = p_TasqHandlerResolver.ResolveHandler(tasqType);
            LogHelper.Log(resolvedHandler.Handler);

            if (resolvedHandler.Handler is TasqHandlerAsync)
            {
                RunAsync(tasq).Wait();
            }
            else
            {
                var tasqHandlerInstance = (TasqHandler)resolvedHandler.Handler;

                if (resolvedHandler.Reference.HandlerInterface.IsGenericType
                    && resolvedHandler.Reference.HandlerInterface.GetGenericArguments().Length == 3)
                {
                    var argT1 = resolvedHandler.Reference.HandlerInterface.GetGenericArguments()[1];
                    var argT2 = resolvedHandler.Reference.HandlerInterface.GetGenericArguments()[2];

                    var method = this.GetType().GetMethods().Single(a => a.Name == nameof(Run) && a.GetGenericArguments().Length == 2);

                    method.MakeGenericMethod(argT1, argT2).Invoke(this, parameters: new object[] { tasq });
                }
                else if (resolvedHandler.Reference.HandlerInterface.IsGenericType
                    && resolvedHandler.Reference.HandlerInterface.GetGenericArguments().Length == 2)
                {
                    var argT1 = resolvedHandler.Reference.HandlerInterface.GetGenericArguments()[1];

                    var method = this.GetType().GetMethods().Single(a => a.Name == nameof(Run) && a.GetGenericArguments().Length == 1);

                    method.MakeGenericMethod(argT1).Invoke(this, parameters: new object[] { tasq });
                }
                else
                {
                    TasqProcessEventHandler.Invoke
                    (
                        startEvent: OnInitializeExecuting,
                        method: () => tasqHandlerInstance.Initialize(tasq),
                        tasq: tasq,
                        endEvent: OnInitializeExecuted
                    );

                    TasqProcessEventHandler.Invoke
                    (
                        startEvent: OnBeforeRunExecuting,
                        method: () => tasqHandlerInstance.BeforeRun(tasq),
                        tasq: tasq,
                        endEvent: OnBeforeRunExecuted
                    );

                    TasqProcessEventHandler.Invoke
                        (
                            startEvent: OnRunExecuting,
                            method: () => tasqHandlerInstance.Run(null, tasq),
                            tasq: tasq,
                            endEvent: OnRunExecuted
                        );

                    TasqProcessEventHandler.Invoke
                    (
                        startEvent: OnAfterRunExecuting,
                        method: () => tasqHandlerInstance.AfterRun(tasq),
                        tasq: tasq,
                        endEvent: OnAfterRunExecuted
                    );
                }
            }
        }

        public Task RunAsync
            (
                ITasq tasq,
                CancellationToken cancellationToken = default
            )
        {
            var tasqType = tasq.GetType();
            LogHelper.Log(tasq);

            var resolvedHandler = p_TasqHandlerResolver.ResolveHandler(tasqType);
            LogHelper.Log(resolvedHandler.Handler);

            if (resolvedHandler.Reference.HandlerInterface.IsGenericType
                && resolvedHandler.Reference.HandlerInterface.GetGenericArguments().Length == 3)
            {
                var argT1 = resolvedHandler.Reference.HandlerInterface.GetGenericArguments()[1];
                var argT2 = resolvedHandler.Reference.HandlerInterface.GetGenericArguments()[2];

                var method = this.GetType().GetMethods().Single(a => a.Name == nameof(RunAsync) && a.GetGenericArguments().Length == 2);

                return (Task)method.MakeGenericMethod(argT1, argT2).Invoke(this, parameters: new object[] { tasq, cancellationToken });
            }
            else if (resolvedHandler.Reference.HandlerInterface.IsGenericType
                && resolvedHandler.Reference.HandlerInterface.GetGenericArguments().Length == 2)
            {
                var argT1 = resolvedHandler.Reference.HandlerInterface.GetGenericArguments()[1];

                var method = this.GetType().GetMethods().Single(a => a.Name == nameof(RunAsync) && a.GetGenericArguments().Length == 1);

                return (Task)method.MakeGenericMethod(argT1).Invoke(this, parameters: new object[] { tasq, cancellationToken });
            }

            var tasqHandlerInstance = (TasqHandlerAsync)resolvedHandler.Handler;

            var arg1 = new ProcessEventArgs();
            var arg2 = new ProcessEventArgs();
            var arg3 = new ProcessEventArgs();
            var arg4 = new ProcessEventArgs();

            arg1.StartStopwatch();
            OnInitializeExecuting?.Invoke(tasq, arg1);

            return tasqHandlerInstance.InitializeAsync(tasq)
                .ContinueWith(res1 =>
                {
                    arg1.StopStopwatch();
                    OnInitializeExecuted?.Invoke(tasq, arg1);

                    if (res1.IsFaulted) throw new TasqException(res1.Exception);

                    arg2.StartStopwatch();
                    OnBeforeRunExecuting?.Invoke(tasq, arg2);

                    return tasqHandlerInstance.BeforeRunAsync(tasq)
                        .ContinueWith(res2 =>
                        {
                            arg2.StopStopwatch();
                            OnBeforeRunExecuted?.Invoke(tasq, arg2);

                            if (res2.IsFaulted) throw new TasqException(res2.Exception);

                            arg3.StartStopwatch();
                            OnRunExecuting?.Invoke(tasq, arg3);

                            return tasqHandlerInstance.XRunAsync(null, tasq)
                                .ContinueWith(res3 =>
                                {
                                    arg3.StopStopwatch();
                                    OnRunExecuted?.Invoke(tasq, arg3);

                                    if (res3.IsFaulted) throw new TasqException(res3.Exception);


                                    arg4.StartStopwatch();
                                    OnAfterRunExecuting?.Invoke(tasq, arg4);

                                    return tasqHandlerInstance.AfterRunAsync(tasq)
                                        .ContinueWith(res4 =>
                                        {
                                            arg4.StopStopwatch();
                                            OnAfterRunExecuted?.Invoke(tasq, arg4);

                                            if (res4.IsFaulted) throw new TasqException(res4.Exception);

                                            return res3;
                                        }).Unwrap();
                                }).Unwrap();
                        }).Unwrap();
                }).Unwrap();
        }
        #endregion

        #region Run (with return)
        public TResponse Run<TResponse>
            (
                ITasq<TResponse> tasq
            )
        {
            TResponse retVal;
            var tasqType = tasq.GetType();
            LogHelper.Log(tasq);

            var resolvedHandler = p_TasqHandlerResolver.ResolveHandler(tasqType);
            LogHelper.Log(resolvedHandler.Handler);

            if (resolvedHandler.Handler is TasqHandlerAsync)
            {
                retVal = RunAsync(tasq).Result;
            }
            else
            {
                TasqHandler tasqHandlerInstance = (TasqHandler)resolvedHandler.Handler;


                TasqProcessEventHandler.Invoke
                (
                    startEvent: OnInitializeExecuting,
                    method: () => tasqHandlerInstance.Initialize(tasq),
                    tasq: tasq,
                    endEvent: OnInitializeExecuted
                );

                TasqProcessEventHandler.Invoke
                (
                    startEvent: OnBeforeRunExecuting,
                    method: () => tasqHandlerInstance.BeforeRun(tasq),
                    tasq: tasq,
                    endEvent: OnBeforeRunExecuted
                );

                retVal = TasqProcessEventHandler.Invoke
                    (
                        startEvent: OnRunExecuting,
                        method: () => (TResponse)tasqHandlerInstance.Run(null, tasq),
                        tasq: tasq,
                        endEvent: OnRunExecuted
                    );

                TasqProcessEventHandler.Invoke
                (
                    startEvent: OnAfterRunExecuting,
                    method: () => tasqHandlerInstance.AfterRun(tasq),
                    tasq: tasq,
                    endEvent: OnAfterRunExecuted
                );
            }

            return retVal;
        }

        public Task<TResponse> RunAsync<TResponse>
            (
                ITasq<TResponse> tasq,
                CancellationToken cancellationToken = default
            )
        {
            Task<TResponse> retVal = null;
            var tasqType = tasq.GetType();
            LogHelper.Log(tasq);

            var resolvedHandler = p_TasqHandlerResolver.ResolveHandler(tasqType);
            LogHelper.Log(resolvedHandler.Handler);

            if (resolvedHandler.Handler is TasqHandler)
            {
                retVal = Task.FromResult(Run(tasq));
            }
            else
            {
                TasqHandlerAsync tasqHandlerInstance = (TasqHandlerAsync)resolvedHandler.Handler;

                tasqHandlerInstance.p_CancellationToken = cancellationToken;


                var arg1 = new ProcessEventArgs();
                var arg2 = new ProcessEventArgs();
                var arg3 = new ProcessEventArgs();
                var arg4 = new ProcessEventArgs();

                arg1.StartStopwatch();
                OnInitializeExecuting?.Invoke(tasq, arg1);

                retVal = tasqHandlerInstance.InitializeAsync(tasq)
                    .ContinueWith(res1 =>
                    {
                        arg1.StopStopwatch();
                        OnInitializeExecuted?.Invoke(tasq, arg1);

                        if (res1.IsFaulted) throw new TasqException(res1.Exception);

                        arg2.StartStopwatch();
                        OnBeforeRunExecuting?.Invoke(tasq, arg2);

                        return tasqHandlerInstance.BeforeRunAsync(tasq)
                            .ContinueWith(res2 =>
                            {
                                arg2.StopStopwatch();
                                OnBeforeRunExecuted?.Invoke(tasq, arg2);

                                if (res2.IsFaulted) throw new TasqException(res2.Exception);

                                arg3.StartStopwatch();
                                OnRunExecuting?.Invoke(tasq, arg3);

                                return tasqHandlerInstance.XRunAsync(null, tasq)
                                    .ContinueWith(res3 =>
                                    {
                                        arg3.StopStopwatch();
                                        OnRunExecuted?.Invoke(tasq, arg3);

                                        if (res3.IsFaulted) throw new TasqException(res3.Exception);


                                        arg4.StartStopwatch();
                                        OnAfterRunExecuting?.Invoke(tasq, arg4);

                                        return tasqHandlerInstance.AfterRunAsync(tasq)
                                            .ContinueWith(res4 =>
                                            {
                                                arg4.StopStopwatch();
                                                OnAfterRunExecuted?.Invoke(tasq, arg4);

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
        public IEnumerable<TResponse> Run<TKey, TResponse>
            (
                ITasq<TKey, TResponse> tasq
            )
        {
            var tasqType = tasq.GetType();
            LogHelper.Log(tasq);

            var resolvedHandler = p_TasqHandlerResolver.ResolveHandler(tasqType);
            LogHelper.Log(resolvedHandler.Handler);

            if (resolvedHandler.Handler is TasqHandlerAsync)
            {
                return RunAsync(tasq).Result;
            }

            TasqHandler tasqHandlerInstance = (TasqHandler)resolvedHandler.Handler;

            List<TResponse> retVal = new List<TResponse>();

            TasqProcessEventHandler.Invoke
            (
                startEvent: OnInitializeExecuting,
                method: () => tasqHandlerInstance.Initialize(tasq),
                tasq: tasq,
                endEvent: OnInitializeExecuted
            );

            TasqProcessEventHandler.Invoke
            (
                startEvent: OnBeforeRunExecuting,
                method: () => tasqHandlerInstance.BeforeRun(tasq),
                tasq: tasq,
                endEvent: OnBeforeRunExecuted
            );

            var selectionCriteria = TasqProcessEventHandler.Invoke
                (
                    startEvent: OnSelectionCriteriaExecuting,
                    method: () => tasqHandlerInstance.SelectionCriteria(tasq),
                    tasq: tasq,
                    endEvent: OnSelectionCriteriaExecuted
                );

            if (selectionCriteria != null)
            {
                foreach (var eachSelection in selectionCriteria)
                {
                    var result = TasqProcessEventHandler.Invoke
                    (
                        startEvent: OnRunExecuting,
                        method: () => (TResponse)tasqHandlerInstance.Run(eachSelection, tasq),
                        tasq: tasq,
                        endEvent: OnRunExecuted
                    );

                    retVal.Add(result);
                }
            }

            TasqProcessEventHandler.Invoke
            (
                startEvent: OnAfterRunExecuting,
                method: () => tasqHandlerInstance.AfterRun(tasq),
                tasq: tasq,
                endEvent: OnAfterRunExecuted
            );


            return retVal;
        }

        public Task<IEnumerable<TResponse>> RunAsync<TKey, TResponse>
            (
                ITasq<TKey, TResponse> tasq,
                CancellationToken cancellationToken = default
            )
        {
            Task<IEnumerable<TResponse>> retVal = null;
            var tasqType = tasq.GetType();
            LogHelper.Log(tasq);

            var resolvedHandler = p_TasqHandlerResolver.ResolveHandler(tasqType);
            LogHelper.Log(resolvedHandler.Handler);

            if (resolvedHandler.Handler is TasqHandler)
            {
                retVal = Task.FromResult(Run(tasq));
            }
            else
            {
                TasqHandlerAsync tasqHandlerInstance = (TasqHandlerAsync)resolvedHandler.Handler;

                var arg1 = new ProcessEventArgs();
                var arg2 = new ProcessEventArgs();
                var arg3 = new ProcessEventArgs();
                var arg4 = new ProcessEventArgs();
                var arg5 = new ProcessEventArgs();

                arg1.StartStopwatch();
                OnInitializeExecuting?.Invoke(tasq, arg1);

                retVal = tasqHandlerInstance.InitializeAsync(tasq)
                    .ContinueWith(res1 =>
                    {
                        arg1.StopStopwatch();
                        OnInitializeExecuted?.Invoke(tasq, arg1);

                        if (res1.IsFaulted) throw new TasqException(res1.Exception);

                        arg2.StartStopwatch();
                        OnSelectionCriteriaExecuting?.Invoke(tasq, arg2);


                        List<TResponse> abRet = new List<TResponse>();

                        return tasqHandlerInstance.SelectionCriteriaAsync(tasq)
                            .ContinueWith(a =>
                            {
                                List<Task<TResponse>> listOfTask = new List<Task<TResponse>>();

                                foreach (var item in a.Result)
                                {
                                    var task = tasqHandlerInstance.BeforeRunAsync(tasq)
                                             .ContinueWith(res2 =>
                                             {
                                                 arg3.StopStopwatch();
                                                 OnBeforeRunExecuted?.Invoke(tasq, arg3);

                                                 if (res2.IsFaulted) throw new TasqException(res2.Exception);

                                                 arg4.StartStopwatch();
                                                 OnRunExecuting?.Invoke(tasq, arg4);

                                                 return tasqHandlerInstance.XRunAsync(item, tasq)
                                                     .ContinueWith(res3 =>
                                                     {
                                                         arg4.StopStopwatch();
                                                         OnRunExecuted?.Invoke(tasq, arg4);

                                                         if (res3.IsFaulted) throw new TasqException(res3.Exception);


                                                         arg5.StartStopwatch();
                                                         OnAfterRunExecuting?.Invoke(tasq, arg5);

                                                         return tasqHandlerInstance.AfterRunAsync(tasq)
                                                             .ContinueWith(res5 =>
                                                             {
                                                                 arg5.StopStopwatch();
                                                                 OnAfterRunExecuted?.Invoke(tasq, arg5);

                                                                 if (res5.IsFaulted) throw new TasqException(res5.Exception);

                                                                 var tasR3 = (Task<TResponse>)res3;

                                                                 abRet.Add(tasR3.Result);

                                                                 return tasR3;
                                                             }).Unwrap();
                                                     }).Unwrap();
                                             }).Unwrap();

                                    listOfTask.Add(task);
                                }

                                return Task.WhenAll(listOfTask)
                                    .ContinueWith(ret =>
                                    {
                                        return Task.FromResult((IEnumerable<TResponse>)abRet);
                                    }).Unwrap();
                            }).Unwrap();
                    }).Unwrap();
            }

            return retVal;
        }
        #endregion
    }
}