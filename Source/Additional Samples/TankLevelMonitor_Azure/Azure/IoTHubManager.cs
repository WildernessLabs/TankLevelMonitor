using Meadow;
using Meadow.Units;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace TankLevelMonitor_Azure.Azure;

/// <summary>
/// You'll need to create an IoT Hub - https://learn.microsoft.com/en-us/azure/iot-hub/iot-hub-create-through-portal
/// Create a device within your IoT Hub
/// And then generate a SAS token - this can be done via the Azure CLI 
/// 
/// Example
/// az iot hub generate-sas-token
/// --hub-name HUB_NAME 
/// --device-id DEVICE_ID 
/// --resource-group RESOURCE_GROUP 
/// --login [Open Shared access policies -> Select iothubowner -> copy Primary connection string]
/// </summary>
public class IotHubManager
{
    string IOT_HUB_NAME = Secrets.HUB_NAME;
    string IOT_HUB_DEVICE_ID = Secrets.DEVICE_ID;
    string IOT_HUB_SAS_TOKEN = Secrets.SAS_TOKEN;

    IMqttClient mqttClient;

    public bool isAuthenticated { get; private set; }

    public IotHubManager() { }

    public async Task<bool> Initialize()
    {
        try
        {
            Resolver.Log.Info("Create connection factory...");
            var factory = new MqttFactory();

            Resolver.Log.Info("Create MQTT client...");
            mqttClient = factory.CreateMqttClient();

            var iotHubUri = $"{IOT_HUB_NAME}.azure-devices.net";

            var username = $"{IOT_HUB_NAME}.azure-devices.net/{IOT_HUB_DEVICE_ID}/api-version=2021-04-12";

            Resolver.Log.Info("Creating MQTT options ...");
            var options = new MqttClientOptionsBuilder()
                .WithClientId(IOT_HUB_DEVICE_ID)
                .WithTcpServer(iotHubUri, 8883)
                .WithCredentials(username, IOT_HUB_SAS_TOKEN)
                .WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V311)
                .WithTls(new MqttClientOptionsBuilderTlsParameters
                {
                    UseTls = true,
                    SslProtocol = SslProtocols.Tls12,
                })
                .Build();


            Resolver.Log.Info("Connecting...");
            await mqttClient.ConnectAsync(options, new System.Threading.CancellationToken());

            isAuthenticated = true;
            return true;
        }
        catch (Exception ex)
        {
            Resolver.Log.Info($"{ex.Message}");
            isAuthenticated = false;
            return false;
        }
    }

    public async Task SendEnvironmentalReading(Volume volume)
    {
        try
        {
            string messagePayload = $"" +
                $"{{" +
                $"\"Volume\":{volume.Milliliters.ToString("F1")}" +
                $"}}";

            Resolver.Log.Info("Create message");
            var mqttMessage = new MqttApplicationMessageBuilder()
                .WithTopic($"devices/{IOT_HUB_DEVICE_ID}/messages/events/")
                .WithPayload(messagePayload)
                .Build();

            await mqttClient.PublishAsync(mqttMessage, new System.Threading.CancellationToken());

            Resolver.Log.Info($"MQTT - DATA SENT - " +
                $"Volume: {volume.Milliliters:n1}ml");
        }
        catch (Exception ex)
        {
            Resolver.Log.Info($"-- D2C Error - {ex.Message} --");
        }
    }
}