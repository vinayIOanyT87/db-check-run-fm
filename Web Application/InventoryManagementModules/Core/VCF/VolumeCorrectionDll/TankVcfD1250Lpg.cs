using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Varec.CommonComponents.EngineeringUnitsLibrary;

namespace VCF
{
	class TankVcfD1250Lpg : TankBaseVcf
	{
		protected double m_dTable54ReferenceTemperature;
		public TankVcfD1250Lpg()
		{
			m_byCorrectionTypeMajor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_ASTM_D1250_1952;
			m_byCorrectionTypeMinor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_D125020DEGC;
			m_bUsesDensity = false;
			m_dTable54ReferenceTemperature = TAB54_DEF_REF_TEMP;
			m_bStandardCalculationType = EApiCalc.API_CALC_STANDARD;
		}

		public override bool TemperatureCorr(double dDensity,
											double dMeasTemp,
											double dStdTempInC,
											double dStdTemp,
											EngineeringUnit bDensityUnits,
											EngineeringUnit bTempUnits,
											double dDensityPress,           // density pressure for api 2004
											EngineeringUnit bDensityPressUnits,    // density pressure units for api 2004
											double dAlternateTemperature,   // selected refined product sub catagory for api 2004
											double dBaseTemp,   // api 2004 alternate base temp reference
											double dAlternateBasePress, // api 2004 alternate base pressure reference
											ref double[] dK,
											ref double pdVcfc,
											ref int piFlag,
												ref double CTLReturn,
												ref double CPLReturn,
											bool RangeCk,
											bool bRound,
											bool bTable60,              //	Optional
											bool UseDensity)                //	Optional
		{
			double dDensityInSPGrav = 0.0;
			double dTempInDegC = 0.0;
			double dVCF = 0.0;
			double dHydrometerTemp = 0.0;
			double dTempValue = 0.0;
			int iLoop = 0;

			var DensityTableData = new List<List<double>>{
			new List<double>{-0.0045946489678832,0.0061232431795680,-0.0000317074831323,0.0000548397230037,0.498},
			new List<double>{-0.0044279278953793,0.0057882992380437,-0.0000263544808934,0.0000441694652077,0.518},
			new List<double>{-0.0042635157420811,0.0054649855357069,-0.0000263293867090,0.0000438862463121,0.539},
			new List<double>{-0.0039313336083154,0.0048491424883735,-0.0000171988380071,0.0000271198135388,0.559},
			new List<double>{-0.0035459928199061,0.0041555627486596,-0.0000174082405540,0.0000272052538293,0.579},
			new List<double>{-0.0044795785597695,0.0057678078599348,-0.0000384017053042,0.0000636945533674,0.6},
			new List<double>{-0.0024361018961719,0.0023329279647329,-0.0000015650912583,0.0000019239173808,0.615},
			new List<double>{-0.0022189302188432,0.0019797818956931,-0.0000015669676936,0.0000019269686800,0.635},
			new List<double>{-0.0019375650211732,0.0015367709455658,-0.0000015693987823,0.0000019307964416,0.655},
			new List<double>{-0.0018211308776796,0.0013590733713544,-0.0000015704048119,0.0000019323318076,0.675},
			new List<double>{-0.0017610562127539,0.0012701185634916,-0.0000015709238769,0.0000019331004067,0.695},
			new List<double>{-0.0018105498111601,0.0013412880691686,-0.0000015704962358,0.0000019324854785,0.746},
			new List<double>{-0.0022215907273459,0.0018913202829245,-0.0000015669447059,0.0000019277330177,0.766},
			new List<double>{-0.0019500669736450,0.0015367709455658,-0.0000015692907613,0.0000019307964416,0.786},
			new List<double>{-0.0017395987201257,0.0012701185634916,-0.0000015711092768,0.0000019331004067,0.806},
			new List<double>{-0.0015241519231996,0.0010028290435730,-0.0000015729708086,0.0000019354098768,0.826},
			new List<double>{-0.0013028125169482,0.0007349000995177,-0.0000015748832545,0.0000019377248718,0.846},
			new List<double>{-0.0011210535017199,0.0005200950155166,-0.0000015764537127,0.0000019395808590,0.871},
			new List<double>{-0.0009335584519317,0.0003048780487804,-0.0000015780737322,0.0000019414404050,0.896},
			new List<double>{-0.0007238306025283,0.0000712601611711,-0.0000015798858504,0.0000019434589410,0.996},
			new List<double>{-0.0009082062326932,0.0002563514599689,0.0000074474093761,-0.0000071188764479,9.999},
			};

			// convert the passed in values to the eng units we want for this table
			if (!ConvertEngUnits.ConvEngrUnits(ref dDensityInSPGrav, dDensity, EngineeringUnit.FmdSpGrav, bDensityUnits, 20.0))
				return false;

			if (!ConvertEngUnits.ConvEngrUnits(ref dTempInDegC, dMeasTemp, EngineeringUnit.FmtDegC, bTempUnits, 15.0))
				return false;

			// do range checks
			if (dDensityInSPGrav >= 2.0)
				return false;

			// calculate the hydrometer factor
			dTempValue = dTempInDegC - 20.0;
			dHydrometerTemp = 1 - 0.000023 * dTempValue - 0.00000002 * (dTempValue * dTempValue);

			for (iLoop = 0; iLoop < 21; iLoop++)
			{
				if (dDensityInSPGrav <= DensityTableData[iLoop][4])
				{
					dVCF = 1.0 + DensityTableData[iLoop][1] * dTempValue + DensityTableData[iLoop][3] * (dTempValue * dTempValue);
					dVCF = dVCF + (DensityTableData[iLoop][0] * dTempValue + DensityTableData[iLoop][2] * (dTempValue * dTempValue)) / dDensityInSPGrav;
					RoundDouble(dVCF, ref dVCF, 3, false, false);
					break;
				}
			}

			if (iLoop >= 21 || dVCF < 0.0)
			{
				piFlag = -1;
				return false;
			}
			piFlag = 0;

			pdVcfc = dVCF;

			return true;
		}

