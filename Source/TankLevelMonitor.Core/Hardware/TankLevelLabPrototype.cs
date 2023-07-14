using Meadow.Devices;
using Meadow.Foundation.Sensors.Distance;
using Meadow.Peripherals.Sensors;
using TankLevelMonitor.Contracts;

namespace TankLevelMonitor.Hardware
{
    public class TankLevelLabPrototype : ITankLevelHardware
    {
        public IProjectLabHardware ProjectLab { get; set; }

        public IRangeFinder DistanceSensor { get; set; }

        public TankLevelLabPrototype()
        {
            ProjectLab = Meadow.Devices.ProjectLab.Create();

            DistanceSensor = new MaxBotix(ProjectLab.I2cBus, MaxBotix.SensorType.XL);
        }
    }
}