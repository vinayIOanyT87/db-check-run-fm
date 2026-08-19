
namespace Quantities
{
	using System;
	using System.Collections.Generic;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;
	using FMBusinessObjects.DataObjects.CodedVariables;
	using Varec.CommonComponents.EngineeringUnitsLibrary;
	using FMPointCommon;
	using Opc.Ua;
	public class FMQuantities : FuelsManagerModule, IFuelsManagerModule
	{
		public QuantityModuleSettings QuantitySettings { get; set; }

		public VcfModuleSettings VcfSettings { get; set; }

		public Vessel Vessel { get; set; }

		public FMQuantities() : base() { }


		public bool? QuantityCalculation(PointTag RoofVolume,
											PointTag StrapVolume,
											PointTag WaterVolume,
											PointTag SolidsVolume,
											PointTag PercentBSW,
											PointTag VCF,
											PointTag VCFUnrounded,
											PointTag StdDensity,
											PointTag ProductDensity,
											PointTag TankShellCorrection,
											PointTag DensityInAir,
											PointTag StdDensityInAir,
											PointTag DensityVapor,
											PointTag TemperatureVapor,
											PointTag PressureVapor,
											PointTag BottomVolume,
											PointTag GrossObsVolume,
											PointTag NetStandardVolume,
											PointTag NetStandardVolumeUnrounded,
											PointTag Mass,
											PointTag GrossStdWeight,
											PointTag GrossStdVolume,
											PointTag NetStdWeight,
											PointTag BSWVolume,
											PointTag TotalCalculatedVolume,
											PointTag VolumeVaporNet,
											PointTag MassVapor)
		{
			// Calculate total bottom volume
			CalculateBottomsVolume(WaterVolume, SolidsVolume, ref BottomVolume);

			// Calculate gross volume
			CalculateGrossObserverdVolume(RoofVolume, StrapVolume, BottomVolume, TankShellCorrection, GrossObsVolume);

			// Calculate gross standard volume
			CalculateGrossStandardVolume(GrossObsVolume, VCF, GrossStdVolume);

			// Calculate bsw volume
			CalculateBSWVolume(GrossObsVolume, BottomVolume, GrossStdVolume, PercentBSW, BSWVolume);

			// Calculate total calculated volume
			CalculateTotalCalculatedVolume(GrossStdVolume, BottomVolume, TotalCalculatedVolume);

			// Calculate net standard volume
			CalculateNetStandardVolume(GrossObsVolume, PercentBSW, VCF, NetStandardVolume);

			// Calculate net standard volume unrounded
			CalculateNetStandardVolumeUnrounded(GrossObsVolume, PercentBSW, VCFUnrounded, NetStandardVolumeUnrounded);

			// Calculate the product mass
			CalculateLiquidMass(GrossObsVolume, NetStandardVolume, StdDensity, StdDensityInAir, Mass);

			// Calculate Gross Standard Weight
			CalculateStdWeight(GrossStdVolume, DensityInAir, GrossStdWeight);

			// Calculate Net Standard Weight
			CalculateStdWeight(NetStandardVolume, DensityInAir, NetStdWeight);

			// Calculate Vapor Mass first
			CalculateVaporMass(GrossObsVolume, DensityVapor, TemperatureVapor, PressureVapor, StdDensity, MassVapor);

			// Calculate the vapor net volume
			CalculateVaporNetVolume(StdDensity, MassVapor, VolumeVaporNet);

			return true;
		}


		public bool? QuantityCalculationVolumeOnly(PointTag RoofVolume,
											PointTag StrapVolume,
											PointTag WaterVolume,
											PointTag SolidsVolume,
											PointTag PercentBSW,
											PointTag VCF,
											PointTag TankShellCorrection,
											PointTag BottomVolume,
											PointTag GrossObsVolume,
											PointTag NetStandardVolume)
		{
			// Calculate total bottom volume
			CalculateBottomsVolume(WaterVolume, SolidsVolume, ref BottomVolume);

			// Calculate gross volume
			CalculateGrossObserverdVolume(RoofVolume, StrapVolume, BottomVolume, TankShellCorrection, GrossObsVolume);

			// Calculate net standard volume
			CalculateNetStandardVolume(GrossObsVolume, PercentBSW, VCF, NetStandardVolume);

			return true;
		}


		public ModuleInputOutputCollection GetInputOutputCollection(string calculationName)
		{
			var properties = new ModuleInputOutputCollection
							{
								new ModuleInputOutput
								{
									ID = "Volume Total Observed",
									Type = typeof(double?),
									ParameterType = ModuleInputOutputType.Input
								},
								new ModuleInputOutput
								{
									ID = "Roof Correction",
									Type = typeof(double?),
									ParameterType = ModuleInputOutputType.Input
								},
								new ModuleInputOutput
								{
									ID = "Volume Bottom",
									Type = typeof(double?),
									ParameterType = ModuleInputOutputType.Input
								},
								new ModuleInputOutput
								{
									ID = "Percent BSW",
									Type = typeof(double?),
									ParameterType = ModuleInputOutputType.Input
								},
								new ModuleInputOutput
								{
									ID = "Tank Shell Correction",
									Type = typeof(double?),
									ParameterType = ModuleInputOutputType.Input
								},
								new ModuleInputOutput
								{
									ID = "Volume Water",
									Type = typeof(double?),
									ParameterType = ModuleInputOutputType.Input
								},
								new ModuleInputOutput
								{
									ID = "Volume Gross Observed",
									Type = typeof(double?),
									ParameterType = ModuleInputOutputType.Output
								},
							};
			return properties;
		}

		private void CalculateBottomsVolume(PointTag WaterVolume, PointTag SolidsVolume, ref PointTag BottomVolume)
		{
			if (BottomVolume.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
				BottomVolume.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}

			if (!IsValueGood(WaterVolume) ||
				!IsValueGood(SolidsVolume))
			{
				if (BottomVolume.Value != null
				|| BottomVolume.Status != StatusCodes.Bad)
				{
					BottomVolume.Value = null;
					BottomVolume.Status = StatusCodes.Bad;
					base.SetTimeStamps(new PointTag[] { SolidsVolume, WaterVolume, }, BottomVolume);

					}

				return;
			}

			// calculate the total bottoms volume
			double watervolumem3 = 0.0;
			double solidsvolumem3 = 0.0;
			double bottomsvolumem3 = 0.0;

			EngineeringUnits.Convert((double)WaterVolume.Value, WaterVolume.Units, ref watervolumem3, EngineeringUnit.FmvMeter3, 60.0);
			EngineeringUnits.Convert((double)SolidsVolume.Value, SolidsVolume.Units, ref solidsvolumem3, EngineeringUnit.FmvMeter3, 60.0);

			// bottom volume is just water volume + solids volume
			bottomsvolumem3 = watervolumem3 + solidsvolumem3;

			EngineeringUnits.Convert(bottomsvolumem3, EngineeringUnit.FmvMeter3, ref bottomsvolumem3, BottomVolume.Units, 60.0);

			long newStatus = StatusCodes.Good;

			// if either of the variables are over/under ranged set status to warning
			if (IsStatusUncertain(WaterVolume) ||
				IsStatusUncertain(SolidsVolume))
			{
				newStatus = StatusCodes.Uncertain;
			}


			if (BottomVolume.Value == null
			|| (double)BottomVolume.Value != bottomsvolumem3
			|| IsStatusChange(BottomVolume.Status, newStatus))
			{
				BottomVolume.Value = bottomsvolumem3;
				BottomVolume.Status = newStatus;

				CheckForAndSetOverUnderRange(BottomVolume);

				// determine which time stamp to use
				if (SolidsVolume.SourceTimeStamp > WaterVolume.SourceTimeStamp)
				{
					BottomVolume.ServerTimeStamp = SolidsVolume.ServerTimeStamp;
					BottomVolume.SourceTimeStamp = SolidsVolume.SourceTimeStamp;
				}
				else
				{
					BottomVolume.ServerTimeStamp = WaterVolume.ServerTimeStamp;
					BottomVolume.SourceTimeStamp = WaterVolume.SourceTimeStamp;
				}
			}
		}


		public void CalculateStrapVolumeFromGrossObservedVolume(PointTag RoofVolume, PointTag GrossObsVolume, PointTag BottomVolume, PointTag TankShellCorrection, PointTag StrapVolume)
		{
			if (StrapVolume.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
			StrapVolume.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}

			if (!IsValueGood(RoofVolume) ||
				!IsValueGood(BottomVolume) ||
				!IsValueGood(TankShellCorrection) ||
				!IsValueGood(GrossObsVolume))
			{
				if (StrapVolume.Value != null
				|| StrapVolume.Status != StatusCodes.Bad)
				{
					StrapVolume.Value = null;
					StrapVolume.Status = StatusCodes.Bad;
						if ((double)RoofVolume.Value != 0.0)
								base.SetTimeStamps(new PointTag[] { RoofVolume, GrossObsVolume, BottomVolume, TankShellCorrection }, StrapVolume);
						else
								base.SetTimeStamps(new PointTag[] { GrossObsVolume, BottomVolume, TankShellCorrection }, StrapVolume);
					}

				return;
			}

			// calculate the strap volume
			// convert the units to m3
			double grossVolm3 = 0.0;
			double strapVolm3 = 0.0;
			double bottomVolm3 = 0.0;
			double roofVolm3 = 0.0;

			EngineeringUnits.Convert((double)BottomVolume.Value, BottomVolume.Units, ref bottomVolm3, EngineeringUnit.FmvMeter3, 60.0);
			EngineeringUnits.Convert((double)GrossObsVolume.Value, GrossObsVolume.Units, ref grossVolm3, EngineeringUnit.FmvMeter3, 60.0);
			EngineeringUnits.Convert((double)RoofVolume.Value, RoofVolume.Units, ref roofVolm3, EngineeringUnit.FmvMeter3, 60.0);

			strapVolm3 = ((grossVolm3 - roofVolm3) / (double)TankShellCorrection.Value) + bottomVolm3;

			double newValue = 0.0;
			EngineeringUnits.Convert(strapVolm3, EngineeringUnit.FmvMeter3, ref newValue, StrapVolume.Units, 60.0);

			long newStatus = StatusCodes.Good;

			// if either of the variables are over/under ranged set status to warning
			if (IsStatusUncertain(RoofVolume) ||
				IsStatusUncertain(GrossObsVolume) ||
				IsStatusUncertain(BottomVolume) ||
				IsStatusUncertain(TankShellCorrection))
			{
				newStatus = StatusCodes.Uncertain;
			}

			if (StrapVolume.Value == null
			|| (double)StrapVolume.Value != newValue
			|| IsStatusChange(StrapVolume.Status, newStatus))
			{
				StrapVolume.Value = newValue;
				StrapVolume.Status = newStatus;
				CheckForAndSetOverUnderRange(StrapVolume);
					if ((double)RoofVolume.Value != 0.0)
						base.SetTimeStamps(new PointTag[] { RoofVolume, GrossObsVolume, BottomVolume, TankShellCorrection }, StrapVolume);
					else
						base.SetTimeStamps(new PointTag[] { GrossObsVolume, BottomVolume, TankShellCorrection }, StrapVolume);
				}
		}

		public void CalculateGrossObserverdVolume(PointTag RoofVolume, PointTag StrapVolume, PointTag BottomVolume, PointTag TankShellCorrection,PointTag GrossObsVolume, bool AllowNegativeGrossVolume = false)
		{
			// for api 1980 and api 2004 we do not care about bottom volume, bsw, water volume
			if (GrossObsVolume.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
				GrossObsVolume.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}

			if (!IsValueGood(RoofVolume) ||
				!IsValueGood(BottomVolume) ||
				!IsValueGood(TankShellCorrection) ||
				!IsValueGood(StrapVolume))
			{
				if (GrossObsVolume.Value != null
				|| GrossObsVolume.Status != StatusCodes.Bad)
				{
					GrossObsVolume.Value = null;
					GrossObsVolume.Status = StatusCodes.Bad;
					if (RoofVolume.Value is double && (double) RoofVolume.Value != 0.0)
						base.SetTimeStamps(new PointTag[] { RoofVolume, BottomVolume, TankShellCorrection, StrapVolume }, GrossObsVolume);
					else
						base.SetTimeStamps(new PointTag[] { BottomVolume, TankShellCorrection, StrapVolume }, GrossObsVolume);
				}

				return;
			}

			// calculate the gross volume
			// convert the units to m3
			double grossVolm3 = 0.0;
			double strapVolm3 = 0.0;
			double bottomVolm3 = 0.0;
			double roofVolm3 = 0.0;

			EngineeringUnits.Convert((double)BottomVolume.Value, BottomVolume.Units, ref bottomVolm3, EngineeringUnit.FmvMeter3, 60.0);
			EngineeringUnits.Convert((double)StrapVolume.Value, StrapVolume.Units, ref strapVolm3, EngineeringUnit.FmvMeter3, 60.0);
			EngineeringUnits.Convert((double)RoofVolume.Value, RoofVolume.Units, ref roofVolm3, EngineeringUnit.FmvMeter3, 60.0);

			grossVolm3 = ((strapVolm3 - bottomVolm3) * (double)TankShellCorrection.Value) + roofVolm3;

			double newValue = 0.0;
			EngineeringUnits.Convert(grossVolm3, EngineeringUnit.FmvMeter3, ref newValue, GrossObsVolume.Units, 60.0);

			// negative gross volume is not allowed
			if (newValue < 0 && !AllowNegativeGrossVolume)
				newValue = 0.0;

			long newStatus = StatusCodes.Good;

			// if either of the variables are over/under ranged set status to warning
			if (IsStatusUncertain(RoofVolume) ||
				IsStatusUncertain(StrapVolume) ||
				IsStatusUncertain(BottomVolume) ||
				IsStatusUncertain(TankShellCorrection))
			{
				newStatus = StatusCodes.Uncertain;
			}

			if (GrossObsVolume.Value == null
			|| (double)GrossObsVolume.Value != newValue
			|| IsStatusChange(GrossObsVolume.Status, newStatus))
			{
				GrossObsVolume.Value = newValue;
				GrossObsVolume.Status = newStatus;
				CheckForAndSetOverUnderRange(GrossObsVolume);
					// test the roof type and if roof mass or roof mass not in strap, include in 
					if ((double)RoofVolume.Value != 0.0)
				base.SetTimeStamps(new PointTag[] { RoofVolume, StrapVolume, BottomVolume, TankShellCorrection }, GrossObsVolume);
					else
				base.SetTimeStamps(new PointTag[] { StrapVolume, BottomVolume, TankShellCorrection }, GrossObsVolume);
				}
		}

