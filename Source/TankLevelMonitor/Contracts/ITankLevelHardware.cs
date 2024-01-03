using Meadow.Devices;
using Meadow.Peripherals.Sensors.Distance;

namespace WildernessLabs.Hardware.TankLevelMonitor
{
    public interface ITankLevelHardware
    {
        IProjectLabHardware ProjectLab { get; set; }

        IRangeFinder DistanceSensor { get; }
    }
}