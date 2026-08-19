using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCF
{
	class TankVcfD1555_15C_Benzene_1980 : TankVcfD1555_15C_Benzene
	{
		public TankVcfD1555_15C_Benzene_1980()
		{
			m_byCorrectionTypeMajor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_ASTM_D1555_C_1980;
			m_byCorrectionTypeMinor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_BENZENE;
			m_bStandardCalculationType = EApiCalc.API_CALC_STANDARD;
		}
	}
}
