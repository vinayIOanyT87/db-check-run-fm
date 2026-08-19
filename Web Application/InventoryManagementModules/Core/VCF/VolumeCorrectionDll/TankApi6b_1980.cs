using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Varec.CommonComponents.EngineeringUnitsLibrary;


namespace VCF
{
    public class TankApi6b_1980 : TankApi6b
    {
        public TankApi6b_1980()
        {
            m_byCorrectionTypeMajor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_F_1980;
            m_byCorrectionTypeMinor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API6B;
            m_bUseApi1980 = true;
            m_bStandardCalculationType = EApiCalc.API_CALC_STANDARD;
        }
    }
}
