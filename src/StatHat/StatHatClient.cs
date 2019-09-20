using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Carbon.Metrics;

using StatHat.Models;

namespace StatHat
{
    public sealed class StatHatClient : IMetricStore
    {
        private const string endpoint = "https://api.stathat.com/ez";

        private readonly HttpClient httpClient = new HttpClient {
            Timeout = TimeSpan.FromSeconds(2)
        };

        private readonly string key;

        public StatHatClient(string key)
        {
            this.key = key ?? throw new ArgumentNullException(nameof(key));
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
            var ezRequest = new EZRequest(key, data);

            var request = new HttpRequestMessage(HttpMethod.Post, endpoint) {
                Content = new StringContent(ezRequest.ToString(), Encoding.UTF8, "application/json")
            };

            // StatHat doesn't reconize the content-type with a character set
            request.Content.Headers.ContentType.CharSet = null;

            using var response = await httpClient.SendAsync(request).ConfigureAwait(false);

            return response.IsSuccessStatusCode;
        }
    }
}