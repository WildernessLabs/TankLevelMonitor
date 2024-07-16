using Meadow;
using Meadow.Units;
using System;
using System.Threading.Tasks;
using TankLevelMonitor.Azure.Contracts;
using TankLevelMonitor_Azure.Azure;

namespace TankLevelMonitor_Azure;

public class MainController
{
    private ITankLevelHardware hardware;
    private DisplayController displayController;

    public IotHubManager iotHubManager { get; protected set; }

    public MainController(ITankLevelHardware hardware)
    {
        Resolver.Log.Info("Initialize MainAppController...");

        this.hardware = hardware;

        iotHubManager = new IotHubManager();

        if (hardware.TankLevelSensors is { } tankLevelSensors)
        {
            tankLevelSensors.Updated += StorageContainerUpdated;
        }

        if (hardware.Display is { } display)
        {
            displayController = new DisplayController(display);
        }
    }

    private async void StorageContainerUpdated(object sender, IChangeResult<Volume> result)
    {
        Resolver.Log.Info($"Tank Level Reading: " +
            $"Distance Sensor - {hardware.TankLevelSensors.DistanceToTopOfLiquid.Centimeters:n2}cm |" +
            $"Storage Container: {result.New.Milliliters:n2}ml | " +
            $"Fill Percent: {(int)(hardware.TankLevelSensors.FillPercent * 100)}%");

        displayController.VolumePercent = (int)(hardware.TankLevelSensors.FillPercent * 100);

        await iotHubManager.SendEnvironmentalReading(hardware.TankLevelSensors.FillAmount);
    }

    public Task Run()
    {
        hardware.TankLevelSensors.StartUpdating(TimeSpan.FromSeconds(1));

        displayController.Update();

        return Task.CompletedTask;
    }
}