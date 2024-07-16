using Meadow.Devices;
using Meadow.Foundation.Sensors.Distance;
using Meadow.Peripherals.Displays;
using TankLevelMonitor.Core.Contracts;

namespace TankLevelMonitor.F7.Hardware;

public class BenchHardware : ITankLevelHardware
{
    private readonly IProjectLabHardware projectLab;

    public IPixelDisplay Display => projectLab.Display;

    public RotationType DisplayRotation => RotationType._270Degrees;

    public TankLevelSensors TankLevelSensors { get; protected set; }

    public BenchHardware(IProjectLabHardware projectLab, TankSpecs tankSpecs)
    {
        this.projectLab = projectLab;

        var distanceSensor = new Vl53l0x(projectLab.Qwiic.I2cBus);
        TankLevelSensors = new TankLevelSensors(distanceSensor, tankSpecs);
    }
}