		public void CalculateGrossStandardVolume(PointTag GrossObsVolume, PointTag VCF, PointTag GrossStdVolume)
		{
			// for api 1980 and api 2004 we do not care about bottom volume, bsw, water volume
			if (GrossStdVolume.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
				GrossStdVolume.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}

			if (QuantitySettings.VolumeCalculationType == VolumeCalculationType.API1995Calculations ||
				!IsValueGood(GrossObsVolume) ||
				!IsValueGood(VCF))
			{
				if (GrossStdVolume.Value != null
				|| GrossStdVolume.Status != StatusCodes.Bad)
				{
					GrossStdVolume.Value = null;
					GrossStdVolume.Status = StatusCodes.Bad;
					base.SetTimeStamps(new PointTag[] { VCF, GrossObsVolume }, GrossStdVolume);
					}
			}
			else
			{
				// gross standard volume is just gross volume times vcf
				double grossobsvolinm3 = 0.0;
				double grossstdvolinm3 = 0.0;

				EngineeringUnits.Convert((double)GrossObsVolume.Value, GrossObsVolume.Units, ref grossobsvolinm3, EngineeringUnit.FmvMeter3, 60.0);

				grossstdvolinm3 = grossobsvolinm3 * (double)VCF.Value;

				double newValue = 0.0;

				EngineeringUnits.Convert(grossstdvolinm3, EngineeringUnit.FmvMeter3, ref newValue, GrossStdVolume.Units, 60.0);

				long newStatus = StatusCodes.Good;

				// if either of the variables are over/under ranged set status to warning
				if (IsStatusUncertain(GrossObsVolume) ||
					IsStatusUncertain(VCF))
				{
					newStatus = StatusCodes.Uncertain;
				}


				if (GrossStdVolume.Value == null
				|| (double)GrossStdVolume.Value != newValue
				|| IsStatusChange(GrossStdVolume.Status, newStatus))
				{
					GrossStdVolume.Value = newValue;
					GrossStdVolume.Status = newStatus;
					CheckForAndSetOverUnderRange(GrossStdVolume);

					// determine which time stamp to use
					if (VCF.SourceTimeStamp > GrossObsVolume.SourceTimeStamp)
					{
						GrossStdVolume.ServerTimeStamp = VCF.ServerTimeStamp;
						GrossStdVolume.SourceTimeStamp = VCF.SourceTimeStamp;
					}
					else
					{
						GrossStdVolume.ServerTimeStamp = GrossObsVolume.ServerTimeStamp;
						GrossStdVolume.SourceTimeStamp = GrossObsVolume.SourceTimeStamp;
					}
				}
			}
		}


		public void CalculateGrossObservedVolumeFromNetStandardVolume(PointTag NetStandardVolume, PointTag PercentBSW, PointTag VCF, PointTag GrossObsVolume)
		{
			// make sure gross volume is calculated
			if (GrossObsVolume.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
				GrossObsVolume.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}

			// make sure all of the data is valid
			if (!IsValueGood(NetStandardVolume)
			|| !IsValueGood(PercentBSW)
			|| !IsValueGood(VCF))
			{
				if (GrossObsVolume.Value != null
				|| GrossObsVolume.Status != StatusCodes.Bad)
				{
					GrossObsVolume.Value = null;
					GrossObsVolume.Status = StatusCodes.Bad;
					base.SetTimeStamps(new PointTag[] { NetStandardVolume, PercentBSW, VCF }, GrossObsVolume);
				}
				return;
			}

			double netstandardvolumem3 = 0.0;

			// convert all volumes to the same units
			EngineeringUnits.Convert((double)NetStandardVolume.Value, NetStandardVolume.Units, ref netstandardvolumem3, EngineeringUnit.FmvMeter3, 60.0);


			// calculate the gross observed volume
			double grossobsvolumem3 = netstandardvolumem3 / ((1.0 - (double)PercentBSW.Value / 100.0) * (double)VCF.Value);


			double newValue = 0.0;

			// convert to net volume units
			EngineeringUnits.Convert(grossobsvolumem3, EngineeringUnit.FmvMeter3, ref newValue, GrossObsVolume.Units, 60.0);

			long newStatus = StatusCodes.Good;

			// if either of the variables are over/under ranged set status to warning
			if (IsStatusUncertain(NetStandardVolume) ||
				IsStatusUncertain(PercentBSW) ||
				IsStatusUncertain(VCF))
			{
				newStatus = StatusCodes.Uncertain;
			}


			if (GrossObsVolume.Value == null
			|| (double)GrossObsVolume.Value != newValue
			|| IsStatusChange(GrossObsVolume.Status, newStatus))
			{
				GrossObsVolume.Value = newValue;
				GrossObsVolume.Status = newStatus;
				CheckForAndSetOverUnderRange(GrossObsVolume);
				base.SetTimeStamps(new PointTag[] { NetStandardVolume, PercentBSW, VCF }, GrossObsVolume);
			}
		}


		public void CalculateNetStandardVolume(PointTag GrossObsVolume, PointTag PercentBSW, PointTag VCF, PointTag NetStandardVolume)
		{
			// make sure net volume is calculated
			if (NetStandardVolume.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
				NetStandardVolume.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}

			// make sure all of the data is valid
			if (!IsValueGood(GrossObsVolume)
			|| !IsValueGood(PercentBSW)
			|| !IsValueGood(VCF))
			{
				if (NetStandardVolume.Value != null
				|| NetStandardVolume.Status != StatusCodes.Bad)
				{
					NetStandardVolume.Value = null;
					NetStandardVolume.Status = StatusCodes.Bad;
					base.SetTimeStamps(new PointTag[] { GrossObsVolume, PercentBSW, VCF }, NetStandardVolume);
				}
				return;
			}

			double grossobsvolumem3 = 0.0;
			double netstandardvolumem3 = 0.0;

			// convert all volumes to the same units
			EngineeringUnits.Convert((double)GrossObsVolume.Value, GrossObsVolume.Units, ref grossobsvolumem3, EngineeringUnit.FmvMeter3, 60.0);

			// calculate the net standard volume
			netstandardvolumem3 = (grossobsvolumem3) * (1.0 - (double)PercentBSW.Value / 100.0) * (double)VCF.Value;

			double newValue = 0.0;

			// convert to net volume units
			EngineeringUnits.Convert(netstandardvolumem3, EngineeringUnit.FmvMeter3, ref newValue, NetStandardVolume.Units, 60.0);

			long newStatus = StatusCodes.Good;

			// if either of the variables are over/under ranged set status to warning
			if (IsStatusUncertain(GrossObsVolume) ||
				IsStatusUncertain(PercentBSW) ||
				IsStatusUncertain(VCF))
			{
				newStatus = StatusCodes.Uncertain;
			}


			if (NetStandardVolume.Value == null
			|| (double)NetStandardVolume.Value != newValue
			|| IsStatusChange(NetStandardVolume.Status, newStatus))
			{
				NetStandardVolume.Value = newValue;
				NetStandardVolume.Status = newStatus;
				CheckForAndSetOverUnderRange(NetStandardVolume);


				NetStandardVolume.ServerTimeStamp = GrossObsVolume.ServerTimeStamp;
				NetStandardVolume.SourceTimeStamp = GrossObsVolume.SourceTimeStamp;

				if (PercentBSW.SourceTimeStamp > NetStandardVolume.SourceTimeStamp)
				{
					NetStandardVolume.ServerTimeStamp = PercentBSW.ServerTimeStamp;
					NetStandardVolume.SourceTimeStamp = PercentBSW.SourceTimeStamp;
				}

				if (VCF.SourceTimeStamp > NetStandardVolume.SourceTimeStamp)
				{
					NetStandardVolume.ServerTimeStamp = VCF.ServerTimeStamp;
					NetStandardVolume.SourceTimeStamp = VCF.SourceTimeStamp;
				}
			}
		}


		public void CalculateNetStandardVolumeUnrounded(PointTag GrossObsVolume, PointTag PercentBSW, PointTag VCFUnrounded, PointTag NetStandardVolumeUnrounded)
		{
			// make sure net volume is calculated
			if (NetStandardVolumeUnrounded.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
				NetStandardVolumeUnrounded.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}

			// make sure all of the data is valid
			if (!IsValueGood(GrossObsVolume)
			|| !IsValueGood(PercentBSW)
			|| !IsValueGood(VCFUnrounded))
			{
				if (NetStandardVolumeUnrounded.Value != null
				|| NetStandardVolumeUnrounded.Status != StatusCodes.Bad)
				{
					NetStandardVolumeUnrounded.Value = null;
					NetStandardVolumeUnrounded.Status = StatusCodes.Bad;
					base.SetTimeStamps(new PointTag[] { GrossObsVolume, PercentBSW, VCFUnrounded }, NetStandardVolumeUnrounded);
				}
				return;
			}

			double grossobsvolumem3 = 0.0;
			double NetStandardVolumeUnroundedm3 = 0.0;

			// convert all volumes to the same units
			EngineeringUnits.Convert((double)GrossObsVolume.Value, GrossObsVolume.Units, ref grossobsvolumem3, EngineeringUnit.FmvMeter3, 60.0);

			// calculate the net standard volume
			NetStandardVolumeUnroundedm3 = (grossobsvolumem3) * (1.0 - (double)PercentBSW.Value / 100.0) * (double)VCFUnrounded.Value;

			double newValue = 0.0;

			// convert to net volume units
			EngineeringUnits.Convert(NetStandardVolumeUnroundedm3, EngineeringUnit.FmvMeter3, ref newValue, NetStandardVolumeUnrounded.Units, 60.0);

			long newStatus = StatusCodes.Good;

			// if either of the variables are over/under ranged set status to warning
			if (IsStatusUncertain(GrossObsVolume) ||
				IsStatusUncertain(PercentBSW) ||
				IsStatusUncertain(VCFUnrounded))
			{
				newStatus = StatusCodes.Uncertain;
			}


			if (NetStandardVolumeUnrounded.Value == null
			|| (double)NetStandardVolumeUnrounded.Value != newValue
			|| IsStatusChange(NetStandardVolumeUnrounded.Status, newStatus))
			{
				NetStandardVolumeUnrounded.Value = newValue;
				NetStandardVolumeUnrounded.Status = newStatus;
				CheckForAndSetOverUnderRange(NetStandardVolumeUnrounded);


				NetStandardVolumeUnrounded.ServerTimeStamp = GrossObsVolume.ServerTimeStamp;
				NetStandardVolumeUnrounded.SourceTimeStamp = GrossObsVolume.SourceTimeStamp;

				if (PercentBSW.SourceTimeStamp > NetStandardVolumeUnrounded.SourceTimeStamp)
				{
					NetStandardVolumeUnrounded.ServerTimeStamp = PercentBSW.ServerTimeStamp;
					NetStandardVolumeUnrounded.SourceTimeStamp = PercentBSW.SourceTimeStamp;
				}

				if (VCFUnrounded.SourceTimeStamp > NetStandardVolumeUnrounded.SourceTimeStamp)
				{
					NetStandardVolumeUnrounded.ServerTimeStamp = VCFUnrounded.ServerTimeStamp;
					NetStandardVolumeUnrounded.SourceTimeStamp = VCFUnrounded.SourceTimeStamp;
				}
			}
		}

		public void CalculatePercentBSWFromNetBottomBSWVolumeVCF(PointTag NetStandardVolume, PointTag BottomVolume, PointTag BSWVolume, PointTag VCF, PointTag PercentBSW)
		{
			if (PercentBSW.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
			PercentBSW.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}

			// NetStandardVolume common to both API 1995 and API 2012 
			if (!IsValueGood(NetStandardVolume)
			|| !IsValueGood(BSWVolume))
			{
				if (PercentBSW.Value != null ||
					PercentBSW.Status != StatusCodes.Bad)
				{
					PercentBSW.Status = StatusCodes.Bad;
					PercentBSW.Value = null;
					base.SetTimeStamps(new PointTag[] { NetStandardVolume, BSWVolume }, PercentBSW);
				}
				return;
			}

			double percentBSW = 0.0;

			double netstandardvolumem3 = 0.0;
			double bottomVolumeM3 = 0.0;
			double bswVolumeM3 = 0.0;

			// convert all volumes to the same units
			EngineeringUnits.Convert((double)NetStandardVolume.Value, NetStandardVolume.Units, ref netstandardvolumem3, EngineeringUnit.FmvMeter3, 60.0);
			EngineeringUnits.Convert((double)BottomVolume.Value, BottomVolume.Units, ref bottomVolumeM3, EngineeringUnit.FmvMeter3, 60.0);
			EngineeringUnits.Convert((double)BSWVolume.Value, BSWVolume.Units, ref bswVolumeM3, EngineeringUnit.FmvMeter3, 60.0);


			if (QuantitySettings.VolumeCalculationType == VolumeCalculationType.API1995Calculations)
			{
				if((bswVolumeM3 + ((double)VCF.Value * bottomVolumeM3)) != 0.0 )
				{
						  //percentBSW = 100 * (netstandardvolumem3 -  (double)VCF.Value * bswVolumeM3) / - (bswVolumeM3 * ((double)VCF.Value * bottomVolumeM3));
						  percentBSW = (bswVolumeM3 + ((double)VCF.Value * bottomVolumeM3)) / (netstandardvolumem3 - (double)VCF.Value * bswVolumeM3) * 100;
				}
				else
				{
					percentBSW = 0;
				}

				if (!IsValueGood(BottomVolume)
				|| !IsValueGood(VCF))
				{
					if (PercentBSW.Value != null ||
						PercentBSW.Status != StatusCodes.Bad)
					{
						PercentBSW.Status = StatusCodes.Bad;
						PercentBSW.Value = null;
						base.SetTimeStamps(new PointTag[] { BottomVolume }, PercentBSW);
					}
					return;
				}




				long newStatus = StatusCodes.Good;

				// if any of the input variables are over/under ranged set status to warning
				if (IsStatusUncertain(NetStandardVolume) ||
					IsStatusUncertain(VCF) ||
					IsStatusUncertain(BottomVolume))
				{
					newStatus = StatusCodes.Uncertain;
				}


				if (PercentBSW.Value == null ||
				(double)PercentBSW.Value != percentBSW
				|| IsStatusChange(PercentBSW.Status, newStatus))
				{
					PercentBSW.Value = percentBSW;
					PercentBSW.Status = newStatus;
					CheckForAndSetOverUnderRange(PercentBSW);
					base.SetTimeStamps(new PointTag[] { NetStandardVolume, BottomVolume, BSWVolume, VCF }, PercentBSW);
				}
			}

			else
			{

				if (netstandardvolumem3 + bswVolumeM3 != 0.0)
				{
					percentBSW = 100 * bswVolumeM3 / (netstandardvolumem3 + bswVolumeM3);
				}
				else
				{
					percentBSW = 0.0;
				}

				long newStatus = StatusCodes.Good;
				// if any of the input variables are over/under ranged set status to warning
				if (IsStatusUncertain(NetStandardVolume)
				||	IsStatusUncertain(BSWVolume))
				{
					newStatus = StatusCodes.Uncertain;
				}

				if (PercentBSW.Value == null
				||	(double)PercentBSW.Value != percentBSW
				|| IsStatusChange(PercentBSW.Status, newStatus))
				{
					PercentBSW.Value = percentBSW;
					PercentBSW.Status = newStatus;
					CheckForAndSetOverUnderRange(PercentBSW);
					base.SetTimeStamps(new PointTag[] { NetStandardVolume, BSWVolume }, PercentBSW);
				}
			}
		}

