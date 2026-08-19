using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.Constants;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.Exceptions;
using FMBusinessObjects.UtilityObjects;
using FMBusinessServices.DataAccessLayer;
using FMBusinessServices.InternalClasses;
using FMBusinessServices.InternalInterfaces;
using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.ServiceModel;
using Varec.CommonComponents.EngineeringUnitsLibrary;

namespace FMBusinessServices.ServiceClasses
{
    [ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class LeakTestsClass : ILeakTests
	{
		private readonly ConsolidatedDAClass consolidatedDA;

		public const string IDS_PASSED = "Test Passed";
		public const string IDS_FAILED = "Test Failed";
		public const string IDS_NOT_ENOUGH_DATA = "Not enough quiet time samples";
		public const string IDS_NOT_APPLICABLE = "N/A (Not a certified gauge)";

		public LeakTestsClass()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
		}

		public bool CleanupLeakReportData(SecurityClass security, Guid LeakReportId)
		{
			LeakReportClass leakReport = new LeakReportClass { LeakReportId = LeakReportId };

			bool result = this.consolidatedDA.ExecuteQuery(security, leakReport.PurgeSQL) > 0;

			return result;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public LeakDetectionError Run(SecurityClass security, Point point, LeakAnalysisType leakAnalysisType, LeakAnalysisMethod leakAnalysisMethod, DateTimeOffset start, DateTimeOffset end, ref LeakAnalysisResult leakAnalysisResult)
		{
			LeakDetectionError uierror = LeakDetectionError.None;

			if (leakAnalysisResult.LeakRecordId == Guid.Empty)
			{
				leakAnalysisResult.LeakRecordId = Guid.NewGuid();
			}

			SitesClass sites = new SitesClass();
			SiteClass pointSite = sites.Get(security, point.SiteGuid, false);

			IPointTagArchiveDatabase pointTagArchiveDatabase = new PointTagArchiveDatabase();

			DateTimeOffset sampleTime;
			DateTimeOffset? lastTime = null;
			DateTimeOffset? lastSampleTime = null;
			TimeSpan codts;
			DateTimeOffset? descriptionTime;
			double danalysisvolmin = leakAnalysisResult.MinValue;
			double danalysisvolmax = leakAnalysisResult.MaxValue;
			double danalysistempmin = leakAnalysisResult.MinTemperature;
			double danalysistempmax = leakAnalysisResult.MaxTemperature;
			double danalysisdeltatemp = leakAnalysisResult.DeltaTemperature;
			double daverage = 0, dtotal = 0, dtotalvol = 0;
			double dminval = 0, dmaxval = 0, dmintemp = 0, dmaxtemp = 0;
			double dstartvolume = 0, dstarttemp = 0, dcalculatedvolume = 0;
			double dlastlevel = 0, dlastcalculatedvol = 0, dconvertedvolume = 0;
			double dsamplequiettime = 0, dtotalQuiettime = 0, dtotalsamplequiettime = 0;
			double dsampleleakvol = 0;
			double dslope = 0, dintercept = 0;
			double dquiettimeratefactor = 0;       // factor * certification rate = quiet time
			double dquiettimerate = 0;
			double dsamplerate = 0, dsampletime = 0;
			double dminissuewaitperiod = 0;        // 10 min
			double dmintotalquiettime = 0;         // 48 hrs (2880 min)
			int nminnumsamples = 0;                //	5 min samples = 60 min, 10 min samples = 120 min
			int nnumsamples = 0;
			double minimumFillFraction = 0, dminfillvolume = 0;
			bool buseissuewaitperiod = false;
			double dissuewaittime = 0;
			EngineeringUnit currentVolUnits;
			EngineeringUnit currentLevelUnits;
			EngineeringUnit currentPressureUnits;
			EngineeringUnit currentDensityUnits;
			EngineeringUnit currentTemperatureUnits;
			bool bprocessingsample = false;
			List<QuietTimeSample> currentQuietTimeList = new List<QuietTimeSample>();
			List<QuietTimeSample> masterQuietTimeList = new List<QuietTimeSample>();
			List<QuietTimeTotals> quietTimeTotals = new List<QuietTimeTotals>();
			List<MasterSample> masterList = new List<MasterSample>();
			string dumpFileBaseName = "LeakDump";
			bool busedumpfile = false;

			Guid productLevelTagGuid;
			Guid temperatureTagGuid;
			Guid densityTagGuid;
			Guid volumeNetTagGuid;
			Guid unroundedVolumeNetTagGuid;
			Guid waterLevelTagGuid;
			Guid pressureBottomTagGuid;
			//Guid hydroPressureVolumeTagGuid;
			int currentPosition;
			int? productLevelTagPosition = null;
			int? temperatureTagPosition = null;
			int? densityTagPosition = null;
			int? volumeNetTagPosition = null;
			int? unroundedVolumeNetTagPosition = null;
			int? waterLevelTagPosition = null;
			int? pressureBottomTagPosition = null;
			List<TrendArchiveDataElement> productLevelData;
			List<TrendArchiveDataElement> temperatureData;
			List<TrendArchiveDataElement> densityData;
			List<TrendArchiveDataElement> volumeNetData;
			List<TrendArchiveDataElement> unroundedVolumeNetData;
			List<TrendArchiveDataElement> waterLevelData;
			List<TrendArchiveDataElement> pressureBottomData;
			List<Guid> tagList = new List<Guid>();
			SortedList<DateTimeOffset, ConsolidatedTagData> detectionSampleList = new SortedList<DateTimeOffset, ConsolidatedTagData>();
			double dPreviousVolume = 0;

			// get the tag guids for the specific tags for the tested point based on the well known guids for the tags
			// Because we can only compare well known guids in the values of the tag dictionary, just iterate them all
			// and get the ones we need in one pass.  May look a little clunky, but any other method requires multiple loops,
			// whether done explicitly or implicitly inside different find calls
			currentPosition = 0;
			foreach (Guid tagKey in point.Tags.Keys)
			{
				PointTag currentTag = point.Tags[tagKey];
				if (currentTag == null)
				{
					continue;
				}
				else if (currentTag.WellKnownIdentityGuid == Guids.LevelProductGuid)
				{
					productLevelTagGuid = currentTag.PointTagGuid;
					tagList.Add(productLevelTagGuid);
					productLevelTagPosition = currentPosition++;
				}
				else if (currentTag.WellKnownIdentityGuid == Guids.TemperatureProductGuid)
				{
					temperatureTagGuid = currentTag.PointTagGuid;
					tagList.Add(temperatureTagGuid);
					temperatureTagPosition = currentPosition++;
				}
				else if (currentTag.WellKnownIdentityGuid == Guids.DensityProductStandardGuid)
				{
					densityTagGuid = currentTag.PointTagGuid;
					tagList.Add(densityTagGuid);
					densityTagPosition = currentPosition++;
				}
				else if (currentTag.WellKnownIdentityGuid == Guids.VolumeNetStandardGuid)
				{
					volumeNetTagGuid = currentTag.PointTagGuid;
					tagList.Add(volumeNetTagGuid);
					volumeNetTagPosition = currentPosition++;
				}
				else if (currentTag.WellKnownIdentityGuid == Guids.VolumeNetStandardUnroundedGuid)
				{
					unroundedVolumeNetTagGuid = currentTag.PointTagGuid;
					tagList.Add(unroundedVolumeNetTagGuid);
					unroundedVolumeNetTagPosition = currentPosition++;
				}
				else if (currentTag.WellKnownIdentityGuid == Guids.LevelWaterGuid)
				{
					waterLevelTagGuid = currentTag.PointTagGuid;
					tagList.Add(waterLevelTagGuid);
					waterLevelTagPosition = currentPosition++;
				}
				else if (currentTag.WellKnownIdentityGuid == Guids.PressureBottomGuid)
				{
					pressureBottomTagGuid = currentTag.PointTagGuid;
					tagList.Add(pressureBottomTagGuid);
					pressureBottomTagPosition = currentPosition++;
				}
			}

			List<List<TrendArchiveDataElement>> trendArchiveDataElements = pointTagArchiveDatabase.GetLeakArchiveData(security, tagList, start, end);
			productLevelData = productLevelTagPosition.HasValue ? trendArchiveDataElements[productLevelTagPosition.Value] : new List<TrendArchiveDataElement>();
			temperatureData = temperatureTagPosition.HasValue ? trendArchiveDataElements[temperatureTagPosition.Value] : new List<TrendArchiveDataElement>();
			densityData = densityTagPosition.HasValue ? trendArchiveDataElements[densityTagPosition.Value] : new List<TrendArchiveDataElement>();
			volumeNetData = volumeNetTagPosition.HasValue ? trendArchiveDataElements[volumeNetTagPosition.Value] : new List<TrendArchiveDataElement>();
			unroundedVolumeNetData = unroundedVolumeNetTagPosition.HasValue ? trendArchiveDataElements[unroundedVolumeNetTagPosition.Value] : new List<TrendArchiveDataElement>();
			waterLevelData = waterLevelTagPosition.HasValue ? trendArchiveDataElements[waterLevelTagPosition.Value] : new List<TrendArchiveDataElement>();
			pressureBottomData = pressureBottomTagPosition.HasValue ? trendArchiveDataElements[pressureBottomTagPosition.Value] : new List<TrendArchiveDataElement>();

			// Combine the archive elements
			// Start with product level
			// These will be all new
			foreach (TrendArchiveDataElement item in productLevelData)
			{
				ConsolidatedTagData leakSample = new ConsolidatedTagData
				{
					dataTime = item.ValueTimeStamp,
					productLevelStatus = item.ValueOpcStatus,
					productLevelUnit = (EngineeringUnit)item.EngineeringUnitsIndex
				};
				if (double.TryParse(item.Value, out double testLevel))
				{
					leakSample.productLevelValue = testLevel;
				}

				detectionSampleList.Add(item.ValueTimeStamp, leakSample);
			}

			// Now do temperature.
			// These may not be new; we may be updating an existing sample
			foreach (TrendArchiveDataElement item in temperatureData)
			{
				ConsolidatedTagData leakSample;
				if (detectionSampleList.ContainsKey(item.ValueTimeStamp))
				{
					leakSample = detectionSampleList[item.ValueTimeStamp];
				}
				else 
				{ 
					leakSample = new ConsolidatedTagData
					{
						dataTime = item.ValueTimeStamp,
					};

					detectionSampleList.Add(item.ValueTimeStamp, leakSample);
				}

				leakSample.temperatureStatus = item.ValueOpcStatus;
				leakSample.temperatureUnit = (EngineeringUnit)item.EngineeringUnitsIndex;
				if (double.TryParse(item.Value, out double testTemperature))
				{
					leakSample.temperatureValue = testTemperature;
				}
			}

			// Density is next.
			// These may not be new; we may be updating an existing sample
			foreach (TrendArchiveDataElement item in densityData)
			{
				ConsolidatedTagData leakSample;
				if (detectionSampleList.ContainsKey(item.ValueTimeStamp))
				{
					leakSample = detectionSampleList[item.ValueTimeStamp];
				}
				else
				{
					leakSample = new ConsolidatedTagData
					{
						dataTime = item.ValueTimeStamp,
					};

					detectionSampleList.Add(item.ValueTimeStamp, leakSample);
				}

				leakSample.densityStatus = item.ValueOpcStatus;
				leakSample.densityUnit = (EngineeringUnit)item.EngineeringUnitsIndex;
				if (double.TryParse(item.Value, out double testDensity))
				{
					leakSample.densityValue = testDensity;
				}
			}

			// Now do Volume Net.
			// These may not be new; we may be updating an existing sample
			foreach (TrendArchiveDataElement item in volumeNetData)
			{
				ConsolidatedTagData leakSample;
				if (detectionSampleList.ContainsKey(item.ValueTimeStamp))
				{
					leakSample = detectionSampleList[item.ValueTimeStamp];
				}
				else
				{
					leakSample = new ConsolidatedTagData
					{
						dataTime = item.ValueTimeStamp,
					};

					detectionSampleList.Add(item.ValueTimeStamp, leakSample);
				}

				leakSample.volumeNetStatus = item.ValueOpcStatus;
				leakSample.volumeNetUnit = (EngineeringUnit)item.EngineeringUnitsIndex;
				if (double.TryParse(item.Value, out double testVolumeNet))
				{
					leakSample.volumeNetValue = testVolumeNet;
				}
			}

			// Now do unroundedVolumeNet.
			// These may not be new; we may be updating an existing sample
			foreach (TrendArchiveDataElement item in unroundedVolumeNetData)
			{
				ConsolidatedTagData leakSample;
				if (detectionSampleList.ContainsKey(item.ValueTimeStamp))
				{
					leakSample = detectionSampleList[item.ValueTimeStamp];
				}
				else
				{
					leakSample = new ConsolidatedTagData
					{
						dataTime = item.ValueTimeStamp,
					};

					detectionSampleList.Add(item.ValueTimeStamp, leakSample);
				}

				leakSample.unroundedVolumeNetStatus = item.ValueOpcStatus;
				leakSample.unroundedVolumeNetUnit = (EngineeringUnit)item.EngineeringUnitsIndex;
				if (double.TryParse(item.Value, out double testUnroundedVolumeNet))
				{
					leakSample.unroundedVolumeNetValue = testUnroundedVolumeNet;
				}
			}

			// Now do water level.
			// These may not be new; we may be updating an existing sample
			foreach (TrendArchiveDataElement item in waterLevelData)
			{
				ConsolidatedTagData leakSample;
				if (detectionSampleList.ContainsKey(item.ValueTimeStamp))
				{
					leakSample = detectionSampleList[item.ValueTimeStamp];
				}
				else
				{
					leakSample = new ConsolidatedTagData
					{
						dataTime = item.ValueTimeStamp,
					};

					detectionSampleList.Add(item.ValueTimeStamp, leakSample);
				}

				leakSample.waterLevelStatus = item.ValueOpcStatus;
				leakSample.waterLevelUnit = (EngineeringUnit)item.EngineeringUnitsIndex;
				if (double.TryParse(item.Value, out double testWaterLevel))
				{
					leakSample.waterLevelValue = testWaterLevel;
				}
			}

			// Now do pressure bottom.
			// These may not be new; we may be updating an existing sample
			foreach (TrendArchiveDataElement item in pressureBottomData)
			{
				ConsolidatedTagData leakSample;
				if (detectionSampleList.ContainsKey(item.ValueTimeStamp))
				{
					leakSample = detectionSampleList[item.ValueTimeStamp];
				}
				else
				{
					leakSample = new ConsolidatedTagData
					{
						dataTime = item.ValueTimeStamp,
					};

					detectionSampleList.Add(item.ValueTimeStamp, leakSample);
				}

				leakSample.pressureBottomStatus = item.ValueOpcStatus;
				leakSample.pressureBottomUnit = (EngineeringUnit)item.EngineeringUnitsIndex;
				if (double.TryParse(item.Value, out double testPressureBottom))
				{
					leakSample.pressureBottomValue = testPressureBottom;
				}
			}

			// Due to the archive process only archiving on change and not at fixed interval, we have to synthesize
			// data by "smearing" unchanged data forwards in time to fill in missing data.  Missing data will be differentiated
			// from bad/no value data by the status; a missing data point will not have a status while bad data will have a status,
			// even though it may not have a value.
			// Note that this data semaring is potentially problematic, as we are synthesizing data for a leak detection report, not
			// just gathering data that was recorded from a gauge.
			ConsolidatedTagData previousSample = null;
			ConsolidatedTagData currentSample = null;
			foreach (DateTimeOffset currentSampleKey in detectionSampleList.Keys)
			{
				currentSample = detectionSampleList[currentSampleKey];
				if (previousSample != null)
				{
					// For a given trend, a present data element may have a null _value_, for example when the data source 
					// is unavailable or invalid, but it will always have a status and and engineering units.
					// If those values are missing in the consolidated tag data then that represents a time where the data
					// didn't change from previous (data is archived on change (and at UTC midnight)); in this case we need to
					// copy the three values (value, status, engineeringunit) from the previous sample.  As we're on a loop 
					// from the beginning to the end, we only need to one previous.  The status will be a reliable indicator.
					
					// Check and potentially drag forward Product Level
					if (!currentSample.productLevelStatus.HasValue)
					{
						currentSample.productLevelValue = previousSample.productLevelValue;
						currentSample.productLevelStatus = previousSample.productLevelStatus;
						currentSample.productLevelUnit = previousSample.productLevelUnit;
					}

					// Check and potentially drag forward temperature
					if (!currentSample.temperatureStatus.HasValue)
					{
						currentSample.temperatureValue = previousSample.temperatureValue;
						currentSample.temperatureStatus = previousSample.temperatureStatus;
						currentSample.temperatureUnit = previousSample.temperatureUnit;
					}

					// Check and potentially drag forward density
					if (!currentSample.densityStatus.HasValue)
					{
						currentSample.densityValue = previousSample.densityValue;
						currentSample.densityStatus = previousSample.densityStatus;
						currentSample.densityUnit = previousSample.densityUnit;
					}

					// Check and potentially drag forward volumeNet
					if (!currentSample.volumeNetStatus.HasValue)
					{
						currentSample.volumeNetValue = previousSample.volumeNetValue;
						currentSample.volumeNetStatus = previousSample.volumeNetStatus;
						currentSample.volumeNetUnit = previousSample.volumeNetUnit;
					}

					// Check and potentially drag forward unrounded volume net
					if (!currentSample.unroundedVolumeNetStatus.HasValue)
					{
						currentSample.unroundedVolumeNetValue = previousSample.unroundedVolumeNetValue;
						currentSample.unroundedVolumeNetStatus = previousSample.unroundedVolumeNetStatus;
						currentSample.unroundedVolumeNetUnit = previousSample.unroundedVolumeNetUnit;
					}

					// Check and potentially drag forward water level
					if (!currentSample.waterLevelStatus.HasValue)
					{
						currentSample.waterLevelValue = previousSample.waterLevelValue;
						currentSample.waterLevelStatus = previousSample.waterLevelStatus;
						currentSample.waterLevelUnit = previousSample.waterLevelUnit;
					}

					// Check and potentially drag forward pressure bottom
					if (!currentSample.pressureBottomStatus.HasValue)
					{
						currentSample.pressureBottomValue = previousSample.pressureBottomValue;
						currentSample.pressureBottomStatus = previousSample.pressureBottomStatus;
						currentSample.pressureBottomUnit = previousSample.pressureBottomUnit;
					}
				}

				previousSample = currentSample;
			}

			sampleTime = DateTimeOffset.Now;
			descriptionTime = DateTimeOffset.Now;
			lastTime = DateTimeOffset.Now;
			lastSampleTime = null;
			codts = TimeSpan.Zero;
			lastTime = DateTimeOffset.Now;
			currentVolUnits = EngineeringUnit.FmuNone;
			currentLevelUnits = EngineeringUnit.FmuNone;
			currentPressureUnits = EngineeringUnit.FmuNone;
			currentDensityUnits = EngineeringUnit.FmuNone;
			currentTemperatureUnits = EngineeringUnit.FmuNone;

			// Tank Calculator variables
			PointServiceManager pointServiceManager = new PointServiceManager();


			// Barton method adjustments
			double dBartonOffset = 0.0;
			double dStartSpecificGravity = 1.0;

			bool fBartonOffsetSet = false;
			double dLevelFromHydro = 0.0;

			// Try to get the site-wide settings
			// default should be 8
			dquiettimeratefactor = pointSite.LeakDetectionQuietTimeFactor;
			dquiettimerate = leakAnalysisResult.CertRate * dquiettimeratefactor;

			// default should be 1440
			dmintotalquiettime = pointSite.LeakDetectionMinQuietTime;

			// Use the quiet time from the gauge configuration when greater than zero. This fixes CSI #6010. (IGO 15-Oct-2008)
			if (leakAnalysisResult.MinGaugeTestTime > 0)
			{
				// MinGaugeTestTime convert from hours to minutes
				dmintotalquiettime = leakAnalysisResult.MinGaugeTestTime * 60;
			}

			// default should be 6
			nminnumsamples = pointSite.LeakDetectionMinQuietSamples;

			// default should be false
			buseissuewaitperiod = pointSite.LeakDetectionUseMinWait;

			if (buseissuewaitperiod)
			{
				// default should be 10 - Minimum Issue Wait Period
				dminissuewaitperiod = 10;
			}

			minimumFillFraction = leakAnalysisResult.MinimumFillPercentage * 0.01;  // convert from percentage to decimal fraction
			// Calculate minimum fill volume.
			dminfillvolume = danalysisvolmax * minimumFillFraction;

			try
			{
				//Prepare tank calculator for Barton Method Analysis only
				// Also set query filter 
				switch (leakAnalysisMethod)
				{
					case LeakAnalysisMethod.Hydrostatic:
						break;
					case LeakAnalysisMethod.UnroundedNet:
						break;
					case LeakAnalysisMethod.NetVolume:
						break;
				}

				foreach (DateTimeOffset psetDateTime in detectionSampleList.Keys)
				{
					ConsolidatedTagData pset = detectionSampleList[psetDateTime];
					switch (leakAnalysisMethod)
					{
						case LeakAnalysisMethod.Hydrostatic:
							if (!pset.pressureBottomValue.HasValue ||
								 !pset.temperatureValue.HasValue ||
								 (!fBartonOffsetSet && (!pset.productLevelValue.HasValue ||
																!pset.densityValue.HasValue)))
							{
								// Point has insufficient data to include in leak test
								continue;
							}
							break;
						case LeakAnalysisMethod.UnroundedNet:
							if (!pset.productLevelValue.HasValue ||
								!pset.unroundedVolumeNetValue.HasValue ||
								!pset.temperatureValue.HasValue)
							{
								// Point has insufficient data to include in leak test
								continue;
							}
							break;
						case LeakAnalysisMethod.NetVolume:
							if (!pset.productLevelValue.HasValue ||
								!pset.volumeNetValue.HasValue ||
								!pset.temperatureValue.HasValue)
							{
								// Point has insufficient data to include in leak test
								continue;
							}
							break;
					}

					sampleTime = pset.dataTime;

					if (lastSampleTime.HasValue)
					{
						codts = sampleTime - lastSampleTime.Value;
						dsampletime = codts.TotalMinutes;
					}

					lastSampleTime = sampleTime;

					// Get the volumetric units needed for conversion.
					currentVolUnits = pset.volumeNetUnit.GetValueOrDefault();
					currentLevelUnits = pset.productLevelUnit.GetValueOrDefault();
					currentDensityUnits = pset.densityUnit.GetValueOrDefault();
					currentPressureUnits = pset.pressureBottomUnit.GetValueOrDefault();
					currentTemperatureUnits = pset.temperatureUnit.GetValueOrDefault();

					double? currentSamplePressureH2O = null;
					if(pset.pressureBottomValue.HasValue)
					{
                        currentSamplePressureH2O = EngineeringUnits.Convert(pset.pressureBottomValue.Value, currentPressureUnits, EngineeringUnit.FmpInH2O, 0);
                    }

                    // Use different volumes types based on which analysis method is chosen
                    // by the user (01-Aug-2002 IGO)
                    switch (leakAnalysisMethod)
					{
						case LeakAnalysisMethod.NetVolume:
							{
								dcalculatedvolume = pset.volumeNetValue.Value;

								// Convert the current volumetric units to gallons
								if (EngineeringUnit.FmvUsGal != currentVolUnits)
								{
									dconvertedvolume = EngineeringUnits.Convert(dcalculatedvolume, currentVolUnits, EngineeringUnit.FmvUsGal, 0);
									dcalculatedvolume = dconvertedvolume;
								}
								dlastlevel = pset.productLevelValue.Value;
								dlastcalculatedvol = dcalculatedvolume;
							}
							break;

						case LeakAnalysisMethod.UnroundedNet:
							{
								dcalculatedvolume = pset.unroundedVolumeNetValue.Value;

								// Convert the current volumetric units to gallons
								if (EngineeringUnit.FmvUsGal != currentVolUnits)
								{
									dconvertedvolume = EngineeringUnits.Convert(dcalculatedvolume, currentVolUnits, EngineeringUnit.FmvUsGal, 0);
									dcalculatedvolume = dconvertedvolume;
								}
								dlastlevel = pset.productLevelValue.Value;
								dlastcalculatedvol = dcalculatedvolume;
							}
							break;

						case LeakAnalysisMethod.Hydrostatic:
							{
								if (!fBartonOffsetSet)
								{
									// Determine the offset of the Barton gauge from the bottom of the tank
									// From Barton, the offset(inches) is defined as level(inches) - (hydro(inches H20)/Density(sg))
									double dTankLevelInches;
									double dHydroValueForOffset;
									double dTemperatureCelcius;

									dTankLevelInches = EngineeringUnits.Convert(pset.productLevelValue.Value, currentLevelUnits, EngineeringUnit.FmlInch, 0);
									dTemperatureCelcius = EngineeringUnits.Convert(pset.temperatureValue.Value, currentTemperatureUnits, EngineeringUnit.FmtDegC, 0);
									dStartSpecificGravity = EngineeringUnits.Convert(pset.densityValue.Value, currentDensityUnits, EngineeringUnit.FmdSpGrav, dTemperatureCelcius);
									dHydroValueForOffset = EngineeringUnits.Convert(pset.pressureBottomValue.Value, currentPressureUnits, EngineeringUnit.FmpInH2O, 0);

									dBartonOffset = dTankLevelInches - (dHydroValueForOffset / dStartSpecificGravity);

									// Convert Barton Offset to tank units
									dBartonOffset = EngineeringUnits.Convert(dBartonOffset, EngineeringUnit.FmlInch, currentLevelUnits, 0);
									fBartonOffsetSet = true;
								}

								// Convert Barton pressure to a level(product) in inches, then to level(product) in tank units;
								double dHydroValue;
								dHydroValue = EngineeringUnits.Convert(pset.pressureBottomValue.Value, currentPressureUnits, EngineeringUnit.FmpInH2O, 0);

								dLevelFromHydro = dHydroValue / dStartSpecificGravity;
								dLevelFromHydro = EngineeringUnits.Convert(dLevelFromHydro, EngineeringUnit.FmlInch, currentLevelUnits, 0);
								dLevelFromHydro += dBartonOffset; // Add gauge offset

								// Feed to tank calculator to go from level to gross volume
								// Using gross volume as Barton actually measures mass above it, which should be independent of temperature.
								PointTag inputLevelTag = (PointTag)point.Tags.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.LevelProductGuid).Clone();
								PointTag inputVolumeTag = (PointTag)point.Tags.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.VolumeGrossObservedGuid).Clone();
								inputLevelTag.Value = dLevelFromHydro;
								List<PointTag> inputTags = new List<PointTag> { inputLevelTag, inputVolumeTag };
								List<PointTag> outputTags = pointServiceManager.RunPointCalculatorX(security, point.PointGuid, inputTags);

								// output tags should be in the same order as input tags
								try
								{
									dcalculatedvolume = (double)outputTags[1].Value;
								}
								catch (Exception ex)
								{
									// couldn't calculate this poihnt
									_ = ex;
									continue;
								}

								// Convert the current volumetric units to gallons
								if (EngineeringUnit.FmvUsGal != currentVolUnits)
								{
									dconvertedvolume = EngineeringUnits.Convert(dcalculatedvolume, currentVolUnits, EngineeringUnit.FmvUsGal, 0);
									dcalculatedvolume = dconvertedvolume;
								}
								dlastlevel = pset.pressureBottomValue.Value;
								dlastcalculatedvol = dcalculatedvolume;
							}
							break;
					}

					switch (leakAnalysisType)
					{
						case LeakAnalysisType.Continuous:
						case LeakAnalysisType.RealTime:
							{
								// Initialize the start volume and temperature if it has not been (IGO 12-Sep-2002)
								if (0 == dstartvolume)
								{
									dstartvolume = dcalculatedvolume;
									dstarttemp = pset.temperatureValue.GetValueOrDefault();
									dPreviousVolume = dcalculatedvolume;
								}

								if (0 != dsampletime) // protect against unlikely, but possible, divide by zero
								{
									dsamplerate = (dcalculatedvolume - dPreviousVolume) / (dsampletime / 60); //determine instantaneous rate
								}

								// Try to find quiet time starting with the current net volume.
								// If the current sample rate (gal/hr) is less than of the calculated
								// quiet time sample rate and the calculated volume and the temperature
								// are within range add it as a valid sample.
								if ((0 != dstartvolume) &&
									(dquiettimerate >= Math.Abs(dsamplerate)) &&
									(danalysisvolmin < dcalculatedvolume) &&
									(danalysisvolmax > dcalculatedvolume) &&
									(danalysistempmin < pset.temperatureValue.Value) &&
									(danalysistempmax > pset.temperatureValue.Value) &&
									(dminfillvolume < dcalculatedvolume) &&   // This fixes CSI #6011. (IGO 16-Oct-2008)
									(danalysisdeltatemp > Math.Abs(pset.temperatureValue.Value - dstarttemp)))
								{
									// If issue wait time is configured to be used throw away atleast
									// the amount of the minimum time, which means removing the previously
									// added sample and reseting the start volume

									// Check the the direction the volume is moving and only apply wait period to
									// product entering the tank. This fixes CSI #6013. (IGO 15-Oct-2008)
									//Eric Simmons - 2/17/2012
									if (buseissuewaitperiod && (dminissuewaitperiod > dissuewaittime))
									{
										dissuewaittime += dsampletime;
										if (nnumsamples > 0)
										{
											nnumsamples--;
											currentQuietTimeList.RemoveAt(currentQuietTimeList.FindLastIndex(x => true));

											//	Update the removed item in the master sample list
											MasterSample pcms = masterList.FindLast(x => true);
											pcms.Reason = "Not Used - Removed due to minimum issue wait period.";

											dstartvolume = dcalculatedvolume;
											dstarttemp = pset.temperatureValue.Value;
										}
									}

									bprocessingsample = true;
									nnumsamples++;
									currentQuietTimeList.Add(new QuietTimeSample { Volume = dcalculatedvolume,
																									ProductLevel = (LeakAnalysisMethod.NetVolume == leakAnalysisMethod || LeakAnalysisMethod.UnroundedNet == leakAnalysisMethod) ? pset.productLevelValue.Value : pset.pressureBottomValue.Value,     // use pressure value for barton (IGO 17-Nov-2003)
																									WaterLevel = pset.waterLevelValue.GetValueOrDefault(),
																									Temperature = pset.temperatureValue.Value,
																									TimeStamp = sampleTime,
																									PressureH2O = currentSamplePressureH2O
																					});
									// Add good sample to master list
									masterList.Add(new MasterSample {
																				Density = pset.densityValue,
																				Volume = dcalculatedvolume,
																				ProductLevel = (LeakAnalysisMethod.NetVolume == leakAnalysisMethod || LeakAnalysisMethod.UnroundedNet == leakAnalysisMethod) ? pset.productLevelValue.Value : pset.pressureBottomValue.Value,     // use pressure value for barton (IGO 17-Nov-2003)
																				CalcProductLevel = (LeakAnalysisMethod.NetVolume == leakAnalysisMethod || LeakAnalysisMethod.UnroundedNet == leakAnalysisMethod) ? pset.productLevelValue.Value : dLevelFromHydro,     // use pressure value for barton (IGO 17-Nov-2003)
																				WaterLevel = pset.waterLevelValue.GetValueOrDefault(),
																				Temperature = pset.temperatureValue.GetValueOrDefault(),
																				PressureH2O = currentSamplePressureH2O,
																				TimeStamp = sampleTime,
																				Reason = string.Empty});
									dPreviousVolume = dcalculatedvolume;
								}
								else
								{
									string csreason = string.Empty;
									bprocessingsample = false;
									fBartonOffsetSet = false; // Barton offset must be recalculated at beginning of each quiet time period

									if (dquiettimerate <= Math.Abs(dsamplerate))
									{
										csreason = "Not Used - Sample change rate greater than maximum allowed.";
									}
									if (danalysisvolmin > dcalculatedvolume)
									{
										csreason = "Not Used - Volume less than the minimum allowed.";
									}
									if (danalysisvolmax < dcalculatedvolume)
									{
										csreason = "Not Used - Volume greater than the maximum allowed."; 
									}
									if (danalysistempmin > pset.temperatureValue.Value)
									{
										csreason = "Not Used - Temperature less than the minimum allowed.";
									}
									if (danalysistempmax < pset.temperatureValue.Value)
									{
										csreason = "Not Used - Temperature greater than the maximum allowed.";
									}
									if (danalysisdeltatemp < Math.Abs(pset.temperatureValue.Value - dstarttemp))
									{
										csreason = "Not Used - Temperature delta change greater the maximum allowed.";
									}
									if (dminfillvolume > dcalculatedvolume)   // This fixes CSI #6011. (IGO 16-Oct-2008)
									{
										csreason = "Not Used - Volume less than the minimum percentage allowed.";
									}

									// Add bad sample to master list and state the resons for rejection.
									masterList.Add(new MasterSample{ Density = pset.densityValue,
																				Volume = dcalculatedvolume,
																				ProductLevel = (LeakAnalysisMethod.NetVolume == leakAnalysisMethod || LeakAnalysisMethod.UnroundedNet == leakAnalysisMethod) ? pset.productLevelValue.Value : pset.pressureBottomValue.Value,      // use pressure value for barton (IGO 17-Nov-2003)
																				CalcProductLevel = (LeakAnalysisMethod.NetVolume == leakAnalysisMethod || LeakAnalysisMethod.UnroundedNet == leakAnalysisMethod) ? pset.productLevelValue.Value : dLevelFromHydro,     // use pressure value for barton (IGO 17-Nov-2003)
																				WaterLevel = pset.waterLevelValue.GetValueOrDefault(),
																				Temperature = pset.temperatureValue.GetValueOrDefault(),
																				PressureH2O = currentSamplePressureH2O,
																				TimeStamp = sampleTime,
																				Reason = csreason });
								}

								// If EOF or end of currnet sample set process the data accordingly
								if (!bprocessingsample || (psetDateTime == detectionSampleList.Keys.Last()))
								{
									// If the total number of samples is not met throw out the whole thing
									// otherwize add it to the master quiet time list
									if (nminnumsamples <= nnumsamples)
									{
										foreach (QuietTimeSample pcqt in currentQuietTimeList)
										{
											// compute the total time for the current set of samples
											if (lastTime.HasValue)
											{
												codts = pcqt.TimeStamp - lastTime.Value;
												dsamplequiettime += codts.TotalMinutes;
											}
											lastTime = pcqt.TimeStamp;
											dtotal += pcqt.Volume;
											if (0 == dminval)
											{
												dmaxval = dminval = pcqt.Volume;
												dmaxtemp = dmintemp = pcqt.Temperature;
											}
											else
											{
												dminval = Math.Min(dminval, pcqt.Volume);
												dmaxval = Math.Max(dmaxval, pcqt.Volume);
												dmintemp = Math.Min(dmintemp, pcqt.Temperature);
												dmaxtemp = Math.Max(dmaxtemp, pcqt.Temperature);
											}
											masterQuietTimeList.Add(new QuietTimeSample{ Volume = pcqt.Volume,
																										ProductLevel = pcqt.ProductLevel,
																										WaterLevel = pcqt.WaterLevel,
																										Temperature = pcqt.Temperature,
																										TimeStamp = pcqt.TimeStamp,
																										PressureH2O = pcqt.PressureH2O
																						});
										}
										lastTime = null;
										daverage = dtotal / nnumsamples;

										// Calculate the leak rate for the sample just added to the master list
										double dsigmax = 0, dsigmay = 0, dsigmaxy = 0, dsigmax2 = 0;
										double dsigma2x = 0, denom = 0, dvalue = 0;
										long tvalue, tinittime = 0;
										DateTimeOffset? lastSpanSampleTime = null;

										// Calculate the deltas needed for slope / intercept 
										foreach (QuietTimeSample pcqt in currentQuietTimeList)
										{
											if (0 == tinittime)
											{
												tinittime = pcqt.TimeStamp.ToUnixTimeSeconds();
											}

											tvalue = pcqt.TimeStamp.ToUnixTimeSeconds();

											tvalue -= tinittime;
											dvalue = pcqt.Volume - daverage;

											dsigmax += tvalue;
											dsigmay += dvalue;
											dsigmaxy += tvalue * dvalue;
											dsigmax2 += tvalue * (double)tvalue;

											lastSpanSampleTime = pcqt.TimeStamp;
										}

										dsigma2x = dsigmax * dsigmax;
										denom = (nnumsamples * dsigmax2) - dsigma2x;

										if (0 != denom)
										{
											dintercept = ((dsigmay * dsigmax2) - (dsigmax * dsigmaxy)) / denom;
											dslope = ((nnumsamples * dsigmaxy) - (dsigmax * dsigmay)) / denom;

											dsampleleakvol = dslope * (60 * dsamplequiettime);
											dtotalQuiettime += dsamplequiettime;
											quietTimeTotals.Add(new QuietTimeTotals { SampleLeakSlope = dslope,
																										SampleLeakVolume = dsampleleakvol,
																										SampleTestTime = dsamplequiettime,
																										LastTimeStamp = lastSpanSampleTime.GetValueOrDefault()
																									});
										}
										else
										{
											uierror = LeakDetectionError.InvalidIndex;
										}
									}
									else  // Mark the records in the master list as not used
									{
										if (0 != masterList.Count)
										{
											int ncount = 0;
											for (int pos = masterList.Count - 1; pos >= 0 && ncount < nnumsamples; ncount++)
											{
												MasterSample pcms = masterList[pos];
												pcms.Reason = "Not Used - Removed due to minimum number samples not met.";
												pos--;
											}
										}
									}

									// Cleanup all data
									dissuewaittime = 0;
									dsamplequiettime = 0;
									nnumsamples = 0;
									dstartvolume = dcalculatedvolume;
									dstarttemp = pset.temperatureValue.Value;
									lastTime = null;
									currentQuietTimeList.Clear();

									// Start a new list
									nnumsamples++;
									currentQuietTimeList.Add(new QuietTimeSample{ Volume = dcalculatedvolume,
																									ProductLevel = (LeakAnalysisMethod.NetVolume == leakAnalysisMethod || LeakAnalysisMethod.UnroundedNet == leakAnalysisMethod) ? pset.productLevelValue.Value : pset.pressureBottomValue.Value,     // use pressure value for barton (IGO 17-Nov-2003)
																									WaterLevel = pset.waterLevelValue.Value,
																									Temperature = pset.temperatureValue.Value,
																									TimeStamp = sampleTime,
																									PressureH2O = currentSamplePressureH2O
																								});
									// Move to the next record to process if not end of file
									dPreviousVolume = dcalculatedvolume;
								}
							}
							break;
						case LeakAnalysisType.Static:
							{
								if (dminfillvolume < dcalculatedvolume)
								{
									// Add good sample to master list
									masterList.Add(new MasterSample { Density = pset.densityValue,
																					Volume = dcalculatedvolume,
																					ProductLevel = (LeakAnalysisMethod.NetVolume == leakAnalysisMethod || LeakAnalysisMethod.UnroundedNet == leakAnalysisMethod) ? pset.productLevelValue.Value : pset.pressureBottomValue.Value,      // use pressure value for barton (IGO 17-Nov-2003)
																					CalcProductLevel = (LeakAnalysisMethod.NetVolume == leakAnalysisMethod || LeakAnalysisMethod.UnroundedNet == leakAnalysisMethod) ? pset.productLevelValue.Value : dLevelFromHydro,     // use pressure value for barton (IGO 17-Nov-2003)
																					WaterLevel = pset.waterLevelValue.GetValueOrDefault(),
																					Temperature = pset.temperatureValue.GetValueOrDefault(),
																					PressureH2O = currentSamplePressureH2O,
																					TimeStamp = sampleTime,
																					Reason = string.Empty
																				});
								}
								else  // otherwise add reason for not using sample (IGO 02-Sep-2005)
								{
									masterList.Add(new MasterSample{ Density = pset.densityValue,
																						Volume = dcalculatedvolume,
																						ProductLevel = (LeakAnalysisMethod.NetVolume == leakAnalysisMethod || LeakAnalysisMethod.UnroundedNet == leakAnalysisMethod) ? pset.productLevelValue.Value : pset.pressureBottomValue.Value,      // use pressure value for barton (IGO 17-Nov-2003)
																						CalcProductLevel = (LeakAnalysisMethod.NetVolume == leakAnalysisMethod || LeakAnalysisMethod.UnroundedNet == leakAnalysisMethod) ? pset.productLevelValue.Value : dLevelFromHydro,     // use pressure value for barton (IGO 17-Nov-2003)
																						WaterLevel = pset.waterLevelValue.GetValueOrDefault(),
																						Temperature = pset.temperatureValue.GetValueOrDefault(),
																						TimeStamp = sampleTime,
																						Reason = "Not Used - Volume less than the minimum percentage allowed."});
								}

								// If EOF process the data accordingly
								if (psetDateTime == detectionSampleList.Keys.Last())
								{
									dsamplequiettime = 0;
									lastTime = null;
									foreach (MasterSample pcms in masterList)
									{
										// only add sample if there is no reason to skip (IGO 02-Sep-2005)
										if (string.IsNullOrEmpty(pcms.Reason))
										{
											// compute the total time for the current set of samples
											if (lastTime.HasValue)
											{
												codts = pcms.TimeStamp - lastTime.Value;
												dsamplequiettime += codts.TotalMinutes;
											}
											lastTime = pcms.TimeStamp;
											dtotal += pcms.Volume;
											if (0 == dminval)
											{
												dmaxval = dminval = pcms.Volume;
												dmaxtemp = dmintemp = pcms.Temperature;
											}
											else
											{
												dminval = Math.Min(dminval, pcms.Volume);
												dmaxval = Math.Max(dmaxval, pcms.Volume);
												dmintemp = Math.Min(dmintemp, pcms.Temperature);
												dmaxtemp = Math.Max(dmaxtemp, pcms.Temperature);
											}
											nnumsamples++;
											masterQuietTimeList.Add(new QuietTimeSample { Volume = pcms.Volume,
																							ProductLevel = pcms.CalcProductLevel,
																							WaterLevel = pcms.WaterLevel,
																							Temperature = pcms.Temperature,
																							TimeStamp = pcms.TimeStamp,
																							PressureH2O = pcms.PressureH2O
                                            });
										}
										else
										{
											lastTime = pcms.TimeStamp;
										}
									}
									lastTime = null;
									daverage = dtotal / nnumsamples;

									// Calculate the leak rate for the sample 
									double dsigmax = 0, dsigmay = 0, dsigmaxy = 0, dsigmax2 = 0;
									double dsigma2x = 0, denom = 0, dvalue = 0;
									long tvalue, tinittime = 0;
									DateTimeOffset? lastSampleSpanTime = null;

									// Calculate the deltas needed for slope / intercept 
									foreach (MasterSample pcms in masterList)
									{
										if (0 == tinittime)
										{
											tinittime = pcms.TimeStamp.ToUnixTimeSeconds();
										}

										tvalue = pcms.TimeStamp.ToUnixTimeSeconds();

										tvalue -= tinittime;
										dvalue = pcms.Volume - daverage;

										dsigmax += tvalue;
										dsigmay += dvalue;
										dsigmaxy += tvalue * dvalue;
										dsigmax2 += tvalue * (double)tvalue;

										lastSampleSpanTime = pcms.TimeStamp;
									}

									dsigma2x = dsigmax * dsigmax;
									denom = (nnumsamples * dsigmax2) - dsigma2x;

									if (0 != denom)
									{
										dintercept = ((dsigmay * dsigmax2) - (dsigmax * dsigmaxy)) / denom;
										dslope = ((nnumsamples * dsigmaxy) - (dsigmax * dsigmay)) / denom;

										dsampleleakvol = dslope * (60 * dsamplequiettime);
										dtotalQuiettime += dsamplequiettime;
										quietTimeTotals.Add(new QuietTimeTotals
										{
											SampleLeakSlope = dslope,
											SampleLeakVolume = dsampleleakvol,
											SampleTestTime = dsamplequiettime,
											LastTimeStamp = lastSampleSpanTime.GetValueOrDefault()
										});
									}
									else
									{
										uierror = LeakDetectionError.InvalidIndex;
									}
								}
								break;
							}
					}

					//descriptionTime = pset->m_Description_Time_stamp;
				}

				// if total time of all samples is greater than or equal to the minimum total
				// time see if the set is valid 
				// Eric Simmons 3-12-2012
				// Added expression to check if the masterquiettimelist collection has at least 1 item in the list.
				// This is to resolve Bug 27954
				if (dmintotalquiettime <= dtotalQuiettime && masterQuietTimeList.Count > 0)
				{
					// Calculate the average of all the volumes
					foreach (QuietTimeTotals pcqtt in quietTimeTotals)
					{
						dtotalvol += pcqtt.SampleLeakVolume;
						dtotalsamplequiettime += pcqtt.SampleTestTime;
					}
					dslope = dtotalvol / dtotalsamplequiettime;
					leakAnalysisResult.LeakRate = dslope * 60;

					// Check Result for Pass/Fail
					if (Math.Abs(leakAnalysisResult.LeakRate) > Math.Abs(leakAnalysisResult.LeakThreshold) && Math.Abs(leakAnalysisResult.LeakThreshold) >0)
					{
						leakAnalysisResult.AnalysisStatus |= LeakDetectionError.LeakrateToHigh;
						leakAnalysisResult.AnalysisStatus |= LeakDetectionError.TestFailed;
					}

					// Check Result for Over Certification Leak Rate
					if (Math.Abs(leakAnalysisResult.LeakRate) > Math.Abs(leakAnalysisResult.CertRate) && Math.Abs(leakAnalysisResult.CertRate) > 0)
					{
						leakAnalysisResult.AnalysisStatus |= LeakDetectionError.OverCertRate;
						leakAnalysisResult.AnalysisStatus |= LeakDetectionError.TestFailed;
					}

					leakAnalysisResult.StartTime = start;
					leakAnalysisResult.StopTime = end;
					leakAnalysisResult.ReportTime = (long)(end - start).Ticks;
					leakAnalysisResult.UsableSampleTime = dtotalsamplequiettime;
					leakAnalysisResult.NumSamples = (uint)masterQuietTimeList.Count;
					leakAnalysisResult.MinValue = dminval;
					leakAnalysisResult.MaxValue = dmaxval;
					leakAnalysisResult.MinTemperature = dmintemp;
					leakAnalysisResult.MaxTemperature = dmaxtemp;
					leakAnalysisResult.GraphTemperatureDelta = leakAnalysisResult.MaxTemperature - leakAnalysisResult.MinTemperature;
					leakAnalysisResult.GraphMinValue = dminval;
					leakAnalysisResult.GraphMaxValue = dmaxval;

					// if the tank doesn't move at all the data is invalid, per Steve J. (IGO 24-Nov-2003)
					if (0 == leakAnalysisResult.LeakRate &&
						 leakAnalysisResult.MinValue == leakAnalysisResult.MaxValue)
					{
						leakAnalysisResult.AnalysisStatus |= LeakDetectionError.NoMovement;
						leakAnalysisResult.AnalysisStatus |= LeakDetectionError.TestFailed;
					}

					// Only check delta temperature on static test. It is already used to throw out samples on 
					// continuous and real time tests. This fixes CSI #6012. (IGO 24-Oct-2008)
					if (LeakAnalysisType.Static == leakAnalysisType)
					{
						// Check to see if delta temperature is out of range (IGO 06-Apr-2004)
						if ((Math.Abs(leakAnalysisResult.GraphTemperatureDelta) > Math.Abs(leakAnalysisResult.DeltaTemperature)) && leakAnalysisResult.DeltaTemperature > 0)
						{
							leakAnalysisResult.AnalysisStatus |= LeakDetectionError.OverDeltaTemp;
							leakAnalysisResult.AnalysisStatus |= LeakDetectionError.TestFailed;
						}
					}

                    // Get the beginning and ending product and water levels. 
                    var pressureUnits = ((PointTag)point.Tags.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.PressureBottomGuid))?.Units ?? EngineeringUnit.FmuNone;
                    QuietTimeSample pcqt;
					pcqt = masterQuietTimeList[0];
					leakAnalysisResult.LevelStart = pcqt.ProductLevel;
					leakAnalysisResult.WaterLevelStart = pcqt.WaterLevel;
					if(pcqt.PressureH2O.HasValue && EngineeringUnit.FmuNone != pressureUnits)
					{
                        leakAnalysisResult.PressureStart = EngineeringUnits.Convert(pcqt.PressureH2O.Value, EngineeringUnit.FmpInH2O, pressureUnits, 0);
                    }

					pcqt = masterQuietTimeList[masterQuietTimeList.Count - 1];
					leakAnalysisResult.LevelEnd = pcqt.ProductLevel;
					leakAnalysisResult.WaterLevelEnd = pcqt.WaterLevel;
                    if (pcqt.PressureH2O.HasValue && EngineeringUnit.FmuNone != pressureUnits)
                    {
                        leakAnalysisResult.PressureEnd = EngineeringUnits.Convert(pcqt.PressureH2O.Value, EngineeringUnit.FmpInH2O, pressureUnits, 0);
                    }
                    uierror = LeakDetectionError.None;
				}
				else
				{
					uierror = LeakDetectionError.NotEnoughData;
				}

				// Write to dump file
				if (busedumpfile)
				{
                    try { 
						StreamWriter dumpfile;
						string csheader, csline;
						// Can get a write protected system directory
						string dumpfilePath = Path.Combine(Directory.GetCurrentDirectory(), dumpFileBaseName + "-" + leakAnalysisResult.LeakRecordId.ToString() + ".csv");
						using (dumpfile = new StreamWriter(dumpfilePath))
						{
							csheader = "MinQuietime,ActualQuieTime,QuietTimeListCount,GaugeMaxDeltaTemp";
							dumpfile.WriteLine(csheader);
							csline = string.Format("{0},{1},{2},{3}", dmintotalquiettime, dtotalQuiettime, masterQuietTimeList.Count, leakAnalysisResult.DeltaTemperature);
							dumpfile.WriteLine(csline);
							// Write Header
							csheader = "Time,AnalysisValue,Level,Calc Level,Temperature,WaterLevel,Density,Offset,Status";
							dumpfile.WriteLine(csheader);
							foreach (MasterSample pcms in masterList)
							{
								csline = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}",
																pcms.TimeStamp,
																pcms.Volume,
																pcms.ProductLevel,
																pcms.CalcProductLevel,
																pcms.Temperature,
																pcms.WaterLevel,
																pcms.Density,
																dBartonOffset,
																pcms.Reason);
								dumpfile.WriteLine(csline);
							}
						}
					}catch(Exception ex)
                    {
						_ = ex;
						uierror = LeakDetectionError.ConnectionFailed;
					}
				}

				// Get leak detection and vessel settings
				LeakDetectionSettings leakDetectionSettings = this.GetLeakDetectionSettings(point);
				Vessel vessel = this.GetVesselSettings(point);

				// Write data to the fm_leak_report_data table first
				LeakReportClass leakRecord = new LeakReportClass();

				string cstemp;
				DateTimeOffset codtnow = TimeConverter.Now(pointSite);

				leakRecord.LeakReportId = leakAnalysisResult.LeakRecordId;
				leakRecord.PointId = point.PointId;
				leakRecord.PointDescription = Points.BulkPointDescription == point.Description ? string.Empty : point.Description;
				leakRecord.SiteID = pointSite.ID;
				leakRecord.TestType = leakAnalysisType.ToString();
				leakRecord.TestMethod = leakAnalysisMethod.ToString();
				if (LeakDetectionError.NotEnoughData == uierror)
				{
					cstemp = IDS_NOT_ENOUGH_DATA;
					leakRecord.LeakRate = 0;
					leakRecord.LevelStart = 0;
					leakRecord.LevelEnd = 0;
					leakRecord.MinTemp = 0;
					leakRecord.MaxTemp = 0;
					leakRecord.MinVolume = 0;
					leakRecord.MaxVolume = 0;

					//Eric Simmons 2-15-2012
					//Added to support change request 23693
					leakRecord.WaterLevelStart = 0.0;
					leakRecord.WaterLevelStop = 0.0;
				}
				else
				{
					leakRecord.LeakRate = leakAnalysisResult.LeakRate;
					leakRecord.LevelStart = leakAnalysisResult.LevelStart;
					leakRecord.LevelEnd = leakAnalysisResult.LevelEnd;
					leakRecord.PressureStart = leakAnalysisResult.PressureStart;
					leakRecord.PressureEnd = leakAnalysisResult.PressureEnd;
					leakRecord.MinTemp = leakAnalysisResult.MinTemperature;
					leakRecord.MaxTemp = leakAnalysisResult.MaxTemperature;
					leakRecord.MinVolume = leakAnalysisResult.MinValue;
					leakRecord.MaxVolume = leakAnalysisResult.MaxValue;

					//Eric Simmons 2-15-2012
					//Added to support change request 23693
					leakRecord.WaterLevelStart = leakAnalysisResult.WaterLevelStart;
					leakRecord.WaterLevelStop = leakAnalysisResult.WaterLevelEnd;

					// Non-certified gauges are always not applicable for status (IGO 06-Apr-2004)
					if (0 == leakAnalysisResult.MinGaugeTestTime &&
							0 == leakAnalysisResult.CertRate &&
							0 == leakAnalysisResult.LeakThreshold)
					{
						leakAnalysisResult.AnalysisStatus &= ~LeakDetectionError.TestFailed; // For non-certified, "Failed test" is meaningess.
						cstemp = IDS_NOT_APPLICABLE;
					}
					else
					{
						cstemp = (leakAnalysisResult.AnalysisStatus & LeakDetectionError.TestFailed) == LeakDetectionError.TestFailed ? IDS_FAILED : IDS_PASSED;
					}
				}
				leakRecord.TestResult = cstemp;
				leakRecord.LeakThreshold = leakAnalysisResult.LeakThreshold;
				leakRecord.CertRate = leakAnalysisResult.CertRate;
				// Convert the start time to a TIMESTAMP_STRUCT
				leakRecord.StartTime = start;
				leakRecord.EndTime = end;
				leakRecord.TimeStamp = codtnow;
				leakRecord.DateInstalled = vessel?.TankInstallationDate != DateTime.MinValue ? vessel?.TankInstallationDate :null;
				leakRecord.TankGauge = leakDetectionSettings.GaugeType;
				leakRecord.LeakDetectionSystem = "FuelsManager";
				leakRecord.TankLengthOrHeight = vessel?.TankHeight?.Value;
				leakRecord.TankRadius = vessel?.TankRadius?.Value;
				leakRecord.TankVolume = vessel?.TankVolume?.Value;
				leakRecord.LiningMaterial = vessel?.TankLiningMaterial ?? string.Empty;
				leakRecord.ConstructionMaterial = vessel?.TankMaterial.ToString() ?? string.Empty;
				leakRecord.CathodicProtection = vessel?.CathodicProtectionSupported;
				leakRecord.OverfillProtection = vessel?.OverfillProtectionSupported;
				leakRecord.SpillProtection = vessel?.SpillProtectionSupported;

				leakRecord.WaterLevelUnits = ((PointTag)point.Tags.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.LevelWaterGuid))?.Units ?? EngineeringUnit.FmuNone;
				leakRecord.VolumeUnits = ((PointTag)point.Tags.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.VolumeNetStandardGuid))?.Units ?? EngineeringUnit.FmuNone;
				leakRecord.TemperatureUnits = ((PointTag)point.Tags.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.TemperatureProductGuid))?.Units ?? EngineeringUnit.FmuNone;
				leakRecord.ProducLevelUnits = ((PointTag)point.Tags.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.LevelProductGuid))?.Units ?? EngineeringUnit.FmuNone;
                leakRecord.PressureUnits = ((PointTag)point.Tags.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.PressureBottomGuid))?.Units ?? EngineeringUnit.FmuNone;
                leakRecord.LeakRateUnits = ((PointTag)point.Tags.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.LeakRateGuid))?.Units ?? EngineeringUnit.FmuNone;
				leakRecord.LeakRatePrecision = ((PointTag)point.Tags.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.LeakRateGuid))?.DecimalPlaces ?? 3;
				leakRecord.VolumePrecision = ((PointTag)point.Tags.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.VolumeNetStandardGuid))?.DecimalPlaces ?? 3;
				leakRecord.TemperaturePrecision = ((PointTag)point.Tags.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.TemperatureProductGuid))?.DecimalPlaces ?? 3;
				leakRecord.ProductLevelPrecision = ((PointTag)point.Tags.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.LevelProductGuid))?.DecimalPlaces ?? 3;
                leakRecord.PressurePrecision = ((PointTag)point.Tags.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.PressureBottomGuid))?.DecimalPlaces ?? 3;
                leakRecord.WaterLevelPrecision = ((PointTag)point.Tags.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.LevelWaterGuid))?.DecimalPlaces ?? 3;

				// used for tank / vessle height and radius
				leakRecord.BasePointLevelUnits = point.LevelUnit;
				// used for tank /vessle volume
				leakRecord.BasePointVolumeUnits = point.VolumeUnit;

				switch (leakAnalysisMethod)
                {
                    case LeakAnalysisMethod.NetVolume:

						leakRecord.VolumeUnits = ((PointTag)point.Tags.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.VolumeNetStandardGuid))?.Units ?? EngineeringUnit.FmuNone;
						break;
					case LeakAnalysisMethod.UnroundedNet:
						
						leakRecord.VolumeUnits = ((PointTag)point.Tags.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.VolumeNetStandardUnroundedGuid))?.Units ?? EngineeringUnit.FmuNone;
                        break;

                    case LeakAnalysisMethod.Hydrostatic:

						if (!fBartonOffsetSet)
                        {
							if (leakRecord.TemperatureUnits != EngineeringUnit.FmuNone)
							{
								leakRecord.MinTemp = EngineeringUnits.Convert((double)leakRecord.MinTemp, EngineeringUnit.FmtDegC, leakRecord.TemperatureUnits, 0);
								leakRecord.MaxTemp = EngineeringUnits.Convert((double)leakRecord.MaxTemp, EngineeringUnit.FmtDegC, leakRecord.TemperatureUnits, 0);
							}

							if (leakRecord.ProducLevelUnits != EngineeringUnit.FmuNone)
							{
								leakRecord.LevelStart = EngineeringUnits.Convert((double)leakRecord.LevelStart, EngineeringUnit.FmlInch, leakRecord.ProducLevelUnits, 0);
								leakRecord.LevelEnd = EngineeringUnits.Convert((double)leakRecord.LevelEnd, EngineeringUnit.FmlInch, leakRecord.ProducLevelUnits, 0);
							}
						}

                        break;
                }

				// Convert the volumetric units to tank units
				if (EngineeringUnit.FmuNone != leakRecord.VolumeUnits)
				{
					leakRecord.MinVolume = EngineeringUnits.Convert((double)leakRecord.MinVolume, EngineeringUnit.FmvUsGal, leakRecord.VolumeUnits, 0);
					leakRecord.MaxVolume = EngineeringUnits.Convert((double)leakRecord.MaxVolume, EngineeringUnit.FmvUsGal, leakRecord.VolumeUnits, 0);
				}

				// Convert the leak rate units to tank units
				if (EngineeringUnit.FmuNone != leakRecord.LeakRateUnits)
				{
					leakRecord.LeakRate = EngineeringUnits.Convert((double)leakRecord.LeakRate, EngineeringUnit.FmvfGph, leakRecord.LeakRateUnits, 0);
					leakRecord.CertRate = EngineeringUnits.Convert((double)leakRecord.CertRate, EngineeringUnit.FmvfGph, leakRecord.LeakRateUnits, 0);
					leakRecord.LeakThreshold = EngineeringUnits.Convert((double)leakRecord.LeakThreshold, EngineeringUnit.FmvfGph, leakRecord.LeakRateUnits, 0);
				}

				// Copy back converted units for display
				leakAnalysisResult.VolumeUnits = leakRecord.VolumeUnits;
				leakAnalysisResult.TemperatureUnits = leakRecord.TemperatureUnits;
				leakAnalysisResult.LeakRateUnits = leakRecord.LeakRateUnits;

                leakAnalysisResult.MinValue = leakRecord.MinVolume.GetValueOrDefault();
				leakAnalysisResult.MaxValue = leakRecord.MaxVolume.GetValueOrDefault();
				leakAnalysisResult.MaxTemperature = leakRecord.MaxTemp.GetValueOrDefault();
				leakAnalysisResult.MinTemperature = leakRecord.MinTemp.GetValueOrDefault();
				leakAnalysisResult.GraphTemperatureDelta = leakRecord.MaxTemp.GetValueOrDefault() - leakRecord.MinTemp.GetValueOrDefault();
				leakAnalysisResult.LeakRate = leakRecord.LeakRate.GetValueOrDefault();
				leakAnalysisResult.TestResult = leakRecord.TestResult;
				leakAnalysisResult.LeakRatePrecision = leakRecord.LeakRatePrecision;
				leakAnalysisResult.VolumePrecision = leakRecord.VolumePrecision;
				leakAnalysisResult.TemperaturePrecision = leakRecord.TemperaturePrecision;

				try
				{
					SqlCommand cmd = new SqlCommand();
					leakRecord.InsertSQL(cmd);
					consolidatedDA.ExecuteQuery(security, cmd);
				}
				catch (ConsolidatedDAException ex)
				{
					_ = ex;
					uierror = LeakDetectionError.ConnectionFailed;
				}

				if (LeakDetectionError.ConnectionFailed != uierror && masterList.Count > 0)
				{
					try
					{
						LeakGraphDBI leakGraphDBI = new LeakGraphDBI();
						leakGraphDBI.Save(security, leakRecord.LeakReportId, masterList);
					}
					catch (ConsolidatedDAException ex)
					{
						_ = ex;
						uierror = LeakDetectionError.ConnectionFailed;
					}
				}

				// Delete all allocated CObList data
				masterList.Clear();
				currentQuietTimeList.Clear();
				masterQuietTimeList.Clear();
				quietTimeTotals.Clear();
			}
			catch (DbException e)
			{
				string lpString;
				lpString = e.Message;
				//ReportEvent(g_hEventSource, EVENTLOG_ERROR_TYPE, 0, LD_SQL_ERROR, NULL, 1, 0, &lpString, NULL);
				uierror = LeakDetectionError.SqlError;
			}
			catch (OutOfMemoryException pe)
			{
				string lpString;
				lpString = pe.Message;
				//ReportEvent(g_hEventSource, EVENTLOG_ERROR_TYPE, 0, LD_MEM_ERROR, NULL, 1, 0, &lpString, NULL);
				uierror = LeakDetectionError.NotEnoughMemory;
			}
			catch (Exception pe)
			{
				string lpString = pe.Message;
				//ReportEvent(g_hEventSource, EVENTLOG_ERROR_TYPE, 0, LD_CONNECTION_ERROR, NULL, 1, 0, &lpString, NULL);
				uierror = LeakDetectionError.ConnectionFailed;
			}

			SetStatusMessage(leakAnalysisResult);
			return uierror;
		}

        public void SetStatusMessage(LeakAnalysisResult leakAnalysisResult)
        {
            if (leakAnalysisResult != null)
            {
				//AddMessageIfApplicable(leakAnalysisResult, LeakDetectionError.None);
				//AddMessageIfApplicable(leakAnalysisResult, LeakDetectionError.TestFailed);
				//AddMessageIfApplicable(leakAnalysisResult, LeakDetectionError.NotEnoughData);
				AddMessageIfApplicable(leakAnalysisResult, LeakDetectionError.InvalidIndex);
				AddMessageIfApplicable(leakAnalysisResult, LeakDetectionError.LeakrateToHigh);
				AddMessageIfApplicable(leakAnalysisResult, LeakDetectionError.OverCertRate);
				AddMessageIfApplicable(leakAnalysisResult, LeakDetectionError.ConnectionFailed);
				AddMessageIfApplicable(leakAnalysisResult, LeakDetectionError.SqlError);
				AddMessageIfApplicable(leakAnalysisResult, LeakDetectionError.OverDeltaTemp);
				AddMessageIfApplicable(leakAnalysisResult, LeakDetectionError.NotEnoughMemory);
				AddMessageIfApplicable(leakAnalysisResult, LeakDetectionError.ArchiveAccessError);
				AddMessageIfApplicable(leakAnalysisResult, LeakDetectionError.NoMovement);
            }
        }

		private void AddMessageIfApplicable(LeakAnalysisResult leakAnalysisResult, LeakDetectionError error)
        {
			if ((leakAnalysisResult.AnalysisStatus & error) == error)
			{
				leakAnalysisResult.AnalysisStatusMessage.Add(LeakAnalysisResult.GetDispalyMessgae(error));
			}
		}

        private LeakDetectionSettings GetLeakDetectionSettings(Point point)
		{
			LeakDetectionSettings settings = null;
			PointProperty leakDetectionSettingsProperty = point.Properties.Values.SingleOrDefault(u => u.ValueTypeString == LeakDetectionSettings.LeakDetectionSettingsIdentifier);
			if (leakDetectionSettingsProperty != null)
			{
				settings = (LeakDetectionSettings)leakDetectionSettingsProperty.Value;
			}
			return settings;
		}

		private Vessel GetVesselSettings(Point point)
		{
			Vessel settings = null;
			PointProperty vesselSettingsProperty = point.Properties.Values.SingleOrDefault(u => u.ID == "Vessel");
			if (vesselSettingsProperty != null)
			{
				settings = (Vessel)vesselSettingsProperty.Value;
			}
			return settings;
		}
	}

	internal struct MasterSample
	{
		public double? Density { get; set; } // we really only need a density at the start of Hydrostatic
		public double Volume { get; set; }
		public double ProductLevel { get; set; }
		public double CalcProductLevel { get; set; }
		public double WaterLevel { get; set; }
		public double Temperature { get; set; }
        public double? PressureH2O { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
		public string Reason { get; set; }
	}

	internal struct QuietTimeSample
	{
		public double Volume { get; set; }
		public double ProductLevel { get; set; }
		public double WaterLevel { get; set; }
		public double Temperature { get; set; }
        public double? PressureH2O { get; set; }
        public DateTimeOffset TimeStamp { get; set;}
	}

	internal struct QuietTimeTotals
	{
		public double SampleLeakSlope { get; set; }
		public double SampleLeakVolume { get; set; }
		public double SampleTestTime { get; set; }
		public DateTimeOffset LastTimeStamp { get; set; }
	}

	internal class ConsolidatedTagData
	{
		public DateTimeOffset dataTime = DateTimeOffset.MinValue;
		public double? productLevelValue = null;
		public EngineeringUnit? productLevelUnit = null;
		public long? productLevelStatus = null;
		public double? temperatureValue = null;
		public EngineeringUnit? temperatureUnit = null;
		public long? temperatureStatus = null;
		public double? densityValue = null;
		public EngineeringUnit? densityUnit = null;
		public long? densityStatus = null;
		public double? volumeNetValue = null;
		public EngineeringUnit? volumeNetUnit = null;
		public long? volumeNetStatus = null;
		public double? unroundedVolumeNetValue = null;
		public EngineeringUnit? unroundedVolumeNetUnit = null;
		public long? unroundedVolumeNetStatus = null;
		public double? waterLevelValue = null;
		public EngineeringUnit? waterLevelUnit = null;
		public long? waterLevelStatus = null;
		public double? pressureBottomValue = null;
		public EngineeringUnit? pressureBottomUnit = null;
		public long? pressureBottomStatus = null;
	}
}
