using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TasqR.Common
{
    public class TasqHandlerResolver : ITasqHandlerResolver
    {
        public Dictionary<Type, TypeTasqReference> TasqHanders { get; } = new Dictionary<Type, TypeTasqReference>();
        public IEnumerable<TypeTasqReference> RegisteredReferences => TasqHanders.Select(a => a.Value);

        public virtual object GetService(TypeTasqReference typeTasqReference)
        {
            return Activator.CreateInstance(typeTasqReference.HandlerImplementation);
        }

        public virtual TasqHandlerDetail ResolveHandler<TTasq>() where TTasq : ITasq
        {
            return ResolveHandler(typeof(TTasq));
        }

        public virtual TasqHandlerDetail ResolveHandler(Type type)
        {
            if (!TasqHanders.ContainsKey(type))
            {
                throw new TasqException($"Type {GetFullName(type)} not registered");
            }

            var tasqHandlerType = TasqHanders[type];

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

        public virtual void Register(TypeTasqReference handler)
        {
            TasqHanders[handler.TasqProcess] = handler;
        }

        public virtual void Register<THandler>() where THandler : ITasqHandler
        {
            Register(TypeTasqReference.Resolve<THandler>());
        }

        public virtual void RegisterFromAssembly(params Assembly[] assemblies)
        {
            var handlers = GetAllHandlers(assemblies);

            foreach (var handler in handlers)
            {
                Register(TypeTasqReference.Resolve(handler));
            }
        }

        public IEnumerable<Type> GetAllHandlers(params Assembly[] assemblies)
        {
            List<Type> handlers = new List<Type>();

            var assembliesToScan = assemblies.Distinct().ToList();

            if (assembliesToScan.Count == 0)
            {
                assembliesToScan.Add(Assembly.GetExecutingAssembly());
            }

            foreach (var assembly in assembliesToScan)
            {
                var ttHandlers = assembly.DefinedTypes
                    .Where(t => TypeTasqReference.IsConcrete(t) && TypeTasqReference.IsValidHandler(t))
                    .Select(a => TypeTasqReference.Resolve(a))
                    .ToList();

                foreach (var ttHandler in ttHandlers)
                {
                    handlers.Add(ttHandler.HandlerImplementation);
                }
            }

            return handlers;
        }
    }
}
