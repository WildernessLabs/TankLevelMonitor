using Meadow;
using Meadow.Devices;
using System.Threading.Tasks;
using WildernessLabs.Hardware.TankLevelMonitor;

namespace TankLevelMonitor_Demo
{
    public class MeadowApp : App<F7CoreComputeV2>
    {
        MainAppController mainAppController;

        public override Task Initialize()
        {
            Resolver.Log.Info("Initialize...");

            TankSpecs tankSpecs;
            ITankLevelHardware hardware;

            HardwareTypes hardwareType = HardwareTypes.BenchPrototype;
            //HardwareTypes hardwareType = HardwareTypes.LabPrototype;

            switch (hardwareType)
            {
                case HardwareTypes.BenchPrototype:
                    Resolver.Log.Info("instantiating bench prototype hardware.");
                    hardware = new TankLevelBenchPrototype();
                    tankSpecs = KnownStorageContainerConfigs.Container3500ml;
                    break;
                default:
                case HardwareTypes.LabPrototype:
                    Resolver.Log.Info("Instantiating lab prototype hardware.");
                    hardware = new TankLevelLabPrototype();
                    tankSpecs = KnownStorageContainerConfigs.Standard55GalDrum;
                    break;
            }

            mainAppController = new MainAppController(hardware, tankSpecs);

            return base.Initialize();
        }

        public override Task Run()
        {
            Resolver.Log.Info("Run...");

            mainAppController.Run();

            return base.Run();
        }


        //Vl53l0x sensor;

        //public override Task Initialize()
        //{
        //    Resolver.Log.Info("Initializing hardware...");

        //    var i2cBus = ProjectLab.Create();
        //    sensor = new Vl53l0x(i2cBus.I2cBus, (byte)Vl53l0x.Addresses.Default);

        //    sensor.DistanceUpdated += Sensor_Updated;

        //    return Task.CompletedTask;
        //}

        //public override Task Run()
        //{
        //    sensor.StartUpdating(TimeSpan.FromMilliseconds(250));

        //    return Task.CompletedTask;
        //}

        //private void Sensor_Updated(object sender, IChangeResult<Length> result)
        //{
        //    if (result.New == null) { return; }

        //    if (result.New < new Length(0, Length.UnitType.Millimeters))
        //    {
        //        Resolver.Log.Info("out of range.");
        //    }
        //    else
        //    {
        //        Resolver.Log.Info($"{result.New.Millimeters}");
        //    }
        //}
    }
}