		public void CalculateBSWVolume(PointTag GrossObsVolume, PointTag BottomVolume, PointTag GrossStdVolume, PointTag PercentBSW, PointTag BSWVolume)
		{
			if (BSWVolume.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
			BSWVolume.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}

			// PercentBSW common to both API 1995 and API 2012 
			if (StatusCode.IsBad(PercentBSW.OpcStatusSubCode) ||
			!(PercentBSW.Value is double?) ||
			!((double?)PercentBSW.Value).HasValue)
			{
				if (BSWVolume.Value != null ||
					BSWVolume.Status != StatusCodes.Bad)
				{
					BSWVolume.Status = StatusCodes.Bad;
					BSWVolume.Value = null;
					base.SetTimeStamps(new PointTag[] { PercentBSW }, BSWVolume);
				}
				return;
			}


			double baseSedmentAndWaterVolume = 0.0;

			if (QuantitySettings.VolumeCalculationType == VolumeCalculationType.API1995Calculations)
			{
				if (StatusCode.IsBad(BottomVolume.OpcStatusSubCode) ||
				!(BottomVolume.Value is double?) ||
				!((double?)BottomVolume.Value).HasValue)
				{
					if (BSWVolume.Value != null ||
						BSWVolume.Status != StatusCodes.Bad)
					{
						BSWVolume.Status = StatusCodes.Bad;
						BSWVolume.Value = null;
						base.SetTimeStamps(new PointTag[] { BottomVolume }, BSWVolume);
					}
					return;
				}

				if (StatusCode.IsBad(GrossObsVolume.OpcStatusSubCode) ||
				!(GrossObsVolume.Value is double?) ||
				!((double?)GrossObsVolume.Value).HasValue)
				{
					if (BSWVolume.Value != null ||
						BSWVolume.Status != StatusCodes.Bad)
					{
						BSWVolume.Status = StatusCodes.Bad;
						BSWVolume.Value = null;
						base.SetTimeStamps(new PointTag[] { GrossObsVolume }, BSWVolume);
					}
					return;
				}

				baseSedmentAndWaterVolume = ((double)GrossObsVolume.Value - (double)BottomVolume.Value) * (double)PercentBSW.Value / 100.0;


				long newStatus = StatusCodes.Good;

				// if any of the input variables are over/under ranged set status to warning
				if (IsStatusUncertain(GrossObsVolume) ||
					IsStatusUncertain(BottomVolume) ||
					IsStatusUncertain(PercentBSW))
				{
					newStatus = StatusCodes.Uncertain;
				}


				if (BSWVolume.Value == null ||
				(double)BSWVolume.Value != baseSedmentAndWaterVolume
				|| IsStatusChange(BSWVolume.Status, newStatus))
				{
					BSWVolume.Value = baseSedmentAndWaterVolume;
					BSWVolume.Status = newStatus;
					CheckForAndSetOverUnderRange(BSWVolume);

					BSWVolume.ServerTimeStamp = GrossObsVolume.ServerTimeStamp;
					BSWVolume.SourceTimeStamp = GrossObsVolume.SourceTimeStamp;
					if (BottomVolume.SourceTimeStamp > BSWVolume.SourceTimeStamp)
					{
						BSWVolume.ServerTimeStamp = BottomVolume.ServerTimeStamp;
						BSWVolume.SourceTimeStamp = BottomVolume.SourceTimeStamp;
					}
					if (PercentBSW.SourceTimeStamp > PercentBSW.SourceTimeStamp)
					{
						BSWVolume.ServerTimeStamp = PercentBSW.ServerTimeStamp;
						BSWVolume.SourceTimeStamp = PercentBSW.SourceTimeStamp;
					}
				}
			}

			else
			{
				if (StatusCode.IsBad(GrossStdVolume.OpcStatusSubCode) ||
				!(GrossStdVolume.Value is double?) ||
				!((double?)GrossStdVolume.Value).HasValue)
				{
					if (BSWVolume.Value != null ||
						BSWVolume.Status != StatusCodes.Bad)
					{
						BSWVolume.Status = StatusCodes.Bad;
						BSWVolume.Value = null;
						base.SetTimeStamps(new PointTag[] { GrossStdVolume }, BSWVolume);
						}
					return;
				}

				baseSedmentAndWaterVolume = (double)GrossStdVolume.Value * (double)PercentBSW.Value / 100.0;

				long newStatus = StatusCodes.Good;
				// if any of the input variables are over/under ranged set status to warning
				if (IsStatusUncertain(GrossStdVolume) ||
					IsStatusUncertain(PercentBSW))
				{
					newStatus = StatusCodes.Uncertain;
				}


				if (BSWVolume.Value == null ||
				(double)BSWVolume.Value != baseSedmentAndWaterVolume
				|| IsStatusChange(BSWVolume.Status, newStatus))
				{
					BSWVolume.Value = baseSedmentAndWaterVolume;
					BSWVolume.Status = newStatus;
					CheckForAndSetOverUnderRange(BSWVolume);

					BSWVolume.ServerTimeStamp = GrossStdVolume.ServerTimeStamp;
					BSWVolume.SourceTimeStamp = GrossStdVolume.SourceTimeStamp;

					if (PercentBSW.SourceTimeStamp > BSWVolume.SourceTimeStamp)
					{
						BSWVolume.ServerTimeStamp = PercentBSW.ServerTimeStamp;
						BSWVolume.SourceTimeStamp = PercentBSW.SourceTimeStamp;
					}
				}
			}
		}


		public void CalculateTotalCalculatedVolume(PointTag GrossStdVolume, PointTag BottomVolume, PointTag TotalCalculatedVolume)
		{
			if (TotalCalculatedVolume.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
				TotalCalculatedVolume.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}

			if (QuantitySettings.VolumeCalculationType == VolumeCalculationType.API1995Calculations ||
				!IsValueGood(GrossStdVolume) ||
				!IsValueGood(BottomVolume))
			{
				if (TotalCalculatedVolume.Value != null
				|| TotalCalculatedVolume.Status != StatusCodes.Bad)
				{
					TotalCalculatedVolume.Value = null;
					TotalCalculatedVolume.Status = StatusCodes.Bad;
					base.SetTimeStamps(new PointTag[] { GrossStdVolume, BottomVolume }, TotalCalculatedVolume);
					}

				return;
			}

			double totalCalculatedVolume = (double)GrossStdVolume.Value + (double)BottomVolume.Value;

			long newStatus = StatusCodes.Good;

			// if any of the input variables are over/under ranged set status to warning
			if (IsStatusUncertain(GrossStdVolume) ||
				IsStatusUncertain(BottomVolume))
			{
				newStatus = StatusCodes.Uncertain;
			}


			if (TotalCalculatedVolume.Value == null ||
			(double)TotalCalculatedVolume.Value != totalCalculatedVolume
			|| IsStatusChange(TotalCalculatedVolume.Status, newStatus))
			{
				TotalCalculatedVolume.Value = totalCalculatedVolume;
				TotalCalculatedVolume.Status = newStatus;
				CheckForAndSetOverUnderRange(TotalCalculatedVolume);

				TotalCalculatedVolume.ServerTimeStamp = GrossStdVolume.ServerTimeStamp;
				TotalCalculatedVolume.SourceTimeStamp = GrossStdVolume.SourceTimeStamp;
				if (BottomVolume.SourceTimeStamp > BottomVolume.SourceTimeStamp)
				{
					TotalCalculatedVolume.ServerTimeStamp = BottomVolume.ServerTimeStamp;
					TotalCalculatedVolume.SourceTimeStamp = BottomVolume.SourceTimeStamp;
				}
			}
		}


		public void CalculateLiquidMass(PointTag GrossObsVolume, PointTag NetStandardVolume, PointTag StdDensity, PointTag StdDensityInAir, PointTag Mass, bool Rounding = true)
		{
			// make sure mass is calculated
			if (Mass.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
				Mass.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}

			// make sure all of the data is valid
			if ((QuantitySettings.MassOrWeightCalculationType == MassOrWeightCalculationType.Mass
			|| QuantitySettings.MassOrWeightCalculationType == MassOrWeightCalculationType.WeightInPressurizedTank
			|| QuantitySettings.MassOrWeightCalculationType == MassOrWeightCalculationType.Table52
			|| QuantitySettings.MassOrWeightCalculationType == MassOrWeightCalculationType.Table56
			|| QuantitySettings.MassOrWeightCalculationType == MassOrWeightCalculationType.Table57)
			&& (!IsValueGood(StdDensity)
			|| !IsValueGood(NetStandardVolume))

			|| ((QuantitySettings.MassOrWeightCalculationType == MassOrWeightCalculationType.WeightInAir
			|| QuantitySettings.MassOrWeightCalculationType == MassOrWeightCalculationType.MolarMethod)
			&& (!IsValueGood(StdDensityInAir)
			|| !IsValueGood(NetStandardVolume)))

			|| (QuantitySettings.MassOrWeightCalculationType == MassOrWeightCalculationType.FoodOil
			&& (!IsValueGood(StdDensity)
			|| !IsValueGood(GrossObsVolume))))
			{
				if (Mass.Value != null || Mass.Status != StatusCodes.Bad)
				{
						Mass.Value = null;
						Mass.Status = StatusCodes.Bad;
						if (QuantitySettings.MassOrWeightCalculationType == MassOrWeightCalculationType.Mass
							|| QuantitySettings.MassOrWeightCalculationType == MassOrWeightCalculationType.WeightInPressurizedTank
							|| QuantitySettings.MassOrWeightCalculationType == MassOrWeightCalculationType.Table52
							|| QuantitySettings.MassOrWeightCalculationType == MassOrWeightCalculationType.Table56
							|| QuantitySettings.MassOrWeightCalculationType == MassOrWeightCalculationType.Table57)
								base.SetTimeStamps(new PointTag[] { StdDensity, NetStandardVolume }, Mass);
						else if (QuantitySettings.MassOrWeightCalculationType == MassOrWeightCalculationType.WeightInAir
									|| QuantitySettings.MassOrWeightCalculationType == MassOrWeightCalculationType.MolarMethod)
								base.SetTimeStamps(new PointTag[] { StdDensityInAir, NetStandardVolume }, Mass);
						else if (QuantitySettings.MassOrWeightCalculationType == MassOrWeightCalculationType.FoodOil)
								base.SetTimeStamps(new PointTag[] { StdDensity, GrossObsVolume }, Mass);
					}
			}
			else
			{
				double netvolumem3 = 0.0;
				double masskg = 0.0;

				double stdTemperatureinDegreesC = GetStandardTemperatureinCforSelectedTable();

				EngineeringUnits.Convert((double)NetStandardVolume.Value, NetStandardVolume.Units, ref netvolumem3, EngineeringUnit.FmvMeter3, 60.0);
				if (Rounding)
				{
					netvolumem3 = Math.Round(netvolumem3, 3, MidpointRounding.AwayFromZero);
				}

				switch (QuantitySettings.MassOrWeightCalculationType)
				{
					case MassOrWeightCalculationType.Mass:
						{
							double stddensitykgm3 = 0.0;
							EngineeringUnits.Convert((double)StdDensity.Value, StdDensity.Units, ref stddensitykgm3, EngineeringUnit.FmdKgM3, stdTemperatureinDegreesC);
							if (Rounding)
							{
								stddensitykgm3 = Math.Round(stddensitykgm3, 1, MidpointRounding.AwayFromZero);
							}
							masskg = netvolumem3 * stddensitykgm3;
							break;
						}

					case MassOrWeightCalculationType.WeightInAir:
					case MassOrWeightCalculationType.MolarMethod:
						{
							double StdDensityInAirkgm3 = 0.0;
							EngineeringUnits.Convert((double)StdDensityInAir.Value, StdDensity.Units, ref StdDensityInAirkgm3, EngineeringUnit.FmdKgM3, stdTemperatureinDegreesC);
							if (Rounding)
							{
								StdDensityInAirkgm3 = Math.Round(StdDensityInAirkgm3, 1, MidpointRounding.AwayFromZero);
							}
							masskg = netvolumem3 * StdDensityInAirkgm3;
							break;
						}

					case MassOrWeightCalculationType.WeightInPressurizedTank:
						{
							double stddensitykgm3 = 0.0;
							EngineeringUnits.Convert((double)StdDensity.Value, StdDensity.Units, ref stddensitykgm3, EngineeringUnit.FmdKgM3, stdTemperatureinDegreesC);
							if (Rounding)
							{
								stddensitykgm3 = Math.Round(stddensitykgm3, 1, MidpointRounding.AwayFromZero);
							}
							masskg = netvolumem3 * stddensitykgm3;
							break;
						}

					case MassOrWeightCalculationType.FoodOil:
						{
							double stddensitykgm3 = 0.0;
							EngineeringUnits.Convert((double)StdDensity.Value, StdDensity.Units, ref stddensitykgm3, EngineeringUnit.FmdKgM3, stdTemperatureinDegreesC);
							stddensitykgm3 = Math.Round(stddensitykgm3, 1, MidpointRounding.AwayFromZero);
							double grossvolumem3 = 0.0;
							EngineeringUnits.Convert((double)GrossObsVolume.Value, GrossObsVolume.Units, ref grossvolumem3, EngineeringUnit.FmvMeter3, 60.0);
							if (Rounding)
							{
								grossvolumem3 = Math.Round(grossvolumem3, 3, MidpointRounding.AwayFromZero);
							}
							masskg = grossvolumem3 * stddensitykgm3;
							break;
						}



					case MassOrWeightCalculationType.Table56:
						{
							double stddensitykgm3 = 0.0;
							EngineeringUnits.Convert((double)StdDensity.Value, StdDensity.Units, ref stddensitykgm3, EngineeringUnit.FmdKgM3, stdTemperatureinDegreesC);
							if (Rounding)
							{
								stddensitykgm3 = Math.Round(stddensitykgm3, 1, MidpointRounding.AwayFromZero);
							}
							double valueKG = 0.0;
							double valueMT = 0.0;
							GetTable56Factor(stddensitykgm3, ref valueKG, ref valueMT);
							masskg = netvolumem3 / valueKG;
							if (Mass.Units == EngineeringUnit.FmmMTon)
							{
								double temp = 0.0;
								temp = masskg / valueMT;
								EngineeringUnits.Convert(temp, EngineeringUnit.FmmMTon, ref masskg, EngineeringUnit.FmmKg, 0);
							}
						}
						break;

						case MassOrWeightCalculationType.Table57:
							{
								double stddensitykgm3 = 0.0;
								EngineeringUnits.Convert((double)StdDensity.Value, StdDensity.Units, ref stddensitykgm3, EngineeringUnit.FmdKgM3, stdTemperatureinDegreesC);
								if (Rounding)
								{
									stddensitykgm3 = Math.Round(stddensitykgm3, 1, MidpointRounding.AwayFromZero);
								}
								double valueST = 0.0;
								double valueLT = 0.0;
								double dMassTemp;
								GetTable57Factor(stddensitykgm3, ref valueST, ref valueLT);
								if(Mass.Units == EngineeringUnit.FmmLTon)
								{
									dMassTemp = netvolumem3 / valueLT;
									EngineeringUnits.Convert(dMassTemp, EngineeringUnit.FmmLTon, ref masskg, EngineeringUnit.FmmKg, 0);
								}
								else
								{
									dMassTemp = netvolumem3 / valueST;
									EngineeringUnits.Convert(dMassTemp, EngineeringUnit.FmmSTon, ref masskg, EngineeringUnit.FmmKg, 0);
								}
							}
							break;


						case MassOrWeightCalculationType.Table52:
							{
								double stddensitykgm3 = 0.0;
								EngineeringUnits.Convert((double)StdDensity.Value, StdDensity.Units, ref stddensitykgm3, EngineeringUnit.FmdKgM3, stdTemperatureinDegreesC);
								if (Rounding)
								{
									stddensitykgm3 = Math.Round(stddensitykgm3, 1, MidpointRounding.AwayFromZero);
								}
								double value = 0.0;
								double volBBLs = 0.0;

								GetTable52Factor(stddensitykgm3, ref value);
								EngineeringUnits.Convert(netvolumem3, EngineeringUnit.FmvCm3, ref volBBLs, EngineeringUnit.FmvBlOil, 0);
								netvolumem3 = volBBLs / value;
								masskg = netvolumem3 * stddensitykgm3;
							}
							break;

						default:
							{
								double stddensitykgm3 = 0.0;
								EngineeringUnits.Convert((double)StdDensity.Value, StdDensity.Units, ref stddensitykgm3, EngineeringUnit.FmdKgM3, stdTemperatureinDegreesC);
								if (Rounding)
								{
									stddensitykgm3 = Math.Round(stddensitykgm3, 1, MidpointRounding.AwayFromZero);
								}
								masskg = netvolumem3 * stddensitykgm3;
								break;
							}
				}


				double newValue = 0.0;

				EngineeringUnits.Convert(masskg, EngineeringUnit.FmmKg, ref newValue, Mass.Units, 60.0);

				// make sure all of the data is valid
				if (QuantitySettings.MassOrWeightCalculationType == MassOrWeightCalculationType.Mass
				|| QuantitySettings.MassOrWeightCalculationType == MassOrWeightCalculationType.WeightInPressurizedTank
				|| QuantitySettings.MassOrWeightCalculationType == MassOrWeightCalculationType.Table52
				|| QuantitySettings.MassOrWeightCalculationType == MassOrWeightCalculationType.Table56
				|| QuantitySettings.MassOrWeightCalculationType == MassOrWeightCalculationType.Table57)
				{
					long newStatus = StatusCodes.Good;

					// if either of the variables are over/under ranged set status to warning
					if (IsStatusUncertain(NetStandardVolume) ||
					IsStatusUncertain(StdDensity))
					{
						newStatus = StatusCodes.Uncertain;
					}


					if (Mass.Value == null
					|| (double)Mass.Value != newValue
					|| IsStatusChange(Mass.Status, newStatus))
					{
						Mass.Value = newValue;
						Mass.Status = newStatus;
						CheckForAndSetOverUnderRange(Mass);

						if (StdDensity.SourceTimeStamp > NetStandardVolume.SourceTimeStamp)
						{
							Mass.ServerTimeStamp = StdDensity.ServerTimeStamp;
							Mass.SourceTimeStamp = StdDensity.SourceTimeStamp;
						}
						else
						{
							Mass.ServerTimeStamp = NetStandardVolume.ServerTimeStamp;
							Mass.SourceTimeStamp = NetStandardVolume.SourceTimeStamp;
						}
					}
				}

				else if (QuantitySettings.MassOrWeightCalculationType == MassOrWeightCalculationType.WeightInAir
				|| QuantitySettings.MassOrWeightCalculationType == MassOrWeightCalculationType.MolarMethod)
				{
					long newStatus = StatusCodes.Good;

					// if either of the variables are over/under ranged set status to warning
					if (IsStatusUncertain(NetStandardVolume) ||
					IsStatusUncertain(StdDensityInAir))
					{
						newStatus = StatusCodes.Uncertain;
					}

					if (Mass.Value == null
					|| (double)Mass.Value != newValue
					|| IsStatusChange(Mass.Status, newStatus))
					{
						Mass.Value = newValue;
						Mass.Status = newStatus;
						CheckForAndSetOverUnderRange(Mass);

						if (StdDensityInAir.SourceTimeStamp > NetStandardVolume.SourceTimeStamp)
						{
							Mass.ServerTimeStamp = StdDensityInAir.ServerTimeStamp;
							Mass.SourceTimeStamp = StdDensityInAir.SourceTimeStamp;
						}
						else
						{
							Mass.ServerTimeStamp = NetStandardVolume.ServerTimeStamp;
							Mass.SourceTimeStamp = NetStandardVolume.SourceTimeStamp;
						}
					}
				}

				else if (QuantitySettings.MassOrWeightCalculationType == MassOrWeightCalculationType.FoodOil)
				{
					long newStatus = StatusCodes.Good;

					// if either of the variables are over/under ranged set status to warning
					if (IsStatusUncertain(GrossObsVolume) ||
					IsStatusUncertain(StdDensityInAir))
					{
						newStatus = StatusCodes.Uncertain;
					}

					if (Mass.Value == null
					|| (double)Mass.Value != newValue
					|| IsStatusChange(Mass.Status, newStatus))
					{
						Mass.Value = newValue;
						Mass.Status = newStatus;
						CheckForAndSetOverUnderRange(Mass);

						if (StdDensityInAir.SourceTimeStamp > GrossObsVolume.SourceTimeStamp)
						{
							Mass.ServerTimeStamp = StdDensityInAir.ServerTimeStamp;
							Mass.SourceTimeStamp = StdDensityInAir.SourceTimeStamp;
						}
						else
						{
							Mass.ServerTimeStamp = GrossObsVolume.ServerTimeStamp;
							Mass.SourceTimeStamp = GrossObsVolume.SourceTimeStamp;
						}
					}
				}
			}
		}


