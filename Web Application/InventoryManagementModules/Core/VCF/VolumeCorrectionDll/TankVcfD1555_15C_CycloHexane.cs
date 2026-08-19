using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCF
{
	class TankVcfD1555_15C_CycloHexane : TankVcfD1555_15C_Base
	{
		public TankVcfD1555_15C_CycloHexane()
		{
			m_byCorrectionTypeMajor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_ASTM_D1555_C_2004;
			m_byCorrectionTypeMinor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_CYCLO_HEXANE;
			m_bStandardCalculationType = EApiCalc.API_CALC_STANDARD;
		}
	}
}
