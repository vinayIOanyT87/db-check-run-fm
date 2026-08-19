namespace ShellCorrection
{
	using FMBusinessObjects.DataObjects;
   using FMBusinessObjects.DataObjects.CodedVariables;
	using FMBusinessObjects.Interfaces;
	using Varec.CommonComponents.EngineeringUnitsLibrary;
    using System;
	using FMPointCommon;
	using Opc.Ua;

	public class FMShellCorrection : FuelsManagerModule, IFuelsManagerModule
	{
		public Vessel Vessel { get; set; }

		public StrapTable StrapTable { get; set; }

		public QuantityModuleSettings QuantitySettings { get; set; }

		public VcfModuleSettings VcfSettings { get; set; }

		public GetPointHandler GetPoint = null;


		public FMShellCorrection() : base() { }


		public bool? ShellCorrectionCalculation(PointTag AmbientTemperature,
																PointTag ProductTemperature,
																PointTag TankShellCorrection)
		{

			if (TankShellCorrection.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated
			|| TankShellCorrection.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return false;
			}

			if (!this.Vessel.TankShellCorrectionEnabled
			|| this.Vessel.TankGeometry != TankGeometryEnum.VerticalCylinder
			|| this.Vessel.TankExpansionCoefficient.Value == 0.0)
			{
				if (!(TankShellCorrection.Value is double?)
				|| !((double?)TankShellCorrection.Value).HasValue
				|| ((double?)TankShellCorrection.Value).Value != 1.0
				|| StatusCode.IsBad(TankShellCorrection.OpcStatusSubCode))
				{
					TankShellCorrection.Value = 1.0;
					TankShellCorrection.Status = StatusCodes.Good;
					base.SetTimeStamps(new PointTag[] { AmbientTemperature, ProductTemperature }, TankShellCorrection);
            }
				return true;
			}


			// check the status and values of the input variables
			if ((!this.Vessel.TankShellInsulated
			&& (StatusCode.IsBad(AmbientTemperature.OpcStatusSubCode)
			|| (!(AmbientTemperature.Value is double?)
			|| !((double?)AmbientTemperature.Value).HasValue))
			|| StatusCode.IsBad(ProductTemperature.OpcStatusSubCode)
			|| !(ProductTemperature.Value is double?)
			|| !((double?)ProductTemperature.Value).HasValue))
			{
				if (TankShellCorrection.Status != StatusCodes.Bad ||
					TankShellCorrection.Value != null)
				{
					TankShellCorrection.Status = StatusCodes.Bad;
					TankShellCorrection.Value = null;
					base.SetTimeStamps(new PointTag[] { AmbientTemperature, ProductTemperature }, TankShellCorrection);
				}
				return false;
			}


			if (!StrapTable.StrapInRange)
			{
				if (TankShellCorrection.Status != StatusCodes.Bad ||
					TankShellCorrection.Value != null)
				{
					TankShellCorrection.Status = StatusCodes.Bad;
					TankShellCorrection.Value = null;
					TankShellCorrection.ServerTimeStamp = DateTimeOffset.UtcNow;
					TankShellCorrection.SourceTimeStamp = DateTimeOffset.UtcNow;
				}
				return false;
			}

			double shellTemperatureDegrees = 0.0;
			double StrapTableTankShellReferenceTemperatureDegrees = 0.0;
			double productTempDegress = 0.0;

			var point = this.GetPoint();

			EngineeringUnits.Convert((double)ProductTemperature.Value, ProductTemperature.Units, ref productTempDegress, point.TemperatureUnit, 60.0);
			EngineeringUnits.Convert(StrapTable.TankShellReferenceTemperature.Value, ProductTemperature.Units, ref StrapTableTankShellReferenceTemperatureDegrees, point.TemperatureUnit, 60.0);

			if (QuantitySettings.VolumeCalculationType == VolumeCalculationType.API2012Calculations)
			{
				if (this.Vessel.TankShellInsulated)
				{
					// if insulated just set ambient to liquid temp
					shellTemperatureDegrees = Math.Round(productTempDegress, 0, MidpointRounding.AwayFromZero);
				}
				else
				{
					double ambientTempDegrees = 0.0;
					EngineeringUnits.Convert((double)AmbientTemperature.Value, AmbientTemperature.Units, ref ambientTempDegrees, point.TemperatureUnit, 60.0);
					// TSh = [(7 * tempprod) + Ambient Temp] / 8
					shellTemperatureDegrees = Math.Round(((7 * productTempDegress) + ambientTempDegrees) / 8, 0, MidpointRounding.AwayFromZero);
				}

				// calculate the delta temp
				// delta temp = Tsh(dTs) - TshRef(StrapTemp)
				var deltaTemperature = shellTemperatureDegrees - StrapTableTankShellReferenceTemperatureDegrees;

				// do the actual calculation
				// CTsh = 1 + (( 2 * Linear Coef) * Delta Temp) + ((Linear Coef ^ 2) * (Delta Temp ^ 2))
				double tankShellCorrection = Math.Round(1 + ((2 * Vessel.TankExpansionCoefficient.Value) * (deltaTemperature)) + ((Vessel.TankExpansionCoefficient.Value * Vessel.TankExpansionCoefficient.Value) * (deltaTemperature * deltaTemperature)), 5, MidpointRounding.AwayFromZero);

				long newStatus = StatusCodes.Good;

				// if any of the input variables are over/under ranged set status to warning
				if (IsStatusUncertain(ProductTemperature) ||
					!this.Vessel.TankShellInsulated &&
				  IsStatusUncertain(AmbientTemperature))
				{
					newStatus = StatusCodes.Uncertain;
				}

				if (TankShellCorrection.Value == null ||
				(double)TankShellCorrection.Value != tankShellCorrection
				|| IsStatusChange(TankShellCorrection.Status, newStatus))
				{
					TankShellCorrection.Value = tankShellCorrection;
					TankShellCorrection.Status = newStatus;
					CheckForAndSetOverUnderRange(TankShellCorrection);

					// set the timestamp based on the input variable
					if (AmbientTemperature.SourceTimeStamp > ProductTemperature.SourceTimeStamp
					&& !this.Vessel.TankShellInsulated)
					{
						TankShellCorrection.SourceTimeStamp = AmbientTemperature.SourceTimeStamp;
						TankShellCorrection.ServerTimeStamp = AmbientTemperature.ServerTimeStamp;
					}
					else
					{
						TankShellCorrection.SourceTimeStamp = ProductTemperature.SourceTimeStamp;
						TankShellCorrection.ServerTimeStamp = ProductTemperature.ServerTimeStamp;
					}
				}
			}

			else
			{
				// Temperature Offset for Farenheit correction
				double temeratureOffset = 60;

				if (VcfSettings.CorrectionMethodType == Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_C
				|| VcfSettings.CorrectionMethodType == Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_C_1980
				|| VcfSettings.CorrectionMethodType == Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_LPG_C
				|| VcfSettings.CorrectionMethodType == Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_LPG_C_1980)
				{
					// Temperature Offset for Celsius 15 degrees converted to Farenheit
					temeratureOffset = 15;
				}

				if (this.Vessel.TankShellInsulated)
				{
					// if insulated just set ambient to liquid temp (API 12.1.1 Section 9.2.3.1)
					shellTemperatureDegrees = productTempDegress - temeratureOffset;
				}
				else
				{
					double ambientTempDegrees = 0.0;
					EngineeringUnits.Convert((double)AmbientTemperature.Value, AmbientTemperature.Units, ref ambientTempDegrees, point.TemperatureUnit, 60.0);

					//if non-insulated API 12.1.1 Section 9.2.3.2)
					shellTemperatureDegrees = (((7 * productTempDegress) + ambientTempDegrees) / 8) - temeratureOffset;
				}

				double tankShellCorrection = 1 + (Vessel.TankExpansionCoefficient.Value * shellTemperatureDegrees) + (Vessel.TankExpansionCoefficient.Value * (shellTemperatureDegrees * shellTemperatureDegrees));

				long newStatus = StatusCodes.Good;

				// if any of the input variables are over/under ranged set status to warning
				if (IsStatusUncertain(ProductTemperature) ||
					!this.Vessel.TankShellInsulated &&
				  IsStatusUncertain(AmbientTemperature))
				{
					newStatus = StatusCodes.Uncertain;
				}

				if (TankShellCorrection.Value == null ||
				(double)TankShellCorrection.Value != tankShellCorrection
				|| IsStatusChange(TankShellCorrection.Status, newStatus))
				{
					TankShellCorrection.Value = tankShellCorrection;
					TankShellCorrection.Status = newStatus;
					CheckForAndSetOverUnderRange(TankShellCorrection);


					// set the timestamp based on the input variable
					if (AmbientTemperature.SourceTimeStamp > ProductTemperature.SourceTimeStamp
					&& !this.Vessel.TankShellInsulated)
					{
						TankShellCorrection.SourceTimeStamp = AmbientTemperature.SourceTimeStamp;
						TankShellCorrection.ServerTimeStamp = AmbientTemperature.ServerTimeStamp;
					}
					else
					{
						TankShellCorrection.SourceTimeStamp = ProductTemperature.SourceTimeStamp;
						TankShellCorrection.ServerTimeStamp = ProductTemperature.ServerTimeStamp;
					}
				}
			}
			return true;
		}

		public ModuleInputOutputCollection GetInputOutputCollection(string calculationName)
		{
			var properties = new ModuleInputOutputCollection
								{
									new ModuleInputOutput
									{
										ID = "Temperature Ambient",
										Type = typeof(double?),
										ParameterType = ModuleInputOutputType.Input
									},
									new ModuleInputOutput
									{
										ID = "Temperature Product",
										Type = typeof(double?),
										ParameterType = ModuleInputOutputType.Input
									},
									new ModuleInputOutput
									{
										ID = "Tank Shell Correction",
										Type = typeof(double?),
										ParameterType = ModuleInputOutputType.Output
									},
								};
			return properties;
		}
	}
}
