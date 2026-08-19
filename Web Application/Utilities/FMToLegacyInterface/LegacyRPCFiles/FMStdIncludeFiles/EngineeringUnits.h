/******************************************************************************
FILE NAME:	EngineeringUnits.h

PURPOSE:    Define Engineering Units Constants.

COMMENTS:   

AUTHOR(S):  

VERSION:	1.0.0 19-Feb-99 Current Version Number

MODIFICATION HISTORY:
	Date:			By:			Reason:
	---------	----------	--------
	3-Feb-00		W.Gray		4.2.0.1 - Changed Density Units envolving Litres
									to remove cubed notation.  

Copyright (C) Varec, Inc. Norcross, GA, USA, 1999
This file shall not be copied or reproduced in any form without
the express written consent of Varec, Inc..
********************************************************************************/

///////////////////////////////////////////////////////////////////////////////
//  Define constants for Engineering Units
///////////////////////////////////////////////////////////////////////////////
#ifndef	__ENGINEERINGUNITS_H__
#define	__ENGINEERINGUNITS_H__

#define	FMU_Undefined	0			// No applicable units
// Temperature Units
#define	FMT_DegC 		1			// Degrees Celcius
#define	FMT_DegF 		2			// Degrees Farenheit
#define	FMT_DegK 		3			// Degrees Kelvin
#define	FMT_DegR 		4			// Degrees Rankine
// Time Units				
#define	FMT_Msec 		5			// MilliSeconds
#define	FMT_Sec			6			// Seconds
#define	FMT_Min			7			// Minutes
#define	FMT_Hour 		8			// Hours
#define	FMT_Day			9			// Days
#define	FMT_Week 		10 		// Weeks
#define	FMT_Month		11 		// Months
#define	FMT_Year 		12 		// Years
// ********* 13 - 18 spare
// Length Units
#define	FML_FtIn8th		19			// Feet/Inches/18ths
#define	FML_MM			20 		// Millimeters
#define	FML_CM			21 		// Centimeters
#define	FML_Meter		22 		// Meters
#define	FML_KM			23 		// Kilometers
#define	FML_16th 		24 		// 1/16 inch
#define	FML_Inch 		25 		// Inches
#define	FML_Feet 		26 		// Feet
#define	FML_FtIn16th	27 		// Feet/Inches/16ths
#define	FML_Yard 		28 		// Yards
#define	FML_Mile 		29 		// Miles
// Area units
#define	FMA_MM2			30 		// Millimeter Sq
#define	FMA_CM2			31 		// Centimeter Sq
#define	FMA_Meter2		32 		// Meter Sq
#define	FMA_KM2			33 		// Kilometer Sq
#define	FMA_16TH2		34 		// 1/16 inch Sq
#define	FMA_Inch2		35 		// Inches Sq
#define	FMA_Feet2		36 		// Feet Sq
#define	FMA_Yard2		37 		// Yard Sq
#define	FMA_Mile2		38 		// Mile Sq
// ********* 39 spare
// Volume units
#define	FMV_CM3			40 		// Cubic centimeters
#define	FMV_Meter3		41 		// Cubic meters
#define	FMV_Litre		42 		// Litres
#define	FMV_Inch3		43 		// Cubic inches
#define	FMV_Feet3		44 		// Cubic feet
#define	FMV_Yard3		45 		// Cubic yards
#define	FMV_USGal		46 		// US Gallons
#define	FMV_ImpGal		47 		// Imp Gallons
#define	FMV_BlOil		48 		// Barrel Oil
#define	FMV_BlLiq		49 		// Barrel Liquid
#define	FMV_KL			50			// Kilolitres
// ********* 51 - 59 spare
// Mass Units
#define	FMM_Gram 		60 		// Grams
#define	FMM_KG			61 		// Kilograms
#define	FMM_MTon 		62 		// Metric Ton
#define	FMM_Oz			63 		// Ounce
#define	FMM_Lb			64 		// Pound
#define	FMM_ETon 		65 		// English Ton
#define	FMM_STon 		66 		// Short Ton
#define	FMM_LTon 		67 		// Long Ton
#define	FMM_Mlbs			68			// Thousands of Pounds
// ********* 69 spare
// Pressure Units
#define	FMP_Pa			70 		// Pascal (SI)
#define	FMP_KPa			71 		// Kilopascal
#define	FMP_KgCm2		72 		// Kg per sq cm
#define	FMP_Psi			73 		// lb per sq inch
#define	FMP_PsiG 		74 		// PSI Gauge
#define	FMP_PsiA 		75 		// PSI Absolute
#define	FMP_InH2O		76 		// In. H2O (@ 68F)
#define	FMP_FtH2O		77 		// Ft. H2O (@ 68F)
#define	FMP_InHg 		78 		// In. Mercury (@ 0C)
#define	FMP_LbFt2		79 		// Pounds per square foot
#define	FMP_Torr 		80 		// Torr (@ 0C)
#define	FMP_Bar			81 		// Bar
#define	FMP_MBar 		82 		// Millibar
#define	FMP_MMHg 		83 		// mm Hg (@ 0C)
#define	FMP_MMH2O		84 		// mm H2O (@ 68F)
#define	FMP_GmCm2		85 		// Grams per square cm
#define	FMP_Atm			86 		// Atmosphere
// ********* 87 - 89 spare
// Volumetric Flow units
#define	FMVF_CCMin		90 		// CC/Min
#define	FMVF_CCHr		91 		// CC/Hour
#define	FMVF_M3Sec		92 		// m3/sec
#define	FMVF_M3Min		93 		// m3/Minute
#define	FMVF_M3Hr		94 		// m3/Hour
#define	FMVF_M3Day		95 		// m3/Day
#define	FMVF_LtSec		96 		// Litre/sec
#define	FMVF_LtMin		97 		// Litres/minute
#define	FMVF_LtHr		98 		// Litres/Hour
#define	FMVF_MLPD		99 		// Million litres/day
#define	FMVF_In3Min		100		// Cubic inches/minute
#define	FMVF_In3Hr		101		// Cubic inches/hour
#define	FMVF_Ft3Sec		102		// Cubic feet/second
#define	FMVF_Ft3Min		103		// Cubic feet/minute
#define	FMVF_Ft3Hr		104		// Cubic feet/hour
#define	FMVF_Ft3Day		105		// Cubic feet/day
#define	FMVF_Yd3Min		106		// Cubic yards/minute
#define	FMVF_Yd3Hr		107		// Cubic yards/hour
#define	FMVF_GPS 		108		// Gallons/sec (US)
#define	FMVF_GPM 		109		// Gallons/minute (US)
#define	FMVF_GPH 		110		// Gallons/hour	(US)
#define	FMVF_MGPD		111		// Millions of gallons/day (US)
#define	FMVF_ImpGPS		112		// Imp gallons/sec
#define	FMVF_ImpGPM		113		// Imp gallons/minute
#define	FMVF_ImpGPH		114		// Imp gallons/hour
#define	FMVF_ImpMGD		115		// Imp millions of gallons/day
#define	FMVF_BPMoil		116		// BBL/min (oil)
#define	FMVF_BPHoil		117		// BBL/hour (oil)
#define	FMVF_BPDoil		118		// BBL/day (oil)
#define	FMVF_MBDoil		119		// Millions barrels/day (oil)
#define	FMVF_BPMliq		120		// BBL/min (liq)
#define	FMVF_BPHliq		121		// BBL/hour (liq)
#define	FMVF_BPDliq		122		// BBL/day (liq)
#define	FMVF_MBDliq		123		// Millions barrels/day (liquid)
#define	FMVF_KLSec		124 		// kilolitres/sec
#define	FMVF_KLMin		125 		// kilolitres/Minute
#define	FMVF_KLHr		126 		// kilolitres/Hour
#define	FMVF_KLDay		127 		// kilolitres/Day
// ********* 128 - 129 spare
// Mass Flow Units
#define	FMMF_LbSec		130		// Pounds/sec
#define	FMMF_LbMin		131		// Pounds/minute
#define	FMMF_LbHr		132		// Pounds/hour
#define	FMMF_LbDay		133		// Pounds/day
#define	FMMF_MTonMn		134		// Metric tons/minute
#define	FMMF_MTonHr		135		// Metric tons/hour
#define	FMMF_MTonDy		136		// Metric tons/day
#define	FMMF_STonMn		137		// Short tons/min
#define	FMMF_STonHr		138		// Short tons/hour
#define	FMMF_STonDy		139		// Short tons/day
#define	FMMF_LTonMn		140		// Long tons/min
#define	FMMF_LTonHr		141		// Long tons/hour
#define	FMMF_LTonDy		142		// Long tons/day
#define	FMMF_GmSec		143		// Grams/sec
#define	FMMF_GmMin		144		// Grams/minute
#define	FMMF_GmHr		145		// Grams/hour
#define	FMMF_KgSec		146		// Kilograms/sec
#define	FMMF_KgMin		147		// Kilograms/minute
#define	FMMF_KgHr		148		// Kilograms/hour
#define	FMMF_KgDay		149		// Kilograms/day
#define	FMMF_MlbSec		150		// Thousands of Pounds/sec
#define	FMMF_MlbMin		151		// Thousands of Pounds/minute
#define	FMMF_MlbHr		152		// Thousands of Pounds/hour
#define	FMMF_MlbDay		153		// Thousands of Pounds/day
// ********* 154 - 159 spare
// Velocity & Rate units
#define	FMVR_IPS 		160		// Inch/sec
#define	FMVR_FPS 		161		// Feet/sec
#define	FMVR_FPM 		162		// Feet/min
#define	FMVR_MMSec		163		// Millimeters/sec
#define	FMVR_CMSec		164		// Centimeters/sec
#define	FMVR_MSec		165		// Meters/sec
#define	FMVR_MMin		166		// Meters/min
#define	FMVR_MPH 		167		// Miles per hour
#define	FMVR_MrPH		168		// Meters per hour
#define	FMVR_KMPH		169		// Kilometers per hour
#define	FMVR_KNOT		170		// Knots
#define	FMVR_MMMin		171		// Millimeters/min
// ********* 172 - 179 spare
// Density Units
#define	FMD_GCM3 		180		// Grams/cubic cm
#define	FMD_GML	 		181		// Grams/millilitre
#define	FMD_GL			182		// Grams/litre
#define	FMD_KgM3 		183		// Kilograms/cubic meter
#define 	FMD_KgL	  		184 		// Kilograms/litre
#define	FMD_LbIn3		185		// Pounds/cubic inch
#define	FMD_LbFt3		186		// Pounds/cubic feet
#define	FMD_USLbGal		187		// Pounds/gallon
#define	FMD_ImpLbGl		188		// Pounds/gallon (imperial)
#define	FMD_LbBlOil		189		// Pounds/barrel (oil)
#define	FMD_LbBlLiq		190		// Pounds/barrel (liquid)
#define	FMD_DegAPI		191		// Degrees API
#define	FMD_SpGrav		192		// Specific gravity
#define	FMD_PrPlato		193		// % Plato
#define	FMD_DegBRIX		194		// Degrees BRIX
#define	FMD_DegBmLt		195		// Degrees Baum (light)
#define	FMD_DegBmHy		196		// Degrees Baum (heavy)
#define	FMD_DegTwad		197		// Degrees Twaddell
#define	FMD_DegBal		198		// Degrees Balling
#define	FMD_STnYd3		199		// Short tons/cubic yard
// Energy
#define	FME_BTU			200		// BTU
#define	FME_Cal			201		// Calorie
#define	FME_Joule		202		// Joule (SI)
#define	FME_WH			203		// Watt-hour
#define	FME_KwH			204		// Kilowatt-hour
// ********* 205 - 209 spare
// Power & Heat Transfer Units
#define	FMPH_BTUSec		210		// BTU/sec
#define	FMPH_BTUMin		211		// BTU/min
#define	FMPH_BTUHr		212		// BTU/hour
#define	FMPH_CalMin		213		// Cal/min
#define	FMPH_Watt		214		// Watts
#define	FMPH_KWatts		215		// KiloWatts
#define	FMPH_KVAmp		216		// Kilo Volt-Amp
#define	FMPH_HPower		217		// Horsepower
// ********* 218, 219 spare
//Electrical Units
#define	FMEU_MVolts		220		// Millivolts
#define	FMEU_Volt		221		// Volts
#define	FMEU_MAmps		222		// Milliamps
#define	FMEU_Amp 		223		// Amps
#define	FMEU_Ohm 		224		// Ohms
#define	FMEU_Farad		225		// Farad
#define	FMEU_Coul		226		// Coulomb
#define	FMEU_Henry		227		// Henry
#define	FMEU_MicSie		228		// MicroSiemens
#define	FMEU_Siemen		229		// Siemens
#define	FMEU_MHO 		230		// MHO
// Dimensionless Units
#define	FMDU_PwrFct		231		// Power factor
#define	FMDU_RPM 		232		// Revolutions/min
#define	FMDU_Hertz		233		// Cycle/sec (Hz)
#define	FMDU_PCent		234		// Percent (general)
#define	FMDU_PPM 		235		// Parts per mill
#define	FMDU_PHumid		236		// % Humidity
#define	FMDU_POxygn		237		// % Oxygen
#define	FMDU_RHumid		238		// Relative Humidity
#define	FMDU_PH			239		// pH
// Miscellaneous units
#define	FMMU_Centp		240		// Centipoise
#define	FMMU_SolWt		241		// Solids by weight
#define	FMMU_SolVol		242		// Solids by volume
#define	FMMU_StQual		243		// Steam quality
#define	FMMU_Bushel		244		// Bushel
#define	FMMU_PrfVol		245		// Proof volume
#define	FMMU_PrfMas		246		// Proof mass
#define	FMMU_Ft3Lb		247		// Cubic feet/pound
// ********* 248 - 254 spare
#define	FMU_Source		255		// Source Units

