using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCF
{
	class TankVcfLpg_1980 : TankVcfLpg
	{
		public TankVcfLpg_1980()
		{
			m_byCorrectionTypeMajor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_LPG_C;
			m_byCorrectionTypeMinor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_LPG;
			m_bUsesDensity = false;
			m_dTable54ReferenceTemperature = TAB54_DEF_REF_TEMP;
			m_bStandardCalculationType = EApiCalc.API_CALC_STANDARD;
		}
	}
}
