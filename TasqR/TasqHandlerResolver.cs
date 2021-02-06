using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TasqR
{
    public class TasqHandlerResolver : ITasqHandlerResolver
    {
        private Dictionary<Type, Type> TasqHanders { get; } = new Dictionary<Type, Type>();

        protected virtual object GetService(Type type)
        {
            return Activator.CreateInstance(type);
        }

        public virtual IJobTasqHandler ResolveHandler<TTasq>() where TTasq : ITasq
        {
            return ResolveHandler(typeof(TTasq));
        }

        public virtual IJobTasqHandler ResolveHandler(Type type)
        {
            if (!TasqHanders.ContainsKey(type))
            {
                throw new TasqException($"Type {GetFullName(type)} not registered");
            }

            var tasqHandlerType = TasqHanders[type];

            var tasqHandlerInstance = (IJobTasqHandler)GetService(tasqHandlerType);

            if (tasqHandlerInstance == null)
            {
                throw new TasqException($"Type {GetFullName(tasqHandlerType)} not registered");
            }

            return tasqHandlerInstance;
        }

        private string GetFullName(Type t)
        {
            if (!t.IsGenericType)
                return t.Name;
            var sb = new StringBuilder();

            sb.Append(t.Name.Substring(0, t.Name.LastIndexOf("`")));
            sb.Append
                (
                    t.GetGenericArguments().Aggregate
                        (
                            "<",
                            delegate (string aggregate, Type type)
                            {
                                return aggregate + (aggregate == "<" ? "" : ",") + GetFullName(type);
                            }
                        )
                );

            sb.Append(">");

            return sb.ToString();
        }

        public void Register(Type tasq, Type handler)
        {
            TasqHanders[tasq] = handler;
        }
    }
}
