using Meadow;
using Meadow.Foundation;
using Meadow.Units;
using System;
using System.Threading.Tasks;

namespace WildernessLabs.Hardware.TankLevelMonitor
{
    public class TankLevelMonitor : SamplingSensorBase<Volume>
    {
        protected ITankLevelHardware Hardware;

        public event EventHandler<IChangeResult<Volume>> Updated = delegate { };

        public Length DistanceToTopOfLiquid { get; protected set; }

        public TankSpecs TankSpecs { get; protected set; }

        public double FillPercent => (FillAmount.Liters / TankSpecs.Capacity.Liters);

        public Volume FillAmount => CalculateFillAmount(DistanceToTopOfLiquid);

        public TankLevelMonitor(
            ITankLevelHardware hardware,
            TankSpecs tankSpecs)
        {
            Hardware = hardware;
            TankSpecs = tankSpecs;

            hardware.DistanceSensor.DistanceUpdated += DistanceSensorUpdated;
        }

        private void DistanceSensorUpdated(object sender, IChangeResult<Length> changeResult)
        {
            var oldConditions = FillAmount;
            DistanceToTopOfLiquid = changeResult.New;
            var newConditions = FillAmount;
            Updated(this, new ChangeResult<Volume>(newConditions, oldConditions));
        }

        public override void StartUpdating(TimeSpan? updateInterval = null)
        {
            Hardware.DistanceSensor.StartUpdating(updateInterval);
        }

        public override void StopUpdating()
        {
            Hardware.DistanceSensor.StopUpdating();
        }

        protected override Task<Volume> ReadSensor()
        {
            throw new NotImplementedException();
        }

        protected Volume CalculateFillAmount(Length distanceToTop)
        {
            // if the distance sensor is return negative, it means
            // that it's not getting a reading because nothing is bouncing
            // back cause it's too far away.
            if (DistanceToTopOfLiquid.Centimeters < 0)
            {
                return new Volume(0);
            }

            if (DistanceToTopOfLiquid.Centimeters > TankSpecs.EmptyHeight.Centimeters)
            {
                return new Volume(0);
            }

            // (Height - EmptySpace) * VolumePerCm
            return new Volume((TankSpecs.EmptyHeight.Centimeters - DistanceToTopOfLiquid.Centimeters) * TankSpecs.VolumePerCentimeter.Liters);
        }
    }
}