
namespace RateModules.RateCalculationModules
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using FMBusinessObjects.DataObjects;
	using Varec.CommonComponents.EngineeringUnitsLibrary;

	class AveragingCalculation
	{
		public double[] pointpair { get; set; }

		public ArrayList pointArray = new ArrayList();

		public int numOfEntries { get; set; }
		public DateTimeOffset LastValueTime { get; set; }
		public double LastValueValue { get; set; }
		public double initialValue { get; set; }
		public bool Initialized { get; set; }

		public AveragingCalculation(PointTag inputTag)
		{
			this.numOfEntries = 0;
			this.Initialized = false;
			this.pointpair = new double[2];
			this.LastValueTime = inputTag.SourceTimeStamp.ToUniversalTime();
			this.LastValueValue = (double)inputTag.Value;
			this.initialValue = (double)inputTag.Value;
		}

		public void addDataEntry(PointTag inputTag,double amountOfTime,int numberofEntries)
		{
			// add the entry to the array
			this.pointpair = new double[2];
			this.pointpair[0] = amountOfTime;
			this.pointpair[1] = (double)inputTag.Value;
			this.pointArray.Add(this.pointpair);
			this.numOfEntries = this.pointArray.Count;

			if(this.numOfEntries > numberofEntries)
			{
				double[] ppointvar = (double[]) pointArray[0];
				this.initialValue = ppointvar[1];
				this.pointArray.RemoveAt(0);
			}
		}

		public double? CalculateRate(PointTag valueTag,PointTag rateTag)
		{
			double returnedRate = 0.0;
			double dAmountOfTime = 0.0;
			double? DVolumeAmount = 0.0;
			int iPosition = 0;
			double ChangeInValue = 0.0;
			double LastpointVal = 0.0;

			if (this.pointArray.Count < 2 || Initialized == false)
				return returnedRate;

			foreach(double[] pointValPair in this.pointArray)
			{
				// 0 = time in seconds 1 = value in tag units
				double? convertedSourceValue = null;

				if (iPosition == 0)
				{
					LastpointVal = pointValPair[1];
					++iPosition;
					continue;
				}
				else
				{
					ChangeInValue = pointValPair[1] - LastpointVal;
					LastpointVal = pointValPair[1];
				}

				if (valueTag.EngineeringUnitsType == EngineeringUnitType.FmuLength)
				{
					var tagValue = (double)valueTag.Value;
					convertedSourceValue = this.ConvertToNewUnit(ChangeInValue, valueTag.Units, EngineeringUnit.FmlMm);
				}

				if (valueTag.EngineeringUnitsType == EngineeringUnitType.FmuVolume)
				{
					var tagValue = (double)valueTag.Value;
					convertedSourceValue = this.ConvertToNewUnit(ChangeInValue, valueTag.Units, EngineeringUnit.FmvUsGal);
				}

				if (convertedSourceValue == null)
					return null;


				DVolumeAmount += convertedSourceValue;
				dAmountOfTime += pointValPair[0];

			}
			if (dAmountOfTime == 0.0)
				return null;
			// calculate the rate which is change in volume/change in time

			double? CalculateRateValue = DVolumeAmount / dAmountOfTime;

			// Convert the rate value to the units of the rate tag.  The rate is either in
			// millimeters per second or in US Gallons per second.
			double? rateValue = this.ConvertRateToRateTagUnits(CalculateRateValue.Value, rateTag, valueTag.EngineeringUnitsType);

			return rateValue;
		}

		private double ConvertToNewUnit(double tagValue, EngineeringUnit fromUnit, EngineeringUnit toUnit)
		{
			double newValue = EngineeringUnits.Convert(tagValue, fromUnit, toUnit, tagValue);
			return newValue;
		}

		public double? ConvertRateToRateTagUnits(double inRate, PointTag outputTag, EngineeringUnitType knownUnitType)
		{
			// Convert the rate to value per seconds.
			double rateInSeconds = inRate;// * 10;
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

		public bool IsRateOutsideDeadband(PointTag valueTag, string deadBand, PointTag rateTag, ref double calcRateValue)
		{
			double inputValue = (double)valueTag.Value;
			double DeadBand = System.Convert.ToDouble(deadBand);
			double? convertedSourceValue = null;
			double ChangeInValue = 0.0;
			double dAmountOfTime = 0.0;

			if (DeadBand <= 0.0 || Initialized == true)
			{
				Initialized = true;
				return true;
			}

			// calculate the rate between this point and the last one stored
			//this.LastValueTime
			//this.LastValueValue
			ChangeInValue = this.LastValueValue - inputValue;
			if (valueTag.EngineeringUnitsType == EngineeringUnitType.FmuLength)
			{
				var tagValue = (double)valueTag.Value;
				convertedSourceValue = this.ConvertToNewUnit(tagValue, valueTag.Units, EngineeringUnit.FmlMm);
			}

			if (valueTag.EngineeringUnitsType == EngineeringUnitType.FmuVolume)
			{
				var tagValue = (double)valueTag.Value;
				convertedSourceValue = this.ConvertToNewUnit(tagValue, valueTag.Units, EngineeringUnit.FmvUsGal);
			}

			if (convertedSourceValue == null)
				return false;

			TimeSpan timeDiff = valueTag.ServerTimeStamp.ToUniversalTime() - this.LastValueTime.ToUniversalTime();

			dAmountOfTime = timeDiff.Seconds;

			if (dAmountOfTime <= 0.0)
				return false;

			double? CalculateRateValue = convertedSourceValue / dAmountOfTime;

			double? rateValue = this.ConvertRateToRateTagUnits(CalculateRateValue.Value, rateTag, valueTag.EngineeringUnitsType);

			if (rateValue == null)
			{
				return false;
			}

			if (rateValue < 0)
			{
				if ((rateValue * -1) <= DeadBand)
					return false;
			}
			else if (rateValue <= DeadBand)
			{
				return false;
			}

			calcRateValue = rateValue.Value;
			Initialized = true;
			return true; ;
		}
	}
}
