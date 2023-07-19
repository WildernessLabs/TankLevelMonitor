using System;
using Meadow.Units;
using SQLite;
using MU = Meadow.Units;

namespace TankLevelMonitor_Demo.SQLite.Models
{
    [Table("TankLevelReadings")]
    public class TankLevelReading
    {
        [PrimaryKey, AutoIncrement]
        public int? ID { get; set; }

        public double? TankLevelValue
        {
            get => Temperature?.Celsius;
            set => Temperature = new Temperature(value.Value, MU.Temperature.UnitType.Celsius);
        }

        public double? TemperatureValue
        {
            get => Temperature?.Celsius;
            set => Temperature = new Temperature(value.Value, MU.Temperature.UnitType.Celsius);
        }

        public double? PressureValue
        {
            get => Pressure?.Bar;
            set => Pressure = new Pressure(value.Value, MU.Pressure.UnitType.Bar);
        }

        public double? HumidityValue
        {
            get => Humidity?.Percent;
            set => Humidity = new RelativeHumidity(value.Value, RelativeHumidity.UnitType.Percent);
        }

        [Indexed]
        public DateTime DateTime { get; set; }

        [Ignore]
        public Length? TankLevel { get; set; }

        [Ignore]
        public Temperature? Temperature { get; set; }
        [Ignore]
        public Pressure? Pressure { get; set; }
        [Ignore]
        public RelativeHumidity? Humidity { get; set; }
    }
}