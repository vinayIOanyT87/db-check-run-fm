//Initialize Object Creation
var FMConvertEngUnits = FMConvertEngUnits || {
	version: '1.1.0',
	unitsArray: [],
	unitsTypeArray: ['FmuAll', 'FmuTemp', 'FmuTime', 'FmuLength', 'FmuArea', 'FmuVolume', 'FmuMass', 'FmuPressure', 'FmuVolflow',
							 'FmuMassflow', 'FmuVelocity', 'FmuDensity', 'FmEnergy', 'FmuPower', 'FmuElect', 'FmuNodim', 'FmuNone'],
	FM_Brixb0: math.bignumber( -608.09478 ),
	FM_Brixb1: math.bignumber( 1.0987222 ),
	FM_Brixb2: math.bignumber( -0.00062906325 ),
	FM_Brixb3: math.bignumber( 1.388771e-7 ),
	FM_Brixs0: math.bignumber( 998.17453 ),
	FM_Brixs1: math.bignumber( 3.9124628 ),
	FM_Brixs2: math.bignumber( 0.0096624811 ),
	FM_Brixs3: math.bignumber( 0.00012518984 ),
	FM_Brixs4: math.bignumber( -6.2021547e-7 )
};

if ( typeof exports !== 'undefined' )
{
	exports.FMConvertEngUnits = FMConvertEngUnits;
}

if ( typeof document !== 'undefined' && typeof window !== 'undefined' )
{
	FMConvertEngUnits.document = document;
	FMConvertEngUnits.window = window;
	// ensure globality even if entire library were function wrapped (as in Meteor.js packaging system)
	window.FMConvertEngUnits = FMConvertEngUnits;
}
else
{
	// assume we're running under node.js when document/window are not present
	FMConvertEngUnits.document = require( 'jsdom' )
		 .jsdom( '<!DOCTYPE html><html><head></head><body></body></html>' );

	if ( FMConvertEngUnits.document.createWindow )
	{
		 FMConvertEngUnits.window = FMConvertEngUnits.document.createWindow();
	}
	else
	{
		 FMConvertEngUnits.window = FMConvertEngUnits.document.parentWindow;
	}
}


