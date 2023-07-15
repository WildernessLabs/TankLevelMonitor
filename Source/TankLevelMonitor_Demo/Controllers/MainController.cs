using Meadow;
using Meadow.Units;
using System;
using System.Threading.Tasks;
using WildernessLabs.Hardware.TankLevelMonitor;

namespace TankLevelMonitor_Demo
{
    public class MainAppController
    {
        DisplayController displayController;

        protected ITankLevelHardware Hardware { get; set; }

        TankLevelMonitor tankLevelSensor;

        public MainAppController(ITankLevelHardware hardware, TankSpecs storageConfig)
        {
            Resolver.Log.Info("Initialize MainAppController...");

            Hardware = hardware;

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

        private void Bme688Updated(object sender, IChangeResult<(Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure, Resistance? GasResistance)> e)
        {
            Resolver.Log.Info($"BME688: {(int)e.New.Temperature?.Celsius}°C - {(int)e.New.Humidity?.Percent}% - {(int)e.New.Pressure?.Millibar}mbar");
            if (displayController != null)
            {
                displayController.AtmosphericConditions = e.New;
            }
        }

        private void StorageContainerUpdated(object sender, IChangeResult<Volume> result)
        {
            Resolver.Log.Info($"Distance Sensor: {tankLevelSensor.DistanceToTopOfLiquid.Centimeters:n2}cm");
            Resolver.Log.Info($"Storage container: {result.New.Liters:n2}liters.");
            Resolver.Log.Info($"fill percent: {(int)(tankLevelSensor.FillPercent * 100)}%");
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

            if (displayController != null)
            {
                displayController.Update();
            }

            return Task.CompletedTask;
        }
    }
}