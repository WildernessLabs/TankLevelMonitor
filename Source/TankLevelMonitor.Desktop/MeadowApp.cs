using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Displays;
using System.Threading.Tasks;
using TankLevelMonitor.Core;
using TankLevelMonitor.DesktopApp.Hardware;

namespace TankLevelMonitor.DesktopApp;

public class MeadowApp : App<Desktop>
{
    private MainController mainController;

    public override Task Initialize()
    {
        Resolver.Log.Info("Initialize...");

        Device.Display?.Resize(320, 240, 2);

        var hardware = new SimulatedHardware(Device, KnownStorageContainerConfigs.Container3500ml);

        mainController = new MainController(hardware);

        return base.Initialize();
    }

    public override Task Run()
    {
        Resolver.Log.Info("Run...");

        mainController.Run();

        // NOTE: this will not return until the display is closed
        ExecutePlatformDisplayRunner();

        return Task.CompletedTask;
    }

    private void ExecutePlatformDisplayRunner()
    {
        if (Device.Display is SilkDisplay sd)
        {
            sd.Run();
        }
        MeadowOS.TerminateRun();
        System.Environment.Exit(0);
    }
}