		public void CalculateStdWeight(PointTag SelectedVolume, PointTag DensityInAir, PointTag StdWeight)
		{
			// make sure mass is calculated
			if (StdWeight.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
				StdWeight.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}

			// make sure all of the data is valid
			if (QuantitySettings.VolumeCalculationType == VolumeCalculationType.API1995Calculations ||
				!IsValueGood(SelectedVolume) ||
				!IsValueGood(DensityInAir))
			{
				if (StdWeight.Value != null
				|| StdWeight.Status != StatusCodes.Bad)
				{
					StdWeight.Value = null;
					StdWeight.Status = StatusCodes.Bad;
					base.SetTimeStamps(new PointTag[] { SelectedVolume, DensityInAir }, StdWeight);
					}
			}
			else
			{
				double volumem3 = 0.0;
				double densityinkgm3 = 0.0;
				double NewValue = 0.0;

				EngineeringUnits.Convert((double)DensityInAir.Value, DensityInAir.Units, ref densityinkgm3, EngineeringUnit.FmdKgM3, 60.0);
				EngineeringUnits.Convert((double)SelectedVolume.Value, SelectedVolume.Units, ref volumem3, EngineeringUnit.FmvMeter3, 60.0);

				NewValue = volumem3 * densityinkgm3;

				EngineeringUnits.Convert(NewValue, EngineeringUnit.FmmKg, ref NewValue, StdWeight.Units, 60.0);

				long newStatus = StatusCodes.Good;

				// if either of the variables are over/under ranged set status to warning
				if (IsStatusUncertain(DensityInAir) ||
					IsStatusUncertain(SelectedVolume))
				{
					newStatus = StatusCodes.Uncertain;
				}

				if (StdWeight.Value == null
				|| (double)StdWeight.Value != NewValue
				|| IsStatusChange(StdWeight.Status, newStatus))
				{
					StdWeight.Value = NewValue;
					StdWeight.Status = newStatus;
					CheckForAndSetOverUnderRange(StdWeight);


					StdWeight.ServerTimeStamp = SelectedVolume.ServerTimeStamp;
					StdWeight.SourceTimeStamp = SelectedVolume.SourceTimeStamp;
					if (DensityInAir.SourceTimeStamp > SelectedVolume.SourceTimeStamp)
					{
						StdWeight.ServerTimeStamp = DensityInAir.ServerTimeStamp;
						StdWeight.SourceTimeStamp = DensityInAir.SourceTimeStamp;
					}
				}
			}
		}

		void CalculateVaporMass(PointTag GrossObsVolume, 
								PointTag DensityVapor, 
								PointTag TemperatureVapor, 
								PointTag PressureVapor,
								PointTag StdDensity,
								PointTag MassVapor)
		{
			double NewValue = 0.0;
			long newStatus = StatusCodes.Good;
			DateTimeOffset TimeStamp = GetMostRecentTimeStamp(new List<PointTag> { GrossObsVolume, DensityVapor, TemperatureVapor, PressureVapor, StdDensity, MassVapor });
			EngineeringUnit SelectedTempUnit = new EngineeringUnit();
			double dStdTempInC = 0.0;
			double dMaxVolInM3 = 0.0;
			double dGrossVolInM3 = 0.0;
			double dStdDensityInKgPerL = 0.0;
			double dGasDensityInKgPerL = 0.0;
			double dVaporPressInKgPerCm2 = 0.0;
			double dVaporTempInC = 0.0;
			double dGasWeightMTon = 0.0;
			double dMoleConstant = 0.0;

			// if not calculated do nothing
			if (MassVapor.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
				MassVapor.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}

			if (QuantitySettings.MassOrWeightCalculationType != MassOrWeightCalculationType.WeightInPressurizedTank &&
				QuantitySettings.MassOrWeightCalculationType != MassOrWeightCalculationType.MolarMethod)
			{
				NewValue = 0.0;
				newStatus = StatusCodes.Good;
			}
			else
			{
				var pointTags = new List<PointTag>();
				if (VcfSettings.BaseTemperature.Value == 60.0)  // farenheit
				{
					SelectedTempUnit = EngineeringUnit.FmtDegF;
				}
				else
				{
					SelectedTempUnit = EngineeringUnit.FmtDegC;
				}
				// convert std temp
				EngineeringUnits.Convert(VcfSettings.BaseTemperature.Value,
										SelectedTempUnit,
										ref dStdTempInC,
										EngineeringUnit.FmtDegC, 60.0);
				// make sure that all of the values are good
				if (QuantitySettings.MassOrWeightCalculationType == MassOrWeightCalculationType.WeightInPressurizedTank)
				{
					// check max vol, vapor density, gross vol, vapor press and vapor temp
					if(IsValueGood(DensityVapor) &&
						IsValueGood(GrossObsVolume) &&
						IsValueGood(PressureVapor) &&
						IsValueGood(TemperatureVapor))
					{
						pointTags.Add(DensityVapor);
						pointTags.Add(GrossObsVolume);
						pointTags.Add(PressureVapor);
						pointTags.Add(TemperatureVapor);
						// convert max vol
						EngineeringUnits.Convert(Vessel.TankVolume.Value,
												GrossObsVolume.Units,
												ref dMaxVolInM3,
												EngineeringUnit.FmvMeter3, 60.0);

						// convert gross vol
						EngineeringUnits.Convert((double)GrossObsVolume.Value,
												GrossObsVolume.Units,
												ref dGrossVolInM3,
												EngineeringUnit.FmvMeter3, 60.0);
						// convert vapor density
						EngineeringUnits.Convert((double)DensityVapor.Value,
												DensityVapor.Units,
												ref dGasDensityInKgPerL,
												EngineeringUnit.FmdKgL3, dStdTempInC);
						// vapor temperature
						EngineeringUnits.Convert((double)TemperatureVapor.Value,
												TemperatureVapor.Units,
												ref dVaporTempInC,
												EngineeringUnit.FmtDegC, 60.0);
						// convert vapor pressure
						EngineeringUnits.Convert((double)PressureVapor.Value,
												PressureVapor.Units,
												ref dVaporPressInKgPerCm2,
												EngineeringUnit.FmpKgCm2, 60.0);

						dGasWeightMTon = (dMaxVolInM3 - dGrossVolInM3) * dGasDensityInKgPerL * (1.0 + dVaporPressInKgPerCm2) * (273.0 / (273.0 + dVaporTempInC));

						// convert back to the tag units
						EngineeringUnits.Convert(dGasWeightMTon,
												EngineeringUnit.FmmMTon,
												ref NewValue,
												MassVapor.Units, 60.0);

						TimeStamp = GetMostRecentTimeStamp(pointTags);
						newStatus = StatusCodes.Good;
					}
					else
					{
						// value is bad
						newStatus = StatusCodes.Bad;
						TimeStamp = GetMostRecentTimeStamp(pointTags);
						}
				}
				else
				{
					// check std air density, max vol, gross vol, vapor temp, vapor press,
					if (IsValueGood(StdDensity) &&
						IsValueGood(GrossObsVolume) &&
						IsValueGood(PressureVapor) &&
						IsValueGood(TemperatureVapor))
					{
						pointTags.Add(StdDensity);
						pointTags.Add(GrossObsVolume);
						pointTags.Add(PressureVapor);
						pointTags.Add(TemperatureVapor);

						// convert std density
						EngineeringUnits.Convert((double)StdDensity.Value,
												StdDensity.Units,
												ref dStdDensityInKgPerL,
												EngineeringUnit.FmdKgL3, dStdTempInC);

						// convert max vol
						EngineeringUnits.Convert(Vessel.TankVolume.Value,
												GrossObsVolume.Units,
												ref dMaxVolInM3,
												EngineeringUnit.FmvMeter3, 60.0);
						// convert gross vol
						EngineeringUnits.Convert((double)GrossObsVolume.Value,
												GrossObsVolume.Units,
												ref dGrossVolInM3,
												EngineeringUnit.FmvMeter3, 60.0);
						// vapor temperature
						EngineeringUnits.Convert((double)TemperatureVapor.Value,
												TemperatureVapor.Units,
												ref dVaporTempInC,
												EngineeringUnit.FmtDegC, 60.0);
						// convert vapor pressure
						EngineeringUnits.Convert((double)PressureVapor.Value,
												PressureVapor.Units,
												ref dVaporPressInKgPerCm2,
												EngineeringUnit.FmpKgCm2, 60.0);

						dMoleConstant = 1.0;

						double dConversionFactor = 0.0011;
						if (dStdDensityInKgPerL >= 0.9966 &&
							dStdDensityInKgPerL <= 1.6635)
							dConversionFactor = 0.0010;
						else if (dStdDensityInKgPerL > 1.6635)
							dConversionFactor = 0.0009;
						dGasWeightMTon = (dMaxVolInM3 - dGrossVolInM3) * (273.0 / (273.0 + dVaporTempInC));
						dGasWeightMTon *= (1.033 + dVaporPressInKgPerCm2) / 1.033;
						dGasWeightMTon *= (dMoleConstant / 22.4) * (1.0 / dStdDensityInKgPerL);
						dGasWeightMTon *= (1.0 / 1000.0) * (dStdDensityInKgPerL - dConversionFactor);

						// convert back to the tag units
						EngineeringUnits.Convert(dGasWeightMTon,
												EngineeringUnit.FmmMTon,
												ref NewValue,
												MassVapor.Units, 60.0);

						TimeStamp = GetMostRecentTimeStamp(pointTags);
						newStatus = StatusCodes.Good;
					}
					else
					{
						// value is bad
						newStatus = StatusCodes.Bad;
						TimeStamp = GetMostRecentTimeStamp(pointTags);
						}
				}
			}

			if (MassVapor.Value == null
			|| (double)MassVapor.Value != NewValue
			|| IsStatusChange(MassVapor.Status, newStatus))
			{
				if (newStatus == StatusCodes.Good)
				{
					MassVapor.Value = NewValue;
				}
				else
				{
					MassVapor.Value = null;
				}
				MassVapor.Status = newStatus;
				MassVapor.ServerTimeStamp = TimeStamp;
				MassVapor.SourceTimeStamp = TimeStamp;
				CheckForAndSetOverUnderRange(MassVapor);
			}
			return;
		}

