namespace BusinessObjects.Utilities
{
    using Varec.CommonComponents.EngineeringUnitsLibrary;

    public class SIConversion
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public SIConversion()
        {
            this.Init();
        }

        /// <summary>
        /// This constructor sets the SI Unit value.
        /// </summary>
        public SIConversion(EngineeringUnit siVolumeUnit, EngineeringUnit siTemperatureUnit, EngineeringUnit siMassUnit, EngineeringUnit siDensity)
        {
            this.SiVolumeUnit       = siVolumeUnit;
            this.SiTemperatureUnit  = siTemperatureUnit;
            this.SiMassUnit         = siMassUnit;
            this.SiDensityUnit      = siDensity;
        }
        #endregion

        #region Properties
        public EngineeringUnit SiVolumeUnit { get; set; }
        public EngineeringUnit SiTemperatureUnit { get; set; }
        public EngineeringUnit SiMassUnit { get; set; }
        public EngineeringUnit SiDensityUnit { get; set; }
        #endregion

        #region Public Methods
        public double ConvertVolumeFromSI(EngineeringUnit toUnit, double fromValue)
        {
            double resultValue = 0;

            resultValue = EngineeringUnits.Convert(fromValue, this.SiVolumeUnit, toUnit, resultValue);
            return resultValue;
        }

        public double ConvertVolumeFromSI(int toUnitInt, double fromValue)
        {
            double resultValue = 0;
            EngineeringUnit toUnit = (EngineeringUnit)toUnitInt;

            resultValue = EngineeringUnits.Convert(fromValue, this.SiVolumeUnit, toUnit, resultValue);
            return resultValue;
        }

        public double ConvertTemperatureFromSI(EngineeringUnit toUnit, double fromValue)
        {
            double resultValue = 0;

            resultValue = EngineeringUnits.Convert(fromValue, this.SiTemperatureUnit, toUnit, resultValue);
            return resultValue;
        }

        public double ConvertTemperatureFromSI(int toUnitInt, double fromValue)
        {
            double resultValue = 0;
            EngineeringUnit toUnit = (EngineeringUnit)toUnitInt;

            resultValue = EngineeringUnits.Convert(fromValue, this.SiTemperatureUnit, toUnit, resultValue);
            return resultValue;
        }

        public double ConvertMassFromSI(EngineeringUnit toUnit, double fromValue)
        {
            double resultValue = 0;

            resultValue = EngineeringUnits.Convert(fromValue, this.SiMassUnit, toUnit, resultValue);
            return resultValue;
        }

        public double ConvertMassFromSI(int toUnitInt, double fromValue)
        {
            double resultValue = 0;
            EngineeringUnit toUnit = (EngineeringUnit)toUnitInt;

            resultValue = EngineeringUnits.Convert(fromValue, this.SiMassUnit, toUnit, resultValue);
            return resultValue;
        }

        public double ConvertDensityFromSI(EngineeringUnit toUnit, double fromValue)
        {
            double resultValue = 0;

            resultValue = EngineeringUnits.Convert(fromValue, this.SiDensityUnit, toUnit, resultValue);
            return resultValue;
        }

        public double ConvertDensityFromSI(int toUnitInt, double fromValue)
        {
            double resultValue = 0;
            EngineeringUnit toUnit = (EngineeringUnit)toUnitInt;

            resultValue = EngineeringUnits.Convert(fromValue, this.SiDensityUnit, toUnit, resultValue);
            return resultValue;
        }

        public double ConvertVolumeToSI(EngineeringUnit fromUnit, double fromValue)
        {
            double resultValue = 0;

            resultValue = EngineeringUnits.Convert(fromValue, fromUnit, this.SiVolumeUnit, resultValue);
            return resultValue;
        }

        public double ConvertVolumeToSI(int fromUnitInt, double fromValue)
        {
            double resultValue = 0;
            EngineeringUnit fromUnit = (EngineeringUnit)fromUnitInt;

            resultValue = EngineeringUnits.Convert(fromValue, fromUnit, this.SiVolumeUnit, resultValue);
            return resultValue;
        }

        public double ConvertTemperatureToSI(EngineeringUnit fromUnit, double fromValue)
        {
            double resultValue = 0;

            resultValue = EngineeringUnits.Convert(fromValue, fromUnit, this.SiTemperatureUnit, resultValue);
            return resultValue;
        }

        public double ConvertTemperatureToSI(int fromUnitInt, double fromValue)
        {
            double resultValue = 0;
            EngineeringUnit fromUnit = (EngineeringUnit)fromUnitInt;

            resultValue = EngineeringUnits.Convert(fromValue, fromUnit, this.SiTemperatureUnit, resultValue);
            return resultValue;
        }

        public double ConvertMassToSI(EngineeringUnit fromUnit, double fromValue)
        {
            double resultValue = 0;

            resultValue = EngineeringUnits.Convert(fromValue, fromUnit, this.SiMassUnit, resultValue);
            return resultValue;
        }

        public double ConvertMassToSI(int fromUnitInt, double fromValue)
        {
            double resultValue = 0;
            EngineeringUnit fromUnit = (EngineeringUnit)fromUnitInt;

            resultValue = EngineeringUnits.Convert(fromValue, fromUnit, this.SiMassUnit, resultValue);
            return resultValue;
        }

        public double ConvertDensityToSI(EngineeringUnit fromUnit, double fromValue)
        {
            double resultValue = 0;

            resultValue = EngineeringUnits.Convert(fromValue, fromUnit, this.SiDensityUnit, resultValue);
            return resultValue;
        }

        public double ConvertDensityToSI(int fromUnitInt, double fromValue)
        {
            double resultValue = 0;
            EngineeringUnit fromUnit = (EngineeringUnit)fromUnitInt;

            resultValue = EngineeringUnits.Convert(fromValue, fromUnit, this.SiDensityUnit, resultValue);
            return resultValue;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// This method initializes the object to its initial state.
        /// </summary>
        private void Init()
        {
            this.SiVolumeUnit       = EngineeringUnit.FmvMeter3;
            this.SiTemperatureUnit  = EngineeringUnit.FmtDegC;
            this.SiMassUnit         = EngineeringUnit.FmmKg;
            this.SiDensityUnit      = EngineeringUnit.FmdKgM3;
        }
        #endregion
    }
}
