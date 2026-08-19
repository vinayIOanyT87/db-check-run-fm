using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Varec.CommonComponents.EngineeringUnitsLibrary;
namespace VCF
{
    public class TankVcfAsphaltD4311DegF_2009 : TankBaseVcf
    {
        protected const double MAX_D4311_APIF = 34.9;
        protected const double MIN_D4311_TEMPF = 0.0;
        protected const double MAX_D4311_TEMPF = 500.0;

        protected const double API_SPLIT_VALUE = 15.0;

        public TankVcfAsphaltD4311DegF_2009()
        {
            m_byCorrectionTypeMajor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_ASPHALT;
            m_byCorrectionTypeMinor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_D4311DEGF_2009;
            m_bUsesDensity = false;
        }

        public override bool TemperatureCorr(double dDensity,
                                            double dMeasTemp,
                                            double dStdTempInC,
                                            double dStdTemp,
                                            EngineeringUnit bDensityUnits,
                                            EngineeringUnit bTempUnits,
                                            double dDensityPress,          // density pressure for api 2004
                                            EngineeringUnit bDensityPressUnits, // density pressure units for api 2004
                                            double dAlternateTemperature,  // selected refined product sub catagory for api 2004
                                            double dBaseTemp,  // api 2004 alternate base temp reference
                                            double dAlternateBasePress,    // api 2004 alternate base pressure reference
                                            ref double[] dK,
                                            ref double pdVcfc,
                                            ref int piFlag,
												ref double CTLReturn,
												ref double CPLReturn,
											bool RangeCk,
                                            bool bRound,
                                            bool bTable60,             //	Optional
                                            bool UseDensity)                //	Optional
        {
            double TemperatureInF = 0.0;
            double StdDensityInAPI = 0.0;
            bool bUseACalculation = true;
            double dTempVCF = 1.0;
            // this is where the vcf actually gets calculated

            // calculations are based on std density so if density is selected fail the calculation
            if (UseDensity)
            {
                piFlag = SET_APICORR_ALARM;
                return false;
            }

            // convert the density to kg/m3
            if (!ConvertEngUnits.ConvEngrUnits(ref StdDensityInAPI, dDensity, EngineeringUnit.FmdDegApi, bDensityUnits, 0))
            {
                piFlag = SET_APICORR_ALARM;
                return false;
            }

            // do range check on density
            if (StdDensityInAPI < 0 ||
                StdDensityInAPI > MAX_D4311_APIF)
            {
                piFlag = SET_APICORR_ALARM;
                return false;
            }

            // determine which calculation to use
            // use a when std density is 966 kg/m3 or higher
            // use b when std density is 850 to 965 kg/m3


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

            if (StdDensityInAPI >= 15.0 &&
                StdDensityInAPI <= 34.9)
            {
                bUseACalculation = false;
            }

            if (bUseACalculation)
            {
                dTempVCF = 1.0211 - (3.5490E-4 * TemperatureInF) + (4.4988E-8 * (TemperatureInF * TemperatureInF));
            }
            else
            {
                dTempVCF = 1.0241 - (4.0641E-4 * TemperatureInF) + (6.7918E-8 * (TemperatureInF * TemperatureInF));
            }

            // this calculation seems to be off just a hair from most of the table so add 0.00004 to force a round up
            dTempVCF += 0.00004;
            // round to 4 decimal places
            RoundDouble(dTempVCF, ref dTempVCF, 4, false, false);

            pdVcfc = dTempVCF;

            return true;
        }

    }
}