FMConvertEngUnits.WaterDensity = function( temperature )
{
	temperature = temperature || (60-32)/1.8;

	return math.bignumber(((((((((((-280.5425E-12 * temperature) + 105.56302E-9) *
					temperature ) - 46.170461E-6 ) * temperature ) -
				7.9870401E-3 ) * temperature ) + 16.945176 ) *
		 temperature ) + 999.83952 ) / ( 1.0 + 16.879850E-3 *
		 temperature ) );
}; //Define Functions
FMConvertEngUnits.ConvertToSIUnits =
	function( value, units, tempReferenceValue )
	{
		 var returnValue = math.bignumber( value );
		 if ( isNaN( value ) || isNaN( units ) )
		 {
				return math.bignumber( 0.0 );
		 }

		 switch ( units )
		 {
				//Temperature
				// Centigrade (SI)
				case 1:
					returnValue = value;
					break;
				// Fahrenheit
				case 2:
					returnValue = returnValue.sub( math.bignumber( 32 ) );
					returnValue = returnValue.div( math.bignumber( 1.8 ) );
					break;
				// Kelvin
				case 3:
					returnValue = returnValue.sub( 273.15 );
					break;
				// Rankine
				case 4:
					returnValue = returnValue.sub( math.bignumber( 491.67 ) );
					returnValue = returnValue.times( math.bignumber( 5 / 9 ) );
					break;

				// Length
				// Ft/In/8th
				case 19:
					returnValue = returnValue.times( 3.048000E-01 );
					break;
				// Millimeters
				case 20:
					returnValue = returnValue.times( 1.00000E-03 );
					break;
				// Centimeters
				case 21:
					returnValue = returnValue.times( 1.00000E-02 );
					break;
				// Meters (SI)
				case 22:
					returnValue = value;
					break;
				// Killometers
				case 23:
					returnValue = returnValue.times( 1.000000E+03 );
					break;
				// 16th of inch
				case 24:
					returnValue = returnValue.times( 1.587500E-03 );
					break;
				// Inches
				case 25:
					returnValue = returnValue.times( 2.540000E-02 );
					break;
				// Feet
				case 26:
					returnValue = returnValue.times( 3.048000E-01 );
					break;
				// ft/in/16th
				case 27:
					returnValue = returnValue.times( 3.048000E-01 );
					break;
				// yard
				case 28:
					returnValue = returnValue.times( 9.144000E-01 );
					break;
				// mile
				case 29:
					returnValue = returnValue.times( 1.609347E+03 );
					break;

				// Area
				// sq mm
				case 30:
					returnValue = returnValue.times( 1.000000E-06 );
					break;
				// sq cm
				case 31:
					returnValue = returnValue.times( 1.000000E-04 );
					break;
				// meter (SI)
				case 32:
					returnValue = value;
					break;
				// sq kilometer
				case 33:
					returnValue = returnValue.times( 1.000000E+06 );
					break;
				// sq 16th
				case 34:
					returnValue = returnValue.times( 2.520156E-6 );
					break;
				// sq inch
				case 35:
					returnValue = returnValue.times( 6.451600E-04 );
					break;
				// sq feet
				case 36:
					returnValue = returnValue.times( 9.290304E-02 );
					break;
				// sq yard
				case 37:
					returnValue = returnValue.times( 8.361274E-01 );
					break;
				// sq mile
				case 38:
					returnValue = returnValue.times( 2.589998E+06 );
					break;

				// Volume
				// cu. cm
				case 40:
					returnValue = returnValue.times( 1.000000E-06 );
					break;
				// Cu. Meter (SI)
				case 41:
					returnValue = value;
					break;
				// Litre
				case 42:
					returnValue = returnValue.times( 1.000000E-03 );
					break;
				// Cubic Inch
				case 43:
					returnValue = returnValue.times( 1.638706E-05 );
					break;
				// Cubic Feet
				case 44:
					returnValue = returnValue.times( 2.831685E-02 );
					break;
				// Cubic Yard
				case 45:
					returnValue = returnValue.times( 7.645549E-01 );
					break;
				// US Gallon
				case 46:
					returnValue = returnValue.times( 3.785412E-03 );
					break;
				// Imperial Gallon
				case 47:
					returnValue = returnValue.times( 4.546092E-03 );
					break;
				// Barrel Oil
				case 48:
					returnValue = returnValue.times( 1.589873E-01 );
					break;
				// Barrel Liquid
				case 49:
					returnValue = returnValue.times( 1.192401E-01 );
					break;
				// Kiloleter 
				case 50:
					returnValue = value;
					break;
				// 1000 standard cubic feet 			
				case 51:
					returnValue = returnValue.times( 2.6853E+01 );
					break;

				// Mass
				// grams
				case 60:
					returnValue = returnValue.times( 1.000000E-03 );
					break;
				// Kilogram (SI)
				case 61:
					returnValue = value;
					break;
				// Metric Ton
				case 62:
					returnValue = returnValue.times( 1.000000E+03 );
					break;
				// Ounce
				case 63:
					returnValue = returnValue.times( 2.834952E-02 );
					break;
				// Pound
				case 64:
					returnValue = returnValue.times( 4.535924E-01 );
					break;
				// English Ton
				case 65:
					returnValue = returnValue.times( 1.000000E+03 );
					break;
				// Short Ton (2000 lbs)
				case 66:
					returnValue = returnValue.times( 9.071847E+02 );
					break;
				// Long Ton
				case 67:
					returnValue = returnValue.times( 1.016047E+03 );
					break;
				// Mlbs. (Thousands of Pounds)
				case 68:
					returnValue = returnValue.times( 4.535924E+02 );
					break;

				// Pressure Units
				// Pascal (SI)
				case 70:
					returnValue = value;
					break;
				// KiloPascal
				case 71:
					returnValue = returnValue.times( 1.000000E+03 );
					break;
				// kg/ sq cm
				case 72:
					returnValue = returnValue.times( 9.806650E+04 );
					break;
				// lb/sq inch
				case 73:
					returnValue = returnValue.times( 6.894757E+03 );
					break;
				// PSI Gauge
				case 74:
					returnValue = returnValue.times( 6.894757E+03 );
					break;
				// PSI Absolute
				case 75:
					returnValue = returnValue.times( 6.894757E+03 );
					break;
				// in. H20 (39.2 F)
				case 76:
					returnValue = returnValue.times( 2.490820E+02 );
					break;
				// Ft H20 (39.2 F)
				case 77:
					returnValue = returnValue.times( 2.988980E+03 );
					break;
				// In. Mercury(@ 0C)
				case 78:
					returnValue = returnValue.times( 3.386380E+03 );
					break;
				// lb/sq ft
				case 79:
					returnValue = returnValue.times( 4.788026E+01 );
					break;
				// Torr (@ 0C)
				case 80:
					returnValue = returnValue.times( 1.333220E+02 );
					break;
				// Bar
				case 81:
					returnValue = returnValue.times( 1.000000E+05 );
					break;
				// MilliBar
				case 82:
					returnValue = returnValue.times( 1.000000E+02 );
					break;
				// mm HG (@ 0C)
				case 83:
					returnValue = returnValue.times( 1.333220E+02 );
					break;
				// mm H2O (@ 4C)
				case 84:
					returnValue = returnValue.times( 9.80638E+00 );
					break;
				// gr/ sq cm
				case 85:
					returnValue = returnValue.times( 9.806650E+01 );
					break;
				// Atmosphere
				case 86:
					returnValue = returnValue.times( 1.013250E+05 );
					break;

				// Volumetric Flow
				// CC/min
				case 90:
					returnValue = returnValue.times( 1.666667E-08 );
					break;
				// CC/hour
				case 91:
					returnValue = returnValue.times( 2.777778E-10 );
					break;
				// m3/sec (SI)
				case 92:
					returnValue = value;
					break;
				// m3/min
				case 93:
					returnValue = returnValue.times( 1.666667E-02 );
					break;
				// m3/hr
				case 94:
					returnValue = returnValue.times( 2.777778E-04 );
					break;
				// m3/day 
				case 95:
					returnValue = returnValue.times( 1.157408E-05 );
					break;
				// lit/sec
				case 96:
					returnValue = returnValue.times( 1.000000E-03 );
					break;
				// lit/min
				case 97:
					returnValue = returnValue.times( 1.666667E-05 );
					break;
				// lit/hour
				case 98:
					returnValue = returnValue.times( 2.777778E-07 );
					break;
				// million lit/day
				case 99:
					returnValue = returnValue.times( 1.157407E-02 );
					break;
				// in3/min
				case 100:
					returnValue = returnValue.times( 2.731177E-07 );
					break;
				// in3/hour
				case 101:
					returnValue = returnValue.times( 4.55195E-09 );
					break;
				// ft3/sec
				case 102:
					returnValue = returnValue.times( 2.831685E-02 );
					break;
				// ft3/min
				case 103:
					returnValue = returnValue.times( 4.719475E-04 );
					break;
				// ft3/hour
				case 104:
					returnValue = returnValue.times( 7.865792E-06 );
					break;
				// ft3/day
				case 105:
					returnValue = returnValue.times( 3.277413E-07 );
					break;
				// yd3/min
				case 106:
					returnValue = returnValue.times( 1.274258E-02 );
					break;
				// yd3/hour
				case 107:
					returnValue = returnValue.times( 2.123763E-04 );
					break;
				// Gal/sec(US)
				case 108:
					returnValue = returnValue.times( 3.785412E-03 );
					break;
				// Gal/min(US)
				case 109:
					returnValue = returnValue.times( 6.309020E-05 );
					break;
				// Gal/Hour(US)
				case 110:
					returnValue = returnValue.times( 1.051503E-06 );
					break;
				// Million Gal/Day(US)
				case 111:
					returnValue = returnValue.times( 4.381264E-02 );
					break;
				// Gal/Sec(IMP)
				case 112:
					returnValue = returnValue.times( 4.546092E-03 );
					break;
				// Gal/Min(IMP)
				case 113:
					returnValue = returnValue.times( 7.576820E-05 );
					break;
				// Gal/Hour(IMP)
				case 114:
					returnValue = returnValue.times( 1.262803E-06 );
					break;
				// Million Gal/Day(IMP)
				case 115:
					returnValue = returnValue.times( 5.261680E-02 );
					break;
				// BBL/min(OIL)
				case 116:
					returnValue = returnValue.times( 2.649788E-03 );
					break;
				// BBL/hour(Oil)
				case 117:
					returnValue = returnValue.times( 4.416314E-05 );
					break;
				// BBL/day(Oil)
				case 118:
					returnValue = returnValue.times( 1.840131E-06 );
					break;
				// MBPD
				case 119:
					returnValue = returnValue.times( 1.840131 );
					break;
				// bar/Min(liq)
				case 120:
					returnValue = returnValue.times( 1.987335E-03 );
					break;
				// bar/hr(Liq)
				case 121:
					returnValue = returnValue.times( 3.312225E-05 );
					break;
				// bar/day(Liq)
				case 122:
					returnValue = returnValue.times( 1.380094E-06 );
					break;
				// MBPD(Liq)
				case 123:
					returnValue = returnValue.times( 1.380094 );
					break;
				// kl/sec
				case 124:
					returnValue = value;
					break;
				// kl/min
				case 125:
					returnValue = returnValue.times( 1.666667E-02 );
					break;
				// kl/hour
				case 126:
					returnValue = returnValue.times( 2.777778E-04 );
					break;
				// kl/day
				case 127:
					returnValue = returnValue.times( 1.157408E-05 );
					break;

				// Mass Flow
				// lb per sec
				case 130:
					returnValue = returnValue.times( 4.535924E-01 );
					break;
				// lb per min
				case 131:
					returnValue = returnValue.times( 7.559873E-03 );
					break;
				// lb per hour
				case 132:
					returnValue = returnValue.times( 1.259979E-04 );
					break;
				// lb per day
				case 133:
					returnValue = returnValue.times( 5.249912E-06 );
					break;
				// metric tons per min
				case 134:
					returnValue = returnValue.times( 1.666667E+01 );
					break;
				// metric tons per hour
				case 135:
					returnValue = returnValue.times( 2.777778E-01 );
					break;
				// metric tons per day
				case 136:
					returnValue = returnValue.times( 1.157407E-02 );
					break;
				// short tons per min
				case 137:
					returnValue = returnValue.times( 1.511975E+01 );
					break;
				// short tons per hour
				case 138:
					returnValue = returnValue.times( 2.519958E-01 );
					break;
				// short tons per day
				case 139:
					returnValue = returnValue.times( 1.049983E-02 );
					break;
				// long tons per min
				case 140:
					returnValue = returnValue.times( 1.693412E+01 );
					break;
				// long tons per hour
				case 141:
					returnValue = returnValue.times( 2.822353E-01 );
					break;
				// long tons per day
				case 142:
					returnValue = returnValue.times( 1.175980E-02 );
					break;
				// g/sec 
				case 143:
					returnValue = returnValue.times( 1.000000E-03 );
					break;
				// g/min	 
				case 144:
					returnValue = returnValue.times( 1.666667E-05 );
					break;
				// g/hr
				case 145:
					returnValue = returnValue.times( 2.777778E-07 );
					break;
				// Kg/Sec (SI)
				case 146:
					returnValue = value;
					break;
				// Kg/min
				case 147:
					returnValue = returnValue.times( 1.666667E-02 );
					break;
				// Kg/hr
				case 148:
					returnValue = returnValue.times( 2.777778E-04 );
					break;
				// Kg/day
				case 149:
					returnValue = returnValue.times( 1.157407E-05 );
					break;
				// Mlbs/Sec
				case 150:
					returnValue = returnValue.times( 4.535924E+02 );
					break;
				// Mlbs/Minute
				case 151:
					returnValue = returnValue.times( 7.559873E+00 );
					break;
				// Mlbs/Hour
				case 152:
					returnValue = returnValue.times( 1.259979E-01 );
					break;
				// Mlbs/Day
				case 153:
					returnValue = returnValue.times( 5.249912E-03 );
					break;

				// Velocity & Rate Units
				// in per sec
				case 160:
					returnValue = returnValue.times( 2.540000E-02 );
					break;
				// ft per sec
				case 161:
					returnValue = returnValue.times( 3.048000E+01 );
					break;
				// ft per min
				case 162:
					returnValue = returnValue.times( 5.080000E-03 );
					break;
				// mm per sec
				case 163:
					returnValue = returnValue.times( 1.000000E-03 );
					break;
				// cm per sec
				case 164:
					returnValue = returnValue.times( 1.000000E-02 );
					break;
				// meter per sec (SI)
				case 165:
					returnValue = value;
					break;
				// meter per min
				case 166:
					returnValue = returnValue.times( 1.666667E-02 );
					break;
				// miles per hour
				case 167:
					returnValue = returnValue.times( 4.470400E-01 );
					break;
				// meter/hour
				case 168:
					returnValue = returnValue.times( 2.777778E-04 );
					break;
				// kilometer/hour
				case 169:
					returnValue = returnValue.times( 2.777778E-01 );
					break;
				// knots
				case 170:
					returnValue = returnValue.times( 5.144444E-01 );
					break;
				// mm per min
				case 171:
					returnValue = returnValue.times( 1.666667E-05 );
					break;

				// Density
				// Grams per Cubic Centimeter
				case 180:
					returnValue = returnValue.times( 1.000000E+03 );
					break;
				// Grams per Milliletre
				case 181:
					returnValue = returnValue.times( 1.000000E+03 );
					break;
				// gram/liter
				case 182:
					returnValue = value;
					break;
				// kilogram/cu meter (SI)
				case 183:
					returnValue = value;
					break;
				// Killogram per Litre
				case 184:
					returnValue = returnValue.times( 1.000000E+03 );
					break;
				// Pound per Cubic  Inch
				case 185:
					returnValue = returnValue.times( 2.767990E+4 );
					break;
				// Pound per Cubic Foot
				case 186:
					returnValue = returnValue.times( 1.601846E+1 );
					break;
				// Pound per Gallon
				case 187:
					returnValue = returnValue.times( 1.19829363E+2 );
					break;
				// Pound per Imperial Gallon
				case 188:
					returnValue = returnValue.times( 9.977633E+1 );
					break;
				// Pound per Barrel Oil
				case 189:
					returnValue = returnValue.times( 2.853010 );
					break;
				// Pound per Barrel Liquid
				case 190:
					returnValue = returnValue.times( 3.804026 );
					break;
				// Degrees API
				case 191:
					returnValue = math.divide( math.bignumber( 141.5 * 999.012 ), ( returnValue.add( 131.5 ) ) );
					break;
				//Specific Gravity
		 	case 192:
					returnValue = returnValue.mul( FMConvertEngUnits.WaterDensity( tempReferenceValue ) );
					break;
				//%Plato
				case 193:
					returnValue = math.subtract( math.divide( math.bignumber( 141360.48 ), returnValue ), 131.5 ); 
					break;
				//Degrees Brix	
		 	case 194:
					var term1 = math.add( returnValue.mul( this.FM_Brixs4 ), this.FM_Brixs3 );
					var term2 = math.add( term1.mul( returnValue ), this.FM_Brixs2 );
					var term3 = math.add( term2.mul( returnValue ), this.FM_Brixs1 );
					var term4 = math.add( term3.mul( returnValue ), this.FM_Brixs0 );
					returnValue = term4;
					break;
				// Degrees Baume (light)
				case 195:
					returnValue = math.divide( math.multiply( 140.0, this.WaterDensity( tempReferenceValue ) ), returnValue.add( 130.0 ) );
					break;
				// Degrees Baume (heavy)
				case 196:
					returnValue = math.divide( math.multiply( 145.0, this.WaterDensity( tempReferenceValue ) ), math.bignumber( 145.0 ).sub( returnValue ) );
					break;
				// short ton/cu yard
				case 199:
					returnValue = returnValue.times( 1.186553E+03 );
					break;
				default:
					returnValue = value;
					break;
		 }

		 return returnValue;
	};
