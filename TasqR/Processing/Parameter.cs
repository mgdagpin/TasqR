using System;

namespace TasqR.Processing
{
    public class Parameter
    {
        public string Name { get; set; }
        public object DefaultValue { get; set; }
        public object Value { get; set; }
        public string DataType { get; set; }
        public bool IsRequired { get; set; }
        public int? MaxLength { get; set; }

        public virtual bool IsValid()
        {
            bool retVal = false;

            if (IsRequired && string.IsNullOrWhiteSpace(Value?.ToString()))
            {
                return false;
            }

            if (!IsRequired && string.IsNullOrWhiteSpace(Value?.ToString()))
            {
                return true;
            }

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

            return retVal;
        }

        public static implicit operator Parameter(string data) => new Parameter { Value = data };
    }
}
