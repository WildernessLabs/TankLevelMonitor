using Meadow;
using Meadow.Units;
using System;
using System.Threading.Tasks;
using TankLevelMonitor_Azure.Azure;
using WildernessLabs.Hardware.TankLevelMonitor;

namespace TankLevelMonitor_Azure
{
    public class MainAppController
    {
        DisplayController displayController;

        public IotHubManager iotHubManager { get; protected set; }

        protected ITankLevelHardware Hardware { get; set; }

        protected TankLevelMonitor tankLevelSensor { get; set; }

        public MainAppController(ITankLevelHardware hardware, TankSpecs storageConfig)
        {
            Resolver.Log.Info("Initialize MainAppController...");

            Hardware = hardware;

            iotHubManager = new IotHubManager();

            tankLevelSensor = new TankLevelMonitor(hardware, storageConfig);
            tankLevelSensor.Updated += StorageContainerUpdated;

            displayController = new DisplayController(hardware.ProjectLab.Display);

            hardware.ProjectLab.EnvironmentalSensor.Updated += Bme688Updated;
        }

        private async void Bme688Updated(object sender, IChangeResult<(Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure, Resistance? GasResistance)> e)
        {
            //Resolver.Log.Info($"Temperature: {(int)e.New.Temperature?.Celsius}°C | " +
            //    $"Pressure: {(int)e.New.Pressure?.Millibar}% | " +
            //    $"Humidity: {(int)e.New.Humidity?.Percent}mbar");

            displayController.AtmosphericConditions = e.New;

            await iotHubManager.SendEnvironmentalReading(e.New, tankLevelSensor.FillAmount);
        }

        private void StorageContainerUpdated(object sender, IChangeResult<Volume> result)
        {
            //Resolver.Log.Info($"Distance Sensor: {tankLevelSensor.DistanceToTopOfLiquid.Centimeters:n2}cm |" +
            //    $"Storage container: {result.New.Milliliters:n2}ml | " +
            //    $"fill percent: {(int)(tankLevelSensor.FillPercent * 100)}%");

            displayController.VolumePercent = (int)(tankLevelSensor.FillPercent * 100);
        }

        public Task Run()
        {
            Hardware.ProjectLab.EnvironmentalSensor.StartUpdating(TimeSpan.FromSeconds(1));

            tankLevelSensor.StartUpdating(TimeSpan.FromSeconds(1));

            displayController.Update();

            return Task.CompletedTask;
        }
    }
}