using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;

using Carbon.Metrics;

using StatHat.Models;

namespace StatHat;

public sealed class StatHatClient : IMetricStore
{
    private const string endpoint = "https://api.stathat.com/ez";

    private readonly HttpClient _httpClient = new() {
        Timeout = TimeSpan.FromSeconds(2)
    };

    private readonly string _key;

    public StatHatClient(string key)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);

        _key = key;
    }

    public ValueTask<bool> PutAsync(Measurement stat)
    {
        return PutAsync([stat]);
    }

    public Task PutAsync(params EZStat[] stats)
    {
        return SendAsync(stats);
    }

    public async ValueTask<bool> PutAsync(IReadOnlyList<Measurement> stats)
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

    private static EZStat[] Transform(IReadOnlyList<Measurement> measurements)
    {
        var result = new EZStat[measurements.Count];

        for (int i = 0; i < measurements.Count; i++)
        {
            result[i] = new EZStat(measurements[i]);
        }

        return result;
    }

    private async Task<bool> SendAsync(EZStat[] data)
    {
        var ezRequest = new EZRequest(_key, data);

        var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = new ByteArrayContent(ezRequest.SerializeToUtf8Bytes()) {
                Headers = { { "Content-Type", MediaTypeNames.Application.Json } }
            }
        };

        using var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

        return response.IsSuccessStatusCode;
    }
}