// Engineering Table range constants
#define FMU_Start 		1		  	// Valid start of table
#define FMU_End	 		254	  	// Valid end of table

// Any conversions which require special processing have the
//  following constant specified as the conversion factor
#define	FMU_SPECPROC	-1.000000

// Define Constants for Engineering Units Types

#define		FMU_ALL			0			// All Units
#define		FMU_TEMP 		1			// Temperature Units -	Auto Convert
#define		FMU_TIME 		2			// Time Units			-	Auto Convert
#define		FMU_LENGTH		3			// Length Units		-	Auto Convert
#define		FMU_AREA 		4			// Area Units			-	Auto Convert
#define		FMU_VOLUME		5			// Volume Units		-	Auto Convert
#define		FMU_MASS 		6			// Mass/Weight Units -	Auto Convert
#define		FMU_PRESSURE	7			// Pressure Units 	-	Auto Convert
#define		FMU_VOLFLOW 	8			// Volumetric Flow	-	Auto Convert
#define		FMU_MASSFLOW	9			// Mass Flow			-	Auto Convert
#define		FMU_VELOCITY  10			// Velocity/Rate		-	Auto Convert
#define		FMU_DENSITY   11			// Density Units		-	Auto Convert
#define		FMU_ENERGY	  12			// Energy Units		-	Auto Convert
#define		FMU_POWER	  13			// Power/Heat XFR 	-	Auto Convert
#define		FMU_ELECT	  14			// Electrical			-	Strings Only
#define		FMU_NODIM	  15			// Dimensionless		-	Strings Only

#define		FMU_NONE 	  0x8000 	// Units Not Allowed

#define		FMU_NOPERCENT	0x4000	// if set do not allow percent as an option

/******************************************************************************
 *  Function Prototypes
 ******************************************************************************/


// Define header structure for Units Conversion data array
typedef	struct
{
	int		iUnitType;		  	// Type the unit belongs to
	double	ScaleToSI;		  	// Unit's scaling factor

} ENGRUNIT;

#endif