FMConvertEngUnits.ConvertFromSIUnits =
	function( value, units, tempReferenceValue, roundfactor )
	{
		 var returnValue = math.bignumber( value );
		 if ( isNaN( value ) || isNaN( units ) )
		 {
				return math.bignumber( 0 );
		 }
		 var performRound = !isNaN( roundfactor );
		 switch ( units )
		 {
				//Temperature
				// Centigrade (SI)
				case 1:
					returnValue = value;
					break;
				// Fahrenheit
				case 2:
					returnValue = returnValue.times( math.bignumber( 1.8 ) );
					returnValue = returnValue.add( math.bignumber( 32 ) );
					break;
				// Kelvin
				case 3:
					returnValue = returnValue.add( 273.15 );
					break;
				// Rankine
				case 4:
					returnValue = returnValue.add( 273.15 );
					returnValue = returnValue.mul( 9 / 5 );
					break;

				// Length
				// Ft/In/8th
				case 19:
					returnValue = returnValue.div( 3.048000E-01 );
					break;
				// Millimeters
				case 20:
					returnValue = returnValue.div( 1.00000E-03 );
					break;
				// Centimeters
				case 21:
					returnValue = returnValue.div( 1.00000E-02 );
					break;
				// Meters (SI)
				case 22:
					returnValue = value;
					break;
				// Kilometers
				case 23:
					returnValue = returnValue.div( 1.000000E+03 );
					break;
				// 16th of inch
				case 24:
					returnValue = returnValue.div( 1.587500E-03 );
					break;
				// Inches
				case 25:
					returnValue = returnValue.div( 2.540000E-02 );
					break;
				// Feet
				case 26:
					returnValue = returnValue.div( 3.048000E-01 );
					break;
				// ft/in/16th
				case 27:
					returnValue = returnValue.div( 3.048000E-01 );
					break;
				// yard
				case 28:
					returnValue = returnValue.div( 9.144000E-01 );
					break;
				// mile
				case 29:
					returnValue = returnValue.div( 1.609347E+03 );
					break;

				// Area
				// sq mm
				case 30:
					returnValue = returnValue.div( 1.000000E-06 );
					break;
				// sq cm
				case 31:
					returnValue = returnValue.div( 1.000000E-04 );
					break;
				// meter (SI)
				case 32:
					returnValue = value;
					break;
				// sq kilometer
				case 33:
					returnValue = returnValue.div( 1.000000E+06 );
					break;
				// sq 16th
				case 34:
					returnValue = returnValue.div( 2.520156E-6 );
					break;
				// sq inch
				case 35:
					returnValue = returnValue.div( 6.451600E-04 );
					break;
				// sq feet
				case 36:
					returnValue = returnValue.div( 9.290304E-02 );
					break;
				// sq yard
				case 37:
					returnValue = returnValue.div( 8.361274E-01 );
					break;
				// sq mile
				case 38:
					returnValue = returnValue.div( 2.589998E+06 );
					break;

				// Volume
				// cu. cm
				case 40:
					returnValue = returnValue.div( 1.000000E-06 );
					break;
				// Cu. Meter (SI)
				case 41:
					returnValue = value;
					break;
				// Litre
				case 42:
					returnValue = returnValue.div( 1.000000E-03 );
					break;
				// Cubic Inch
				case 43:
					returnValue = returnValue.div( 1.638706E-05 );
					break;
				// Cubic Feet
				case 44:
					returnValue = returnValue.div( 2.831685E-02 );
					break;
				// Cubic Yard
				case 45:
					returnValue = returnValue.div( 7.645549E-01 );
					break;
				// US Gallon
				case 46:
					returnValue = returnValue.div( 3.785412E-03 );
					break;
				// Imperial Gallon
				case 47:
					returnValue = returnValue.div( 4.546092E-03 );
					break;
				// Barrel Oil
				case 48:
					returnValue = returnValue.div( 1.589873E-01 );
					break;
				// Barrel Liquid
				case 49:
					returnValue = returnValue.div( 1.192401E-01 );
					break;
				// Kiloleter 
				case 50:
					returnValue = value;
					break;
				// 1000 standard cubic feet 			
				case 51:
					returnValue = returnValue.div( 2.6853E+01 );
					break;

				// Mass
				// grams
				case 60:
					returnValue = returnValue.div( 1.000000E-03 );
					break;
				// Kilogram (SI)
				case 61:
					returnValue = value;
					break;
				// Metric Ton
				case 62:
					returnValue = returnValue.div( 1.000000E+03 );
					break;
				// Ounce
				case 63:
					returnValue = returnValue.div( 2.834952E-02 );
					break;
				// Pound
				case 64:
					returnValue = returnValue.div( 4.535924E-01 );
					break;
				// English Ton
				case 65:
					returnValue = returnValue.div( 1.000000E+03 );
					break;
				// Short Ton (2000 lbs)
				case 66:
					returnValue = returnValue.div( 9.071847E+02 );
					break;
				// Long Ton
				case 67:
					returnValue = returnValue.div( 1.016047E+03 );
					break;
				// Mlbs. (Thousands of Pounds)
				case 68:
					returnValue = returnValue.div( 4.535924E+02 );
					break;

				// Pressure Units
				// Pascal (SI)
				case 70:
					returnValue = value;
					break;
				// KiloPascal
				case 71:
					returnValue = returnValue.div( 1.000000E+03 );
					break;
				// kg/ sq cm
				case 72:
					returnValue = returnValue.div( 9.806650E+04 );
					break;
				// lb/sq inch
				case 73:
					returnValue = returnValue.div( 6.894757E+03 );
					break;
				// PSI Gauge
				case 74:
					returnValue = returnValue.div( 6.894757E+03 );
					break;
				// PSI Absolute
				case 75:
					returnValue = returnValue.div( 6.894757E+03 );
					break;
				// in. H20 (39.2 F)
				case 76:
					returnValue = returnValue.div( 2.490820E+02 );
					break;
				// Ft H20 (39.2 F)
				case 77:
					returnValue = returnValue.div( 2.988980E+03 );
					break;
				// In. Mercury(@ 0C)
				case 78:
					returnValue = returnValue.div( 3.386380E+03 );
					break;
				// lb/sq ft
				case 79:
					returnValue = returnValue.div( 4.788026E+01 );
					break;
				// Torr (@ 0C)
				case 80:
					returnValue = returnValue.div( 1.333220E+02 );
					break;
				// Bar
				case 81:
					returnValue = returnValue.div( 1.000000E+05 );
					break;
				// MilliBar
				case 82:
					returnValue = returnValue.div( 1.000000E+02 );
					break;
				// mm HG (@ 0C)
				case 83:
					returnValue = returnValue.div( 1.333220E+02 );
					break;
				// mm H2O (@ 4C)
				case 84:
					returnValue = returnValue.div( 9.80638E+00 );
					break;
				// gr/ sq cm
				case 85:
					returnValue = returnValue.div( 9.806650E+01 );
					break;
				// Atmosphere
				case 86:
					returnValue = returnValue.div( 1.013250E+05 );
					break;

				// Volumetric Flow
				// CC/min
				case 90:
					returnValue = returnValue.div( 1.666667E-08 );
					break;
				// CC/hour
				case 91:
					returnValue = returnValue.div( 2.777778E-10 );
					break;
				// m3/sec (SI)
				case 92:
					returnValue = value;
					break;
				// m3/min
				case 93:
					returnValue = returnValue.div( 1.666667E-02 );
					break;
				// m3/hr
				case 94:
					returnValue = returnValue.div( 2.777778E-04 );
					break;
				// m3/day 
				case 95:
					returnValue = returnValue.div( 1.157408E-05 );
					break;
				// lit/sec
				case 96:
					returnValue = returnValue.div( 1.000000E-03 );
					break;
				// lit/min
				case 97:
					returnValue = returnValue.div( 1.666667E-05 );
					break;
				// lit/hour
				case 98:
					returnValue = returnValue.div( 2.777778E-07 );
					break;
				// million lit/day
				case 99:
					returnValue = returnValue.div( 1.157407E-02 );
					break;
				// in3/min
				case 100:
					returnValue = returnValue.div( 2.731177E-07 );
					break;
				// in3/hour
				case 101:
					returnValue = returnValue.div( 4.55195E-09 );
					break;
				// ft3/sec
				case 102:
					returnValue = returnValue.div( 2.831685E-02 );
					break;
				// ft3/min
				case 103:
					returnValue = returnValue.div( 4.719475E-04 );
					break;
				// ft3/hour
				case 104:
					returnValue = returnValue.div( 7.865792E-06 );
					break;
				// ft3/day
				case 105:
					returnValue = returnValue.div( 3.277413E-07 );
					break;
				// yd3/min
				case 106:
					returnValue = returnValue.div( 1.274258E-02 );
					break;
				// yd3/hour
				case 107:
					returnValue = returnValue.div( 2.123763E-04 );
					break;
				// Gal/sec(US)
				case 108:
					returnValue = returnValue.div( 3.785412E-03 );
					break;
				// Gal/min(US)
				case 109:
					returnValue = returnValue.div( 6.309020E-05 );
					break;
				// Gal/Hour(US)
				case 110:
					returnValue = returnValue.div( 1.051503E-06 );
					break;
				// Million Gal/Day(US)
				case 111:
					returnValue = returnValue.div( 4.381264E-02 );
					break;
				// Gal/Sec(IMP)
				case 112:
					returnValue = returnValue.div( 4.546092E-03 );
					break;
				// Gal/Min(IMP)
				case 113:
					returnValue = returnValue.div( 7.576820E-05 );
					break;
				// Gal/Hour(IMP)
				case 114:
					returnValue = returnValue.div( 1.262803E-06 );
					break;
				// Million Gal/Day(IMP)
				case 115:
					returnValue = returnValue.div( 5.261680E-02 );
					break;
				// BBL/min(OIL)
				case 116:
					returnValue = returnValue.div( 2.649788E-03 );
					break;
				// BBL/hour(Oil)
				case 117:
					returnValue = returnValue.div( 4.416314E-05 );
					break;
				// BBL/day(Oil)
				case 118:
					returnValue = returnValue.div( 1.840131E-06 );
					break;
				// MBPD
				case 119:
					returnValue = returnValue.div( 1.840131 );
					break;
				// bar/Min(liq)
				case 120:
					returnValue = returnValue.div( 1.987335E-03 );
					break;
				// bar/hr(Liq)
				case 121:
					returnValue = returnValue.div( 3.312225E-05 );
					break;
				// bar/day(Liq)
				case 122:
					returnValue = returnValue.div( 1.380094E-06 );
					break;
				// MBPD(Liq)
				case 123:
					returnValue = returnValue.div( 1.380094 );
					break;
				// kl/sec
				case 124:
					returnValue = value;
					break;
				// kl/min
				case 125:
					returnValue = returnValue.div( 1.666667E-02 );
					break;
				// kl/hour
				case 126:
					returnValue = returnValue.div( 2.777778E-04 );
					break;
				// kl/day
				case 127:
					returnValue = returnValue.div( 1.157408E-05 );
					break;

				// Mass Flow
				// lb per sec
				case 130:
					returnValue = returnValue.div( 4.535924E-01 );
					break;
				// lb per min
				case 131:
					returnValue = returnValue.div( 7.559873E-03 );
					break;
				// lb per hour
				case 132:
					returnValue = returnValue.div( 1.259979E-04 );
					break;
				// lb per day
				case 133:
					returnValue = returnValue.div( 5.249912E-06 );
					break;
				// metric tons per min
				case 134:
					returnValue = returnValue.div( 1.666667E+01 );
					break;
				// metric tons per hour
				case 135:
					returnValue = returnValue.div( 2.777778E-01 );
					break;
				// metric tons per day
				case 136:
					returnValue = returnValue.div( 1.157407E-02 );
					break;
				// short tons per min
				case 137:
					returnValue = returnValue.div( 1.511975E+01 );
					break;
				// short tons per hour
				case 138:
					returnValue = returnValue.div( 2.519958E-01 );
					break;
				// short tons per day
				case 139:
					returnValue = returnValue.div( 1.049983E-02 );
					break;
				// long tons per min
				case 140:
					returnValue = returnValue.div( 1.693412E+01 );
					break;
				// long tons per hour
				case 141:
					returnValue = returnValue.div( 2.822353E-01 );
					break;
				// long tons per day
				case 142:
					returnValue = returnValue.div( 1.175980E-02 );
					break;
				// g/sec 
				case 143:
					returnValue = returnValue.div( 1.000000E-03 );
					break;
				// g/min	 
				case 144:
					returnValue = returnValue.div( 1.666667E-05 );
					break;
				// g/hr
				case 145:
					returnValue = returnValue.div( 2.777778E-07 );
					break;
				// Kg/Sec (SI)
				case 146:
					returnValue = value;
					break;
				// Kg/min
				case 147:
					returnValue = returnValue.div( 1.666667E-02 );
					break;
				// Kg/hr
				case 148:
					returnValue = returnValue.div( 2.777778E-04 );
					break;
				// Kg/day
				case 149:
					returnValue = returnValue.div( 1.157407E-05 );
					break;
				// Mlbs/Sec
				case 150:
					returnValue = returnValue.div( 4.535924E+02 );
					break;
				// Mlbs/Minute
				case 151:
					returnValue = returnValue.div( 7.559873E+00 );
					break;
				// Mlbs/Hour
				case 152:
					returnValue = returnValue.div( 1.259979E-01 );
					break;
				// Mlbs/Day
				case 153:
					returnValue = returnValue.div( 5.249912E-03 );
					break;

				// Velocity & Rate Units
				// in per sec
				case 160:
					returnValue = returnValue.div( 2.540000E-02 );
					break;
				// ft per sec
				case 161:
					returnValue = returnValue.div( 3.048000E+01 );
					break;
				// ft per min
				case 162:
					returnValue = returnValue.div( 5.080000E-03 );
					break;
				// mm per sec
				case 163:
					returnValue = returnValue.div( 1.000000E-03 );
					break;
				// cm per sec
				case 164:
					returnValue = returnValue.div( 1.000000E-02 );
					break;
				// meter per sec (SI)
				case 165:
					returnValue = value;
					break;
				// meter per min
				case 166:
					returnValue = returnValue.div( 1.666667E-02 );
					break;
				// miles per hour
				case 167:
					returnValue = returnValue.div( 4.470400E-01 );
					break;
				// meter/hour
				case 168:
					returnValue = returnValue.div( 2.777778E-04 );
					break;
				// kilometer/hour
				case 169:
					returnValue = returnValue.div( 2.777778E-01 );
					break;
				// knots
				case 170:
					returnValue = returnValue.div( 5.144444E-01 );
					break;
				// mm per min
				case 171:
					returnValue = returnValue.div( 1.666667E-05 );
					break;

				// Density
				// Grams per Cubic Centimeter
				case 180:
					returnValue = returnValue.div( 1.000000E+03 );
					break;
				// Grams per Milliletre
				case 181:
					returnValue = returnValue.div( 1.000000E+03 );
					break;
				// gram/liter
				case 182:
					returnValue = value;
					break;
				// kilogram/cu meter (SI)
				case 183:
					returnValue = value;
					break;
				// Killogram per Litre
				case 184:
					returnValue = returnValue.div( 1.000000E+03 );
					break;
				// Pound per Cubic  Inch
				case 185:
					returnValue = returnValue.div( 2.767990E+4 );
					break;
				// Pound per Cubic Foot
				case 186:
					returnValue = returnValue.div( 1.601846E+1 );
					break;
				// Pound per Gallon
				case 187:
					returnValue = returnValue.div( 1.19829363E+2 );
					break;
				// Pound per Imperial Gallon
				case 188:
					returnValue = returnValue.div( 9.977633E+1 );
					break;
				// Pound per Barrel Oil
				case 189:
					returnValue = returnValue.div( 2.853010 );
					break;
				// Pound per Barrel Liquid
				case 190:
					returnValue = returnValue.div( 3.804026 );
					break;
				// Degrees API
				case 191:
					switch ( value )
					{
						 case 0:
								returnValue = math.bigumber( 1000 );
								break;
						 default:
								returnValue = math.subtract( math.divide( math.bignumber( 141.5 * 999.012 ), returnValue ), 131.5 );
								break;
					};
					break;
				//Specific Gravity
				case 192:
					returnValue = returnValue.div( FMConvertEngUnits.WaterDensity( tempReferenceValue ) );
					break;
				// % Plato
				case 193:
					returnValue = math.divide( math.bignumber( 141360.48 ), math.add( returnValue, 131.5 ) );
					break;
				// Degrees Brix
				case 194:
					var term1 = math.add( returnValue.mul( this.FM_Brixb3 ), this.FM_Brixb2 );
					var term2 = math.add( term1.mul( returnValue ), this.FM_Brixb1 );
					var term3 = math.add( term2.mul( returnValue ), this.FM_Brixb0 );
					returnValue = term3;
					break;
				// Degrees Baume (light)
				case 195:
					returnValue = math.subtract( math.divide( math.multiply( 140.0, this.WaterDensity( tempReferenceValue ) ), returnValue ), 130.0 );
					break;
				// Degrees Baume (heavy)
				case 196:
					returnValue = math.bignumber( 145.0 ).sub( math.divide( math.multiply( 145.0, this.WaterDensity( tempReferenceValue ) ), returnValue ) );
					break;
				// short ton/cu yard
				case 199:
					returnValue = returnValue.div( 1.186553E+03 );
					break;
				default:
					returnValue = value;
					break;
		 }

		 //If the roundfactor parameter is not supplied then return value with all precision.
		 return ( performRound ) ? math.bignumber( returnValue.toFixed( roundfactor ) ) : returnValue;
	};
