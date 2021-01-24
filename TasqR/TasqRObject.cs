using System;
using System.Linq;
using System.Text;

namespace TasqR
{
    public class TasqRObject : ITasqR
    {
        private readonly ITasqHandlerCollection handlerCollection;

        public TasqRObject(ITasqHandlerCollection handlerCollection)
        {
            this.handlerCollection = handlerCollection;
        }

        public void Run(ITasq tasq)
        {
            var _tt = tasq.GetType();
            var _t = handlerCollection.TasqHanders[_tt];

            var _a = (IJobTasqHandler)handlerCollection.GetService(_t);

            if (_a == null)
            {
                throw new Exception($"Type {GetFullName(_t)} not registered");
            }

            _a.Initialize(tasq);

            _a.BeforeRun(tasq);

            if (_t.GetGenericArguments().Length == 3)
            {
                while (_a.ReadKey(out object key))
                {
                    _a.Run(key, tasq);
                }
            }
            else
            {
                _a.Run(null, tasq);
            }

            _a.AfterRun(tasq);

            _a.Dispose();
        }

        public TResponse Run<TResponse>(ITasq<TResponse> tasq)
        {
            var _t = typeof(IJobTasqHandler<,>).MakeGenericType(tasq.GetType(), typeof(TResponse));

            var _a = (IJobTasqHandler)handlerCollection.GetService(_t);

            if (_a == null)
            {
                throw new Exception($"Type {GetFullName(_t)} not registered");
            }

            _a.Initialize(tasq);

            _a.BeforeRun(tasq);

            var retVal = (TResponse)_a.Run(null, tasq);

            _a.AfterRun(tasq);

            _a.Dispose();

            return retVal;
        }

        public TResponse Run<TKey, TResponse>(ITasq<TKey, TResponse> tasq)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        /// See <see href=" https://stackoverflow.com/a/1533349" /> for reference
        private string GetFullName(Type t)
        {
            if (!t.IsGenericType)
                return t.Name;
            StringBuilder sb = new StringBuilder();

            sb.Append(t.Name.Substring(0, t.Name.LastIndexOf("`")));
            sb.Append(t.GetGenericArguments().Aggregate("<",

                delegate (string aggregate, Type type)
                {
                    return aggregate + (aggregate == "<" ? "" : ",") + GetFullName(type);
                }
                ));
            sb.Append(">");

            return sb.ToString();
        }
    }
}
