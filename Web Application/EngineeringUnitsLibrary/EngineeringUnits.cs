using System;
using System.Collections.Generic;
using System.Text;

namespace EngineeringUnitsLibrary
{

	public enum ENGINEERING_UNIT_TYPE
	{
		FMU_ALL,					// All Units
		FMU_TEMP, 				// Temperature Units -	Auto Convert
		FMU_TIME, 				// Time Units			-	Auto Convert
		FMU_LENGTH,				// Length Units		-	Auto Convert
		FMU_AREA, 				// Area Units			-	Auto Convert
		FMU_VOLUME,				// Volume Units		-	Auto Convert
		FMU_MASS, 				// Mass/Weight Units -	Auto Convert
		FMU_PRESSURE,			// Pressure Units 	-	Auto Convert
		FMU_VOLFLOW, 			// Volumetric Flow	-	Auto Convert
		FMU_MASSFLOW,			// Mass Flow			-	Auto Convert
		FMU_VELOCITY,			// Velocity/Rate		-	Auto Convert
		FMU_DENSITY,			// Density Units		-	Auto Convert
		FMU_ENERGY,				// Energy Units		-	Auto Convert
		FMU_POWER,				// Power/Heat XFR 	-	Auto Convert
		FMU_ELECT,				// Electrical			-	Strings Only
		FMU_NODIM,				// Dimensionless		-	Strings Only
		FMU_NONE					// Invalid
	};

	public enum ENGINEERING_UNIT
	{
		FM_SiteUnits = 0,
										// Temperature Units
		FMT_DegC = 1,				// Degrees Celcius
		FMT_DegF,					// Degrees Farenheit
		FMT_DegK, 					// Degrees Kelvin
		FMT_DegR, 					// Degrees Rankine
										// Time Units				
		FMT_Msec = 5, 				// MilliSeconds
		FMT_Sec,						// Seconds
		FMT_Min,						// Minutes
		FMT_Hour, 					// Hours
		FMT_Day,						// Days
		FMT_Week, 	 				// Weeks
		FMT_Month, 					// Months
		FMT_Year,					// Years
										// Length Units
		FML_FtIn8th = 19,			// Feet/Inches/8ths
		FML_MM,			 			// Millimeters
		FML_CM,						// Centimeters
		FML_Meter,					// Meters
		FML_KM,				 		// Kilometers
		FML_16th, 					// 1/16 inch
		FML_Inch,		 			// Inches
		FML_Feet,			 		// Feet
		FML_FtIn16th,	 			// Feet/Inches/16ths
		FML_Yard, 					// Yards
		FML_Mile, 	 				// Miles
										// Area units
		FMA_MM2 = 30,	 			// Millimeters Sq
		FMA_CM2,		 				// Centimeters Sq
		FMA_Meter2,					// Meters Sq
		FMA_KM2,				 		// Kilometers Sq
		FMA_16TH2,					// 1/16 inch Sq
		FMA_Inch2,					// Inches Sq
		FMA_Feet2,			 		// Feet Sq
		FMA_Yard2,	 				// Yards Sq
		FMA_Mile2,			 		// Miles Sq
										// Volume units
		FMV_CM3 = 40,				// Cubic centimeters
		FMV_Meter3,					// Cubic meters
		FMV_Litre,					// Litres
		FMV_Inch3,					// Cubic inches
		FMV_Feet3,					// Cubic feet
		FMV_Yard3,					// Cubic yards
		FMV_USGal,					// US Gallons
		FMV_ImpGal,					// Imp Gallons
		FMV_BlOil,					// Barrels Oil
		FMV_BlLiq,					// Barrels Liquid
		FMV_KL,						// Kilolitres
		FMV_MsFt3,					// 1000 standard cubic feet 
										// Mass Units
		FMM_Gram = 60, 	 		// Grams
		FMM_KG,				 		// Kilograms
		FMM_MTon, 					// Metric Tons
		FMM_Oz,						// Ounces
		FMM_Lb,				 		// Pounds
		FMM_ETon,			 	 	// English Tons
		FMM_STon, 	 				// Short Tons
		FMM_LTon,		 	 		// Long Tons
		FMM_Mlbs,					// Thousands of Pounds
										// Pressure Units
		FMP_Pa = 70,	 			// Pascal (SI)
		FMP_KPa,				 		// Kilopascal
		FMP_KgCm2,			 		// Kg per sq cm
		FMP_Psi,		 				// lb per sq inch
		FMP_PsiG,		 	 		// PSI Gauge
		FMP_PsiA,  					// PSI Absolute
		FMP_InH2O,		 			// In. H2O (@ 68F)
		FMP_FtH2O,	 				// Ft. H2O (@ 68F)
		FMP_InHg,		 	 		// In. Mercury (@ 0C)
		FMP_LbFt2,	 				// Pounds per square foot
		FMP_Torr,					// Torr (@ 0C)
		FMP_Bar,	 					// Bar
		FMP_MBar,		 			// Millibar
		FMP_MMHg,		 	 		// mm Hg (@ 0C)
		FMP_MMH2O,	 				// mm H2O (@ 68F)
		FMP_GmCm2,					// Grams per square cm
		FMP_Atm,	 					// Atmospheres
										// Volumetric Flow units
		FMVF_CCMin = 90, 			// CC/Min
		FMVF_CCHr,	 				// CC/Hour
		FMVF_M3Sec,					// m3/sec
		FMVF_M3Min,					// m3/Minute
		FMVF_M3Hr,					// m3/Hour
		FMVF_M3Day,					// m3/Day
		FMVF_LtSec,					// Litre/sec
		FMVF_LtMin,					// Litres/minute
		FMVF_LtHr,					// Litres/Hour
		FMVF_MLPD,					// Million litres/day
		FMVF_In3Min,				// Cubic inches/minute
		FMVF_In3Hr,					// Cubic inches/hour
		FMVF_Ft3Sec,				// Cubic feet/second
		FMVF_Ft3Min,				// Cubic feet/minute
		FMVF_Ft3Hr,					// Cubic feet/hour
		FMVF_Ft3Day,				// Cubic feet/day
		FMVF_Yd3Min,				// Cubic yards/minute
		FMVF_Yd3Hr,					// Cubic yards/hour
		FMVF_GPS, 					// Gallons/sec (US)
		FMVF_GPM,		 			// Gallons/minute (US)
		FMVF_GPH,		 			// Gallons/hour	(US)
		FMVF_MGPD,					// Millions of gallons/day (US)
		FMVF_ImpGPS,				// Imp gallons/sec
		FMVF_ImpGPM,				// Imp gallons/minute
		FMVF_ImpGPH,				// Imp gallons/hour
		FMVF_ImpMGD,				// Imp millions of gallons/day
		FMVF_BPMoil,				// BBL/min (oil)
		FMVF_BPHoil,				// BBL/hour (oil)
		FMVF_BPDoil,				// BBL/day (oil)
		FMVF_MBDoil,				// Millions barrels/day (oil)
		FMVF_BPMliq,				// BBL/min (liq)
		FMVF_BPHliq,				// BBL/hour (liq)
		FMVF_BPDliq,				// BBL/day (liq)
		FMVF_MBDliq,				// Millions barrels/day (liquid)
		FMVF_KLSec,					// kilolitres/sec
		FMVF_KLMin,					// kilolitres/Minute
		FMVF_KLHr,				 	// kilolitres/Hour
		FMVF_KLDay,					// kilolitres/Day
										// Mass Flow Units
		FMMF_LbSec = 130,			// Pounds/sec
		FMMF_LbMin,					// Pounds/minute
		FMMF_LbHr,					// Pounds/hour
		FMMF_LbDay,					// Pounds/day
		FMMF_MTonMn,				// Metric tons/minute
		FMMF_MTonHr,				// Metric tons/hour
		FMMF_MTonDy,				// Metric tons/day
		FMMF_STonMn,				// Short tons/min
		FMMF_STonHr,				// Short tons/hour
		FMMF_STonDy,				// Short tons/day
		FMMF_LTonMn,				// Long tons/min
		FMMF_LTonHr,				// Long tons/hour
		FMMF_LTonDy,				// Long tons/day
		FMMF_GmSec,					// Grams/sec
		FMMF_GmMin,					// Grams/minute
		FMMF_GmHr,					// Grams/hour
		FMMF_KgSec,					// Kilograms/sec
		FMMF_KgMin,					// Kilograms/minute
		FMMF_KgHr,					// Kilograms/hour
		FMMF_KgDay,					// Kilograms/day
		FMMF_MlbSec,				// Millions of Pounds/sec
		FMMF_MlbMin,				// Millions of Pounds/minute
		FMMF_MlbHr,					// Millions of Pounds/hour
		FMMF_MlbDay,				// Millions of Pounds/day
										// Velocity & Rate units
		FMVR_IPS = 160, 			// Inches/sec
		FMVR_FPS,					// Feet/sec
		FMVR_FPM, 					// Feet/min
		FMVR_MMSec,					// Millimeters/sec
		FMVR_CMSec,					// Centimeters/sec
		FMVR_MSec,					// Meters/sec
		FMVR_MMin,					// Meters/min
		FMVR_MPH,					// Miles per hour
		FMVR_MrPH,					// Meters per hour
		FMVR_KMPH,					// Kilometers per hour
		FMVR_KNOT,					// Knots
		FMVR_MMMin,					// Millimeters/min
										// Density Units
		FMD_GCM3 = 180, 			// Grams/cubic cm
		FMD_GMl3,			 		// Grams/millilitre
		FMD_GL3,						// Grams/litre
		FMD_KgM3,		 			// Kilograms/cubic meter
		FMD_KgL3,			  	 	// Kilograms/litre
		FMD_LbIn3,					// Pounds/cubic inch
		FMD_LbFt3,					// Pounds/cubic feet
		FMD_USLbGal,				// Pounds/gallon
		FMD_ImpLbGl,				// Pounds/gallon (imperial)
		FMD_LbBlOil,				// Pounds/barrel (oil)
		FMD_LbBlLiq,				// Pounds/barrel (liquid)
		FMD_DegAPI,					// Degrees API
		FMD_SpGrav,					// Specific gravity
		FMD_PrPlato,				// % Plato
		FMD_DegBRIX,				// Degrees BRIX
		FMD_DegBmLt,				// Degrees Baum (light)
		FMD_DegBmHy,				// Degrees Baum (heavy)
		FMD_STnYd3 = 199,			// Short tons/cubic yard
		
