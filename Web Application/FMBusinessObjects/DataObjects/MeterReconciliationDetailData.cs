///***************************************************************************
/// Module Name:  MeterReconciliationDetailData.cs
/// Author:       Ryan Hill
/// Copyright (c) Varec, Inc.  All rights reserved.
///***************************************************************************

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.Serialization;

	/// <summary>
	/// Defines a row of data to be returned to the meter reconciliation detail screen's detail grid.
	/// Each record represents a transaction line item or sub line item.
	/// </summary>
	[Serializable]
	[DataContract]
	public class MeterReconciliationDetailData
	{
		[DataMember]
		public string TransactionID { get; set; }

		[DataMember]
		public string InventoryDate { get; set; }

		[DataMember]
		public string Product { get; set; }

		[DataMember]
		public double MeterStart { get; set; }

		[DataMember]
		public double MeterStop { get; set; }

		[DataMember]
		public double MeterTotal { get; set; }

		[DataMember]
		public double MeterSkip { get; set; }

		[DataMember]
		public string Carrier { get; set; }

		[DataMember]
		public string StationID { get; set; }

		[DataMember]
		public string TransactionAlias { get; set; }

		[DataMember]
		public string FlightNumber { get; set; }

		[DataMember]
		public string TicketNumber { get; set; }

		[DataMember]
		public bool RotatesBackwardsFlag { get; set; }

		[DataMember]
		public int NumberOfDigits { get; set; }

		[DataMember]
		public Guid TransactionGuid { get; set; }

		[DataMember]
		public double GrossVolume { get; set; }
	}
}
