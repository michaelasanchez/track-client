using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Track.Broker.Models;

namespace Track.Client.Auth.Models
{
    internal class TrackMqttRequest : MqttRequest
    {
        public string Entity { get; set; }

        public string[] Args { get; set; }

        public TrackMqttRequest(string[] requestParts)
            : base(requestParts)
        {
            Entity = requestParts[0];
            Args = requestParts[1..];
        }
    }
}
