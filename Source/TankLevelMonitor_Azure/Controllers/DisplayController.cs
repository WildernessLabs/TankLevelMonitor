using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Units;

namespace TankLevelMonitor_Azure
{
    public class DisplayController
    {
        readonly MicroGraphics graphics;

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

        public void Update()
        {
            if (isUpdating)
            {
                needsUpdate = true;
                return;
            }

            isUpdating = true;

            graphics.Clear();
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
            if (AtmosphericConditions?.Temperature is { } temp)
            {
                DrawTemp(temp);
            }

            DrawWaterVolumeGraph(VolumePercent);
        }

        void DrawTemp(Temperature temp)
        {
            // draw C/F
            graphics.DrawText(2, 2, $"{temp.Celsius:n0}C°/{temp.Fahrenheit:n0}F°",
                color: WildernessLabsColors.ChileanFire,
                alignmentH: HorizontalAlignment.Left);

            //// draw F
            //canvas.DrawText(2, 44, $"{temp.Fahrenheit:n0}F°",
            //    color: WildernessLabsColors.ChileanFire,
            //    alignmentH: HorizontalAlignment.Left);
        }

        void DrawWaterVolumeGraph(int volumePercent)
        {
            Color color = Color.FromHex("004B6B");

            int width = 100;
            int height = 218;
            int x = 140;
            int y = 12;
            int barHeight = (int)System.Math.Round(((float)height / (float)10), 0);

            graphics.DrawRectangle(x, y, width, height, color, true);

            int percentageGraph = (int)(volumePercent == 100 ? 9 : volumePercent / 10);
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

                graphics.DrawRectangle(x, 222 - (barHeight * i + 12), width, 20, color, true);
            }
        }

        void DrawStatus(string label, string value, Color color, int yPosition)
        {
            graphics.DrawText(x: 2, y: yPosition, label, color: color);
            graphics.DrawText(x: 238, y: yPosition, value, alignmentH: HorizontalAlignment.Right, color: color);
        }
    }
}