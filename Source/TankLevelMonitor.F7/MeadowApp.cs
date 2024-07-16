using Meadow;
using Meadow.Devices;
using System.Threading.Tasks;
using TankLevelMonitor.Core;
using TankLevelMonitor.Core.Contracts;
using TankLevelMonitor.Core.Enums;
using TankLevelMonitor.F7.Hardware;

namespace TankLevelMonitor.F7;

public class MeadowApp : ProjectLabCoreComputeApp
{
    private MainController mainController;

    public override Task Initialize()
    {
        Resolver.Log.Info("Initialize...");

        ITankLevelHardware hardware = null;

        HardwareType hardwareType = HardwareType.BenchPrototype;
        //HardwareTypes hardwareType = HardwareTypes.LabPrototype;

        switch (hardwareType)
        {
            case HardwareType.BenchPrototype:
                Resolver.Log.Info("instantiating bench prototype hardware");
                hardware = new BenchHardware(Hardware, KnownStorageContainerConfigs.Container3500ml);
                break;

            case HardwareType.LabPrototype:
                Resolver.Log.Info("Instantiating lab prototype hardware");
                hardware = new LabHardware(Hardware, KnownStorageContainerConfigs.Standard55GalDrum);
                break;

            default:
                Resolver.Log.Info("Undefined hardware configuration");
                break;
        }

        mainController = new MainController(hardware);

        return Task.CompletedTask;
    }

    public override Task Run()
    {
        Resolver.Log.Info("Run...");

        mainController.Run();

        return Task.CompletedTask;
    }
}