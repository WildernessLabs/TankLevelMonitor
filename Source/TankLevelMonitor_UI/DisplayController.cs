using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Units;

namespace TankLevelMonitor_UI
{
    public class DisplayController
    {
        readonly MicroGraphics graphics;

        public int VolumePercent
        {
            set
            {
                volumePercent = value;
                Update();
            }
            get { return volumePercent; }
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

        public bool PumpOn
        {
            get => pumpOn;
            set
            {
                pumpOn = value;
                Update();
            }
        }
        protected bool pumpOn = false;

        public Volume VolumePumped
        {
            get => volumePumped;
            set
            {
                volumePumped = value;
                Update();
            }
        }
        protected Volume volumePumped;

        bool isUpdating = false;
        bool needsUpdate = false;

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
            graphics.DrawText(x: 2, y: 22, "Pump Status", WildernessLabsColors.AzureBlue);

            if (AtmosphericConditions?.Temperature is { } temp)
            {
                DrawTemp(temp);
            }

            DrawPumpStatus();
            DrawVolumePumped();
            DrawWaterVolumeGraph(VolumePercent);
        }

        void DrawPumpStatus()
        {
            graphics.DrawText(2, 42, $"Pump: {(PumpOn ? "on" : "off")}",
                color: WildernessLabsColors.ChileanFire,
                alignmentH: Meadow.Foundation.Graphics.HorizontalAlignment.Left);
        }

        void DrawTemp(Temperature temp)
        {
            // draw C/F
            graphics.DrawText(2, 2, $"{temp.Celsius:n0}C°/{temp.Fahrenheit:n0}F°",
                color: WildernessLabsColors.ChileanFire,
                alignmentH: Meadow.Foundation.Graphics.HorizontalAlignment.Left);

            //// draw F
            //canvas.DrawText(2, 44, $"{temp.Fahrenheit:n0}F°",
            //    color: WildernessLabsColors.ChileanFire,
            //    alignmentH: HorizontalAlignment.Left);
        }

        void DrawVolumePumped()
        {
            graphics.DrawText(2, 62, $"Pumped Ltrs",
                color: WildernessLabsColors.AzureBlue,
                alignmentH: Meadow.Foundation.Graphics.HorizontalAlignment.Left);

            graphics.DrawText(2, 82, $"{VolumePumped.Liters:N2}L",
                color: WildernessLabsColors.AzureBlue,
                alignmentH: Meadow.Foundation.Graphics.HorizontalAlignment.Left);
        }

        void DrawWaterVolumeGraph(int volumePercent)
        {
            Meadow.Foundation.Color color = Meadow.Foundation.Color.FromHex("004B6B");

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

                graphics.DrawRectangle(x, 222 - (barHeight * i + 12), width, 20, color, true);
            }
        }

        void DrawStatus(string label, string value, Meadow.Foundation.Color color, int yPosition)
        {
            graphics.DrawText(x: 2, y: yPosition, label, color: color);
            graphics.DrawText(x: 238, y: yPosition, value, alignmentH: Meadow.Foundation.Graphics.HorizontalAlignment.Right, color: color);
        }
    }
}