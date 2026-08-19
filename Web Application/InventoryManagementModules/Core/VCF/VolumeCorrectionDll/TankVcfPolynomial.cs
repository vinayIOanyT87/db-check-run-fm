using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Varec.CommonComponents.EngineeringUnitsLibrary;

namespace VCF
{

	class TankVcfPolynomial : TankBaseVcf
	{
		public TankVcfPolynomial()
		{
			m_byCorrectionTypeMajor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_POLYNOMIAL_F;
			m_byCorrectionTypeMinor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_POLYNOMIAL;
			m_bUsesDensity  = false;
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

			//FMTRACE(_T("CTankPolynomialVcf::TemperatureCorr"));
			PolynomialCorrection(ref pdVcfc, dMeasTemp, dStdTemp, ref dK);

			return true;
		}

		public void PolynomialCorrection(	ref double pdVcf,
											double dMeasTemp,
											double dStdTemp,
											ref double[] dKfactors)
		{
			double dVcf, dDeltaTemp;
			double[] dK;
			int i;

			// Calculate Deviation from Standard Temperature
			dDeltaTemp = dMeasTemp - dStdTemp;
			dK = dKfactors;

			// Uses Horner's Rule to calc a polynomial
			for (dVcf = dK[4], i = 3; i >= 0; --i)
				dVcf = dVcf * dDeltaTemp + dK[i];

			// Do not allow a Vcf of 0
			if (dVcf <= 0)
				dVcf = 1;
			pdVcf = dVcf;
			return;
		}

	}
}
