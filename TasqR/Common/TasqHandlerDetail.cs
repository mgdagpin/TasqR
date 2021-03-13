using System;
using System.Reflection;

namespace TasqR.Common
{
    public class TasqHandlerDetail
    {
        public ITasqHandler Handler { get; set; }
        public TypeTasqReference Reference { get; set; }


        public static TasqHandlerDetail TryGetFromType(Type type, ITasqHandlerResolver handlerResolver)
        {
            if (!TypeTasqReference.IsValidHandler(type))
            {
                return null;
            }

            var reference = TypeTasqReference.Resolve(type);

            return new TasqHandlerDetail
            {
                Handler = (ITasqHandler)handlerResolver.GetService(reference),
                Reference = reference
            };
        }
    }
}
