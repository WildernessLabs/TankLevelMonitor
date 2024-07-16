using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Sensors;
using Meadow.Peripherals.Displays;
using Meadow.Units;
using TankLevelMonitor.Core.Contracts;

namespace TankLevelMonitor.DesktopApp.Hardware;

public class SimulatedHardware : ITankLevelHardware
{
    private readonly Desktop device;

    public IPixelDisplay? Display => device.Display;

    public RotationType DisplayRotation => RotationType.Default;

    public TankLevelSensors TankLevelSensors { get; protected set; }

    public SimulatedHardware(Desktop device, TankSpecs tankSpecs)
    {
        this.device = device;

        var distanceSensor = new SimulatedRangeFinder(
            initialLength: new Length(15, Length.UnitType.Centimeters),
            minimumLength: new Length(10, Length.UnitType.Centimeters),
            maximumLength: new Length(30, Length.UnitType.Centimeters),
            simulationBehavior: Meadow.Peripherals.Sensors.SimulationBehavior.RandomWalk);
        TankLevelSensors = new TankLevelSensors(distanceSensor, tankSpecs);
    }
}