FMConvertEngUnits.Convert =
	function( value, fromUnit, toUnit, roundFactor )
	{
		//debugger;
		return this.ConvertFromSIUnits(this.ConvertToSIUnits(value, fromUnit), toUnit, roundFactor);
	};
FMConvertEngUnits.PopulateUnitsArray =
	function()
	{
		 this.unitsArray.push( {
				EngineeringUnitIndex: 1,
				EngineeringUnitName: 'Degrees Celcius',
				EngineeringUnitAbbreviation: '°C'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 2,
				EngineeringUnitName: 'Degrees Farenheit',
				EngineeringUnitAbbreviation: '°F'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 3,
				EngineeringUnitName: 'Degrees Kelvin',
				EngineeringUnitAbbreviation: 'Kelvin'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 4,
				EngineeringUnitName: 'Degrees Rankine',
				EngineeringUnitAbbreviation: '°R'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 5,
				EngineeringUnitName: 'MilliSeconds',
				EngineeringUnitAbbreviation: 'msec'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 6,
				EngineeringUnitName: 'Seconds',
				EngineeringUnitAbbreviation: 'sec'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 7,
				EngineeringUnitName: 'Minutes',
				EngineeringUnitAbbreviation: 'min'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 8,
				EngineeringUnitName: 'Hours',
				EngineeringUnitAbbreviation: 'hr'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 9,
				EngineeringUnitName: 'Days',
				EngineeringUnitAbbreviation: 'days'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 10,
				EngineeringUnitName: 'Weeks',
				EngineeringUnitAbbreviation: 'wks'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 11,
				EngineeringUnitName: 'Months',
				EngineeringUnitAbbreviation: 'mon'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 12,
				EngineeringUnitName: 'Years',
				EngineeringUnitAbbreviation: 'yrs'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 19,
				EngineeringUnitName: 'ft/inch/8th',
				EngineeringUnitAbbreviation: 'ft-in-8th'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 20,
				EngineeringUnitName: 'Millimeters',
				EngineeringUnitAbbreviation: 'mm'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 21,
				EngineeringUnitName: 'Centimeters',
				EngineeringUnitAbbreviation: 'cm'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 22,
				EngineeringUnitName: 'Meters',
				EngineeringUnitAbbreviation: 'm'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 23,
				EngineeringUnitName: 'Kilometers',
				EngineeringUnitAbbreviation: 'km'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 24,
				EngineeringUnitName: '16th of Inch',
				EngineeringUnitAbbreviation: '16th'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 25,
				EngineeringUnitName: 'Inches',
				EngineeringUnitAbbreviation: 'in'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 26,
				EngineeringUnitName: 'Feet',
				EngineeringUnitAbbreviation: 'ft'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 27,
				EngineeringUnitName: 'ft/inch/16th',
				EngineeringUnitAbbreviation: 'ft-in-16th'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 28,
				EngineeringUnitName: 'Yards',
				EngineeringUnitAbbreviation: 'yd'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 29,
				EngineeringUnitName: 'Miles',
				EngineeringUnitAbbreviation: 'mi'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 30,
				EngineeringUnitName: 'square millimeters',
				EngineeringUnitAbbreviation: 'mm²'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 31,
				EngineeringUnitName: 'square centimeters',
				EngineeringUnitAbbreviation: 'cm²'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 32,
				EngineeringUnitName: 'square meters',
				EngineeringUnitAbbreviation: 'm²'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 33,
				EngineeringUnitName: 'square kilometers',
				EngineeringUnitAbbreviation: 'km²'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 34,
				EngineeringUnitName: 'square 16ths inch',
				EngineeringUnitAbbreviation: '16th²'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 35,
				EngineeringUnitName: 'square inches',
				EngineeringUnitAbbreviation: 'in²'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 36,
				EngineeringUnitName: 'square feet',
				EngineeringUnitAbbreviation: 'ft²'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 37,
				EngineeringUnitName: 'square yards',
				EngineeringUnitAbbreviation: 'yd²'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 38,
				EngineeringUnitName: 'square miles',
				EngineeringUnitAbbreviation: 'mi²'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 40,
				EngineeringUnitName: 'Cubic centimeters',
				EngineeringUnitAbbreviation: 'cc'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 41,
				EngineeringUnitName: 'Cubic meters',
				EngineeringUnitAbbreviation: 'm³'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 42,
				EngineeringUnitName: 'Liters',
				EngineeringUnitAbbreviation: 'l'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 43,
				EngineeringUnitName: 'Cubic inches',
				EngineeringUnitAbbreviation: 'in³'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 44,
				EngineeringUnitName: 'Cubic feet',
				EngineeringUnitAbbreviation: 'ft³'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 45,
				EngineeringUnitName: 'Cubic yards',
				EngineeringUnitAbbreviation: 'yd³'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 46,
				EngineeringUnitName: 'U.S. Gallons',
				EngineeringUnitAbbreviation: 'gal (US)'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 47,
				EngineeringUnitName: 'Imperial Gallons',
				EngineeringUnitAbbreviation: 'gal (UK)'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 48,
				EngineeringUnitName: 'Barrels (Oil)',
				EngineeringUnitAbbreviation: 'bbl (Oil)'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 49,
				EngineeringUnitName: 'Barrels Liquid',
				EngineeringUnitAbbreviation: 'bbl (Liq)'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 50,
				EngineeringUnitName: 'Kiloliters',
				EngineeringUnitAbbreviation: 'kl'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 51,
				EngineeringUnitName: '1000 standard cubic feet',
				EngineeringUnitAbbreviation: 'MsFt3'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 60,
				EngineeringUnitName: 'Grams',
				EngineeringUnitAbbreviation: 'g'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 61,
				EngineeringUnitName: 'Kilograms',
				EngineeringUnitAbbreviation: 'kg'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 62,
				EngineeringUnitName: 'Metric Tons',
				EngineeringUnitAbbreviation: 'ton (m)'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 63,
				EngineeringUnitName: 'Ounces',
				EngineeringUnitAbbreviation: 'oz'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 64,
				EngineeringUnitName: 'Pounds',
				EngineeringUnitAbbreviation: 'lb'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 65,
				EngineeringUnitName: 'English Tons',
				EngineeringUnitAbbreviation: 'ton (e)'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 66,
				EngineeringUnitName: 'Short Tons',
				EngineeringUnitAbbreviation: 'ton (s)'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 67,
				EngineeringUnitName: 'Long Tons',
				EngineeringUnitAbbreviation: 'ton (l)'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 68,
				EngineeringUnitName: 'Pounds (Thousands)',
				EngineeringUnitAbbreviation: 'Mlbs'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 70,
				EngineeringUnitName: 'Pascals',
				EngineeringUnitAbbreviation: 'Pa'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 71,
				EngineeringUnitName: 'KiloPascals',
				EngineeringUnitAbbreviation: 'kPa'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 72,
				EngineeringUnitName: 'kilograms/sq cm',
				EngineeringUnitAbbreviation: 'kg/cm²'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 73,
				EngineeringUnitName: 'pounds/sq in (PSI)',
				EngineeringUnitAbbreviation: 'PSI'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 74,
				EngineeringUnitName: 'PSI Gauge',
				EngineeringUnitAbbreviation: 'psig'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 75,
				EngineeringUnitName: 'PSI Absolute',
				EngineeringUnitAbbreviation: 'psia'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 76,
				EngineeringUnitName: 'inches H2O @ 68F',
				EngineeringUnitAbbreviation: 'in H2O'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 77,
				EngineeringUnitName: 'feet H2O @ 68F',
				EngineeringUnitAbbreviation: 'ft H2O'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 78,
				EngineeringUnitName: 'Inches Mercury @ 0C',
				EngineeringUnitAbbreviation: 'in Hg'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 79,
				EngineeringUnitName: 'pounds per sq. ft',
				EngineeringUnitAbbreviation: 'lb/ft²'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 80,
				EngineeringUnitName: 'Torr @ 0C',
				EngineeringUnitAbbreviation: 'torr'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 81,
				EngineeringUnitName: 'Bar',
				EngineeringUnitAbbreviation: 'bar'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 82,
				EngineeringUnitName: 'Millibar',
				EngineeringUnitAbbreviation: 'mbar'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 83,
				EngineeringUnitName: 'mm Mercury @ 0C',
				EngineeringUnitAbbreviation: 'mm Hg'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 84,
				EngineeringUnitName: 'mm H2O @ 68F',
				EngineeringUnitAbbreviation: 'mm H2O'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 85,
				EngineeringUnitName: 'grams/sq cm',
				EngineeringUnitAbbreviation: 'g/cm²'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 86,
				EngineeringUnitName: 'Atmospheres',
				EngineeringUnitAbbreviation: 'atm'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 90,
				EngineeringUnitName: 'cc per min',
				EngineeringUnitAbbreviation: 'cc/min'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 91,
				EngineeringUnitName: 'cc per hour',
				EngineeringUnitAbbreviation: 'cc/hr'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 92,
				EngineeringUnitName: 'cu. meters per sec',
				EngineeringUnitAbbreviation: 'm³/sec'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 93,
				EngineeringUnitName: 'cu. meters per Minute',
				EngineeringUnitAbbreviation: 'm³/min'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 94,
				EngineeringUnitName: 'cu. meters per Hour',
				EngineeringUnitAbbreviation: 'm³/hr'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 95,
				EngineeringUnitName: 'cu. meters per Day',
				EngineeringUnitAbbreviation: 'm³/day'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 96,
				EngineeringUnitName: 'Liters per sec',
				EngineeringUnitAbbreviation: 'l/sec'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 97,
				EngineeringUnitName: 'Liters per minute',
				EngineeringUnitAbbreviation: 'l/min'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 98,
				EngineeringUnitName: 'Liters per Hour',
				EngineeringUnitAbbreviation: 'l/hr'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 99,
				EngineeringUnitName: 'Million liters per day',
				EngineeringUnitAbbreviation: 'Ml/day'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 100,
				EngineeringUnitName: 'cu. inches / min',
				EngineeringUnitAbbreviation: 'in³/min'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 101,
				EngineeringUnitName: 'cu. inches / hour',
				EngineeringUnitAbbreviation: 'in³/hr'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 102,
				EngineeringUnitName: 'cu. feet / sec',
				EngineeringUnitAbbreviation: 'ft³/sec'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 103,
				EngineeringUnitName: 'cu. feet / min',
				EngineeringUnitAbbreviation: 'ft³/min'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 104,
				EngineeringUnitName: 'cu. feet / hour',
				EngineeringUnitAbbreviation: 'ft³/hr'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 105,
				EngineeringUnitName: 'cu. feet / day',
				EngineeringUnitAbbreviation: 'ft³/day'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 106,
				EngineeringUnitName: 'cu. yards / min',
				EngineeringUnitAbbreviation: 'yd³/min'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 107,
				EngineeringUnitName: 'cu. yards / hour',
				EngineeringUnitAbbreviation: 'yd³/hr'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 108,
				EngineeringUnitName: 'U.S. Gallons / sec',
				EngineeringUnitAbbreviation: 'gps (US)'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 109,
				EngineeringUnitName: 'U.S. Gallons / min',
				EngineeringUnitAbbreviation: 'gpm (US)'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 110,
				EngineeringUnitName: 'U.S. Gallons / hour',
				EngineeringUnitAbbreviation: 'gph (US)'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 111,
				EngineeringUnitName: 'Mill. U.S. Gallons / day',
				EngineeringUnitAbbreviation: 'MGPD (US)'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 112,
				EngineeringUnitName: 'U.K. Gallons / sec',
				EngineeringUnitAbbreviation: 'gps (UK)'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 113,
				EngineeringUnitName: 'U.K. Gallons / min',
				EngineeringUnitAbbreviation: 'gpm (UK)'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 114,
				EngineeringUnitName: 'U.K. Gallons / hour',
				EngineeringUnitAbbreviation: 'gph (UK)'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 115,
				EngineeringUnitName: 'Mill. U.K. Gallons / day',
				EngineeringUnitAbbreviation: 'MGPD (UK)'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 116,
				EngineeringUnitName: 'bbl per min (oil)',
				EngineeringUnitAbbreviation: 'BPM (Oil)'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 117,
				EngineeringUnitName: 'bbl per hour (oil)',
				EngineeringUnitAbbreviation: 'BPH (Oil)'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 118,
				EngineeringUnitName: 'bbl per day (oil)',
				EngineeringUnitAbbreviation: 'BPD (Oil)'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 119,
				EngineeringUnitName: 'Mbbl / day (oil)',
				EngineeringUnitAbbreviation: 'MBPD (Oil)'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 120,
				EngineeringUnitName: 'bbl per min (liq)',
				EngineeringUnitAbbreviation: 'BPM (Liq)'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 121,
				EngineeringUnitName: 'bbl per hour (liq)',
				EngineeringUnitAbbreviation: 'BPH (Liq)'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 122,
				EngineeringUnitName: 'bbl per day (liq)',
				EngineeringUnitAbbreviation: 'BPD (Liq)'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 123,
				EngineeringUnitName: 'Mbbl / day (liq)',
				EngineeringUnitAbbreviation: 'MBPD (Liq)'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 124,
				EngineeringUnitName: 'kiloliters / sec',
				EngineeringUnitAbbreviation: 'kl/sec'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 125,
				EngineeringUnitName: 'kiloliters / min',
				EngineeringUnitAbbreviation: 'kl/min'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 126,
				EngineeringUnitName: 'kiloliters / hr',
				EngineeringUnitAbbreviation: 'kl/hr'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 127,
				EngineeringUnitName: 'kiloliters / day',
				EngineeringUnitAbbreviation: 'kl/day'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 130,
				EngineeringUnitName: 'Pounds per sec',
				EngineeringUnitAbbreviation: 'lb/sec'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 131,
				EngineeringUnitName: 'Pounds per min',
				EngineeringUnitAbbreviation: 'lb/min'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 132,
				EngineeringUnitName: 'Pounds per hour',
				EngineeringUnitAbbreviation: 'lb/hr'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 133,
				EngineeringUnitName: 'Pounds per day',
				EngineeringUnitAbbreviation: 'lb/day'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 134,
				EngineeringUnitName: 'Metric tons per min',
				EngineeringUnitAbbreviation: 'ton(m)/min'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 135,
				EngineeringUnitName: 'Metric tons per hour',
				EngineeringUnitAbbreviation: 'ton(m)/hr'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 136,
				EngineeringUnitName: 'Metric tons per day',
				EngineeringUnitAbbreviation: 'ton(m)/day'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 137,
				EngineeringUnitName: 'Short tons per min',
				EngineeringUnitAbbreviation: 'ton(s)/min'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 138,
				EngineeringUnitName: 'Short tons per hour',
				EngineeringUnitAbbreviation: 'ton(s)/hr'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 139,
				EngineeringUnitName: 'Short tons per day',
				EngineeringUnitAbbreviation: 'ton(s)/day'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 140,
				EngineeringUnitName: 'Long tons per min',
				EngineeringUnitAbbreviation: 'ton(l)/min'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 141,
				EngineeringUnitName: 'Long tons per hour',
				EngineeringUnitAbbreviation: 'ton(l)/hr'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 142,
				EngineeringUnitName: 'Long tons per day',
				EngineeringUnitAbbreviation: 'ton(l)/day'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 143,
				EngineeringUnitName: 'Grams per sec',
				EngineeringUnitAbbreviation: 'g/sec'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 144,
				EngineeringUnitName: 'Grams per min',
				EngineeringUnitAbbreviation: 'g/min'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 145,
				EngineeringUnitName: 'Grams per hour',
				EngineeringUnitAbbreviation: 'g/hr'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 146,
				EngineeringUnitName: 'Kilograms per sec',
				EngineeringUnitAbbreviation: 'kg/sec'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 147,
				EngineeringUnitName: 'Kilograms per min',
				EngineeringUnitAbbreviation: 'kg/min'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 148,
				EngineeringUnitName: 'Kilograms per hr',
				EngineeringUnitAbbreviation: 'kg/hr'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 149,
				EngineeringUnitName: 'Kilograms per day',
				EngineeringUnitAbbreviation: 'kg/day'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 150,
				EngineeringUnitName: 'Million Pounds per sec',
				EngineeringUnitAbbreviation: 'Mlbs/sec'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 151,
				EngineeringUnitName: 'Million Pounds per min',
				EngineeringUnitAbbreviation: 'Mlbs/min'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 152,
				EngineeringUnitName: 'Million Pounds per hour',
				EngineeringUnitAbbreviation: 'Mlbs/hr'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 153,
				EngineeringUnitName: 'Million Pounds per day',
				EngineeringUnitAbbreviation: 'Mlbs/day'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 160,
				EngineeringUnitName: 'Inches per sec',
				EngineeringUnitAbbreviation: 'in/sec'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 161,
				EngineeringUnitName: 'Feet per sec',
				EngineeringUnitAbbreviation: 'ft/sec'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 162,
				EngineeringUnitName: 'Feet per min',
				EngineeringUnitAbbreviation: 'ft/min'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 163,
				EngineeringUnitName: 'Millimeters per sec',
				EngineeringUnitAbbreviation: 'mm/sec'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 164,
				EngineeringUnitName: 'Centimeters per sec',
				EngineeringUnitAbbreviation: 'cm/sec'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 165,
				EngineeringUnitName: 'Meters per sec',
				EngineeringUnitAbbreviation: 'm/sec'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 166,
				EngineeringUnitName: 'Meters per min',
				EngineeringUnitAbbreviation: 'm/min'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 167,
				EngineeringUnitName: 'Miles per hour',
				EngineeringUnitAbbreviation: 'MPH'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 168,
				EngineeringUnitName: 'Meters per hour',
				EngineeringUnitAbbreviation: 'm/hr'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 169,
				EngineeringUnitName: 'Kilometers per hour',
				EngineeringUnitAbbreviation: 'KPH'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 170,
				EngineeringUnitName: 'Knots',
				EngineeringUnitAbbreviation: 'KNOT'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 171,
				EngineeringUnitName: 'Millimeters / min',
				EngineeringUnitAbbreviation: 'mm/min'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 180,
				EngineeringUnitName: 'Grams / cu. cm.',
				EngineeringUnitAbbreviation: 'g/cm³'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 181,
				EngineeringUnitName: 'Grams / milliliter',
				EngineeringUnitAbbreviation: 'g/ml'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 182,
				EngineeringUnitName: 'Grams / liter',
				EngineeringUnitAbbreviation: 'g/l'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 183,
				EngineeringUnitName: 'Kilograms / cu. meter',
				EngineeringUnitAbbreviation: 'kg/m³'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 184,
				EngineeringUnitName: 'Kilograms / liter',
				EngineeringUnitAbbreviation: 'kg/l'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 185,
				EngineeringUnitName: 'Pounds / cu. inch',
				EngineeringUnitAbbreviation: 'lb/in³'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 186,
				EngineeringUnitName: 'Pounds / cu. foot',
				EngineeringUnitAbbreviation: 'lb/ft³'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 187,
				EngineeringUnitName: 'Pounds / gallon (U.S.)',
				EngineeringUnitAbbreviation: 'lb/gal(US)'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 188,
				EngineeringUnitName: 'Pounds / gallon (U.K.)',
				EngineeringUnitAbbreviation: 'lb/gal(UK)'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 189,
				EngineeringUnitName: 'Pounds / barrel (oil)',
				EngineeringUnitAbbreviation: 'lb/bbl(o)'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 190,
				EngineeringUnitName: 'Pounds / barrel (liq)',
				EngineeringUnitAbbreviation: 'lb/bbl(l)'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 191,
				EngineeringUnitName: 'Degrees API',
				EngineeringUnitAbbreviation: '°API'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 192,
				EngineeringUnitName: 'Specific gravity',
				EngineeringUnitAbbreviation: 'sp gr'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 193,
				EngineeringUnitName: 'Percent Plato',
				EngineeringUnitAbbreviation: '% Plato'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 194,
				EngineeringUnitName: 'Degrees BRIX',
				EngineeringUnitAbbreviation: '°BRIX'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 195,
				EngineeringUnitName: 'Degrees Baume (light)',
				EngineeringUnitAbbreviation: '°Ba (l)'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 196,
				EngineeringUnitName: 'Degrees Baume (heavy)',
				EngineeringUnitAbbreviation: '°Ba (h)'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 197,
				EngineeringUnitName: 'Degrees Twaddell',
				EngineeringUnitAbbreviation: '°Tw'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 198,
				EngineeringUnitName: 'Degrees Balling',
				EngineeringUnitAbbreviation: '°Balling'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 199,
				EngineeringUnitName: 'Short tons / cubic yard',
				EngineeringUnitAbbreviation: 'ton(s)/yd³'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 200,
				EngineeringUnitName: 'British Thermal Units',
				EngineeringUnitAbbreviation: 'BTU'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 201,
				EngineeringUnitName: 'Calories',
				EngineeringUnitAbbreviation: 'cal'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 202,
				EngineeringUnitName: 'Joules',
				EngineeringUnitAbbreviation: 'J'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 203,
				EngineeringUnitName: 'Watt-hours',
				EngineeringUnitAbbreviation: 'WH'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 204,
				EngineeringUnitName: 'Kilowatt-hours',
				EngineeringUnitAbbreviation: 'kWH'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 210,
				EngineeringUnitName: 'BTU / sec',
				EngineeringUnitAbbreviation: 'BTU/sec'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 211,
				EngineeringUnitName: 'BTU / min',
				EngineeringUnitAbbreviation: 'BTU/min'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 212,
				EngineeringUnitName: 'BTU / hour',
				EngineeringUnitAbbreviation: 'BTU/hr'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 213,
				EngineeringUnitName: 'Cal / min',
				EngineeringUnitAbbreviation: 'cal/min'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 214,
				EngineeringUnitName: 'Watts',
				EngineeringUnitAbbreviation: 'W'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 215,
				EngineeringUnitName: 'KiloWatts',
				EngineeringUnitAbbreviation: 'kW'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 216,
				EngineeringUnitName: 'Kilo Volt-Amperes',
				EngineeringUnitAbbreviation: 'kVA'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 217,
				EngineeringUnitName: 'Horsepower',
				EngineeringUnitAbbreviation: 'hp'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 220,
				EngineeringUnitName: 'Millivolts',
				EngineeringUnitAbbreviation: 'mV'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 221,
				EngineeringUnitName: 'Volts',
				EngineeringUnitAbbreviation: 'V'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 222,
				EngineeringUnitName: 'Milliamperes',
				EngineeringUnitAbbreviation: 'mA'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 223,
				EngineeringUnitName: 'Amperes',
				EngineeringUnitAbbreviation: 'A'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 224,
				EngineeringUnitName: 'Ohms',
				EngineeringUnitAbbreviation: 'ohm'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 225,
				EngineeringUnitName: 'Farads',
				EngineeringUnitAbbreviation: 'F'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 226,
				EngineeringUnitName: 'Coulombs',
				EngineeringUnitAbbreviation: 'C'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 227,
				EngineeringUnitName: 'Henrys',
				EngineeringUnitAbbreviation: 'H'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 228,
				EngineeringUnitName: 'MicroSiemens',
				EngineeringUnitAbbreviation: 'µS'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 229,
				EngineeringUnitName: 'Siemens',
				EngineeringUnitAbbreviation: 'S'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 230,
				EngineeringUnitName: 'MHOs',
				EngineeringUnitAbbreviation: 'mho'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 231,
				EngineeringUnitName: 'Power factor',
				EngineeringUnitAbbreviation: 'P.F.'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 232,
				EngineeringUnitName: 'Revolutions / min',
				EngineeringUnitAbbreviation: 'RPM'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 233,
				EngineeringUnitName: 'Cycles / sec (Hz)',
				EngineeringUnitAbbreviation: 'Hz'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 234,
				EngineeringUnitName: 'Percent',
				EngineeringUnitAbbreviation: '%'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 235,
				EngineeringUnitName: 'Parts per million',
				EngineeringUnitAbbreviation: 'PPM'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 236,
				EngineeringUnitName: '% Humidity',
				EngineeringUnitAbbreviation: '%H'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 237,
				EngineeringUnitName: '% Oxygen',
				EngineeringUnitAbbreviation: '%O2'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 238,
				EngineeringUnitName: 'Relative Humidity',
				EngineeringUnitAbbreviation: 'RH'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 239,
				EngineeringUnitName: 'pH',
				EngineeringUnitAbbreviation: 'pH'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 240,
				EngineeringUnitName: 'Centipoise',
				EngineeringUnitAbbreviation: 'centp'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 241,
				EngineeringUnitName: '% Solids by weight',
				EngineeringUnitAbbreviation: '%sol-wt'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 242,
				EngineeringUnitName: '% Solids by volume',
				EngineeringUnitAbbreviation: '%sol-vol'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 243,
				EngineeringUnitName: '% Steam quality',
				EngineeringUnitAbbreviation: '%quality'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 244,
				EngineeringUnitName: 'Bushels',
				EngineeringUnitAbbreviation: 'bushel'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 245,
				EngineeringUnitName: 'Proof volume',
				EngineeringUnitAbbreviation: 'pr vol'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 246,
				EngineeringUnitName: 'Proof mass',
				EngineeringUnitAbbreviation: 'pr mass'
		 } );
		 this.unitsArray.push( {
				EngineeringUnitIndex: 247,
				EngineeringUnitName: 'Cubic feet / pound',
				EngineeringUnitAbbreviation: 'ft³/lb'
		 } );
	};
