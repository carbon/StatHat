using System.Buffers;

using StatHat.Models;

using Xunit;

namespace StatHat.Tests;

public class EZRequestTests
{
    /*
    [Fact]
    public void SerializeEZStat()
    {
        var a = new EZRequest("key", new[] { new EZStat("counter", 1) });

        var json = JsonObject.FromObject(a);

        Assert.Equal(@"{""ezkey"":""key"",""data"":[{""stat"":""counter"",""count"":1}]}", json.ToString(false));
        Assert.Equal(@"{""ezkey"":""key"",""data"":[{""stat"":""counter"",""count"":1}]}", a.ToString());
    }
    */

    [Fact]
    public void SerializeMultipleEZTests()
    {
        var a = new EZRequest("a", new[] { new EZStat("counter", 0, 1), new EZStat("counter", 0, 2) });

        Assert.Equal("""{"ezkey":"a","data":[{"stat":"counter","value":1},{"stat":"counter","value":2}]}""", a.ToString());
    }


    [Fact]
    public void SerializeUtf8Bytes()
    {
        var a = new EZRequest("a", new[] { new EZStat("counter", 0, 1), new EZStat("counter", 0, 2) });

        Assert.Equal("""{"ezkey":"a","data":[{"stat":"counter","value":1},{"stat":"counter","value":2}]}"""u8.ToArray(), a.SerializeToUtf8Bytes());
    }


    [Fact]
    public void SerializeUtf8BytesLoad()
    {
        var b = """{"ezkey":"thekey","data":[{"stat":"counter","value":1},{"stat":"counter","value":2}]}"""u8.ToArray();

        var request = new EZRequest("thekey", new[] { new EZStat("counter", 0, 1), new EZStat("counter", 0, 2) });

        for (int i = 0; i < 1_000_000; i++)
        {
            Assert.True(b.SequenceEqual(request.SerializeToUtf8Bytes()));
        }
    }

    [Fact]
    public void SerializeEnumerable()
    {
        var a = new EZRequest("a", Stats().ToArray());

        Assert.Equal("""{"ezkey":"a","data":[{"stat":"counter","count":1},{"stat":"counter","count":2}]}""", a.ToString());
    }

    private static IEnumerable<EZStat> Stats()
    {
        yield return new EZStat("counter", 1);
        yield return new EZStat("counter", 2);
    }
}