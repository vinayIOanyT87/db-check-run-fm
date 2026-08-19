namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Runtime.Serialization;
	using System.Xml.Serialization;

	[DataContract]
	[Serializable]
	public class TransactionSummaryClass
	{
		[DataMember]
		public string TransID { get; set; }

		public string DisplayTransID { get { return this.TransID; } }

		[DataMember]
		public string AliasName { get; set; }

		[DataMember]
		public double GrossQuantity { get; set; }

		[DataMember]
		public DateTime InventoryDate { get; set; }

		/// <summary>
		/// Get the inventory date formatted according to the site's regional date settings
		/// </summary>
		public string InventoryDateInSiteFormat
		{
			get
			{
				return this.InventoryDate.ToString(this.ShortDatePattern);
			}
		}

		[DataMember]
		public DateTimeOffset TransDateTime { get; set; }

		/// <summary>
		/// Get the Transaction Date Time formatted according to the site's regional date settings
		/// </summary>
		public string TransDateTimeInSiteFormat
		{
			get
			{
				return this.TransDateTime.ToString(this.ShortDatePattern + " " + this.TimePattern);
			}
		}

		[DataMember]
		public DateTimeOffset EffectiveDate { get; set; }

		/// <summary>
		/// Get the Effective Date formatted according to the site's regional date settings
		/// </summary>
		public string EffectiveDateInSiteFormat
		{
			get
			{
				return this.EffectiveDate == DateTimeOffset.MinValue ? "" : this.EffectiveDate.ToString(this.ShortDatePattern + " " + this.TimePattern);
			}
		}

		[DataMember]
		public DateTimeOffset ExpirationDate { get; set; }

		/// <summary>
		/// Get the Expiration Date formatted according to the site's regional date settings
		/// </summary>
		public string ExpirationDateInSiteFormat
		{
			get
			{
				return this.ExpirationDate == DateTimeOffset.MinValue ? "" : this.ExpirationDate.ToString(this.ShortDatePattern + " " + this.TimePattern);
			}
		}

		[DataMember]
		public DateTimeOffset RequestedDeliveryDate { get; set; }

		/// <summary>
		/// Get the Requested Delivery Date formatted according to the site's regional date settings
		/// </summary>
		public string RequestedDeliveryDateInSiteFormat
		{
			get
			{
				return this.RequestedDeliveryDate == DateTimeOffset.MinValue ? "" : this.RequestedDeliveryDate.ToString(this.ShortDatePattern + " " + this.TimePattern);
			}
		}

		[DataMember]
		public DateTimeOffset ScheduledDate { get; set; }

		/// <summary>
		/// Get the Scheduled Date formatted according to the site's regional date settings
		/// </summary>
		public string ScheduledDateInSiteFormat
		{
			get
			{
				return this.ScheduledDate == DateTimeOffset.MinValue ? "" : this.ScheduledDate.ToString(this.ShortDatePattern + " " + this.TimePattern);
			}
		}

		[DataMember]
		public DateTimeOffset TimeIn { get; set; }

		/// <summary>
		/// Get the TimeIn Date Time formatted according to the site's regional date settings
		/// </summary>
		public string TimeInInSiteFormat
		{
			get
			{
				return this.TimeIn == DateTimeOffset.MinValue ? "" : this.TimeIn.ToString(this.ShortDatePattern + " " + this.TimePattern);
			}
		}

		[DataMember]
		public DateTimeOffset TimeOut { get; set; }

		/// <summary>
		/// Get the TimeOut Date Time formatted according to the site's regional date settings
		/// </summary>
		public string TimeOutInSiteFormat
		{
			get
			{
				return this.TimeOut == DateTimeOffset.MinValue ? "" : this.TimeOut.ToString(this.ShortDatePattern + " " + this.TimePattern);
			}
		}

		[DataMember]
		public bool AutoComplete { get; set; }

		[DataMember]
		public string BillToID { get; set; }

		[DataMember]
		public string CarrierID { get; set; }

		[DataMember]
		public string ConjoinTransID { get; set; }

		[DataMember]
		public string DestinationRegistrationID1 { get; set; }

		[DataMember]
		public string DestinationRegistrationID2 { get; set; }

		[DataMember]
		public string DestinationSerialNumber1 { get; set; }

		[DataMember]
		public string DestinationSerialNumber2 { get; set; }

		[DataMember]
		public bool Flag01 { get; set; }

		[DataMember]
		public string LegacyNumber { get; set; }

		[DataMember]
		public string ManagerID { get; set; }

		[DataMember]
		public string OperatorID { get; set; }

		[DataMember]
		public string PONumber { get; set; }

		[DataMember]
		public string ReversalType { get; set; }

		[DataMember]
		public string ShipmentNumber { get; set; }

		[DataMember]
		public string ShipperID { get; set; }

		[DataMember]
		public string Site { get; set; }

		[DataMember]
		public string SupplierID { get; set; }

		[DataMember]
		public int ArmNumber { get; set; }

		[DataMember]
		public string AdditiveProfileID { get; set; }

		[DataMember]
		public string BatchNumber { get; set; }

		[DataMember]
		public double Density { get; set; }

		[DataMember]
		public string DestinationCompartmentID { get; set; }

		[DataMember]
		public string DestinationRegistrationID { get; set; }

		[DataMember]
		public string InvoiceLineNumber { get; set; }

		[DataMember]
		public string InvoiceNumber { get; set; }

		[DataMember]
		public int LineNumber { get; set; }

		[DataMember]
		public string LoadingLocationID { get; set; }

		[DataMember]
		public double MassQuantity { get; set; }

		[DataMember]
		public string MeterID { get; set; }

		[DataMember]
		public double MeterStart { get; set; }

		[DataMember]
		public DateTimeOffset MeterStartDateTime { get; set; }

		/// <summary>
		/// Get the Meter Start Date formatted according to the site's regional date settings
		/// </summary>
		public string MeterStartDateTimeInSiteFormat
		{
			get
			{
				return this.MeterStartDateTime == DateTimeOffset.MinValue ? "" : this.MeterStartDateTime.ToString(this.ShortDatePattern + " " + this.TimePattern);
			}
		}

		[DataMember]
		public double MeterStop { get; set; }

		[DataMember]
		public DateTimeOffset MeterStopDateTime { get; set; }

		/// <summary>
		/// Get the Meter Stop Date formatted according to the site's regional date settings
		/// </summary>
		public string MeterStopDateTimeInSiteFormat
		{
			get
			{
				return this.MeterStopDateTime == DateTimeOffset.MinValue ? "" : this.MeterStopDateTime.ToString(this.ShortDatePattern + " " + this.TimePattern);
			}
		}

		[DataMember]
		public double PresetAmount { get; set; }

		[DataMember]
		public string StorageLocationID { get; set; }

		[DataMember]
		public double Temperature { get; set; }

		[DataMember]
		public double Vcf { get; set; }

		[DataMember]
		public string Notes { get; set; }

		[DataMember]
		public double NetQuantity { get; set; }

		[DataMember]
		public string OwnerID { get; set; }

		[DataMember]
		public string ProductID { get; set; }

		[DataMember]
		public string ShipToID { get; set; }

		[DataMember]
		public string TransactionStatus { get; set; }

		[DataMember]
		public string ShortDatePattern { get; set; }

		[DataMember]
		public string TimePattern { get; set; }

		[DataMember]
		public bool DeleteFlag { get; set; }

		[DataMember]
		public string DocumentNumber { get; set; }

		[DataMember]
		[XmlIgnore]
		public int RecordCount { get; set; }

      [DataMember]
      public double DeliveredGrossQuantity { get; set; }

      [DataMember]
      public double DeliveredNetQuantity { get; set; }

      [DataMember]
      public bool DeliveredGrossManualValueFlag { get; set; }

      [DataMember]
      public bool DeliveredNetManualValueFlag { get; set; }

      [DataMember]
      public double Pressure { get; set; }

   }
}
