using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasqR
{
    public interface ITasqHandlerCollection
    {
        Dictionary<Type, Type> TasqHanders { get; }

        object GetService(Type type);
    }
}
