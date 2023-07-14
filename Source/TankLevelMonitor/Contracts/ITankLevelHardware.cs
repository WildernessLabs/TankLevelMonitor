using Meadow.Devices;
using Meadow.Peripherals.Sensors;

namespace TankLevelMonitor.Contracts
{
    public interface ITankLevelHardware : IProjectLabHardware
    {
        IRangeFinder DistanceSensor { get; }
    }
}