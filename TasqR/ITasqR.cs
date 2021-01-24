using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasqR
{
    public interface ITasqR
    {
        event ProcessEventHandler OnInitializeExecuting;
        event ProcessEventHandler OnInitializeExecuted;
        event ProcessEventHandler OnBeforeRunExecuting;
        event ProcessEventHandler OnBeforeRunExecuted;
        event ProcessEventHandler OnAfterRunExecuting;
        event ProcessEventHandler OnAfterRunExecuted;

        void Run(ITasq tasq);
        TResponse Run<TResponse>(ITasq<TResponse> tasq);
        TResponse Run<TKey, TResponse>(ITasq<TKey, TResponse> tasq);
    }
}
