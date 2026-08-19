namespace VCF
{
	using System;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;
	using Varec.CommonComponents.EngineeringUnitsLibrary;
	using FMPointCommon;
	using Opc.Ua;

	public class FMVcf : FuelsManagerModule, IFuelsManagerModule
	{

		public VcfModuleSettings VcfSettings { get; set; }

		public FMVcf() : base() { }

		public bool? VcfCalculation(PointTag ProdTemperature,
									PointTag DensityTemperature,
									PointTag VaporTemperature,
									PointTag StdDensity,
									PointTag Density,
									PointTag VaporPress,
									PointTag ctl,
									PointTag cpl,
									PointTag ctpl,
									PointTag returnedvcf,
									PointTag vcfUnrounded,
									PointTag apicorrerror,
									PointTag DensityInAir,
									PointTag StdDensityInAir,
									PointTag DensityGauge)
		{
			bool usingGaugedDensity = false;
			_ = VaporTemperature; // Silence code analysis warning

			Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor bVolCorecTypeMajor = VcfSettings.CorrectionMethodType;
			Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor bVolCorecTypeMinor = VcfSettings.CorrectionMethodSpecific;

			// in the past we determined which to calculate density or std density based on which one is calculated
			// for gauge density both of these will be calculated and gauge density will be opc or manual
			// gauge density is used in conjunction with density temp. We calculate a gauge density
			// and then use this gauge density to calculate a standard density which in turn then is used
			// to calculate product density.
			// to make sure we fall through all of the calculations we are going to set a vlue called usinggaugeddensity = true;
			if (Density.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated &&
				 StdDensity.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated &&
				(DensityGauge.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
				DensityGauge.OpcStatusSubCode == StatusCodes.GoodLocalOverride))
			{
				usingGaugedDensity = true;
			}

			uint OpcStatusCodeBits;
			// if no vcf calculation then set variables and return
			if (bVolCorecTypeMajor == Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_NONE ||
			 bVolCorecTypeMajor == Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_NONE_1980)
			{
				DateTimeOffset CurrentDateTime = DateTimeOffset.UtcNow;
				// set density equal to the other
				// check if gauge density is being used
				if (usingGaugedDensity == true)
				{
					SetDensityValueandStatus(DensityGauge, ref StdDensity);
					SetDensityValueandStatus(DensityGauge, ref Density);
					 CurrentDateTime = DensityGauge.ServerTimeStamp;

            }
				else
				{
					if (Density.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated &&
						 StdDensity.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated &&
						 StdDensity.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
					{
						SetDensityValueandStatus(Density, ref StdDensity);
                  CurrentDateTime = Density.ServerTimeStamp;
               }
					if (StdDensity.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated &&
						 Density.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated &&
						 Density.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
					{
						SetDensityValueandStatus(StdDensity, ref Density);
                  CurrentDateTime = StdDensity.ServerTimeStamp;
               }
				}

				// set vcf at 1.00 and api correction error flag to false

				if (cpl.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated &&
				cpl.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
				{
					OpcStatusCodeBits = new StatusCode((uint)cpl.Status).CodeBits;
					if (cpl.Value != null ||
					  OpcStatusCodeBits != StatusCodes.BadNotImplemented)
					{
						cpl.Value = null;
						cpl.Status = StatusCodes.BadNotImplemented;
						cpl.ServerTimeStamp = CurrentDateTime;
						cpl.SourceTimeStamp = CurrentDateTime;
					}
				}

				if (ctl.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated &&
				ctl.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
				{
					OpcStatusCodeBits = new StatusCode((uint)ctl.Status).CodeBits;
					if (ctl.Value == null ||
						(double)ctl.Value != 1.0 ||
						OpcStatusCodeBits != StatusCodes.Good)
					{
						ctl.Value = 1.0;
						ctl.Status = StatusCodes.Good;
						ctl.ServerTimeStamp = CurrentDateTime;
						ctl.SourceTimeStamp = CurrentDateTime;
					}
				}

				if (ctpl.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated &&
				ctpl.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
				{
					OpcStatusCodeBits = new StatusCode((uint)ctpl.Status).CodeBits;
					if (ctpl.Value == null ||
							(double)ctpl.Value != 1.0 ||
							OpcStatusCodeBits != StatusCodes.Good)
					{
						ctpl.Value = 1.0;
						ctpl.Status = StatusCodes.Good;
						ctpl.ServerTimeStamp = CurrentDateTime;
						ctpl.SourceTimeStamp = CurrentDateTime;
					}
				}

				if (returnedvcf.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated &&
				returnedvcf.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
				{
					OpcStatusCodeBits = new StatusCode((uint)returnedvcf.Status).CodeBits;
					if (returnedvcf.Value == null ||
							(double)returnedvcf.Value != 1.0 ||
							OpcStatusCodeBits != StatusCodes.Good)
					{
						returnedvcf.Value = 1.0;
						returnedvcf.Status = StatusCodes.Good;
						returnedvcf.ServerTimeStamp = CurrentDateTime;
						returnedvcf.SourceTimeStamp = CurrentDateTime;
					}
				}

				if (vcfUnrounded.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated &&
				vcfUnrounded.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
				{
					OpcStatusCodeBits = new StatusCode((uint)vcfUnrounded.Status).CodeBits;
					if (vcfUnrounded.Value == null ||
							(double)vcfUnrounded.Value != 1.0 ||
							OpcStatusCodeBits != StatusCodes.Good)
					{
						vcfUnrounded.Value = 1.0;
						vcfUnrounded.Status = StatusCodes.Good;
						vcfUnrounded.ServerTimeStamp = CurrentDateTime;
						vcfUnrounded.SourceTimeStamp = CurrentDateTime;
					}
				}

				if (apicorrerror.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated &&
				apicorrerror.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
				{
					if (apicorrerror.Value == null ||
						 (bool)apicorrerror.Value != false)
					{
						apicorrerror.Value = false;
						apicorrerror.Status = StatusCodes.Good;
						apicorrerror.ServerTimeStamp = CurrentDateTime;
						apicorrerror.SourceTimeStamp = CurrentDateTime;
					}
				}

				// calculate densities in air
				if (DensityInAir.InputOutputType != PointTemplateTag.PointTagInputOutputType.Manual &&
				DensityInAir.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
				{
					CalculateDensityInAir(Density, ref DensityInAir);
				}

				if (StdDensityInAir.InputOutputType != PointTemplateTag.PointTagInputOutputType.Manual &&
				StdDensityInAir.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
				{
					CalculateDensityInAir(StdDensity, ref StdDensityInAir);
				}

				return true;
			}

			// Propogate Manual or Overriden Values
			if (Density.InputOutputType == PointTemplateTag.PointTagInputOutputType.Manual ||
			Density.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				CalculateDensityInAir(Density, ref DensityInAir);
			}

			if (StdDensity.InputOutputType == PointTemplateTag.PointTagInputOutputType.Manual ||
			StdDensity.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				CalculateDensityInAir(StdDensity, ref StdDensityInAir);
			}

			// this is where the density and vcf calculations are done
			double StdTempInDegreesC = 0.0;
			bool InitialUseDensity = false;
			DateTimeOffset pointServerTimeStamp = DateTimeOffset.UtcNow;
			DateTimeOffset pointSourceTimeStamp = DateTimeOffset.UtcNow;
			PointTag ctplTemp = new PointTag();

			// set the variables based on the VCF module settings
			bool useHydroCorrection = VcfSettings.UseHydrometerCorrection;
			double API2004DensityPressure = VcfSettings.DensityPressure.Value;
			double API2004AlternateTemperature = VcfSettings.AlternateTemperature.Value;
			double API2004BaseTemperature = VcfSettings.BaseTemperature.Value;
			double API2004AlternateBasePressure = VcfSettings.AlternateBasePressure.Value;
			bool forceVcfTo4Digits = VcfSettings.ForceVcfTo4Digits;
			bool useProductObserverdDensity = VcfSettings.UseProductObservedDensity;    // this needs to be finished when the product is integrated
			double[] Kfactors = VcfSettings.K;

			// WCG : Alpha was added to VcfSettings, in IM it was stored in K0.  In the future rework to pass Alpha directly.
			if (bVolCorecTypeMinor == Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_ALPHA60_SUPPLIED
			|| bVolCorecTypeMinor == Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54C
			|| bVolCorecTypeMinor == Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54C_30
			|| bVolCorecTypeMinor == Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API6C)
			{
				Kfactors[0] = VcfSettings.Alpha;
			}

			// api 2004 specification requirement if alternate pressure is negative force the value to 0
			if (API2004AlternateBasePressure < 0.0)
			{
				API2004AlternateBasePressure = 0.0;
			}

			// rounding methods are not used at this point but we initialize them here anyway
			TankBaseVcf.ETempRounding byTempRoundingMethod = TankBaseVcf.ETempRounding.TEMP_ROUNDING_NONE;
			TankBaseVcf.EVcfRounding byVcfRoundingMethod = TankBaseVcf.EVcfRounding.VCF_ROUNDING_NONE;

			// check that atleast one of the outputs is in calculated
			if ((ctl.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
				ctl.OpcStatusSubCode == StatusCodes.GoodLocalOverride) &&
				(cpl.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
				cpl.OpcStatusSubCode == StatusCodes.GoodLocalOverride) &&
				(ctpl.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
				ctpl.OpcStatusSubCode == StatusCodes.GoodLocalOverride) &&
				(returnedvcf.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
				returnedvcf.OpcStatusSubCode == StatusCodes.GoodLocalOverride) &&
				(apicorrerror.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
				apicorrerror.OpcStatusSubCode == StatusCodes.GoodLocalOverride))
			{
				return false;
			}

			bool UseDensity;
			// if using gauge density calculate standard first

			bool UseTemperatureDensityForVcf = false; // set true if Temperature Density should be used instead of Temperature Product to find VCF
														// from a set or measured Density Standard (opposed to a calculated one)

			if (usingGaugedDensity == true)
			{
				// make sure the data is valid gauge density will always use density temperature
				if (DensityGauge.Value == null || DensityTemperature.Value == null)
				{
					SetDensityValueandStatus(DensityGauge, ref Density);
					SetVariablesasUncertian(bVolCorecTypeMajor, ref ctpl, ref ctl, ref returnedvcf, ref vcfUnrounded, ref cpl, DensityGauge.ServerTimeStamp, DensityGauge.SourceTimeStamp);
					SetPointTagStatus(true, ref apicorrerror, ref StdDensity, ref Density, DensityGauge.ServerTimeStamp, DensityGauge.SourceTimeStamp);
					CalculateDensityInAir(StdDensity, ref StdDensityInAir);
					return false;
				}

				UseDensity = true;   // this will be a three part operation.
			}
			// determine which density to use based on the density configuration
			else if (StdDensity.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated &&
				Density.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated)
			{
					 if (ProdTemperature.SourceTimeStamp > Density.SourceTimeStamp)
						  pointSourceTimeStamp = ProdTemperature.SourceTimeStamp;
					 else
						  pointSourceTimeStamp = Density.SourceTimeStamp;

					 if (ProdTemperature.ServerTimeStamp > Density.ServerTimeStamp)
						  pointServerTimeStamp = ProdTemperature.ServerTimeStamp;
					 else
                    pointServerTimeStamp = Density.ServerTimeStamp;

            // make sure the data is valid
            if (Density.Value == null || ProdTemperature.Value == null)
				{
					SetVariablesasUncertian(bVolCorecTypeMajor, ref ctpl, ref ctl, ref returnedvcf, ref vcfUnrounded, ref cpl, pointServerTimeStamp, pointSourceTimeStamp);
					SetPointTagStatus(true, ref apicorrerror, ref StdDensity, ref Density, pointServerTimeStamp, pointSourceTimeStamp);
					CalculateDensityInAir(StdDensity, ref StdDensityInAir);
					return false;
				}
				UseDensity = true;
				InitialUseDensity = true;
			}
			else if (StdDensity.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated &&
				Density.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated)
			{
                if (ProdTemperature.SourceTimeStamp > StdDensity.SourceTimeStamp)
                    pointSourceTimeStamp = ProdTemperature.SourceTimeStamp;
                else
                    pointSourceTimeStamp = StdDensity.SourceTimeStamp;

                if (ProdTemperature.ServerTimeStamp > StdDensity.ServerTimeStamp)
                    pointServerTimeStamp = ProdTemperature.ServerTimeStamp;
                else
                    pointServerTimeStamp = StdDensity.ServerTimeStamp;

                if (StdDensity.Value == null || (DensityTemperature.Value == null && ProdTemperature.Value == null))
				{
					SetVariablesasUncertian(bVolCorecTypeMajor, ref ctpl, ref ctl, ref returnedvcf, ref vcfUnrounded, ref cpl, pointServerTimeStamp, pointSourceTimeStamp);
					SetPointTagStatus(true, ref apicorrerror, ref StdDensity, ref Density, pointServerTimeStamp, pointSourceTimeStamp);
					CalculateDensityInAir(Density, ref DensityInAir);
					return false;
				}
				UseDensity = false;
				InitialUseDensity = false;

				if (DensityTemperature.InputOutputType != PointTemplateTag.PointTagInputOutputType.UnAssigned)
				{
					UseTemperatureDensityForVcf = true;
				}
			}
			else
			{
				// invalid configuration. Set the status and exit
				SetVariablesasUncertian(bVolCorecTypeMajor, ref ctpl, ref ctl, ref returnedvcf, ref vcfUnrounded, ref cpl, DateTime.UtcNow, DateTime.UtcNow);
				SetPointTagStatus(true, ref apicorrerror, ref StdDensity, ref Density, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow);
				CalculateDensityInAir(Density, ref DensityInAir);
				CalculateDensityInAir(StdDensity, ref StdDensityInAir);
				return false;
			}

			// convert the standard temperature to degrees C
			EngineeringUnits.Convert(VcfSettings.BaseTemperature.Value, EngineeringUnit.FmtDegF, ref StdTempInDegreesC, EngineeringUnit.FmtDegC, 60.0);

			// get the associated base vcf object from the volumn correct factory
			var BaseVcfObject = VolumeCorrectionFactory.GetVolumeCorrection(bVolCorecTypeMajor,
													bVolCorecTypeMinor,
													UseDensity,
													useHydroCorrection,
													false,
													false,
													forceVcfTo4Digits);

			if (BaseVcfObject == null)
			{
				SetVariablesasFailed(bVolCorecTypeMajor, ref ctpl, ref ctl, ref returnedvcf, ref vcfUnrounded, ref cpl);
				SetPointTagStatus(true, ref apicorrerror, ref StdDensity, ref Density, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow);
				CalculateDensityInAir(Density, ref DensityInAir);
				CalculateDensityInAir(StdDensity, ref StdDensityInAir);
				return false;
			}
			// call the standard volume correction function
			// declare and set the passed in variables
			double MeasuredTemperature = 0.0;
			double StandardDensity = 0.0;
			double MeasuredDensity = 0.0;
			double ReturnedVCF = 0.0;
			double ReturnedUnroundedVCF = 0.0;
			double ReturnedRoundedVCF = 0.0;
			double CTLReturn = 1.0;
			double CPLReturn = 1.0;

			//double StandardTemperature = 60.0;
			double StandardTemperature = VcfSettings.BaseTemperature.Value;

			EngineeringUnits.Convert(StdTempInDegreesC, EngineeringUnit.FmtDegC, ref StandardTemperature, ProdTemperature.Units, 15.0);

			bool VCFCalculationSucceeded;
			// if we are using gauge density calculate standard from gauge
			if (usingGaugedDensity == true)
			{
				// gauge density is done in 3 parts
				// first calculate std density from gauge density using density temp
				// this will give us the standard density for the product at 60 degrees F
				// then calculate product density from standard density
				// then calculate the actual vcf.
				// vcf is calculated for each of the above because it is required to calculate density

				// make sure density temp is valid
				if (!(DensityTemperature.Value is double?) ||
				!((double?)DensityTemperature.Value).HasValue)
				{
					SetVariablesasUncertian(bVolCorecTypeMajor, ref ctpl, ref ctl, ref returnedvcf, ref vcfUnrounded, ref cpl, DensityTemperature.ServerTimeStamp, DensityTemperature.SourceTimeStamp);
					SetPointTagStatus(true, ref apicorrerror, ref StdDensity, ref Density, DensityTemperature.ServerTimeStamp, DensityTemperature.SourceTimeStamp);
					CalculateDensityInAir(Density, ref DensityInAir);
					CalculateDensityInAir(StdDensity, ref StdDensityInAir);
					return false;
				}
				MeasuredTemperature = (double)DensityTemperature.Value;

				MeasuredDensity = (double)DensityGauge.Value;

				if ((VaporPress.Value is double?) &&
					((double?)VaporPress.Value).HasValue)
				{
					API2004DensityPressure = (double)VaporPress.Value;
				}
				double ReferenceTemperature = (double)DensityTemperature.Value;

				VCFCalculationSucceeded = BaseVcfObject.CalcTankProdVcf(ReferenceTemperature, // Measured Temperature
													StandardTemperature,                // Standard Temperature
													ProdTemperature.Units,              // Standard Temp. Engineering Units
													ProdTemperature.Units,              // Temp. Engineering Units
													byTempRoundingMethod,
													byVcfRoundingMethod,
													MeasuredDensity,                    // Measured Density
													Density.Units,                      // Measured Density Engineering Units
													StandardDensity,                    // Standard Product Density
													StdDensity.Units,                   // Std. density Engineering Units
													UseDensity,                         // Use Measured Density in Calc
													API2004DensityPressure,             // density pressure for api 2004
													VaporPress.Units,                   // density pressure units for api 2004
													API2004AlternateTemperature,        // alternate temp for api 2004
													API2004BaseTemperature,             // api 2004 alternate base temp reference
													API2004AlternateBasePressure,    // api 2004 alternate base pressure reference
													ref CTLReturn,
													ref CPLReturn,
													ref Kfactors,                       // Pointer to K Factors Array
													ref ReturnedVCF,                    // VCF for calculation purposes
													ref ReturnedUnroundedVCF,           // Unrounded VCF
													ref ReturnedRoundedVCF);            // Volume Correction Factor (rounded)

				if (!VCFCalculationSucceeded)
				{
					SetVariablesasUncertian(bVolCorecTypeMajor, ref ctpl, ref ctl, ref returnedvcf, ref vcfUnrounded, ref cpl, DensityGauge.ServerTimeStamp > DensityTemperature.ServerTimeStamp ? DensityGauge.ServerTimeStamp : DensityTemperature.ServerTimeStamp, DensityGauge.SourceTimeStamp > DensityTemperature.SourceTimeStamp ? DensityGauge.SourceTimeStamp : DensityTemperature.SourceTimeStamp);
					SetPointTagStatus(true, ref apicorrerror, ref StdDensity, ref Density, DensityGauge.ServerTimeStamp > DensityTemperature.ServerTimeStamp ? DensityGauge.ServerTimeStamp : DensityTemperature.ServerTimeStamp, DensityGauge.SourceTimeStamp > DensityTemperature.SourceTimeStamp ? DensityGauge.SourceTimeStamp : DensityTemperature.SourceTimeStamp);
					CalculateDensityInAir(Density, ref DensityInAir);
					CalculateDensityInAir(StdDensity, ref StdDensityInAir);
					return true;
				}

				// calculate standard density from vcf calculated above
				if (StdDensity.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated &&
					StdDensity.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
				{
					double pdStdDensity = 0.0;
					double dHydrometer = 0.0;
					double[] dk = Kfactors;

					if (!BaseVcfObject.CalcTankStdDensity(StdDensity.Units,        // Std Density Engr Units
															ReferenceTemperature,            // Measured Temperature
															ProdTemperature.Units,           // Temperature Engr Units
															0,                               //ETempRounding byTempRoundingMethod,
															0,                               //EVcfRounding byVcfRoundingMethod,
															MeasuredDensity,                 // Measured Density
															Density.Units,                   // Density Engr Units
															ReturnedUnroundedVCF,            // Volume Correction Factor
															API2004DensityPressure,          // density pressure for api 2004
															VaporPress.Units,                // density pressure units for api 2004
															API2004AlternateTemperature,     // alternate temp for api 2004
															API2004BaseTemperature,          // api 2004 alternate base temp reference
															API2004AlternateBasePressure,    // api 2004 alternate base pressure reference
															ref CTLReturn,
															ref CPLReturn,
															ref dk,
															ref pdStdDensity,                // Standard Density Variable
															ref dHydrometer))
					{
						SetVariablesasUncertian(bVolCorecTypeMajor, ref ctpl, ref ctl, ref returnedvcf, ref vcfUnrounded, ref cpl, StdDensity.ServerTimeStamp, StdDensity.SourceTimeStamp);
						SetPointTagStatus(true, ref apicorrerror, ref StdDensity, ref Density, StdDensity.ServerTimeStamp, StdDensity.SourceTimeStamp);
						CalculateDensityInAir(Density, ref DensityInAir);
						CalculateDensityInAir(StdDensity, ref StdDensityInAir);
						return true;
					}
					else if (StdDensity.Value == null || (double)StdDensity.Value != pdStdDensity)
					{
						StdDensity.Value = pdStdDensity;
						StdDensity.ServerTimeStamp = DateTimeOffset.UtcNow;
						StdDensity.SourceTimeStamp = DateTimeOffset.UtcNow;
						StdDensity.Status = StatusCodes.Good;
					}
				}
			}  // end density gauge calculation area


			if (UseTemperatureDensityForVcf)
			{
				if ((DensityTemperature.Value is double?) &&
					((double?)ProdTemperature.Value).HasValue)
				{
					MeasuredTemperature = (double)DensityTemperature.Value;
				}
			}
			else
			{
				if ((ProdTemperature.Value is double?) &&
					((double?)ProdTemperature.Value).HasValue)
				{
					MeasuredTemperature = (double)ProdTemperature.Value;
				}
			}
			if ((StdDensity.Value is double?) &&
				((double?)StdDensity.Value).HasValue)
			{
				StandardDensity = (double)StdDensity.Value;
			}
			if ((Density.Value is double?) &&
				((double?)Density.Value).HasValue)
			{
				MeasuredDensity = (double)Density.Value;
			}
			if ((VaporPress.Value is double?) &&
				((double?)VaporPress.Value).HasValue)
			{
				API2004DensityPressure = (double)VaporPress.Value;
			}

			if (UseDensity == true)
			{
				bool UseDensityTemp = false;
				if (DensityTemperature.InputOutputType == PointTemplateTag.PointTagInputOutputType.UnAssigned || usingGaugedDensity == true)
				{
					if (!(ProdTemperature.Value is double?) ||
					!((double?)ProdTemperature.Value).HasValue)
					{
						SetVariablesasUncertian(bVolCorecTypeMajor, ref ctpl, ref ctl, ref returnedvcf, ref vcfUnrounded, ref cpl, ProdTemperature.ServerTimeStamp, ProdTemperature.SourceTimeStamp);
						SetPointTagStatus(true, ref apicorrerror, ref StdDensity, ref Density, ProdTemperature.ServerTimeStamp, ProdTemperature.SourceTimeStamp);
						CalculateDensityInAir(Density, ref DensityInAir);
						CalculateDensityInAir(StdDensity, ref StdDensityInAir);
						return false;
					}
				}
				else
				{
					if (!(DensityTemperature.Value is double?) ||
					!((double?)DensityTemperature.Value).HasValue)
					{
						SetVariablesasUncertian(bVolCorecTypeMajor, ref ctpl, ref ctl, ref returnedvcf, ref vcfUnrounded, ref cpl, DensityTemperature.ServerTimeStamp, DensityTemperature.SourceTimeStamp);
						SetPointTagStatus(true, ref apicorrerror, ref StdDensity, ref Density, DensityTemperature.ServerTimeStamp, DensityTemperature.SourceTimeStamp);
						CalculateDensityInAir(Density, ref DensityInAir);
						CalculateDensityInAir(StdDensity, ref StdDensityInAir);
						return false;
					}
				}
				// we only need to set the flag to true to calculate standard density
				if ((StdDensity.Value == null ||
					StdDensity.SourceTimeStamp < Density.SourceTimeStamp ||
					StdDensity.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated ||
					(DensityTemperature.InputOutputType != PointTemplateTag.PointTagInputOutputType.UnAssigned &&
					StdDensity.SourceTimeStamp < DensityTemperature.SourceTimeStamp) ||
					(DensityTemperature.InputOutputType == PointTemplateTag.PointTagInputOutputType.UnAssigned &&
					StdDensity.SourceTimeStamp < ProdTemperature.SourceTimeStamp)) && usingGaugedDensity == false)
				{
					UseDensityTemp = true;
				}

				if (UseDensityTemp)
				{
					double ReferenceTemperature;
					if (DensityTemperature.InputOutputType == PointTemplateTag.PointTagInputOutputType.UnAssigned)
					{
						// use product temperature
						ReferenceTemperature = (double)ProdTemperature.Value;
								pointServerTimeStamp = ProdTemperature.ServerTimeStamp > Density.ServerTimeStamp ? ProdTemperature.ServerTimeStamp : Density.ServerTimeStamp;
								pointSourceTimeStamp = ProdTemperature.SourceTimeStamp > Density.SourceTimeStamp ? ProdTemperature.SourceTimeStamp: Density.SourceTimeStamp;
					}
					else
					{
						ReferenceTemperature = (double)DensityTemperature.Value;
                        pointServerTimeStamp = DensityTemperature.ServerTimeStamp > Density.ServerTimeStamp ? DensityTemperature.ServerTimeStamp : Density.ServerTimeStamp;
                        pointSourceTimeStamp = DensityTemperature.SourceTimeStamp > Density.SourceTimeStamp ? DensityTemperature.SourceTimeStamp : Density.SourceTimeStamp;
                    }

					VCFCalculationSucceeded = BaseVcfObject.CalcTankProdVcf(ReferenceTemperature,                // Measured Temperature
																		StandardTemperature,                // Standard Temperature
																		ProdTemperature.Units,              // Standard Temp. Engineering Units
																		ProdTemperature.Units,              // Temp. Engineering Units
																		byTempRoundingMethod,
																		byVcfRoundingMethod,
																		MeasuredDensity,                    // Measured Density
																		Density.Units,                      // Measured Density Engineering Units
																		StandardDensity,                    // Standard Product Density
																		StdDensity.Units,                   // Std. density Engineering Units
																		UseDensity,                         // Use Measured Density in Calc
																		API2004DensityPressure,             // density pressure for api 2004
																		VaporPress.Units,                   // density pressure units for api 2004
																		API2004AlternateTemperature,        // alternate temp for api 2004
																		API2004BaseTemperature,             // api 2004 alternate base temp reference
																		API2004AlternateBasePressure,    // api 2004 alternate base pressure reference
																		ref CTLReturn,
																		ref CPLReturn,
																		ref Kfactors,                       // Pointer to K Factors Array
																		ref ReturnedVCF,                    // VCF for calculation purposes
																		ref ReturnedUnroundedVCF,           // Unrounded VCF
																		ref ReturnedRoundedVCF);            // Volume Correction Factor (rounded)

					if (!VCFCalculationSucceeded)
					{
						SetVariablesasUncertian(bVolCorecTypeMajor, ref ctpl, ref ctl, ref returnedvcf, ref vcfUnrounded, ref cpl, pointServerTimeStamp, pointSourceTimeStamp);
						SetPointTagStatus(true, ref apicorrerror, ref StdDensity, ref Density, pointServerTimeStamp, pointSourceTimeStamp);
						CalculateDensityInAir(Density, ref DensityInAir);
						CalculateDensityInAir(StdDensity, ref StdDensityInAir);
						return true;
					}
					if (StdDensity.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated &&
						StdDensity.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
					{
						double pdStdDensity = 0.0;
						double dHydrometer = 0.0;
						double[] dk = Kfactors;// { 0.0, 0.0, 0.0, 0.0, 0.0 };

						if (!BaseVcfObject.CalcTankStdDensity(StdDensity.Units, // Std Density Engr Units
																ReferenceTemperature,//MeasuredTemperature,               // Measured Temperature
																ProdTemperature.Units,        // Temperature Engr Units
																0,//ETempRounding byTempRoundingMethod,
																0,//EVcfRounding byVcfRoundingMethod,
																MeasuredDensity,            // Measured Density
																Density.Units,     // Density Engr Units
																ReturnedUnroundedVCF,  // Volume Correction Factor
																API2004DensityPressure,           // density pressure for api 2004
																VaporPress.Units,    // density pressure units for api 2004
																API2004AlternateTemperature,        // alternate temp for api 2004
																API2004BaseTemperature,             // api 2004 alternate base temp reference
																API2004AlternateBasePressure,    // api 2004 alternate base pressure reference
																ref CTLReturn,
																ref CPLReturn,
																ref dk,
																ref pdStdDensity,       // Standard Density Variable
																ref dHydrometer))
						{
							SetVariablesasUncertian(bVolCorecTypeMajor, ref ctpl, ref ctl, ref returnedvcf, ref vcfUnrounded, ref cpl, pointServerTimeStamp, pointSourceTimeStamp);
							SetPointTagStatus(true, ref apicorrerror, ref StdDensity, ref Density, pointServerTimeStamp, pointSourceTimeStamp);
							CalculateDensityInAir(Density, ref DensityInAir);
							CalculateDensityInAir(StdDensity, ref StdDensityInAir);
							return true;
						}
						else if (StdDensity.Value == null || (double)StdDensity.Value != pdStdDensity)
						{
							StdDensity.Value = pdStdDensity;
							StdDensity.ServerTimeStamp = pointServerTimeStamp;
							StdDensity.SourceTimeStamp = pointSourceTimeStamp;
							StdDensity.Status = StatusCodes.Good;
						}
					}
				}
				// required fields are std density and product temperature
				if (StdDensity.Value == null || ProdTemperature.Value == null)
				{
					SetVariablesasUncertian(bVolCorecTypeMajor, ref ctpl, ref ctl, ref returnedvcf, ref vcfUnrounded, ref cpl, StdDensity.ServerTimeStamp > ProdTemperature.ServerTimeStamp ? StdDensity.ServerTimeStamp : ProdTemperature.ServerTimeStamp, StdDensity.SourceTimeStamp > ProdTemperature.SourceTimeStamp ? StdDensity.SourceTimeStamp : ProdTemperature.SourceTimeStamp);
					SetPointTagStatus(true, ref apicorrerror, ref StdDensity, ref Density, StdDensity.ServerTimeStamp > ProdTemperature.ServerTimeStamp ? StdDensity.ServerTimeStamp : ProdTemperature.ServerTimeStamp, StdDensity.SourceTimeStamp > ProdTemperature.SourceTimeStamp ? StdDensity.SourceTimeStamp : ProdTemperature.SourceTimeStamp);
					CalculateDensityInAir(Density, ref DensityInAir);
					CalculateDensityInAir(StdDensity, ref StdDensityInAir);
					return false;
				}
				StandardDensity = (double)StdDensity.Value;
				UseDensity = false;
			}

			VCFCalculationSucceeded = BaseVcfObject.CalcTankProdVcf(MeasuredTemperature,           // Measured Temperature
																StandardTemperature,          // Standard Temperature
																ProdTemperature.Units,           // Standard Temp. Engineering Units
																ProdTemperature.Units,           // Temp. Engineering Units
																byTempRoundingMethod,
																byVcfRoundingMethod,
																MeasuredDensity,              // Measured Density
																Density.Units,                // Measured Density Engineering Units
																StandardDensity,              // Standard Product Density
																StdDensity.Units,             // Std. density Engineering Units
																UseDensity,                   // Use Measured Density in Calc
																API2004DensityPressure,          // density pressure for api 2004
																VaporPress.Units,             // density pressure units for api 2004
																API2004AlternateTemperature,     // alternate temp for api 2004
																API2004BaseTemperature,             // api 2004 alternate base temp reference
																API2004AlternateBasePressure, // api 2004 alternate base pressure reference
																ref CTLReturn,
																ref CPLReturn,
																ref Kfactors,                       // Pointer to K Factors Array
																ref ReturnedVCF,              // VCF for calculation purposes
																ref ReturnedUnroundedVCF,        // Unrounded VCF
																ref ReturnedRoundedVCF);            // Volume Correction Factor (rounded)

			if (!VCFCalculationSucceeded)
			{
				SetVariablesasUncertian(bVolCorecTypeMajor, ref ctpl, ref ctl, ref returnedvcf, ref vcfUnrounded, ref cpl, pointServerTimeStamp, pointSourceTimeStamp);
				SetPointTagStatus(true, ref apicorrerror, ref StdDensity, ref Density, pointServerTimeStamp, pointSourceTimeStamp);
				CalculateDensityInAir(Density, ref DensityInAir);
				CalculateDensityInAir(StdDensity, ref StdDensityInAir);
				return true;
			}

			// set the variables in the tag storage
			if (InitialUseDensity == true)
			{
				// this is based on std density and product temperature
				pointServerTimeStamp = Density.ServerTimeStamp;
				pointSourceTimeStamp = Density.SourceTimeStamp;
				if (ProdTemperature.SourceTimeStamp > pointSourceTimeStamp)
				{
					pointServerTimeStamp = ProdTemperature.ServerTimeStamp;
					pointSourceTimeStamp = ProdTemperature.SourceTimeStamp;
				}
			}
			else if (InitialUseDensity == false)
			{
				// this is based on density and density temperature
				pointServerTimeStamp = StdDensity.ServerTimeStamp;
				pointSourceTimeStamp = StdDensity.SourceTimeStamp;
                if (ProdTemperature.SourceTimeStamp > pointSourceTimeStamp)
                {
                    pointServerTimeStamp = ProdTemperature.ServerTimeStamp;
                    pointSourceTimeStamp = ProdTemperature.SourceTimeStamp;
                }
				if(!(DensityTemperature.InputOutputType == PointTemplateTag.PointTagInputOutputType.UnAssigned && usingGaugedDensity == false))
				{
					if (DensityTemperature.SourceTimeStamp > pointSourceTimeStamp && usingGaugedDensity == false)
					{
						pointServerTimeStamp = DensityTemperature.ServerTimeStamp;
						pointSourceTimeStamp = DensityTemperature.SourceTimeStamp;
					}
				}
			}

			if (returnedvcf.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated &&
			returnedvcf.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
			{
				if (returnedvcf.Value == null || (double)returnedvcf.Value != ReturnedVCF || returnedvcf.Status != StatusCodes.Good)
				{
					returnedvcf.Value = ReturnedVCF;
					returnedvcf.ServerTimeStamp = pointServerTimeStamp;
					returnedvcf.SourceTimeStamp = pointSourceTimeStamp;
					returnedvcf.Status = StatusCodes.Good;
				}
			}
			if (vcfUnrounded.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated &&
			vcfUnrounded.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
			{
				if (vcfUnrounded.Value == null || (double)vcfUnrounded.Value != ReturnedUnroundedVCF || vcfUnrounded.Status != StatusCodes.Good)
				{
					vcfUnrounded.Value = ReturnedUnroundedVCF;
					vcfUnrounded.ServerTimeStamp = pointServerTimeStamp;
					vcfUnrounded.SourceTimeStamp = pointSourceTimeStamp;
					vcfUnrounded.Status = StatusCodes.Good;
				}
			}
			if (ctpl.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated &&
			ctpl.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
			{
				if (ctpl.Value == null || (double)ctpl.Value != ReturnedRoundedVCF || ctpl.Status != StatusCodes.Good)
				{
					ctpl.Value = ReturnedRoundedVCF;
					ctpl.ServerTimeStamp = pointServerTimeStamp;
					ctpl.SourceTimeStamp = pointSourceTimeStamp;
					ctpl.Status = StatusCodes.Good;
				}
			}
			if (ctl.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated &&
			ctl.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
			{
				if (bVolCorecTypeMajor == Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_ASTM_COMM_2004)
				{
					if (ctl.Value == null || (double)ctl.Value != CTLReturn)
					{
						ctl.Value = CTLReturn;
						ctl.ServerTimeStamp = pointServerTimeStamp;
						ctl.SourceTimeStamp = pointSourceTimeStamp;
						ctl.Status = StatusCodes.Good;
					}
				}
				else
				{
					if (ctl.Value == null || (double)ctl.Value != ReturnedRoundedVCF)
					{
						ctl.Value = ReturnedRoundedVCF;
						ctl.ServerTimeStamp = pointServerTimeStamp;
						ctl.SourceTimeStamp = pointSourceTimeStamp;
						ctl.Status = StatusCodes.Good;
					}
				}
			}

			if (cpl.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated &&
			cpl.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
			{
				if (bVolCorecTypeMajor == Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_ASTM_COMM_2004)
				{
					if (cpl.Value == null || (double)cpl.Value != CPLReturn)
					{
						cpl.Value = CPLReturn;
						cpl.Status = StatusCodes.Good;
						cpl.ServerTimeStamp = pointServerTimeStamp;
						cpl.SourceTimeStamp = pointSourceTimeStamp;
					}
				}
				else
				{
					// set as not supported
					if (cpl.Value != null)
					{
						cpl.Value = null;
						cpl.Status = StatusCodes.BadNotImplemented;
						cpl.ServerTimeStamp = pointServerTimeStamp;
						cpl.SourceTimeStamp = pointSourceTimeStamp;
					}
				}
			}

			SetPointTagStatus(false, ref apicorrerror, ref StdDensity, ref Density, pointServerTimeStamp, pointSourceTimeStamp);

			// calculate the associated density
			if (InitialUseDensity == false)  // calculate density
			{
				if (Density.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated &&
					Density.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
				{
					double pdDensity = 0.0;
					double dHydrometer = 0.0;
					double[] dk = Kfactors;// { 0.0, 0.0, 0.0, 0.0, 0.0 };

					if (!BaseVcfObject.CalcTankDensity(Density.Units,
															MeasuredTemperature,
															ProdTemperature.Units,
															byTempRoundingMethod,
															byVcfRoundingMethod,
															StandardDensity,
															StdDensity.Units,
															ReturnedUnroundedVCF,
															API2004DensityPressure,
															VaporPress.Units,
															API2004BaseTemperature,
															API2004AlternateBasePressure,
															ref CTLReturn,
															ref CPLReturn,
															ref dk,
															ref pdDensity,
															ref dHydrometer))
					{
						if (Density.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated &&
						Density.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
						{
							OpcStatusCodeBits = new StatusCode((uint)Density.Status).CodeBits;
							if (OpcStatusCodeBits != StatusCodes.Bad)
							{
								Density.Value = null;
								Density.ServerTimeStamp = DateTimeOffset.UtcNow;
								Density.SourceTimeStamp = DateTimeOffset.UtcNow;
								Density.Status = StatusCodes.Bad;
							}
						}
					}
					else
					{
						if (Density.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated &&
						Density.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
						{
							if (Density.Value == null || (double)Density.Value != pdDensity)
							{
								Density.Value = pdDensity;
								Density.ServerTimeStamp = DateTimeOffset.UtcNow;
								Density.SourceTimeStamp = DateTimeOffset.UtcNow;
								Density.Status = StatusCodes.Good;
							}
						}
					}
				}
			}

			// always calculate densities in air
			CalculateDensityInAir(Density, ref DensityInAir);
			CalculateDensityInAir(StdDensity, ref StdDensityInAir);

			if(usingGaugedDensity == false &&
				DensityGauge.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated &&
				DensityGauge.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
			{
				if(StatusCode.IsNotGood(new StatusCode((uint)Density.Status).CodeBits))
				{
					uint dgOpcStatusCodeBits = new StatusCode((uint)DensityGauge.Status).CodeBits;
					if (dgOpcStatusCodeBits != StatusCodes.Bad)
					{
						DensityGauge.Value = null;
						DensityGauge.ServerTimeStamp = DateTimeOffset.UtcNow;
						DensityGauge.SourceTimeStamp = DateTimeOffset.UtcNow;
						DensityGauge.Status = StatusCodes.Bad;
					}
				}
				else if (DensityGauge.Value == null || (double)DensityGauge.Value != (double)Density.Value)
				{
					DensityGauge.Value = Density.Value;
					DensityGauge.ServerTimeStamp = Density.ServerTimeStamp;
					DensityGauge.SourceTimeStamp = Density.SourceTimeStamp;
					DensityGauge.Status = StatusCodes.Good;
				}
			}

			return true;

		}



		private void CalculateDensityInAir(PointTag ReferenceDensity, ref PointTag OutputDensityInAir)
		{
			// make sure mass is calculated
			if (OutputDensityInAir.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
				OutputDensityInAir.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}

			// make sure all of the data is valid
			if (!IsValueGood(ReferenceDensity))
			{
				uint OpcStatusCodeBits = new StatusCode((uint)OutputDensityInAir.Status).CodeBits;
				if (OutputDensityInAir.Value != null ||
				OpcStatusCodeBits != StatusCodes.Bad)
				{
					OutputDensityInAir.Value = null;
					OutputDensityInAir.Status = StatusCodes.Bad;
					OutputDensityInAir.ServerTimeStamp = ReferenceDensity.ServerTimeStamp;
					OutputDensityInAir.SourceTimeStamp = ReferenceDensity.SourceTimeStamp;
				}

				return;
			}

			double densityinkgm3 = 0.0;
			double ConversionFactor = 1.1;
			EngineeringUnits.Convert((double)ReferenceDensity.Value, ReferenceDensity.Units, ref densityinkgm3, EngineeringUnit.FmdKgM3, 60.0);

			if (densityinkgm3 >= 996.6 &&
				densityinkgm3 <= 1663.5)
				ConversionFactor = 1.0;
			else if (densityinkgm3 > 1663.5)
				ConversionFactor = 0.9;

			double NewValue = densityinkgm3 - ConversionFactor;
			EngineeringUnits.Convert(NewValue, EngineeringUnit.FmdKgM3, ref NewValue, OutputDensityInAir.Units, 60.0);

			long newStatus = StatusCodes.Good;

			// if either of the variables are over/under ranged set status to warning
			if (IsStatusUncertain(ReferenceDensity))
			{
				newStatus = StatusCodes.Uncertain;
			}



			if (OutputDensityInAir.Value == null ||
			(double)OutputDensityInAir.Value != NewValue
			|| IsStatusChange(OutputDensityInAir.Status, newStatus))
			{
				OutputDensityInAir.Value = NewValue;
				OutputDensityInAir.Status = newStatus;
				CheckForAndSetOverUnderRange(OutputDensityInAir);

				OutputDensityInAir.ServerTimeStamp = ReferenceDensity.ServerTimeStamp;
				OutputDensityInAir.SourceTimeStamp = ReferenceDensity.SourceTimeStamp;
			}
		}


		public ModuleInputOutputCollection GetInputOutputCollection(string calculationName)
		{
			var properties = new ModuleInputOutputCollection
							{
								new ModuleInputOutput
								{
									ID = "Temperature Product",
									Type = typeof(double?),
									ParameterType = ModuleInputOutputType.Input
								},
								new ModuleInputOutput
								{
									ID = "Temperature Density",
									Type = typeof(double?),
									ParameterType = ModuleInputOutputType.Input
								},
								new ModuleInputOutput
								{
									ID = "Density Product Standard",
									Type = typeof(double?),
									ParameterType = ModuleInputOutputType.Input
								},
								new ModuleInputOutput
								{
									ID = "Density Product Observed",
									Type = typeof(double?),
									ParameterType = ModuleInputOutputType.Input
								},
								new ModuleInputOutput
								{
									ID = "Pressure Vapor",
									Type = typeof(double?),
									ParameterType = ModuleInputOutputType.Input
								},
								new ModuleInputOutput
								{
									ID = "Volume Correction for Temperature",
									Type = typeof(double?),
									ParameterType = ModuleInputOutputType.Output
								},
								new ModuleInputOutput
								{
									ID = "Volume Correction for Pressure",
									Type = typeof(double?),
									ParameterType = ModuleInputOutputType.Output
								},
								new ModuleInputOutput
								{
									ID = "Volume Correction for Press and Temp",
									Type = typeof(double?),
									ParameterType = ModuleInputOutputType.Output
								},
								new ModuleInputOutput
								{
									ID = "Volume Correction Factor",
									Type = typeof(double?),
									ParameterType = ModuleInputOutputType.Output
								},
								new ModuleInputOutput
								{
									ID = "API Correction Error",
									Type = typeof(bool?),
									ParameterType = ModuleInputOutputType.Output
								},
								new ModuleInputOutput
								{
									ID = "Density Product In Air",
									Type = typeof(double?),
									ParameterType = ModuleInputOutputType.Output
								},
								new ModuleInputOutput
								{
									ID = "Density Produt Standard In Air",
									Type = typeof(double?),
									ParameterType = ModuleInputOutputType.Output
								}
							};
			return properties;
		}

		private void SetPointTagStatus(bool valueToSet, ref PointTag pointTag, ref PointTag StdDensity, ref PointTag Density, DateTimeOffset serverTimestamp, DateTimeOffset sourceTimestamp)
		{
			if (pointTag.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated &&
			pointTag.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
			{
				if (pointTag.Value == null || (bool)pointTag.Value != valueToSet)
				{
					pointTag.Value = valueToSet;
					pointTag.Status = StatusCodes.Good;
					pointTag.ServerTimeStamp = serverTimestamp;
					pointTag.SourceTimeStamp = sourceTimestamp;
				}
			}
			uint OpcStatusCodeBits;
			// if we have an api correction error the calculated density cannot be good
			// set the status at failed
			if (StdDensity.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated &&
				StdDensity.OpcStatusSubCode != StatusCodes.GoodLocalOverride &&
				StdDensity.Value != null)
			{
				OpcStatusCodeBits = new StatusCode((uint)StdDensity.Status).CodeBits;
				if (valueToSet == true && OpcStatusCodeBits != StatusCodes.Bad)
				{
					StdDensity.Status = StatusCodes.Bad;
					StdDensity.Value = null;
					StdDensity.ServerTimeStamp = serverTimestamp;
					StdDensity.SourceTimeStamp = sourceTimestamp;
				}
				else if (OpcStatusCodeBits != StatusCodes.Good)
				{
					StdDensity.Status = StatusCodes.Good;
					StdDensity.ServerTimeStamp = serverTimestamp;
					StdDensity.SourceTimeStamp = sourceTimestamp;
				}
			}
			if (Density.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated &&
				Density.OpcStatusSubCode != StatusCodes.GoodLocalOverride &&
				Density.Value != null)
			{
				OpcStatusCodeBits = new StatusCode((uint)Density.Status).CodeBits;

				if (valueToSet == true && OpcStatusCodeBits != StatusCodes.Bad)
				{
					Density.Status = StatusCodes.Bad;
					Density.Value = null;
					Density.ServerTimeStamp = serverTimestamp;
					Density.SourceTimeStamp = sourceTimestamp;
				}
				else if (OpcStatusCodeBits != StatusCodes.Good)
				{
					Density.Status = StatusCodes.Good;
					Density.ServerTimeStamp = serverTimestamp;
					Density.SourceTimeStamp = sourceTimestamp;
				}
			}

		}

		private void SetVariablesasUncertian(Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor bVolCorecTypeMajor, ref PointTag ctpl, ref PointTag ctl, ref PointTag returnedvcf, ref PointTag unroundedVcf, ref PointTag cpl, DateTimeOffset serverTimestamp, DateTimeOffset sourceTimestamp)
		{
			if (ctpl.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated &&
			ctpl.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
			{
				if (ctpl.Value == null || (double)ctpl.Value != 1.0)
				{
					ctpl.Value = 1.0;
					ctpl.Status = StatusCodes.Uncertain;
					ctpl.ServerTimeStamp = serverTimestamp;
					ctpl.SourceTimeStamp = sourceTimestamp;
				}
			}
			if (ctl.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated &&
			ctl.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
			{
				if (ctl.Value == null || (double)ctl.Value != 1.0)
				{
					ctl.Value = 1.0;
					ctl.Status = StatusCodes.Uncertain;
					ctl.ServerTimeStamp = serverTimestamp;
					ctl.SourceTimeStamp = sourceTimestamp;
				}
			}
			if (returnedvcf.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated &&
			returnedvcf.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
			{
				if (returnedvcf.Value == null || (double)returnedvcf.Value != 1.0)
				{
					returnedvcf.Value = 1.0;
					returnedvcf.Status = StatusCodes.Uncertain;
					returnedvcf.ServerTimeStamp = serverTimestamp;
					returnedvcf.SourceTimeStamp = sourceTimestamp;
				}
			}
			if (unroundedVcf.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated &&
			unroundedVcf.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
			{
				if (unroundedVcf.Value == null || (double)unroundedVcf.Value != 1.0)
				{
					unroundedVcf.Value = 1.0;
					unroundedVcf.Status = StatusCodes.Uncertain;
					unroundedVcf.ServerTimeStamp = serverTimestamp;
					unroundedVcf.SourceTimeStamp = sourceTimestamp;
				}
			}
			if (cpl.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated &&
			cpl.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
			{
				if (bVolCorecTypeMajor == Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_ASTM_COMM_2004)
				{
					if (cpl.Value == null || (double)cpl.Value != 1.0)
					{
						cpl.Value = 1.0;
						cpl.Status = StatusCodes.Uncertain;
						cpl.ServerTimeStamp = serverTimestamp;
						cpl.SourceTimeStamp = sourceTimestamp;
					}
				}
				else
				{
					if (cpl.Value != null)
					{
						cpl.Value = null;
						cpl.Status = StatusCodes.BadNotImplemented;
						cpl.ServerTimeStamp = serverTimestamp;
						cpl.SourceTimeStamp = sourceTimestamp;
					}
				}
			}
		}

		private void SetVariablesasFailed(Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor bVolCorecTypeMajor, ref PointTag ctpl, ref PointTag ctl, ref PointTag returnedvcf, ref PointTag unroundedVcf, ref PointTag cpl)
		{
			if (ctpl.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated &&
			ctpl.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
			{
				if (ctpl.Value == null || (double)ctpl.Value != 1.0)
				{
					ctpl.Value = 1.0;
					ctpl.Status = StatusCodes.Bad;
					ctpl.ServerTimeStamp = DateTimeOffset.UtcNow;
					ctpl.SourceTimeStamp = DateTimeOffset.UtcNow;
				}
			}
			if (ctl.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated &&
			ctl.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
			{
				if (ctl.Value == null || (double)ctl.Value != 1.0)
				{
					ctl.Value = 1.0;
					ctl.Status = StatusCodes.Bad;
					ctl.ServerTimeStamp = DateTimeOffset.UtcNow;
					ctl.SourceTimeStamp = DateTimeOffset.UtcNow;
				}
			}
			if (returnedvcf.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated &&
			returnedvcf.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
			{
				if (returnedvcf.Value == null || (double)returnedvcf.Value != 1.0)
				{
					returnedvcf.Value = 1.0;
					returnedvcf.Status = StatusCodes.Bad;
					returnedvcf.ServerTimeStamp = DateTimeOffset.UtcNow;
					returnedvcf.SourceTimeStamp = DateTimeOffset.UtcNow;
				}
			}
			if (unroundedVcf.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated &&
			unroundedVcf.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
			{
				if (unroundedVcf.Value == null || (double)unroundedVcf.Value != 1.0)
				{
					unroundedVcf.Value = 1.0;
					unroundedVcf.Status = StatusCodes.Bad;
					unroundedVcf.ServerTimeStamp = DateTimeOffset.UtcNow;
					unroundedVcf.SourceTimeStamp = DateTimeOffset.UtcNow;
				}
			}
			if (cpl.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated &&
			cpl.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
			{
				if (bVolCorecTypeMajor == Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_ASTM_COMM_2004)
				{
					if (cpl.Value == null || (double)cpl.Value != 1.0)
					{
						cpl.Value = 1.0;
						cpl.Status = StatusCodes.Bad;
						cpl.ServerTimeStamp = DateTimeOffset.UtcNow;
						cpl.SourceTimeStamp = DateTimeOffset.UtcNow;
					}
				}
				else
				{
					if (cpl.Value != null)
					{
						cpl.Value = null;
						cpl.Status = StatusCodes.BadNotImplemented;
						cpl.ServerTimeStamp = DateTimeOffset.UtcNow;
						cpl.SourceTimeStamp = DateTimeOffset.UtcNow;
					}
				}
			}
		}

		private void SetDensityValueandStatus(PointTag FromDensity, ref PointTag ToDensity)
		{
			double dTempValue = 0.0;
			uint OpcStatusCodeBits;
			if (!IsValueGood(FromDensity))
			{
				OpcStatusCodeBits = new StatusCode((uint)ToDensity.Status).CodeBits;
				if (ToDensity.Value != null ||
				 OpcStatusCodeBits != StatusCodes.Bad)
				{
					ToDensity.Value = null;
					ToDensity.Status = StatusCodes.Bad;
					ToDensity.ServerTimeStamp = FromDensity.ServerTimeStamp;
					ToDensity.SourceTimeStamp = FromDensity.SourceTimeStamp;
				}
			}
			else
			{
				if (FromDensity.Value != null)
				{
					OpcStatusCodeBits = new StatusCode((uint)ToDensity.Status).CodeBits;
					EngineeringUnits.Convert((double)FromDensity.Value, FromDensity.Units, ref dTempValue, ToDensity.Units, 60.0);
					if (ToDensity.Value == null ||
						 (double)ToDensity.Value != dTempValue ||
						 OpcStatusCodeBits != StatusCodes.Good)
					{
						ToDensity.Value = dTempValue;
						ToDensity.Status = StatusCodes.Good;
						ToDensity.ServerTimeStamp = FromDensity.ServerTimeStamp;
						ToDensity.SourceTimeStamp = FromDensity.SourceTimeStamp;
						CheckForAndSetOverUnderRange(ToDensity);
					}
				}
			}

		}

	}
}
