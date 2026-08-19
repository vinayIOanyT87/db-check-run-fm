using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCF
{
	class TankVcfD1555_2009_60F_Toluene : TankVcfD1555_2009_60F_Base
	{
		public TankVcfD1555_2009_60F_Toluene()
		{
			m_byCorrectionTypeMajor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_ASTM_D1555_F_2009;
			m_byCorrectionTypeMinor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_TOLUENE;
			m_bStandardCalculationType = EApiCalc.API_CALC_STANDARD;
		}
	}
}
