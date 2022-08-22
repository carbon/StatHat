using System.Text;
using System.Text.Json.Serialization;

using Carbon.Metrics;

namespace StatHat.Models;

public readonly struct EZStat
{
    public EZStat(string name, int count = 0, double value = 0)
    {
        Name = name;
        Count = count;
        Value = value;
    }

    internal EZStat(in Measurement measurement)
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

    [JsonPropertyName("stat")]
    public string Name { get; }

    [JsonPropertyName("count")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int Count { get; }

    [JsonPropertyName("value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public double Value { get; }

    // TODO: Timestamp (t)

    internal void WriteTo(ref ValueStringBuilder sb)
    {
        sb.Append('{');
        sb.WriteJsonProperty("stat", Name);

        if (Count > 0)
        {
            sb.Append(',');
            sb.WriteJsonProperty("count", Count);
        }

        if (Value != 0)
        {
            sb.Append(',');
            sb.WriteJsonProperty("value", Value);
        }

        sb.Append('}');
    }
}
