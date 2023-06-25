using System;

namespace TasqR.Processing
{
    public class Parameter
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string DataType { get; set; }
        public bool IsRequired { get; set; }
        public int? MaxLength { get; set; }

        public Parameter() { }

        public Parameter(string name, object value)
        {
            Name = name;
            Value = value?.ToString();
        }

        public bool IsValid()
        {
            //bool retVal = false;

            //if (IsRequired && string.IsNullOrWhiteSpace(Value))
            //{
            //    return false;
            //}

            //if (!IsRequired && string.IsNullOrWhiteSpace(Value))
            //{
            //    return true;
            //}

            //switch (DataType)
            //{
            //    case InputDataTypeConstants.SmallInt:
            //        return short.TryParse(Value, out _);

            //    case InputDataTypeConstants.Int:
            //        return int.TryParse(Value, out _);

            //    case InputDataTypeConstants.Date:
            //    case InputDataTypeConstants.DateTime:
            //        return DateTime.TryParse(Value, out _);

            //    case InputDataTypeConstants.String:
            //        if (MaxLength.HasValue && Value != null && Value.Trim().Length > MaxLength)
            //        {
            //            return false;
            //        }

            //        return true;
            //    default:
            //        break;
            //}

            //return retVal;
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            string value = Value;

            if (value == null)
            {
                value = "Nothing passed";
            }
            else if (value.Trim() == "")
            {
                value = "Passed with empty string";
            }

            return $"{Name}: {value}";
        }

        public static implicit operator Parameter(string data) => new Parameter(null, data);
    }
}
