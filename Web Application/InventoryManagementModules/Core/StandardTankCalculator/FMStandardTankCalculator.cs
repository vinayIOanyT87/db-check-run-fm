namespace StandardTankCalculator
{
	using System;
	using System.Linq;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;
	using System.Collections.Generic;
	using Varec.CommonComponents.EngineeringUnitsLibrary;
	using FMPointCommon;
	using StrapTables;
	using VCF;
	using ShellCorrection;
	using FloatingRoofCorrection;
	using Quantities;
	using AvailableAndRemainingVolume;
	using Opc.Ua;


	public class FMStandardTankCalculator : FuelsManagerModule, IFuelsManagerModule
	{
		public FMStrapTable StrapTable { get; set; }

		public FMQuantities Quantities { get; set; }

		public FMVcf VolumeCorrection { get; set; }

		public FMShellCorrection ShellCorrection { get; set; }

		public FMFloatingRoofCorrection FloatingRoofCorrection { get; set; }

		public FMAvailableAndRemainingVolume AvailableAndRemainingVolume { get; set; }

		PointTagCollection pointTagCollection;

		public void PerformCalculations(List<PointTag> pointTags, List<PointTag> pointLogicTags)
		{
				#region commented out code
				var LevelProduct = pointTags.First(x => x.ID == "Level Product");
				var LevelWater = pointTags.First(x => x.ID == "Level Water");
				var LevelSolids = pointTags.First(x => x.ID == "Level Solids");
				var VolumeStrapProduct = pointTags.First(x => x.ID == "Volume Total Observed"); // Volume Strap Product
				var VolumeStrapWater = pointTags.First(x => x.ID == "Volume Water"); // Volume Strap Water
				var VolumeStrapSolids = pointTags.First(x => x.ID == "Volume Solids"); // Volume Strap Solids
				var TemperatureProduct = pointTags.First(x => x.ID == "Temperature Product");
				var TemperatureDensity = pointTags.First(x => x.ID == "Temperature Density");
				var TemperatureVapor = pointTags.First(x => x.ID == "Temperature Vapor");
				var TemperatureAmbient = pointTags.First(x => x.ID == "Temperature Ambient");
				var PressureVapor = pointTags.First(x => x.ID == "Pressure Vapor");
				var PercentBSW = pointTags.First(x => x.ID == "Percent BSW");
				var DensityStandard = pointTags.First(x => x.ID == "Density Product Standard"); // Density Standard
				var DensityObserved = pointTags.First(x => x.ID == "Density Product Observed"); // Density Observed
				var DensityVapor = pointTags.First(x => x.ID == "Density Vapor");
				var TankShellCorrection = pointTags.First(x => x.ID == "Tank Shell Correction");
				var VolumeCorrectionFactor = pointTags.First(x => x.ID == "Volume Correction Factor");
				var VolumeCorrectionFactorUnrounded = pointLogicTags.First(x => x.ID == "Volume Correction Factor Unrounded");
				var VolumeBottom = pointTags.First(x => x.ID == "Volume Bottoms"); // Volume Bottom
				var VolumeGrossObserved = pointTags.First(x => x.ID == "Volume Gross Observed");
				var VolumeGOVAvailable = pointTags.First(x => x.ID == "Volume Gross Observed Available"); // Volume GOV Available
				var VolumeGOVRemaining = pointTags.First(x => x.ID == "Volume Gross Observed Remaining"); // Volume GOV Remaining
				var VolumeNetStandard = pointTags.First(x => x.ID == "Volume Net Standard");
				var VolumeNetStandardUnrounded = pointLogicTags.First(x => x.ID == "Volume Net Standard Unrounded");
				var VolumeNSVAvailable = pointTags.First(x => x.ID == "Volume Net Standard Available"); // Volume NSV Available
				var VolumeNSVRemaining = pointTags.First(x => x.ID == "Volume Net Standard Remaining"); // Volume NSV Remaining
				var VolumeRoofCorrection = pointTags.First(x => x.ID == "Volume Roof Correction");
				var Mass = pointTags.First(x => x.ID == "Mass Liquid"); // Mass
				var MassVapor = pointTags.First(x => x.ID == "Mass Vapor");
				var VolumeCorrectionFactorForTemperature = pointLogicTags.First(x => x.ID == "Volume Correction for Temperature"); // Volume Correction Factor For Temperature
				var VolumeCorrectionFactorForPressure = pointLogicTags.First(x => x.ID == "Volume Correction for Pressure"); // Volume Correction Factor For Pressure
				var VolumeCorrectionFactorForPressureAndTemperature = pointLogicTags.First(x => x.ID == "Volume Correction for Temperature and Pressure"); // Volume Correction Factor For Pressure And Temperature
				var APICorrectionError = pointLogicTags.First(x => x.ID == "API Correction Error");
				var DensityObservedInAir = pointLogicTags.First(x => x.ID == "Density Product in Air"); // Density Observed In Air
				var DensityStandardInAir = pointLogicTags.First(x => x.ID == "Density Product Standard in Air"); // Density Standard In Air
				var CriticalZone = pointLogicTags.First(x => x.ID == "Roof Critical Zone");
				var WeightGrossStandard = pointTags.First(x => x.ID == "Weight Gross Standard");
				var WeightNetStandard = pointTags.First(x => x.ID == "Weight Net Standard");
				var VolumeGrossStandard = pointTags.First(x => x.ID == "Volume Gross Standard");
				var VolumeBSW = pointTags.First(x => x.ID == "Volume BSW");
				var VolumeTotalCalculated = pointTags.First(x => x.ID == "Volume Total Calculated");
				var VolumeVaporNet = pointLogicTags.First(x => x.ID == "Volume Vapor Net");
				var LevelMinOpLimit = pointLogicTags.First(x => x.ID == "Level Product Min Op Limit"); // Level Min Op Limit
				var LevelMaxOpLimit = pointLogicTags.First(x => x.ID == "Level Product Max Op Limit"); // Level Max Op Limit
				var DensityGauge = pointLogicTags.First(x => x.ID == "Density Product Gauge"); // Density Gauge

				pointTagCollection = new PointTagCollection();
				CheckandInitializeVariable(LevelProduct, 0.0);
				CheckandInitializeVariable(LevelWater, 0.0);
				CheckandInitializeVariable(LevelSolids, 0.0);
				CheckandInitializeVariable(VolumeStrapProduct, 0.0);
				CheckandInitializeVariable(VolumeStrapWater, 0.0);
				CheckandInitializeVariable(VolumeStrapSolids, 0.0);
				CheckandInitializeVariable(TemperatureProduct, 0.0);
				CheckandInitializeVariable(TemperatureDensity, 0.0);
				CheckandInitializeVariable(TemperatureVapor, 0.0);
				CheckandInitializeVariable(TemperatureAmbient, 0.0);
				CheckandInitializeVariable(PressureVapor, 0.0);
				CheckandInitializeVariable(PercentBSW, 0.0);
				CheckandInitializeVariable(DensityStandard, 1.0);
				CheckandInitializeVariable(DensityObserved, 1.0);
				CheckandInitializeVariable(DensityVapor, 1.0);
				CheckandInitializeVariable(TankShellCorrection, 1.0);
				CheckandInitializeVariable(VolumeCorrectionFactor, 1.0);
				CheckandInitializeVariable(VolumeCorrectionFactorUnrounded, 1.0);
				CheckandInitializeVariable(VolumeBottom, 0.0);
				CheckandInitializeVariable(VolumeGrossObserved, 0.0);
				CheckandInitializeVariable(VolumeGOVAvailable, 0.0);
				CheckandInitializeVariable(VolumeGOVRemaining, 0.0);
				CheckandInitializeVariable(VolumeNetStandard, 0.0);
				CheckandInitializeVariable(VolumeNetStandardUnrounded, 0.0);
				CheckandInitializeVariable(VolumeNSVAvailable, 0.0);
				CheckandInitializeVariable(VolumeNSVRemaining, 0.0);
				CheckandInitializeVariable(VolumeRoofCorrection, 0.0);
				CheckandInitializeVariable(Mass, 0.0);
				CheckandInitializeVariable(MassVapor, 0.0);
				CheckandInitializeVariable(VolumeCorrectionFactorForTemperature, 1.0);
				CheckandInitializeVariable(VolumeCorrectionFactorForPressure, 1.0);
				CheckandInitializeVariable(VolumeCorrectionFactorForPressureAndTemperature, 1.0);
				CheckandInitializeVariable(DensityObservedInAir, 1.0);
				CheckandInitializeVariable(DensityStandardInAir, 1.0);
				CheckandInitializeVariable(WeightGrossStandard, 0.0);
				CheckandInitializeVariable(WeightNetStandard, 0.0);
				CheckandInitializeVariable(VolumeGrossStandard, 0.0);
				CheckandInitializeVariable(VolumeBSW, 0.0);
				CheckandInitializeVariable(VolumeTotalCalculated, 0.0);
				CheckandInitializeVariable(VolumeVaporNet, 0.0);
				CheckandInitializeVariable(LevelMinOpLimit, 0.0);
				CheckandInitializeVariable(LevelMaxOpLimit, 40.0);

				CheckandInitializeVariable(DensityGauge, 0.0);

				pointTagCollection.Add(LevelProduct);
				pointTagCollection.Add(LevelWater);
				pointTagCollection.Add(LevelSolids);
				pointTagCollection.Add(VolumeStrapProduct);
				pointTagCollection.Add(VolumeStrapWater);
				pointTagCollection.Add(VolumeStrapSolids);
				pointTagCollection.Add(TemperatureProduct);
				pointTagCollection.Add(TemperatureDensity);
				pointTagCollection.Add(TemperatureVapor);
				pointTagCollection.Add(TemperatureAmbient);
				pointTagCollection.Add(PressureVapor);
				pointTagCollection.Add(PercentBSW);
				pointTagCollection.Add(DensityStandard);
				pointTagCollection.Add(DensityObserved);
				pointTagCollection.Add(DensityVapor);
				pointTagCollection.Add(TankShellCorrection);
				pointTagCollection.Add(VolumeCorrectionFactor);
				pointTagCollection.Add(VolumeCorrectionFactorUnrounded);
				pointTagCollection.Add(VolumeBottom);
				pointTagCollection.Add(VolumeGrossObserved);
				pointTagCollection.Add(VolumeGOVAvailable);
				pointTagCollection.Add(VolumeGOVRemaining);
				pointTagCollection.Add(VolumeNetStandard);
				pointTagCollection.Add(VolumeNetStandardUnrounded);
				pointTagCollection.Add(VolumeNSVAvailable);
				pointTagCollection.Add(VolumeNSVRemaining);
				pointTagCollection.Add(VolumeRoofCorrection);
				pointTagCollection.Add(Mass);
				pointTagCollection.Add(MassVapor);
				pointTagCollection.Add(VolumeCorrectionFactorForTemperature);
				pointTagCollection.Add(VolumeCorrectionFactorForPressure);
				pointTagCollection.Add(VolumeCorrectionFactorForPressureAndTemperature);
				pointTagCollection.Add(APICorrectionError);
				pointTagCollection.Add(DensityObservedInAir);
				pointTagCollection.Add(DensityStandardInAir);
				pointTagCollection.Add(CriticalZone);
				pointTagCollection.Add(WeightGrossStandard);
				pointTagCollection.Add(WeightNetStandard);
				pointTagCollection.Add(VolumeGrossStandard);
				pointTagCollection.Add(VolumeBSW);
				pointTagCollection.Add(VolumeTotalCalculated);
				pointTagCollection.Add(VolumeVaporNet);
				pointTagCollection.Add(LevelMinOpLimit);
				pointTagCollection.Add(LevelMaxOpLimit);
				pointTagCollection.Add(DensityGauge);

				foreach (PointTag pointTag in pointTagCollection)
				{
					SetTagStatusToGood(pointTag);
				}


				// we need to determine if this is a single calculation or a iteration based on the timestamp
				CalculateIterationValuesIfRequired(LevelProduct,
																LevelWater,
																LevelSolids,
																VolumeStrapProduct,
																VolumeStrapWater,
																VolumeStrapSolids,
																TemperatureProduct,
																TemperatureDensity,
																TemperatureVapor,
																TemperatureAmbient,
																PressureVapor,
																PercentBSW,
																DensityStandard,
																DensityObserved,
																DensityVapor,
																TankShellCorrection,
																VolumeCorrectionFactor,
																VolumeBottom,
																VolumeGrossObserved,
																VolumeGOVAvailable,
																VolumeGOVRemaining,
																VolumeNetStandard,
																VolumeNSVAvailable,
																VolumeNSVRemaining,
																VolumeRoofCorrection,
																Mass,
																MassVapor,
																VolumeCorrectionFactorForTemperature,
																VolumeCorrectionFactorForPressure,
																VolumeCorrectionFactorForPressureAndTemperature,
																APICorrectionError,
																DensityObservedInAir,
																DensityStandardInAir,
																CriticalZone,
																WeightGrossStandard,
																WeightNetStandard,
																VolumeGrossStandard,
																VolumeBSW,
																VolumeTotalCalculated,
																VolumeVaporNet,
																LevelMinOpLimit,
																LevelMaxOpLimit,
																VolumeCorrection,
																ShellCorrection,
																FloatingRoofCorrection,
																//Quantities,
																AvailableAndRemainingVolume);

				// always do a complete calculation at the end
				PerformStandardTankCalculatorCalculations(LevelProduct,
																LevelWater,
																LevelSolids,
																VolumeStrapProduct,
																VolumeStrapWater,
																VolumeStrapSolids,
																TemperatureProduct,
																TemperatureDensity,
																TemperatureVapor,
																TemperatureAmbient,
																PressureVapor,
																PercentBSW,
																DensityStandard,
																DensityObserved,
																DensityVapor,
																TankShellCorrection,
																VolumeCorrectionFactor,
																VolumeCorrectionFactorUnrounded,
																VolumeBottom,
																VolumeGrossObserved,
																VolumeGOVAvailable,
																VolumeGOVRemaining,
																VolumeNetStandard,
																VolumeNetStandardUnrounded,
																VolumeNSVAvailable,
																VolumeNSVRemaining,
																VolumeRoofCorrection,
																Mass,
																MassVapor,
																VolumeCorrectionFactorForTemperature,
																VolumeCorrectionFactorForPressure,
																VolumeCorrectionFactorForPressureAndTemperature,
																APICorrectionError,
																DensityObservedInAir,
																DensityStandardInAir,
																CriticalZone,
																WeightGrossStandard,
																WeightNetStandard,
																VolumeGrossStandard,
																VolumeBSW,
																VolumeTotalCalculated,
																VolumeVaporNet,
																LevelMinOpLimit,
																LevelMaxOpLimit,
																DensityGauge,
																VolumeCorrection,
																ShellCorrection,
																FloatingRoofCorrection,
																//Quantities,
																AvailableAndRemainingVolume);

				// set tag status to good after the calc are run
				//foreach (PointTag pointTag in pointTagCollection)
				//{
				//	SetTagStatusToGood(pointTag);
				//}
				#endregion

		}

		public void TankCalculatorCalculateBatch(PointCalculatorData pointCalculatorData, List<PointTag> pointLogicTags)
		{
			var batchTemperatureProduct = pointCalculatorData.DiffTags.First(x => x.ID == "Temperature Product");
			var batchTemperatureDensity = pointCalculatorData.DiffTags.First(x => x.ID == "Temperature Density");
			var batchTemperatureVapor = pointCalculatorData.DiffTags.First(x => x.ID == "Temperature Vapor");
			var batchDensityProductStandard = pointCalculatorData.DiffTags.First(x => x.ID == "Density Product Standard");
			var batchDensityProducObserved = pointCalculatorData.DiffTags.First(x => x.ID == "Density Product Observed");
			var batchPressureVapor = pointCalculatorData.DiffTags.First(x => x.ID == "Pressure Vapor");
			var batchVolumeCorrectionForTemperature = pointLogicTags.First(x => x.ID == "Volume Correction for Temperature");
			var batchVolumeCorrectionForPressure = pointLogicTags.First(x => x.ID == "Volume Correction for Pressure");
			var batchVolumeCorrectionForTemperatureAndPressure = pointLogicTags.First(x => x.ID == "Volume Correction for Temperature and Pressure");
			var batchVolumeCorrectionFactor = pointCalculatorData.DiffTags.First(x => x.ID == "Volume Correction Factor");
			var batchVolumeCorrectionFactorUnrounded = pointLogicTags.First(x => x.ID == "Volume Correction Factor Unrounded");
			var batchAPICorrectionError = pointLogicTags.First(x => x.ID == "API Correction Error");
			var batchDensityProductInAir = pointLogicTags.First(x => x.ID == "Density Product in Air");
			var batchDensityProductStandardInAir = pointLogicTags.First(x => x.ID == "Density Product Standard in Air");
			var batchDensityProductGauge = pointLogicTags.First(x => x.ID == "Density Product Gauge");

			var batchVolumeRoofCorrection = pointCalculatorData.DiffTags.First(x => x.ID == "Volume Roof Correction");
			batchVolumeRoofCorrection.Value = 0.0;
			batchVolumeRoofCorrection.Status = StatusCodes.Good;

			var batchTankShellCorrection = pointCalculatorData.DiffTags.First(x => x.ID == "Tank Shell Correction");
			batchTankShellCorrection.Value = 1.0;
			batchTankShellCorrection.Status = StatusCodes.Good;

			var startTmperatureProduct = pointCalculatorData.StartTags.First(x => x.ID == "Temperature Product");
			var endTemperatureProduct = pointCalculatorData.EndTags.First(x => x.ID == "Temperature Product");

			var startDensityProductStandard = pointCalculatorData.StartTags.First(x => x.ID == "Density Product Standard");
			var endDensityProductStandard = pointCalculatorData.EndTags.First(x => x.ID == "Density Product Standard");

			// Calculate Batch Volume Water
			var startVolumeWater = pointCalculatorData.StartTags.First(x => x.ID == "Volume Water");
			var endVolumeWater = pointCalculatorData.EndTags.First(x => x.ID == "Volume Water");
			var batchVolumeWater = pointCalculatorData.DiffTags.First(x => x.ID == "Volume Water");

			if (endVolumeWater.Value is double
			&& startVolumeWater.Value is double)
			{
				batchVolumeWater.Value = (double)endVolumeWater.Value - (double)startVolumeWater.Value;
				batchVolumeWater.Status = StatusCodes.Good;
			}
			else
			{
				batchVolumeWater.Value = null;
				batchVolumeWater.Status = StatusCodes.Bad;
			}

			// Calculate Batch Volume Solids
			var startVolumeSolids = pointCalculatorData.StartTags.First(x => x.ID == "Volume Solids");
			var endVolumeSolids = pointCalculatorData.EndTags.First(x => x.ID == "Volume Solids");
			var batchVolumeSolids = pointCalculatorData.DiffTags.First(x => x.ID == "Volume Solids");

			if (endVolumeSolids.Value is double
			&& startVolumeSolids.Value is double)
			{
				batchVolumeSolids.Value = (double)endVolumeSolids.Value - (double)startVolumeSolids.Value;
				batchVolumeSolids.Status = StatusCodes.Good;
			}
			else
			{
				batchVolumeSolids.Value = null;
				batchVolumeSolids.Status = StatusCodes.Bad;
			}

			// Calculate Batch Volume Bottoms
			var startVolumeBottoms = pointCalculatorData.StartTags.First(x => x.ID == "Volume Bottoms");
			var endVolumeBottoms = pointCalculatorData.EndTags.First(x => x.ID == "Volume Bottoms");
			var batchVolumeBottoms = pointCalculatorData.DiffTags.First(x => x.ID == "Volume Bottoms");

			if (endVolumeBottoms.Value is double
			&& startVolumeBottoms.Value is double)
			{
				batchVolumeBottoms.Value = (double)endVolumeBottoms.Value - (double)startVolumeBottoms.Value;
				batchVolumeBottoms.Status = StatusCodes.Good;
			}
			else
			{
				batchVolumeBottoms.Value = null;
				batchVolumeBottoms.Status = StatusCodes.Bad;
			}


			// Calculate the Batch Mass
			var startVolumeGrossObserved = pointCalculatorData.StartTags.First(x => x.ID == "Volume Gross Observed");
			var endVolumeGrossObserved = pointCalculatorData.EndTags.First(x => x.ID == "Volume Gross Observed");
			var startVolumeNetStandard = pointCalculatorData.StartTags.First(x => x.ID == "Volume Net Standard");
			var endVolumeNetStandard = pointCalculatorData.EndTags.First(x => x.ID == "Volume Net Standard");
			var startMassLiquid = pointCalculatorData.StartTags.First(x => x.ID == "Mass Liquid");
			var endMassLiquid = pointCalculatorData.EndTags.First(x => x.ID == "Mass Liquid");
			var batcMassLiquid = pointCalculatorData.DiffTags.First(x => x.ID == "Mass Liquid");

            // Not Suitable for WightInAir or Molar methods, possibly add Density Standard in Air to calculator?
            this.Quantities.CalculateLiquidMass(startVolumeGrossObserved, startVolumeNetStandard, startDensityProductStandard, null, startMassLiquid, false);
            this.Quantities.CalculateLiquidMass(endVolumeGrossObserved, endVolumeNetStandard, endDensityProductStandard, null, endMassLiquid, false);


			if (endMassLiquid.Value is double
			&& startMassLiquid.Value is double)
			{
				batcMassLiquid.Value = (double)endMassLiquid.Value - (double)startMassLiquid.Value;
				batcMassLiquid.Status = StatusCodes.Good;
			}
			else
			{
				batcMassLiquid.Value = null;
				batcMassLiquid.Status = StatusCodes.Bad;
			}

			// Calculate Batch Net Standard
			var batchVolumeNetStandard = pointCalculatorData.DiffTags.First(x => x.ID == "Volume Net Standard");

			if (endVolumeNetStandard.Value is double
			&& startVolumeNetStandard.Value is double)
			{
				batchVolumeNetStandard.Value = (double)endVolumeNetStandard.Value - (double)startVolumeNetStandard.Value;
				batchVolumeNetStandard.Status = StatusCodes.Good;
			}
			else
			{
				batchVolumeNetStandard.Value = null;
				batchVolumeNetStandard.Status = StatusCodes.Bad;
			}


			// Calculate Batch Standard Density
			if (startDensityProductStandard.Value is double
			&& endDensityProductStandard.Value is double)
			{
				var batchMassLiquid = pointCalculatorData.DiffTags.First(x => x.ID == "Mass Liquid");

				double endStddensitykgm3 = 0.0;
				double startVolumeNetStandardm3 = 0.0;
				double batchVolumeNetStandardm3 = 0.0;
				double startMassLiquidKg = 0.0;

				double stdTemperatureinDegreesC = this.Quantities.GetStandardTemperatureinCforSelectedTable();

				EngineeringUnits.Convert((double)endDensityProductStandard.Value, endDensityProductStandard.Units, ref endStddensitykgm3, EngineeringUnit.FmdKgM3, stdTemperatureinDegreesC);
				EngineeringUnits.Convert((double)startVolumeNetStandard.Value, startVolumeNetStandard.Units, ref startVolumeNetStandardm3, EngineeringUnit.FmvMeter3, 0.0);
				EngineeringUnits.Convert((double)batchVolumeNetStandard.Value, batchVolumeNetStandard.Units, ref batchVolumeNetStandardm3, EngineeringUnit.FmvMeter3, 0.0);
				EngineeringUnits.Convert((double)startMassLiquid.Value, batchMassLiquid.Units, ref startMassLiquidKg, EngineeringUnit.FmmKg, 0.0);

				if (batchMassLiquid.Value is double
				&& (double)batchMassLiquid.Value != 0.0)
				{
					double batchStddensitykmg3 = ((endStddensitykgm3 * (startVolumeNetStandardm3 + batchVolumeNetStandardm3) - startMassLiquidKg) / batchVolumeNetStandardm3);
					double batchStddensity = 0.0;
					EngineeringUnits.Convert(batchStddensitykmg3, EngineeringUnit.FmdKgM3, ref batchStddensity, batchDensityProductStandard.Units, stdTemperatureinDegreesC);
					batchStddensity = Math.Round(batchStddensity, batchDensityProductStandard.DecimalPlaces, MidpointRounding.AwayFromZero);
					batchDensityProductStandard.Value = batchStddensity;
					batchDensityProductStandard.Status = StatusCodes.Good;

					// Batch Density Product Standard must be manual
					batchDensityProductStandard.InputOutputType = PointTemplateTag.PointTagInputOutputType.Manual;
				}
				else
				{
					batchDensityProductStandard.Value = endDensityProductStandard.Value;
					batchDensityProductStandard.Status = StatusCodes.Good;
				}
			}
			else
			{
				batchDensityProductStandard.Value = null;
				batchDensityProductStandard.Status = StatusCodes.Bad;
			}

			// Calcilate Batch Temperature Product
			var batchVolumeGrossObserved = pointCalculatorData.DiffTags.First(x => x.ID == "Volume Gross Observed");

			if (startTmperatureProduct.Value is double
			&& endTemperatureProduct.Value is double)
			{
				var batchMassLiquid = pointCalculatorData.DiffTags.First(x => x.ID == "Mass Liquid");

				// Not Suitable for WightInAir or Molar methods, possibly add Density Standard in Air
				this.Quantities.CalculateLiquidMass(batchVolumeGrossObserved, batchVolumeNetStandard, batchDensityProductStandard, batchDensityProductStandardInAir, batchMassLiquid, false);
				if (batchMassLiquid.Value is double
				&& (double)batchMassLiquid.Value != 0.0)
				{
					batchTemperatureProduct.Value = ((double)endTemperatureProduct.Value * ((double)startMassLiquid.Value + (double)batchMassLiquid.Value) - ((double)startTmperatureProduct.Value * (double)startMassLiquid.Value)) / (double)batchMassLiquid.Value;
					batchTemperatureProduct.Status = StatusCodes.Good;
				}
				else
				{
					batchTemperatureProduct.Value = endTemperatureProduct.Value;
					batchTemperatureProduct.Status = StatusCodes.Good;
				}
			}
			else
			{
				batchTemperatureProduct.Value = null;
				batchTemperatureProduct.Status = StatusCodes.Bad;
			}


			// Batch Density Product Observed must be calculated
			batchDensityProducObserved.InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated;

			this.VolumeCorrection.VcfCalculation(batchTemperatureProduct,
									batchTemperatureDensity,
									batchTemperatureVapor,
									batchDensityProductStandard,
									batchDensityProducObserved,
									batchPressureVapor,
									batchVolumeCorrectionForTemperature,
									batchVolumeCorrectionForPressure,
									batchVolumeCorrectionForTemperatureAndPressure,
									batchVolumeCorrectionFactor,
									batchVolumeCorrectionFactorUnrounded,
									batchAPICorrectionError,
									batchDensityProductInAir,
									batchDensityProductStandardInAir,
									batchDensityProductGauge);

			// Calculate the Batch Volume BSW
			var startVolumeBSW = pointCalculatorData.StartTags.First(x => x.ID == "Volume BSW");
			var endVolumeBSW = pointCalculatorData.EndTags.First(x => x.ID == "Volume BSW");
			var batchVolumeBSW = pointCalculatorData.DiffTags.First(x => x.ID == "Volume BSW");


			if (startVolumeBSW.Value is double
			&& endVolumeBSW.Value is double)
			{
				batchVolumeBSW.Value = (double)endVolumeBSW.Value - (double)startVolumeBSW.Value;
				batchVolumeBSW.Status = StatusCodes.Good;
			}
			else
			{
				batchVolumeBSW.Value = null;
				batchVolumeBSW.Status = StatusCodes.Good;
			}

			// Calculate Batch Percent BSW
			var batchPercentBSW = pointCalculatorData.DiffTags.First(x => x.ID == "Percent BSW");
			batchPercentBSW.InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated;
			batchPercentBSW.Status = StatusCodes.Good;
			this.Quantities.CalculatePercentBSWFromNetBottomBSWVolumeVCF(batchVolumeNetStandard, batchVolumeBottoms, batchVolumeBSW, batchVolumeCorrectionFactor, batchPercentBSW);

			// Calculate Batch Volumme Gross Observed
			this.Quantities.CalculateGrossObservedVolumeFromNetStandardVolume(batchVolumeNetStandard, batchPercentBSW, batchVolumeCorrectionFactor, batchVolumeGrossObserved);

			// Calculate Batch Volpume Gross Standard
			var batchVolumeGrossStandard = pointCalculatorData.DiffTags.First(x => x.ID == "Volume Gross Standard");
			this.Quantities.CalculateGrossStandardVolume(batchVolumeGrossObserved, batchVolumeCorrectionFactor, batchVolumeGrossStandard);

			// Calculate Batch Volumme Total Observed
			var batchVolumeTotalObserved = pointCalculatorData.DiffTags.First(x => x.ID == "Volume Total Observed");

			this.Quantities.CalculateStrapVolumeFromGrossObservedVolume(batchVolumeRoofCorrection, batchVolumeGrossObserved, batchVolumeBottoms, batchTankShellCorrection, batchVolumeTotalObserved);

		}

		public void TankCalculatorCalculateEnd(PointCalculatorData pointCalculatorData, List<PointTag> pointLogicTags)
		{
			var batchTemperatureProduct = pointCalculatorData.DiffTags.First(x => x.ID == "Temperature Product");
			var batchTemperatureDensity = pointCalculatorData.DiffTags.First(x => x.ID == "Temperature Density");
			var batchTemperatureVapor = pointCalculatorData.DiffTags.First(x => x.ID == "Temperature Vapor");
			var batchDensityProductStandard = pointCalculatorData.DiffTags.First(x => x.ID == "Density Product Standard");
			var batchDensityProducObserved = pointCalculatorData.DiffTags.First(x => x.ID == "Density Product Observed");
			var batchPressureVapor = pointCalculatorData.DiffTags.First(x => x.ID == "Pressure Vapor");
			var batchVolumeCorrectionForTemperature = pointLogicTags.First(x => x.ID == "Volume Correction for Temperature");
			var batchVolumeCorrectionForPressure = pointLogicTags.First(x => x.ID == "Volume Correction for Pressure");
			var batchVolumeCorrectionForTemperatureAndPressure = pointLogicTags.First(x => x.ID == "Volume Correction for Temperature and Pressure");
			var batchVolumeCorrectionFactor = pointCalculatorData.DiffTags.First(x => x.ID == "Volume Correction Factor");
			var batchVolumeCorrectionFactorUnrounded = pointLogicTags.First(x => x.ID == "Volume Correction Factor Unrounded");
			var batchAPICorrectionError = pointLogicTags.First(x => x.ID == "API Correction Error");
			var batchDensityProductInAir = pointLogicTags.First(x => x.ID == "Density Product in Air");
			var batchDensityProductStandardInAir = pointLogicTags.First(x => x.ID == "Density Product Standard in Air");
			var batchDensityProductGauge = pointLogicTags.First(x => x.ID == "Density Product Gauge");


			this.VolumeCorrection.VcfCalculation(batchTemperatureProduct,
										batchTemperatureDensity,
										batchTemperatureVapor,
										batchDensityProductStandard,
										batchDensityProducObserved,
										batchPressureVapor,
										batchVolumeCorrectionForTemperature,
										batchVolumeCorrectionForPressure,
										batchVolumeCorrectionForTemperatureAndPressure,
										batchVolumeCorrectionFactor,
										batchVolumeCorrectionFactorUnrounded,
										batchAPICorrectionError,
										batchDensityProductInAir,
										batchDensityProductStandardInAir,
										batchDensityProductGauge);


			var batchVolumeRoofCorrection = pointCalculatorData.DiffTags.First(x => x.ID == "Volume Roof Correction");
			batchVolumeRoofCorrection.Value = 0.0;
			batchVolumeRoofCorrection.Status = StatusCodes.Good;

			var batchTankShellCorrection = pointCalculatorData.DiffTags.First(x => x.ID == "Tank Shell Correction");
			batchTankShellCorrection.Value = 1.0;
			batchTankShellCorrection.Status = StatusCodes.Good;

			var startTmperatureProduct = pointCalculatorData.StartTags.First(x => x.ID == "Temperature Product");
			var endTemperatureProduct = pointCalculatorData.EndTags.First(x => x.ID == "Temperature Product");

			var startDensityProductStandard = pointCalculatorData.StartTags.First(x => x.ID == "Density Product Standard");
			var endDensityProductStandard = pointCalculatorData.EndTags.First(x => x.ID == "Density Product Standard");

			// Calculate End Volume Water
			var startVolumeWater = pointCalculatorData.StartTags.First(x => x.ID == "Volume Water");
			var endVolumeWater = pointCalculatorData.EndTags.First(x => x.ID == "Volume Water");
			var batchVolumeWater = pointCalculatorData.DiffTags.First(x => x.ID == "Volume Water");

			if (batchVolumeWater.Value is double
			&& startVolumeWater.Value is double)
			{
				endVolumeWater.Value = (double)startVolumeWater.Value - (double)batchVolumeWater.Value;
				endVolumeWater.Status = StatusCodes.Good;
			}
			else
			{
				endVolumeWater.Value = null;
				endVolumeWater.Status = StatusCodes.Bad;
			}

			// Calculate End Volume Solids
			var startVolumeSolids = pointCalculatorData.StartTags.First(x => x.ID == "Volume Solids");
			var endVolumeSolids = pointCalculatorData.EndTags.First(x => x.ID == "Volume Solids");
			var batchVolumeSolids = pointCalculatorData.DiffTags.First(x => x.ID == "Volume Solids");

			if (batchVolumeSolids.Value is double
			&& startVolumeSolids.Value is double)
			{
				endVolumeSolids.Value = (double)startVolumeSolids.Value - (double)batchVolumeSolids.Value;
				endVolumeSolids.Status = StatusCodes.Good;
			}
			else
			{
				endVolumeSolids.Value = null;
				endVolumeSolids.Status = StatusCodes.Bad;
			}

			// Calculate End Volume Bottomss
			var startVolumeBottoms = pointCalculatorData.StartTags.First(x => x.ID == "Volume Bottoms");
			var endVolumeBottoms = pointCalculatorData.EndTags.First(x => x.ID == "Volume Bottoms");
			var batchVolumeBottoms = pointCalculatorData.DiffTags.First(x => x.ID == "Volume Bottoms");

			if (batchVolumeBottoms.Value is double
			&& startVolumeBottoms.Value is double)
			{
				endVolumeBottoms.Value = (double)startVolumeBottoms.Value - (double)batchVolumeBottoms.Value;
				endVolumeBottoms.Status = StatusCodes.Good;
			}
			else
			{
				endVolumeBottoms.Value = null;
				endVolumeBottoms.Status = StatusCodes.Bad;
			}

			var batchVolumeGrossObserved = pointCalculatorData.DiffTags.First(x => x.ID == "Volume Gross Observed");
			var batchPercentBSW = pointCalculatorData.DiffTags.First(x => x.ID == "Percent BSW");
			var batchVolumeNetStandard = pointCalculatorData.DiffTags.First(x => x.ID == "Volume Net Standard");
			var batchVolumeTotalObserved = pointCalculatorData.DiffTags.First(x => x.ID == "Volume Total Observed");

			if (pointCalculatorData.ChangedPointTagId == "Volume Net Standard"
			|| pointCalculatorData.BatchModeKey == BatchModeKey.BatchNSV)
			{
				this.Quantities.CalculateGrossObservedVolumeFromNetStandardVolume(batchVolumeNetStandard, batchPercentBSW, batchVolumeCorrectionFactor, batchVolumeGrossObserved);
				this.Quantities.CalculateStrapVolumeFromGrossObservedVolume(batchVolumeRoofCorrection, batchVolumeGrossObserved, batchVolumeBottoms, batchTankShellCorrection, batchVolumeTotalObserved);
			}

			else if (pointCalculatorData.ChangedPointTagId == "Volume Gross Observed"
			|| pointCalculatorData.BatchModeKey == BatchModeKey.BatchGOV)
			{
				this.Quantities.CalculateNetStandardVolume(batchVolumeGrossObserved, batchPercentBSW, batchVolumeCorrectionFactor, batchVolumeNetStandard);
				this.Quantities.CalculateStrapVolumeFromGrossObservedVolume(batchVolumeRoofCorrection, batchVolumeGrossObserved, batchVolumeBottoms, batchTankShellCorrection, batchVolumeTotalObserved);
			}

			else if (pointCalculatorData.ChangedPointTagId == "Volume Total Observed"
			|| pointCalculatorData.BatchModeKey == BatchModeKey.BatchTOV)
			{
				this.Quantities.CalculateGrossObserverdVolume(batchVolumeRoofCorrection, batchVolumeTotalObserved, batchVolumeBottoms, batchTankShellCorrection, batchVolumeGrossObserved, true);
				this.Quantities.CalculateNetStandardVolume(batchVolumeGrossObserved, batchPercentBSW, batchVolumeCorrectionFactor, batchVolumeNetStandard);
			}

			// Calculate Batch Volume Gross Net
			var batchVolumeGrossStandard = pointCalculatorData.DiffTags.First(x => x.ID == "Volume Gross Standard");
			this.Quantities.CalculateNetStandardVolume(batchVolumeGrossObserved, batchPercentBSW, batchVolumeCorrectionFactor, batchVolumeGrossStandard);

			// Calculate Batch Volume BSW
			var batchVolumeBSW = pointCalculatorData.DiffTags.First(x => x.ID == "Volume BSW");
			this.Quantities.CalculateBSWVolume(batchVolumeGrossObserved, batchVolumeBottoms, batchVolumeGrossStandard, batchPercentBSW, batchVolumeBSW);


			// Calculate End Net Standard
			var startVolumeNetStandard = pointCalculatorData.StartTags.First(x => x.ID == "Volume Net Standard");
			var endVolumeNetStandard = pointCalculatorData.EndTags.First(x => x.ID == "Volume Net Standard");
			if (batchVolumeNetStandard.Value is double
			&& startVolumeNetStandard.Value is double)
			{
				endVolumeNetStandard.Value = (double)startVolumeNetStandard.Value + (double)batchVolumeNetStandard.Value;
				endVolumeNetStandard.Status = StatusCodes.Good;
			}
			else
			{
				endVolumeNetStandard.Value = null;
				endVolumeNetStandard.Status = StatusCodes.Bad;
			}

			// Calculate the End Mass
			var startMassLiquid = pointCalculatorData.StartTags.First(x => x.ID == "Mass Liquid");
			var endMassLiquid = pointCalculatorData.EndTags.First(x => x.ID == "Mass Liquid");
			var batchMassLiquid = pointCalculatorData.DiffTags.First(x => x.ID == "Mass Liquid");
			this.Quantities.CalculateLiquidMass(batchVolumeGrossObserved, batchVolumeNetStandard, batchDensityProductStandard, batchDensityProductStandardInAir, batchMassLiquid, false);
			if (batchMassLiquid.Value is double
			&& startMassLiquid.Value is double)
			{
				endMassLiquid.Value = (double)startMassLiquid.Value + (double)batchMassLiquid.Value;
				endMassLiquid.Status = StatusCodes.Good;
			}
			else
			{
				endMassLiquid.Value = null;
				endMassLiquid.Status = StatusCodes.Bad;
			}

			// Calculate End Standard Density
			if (startVolumeNetStandard.Value is double
			&& startDensityProductStandard.Value is double
			&& batchDensityProductStandard.Value is double)
			{
				double startVolumeNetStandardm3 = 0.0;
				double batchVolumeNetStandardm3 = 0.0;
				double startMassLiquidkg = 0.0;
				double batchMassLiquidkg = 0.0;

				double stdTemperatureinDegreesC = this.Quantities.GetStandardTemperatureinCforSelectedTable();

				EngineeringUnits.Convert((double)batchMassLiquid.Value, batchMassLiquid.Units, ref batchMassLiquidkg, EngineeringUnit.FmmKg, stdTemperatureinDegreesC);
				EngineeringUnits.Convert((double)startMassLiquid.Value, startMassLiquid.Units, ref startMassLiquidkg, EngineeringUnit.FmmKg, stdTemperatureinDegreesC);
				EngineeringUnits.Convert((double)batchVolumeNetStandard.Value, batchVolumeNetStandard.Units, ref batchVolumeNetStandardm3, EngineeringUnit.FmvMeter3, 0.0);
				EngineeringUnits.Convert((double)startVolumeNetStandard.Value, startVolumeNetStandard.Units, ref startVolumeNetStandardm3, EngineeringUnit.FmvMeter3, 0.0);

				if (batchMassLiquid.Value is double
				&& (double)batchMassLiquid.Value != 0.0)
				{
					double endStddensitykmg3 = (startMassLiquidkg + batchMassLiquidkg) / (startVolumeNetStandardm3 + batchVolumeNetStandardm3);
					double endStddensity = 0.0;
					EngineeringUnits.Convert(endStddensitykmg3, EngineeringUnit.FmdKgM3, ref endStddensity, endDensityProductStandard.Units, stdTemperatureinDegreesC);
					endStddensity = Math.Round(endStddensity, endDensityProductStandard.DecimalPlaces, MidpointRounding.AwayFromZero);
					endDensityProductStandard.Value = endStddensity;
					endDensityProductStandard.Status = StatusCodes.Good;

					// endDensityProductStandard is always Manual for Batch Mode
					endDensityProductStandard.InputOutputType = PointTemplateTag.PointTagInputOutputType.Manual;

				}
				else
				{
					endDensityProductStandard.Value = null;
					endDensityProductStandard.Status = StatusCodes.Bad;
				}
			}
			else
			{
				endDensityProductStandard.Value = null;
				endDensityProductStandard.Status = StatusCodes.Bad;
			}

			// Calcilate End Temperature Product
			if (startTmperatureProduct.Value is double
			&& batchTemperatureProduct.Value is double)
			{
				if (startMassLiquid.Value is double
				&& batchMassLiquid.Value is double
				&& ((double)startMassLiquid.Value + (double)batchMassLiquid.Value) != 0.0)
				{
					endTemperatureProduct.Value = ((double)startTmperatureProduct.Value * (double)startMassLiquid.Value + (double)batchTemperatureProduct.Value * (double)batchMassLiquid.Value) / ((double)startMassLiquid.Value + (double)batchMassLiquid.Value);
					endTemperatureProduct.Status = StatusCodes.Good;
				}
				else
				{
					endTemperatureProduct.Value = null;
					endTemperatureProduct.Status = StatusCodes.Bad;
				}
			}
			else
			{
				endTemperatureProduct.Value = null;
				endTemperatureProduct.Status = StatusCodes.Bad;
			}


			// Calculate the End VCF
			var endTemperatureDensity = pointCalculatorData.EndTags.First(x => x.ID == "Temperature Density");
			var endTemperatureVapor = pointCalculatorData.EndTags.First(x => x.ID == "Temperature Vapor");
			var endDensityProducObserved = pointCalculatorData.EndTags.First(x => x.ID == "Density Product Observed");
			var endPressureVapor = pointCalculatorData.EndTags.First(x => x.ID == "Pressure Vapor");
			var endVolumeCorrectionForTemperature = pointLogicTags.First(x => x.ID == "Volume Correction for Temperature");
			var endVolumeCorrectionForPressure = pointLogicTags.First(x => x.ID == "Volume Correction for Pressure");
			var endVolumeCorrectionForTemperatureAndPressure = pointLogicTags.First(x => x.ID == "Volume Correction for Temperature and Pressure");
			var endVolumeCorrectionFactor = pointCalculatorData.EndTags.First(x => x.ID == "Volume Correction Factor");
			var endVolumeCorrectionFactorUnrounded = pointLogicTags.First(x => x.ID == "Volume Correction Factor Unrounded");
			var endAPICorrectionError = pointLogicTags.First(x => x.ID == "API Correction Error");
			var endDensityProductInAir = pointLogicTags.First(x => x.ID == "Density Product in Air");
			var endDensityProductStandardInAir = pointLogicTags.First(x => x.ID == "Density Product Standard in Air");
			var endDensityProductGauge = pointLogicTags.First(x => x.ID == "Density Product Gauge");

			// End Density Product Observed is Calculated regardless in Batch Mode
			endDensityProducObserved.InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated;

			this.VolumeCorrection.VcfCalculation(endTemperatureProduct,
										endTemperatureDensity,
										endTemperatureVapor,
										endDensityProductStandard,
										endDensityProducObserved,
										endPressureVapor,
										endVolumeCorrectionForTemperature,
										endVolumeCorrectionForPressure,
										endVolumeCorrectionForTemperatureAndPressure,
										endVolumeCorrectionFactor,
										endVolumeCorrectionFactorUnrounded,
										endAPICorrectionError,
										endDensityProductInAir,
										endDensityProductStandardInAir,
										endDensityProductGauge);

			// Calculate the End Volume BSW
			var startVolumeBSW = pointCalculatorData.StartTags.First(x => x.ID == "Volume BSW");
			var endVolumeBSW = pointCalculatorData.EndTags.First(x => x.ID == "Volume BSW");
			if (startVolumeBSW.Value is double
			&& batchVolumeBSW.Value is double)
			{
				endVolumeBSW.Value = (double)startVolumeBSW.Value + (double)batchVolumeBSW.Value;
				endVolumeBSW.Status = StatusCodes.Good;
			}
			else
			{
				endVolumeBSW.Value = null;
				endVolumeBSW.Status = StatusCodes.Good;
			}


			//	Calculate End Percent BSW
			var endPercentBSW = pointCalculatorData.EndTags.First(x => x.ID == "Percent BSW");
			endPercentBSW.InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated;
			endPercentBSW.Status = StatusCodes.Good;
			this.Quantities.CalculatePercentBSWFromNetBottomBSWVolumeVCF(endVolumeNetStandard, endVolumeBottoms, endVolumeBSW, endVolumeCorrectionFactor, endPercentBSW);


			// Calculate End Gross Observed
			var startVolumeGrossObserved = pointCalculatorData.StartTags.First(x => x.ID == "Volume Gross Observed");
			var endVolumeGrossObserved = pointCalculatorData.EndTags.First(x => x.ID == "Volume Gross Observed");
			var startPercentBSW = pointCalculatorData.StartTags.First(x => x.ID == "Percent BSW");
			var startVolumeCorrectionFactor = pointCalculatorData.StartTags.First(x => x.ID == "Volume Correction Factor");

			this.Quantities.CalculateGrossObservedVolumeFromNetStandardVolume(batchVolumeNetStandard, batchPercentBSW, batchVolumeCorrectionFactor, batchVolumeGrossObserved);

			if (startVolumeGrossObserved.Value is double
			&& batchVolumeGrossObserved.Value is double)
			{
				endVolumeGrossObserved.Value = (double)startVolumeGrossObserved.Value + (double)batchVolumeGrossObserved.Value;
			}

			// Calculate Volpume Gross Standard
			var endVolumeGrossStandard = pointCalculatorData.EndTags.First(x => x.ID == "Volume Gross Standard");
			endVolumeGrossStandard.InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated;
			endVolumeGrossStandard.Status = StatusCodes.Good;
			this.Quantities.CalculateGrossStandardVolume(endVolumeGrossObserved, endVolumeCorrectionFactor, endVolumeGrossStandard);

			// Calculate End Strap Volume
			var endVolumeTotalObserved = pointCalculatorData.EndTags.First(x => x.ID == "Volume Total Observed");
			endVolumeTotalObserved.InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated;
			endVolumeTotalObserved.Status = StatusCodes.Good;
			var endVolumeRoofCorrection = pointCalculatorData.EndTags.First(x => x.ID == "Volume Roof Correction");
			var endTankShellCorrection = pointCalculatorData.EndTags.First(x => x.ID == "Tank Shell Correction");
			this.Quantities.CalculateStrapVolumeFromGrossObservedVolume(endVolumeRoofCorrection, endVolumeGrossObserved, endVolumeBottoms, endTankShellCorrection, endVolumeTotalObserved);

			// Calculate End Level Water
			var endLevelWater = pointCalculatorData.EndTags.First(x => x.ID == "Level Water");
			endLevelWater.InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated;
			endLevelWater.Status = StatusCodes.Good;
			this.StrapTable.StrapLevelCalculation(endVolumeWater, endLevelWater);

			// Calculate Level Solids
			var endLevelSolids = pointCalculatorData.EndTags.First(x => x.ID == "Level Solids");
			endLevelSolids.InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated;
			endLevelSolids.Status = StatusCodes.Good;
			this.StrapTable.StrapLevelCalculation(endVolumeSolids, endLevelSolids);

			// Calculate End Level Product
			var endLevelProduct = pointCalculatorData.EndTags.First(x => x.ID == "Level Product");
			endLevelProduct.InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated;
			endLevelProduct.Status = StatusCodes.Good;
			this.StrapTable.StrapLevelCalculation(endVolumeTotalObserved, endLevelProduct);

			// Calculate End Mass Liquid
			endMassLiquid.InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated;
			endMassLiquid.Status = StatusCodes.Good;
			this.Quantities.CalculateLiquidMass(endVolumeGrossObserved, endVolumeNetStandard, endDensityProductStandard, endDensityProductStandardInAir, endMassLiquid, false);

			// Calculate Gross Standard Weight
			var endWeightGrossStandard = pointCalculatorData.EndTags.First(x => x.ID == "Weight Gross Standard");
			endWeightGrossStandard.InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated;
			endWeightGrossStandard.Status = StatusCodes.Good;
			this.Quantities.CalculateStdWeight(endVolumeGrossStandard, endDensityProductInAir, endWeightGrossStandard);

			// Calculate Net Standard Weight
			var endWeightNetStandard = pointCalculatorData.EndTags.First(x => x.ID == "Weight Net Standard");
			endWeightNetStandard.InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated;
			endWeightNetStandard.Status = StatusCodes.Good;
			this.Quantities.CalculateStdWeight(endVolumeNetStandard, endDensityProductStandardInAir, endWeightNetStandard);

			// Calculate Total Calculated Volume
			var endVolumeTotalCalculated = pointCalculatorData.EndTags.First(x => x.ID == "Volume Total Calculated");
            endVolumeTotalCalculated.InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated;
            endVolumeTotalCalculated.Status = StatusCodes.Good;
            this.Quantities.CalculateTotalCalculatedVolume(endVolumeGrossStandard, endVolumeBottoms, endVolumeTotalCalculated);

            // Calculate End Volume Gross Observed Available
            var endVolumeGrossObservedAvailable = pointCalculatorData.EndTags.First(x => x.ID == "Volume Gross Observed Available");
			endVolumeGrossObservedAvailable.InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated;
			endVolumeGrossObservedAvailable.Status = StatusCodes.Good;
			var endVolumeGrossObservedRemaining = pointCalculatorData.EndTags.First(x => x.ID == "Volume Gross Observed Remaining");
			endVolumeGrossObservedRemaining.InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated;
			endVolumeGrossObservedRemaining.Status = StatusCodes.Good;
			var endVolumeNetStandardAvailable = pointCalculatorData.EndTags.First(x => x.ID == "Volume Net Standard Available");
			endVolumeNetStandardAvailable.InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated;
			endVolumeNetStandardAvailable.Status = StatusCodes.Good;
			var endVolumeNetStandardRemaining = pointCalculatorData.EndTags.First(x => x.ID == "Volume Net Standard Remaining");
			endVolumeNetStandardRemaining.InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated;
			endVolumeNetStandardRemaining.Status = StatusCodes.Good;
			var LevelMinOpLimit = pointLogicTags.First(x => x.ID == "Level Product Min Op Limit");
			var LevelMaxOpLimit = pointLogicTags.First(x => x.ID == "Level Product Max Op Limit");
			var CriticalZone = pointLogicTags.First(x => x.ID == "Roof Critical Zone");

			this.AvailableAndRemainingVolume.AvailableAndRemainingVolumeCalculation(LevelMinOpLimit, LevelMaxOpLimit,
					endVolumeTotalObserved, endVolumeRoofCorrection, endTemperatureProduct, endDensityProducObserved, endDensityProductStandardInAir, endMassLiquid, endVolumeCorrectionFactor,
					endVolumeBottoms, CriticalZone, endVolumeWater, endVolumeSolids, endPercentBSW, endTankShellCorrection, endVolumeGrossObserved, endVolumeNetStandard,
					endVolumeGrossObservedAvailable, endVolumeNetStandardAvailable, endVolumeGrossObservedRemaining, endVolumeNetStandardRemaining);


		}

		public void TankCalculatorCalculation(PointCalculatorData pointCalculatorData, List<PointTag> pointLogicTags)
		{
			if (pointCalculatorData.IsBatchMode)
			{
				// process the updated set of tags (either start or end)
				if (pointCalculatorData.ChangedDataSet == ChangedDataSet.Start)
				{
					PerformCalculations(pointCalculatorData.StartTags, pointLogicTags);
				}
				else if (pointCalculatorData.ChangedDataSet == ChangedDataSet.End)
				{
					PerformCalculations(pointCalculatorData.EndTags, pointLogicTags);
				}
				else if (pointCalculatorData.ChangedDataSet == ChangedDataSet.None)
				{
                    PerformCalculations(pointCalculatorData.StartTags, pointLogicTags);
                    PerformCalculations(pointCalculatorData.EndTags, pointLogicTags);
                }



                if (pointCalculatorData.ChangedDataSet == ChangedDataSet.Start
				|| pointCalculatorData.ChangedDataSet == ChangedDataSet.End
				|| pointCalculatorData.ChangedDataSet == ChangedDataSet.None)
				{
					this.TankCalculatorCalculateBatch(pointCalculatorData, pointLogicTags);
				}

				else if(pointCalculatorData.ChangedDataSet == ChangedDataSet.Diff)
				{
					this.TankCalculatorCalculateEnd(pointCalculatorData, pointLogicTags);
				}
			}
			else
			{
				// if the change is to differential data, determine new end values
				if (pointCalculatorData.ChangedDataSet == ChangedDataSet.Diff)
				{
					var startPointTag = pointCalculatorData.StartTags.First(x => x.ID == pointCalculatorData.ChangedPointTagId);
					var endPointTag = pointCalculatorData.EndTags.First(x => x.ID == pointCalculatorData.ChangedPointTagId);
					var diffPointTag = pointCalculatorData.DiffTags.First(x => x.ID == pointCalculatorData.ChangedPointTagId);
					endPointTag.Value = (double)startPointTag.Value + (double)diffPointTag.Value;
					endPointTag.SourceTimeStamp = diffPointTag.SourceTimeStamp;
				}

				// process the updated set of tags (either start or end)
				if (pointCalculatorData.ChangedDataSet == ChangedDataSet.Start)
				{
					PerformCalculations(pointCalculatorData.StartTags, pointLogicTags);
				}
				else if (pointCalculatorData.ChangedDataSet == ChangedDataSet.End || pointCalculatorData.ChangedDataSet == ChangedDataSet.Diff)
				{
					PerformCalculations(pointCalculatorData.EndTags, pointLogicTags);
				}

				// determine new differential values
				foreach (var pt in pointCalculatorData.DiffTags)
				{
					pt.Value = (double)pointCalculatorData.EndTags.First(x => x.ID == pt.ID).Value - (double)pointCalculatorData.StartTags.First(x => x.ID == pt.ID).Value;
				}
			}
		}

		public void TankCalculatorCalculation(PointTag LevelProduct,
																PointTag LevelWater,
																PointTag LevelSolids,
																PointTag VolumeStrapProduct,
																PointTag VolumeStrapWater,
																PointTag VolumeStrapSolids,
																PointTag TemperatureProduct,
																PointTag TemperatureDensity,
																PointTag TemperatureVapor,
																PointTag TemperatureAmbient,
																PointTag PressureVapor,
																PointTag PercentBSW,
																PointTag DensityStandard,
																PointTag DensityObserved,
																PointTag DensityVapor,
																PointTag TankShellCorrection,
																PointTag VolumeCorrectionFactor,
																PointTag VolumeBottom,
																PointTag VolumeGrossObserved,
																PointTag VolumeGOVAvailable,
																PointTag VolumeGOVRemaining,
																PointTag VolumeNetStandard,
																PointTag VolumeNSVAvailable,
																PointTag VolumeNSVRemaining,
																PointTag VolumeRoofCorrection,
																PointTag Mass,
																PointTag MassVapor,
																PointTag VolumeCorrectionFactorForTemperature,
																PointTag VolumeCorrectionFactorForPressure,
																PointTag VolumeCorrectionFactorForPressureAndTemperature,
																PointTag APICorrectionError,
																PointTag DensityObservedInAir,
																PointTag DensityStandardInAir,
																PointTag CriticalZone,
																PointTag WeightGrossStandard,
																PointTag WeightNetStandard,
																PointTag VolumeGrossStandard,
																PointTag VolumeBSW,
																PointTag VolumeTotalCalculated,
																PointTag VolumeVaporNet,
																PointTag LevelMinOpLimit,
																PointTag LevelMaxOpLimit,
																PointTag DensityGauge,
																PointTag VolumeCorrectionFactorUnrounded,
																PointTag VolumeNetStandardUnrounded
					)
		{
				pointTagCollection = new PointTagCollection();
				CheckandInitializeVariable(LevelProduct, 0.0);
				CheckandInitializeVariable(LevelWater, 0.0);
				CheckandInitializeVariable(LevelSolids, 0.0);
				CheckandInitializeVariable(VolumeStrapProduct, 0.0);
				CheckandInitializeVariable(VolumeStrapWater, 0.0);
				CheckandInitializeVariable(VolumeStrapSolids, 0.0);
				CheckandInitializeVariable(TemperatureProduct, 0.0);
				CheckandInitializeVariable(TemperatureDensity, 0.0);
				CheckandInitializeVariable(TemperatureVapor, 0.0);
				CheckandInitializeVariable(TemperatureAmbient, 0.0);
				CheckandInitializeVariable(PressureVapor, 0.0);
				CheckandInitializeVariable(PercentBSW, 0.0);
				CheckandInitializeVariable(DensityStandard, 1.0);
				CheckandInitializeVariable(DensityObserved, 1.0);
				CheckandInitializeVariable(DensityVapor, 1.0);
				CheckandInitializeVariable(TankShellCorrection, 1.0);
				CheckandInitializeVariable(VolumeCorrectionFactor, 1.0);
				CheckandInitializeVariable(VolumeCorrectionFactorUnrounded, 1.0);
				CheckandInitializeVariable(VolumeBottom, 0.0);
				CheckandInitializeVariable(VolumeGrossObserved, 0.0);
				CheckandInitializeVariable(VolumeGOVAvailable, 0.0);
				CheckandInitializeVariable(VolumeGOVRemaining, 0.0);
				CheckandInitializeVariable(VolumeNetStandard, 0.0);
				CheckandInitializeVariable(VolumeNetStandardUnrounded, 0.0);
				CheckandInitializeVariable(VolumeNSVAvailable, 0.0);
				CheckandInitializeVariable(VolumeNSVRemaining, 0.0);
				CheckandInitializeVariable(VolumeRoofCorrection, 0.0);
				CheckandInitializeVariable(Mass, 0.0);
				CheckandInitializeVariable(MassVapor, 0.0);
				CheckandInitializeVariable(VolumeCorrectionFactorForTemperature, 1.0);
				CheckandInitializeVariable(VolumeCorrectionFactorForPressure, 1.0);
				CheckandInitializeVariable(VolumeCorrectionFactorForPressureAndTemperature, 1.0);
				CheckandInitializeVariable(DensityObservedInAir, 1.0);
				CheckandInitializeVariable(DensityStandardInAir, 1.0);
				CheckandInitializeVariable(WeightGrossStandard, 0.0);
				CheckandInitializeVariable(WeightNetStandard, 0.0);
				CheckandInitializeVariable(VolumeGrossStandard, 0.0);
				CheckandInitializeVariable(VolumeBSW, 0.0);
				CheckandInitializeVariable(VolumeTotalCalculated, 0.0);
				CheckandInitializeVariable(VolumeVaporNet, 0.0);
				CheckandInitializeVariable(LevelMinOpLimit, 0.0);
				CheckandInitializeVariable(LevelMaxOpLimit, 40.0);

				CheckandInitializeVariable(DensityGauge, 0.0);

				pointTagCollection.Add(LevelProduct);
				pointTagCollection.Add(LevelWater);
				pointTagCollection.Add(LevelSolids);
				pointTagCollection.Add(VolumeStrapProduct);
				pointTagCollection.Add(VolumeStrapWater);
				pointTagCollection.Add(VolumeStrapSolids);
				pointTagCollection.Add(TemperatureProduct);
				pointTagCollection.Add(TemperatureDensity);
				pointTagCollection.Add(TemperatureVapor);
				pointTagCollection.Add(TemperatureAmbient);
				pointTagCollection.Add(PressureVapor);
				pointTagCollection.Add(PercentBSW);
				pointTagCollection.Add(DensityStandard);
				pointTagCollection.Add(DensityObserved);
				pointTagCollection.Add(DensityVapor);
				pointTagCollection.Add(TankShellCorrection);
				pointTagCollection.Add(VolumeCorrectionFactor);
				pointTagCollection.Add(VolumeCorrectionFactorUnrounded);
				pointTagCollection.Add(VolumeBottom);
				pointTagCollection.Add(VolumeGrossObserved);
				pointTagCollection.Add(VolumeGOVAvailable);
				pointTagCollection.Add(VolumeGOVRemaining);
				pointTagCollection.Add(VolumeNetStandard);
				pointTagCollection.Add(VolumeNetStandardUnrounded);
				pointTagCollection.Add(VolumeNSVAvailable);
				pointTagCollection.Add(VolumeNSVRemaining);
				pointTagCollection.Add(VolumeRoofCorrection);
				pointTagCollection.Add(Mass);
				pointTagCollection.Add(MassVapor);
				pointTagCollection.Add(VolumeCorrectionFactorForTemperature);
				pointTagCollection.Add(VolumeCorrectionFactorForPressure);
				pointTagCollection.Add(VolumeCorrectionFactorForPressureAndTemperature);
				pointTagCollection.Add(APICorrectionError);
				pointTagCollection.Add(DensityObservedInAir);
				pointTagCollection.Add(DensityStandardInAir);
				pointTagCollection.Add(CriticalZone);
				pointTagCollection.Add(WeightGrossStandard);
				pointTagCollection.Add(WeightNetStandard);
				pointTagCollection.Add(VolumeGrossStandard);
				pointTagCollection.Add(VolumeBSW);
				pointTagCollection.Add(VolumeTotalCalculated);
				pointTagCollection.Add(VolumeVaporNet);
				pointTagCollection.Add(LevelMinOpLimit);
				pointTagCollection.Add(LevelMaxOpLimit);
				pointTagCollection.Add(DensityGauge);

				foreach (PointTag pointTag in pointTagCollection)
				{
					SetTagStatusToGood(pointTag);
				}

				// we need to determine if this is a single calculation or a iteration based on the timestamp
				CalculateIterationValuesIfRequired(LevelProduct,
																LevelWater,
																LevelSolids,
																VolumeStrapProduct,
																VolumeStrapWater,
																VolumeStrapSolids,
																TemperatureProduct,
																TemperatureDensity,
																TemperatureVapor,
																TemperatureAmbient,
																PressureVapor,
																PercentBSW,
																DensityStandard,
																DensityObserved,
																DensityVapor,
																TankShellCorrection,
																VolumeCorrectionFactor,
																VolumeBottom,
																VolumeGrossObserved,
																VolumeGOVAvailable,
																VolumeGOVRemaining,
																VolumeNetStandard,
																VolumeNSVAvailable,
																VolumeNSVRemaining,
																VolumeRoofCorrection,
																Mass,
																MassVapor,
																VolumeCorrectionFactorForTemperature,
																VolumeCorrectionFactorForPressure,
																VolumeCorrectionFactorForPressureAndTemperature,
																APICorrectionError,
																DensityObservedInAir,
																DensityStandardInAir,
																CriticalZone,
																WeightGrossStandard,
																WeightNetStandard,
																VolumeGrossStandard,
																VolumeBSW,
																VolumeTotalCalculated,
																VolumeVaporNet,
																LevelMinOpLimit,
																LevelMaxOpLimit,
																VolumeCorrection,
																ShellCorrection,
																FloatingRoofCorrection,
																AvailableAndRemainingVolume);

				// always do a complete calculation at the end
				PerformStandardTankCalculatorCalculations(LevelProduct,
																LevelWater,
																LevelSolids,
																VolumeStrapProduct,
																VolumeStrapWater,
																VolumeStrapSolids,
																TemperatureProduct,
																TemperatureDensity,
																TemperatureVapor,
																TemperatureAmbient,
																PressureVapor,
																PercentBSW,
																DensityStandard,
																DensityObserved,
																DensityVapor,
																TankShellCorrection,
																VolumeCorrectionFactor,
																VolumeCorrectionFactorUnrounded,
																VolumeBottom,
																VolumeGrossObserved,
																VolumeGOVAvailable,
																VolumeGOVRemaining,
																VolumeNetStandard,
																VolumeNetStandardUnrounded,
																VolumeNSVAvailable,
																VolumeNSVRemaining,
																VolumeRoofCorrection,
																Mass,
																MassVapor,
																VolumeCorrectionFactorForTemperature,
																VolumeCorrectionFactorForPressure,
																VolumeCorrectionFactorForPressureAndTemperature,
																APICorrectionError,
																DensityObservedInAir,
																DensityStandardInAir,
																CriticalZone,
																WeightGrossStandard,
																WeightNetStandard,
																VolumeGrossStandard,
																VolumeBSW,
																VolumeTotalCalculated,
																VolumeVaporNet,
																LevelMinOpLimit,
																LevelMaxOpLimit,
																DensityGauge,
																VolumeCorrection,
																ShellCorrection,
																FloatingRoofCorrection,
																AvailableAndRemainingVolume);
		}

		private void PerformStandardTankCalculatorCalculations(PointTag LevelProduct,
																PointTag LevelWater,
																PointTag LevelSolids,
																PointTag VolumeStrapProduct,
																PointTag VolumeStrapWater,
																PointTag VolumeStrapSolids,
																PointTag TemperatureProduct,
																PointTag TemperatureDensity,
																PointTag TemperatureVapor,
																PointTag TemperatureAmbient,
																PointTag PressureVapor,
																PointTag PercentBSW,
																PointTag DensityStandard,
																PointTag DensityObserved,
																PointTag DensityVapor,
																PointTag TankShellCorrection,
																PointTag VolumeCorrectionFactor,
																PointTag VolumeCorrectionFactorUnrounded,
																PointTag VolumeBottom,
																PointTag VolumeGrossObserved,
																PointTag VolumeGOVAvailable,
																PointTag VolumeGOVRemaining,
																PointTag VolumeNetStandard,
																PointTag VolumeNetStandardUnrounded,
																PointTag VolumeNSVAvailable,
																PointTag VolumeNSVRemaining,
																PointTag VolumeRoofCorrection,
																PointTag Mass,
																PointTag MassVapor,
																PointTag VolumeCorrectionFactorForTemperature,
																PointTag VolumeCorrectionFactorForPressure,
																PointTag VolumeCorrectionFactorForPressureAndTemperature,
																PointTag APICorrectionError,
																PointTag DensityObservedInAir,
																PointTag DensityStandardInAir,
																PointTag CriticalZone,
																PointTag WeightGrossStandard,
																PointTag WeightNetStandard,
																PointTag VolumeGrossStandard,
																PointTag VolumeBSW,
																PointTag VolumeTotalCalculated,
																PointTag VolumeVaporNet,
																PointTag LevelMinOpLimit,
																PointTag LevelMaxOpLimit,
																PointTag DensityGauge,
																FMVcf VCF,
																FMShellCorrection ShellCorrection,
																FMFloatingRoofCorrection RoofCorrection,
																FMAvailableAndRemainingVolume AvailableAndRemainingVolume)
		{
				// calculate strap and water volumes
				this.StrapTable.StrapCalculation(LevelProduct,
														LevelWater,
														LevelSolids,
														VolumeStrapProduct,
														VolumeStrapWater,
														VolumeStrapSolids);

				// do vcf calculation
				VCF.VcfCalculation(TemperatureProduct, TemperatureDensity, TemperatureVapor, DensityStandard, DensityObserved, PressureVapor,
												VolumeCorrectionFactorForTemperature, VolumeCorrectionFactorForPressure, VolumeCorrectionFactorForPressureAndTemperature,
												VolumeCorrectionFactor, VolumeCorrectionFactorUnrounded, APICorrectionError, DensityObservedInAir, DensityStandardInAir, DensityGauge);

				ShellCorrection.ShellCorrectionCalculation(TemperatureAmbient, TemperatureProduct, TankShellCorrection);

				RoofCorrection.FloatingRoofCorrectionCalculation(TemperatureProduct, DensityObserved, DensityObservedInAir, Mass, LevelProduct, VolumeCorrectionFactor, CriticalZone, VolumeRoofCorrection);

				this.Quantities.QuantityCalculation(VolumeRoofCorrection, VolumeStrapProduct, VolumeStrapWater, VolumeStrapSolids,
												PercentBSW, VolumeCorrectionFactor, VolumeCorrectionFactorUnrounded, DensityStandard, DensityObserved,
												TankShellCorrection, DensityObservedInAir, DensityStandardInAir, DensityVapor, TemperatureVapor,
												PressureVapor, VolumeBottom, VolumeGrossObserved, VolumeNetStandard, VolumeNetStandardUnrounded,
												Mass, WeightGrossStandard, VolumeGrossStandard, WeightNetStandard,
												VolumeBSW, VolumeTotalCalculated, VolumeVaporNet, MassVapor);

				AvailableAndRemainingVolume.AvailableAndRemainingVolumeCalculation(LevelMinOpLimit, LevelMaxOpLimit,
					VolumeStrapProduct, VolumeRoofCorrection, TemperatureProduct, DensityObserved, DensityStandardInAir, Mass, VolumeCorrectionFactor,
					VolumeBottom, CriticalZone, VolumeStrapWater, VolumeStrapSolids, PercentBSW, TankShellCorrection, VolumeGrossObserved, VolumeNetStandard,
					VolumeGOVAvailable, VolumeNSVAvailable, VolumeGOVRemaining, VolumeNSVRemaining);

		}

		public ModuleInputOutputCollection GetInputOutputCollection(string calculationName)
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

		private void CalculateIterationValuesIfRequired(PointTag LevelProduct,
																PointTag LevelWater,
																PointTag LevelSolids,
																PointTag VolumeStrapProduct,
																PointTag VolumeStrapWater,
																PointTag VolumeStrapSolids,
																PointTag TemperatureProduct,
																PointTag TemperatureDensity,
																PointTag TemperatureVapor,
																PointTag TemperatureAmbient,
																PointTag PressureVapor,
																PointTag PercentBSW,
																PointTag DensityStandard,
																PointTag DensityObserved,
																PointTag DensityVapor,
																PointTag TankShellCorrection,
																PointTag VolumeCorrectionFactor,
																PointTag VolumeBottom,
																PointTag VolumeGrossObserved,
																PointTag VolumeGOVAvailable,
																PointTag VolumeGOVRemaining,
																PointTag VolumeNetStandard,
																PointTag VolumeNSVAvailable,
																PointTag VolumeNSVRemaining,
																PointTag VolumeRoofCorrection,
																PointTag Mass,
																PointTag MassVapor,
																PointTag VolumeCorrectionFactorForTemperature,
																PointTag VolumeCorrectionFactorForPressure,
																PointTag VolumeCorrectionFactorForPressureAndTemperature,
																PointTag APICorrectionError,
																PointTag DensityObservedInAir,
																PointTag DensityStandardInAir,
																PointTag CriticalZone,
																PointTag WeightGrossStandard,
																PointTag WeightNetStandard,
																PointTag VolumeGrossStandard,
																PointTag VolumeBSW,
																PointTag VolumeTotalCalculated,
																PointTag VolumeVaporNet,
																PointTag LevelMinOpLimit,
																PointTag LevelMaxOpLimit,
																FMVcf VCF,
																FMShellCorrection ShellCorrection,
																FMFloatingRoofCorrection RoofCorrection,
																FMAvailableAndRemainingVolume AvailableAndRemainingVolume)
		{
				// there are only a few backwards calc that we support so we will check them one at a time
				// first water
				CheckandDowaterLevelCalculation(LevelWater,
																LevelSolids,
																VolumeStrapWater,
																VolumeStrapSolids);

				// solids next
				CheckandDosolidsLevelCalculation(LevelWater,
																LevelSolids,
																VolumeStrapWater,
																VolumeStrapSolids);

				// strap volume
				CheckandDostrapvolumeLevelCalculation(LevelProduct,
																	VolumeStrapProduct);

				// Gross Volume
				CheckandDogrossvolumeLevelCalculation(LevelProduct,
																	VolumeStrapProduct,
																	VolumeRoofCorrection,
																	VolumeBottom,
																	TankShellCorrection,
																	VolumeGrossObserved);

				// net volume
				CheckandDonetvolumeLevelCalculation(LevelProduct,
																	VolumeStrapProduct,
																	VolumeRoofCorrection,
																	VolumeBottom,
																	TankShellCorrection,
																	VolumeGrossObserved,
																	PercentBSW,
																	VolumeCorrectionFactorForPressureAndTemperature,
																	VolumeNetStandard);

				// Mass
				CheckandDomassLevelCalculation(LevelProduct,
														VolumeStrapProduct,
														VolumeRoofCorrection,
														VolumeBottom,
														TankShellCorrection,
														VolumeGrossObserved,
														PercentBSW,
														VolumeCorrectionFactorForPressureAndTemperature,
														VolumeNetStandard,
														DensityStandard,
														DensityStandardInAir,
														Mass);


		}

		private bool timeStampistheGreatest(PointTag pointtocheck)
		{
				foreach (PointTag pointTag in pointTagCollection)
				{
					if (pointTag == pointtocheck)
						continue;
					if (pointtocheck.SourceTimeStamp != null && pointtocheck.SourceTimeStamp.UtcTicks < pointTag.SourceTimeStamp.UtcTicks)
						return false;
				}
				return true;
		}

		private double GetPercentageBasedonMinandMax(PointTag pointTag)
		{
				double ReturnValue = 0.0;

				if ((double)pointTag.Value >= pointTag.Maximum)
					return 1.0;
				if ((double)pointTag.Value <= pointTag.Minimum)
					return 0.0;

				ReturnValue = (double)pointTag.Value / (double)(pointTag.Maximum - pointTag.Minimum);

				return ReturnValue;
		}

		//function to return a Value that is .10 percent of the tag scale
		private double GetOnePercentValue(PointTag pointTag)
		{
				double ReturnValue = 10.0;
				ReturnValue = (double)(pointTag.Maximum - pointTag.Minimum) * .001;
				return ReturnValue;
		}

		private void CheckandDowaterLevelCalculation(PointTag LevelWater,
																	PointTag LevelSolids,
																	PointTag VolumeStrapWater,
																	PointTag VolumeStrapSolids)
		{

				// we only do one calc at a time so check the water volume time stamp is greater than any of the others
				if (timeStampistheGreatest(VolumeStrapWater) == false)
					return;

				// figure out what the associated water level is for the selected volume
				if (VolumeStrapWater.Value == null)
					return;

				// get the percentage of the value based on the range
				double percentageRange = GetPercentageBasedonMinandMax(VolumeStrapWater);
				double onePercentValue = GetOnePercentValue(VolumeStrapWater);

				double dvalueAmount = SetLevelValueAmount(LevelWater);  // bds
				double valueRangeCheck = onePercentValue / 100.0;

				if (percentageRange == 1.0)
					LevelWater.Value = LevelWater.Maximum;
				else if (percentageRange == 0.0)
					LevelWater.Value = LevelWater.Minimum;
				else
				{
					// set the level value to approxomately what it should be based on the percentage
					LevelWater.Value = (double)((LevelWater.Maximum - LevelWater.Minimum) * percentageRange);
					bool bExit = false;
					PointTag TempVolumeStrapWater = (PointTag)VolumeStrapWater.Clone();
					TempVolumeStrapWater.InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated;
					TempVolumeStrapWater.Status = StatusCodes.Good;
					// make the initial call to the strap module to determine the direction
					this.StrapTable.WaterStrapVolumeCalculation(LevelWater, LevelSolids, VolumeStrapSolids, TempVolumeStrapWater);
					if ((double)TempVolumeStrapWater.Value - (double)VolumeStrapWater.Value < valueRangeCheck &&
						(double)TempVolumeStrapWater.Value - (double)VolumeStrapWater.Value > -valueRangeCheck)
					{
						if ((double)TempVolumeStrapWater.Value - (double)VolumeStrapWater.Value < 0.0)
								LevelWater.Value = SetLevelValueBasedonUnits(LevelWater, true);
						else
								LevelWater.Value = SetLevelValueBasedonUnits(LevelWater, false);
						return;
					}

					if (((double)TempVolumeStrapWater.Value - (double)VolumeStrapWater.Value) > 0.0)
						dvalueAmount *= -1;

					while (bExit == false)
					{
						LevelWater.Value = (double)LevelWater.Value + dvalueAmount;
						if (this.StrapTable.WaterStrapVolumeCalculation(LevelWater, LevelSolids, VolumeStrapSolids, TempVolumeStrapWater) == false)
						{
								LevelWater.Value = 0.0;
								return;
						}
						else if ((double)TempVolumeStrapWater.Value - (double)VolumeStrapWater.Value < valueRangeCheck &&
								(double)TempVolumeStrapWater.Value - (double)VolumeStrapWater.Value > -valueRangeCheck)
						{
								if ((double)TempVolumeStrapWater.Value - (double)VolumeStrapWater.Value < 0.0)
									LevelWater.Value = SetLevelValueBasedonUnits(LevelWater, true);
								else
									LevelWater.Value = SetLevelValueBasedonUnits(LevelWater, false);
								return;
						}
						else if (dvalueAmount < 0.0 &&
								(double)TempVolumeStrapWater.Value - (double)VolumeStrapWater.Value < 0.0)
						{
								LevelWater.Value = SetLevelValueBasedonUnits(LevelWater, true);
								return;
						}
						else if (dvalueAmount > 0.0 &&
								(double)TempVolumeStrapWater.Value - (double)VolumeStrapWater.Value > 0.0)
						{
								LevelWater.Value = SetLevelValueBasedonUnits(LevelWater, false);
								return;
						}
						else if ((double)LevelWater.Value >= LevelWater.Maximum ||
								(double)LevelWater.Value <= LevelWater.Minimum)
						{
								return;
						}
						else
						{
								if (LevelWater.Units != EngineeringUnit.FmlFtIn16Th &&
									LevelWater.Units != EngineeringUnit.FmlFtIn8Th)
								{
									if (LevelWater.DecimalPlaces > 3)
									{
										if (Math.Abs((double)TempVolumeStrapWater.Value - (double)VolumeStrapWater.Value) > onePercentValue)
										{
												dvalueAmount = 0.001;
										}
										else
										{
												dvalueAmount = SetLevelValueAmount(LevelWater);
										}
									}
								}
						}
					}
				}
		}	// end CheckandDowaterLevelCalculation

		private void CheckandDosolidsLevelCalculation(PointTag LevelWater,
																	PointTag LevelSolids,
																	PointTag VolumeStrapWater,
																	PointTag VolumeStrapSolids)
		{
				// we only do one calc at a time so check the Solids volume time stamp is greater than any of the others
				if (timeStampistheGreatest(VolumeStrapSolids) == false)
					return;

				// figure out what the associated Solids level is for the selected volume
				if (VolumeStrapSolids.Value == null)
					return;

				// get the percentage of the value based on the range
				double percentageRange = GetPercentageBasedonMinandMax(VolumeStrapSolids);
				double onePercentValue = GetOnePercentValue(VolumeStrapSolids);

				double dvalueAmount = SetLevelValueAmount(LevelSolids);
				double valueRangeCheck = onePercentValue / 100.0;

				if (percentageRange == 1.0)
					LevelSolids.Value = LevelSolids.Maximum;
				else if (percentageRange == 0.0)
					LevelSolids.Value = LevelSolids.Minimum;
				else
				{
					// set the level value to approxomately what it should be based on the percentage
					LevelSolids.Value = (double)((LevelSolids.Maximum - LevelSolids.Minimum) * percentageRange);

					bool bExit = false;
					PointTag TempVolumeStrapSolids = (PointTag)VolumeStrapSolids.Clone();
					TempVolumeStrapSolids.InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated;
					TempVolumeStrapSolids.Status = StatusCodes.Good;
					// make the initial call to the strap module to determine the direction
					this.StrapTable.WaterStrapVolumeCalculation(LevelSolids, LevelSolids, TempVolumeStrapSolids, VolumeStrapWater);
					if ((double)TempVolumeStrapSolids.Value - (double)VolumeStrapSolids.Value < valueRangeCheck &&
						(double)TempVolumeStrapSolids.Value - (double)VolumeStrapSolids.Value > -valueRangeCheck)
					{
						if ((double)TempVolumeStrapSolids.Value - (double)VolumeStrapSolids.Value < 0.0)
								LevelSolids.Value = SetLevelValueBasedonUnits(LevelSolids, true);
						else
								LevelSolids.Value = SetLevelValueBasedonUnits(LevelSolids, false);
						return;
					}

					if (((double)TempVolumeStrapSolids.Value - (double)VolumeStrapSolids.Value) > 0.0)
						dvalueAmount *= -1;

					while (bExit == false)
					{
						LevelSolids.Value = (double)LevelSolids.Value + dvalueAmount;
						if (this.StrapTable.WaterStrapVolumeCalculation(LevelWater, LevelSolids, TempVolumeStrapSolids, VolumeStrapWater) == false)
						{
								LevelSolids.Value = 0.0;
								return;
						}
						else if ((double)TempVolumeStrapSolids.Value - (double)VolumeStrapSolids.Value < valueRangeCheck &&
								(double)TempVolumeStrapSolids.Value - (double)VolumeStrapSolids.Value > -valueRangeCheck)
						{
								if ((double)TempVolumeStrapSolids.Value - (double)VolumeStrapSolids.Value < 0.0)
									LevelSolids.Value = SetLevelValueBasedonUnits(LevelSolids, true);
								else
									LevelSolids.Value = SetLevelValueBasedonUnits(LevelSolids, false);
								return;
						}
						else if (dvalueAmount < 0.0 &&
								(double)TempVolumeStrapSolids.Value - (double)VolumeStrapSolids.Value < 0.0)
						{
								LevelSolids.Value = SetLevelValueBasedonUnits(LevelSolids, true);
								return;
						}
						else if (dvalueAmount > 0.0 &&
								(double)TempVolumeStrapSolids.Value - (double)VolumeStrapSolids.Value > 0.0)
						{
								LevelSolids.Value = SetLevelValueBasedonUnits(LevelSolids, false);
								return;
						}
						else if ((double)LevelSolids.Value >= LevelSolids.Maximum ||
								(double)LevelSolids.Value <= LevelSolids.Minimum)
						{
								return;
						}
						else
						{
								if (LevelSolids.Units != EngineeringUnit.FmlFtIn16Th &&
									LevelSolids.Units != EngineeringUnit.FmlFtIn8Th)
								{
									if (LevelSolids.DecimalPlaces > 3)
									{
										if (Math.Abs((double)TempVolumeStrapSolids.Value - (double)VolumeStrapSolids.Value) > onePercentValue)
										{
												dvalueAmount = 0.001;
										}
										else
										{
												dvalueAmount = SetLevelValueAmount(LevelSolids);
										}
									}
								}
						}
					}
				}
		}	// CheckandDosolidsLevelCalculation

		private void CheckandDostrapvolumeLevelCalculation(PointTag LevelProduct,
																				PointTag VolumeStrapProduct)
		{
				// calculate the level for a given strap volume input
				if (timeStampistheGreatest(VolumeStrapProduct) == false)
					return;

				// figure out what the associated Solids level is for the selected volume
				if (VolumeStrapProduct.Value == null)
					return;

				// get the percentage of the value based on the range
				double percentageRange = GetPercentageBasedonMinandMax(VolumeStrapProduct);
				double onePercentValue = GetOnePercentValue(VolumeStrapProduct);

				double dvalueAmount = SetLevelValueAmount(LevelProduct);
				double valueRangeCheck = onePercentValue / 100.0;

				if (percentageRange == 1.0)
					LevelProduct.Value = LevelProduct.Maximum;
				else if (percentageRange == 0.0)
					LevelProduct.Value = LevelProduct.Minimum;
				else
				{
					// set the level value to approxomately what it should be based on the percentage
					LevelProduct.Value = (double)((LevelProduct.Maximum - LevelProduct.Minimum) * percentageRange);

					bool bExit = false;
					PointTag TempVolumeStrap = (PointTag)VolumeStrapProduct.Clone();
					TempVolumeStrap.InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated;
					TempVolumeStrap.Status = StatusCodes.Good;
					// make the initial call to the strap module to determine the direction
					this.StrapTable.StrapVolumeCalculation(LevelProduct, TempVolumeStrap);

					if ((double)TempVolumeStrap.Value - (double)VolumeStrapProduct.Value < valueRangeCheck &&
						(double)TempVolumeStrap.Value - (double)VolumeStrapProduct.Value > -valueRangeCheck)
					{
						if ((double)TempVolumeStrap.Value - (double)VolumeStrapProduct.Value < 0.0)
								LevelProduct.Value = SetLevelValueBasedonUnits(LevelProduct, true);
						else
								LevelProduct.Value = SetLevelValueBasedonUnits(LevelProduct, false);
						return;
					}

					if (((double)TempVolumeStrap.Value - (double)VolumeStrapProduct.Value) > 0.0)
						dvalueAmount *= -1;

					while (bExit == false)
					{
						LevelProduct.Value = (double)LevelProduct.Value + dvalueAmount;
						if (this.StrapTable.StrapVolumeCalculation(LevelProduct, TempVolumeStrap) == false)
						{
								LevelProduct.Value = 0.0;
								return;
						}
						else if ((double)TempVolumeStrap.Value - (double)VolumeStrapProduct.Value < valueRangeCheck &&
								(double)TempVolumeStrap.Value - (double)VolumeStrapProduct.Value > -valueRangeCheck)
						{
								if ((double)TempVolumeStrap.Value - (double)VolumeStrapProduct.Value < 0.0)
									LevelProduct.Value = SetLevelValueBasedonUnits(LevelProduct, true);
								else
									LevelProduct.Value = SetLevelValueBasedonUnits(LevelProduct, false);
								return;
						}
						else if (dvalueAmount < 0.0 &&
								(double)TempVolumeStrap.Value - (double)VolumeStrapProduct.Value < 0.0)
						{
								LevelProduct.Value = SetLevelValueBasedonUnits(LevelProduct, true);
								return;
						}
						else if (dvalueAmount > 0.0 &&
								(double)TempVolumeStrap.Value - (double)VolumeStrapProduct.Value > 0.0)
						{
								LevelProduct.Value = SetLevelValueBasedonUnits(LevelProduct, false);
								return;
						}
						else if ((double)LevelProduct.Value >= LevelProduct.Maximum)
						{
								LevelProduct.Value = LevelProduct.Maximum;
								return;
						}
						else if ((double)LevelProduct.Value <= LevelProduct.Minimum)
						{
								LevelProduct.Value = LevelProduct.Minimum;
								return;
						}
						else
						{
								if (LevelProduct.Units != EngineeringUnit.FmlFtIn16Th &&
									LevelProduct.Units != EngineeringUnit.FmlFtIn8Th)
								{
									if (LevelProduct.DecimalPlaces > 3)
									{
										if (Math.Abs((double)TempVolumeStrap.Value - (double)VolumeStrapProduct.Value) > onePercentValue)
										{
												dvalueAmount = 0.001;
										}
										else
										{
												dvalueAmount = SetLevelValueAmount(LevelProduct);
										}
									}
								}
						}
					}
				}
		}	// end CheckandDostrapvolumeLevelCalculation

		private void CheckandDogrossvolumeLevelCalculation(PointTag LevelProduct,
																	PointTag VolumeStrapProduct,
																	PointTag VolumeRoofCorrection,
																	PointTag VolumeBottom,
																	PointTag TankShellCorrection,
																	PointTag VolumeGrossObserved)
		{
				if (timeStampistheGreatest(VolumeGrossObserved) == false)
					return;

				// figure out what the associated Solids level is for the selected volume
				if (VolumeGrossObserved.Value == null)
					return;

				// get the percentage of the value based on the range
				double percentageRange = GetPercentageBasedonMinandMax(VolumeGrossObserved);
				double onePercentValue = GetOnePercentValue(VolumeGrossObserved);

				double dvalueAmount = SetLevelValueAmount(LevelProduct);
				double valueRangeCheck = onePercentValue / 100.0;

				if (percentageRange == 1.0)
					LevelProduct.Value = LevelProduct.Maximum;
				else if (percentageRange == 0.0)
					LevelProduct.Value = LevelProduct.Minimum;
				else
				{
					// set the level value to approxomately what it should be based on the percentage
					LevelProduct.Value = (double)((LevelProduct.Maximum - LevelProduct.Minimum) * percentageRange);

					bool bExit = false;
					PointTag TempGrossVolume = (PointTag)VolumeGrossObserved.Clone();
					TempGrossVolume.InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated;
					TempGrossVolume.Status = StatusCodes.Good;

					// make the initial call to the strap module to determine the direction
					// strap volume
					this.StrapTable.StrapVolumeCalculation(LevelProduct, VolumeStrapProduct);

					// gross volume
					CheckandInitializeVariable(VolumeRoofCorrection, 0.0);
					CheckandInitializeVariable(VolumeBottom, 0.0);
					CheckandInitializeVariable(TankShellCorrection, 1.0);
					this.Quantities.CalculateGrossObserverdVolume(VolumeRoofCorrection, VolumeStrapProduct, VolumeBottom, TankShellCorrection, TempGrossVolume);

					if (TempGrossVolume.Value == null)
						return;

					if ((double)TempGrossVolume.Value - (double)VolumeGrossObserved.Value < valueRangeCheck &&
						(double)TempGrossVolume.Value - (double)VolumeGrossObserved.Value > -valueRangeCheck)
					{
						if ((double)TempGrossVolume.Value - (double)VolumeGrossObserved.Value < 0.0)
								LevelProduct.Value = SetLevelValueBasedonUnits(LevelProduct, true);
						else
								LevelProduct.Value = SetLevelValueBasedonUnits(LevelProduct, false);
						return;
					}

					if (((double)TempGrossVolume.Value - (double)VolumeGrossObserved.Value) > 0.0)
						dvalueAmount *= -1;

					while (bExit == false)
					{
						LevelProduct.Value = (double)LevelProduct.Value + dvalueAmount;

						if (this.StrapTable.StrapVolumeCalculation(LevelProduct, VolumeStrapProduct) == false)
						{
								return;
						}
						this.Quantities.CalculateGrossObserverdVolume(VolumeRoofCorrection, VolumeStrapProduct, VolumeBottom, TankShellCorrection, TempGrossVolume);
						if (TempGrossVolume.Value == null)
						{
								return;
						}
						if ((double)TempGrossVolume.Value - (double)VolumeGrossObserved.Value < valueRangeCheck &&
								(double)TempGrossVolume.Value - (double)VolumeGrossObserved.Value > -valueRangeCheck)
						{
								if ((double)TempGrossVolume.Value - (double)VolumeGrossObserved.Value < 0.0)
									LevelProduct.Value = SetLevelValueBasedonUnits(LevelProduct, true);
								else
									LevelProduct.Value = SetLevelValueBasedonUnits(LevelProduct, false);
								return;
						}
						else if (dvalueAmount < 0.0 &&
								(double)TempGrossVolume.Value - (double)VolumeGrossObserved.Value < 0.0)
						{
								LevelProduct.Value = SetLevelValueBasedonUnits(LevelProduct, true);
								return;
						}
						else if (dvalueAmount > 0.0 &&
								(double)TempGrossVolume.Value - (double)VolumeGrossObserved.Value > 0.0)
						{
								LevelProduct.Value = SetLevelValueBasedonUnits(LevelProduct, false);
								return;
						}
						else if ((double)LevelProduct.Value >= LevelProduct.Maximum)
						{
								LevelProduct.Value = LevelProduct.Maximum;
								return;
						}
						else if ((double)LevelProduct.Value <= LevelProduct.Minimum)
						{
								LevelProduct.Value = LevelProduct.Minimum;
								return;
						}
						else
						{
								if (LevelProduct.Units != EngineeringUnit.FmlFtIn16Th &&
									LevelProduct.Units != EngineeringUnit.FmlFtIn8Th)
								{
									if (LevelProduct.DecimalPlaces > 3)
									{
										if (Math.Abs((double)TempGrossVolume.Value - (double)VolumeGrossObserved.Value) > onePercentValue)
										{
												dvalueAmount = 0.001;
										}
										else
										{
												dvalueAmount = SetLevelValueAmount(LevelProduct);
										}
									}
								}
						}
					}
				}
		} // end CheckandDogrossvolumeLevelCalculation

		private void CheckandDonetvolumeLevelCalculation(PointTag LevelProduct,
																	PointTag VolumeStrapProduct,
																	PointTag VolumeRoofCorrection,
																	PointTag VolumeBottom,
																	PointTag TankShellCorrection,
																	PointTag VolumeGrossObserved,
																	PointTag PercentBSW,
																	PointTag VolumeCorrectionFactorForPressureAndTemperature,
																	PointTag VolumeNetStandard)
		{
				if (timeStampistheGreatest(VolumeNetStandard) == false)
					return;

				// figure out what the associated Solids level is for the selected volume
				if (VolumeNetStandard.Value == null)
					return;

				// get the percentage of the value based on the range
				double percentageRange = GetPercentageBasedonMinandMax(VolumeNetStandard);
				double onePercentValue = GetOnePercentValue(VolumeNetStandard);

				double dvalueAmount = SetLevelValueAmount(LevelProduct);
				double valueRangeCheck = onePercentValue / 100.0;

				if (percentageRange == 1.0)
					LevelProduct.Value = LevelProduct.Maximum;
				else if (percentageRange == 0.0)
					LevelProduct.Value = LevelProduct.Minimum;
				else
				{
					// set the level value to approxomately what it should be based on the percentage
					LevelProduct.Value = (double)((LevelProduct.Maximum - LevelProduct.Minimum) * percentageRange);

					bool bExit = false;
					PointTag TempNetVolume = (PointTag)VolumeNetStandard.Clone();
					TempNetVolume.InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated;
					TempNetVolume.Status = StatusCodes.Good;

					// make the initial call to the strap module to determine the direction
					// strap volume
					this.StrapTable.StrapVolumeCalculation(LevelProduct, VolumeStrapProduct);

					// since the variables may not be on the calculator or supported check if they are null and initialize if they are
					CheckandInitializeVariable(VolumeRoofCorrection, 0.0);
					CheckandInitializeVariable(VolumeBottom, 0.0);
					CheckandInitializeVariable(TankShellCorrection, 1.0);
					// gross volume
					this.Quantities.CalculateGrossObserverdVolume(VolumeRoofCorrection,
																					VolumeStrapProduct,
																					VolumeBottom,
																					TankShellCorrection,
																					VolumeGrossObserved);

					CheckandInitializeVariable(PercentBSW, 0.0);
					CheckandInitializeVariable(VolumeCorrectionFactorForPressureAndTemperature, 1.0);
					// net volume
					this.Quantities.CalculateNetStandardVolume(VolumeGrossObserved,
																				PercentBSW,
																				VolumeCorrectionFactorForPressureAndTemperature,
																				TempNetVolume);

					if (TempNetVolume.Value == null)
						return;

					if ((double)TempNetVolume.Value - (double)VolumeNetStandard.Value < valueRangeCheck &&
						(double)TempNetVolume.Value - (double)VolumeNetStandard.Value > -valueRangeCheck)
					{
						if ((double)TempNetVolume.Value - (double)VolumeNetStandard.Value < 0.0)
								LevelProduct.Value = SetLevelValueBasedonUnits(LevelProduct, true);
						else
								LevelProduct.Value = SetLevelValueBasedonUnits(LevelProduct, false);
						return;
					}

					if (((double)TempNetVolume.Value - (double)VolumeNetStandard.Value) > 0.0)
						dvalueAmount *= -1;

					while (bExit == false)
					{
						LevelProduct.Value = (double)LevelProduct.Value + dvalueAmount;

						if (this.StrapTable.StrapVolumeCalculation(LevelProduct, VolumeStrapProduct) == false)
						{
								return;
						}
						this.Quantities.CalculateGrossObserverdVolume(VolumeRoofCorrection,
																						VolumeStrapProduct,
																						VolumeBottom,
																						TankShellCorrection,
																						VolumeGrossObserved);

						this.Quantities.CalculateNetStandardVolume(VolumeGrossObserved,
																					PercentBSW,
																					VolumeCorrectionFactorForPressureAndTemperature,
																					TempNetVolume);

						if (TempNetVolume.Value == null)
						{
								return;
						}
						if ((double)TempNetVolume.Value - (double)VolumeNetStandard.Value < valueRangeCheck &&
								(double)TempNetVolume.Value - (double)VolumeNetStandard.Value > -valueRangeCheck)
						{
								if ((double)TempNetVolume.Value - (double)VolumeNetStandard.Value < 0.0)
									LevelProduct.Value = SetLevelValueBasedonUnits(LevelProduct, true);
								else
									LevelProduct.Value = SetLevelValueBasedonUnits(LevelProduct, false);
								return;
						}
						else if (dvalueAmount < 0.0 &&
								(double)TempNetVolume.Value - (double)VolumeNetStandard.Value < 0.0)
						{
								LevelProduct.Value = SetLevelValueBasedonUnits(LevelProduct, true);
								return;
						}
						else if (dvalueAmount > 0.0 &&
								(double)TempNetVolume.Value - (double)VolumeNetStandard.Value > 0.0)
						{
								LevelProduct.Value = SetLevelValueBasedonUnits(LevelProduct, false);
								return;
						}
						else if ((double)LevelProduct.Value >= LevelProduct.Maximum)
						{
								LevelProduct.Value = LevelProduct.Maximum;
								return;
						}
						else if ((double)LevelProduct.Value <= LevelProduct.Minimum)
						{
								LevelProduct.Value = LevelProduct.Minimum;
								return;
						}
						else
						{
								if (LevelProduct.Units != EngineeringUnit.FmlFtIn16Th &&
									LevelProduct.Units != EngineeringUnit.FmlFtIn8Th)
								{
									if (LevelProduct.DecimalPlaces > 3)
									{
										if (Math.Abs((double)TempNetVolume.Value - (double)VolumeNetStandard.Value) > onePercentValue)
										{
												dvalueAmount = 0.001;
										}
										else
										{
												dvalueAmount = SetLevelValueAmount(LevelProduct);
										}
									}
								}
						}
					}
				}

		} // end CheckandDonetvolumeLevelCalculation

		private void CheckandDomassLevelCalculation(PointTag LevelProduct,
														PointTag VolumeStrapProduct,
														PointTag VolumeRoofCorrection,
														PointTag VolumeBottom,
														PointTag TankShellCorrection,
														PointTag VolumeGrossObserved,
														PointTag PercentBSW,
														PointTag VolumeCorrectionFactorForPressureAndTemperature,
														PointTag VolumeNetStandard,
														PointTag DensityStandard,
														PointTag DensityStandardInAir,
														PointTag Mass)

		{
				if (timeStampistheGreatest(Mass) == false)
					return;

				// figure out what the associated Solids level is for the selected volume
				if (Mass.Value == null)
					return;

				// get the percentage of the value based on the range
				double percentageRange = GetPercentageBasedonMinandMax(Mass);
				double onePercentValue = GetOnePercentValue(Mass);

				double dvalueAmount = SetLevelValueAmount(LevelProduct);
				double valueRangeCheck = onePercentValue / 100.0;

				if (percentageRange == 1.0)
					LevelProduct.Value = LevelProduct.Maximum;
				else if (percentageRange == 0.0)
					LevelProduct.Value = LevelProduct.Minimum;
				else
				{
					// set the level value to approxomately what it should be based on the percentage
					LevelProduct.Value = (double)((LevelProduct.Maximum - LevelProduct.Minimum) * percentageRange);

					bool bExit = false;
					PointTag TempMass = (PointTag)Mass.Clone();
					TempMass.InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated;
					SetTagStatusToGood(TempMass);

					// make the initial call to the strap module to determine the direction
					// strap volume
					this.StrapTable.StrapVolumeCalculation(LevelProduct, VolumeStrapProduct);
					if (VolumeStrapProduct.Value == null)
						return;

					SetTagStatusToGood(VolumeStrapProduct);

					// gross volume
					CheckandInitializeVariable(VolumeRoofCorrection, 0.0);
					CheckandInitializeVariable(VolumeBottom, 0.0);
					CheckandInitializeVariable(TankShellCorrection, 1.0);
					this.Quantities.CalculateGrossObserverdVolume(VolumeRoofCorrection,
																					VolumeStrapProduct,
																					VolumeBottom,
																					TankShellCorrection,
																					VolumeGrossObserved);

					if (VolumeGrossObserved.Value == null)
						return;

					SetTagStatusToGood(VolumeGrossObserved);

					// net volume
					CheckandInitializeVariable(PercentBSW, 0.0);
					CheckandInitializeVariable(VolumeCorrectionFactorForPressureAndTemperature, 1.0);
					this.Quantities.CalculateNetStandardVolume(VolumeGrossObserved,
																				PercentBSW,
																				VolumeCorrectionFactorForPressureAndTemperature,
																				VolumeNetStandard);

					if (VolumeNetStandard.Value == null)
						return;

					SetTagStatusToGood(VolumeNetStandard);

					// mass
					this.Quantities.CalculateLiquidMass(VolumeGrossObserved,
																	VolumeNetStandard,
																	DensityStandard,
																	DensityStandardInAir,
																	TempMass);
					if (TempMass.Value == null)
						return;

					SetTagStatusToGood(TempMass);

					if ((double)TempMass.Value - (double)Mass.Value < valueRangeCheck &&
						(double)TempMass.Value - (double)Mass.Value > -valueRangeCheck)
					{
						if ((double)TempMass.Value - (double)Mass.Value < 0.0)
								LevelProduct.Value = SetLevelValueBasedonUnits(LevelProduct, true);
						else
								LevelProduct.Value = SetLevelValueBasedonUnits(LevelProduct, false);
						return;
					}

					if (((double)TempMass.Value - (double)Mass.Value) > 0.0)
						dvalueAmount *= -1;

					while (bExit == false)
					{
						LevelProduct.Value = (double)LevelProduct.Value + dvalueAmount;

						if (this.StrapTable.StrapVolumeCalculation(LevelProduct, VolumeStrapProduct) == false)
						{
								return;
						}
						if (VolumeStrapProduct.Value == null)
								return;

						SetTagStatusToGood(VolumeStrapProduct);
						this.Quantities.CalculateGrossObserverdVolume(VolumeRoofCorrection,
																						VolumeStrapProduct,
																						VolumeBottom,
																						TankShellCorrection,
																						VolumeGrossObserved);

						if (VolumeGrossObserved.Value == null)
								return;

						SetTagStatusToGood(VolumeGrossObserved);

						this.Quantities.CalculateNetStandardVolume(VolumeGrossObserved,
																					PercentBSW,
																					VolumeCorrectionFactorForPressureAndTemperature,
																					VolumeNetStandard);
						if (VolumeNetStandard.Value == null)
								return;

						SetTagStatusToGood(VolumeNetStandard);

						this.Quantities.CalculateLiquidMass(VolumeGrossObserved,
																		VolumeNetStandard,
																		DensityStandard,
																		DensityStandardInAir,
																		TempMass);
						if (TempMass.Value == null)
						{
								return;
						}

						SetTagStatusToGood(TempMass);

						if ((double)TempMass.Value - (double)Mass.Value < valueRangeCheck &&
								(double)TempMass.Value - (double)Mass.Value > -valueRangeCheck)
						{
								if ((double)TempMass.Value - (double)Mass.Value < 0.0)
									LevelProduct.Value = SetLevelValueBasedonUnits(LevelProduct, true);
								else
									LevelProduct.Value = SetLevelValueBasedonUnits(LevelProduct, false);
								return;
						}
						else if (dvalueAmount < 0.0 &&
								(double)TempMass.Value - (double)Mass.Value < 0.0)
						{
								LevelProduct.Value = SetLevelValueBasedonUnits(LevelProduct, true);
								return;
						}
						else if (dvalueAmount > 0.0 &&
								(double)TempMass.Value - (double)Mass.Value > 0.0)
						{
								LevelProduct.Value = SetLevelValueBasedonUnits(LevelProduct, false);
								return;
						}
						else if ((double)LevelProduct.Value >= LevelProduct.Maximum)
						{
								LevelProduct.Value = LevelProduct.Maximum;
								return;
						}
						else if ((double)LevelProduct.Value <= LevelProduct.Minimum)
						{
								LevelProduct.Value = LevelProduct.Minimum;
								return;
						}
						else
						{
								if (LevelProduct.Units != EngineeringUnit.FmlFtIn16Th &&
									LevelProduct.Units != EngineeringUnit.FmlFtIn8Th)
								{
									if (LevelProduct.DecimalPlaces > 3)
									{
										if (Math.Abs((double)TempMass.Value - (double)Mass.Value) > onePercentValue)
										{
												dvalueAmount = 0.001;
										}
										else
										{
												dvalueAmount = SetLevelValueAmount(LevelProduct);
										}
									}
								}
						}
					}
				}

		} // end CheckandDomassLevelCalculation

		private void SetTagStatusToGood(PointTag tagToSet)
		{
				if (tagToSet.Value == null)
					return;

				tagToSet.Status = StatusCodes.Good;
		}

		private void CheckandInitializeVariable(PointTag pointTag, double valuetoSet)
		{
				if (pointTag.Value == null)
				{
					pointTag.Value = valuetoSet;
					pointTag.Status = StatusCodes.Good;
				}
		}
		// this function will scale or lock the returned value to the nearest 16 or 8 inch if the level units are set
		private double SetLevelValueBasedonUnits(PointTag LevelProduct, bool addtoValue)
		{
				double returnValue = (double)LevelProduct.Value;
				double tempCalcValue = 0.0;

				if (LevelProduct.Units == EngineeringUnit.FmlFtIn16Th)
				{
					// scale to the 16 value
					tempCalcValue = (returnValue % ((1.0 / 12.0) / 16.0));
					if (addtoValue)
						tempCalcValue = ((1.0 / 12.0) / 16.0) - tempCalcValue;
				}
				else if (LevelProduct.Units == EngineeringUnit.FmlFtIn8Th)
				{
					// scale to the 8 value
					tempCalcValue = (returnValue % ((1.0 / 12.0) / 8.0));
					if (addtoValue)
						tempCalcValue = ((1.0 / 12.0) / 8.0) - tempCalcValue;
				}
				else
				{
					// if decimal just truncate at the decimals
					int numDecimals = 1;

					switch (LevelProduct.DecimalPlaces)
					{
						case (0):
								numDecimals = 1;
								break;
						case (1):
								numDecimals = 10;
								break;
						case (2):
								numDecimals = 100;
								break;
						case (3):
								numDecimals = 1000;
								break;
						case (4):
								numDecimals = 10000;
								break;
						default:
								numDecimals = 100000;
								break;
					}

					returnValue = (double)(Math.Truncate(returnValue * numDecimals) / numDecimals);
				}
				if (addtoValue)
					returnValue += tempCalcValue;
				else
					returnValue -= tempCalcValue;

				if (returnValue > LevelProduct.Maximum)
					returnValue = LevelProduct.Maximum;
				else if (returnValue < LevelProduct.Minimum)
					returnValue = LevelProduct.Minimum;

				return returnValue;
		}

		// this function sets the amount to increment/decrement the level by when back calculating
		// it is based on level units first then level units number of decimals
		private double SetLevelValueAmount(PointTag Level)
		{
				double returnValue = 0.001;
				if (Level.Units == EngineeringUnit.FmlFtIn16Th)
				{
					returnValue = 0.005;
				}
				else if (Level.Units == EngineeringUnit.FmlFtIn8Th)
				{
					returnValue = 0.01;
				}
				else
				{
					switch (Level.DecimalPlaces)
					{
						case (0):
								returnValue = 1.0;
								break;
						case (1):
								returnValue = 0.1;
								break;
						case (2):
								returnValue = 0.01;
								break;
						case (3):
								returnValue = 0.001;
								break;
						case (4):
								returnValue = 0.0001;
								break;
						default:
								returnValue = 0.00001;
								break;
					}
				}
				return returnValue;
		}
	}

}
