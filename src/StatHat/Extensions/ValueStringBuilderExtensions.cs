using System.Text;
using System.Text.Encodings.Web;

namespace StatHat;

internal static class ValueStringBuilderExtensions
{
    public static void WriteQuoted(this ref ValueStringBuilder output, string text, bool encode = true)
    {
        output.Append('"');

        if (encode)
        {
            output.Append(JavaScriptEncoder.Default.Encode(text));
        }
        else
        {
            output.Append(text);
        }
        output.Append('"');
    }

    public static void WriteJsonProperty(this ref ValueStringBuilder output, string name, string value)
    {
        output.WriteQuoted(name, false);
        output.Append(':');
        output.WriteQuoted(value);
    }

    public static void WriteJsonProperty(this ref ValueStringBuilder output, string name, int value)
    {
        output.WriteQuoted(name, false);
        output.Append(':');
        output.AppendSpanFormattable(value);
    }

    public static void WriteJsonProperty(this ref ValueStringBuilder output, string name, double value)
    {
        output.WriteQuoted(name, false);
        output.Append(':');
        output.AppendSpanFormattable(value);
    }
}
