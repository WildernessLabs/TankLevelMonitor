using Meadow.Devices;
using Meadow.Foundation.Sensors.Distance;
using Meadow.Peripherals.Displays;
using TankLevelMonitor.Azure.Contracts;

namespace TankLevelMonitor.Azure.Hardware;

public class LabHardware : ITankLevelHardware
{
    private readonly IProjectLabHardware projectLab;

    public IPixelDisplay Display => projectLab.Display;

    public RotationType DisplayRotation => RotationType._270Degrees;

    public TankLevelSensors TankLevelSensors { get; protected set; }

    public LabHardware(IProjectLabHardware projectLab, TankSpecs tankSpecs)
    {
        this.projectLab = projectLab;

        var distanceSensor = new MaxBotix(projectLab.Qwiic.I2cBus, MaxBotix.SensorType.XL);
        TankLevelSensors = new TankLevelSensors(distanceSensor, tankSpecs);
    }
}