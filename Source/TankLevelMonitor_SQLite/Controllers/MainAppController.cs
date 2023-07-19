using Meadow;
using Meadow.Units;
using System;
using System.Threading.Tasks;
using TankLevelMonitor_Demo.SQLite.Database;
using TankLevelMonitor_Demo.SQLite.Models;
using WildernessLabs.Hardware.TankLevelMonitor;

namespace TankLevelMonitor_Demo
{
    public class MainAppController
    {
        readonly DisplayController displayController;

        protected ITankLevelHardware Hardware { get; set; }

        readonly TankLevelMonitor tankLevelSensor;

        AtmosphericConditions? atmosphericConditions;

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
            if (displayController != null)
            {
                atmosphericConditions = displayController.AtmosphericConditions = new AtmosphericConditions(e.New.Temperature, e.New.Humidity, e.New.Pressure, e.New.GasResistance);
            }
        }

        private void StorageContainerUpdated(object sender, IChangeResult<Volume> result)
        {
            Resolver.Log.Info($"Distance Sensor: {tankLevelSensor.DistanceToTopOfLiquid.Centimeters:n2}cm / Storage container: {result.New.Liters:n2}liters. / fill percent: {(int)(tankLevelSensor.FillPercent * 100)}%");
            displayController.VolumePercent = (int)(tankLevelSensor.FillPercent * 100);

            var reading = new TankLevelReading()
            {
                TankLevel = tankLevelSensor.DistanceToTopOfLiquid
            };

            if (atmosphericConditions is not null)
            {
                reading.Temperature = atmosphericConditions.Value.Temperature;
                reading.Pressure = atmosphericConditions.Value.Pressure;
                reading.Humidity = atmosphericConditions.Value.Humidity;
            }

            DatabaseManager.Instance.SaveReading(reading);
        }

        public Task Run()
        {
            if (Hardware.ProjectLab.EnvironmentalSensor is { } bme688)
            {
                bme688.StartUpdating(TimeSpan.FromSeconds(5));
            }

            Resolver.Log.Info("Starting storage container update.");
            tankLevelSensor.StartUpdating(TimeSpan.FromSeconds(5));

            displayController?.Update();

            return Task.CompletedTask;
        }
    }
}