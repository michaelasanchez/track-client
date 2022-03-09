using System.Text;
using System.Text.Json.Nodes;
using Track.Broker.Clients.Interfaces;
using Track.Broker.Models;
using Track.Broker.Models.Api;

namespace Track.Broker.Clients
{


    internal class TrackClient : ITrackClient
    {
        private readonly TrackClientSettings _settings;

        private readonly HttpClient _httpClient;

        private string ApiUrl => $"https://{_settings.ApiDomain}:{_settings.ApiPort}/{_settings.ApiPath}";

        public TrackClient(TrackClientSettings settings)
        {
            _settings = settings;
            _httpClient = new HttpClient();
        }

        public async Task<HttpResponseMessage> CreateRecordAsync(int datasetId, CreateRecordPayload payload)
        {
            var props = new JsonObject();

            foreach (var prop in payload.Properties)
            {
                props[prop.Key.ToString()] = prop.Value;
            }

            var req = new JsonObject
            {
                ["dateTime"] = payload.RecordDate.ToString("O"),
                ["properties"] = props
            };

            var content = new StringContent(req.ToJsonString(),
                                            Encoding.UTF8,
                                            "application/json");

            return await _httpClient.PostAsync(ApiUrl + $"record/{datasetId}", content);
        }
    }
}
