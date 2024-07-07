using Meadow;
using Meadow.Devices;
using System.Threading.Tasks;

namespace TankLevelMonitor.F7;

public class MeadowApp : ProjectLabCoreComputeApp
{
    private MainController mainController;

    public override Task Initialize()
    {
        Resolver.Log.Info("Initialize...");

        ITankLevelHardware hardware = null;
        TankSpecs tankSpecs = null;

        HardwareTypes hardwareType = HardwareTypes.BenchPrototype;
        //HardwareTypes hardwareType = HardwareTypes.LabPrototype;

        switch (hardwareType)
        {
            case HardwareTypes.BenchPrototype:
                Resolver.Log.Info("instantiating bench prototype hardware");
                hardware = new TankLevelBenchPrototype(Hardware);
                tankSpecs = KnownStorageContainerConfigs.Container3500ml;
                break;

            case HardwareTypes.LabPrototype:
                Resolver.Log.Info("Instantiating lab prototype hardware");
                hardware = new TankLevelLabPrototype(Hardware);
                tankSpecs = KnownStorageContainerConfigs.Standard55GalDrum;
                break;

            default:
                Resolver.Log.Info("Undefined hardware configuration");
                break;
        }

        mainController = new MainController(hardware, tankSpecs);

        return Task.CompletedTask;
    }

    public override Task Run()
    {
        Resolver.Log.Info("Run...");

        mainController.Run();

        return Task.CompletedTask;
    }
}