		// The following are considered obsolete and never used.
		//FMD_DegTwad = 197,		// Degrees Twaddell
		//FMD_DegBal = 198,			// Degrees Balling

										// Energy
		FME_BTU = 200,				// BTU
		FME_Cal,						// Calories
		FME_Joule,					// Joules (SI)
		FME_WH,						// Watt-hours
		FME_KwH,						// Kilowatt-hours
										// Power & Heat Transfer Units
		FMPH_BTUSec = 210,		// BTU/sec
		FMPH_BTUMin,				// BTU/min
		FMPH_BTUHr,					// BTU/hour
		FMPH_CalMin,				// Cal/min
		FMPH_Watt,					// Watts
		FMPH_KWatts,				// KiloWatts
		FMPH_KVAmp,					// Kilo Volt-Amps
		FMPH_HPower,				// Horsepower
										//Electrical Units
		FMEU_MVolts = 220,		// Millivolts
		FMEU_Volt,					// Volts
		FMEU_MAmps,					// Milliamps
		FMEU_Amp,		 			// Amps
		FMEU_Ohm, 					// Ohms
		FMEU_Farad,					// Farads
		FMEU_Coul,					// Coulombs
		FMEU_Henry,					// Henrys
		FMEU_MicSie,				// MicroSiemens
		FMEU_Siemen,				// Siemens
		FMEU_MHO, 					// MHOs
										// Dimensionless Units
		FMDU_PwrFct = 231,		// Power factor
		FMDU_RPM,	 				// Revolutions/min
		FMDU_Hertz,					// Cycles/sec (Hz)
		FMDU_PCent,					// Percent (general)
		FMDU_PPM, 					// Parts per mill
		FMDU_PHumid,				// % Humidity
		FMDU_POxygn,				// % Oxygen
		FMDU_RHumid,				// Relative Humidity
		FMDU_PH,						// pH
										// Miscellaneous units
		FMMU_Centp = 240,			// Centipoise
		FMMU_SolWt,					// Solids by weight
		FMMU_SolVol,				// Solids by volume
		FMMU_StQual,				// Steam quality
		FMMU_Bushel,				// Bushels
		FMMU_PrfVol,				// Proof volume
		FMMU_PrfMas,				// Proof mass
		FMMU_Ft3Lb,					// Cubic feet/pound
	};		


	public class EngineeringUnits
	{
		// Coefficients used for deg_brix calculation
		const double FM_Brixb0 = -608.09478;
		const double FM_Brixb1 = 1.0987222;
		const double FM_Brixb2 = -0.00062906325;
		const double FM_Brixb3 = 1.388771e-7;

		const double FM_Brixs0 = 998.17453;
		const double FM_Brixs1 = 3.9124628;
		const double FM_Brixs2 = 0.0096624811;
		const double FM_Brixs3 = 0.00012518984;
		const double FM_Brixs4 = -6.2021547e-7;

		// Unit overflow condition - caused by divide by 0.
		const double UNIT_OVERFLOW = 1e20;


		struct ENGRUNIT {
			ENGINEERING_UNIT_TYPE	type;
			double						conversionFactor;

			public ENGRUNIT(ENGINEERING_UNIT_TYPE type, double conversionFactor)
			{
				this.type=type;
				this.conversionFactor=conversionFactor;
			}
			
			public ENGINEERING_UNIT_TYPE	Type{get{return type;}}
			public double ConversionFactor{get{return conversionFactor;}}
		}

		static ENGRUNIT [] EngineerUnit = {

			new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE, -1.000000),          // 0   - Undefined

			 // Temperature Units - these require a special conversion
			 // program and therefore have no conversion factors.

