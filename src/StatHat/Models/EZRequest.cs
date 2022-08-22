using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;

namespace StatHat.Models;

public readonly struct EZRequest
{
    public EZRequest(string key, IEnumerable<EZStat> data)
    {
        Key = key;
        Data = data;
    }

    [JsonPropertyName("ezkey")]
    public string Key { get; }

    [JsonPropertyName("data")]
    public IEnumerable<EZStat> Data { get; }

    [SkipLocalsInit]
    public override string ToString()
    {
        var sb = new ValueStringBuilder(stackalloc char[256]);

        WriteTo(ref sb);

        return sb.ToString();
    }


    [SkipLocalsInit]
    public byte[] SerializeToUtf8Bytes()
    {
        var sb = new ValueStringBuilder(stackalloc char[256]);

        try
        {
            WriteTo(ref sb);

            var buffer = ArrayPool<byte>.Shared.Rent(Encoding.UTF8.GetMaxByteCount(sb.Length));

            int count = Encoding.UTF8.GetBytes(sb.AsSpan(), buffer);

            var result = buffer.AsSpan(0, count).ToArray();

            ArrayPool<byte>.Shared.Return(buffer);

            return result;
        }
        finally
        {
            sb.Dispose();
        }
    }

    internal void WriteTo(ref ValueStringBuilder sb)
    {
        sb.Append('{');

        sb.WriteJsonProperty("ezkey", Key);
        sb.Append(',');
        sb.WriteQuoted("data");
        sb.Append(':');
        sb.Append('[');

        int i = 0;

        foreach (var stat in Data)
        {
            if (i > 0)
            {
                sb.Append(',');
            }

            stat.WriteTo(ref sb);

            i++;
        }

        sb.Append(']');

        sb.Append('}');
    }
}

/*
{
  "ezkey": "XXXYYYZZZ",
  "data": [
      {"stat": "page view", "count": 2},
      {"stat": "messages sent~total,female,europe", "count": 1},
      {"stat": "request time~total,messages", "value": 23.094},
      {"stat": "ws0: load average", "value": 0.732, "t": 1363118126},
  ]
}
*/