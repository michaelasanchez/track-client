namespace Track.Broker.Models
{
    internal class MqttRequest
    {
        protected string[] _parts { get; set; }

        protected MqttRequest(string[] requestParts)
        {
            _parts = requestParts;
        }
    }
}
