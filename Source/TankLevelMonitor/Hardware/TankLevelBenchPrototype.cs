using Meadow.Devices;
using Meadow.Foundation.Sensors.Distance;
using Meadow.Peripherals.Sensors.Distance;
using TankLevelMonitor.Core.Contracts;

namespace TankLevelMonitor.Core.Hardware;

public class TankLevelBenchPrototype : ITankLevelHardware
{
    public IProjectLabHardware ProjectLab { get; set; }

    public IRangeFinder DistanceSensor { get; set; }

    public TankLevelBenchPrototype()
    {
        DistanceSensor = new Vl53l0x(ProjectLab.Qwiic.I2cBus);
    }
}