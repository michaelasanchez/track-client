using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Track.Broker.Models
{
    internal class MqttEvent
    {
        public MqttTopic Topic { get; }

        public string Message { get; }

        public string Type => Topic.Type;
        public string Target => Topic.Target;
        public string[] Request => Topic.Request;

        public MqttEvent(MqttMsgPublishEventArgs args)
        {
            Topic = new MqttTopic(args.Topic);

            Message = Encoding.UTF8.GetString(args.Message);
        }
    }
}
