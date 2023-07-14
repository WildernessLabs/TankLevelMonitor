using Meadow.Devices;
using Meadow.Foundation.Sensors.Distance;
using Meadow.Peripherals.Sensors;

namespace WildernessLabs.Hardware.TankLevelMonitor
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