		void CalculateVaporNetVolume(PointTag StdDensity,PointTag MassVapor, PointTag VolumeVaporNet)
		{
			double NewValue = 0.0;
			long newStatus = StatusCodes.Bad;
			DateTimeOffset TimeStamp = GetMostRecentTimeStamp(new List<PointTag>{ StdDensity, MassVapor, VolumeVaporNet });
			EngineeringUnit SelectedTempUnit = new EngineeringUnit();
			double dStdTempInC = 0.0;
			double dVaporMassInKg = 0.0;
			double dVaporNetVolInM3 = 0.0;
			double dStdDensityInKgPerM3 = 0.0;

			// if not calculated do nothing
			if (VolumeVaporNet.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
				VolumeVaporNet.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}

			// check and make sure that the values are valid
			if (IsValueGood(StdDensity) &&
				IsValueGood(MassVapor))
			{
				var pointTags = new List<PointTag>();
				pointTags.Add(StdDensity);
				pointTags.Add(MassVapor);
				if (VcfSettings.BaseTemperature.Value == 60.0)  // farenheit
				{
					SelectedTempUnit = EngineeringUnit.FmtDegF;
				}
				else
				{
					SelectedTempUnit = EngineeringUnit.FmtDegC;
				}
				// convert std temp
				EngineeringUnits.Convert(VcfSettings.BaseTemperature.Value,
										SelectedTempUnit,
										ref dStdTempInC,
										EngineeringUnit.FmtDegC, 60.0);

				// convert mass vapor
				EngineeringUnits.Convert((double)MassVapor.Value,
										MassVapor.Units,
										ref dVaporMassInKg,
										EngineeringUnit.FmmKg, 60.0);

				// convert standard density
				EngineeringUnits.Convert((double)StdDensity.Value,
										StdDensity.Units,
										ref dStdDensityInKgPerM3,
										EngineeringUnit.FmdKgM3, dStdTempInC);

				switch (QuantitySettings.MassOrWeightCalculationType)
				{
					case MassOrWeightCalculationType.MolarMethod:
						{
							double dConversionFactor = 0.0011;
							if (dStdDensityInKgPerM3 >= 996.6 &&
								dStdDensityInKgPerM3 <= 1663.5)
								dConversionFactor = 0.0010;
							else if (dStdDensityInKgPerM3 > 1663.5)
								dConversionFactor = 0.0009;
							dVaporNetVolInM3 = dVaporMassInKg / (dStdDensityInKgPerM3 - (dConversionFactor * 1000.0));
							break;
						}
					case MassOrWeightCalculationType.Mass:
					case MassOrWeightCalculationType.WeightInAir:
					case MassOrWeightCalculationType.WeightInPressurizedTank:
					case MassOrWeightCalculationType.FoodOil:
					default:
						dVaporNetVolInM3 = dVaporMassInKg / dStdDensityInKgPerM3;
						break;
				}

				// convert back to the tag units
				EngineeringUnits.Convert(dVaporNetVolInM3,
										EngineeringUnit.FmvMeter3,
										ref NewValue,
										VolumeVaporNet.Units, 60.0);

				TimeStamp = GetMostRecentTimeStamp(pointTags);
				newStatus = StatusCodes.Good;
			}


			if (VolumeVaporNet.Value == null
			|| (double)VolumeVaporNet.Value != NewValue
			|| IsStatusChange(VolumeVaporNet.Status, newStatus))
			{
				if (newStatus == StatusCodes.Good)
				{
					VolumeVaporNet.Value = NewValue;
				}
				else
				{
					VolumeVaporNet.Value = null;
				}
				VolumeVaporNet.Status = newStatus;
				VolumeVaporNet.ServerTimeStamp = TimeStamp;
				VolumeVaporNet.SourceTimeStamp = TimeStamp;
				CheckForAndSetOverUnderRange(VolumeVaporNet);
			}
			return;
		}

		public double GetStandardTemperatureinCforSelectedTable()
		{
			double StdTempInDegreesC = 0.0;

			if (VcfSettings.CorrectionMethodType == Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_C ||
				VcfSettings.CorrectionMethodType == Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_C_1980)
			{
				if (VcfSettings.CorrectionMethodSpecific == Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54A_30 ||
					VcfSettings.CorrectionMethodSpecific == Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54B_30 ||
					VcfSettings.CorrectionMethodSpecific == Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54C_30 ||
					VcfSettings.CorrectionMethodSpecific == Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54D_30)
				{
					StdTempInDegreesC = 30.0;
				}
				else
				{
					StdTempInDegreesC = 15.0;
				}
			}
			else
			{
				EngineeringUnits.Convert(60.0, EngineeringUnit.FmtDegF, ref StdTempInDegreesC, EngineeringUnit.FmtDegC, 60.0);
			}

			return StdTempInDegreesC;
		}

		void GetTable56Factor(double stdDensityInKgPerM3, ref double valuekg, ref double valuemt)
		{
			bool exitLoop = false;
			ushort position;
			valuekg = 1.0;
			valuemt = 1.0;

			// check the range of the passed in std density value
			if (stdDensityInKgPerM3 < 654.0
			|| stdDensityInKgPerM3 > 1075.0)
			{
				return;
			}

			position = 0;

			if (stdDensityInKgPerM3 == API_Lookup_Table56[position].density)
			{
				valuekg = API_Lookup_Table56[position].kilogramFactor;
				valuemt = API_Lookup_Table56[position].cubicMetersFactor;
				exitLoop = true;
			}

			while (!exitLoop)
			{
				++position;
				// check if the value matched
				if (stdDensityInKgPerM3 == API_Lookup_Table56[position].density)
				{
					valuekg = API_Lookup_Table56[position].kilogramFactor;
					valuemt = API_Lookup_Table56[position].cubicMetersFactor;
					exitLoop = true;
				}
				// check if value needs to be scaled
				else if (stdDensityInKgPerM3 > API_Lookup_Table56[position - 1].density
				&&	stdDensityInKgPerM3 < API_Lookup_Table56[position].density)
				{
					double percent = 0.0;
					percent = (stdDensityInKgPerM3 - API_Lookup_Table56[position - 1].density) / (API_Lookup_Table56[position].density - API_Lookup_Table56[position - 1].density);
					valuekg = ((API_Lookup_Table56[position].kilogramFactor - API_Lookup_Table56[position - 1].kilogramFactor) * percent) + API_Lookup_Table56[position - 1].kilogramFactor;
					valuemt = ((API_Lookup_Table56[position].cubicMetersFactor - API_Lookup_Table56[position - 1].cubicMetersFactor) * percent) + API_Lookup_Table56[position - 1].cubicMetersFactor;
					exitLoop = true;
				}
			}
		}

		void GetTable57Factor(double stdDensityInKgPerM3, ref double valueST, ref double valueLT)
		{
			bool exitLoop = false;
			ushort position;
			valueST = 1.0;
			valueLT = 1.0;

			// check the range of the passed in std density value
			if (stdDensityInKgPerM3 < 654.0
			||	stdDensityInKgPerM3 > 1075.1)
			{
				return;
			}

			position = 0;
			if (stdDensityInKgPerM3 == API_Lookup_Table57[position].density)
			{
				valueST = API_Lookup_Table57[position].shortTonsFactor;
				valueLT = API_Lookup_Table57[position].longTonsFactor;
				exitLoop = true;
			}
			while (!exitLoop)
			{
				++position;
				// check if the value matched
				if (stdDensityInKgPerM3 == API_Lookup_Table57[position].density)
				{
					valueST = API_Lookup_Table57[position].shortTonsFactor;
					valueLT = API_Lookup_Table57[position].longTonsFactor;
					exitLoop = true;
				}
				// check if value needs to be scaled
				else if (stdDensityInKgPerM3 > API_Lookup_Table57[position - 1].density &&
							stdDensityInKgPerM3 < API_Lookup_Table57[position].density)
				{
					double percent = 0.0;
					percent = (stdDensityInKgPerM3 - API_Lookup_Table57[position - 1].density) / (API_Lookup_Table57[position].density - API_Lookup_Table57[position - 1].density);
					valueST = ((API_Lookup_Table57[position].shortTonsFactor - API_Lookup_Table57[position - 1].shortTonsFactor) * percent) + API_Lookup_Table57[position - 1].shortTonsFactor;
					valueLT = ((API_Lookup_Table57[position].longTonsFactor - API_Lookup_Table57[position - 1].longTonsFactor) * percent) + API_Lookup_Table57[position - 1].longTonsFactor;
					exitLoop = true;
				}
			}
		}

		void GetTable52Factor(double stdDensityInKgPerM3, ref double value)
		{
			ushort position;
					
			value = 1.0;

			// check the range of the passed in std density value
			if (stdDensityInKgPerM3 < 654.0 ||
			stdDensityInKgPerM3 > 1074.0)
				return;

			if (stdDensityInKgPerM3 >= API_Lookup_Table52[0].densityStart
			&&	stdDensityInKgPerM3 <= API_Lookup_Table52[0].densityStop)
			{
				value = API_Lookup_Table52[0].cubicMetersperBarrel;
			}
			else
			{
				for (position = 1; position < API_TABLE52_ENTRIES; position++)
				{
					if (stdDensityInKgPerM3 >= API_Lookup_Table52[position].densityStart &&
						stdDensityInKgPerM3 <= API_Lookup_Table52[position].densityStop)
					{
						value = API_Lookup_Table52[position].cubicMetersperBarrel;
						break;
					}
					else if (stdDensityInKgPerM3 > API_Lookup_Table52[position - 1].densityStop &&
						stdDensityInKgPerM3 < API_Lookup_Table52[position].densityStart)
					{
						value = API_Lookup_Table52[position].cubicMetersperBarrel;
						break;
					}
				}
			}
		}

		DateTimeOffset GetMostRecentTimeStamp(List<PointTag> pointTags)
		{
			DateTimeOffset Returndatetime = DateTimeOffset.UtcNow.AddYears(-5);

			foreach(var pointTag in pointTags)
			{
				if ((pointTag.SourceTimeStamp > pointTag.ServerTimeStamp) &&
					pointTag.SourceTimeStamp >= Returndatetime)
				{
					Returndatetime = pointTag.SourceTimeStamp;
				}
				else if ((pointTag.SourceTimeStamp < pointTag.ServerTimeStamp) &&
					pointTag.ServerTimeStamp >= Returndatetime)
				{
					Returndatetime = pointTag.ServerTimeStamp;
				}
				else if(pointTag.ServerTimeStamp >= Returndatetime)
				{
					Returndatetime = pointTag.ServerTimeStamp;
				}
			}

			return Returndatetime;
		}

		public class API_TABLE56
		{
			public double density;
			public double kilogramFactor;
			public double cubicMetersFactor;

			public API_TABLE56(double density, double kilogramFactor, double cubicMetersFactor)
			{
				this.density = density;
				this.kilogramFactor = kilogramFactor;
				this.cubicMetersFactor = cubicMetersFactor;
			}
		}

