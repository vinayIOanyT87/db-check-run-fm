using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Varec.CommonComponents.EngineeringUnitsLibrary;
namespace VCF
{
    public class TankVcfAsphaltD4311DegC_2009 : TankBaseVcf
    {
        protected const double MIN_D4311_DENSITYC = 850.0;
        protected const double MIN_D4311_TEMPC = -25.0;
        protected const double MAX_D4311_TEMPC = 274.5;
        protected const double DENSITY_SPLIT_VALUE = 966.0;

        public TankVcfAsphaltD4311DegC_2009()
        {
            m_byCorrectionTypeMajor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_ASPHALT;
            m_byCorrectionTypeMinor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_D4311DEGC_2009;
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
            double TemperatureInC = 0.0;
            double StdDensityInkgM3 = 0.0;
            double dVariableA = 0.0;
            double dVariableB = 0.0;
            double dVariableC = 0.0;
            double dTempVCF = 0.0;
            double dOffSet = 0.0;
            // this is where the vcf actually gets calculated

            // calculations are based on std density so if density is selected fail the calculation
            if (UseDensity)
            {
                piFlag = SET_APICORR_ALARM;
                return false;
            }

            // convert the density to kg per m3
            if (!ConvertEngUnits.ConvEngrUnits(ref StdDensityInkgM3, dDensity, EngineeringUnit.FmdKgM3, bDensityUnits, 60.0))
            {
                piFlag = SET_APICORR_ALARM;
                return false;
            }

            // do range check on density
            if (StdDensityInkgM3 < MIN_D4311_DENSITYC)
            {
                piFlag = SET_APICORR_ALARM;
                return false;
            }

            // convert the temperature to C
            if (!ConvertEngUnits.ConvEngrUnits(ref TemperatureInC, dMeasTemp, EngineeringUnit.FmtDegC, bTempUnits, 0))
            {
                piFlag = SET_APICORR_ALARM;
                return false;
            }
            // do range check on temperature
            if (TemperatureInC < MIN_D4311_TEMPC ||
                TemperatureInC > MAX_D4311_TEMPC)
            {
                piFlag = SET_APICORR_ALARM;
                return false;
            }

            // calculate the vcf
            if (StdDensityInkgM3 >= DENSITY_SPLIT_VALUE)
            {
                dVariableA = 1.009;
                dVariableB = 6.3341E-4;
                dVariableC = 1.4571E-7;
                dOffSet = 0.0004;
                if (TemperatureInC < 10.0)
                    dOffSet += 0.0001;
            }
            else
            {
                dVariableA = 1.0108;
                dVariableB = 7.2344E-4;
                dVariableC = 2.199E-7;
                dOffSet = 0.0;
            }

            dTempVCF = (dVariableA - (dVariableB * TemperatureInC)) + (dVariableC * (TemperatureInC * TemperatureInC));

            dTempVCF += dOffSet;

            // round to 4 decimal places
            RoundDouble(dTempVCF, ref dTempVCF, 4, false, false);

            pdVcfc = dTempVCF;

            return true;
        }

    }
}
