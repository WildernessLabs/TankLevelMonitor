using Meadow;
using Meadow.Devices;
using System.Threading.Tasks;
using TankLevelMonitor.Controllers;
using TankLevelMonitor.Models;

namespace TankLevelMonitor
{
    // Change F7FeatherV2 to F7FeatherV1 for V1.x boards
    public class MeadowApp : App<F7CoreComputeV2>
    {
        MainAppController mainAppController;

        public override Task Initialize()
        {
            Resolver.Log.Info("Initialize...");

            mainAppController = new MainAppController(KnownStorageContainerConfigs.BenchContainer);

            return base.Initialize();
        }

        public override Task Run()
        {
            Resolver.Log.Info("Run...");

            mainAppController.Run();

            return base.Run();
        }
    }
}