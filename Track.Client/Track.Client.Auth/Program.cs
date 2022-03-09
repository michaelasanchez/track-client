﻿using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json.Nodes;
using Track.Broker.Clients;
using Track.Broker.Models;
using Track.Broker.Models.Api;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();


var settings = config.GetRequiredSection("trackSettings").Get<TrackClientSettings>();
var mqttBrokerAddress = config.GetRequiredSection("mqttBrokerAddress").Get<string>();

// create client instance
MqttClient client = new MqttClient(mqttBrokerAddress);

// register to message received
client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

string clientId = Guid.NewGuid().ToString();
client.Connect(clientId);

try
{
    client.Subscribe(new string[] { "app/#" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
}
catch (Exception ex)
{
    Console.WriteLine($"[Failed to connect to MQTT broker]: {ex.Message}");
}

Console.WriteLine("[Connected to MQTT broker]");

void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
{
    var @event = new MqttEvent(e);
    // handle message received
    string message = Encoding.UTF8.GetString(e.Message);

    var topic = new MqttTopic(e.Topic);

    if (@event.Type == "app")
    {
        switch (@event.Target)
        {
            case "track":
                ExecuteTrackRequest(@event);
                break;
        }
    }

    Console.WriteLine($"[Message Received]: {@event.Topic} - {@event.Message}");

}

async void ExecuteTrackRequest(MqttEvent @event)
{
    TrackClient trackClient = new TrackClient(settings);

    switch (@event.Request[0])
    {
        case "record":
            {
                var json = JsonNode.Parse(@event.Message) ?? throw new ArgumentNullException(nameof(@event.Message));

                var props = new Dictionary<int, decimal?>();

                foreach (var prop in json.AsObject())
                {
                    props.Add(int.Parse(prop.Key), prop.Value.GetValue<decimal>());
                }

                //var properties = json.AsObject()
                //    .ToDictionary(x => int.Parse(x.Key), x => (decimal?)decimal.Parse(x.Value?.GetValue<decimal>() ?? throw new ArgumentNullException()));

                //var properties = json.AsObject()
                //    .ToDictionary(x => int.Parse(x.Key), x => (decimal?)(x.Value.GetValue<decimal>() ?? throw new ArgumentNullException()));

                var payload = new CreateRecordPayload
                {
                    RecordDate = DateTime.UtcNow,
                    Properties = props
                };

                var datasetIdIsValid = int.TryParse(@event.Request[1], out var datasetId);

                if (datasetIdIsValid)
                {
                    await trackClient.CreateRecordAsync(datasetId, payload);
                }

                break;
            }
    }
}