			 new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_TEMP, -1.000000),          // 1   - Degrees Centigrade
			 new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_TEMP, -1.000000),          // 2   - Degrees Farenheit
			 new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_TEMP, -1.000000),          // 3   - Kelvin
			 new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_TEMP, -1.000000),          // 4   - Degrees Rankine

			 // Time Units

			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_TIME,  1.00000E-03),       // 5   - Time : Milliseconds
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_TIME,  1.00000),           // 6   -      : Seconds (SI)
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_TIME,  6.00000E+01),       // 7   -      : Minutes
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_TIME,  3.60000E+03),       // 8   -      : Hours 
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_TIME,  8.64000E+04),       // 9   -      : Days
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_TIME,  6.04800E+05),       // 10  -      : Weeks
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_TIME,  2.62080E+06),       // 11  -      : Months
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_TIME,  3.15360E+07),       // 12  -      : Years

			 // Spare

			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE, -1.000000),          // 13  -  * Not Assigned
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE, -1.000000),          // 14  -  * Not Assigned
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE, -1.000000),          // 15  -  * Not Assigned
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE, -1.000000),          // 16  -  * Not Assigned
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE, -1.000000),          // 17  -  * Not Assigned
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE, -1.000000),          // 18  -  * Not Assigned

			 // Length Units

			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_LENGTH, 3.048000E-01),       // 19  - Length : Ft/inch/8th
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_LENGTH, 1.00000E-03),        // 20  -        : Millimeters
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_LENGTH, 1.00000E-02),        // 21  -        : Centimeters
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_LENGTH, 1.000000),           // 22  -        : Meters (SI)
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_LENGTH, 1.000000E+03),       // 23  -        : Kilometers
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_LENGTH, 1.587500E-03),       // 24  -        : 16th of Inch
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_LENGTH, 2.540000E-02),       // 25  -        : inches
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_LENGTH, 3.048000E-01),       // 26  -        : feet
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_LENGTH, 3.048000E-01),       // 27  -        : ft/inch/16th
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_LENGTH, 9.144000E-01),       // 28  -        : yard
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_LENGTH, 1.609347E+03),       // 29  -        : miles

			 // Area Units

			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_AREA,   1.000000E-06),       // 30  - Area   : sq mm
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_AREA,   1.000000E-04),       // 31  -        : sq cm
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_AREA,   1.000000),           // 32  -        : sq meter(SI)
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_AREA,   1.000000E+06),       // 33  -        : sq kilometer
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_AREA,   2.520156E-06),       // 34  -        : sq 16th of in
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_AREA,   6.451600E-04),       // 35  -        : sq inch
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_AREA,   9.290304E-02),       // 36  -        : sq feet
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_AREA,   8.361274E-01),       // 37  -        : sq yard
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_AREA,   2.589998E+06),       // 38  -        : sq mile
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,   -1.000000),          // 39  -  * Not Assigned

			 // Volume Units

			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLUME, 1.000000E-06),       // 40  - Volume : cu. cm
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLUME, 1.000000),           // 41  -        : Cu. Meter (SI)
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLUME, 1.000000E-03),       // 42  -        : Litre
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLUME, 1.638706E-05),       // 43  -        : cu.Inch
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLUME, 2.831685E-02),       // 44  -        : Cu Feet
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLUME, 7.645549E-01),       // 45  -        : Cu yard
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLUME, 3.785412E-03),       // 46  -        : US Gallon
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLUME, 4.546092E-03),       // 47  -        : Imp Gallon
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLUME, 1.589873E-01),       // 48  -        : Barrel Oil

			 // Barrel Liq conversion taken from Mark Rizkallah (units.c)
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLUME, 1.192401E-01),       // 49  -        : Barrel Liq
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLUME, 1.000000),           // 50  -        : Kilolitre
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLUME, 2.6853E+01 ),        // 51  -		  : 1000 standard cubic feet
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,  -1.000000),           // 52  -  * Not Assigned
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,  -1.000000),           // 53  -  * Not Assigned
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,  -1.000000),           // 54  -  * Not Assigned
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,  -1.000000),           // 55  -  * Not Assigned
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,  -1.000000),           // 56  -  * Not Assigned
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,  -1.000000),           // 57  -  * Not Assigned
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,  -1.000000),           // 58  -  * Not Assigned
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,  -1.000000),           // 59  -  * Not Assigned

			 // Mass Units

			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_MASS,   1.000000E-03),			// 60  - Mass	: grams
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_MASS,   1.000000),				// 61  -			: Kilogram (SI)
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_MASS,   1.000000E+03),			// 62  -			: Metric Ton
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_MASS,   2.834952E-02),			// 63  -			: Ounce
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_MASS,   4.535924E-01),			// 64  -			: Pound
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_MASS,   1.000000E+03),			// 65  -			: English Ton
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_MASS,   9.071847E+02),			// 66  -			: Short Ton (2000 lbs)
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_MASS,   1.016047E+03),			// 67  -			: Long Ton
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_MASS,   4.535924E+02),			// 68  -			: Mlbs. (Thousands of Pounds)
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,  -1.000000),				// 69  -  * Not Assigned

			 // Pressure Units

			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_PRESSURE,  1.000000),        // 70  - Pressure: Pascal (SI)
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_PRESSURE,  1.000000E+03),    // 71  -         : KiloPascal
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_PRESSURE,  9.806650E+04),    // 72  -         : kg/ sq cm
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_PRESSURE,  6.894757E+03),    // 73  -         : lb/sq inch
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_PRESSURE,  6.894757E+03),    // 74  -         : PSI Gauge
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_PRESSURE,  6.894757E+03),    // 75  -         : PSI Absolute
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_PRESSURE,  2.490820E+02),    // 76  -         : in. H20 (39.2 F)
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_PRESSURE,  2.988980E+03),    // 77  -         : Ft H20 (39.2 F)
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_PRESSURE,  3.386380E+03),    // 78  -         : In. Mercury(@ 0C)
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_PRESSURE,  4.788026E+01),    // 79  -         : lb/sq ft
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_PRESSURE,  1.333220E+02),    // 80  -         : Torr (@ 0C)
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_PRESSURE,  1.000000E+05),    // 81  -         : Bar
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_PRESSURE,  1.000000E+02),    // 82  -         : MilliBar
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_PRESSURE,  1.333220E+02),    // 83  -         : mm HG (@ 0C)

			 // Derived mm H2O from cm H2O - needs verification
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_PRESSURE,  9.80638E+00),     // 84  -         : mm H2O (@ 4C)
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_PRESSURE,  9.806650E+01),    // 85  -         : gr/ sq cm
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_PRESSURE,  1.013250E+05),    // 86  -         : Atmosphere
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,     -1.000000),        // 87  -  * Not Assigned
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,     -1.000000),        // 88  -  * Not Assigned
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,     -1.000000),        // 89  -  * Not Assigned

			 // Volumetric Flow Units
			 // These need to be verified sometime using a proper book.

			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLFLOW,   1.666667E-08),    // 90  - Vol. Flow : CC/min
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLFLOW,   2.777778E-10),    // 91  -           : CC/hour
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLFLOW,   1.000000),        // 92  -           : m3/sec (SI)
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLFLOW,   1.666667E-02),    // 93  -           : m3/min
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLFLOW,   2.777778E-04),    // 94  -           : m3/hr
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLFLOW,   1.157408E-05),    // 95  -           : m3/day 
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLFLOW,   1.000000E-03),    // 96  -           : lit/sec
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLFLOW,   1.666667E-05),    // 97  -           : lit/min
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLFLOW,   2.777778E-07),    // 98  -           : lit/hour
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLFLOW,   1.157407E-02),    // 99  -           : million lit/day
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLFLOW,   2.731177E-07),    // 100 -           : in3/min
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLFLOW,   4.55195E-09),     // 101 -           : in3/hour
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLFLOW,   2.831685E-02),    // 102 -           : ft3/sec
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLFLOW,   4.719475E-04),    // 103 -           : ft3/min
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLFLOW,   7.865792E-06),    // 104 -           : ft3/hour
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLFLOW,   3.277413E-07),    // 105 -           : ft3/day
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLFLOW,   1.274258E-02),    // 106 -           : yd3/min
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLFLOW,   2.123763E-04),    // 107 -           : yd3/hour
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLFLOW,   3.785412E-03),    // 108 -           : Gal/sec(US)
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLFLOW,   6.309020E-05),    // 109 -           : Gal/min(US)
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLFLOW,   1.051503E-06),    // 110 -           : Gal/Hour(US)
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLFLOW,   4.381264E-02),    // 111 -           : Million Gal/Day(US)
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLFLOW,   4.546092E-03),    // 112 -           : Gal/Sec(IMP)
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLFLOW,   7.576820E-05),    // 113 -           : Gal/Min(IMP)
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLFLOW,   1.262803E-06),    // 114 -           : Gal/Hour(IMP)
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLFLOW,   5.261680E-02),    // 115 -           : Million Gal/Day(IMP)
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLFLOW,   2.649788E-03),    // 116 -           : BBL/min(OIL)
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLFLOW,   4.416314E-05),    // 117 -           : BBL/hour(Oil)
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLFLOW,   1.840131E-06),    // 118 -           : BBL/day(Oil)
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLFLOW,   1.840131),        // 119 -           : MBPD
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLFLOW,   1.987335E-03),    // 120 -           : bar/Min(liq)
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLFLOW,   3.312225E-05),    // 121 -           : bar/hr(Liq)
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLFLOW,   1.380094E-06),    // 122 -           : bar/day(Liq)
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLFLOW,   1.380094),        // 123 -           : MBPD(Liq)
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLFLOW,   1.000000),        // 124 -           : kl/sec
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLFLOW,   1.666667E-02),    // 125 -           : kl/min
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLFLOW,   2.777778E-04),    // 126 -           : kl/hour
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VOLFLOW,   1.157408E-05),    // 127 -           : kl/day
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,     -1.000000),        // 128 -  * Not Assigned
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,     -1.000000),        // 129 -  * Not Assigned

			 // Mass Flow Units
			 // SI = Kg/Sec

			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_MASSFLOW,  4.535924E-01),    // 130	- Mass Flow : lb per sec
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_MASSFLOW,  7.559873E-03),    // 131	-           : lb per min
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_MASSFLOW,  1.259979E-04),    // 132	-           : lb per hour
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_MASSFLOW,  5.249912E-06),    // 133	-           : lb per day
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_MASSFLOW,  1.666667E+01),    // 134	-           : metric tons per min
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_MASSFLOW,  2.777778E-01),    // 135	-           : metric tons per hour
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_MASSFLOW,  1.157407E-02),    // 136	-           : metric tons per day
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_MASSFLOW,  1.511975E+01),    // 137	-           : short tons per min
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_MASSFLOW,  2.519958E-01),    // 138	-           : short tons per hour
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_MASSFLOW,  1.049983E-02),    // 139	-           : short tons per day
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_MASSFLOW,  1.693412E+01),    // 140	-           : long tons per min
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_MASSFLOW,  2.822353E-01),    // 141	-           : long tons per hour
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_MASSFLOW,  1.175980E-02),    // 142	-           : long tons per day
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_MASSFLOW,  1.000000E-03),    // 143	-           : g/sec 
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_MASSFLOW,  1.666667E-05),    // 144	-           : g/min     
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_MASSFLOW,  2.777778E-07),    // 145	-           : g/hr
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_MASSFLOW,  1.000000),        // 146	-           : Kg/Sec (SI)
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_MASSFLOW,  1.666667E-02),    // 147	-           : Kg/min
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_MASSFLOW,  2.777778E-04),    // 148	-           : Kg/hr
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_MASSFLOW,  1.157407E-05),    // 149	-           : Kg/day
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_MASSFLOW,  4.535924E+02),    // 150	-				: Mlbs/Sec
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_MASSFLOW,  7.559873E+00),    // 151	- 				: Mlbs/Minute
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_MASSFLOW,  1.259979E-01),    // 152	- 				: Mlbs/Hour
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_MASSFLOW,  5.249912E-03),    // 153	- 				: Mlbs/Day
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,      1.000000),        // 154	-  * Not Assigned
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,      1.000000),        // 155	-  * Not Assigned
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,      1.000000),        // 156	-  * Not Assigned
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,      1.000000),        // 157	-  * Not Assigned
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,      1.000000),        // 158	-  * Not Assigned
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,      1.000000),        // 159	-  * Not Assigned

			 // Velocity & Rate Units

			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VELOCITY,  2.540000E-02),    // 160    - Velocity  : in per sec
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VELOCITY,  3.048000E+01),    // 161    -           : ft per sec
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VELOCITY,  5.080000E-03),    // 162    -           : ft per min
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VELOCITY,  1.000000E-03),    // 163    -           : mm per sec
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VELOCITY,  1.000000E-02),    // 164    -           : cm per sec
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VELOCITY,  1.000000),        // 165    -           : meter per sec (SI)
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VELOCITY,  1.666667E-02),    // 166    -           : meter per min
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VELOCITY,  4.470400E-01),    // 167    -           : miles per hour
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VELOCITY,  2.777778E-04),    // 168    -           : meter/hour
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VELOCITY,  2.777778E-01),    // 169    -           : kilometer/hour
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VELOCITY,  5.144444E-01),    // 170    -           : knots
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_VELOCITY,  1.666667E-05),    // 163    -           : mm per min
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,      1.000000),        // 172    -  * Not Assigned
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,      1.000000),        // 173    -  * Not Assigned
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,      1.000000),        // 175    -  * Not Assigned
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,      1.000000),        // 174    -  * Not Assigned
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,      1.000000),        // 176    -  * Not Assigned
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,      1.000000),        // 177    -  * Not Assigned
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,      1.000000),        // 178    -  * Not Assigned
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,      1.000000),        // 179    -  * Not Assigned

			 // Density Units

			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_DENSITY,   1.000000E+03),       // 180    - Density   : gram/cu cm
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_DENSITY,   1.000000E+03),       // 181    -           : gram/millilitre
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_DENSITY,   1.000000),           // 182    -           : gram/liter
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_DENSITY,   1.000000),           // 183    -           : kilogram/cu meter (SI)
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_DENSITY,   1.000000E+03),       // 184    -           : kilogram/liter
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_DENSITY,   2.767990E+4),        // 185    -           : lb/cu inch
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_DENSITY,   1.601846E+1),        // 186    -           : lb/cu ft
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_DENSITY,   1.19829363E+2),      // 187    -           : lb/gal(US)
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_DENSITY,   9.977633E+1),        // 188    -           : lb/gal(IMP)
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_DENSITY,   2.853010),           // 189    -           : lb/barrel(Oil)
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_DENSITY,   3.804026),           // 190    -           : lb/barrel(LIQ)
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_DENSITY,   -1.000000),          // 191    -           : Degrees API
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_DENSITY,   -1.000000),          // 192    -           : Specific Gravity
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_DENSITY,   -1.000000),          // 193    -           : % Plato
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_DENSITY,   -1.000000),          // 194    -           : deg BRIX
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_DENSITY,   -1.000000),          // 195    -           : Deg Baume (light)
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_DENSITY,   -1.000000),          // 196    -           : Deg Baume (heavy)
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,      -1.000000),          // 197    -           : Deg Twaddell
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,      -1.000000),          // 198    -           : Deg Balling
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_DENSITY,   1.186553E+03),       // 199    -           : short ton/cu yard

			 // Energy

			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,     -1.000000),           // 200    - Energy    : BTU
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,     -1.000000),           // 201    -           : calorie
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_ENERGY,    1.000000),           // 202    -           : Joule (SI)
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_ENERGY,    3.600000E+03),       // 203    -           : Watt-Hour
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_ENERGY,    3.600000E+06),       // 204    -           : KiloWatt-Hour
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,     -1.000000),           // 205    -  * Not Assigned
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,     -1.000000),           // 206    -  * Not Assigned
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,     -1.000000),           // 207    -  * Not Assigned
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,     -1.000000),           // 208    -  * Not Assigned
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,     -1.000000),           // 209    -  * Not Assigned

			 // Power & Heat Transfer Units

			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_POWER,      1.055056E+03),      // 210    - Power     : BTU per sec
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_POWER,      1.758427E+01),      // 211    -           : BTU per min
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_POWER,      2.930722E-01),      // 212    -           : BTU per hour
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_POWER,      6.978000E-02),      // 213    -           : cal per min
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_POWER,      1.000000),          // 214    -           : Watts (SI)
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_POWER,      1.000000E+03),      // 215    -           : Kilowatts
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,      -1.000000),          // 216    -           : Kilo Volt-Amp
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,      -1.000000),          // 217    -           : Horsepower
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,      -1.000000),          // 218    -  * Not Assigned
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,      -1.000000),          // 219    -  * Not Assigned

			 // Electrical Units
			 // Don't know about these

			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_ELECT,      -1.000000),          // 220    - Electrical: millivolts
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_ELECT,      -1.000000),          // 221    -           : Volts
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_ELECT,      -1.000000),          // 222    -           : milliamps
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_ELECT,      -1.000000),          // 223    -           : Amps
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_ELECT,      -1.000000),          // 224    -           : ohms
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_ELECT,      -1.000000),          // 225    -           : farad
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_ELECT,      -1.000000),          // 226    -           : Coulomb
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_ELECT,      -1.000000),          // 227    -           : Henry
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_ELECT,      -1.000000),          // 228    -           : MicroSiemens
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_ELECT,      -1.000000),          // 229    -           : Siemens
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_ELECT,      -1.000000),          // 230    -           : MHO

			 // Dimensionless Units
			 // Don't know about these

			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NODIM,      -1.000000),       // 231    -           : Power Factor
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NODIM,      -1.000000),       // 232    -           : Rev per Min
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NODIM,      -1.000000),       // 233    -           : Cycle per Sec
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NODIM,      -1.000000),       // 234    - General   : Percent
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NODIM,      -1.000000),       // 235    -           : parts per mill
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NODIM,      -1.000000),       // 236    -           : % Humidity
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NODIM,      -1.000000),       // 237    -           : % Oxygen
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NODIM,      -1.000000),       // 238    -           : Relative Humidy
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NODIM,      -1.000000),       // 239    -           : pH

			 // Miscellaneous Units

			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NODIM,     -1.000000),          // 240    -  : centipoise
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NODIM,     -1.000000),          // 241    -  : % solids by Wt
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NODIM,     -1.000000),          // 242    -  : % solids by vol
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NODIM,     -1.000000),          // 243    -  : % steam quality
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NODIM,     -1.000000),          // 244    -  : Bushel
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NODIM,     -1.000000),          // 245    -  : proof volume
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NODIM,     -1.000000),          // 246    -  : proof mass
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NODIM,     -1.000000),          // 247    -  : cu ft / pound
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,     -1.000000),          // 248    -  * Not Assigned
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,     -1.000000),          // 249    -  * Not Assigned
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,     -1.000000),          // 250    -  * Not Assigned
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,     -1.000000),          // 251    -  * Not Assigned
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,     -1.000000),          // 252    -  * Not Assigned
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,     -1.000000),          // 253    -  * Not Assigned
			  new ENGRUNIT(ENGINEERING_UNIT_TYPE.FMU_NONE,     -1.000000)           // 254    -  * Not Assigned

		};

		public static double Convert(double fromValue,ENGINEERING_UNIT fromUnit, ENGINEERING_UNIT toUnit, double referenceValue)
		{
			double toValue=0.0;
			Convert(fromValue, fromUnit, ref toValue, toUnit, referenceValue);
			return toValue;
		}

		public static void Convert(double fromValue,ENGINEERING_UNIT fromUnit, ref double toValue, ENGINEERING_UNIT toUnit, double referenceValue)
		{
			double  toSIScale, fromSIScale;
	 
			// Take a local copy of the To and From unit types
			ENGINEERING_UNIT_TYPE toType   = EngineerUnit[(int) toUnit].Type;
			ENGINEERING_UNIT_TYPE fromType = EngineerUnit[(int) fromUnit].Type;

			// Check that both units are not unassigned and are in range
			if((ENGINEERING_UNIT_TYPE.FMU_NONE == toType) ||
			(ENGINEERING_UNIT_TYPE.FMU_NONE == fromType))
			{
				// Invalid units - set error code and return FALSE
				throw new Exception("Bad Units");
			}

			// If the units are the same then do a simple copy and exit
			if( toUnit == fromUnit )
			{
				toValue = fromValue;
				return;
			}

			// Take a local copy of the SI conversion scales
			toSIScale   = EngineerUnit[(int) toUnit].ConversionFactor;
			fromSIScale = EngineerUnit[(int) fromUnit].ConversionFactor;


			// Ensure that both units are within the same type range.
			if (toType != fromType)
				throw new Exception("Unit Type Mismatch");

			// Check the SI conversion scales in case special processing is
			//  required for the conversion. 
			if(toSIScale   != -1.00000
			&& fromSIScale != -1.00000)
			{
				// Simple Conversion - calculate and return the data.
				toValue = (( fromValue * fromSIScale ) / toSIScale );

				return;
			}
			else
			{
				// Special conversion required - Need to determine if the
				// calculation is for temperature or density.
				switch (toType)
				{
					case ENGINEERING_UNIT_TYPE.FMU_TEMP:
					{
						// Invoke TempConvert to do temperature conversion
						toValue= TempConvert( fromValue, toUnit, fromUnit );
						return;
					}

					case ENGINEERING_UNIT_TYPE.FMU_DENSITY:
					{
						// Invoke DensityConvert to calculate certain
						// convoluted densities

						DensityConvert( ref toValue, fromValue, toUnit,
								fromUnit, referenceValue );

						// Check if the density conversion returned a
						// unit overflow error (divide by zero)
						if( toValue == UNIT_OVERFLOW )
							throw new Exception("Overflow");

						return;
					}
				}
			}

			// If processing gets to here then nothing was converted.
			throw new Exception("No Conversiont");
		} 

		static double WaterDensity(double dTemperature)
		{
			return ((((((((((-280.5425E-12 * dTemperature ) + 105.56302E-9 ) *
								  dTemperature) - 46.170461E-6) * dTemperature) -
								  7.9870401E-3) * dTemperature) + 16.945176) *
								  dTemperature) + 999.83952) / (1.0 + 16.879850E-3 *
								  dTemperature);
		}


		static void DensityConvert( ref double toValue, double fromValue, ENGINEERING_UNIT toUnit,ENGINEERING_UNIT fromUnit,double	referenceValue)
		{
			double   dKgPerM3=0.0;

			switch(toUnit)
			{
				// Convert to API from any other unit
				case ENGINEERING_UNIT.FMD_DegAPI:
				{
					// First convert to Kg/m3
					Convert( fromValue, fromUnit, ref dKgPerM3, ENGINEERING_UNIT.FMD_KgM3, referenceValue );

					// Convert from Kg/m3 to API
					toValue = (( 141.5*999.012 ) / dKgPerM3 ) - 131.5;
					break;
				}

				// Convert to Specific Gravity from any other units
				//  Extra Param is the temp in C

				case ENGINEERING_UNIT.FMD_SpGrav:
				{
					// Convert to Kg/m3
					Convert(fromValue, fromUnit, ref dKgPerM3, ENGINEERING_UNIT.FMD_KgM3, referenceValue);

					// Convert from Kg/m3 -> Specific Gravity
					toValue = dKgPerM3/WaterDensity( referenceValue );
					break;
				}

				// Convert to % Plato from any other units
				case ENGINEERING_UNIT.FMD_PrPlato:
				{
					// Convert to Kg/m3
					Convert(fromValue, fromUnit, ref dKgPerM3, ENGINEERING_UNIT.FMD_KgM3, referenceValue);

					// Convert from Kg/m3 -> %Plato
					toValue = ( 141360.48 / ( dKgPerM3 + 131.5 ));
					break;
				}

				// Convert to degrees Brix from any other units

				case ENGINEERING_UNIT.FMD_DegBRIX:
				{
					// Convert to Kg/m3
					Convert(fromValue, fromUnit, ref dKgPerM3, ENGINEERING_UNIT.FMD_KgM3, referenceValue);
			
					// Convert to BRIX
					toValue = ((((((FM_Brixb3 * dKgPerM3)
					+ FM_Brixb2) * dKgPerM3) + FM_Brixb1) * dKgPerM3)+
					FM_Brixb0);
					break;
				}

				// Convert to Baume Light from any other units
				// Watch for divide by zero
				case ENGINEERING_UNIT.FMD_DegBmLt:
				{
					// Convert to Kg/m3
					Convert(fromValue, fromUnit, ref dKgPerM3, ENGINEERING_UNIT.FMD_KgM3, referenceValue);

					// Convert to Baume Light
					toValue = ((140.0 * WaterDensity(referenceValue)) / dKgPerM3)-130.0;
					break;
				}

				// Convert to Baume heavy from any other units
				// Watch for divide by zero
				case ENGINEERING_UNIT.FMD_DegBmHy:
				{
					// Convert to Kg/m3
					Convert(fromValue, fromUnit, ref dKgPerM3, ENGINEERING_UNIT.FMD_KgM3, referenceValue);

					// Convert to Baume Heavy
					toValue= 145.0 - ((145.0 * WaterDensity(referenceValue)) / dKgPerM3);
					break;
				}

				default:
				{
 					switch(fromUnit)
					{

						// from API to any other units
						case ENGINEERING_UNIT.FMD_DegAPI:
						{
							dKgPerM3=(141.5 * 999.012)/(131.5 + fromValue);

							// convert from kg/m3
							Convert(dKgPerM3, ENGINEERING_UNIT.FMD_KgM3, ref toValue, toUnit, referenceValue);
							break;
						}

						// from specific gravity to any other units

						case ENGINEERING_UNIT.FMD_SpGrav:
						{
							dKgPerM3= fromValue * WaterDensity(referenceValue);
			
							// convert from kg/m3
							Convert(dKgPerM3, ENGINEERING_UNIT.FMD_KgM3, ref toValue, toUnit, referenceValue);
							break;
						}

						// from % plato to any other units

						case ENGINEERING_UNIT.FMD_PrPlato:
						{
							dKgPerM3 = (141360.48 / fromValue) - 131.5;

							// convert from kg/m3
							Convert(dKgPerM3, ENGINEERING_UNIT.FMD_KgM3, ref toValue, toUnit, referenceValue);
							break;
						}

						// from degrees brix to any other units

						case ENGINEERING_UNIT.FMD_DegBRIX:
						{
							dKgPerM3=((((((((FM_Brixs4 * fromValue) + FM_Brixs3) *
							fromValue) + FM_Brixs2) * fromValue) + FM_Brixs1) *
							fromValue) + FM_Brixs0);

							// convert from kg/m3
							Convert(dKgPerM3, ENGINEERING_UNIT.FMD_KgM3, ref toValue, toUnit, referenceValue);
                
							break;
						}

						// from baume light to any other units

						case ENGINEERING_UNIT.FMD_DegBmLt:
						{
							dKgPerM3=(140.0 * WaterDensity(referenceValue)) / (fromValue + 130.0);

							// convert from kg/m3
							Convert(dKgPerM3, ENGINEERING_UNIT.FMD_KgM3, ref toValue, toUnit, referenceValue);
							break;
						}

						// from baume heavy to any other units
						case ENGINEERING_UNIT.FMD_DegBmHy:
						{
							dKgPerM3=(145.0 * WaterDensity(referenceValue)) / (145.0 - fromValue);

							// convert from kg/m3
							Convert(dKgPerM3, ENGINEERING_UNIT.FMD_KgM3, ref toValue, toUnit, referenceValue);
							break;
						}

						default:
							break;
					}
				}

				break;
			}
		}



		static double TempConvert( double fromValue, ENGINEERING_UNIT toUnit, ENGINEERING_UNIT fromUnit )
		{
			 switch( toUnit )
			 {
				  // Converting to Celcius from anything but Celcius
				  case ENGINEERING_UNIT.FMT_DegC:
				  {
            
						switch( fromUnit )
						{
							case ENGINEERING_UNIT.FMT_DegF:
								// Convert from Fahrenheit to Celcius
								return(( fromValue - 32 ) / 1.8 );


							case ENGINEERING_UNIT.FMT_DegK:
								// Convert from Kelvin to Celcius
								return( fromValue - 273.15 );

							 case ENGINEERING_UNIT.FMT_DegR:
								  // Convert from Rankine to Celcius.
								  // First convert to Kelvin + then subtract 273.15
								  return(( fromValue / 1.8 ) - 273.15 );
						}
						break;
				  }

				  // Converting to Fahrenheit from anything but Fahrenheit
				  case ENGINEERING_UNIT.FMT_DegF:
				  {
            
						switch( fromUnit )
						{
							 case ENGINEERING_UNIT.FMT_DegC:
								  // Convert from Celcius to Fahrenheit
								  return(( fromValue * 1.8 ) + 32 );

							 case ENGINEERING_UNIT.FMT_DegK:
								  // Convert from Kelvin to Fahrenheit
								  return((( fromValue - 273.15 ) * 1.8 ) + 32 );


							 case ENGINEERING_UNIT.FMT_DegR:
								  // Convert from Rankine to Fahrenheit.
								  return( fromValue - 459.67 );
						}
						break;
				  }

				  // Converting to Kelvin from anything but Kelvin
				  case ENGINEERING_UNIT.FMT_DegK:
				  {
            
						switch( fromUnit )
						{
							 case ENGINEERING_UNIT.FMT_DegC:
								  // Convert from Celcius to Kelvin
								  return( fromValue + 273.15 );

							 case ENGINEERING_UNIT.FMT_DegF:
								  // Convert from Fahrenheit to Kelvin
								  return(( fromValue + 459.67 ) / 1.8 );

							 case ENGINEERING_UNIT.FMT_DegR:
								  // Convert from Rankine to Kelvin.
								  return(fromValue / 1.8);
						}
						break;
				  }

				  // Converting to Rankine from anything but Rankine
				  case ENGINEERING_UNIT.FMT_DegR:
				  {
            
						switch (fromUnit)
						{
							 case ENGINEERING_UNIT.FMT_DegC:
								  // Convert from Celcius to Rankine
								  return((fromValue + 273.15) * 1.8);

							 case ENGINEERING_UNIT.FMT_DegF:
								  // Convert from Fahrenheit to Rankine
								  return(fromValue + 459.67);


							 case ENGINEERING_UNIT.FMT_DegK:
								  // Convert from Kelvin to Rankine.
								  return(fromValue * 1.8);
						}

						break;
				  }
			 }

			 return(0);
		}


		public static string GetUnitString(ENGINEERING_UNIT unit)
		{
			switch(unit)
			{
				case ENGINEERING_UNIT.FMT_DegC:
					return "Degrees Celcius";
				case ENGINEERING_UNIT.FMT_DegF:
					return "Degrees Fahrenheit";
				case ENGINEERING_UNIT.FMT_DegK:
					return "Degrees Kelvin";
				case ENGINEERING_UNIT.FMT_DegR:
					return "Degrees Rankine";
				case ENGINEERING_UNIT.FMT_Msec:
					return "MilliSeconds";
				case ENGINEERING_UNIT.FMT_Sec:
					return "Seconds";
				case ENGINEERING_UNIT.FMT_Min:
					return "Minutes";
				case ENGINEERING_UNIT.FMT_Hour:
					return "Hours";
				case ENGINEERING_UNIT.FMT_Day:
					return "Days";
				case ENGINEERING_UNIT.FMT_Week:
					return "Weeks";
				case ENGINEERING_UNIT.FMT_Month:
					return "Months";
				case ENGINEERING_UNIT.FMT_Year:
					return "Years";
				case ENGINEERING_UNIT.FML_FtIn8th:
					return "ft/inch/8th";
				case ENGINEERING_UNIT.FML_MM:
					return "Millimeters";
				case ENGINEERING_UNIT.FML_CM:
					return "Centimeters";
				case ENGINEERING_UNIT.FML_Meter:
					return "Meters";
				case ENGINEERING_UNIT.FML_KM:
					return "Kilometers";
				case ENGINEERING_UNIT.FML_16th:
					return "16th of Inch";
				case ENGINEERING_UNIT.FML_Inch:
					return "inches";
				case ENGINEERING_UNIT.FML_Feet:
					return "feet";
				case ENGINEERING_UNIT.FML_FtIn16th:
					return "ft/inch/16th";
				case ENGINEERING_UNIT.FML_Yard:
					return "yards";
				case ENGINEERING_UNIT.FML_Mile:
					return "miles";
				case ENGINEERING_UNIT.FMA_MM2:
					return "square millimeters";
				case ENGINEERING_UNIT.FMA_CM2:
					return "square centimeters";
				case ENGINEERING_UNIT.FMA_Meter2:
					return "square meters";
				case ENGINEERING_UNIT.FMA_KM2:
					return "square kilometers";
				case ENGINEERING_UNIT.FMA_16TH2:
					return "square 16ths inch";
				case ENGINEERING_UNIT.FMA_Inch2:
					return "square inches";
				case ENGINEERING_UNIT.FMA_Feet2:
					return "square feet";
				case ENGINEERING_UNIT.FMA_Yard2:
					return "square yards";
				case ENGINEERING_UNIT.FMA_Mile2:
					return "square miles";
				case ENGINEERING_UNIT.FMV_CM3:
					return "cubic centimeters";
				case ENGINEERING_UNIT.FMV_Meter3:
					return "cubic meters";
				case ENGINEERING_UNIT.FMV_Litre:
					return "liters";
				case ENGINEERING_UNIT.FMV_Inch3:
					return "cubic inches";
				case ENGINEERING_UNIT.FMV_Feet3:
					return "cubic feet";
				case ENGINEERING_UNIT.FMV_Yard3:
					return "cubic yards";
				case ENGINEERING_UNIT.FMV_USGal:
					return "U.S. Gallons";
				case ENGINEERING_UNIT.FMV_ImpGal:
					return "Imperial Gallons";
				case ENGINEERING_UNIT.FMV_BlOil:
					return "Barrels (Oil)";
				case ENGINEERING_UNIT.FMV_BlLiq:
					return "Barrels Liquid";
				case ENGINEERING_UNIT.FMV_KL:
					return "kiloliters";
            case ENGINEERING_UNIT.FMV_MsFt3:
               return "1000 standard cubic feet";
				case ENGINEERING_UNIT.FMM_Gram:
					return "grams";
				case ENGINEERING_UNIT.FMM_KG:
					return "Kilograms";
				case ENGINEERING_UNIT.FMM_MTon:
					return "Metric Tons";
				case ENGINEERING_UNIT.FMM_Oz:
					return "Ounces";
				case ENGINEERING_UNIT.FMM_Lb:
					return "Pounds";
				case ENGINEERING_UNIT.FMM_ETon:
					return "English Tons";
				case ENGINEERING_UNIT.FMM_STon:
					return "Short Tons";
				case ENGINEERING_UNIT.FMM_LTon:
					return "Long Tons";
				case ENGINEERING_UNIT.FMM_Mlbs:
					return "Pounds (Thousands)";
				case ENGINEERING_UNIT.FMP_Pa:
					return "Pascals";
				case ENGINEERING_UNIT.FMP_KPa:
					return "KiloPascals";
				case ENGINEERING_UNIT.FMP_KgCm2:
					return "kilograms/sq cm";
				case ENGINEERING_UNIT.FMP_Psi:
					return "pounds/sq in (PSI)";
				case ENGINEERING_UNIT.FMP_PsiG:
					return "PSI Gauge";
				case ENGINEERING_UNIT.FMP_PsiA:
					return "PSI Absolute";
				case ENGINEERING_UNIT.FMP_InH2O:
					return "inches H2O @ 68F";
				case ENGINEERING_UNIT.FMP_FtH2O:
					return "feet H2O @ 68F";
				case ENGINEERING_UNIT.FMP_InHg:
					return "Inches Mercury @ 0C";
				case ENGINEERING_UNIT.FMP_LbFt2:
					return "pounds per sq. ft";
				case ENGINEERING_UNIT.FMP_Torr:
					return "Torr @ 0C";
				case ENGINEERING_UNIT.FMP_Bar:
					return "Bar";
				case ENGINEERING_UNIT.FMP_MBar:
					return "MilliBar";
				case ENGINEERING_UNIT.FMP_MMHg:
					return "mm Mercury @ 0C";
				case ENGINEERING_UNIT.FMP_MMH2O:
					return "mm H2O @ 68 F";
				case ENGINEERING_UNIT.FMP_GmCm2:
					return "grams/sq cm";
				case ENGINEERING_UNIT.FMP_Atm:
					return "Atmospheres";
				case ENGINEERING_UNIT.FMVF_CCMin:
					return "cc per min";
				case ENGINEERING_UNIT.FMVF_CCHr:
					return "cc per hour";
				case ENGINEERING_UNIT.FMVF_M3Sec:
					return "cu. meters per sec";
				case ENGINEERING_UNIT.FMVF_M3Min:
					return "cu. meters per min";
				case ENGINEERING_UNIT.FMVF_M3Hr:
					return "cu. meters per hour";
				case ENGINEERING_UNIT.FMVF_M3Day:
					return "cu. meters per day";
				case ENGINEERING_UNIT.FMVF_LtSec:
					return "liters per sec";
				case ENGINEERING_UNIT.FMVF_LtMin:
					return "liters per min";
				case ENGINEERING_UNIT.FMVF_LtHr:
					return "liters per hour";
				case ENGINEERING_UNIT.FMVF_MLPD:
					return "million liters / day";
				case ENGINEERING_UNIT.FMVF_In3Min:
					return "cu. inches / min";
				case ENGINEERING_UNIT.FMVF_In3Hr:
					return "cu. inches / hour";
				case ENGINEERING_UNIT.FMVF_Ft3Sec:
					return "cu. feet / sec";
				case ENGINEERING_UNIT.FMVF_Ft3Min:
					return "cu. feet / min";
				case ENGINEERING_UNIT.FMVF_Ft3Hr:
					return "cu. feet / hour";
				case ENGINEERING_UNIT.FMVF_Ft3Day:
					return "cu. feet / day";
				case ENGINEERING_UNIT.FMVF_Yd3Min:
					return "cu. yards / min";
				case ENGINEERING_UNIT.FMVF_Yd3Hr:
					return "cu. yards / hour";
				case ENGINEERING_UNIT.FMVF_GPS:
					return "U.S. Gallons / sec";
				case ENGINEERING_UNIT.FMVF_GPM:
					return "U.S. Gallons / min";
				case ENGINEERING_UNIT.FMVF_GPH:
					return "U.S. Gallons / hour";
				case ENGINEERING_UNIT.FMVF_MGPD:
					return "Mill. U.S. Gallons / day";
				case ENGINEERING_UNIT.FMVF_ImpGPS:
					return "U.K. Gallons / sec";
				case ENGINEERING_UNIT.FMVF_ImpGPM:
					return "U.K. Gallons / min";
				case ENGINEERING_UNIT.FMVF_ImpGPH:
					return "U.K. Gallons / hour";
				case ENGINEERING_UNIT.FMVF_ImpMGD:
					return "Mill. U.K. Gallons / day";
				case ENGINEERING_UNIT.FMVF_BPMoil:
					return "bbl per min  (oil)";
				case ENGINEERING_UNIT.FMVF_BPHoil:
					return "bbl per hour (oil)";
				case ENGINEERING_UNIT.FMVF_BPDoil:
					return "bbl per day  (oil)";
				case ENGINEERING_UNIT.FMVF_MBDoil:
					return "Mbbl / day (oil)";
				case ENGINEERING_UNIT.FMVF_BPMliq:
					return "bbl per min  (liq)";
				case ENGINEERING_UNIT.FMVF_BPHliq:
					return "bbl per hour (liq)";
				case ENGINEERING_UNIT.FMVF_BPDliq:
					return "bbl per day  (liq)";
				case ENGINEERING_UNIT.FMVF_MBDliq:
					return "Mbbl / day (liq)";
				case ENGINEERING_UNIT.FMVF_KLSec:
					return "kiloliters / sec";
				case ENGINEERING_UNIT.FMVF_KLMin:
					return "kiloliters / min";
				case ENGINEERING_UNIT.FMVF_KLHr:
					return "kiloliters / hr";
				case ENGINEERING_UNIT.FMVF_KLDay:
					return "kiloliters / day";
				case ENGINEERING_UNIT.FMMF_LbSec:
					return "pounds per sec";
				case ENGINEERING_UNIT.FMMF_LbMin:
					return "pounds per min";
				case ENGINEERING_UNIT.FMMF_LbHr:
					return "pounds per hour";
				case ENGINEERING_UNIT.FMMF_LbDay:
					return "pounds per day";
				case ENGINEERING_UNIT.FMMF_MTonMn:
					return "metric tons per min";
				case ENGINEERING_UNIT.FMMF_MTonHr:
					return "metric tons per hour";
				case ENGINEERING_UNIT.FMMF_MTonDy:
					return "metric tons per day";
				case ENGINEERING_UNIT.FMMF_STonMn:
					return "short tons per min";
				case ENGINEERING_UNIT.FMMF_STonHr:
					return "short tons per hour";
				case ENGINEERING_UNIT.FMMF_STonDy:
					return "short tons per day";
				case ENGINEERING_UNIT.FMMF_LTonMn:
					return "long tons per min";
				case ENGINEERING_UNIT.FMMF_LTonHr:
					return "long tons per hour";
				case ENGINEERING_UNIT.FMMF_LTonDy:
					return "long tons per day";
				case ENGINEERING_UNIT.FMMF_GmSec:
					return "grams per sec";
				case ENGINEERING_UNIT.FMMF_GmMin:
					return "grams per min";
				case ENGINEERING_UNIT.FMMF_GmHr:
					return "grams per hour";
				case ENGINEERING_UNIT.FMMF_KgSec:
					return "kilograms per sec";
				case ENGINEERING_UNIT.FMMF_KgMin:
					return "kilograms per min";
				case ENGINEERING_UNIT.FMMF_KgHr:
					return "kilograms per hr";
				case ENGINEERING_UNIT.FMMF_KgDay:
					return "kilograms per day";
				case ENGINEERING_UNIT.FMMF_MlbSec:
					return "M pounds per sec";
				case ENGINEERING_UNIT.FMMF_MlbMin:
					return "M pounds per min";
				case ENGINEERING_UNIT.FMMF_MlbHr:
					return "M pounds per hour";
				case ENGINEERING_UNIT.FMMF_MlbDay:
					return "M pounds per day";
				case ENGINEERING_UNIT.FMVR_IPS:
					return "inches per sec";
				case ENGINEERING_UNIT.FMVR_FPS:
					return "feet per second";
				case ENGINEERING_UNIT.FMVR_FPM:
					return "feet per minute";
				case ENGINEERING_UNIT.FMVR_MMSec:
					return "millimeters per sec";
				case ENGINEERING_UNIT.FMVR_CMSec:
					return "centimeters per sec";
				case ENGINEERING_UNIT.FMVR_MSec:
					return "meters per second";
				case ENGINEERING_UNIT.FMVR_MMin:
					return "meters per minute";
				case ENGINEERING_UNIT.FMVR_MPH:
					return "miles per hour";
				case ENGINEERING_UNIT.FMVR_MrPH:
					return "meters per hour";
				case ENGINEERING_UNIT.FMVR_KMPH:
					return "kilometers per hour";
				case ENGINEERING_UNIT.FMVR_KNOT:
					return "knots";
				case ENGINEERING_UNIT.FMVR_MMMin:
					return "millimeters / min";
				case ENGINEERING_UNIT.FMD_GCM3:
					return "grams / cu. cm.";
				case ENGINEERING_UNIT.FMD_GMl3:
					return "grams / milliliter";
				case ENGINEERING_UNIT.FMD_GL3:
					return "grams / liter";
				case ENGINEERING_UNIT.FMD_KgM3:
					return "kilograms / cu. meter";
				case ENGINEERING_UNIT.FMD_KgL3:
					return "kilograms / liter";
				case ENGINEERING_UNIT.FMD_LbIn3:
					return "pounds / cu. inch";
				case ENGINEERING_UNIT.FMD_LbFt3:
					return "pounds / cu. foot";
				case ENGINEERING_UNIT.FMD_USLbGal:
					return "lbs per gallon (U.S.)";
				case ENGINEERING_UNIT.FMD_ImpLbGl:
					return "lbs per gallon (U.K.)";
				case ENGINEERING_UNIT.FMD_LbBlOil:
					return "lbs per barrel (oil)";
				case ENGINEERING_UNIT.FMD_LbBlLiq:
					return "lbs per barrel (liq)";
				case ENGINEERING_UNIT.FMD_DegAPI:
					return "Degrees API";
				case ENGINEERING_UNIT.FMD_SpGrav:
					return "Specific Gravity";
				case ENGINEERING_UNIT.FMD_PrPlato:
					return "Percent Plato";
				case ENGINEERING_UNIT.FMD_DegBRIX:
					return "Degrees BRIX";
				case ENGINEERING_UNIT.FMD_DegBmLt:
					return "Degrees Baume (light)";
				case ENGINEERING_UNIT.FMD_DegBmHy:
					return "Degrees Baume (heavy)";
				case ENGINEERING_UNIT.FMD_STnYd3:
					return "short tons/cu. yard";
				case ENGINEERING_UNIT.FME_BTU:
					return "British Thermal Units";
				case ENGINEERING_UNIT.FME_Cal:
					return "calories";
				case ENGINEERING_UNIT.FME_Joule:
					return "Joules";
				case ENGINEERING_UNIT.FME_WH:
					return "Watt-Hours";
				case ENGINEERING_UNIT.FME_KwH:
					return "Kilowatt-Hours";
				case ENGINEERING_UNIT.FMPH_BTUSec:
					return "BTU's per second";
				case ENGINEERING_UNIT.FMPH_BTUMin:
					return "BTU's per minute";
				case ENGINEERING_UNIT.FMPH_BTUHr:
					return "BTU's per hour";
				case ENGINEERING_UNIT.FMPH_CalMin:
					return "calories per min";
				case ENGINEERING_UNIT.FMPH_Watt:
					return "Watts";
				case ENGINEERING_UNIT.FMPH_KWatts:
					return "Kilowatts";
				case ENGINEERING_UNIT.FMPH_KVAmp:
					return "Kilo Volt-Amperes";
				case ENGINEERING_UNIT.FMPH_HPower:
					return "Horsepower";
				case ENGINEERING_UNIT.FMEU_MVolts:
					return "millivolts";
				case ENGINEERING_UNIT.FMEU_Volt:
					return "Volts";
				case ENGINEERING_UNIT.FMEU_MAmps:
					return "milliamperes";
				case ENGINEERING_UNIT.FMEU_Amp:
					return "Amperes";
				case ENGINEERING_UNIT.FMEU_Ohm:
					return "ohms";
				case ENGINEERING_UNIT.FMEU_Farad:
					return "farads";
				case ENGINEERING_UNIT.FMEU_Coul:
					return "Coulombs";
				case ENGINEERING_UNIT.FMEU_Henry:
					return "Henrys";
				case ENGINEERING_UNIT.FMEU_MicSie:
					return "MicroSiemens";
				case ENGINEERING_UNIT.FMEU_Siemen:
					return "Siemens";
				case ENGINEERING_UNIT.FMEU_MHO:
					return "MHOs";
				case ENGINEERING_UNIT.FMDU_PwrFct:
					return "Power Factor";
				case ENGINEERING_UNIT.FMDU_RPM:
					return "Revolutions per min";
				case ENGINEERING_UNIT.FMDU_Hertz:
					return "Cycles per second";
				case ENGINEERING_UNIT.FMDU_PCent:
					return "Percent";
				case ENGINEERING_UNIT.FMDU_PPM:
					return "parts per million";
				case ENGINEERING_UNIT.FMDU_PHumid:
					return "% Humidity";
				case ENGINEERING_UNIT.FMDU_POxygn:
					return "% Oxygen";
				case ENGINEERING_UNIT.FMDU_RHumid:
					return "Relative Humidity";
				case ENGINEERING_UNIT.FMDU_PH:
					return "pH";
				case ENGINEERING_UNIT.FMMU_Centp:
					return "centipoise";
				case ENGINEERING_UNIT.FMMU_SolWt:
					return "% solids by weight";
				case ENGINEERING_UNIT.FMMU_SolVol:
					return "% solids by volume";
				case ENGINEERING_UNIT.FMMU_StQual:
					return "% steam quality";
				case ENGINEERING_UNIT.FMMU_Bushel:
					return "Bushels";
				case ENGINEERING_UNIT.FMMU_PrfVol:
					return "proof volume";
				case ENGINEERING_UNIT.FMMU_PrfMas:
					return "proof mass";
				case ENGINEERING_UNIT.FMMU_Ft3Lb:
					return "cu. ft. / pound";
				default:
					return "Undefined";
			}
		}

		public static string GetUnitAbbreviation(ENGINEERING_UNIT unit)
		{
			switch(unit)
			{
				case ENGINEERING_UNIT.FMT_DegC:
					return "°C";
				case ENGINEERING_UNIT.FMT_DegF:
					return "°F";
				case ENGINEERING_UNIT.FMT_DegK:
					return "Kelvin";
				case ENGINEERING_UNIT.FMT_DegR:
					return "°R";
				case ENGINEERING_UNIT.FMT_Msec:
					return "msec";
				case ENGINEERING_UNIT.FMT_Sec:
					return "sec";
				case ENGINEERING_UNIT.FMT_Min:
					return "min";
				case ENGINEERING_UNIT.FMT_Hour:
					return "hr";
				case ENGINEERING_UNIT.FMT_Day:
					return "days";
				case ENGINEERING_UNIT.FMT_Week:
					return "wks";
				case ENGINEERING_UNIT.FMT_Month:
					return "mon";
				case ENGINEERING_UNIT.FMT_Year:
					return "yrs";
				case ENGINEERING_UNIT.FML_FtIn8th:
					return "ft-in-8th";
				case ENGINEERING_UNIT.FML_MM:
					return "mm";
				case ENGINEERING_UNIT.FML_CM:
					return "cm";
				case ENGINEERING_UNIT.FML_Meter:
					return "m";
				case ENGINEERING_UNIT.FML_KM:
					return "km";
				case ENGINEERING_UNIT.FML_16th:
					return "16th";
				case ENGINEERING_UNIT.FML_Inch:
					return "in";
				case ENGINEERING_UNIT.FML_Feet:
					return "ft";
				case ENGINEERING_UNIT.FML_FtIn16th:
					return "ft-in-16th";
				case ENGINEERING_UNIT.FML_Yard:
					return "yd";
				case ENGINEERING_UNIT.FML_Mile:
					return "mi";
				case ENGINEERING_UNIT.FMA_MM2:
					return "mm²";
				case ENGINEERING_UNIT.FMA_CM2:
					return "cm²";
				case ENGINEERING_UNIT.FMA_Meter2:
					return "m²";
				case ENGINEERING_UNIT.FMA_KM2:
					return "km²";
				case ENGINEERING_UNIT.FMA_16TH2:
					return "16th²";
				case ENGINEERING_UNIT.FMA_Inch2:
					return "in²";
				case ENGINEERING_UNIT.FMA_Feet2:
					return "ft²";
				case ENGINEERING_UNIT.FMA_Yard2:
					return "yd²";
				case ENGINEERING_UNIT.FMA_Mile2:
					return "mi²";
				case ENGINEERING_UNIT.FMV_CM3:
					return "cc";
				case ENGINEERING_UNIT.FMV_Meter3:
					return "m³";
				case ENGINEERING_UNIT.FMV_Litre:
					return "l";
				case ENGINEERING_UNIT.FMV_Inch3:
					return "in³";
				case ENGINEERING_UNIT.FMV_Feet3:
					return "ft³";
				case ENGINEERING_UNIT.FMV_Yard3:
					return "yd³";
				case ENGINEERING_UNIT.FMV_USGal:
					return "gal (US)";
				case ENGINEERING_UNIT.FMV_ImpGal:
					return "gal (UK)";
				case ENGINEERING_UNIT.FMV_BlOil:
					return "bbl (Oil)";
				case ENGINEERING_UNIT.FMV_BlLiq:
					return "bbl (Liq)";
				case ENGINEERING_UNIT.FMV_KL:
					return "kl";
				case ENGINEERING_UNIT.FMV_MsFt3:
					return "MsFt3";
				case ENGINEERING_UNIT.FMM_Gram:
					return "g";
				case ENGINEERING_UNIT.FMM_KG:
					return "kg";
				case ENGINEERING_UNIT.FMM_MTon:
					return "ton (m)";
				case ENGINEERING_UNIT.FMM_Oz:
					return "oz";
				case ENGINEERING_UNIT.FMM_Lb:
					return "lb";
				case ENGINEERING_UNIT.FMM_ETon:
					return "ton (e)";
				case ENGINEERING_UNIT.FMM_STon:
					return "ton (s)";
				case ENGINEERING_UNIT.FMM_LTon:
					return "ton (l)";
				case ENGINEERING_UNIT.FMM_Mlbs:
					return "Mlbs";
				case ENGINEERING_UNIT.FMP_Pa:
					return "Pa";
				case ENGINEERING_UNIT.FMP_KPa:
					return "kPa";
				case ENGINEERING_UNIT.FMP_KgCm2:
					return "kg/cm²";
				case ENGINEERING_UNIT.FMP_Psi:
					return "PSI";
				case ENGINEERING_UNIT.FMP_PsiG:
					return "psig";
				case ENGINEERING_UNIT.FMP_PsiA:
					return "psia";
				case ENGINEERING_UNIT.FMP_InH2O:
					return "in H2O";
				case ENGINEERING_UNIT.FMP_FtH2O:
					return "ft H2O";
				case ENGINEERING_UNIT.FMP_InHg:
					return "in Hg";
				case ENGINEERING_UNIT.FMP_LbFt2:
					return "lb/ft²";
				case ENGINEERING_UNIT.FMP_Torr:
					return "torr";
				case ENGINEERING_UNIT.FMP_Bar:
					return "bar";
				case ENGINEERING_UNIT.FMP_MBar:
					return "mbar";
				case ENGINEERING_UNIT.FMP_MMHg:
					return "mm Hg";
				case ENGINEERING_UNIT.FMP_MMH2O:
					return "mm H2O";
				case ENGINEERING_UNIT.FMP_GmCm2:
					return "g/cm²";
				case ENGINEERING_UNIT.FMP_Atm:
					return "atm";
				case ENGINEERING_UNIT.FMVF_CCMin:
					return "cc/min";
				case ENGINEERING_UNIT.FMVF_CCHr:
					return "cc/hr";
				case ENGINEERING_UNIT.FMVF_M3Sec:
					return "m³/sec";
				case ENGINEERING_UNIT.FMVF_M3Min:
					return "m³/min";
				case ENGINEERING_UNIT.FMVF_M3Hr:
					return "m³/hr";
				case ENGINEERING_UNIT.FMVF_M3Day:
					return "m³/day";
				case ENGINEERING_UNIT.FMVF_LtSec:
					return "l/sec";
				case ENGINEERING_UNIT.FMVF_LtMin:
					return "l/min";
				case ENGINEERING_UNIT.FMVF_LtHr:
					return "l/hr";
				case ENGINEERING_UNIT.FMVF_MLPD:
					return "Ml/day";
				case ENGINEERING_UNIT.FMVF_In3Min:
					return "in³/min";
				case ENGINEERING_UNIT.FMVF_In3Hr:
					return "in³/hr";
				case ENGINEERING_UNIT.FMVF_Ft3Sec:
					return "ft³/sec";
				case ENGINEERING_UNIT.FMVF_Ft3Min:
					return "ft³/min";
				case ENGINEERING_UNIT.FMVF_Ft3Hr:
					return "ft³/hr";
				case ENGINEERING_UNIT.FMVF_Ft3Day:
					return "ft³/day";
				case ENGINEERING_UNIT.FMVF_Yd3Min:
					return "yd³/min";
				case ENGINEERING_UNIT.FMVF_Yd3Hr:
					return "yd³/hr";
				case ENGINEERING_UNIT.FMVF_GPS:
					return "gps (US)";
				case ENGINEERING_UNIT.FMVF_GPM:
					return "gpm (US)";
				case ENGINEERING_UNIT.FMVF_GPH:
					return "gph (US)";
				case ENGINEERING_UNIT.FMVF_MGPD:
					return "MGPD (US)";
				case ENGINEERING_UNIT.FMVF_ImpGPS:
					return "gps (UK)";
				case ENGINEERING_UNIT.FMVF_ImpGPM:
					return "gpm (UK)";
				case ENGINEERING_UNIT.FMVF_ImpGPH:
					return "gph (UK)";
				case ENGINEERING_UNIT.FMVF_ImpMGD:
					return "MGPD (UK)";
				case ENGINEERING_UNIT.FMVF_BPMoil:
					return "BPM (Oil)";
				case ENGINEERING_UNIT.FMVF_BPHoil:
					return "BPH (Oil)";
				case ENGINEERING_UNIT.FMVF_BPDoil:
					return "BPD (Oil)";
				case ENGINEERING_UNIT.FMVF_MBDoil:
					return "MBPD (Oil)";
				case ENGINEERING_UNIT.FMVF_BPMliq:
					return "BPM (Liq)";
				case ENGINEERING_UNIT.FMVF_BPHliq:
					return "BPH (Liq)";
				case ENGINEERING_UNIT.FMVF_BPDliq:
					return "BPD (Liq)";
				case ENGINEERING_UNIT.FMVF_MBDliq:
					return "MBPD (Liq)";
				case ENGINEERING_UNIT.FMVF_KLSec:
					return "kl/sec";
				case ENGINEERING_UNIT.FMVF_KLMin:
					return "kl/min";
				case ENGINEERING_UNIT.FMVF_KLHr:
					return "kl/hr";
				case ENGINEERING_UNIT.FMVF_KLDay:
					return "kl/day";
				case ENGINEERING_UNIT.FMMF_LbSec:
					return "lb/sec";
				case ENGINEERING_UNIT.FMMF_LbMin:
					return "lb/min";
				case ENGINEERING_UNIT.FMMF_LbHr:
					return "lb/hr";
				case ENGINEERING_UNIT.FMMF_LbDay:
					return "lb/day";
				case ENGINEERING_UNIT.FMMF_MTonMn:
					return "ton(m)/min";
				case ENGINEERING_UNIT.FMMF_MTonHr:
					return "ton(m)/hr";
				case ENGINEERING_UNIT.FMMF_MTonDy:
					return "ton(m)/day";
				case ENGINEERING_UNIT.FMMF_STonMn:
					return "ton(s)/min";
				case ENGINEERING_UNIT.FMMF_STonHr:
					return "ton(s)/hr";
				case ENGINEERING_UNIT.FMMF_STonDy:
					return "ton(s)/day";
				case ENGINEERING_UNIT.FMMF_LTonMn:
					return "ton(l)/min";
				case ENGINEERING_UNIT.FMMF_LTonHr:
					return "ton(l)/hr";
				case ENGINEERING_UNIT.FMMF_LTonDy:
					return "ton(l)/day";
				case ENGINEERING_UNIT.FMMF_GmSec:
					return "g/sec";
				case ENGINEERING_UNIT.FMMF_GmMin:
					return "g/min";
				case ENGINEERING_UNIT.FMMF_GmHr:
					return "g/hr";
				case ENGINEERING_UNIT.FMMF_KgSec:
					return "kg/sec";
				case ENGINEERING_UNIT.FMMF_KgMin:
					return "kg/min";
				case ENGINEERING_UNIT.FMMF_KgHr:
					return "kg/hr";
				case ENGINEERING_UNIT.FMMF_KgDay:
					return "kg/day";
				case ENGINEERING_UNIT.FMMF_MlbSec:
					return "Mlbs/sec";
				case ENGINEERING_UNIT.FMMF_MlbMin:
					return "Mlbs/min";
				case ENGINEERING_UNIT.FMMF_MlbHr:
					return "Mlbs/hr";
				case ENGINEERING_UNIT.FMMF_MlbDay:
					return "Mlbs/day";
				case ENGINEERING_UNIT.FMVR_IPS:
					return "in/sec";
				case ENGINEERING_UNIT.FMVR_FPS:
					return "ft/sec";
				case ENGINEERING_UNIT.FMVR_FPM:
					return "ft/min";
				case ENGINEERING_UNIT.FMVR_MMSec:
					return "mm/sec";
				case ENGINEERING_UNIT.FMVR_CMSec:
					return "cm/sec";
				case ENGINEERING_UNIT.FMVR_MSec:
					return "m/sec";
				case ENGINEERING_UNIT.FMVR_MMin:
					return "m/min";
				case ENGINEERING_UNIT.FMVR_MPH:
					return "MPH";
				case ENGINEERING_UNIT.FMVR_MrPH:
					return "m/hr";
				case ENGINEERING_UNIT.FMVR_KMPH:
					return "KPH";
				case ENGINEERING_UNIT.FMVR_KNOT:
					return "KNOT";
				case ENGINEERING_UNIT.FMVR_MMMin:
					return "mm/min";
				case ENGINEERING_UNIT.FMD_GCM3:
					return "g/cm³";
				case ENGINEERING_UNIT.FMD_GMl3:
					return "g/ml";
				case ENGINEERING_UNIT.FMD_GL3:
					return "g/l";
				case ENGINEERING_UNIT.FMD_KgM3:
					return "kg/m³";
				case ENGINEERING_UNIT.FMD_KgL3:
					return "kg/l";
				case ENGINEERING_UNIT.FMD_LbIn3:
					return "lb/in³";
				case ENGINEERING_UNIT.FMD_LbFt3:
					return "lb/ft³";
				case ENGINEERING_UNIT.FMD_USLbGal:
					return "lb/gal(US)";
				case ENGINEERING_UNIT.FMD_ImpLbGl:
					return "lb/gal(UK)";
				case ENGINEERING_UNIT.FMD_LbBlOil:
					return "lb/bbl(o)";
				case ENGINEERING_UNIT.FMD_LbBlLiq:
					return "lb/bbl(l)";
				case ENGINEERING_UNIT.FMD_DegAPI:
					return "°API";
				case ENGINEERING_UNIT.FMD_SpGrav:
					return "sp gr";
				case ENGINEERING_UNIT.FMD_PrPlato:
					return "% Plato";
				case ENGINEERING_UNIT.FMD_DegBRIX:
					return "°BRIX";
				case ENGINEERING_UNIT.FMD_DegBmLt:
					return "°Ba (l)";
				case ENGINEERING_UNIT.FMD_DegBmHy:
					return "°Ba (h)";
				case ENGINEERING_UNIT.FMD_STnYd3:
					return "ton(s)/yd³";
				case ENGINEERING_UNIT.FME_BTU:
					return "BTU";
				case ENGINEERING_UNIT.FME_Cal:
					return "cal";
				case ENGINEERING_UNIT.FME_Joule:
					return "J";
				case ENGINEERING_UNIT.FME_WH:
					return "WH";
				case ENGINEERING_UNIT.FME_KwH:
					return "kWH";
				case ENGINEERING_UNIT.FMPH_BTUSec:
					return "BTU/sec";
				case ENGINEERING_UNIT.FMPH_BTUMin:
					return "BTU/min";
				case ENGINEERING_UNIT.FMPH_BTUHr:
					return "BTU/hr";
				case ENGINEERING_UNIT.FMPH_CalMin:
					return "cal/min";
				case ENGINEERING_UNIT.FMPH_Watt:
					return "W";
				case ENGINEERING_UNIT.FMPH_KWatts:
					return "kW";
				case ENGINEERING_UNIT.FMPH_KVAmp:
					return "kVA";
				case ENGINEERING_UNIT.FMPH_HPower:
					return "hp";
				case ENGINEERING_UNIT.FMEU_MVolts:
					return "mV";
				case ENGINEERING_UNIT.FMEU_Volt:
					return "V";
				case ENGINEERING_UNIT.FMEU_MAmps:
					return "mA";
				case ENGINEERING_UNIT.FMEU_Amp:
					return "A";
				case ENGINEERING_UNIT.FMEU_Ohm:
					return "ohm";
				case ENGINEERING_UNIT.FMEU_Farad:
					return "F";
				case ENGINEERING_UNIT.FMEU_Coul:
					return "C";
				case ENGINEERING_UNIT.FMEU_Henry:
					return "H";
				case ENGINEERING_UNIT.FMEU_MicSie:
					return "µS";
				case ENGINEERING_UNIT.FMEU_Siemen:
					return "S";
				case ENGINEERING_UNIT.FMEU_MHO:
					return "mho";
				case ENGINEERING_UNIT.FMDU_PwrFct:
					return "P.F.";
				case ENGINEERING_UNIT.FMDU_RPM:
					return "RPM";
				case ENGINEERING_UNIT.FMDU_Hertz:
					return "Hz";
				case ENGINEERING_UNIT.FMDU_PCent:
					return "%";
				case ENGINEERING_UNIT.FMDU_PPM:
					return "PPM";
				case ENGINEERING_UNIT.FMDU_PHumid:
					return "%H";
				case ENGINEERING_UNIT.FMDU_POxygn:
					return "%O2";
				case ENGINEERING_UNIT.FMDU_RHumid:
					return "RH";
				case ENGINEERING_UNIT.FMDU_PH:
					return "pH";
				case ENGINEERING_UNIT.FMMU_Centp:
					return "centp";
				case ENGINEERING_UNIT.FMMU_SolWt:
					return "%sol-wt";
				case ENGINEERING_UNIT.FMMU_SolVol:
					return "%sol-vol";
				case ENGINEERING_UNIT.FMMU_StQual:
					return "%quality";
				case ENGINEERING_UNIT.FMMU_Bushel:
					return "bushel";
				case ENGINEERING_UNIT.FMMU_PrfVol:
					return "pr vol";
				case ENGINEERING_UNIT.FMMU_PrfMas:
					return "pr mass";
				case ENGINEERING_UNIT.FMMU_Ft3Lb:
					return "ft³/lb";
				default:
					return "Undefined";
			}
		}
	}
}
