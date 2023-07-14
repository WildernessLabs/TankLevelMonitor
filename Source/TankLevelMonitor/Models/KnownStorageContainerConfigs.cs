using Meadow.Units;

namespace WildernessLabs.Hardware.TankLevelMonitor
{
    public static class KnownStorageContainerConfigs
    {
        public static TankSpecs Standard55GalDrum
        {
            get
            {
                if (standard55GalDrum == null)
                {
                    standard55GalDrum = new TankSpecs
                    {
                        Capacity = new Volume(55, Volume.UnitType.Gallons),
                        EmptyHeight = new Length(85, Length.UnitType.Centimeters),

                        // 55 gal drum 1.72 gal per inch
                        // 3.78541 liters per gallon
                        // 3.78541 * 1.72 = 6.5109052 liters per inch
                        // 6.5109052 / 2.54 = 2.563348503937008 liters per cm
                        VolumePerCentimeter = new Volume(2.563, Volume.UnitType.Liters)
                    };
                }
                return standard55GalDrum;
            }
        }
        private static TankSpecs standard55GalDrum;

        public static TankSpecs BenchContainer
        {
            get
            {
                if (benchContainer == null)
                {
                    benchContainer = new TankSpecs
                    {
                        Capacity = new Volume(55, Volume.UnitType.Gallons),
                        EmptyHeight = new Length(20, Length.UnitType.Centimeters),

                        VolumePerCentimeter = new Volume(10.5, Volume.UnitType.Liters)
                    };
                }
                return benchContainer;
            }
        }
        private static TankSpecs benchContainer;
    }
}
