using Amqp;
using Meadow;
using Meadow.Units;
using System;
using System.Text;
using System.Threading.Tasks;

namespace TankLevelMonitor_Azure.Azure
{
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
        private const string HubName = Secrets.HUB_NAME;
        private const string SasToken = Secrets.SAS_TOKEN;
        private const string DeviceId = Secrets.DEVICE_ID;

        private Connection connection;
        private SenderLink sender;

        private int messageId = 0;

        public IotHubManager() { }

        public async Task Initialize()
        {
            string hostName = HubName + ".azure-devices.net";
            string userName = DeviceId + "@sas." + HubName;
            string senderAddress = "devices/" + DeviceId + "/messages/events";

            Resolver.Log.Info("Create connection factory...");
            var factory = new ConnectionFactory();

            Resolver.Log.Info("Create connection ...");
            connection = await factory.CreateAsync(new Address(hostName, 5671, userName, SasToken));

            Resolver.Log.Info("Create session ...");
            var session = new Session(connection);

            Resolver.Log.Info("Create SenderLink ...");
            sender = new SenderLink(session, "send-link", senderAddress);
        }

        public Task SendEnvironmentalReading((Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure, Resistance? GasResistance) reading)
        {
            try
            {
                string messagePayload = $"" +
                        $"{{" +
                        $"\"messageId\":{messageId++}," +
                        $"\"deviceId\":\"{Secrets.DEVICE_ID}\"," +
                        $"\"temperature\":{reading.Temperature.Value.Celsius}," +
                        $"\"humidity\":{reading.Humidity.Value.Percent}," +
                        $"\"pressure\":{reading.Pressure.Value.Millibar}" +
                        $"}}";

                var message = new Message(Encoding.UTF8.GetBytes(messagePayload));
                message.ApplicationProperties = new Amqp.Framing.ApplicationProperties();

                sender.Send(message, null, null);

                Resolver.Log.Info($"*** DATA SENT - Temperature - {reading.Temperature.Value.Celsius}, Humidity - {reading.Humidity.Value.Percent}, Pressure - {reading.Pressure.Value.Millibar} ***");
            }
            catch (Exception ex)
            {
                Resolver.Log.Info($"-- D2C Error - {ex.Message} --");
            }

            return Task.CompletedTask;
        }

        public Task SendVolumeReading(Volume reading)
        {
            try
            {
                string messagePayload = $"" +
                        $"{{" +
                        $"\"messageId\":{messageId++}," +
                        $"\"deviceId\":\"{Secrets.DEVICE_ID}\"," +
                        $"\"volume\":{reading.Milliliters}," +
                        $"}}";

                var message = new Message(Encoding.UTF8.GetBytes(messagePayload));
                message.ApplicationProperties = new Amqp.Framing.ApplicationProperties();

                sender.Send(message, null, null);

                Resolver.Log.Info($"*** DATA SENT - Volume - {reading.Milliliters} ***");
            }
            catch (Exception ex)
            {
                Resolver.Log.Info($"-- D2C Error - {ex.Message} --");
            }

            return Task.CompletedTask;
        }
    }
}