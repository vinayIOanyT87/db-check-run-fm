using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCF
{
	class TankVcfD1555_15C_300Aromatic_1980 : TankVcfD1555_15C_300Aromatic
	{
		public TankVcfD1555_15C_300Aromatic_1980()
		{
			m_byCorrectionTypeMajor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_ASTM_D1555_C_1980;
			m_byCorrectionTypeMinor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_300_AROMATIC;
			m_bStandardCalculationType = EApiCalc.API_CALC_STANDARD;
		}
	}
}
