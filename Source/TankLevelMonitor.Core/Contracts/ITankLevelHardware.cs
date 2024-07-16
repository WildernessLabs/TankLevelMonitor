using Meadow.Devices;
using Meadow.Peripherals.Displays;

namespace TankLevelMonitor.Core.Contracts;

public interface ITankLevelHardware
{
    IPixelDisplay Display { get; }

    RotationType DisplayRotation { get; }

    TankLevelSensors TankLevelSensors { get; }
}