		static API_TABLE56[] API_Lookup_Table56 = new API_TABLE56[]
		{
			new API_TABLE56(654.0,652.9,1.5317),
			new API_TABLE56(655,653.9,1.5293),
			new API_TABLE56(656,654.9,1.5270),
			new API_TABLE56(657,655.9,1.5247),
			new API_TABLE56(658,656.9,1.5224),
			new API_TABLE56(659,657.9,1.5200),
			new API_TABLE56(660,658.9,1.5177),
			new API_TABLE56(661,659.9,1.5154),
			new API_TABLE56(662,660.9,1.5131),
			new API_TABLE56(663,661.9,1.5108),
			new API_TABLE56(664,662.9,1.5086),
			new API_TABLE56(665,663.9,1.5063),
			new API_TABLE56(666,664.9,1.5040),
			new API_TABLE56(667,665.9,1.5018),
			new API_TABLE56(668,666.9,1.4995),
			new API_TABLE56(669,667.9,1.4973),
			new API_TABLE56(670,668.9,1.4950),
			new API_TABLE56(671,669.9,1.4928),
			new API_TABLE56(672,670.9,1.4906),
			new API_TABLE56(673,671.9,1.4884),
			new API_TABLE56(674,672.9,1.4861),
			new API_TABLE56(675,673.9,1.4839),
			new API_TABLE56(676,674.9,1.4817),
			new API_TABLE56(677,675.9,1.4795),
			new API_TABLE56(678,676.9,1.4774),
			new API_TABLE56(679,677.9,1.4752),
			new API_TABLE56(680,678.9,1.4730),
			new API_TABLE56(681,679.9,1.4708),
			new API_TABLE56(682,680.9,1.4687),
			new API_TABLE56(683,681.9,1.4665),
			new API_TABLE56(684,682.9,1.4644),
			new API_TABLE56(685,683.9,1.4622),
			new API_TABLE56(686,684.9,1.4601),
			new API_TABLE56(687,685.9,1.4580),
			new API_TABLE56(688,686.9,1.4559),
			new API_TABLE56(689,687.9,1.4537),
			new API_TABLE56(690,688.9,1.4516),
			new API_TABLE56(691,689.9,1.4495),
			new API_TABLE56(692,690.9,1.4474),
			new API_TABLE56(693,691.9,1.4453),
			new API_TABLE56(694,692.9,1.4432),
			new API_TABLE56(695,693.9,1.4412),
			new API_TABLE56(696,694.9,1.4391),
			new API_TABLE56(697,695.9,1.4370),
			new API_TABLE56(698,696.9,1.4350),
			new API_TABLE56(699,697.9,1.4329),
			new API_TABLE56(700,698.9,1.4309),
			new API_TABLE56(701,699.9,1.4288),
			new API_TABLE56(702,700.9,1.4268),
			new API_TABLE56(703,701.9,1.4247),
			new API_TABLE56(704,702.9,1.4227),
			new API_TABLE56(705,703.9,1.4207),
			new API_TABLE56(706,704.9,1.4187),
			new API_TABLE56(707,705.9,1.4167),
			new API_TABLE56(708,706.9,1.4147),
			new API_TABLE56(709,707.9,1.4127),
			new API_TABLE56(710,708.9,1.4107),
			new API_TABLE56(711,709.9,1.4087),
			new API_TABLE56(712,710.9,1.4067),
			new API_TABLE56(713,711.9,1.4047),
			new API_TABLE56(714,712.9,1.4027),
			new API_TABLE56(715,713.9,1.4008),
			new API_TABLE56(716,714.9,1.3988),
			new API_TABLE56(717,715.9,1.3969),
			new API_TABLE56(718,716.9,1.3949),
			new API_TABLE56(719,717.9,1.3930),
			new API_TABLE56(720,718.9,1.3910),
			new API_TABLE56(721,719.9,1.3891),
			new API_TABLE56(722,720.9,1.3872),
			new API_TABLE56(723,721.9,1.3853),
			new API_TABLE56(724,722.9,1.3833),
			new API_TABLE56(725,723.9,1.3814),
			new API_TABLE56(726,724.9,1.3795),
			new API_TABLE56(727,725.9,1.3776),
			new API_TABLE56(728,726.9,1.3757),
			new API_TABLE56(729,727.9,1.3738),
			new API_TABLE56(730,728.9,1.3719),
			new API_TABLE56(731,729.9,1.3701),
			new API_TABLE56(732,730.9,1.3682),
			new API_TABLE56(733,731.9,1.3663),
			new API_TABLE56(734,732.9,1.3645),
			new API_TABLE56(735,733.9,1.3626),
			new API_TABLE56(736,734.9,1.3607),
			new API_TABLE56(737,735.9,1.3589),
			new API_TABLE56(738,736.9,1.3571),
			new API_TABLE56(739,737.9,1.3552),
			new API_TABLE56(740,738.9,1.3534),
			new API_TABLE56(741,739.9,1.3515),
			new API_TABLE56(742,740.9,1.3497),
			new API_TABLE56(743,741.9,1.3479),
			new API_TABLE56(744,742.9,1.3461),
			new API_TABLE56(745,743.9,1.3443),
			new API_TABLE56(746,744.9,1.3425),
			new API_TABLE56(747,745.9,1.3407),
			new API_TABLE56(748,746.9,1.3389),
			new API_TABLE56(749,747.9,1.3371),
			new API_TABLE56(750,748.9,1.3353),
			new API_TABLE56(751,749.9,1.3335),
			new API_TABLE56(752,750.9,1.3317),
			new API_TABLE56(753,751.9,1.3300),
			new API_TABLE56(754,752.9,1.3282),
			new API_TABLE56(755,753.9,1.3264),
			new API_TABLE56(756,754.9,1.3247),
			new API_TABLE56(757,755.9,1.3229),
			new API_TABLE56(758,756.9,1.3212),
			new API_TABLE56(759,757.9,1.3194),
			new API_TABLE56(760,758.9,1.3177),
			new API_TABLE56(761,759.9,1.3160),
			new API_TABLE56(762,760.9,1.3142),
			new API_TABLE56(763,761.9,1.3125),
			new API_TABLE56(764,762.9,1.3108),
			new API_TABLE56(765,763.9,1.3091),
			new API_TABLE56(766,764.9,1.3074),
			new API_TABLE56(767,765.9,1.3057),
			new API_TABLE56(768,766.9,1.3040),
			new API_TABLE56(769,767.9,1.3023),
			new API_TABLE56(770,768.9,1.3006),
			new API_TABLE56(771,769.9,1.2989),
			new API_TABLE56(772,770.9,1.2972),
			new API_TABLE56(773,771.9,1.2955),
			new API_TABLE56(774,772.9,1.2938),
			new API_TABLE56(775,773.9,1.2922),
			new API_TABLE56(776,774.9,1.2905),
			new API_TABLE56(777,775.9,1.2888),
			new API_TABLE56(778,776.9,1.2872),
			new API_TABLE56(779,777.9,1.2855),
			new API_TABLE56(780,778.9,1.2839),
			new API_TABLE56(781,779.9,1.2822),
			new API_TABLE56(782,780.9,1.2806),
			new API_TABLE56(783,781.9,1.2789),
			new API_TABLE56(784,782.9,1.2773),
			new API_TABLE56(785,783.9,1.2757),
			new API_TABLE56(786,784.9,1.2741),
			new API_TABLE56(787,785.9,1.2724),
			new API_TABLE56(788,786.9,1.2708),
			new API_TABLE56(789,787.9,1.2692),
			new API_TABLE56(790,788.9,1.2676),
			new API_TABLE56(791,789.9,1.2660),
			new API_TABLE56(792,790.9,1.2644),
			new API_TABLE56(793,791.9,1.2628),
			new API_TABLE56(794,792.9,1.2612),
			new API_TABLE56(795,793.9,1.2596),
			new API_TABLE56(796,794.9,1.2580),
			new API_TABLE56(797,795.9,1.2564),
			new API_TABLE56(798,796.9,1.2549),
			new API_TABLE56(799,797.9,1.2533),
			new API_TABLE56(800,798.9,1.2517),
			new API_TABLE56(801,799.9,1.2502),
			new API_TABLE56(802,800.9,1.2486),
			new API_TABLE56(803,801.9,1.2470),
			new API_TABLE56(804,802.9,1.2455),
			new API_TABLE56(805,803.9,1.2439),
			new API_TABLE56(806,804.9,1.2424),
			new API_TABLE56(807,805.9,1.2408),
			new API_TABLE56(808,806.9,1.2393),
			new API_TABLE56(809,807.9,1.2378),
			new API_TABLE56(810,808.9,1.2362),
			new API_TABLE56(811,809.9,1.2347),
			new API_TABLE56(812,810.9,1.2332),
			new API_TABLE56(813,811.9,1.2317),
			new API_TABLE56(814,812.9,1.2302),
			new API_TABLE56(815,813.9,1.2286),
			new API_TABLE56(816,814.9,1.2271),
			new API_TABLE56(817,815.9,1.2256),
			new API_TABLE56(818,816.9,1.2241),
			new API_TABLE56(819,817.9,1.2226),
			new API_TABLE56(820,818.9,1.2211),
			new API_TABLE56(821,819.9,1.2197),
			new API_TABLE56(822,820.9,1.2182),
			new API_TABLE56(823,821.9,1.2167),
			new API_TABLE56(824,822.9,1.2152),
			new API_TABLE56(825,823.9,1.2137),
			new API_TABLE56(826,824.9,1.2123),
			new API_TABLE56(827,825.9,1.2108),
			new API_TABLE56(828,826.9,1.2093),
			new API_TABLE56(829,827.9,1.2079),
			new API_TABLE56(830,828.9,1.2064),
			new API_TABLE56(831,829.9,1.2050),
			new API_TABLE56(832,830.9,1.2035),
			new API_TABLE56(833,831.9,1.2021),
			new API_TABLE56(834,832.9,1.2006),
			new API_TABLE56(835,833.9,1.1992),
			new API_TABLE56(836,834.9,1.1977),
			new API_TABLE56(837,835.9,1.1963),
			new API_TABLE56(838,836.9,1.1949),
			new API_TABLE56(839,837.9,1.1935),
			new API_TABLE56(840,838.9,1.1920),
			new API_TABLE56(841,839.9,1.1906),
			new API_TABLE56(842,840.9,1.1892),
			new API_TABLE56(843,841.9,1.1878),
			new API_TABLE56(844,842.9,1.1864),
			new API_TABLE56(845,843.9,1.1850),
			new API_TABLE56(846,844.9,1.1836),
			new API_TABLE56(847,845.9,1.1822),
			new API_TABLE56(848,846.9,1.1808),
			new API_TABLE56(849,847.9,1.1794),
			new API_TABLE56(850,848.9,1.1780),
			new API_TABLE56(851,849.9,1.1766),
			new API_TABLE56(852,850.9,1.1752),
			new API_TABLE56(853,851.9,1.1738),
			new API_TABLE56(854,852.9,1.1725),
			new API_TABLE56(855,853.9,1.1711),
			new API_TABLE56(856,854.9,1.1697),
			new API_TABLE56(857,855.9,1.1683),
			new API_TABLE56(858,856.9,1.1670),
			new API_TABLE56(859,857.9,1.1656),
			new API_TABLE56(860,858.9,1.1643),
			new API_TABLE56(861,859.9,1.1629),
			new API_TABLE56(862,860.9,1.1616),
			new API_TABLE56(863,861.9,1.1602),
			new API_TABLE56(864,862.9,1.1589),
			new API_TABLE56(865,863.9,1.1575),
			new API_TABLE56(866,864.9,1.1562),
			new API_TABLE56(867,865.9,1.1549),
			new API_TABLE56(868,866.9,1.1535),
			new API_TABLE56(869,867.9,1.1522),
			new API_TABLE56(870,868.9,1.1509),
			new API_TABLE56(871,869.9,1.1495),
			new API_TABLE56(872,870.9,1.1482),
			new API_TABLE56(873,871.9,1.1469),
			new API_TABLE56(874,872.9,1.1456),
			new API_TABLE56(875,873.9,1.1443),
			new API_TABLE56(876,874.9,1.1430),
			new API_TABLE56(877,875.9,1.1417),
			new API_TABLE56(878,876.9,1.1404),
			new API_TABLE56(879,877.9,1.1391),
			new API_TABLE56(880,878.9,1.1378),
			new API_TABLE56(881,879.9,1.1365),
			new API_TABLE56(882,880.9,1.1352),
			new API_TABLE56(883,881.9,1.1339),
			new API_TABLE56(884,882.9,1.1326),
			new API_TABLE56(885,883.9,1.1313),
			new API_TABLE56(886,884.9,1.1301),
			new API_TABLE56(887,885.9,1.1288),
			new API_TABLE56(888,886.9,1.1275),
			new API_TABLE56(889,887.9,1.1262),
			new API_TABLE56(890,888.9,1.1250),
			new API_TABLE56(891,889.9,1.1237),
			new API_TABLE56(892,890.9,1.1224),
			new API_TABLE56(893,891.9,1.1212),
			new API_TABLE56(894,892.9,1.1199),
			new API_TABLE56(895,893.9,1.1187),
			new API_TABLE56(896,894.9,1.1174),
			new API_TABLE56(897,895.9,1.1162),
			new API_TABLE56(898,896.9,1.1149),
			new API_TABLE56(899,897.9,1.1137),
			new API_TABLE56(900,898.9,1.1125),
			new API_TABLE56(901,899.9,1.1112),
			new API_TABLE56(902,900.9,1.1100),
			new API_TABLE56(903,901.9,1.1088),
			new API_TABLE56(904,902.9,1.1075),
			new API_TABLE56(905,903.9,1.1063),
			new API_TABLE56(906,904.9,1.1051),
			new API_TABLE56(907,905.9,1.1039),
			new API_TABLE56(908,906.9,1.1026),
			new API_TABLE56(909,907.9,1.1014),
			new API_TABLE56(910,908.9,1.1002),
			new API_TABLE56(911,909.9,1.0990),
			new API_TABLE56(912,910.9,1.0978),
			new API_TABLE56(913,911.9,1.0966),
			new API_TABLE56(914,912.9,1.0954),
			new API_TABLE56(915,913.9,1.0942),
			new API_TABLE56(916,914.9,1.0930),
			new API_TABLE56(917,915.9,1.0918),
			new API_TABLE56(918,916.9,1.0906),
			new API_TABLE56(919,917.9,1.0894),
			new API_TABLE56(920,918.9,1.0882),
			new API_TABLE56(921,919.9,1.0871),
			new API_TABLE56(922,920.9,1.0859),
			new API_TABLE56(923,921.9,1.0847),
			new API_TABLE56(924,922.9,1.0835),
			new API_TABLE56(925,923.9,1.0823),
			new API_TABLE56(926,924.9,1.0812),
			new API_TABLE56(927,925.9,1.0800),
			new API_TABLE56(928,926.9,1.0788),
			new API_TABLE56(929,927.9,1.0777),
			new API_TABLE56(930,928.9,1.0765),
			new API_TABLE56(931,929.9,1.0754),
			new API_TABLE56(932,930.9,1.0742),
			new API_TABLE56(933,931.9,1.0731),
			new API_TABLE56(934,932.9,1.0719),
			new API_TABLE56(935,933.9,1.0708),
			new API_TABLE56(936,934.9,1.0696),
			new API_TABLE56(937,935.9,1.0685),
			new API_TABLE56(938,936.9,1.0673),
			new API_TABLE56(939,937.9,1.0662),
			new API_TABLE56(940,938.9,1.0651),
			new API_TABLE56(941,939.9,1.0639),
			new API_TABLE56(942,940.9,1.0628),
			new API_TABLE56(943,941.9,1.0617),
			new API_TABLE56(944,942.9,1.0605),
			new API_TABLE56(945,943.9,1.0594),
			new API_TABLE56(946,944.9,1.0583),
			new API_TABLE56(947,945.9,1.0572),
			new API_TABLE56(948,946.9,1.0561),
			new API_TABLE56(949,947.9,1.0549),
			new API_TABLE56(950,948.9,1.0538),
			new API_TABLE56(951,949.9,1.0527),
			new API_TABLE56(952,950.9,1.0516),
			new API_TABLE56(953,951.9,1.0505),
			new API_TABLE56(954,952.9,1.0494),
			new API_TABLE56(955,953.9,1.0483),
			new API_TABLE56(956,954.9,1.0472),
			new API_TABLE56(957,955.9,1.0461),
			new API_TABLE56(958,956.9,1.0450),
			new API_TABLE56(959,957.9,1.0439),
			new API_TABLE56(960,958.9,1.0428),
			new API_TABLE56(961,959.9,1.0417),
			new API_TABLE56(962,960.9,1.0407),
			new API_TABLE56(963,961.9,1.0396),
			new API_TABLE56(964,962.9,1.0385),
			new API_TABLE56(965,963.9,1.0374),
			new API_TABLE56(966,964.9,1.0363),
			new API_TABLE56(967,965.9,1.0353),
			new API_TABLE56(968,966.9,1.0342),
			new API_TABLE56(969,967.9,1.0331),
			new API_TABLE56(970,968.9,1.0321),
			new API_TABLE56(971,969.9,1.0310),
			new API_TABLE56(972,970.9,1.0299),
			new API_TABLE56(973,971.9,1.0289),
			new API_TABLE56(974,972.9,1.0278),
			new API_TABLE56(975,973.9,1.0268),
			new API_TABLE56(976,974.9,1.0257),
			new API_TABLE56(977,975.9,1.0247),
			new API_TABLE56(978,976.9,1.0236),
			new API_TABLE56(979,977.9,1.0226),
			new API_TABLE56(980,978.9,1.0215),
			new API_TABLE56(981,979.9,1.0205),
			new API_TABLE56(982,980.9,1.0194),
			new API_TABLE56(983,981.9,1.0184),
			new API_TABLE56(984,982.9,1.0174),
			new API_TABLE56(985,983.9,1.0163),
			new API_TABLE56(986,984.9,1.0153),
			new API_TABLE56(987,985.9,1.0143),
			new API_TABLE56(988,986.9,1.0132),
			new API_TABLE56(989,987.9,1.0122),
			new API_TABLE56(990,988.9,1.0112),
			new API_TABLE56(991,989.9,1.0102),
			new API_TABLE56(992,990.9,1.0092),
			new API_TABLE56(993,991.9,1.0081),
			new API_TABLE56(994,992.9,1.0071),
			new API_TABLE56(995,993.9,1.0061),
			new API_TABLE56(996,994.9,1.0051),
			new API_TABLE56(997,995.9,1.0041),
			new API_TABLE56(998,996.9,1.0031),
			new API_TABLE56(999,997.9,1.0021),
			new API_TABLE56(1000,998.9,1.0011),
			new API_TABLE56(1001,999.9,1.0001),
			new API_TABLE56(1002,1000.9,0.9991),
			new API_TABLE56(1003,1001.9,0.9981),
			new API_TABLE56(1004,1002.9,0.9971),
			new API_TABLE56(1005,1003.9,0.9961),
			new API_TABLE56(1006,1004.9,0.9951),
			new API_TABLE56(1007,1005.9,0.9941),
			new API_TABLE56(1008,1006.9,0.9931),
			new API_TABLE56(1009,1007.9,0.9921),
			new API_TABLE56(1010,1008.9,0.9911),
			new API_TABLE56(1011,1009.9,0.9902),
			new API_TABLE56(1012,1010.9,0.9892),
			new API_TABLE56(1013,1011.9,0.9882),
			new API_TABLE56(1014,1012.9,0.9872),
			new API_TABLE56(1015,1013.9,0.9863),
			new API_TABLE56(1016,1014.9,0.9853),
			new API_TABLE56(1017,1015.9,0.9843),
			new API_TABLE56(1018,1016.9,0.9833),
			new API_TABLE56(1019,1017.9,0.9824),
			new API_TABLE56(1020,1018.9,0.9814),
			new API_TABLE56(1021,1019.9,0.9805),
			new API_TABLE56(1022,1020.9,0.9795),
			new API_TABLE56(1023,1021.9,0.9785),
			new API_TABLE56(1024,1022.9,0.9776),
			new API_TABLE56(1025,1023.9,0.9766),
			new API_TABLE56(1026,1024.9,0.9757),
			new API_TABLE56(1027,1025.9,0.9747),
			new API_TABLE56(1028,1026.9,0.9738),
			new API_TABLE56(1029,1027.9,0.9728),
			new API_TABLE56(1030,1028.9,0.9719),
			new API_TABLE56(1031,1029.9,0.9709),
			new API_TABLE56(1032,1030.9,0.9700),
			new API_TABLE56(1033,1031.9,0.9691),
			new API_TABLE56(1034,1032.9,0.9681),
			new API_TABLE56(1035,1033.9,0.9672),
			new API_TABLE56(1036,1034.9,0.9662),
			new API_TABLE56(1037,1035.9,0.9653),
			new API_TABLE56(1038,1036.9,0.9644),
			new API_TABLE56(1039,1037.9,0.9635),
			new API_TABLE56(1040,1038.9,0.9625),
			new API_TABLE56(1041,1039.9,0.9616),
			new API_TABLE56(1042,1040.9,0.9607),
			new API_TABLE56(1043,1041.9,0.9598),
			new API_TABLE56(1044,1042.9,0.9588),
			new API_TABLE56(1045,1043.9,0.9579),
			new API_TABLE56(1046,1044.9,0.9570),
			new API_TABLE56(1047,1045.9,0.9561),
			new API_TABLE56(1048,1046.9,0.9552),
			new API_TABLE56(1049,1047.9,0.9543),
			new API_TABLE56(1050,1048.9,0.9533),
			new API_TABLE56(1051,1049.9,0.9524),
			new API_TABLE56(1052,1050.9,0.9515),
			new API_TABLE56(1053,1051.9,0.9506),
			new API_TABLE56(1054,1052.9,0.9497),
			new API_TABLE56(1055,1053.9,0.9488),
			new API_TABLE56(1056,1054.9,0.9479),
			new API_TABLE56(1057,1055.9,0.9470),
			new API_TABLE56(1058,1056.9,0.9461),
			new API_TABLE56(1059,1057.9,0.9452),
			new API_TABLE56(1060,1058.9,0.9443),
			new API_TABLE56(1061,1059.9,0.9434),
			new API_TABLE56(1062,1060.9,0.9426),
			new API_TABLE56(1063,1061.9,0.9417),
			new API_TABLE56(1064,1062.9,0.9408),
			new API_TABLE56(1065,1063.9,0.9399),
			new API_TABLE56(1066,1064.9,0.9390),
			new API_TABLE56(1067,1065.9,0.9381),
			new API_TABLE56(1068,1066.9,0.9373),
			new API_TABLE56(1069,1067.9,0.9364),
			new API_TABLE56(1070,1068.9,0.9355),
			new API_TABLE56(1071,1069.9,0.9346),
			new API_TABLE56(1072,1070.9,0.9338),
			new API_TABLE56(1073,1071.9,0.9329),
			new API_TABLE56(1074,1072.9,0.9320),
			new API_TABLE56(1075,1073.9,0.9311)
		};


