using Meadow;
using Meadow.Foundation.Displays;
using Meadow.Peripherals.Sensors;
using Meadow.Units;
using TankLevelMonitor.UI;

namespace TankLevelMonitor_UI
{
    public class MeadowApp : App<Windows>
    {
        private IRangeFinder _distanceSensor;
        private WinFormsDisplay _display = default!;
        private DisplayController _displayController;

        public override Task Initialize()
        {
            Console.WriteLine("Initialize...");

            _display = new WinFormsDisplay(width: 320, height: 240);

            _displayController = new DisplayController(_display);

            _distanceSensor = new SimulatedDistanceSensor(new Length(100, Length.UnitType.Centimeters), new Length(0), new Length(100, Length.UnitType.Centimeters));
            _distanceSensor.StartUpdating(TimeSpan.FromSeconds(1));

            return Task.CompletedTask;
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

            Application.Run(_display);

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

        public static async Task Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ApplicationConfiguration.Initialize();

            await MeadowOS.Start(args);
        }
    }
}