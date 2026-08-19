using System;
namespace FloatingRoofCorrection
{
	using System;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;
	using Varec.CommonComponents.EngineeringUnitsLibrary;
	using FMBusinessObjects.DataObjects.CodedVariables;
	using FMPointCommon;
	using Opc.Ua;

	public class FMFloatingRoofCorrection : FuelsManagerModule, IFuelsManagerModule
	{
		public StrapTable StrapTable { get; set; }

		public QuantityModuleSettings QuantitySettings { get; set; }

		public FMFloatingRoofCorrection() : base() { }

		public bool? FloatingRoofCorrectionCalculation(PointTag ProdTemperature,
														PointTag ProdDensity,
														PointTag DensityInAir,
														PointTag Mass,
														PointTag Level,
														PointTag VCF,
														PointTag CriticalZone,
														PointTag RoofVolume)
		{
			double LevelInFeet = 0.0;
			double RoofLandingHeightFeet = 0.0;
			double RoofFloatingHeightFeet = 0.0;
			bool ValueToSet = false;

			// Can any module calculations be run?
			if (StatusCode.IsBad(Level.OpcStatusSubCode) ||
				!(Level.Value is double?) ||
				!((double?)Level.Value).HasValue ||
				!StrapTable.StrapInRange)
			{
				if (CriticalZone.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated &&
				CriticalZone.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
				{
					if (CriticalZone.Value != null ||
						CriticalZone.Status != StatusCodes.Bad)
					{
						CriticalZone.Status = StatusCodes.Bad;
						CriticalZone.Value = null;
						base.SetTimeStamps(new PointTag[] { Level }, CriticalZone);
					}
				}

				if (RoofVolume.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated ||
				RoofVolume.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
				{
					if (RoofVolume.Value != null ||
						RoofVolume.Status != StatusCodes.Bad)
					{
						RoofVolume.Status = StatusCodes.Bad;
						RoofVolume.Value = null;
                  base.SetTimeStamps(new PointTag[] { Level }, RoofVolume);
                    }
				}

				return false;
			}

			double StrapTableRoofLandingHeight = 0.0;
			double StrapTableRoofFloatingHeight = 0.0;
			RoofTypeEnum StrapTableRoofType = RoofTypeEnum.RoofMassNotInStrap;
			StrapTableRoofLandingHeight = StrapTable.RoofLandingHeight.Value;
			StrapTableRoofFloatingHeight = StrapTable.RoofFloatingHeight.Value;

			EngineeringUnits.Convert((double)Level.Value, Level.Units, ref LevelInFeet, EngineeringUnit.FmlFeet, 60.0);
			EngineeringUnits.Convert(StrapTableRoofLandingHeight, Level.Units, ref RoofLandingHeightFeet, EngineeringUnit.FmlFeet, 60.0);
			EngineeringUnits.Convert(StrapTableRoofFloatingHeight, Level.Units, ref RoofFloatingHeightFeet, EngineeringUnit.FmlFeet, 60.0);


			// Determine Critical Zone
			if (CriticalZone.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated &&
			CriticalZone.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
			{
				if (StrapTableRoofType == RoofTypeEnum.RoofMassInStrap ||
				StrapTableRoofType == RoofTypeEnum.RoofMassNotInStrap)
				{
					if (LevelInFeet < RoofFloatingHeightFeet)
					{
						// in the critical zone
						if (LevelInFeet >= RoofLandingHeightFeet)
						{
							ValueToSet = true;
						}
					}
				}

				if (CriticalZone.Value == null ||
					(bool)CriticalZone.Value != ValueToSet)
				{
					CriticalZone.Value = ValueToSet;
					CriticalZone.Status = StatusCodes.Good;
               base.SetTimeStamps(new PointTag[] { Level }, CriticalZone);
                }
			}

			if (RoofVolume.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
			RoofVolume.OpcStatusSubCode == StatusCodes.GoodLocalOverride ||
			CriticalZone.Status == StatusCodes.Bad)
			{
				return false;
			}


			double StrapTableRoofMass = 0.0;
			double StrapTableStrapDensity = 0.0;

			StrapTableRoofMass = StrapTable.RoofMass.Value;
			StrapTableRoofType = StrapTable.RoofType;
			StrapTableStrapDensity = StrapTable.StrapDensity.Value;


			if (StrapTableRoofMass == 0.0 ||
			StrapTableRoofType == RoofTypeEnum.FixedRoof ||
			StrapTableRoofType == RoofTypeEnum.NoRoof ||
			(bool)CriticalZone.Value == true ||
			LevelInFeet < RoofFloatingHeightFeet)
			{
				if (RoofVolume.Value == null ||
					(double)RoofVolume.Value != 0.0)
				{
					RoofVolume.Value = 0.0;
					RoofVolume.Status = StatusCodes.Good;
               base.SetTimeStamps(new PointTag[] { CriticalZone, Level }, RoofVolume);
                }

				return true;
			}


			double RoofMassInKg = 0.0;
			double RoofVolInM3 = 0.0;

			EngineeringUnits.Convert(StrapTableRoofMass, Mass.Units, ref RoofMassInKg, EngineeringUnit.FmmKg, 60.0);

			if (QuantitySettings.VolumeCalculationType == VolumeCalculationType.API1995Calculations)
			{
				if(!IsValueGood(ProdDensity)
				|| (double)ProdDensity.Value == 0.0)
				{ 
					if (RoofVolume.Value != null ||
						RoofVolume.Status != StatusCodes.Bad)
					{
						RoofVolume.Status = StatusCodes.Bad;
						RoofVolume.Value = null;
                  base.SetTimeStamps(new PointTag[] { ProdDensity }, RoofVolume);

                    }
					return true;
				}

				double DensityInKgPerM3 = 0.0;
				EngineeringUnits.Convert((double)ProdDensity.Value, ProdDensity.Units, ref DensityInKgPerM3, EngineeringUnit.FmdKgM3, 60.0);

				if (StrapTableRoofType == RoofTypeEnum.RoofMassNotInStrap)
				{
					if (DensityInKgPerM3 != 0.0)
					{
						RoofVolInM3 = (RoofMassInKg / DensityInKgPerM3) * -1.0;
					}
					else
					{
						if (RoofVolume.Value != null ||
							RoofVolume.Status != StatusCodes.Bad)
						{
							RoofVolume.Status = StatusCodes.Bad;
							RoofVolume.Value = null;
                     base.SetTimeStamps(new PointTag[] { ProdDensity }, RoofVolume);
                        }
					}
				}
				else
				{
					double StrapDensityInKgPerM3 = 0.0;
					EngineeringUnits.Convert(StrapTableStrapDensity, ProdDensity.Units, ref StrapDensityInKgPerM3, EngineeringUnit.FmdKgM3, 60.0);

					if (StrapDensityInKgPerM3 != 0.0 && DensityInKgPerM3 != 0.0)
					{
						double StrapRoofVolume = RoofMassInKg / StrapDensityInKgPerM3;

						RoofVolInM3 = StrapRoofVolume - (RoofMassInKg / DensityInKgPerM3);
					}
					else
					{
						if (RoofVolume.Value != null ||
							RoofVolume.Status != StatusCodes.Bad)
						{
							RoofVolume.Status = StatusCodes.Bad;
							RoofVolume.Value = null;
                     base.SetTimeStamps(new PointTag[] { ProdDensity }, RoofVolume);
                        }
					}

				}
			}

			// api2012 calculations
			else
			{
				if (StrapTableRoofType == RoofTypeEnum.RoofMassNotInStrap)
				{
					if (!IsValueGood(DensityInAir)
					|| !IsValueGood(VCF)
					|| (double)DensityInAir.Value == 0.0
					|| (double)VCF.Value == 0.0)
					{
						if (RoofVolume.Value != null ||
							RoofVolume.Status != StatusCodes.Bad)
						{
							RoofVolume.Status = StatusCodes.Bad;
							RoofVolume.Value = null;
                     base.SetTimeStamps(new PointTag[] { DensityInAir, VCF }, RoofVolume);
                        }
						return true;
					}

					double DensityInAirInKgPerM3 = 0.0;
					EngineeringUnits.Convert((double)DensityInAir.Value, DensityInAir.Units, ref DensityInAirInKgPerM3, EngineeringUnit.FmdKgM3, 60.0);

					if (DensityInAirInKgPerM3 != 0.0 && (double)VCF.Value != 0.0)
					{
						RoofVolInM3 = (RoofMassInKg / (DensityInAirInKgPerM3 * (double)VCF.Value)) * -1.0;
					}
					else
					{
						if (RoofVolume.Value != null ||
							RoofVolume.Status != StatusCodes.Bad)
						{
							RoofVolume.Status = StatusCodes.Bad;
							RoofVolume.Value = null;
                     base.SetTimeStamps(new PointTag[] { DensityInAir, VCF }, RoofVolume);
                        }
					}
				}
				else
				{
					if (!IsValueGood(ProdDensity)
					|| !IsValueGood(VCF)
					|| (double)ProdDensity.Value == 0.0
					|| (double)VCF.Value == 0.0)
					{
						if (RoofVolume.Value != null ||
						    RoofVolume.Status != StatusCodes.Bad)
						{
							RoofVolume.Status = StatusCodes.Bad;
							RoofVolume.Value = null;
                     base.SetTimeStamps(new PointTag[] { ProdDensity, VCF }, RoofVolume);
                        }
						return true;
					}


					double StrapDensityInKgPerM3 = 0.0;
					EngineeringUnits.Convert(StrapTableStrapDensity, ProdDensity.Units, ref StrapDensityInKgPerM3, EngineeringUnit.FmdKgM3, 60.0);

					double DensityInKgPerM3 = 0.0;
					EngineeringUnits.Convert((double)ProdDensity.Value, ProdDensity.Units, ref DensityInKgPerM3, EngineeringUnit.FmdKgM3, 60.0);

					if (StrapDensityInKgPerM3 != 0.0 && DensityInKgPerM3 != 0.0)
					{
						double StrapRoofVolume = RoofMassInKg / StrapDensityInKgPerM3;

						RoofVolInM3 = StrapRoofVolume - (RoofMassInKg / DensityInKgPerM3);
					}
					else
					{
						if (RoofVolume.Value != null ||
							RoofVolume.Status != StatusCodes.Bad)
						{
							RoofVolume.Status = StatusCodes.Bad;
							RoofVolume.Value = null;
                     base.SetTimeStamps(new PointTag[] { ProdDensity }, RoofVolume);
                        }
					}
				}
			}

			EngineeringUnits.Convert(RoofVolInM3, EngineeringUnit.FmvMeter3, ref RoofVolInM3, RoofVolume.Units, 60.0);

			long newStatus = StatusCodes.Good;

			if (QuantitySettings.VolumeCalculationType == VolumeCalculationType.API1995Calculations)
			{
				if (IsStatusUncertain(ProdDensity))
				{
					newStatus = StatusCodes.Uncertain;
				}
			}
			else
			{
				if (IsStatusUncertain(DensityInAir)
				|| IsStatusUncertain(VCF))
				{
					newStatus = StatusCodes.Uncertain;
				}
			}

			if (RoofVolume.Value == null
			||	(double)RoofVolume.Value != RoofVolInM3
			|| IsStatusChange(RoofVolume.Status, newStatus))
			{
				RoofVolume.Value = RoofVolInM3;
				RoofVolume.Status = newStatus;
				CheckForAndSetOverUnderRange(RoofVolume);


				if (QuantitySettings.VolumeCalculationType == VolumeCalculationType.API1995Calculations)
				{
					RoofVolume.ServerTimeStamp = ProdDensity.ServerTimeStamp;
					RoofVolume.SourceTimeStamp = ProdDensity.SourceTimeStamp;
				}
				else if(DensityInAir.SourceTimeStamp > VCF.SourceTimeStamp)
				{
					RoofVolume.ServerTimeStamp = DensityInAir.ServerTimeStamp;
					RoofVolume.SourceTimeStamp = DensityInAir.SourceTimeStamp;
				}
				else
				{
					RoofVolume.ServerTimeStamp = VCF.ServerTimeStamp;
					RoofVolume.SourceTimeStamp = VCF.SourceTimeStamp;
				}
			}

			return true;
		}

		// required interface
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
									ID = "Density Product Observed",
									Type = typeof(double?),
									ParameterType = ModuleInputOutputType.Input
								},
								new ModuleInputOutput
								{
									ID = "Styandard Density In Air",
									Type = typeof(double?),
									ParameterType = ModuleInputOutputType.Input
								},
								new ModuleInputOutput
								{
									ID = "Mass",
									Type = typeof(double?),
									ParameterType = ModuleInputOutputType.Input
								},
								new ModuleInputOutput
								{
									ID = "Level",
									Type = typeof(double?),
									ParameterType = ModuleInputOutputType.Input
								},
								new ModuleInputOutput
								{
									ID = "Volume Correction for Press and Temp",
									Type = typeof(double?),
									ParameterType = ModuleInputOutputType.Input
								},
								new ModuleInputOutput
								{
									ID = "Critical Zone",
									Type = typeof(double?),
									ParameterType = ModuleInputOutputType.InOut
								},
								new ModuleInputOutput
								{
									ID = "Volume Roof Correction",
									Type = typeof(double?),
									ParameterType = ModuleInputOutputType.InOut
								}
							};
			return properties;
		}
	}
}
