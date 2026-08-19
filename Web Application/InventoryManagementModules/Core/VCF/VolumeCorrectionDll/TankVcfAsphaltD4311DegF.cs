using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Varec.CommonComponents.EngineeringUnitsLibrary;
namespace VCF
{
    public class TankVcfAsphaltD4311DegF : TankBaseVcf
    {
        protected const double MAX_D4311_APIF = 34.9;
        protected const double MIN_D4311_TEMPF = 0.0;
        protected const double MAX_D4311_TEMPF = 500.0;
        protected const double API_SPLIT_VALUE = 15.0;
        public TankVcfAsphaltD4311DegF()
{
	m_byCorrectionTypeMajor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_ASPHALT;
	m_byCorrectionTypeMinor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_D4311DEGF_2004;
	m_bUsesDensity  = false;
}

        public override bool TemperatureCorr(double dDensity,
                                            double dMeasTemp,
                                            double dStdTempInC,
                                            double dStdTemp,
                                            EngineeringUnit bDensityUnits,
                                            EngineeringUnit bTempUnits,
                                            double dDensityPress,			// density pressure for api 2004
                                            EngineeringUnit bDensityPressUnits,	// density pressure units for api 2004
                                            double dAlternateTemperature,	// selected refined product sub catagory for api 2004
                                            double dBaseTemp,	// api 2004 alternate base temp reference
                                            double dAlternateBasePress,	// api 2004 alternate base pressure reference
                                            ref double[] dK,
                                            ref double pdVcfc,
                                            ref int piFlag,
												ref double CTLReturn,
												ref double CPLReturn,
											bool RangeCk,
                                            bool bRound,
                                            bool bTable60,				//	Optional
                                            bool UseDensity)                //	Optional
        {
        double TemperatureInF = 0.0;
        double StdDensityInapi = 0.0;
        // this is where the vcf actually gets calculated

        // calculations are based on std density so if density is selected fail the calculation
        if (UseDensity)
        {
                piFlag = SET_APICORR_ALARM;
            return false;
        }

        // convert the density to api
        if (!ConvertEngUnits.ConvEngrUnits(ref StdDensityInapi, dDensity, EngineeringUnit.FmdDegApi, bDensityUnits, 0))
        {
                piFlag = SET_APICORR_ALARM;
            return false;
        }

        // do range check on density
        if (StdDensityInapi > MAX_D4311_APIF)
        {
                piFlag = SET_APICORR_ALARM;
            return false;
        }

        // convert the temperature to F
        if (!ConvertEngUnits.ConvEngrUnits(ref TemperatureInF, dMeasTemp, EngineeringUnit.FmtDegF, bTempUnits, 0))
        {
                piFlag = SET_APICORR_ALARM;
            return false;
        }
        // do range check on temperature
        if (TemperatureInF < MIN_D4311_TEMPF ||
            TemperatureInF > MAX_D4311_TEMPF)
        {
                piFlag = SET_APICORR_ALARM;
            return false;
        }

        // calculate the vcf
        if (StdDensityInapi >= API_SPLIT_VALUE) // table b
        {
            pdVcfc = 1.02413769 - 4.0641418E-4 * TemperatureInF + 6.79176E-8 * TemperatureInF * TemperatureInF;
        }
        else    // table a
        {
            pdVcfc = 1.0211326242 - (3.548988118E-4 * TemperatureInF) + 4.49881E-8 * (TemperatureInF * TemperatureInF);
        }

        return true;
    }

}
}
