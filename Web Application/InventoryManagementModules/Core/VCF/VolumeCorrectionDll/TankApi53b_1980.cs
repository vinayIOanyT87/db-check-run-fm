using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Varec.CommonComponents.EngineeringUnitsLibrary;
namespace VCF
{
    public class TankApi53b_1980 : TankApi53b
    {
        public TankApi53b_1980()
        {
            m_byCorrectionTypeMajor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_C_1980;
            m_byCorrectionTypeMinor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54B;
            m_bUseApi1980 = true;
            m_bStandardCalculationType = EApiCalc.API_CALC_STANDARD;
        }
    }
}