		/*		public bool CalcTankStdDensity(EngineeringUnit bStdDensityUnits, // Std Density Engr Units
												double dTemp,               // Measured Temperature
												EngineeringUnit bTempUnits,        // Temperature Engr Units
												byte byTempRoundingMethod,
												byte byVcfRoundingMethod,
												double dDensity,            // Measured Density
												EngineeringUnit bDensityUnits,     // Density Engr Units
												double dVolCorrFactor,  // Volume Correction Factor
												double dDensityPress,           // density pressure for api 2004
												EngineeringUnit bDensityPressUnits,    // density pressure units for api 2004
												double dAlternateTemperature,   // selected refined product sub catagory for api 2004
												double dBaseTemp,   // api 2004 alternate base temp reference
												double dAlternateBasePress, // api 2004 alternate base pressure reference
												ref double[] dK,
												ref double pdStdDensity,       // Standard Density Variable
												ref double dHydrometer)
		*/
		public override bool CalcTankStdDensity(EngineeringUnit bStdDensityUnits, // Std Density Engr Units
																double dTemp,               // Measured Temperature
																EngineeringUnit bTempUnits,        // Temperature Engr Units
																ETempRounding byTempRoundingMethod,
																EVcfRounding byVcfRoundingMethod,
																double dDensity,            // Measured Density
																EngineeringUnit bDensityUnits,     // Density Engr Units
																double dVolCorrFactor,  // Volume Correction Factor
																double dDensityPress,           // density pressure for api 2004
																EngineeringUnit bDensityPressUnits,    // density pressure units for api 2004
																double dAlternateTemperature,   // selected refined product sub catagory for api 2004
																double dBaseTemp,   // api 2004 alternate base temp reference
																double dAlternateBasePress, // api 2004 alternate base pressure reference
																ref double CTLReturn,
																ref double CPLReturn,
																ref double[] dK,
																ref double pdStdDensity,       // Standard Density Variable
																ref double dHydrometer)
		{
			double dDensityInSPGrav = 0.0;
			double dTempInDegC = 0.0;
			double dStdDensity = 0.0;
			double dHydrometerTemp = 0.0;
			double dTempValue = 0.0;
			int iLoop = 0;

			var DensityTableData = new List<List<double>>{
			new List<double>{-0.0045946489678832,0.0061232431795680,-0.0000317074831323,0.0000548397230037,0.498},
			new List<double>{-0.0044279278953793,0.0057882992380437,-0.0000263544808934,0.0000441694652077,0.518},
			new List<double>{-0.0042635157420811,0.0054649855357069,-0.0000263293867090,0.0000438862463121,0.539},
			new List<double>{-0.0039313336083154,0.0048491424883735,-0.0000171988380071,0.0000271198135388,0.559},
			new List<double>{-0.0035459928199061,0.0041555627486596,-0.0000174082405540,0.0000272052538293,0.579},
			new List<double>{-0.0044795785597695,0.0057678078599348,-0.0000384017053042,0.0000636945533674,0.6},
			new List<double>{-0.0024361018961719,0.0023329279647329,-0.0000015650912583,0.0000019239173808,0.615},
			new List<double>{-0.0022189302188432,0.0019797818956931,-0.0000015669676936,0.0000019269686800,0.635},
			new List<double>{-0.0019375650211732,0.0015367709455658,-0.0000015693987823,0.0000019307964416,0.655},
			new List<double>{-0.0018211308776796,0.0013590733713544,-0.0000015704048119,0.0000019323318076,0.675},
			new List<double>{-0.0017610562127539,0.0012701185634916,-0.0000015709238769,0.0000019331004067,0.695},
			new List<double>{-0.0018105498111601,0.0013412880691686,-0.0000015704962358,0.0000019324854785,0.746},
			new List<double>{-0.0022215907273459,0.0018913202829245,-0.0000015669447059,0.0000019277330177,0.766},
			new List<double>{-0.0019500669736450,0.0015367709455658,-0.0000015692907613,0.0000019307964416,0.786},
			new List<double>{-0.0017395987201257,0.0012701185634916,-0.0000015711092768,0.0000019331004067,0.806},
			new List<double>{-0.0015241519231996,0.0010028290435730,-0.0000015729708086,0.0000019354098768,0.826},
			new List<double>{-0.0013028125169482,0.0007349000995177,-0.0000015748832545,0.0000019377248718,0.846},
			new List<double>{-0.0011210535017199,0.0005200950155166,-0.0000015764537127,0.0000019395808590,0.871},
			new List<double>{-0.0009335584519317,0.0003048780487804,-0.0000015780737322,0.0000019414404050,0.896},
			new List<double>{-0.0007238306025283,0.0000712601611711,-0.0000015798858504,0.0000019434589410,0.996},
			new List<double>{-0.0009082062326932,0.0002563514599689,0.0000074474093761,-0.0000071188764479,9.999},
			};
			// convert the passed in values to the eng units we want for this table
			if (!ConvertEngUnits.ConvEngrUnits(ref dDensityInSPGrav, dDensity, EngineeringUnit.FmdSpGrav, bDensityUnits, 20.0))
				return false;

			if (!ConvertEngUnits.ConvEngrUnits(ref dTempInDegC, dTemp, EngineeringUnit.FmtDegC, bTempUnits, 15.0))
				return false;

			// do range checks
			if (dDensityInSPGrav >= 2.0)
				return false;

			// calculate the hydrometer factor
			dTempValue = dTempInDegC - 20.0;
			dHydrometerTemp = 1 - 0.000023 * dTempValue - 0.00000002 * (dTempValue * dTempValue);
			for (iLoop = 0; iLoop < 21; iLoop++)
			{
				dStdDensity = dDensityInSPGrav - DensityTableData[iLoop][0] * dTempValue - DensityTableData[iLoop][2] * (dTempValue * dTempValue);

				dStdDensity = dStdDensity / (1 + DensityTableData[iLoop][1] * dTempValue + DensityTableData[iLoop][3] * (dTempValue * dTempValue));

				dStdDensity = dStdDensity * dHydrometerTemp;
				if (dStdDensity <= DensityTableData[iLoop][4])
					break;
			}

			if (iLoop >= 21)
				return false;

			// do the rounding
			if (dStdDensity > 0.65)
				RoundDouble(dStdDensity, ref dStdDensity, 4, false, false);
			else
				RoundDouble(dStdDensity, ref dStdDensity, 3, false, false );

			if (!ConvertEngUnits.ConvEngrUnits(ref pdStdDensity, dStdDensity, bStdDensityUnits, EngineeringUnit.FmdSpGrav, 20.0))
				return false;

			dHydrometer = dHydrometerTemp;

			return true;
		}

