using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using System;
using TankLevelMonitor_Demo.SQLite.Models;

namespace TankLevelMonitor_Demo
{
    public class DisplayController
    {
        readonly MicroGraphics graphics;

        Color backgroundColor = Color.White;
        Color foregroundColor = Color.Black;

        bool isUpdating = false;
        bool needsUpdate = false;

        public int VolumePercent
        {
            get => volumePercent;
            set
            {
                volumePercent = Math.Clamp(value, 0, 100);
                Update();
            }
        }
        int volumePercent = 0;

        public AtmosphericConditions? AtmosphericConditions
        {
            get => atmosphericConditions;
            set
            {
                atmosphericConditions = value;
                Update();
            }
        }
        AtmosphericConditions? atmosphericConditions;

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
            graphics.CurrentFont = new Font8x12();

            graphics.DrawText(
                x: 11,
                y: 11,
                text: "Temperature",
                color: foregroundColor,
                scaleFactor: ScaleFactor.X2);

            graphics.DrawText(
                x: 11,
                y: 90,
                text: "Humidity",
                color: foregroundColor,
                scaleFactor: ScaleFactor.X2);

            graphics.DrawText(
                x: 11,
                y: 169,
                text: "Pressure",
                color: foregroundColor,
                scaleFactor: ScaleFactor.X2);

            DrawAtmosphericConditions(AtmosphericConditions);

            DrawWaterVolumeGraph(VolumePercent);
        }

        void DrawAtmosphericConditions(AtmosphericConditions? conditions)
        {
            graphics.DrawText(
                x: 19,
                y: 47,
                text: $"{conditions?.Temperature.Value.Celsius:n0}C/{conditions?.Temperature.Value.Fahrenheit:n0}F",
                color: foregroundColor,
                scaleFactor: ScaleFactor.X2);

            graphics.DrawText(
                x: 19,
                y: 126,
                text: $"{conditions?.Humidity.Value.Percent:n0}%",
                color: foregroundColor,
                scaleFactor: ScaleFactor.X2);

            graphics.DrawText(
                x: 19,
                y: 205,
                text: $"{conditions?.Pressure.Value.Millibar:n0}mbar",
                color: foregroundColor,
                scaleFactor: ScaleFactor.X2);
        }

        void DrawWaterVolumeGraph(int volumePercent)
        {
            Color color = Color.FromHex("004B6B");

            int width = 100;
            int height = 218;
            int x = 209;
            int y = 11;
            int barHeight = (int)Math.Round(height / 10.0, 0);

            graphics.DrawRectangle(x, y, width, height, color, true);

            int percentageGraph = volumePercent == 100 ? 9 : volumePercent / 10;

            for (int i = percentageGraph; i >= 0; i--)
            {
                switch (i)
                {
                    case 0:
                    case 1:
                        color = Color.FromHex("FF3535");
                        break;
                    case 2:
                    case 3:
                    case 4:
                        color = Color.FromHex("FF8251");
                        break;
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                        color = Color.FromHex("35FF3D");
                        break;
                    case 9:
                        color = Color.FromHex("475AFF");
                        break;
                }

                graphics.DrawRectangle(x, 222 - (barHeight * i + 13), width, 20, color, true);
            }

            graphics.DrawRectangle(225, 103, 68, 28, Color.FromHex("004B6B"), true);
            graphics.DrawText(
                x: 259,
                y: graphics.Height / 2,
                text: $"{volumePercent}%",
                color: Color.White,
                scaleFactor: ScaleFactor.X2,
                alignmentH: HorizontalAlignment.Center,
                alignmentV: VerticalAlignment.Center);
        }
    }
}