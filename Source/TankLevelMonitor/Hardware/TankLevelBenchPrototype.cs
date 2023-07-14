using Meadow.Devices;
using Meadow.Foundation.Sensors.Distance;
using Meadow.Peripherals.Sensors;
using WildernessLabs.Hardware.TankLevelMonitor.Contracts;

namespace WildernessLabs.Hardware.TankLevelMonitor.Hardware
{
    public class TankLevelBenchPrototype : ITankLevelHardware
    {
        public IProjectLabHardware ProjectLab { get; set; }

        public IRangeFinder DistanceSensor { get; set; }

        public TankLevelBenchPrototype()
        {
            ProjectLab = Meadow.Devices.ProjectLab.Create();

            DistanceSensor = new Vl53l0x(ProjectLab.I2cBus);
        }
    }
}