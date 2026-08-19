using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Varec.CommonComponents.EngineeringUnitsLibrary;

namespace VCF
{
	class TankApi54d_30C : TankApi54d
	{
		public TankApi54d_30C()
		{
			m_byCorrectionTypeMajor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_C;
			m_byCorrectionTypeMinor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54D_30;
			// m_bUsesDensity set in CTankApi54d
			m_dTable54ReferenceTemperature = TAB54_30_REF_TEMP;
			m_bStandardCalculationType = EApiCalc.API_CALC_STANDARD;
		}
	}
}
