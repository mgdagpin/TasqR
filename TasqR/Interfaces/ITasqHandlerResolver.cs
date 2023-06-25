using System;
using System.Collections.Generic;
using System.Reflection;
using TasqR.Common;

namespace TasqR
{
    public interface ITasqHandlerResolver
    {
        IEnumerable<TypeTasqReference> RegisteredReferences { get; }


        TasqHandlerDetail ResolveHandler(Type type);

        object GetService(TypeTasqReference typeTasqReference);
    }
}
