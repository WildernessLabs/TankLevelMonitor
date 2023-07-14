using Meadow.Units;

namespace WildernessLabs.Hardware.TankLevelMonitor.Models
{
    public class TankSpecs
    {
        /// <summary>
        /// Total capacity of the container.
        /// </summary>
        public Volume Capacity { get; set; }

        /// <summary>
        /// The volume of liquid per centimeter of height.
        /// </summary>
        public Volume VolumePerCentimeter { get; set; }

        /// <summary>
        /// The internal height of an empty storage unit.
        /// </summary>
        public Length EmptyHeight { get; set; }
    }
}