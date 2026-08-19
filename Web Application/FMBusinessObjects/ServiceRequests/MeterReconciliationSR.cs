///***************************************************************************
/// Module Name:  MeterReconciliationSR.cs
/// Author:       Ryan Hill
/// Copyright (c) Varec, Inc.  All rights reserved.
///***************************************************************************

using System;
using System.Runtime.Serialization;

using FMBusinessObjects.UtilityObjects;

namespace FMBusinessObjects.ServiceRequests
{
    [Serializable]
    [DataContract]
	public class MeterReconciliationSR : AccountingServiceRequest
	{
		[DataMember]
		protected DateTime inventoryDate;

		/// <summary>
		/// This is the default constructor for the meter reconciliation service
		/// request class.
		/// </summary>
		public MeterReconciliationSR()
		{
		}

		public DateTime InventoryDate
		{
			get { return inventoryDate; }
			set { inventoryDate = TimeConverter.ToDate(value).Date; }
		}

		[DataMember]
		public Guid MeterGuid { get; set; }

		[DataMember]
		public Guid AssetGuid { get; set; }

		[DataMember]
		public Guid ManagerCompanyGuid { get; set; }

		[DataMember]
		public Guid ProductGuid { get; set; }

		[DataMember]
		public Guid CarrierCompanyGuid { get; set; }

		[DataMember]
		public bool? InOutOfTolerance { get; set; }

		[DataMember]
		public double ToleranceValue { get; set; }

		[DataMember]
		public bool ToleranceIsPercent { get; set; }
	}
}
