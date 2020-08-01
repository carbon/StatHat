using System.IO;
using System.Runtime.Serialization;

using Carbon.Metrics;

namespace StatHat.Models
{
    public readonly struct EZStat
    {
        public EZStat(string name, int count = 0, double value = 0)
        {
            Name = name;
            Count = count;
            Value = value;
        }

        internal EZStat(Measurement measurement)
        {
            Name = measurement.Name!;

            if (measurement.Unit == UnitType.Milliseconds ||
                measurement.Unit == UnitType.Bytes)
            {
                Count = 0;
                Value = measurement.Value;
            }
            else
            {
                Count = (int)measurement.Value;
                Value = 0;
            }
        }

        [DataMember(Name = "stat")]
        public string Name { get; }

        [DataMember(Name = "count", EmitDefaultValue = false)]
        public int Count { get; }

        [DataMember(Name = "value", EmitDefaultValue = false)]
        public double Value { get; }

        // TODO: Timestamp (t)

        public void WriteTo(TextWriter writer)
        {
            writer.Write('{');
            writer.WriteJsonProperty("stat", Name);

            if (Count > 0)
            {
                writer.Write(',');
                writer.WriteJsonProperty("count", Count);
            }

            if (Value != 0)
            {
                writer.Write(',');
                writer.WriteJsonProperty("value", Value);
            }

            writer.Write('}');
        }
    }
}