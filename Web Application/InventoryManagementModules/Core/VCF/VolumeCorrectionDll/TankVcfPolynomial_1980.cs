using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCF
{
	class TankVcfPolynomial_1980 : TankVcfPolynomial
	{
		public TankVcfPolynomial_1980()
		{
			m_byCorrectionTypeMajor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_POLYNOMIAL_F;
			m_byCorrectionTypeMinor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_POLYNOMIAL;
			m_bUsesDensity = false;
			m_bStandardCalculationType = EApiCalc.API_CALC_STANDARD;
		}
	}
}
