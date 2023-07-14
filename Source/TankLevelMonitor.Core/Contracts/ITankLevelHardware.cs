using Meadow.Devices;
using Meadow.Peripherals.Sensors;

namespace TankLevelMonitor.Contracts
{
    public interface ITankLevelHardware
    {
        IProjectLabHardware ProjectLab { get; set; }

        IRangeFinder DistanceSensor { get; }
    }
}