using TasqR.Processing.Interfaces;

namespace TasqR.Processing
{
    public class TaskJobParameter
    {
        public string Name { get; set; }
        public string Display { get; set; }

        public string DefaultValue { get; set; }
        public string DataType { get; set; }
        public int? MaxLength { get; set; }

        public string DataSource { get; set; }

        public string Hint { get; set; }


        public bool IsRequired { get; set; }
        public short Order { get; set; }
    }
}
