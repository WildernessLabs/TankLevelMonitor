using Meadow.Devices;
using Meadow.Peripherals.Sensors.Distance;

namespace TankLevelMonitor.Core.Contracts;

public interface ITankLevelHardware
{
    IProjectLabHardware ProjectLab { get; set; }

    IRangeFinder DistanceSensor { get; }
}