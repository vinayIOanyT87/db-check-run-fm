using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Varec.CommonComponents.EngineeringUnitsLibrary;

namespace VCF
{
	class TankVcfLpg : TankBaseVcf
	{
		protected double m_dTable54ReferenceTemperature;
		public TankVcfLpg()
		{
			m_byCorrectionTypeMajor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_LPG_C;
			m_byCorrectionTypeMinor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_LPG;
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
											ref int Iflag,
												ref double CTLReturn,
												ref double CPLReturn,
											bool RangeCk,
											bool bRound,
											bool bTable60,              //	Optional
											bool UseDensity)                //	Optional
		{
			double dKgPerM3 = 0;
			double dDegC = 0;

			if (!ConvertEngUnits.ConvEngrUnits(ref dKgPerM3, dDensity, EngineeringUnit.FmdKgM3, bDensityUnits, dStdTempInC)
				|| !ConvertEngUnits.ConvEngrUnits(ref dDegC, dMeasTemp, EngineeringUnit.FmtDegC, bTempUnits, 0))
				return false;

			LpgCorrection(dKgPerM3, dDegC, ref pdVcfc, ref Iflag);

			return true;
		}

		public void LpgCorrection(double dDen15,
									double dDegC,
									ref double pdVcfc,
									ref int piFlag)
		{
			double x,
						x2,
						x3,
						x4,
						y1,
						y2,
						tr,
						tt,
						v0,
						vd,
						v1,
						v2,
						vcf;



			pdVcfc = -1.0;
			piFlag = -1;

			// Range check Density and Temperature;
			if (dDen15 < 500.0 || dDen15 > 640.0)
				return;
			if (dDegC < -110.0 || dDegC > 60.0)
				return;

			// Now for the  calculations , these are not nice!
			// Don't ask me what all these parameters mean

			x = (dDen15 - 500.0f) / 25.0f;
			x2 = x * x;
			x3 = x * x * x;
			x4 = x * x * x * x;
			y1 = 0.296f - 0.2395f * x + 0.2449167f * x2 - 0.105f * x3 + 0.01658334f * x4;
			y2 = 368.8f + 4.924927f * x + 13.66258f * x2 - 6.375f * x3 + 1.087503f * x4;

			if (y2 != 0.0f)   // avoid divide by zero errors
			{
				tr = 288.2f / y2;
				tt = (double)Math.Pow((1.0 - tr), 1.0 / 3.0);
				v0 = 1.0f - 1.52816f * tt + 1.43907f * tt * tt - 0.81446f * tt * tt * tt + 0.190454f * tt * tt * tt * tt;
				vd = (-0.296123f + 0.386914f * tr - 0.0427258f * tr * tr - 0.0480645f * tr * tr * tr) / (tr - 1.00001f);
				v1 = v0 * (1.0f - y1 * vd);
				tr = (dDegC + 273.2f) / y2;
				tt = (double)Math.Pow((1.0 - tr), 1.0 / 3.0);
				v0 = 1.0f - 1.52816f * tt + 1.43907f * tt * tt - 0.81446f * tt * tt * tt + 0.190454f * tt * tt * tt * tt;
				vd = (-0.296123f + 0.386914f * tr - 0.0427258f * tr * tr - 0.0480645f * tr * tr * tr) / (tr - 1.00001f);
				v2 = v0 * (1.0f - y1 * vd);

				if (v2 != 0.0f)  // avoid divide by zero errors
				{
					vcf = v1 / v2;
					v1 = Math.Floor((vcf + 0.000005f) * 100000.0f);
					vcf = v1 / 100000.0f;

					pdVcfc = vcf;
					piFlag = 0;

				}
			}
		}
	}
}