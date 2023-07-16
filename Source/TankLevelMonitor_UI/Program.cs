using Meadow;
using Meadow.Foundation.Displays;
using Meadow.Units;
using System.Net.Http.Headers;

namespace TankLevelMonitor_UI
{
    public class MeadowApp : App<Windows>
    {
        private WinFormsDisplay _display = default!;
        private DisplayController _displayController;
        //private DisplayControllerWithStatus _displayController;

        public override Task Initialize()
        {
            Console.WriteLine("Initialize...");

            _display = new WinFormsDisplay(width: 320, height: 240);

            _displayController = new DisplayController(_display);
            //_displayController = new DisplayControllerWithStatus(_display);

            return Task.CompletedTask;
        }

        public override Task Run()
        {
            Console.WriteLine("Run...");

            //_displayController.ShowSplashScreen();

            Task.Run(() => 
            {
                while (true)
                {
                    _displayController.AtmosphericConditions = RandomAtmosphericValue();

                    for (int i = 0; i <= 100; i++)
                    {
                        _displayController.VolumePercent = i;
                        Thread.Sleep(100);
                    }

                    _displayController.AtmosphericConditions = RandomAtmosphericValue();

                    for (int i = 100; i >= 0; i--)
                    {
                        _displayController.VolumePercent = i;
                        Thread.Sleep(100);
                    }
                }
            });

            Application.Run(_display);

            return Task.CompletedTask;
        }

        private (Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure, Resistance? GasResistance)? RandomAtmosphericValue() 
        {
            var random = new Random();

            Temperature? temperature = new Temperature?(new Temperature(random.Next(24, 29)));
            RelativeHumidity? Humidity = new RelativeHumidity?(new RelativeHumidity(random.Next(75, 85)));
            Pressure? pressure = new Pressure(new Pressure(random.Next(1100, 1200), Pressure.UnitType.Millibar));
            Resistance? resistance = new Resistance(new Resistance(55));

            var tuple = (
                temperature, Humidity, pressure, resistance
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