using System;
using System.Runtime.InteropServices;
using EngineeringUnitsLibrary;
using FMBusinessObjects.DataObjects;

namespace VolumeCorrectionDotNet
{
	/// <summary>
	/// Summary description for VolumeCorrection.
	/// </summary>
	public abstract class VolumeCorrection
	{
        [DllImport("VolumeCorrection.dll")]
        internal static extern Boolean CalculateVcf
        (
            byte bVolCorecTypeMajor,
            byte bVolCorecTypeMinor,
             double dMeasTemp,				// Measured Temperature
            double dStdTemp,				// Standard Temperature
            byte bStdTempUnits, 		// Standard Temp. Engineering Units
            byte bTempUnits, 			// Temp. Engineering Units
            byte byTempRoundingMethod,
            byte byVcfRoundingMethod,
            double dMeasDensity,			// Measured Density
            byte bDensUnits, 			// Density Engineering Units
            double dStdDensity,			// Standard Product Density
            byte bStdDensUnits,			// Std. density Engineering Units
            Boolean bUseDensity, 			// Use Measured Density in Calc
            double dDensityPress,			// density pressure for api 2004
            byte bDensityPressUnits,	// density pressure units for api 2004
            double dAlternateTemperature,	// alternate temperature for api 2004
            double dAlternateBaseTemp,	// api 2004 alternate base temp reference
            double dAlternateBasePress,	// api 2004 alternate base pressure reference
            double [] pdKfactors, 			// Pointer to K Factors Array
            out double dUnRoundedVcf,
            out double pdVcf						// Pnt to Volume Correction Factor (rounded)
        );
        
 		public VolumeCorrection()
		{
			
		}

		static public double CalculateVCF(double measuredTemperature, double measuredPressure, double standardDensity,
			SiteClass site, ProductClass product)
		{
			return CalculateVCF(
				product._MajorCorrectionMethod,
				(byte) product._MinorCorrectionMethod,
				measuredTemperature,
				site.TemperatureUnits,
				product._StandardTemperature.Value,
				product._StandardTemperature.Units,
				standardDensity,
				site.DensityUnits,
				measuredPressure,
				site.PressureUnits,
				product._AlternateTemperature.Value,
				product._AlternatePressure.Value,
				product.CorrectionFactor.CorrectionFactorData);
		}

		static public double CalculateVCF(
			MAJOR_CORRECTION_TYPE majorMethod,
			byte minorMethod,
			double measuredTemperature,
			ENGINEERING_UNIT measuredTemperatureUnits,
			double standardTemperature,
			ENGINEERING_UNIT standardTemperatureUnits,
			double standardDensity,
			ENGINEERING_UNIT standardDensityUnits,
			double measuredPressure,
			ENGINEERING_UNIT pressureUnits,
			double alternateTemperature,
			double alternatePressure,
			double [] kFactors)
		{
			bool useDensity = false;
			byte byTempRoundingMethod = 0; 
			byte byVcfRoundingMethod = 0;
			double measuredDensity = 0.0;
			byte measuredDensityUnits=(byte) standardDensityUnits;

			double roundedVcf;
			double unroundedVcf;

            bool result=CalculateVcf(
                (byte) majorMethod,
                (byte) minorMethod,
                measuredTemperature,
                standardTemperature,
				(byte)standardTemperatureUnits,
				(byte)measuredTemperatureUnits,
				byTempRoundingMethod,
                byVcfRoundingMethod,
                measuredDensity,
                (byte) measuredDensityUnits,
                standardDensity,
                (byte) standardDensityUnits,
                useDensity,
                measuredPressure,
                (byte) pressureUnits,
                alternateTemperature,
                standardTemperature,
                alternatePressure,
                kFactors,
                out unroundedVcf,
                out roundedVcf);

			return roundedVcf;
		}
	}
}
