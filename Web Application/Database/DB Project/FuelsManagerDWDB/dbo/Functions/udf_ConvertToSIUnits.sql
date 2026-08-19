CREATE FUNCTION [dbo].[udf_ConvertToSIUnits]
(@Value FLOAT, @Units INT)
RETURNS FLOAT
AS
BEGIN 
	DECLARE @Result float

	set @Result =
		case @Units
--Temperature
-- Centigrade (SI)
			when 1 then @Value
-- Ferenheit
			when 2 then (@Value-32)/1.8
-- Kelvin
			when 3 then @Value+273.15
-- Rankine
			when 4 then (@Value-491.67)*5/9

-- Length
-- Ft/In/8th
			when 19 then @Value*3.048000E-01
-- Millimeters
			when 20 then @Value*1.00000E-03
-- Centimeters
			when 21 then @Value*1.00000E-02
-- Meters (SI)
			when 22 then @Value
-- Killometers
			when 23 then @Value*1.000000E+03
-- 16th of inch
			when 24 then @Value*1.587500E-03
-- Inches
			when 25 then @Value*2.540000E-02
-- Feet
			when 26 then @Value*3.048000E-01
-- ft/in/16th
			when 27 then @Value*3.048000E-01
-- yard
			when 28 then @Value*9.144000E-01
-- mile
			when 29 then @Value*1.609347E+03
-- Area
-- sq mm
			when 30 then @Value*1.000000E-06
-- sq cm
			when 31 then @Value*1.000000E-04
-- meter (SI)
			when 32 then @Value
-- sq kilometer
			when 33 then @Value*1.000000E+06
-- sq 16th
			when 34 then @Value*2.520156E-6
-- sq inch
			when 35 then @Value*6.451600E-04
-- sq feet
			when 36 then @Value*9.290304E-02
-- sq yard
			when 37 then @Value*8.361274E-01
-- sq mile
			when 38 then @Value*2.589998E+06

-- Volume
-- cu. cm
			when 40 then @Value*1.000000E-06
-- Cu. Meter (SI)
			when 41 then @Value
-- Litre
			when 42 then @Value*1.000000E-03
-- Cubic Inch
			when 43 then @Value*1.638706E-05
-- Cubic Feet
			when 44 then @Value*2.831685E-02
-- Cubic Yard
			when 45 then @Value*7.645549E-01
-- US Gallon
			when 46 then @Value*3.785412E-03
-- Imperial Gallon
			when 47 then @Value*4.546092E-03
-- Barrel Oil
			when 48 then @Value*1.589873E-01
-- Barrel Liquid
			when 49 then @Value*1.192401E-01
-- Kiloleter 
			when 50 then @Value			
-- 1000 standard cubic feet 			
			when 51 then @Value*2.6853E+01 

-- Mass
-- grams
			when 60 then @Value*1.000000E-03
-- Kilogram (SI)
			when 61 then @Value
-- Metric Ton
			when 62 then @Value*1.000000E+03
-- Ounce
			when 63 then @Value*2.834952E-02
-- Pound
			when 64 then @Value*4.535924E-01
-- English Ton
			when 65 then @Value*1.000000E+03
-- Short Ton (2000 lbs)
			when 66 then @Value*9.071847E+02
-- Long Ton
			when 67 then @Value*1.016047E+03
-- Mlbs. (Thousands of Pounds)
			when 68 then @Value*4.535924E+02

-- Pressure Units
-- Pascal (SI)
			when 70 then @Value
-- KiloPascal
			when 71 then @Value*1.000000E+03
-- kg/ sq cm
			when 72 then @Value*9.806650E+04
-- lb/sq inch
			when 73 then @Value*6.894757E+03
-- PSI Gauge
			when 74 then @Value*6.894757E+03
-- PSI Absolute
			when 75 then @Value*6.894757E+03
-- in. H20 (39.2 F)
			when 76 then @Value*2.490820E+02
-- Ft H20 (39.2 F)
			when 77 then @Value*2.988980E+03
-- In. Mercury(@ 0C)
			when 78 then @Value*3.386380E+03
-- lb/sq ft
			when 79 then @Value*4.788026E+01
-- Torr (@ 0C)
			when 80 then @Value*1.333220E+02
-- Bar
			when 81 then @Value*1.000000E+05
-- MilliBar
			when 82 then @Value*1.000000E+02
-- mm HG (@ 0C)
			when 83 then @Value*1.333220E+02
-- mm H2O (@ 4C)
			when 84 then @Value*9.80638E+00
-- gr/ sq cm
			when 85 then @Value*9.806650E+01
-- Atmosphere
			when 86 then @Value*1.013250E+05

-- Volumetric Flow
-- CC/min
			when 90 then @Value*1.666667E-08
-- CC/hour
			when 91 then @Value*2.777778E-10
-- m3/sec (SI)
			when 92 then @Value
-- m3/min
			when 93 then @Value*1.666667E-02
-- m3/hr
			when 94 then @Value*2.777778E-04
-- m3/day 
			when 95 then @Value*1.157408E-05
-- lit/sec
			when 96 then @Value*1.000000E-03
-- lit/min
			when 97 then @Value*1.666667E-05
-- lit/hour
			when 98 then @Value*2.777778E-07
-- million lit/day
			when 99 then @Value*1.157407E-02
-- in3/min
			when 100 then @Value*2.731177E-07
-- in3/hour
			when 101 then @Value*4.55195E-09
