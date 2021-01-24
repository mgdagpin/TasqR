using System;
using System.Collections.Generic;

namespace TasqR
{
    public interface ITasqHandlerCollection
    {
        Dictionary<Type, Type> TasqHanders { get; }
        IEnumerable<TypeTasqReference> TypeReferences { get; }

        object GetService(Type type);
    }
}