FMConvertEngUnits.FindByUnitIndex = function( unit, unitIndex )
{
	return unit.EngineeringUnitIndex === unitIndex;
};
FMConvertEngUnits.FindByAbbrev = function( unit, abbrev )
{
	return unit.EngineeringUnitAbbreviation === abbrev;
};
FMConvertEngUnits.FindByName = function( unit, name )
{
	return unit.EngineeringUnitName === name;
};
FMConvertEngUnits.GetEngineeringUnitByIndex =
	function( unitIndex )
	{
		 'use strict';
		 var returnVal = null;
		 this.unitsArray.forEach( function( unit )
		 {
				if ( unit.EngineeringUnitIndex === unitIndex )
				{
					returnVal = unit;
					return false;
				}
				return true;
		 } );
		 return returnVal;
	};
FMConvertEngUnits.GetEngineeringUnitName =
	function( unit )
	{
		 'use strict';
		 var unitObj = this.GetEngineeringUnitByIndex( unit );
		 if ( unitObj )
		 {
				return unitObj.EngineeringUnitName;
		 }
		 else
		 {
				return undefined;
		 }
	};
FMConvertEngUnits.GetEngineeringUnitAbbreviation =
	function( unit )
	{
		 'use strict';
		 var unitObj = this.GetEngineeringUnitByIndex( unit );
		 if ( unitObj )
		 {
				return unitObj.EngineeringUnitAbbreviation;
		 }
		 else
		 {
				return undefined;
		 }
	};
