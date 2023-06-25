using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TasqR.Processing
{
    public class ParameterDictionary : Dictionary<string, Parameter>
    {
        public new Parameter this[string key]
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
                Parameter newParam = value;

                if (newParam.Name == null)
                {
                    newParam.Name = key;
                }

                base[key] = newParam;
            }
        }

        public ParameterDictionary() : base(StringComparer.OrdinalIgnoreCase) { }

        public ParameterDictionary(params Parameter[] args) : base(StringComparer.OrdinalIgnoreCase)
        {
            if (args != null)
            {
                foreach (var parameter in args)
                {
                    this[parameter.Name.ToLower()] = parameter;
                }
            }
        }

        public ParameterDictionary(IEnumerable<Parameter> parameters) : base(StringComparer.OrdinalIgnoreCase)
        {
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    this[parameter.Name.ToLower()] = parameter;
                }
            }
        }

        public virtual T GetAs<T>(string key)
        {
            var data = this[key];

            if (data == null)
            {
                return default(T);
            }

            if (string.IsNullOrWhiteSpace(data.Value))
            {
                return default(T);
            }

            // https://stackoverflow.com/a/2961702/403971
            try
            {
                var converter = TypeDescriptor.GetConverter(typeof(T));
                if (converter != null)
                {
                    // Cast ConvertFromString(string text) : object to (T)
                    return (T)converter.ConvertFromString(data.Value);
                }
                return default(T);
            }
            catch (NotSupportedException)
            {
                return default(T);
            }
        }

        public virtual bool Exists(string key) => ContainsKey(key);
    }
}
