namespace RateModules
{
	using System;
	using System.Collections.Generic;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;
	using FMPointCommon;
	using System.Configuration;
	using RateCalculationModules;
	using System.IO;
	using System.Text;
	using Opc.Ua;

	public class FMRateModule : FuelsManagerModule, IFuelsManagerModule
	{
		#region Properties
		public RateModuleSettings Settings { get; set; }
		#endregion

		#region Private data members
		private LeastSquaredQuadRegression leastSquareCalculator = null;
		private AveragingCalculation averagingCalculation = null;
		private List<double> intervals;
		private string directoryForFiles = string.Empty;
		private int daystoRetainRecords = 1;
		private bool logDataTrueorFalse = false;	// this is named this way so people know how to enter the value in the config file
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public FMRateModule() : base()
		{
			this.Init();
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method is the entry point for the client to calculate the rate.
		/// </summary>
		/// <param name="valueTag">The point tag to calculate a rate.</param>
		/// <param name="rateTag">The calculated rate tag.</param>
		/// <returns>Returns true if successful.</returns>
		public bool? RateCalculation(PointTag valueTag, PointTag rateTag)
		{
			if (rateTag.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
			rateTag.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
				return false;


			if (this.Settings.FlowCalculationType == "Least Squared")
				RateCalculationLeastSquared(ref valueTag, ref rateTag);
			else
				RateCalculationAveraging(ref valueTag, ref rateTag);

			return true;
		}

		/// <summary>
		/// this is the entry point for doing the least squared calculation
		/// </summary>
		/// <param name="valueTag">The point tag to calculate a rate.</param>
		/// <param name="rateTag">The calculated rate tag.</param>
		/// <returns>Returns true if successful.</returns>
		private bool? RateCalculationAveraging(ref PointTag valueTag, ref PointTag rateTag)
		{
			int returnValue = 0;
			double instantaneousRate = 0.0;
			if (this.PerformValidation(valueTag, rateTag) == false)
			{
				if (this.averagingCalculation != null)
					this.averagingCalculation = null;
				if (rateTag.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated)
				{
					rateTag.Value = null;
					rateTag.Status = StatusCodes.BadNoData;
					rateTag.ServerTimeStamp = DateTimeOffset.UtcNow;
					rateTag.SourceTimeStamp = DateTimeOffset.UtcNow;
				}
				return false;
			}

			// Check to see if this is the initial entry.
			if (this.averagingCalculation == null)
			{
				this.ResetToInitialState(valueTag, rateTag);
				return true;
			}

			// If the Timer expiration based on it being greater than the stale time
			// is true, that means there has been no updates and therefore a stoppage.
			returnValue = this.CheckTimerExpiration(valueTag, rateTag, ref instantaneousRate);
			if (returnValue == 1)
			{
				this.ResetToInitialState(valueTag, rateTag);
				return true;
			}
			else if (returnValue == 2)
			{
				double? returnedRateValue = null;
				// calculate the average rate
				returnedRateValue = this.averagingCalculation.CalculateRate(valueTag, rateTag);

				double? convertedDeadband = null;

				// Converted the deadband from the stored units of Meters/Minute to Millimeters/second. The reason
				// is that the instantaneous level is in millimeters per second.
				if (rateTag.EngineeringUnitsType == EngineeringUnitType.FmuVelocity)
				{
					var deadbandtagValue = System.Convert.ToDouble(this.Settings.Deadband);
					convertedDeadband = this.ConvertToNewUnit(deadbandtagValue, EngineeringUnit.FmvrMMin, rateTag.Units);
				}

				// Converted the deadband from the stored units of Meters cube/Minute to Gallons/Min. The reason
				// is that the instantaneous volume is in gallons per second.
				if (rateTag.EngineeringUnitsType == EngineeringUnitType.FmuVolflow)
				{
					var deadbandtagValue = System.Convert.ToDouble(this.Settings.Deadband);
					convertedDeadband = this.ConvertToNewUnit(deadbandtagValue, EngineeringUnit.FmvfM3Min, rateTag.Units);
				}

				if (returnedRateValue == null || convertedDeadband == null)
				{
					rateTag.Value = null;
					rateTag.Status = StatusCodes.BadNoData;
					rateTag.ServerTimeStamp = DateTimeOffset.UtcNow;
					rateTag.SourceTimeStamp = DateTimeOffset.UtcNow;
					return false;
				}

				double rateVluetoCheck = returnedRateValue.Value;
				if (returnedRateValue.Value < 0.0)
					rateVluetoCheck *= -1;


				if (convertedDeadband.Value != 0.0 && rateVluetoCheck < convertedDeadband.Value)
				{
					if ((double)rateTag.Value != 0.0 ||
						rateTag.Status != StatusCodes.Good)
					{
						rateTag.Value = 0.0;
						rateTag.Status = StatusCodes.Good;
						rateTag.ServerTimeStamp = DateTimeOffset.UtcNow;
						rateTag.SourceTimeStamp = DateTimeOffset.UtcNow;
					}
				}
				else
				{
					rateTag.Value = returnedRateValue;
					rateTag.Status = StatusCodes.Good;
					rateTag.ServerTimeStamp = DateTimeOffset.UtcNow;
					rateTag.SourceTimeStamp = DateTimeOffset.UtcNow;
				}

				this.SetRateModuleTimer(valueTag);

				return true;
			}

			// Set the rate module timer.
			this.SetRateModuleTimer(valueTag);

			return true;
		}

		/// <summary>
		/// this is the entry point for doing the least squared calculation
		/// </summary>
		/// <param name="valueTag">The point tag to calculate a rate.</param>
		/// <param name="rateTag">The calculated rate tag.</param>
		/// <returns>Returns true if successful.</returns>
		private bool? RateCalculationLeastSquared(ref PointTag valueTag, ref PointTag rateTag)
		{ 
			string logText = string.Empty;
			string logFileNameText = string.Empty;
			double tempRate = 0.0;
			// There are many validation that are initially performed and if there
			// are any failure, return.  The method will set the rate tag status.

			logText = valueTag.IdentityGuid.ToString() + ",";

			if (this.PerformValidation(valueTag, rateTag) == false)
			{
				if (this.leastSquareCalculator != null)
					this.leastSquareCalculator = null;
				return false;
			}

			// Check to see if this is the initial entry.
			if (this.leastSquareCalculator == null)
			{
				this.ResetToInitialState(valueTag, rateTag);
				return true;
			}

			// If the Timer expiration based on it being greater than the stale time
			// is true, that means there has been no updates and therefore a stoppage.
			if (this.CheckTimerExpiration(valueTag, rateTag, ref tempRate) > 0)
			{
				this.ResetToInitialState(valueTag, rateTag);
				return true;
			}

			// Set the rate module timer.
			this.SetRateModuleTimer(valueTag);

			// Convert the input tag value to a common unit for the calculations.
			double? convertedSourceValue = null;
			double? deadbandConvertedValue = null; ;

			if (valueTag.EngineeringUnitsType == EngineeringUnitType.FmuLength)
			{
				var tagValue = (double)valueTag.Value;
				convertedSourceValue = this.ConvertToNewUnit(tagValue, valueTag.Units, EngineeringUnit.FmlMm);
				var deadbandtagValue = System.Convert.ToDouble(this.Settings.Deadband);
				deadbandConvertedValue = this.ConvertToNewUnit(deadbandtagValue, valueTag.Units, EngineeringUnit.FmlMm);
			}

			if (valueTag.EngineeringUnitsType == EngineeringUnitType.FmuVolume)
			{
				var tagValue = (double) valueTag.Value;
				convertedSourceValue = this.ConvertToNewUnit(tagValue, valueTag.Units, EngineeringUnit.FmvUsGal);
				var deadbandtagValue = System.Convert.ToDouble(this.Settings.Deadband);
				deadbandConvertedValue = this.ConvertToNewUnit(deadbandtagValue, valueTag.Units, EngineeringUnit.FmvUsGal);
			}

			if (convertedSourceValue == null || deadbandConvertedValue == null)
			{
				rateTag.Value = null;
				rateTag.Status = StatusCodes.BadNoData;
            rateTag.ServerTimeStamp = DateTimeOffset.UtcNow;
            rateTag.SourceTimeStamp = DateTimeOffset.UtcNow;
            return false;
			}

			// Convert the source timestamp from nano seconds to milliseconds.
			//double sourceTimeInMilliSec = TimeSpan.FromTicks(valueTag.SourceTimeStamp.Ticks).TotalMilliseconds;
			double sourceTimeInMilliSec = valueTag.SourceTimeStamp.Ticks / 1000000.0;

			// We are only initializing the last time and value if there are no entries in the equation and 
			// the last time is zero.  This means the first time through.
			if (this.leastSquareCalculator.NumOfEntries == 0 && (long) this.leastSquareCalculator.LastValueTime == 0)
			{
				this.leastSquareCalculator.LastValueTime = sourceTimeInMilliSec;
				this.leastSquareCalculator.LastValueValue = convertedSourceValue.Value;

				return true;
			}

			// Do nothing if the time differential as not changed.
			if (this.leastSquareCalculator.HasTimeChanged(sourceTimeInMilliSec) == false)
			{
				// No changes.
				return true;
			}

			// Detect stoppage.
			bool hasStopped = this.StoppageDetected(sourceTimeInMilliSec, convertedSourceValue.Value, rateTag);
			if (hasStopped)
			{
				this.ResetToInitialState(valueTag, rateTag);
				return true;
			}

			// Get the instantaneous rate. The method will preform a time differential.
			double changeInTime = 0.0;
			double changeInValue = 0.0;

			double instantaneousRate = this.leastSquareCalculator.CalculateInstantaneousRate(sourceTimeInMilliSec, convertedSourceValue.Value,ref changeInTime,ref changeInValue);

			// Calculate the time span between the tag source time and the last known time.
			// Set the last time and value for the next change.
			double timeDifferential = Math.Abs(sourceTimeInMilliSec - this.leastSquareCalculator.LastValueTime);
			this.leastSquareCalculator.LastValueTime = sourceTimeInMilliSec;
			this.leastSquareCalculator.LastValueValue = convertedSourceValue.Value;

			// Do not perform Calculation of the instantaneous rate is less than or
			// equal to the deadband. Just return.
			//if (this.WithinDeadband(instantaneousRate, rateTag))
			//{
				//return true;
			//}

			// Time has changed, start process
			double ATerm = 0.0;
			double BTerm = 0.0;
			double CTerm = 0.0;
			double? sampleValue = this.leastSquareCalculator.GetPredictedY(timeDifferential,ref ATerm,ref BTerm,ref CTerm);

			// There could be no values in the array for the first prediction.
			if (sampleValue == null)
			{
				sampleValue = 0;
			}

			this.leastSquareCalculator.AddPoints(timeDifferential, instantaneousRate);

			// Convert the rate value to the units of the rate tag.  The rate is either in
			// millimeters per second or in US Gallons per second.
			double? rateValue = this.ConvertRateToRateTagUnits(sampleValue.Value, rateTag, valueTag.EngineeringUnitsType);

			// convert the deadband deadbandConvertedValue
			double? DeadbandValue = this.ConvertRateToRateTagUnits(deadbandConvertedValue.Value, rateTag, valueTag.EngineeringUnitsType);

			if (rateValue == null || DeadbandValue == null)
			{
				rateTag.Value = null;
				rateTag.Status = StatusCodes.BadNoData;
            rateTag.ServerTimeStamp = DateTimeOffset.UtcNow;
            rateTag.SourceTimeStamp = DateTimeOffset.UtcNow;

            try
				{
					logText += " null," + sampleValue.Value.ToString();
					logFileNameText = rateTag.ID.Replace(' ', '-');
					writeDataToLogFile(logFileNameText, logText);
				}
				catch
				{
					// do nothing
				}

				return false;
			}


			double dRateAmount = rateValue.Value;

			if (rateValue.Value < 0.0)
				dRateAmount *= -1;

			if (dRateAmount <= DeadbandValue.Value)
			{
				if ((double)rateTag.Value != 0.0 ||
					rateTag.Status != StatusCodes.Good)
				{
					rateTag.Value = rateValue;
					rateTag.Status = StatusCodes.Good;
					rateTag.ServerTimeStamp = DateTimeOffset.UtcNow;
					rateTag.SourceTimeStamp = DateTimeOffset.UtcNow;
				}
			}
			else
			{
				rateTag.Value = rateValue;
				rateTag.Status = StatusCodes.Good;
				rateTag.ServerTimeStamp = DateTimeOffset.UtcNow;
				rateTag.SourceTimeStamp = DateTimeOffset.UtcNow;
			}

			try
			{
				logText += rateValue.ToString() + "," + valueTag.Value.ToString() + "," + sourceTimeInMilliSec.ToString();
				logText += "," + changeInTime.ToString() + "," + changeInValue.ToString() + "," + instantaneousRate.ToString();
				logText += "," + ATerm.ToString() + "," + BTerm.ToString() + "," + CTerm.ToString();

				int x = 0;
				foreach (double[] ppair in this.leastSquareCalculator.pointArray)
				{
					logText += "," + ppair[0].ToString();
					logText += "," + ppair[1].ToString();
					++x;
				}

				logFileNameText = rateTag.ID.Replace(' ', '-');
				writeDataToLogFile(logFileNameText, logText);
			}
			catch
			{
				// do nothing
			}

			return true;
		}

		/// <summary>
		/// This method must be defined due to the interface.  Note: At this point it serves no
		/// purpose until there are custom modules.
		/// </summary>
		/// <param name="calculationName"></param>
		/// <returns>Returns a collection of module input/output objects.</returns>
		public ModuleInputOutputCollection GetInputOutputCollection(string calculationName)
		{
			var properties = new ModuleInputOutputCollection
							{
								new ModuleInputOutput
								{
									ID = "Level Product",
									Type = typeof(double?),
									ParameterType = ModuleInputOutputType.Input
								},
								new ModuleInputOutput
								{
									ID = "Volume Total Observed",
									Type = typeof(double?),
									ParameterType = ModuleInputOutputType.Input
								},
								new ModuleInputOutput
								{
									ID = "Volume Net Standard",
									Type = typeof(double?),
									ParameterType = ModuleInputOutputType.Input
								},
								new ModuleInputOutput
								{
									ID = "Level Product Rate",
									Type = typeof(double?),
									ParameterType = ModuleInputOutputType.Output
								},
								new ModuleInputOutput
								{
									ID = "Volume Total Observed Rate",
									Type = typeof(double?),
									ParameterType = ModuleInputOutputType.Output
								},
								new ModuleInputOutput
								{
									ID = "Volume Net Standard Rate",
									Type = typeof(double?),
									ParameterType = ModuleInputOutputType.Output
								}
							};

			return properties;
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will perform validation on the input and output tags.
		/// </summary>
		/// <param name="inputTag"></param>
		/// <param name="outputTag"></param>
		/// <returns></returns>
		private bool PerformValidation(PointTag inputTag, PointTag outputTag)
		{
			// Ensure that the output tag is not set to calculate or the status code is equal to override.
			// In this case, do not set the output tag and just return.
			if (outputTag.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
				outputTag.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return false;
			}

			// Check to see if the input tag has a value and the status is not a bad status.
			if (IsValueGood(inputTag) == false)
			{
				outputTag.Value = null;
				outputTag.Status = StatusCodes.BadNoDataAvailable;
				outputTag.ServerTimeStamp = inputTag.ServerTimeStamp;
				outputTag.SourceTimeStamp = inputTag.SourceTimeStamp;
			}

			// Check for output tag unit type is a velocity.
			if (outputTag.EngineeringUnitsType != EngineeringUnitType.FmuVelocity && outputTag.EngineeringUnitsType != EngineeringUnitType.FmuVolflow)
			{
				outputTag.Value = null;
				outputTag.Status = StatusCodes.UncertainEngineeringUnitsExceeded;
				outputTag.ServerTimeStamp = inputTag.ServerTimeStamp;
				outputTag.SourceTimeStamp = inputTag.SourceTimeStamp;

				return false;
			}

			// Check for value being populated
			if (inputTag.Value == null)
			{
				outputTag.Value = null;
				outputTag.Status = StatusCodes.BadNoDataAvailable;
				outputTag.ServerTimeStamp = inputTag.ServerTimeStamp;
				outputTag.SourceTimeStamp = inputTag.SourceTimeStamp;

				return false;
			}

			// The engineering unit type must be either a length of volume.
			if (inputTag.EngineeringUnitsType != EngineeringUnitType.FmuLength && inputTag.EngineeringUnitsType != EngineeringUnitType.FmuVolume)
			{
				outputTag.Value = null;
				outputTag.Status = StatusCodes.BadNoMatch;
				outputTag.ServerTimeStamp = inputTag.ServerTimeStamp;
				outputTag.SourceTimeStamp = inputTag.SourceTimeStamp;

				return false;
			}

			return true;
		}

		/// <summary>
		/// This method will convert a level or volume value to a new unit of measure.
		/// </summary>
		/// <param name="tagValue">The level or volume value to convert.</param>
		/// <param name="fromUnit">The original unit.</param>
		/// <param name="toUnit">The unit to convert to.</param>
		/// <returns></returns>
		private double ConvertToNewUnit(double tagValue, EngineeringUnit fromUnit, EngineeringUnit toUnit)
		{
			double newValue = EngineeringUnits.Convert(tagValue, fromUnit, toUnit, tagValue);
			return newValue;
		}

		/// <summary>
		/// This method will convert the rate to rate tag units.
		/// </summary>
		/// <param name="inRate">The rate to convert.</param>
		/// <param name="outputTag">The rate tag.</param>
		/// <param name="knownUnitType">The known unit type which is what is used in the application.</param>
		/// <returns></returns>
		private double? ConvertRateToRateTagUnits(double inRate, PointTag outputTag, EngineeringUnitType knownUnitType)
		{
			// Convert the rate to value per seconds.
			double rateInSeconds = inRate * 10;
			double? convertedRate = null;

			if (knownUnitType == EngineeringUnitType.FmuLength)
			{
				convertedRate = this.ConvertToNewUnit(rateInSeconds, EngineeringUnit.FmvrMmSec, outputTag.Units);
			}

			if (knownUnitType == EngineeringUnitType.FmuVolume)
			{
				convertedRate = this.ConvertToNewUnit(rateInSeconds, EngineeringUnit.FmvfGps, outputTag.Units);
			}

			return convertedRate;
		}

		/// <summary>
		/// This method will determine if the absolute instantaneous rate value is less than or equal 
		/// to the absolute deadband value. If so it will true, otherwise it returns false. If the 
		/// deadband is zero, then return false (ignore).
		/// </summary>
		/// <param name="instantaneousRate">The instantaneous rate to evalulated.</param>
		/// <param name="rateTag">The rate tag that contains the engineering units type.</param>
		/// <returns>Returns true if within the deadband, otherwise it returns false.</returns>
		private bool WithinDeadband(double instantaneousRate, PointTag rateTag)
		{
			double deadband;

			if (double.TryParse(this.Settings.Deadband, out deadband) == false)
			{
				return false;
			}

			// Zero is an invalid deadband, therefore the rate is not within the deadband.
			if (deadband == 0.0)
			{
				return false;
			}

			double? convertedDeadband = null;

			// Converted the deadband from the stored units of Meters/Minute to Millimeters/second. The reason
			// is that the instantaneous level is in millimeters per second.
			if (rateTag.EngineeringUnitsType == EngineeringUnitType.FmuVelocity)
			{
				convertedDeadband = this.ConvertToNewUnit(deadband, EngineeringUnit.FmvrMMin, EngineeringUnit.FmvrMmSec);
			}

			// Converted the deadband from the stored units of Meters cube/Minute to Gallons/Min. The reason
			// is that the instantaneous volume is in gallons per second.
			if (rateTag.EngineeringUnitsType == EngineeringUnitType.FmuVolflow)
			{
				convertedDeadband = this.ConvertToNewUnit(deadband, EngineeringUnit.FmvfM3Min, EngineeringUnit.FmvfGps);
			}

			if (convertedDeadband == null)
			{
				rateTag.Value = null;
				rateTag.Status = StatusCodes.BadUnexpectedError;
				rateTag.ServerTimeStamp = DateTimeOffset.UtcNow;
				rateTag.SourceTimeStamp = DateTimeOffset.UtcNow;

				return false;
			}

			// Convert instantaneous rate to seconds.
			if (Math.Abs(instantaneousRate * 10) <= Math.Abs(convertedDeadband.Value))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// This method will calculate the what the next interval should be. It requires that
		/// there are three previous values prior to calculating. It will return null if there
		/// are not calculations.
		/// </summary>
		/// <param name="mostRecentTimestamp">The input tag source timestamp.</param>
		/// <returns>Returns the predicted next interval or returns null.</returns>
		private double? TrackIntervals(double mostRecentTimestamp)
		{
			double? nextUpdateInterval = null;
			 
			if (this.intervals == null)
			{
				this.intervals = new List<double>();
			}

			this.intervals.Add(mostRecentTimestamp);

			if (this.intervals.Count > 3)
			{
				// Calculate next update interval should be.
				double diff1 = Math.Abs(this.intervals[0] - this.intervals[1]);
				double diff2 = Math.Abs(this.intervals[1] - this.intervals[2]);
				double diff3 = Math.Abs(this.intervals[2] - this.intervals[3]);

				this.intervals.RemoveAt(0);

				nextUpdateInterval = (diff1 + diff2 + diff3) / 3;
			}

			return nextUpdateInterval;
		}

		/// <summary>
		/// This method will determine if the flow has stopped based on the predicted rate being
		/// less than the dead band and a predicted update interval.
		/// </summary>
		/// <param name="mostRecentTimestamp">This is the input tag source timestamp.</param>
		/// <param name="newValue">The converted input tag value.</param>
		/// <param name="rateTag">The Rate or output tag.</param>
		/// <returns>Returns true if a flow stoppage is detected. Otherwise, it returns false.</returns>
		private bool StoppageDetected(double mostRecentTimestamp, double newValue, PointTag rateTag)
		{
			double? nextUpdateInterval = this.TrackIntervals(mostRecentTimestamp);

			if (nextUpdateInterval == null)
			{
				return false;
			}

			double changeInTime = Math.Abs(nextUpdateInterval.Value);
			double changeInValue = newValue - this.leastSquareCalculator.LastValueValue;

			if ((long) changeInTime == 0)
			{
				return true;
			}

			double newInstantaneousRate = changeInValue / changeInTime;

			//double? rateValue = this.leastSquareCalculator.GetPredictedY(nextUpdateInterval.Value);

			//if (rateValue == null)
			//{
			//	return false;
			//}

			return this.WithinDeadband(newInstantaneousRate, rateTag);
		}

		/// <summary>
		/// This method will set the rate module timer to what is set in the 
		/// Rate Module Setting.
		/// </summary>
		/// <param name="inputTag">The input tag object.</param>
		private void SetRateModuleTimer(PointTag inputTag)
		{
			int staleTimeInSeconds = this.Settings.StaleTimePeriodInSeconds;

			if (staleTimeInSeconds <= 5)
			{
				return;
			}

			// The timer ID must be unique per rate module.
			string timerId = this.GetTimerId(inputTag.PointTagGuid);
			if (this.Settings.FlowCalculationType == "Least Squared")
				SRMTimerFunctions.AddTimer(timerId, inputTag.PointGuid, staleTimeInSeconds);
			else
				SRMTimerFunctions.AddTimer(timerId, inputTag.PointGuid, this.Settings.AveragingSampleTimeSeconds);
		}

		/// <summary>
		/// This method will determine if the difference between input tag server timestamp and
		/// the current UTC time is greater than the stale time.  If so, then that means there
		/// has not been any updates and there is a stoppage.
		/// </summary>
		/// <param name="inputTag">The input tag that contains the server timestamp.</param>
		/// <returns>Returns true if the difference is greater than the stale time. Otherwise, it return false.</returns>
		private int CheckTimerExpiration(PointTag inputTag, PointTag rateTag, ref double calcRateValue)
		{
			int expired = 0;
			int staleTimeInSeconds = this.Settings.StaleTimePeriodInSeconds;
			calcRateValue = 0.0;

			if (staleTimeInSeconds <= 5)
			{
				return expired;
			}

			TimeSpan deltaTime = DateTimeOffset.UtcNow.Subtract(inputTag.ServerTimeStamp);

			if (deltaTime.TotalSeconds > staleTimeInSeconds)
			{
				expired = 1;
				return expired;
			}

			if (this.Settings.FlowCalculationType == "Averaging" && this.averagingCalculation != null)
			{
				int returnValue = 2;
				if (averagingCalculation.numOfEntries == 0)
				{
					averagingCalculation.LastValueTime = inputTag.ServerTimeStamp;
					averagingCalculation.addDataEntry(inputTag, 0, this.Settings.AveragingNumberSamples);
				}

				TimeSpan deltaTimeAverTime = inputTag.ServerTimeStamp.ToUniversalTime().Subtract(this.averagingCalculation.LastValueTime.ToUniversalTime());

				if (this.averagingCalculation.Initialized == true)
				{
					TimeSpan deltaTimeAverTimeShutdown = DateTimeOffset.UtcNow.Subtract(this.averagingCalculation.LastValueTime.ToUniversalTime());
					if (deltaTimeAverTimeShutdown.TotalSeconds > staleTimeInSeconds)
					{
						expired = 1;
						return expired;
					}
				}

				if (deltaTimeAverTime.TotalSeconds >= this.Settings.AveragingSampleTimeSeconds && averagingCalculation.numOfEntries > 0)
				{
					this.averagingCalculation.Initialized = true;
					averagingCalculation.LastValueTime = inputTag.ServerTimeStamp;
					averagingCalculation.addDataEntry(inputTag, deltaTimeAverTime.TotalSeconds, this.Settings.AveragingNumberSamples);
				}


				return returnValue;  // this will force a data rate calculation
			}

			return expired;
		}

		/// <summary>
		/// This method will reset the rate module to its initial state.  This happens
		/// when the rate module is initially called or when a stoppage is detected.
		/// </summary>
		/// <param name="inputTag">The input tag object.</param>
		/// <param name="rateTag">The rate or output tag object.</param>
		private void ResetToInitialState(PointTag inputTag, PointTag rateTag)
		{
			if (this.Settings.FlowCalculationType == "Least Squared")
			{
				if (this.intervals != null)
				{
					this.intervals.Clear();
				}

				// Remove the timer.
				string timerId = this.GetTimerId(inputTag.PointTagGuid);
				SRMTimerFunctions.RemoveTimer(timerId);

				this.leastSquareCalculator = new LeastSquaredQuadRegression();
				if (this.averagingCalculation != null)
				{
					this.averagingCalculation = null;
				}
			}
			else
			{
				// Remove the timer.
				string timerId = this.GetTimerId(inputTag.PointTagGuid);
				SRMTimerFunctions.RemoveTimer(timerId);

				this.averagingCalculation = new AveragingCalculation(inputTag);

				if (this.leastSquareCalculator != null)
				{
					this.leastSquareCalculator = null;
				}
			}

			rateTag.Value = 0.0;
			rateTag.Status = StatusCodes.Good;
			rateTag.ServerTimeStamp = DateTimeOffset.UtcNow;
			rateTag.SourceTimeStamp = DateTimeOffset.UtcNow;
		}

		/// <summary>
		/// This method will build the timer ID based on the point tag GUID.
		/// </summary>
		/// <param name="pointTagGuid">Point tag GUID</param>
		/// <returns>Returns the timer ID.</returns>
		private string GetTimerId(Guid pointTagGuid)
		{
			string timerId = pointTagGuid + "_RateModule";
			return timerId;
		}

		/// <summary>
		/// This method will initial the object to its initialize state.
		/// </summary>
		private void Init()
		{
			this.Settings = new RateModuleSettings();
			this.leastSquareCalculator = null;
			LoadAppConfiguration();
		}

		private void LoadAppConfiguration()
		{
			directoryForFiles = string.Empty;
			daystoRetainRecords = 1;
			logDataTrueorFalse = false;
			var s1 = ConfigurationManager.AppSettings.Get("RateModuleLogDataTrueorFalse");
			if (string.IsNullOrEmpty(s1) || s1.ToUpper() != "TRUE")
				return;
			var s2 = System.Convert.ToInt16(ConfigurationManager.AppSettings.Get("RateModuleDaystoRetainRecords"));
			if (s2 < 1 || s2 > 5)
				s2 = 1;
			var s3 = ConfigurationManager.AppSettings.Get("RateModuleDirectoryForFiles");
			if (string.IsNullOrEmpty(s3) || string.IsNullOrWhiteSpace(s3))
				return;

			// make sure the path has a '\' at the end
			if (s3.EndsWith("\\") == false && s3.EndsWith("/") == false)
			{
				s3 += "\\";
			}
			directoryForFiles = s3;
			daystoRetainRecords = s2;
			logDataTrueorFalse = true;
		}
		private void writeDataToLogFile(string logFileNameText,string logText)
		{
			if (logDataTrueorFalse != true)
				return;
			string logPath = directoryForFiles + logFileNameText + DateTime.Now.Day.ToString() + ".csv";
			string oldlogPath = directoryForFiles + logFileNameText + DateTime.Now.AddDays(-(daystoRetainRecords)).Day.ToString() + ".csv";

			logText += Environment.NewLine;
			try
			{
				// check if the file exists from yesterday and if so delete it
				if (File.Exists(oldlogPath))
				{
					File.Delete(oldlogPath);
				}
				// now this no longer a debug file but we now have to add headers even though the data is not really that helpful
				// to people who do not know how this works. Plus I am told that this will be used for other things. I just want to point out
				// that this is not my idea and running this on a system while writing to the hard drive will have a performance impact
				// so if you are in here changing this do not blame me. 
				if (!File.Exists(logPath))
				{
					string header = "DateTime,Tag Guid,Calc Converted Rate,Input Value,Input Time(ms),Rate Calc Change in Time, Rate Calc Change in Value,Instantaneous Rate,ATerm,BTerm,CTerm,Delta T0,Rate0,";
					header += "Delta T1,Rate1,Delta T2,Rate2,Delta T3,Rate3,Delta T4,Rate4,Delta T5,Rate5";
					header += Environment.NewLine;

					// create the file with the useless header
					File.AppendAllText(logPath, header, Encoding.UTF8);
				}
				File.AppendAllText(logPath, DateTime.Now.ToString() + "," + logText, Encoding.UTF8);
			}
			catch(Exception ex)
			{
				// do nothing
			}

		}
		#endregion
	}
}
