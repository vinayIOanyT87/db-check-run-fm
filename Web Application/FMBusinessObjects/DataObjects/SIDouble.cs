namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Globalization;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Xml.Serialization;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    [Serializable] 
	[DataContract]
    public class SIDouble
    {
        [DataMember]
        public double SIValue = 0.0;

        [DataMember]
        public EngineeringUnit Units;

        [DataMember]
		public double ReferenceTemperature = 15.0;

        [DataMember]
		public int numberDecimalDigits = 0;

        [DataMember]
		public int[] numberGroupSizes = { 3 };

        [DataMember]
		public string numberDecimalSeparator = ".";

        [DataMember]
		public string numberGroupSeparator = ",";

        public SIDouble()
        {
            this.Units=0;
            this.Format=NumberFormatInfo.CurrentInfo;
        }

        public SIDouble( EngineeringUnit units, NumberFormatInfo format, double value )
        {
            this.Units=units;
				if (format != null)
				{
					this.Format = format;
				}
            this.SIValue=value;
        }

		public SIDouble(EngineeringUnit units, int decimalDigits, double value)
		{
			this.Units = units;
			this.numberDecimalDigits = decimalDigits;
			this.SIValue = value;
		}


		[XmlIgnore]
        public virtual double Value
        {
            [SecuritySafeCritical]
            get
            {

                double Result=0.0;

                // The following deals with an error in UnitConvert which
                // is not catching divide by 0 
                if (Units == EngineeringUnit.FmdDegApi
				   && SIValue == 0.0)
                    return 1e20;

                if (Units != 0)
                {
                    try
                    {
							  EngineeringUnits.Convert(SIValue,
                                                SIUnits( Units ),
                                                ref Result,
                                                Units,
                                                ReferenceTemperature );
                    }
                    catch
                    {
                        string abbrev = EngineeringUnits.GetUnitAbbreviation(Units);
                        throw new Exception( "Invalid value conversion from SI units: SIValue = " + SIValue.ToString() + " Units=" + abbrev );
                    }
                }
                else
                    Result=SIValue;


                if (Units != EngineeringUnit.FmlFtIn16Th && Units != EngineeringUnit.FmlFtIn8Th && Format != null)
                {
                    try
                    {
                        Result = Math.Round( Result, Format.NumberDecimalDigits, MidpointRounding.AwayFromZero );
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        throw new ArgumentOutOfRangeException( ex.Message );
                    }
                    catch (Exception ex)
                    {
                        throw new Exception( ex.Message );
                    }
                }

                return Result;
            }
            [SecuritySafeCritical]
            set
            {
                if (Units == EngineeringUnit.FmdDegApi && value >= 1e20)
                {
                    SIValue = 0;
                    return;
                }
                if (Units != 0)
                {
						 try
                    {
							  EngineeringUnits.Convert(value,
                                                Units,
                                                ref SIValue,
                                                SIUnits( Units ),
                                                ReferenceTemperature );
                    }
                    catch
                    {
                        string abbrev = EngineeringUnits.GetUnitAbbreviation(Units);
                        throw new Exception( "Invalid value conversion to SI Units : Value = " + value.ToString() + " Units=" + abbrev );
                    }

                }
                else
                    SIValue=value;
            }
        }

        public override int GetHashCode()
        {
            return SIValue.GetHashCode();
        }

        public override bool Equals( object obj )
        {
            if (!typeof( SIDouble ).IsInstanceOfType( obj ))
                return false;

            return Value.Equals( ((SIDouble)obj).Value );
        }


        public override string ToString()
        {
            if (Format == null)
                return Value.ToString();

            return Value.ToString( "N", Format );
        }

        [XmlIgnore]
        public NumberFormatInfo Format
        {
            get
            {
                NumberFormatInfo format=new NumberFormatInfo();

                format.NumberDecimalDigits=numberDecimalDigits;
                format.NumberGroupSizes=numberGroupSizes;
                format.NumberDecimalSeparator=numberDecimalSeparator;
                format.NumberGroupSeparator=numberGroupSeparator;

                return format;
            }
            set
            {
                numberDecimalDigits=value.NumberDecimalDigits;
                numberGroupSizes=value.NumberGroupSizes;
                numberDecimalSeparator=value.NumberDecimalSeparator;
                numberGroupSeparator = value.NumberGroupSeparator;
            }
        }

        public static EngineeringUnit SIUnits( EngineeringUnit Units )
        {
            switch (Units)
            {
                // Temperature Units
                case EngineeringUnit.FmtDegC:				// Degrees Celsius
                case EngineeringUnit.FmtDegF:				// Degrees Fahrenheit
                case EngineeringUnit.FmtDegK: 			// Degrees Kelvin
                case EngineeringUnit.FmtDegR: 			// Degrees Rankine
                    return EngineeringUnit.FmtDegC;

                // Time Units				
                case EngineeringUnit.FmtMsec: 			// MilliSeconds
                case EngineeringUnit.FmtSec:				// Seconds
                case EngineeringUnit.FmtMin:				// Minutes
                case EngineeringUnit.FmtHour: 			// Hours
                case EngineeringUnit.FmtDay:				// Days
                case EngineeringUnit.FmtWeek: 			// Weeks
                case EngineeringUnit.FmtMonth: 			// Months
                case EngineeringUnit.FmtYear:				// Years
                    return EngineeringUnit.FmtSec;

                // Length Units
                case EngineeringUnit.FmlFtIn8Th:			// Feet/Inches/18ths
                case EngineeringUnit.FmlMm:	 			// Millimeters
                case EngineeringUnit.FmlCm:				// Centimeters
                case EngineeringUnit.FmlMeter:			// Meters
                case EngineeringUnit.FmlKm:		 		// Kilometers
                case EngineeringUnit.Fml16Th:				// 1/16 inch
                case EngineeringUnit.FmlInch: 			// Inches
                case EngineeringUnit.FmlFeet:		 		// Feet
                case EngineeringUnit.FmlFtIn16Th:		// Feet/Inches/16ths
                case EngineeringUnit.FmlYard:				// Yards
                case EngineeringUnit.FmlMile:				// Miles
                    return EngineeringUnit.FmlMeter;

                // Area units
                case EngineeringUnit.FmaMm2:			 	// Millimeters Sq
                case EngineeringUnit.FmaCm2:		 		// Centimeters Sq
                case EngineeringUnit.FmaMeter2:			// Meters Sq
                case EngineeringUnit.FmaKm2:				// Kilometers Sq
                case EngineeringUnit.Fma16Th2:			// 1/16 inch Sq
                case EngineeringUnit.FmaInch2:			// Inches Sq
                case EngineeringUnit.FmaFeet2:			// Feet Sq
                case EngineeringUnit.FmaYard2:	 		// Yards Sq
                case EngineeringUnit.FmaMile2:			// Miles Sq
                    return EngineeringUnit.FmaMm2;

                // Volume units
                case EngineeringUnit.FmvCm3:				// Cubic centimeters
                case EngineeringUnit.FmvMeter3:			// Cubic meters
                case EngineeringUnit.FmvLitre:			// Litres
                case EngineeringUnit.FmvInch3:			// Cubic inches
                case EngineeringUnit.FmvFeet3:			// Cubic feet
                case EngineeringUnit.FmvYard3:			// Cubic yards
                case EngineeringUnit.FmvUsGal:			// US Gallons
                case EngineeringUnit.FmvImpGal:			// Imp Gallons
                case EngineeringUnit.FmvBlOil:			// Barrels Oil
                case EngineeringUnit.FmvBlLiq:			// Barrels Liquid
                case EngineeringUnit.FmvKl:				// Kilolitres
                case EngineeringUnit.FmvMsFt3:             // 1000 standard cubic feet 
                    return EngineeringUnit.FmvMeter3;

                // Mass Units
                case EngineeringUnit.FmmGram:	 			// Grams
                case EngineeringUnit.FmmKg:		 		// Kilograms
                case EngineeringUnit.FmmMTon:				// Metric Tons
                case EngineeringUnit.FmmOz:				// Ounces
                case EngineeringUnit.FmmLb:		 		// Pounds
                case EngineeringUnit.FmmETon:		 	 	// English Tons
                case EngineeringUnit.FmmSTon:				// Short Tons
                case EngineeringUnit.FmmLTon:	 	 		// Long Tons
                case EngineeringUnit.FmmMlbs:				// Thousands of Pounds
                    return EngineeringUnit.FmmKg;

                // Pressure Units
                case EngineeringUnit.FmpPa:	 			// Pascal (SI)
                case EngineeringUnit.FmpKPa:			 	// Kilopascal
                case EngineeringUnit.FmpKgCm2:			// Kg per sq cm
                case EngineeringUnit.FmpPsi:		 		// lb per sq inch
                case EngineeringUnit.FmpPsiG:		 		// PSI Gauge
                case EngineeringUnit.FmpPsiA:  			// PSI Absolute
                case EngineeringUnit.FmpInH2O:			// In. H2O (@ 68F)
                case EngineeringUnit.FmpFtH2O:	 		// Ft. H2O (@ 68F)
                case EngineeringUnit.FmpInHg:		 		// In. Mercury (@ 0C)
                case EngineeringUnit.FmpLbFt2:	 		// Pounds per square foot
                case EngineeringUnit.FmpTorr:				// Torr (@ 0C)
                case EngineeringUnit.FmpBar:	 			// Bar
                case EngineeringUnit.FmpMBar:		 		// Millibar
                case EngineeringUnit.FmpMmHg:		 		// mm Hg (@ 0C)
                case EngineeringUnit.FmpMmH2O:	 		// mm H2O (@ 68F)
                case EngineeringUnit.FmpGmCm2:			// Grams per square cm
                case EngineeringUnit.FmpAtm:	 			// Atmospheres
                    return EngineeringUnit.FmpPa;

                // Volumetric Flow units
                case EngineeringUnit.FmvfCcMin: 			// CC/Min
                case EngineeringUnit.FmvfCcHr:			// CC/Hour
                case EngineeringUnit.FmvfM3Sec:			// m3/sec
                case EngineeringUnit.FmvfM3Min:			// m3/Minute
                case EngineeringUnit.FmvfM3Hr:			// m3/Hour
                case EngineeringUnit.FmvfM3Day:			// m3/Day
                case EngineeringUnit.FmvfLtSec:			// Litre/sec
                case EngineeringUnit.FmvfLtMin:			// Litres/minute
                case EngineeringUnit.FmvfLtHr:			// Litres/Hour
                case EngineeringUnit.FmvfMlpd:			// Million litres/day
                case EngineeringUnit.FmvfIn3Min:			// Cubic inches/minute
                case EngineeringUnit.FmvfIn3Hr:			// Cubic inches/hour
                case EngineeringUnit.FmvfFt3Sec:			// Cubic feet/second
                case EngineeringUnit.FmvfFt3Min:			// Cubic feet/minute
                case EngineeringUnit.FmvfFt3Hr:			// Cubic feet/hour
                case EngineeringUnit.FmvfFt3Day:			// Cubic feet/day
                case EngineeringUnit.FmvfYd3Min:			// Cubic yards/minute
                case EngineeringUnit.FmvfYd3Hr:			// Cubic yards/hour
                case EngineeringUnit.FmvfGps:				// Gallons/sec (US)
                case EngineeringUnit.FmvfGpm:	 			// Gallons/minute (US)
                case EngineeringUnit.FmvfGph: 			// Gallons/hour	(US)
                case EngineeringUnit.FmvfMGpd:			// Millions of gallons/day (US)
                case EngineeringUnit.FmvfImpGps:			// Imp gallons/sec
                case EngineeringUnit.FmvfImpGpm:			// Imp gallons/minute
                case EngineeringUnit.FmvfImpGph:			// Imp gallons/hour
                case EngineeringUnit.FmvfImpMGpd:			// Imp millions of gallons/day
                case EngineeringUnit.FmvfBpMoil:			// BBL/min (oil)
                case EngineeringUnit.FmvfBpHoil:			// BBL/hour (oil)
                case EngineeringUnit.FmvfBpDoil:			// BBL/day (oil)
                case EngineeringUnit.FmvfMbDoil:			// Millions barrels/day (oil)
                case EngineeringUnit.FmvfBpMliq:			// BBL/min (liq)
                case EngineeringUnit.FmvfBpHliq:			// BBL/hour (liq)
                case EngineeringUnit.FmvfBpDliq:			// BBL/day (liq)
                case EngineeringUnit.FmvfMbDliq:			// Millions barrels/day (liquid)
                case EngineeringUnit.FmvfKlSec:			// kilolitres/sec
                case EngineeringUnit.FmvfKlMin:			// kilolitres/Minute
                case EngineeringUnit.FmvfKlHr:		 	// kilolitres/Hour
                case EngineeringUnit.FmvfKlDay:			// kilolitres/Day
                    return EngineeringUnit.FmvfM3Sec;

                // Mass Flow Units
                case EngineeringUnit.FmmfLbSec:			// Pounds/sec
                case EngineeringUnit.FmmfLbMin:			// Pounds/minute
                case EngineeringUnit.FmmfLbHr:			// Pounds/hour
                case EngineeringUnit.FmmfLbDay:			// Pounds/day
                case EngineeringUnit.FmmfMTonMn:			// Metric tons/minute
                case EngineeringUnit.FmmfMTonHr:			// Metric tons/hour
                case EngineeringUnit.FmmfMTonDy:			// Metric tons/day
                case EngineeringUnit.FmmfSTonMn:			// Short tons/min
                case EngineeringUnit.FmmfSTonHr:			// Short tons/hour
                case EngineeringUnit.FmmfSTonDy:			// Short tons/day
                case EngineeringUnit.FmmfLTonMn:			// Long tons/min
                case EngineeringUnit.FmmfLTonHr:			// Long tons/hour
                case EngineeringUnit.FmmfLTonDy:			// Long tons/day
                case EngineeringUnit.FmmfGmSec:			// Grams/sec
                case EngineeringUnit.FmmfGmMin:			// Grams/minute
                case EngineeringUnit.FmmfGmHr:			// Grams/hour
                case EngineeringUnit.FmmfKgSec:			// Kilograms/sec
                case EngineeringUnit.FmmfKgMin:			// Kilograms/minute
                case EngineeringUnit.FmmfKgHr:			// Kilograms/hour
                case EngineeringUnit.FmmfKgDay:			// Kilograms/day
                case EngineeringUnit.FmmfMlbSec:			// Thousands of Pounds/sec
                case EngineeringUnit.FmmfMlbMin:			// Thousands of Pounds/minute
                case EngineeringUnit.FmmfMlbHr:			// Thousands of Pounds/hour
                case EngineeringUnit.FmmfMlbDay:			// Thousands of Pounds/day
                    return EngineeringUnit.FmmfKgSec;

                // Velocity & Rate units
                case EngineeringUnit.FmvrIps:				// Inches/sec
                case EngineeringUnit.FmvrFps:				// Feet/sec
                case EngineeringUnit.FmvrFpm:				// Feet/min
                case EngineeringUnit.FmvrMmSec:			// Millimeters/sec
                case EngineeringUnit.FmvrCmSec:			// Centimeters/sec
                case EngineeringUnit.FmvrMSec:			// Meters/sec
                case EngineeringUnit.FmvrMMin:			// Meters/min
                case EngineeringUnit.FmvrMph:				// Miles per hour
                case EngineeringUnit.FmvrMrph:			// Meters per hour
                case EngineeringUnit.FmvrKmph:			// Kilometers per hour
                case EngineeringUnit.FmvrKnot:			// Knots
                case EngineeringUnit.FmvrMmMin:			// Millimeters/min
                    return EngineeringUnit.FmvrMSec;

                // Density Units
                case EngineeringUnit.FmdGcm3: 			// Grams/cubic cm
                case EngineeringUnit.FmdGMl3:		 		// Grams/cubic millilitre
                case EngineeringUnit.FmdGl3:				// Grams/cubic litre
                case EngineeringUnit.FmdKgM3:	 			// Kilograms/cubic meter
                case EngineeringUnit.FmdKgL3:		  	 	// Kilograms/cubic litre
                case EngineeringUnit.FmdLbIn3:			// Pounds/cubic inch
                case EngineeringUnit.FmdLbFt3:			// Pounds/cubic feet
                case EngineeringUnit.FmdUsLbGal:			// Pounds/gallon
                case EngineeringUnit.FmdImpLbGl:			// Pounds/gallon (imperial)
                case EngineeringUnit.FmdLbBlOil:			// Pounds/barrel (oil)
                case EngineeringUnit.FmdLbBlLiq:			// Pounds/barrel (liquid)
                case EngineeringUnit.FmdDegApi:			// Degrees API
                case EngineeringUnit.FmdSpGrav:			// Specific gravity
                case EngineeringUnit.FmdPrPlato:			// % Plato
                case EngineeringUnit.FmdDegBrix:			// Degrees BRIX
                case EngineeringUnit.FmdDegBmLt:			// Degrees Baum (light)
                case EngineeringUnit.FmdDegBmHy:			// Degrees Baum (heavy)
                case EngineeringUnit.FmdSTnYd3:			// Short tons/cubic yard
                    return EngineeringUnit.FmdKgM3;

                // Energy
                case EngineeringUnit.FmeBtu:				// BTU
                case EngineeringUnit.FmeCal:				// Calories
                case EngineeringUnit.FmeJoule:			// Joules (SI)
                case EngineeringUnit.FmeWh:				// Watt-hours
                case EngineeringUnit.FmeKwH:				// Kilowatt-hours
                    return EngineeringUnit.FmeJoule;

                // Power & Heat Transfer Units
                case EngineeringUnit.FmphBtuSec:			// BTU/sec
                case EngineeringUnit.FmphBtuMin:			// BTU/min
                case EngineeringUnit.FmphBtuHr:			// BTU/hour
                case EngineeringUnit.FmphCalMin:			// Cal/min
                case EngineeringUnit.FmphWatt:			// Watts
                case EngineeringUnit.FmphKWatts:			// KiloWatts
                case EngineeringUnit.FmphKvAmp:			// Kilo Volt-Amps
                case EngineeringUnit.FmphHPower:			// Horsepower
                    return EngineeringUnit.FmphWatt;

                //Electrical Units
                case EngineeringUnit.FmeuMVolts:			// Millivolts
                case EngineeringUnit.FmeuVolt:			// Volts
                    return EngineeringUnit.FmeuVolt;

                case EngineeringUnit.FmeuMAmps:			// Milliamps
                case EngineeringUnit.FmeuAmp:	 			// Amps
                    return EngineeringUnit.FmeuAmp;

                default:
                    return Units;

            }
        }
    }
}
