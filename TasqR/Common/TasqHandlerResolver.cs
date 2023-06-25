using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TasqR.Common
{
    public class TasqHandlerResolver : ITasqHandlerResolver
    {
        private readonly TasqAssemblyCollection p_TasqAssemblyCollection;

        public IEnumerable<TypeTasqReference> RegisteredReferences => p_TasqAssemblyCollection.TasqHanders.Select(a => a.Value);

        public TasqHandlerResolver(TasqAssemblyCollection tasqAssemblyCollection)
        {
            p_TasqAssemblyCollection = tasqAssemblyCollection;
        }

        public virtual object GetService(TypeTasqReference typeTasqReference)
        {
            return Activator.CreateInstance(typeTasqReference.HandlerImplementation);
        }

        public virtual TasqHandlerDetail ResolveHandler(Type type)
        {
            if (!p_TasqAssemblyCollection.TasqHanders.ContainsKey(type))
            {
                throw new TasqException($"Type {GetFullName(type)} not registered");
            }

            var tasqHandlerType = p_TasqAssemblyCollection.TasqHanders[type];

            var tasqHandlerInstance = (ITasqHandler)GetService(tasqHandlerType);

            if (tasqHandlerInstance == null)
            {
                throw new TasqException($"Type {GetFullName(tasqHandlerType.HandlerImplementation)} not registered");
            }

            return new TasqHandlerDetail
            {
                Handler = tasqHandlerInstance,
                Reference = tasqHandlerType
            };
        }

        protected string GetFullName(Type t)
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
    }
}
