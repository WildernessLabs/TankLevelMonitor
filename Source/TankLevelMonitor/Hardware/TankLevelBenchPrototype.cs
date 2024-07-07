using Meadow.Foundation.Sensors.Distance;
using Meadow.Peripherals.Displays;
using Meadow.Peripherals.Sensors.Distance;

namespace Meadow.Devices;

public class TankLevelBenchPrototype : ITankLevelHardware
{
    public IPixelDisplay Display { get; private set; }

    public RotationType DisplayRotation { get; private set; }

    public IRangeFinder DistanceSensor { get; set; }

    public TankLevelBenchPrototype(IProjectLabHardware projectLab)
    {
        DistanceSensor = new Vl53l0x(projectLab.Qwiic.I2cBus);
    }
}