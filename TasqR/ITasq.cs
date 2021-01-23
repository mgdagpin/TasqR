using System;

namespace TasqR
{
    public interface ITasq { }
    
    public interface ITasq<TResponse> : ITasq { }

    public interface ITasq<TKey, TResponse> : ITasq<TResponse> { }
}
