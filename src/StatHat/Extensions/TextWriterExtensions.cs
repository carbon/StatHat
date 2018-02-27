using System.IO;
using System.Text.Encodings.Web;

namespace StatHat
{
    internal static class TextWriterExtensions
    {
        public static void WriteQuoted(this TextWriter output, string text, bool encode = true)
        {
            output.Write('"');

            if (encode)
            {
                JavaScriptEncoder.Default.Encode(output, text);
            }
            else
            {
                output.Write(text);
            }
            output.Write('"');
        }

        public static void WriteJsonProperty(this TextWriter output, string name, string value)
        {
            output.WriteQuoted(name, false);
            output.Write(':');
            output.WriteQuoted(value);
        }

        public static void WriteJsonProperty(this TextWriter output, string name, int value)
        {
            output.WriteQuoted(name, false);
            output.Write(':');
            output.Write(value);
        }

        public static void WriteJsonProperty(this TextWriter output, string name, double value)
        {
            output.WriteQuoted(name, false);
            output.Write(':');
            output.Write(value);
        }
    }
}