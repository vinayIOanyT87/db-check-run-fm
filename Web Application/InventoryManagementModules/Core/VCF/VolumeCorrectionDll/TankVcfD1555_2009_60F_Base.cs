using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Varec.CommonComponents.EngineeringUnitsLibrary;

namespace VCF
{
	class TankVcfD1555_2009_60F_Base : TankBaseVcf
	{
		public TankVcfD1555_2009_60F_Base()
		{
			m_byCorrectionTypeMajor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_ASTM_D1555_F_2009;
			// m_byCorrectionTypeMinor is set in the derived class
			m_bUsesDensity  = false;
		}

		public override bool TemperatureCorr(double dDensity,
												double dMeasTemp,
												double dStdTempInC,
												double dStdTemp,
												EngineeringUnit bDensityUnits,
												EngineeringUnit bTempUnits,
												double dDensityPress,           // density pressure for api 2004
												EngineeringUnit bDensityPressUnits,    // density pressure units for api 2004
												double dAlternateTemperature,   // selected refined product sub catagory for api 2004
												double dBaseTemp,   // api 2004 alternate base temp reference
												double dAlternateBasePress, // api 2004 alternate base pressure reference
												ref double[] dK,
												ref double pdVcfc,
												ref int Iflag,
												ref double CTLReturn,
												ref double CPLReturn,
												bool RangeCk,
												bool bRound,
												bool bTable60,              //	Optional
												bool UseDensity)                //	Optional
		{
			double dDegF = 0;

			if (!ConvertEngUnits.ConvEngrUnits(ref dDegF, dMeasTemp, EngineeringUnit.FmtDegF, bTempUnits, 0))
				return false;

			//FMTRACE(_T("CTankD1555Vcf_2009_60F::TemperatureCorr"));
			D1555Correction(dDegF, ref pdVcfc, m_byCorrectionTypeMinor, ref Iflag);

			return true;
		}

		void D1555Correction(double dDegF,
							ref double pdVcfc,
                            Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor bVolCorecTypeMinor,
							ref int piFlag)
		{
			var VCF_Constants_Lookup_60F = new List<List<double>>{
			// benzene 0
			new List<double>{1.038382492,-6.2307E-4,-2.8505E-7,1.2692E-10,0},
			//Toluene 1
			new List<double>{1.035323647,-5.8887E-4,2.46508E-9,-7.2802E-12,0},
			//m-Xylene -a 2
			new List<double>{1.031887514,-5.2326E-4,-1.3253E-7,-7.35960E-11,0},
			//Styrene 3
			new List<double>{1.032227515,-5.3444E-4,-4.4323E-8,0,0},
			//o-Xylene 4
			new List<double>{1.031436449,-5.2302E-4,-2.5217E-9,-2.13840E-10,0},
			// p-Xylene 5
			new List<double>{1.032307000,-5.2815E-4,-1.8416E-7,1.89256E-10,0},
			//Cyclohexane 6
			new List<double>{1.039337296,-6.4728E-4,-1.4582E-7,1.03538E-10,0},
			//Ethylbenzene 7
			new List<double>{1.033346632,-5.5243E-4,8.37035E-10,-1.2692E-9,5.55061E-12},
			// Cumene 8
			new List<double>{1.03240114,-5.3445E-4,-9.5067E-8,3.6272E-11,0},
			// 300-350F 9
			new List<double>{1.031118000,-5.1827E-4,-3.5109E-9,-1.98360E-11,0},
			// 350-400F 10
			new List<double>{1.029099000,-4.8287E-4,-3.7692E-8,3.78575E-11,0},
			};
			double dTemp = 0.0;
			double dTempVCF;
			byte byPrecision = 1;

			// set the temperature in tenths.
			RoundDouble(dDegF, ref dTemp, byPrecision, false, false);

			piFlag = 0;
			pdVcfc = 1.0;
			// check the temperature based on the selected sub group
			if (bVolCorecTypeMinor == Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_300_AROMATIC ||
				bVolCorecTypeMinor == Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_CUMENE ||
				bVolCorecTypeMinor == Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_350_AROMATIC ||
				bVolCorecTypeMinor == Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_ETHYL_BENZENE ||
				bVolCorecTypeMinor == Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_M_XYLENE ||
				bVolCorecTypeMinor == Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_O_XYLENE)
			{
				if (dTemp < 5.0 ||
					dTemp > 140.0)
				{
					piFlag = -1;
				}
			}
			else if (bVolCorecTypeMinor == Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_P_XYLENE)
			{
				if (dTemp < 56.0 ||
					dTemp > 150.0)
				{
					piFlag = -1;
				}
			}
			else if (bVolCorecTypeMinor == Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_BENZENE)
			{
				if (dTemp < 43.0 ||
					dTemp > 140.0)
				{
					piFlag = -1;
				}
			}
			else if (bVolCorecTypeMinor == Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_TOLUENE)
			{
				if (dTemp < -5.0 ||
					dTemp > 140.0)
				{
					piFlag = -1;
				}
			}
			else if (bVolCorecTypeMinor == Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_STYRENE)
			{
				if (dTemp < 15.0 ||
					dTemp > 140.0)
				{
					piFlag = -1;
				}
			}
			else if (bVolCorecTypeMinor == Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_CYCLO_HEXANE)
			{
				if (dTemp < 44.0 ||
					dTemp > 140.0)
				{
					piFlag = -1;
				}
			}

			else // -5 to 140
			{
				piFlag = -1;
			}

			if (piFlag != -1)
			{
				// now get the vcf out of the array and do range checks
				int SelectArrayPos = (int)bVolCorecTypeMinor - 19;
				if(SelectArrayPos < 0 || SelectArrayPos > 10)
				{
					piFlag = -1;
					return;
				}
				// the calculation is a standard polynomial
				// VCF = VCFa + (VCFb  * Temp) + (VCFc * (Temp ^ 2)) + (VCFd  * (Temp ^ 3)) + (VCFe * (Temp ^ 4))
				//calculate the tempersture values
				double	T2 = dTemp * dTemp,
						T3 = dTemp * dTemp * dTemp,
						T4 = dTemp * dTemp * dTemp * dTemp;

				dTempVCF = VCF_Constants_Lookup_60F[SelectArrayPos][0] + 
							(VCF_Constants_Lookup_60F[SelectArrayPos][1] * dTemp) + 
							(VCF_Constants_Lookup_60F[SelectArrayPos][2] * T2) + 
							(VCF_Constants_Lookup_60F[SelectArrayPos][3] * T3) + 
							(VCF_Constants_Lookup_60F[SelectArrayPos][4] * T4);
				// set to 5 decimal points
				byPrecision = 5;
				RoundDouble(dTempVCF, ref dTempVCF, byPrecision, false, false);
				pdVcfc = dTempVCF;
			}

			if (pdVcfc <= 0.0)
				piFlag = -1;
		}
	}
}
