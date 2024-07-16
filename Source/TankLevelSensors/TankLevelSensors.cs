using Meadow.Foundation;
using Meadow.Peripherals.Sensors.Distance;
using Meadow.Units;
using System;
using System.Threading.Tasks;

namespace Meadow.Devices;

public class TankLevelSensors : SamplingSensorBase<Volume>
{
    protected IRangeFinder Hardware;

    public event EventHandler<IChangeResult<Volume>> Updated = delegate { };

    public Length DistanceToTopOfLiquid { get; protected set; }

    public TankSpecs TankSpecs { get; protected set; }

    public double FillPercent => FillAmount.Liters / TankSpecs.Capacity.Liters;

    public Volume FillAmount => CalculateFillAmount(DistanceToTopOfLiquid);

    public TankLevelSensors(
        IRangeFinder hardware,
        TankSpecs tankSpecs)
    {
        Hardware = hardware;
        TankSpecs = tankSpecs;

        hardware.Updated += DistanceSensorUpdated;
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
        Hardware.StartUpdating(updateInterval);
    }

    public override void StopUpdating()
    {
        Hardware.StopUpdating();
    }

    protected override Task<Volume> ReadSensor()
    {
        return Task.FromResult(CalculateFillAmount(Hardware.Read().Result));
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