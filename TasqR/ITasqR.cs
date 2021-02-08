using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TasqR
{
    public interface ITasqR
    {
        event ProcessEventHandler OnInitializeExecuting;
        event ProcessEventHandler OnInitializeExecuted;

        public event ProcessEventHandler OnSelectionCriteriaExecuting;
        public event ProcessEventHandler OnSelectionCriteriaExecuted;

        event ProcessEventHandler OnBeforeRunExecuting;
        event ProcessEventHandler OnBeforeRunExecuted;

        event ProcessEventHandler OnRunExecuting;
        event ProcessEventHandler OnRunExecuted;

        event ProcessEventHandler OnAfterRunExecuting;
        event ProcessEventHandler OnAfterRunExecuted;

        IEnumerable<TypeTasqReference> RegisteredReferences { get; }

        void Run(ITasq tasq);
        TResponse Run<TResponse>(ITasq<TResponse> tasq);
        TResponse Run<TKey, TResponse>(ITasq<TKey, TResponse> tasq);
    }
}