FMConvertEngUnits.GetEngineeringUnitByAbbrev =
	function( abbrev )
	{
		 'use strict';
		 var returnVal = null;
		 this.unitsArray.forEach( function( unit )
		 {
				if ( unit.EngineeringUnitAbbreviation === abbrev )
				{
					returnVal = unit;
					return false;
				}
				return true;
		 } );
		 return returnVal;
	};
FMConvertEngUnits.GetEngineeringUnitIndexByAbbrev =
	function( abbrev )
	{
		 'use strict';
		 var unit = this.GetEngineeringUnitByAbbrev( abbrev );
		 return ( unit ) ? unit.EngineeringUnitIndex : 0;
	};
FMConvertEngUnits.GetEngineeringUnitByName =
	function( name )
	{
		 'use strict';
		 var returnVal = null;
		 this.unitsArray.forEach( function( unit )
		 {
				if ( unit.EngineeringUnitName === name )
				{
					returnVal = unit;
					return false;
				}
				return true;
		 } );
		 return returnVal;
	};
FMConvertEngUnits.GetEngineeringUnitIndexByName =
	function( name )
	{
		 'use strict';
		 var unit = this.GetEngineeringUnitByName( name );
		 return ( unit ) ? unit.EngineeringUnitIndex : 0;
	}; //Call Function to populate the unitsArray
FMConvertEngUnits.PopulateUnitsArray();