using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TasqR.Processing
{
    public class ParameterDictionary<T> : Dictionary<string, T> where T : Parameter
    {
        public new T this[string key]
        {
            get
            {
                if (!ContainsKey(key))
                {
                    return null;
                }

                return base[key];
            }
            set
            {
                T newParam = value;

                if (newParam != null && newParam.Name == null)
                {
                    newParam.Name = key;
                }

                base[key] = newParam;
            }
        }

        public ParameterDictionary() : base(StringComparer.OrdinalIgnoreCase) { }

        public ParameterDictionary(params T[] args) : base(StringComparer.OrdinalIgnoreCase)
        {
            if (args != null)
            {
                foreach (var parameter in args)
                {
                    this[parameter.Name] = parameter;
                }
            }
        }

        public ParameterDictionary(IEnumerable<T> parameters) : base(StringComparer.OrdinalIgnoreCase)
        {
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    this[parameter.Name] = parameter;
                }
            }
        }

        public virtual TType GetAs<TType>(string key)
        {
            var data = this[key];

            if (data == null)
            {
                return default;
            }

            if (string.IsNullOrWhiteSpace(data.Value?.ToString()))
            {
                return default;
            }

            // https://stackoverflow.com/a/2961702/403971
            try
            {
                var converter = TypeDescriptor.GetConverter(typeof(TType));
                if (converter != null)
                {
                    // Cast ConvertFromString(string text) : object to (TType)
                    return (TType)converter.ConvertFromString(data.Value?.ToString());
                }
                return default;
            }
            catch (NotSupportedException)
            {
                return default;
            }
        }

        public virtual bool Exists(string key) => ContainsKey(key);
    }
}
