using Meadow;
using Meadow.Devices;
using Meadow.Hardware;
using System.Threading.Tasks;
using WildernessLabs.Hardware.TankLevelMonitor;

namespace TankLevelMonitor_Azure
{
    public class MeadowApp : App<F7CoreComputeV2>
    {
        MainAppController mainAppController;

        public override Task Initialize()
        {
            Resolver.Log.Info("Initialize...");

            var wifi = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
            wifi.NetworkConnected += NetworkConnected;

            TankSpecs tankSpecs;
            ITankLevelHardware hardware;

            HardwareTypes hardwareType = HardwareTypes.BenchPrototype;
            //HardwareTypes hardwareType = HardwareTypes.LabPrototype;

            switch (hardwareType)
            {
                case HardwareTypes.BenchPrototype:
                    Resolver.Log.Info("instantiating bench prototype hardware.");
                    hardware = new TankLevelBenchPrototype();
                    tankSpecs = KnownStorageContainerConfigs.Container3500ml;
                    break;
                default:
                case HardwareTypes.LabPrototype:
                    Resolver.Log.Info("Instantiating lab prototype hardware.");
                    hardware = new TankLevelLabPrototype();
                    tankSpecs = KnownStorageContainerConfigs.Standard55GalDrum;
                    break;
            }

            mainAppController = new MainAppController(hardware, tankSpecs);

            return base.Initialize();
        }

        private async void NetworkConnected(INetworkAdapter sender, NetworkConnectionEventArgs args)
        {
            Resolver.Log.Info("NetworkConnected...");

            await mainAppController.iotHubManager.Initialize();
            await mainAppController.Run();
        }
    }
}