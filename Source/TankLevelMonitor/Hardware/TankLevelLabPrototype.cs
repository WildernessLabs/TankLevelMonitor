using Meadow.Devices;
using Meadow.Foundation.Sensors.Distance;
using Meadow.Peripherals.Sensors.Distance;
using TankLevelMonitor.Core.Contracts;

namespace TankLevelMonitor.Core.Hardware;

public class TankLevelLabPrototype : ITankLevelHardware
{
    public IProjectLabHardware ProjectLab { get; set; }

    public IRangeFinder DistanceSensor { get; set; }

    public TankLevelLabPrototype()
    {
        DistanceSensor = new MaxBotix(ProjectLab.Qwiic.I2cBus, MaxBotix.SensorType.XL);
    }
}