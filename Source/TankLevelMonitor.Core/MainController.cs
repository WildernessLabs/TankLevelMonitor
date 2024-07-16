using Meadow;
using Meadow.Units;
using System;
using System.Threading.Tasks;
using TankLevelMonitor.Core.Contracts;
using TankLevelMonitor.Core.Controllers;

namespace TankLevelMonitor.Core;

public class MainController
{
    private DisplayController displayController;

    private ITankLevelHardware hardware;

    public MainController(ITankLevelHardware hardware)
    {
        Resolver.Log.Info("Initialize MainAppController...");

        this.hardware = hardware;

        hardware.TankLevelSensors.Updated += StorageContainerUpdated;

        if (hardware.Display is { } display)
        {
            displayController = new DisplayController(display, hardware.DisplayRotation);
        }

        //if ((Hardware as ProjectLabHardwareBase).AtmosphericSensor is { } bme688)
        //{
        //    bme688.Updated += Bme688Updated;
        //}
    }

    //private void Bme688Updated(object sender, IChangeResult<(Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure, Resistance? GasResistance)> e)
    //{
    //    Resolver.Log.Info($"BME688: " +
    //        $"Temperature: {(int)e.New.Temperature?.Celsius}°C - " +
    //        $"Humidity: {(int)e.New.Humidity?.Percent}% - " +
    //        $"Pressure: {(int)e.New.Pressure?.Millibar}mbar");

    //    if (displayController != null)
    //    {
    //        displayController.AtmosphericConditions = e.New;
    //    }
    //}

    private void StorageContainerUpdated(object sender, IChangeResult<Volume> result)
    {
        Resolver.Log.Info($"TankLevelSensor: " +
            $"Distance Sensor: {hardware.TankLevelSensors.DistanceToTopOfLiquid.Centimeters:n2}cm | " +
            $"Storage container: {result.New.Liters:n2}l | " +
            $"Fill Percent: {(int)(hardware.TankLevelSensors.FillPercent * 100)}%");

        displayController.VolumePercent = (int)(hardware.TankLevelSensors.FillPercent * 100);
    }

    public Task Run()
    {
        //if ((Hardware as ProjectLabHardwareBase).AtmosphericSensor is { } bme688)
        //{
        //    bme688.StartUpdating(TimeSpan.FromSeconds(5));
        //}

        Resolver.Log.Info("Starting storage container update.");
        hardware.TankLevelSensors.StartUpdating(TimeSpan.FromSeconds(5));

        if (displayController != null)
        {
            displayController.Update();
        }

        return Task.CompletedTask;
    }
}