-- ft3/sec
			when 102 then @Value*2.831685E-02
-- ft3/min
			when 103 then @Value*4.719475E-04
-- ft3/hour
			when 104 then @Value*7.865792E-06
-- ft3/day
			when 105 then @Value*3.277413E-07
-- yd3/min
			when 106 then @Value*1.274258E-02
-- yd3/hour
			when 107 then @Value*2.123763E-04
-- Gal/sec(US)
			when 108 then @Value*3.785412E-03
-- Gal/min(US)
			when 109 then @Value*6.309020E-05
-- Gal/Hour(US)
			when 110 then @Value*1.051503E-06
-- Million Gal/Day(US)
			when 111 then @Value*4.381264E-02
-- Gal/Sec(IMP)
			when 112 then @Value*4.546092E-03
-- Gal/Min(IMP)
			when 113 then @Value*7.576820E-05
-- Gal/Hour(IMP)
			when 114 then @Value*1.262803E-06
-- Million Gal/Day(IMP)
			when 115 then @Value*5.261680E-02
-- BBL/min(OIL)
			when 116 then @Value*2.649788E-03
-- BBL/hour(Oil)
			when 117 then @Value*4.416314E-05
-- BBL/day(Oil)
			when 118 then @Value*1.840131E-06
-- MBPD
			when 119 then @Value*1.840131
-- bar/Min(liq)
			when 120 then @Value*1.987335E-03
-- bar/hr(Liq)
			when 121 then @Value*3.312225E-05
-- bar/day(Liq)
			when 122 then @Value*1.380094E-06
-- MBPD(Liq)
			when 123 then @Value*1.380094
-- kl/sec
			when 124 then @Value
-- kl/min
			when 125 then @Value*1.666667E-02
-- kl/hour
			when 126 then @Value*2.777778E-04
-- kl/day
			when 127 then @Value*1.157408E-05

-- Mass Flow
-- lb per sec
			when 130 then @Value*4.535924E-01
-- lb per min
			when 131 then @Value*7.559873E-03
-- lb per hour
			when 132 then @Value*1.259979E-04
-- lb per day
			when 133 then @Value*5.249912E-06
-- metric tons per min
			when 134 then @Value*1.666667E+01
-- metric tons per hour
			when 135 then @Value*2.777778E-01
-- metric tons per day
			when 136 then @Value*1.157407E-02
-- short tons per min
			when 137 then @Value*1.511975E+01
-- short tons per hour
			when 138 then @Value*2.519958E-01
-- short tons per day
			when 139 then @Value*1.049983E-02
-- long tons per min
			when 140 then @Value*1.693412E+01
-- long tons per hour
			when 141 then @Value*2.822353E-01
-- long tons per day
			when 142 then @Value*1.175980E-02
-- g/sec 
			when 143 then @Value*1.000000E-03
-- g/min     
			when 144 then @Value*1.666667E-05
-- g/hr
			when 145 then @Value*2.777778E-07
-- Kg/Sec (SI)
			when 146 then @Value
-- Kg/min
			when 147 then @Value*1.666667E-02
-- Kg/hr
			when 148 then @Value*2.777778E-04
-- Kg/day
			when 149 then @Value*1.157407E-05
-- Mlbs/Sec
			when 150 then @Value*4.535924E+02
-- Mlbs/Minute
			when 151 then @Value*7.559873E+00
-- Mlbs/Hour
			when 152 then @Value*1.259979E-01
-- Mlbs/Day
			when 153 then @Value*5.249912E-03

-- Velocity & Rate Units
-- in per sec
			when 160 then @Value*2.540000E-02
-- ft per sec
			when 161 then @Value*3.048000E+01
-- ft per min
			when 162 then @Value*5.080000E-03
-- mm per sec
			when 163 then @Value*1.000000E-03
-- cm per sec
			when 164 then @Value*1.000000E-02
-- meter per sec (SI)
			when 165 then @Value
-- meter per min
			when 166 then @Value*1.666667E-02
-- miles per hour
			when 167 then @Value*4.470400E-01
-- meter/hour
			when 168 then @Value*2.777778E-04
-- kilometer/hour
			when 169 then @Value*2.777778E-01
-- knots
			when 170 then @Value*5.144444E-01
-- mm per min
			when 171 then @Value*1.666667E-05

-- Density
-- Grams per Cubic Centimeter
			when 180 then @Value*1.000000E+03
-- Grams per Milliletre
			when 181 then @Value*1.000000E+03
-- gram/liter
			when 182 then @Value
-- kilogram/cu meter (SI)
			when 183 then @Value
-- Killogram per Litre
			when 184 then @Value*1.000000E+03
-- Pound per Cubic  Inch
			when 185 then @Value*2.767990E+4
-- Pound per Cubic Foot
			when 186 then @Value*1.601846E+1
-- Pound per Gallon
			when 187 then @Value*1.19829363E+2
-- Pound per Imperial Gallon
			when 188 then @Value*9.977633E+1
-- Pound per Barrel Oil
			when 189 then @Value*2.853010
-- Pound per Barrel Liquid
			when 190 then @Value*3.804026
-- Degrees API
			when 191 then
				case 
					when @Value <= -131.5 then 282720
					else (141.5*999.012)/(@Value + 131.5)
				end
-- short ton/cu yard
			when 199 then @Value/1.186553E+03
			else @Value
		end

	return @Result
end