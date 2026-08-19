namespace StrapTables
{
	using System;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;
	using System.Collections.Generic;
	using Varec.CommonComponents.EngineeringUnitsLibrary;
	using FMPointCommon;
	using Opc.Ua;

	public class FMStrapTable : FuelsManagerModule, IFuelsManagerModule
	{
		public Vessel Vessel { get; set; }
		public StrapTable StrapTable { get; set; }

		public FMStrapTable() : base() { }

		public void StrapCalculation(PointTag LevelProduct, PointTag LevelWater, PointTag LevelSolids, PointTag VolumeStrapProduct, PointTag VolumeStrapWater, PointTag VolumeStrapSolids)
		{
			this.StrapVolumeCalculation(LevelProduct, VolumeStrapProduct);

			this.WaterStrapVolumeCalculation(LevelWater, LevelSolids, VolumeStrapSolids, VolumeStrapWater);
		}


		public bool? StrapVolumeCalculation(PointTag level, PointTag StrapVolume)
		{
			double? returnedValue = 0.0;

			// if the output is not in calculated just return

			if (StrapVolume.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
				StrapVolume.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
				return false;

			// check the status if the input variable
			if (!IsValueGood(level))
			{
				if (StrapVolume.Value != null ||
					StrapVolume.Status != StatusCodes.Bad)
				{
					StrapVolume.Value = null;
					StrapVolume.Status = StatusCodes.Bad;
					StrapVolume.ServerTimeStamp = level.ServerTimeStamp;
					StrapVolume.SourceTimeStamp = level.SourceTimeStamp;
				}
				return false;
			}


			var productDatumHeight = this.StrapTable.StrapTables[this.StrapTable.SelectedTableForStrap].DatumHeight.Value;
			var bottomsDatumHeight = this.StrapTable.StrapTables[this.StrapTable.SelectedTableForWaterVolume].DatumHeight.Value;
			var productlevel = (double)level.Value;

			if(level.Units == EngineeringUnit.FmlFtIn16Th || level.Units == EngineeringUnit.FmlFtIn8Th)
            {
				productDatumHeight = Math.Round(productDatumHeight, 12, MidpointRounding.AwayFromZero);
				bottomsDatumHeight = Math.Round(bottomsDatumHeight, 12, MidpointRounding.AwayFromZero);
				productlevel = Math.Round(productlevel, 12, MidpointRounding.AwayFromZero);
			}

			if (productlevel < productDatumHeight)
			{
				if ((productDatumHeight - productlevel) <= bottomsDatumHeight)
				{
					returnedValue = StrapTable.GetVolumeFromLevel(productlevel + bottomsDatumHeight, this.StrapTable.SelectedTableForWaterVolume, Vessel.TankGeometry);
				}

				// level is underrange and no calculation can be performed.
				else
                {
					returnedValue = null;
                }
			}
			else
			{
				returnedValue = StrapTable.GetVolumeFromLevel(productlevel, this.StrapTable.SelectedTableForStrap, Vessel.TankGeometry);
			}

			if (!returnedValue.HasValue)
			{
				if (StrapVolume.Value != null ||
					StrapVolume.Status != StatusCodes.Bad)
				{
					StrapVolume.Value = null;
					StrapVolume.Status = StatusCodes.Bad;
					StrapVolume.ServerTimeStamp = level.ServerTimeStamp;
					StrapVolume.SourceTimeStamp = level.SourceTimeStamp;
				}
				return false;
			}

			long newStatus = StatusCodes.Good;

			if (IsStatusUncertain(level))
			{
				newStatus = StatusCodes.Uncertain;
			}



			if (StrapVolume.Value == null ||
				(double)StrapVolume.Value != returnedValue
				|| IsStatusChange(StrapVolume.Status, newStatus))
			{
				StrapVolume.Value = returnedValue;
				StrapVolume.Status = newStatus;
				CheckForAndSetOverUnderRange(StrapVolume);

				StrapVolume.SourceTimeStamp = level.SourceTimeStamp;
				StrapVolume.ServerTimeStamp = level.ServerTimeStamp;
			}


			return true;
		}

		private bool? SolidsVolumeCalculation(PointTag Solidslevel, ref PointTag SolidsVolume)
		{
			double? returnedValue = 0.0;

			// if the output is not in calculated just return

			if (SolidsVolume.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
				SolidsVolume.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
				return false;

			// check the status if the input variable
			if (!IsValueGood(Solidslevel))
			{
				if (SolidsVolume.Value != null ||
					SolidsVolume.Status != StatusCodes.Bad)
				{
					SolidsVolume.Value = null;
					SolidsVolume.Status = StatusCodes.Bad;
               base.SetTimeStamps(new PointTag[] { Solidslevel }, SolidsVolume);
                }
				return false;
			}

			// if the solid level is zero then set the volume at zero and do not look in the strap table
			// this is how SI wants it to operate. Not sure why

			var bottomsDatumHeight = this.StrapTable.StrapTables[this.StrapTable.SelectedTableForWaterVolume].DatumHeight.Value;
			var solidsLevel = (double)Solidslevel.Value;

			if (Solidslevel.Units == EngineeringUnit.FmlFtIn16Th || Solidslevel.Units == EngineeringUnit.FmlFtIn8Th)
			{
				bottomsDatumHeight = Math.Round(bottomsDatumHeight, 12, MidpointRounding.AwayFromZero);
				solidsLevel = Math.Round(solidsLevel, 12, MidpointRounding.AwayFromZero);
			}


			if ((double)Solidslevel.Value == 0.0)
			{
				returnedValue = 0.0;
			}
			else
			{

				if (solidsLevel <= bottomsDatumHeight)
				{
					returnedValue = StrapTable.GetVolumeFromLevel(solidsLevel, this.StrapTable.SelectedTableForWaterVolume, Vessel.TankGeometry);
				}
				else
				{
					returnedValue = StrapTable.GetVolumeFromLevel(solidsLevel - bottomsDatumHeight, this.StrapTable.SelectedTableForStrap, Vessel.TankGeometry);
				}
			}

			if (!returnedValue.HasValue)
			{
				if (SolidsVolume.Value != null ||
					SolidsVolume.Status != StatusCodes.Bad)
				{
					SolidsVolume.Value = null;
					SolidsVolume.Status = StatusCodes.Bad;
               base.SetTimeStamps(new PointTag[] { Solidslevel }, SolidsVolume);
                }
				return false;
			}

			long newStatus = StatusCodes.Good;

			if (IsStatusUncertain(Solidslevel))
			{
				newStatus = StatusCodes.Uncertain;
			}



			if (SolidsVolume.Value == null ||
				(double)SolidsVolume.Value != returnedValue
				|| IsStatusChange(SolidsVolume.Status, newStatus))
			{
				SolidsVolume.Value = returnedValue;
				SolidsVolume.Status = newStatus;
				CheckForAndSetOverUnderRange(SolidsVolume);

				SolidsVolume.ServerTimeStamp = Solidslevel.ServerTimeStamp;
				SolidsVolume.SourceTimeStamp = Solidslevel.SourceTimeStamp;
			}

			return true;
		}

		public bool? WaterStrapVolumeCalculation(PointTag Waterlevel, PointTag Solidslevel, PointTag SolidsVolume, PointTag WaterVolume)
		{
			double? returnedValue = 0.0;

			DateTimeOffset CurrentDateTime = DateTimeOffset.UtcNow;

			// since solids volume needs to be set for water volume and we can not control the order of the calls into the strap table module
			// we will calculate solids volume here first
			SolidsVolumeCalculation(Solidslevel, ref SolidsVolume);

			// if the output is not in calculated just return
			if (WaterVolume.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
				WaterVolume.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
				return false;

			if(!IsValueGood(Waterlevel) ||
			!IsValueGood(SolidsVolume))
			{
				if (WaterVolume.Value != null ||
					WaterVolume.Status != StatusCodes.Bad)
				{
					WaterVolume.Value = null;
					WaterVolume.Status = StatusCodes.Bad;
               base.SetTimeStamps(new PointTag[] { Waterlevel, SolidsVolume }, WaterVolume);
				}
				return false;
			}

			// if the water level is zero then set the volume at zero and do not look in the strap table
			// this is how SI wants it to operate. Not sure why

			var waterLevel = (double) Waterlevel.Value;
			var bottomsDatumHeight = this.StrapTable.StrapTables[this.StrapTable.SelectedTableForWaterVolume].DatumHeight.Value;

			if (Waterlevel.Units == EngineeringUnit.FmlFtIn16Th || Waterlevel.Units == EngineeringUnit.FmlFtIn8Th)
			{
				bottomsDatumHeight = Math.Round(bottomsDatumHeight, 12, MidpointRounding.AwayFromZero);
				waterLevel = Math.Round(waterLevel, 12, MidpointRounding.AwayFromZero);
			}


			if (waterLevel == 0.0)
			{
				returnedValue = 0.0;
			}
			else
			{

				if (waterLevel <= bottomsDatumHeight)
				{
					returnedValue = StrapTable.GetVolumeFromLevel(waterLevel, this.StrapTable.SelectedTableForWaterVolume, Vessel.TankGeometry);
				}
				else
                {
					returnedValue = StrapTable.GetVolumeFromLevel(waterLevel - bottomsDatumHeight, this.StrapTable.SelectedTableForStrap, Vessel.TankGeometry);
				}
			}
			if (!returnedValue.HasValue)
			{
				if (WaterVolume.Value != null ||
					WaterVolume.Status != StatusCodes.Bad)
				{
					WaterVolume.Value = null;
					WaterVolume.Status = StatusCodes.Bad;
               base.SetTimeStamps(new PointTag[] { Waterlevel }, WaterVolume);
                }
				return false;
			}

			// assume that the units of the strap table are the same as the water volume units
			double solidsvolconverted = 0.0;

			EngineeringUnits.Convert((double)SolidsVolume.Value, SolidsVolume.Units, ref solidsvolconverted, WaterVolume.Units, 60.0);

			double newValue = 0.0;

			if (solidsvolconverted > returnedValue)
			{
				newValue = 0.0;
			}
			else
			{
				newValue = returnedValue.Value - solidsvolconverted;
			}

			long newStatus = StatusCodes.Good;

			if (IsStatusUncertain(Waterlevel) ||
				IsStatusUncertain(SolidsVolume))
			{
				newStatus = StatusCodes.Uncertain;
			}

			if (WaterVolume.Value == null ||
			(double)WaterVolume.Value != newValue
			|| IsStatusChange(WaterVolume.Status, newStatus))
			{
				WaterVolume.Value = newValue;
				WaterVolume.Status = newStatus;
				CheckForAndSetOverUnderRange(WaterVolume);


				// set the timestamp
				WaterVolume.ServerTimeStamp = Waterlevel.ServerTimeStamp;
				WaterVolume.SourceTimeStamp = Waterlevel.SourceTimeStamp;
				if (WaterVolume.SourceTimeStamp < SolidsVolume.SourceTimeStamp)
				{
					WaterVolume.ServerTimeStamp = SolidsVolume.ServerTimeStamp;
					WaterVolume.SourceTimeStamp = SolidsVolume.SourceTimeStamp;
				}
			}

			return true;
		}

		public bool? StrapLevelCalculation(PointTag StrapVolume, PointTag StrapLevel)
		{
			double? returnedValue = 0.0;

			// if the output is not in calculated just return
			if (StrapLevel.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
				StrapLevel.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
				return false;

			// check the status if the input variable
			if (!IsValueGood(StrapVolume))
			{
				if (StrapLevel.Value != null ||
					StrapLevel.Status != StatusCodes.Bad)
				{
					StrapLevel.Value = null;
					StrapLevel.Status = StatusCodes.Bad;
               base.SetTimeStamps(new PointTag[] { StrapVolume }, StrapLevel);
                }
				return false;
			}

			returnedValue = StrapTable.GetLevelFromVolume((double)StrapVolume.Value, this.StrapTable.SelectedTableForStrap, Vessel.TankGeometry);

			if (returnedValue.HasValue)
			{
				var bottomsDatumHeight = this.StrapTable.StrapTables[this.StrapTable.SelectedTableForWaterVolume].DatumHeight.Value;

				if (returnedValue < bottomsDatumHeight)
				{
					returnedValue = StrapTable.GetLevelFromVolume((double)StrapVolume.Value, this.StrapTable.SelectedTableForWaterVolume, Vessel.TankGeometry);

					if (returnedValue.HasValue)
					{
						returnedValue -= bottomsDatumHeight;
					}
				}
			}

			if (!returnedValue.HasValue)
			{
				if (StrapLevel.Value != null ||
					StrapLevel.Status != StatusCodes.Bad)
				{
					StrapLevel.Value = null;
					StrapLevel.Status = StatusCodes.Bad;
               base.SetTimeStamps(new PointTag[] { StrapVolume }, StrapLevel);
				}
				return false;
			}

			long newStatus = StatusCodes.Good;

			if (IsStatusUncertain(StrapVolume))
			{
				newStatus = StatusCodes.Uncertain;
			}

			if (StrapLevel.Value == null ||
				(double)StrapLevel.Value != returnedValue
				|| IsStatusChange(StrapVolume.Status, newStatus))
			{
				StrapLevel.Value = returnedValue;
				StrapLevel.Status = newStatus;
				CheckForAndSetOverUnderRange(StrapLevel);

				StrapLevel.SourceTimeStamp = StrapVolume.SourceTimeStamp;
				StrapLevel.ServerTimeStamp = StrapVolume.ServerTimeStamp;
			}

			return true;
		}



		public ModuleInputOutputCollection GetInputOutputCollection(string calculationName)
		{
			// No need to check calculation name since the parameters are the 
			// same for the two functions (calculations) available in this module.

			if (calculationName == "StrapVolumeCalculation")
			{
				var properties = new ModuleInputOutputCollection
								{
									new ModuleInputOutput
									{
										ID = "level",
										Type = typeof(double?),
										ParameterType = ModuleInputOutputType.Input
									},
									new ModuleInputOutput
									{
										ID = "waterstrapvolume",
										Type = typeof(double?),
										ParameterType = ModuleInputOutputType.Input
									},
									new ModuleInputOutput
									{
										ID = "strapvolume",
										Type = typeof(double?),
										ParameterType = ModuleInputOutputType.Output
									}
								};

				return properties;
			}
			else if (calculationName == "WaterStrapVolumeCalculation")
			{
				var properties = new ModuleInputOutputCollection
								{
									new ModuleInputOutput
									{
										ID = "waterlevel",
										Type = typeof(double?),
										ParameterType = ModuleInputOutputType.Input
									},
									new ModuleInputOutput
									{
										ID = "waterstrapvolume",
										Type = typeof(double?),
										ParameterType = ModuleInputOutputType.Output
									}
								};

				return properties;
			}

			throw new Exception("Invalid Calculation");
		}


		public void GetMinMaxStrapTableLevel(ref double? minLevel, ref double? maxLevel)
		{
			StrapTable.GetMinStrapTableLevelFromSelectedStrap(this.StrapTable.SelectedTableForStrap, ref minLevel, ref maxLevel);
		}
	}
}