		/*		public bool CalcTankDensity(EngineeringUnit bDensityUnits,       // Density Engineering Units
											double dTemp,                   // Current Temperature
											EngineeringUnit bTempUnits,            // Temperature Engineering Units
											byte byTempRoundingMethod,
											byte byVcfRoundingMethod,
											double dStdDensity,         // Product Standard Density
											EngineeringUnit bStdDensityUnits,  // Standard Density Engr Units
											double dVolCorrFactor,      // Volume Correction Factor
											double dDensityPress,           // density pressure for api 2004
											EngineeringUnit bDensityPressUnits,    // density pressure units for api 2004
											double dAlternateBaseTemp,  // api 2004 alternate base temp reference
											double dAlternateBasePress, // api 2004 alternate base pressure reference
											ref double dK,
											ref double pdDensity,              // Pointer to Density Variable
											ref double dHydrometer)
											*/
		public override bool CalcTankDensity(EngineeringUnit bDensityUnits,         // Density Engineering Units
											double dTemp,                   // Current Temperature
											EngineeringUnit bTempUnits,            // Temperature Engineering Units
											ETempRounding byTempRoundingMethod,
											EVcfRounding byVcfRoundingMethod,
											double dStdDensity,         // Product Standard Density
											EngineeringUnit bStdDensityUnits,  // Standard Density Engr Units
											double dVolCorrFactor,      // Volume Correction Factor
											double dDensityPress,           // density pressure for api 2004
											EngineeringUnit bDensityPressUnits,    // density pressure units for api 2004
											double dBaseTemp,   // api 2004 alternate base temp reference
											double dAlternateBasePress, // api 2004 alternate base pressure reference
											ref double CTLReturn,
											ref double CPLReturn,
											ref double[] dK,
											ref double pdDensity,              // Pointer to Density Variable
											ref double dHydrometer)
		{
			double dDensityInSPGrav = 0.0;
			double dTempInDegC = 0.0;
			double dDensity = 0.0;
			double dHydrometerTemp = 0.0;
			double dTempValue = 0.0;
			int iLoop = 0;

			var DensityTableData = new List<List<double>>{
			new List<double>{-0.0045946489678832,0.0061232431795680,-0.0000317074831323,0.0000548397230037,0.498},
			new List<double>{-0.0044279278953793,0.0057882992380437,-0.0000263544808934,0.0000441694652077,0.518},
			new List<double>{-0.0042635157420811,0.0054649855357069,-0.0000263293867090,0.0000438862463121,0.539},
			new List<double>{-0.0039313336083154,0.0048491424883735,-0.0000171988380071,0.0000271198135388,0.559},
			new List<double>{-0.0035459928199061,0.0041555627486596,-0.0000174082405540,0.0000272052538293,0.579},
			new List<double>{-0.0044795785597695,0.0057678078599348,-0.0000384017053042,0.0000636945533674,0.6},
			new List<double>{-0.0024361018961719,0.0023329279647329,-0.0000015650912583,0.0000019239173808,0.615},
			new List<double>{-0.0022189302188432,0.0019797818956931,-0.0000015669676936,0.0000019269686800,0.635},
			new List<double>{-0.0019375650211732,0.0015367709455658,-0.0000015693987823,0.0000019307964416,0.655},
			new List<double>{-0.0018211308776796,0.0013590733713544,-0.0000015704048119,0.0000019323318076,0.675},
			new List<double>{-0.0017610562127539,0.0012701185634916,-0.0000015709238769,0.0000019331004067,0.695},
			new List<double>{-0.0018105498111601,0.0013412880691686,-0.0000015704962358,0.0000019324854785,0.746},
			new List<double>{-0.0022215907273459,0.0018913202829245,-0.0000015669447059,0.0000019277330177,0.766},
			new List<double>{-0.0019500669736450,0.0015367709455658,-0.0000015692907613,0.0000019307964416,0.786},
			new List<double>{-0.0017395987201257,0.0012701185634916,-0.0000015711092768,0.0000019331004067,0.806},
			new List<double>{-0.0015241519231996,0.0010028290435730,-0.0000015729708086,0.0000019354098768,0.826},
			new List<double>{-0.0013028125169482,0.0007349000995177,-0.0000015748832545,0.0000019377248718,0.846},
			new List<double>{-0.0011210535017199,0.0005200950155166,-0.0000015764537127,0.0000019395808590,0.871},
			new List<double>{-0.0009335584519317,0.0003048780487804,-0.0000015780737322,0.0000019414404050,0.896},
			new List<double>{-0.0007238306025283,0.0000712601611711,-0.0000015798858504,0.0000019434589410,0.996},
			new List<double>{-0.0009082062326932,0.0002563514599689,0.0000074474093761,-0.0000071188764479,9.999},
			};
			// convert the passed in values to the eng units we want for this table
			if (!ConvertEngUnits.ConvEngrUnits(ref dDensityInSPGrav, dStdDensity, EngineeringUnit.FmdSpGrav, bStdDensityUnits, 20.0))
				return false;

			if (!ConvertEngUnits.ConvEngrUnits(ref dTempInDegC, dTemp, EngineeringUnit.FmtDegC, bTempUnits, 15.0))
				return false;

			// do range checks
			if (dDensityInSPGrav >= 2.0)
				return false;

			// calculate the hydrometer factor
			dTempValue = dTempInDegC - 20.0;
			dHydrometerTemp = 1 - 0.000023 * dTempValue - 0.00000002 * (dTempValue * dTempValue);

			for (iLoop = 0; iLoop < 21; iLoop++)
			{
				if (dDensityInSPGrav <= DensityTableData[iLoop][4])
				{
					dDensity = dDensityInSPGrav / dHydrometerTemp;
					dDensity = dDensity * (1 + DensityTableData[iLoop][1] * dTempValue + DensityTableData[iLoop][3] * (dTempValue * dTempValue));
					dDensity = dDensity + (DensityTableData[iLoop][0] * dTempValue) + (DensityTableData[iLoop][2] * (dTempValue * dTempValue));
					break;
				}
			}

			if (iLoop >= 21)
				return false;

			if (dDensityInSPGrav > 0.65)
				RoundDouble(dDensity, ref dDensity, 4, false, false);
			else
				RoundDouble(dDensity, ref dDensity, 3, false, false);

			if (!ConvertEngUnits.ConvEngrUnits(ref pdDensity, dDensity, bDensityUnits, EngineeringUnit.FmdSpGrav, 20.0))
				return false;

			dHydrometer = dHydrometerTemp;
			return true;
		}

	}
}
