using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TasqR.Common
{
    public class TasqAssemblyCollection
    {
        public Dictionary<Type, TypeTasqReference> TasqHanders { get; } = new Dictionary<Type, TypeTasqReference>();

        public virtual void RegisterFromAssembly(params Assembly[] assemblies)
        {
            foreach (var handler in GetAllHandlers(assemblies))
            {
                Register(TypeTasqReference.Resolve(handler));
            }
        }

        public virtual void Register(TypeTasqReference handler)
        {
            TasqHanders[handler.TasqProcess] = handler;
        }

        public virtual void Register<THandler>() where THandler : ITasqHandler
        {
            Register(TypeTasqReference.Resolve<THandler>());
        }

        public IEnumerable<Type> GetAllHandlers(params Assembly[] assemblies)
        {
            var handlers = new List<Type>();

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
