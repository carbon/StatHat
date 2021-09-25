using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

using Carbon.Metrics;

using StatHat.Models;

namespace StatHat;

public sealed class StatHatClient : IMetricStore
{
    private const string endpoint = "https://api.stathat.com/ez";

    private readonly HttpClient httpClient = new() {
        Timeout = TimeSpan.FromSeconds(2)
    };

    private readonly string _key;

    public StatHatClient(string key)
    {
        ArgumentNullException.ThrowIfNull(key);

        _key = key;
    }

    public Task<bool> PutAsync(Measurement stat)
    {
        return PutAsync(new[] { stat });
    }

    public Task PutAsync(params EZStat[] stats)
    {
        return SendAsync(stats);
    }

    public async Task<bool> PutAsync(IEnumerable<Measurement> stats)
    {
        try
        {
            return await SendAsync(Transform(stats)).ConfigureAwait(false);
        }
        catch
        {
            return false;
        }
    }

    private static IEnumerable<EZStat> Transform(IEnumerable<Measurement> stats)
    {
        foreach (var stat in stats)
        {
            yield return new EZStat(stat);
        }
    }

    private async Task<bool> SendAsync(IEnumerable<EZStat> data)
    {
        var ezRequest = new EZRequest(_key, data);

        var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = new StringContent(ezRequest.ToString(), Encoding.UTF8, MediaTypeNames.Application.Json)
        };

        // StatHat doesn't reconize the content-type with a character set
        request.Content.Headers.ContentType!.CharSet = null;

        using var response = await httpClient.SendAsync(request).ConfigureAwait(false);

        return response.IsSuccessStatusCode;
    }
}