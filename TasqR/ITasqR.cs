using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasqR
{
    public interface ITasqR
    {
        void Run(ITasq tasq);
        TResponse Run<TResponse>(ITasq<TResponse> tasq);
        TResponse Run<TKey, TResponse>(ITasq<TKey, TResponse> tasq);
    }
}
