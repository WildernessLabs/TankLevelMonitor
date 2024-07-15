using Meadow;
using Meadow.Foundation.Displays;
using Meadow.Peripherals.Sensors.Distance;
using Meadow.Units;
using TankLevelMonitor.UI;

namespace TankLevelMonitor_UI
{
    public class MeadowApp : App<Desktop>
    {
        private IRangeFinder _distanceSensor;
        private DisplayController _displayController;

        public override Task Initialize()
        {
            Console.WriteLine("Initialize...");

            Device.Display!.Resize(320, 240, 3);
            _displayController = new DisplayController(Device.Display!);

            _distanceSensor = new SimulatedDistanceSensor(new Length(100, Length.UnitType.Centimeters), new Length(0), new Length(100, Length.UnitType.Centimeters));
            _distanceSensor.StartUpdating(TimeSpan.FromSeconds(1));

            return Task.CompletedTask;
        }

        private (Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure, Resistance? GasResistance)? RandomAtmosphericValue()
        {
            var random = new Random();

            var temperature = new Temperature?(new Temperature(random.Next(24, 29), Temperature.UnitType.Celsius));
            var humidity = new RelativeHumidity?(new RelativeHumidity(random.Next(75, 85), RelativeHumidity.UnitType.Percent));
            var pressure = new Pressure(new Pressure(random.Next(1100, 1200), Pressure.UnitType.Millibar));
            var resistance = new Resistance(new Resistance(55));

            var tuple = (
                temperature, humidity, pressure, resistance
            );

            return tuple;
        }

        public override Task Run()
        {
            Console.WriteLine("Run...");

            Task.Run(() =>
            {
                while (true)
                {
                    _displayController.AtmosphericConditions = RandomAtmosphericValue();

                    _displayController.VolumePercent = (int)_distanceSensor.Distance?.Centimeters;

                    Thread.Sleep(1000);
                }
            });

            // NOTE: this will not return until the display is closed
            ExecutePlatformDisplayRunner();

            return Task.CompletedTask;
        }

        private void ExecutePlatformDisplayRunner()
        {
            if (Device.Display is SilkDisplay sd)
            {
                sd.Run();
            }
            MeadowOS.TerminateRun();
            System.Environment.Exit(0);
        }

        public static async Task Main(string[] args)
        {
            await MeadowOS.Start(args);
        }
    }
}