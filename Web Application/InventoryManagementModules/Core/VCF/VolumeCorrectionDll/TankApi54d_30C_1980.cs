using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCF
{
	class TankApi54d_30C_1980 : TankApi54d_30C
	{
		public TankApi54d_30C_1980()
		{
			m_byCorrectionTypeMajor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_C_1980;
			m_byCorrectionTypeMinor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54D_30;
			m_bStandardCalculationType = EApiCalc.API_CALC_STANDARD;
		}
	}
}
