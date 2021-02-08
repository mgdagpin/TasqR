namespace TasqR
{
    public interface ITasq { }

    public interface ITasq<in TResponse> : ITasq { }

    public interface ITasq<TKey, in TResponse> : ITasq<TResponse> { }
}
