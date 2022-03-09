namespace Track.Broker.Models
{
    internal class MqttTopic
    {
        public string Type { get; }
        public string Target { get; }
        public string[] Request { get; }

        public MqttTopic(string topicString)
        {
            if (!string.IsNullOrEmpty(topicString))
            {
                var topicParts = topicString.Split('/');

                if (topicParts.Length >= 3)
                {
                    Type = topicParts[0];
                    Target = topicParts[1];
                    Request = topicParts[2..];
                }
            }
        }
    }
}
