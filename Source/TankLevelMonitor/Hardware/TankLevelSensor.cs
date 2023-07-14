using Meadow;
using Meadow.Peripherals.Sensors;
using Meadow.Units;
using System;
using TankLevelMonitor.Models;

namespace TankLevelMonitor.Hardware
{
    public class TankLevelSensor
    {
        protected IRangeFinder distanceSensor;

        public TankContainerConfig ContainerInfo { get; protected set; }

        public Length DistanceToTopOfLiquid { get; protected set; }

        public event EventHandler<IChangeResult<Volume>> Updated = delegate { };

        public double FillPercent => (FillAmount.Liters / ContainerInfo.Capacity.Liters);

        public Volume FillAmount => CalculateFillAmount(DistanceToTopOfLiquid);

        public TankLevelSensor(
            IRangeFinder distanceSensor,
            TankContainerConfig containerInfo)
        {
            this.distanceSensor = distanceSensor;
            ContainerInfo = containerInfo;

            distanceSensor.DistanceUpdated += DistanceSensorUpdated;
        }

        private void DistanceSensorUpdated(object sender, IChangeResult<Length> changeResult)
        {
            var oldConditions = FillAmount;
            DistanceToTopOfLiquid = changeResult.New;
            var newConditions = FillAmount;
            Updated(this, new ChangeResult<Volume>(newConditions, oldConditions));
        }

        public void StartUpdating(TimeSpan? updateInterval = null)
        {
            distanceSensor.StartUpdating(updateInterval);
        }

        public void StopUpdating()
        {
            distanceSensor.StopUpdating();
        }

        protected Volume CalculateFillAmount(Length distanceToTop)
        {
            // if the distance sensor is return negative, it means
            // that it's not getting a reading because nothing is bouncing
            // back cause it's too far away.
            if (DistanceToTopOfLiquid.Centimeters < 0) { return new Volume(0); }
            if (DistanceToTopOfLiquid.Centimeters > ContainerInfo.EmptyHeight.Centimeters) { return new Volume(0); }

            // (Height - EmptySpace) * VolumePerCm
            return new Volume((ContainerInfo.EmptyHeight.Centimeters - DistanceToTopOfLiquid.Centimeters) * ContainerInfo.VolumePerCentimeter.Liters);
        }
    }
}
