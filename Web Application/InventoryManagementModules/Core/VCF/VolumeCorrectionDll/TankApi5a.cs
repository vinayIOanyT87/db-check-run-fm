using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using Varec.CommonComponents.EngineeringUnitsLibrary;

namespace VCF
{
    public class TankApi5a : TankApi6A
    {
        public TankApi5a()
        {
            m_byCorrectionTypeMajor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_F;
            m_byCorrectionTypeMinor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API6A;
            m_bUsesDensity = true;
            m_bStandardCalculationType = EApiCalc.API_CALC_STANDARD;
        }

        public override bool TemperatureCorr(double dDensity,
                                           double dMeasTemp,
                                           double dStdTempInC,
                                           double dStdTemp,
                                           EngineeringUnit bDensityUnits,
                                           EngineeringUnit bTempUnits,
                                           double dDensityPress,            // density pressure for api 2004
                                           EngineeringUnit bDensityPressUnits,   // density pressure units for api 2004
                                           double dAlternateTemperature,    // selected refined product sub catagory for api 2004
                                           double dBaseTemp,    // api 2004 alternate base temp reference
                                           double dAlternateBasePress,  // api 2004 alternate base pressure reference
                                           ref double[] dK,
                                           ref double pdVcfc,
                                           ref int piFlag,
												ref double CTLReturn,
												ref double CPLReturn,
										   bool RangeCk,
                                           bool bRound,
                                           bool bTable60,               //	Optional
                                           bool UseDensity)				//	Optional
        {
            double dApi = 0;
            double dDegF = 0;

            //FMTRACE(_T("CTankApi5a::TemperatureCorr"));
            if (!ConvertEngUnits.ConvEngrUnits(ref dApi, dDensity, EngineeringUnit.FmdDegApi, bDensityUnits, dStdTempInC)
                || !ConvertEngUnits.ConvEngrUnits(ref dDegF, dMeasTemp, EngineeringUnit.FmtDegF, bTempUnits, 0))
                return false;

            if (UseDensity)
                APICorrection(dApi, dDegF, ref pdVcfc, ref piFlag, bRound);
            else
            {
                base.APICorrection(dApi, dDegF, ref pdVcfc, ref piFlag, true, bRound);
            }

            return true;
        }

        /****************************************************************************

            Function 		: 	APICorrection()

            Parameters		:	dApi		- input: density in API gravity units at temp DegF
                                    dDegF		- input: observed temp in F
                                    pdVcfc	- output: volume corec factor for cacluations
                                    piFlag	- output: 0=>normal,1=>extrap region, -1=>out of range

            Description		:	API TABLE 5A VOLUME CORRECTION CALCULATION
                                    Range of Application:
                                                Std Density in API		Temp in F
                                                0 - 40						0.0 - 300.0
                                                40.1 - 50.0 				0.0 - 250.0
                                                50.1 - 100.0				0.0 - 200.0

            Return Codes	:	none

        /****************************************************************************/
        void APICorrection(double dApi,
                                       double dDegF,
                                                 ref double pdVcfc,
                                                 ref int piFlag,
                                                 bool bRound)
        {
            double dVcfc = 0.00, dStdDen1 = 0.00, dStdDen2 = 0.00, dDen = 0.00;
            int iFlag;
            bool RangeCk;
            int i;

            // round input vars
            if (bRound)
            {
                RoundDouble(dApi, ref dApi, 1, false, false);
                RoundDouble(dDegF, ref dDegF, 1, false, false);
            }

            // convert API to density in kg/m3 and start StdDen=Density
            RHO(dApi, ref dDen);

            // apply hydrometer correction
            double dHydrometer = 0.00;
            ApplyHydroCorrection(ref dDen, dDegF, Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_F, Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API6A, false, ref dHydrometer);

            // round density value to nearest 0.01
            if (bRound)
            {
                RoundDouble(dDen, ref dDen, 2, false, false);
            }

            // start with neasured denisty.
            dStdDen1 = dDen;
            dApi = (141360.2 / (dStdDen1) - 131.5);
            if (bRound)
            {
                RoundDouble(dApi, ref dApi, 1, false, false);
            }

            for (i = MAX_ITERATIONS, RangeCk = true, iFlag = 0; (--i != 0) && (iFlag != -1); RangeCk = false)
            {
                base.APICorrection(dApi, dDegF, ref dVcfc, ref iFlag, RangeCk, false);   // call API6A - no rounding required (already done)
                if (Math.Abs((dStdDen2 = dDen / dVcfc) - dStdDen1) <= MAX_DENSITY_DIFF)
                    break;
                else
                {
                    dApi = (141360.2 / (dStdDen1 = dStdDen2)) - 131.5;
                }
                if (bRound)
                {
                    RoundDouble(dApi, ref dApi, 1, false, false);
                }
            }
            piFlag = iFlag;
            pdVcfc = dVcfc;

        } // End of APICorrection()

    }
}
