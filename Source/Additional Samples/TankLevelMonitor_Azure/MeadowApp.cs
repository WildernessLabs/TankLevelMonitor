using Meadow;
using Meadow.Devices;
using Meadow.Hardware;
using System.Threading.Tasks;
using TankLevelMonitor.Azure.Contracts;
using TankLevelMonitor.Azure.Enums;
using TankLevelMonitor.Azure.Hardware;

namespace TankLevelMonitor_Azure;

public class MeadowApp : ProjectLabCoreComputeApp
{
    private MainController mainController;

    public override Task Initialize()
    {
        Resolver.Log.Info("Initialize...");

        var wifi = Hardware.ComputeModule.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
        wifi.NetworkConnected += NetworkConnected;

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

    private async void NetworkConnected(INetworkAdapter sender, NetworkConnectionEventArgs args)
    {
        Resolver.Log.Info("NetworkConnected...");

        await mainController.iotHubManager.Initialize();
        await mainController.Run();
    }
}