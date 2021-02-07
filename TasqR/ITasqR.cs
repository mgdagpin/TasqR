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

        void Run(ITasq tasq);
        TResponse Run<TResponse>(ITasq<TResponse> tasq);
        TResponse Run<TKey, TResponse>(ITasq<TKey, TResponse> tasq);
    }
}