		// define the structure and setup the look up parameters for api table 57
		public class API_TABLE57
		{
			public double density;
			public double shortTonsFactor;
			public double longTonsFactor;

			public API_TABLE57(double density, double shortTonsFactor, double longTonsFactor)
			{
				this.density = density;
				this.shortTonsFactor = shortTonsFactor;
				this.longTonsFactor = longTonsFactor;
			}
		}		

 
		public static API_TABLE57[] API_Lookup_Table57 = new API_TABLE57[] {
			new API_TABLE57(654,0.7197,0.6426),
			new API_TABLE57(655,0.7208,0.6436),
			new API_TABLE57(656,0.7219,0.6445),
			new API_TABLE57(657,0.7230,0.6455),
			new API_TABLE57(658,0.7241,0.6465),
			new API_TABLE57(659,0.7252,0.6475),
			new API_TABLE57(660,0.7263,0.6485),
			new API_TABLE57(661,0.7274,0.6495),
			new API_TABLE57(662,0.7285,0.6504),
			new API_TABLE57(663,0.7296,0.6514),
			new API_TABLE57(664,0.7307,0.6524),
			new API_TABLE57(665,0.7318,0.6534),
			new API_TABLE57(666,0.7329,0.6544),
			new API_TABLE57(667,0.7340,0.6554),
			new API_TABLE57(668,0.7351,0.6563),
			new API_TABLE57(669,0.7362,0.6573),
			new API_TABLE57(670,0.7373,0.6583),
			new API_TABLE57(671,0.7384,0.6593),
			new API_TABLE57(672,0.7395,0.6603),
			new API_TABLE57(673,0.7406,0.6613),
			new API_TABLE57(674,0.7417,0.6623),
			new API_TABLE57(675,0.7428,0.6632),
			new API_TABLE57(676,0.7439,0.6642),
			new API_TABLE57(677,0.7450,0.6652),
			new API_TABLE57(678,0.7461,0.6662),
			new API_TABLE57(679,0.7472,0.6672),
			new API_TABLE57(680,0.7483,0.6682),
			new API_TABLE57(681,0.7494,0.6691),
			new API_TABLE57(682,0.7505,0.6701),
			new API_TABLE57(683,0.7516,0.6711),
			new API_TABLE57(684,0.7527,0.6721),
			new API_TABLE57(685,0.7539,0.6731),
			new API_TABLE57(686,0.7550,0.6741),
			new API_TABLE57(687,0.7561,0.6751),
			new API_TABLE57(688,0.7572,0.6760),
			new API_TABLE57(689,0.7583,0.6770),
			new API_TABLE57(690,0.7594,0.6780),
			new API_TABLE57(691,0.7605,0.6790),
			new API_TABLE57(692,0.7616,0.6800),
			new API_TABLE57(693,0.7627,0.6810),
			new API_TABLE57(694,0.7638,0.6819),
			new API_TABLE57(695,0.7649,0.6829),
			new API_TABLE57(696,0.7660,0.6839),
			new API_TABLE57(697,0.7671,0.6849),
			new API_TABLE57(698,0.7682,0.6859),
			new API_TABLE57(699,0.7693,0.6869),
			new API_TABLE57(700,0.7704,0.6878),
			new API_TABLE57(701,0.7715,0.6888),
			new API_TABLE57(702,0.7726,0.6898),
			new API_TABLE57(703,0.7737,0.6908),
			new API_TABLE57(704,0.7748,0.6918),
			new API_TABLE57(705,0.7759,0.6928),
			new API_TABLE57(706,0.7770,0.6938),
			new API_TABLE57(707,0.7781,0.6947),
			new API_TABLE57(708,0.7792,0.6957),
			new API_TABLE57(709,0.7803,0.6967),
			new API_TABLE57(710,0.7814,0.6977),
			new API_TABLE57(711,0.7825,0.6987),
			new API_TABLE57(712,0.7836,0.6997),
			new API_TABLE57(713,0.7847,0.7006),
			new API_TABLE57(714,0.7858,0.7016),
			new API_TABLE57(715,0.7869,0.7026),
			new API_TABLE57(716,0.7880,0.7036),
			new API_TABLE57(717,0.7891,0.7046),
			new API_TABLE57(718,0.7902,0.7056),
			new API_TABLE57(719,0.7913,0.7066),
			new API_TABLE57(720,0.7924,0.7075),
			new API_TABLE57(721,0.7935,0.7085),
			new API_TABLE57(722,0.7946,0.7095),
			new API_TABLE57(723,0.7957,0.7105),
			new API_TABLE57(724,0.7968,0.7115),
			new API_TABLE57(725,0.7980,0.7125),
			new API_TABLE57(726,0.7991,0.7134),
			new API_TABLE57(727,0.8002,0.7144),
			new API_TABLE57(728,0.8013,0.7154),
			new API_TABLE57(729,0.8024,0.7164),
			new API_TABLE57(730,0.8035,0.7174),
			new API_TABLE57(731,0.8046,0.7184),
			new API_TABLE57(732,0.8057,0.7193),
			new API_TABLE57(733,0.8068,0.7203),
			new API_TABLE57(734,0.8079,0.7213),
			new API_TABLE57(735,0.8090,0.7223),
			new API_TABLE57(736,0.8101,0.7233),
			new API_TABLE57(737,0.8112,0.7243),
			new API_TABLE57(738,0.8123,0.7253),
			new API_TABLE57(739,0.8134,0.7262),
			new API_TABLE57(740,0.8145,0.7272),
			new API_TABLE57(741,0.8156,0.7282),
			new API_TABLE57(742,0.8167,0.7292),
			new API_TABLE57(743,0.8178,0.7302),
			new API_TABLE57(744,0.8189,0.7312),
			new API_TABLE57(745,0.8200,0.7321),
			new API_TABLE57(746,0.8211,0.7331),
			new API_TABLE57(747,0.8222,0.7341),
			new API_TABLE57(748,0.8233,0.7351),
			new API_TABLE57(749,0.8244,0.7361),
			new API_TABLE57(750,0.8255,0.7371),
			new API_TABLE57(751,0.8266,0.7380),
			new API_TABLE57(752,0.8277,0.7390),
			new API_TABLE57(753,0.8288,0.7400),
			new API_TABLE57(754,0.8299,0.7410),
			new API_TABLE57(755,0.8310,0.7420),
			new API_TABLE57(756,0.8321,0.7430),
			new API_TABLE57(757,0.8332,0.7440),
			new API_TABLE57(758,0.8343,0.7449),
			new API_TABLE57(759,0.8354,0.7459),
			new API_TABLE57(760,0.8365,0.7469),
			new API_TABLE57(761,0.8376,0.7479),
			new API_TABLE57(762,0.8387,0.7489),
			new API_TABLE57(763,0.8398,0.7499),
			new API_TABLE57(764,0.8409,0.7508),
			new API_TABLE57(765,0.8421,0.7518),
			new API_TABLE57(766,0.8432,0.7528),
			new API_TABLE57(767,0.8443,0.7538),
			new API_TABLE57(768,0.8454,0.7548),
			new API_TABLE57(769,0.8465,0.7558),
			new API_TABLE57(770,0.8476,0.7568),
			new API_TABLE57(771,0.8487,0.7577),
			new API_TABLE57(772,0.8498,0.7587),
			new API_TABLE57(773,0.8509,0.7597),
			new API_TABLE57(774,0.8520,0.7607),
			new API_TABLE57(775,0.8531,0.7617),
			new API_TABLE57(776,0.8542,0.7627),
			new API_TABLE57(777,0.8553,0.7636),
			new API_TABLE57(778,0.8564,0.7646),
			new API_TABLE57(779,0.8575,0.7656),
			new API_TABLE57(780,0.8586,0.7666),
			new API_TABLE57(781,0.8597,0.7676),
			new API_TABLE57(782,0.8608,0.7686),
			new API_TABLE57(783,0.8619,0.7695),
			new API_TABLE57(784,0.8630,0.7705),
			new API_TABLE57(785,0.8641,0.7715),
			new API_TABLE57(786,0.8652,0.7725),
			new API_TABLE57(787,0.8663,0.7735),
			new API_TABLE57(788,0.8674,0.7745),
			new API_TABLE57(789,0.8685,0.7755),
			new API_TABLE57(790,0.8696,0.7764),
			new API_TABLE57(791,0.8707,0.7774),
			new API_TABLE57(792,0.8718,0.7784),
			new API_TABLE57(793,0.8729,0.7794),
			new API_TABLE57(794,0.8740,0.7804),
			new API_TABLE57(795,0.8751,0.7814),
			new API_TABLE57(796,0.8762,0.7823),
			new API_TABLE57(797,0.8773,0.7833),
			new API_TABLE57(798,0.8784,0.7843),
			new API_TABLE57(799,0.8795,0.7853),
			new API_TABLE57(800,0.8806,0.7863),
			new API_TABLE57(801,0.8817,0.7873),
			new API_TABLE57(802,0.8828,0.7883),
			new API_TABLE57(803,0.8839,0.7892),
			new API_TABLE57(804,0.8850,0.7902),
			new API_TABLE57(805,0.8861,0.7912),
			new API_TABLE57(806,0.8873,0.7922),
			new API_TABLE57(807,0.8884,0.7932),
			new API_TABLE57(808,0.8895,0.7942),
			new API_TABLE57(809,0.8906,0.7951),
			new API_TABLE57(810,0.8917,0.7961),
			new API_TABLE57(811,0.8928,0.7971),
			new API_TABLE57(812,0.8939,0.7981),
			new API_TABLE57(813,0.8950,0.7991),
			new API_TABLE57(814,0.8961,0.8001),
			new API_TABLE57(815,0.8972,0.8010),
			new API_TABLE57(816,0.8983,0.8020),
			new API_TABLE57(817,0.8994,0.8030),
			new API_TABLE57(818,0.9005,0.8040),
			new API_TABLE57(819,0.9016,0.8050),
			new API_TABLE57(820,0.9027,0.8060),
			new API_TABLE57(821,0.9038,0.8070),
			new API_TABLE57(822,0.9049,0.8079),
			new API_TABLE57(823,0.9060,0.8089),
			new API_TABLE57(824,0.9071,0.8099),
			new API_TABLE57(825,0.9082,0.8109),
			new API_TABLE57(826,0.9093,0.8119),
			new API_TABLE57(827,0.9104,0.8129),
			new API_TABLE57(828,0.9115,0.8138),
			new API_TABLE57(829,0.9126,0.8148),
			new API_TABLE57(830,0.9137,0.8158),
			new API_TABLE57(831,0.9148,0.8168),
			new API_TABLE57(832,0.9159,0.8178),
			new API_TABLE57(833,0.9170,0.8188),
			new API_TABLE57(834,0.9181,0.8198),
			new API_TABLE57(835,0.9192,0.8207),
			new API_TABLE57(836,0.9203,0.8217),
			new API_TABLE57(837,0.9214,0.8227),
			new API_TABLE57(838,0.9225,0.8237),
			new API_TABLE57(839,0.9236,0.8247),
			new API_TABLE57(840,0.9247,0.8257),
			new API_TABLE57(841,0.9258,0.8266),
			new API_TABLE57(842,0.9269,0.8276),
			new API_TABLE57(843,0.9280,0.8286),
			new API_TABLE57(844,0.9291,0.8296),
			new API_TABLE57(845,0.9302,0.8306),
			new API_TABLE57(846,0.9314,0.8316),
			new API_TABLE57(847,0.9325,0.8325),
			new API_TABLE57(848,0.9336,0.8335),
			new API_TABLE57(849,0.9347,0.8345),
			new API_TABLE57(850,0.9358,0.8355),
			new API_TABLE57(851,0.9369,0.8365),
			new API_TABLE57(852,0.9380,0.8375),
			new API_TABLE57(853,0.9391,0.8385),
			new API_TABLE57(854,0.9402,0.8394),
			new API_TABLE57(855,0.9413,0.8404),
			new API_TABLE57(856,0.9424,0.8414),
			new API_TABLE57(857,0.9435,0.8424),
			new API_TABLE57(858,0.9446,0.8434),
			new API_TABLE57(859,0.9457,0.8444),
			new API_TABLE57(860,0.9468,0.8453),
			new API_TABLE57(861,0.9479,0.8463),
			new API_TABLE57(862,0.9490,0.8473),
			new API_TABLE57(863,0.9501,0.8483),
			new API_TABLE57(864,0.9512,0.8493),
			new API_TABLE57(865,0.9523,0.8503),
			new API_TABLE57(866,0.9534,0.8513),
			new API_TABLE57(867,0.9545,0.8522),
			new API_TABLE57(868,0.9556,0.8532),
			new API_TABLE57(869,0.9567,0.8542),
			new API_TABLE57(870,0.9578,0.8552),
			new API_TABLE57(871,0.9589,0.8562),
			new API_TABLE57(872,0.9600,0.8572),
			new API_TABLE57(873,0.9611,0.8581),
			new API_TABLE57(874,0.9622,0.8591),
			new API_TABLE57(875,0.9633,0.8601),
			new API_TABLE57(876,0.9644,0.8611),
			new API_TABLE57(877,0.9655,0.8621),
			new API_TABLE57(878,0.9666,0.8631),
			new API_TABLE57(879,0.9677,0.8640),
			new API_TABLE57(880,0.9688,0.8650),
			new API_TABLE57(881,0.9699,0.8660),
			new API_TABLE57(882,0.9710,0.8670),
			new API_TABLE57(883,0.9721,0.8680),
			new API_TABLE57(884,0.9732,0.8690),
			new API_TABLE57(885,0.9743,0.8700),
			new API_TABLE57(886,0.9754,0.8709),
			new API_TABLE57(887,0.9766,0.8719),
			new API_TABLE57(888,0.9777,0.8729),
			new API_TABLE57(889,0.9788,0.8739),
			new API_TABLE57(890,0.9799,0.8749),
			new API_TABLE57(891,0.9810,0.8759),
			new API_TABLE57(892,0.9821,0.8768),
			new API_TABLE57(893,0.9832,0.8778),
			new API_TABLE57(894,0.9843,0.8788),
			new API_TABLE57(895,0.9854,0.8798),
			new API_TABLE57(896,0.9865,0.8808),
			new API_TABLE57(897,0.9876,0.8818),
			new API_TABLE57(898,0.9887,0.8827),
			new API_TABLE57(899,0.9898,0.8837),
			new API_TABLE57(900,0.9909,0.8847),
			new API_TABLE57(901,0.9920,0.8857),
			new API_TABLE57(902,0.9931,0.8867),
			new API_TABLE57(903,0.9942,0.8877),
			new API_TABLE57(904,0.9953,0.8887),
			new API_TABLE57(905,0.9964,0.8896),
			new API_TABLE57(906,0.9975,0.8906),
			new API_TABLE57(907,0.9986,0.8916),
			new API_TABLE57(908,0.9997,0.8926),
			new API_TABLE57(909,1.0008,0.8936),
			new API_TABLE57(910,1.0019,0.8946),
			new API_TABLE57(911,1.0030,0.8955),
			new API_TABLE57(912,1.0041,0.8965),
			new API_TABLE57(913,1.0052,0.8975),
			new API_TABLE57(914,1.0063,0.8985),
			new API_TABLE57(915,1.0074,0.8995),
			new API_TABLE57(916,1.0085,0.9005),
			new API_TABLE57(917,1.0096,0.9015),
			new API_TABLE57(918,1.0107,0.9024),
			new API_TABLE57(919,1.0118,0.9034),
			new API_TABLE57(920,1.0129,0.9044),
			new API_TABLE57(921,1.0140,0.9054),
			new API_TABLE57(922,1.0151,0.9064),
			new API_TABLE57(923,1.0162,0.9074),
			new API_TABLE57(924,1.0173,0.9083),
			new API_TABLE57(925,1.0184,0.9093),
			new API_TABLE57(926,1.0195,0.9103),
			new API_TABLE57(927,1.0207,0.9113),
			new API_TABLE57(928,1.0218,0.9123),
			new API_TABLE57(929,1.0229,0.9133),
			new API_TABLE57(930,1.0240,0.9142),
			new API_TABLE57(931,1.0251,0.9152),
			new API_TABLE57(932,1.0262,0.9162),
			new API_TABLE57(933,1.0273,0.9172),
			new API_TABLE57(934,1.0284,0.9182),
			new API_TABLE57(935,1.0295,0.9192),
			new API_TABLE57(936,1.0306,0.9202),
			new API_TABLE57(937,1.0317,0.9211),
			new API_TABLE57(938,1.0328,0.9221),
			new API_TABLE57(939,1.0339,0.9231),
			new API_TABLE57(940,1.0350,0.9241),
			new API_TABLE57(941,1.0361,0.9251),
			new API_TABLE57(942,1.0372,0.9261),
			new API_TABLE57(943,1.0383,0.9270),
			new API_TABLE57(944,1.0394,0.9280),
			new API_TABLE57(945,1.0405,0.9290),
			new API_TABLE57(946,1.0416,0.9300),
			new API_TABLE57(947,1.0427,0.9310),
			new API_TABLE57(948,1.0438,0.9320),
			new API_TABLE57(949,1.0449,0.9330),
			new API_TABLE57(950,1.0460,0.9339),
			new API_TABLE57(951,1.0471,0.9349),
			new API_TABLE57(952,1.0482,0.9359),
			new API_TABLE57(953,1.0493,0.9369),
			new API_TABLE57(954,1.0504,0.9379),
			new API_TABLE57(955,1.0515,0.9389),
			new API_TABLE57(956,1.0526,0.9398),
			new API_TABLE57(957,1.0537,0.9408),
			new API_TABLE57(958,1.0548,0.9418),
			new API_TABLE57(959,1.0559,0.9428),
			new API_TABLE57(960,1.0570,0.9438),
			new API_TABLE57(961,1.0581,0.9448),
			new API_TABLE57(962,1.0592,0.9457),
			new API_TABLE57(963,1.0603,0.9467),
			new API_TABLE57(964,1.0614,0.9477),
			new API_TABLE57(965,1.0625,0.9487),
			new API_TABLE57(966,1.0636,0.9497),
			new API_TABLE57(967,1.0648,0.9507),
			new API_TABLE57(968,1.0659,0.9517),
			new API_TABLE57(969,1.0670,0.9526),
			new API_TABLE57(970,1.0681,0.9536),
			new API_TABLE57(971,1.0692,0.9546),
			new API_TABLE57(972,1.0703,0.9556),
			new API_TABLE57(973,1.0714,0.9566),
			new API_TABLE57(974,1.0725,0.9576),
			new API_TABLE57(975,1.0736,0.9585),
			new API_TABLE57(976,1.0747,0.9595),
			new API_TABLE57(977,1.0758,0.9605),
			new API_TABLE57(978,1.0769,0.9615),
			new API_TABLE57(979,1.0780,0.9625),
			new API_TABLE57(980,1.0791,0.9635),
			new API_TABLE57(981,1.0802,0.9645),
			new API_TABLE57(982,1.0813,0.9654),
			new API_TABLE57(983,1.0824,0.9664),
			new API_TABLE57(984,1.0835,0.9674),
			new API_TABLE57(985,1.0846,0.9684),
			new API_TABLE57(986,1.0857,0.9694),
			new API_TABLE57(987,1.0868,0.9704),
			new API_TABLE57(988,1.0879,0.9713),
			new API_TABLE57(989,1.0890,0.9723),
			new API_TABLE57(990,1.0901,0.9733),
			new API_TABLE57(991,1.0912,0.9743),
			new API_TABLE57(992,1.0923,0.9753),
			new API_TABLE57(993,1.0934,0.9763),
			new API_TABLE57(994,1.0945,0.9772),
			new API_TABLE57(995,1.0956,0.9782),
			new API_TABLE57(996,1.0967,0.9792),
			new API_TABLE57(997,1.0978,0.9802),
			new API_TABLE57(998,1.0989,0.9812),
			new API_TABLE57(999,1.1000,0.9822),
			new API_TABLE57(1000,1.1011,0.9832),
			new API_TABLE57(1001,1.1022,0.9841),
			new API_TABLE57(1002,1.1033,0.9851),
			new API_TABLE57(1003,1.1044,0.9861),
			new API_TABLE57(1004,1.1055,0.9871),
			new API_TABLE57(1005,1.1066,0.9881),
			new API_TABLE57(1006,1.1077,0.9891),
			new API_TABLE57(1007,1.1088,0.9900),
			new API_TABLE57(1008,1.1100,0.9910),
			new API_TABLE57(1009,1.1111,0.9920),
			new API_TABLE57(1010,1.1122,0.9930),
			new API_TABLE57(1011,1.1133,0.9940),
			new API_TABLE57(1012,1.1144,0.9950),
			new API_TABLE57(1013,1.1155,0.9960),
			new API_TABLE57(1014,1.1166,0.9969),
			new API_TABLE57(1015,1.1177,0.9979),
			new API_TABLE57(1016,1.1188,0.9989),
			new API_TABLE57(1017,1.1199,0.9999),
			new API_TABLE57(1018,1.1210,1.0009),
			new API_TABLE57(1019,1.1221,1.0019),
			new API_TABLE57(1020,1.1232,1.0028),
			new API_TABLE57(1021,1.1243,1.0038),
			new API_TABLE57(1022,1.1254,1.0048),
			new API_TABLE57(1023,1.1265,1.0058),
			new API_TABLE57(1024,1.1276,1.0068),
			new API_TABLE57(1025,1.1287,1.0078),
			new API_TABLE57(1026,1.1298,1.0087),
			new API_TABLE57(1027,1.1309,1.0097),
			new API_TABLE57(1028,1.1320,1.0107),
			new API_TABLE57(1029,1.1331,1.0117),
			new API_TABLE57(1030,1.1342,1.0127),
			new API_TABLE57(1031,1.1353,1.0137),
			new API_TABLE57(1032,1.1364,1.0147),
			new API_TABLE57(1033,1.1375,1.0156),
			new API_TABLE57(1034,1.1386,1.0166),
			new API_TABLE57(1035,1.1397,1.0176),
			new API_TABLE57(1036,1.1408,1.0186),
			new API_TABLE57(1037,1.1419,1.0196),
			new API_TABLE57(1038,1.1430,1.0206),
			new API_TABLE57(1039,1.1441,1.0215),
			new API_TABLE57(1040,1.1452,1.0225),
			new API_TABLE57(1041,1.1463,1.0235),
			new API_TABLE57(1042,1.1474,1.0245),
			new API_TABLE57(1043,1.1485,1.0255),
			new API_TABLE57(1044,1.1496,1.0265),
			new API_TABLE57(1045,1.1507,1.0274),
			new API_TABLE57(1046,1.1518,1.0284),
			new API_TABLE57(1047,1.1529,1.0294),
			new API_TABLE57(1048,1.1541,1.0304),
			new API_TABLE57(1049,1.1552,1.0314),
			new API_TABLE57(1050,1.1563,1.0324),
			new API_TABLE57(1051,1.1574,1.0334),
			new API_TABLE57(1052,1.1585,1.0343),
			new API_TABLE57(1053,1.1596,1.0353),
			new API_TABLE57(1054,1.1607,1.0363),
			new API_TABLE57(1055,1.1618,1.0373),
			new API_TABLE57(1056,1.1629,1.0383),
			new API_TABLE57(1057,1.1640,1.0393),
			new API_TABLE57(1058,1.1651,1.0402),
			new API_TABLE57(1059,1.1662,1.0412),
			new API_TABLE57(1060,1.1673,1.0422),
			new API_TABLE57(1061,1.1684,1.0432),
			new API_TABLE57(1062,1.1695,1.0442),
			new API_TABLE57(1063,1.1706,1.0452),
			new API_TABLE57(1064,1.1717,1.0462),
			new API_TABLE57(1065,1.1728,1.0471),
			new API_TABLE57(1066,1.1739,1.0481),
			new API_TABLE57(1067,1.1750,1.0491),
			new API_TABLE57(1068,1.1761,1.0501),
			new API_TABLE57(1069,1.1772,1.0511),
			new API_TABLE57(1070,1.1783,1.0521),
			new API_TABLE57(1071,1.1794,1.0530),
			new API_TABLE57(1072,1.1805,1.0540),
			new API_TABLE57(1073,1.1816,1.0550),
			new API_TABLE57(1074,1.1827,1.0560),
			new API_TABLE57(1075,1.1838,1.0570)
		};


		const int API_TABLE52_ENTRIES = 8;

		class API_TABLE52
		{
			public double densityStart;
			public double densityStop;
			public double cubicMetersperBarrel;

			public API_TABLE52( double densityStart,double densityStop,double cubicMetersperBarrel)
			{
				this.densityStart = densityStart;
				this.densityStop = densityStop;
				this.cubicMetersperBarrel = cubicMetersperBarrel;
			}
		}

		static API_TABLE52[] API_Lookup_Table52 = new API_TABLE52[] {
				new API_TABLE52 (654.0, 683.0, 0.15886),
				new API_TABLE52 (684.0,722.0,0.15887),
				new API_TABLE52 (723.0,768.0,0.15888),
				new API_TABLE52 (769.0,779.0,0.15889),
				new API_TABLE52 (780.0,798.0,0.15890),
				new API_TABLE52 (799.0,859.0,0.15891),
				new API_TABLE52 (860.0,964.0,0.15892),
				new API_TABLE52 (965.0,1074,0.15893)
		};
	}
}
