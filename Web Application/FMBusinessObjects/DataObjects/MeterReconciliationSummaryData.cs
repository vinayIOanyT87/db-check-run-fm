///***************************************************************************
/// Module Name:  MeterReconciliationSummaryData.cs
/// Author:       Ryan Hill
/// Copyright (c) Varec, Inc.  All rights reserved.
///***************************************************************************

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.Serialization;

	/// <summary>
	/// Defines a row of data to be returned to the meter reconciliation summary grid. 
	/// Each row of data corresponds to one meter
	/// </summary>
	[Serializable]
	[DataContract]
	public class MeterReconciliationSummaryData
	{
		[DataMember]
		public Guid MeterGuid { get; set; }

		[DataMember]
		public string MeterID { get; set; }

		[DataMember]
		public Guid AssetGuid { get; set; }

		[DataMember]
		public string AssetID { get; set; }

		[DataMember]
		public bool RotatesBackwardsFlag { get; set; }

		[DataMember]
		public double MeterStart { get; set; }

		[DataMember]
		public double MeterStop { get; set; }

		[DataMember]
		public double MeterTotal { get; set; }

		[DataMember]
		public double TransactionMeterTotal { get; set; }

		[DataMember]
		public double TransactionVolumeTotal { get; set; }

		[DataMember]
		public string Product { get; set; }

		[DataMember]
		public string Carrier { get; set; }

		[DataMember]
		public string CurrentCloseoutTransactionID { get; set; }

		[DataMember]
		public bool MoreThanOneCloseoutFlag { get; set; }

		[DataMember]
		public bool NoPreviousCloseoutFlag { get; set; }

		[DataMember]
		public bool NoCurrentCloseoutFlag { get; set; }

		[DataMember]
		public double MeterVariance { get; set; }

		[DataMember]
		public double VolumeVariance { get; set; }

		[DataMember]
		public Guid TransactionGuid { get; set; }

		public bool IsError
		{
			get
			{
				return (this.MoreThanOneCloseoutFlag || this.NoPreviousCloseoutFlag || this.NoCurrentCloseoutFlag);
			}
		}

		/// <summary>
		/// Take any errors detected in the summary data row and create an error message to display
		/// </summary>
		/// <param name="inventoryDate">The inventory date specified on the search screen</param>
		/// <returns>An error message containing all of the errors detected</returns>
		public string GenerateErrorText(DateTimeOffset inventoryDate)
		{
			List<string> errors = new List<string>();

			if (this.MoreThanOneCloseoutFlag)
			{
				errors.Add("Two closeouts found on " + inventoryDate.ToString("MM/dd/yyyy") + ". Could not determine meter start.");
			}

			if (this.NoPreviousCloseoutFlag)
			{
				errors.Add("No meter closeout found prior to " + inventoryDate.ToString("MM/dd/yyyy") + ". Could not determine meter stop.");
			}

			if (this.NoCurrentCloseoutFlag)
			{
				errors.Add("No meter closeout found on " + inventoryDate.ToString("MM/dd/yyyy") + ". Could not determine meter start.");
			}

			return string.Join(Environment.NewLine, errors);
		}
	}
}
