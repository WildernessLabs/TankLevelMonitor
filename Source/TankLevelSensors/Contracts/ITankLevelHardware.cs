using Meadow.Peripherals.Displays;
using Meadow.Peripherals.Sensors.Distance;

namespace Meadow.Devices;

public interface ITankLevelHardware
{
    IPixelDisplay Display { get; }

    RotationType DisplayRotation { get; }

    IRangeFinder DistanceSensor { get; }
}