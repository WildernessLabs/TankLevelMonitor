using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Sensors.Distance;
using Meadow.Logging;
using Meadow.Units;
using System;
using System.Threading.Tasks;
using TankLevelMonitor.Hardware;
using TankLevelMonitor.Models;

namespace TankLevelMonitor.Controllers
{
    public class MainAppController
    {
        protected Logger? Logger { get; } = Resolver.Log;

        DisplayController displayController;

        IProjectLabHardware Hardware { get; set; }

        TankLevelSensor tankLevelSensor;

        public MainAppController(TankContainerConfig storageConfig)
        {
            Logger?.Info("Initialize MainAppController...");

            Hardware = ProjectLab.Create();

            var vl53L0X = new Vl53l0x(Hardware.I2cBus);
            tankLevelSensor = new TankLevelSensor(vl53L0X, storageConfig);
            tankLevelSensor.Updated += StorageContainerUpdated;

            if (Hardware.Display is { } display)
            {
                displayController = new DisplayController(display);
            }

            if (Hardware.EnvironmentalSensor is { } bme688)
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
            Logger?.Info($"Distance Sensor: {tankLevelSensor.DistanceToTopOfLiquid.Centimeters:n2}cm");
            Logger?.Info($"Storage container: {result.New.Liters:n2}liters.");
            Logger?.Info($"fill percent: {(int)(tankLevelSensor.FillPercent * 100)}%");
            displayController.VolumePercent = (int)(tankLevelSensor.FillPercent * 100);
        }

        public Task Run()
        {
            if (Hardware.EnvironmentalSensor is { } bme688)
            {
                bme688.StartUpdating(TimeSpan.FromSeconds(5));
            }

            Logger?.Info("Starting storage container update.");
            tankLevelSensor.StartUpdating(TimeSpan.FromSeconds(1));

            if (displayController != null)
            {
                displayController.Update();
            }

            return Task.CompletedTask;
        }
    }
}