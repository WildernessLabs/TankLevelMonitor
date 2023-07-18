using Meadow.Foundation.Graphics;
using Meadow.Units;

namespace TankLevelMonitor_UI
{
    public class DisplayController
    {
        readonly MicroGraphics graphics;

        Meadow.Foundation.Color backgroundColor = Meadow.Foundation.Color.White;
        Meadow.Foundation.Color foregroundColor = Meadow.Foundation.Color.Black;

        bool isUpdating = false;
        bool needsUpdate = false;

        public int VolumePercent
        {
            get => volumePercent;
            set
            {
                volumePercent = value;
                Update();
            }
        }
        int volumePercent = 0;

        public (Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure, Resistance? GasResistance)? AtmosphericConditions
        {
            get => atmosphericConditions;
            set
            {
                atmosphericConditions = value;
                Update();
            }
        }
        (Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure, Resistance? GasResistance)? atmosphericConditions;

        public DisplayController(IGraphicsDisplay display)
        {
            graphics = new MicroGraphics(display)
            {
                CurrentFont = new Font12x20(),
                Stroke = 1,
            };

            graphics.Clear(true);
        }

        public void ShowSplashScreen()
        {
            graphics.Clear(Meadow.Foundation.Color.FromHex("23ABE3"));

            graphics.DrawText(
                x: graphics.Width / 2,
                y: graphics.Height / 2,
                text: "Tank Level Monitor",
                color: Meadow.Foundation.Color.Black,
                scaleFactor: ScaleFactor.X2,
                alignmentH: Meadow.Foundation.Graphics.HorizontalAlignment.Center,
                alignmentV: VerticalAlignment.Center);

            graphics.Show();
        }

        public void Update()
        {
            if (isUpdating)
            {
                needsUpdate = true;
                return;
            }

            isUpdating = true;

            graphics.Clear(backgroundColor);
            Draw();
            graphics.Show();

            isUpdating = false;

            if (needsUpdate)
            {
                needsUpdate = false;
                Update();
            }
        }

        void Draw()
        {
            graphics.CurrentFont = new Font12x20();

            graphics.DrawText(
                x: 11,
                y: 11,
                text: "Temperature",
                color: foregroundColor);

            graphics.DrawText(
                x: 11,
                y: 90,
                text: "Humidity",
                color: foregroundColor);

            graphics.DrawText(
                x: 11,
                y: 169,
                text: "Pressure",
                color: foregroundColor);

            DrawAtmosphericConditions(AtmosphericConditions);

            DrawWaterVolumeGraph(VolumePercent);
        }

        void DrawAtmosphericConditions((Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure, Resistance? GasResistance)? temp)
        {
            graphics.DrawText(
                x: 19,
                y: 47,
                text: $"{temp?.Temperature.Value.Celsius:n0}°C/{temp?.Temperature.Value.Fahrenheit:n0}°F",
                color: foregroundColor,
                scaleFactor: ScaleFactor.X1);

            graphics.DrawText(
                x: 19,
                y: 126,
                text: $"{temp?.Humidity.Value.Percent:n0}%",
                color: foregroundColor,
                scaleFactor: ScaleFactor.X1);

            graphics.DrawText(
                x: 19,
                y: 205,
                text: $"{temp?.Pressure.Value.Millibar:n0}mbar",
                color: foregroundColor,
                scaleFactor: ScaleFactor.X1);
        }

        void DrawWaterVolumeGraph(int volumePercent)
        {
            Meadow.Foundation.Color color = Meadow.Foundation.Color.FromHex("004B6B");

            int width = 100;
            int height = 218;
            int x = 209;
            int y = 11;
            int barHeight = (int)System.Math.Round(((float)height / (float)10), 0);

            graphics.DrawRectangle(x, y, width, height, color, true);

            int percentageGraph = (int)(volumePercent == 100 ? 9 : volumePercent / 10);
            for (int i = percentageGraph; i >= 0; i--)
            {
                switch (i)
                {
                    case 0:
                    case 1:
                        color = Meadow.Foundation.Color.FromHex("FF3535");
                        break;
                    case 2:
                    case 3:
                    case 4:
                        color = Meadow.Foundation.Color.FromHex("FF8251");
                        break;
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                        color = Meadow.Foundation.Color.FromHex("35FF3D");
                        break;
                    case 9:
                        color = Meadow.Foundation.Color.FromHex("475AFF");
                        break;
                }

                graphics.DrawRectangle(x, 222 - (barHeight * i + 13), width, 20, color, true);
            }

            graphics.DrawRectangle(225, 103, 68, 28, Meadow.Foundation.Color.FromHex("004B6B"), true);
            graphics.DrawText(
                x: 259,
                y: graphics.Height / 2,
                text: $"{volumePercent}%",
                color: Meadow.Foundation.Color.White,
                alignmentH: Meadow.Foundation.Graphics.HorizontalAlignment.Center,
                alignmentV: Meadow.Foundation.Graphics.VerticalAlignment.Center);
        }
    }
}