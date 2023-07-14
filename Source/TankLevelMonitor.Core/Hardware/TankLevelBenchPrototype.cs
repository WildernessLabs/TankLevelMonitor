﻿using Meadow.Devices;
using Meadow.Foundation.Sensors.Distance;
using Meadow.Peripherals.Sensors;
using TankLevelMonitor.Contracts;

namespace TankLevelMonitor.Hardware
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