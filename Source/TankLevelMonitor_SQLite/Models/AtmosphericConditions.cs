using Meadow.Units;
namespace TankLevelMonitor_Demo.SQLite.Models
{
    public record struct AtmosphericConditions(Temperature? Temperature,
        RelativeHumidity? Humidity,
        Pressure? Pressure,
        Resistance? GasResistance);
}