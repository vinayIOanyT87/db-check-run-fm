using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Varec.CommonComponents.EngineeringUnitsLibrary;
namespace VCF
{
    public class TankApi54a_30C : TankApi54a
    {
        public TankApi54a_30C()
        {
            m_byCorrectionTypeMajor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_C;
            m_byCorrectionTypeMinor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54A_30;
            m_bUsesDensity = false;
            m_dTable54ReferenceTemperature = TAB54_30_REF_TEMP;
            m_bStandardCalculationType = EApiCalc.API_CALC_STANDARD;
        }
    }
}
