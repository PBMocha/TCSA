using GenetecChallenge.N1.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace GenetecChallenge.N1.Services
{
    public class LicenseApiService
    {
        private readonly HttpClient _client;
        private readonly SecretsConfig _config;

        public LicenseApiService(SecretsConfig config, HttpClient client)
        {
            _client = client;
            _config = config;
        }

        public async Task<string> SendPlate(LicensePlatePayload lp)
        {
            var payload = JsonSerializer.Serialize(lp);
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_config.LicenseRoot}/lpr/platelocation");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", _config.ApiKey);
            request.Content = new StringContent(payload,Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);

            return response.StatusCode.ToString();
        }

        public async Task<IEnumerator<string>> GetWantedList()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_config.LicenseRoot}/lpr/wantedplates");

            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", _config.ApiKey);

            var response = await _client.SendAsync(request);

            return null;
        }
    }
}
