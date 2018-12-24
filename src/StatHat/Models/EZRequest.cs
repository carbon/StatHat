using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace StatHat.Models
{
    public readonly struct EZRequest
    {
        public EZRequest(string key, IEnumerable<EZStat> data)
        {
            Key = key;
            Data = data;
        }

        [DataMember(Name = "ezkey", IsRequired = true)]
        public string Key { get; }

        [DataMember(Name = "data", IsRequired = true)]
        public IEnumerable<EZStat> Data { get; }

        public override string ToString()
        {
            using (var writer = new StringWriter())
            {
                WriteTo(writer);

                return writer.ToString();
            }
        }
        
        public void WriteTo(TextWriter writer)
        {
            writer.Write('{');

            writer.WriteJsonProperty("ezkey", Key);
            writer.Write(',');
            writer.WriteQuoted("data");
            writer.Write(':');
            writer.Write('[');

            int i = 0;

            foreach (var stat in Data)
            {
                if (i > 0) writer.Write(',');

                stat.WriteTo(writer);

                i++;
            }

            writer.Write(']');

            writer.Write('}');
        }
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