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

        (Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure, Resistance? GasResistance) lastAtmosphericConditions;

        public MainAppController(ITankLevelHardware hardware, TankSpecs storageConfig)
        {
            Resolver.Log.Info("Initialize MainAppController...");

            Hardware = hardware;

            iotHubManager = new IotHubManager();

            tankLevelSensor = new TankLevelMonitor(hardware, storageConfig);
            tankLevelSensor.Updated += StorageContainerUpdated;

            if (hardware.ProjectLab.Display is { } display)
            {
                displayController = new DisplayController(display);
            }

            if (hardware.ProjectLab.EnvironmentalSensor is { } bme688)
            {
                bme688.Updated += Bme688Updated;
            }
        }

        private async void Bme688Updated(object sender, IChangeResult<(Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure, Resistance? GasResistance)> e)
        {
            Resolver.Log.Info($"BME688: {(int)e.New.Temperature?.Celsius}°C - {(int)e.New.Humidity?.Percent}% - {(int)e.New.Pressure?.Millibar}mbar");

            await iotHubManager.SendEnvironmentalReading(e.New);
            displayController.AtmosphericConditions = e.New;
        }

        private async void StorageContainerUpdated(object sender, IChangeResult<Volume> result)
        {
            Resolver.Log.Info($"Distance Sensor: {tankLevelSensor.DistanceToTopOfLiquid.Centimeters:n2}cm");
            Resolver.Log.Info($"Storage container: {result.New.Liters:n2}liters.");
            Resolver.Log.Info($"fill percent: {(int)(tankLevelSensor.FillPercent * 100)}%");

            await iotHubManager.SendVolumeReading(result.New);
            displayController.VolumePercent = (int)(tankLevelSensor.FillPercent * 100);
        }

        public Task Run()
        {
            if (Hardware.ProjectLab.EnvironmentalSensor is { } bme688)
            {
                bme688.StartUpdating(TimeSpan.FromSeconds(5));
            }

            Resolver.Log.Info("Starting storage container update.");
            tankLevelSensor.StartUpdating(TimeSpan.FromSeconds(1));

            displayController.Update();

            return Task.CompletedTask;
        }
    }
}