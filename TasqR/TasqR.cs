using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TasqR.Common;

namespace TasqR
{
    public partial class TasqR : ITasqR
    {
        public Guid ID { get; private set; }

        public IEnumerable<TypeTasqReference> RegisteredReferences => p_TasqHandlerResolver.RegisteredReferences;

        private bool autoClearReference;
        private readonly ITasqHandlerResolver p_TasqHandlerResolver;
        internal TasqHandlerDetail ForcedHandlerDetail;

        public TasqR(ITasqHandlerResolver tasqHandlerResolver)
        {
            p_TasqHandlerResolver = tasqHandlerResolver;
            ID = Guid.NewGuid();
        }

        public virtual ITasqR UsingAsHandler(Type type, bool autoClearReference = false)
        {
            this.autoClearReference = autoClearReference;
            ForcedHandlerDetail = TasqHandlerDetail.TryGetFromType(type, p_TasqHandlerResolver);

            return this;
        }

        public virtual ITasqR UsingAsHandler<THandler>(bool autoClearReference = false) where THandler : ITasqHandler
        {
            return UsingAsHandler(typeof(THandler), autoClearReference);
        }


        #region Run (No return)
        public virtual void Run(ITasq tasq)
        {
            var resolvedHandler = GetHandlerDetail(tasq);

            if (resolvedHandler.Handler is TasqHandlerAsync)
            {
                RunAsync(tasq).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            else
            {
                // if the tasq is iteration with key
                if (resolvedHandler.Reference.HandlerInterface.IsGenericType
                    && resolvedHandler.Reference.HandlerInterface.GetGenericArguments().Length == 3)
                {
                    var argT1 = resolvedHandler.Reference.HandlerInterface.GetGenericArguments()[1];
                    var argT2 = resolvedHandler.Reference.HandlerInterface.GetGenericArguments()[2];

                    var method = GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Single(a => a.Name == nameof(RunWithKey));

                    method.MakeGenericMethod(argT1, argT2).Invoke(this, parameters: new object[] { tasq });
                }
                // with return tasq
                else if (resolvedHandler.Reference.HandlerInterface.IsGenericType
                    && resolvedHandler.Reference.HandlerInterface.GetGenericArguments().Length == 2)
                {
                    var argT1 = resolvedHandler.Reference.HandlerInterface.GetGenericArguments()[1];

                    var method = GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Single(a => a.Name == nameof(RunWithReturn));

                    method.MakeGenericMethod(argT1).Invoke(this, parameters: new object[] { tasq });
                }
                // void return tasq
                else
                {
                    var tasqHandlerInstance = (TasqHandler)resolvedHandler.Handler;

                    tasqHandlerInstance.Initialize(tasq);
                    tasqHandlerInstance.BeforeRun(tasq);
                    tasqHandlerInstance.XRun(null, tasq);
                    tasqHandlerInstance.AfterRun(tasq);
                }
            }
        }
        #endregion

        #region Run (with return)
        public virtual TResponse Run<TResponse>(ITasq<TResponse> tasq) => RunWithReturn(tasq);

        protected virtual TResponse RunWithReturn<TResponse>(ITasq<TResponse> tasq)
        {
            var resolvedHandler = GetHandlerDetail(tasq);

            if (resolvedHandler.Handler is TasqHandlerAsync)
            {
                return RunAsync(tasq).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            else
            {
                TasqHandler tasqHandlerInstance = (TasqHandler)resolvedHandler.Handler;

                tasqHandlerInstance.Initialize(tasq);
                tasqHandlerInstance.BeforeRun(tasq);
                var retVal = (TResponse)tasqHandlerInstance.XRun(null, tasq);
                tasqHandlerInstance.AfterRun(tasq);

                return retVal;
            }
        }
        #endregion

        #region Run (with key)
        public virtual IEnumerable<TResponse> Run<TKey, TResponse>(ITasq<TKey, TResponse> tasq) => RunWithKeyEnumerable(tasq);

        protected virtual IEnumerable<TResponse> RunWithKey<TKey, TResponse>(ITasq<TKey, TResponse> tasq)
        {
            var resolvedHandler = GetHandlerDetail(tasq);

            var tasqHandlerInstance = (TasqHandler)resolvedHandler.Handler;

            tasqHandlerInstance.Initialize(tasq);

            var selectionCriteria = tasqHandlerInstance.SelectionCriteria(tasq);
            var result = new List<TResponse>();

            if (selectionCriteria != null)
            {
                foreach (var eachSelection in selectionCriteria)
                {
                    tasqHandlerInstance.BeforeRun(tasq);

                    var runResult = (TResponse)tasqHandlerInstance.XRun(eachSelection, tasq);

                    result.Add(runResult);

                    tasqHandlerInstance.AfterRun(tasq);
                }
            }

            return result;
        }

        protected virtual IEnumerable<TResponse> RunWithKeyEnumerable<TKey, TResponse>(ITasq<TKey, TResponse> tasq)
        {
            var resolvedHandler = GetHandlerDetail(tasq);

            if (resolvedHandler.Handler is TasqHandlerAsync)
            {
                foreach (var item in ToListAsync(RunAsync(tasq)).Result)
                {
                    yield return item;
                }
            }
            else
            {
                var tasqHandlerInstance = (TasqHandler)resolvedHandler.Handler;

                tasqHandlerInstance.Initialize(tasq);

                var selectionCriteria = tasqHandlerInstance.SelectionCriteria(tasq);

                if (selectionCriteria != null)
                {
                    foreach (var eachSelection in selectionCriteria)
                    {
                        tasqHandlerInstance.BeforeRun(tasq);
                        yield return (TResponse)tasqHandlerInstance.XRun(eachSelection, tasq);
                        tasqHandlerInstance.AfterRun(tasq);
                    }
                }
            }
        }
        #endregion

        protected async Task<List<TResponse>> ToListAsync<TResponse>(IAsyncEnumerable<TResponse> temp1)
        {
            var result = new List<TResponse>();

            await foreach (var item in temp1)
            {
                result.Add(item);
            }

            return result;
        }

        public virtual Type GetHandlerType(ITasq tasq)
        {
            var resolvedHandler = GetHandlerDetail(tasq);

            return resolvedHandler.Reference.HandlerImplementation;
        }

        private TasqHandlerDetail GetHandlerDetail(ITasq tasq)
        {
            var tasqType = tasq.GetType();

            var handlerDetail = ForcedHandlerDetail != null
                ? ForcedHandlerDetail
                : p_TasqHandlerResolver.ResolveHandler(tasqType);

            if (autoClearReference)
            {
                ForcedHandlerDetail = null;
            }

            return handlerDetail;
        }

        public virtual ITasqR UsingAsHandler(string taskAssembly, string taskClass, bool autoClearReference = false)
        {
            var assembly = Assembly.LoadFrom(taskAssembly);
            var type = assembly.GetType(taskClass);
            
            return UsingAsHandler(type, autoClearReference);
        }        
    }
}