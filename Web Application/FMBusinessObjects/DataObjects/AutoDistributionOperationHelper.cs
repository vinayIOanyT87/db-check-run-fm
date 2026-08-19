///***************************************************************************
/// Module Name:  AutoDistributionProcessorDAC
/// Author:       Daniel Or
/// Copyright (c) Varec, Inc.  All rights reserved.
///***************************************************************************

namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Data;
    using System.Globalization;
    using System.Runtime.Serialization;

    using FMCore;

    /// <summary>
    /// There are 12 columns in the distributions in 3 sections: Gross, Net, Mass.
    /// In each section, there are 4 columns. (Defined in ColumnTypes)
    /// </summary>
    public enum AutoDistributionQuantityTypes
	{
		Gross = 0,
		Net = 1,
		Mass = 2
	}

	/// <summary>
	/// Column type of the 4 columns in each Quantity type
	/// </summary>
	public enum AutoDistributionColumnTypes
	{
		Thruput = 0,
		ThruputPercent = 1,
		Quantity = 2,
		QuantityPercent = 3
	}

	/// <summary>
	/// This is a helper class to calculate thruput, percent totals and recalculate percentage when quantity is changed.
	/// This class is slightly abused.  It is not pure data object and not pure business logic.  
	/// It helps carry some number format info and do calculation at the same time.
	/// </summary>
	[DataContract]
   [Serializable]
   public class AutoDistributionOperationHelper
	{
		// Database field names
		public const string OwnerIDColumnName = "OwnerID";
		public const string OwnerGuidColumnName = "OwnerGuid";

		public const string GrossThruputColumnName = "GrossThruput";
		public const string GrossThruputPercentColumnName = "GrossThruputPercent";
		public const string GrossQuantityColumnName = "GrossQuantity";
		public const string GrossQuantityPercentColumnName = "GrossQuantityPercent";

		public const string NetThruputColumnName = "NetThruput";
		public const string NetThruputPercentColumnName = "NetThruputPercent";
		public const string NetQuantityColumnName = "NetQuantity";
		public const string NetQuantityPercentColumnName = "NetQuantityPercent";

		public const string MassThruputColumnName = "MassThruput";
		public const string MassThruputPercentColumnName = "MassThruputPercent";
		public const string MassQuantityColumnName = "MassQuantity";
		public const string MassQuantityPercentColumnName = "MassQuantityPercent";

		// number formats used for display and rounding
		// xxxProductNumberFormat is for thruputs
		// xxxTrxNumberFormat is for quantities
		[DataMember]
		public NumberFormatInfo VolumeProductNumberFormat { get; private set; }
		[DataMember]
		public NumberFormatInfo VolumeTrxNumberFormat { get; private set; }

		[DataMember]
		public NumberFormatInfo MassProductNumberFormat { get; private set; }
		[DataMember]
		public NumberFormatInfo MassTrxNumberFormat { get; private set; }

		[DataMember]
		public NumberFormatInfo PercentNumberFormat { get; private set; }

		//The following will be set after the Calculate method
		[DataMember]
		public double[] TotalThruputs { get; private set; }
		[DataMember]
		public double[] TotalQuantities { get; private set; }
		[DataMember]
		public double[] AccumulatedQuantities { get; private set; }


		public AutoDistributionOperationHelper(
			NumberFormatInfo newVolumeTrxNumberFormat,
			NumberFormatInfo newMassTrxNumberFormat,
			NumberFormatInfo newVolumeProductNumberFormat,
			NumberFormatInfo newMassProductNumberFormat,
			NumberFormatInfo newPercentNumberFormat)
		{
			VolumeTrxNumberFormat = newVolumeTrxNumberFormat;
			MassTrxNumberFormat = newMassTrxNumberFormat;
			VolumeProductNumberFormat = newVolumeProductNumberFormat;
			MassProductNumberFormat = newMassProductNumberFormat;
			PercentNumberFormat = newPercentNumberFormat;
		}

		/// <summary>
		/// This assumes the mainDataTable only have owner and thruput info at the first time.
		/// This adds all the quantities and percents columns.
		/// </summary>
		/// <param name="mainDataTable">DataTable to be added with new columns</param>
		private static void AddSupportingColumns(DataTable mainDataTable)
		{

			string[] fieldList = new string[] {
					GrossThruputPercentColumnName,   
					GrossQuantityColumnName,         
					GrossQuantityPercentColumnName,  
					NetThruputPercentColumnName,     
					NetQuantityColumnName,           
					NetQuantityPercentColumnName,    
					MassThruputPercentColumnName,    
					MassQuantityColumnName,          
					MassQuantityPercentColumnName
				};
			for (int idx = 0; idx < fieldList.Length; idx++)
			{
				string field = fieldList[idx];
				mainDataTable.Columns.Add(field, typeof(double));
			}
		}

		/// <summary>
		/// Returns the column/field name of the given column type combination.
		/// </summary>
		/// <param name="quantityType">Type of the quantity</param>
		/// <param name="columnType">Type of the column</param>
		/// <returns>Column Name</returns>
		public static string GetColumnName(AutoDistributionQuantityTypes quantityType, AutoDistributionColumnTypes columnType)
		{
			string columnName = string.Empty;
			switch (quantityType)
			{
				case AutoDistributionQuantityTypes.Gross:
					switch (columnType)
					{
						case AutoDistributionColumnTypes.Thruput:
							columnName = GrossThruputColumnName;
							break;
						case AutoDistributionColumnTypes.ThruputPercent:
							columnName = GrossThruputPercentColumnName;
							break;
						case AutoDistributionColumnTypes.Quantity:
							columnName = GrossQuantityColumnName;
							break;
						case AutoDistributionColumnTypes.QuantityPercent:
							columnName = GrossQuantityPercentColumnName;
							break;
					}
					break;
				case AutoDistributionQuantityTypes.Net:
					switch (columnType)
					{
						case AutoDistributionColumnTypes.Thruput:
							columnName = NetThruputColumnName;
							break;
						case AutoDistributionColumnTypes.ThruputPercent:
							columnName = NetThruputPercentColumnName;
							break;
						case AutoDistributionColumnTypes.Quantity:
							columnName = NetQuantityColumnName;
							break;
						case AutoDistributionColumnTypes.QuantityPercent:
							columnName = NetQuantityPercentColumnName;
							break;
					}
					break;
				case AutoDistributionQuantityTypes.Mass:
					switch (columnType)
					{
						case AutoDistributionColumnTypes.Thruput:
							columnName = MassThruputColumnName;
							break;
						case AutoDistributionColumnTypes.ThruputPercent:
							columnName = MassThruputPercentColumnName;
							break;
						case AutoDistributionColumnTypes.Quantity:
							columnName = MassQuantityColumnName;
							break;
						case AutoDistributionColumnTypes.QuantityPercent:
							columnName = MassQuantityPercentColumnName;
							break;
					}
					break;

			}
			return columnName;
		}

		/// <summary>
		/// Returns the number format of the given type combination
		/// </summary>
		/// <param name="quantityType">Type of the quantity</param>
		/// <param name="columnType">Type of the column</param>
		/// <returns>Number format to be used</returns>
		public NumberFormatInfo GetNumberFormatInfo(AutoDistributionQuantityTypes quantityType, AutoDistributionColumnTypes columnType)
		{
			NumberFormatInfo numberFormat = null;

			bool isMass = (quantityType == AutoDistributionQuantityTypes.Mass);
			switch (columnType)
			{
				case AutoDistributionColumnTypes.Thruput:
					numberFormat = isMass ? this.MassProductNumberFormat : this.VolumeProductNumberFormat;
					break;

				case AutoDistributionColumnTypes.Quantity:
					numberFormat = isMass ? this.MassTrxNumberFormat : this.VolumeTrxNumberFormat;
					break;
				default:
					//case ColumnTypes.ThruputPercent:
					//case ColumnTypes.QuantityPercent:
					numberFormat = this.PercentNumberFormat;
					break;
			}
			return numberFormat;
		}


		/// <summary>
		/// Given the DataRow and types, return the column value
		/// </summary>
		/// <param name="currentDataRow">Given DataRow</param>
		/// <param name="quantityType">Type of Quantity</param>
		/// <param name="columnType">Type of the Column</param>
		/// <returns>column value</returns>
		public static double GetRowDataByType(DataRow currentDataRow, AutoDistributionQuantityTypes quantityType, AutoDistributionColumnTypes columnType)
		{
			return (double)currentDataRow[GetColumnName(quantityType, columnType)];
		}

		/// <summary>
		/// Save the given value to the given row, the column will be determined by the types
		/// </summary>
		/// <param name="currentDataRow">Given DataRow</param>
		/// <param name="quantityType">Type of Quantity</param>
		/// <param name="columnType">Type of the Column</param>
		/// <param name="newValue">New value for the column</param>
		public static void SaveRowDataByType(DataRow currentDataRow, AutoDistributionQuantityTypes quantityType, AutoDistributionColumnTypes columnType, double newValue)
		{
			currentDataRow[GetColumnName(quantityType, columnType)] = newValue;
		}

		private static double FMRounding(double oldValue, int decimalPlaces)
		{
			return Math.Round(oldValue, decimalPlaces, MidpointRounding.AwayFromZero);
		}

		public double FMRoundingByQuantityType(double oldValue, AutoDistributionQuantityTypes quantityType)
		{
			int decimalPlaces = this.VolumeTrxNumberFormat.NumberDecimalDigits;

			if (quantityType == AutoDistributionQuantityTypes.Mass)
			{
				decimalPlaces = this.MassTrxNumberFormat.NumberDecimalDigits;
			}

			return FMRounding(oldValue, decimalPlaces);
		}

		/// <summary>
		/// Calculates the total thruputs and quantities(if not first time)
		/// </summary>
		/// <param name="isFirstTimeCalculating">Are we generating(first time calculating)?</param>
		/// <param name="dataRows">DataRows that we are working with</param>
		/// <param name="expectedTotalQuantities">Expected total quantities or variances</param>
		/// <param name="totalThruputs">Total thruputs</param>
		/// <param name="totalQuantities">Total quanitities</param>
		private static void CalculateTotalThruputsAndQuantities(bool isFirstTimeCalculating, DataRowCollection dataRows,
				double[] expectedTotalQuantities, double[] totalThruputs, double[] totalQuantities)
		{

			foreach (DataRow currentDataRow in dataRows)
			{
				foreach (AutoDistributionQuantityTypes quantityType in Enum.GetValues(typeof(AutoDistributionQuantityTypes)))
				{
					totalThruputs[(int)quantityType] += GetRowDataByType(currentDataRow, quantityType, AutoDistributionColumnTypes.Thruput);
					if (isFirstTimeCalculating == false)
					{
						totalQuantities[(int)quantityType] += GetRowDataByType(currentDataRow, quantityType, AutoDistributionColumnTypes.Quantity);
					}
				}
			}

			if (isFirstTimeCalculating)
			{
				expectedTotalQuantities.CopyTo(totalQuantities, 0);
			}

		}

		/// <summary>
		/// Calculates fraction.  Uses defaultFraction if the denominator is zero.
		/// </summary>
		/// <param name="srcValue">The numerator</param>
		/// <param name="total">The denominator</param>
		/// <param name="defaultFraction">Default value when the denominator is zero</param>
		/// <returns>The fraction value</returns>
		public static double CalculateFraction(double srcValue, double total, double defaultFraction)
		{
			double retValue;
			if (total == 0)
			{
				// when there is no total thruput, let's assume default
				retValue = defaultFraction;
			}
			else
			{
				retValue = srcValue / total;
			}
			return retValue;
		}

		/// <summary>
		/// Calculate thruputs, quantites and percents
		/// </summary>
		/// <param name="mainDataTable">This should have only owner and thru columns if it is first time</param>
		/// <param name="isFirstTimeCalculating">Are we generating/calculating the first time?</param>
		/// <param name="expectedTotalQuantities">Expected total quantities</param>
		public void Calculate(DataTable mainDataTable, bool isFirstTimeCalculating, double[] expectedTotalQuantities)
		{
            mainDataTable.ThrowIfNull("mainDataTable");
            expectedTotalQuantities.ThrowIfNull("expectedTotalQuantities");

			if (isFirstTimeCalculating)
			{
				AddSupportingColumns(mainDataTable);
			}

			DataRowCollection dataRows = mainDataTable.Rows;

			TotalThruputs = new double[] { 0, 0, 0 };
			TotalQuantities = new double[] { 0, 0, 0 };
			AccumulatedQuantities = new double[] { 0, 0, 0 };

			int percentDecimalPlaces = this.PercentNumberFormat.NumberDecimalDigits;
			double defaultFraction = 1D / dataRows.Count;

			CalculateTotalThruputsAndQuantities(isFirstTimeCalculating, dataRows, expectedTotalQuantities, TotalThruputs, TotalQuantities);

			for (int idx = 0; idx < dataRows.Count; idx++)
			{
				DataRow currentDataRow = dataRows[idx];
				// for Gross, Net and Mass
				foreach (AutoDistributionQuantityTypes quantityType in Enum.GetValues(typeof(AutoDistributionQuantityTypes)))
				{
					double theThruput = GetRowDataByType(currentDataRow, quantityType, AutoDistributionColumnTypes.Thruput);
					double theThruputPercentInFraction = 0;

					if (isFirstTimeCalculating)
					{
						theThruputPercentInFraction = CalculateFraction(theThruput, TotalThruputs[(int)quantityType], defaultFraction);
					}

					double theQuantity;
					double theQuantityPercent;
					if (isFirstTimeCalculating)
					{
						double theThruputPercent = FMRounding(theThruputPercentInFraction * 100, percentDecimalPlaces);
						theQuantity = FMRoundingByQuantityType(TotalQuantities[(int)quantityType] * theThruputPercentInFraction, quantityType);
						
						SaveRowDataByType(currentDataRow, quantityType, AutoDistributionColumnTypes.ThruputPercent, theThruputPercent);
						SaveRowDataByType(currentDataRow, quantityType, AutoDistributionColumnTypes.Quantity, theQuantity);
					}
					else
					{
						theQuantity = GetRowDataByType(currentDataRow, quantityType, AutoDistributionColumnTypes.Quantity);
					}

					theQuantityPercent = CalculateFraction(theQuantity, expectedTotalQuantities[(int)quantityType], defaultFraction) * 100;

					AccumulatedQuantities[(int)quantityType] += theQuantity;

					SaveRowDataByType(currentDataRow, quantityType, AutoDistributionColumnTypes.QuantityPercent, theQuantityPercent);
				}
			}

		}
	}
}
