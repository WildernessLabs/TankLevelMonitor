using Meadow;
using Meadow.Devices;
using Meadow.Units;
using System;
using System.Threading.Tasks;

namespace TankLevelMonitor.F7;

public class MainController
{
    private DisplayController displayController;

    protected ITankLevelHardware Hardware { get; set; }

    TankLevelSensors tankLevelSensor;

    public MainController(ITankLevelHardware hardware, TankSpecs storageConfig)
    {
        Resolver.Log.Info("Initialize MainAppController...");

        Hardware = hardware;

        tankLevelSensor = new TankLevelSensors(hardware, storageConfig);
        tankLevelSensor.Updated += StorageContainerUpdated;

        if (hardware.Display is { } display)
        {
            displayController = new DisplayController(display);
        }

        if ((Hardware as ProjectLabHardwareBase).AtmosphericSensor is { } bme688)
        {
            bme688.Updated += Bme688Updated;
        }
    }

    private void Bme688Updated(object sender, IChangeResult<(Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure, Resistance? GasResistance)> e)
    {
        Resolver.Log.Info($"BME688: " +
            $"Temperature: {(int)e.New.Temperature?.Celsius}°C - " +
            $"Humidity: {(int)e.New.Humidity?.Percent}% - " +
            $"Pressure: {(int)e.New.Pressure?.Millibar}mbar");

        if (displayController != null)
        {
            displayController.AtmosphericConditions = e.New;
        }
    }

    private void StorageContainerUpdated(object sender, IChangeResult<Volume> result)
    {
        Resolver.Log.Info($"TankLevelSensor: " +
            $"Distance Sensor: {tankLevelSensor.DistanceToTopOfLiquid.Centimeters:n2}cm | " +
            $"Storage container: {result.New.Liters:n2}l | " +
            $"Fill Percent: {(int)(tankLevelSensor.FillPercent * 100)}%");

        displayController.VolumePercent = (int)(tankLevelSensor.FillPercent * 100);
    }

    public Task Run()
    {
        if ((Hardware as ProjectLabHardwareBase).AtmosphericSensor is { } bme688)
        {
            bme688.StartUpdating(TimeSpan.FromSeconds(5));
        }

        Resolver.Log.Info("Starting storage container update.");
        tankLevelSensor.StartUpdating(TimeSpan.FromSeconds(5));

        if (displayController != null)
        {
            displayController.Update();
        }

        return Task.CompletedTask;
    }
}