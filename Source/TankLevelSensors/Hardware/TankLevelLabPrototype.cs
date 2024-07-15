using Meadow.Foundation.Sensors.Distance;
using Meadow.Peripherals.Displays;
using Meadow.Peripherals.Sensors.Distance;

namespace Meadow.Devices;

public class TankLevelLabPrototype : ITankLevelHardware
{
    public IPixelDisplay Display { get; private set; }

    public RotationType DisplayRotation { get; private set; }

    public IRangeFinder DistanceSensor { get; set; }

    public TankLevelLabPrototype(IProjectLabHardware projectLab)
    {
        DistanceSensor = new MaxBotix(projectLab.Qwiic.I2cBus, MaxBotix.SensorType.XL);
    }
}