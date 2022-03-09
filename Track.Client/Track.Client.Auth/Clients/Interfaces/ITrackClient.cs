using Track.Broker.Models.Api;

namespace Track.Broker.Clients.Interfaces
{
    internal interface ITrackClient
    {
        Task<HttpResponseMessage> CreateRecordAsync(int datasetId, CreateRecordPayload payload);
    }
}
