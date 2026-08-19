namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Runtime.Serialization;

	[Serializable]
	[DataContract]
	public class ExStarsBaseTransactionClass
	{
		protected const string Barge = "BARGE";
		protected const string Ship = "SHIP";
		public const string Truck = "TRUCK";

		public const string TransferTransaction = "Transfer";

		[DataMember]
		public String AliasName { get; set; }

		//public EnumExStarsTrxType TrxType { get; set; }

		[DataMember]
		public Guid SiteGuid { get; set; }

		[DataMember]
		public Guid ProductGuid { get; set; }

		[DataMember]
		public string TaxCode { get; set; }

		[DataMember]
		public int ReportYear { get; set; }

		[DataMember]
		public int ReportMonth { get; set; }

		[DataMember]
		public int ReportDay { get; set; }

		[DataMember]
		public double GrossVolume { get; set; }

		[DataMember]
		public double NetVolume { get; set; }

		// for receipts EquipmentType is the source,
		// for issue, bulk issues
		[DataMember]
		public string EquipmentType { get; set; }
		[DataMember]
		public string EquipmentRegistrationId { get; set; }
		[DataMember]
		public string EquipmentSerialNumber { get; set; }

		[DataMember]
		public string SrcEquipmentType { get; set; }
		[DataMember]
		public string SrcEquipmentRegistrationId { get; set; }
		[DataMember]
		public string SrcEquipmentSerialNumber { get; set; }

		[DataMember]
		public string DestEquipmentType { get; set; }
		[DataMember]
		public string DestEquipmentRegistrationId { get; set; }
		[DataMember]
		public string DestEquipmentSerialNumber { get; set; }





		public string IrsTransportMode { get; set; }


		[DataMember]
		public string Userdata4 { get; set; }

		[DataMember]
		public string Userdata10 { get; set; }

		public bool AdjustmentUsedAsReciept
		{
			get
			{
				return this.AliasName.Equals(ExStarsConstants.ToString(EnumExStarsTrxType.Adjustment), StringComparison.OrdinalIgnoreCase)
				       && this.Userdata4 == "1";
			}
		}

		public bool IsBargeOrShip
		{
			get
			{
				return EquipmentType.ToUpper().StartsWith(Ship)
					   || EquipmentType.ToUpper().StartsWith(Barge);
			}
		}

		public bool AliasIs(EnumExStarsTrxType trxType)
		{
			return this.AliasName.Equals(ExStarsConstants.ToString(trxType), StringComparison.OrdinalIgnoreCase);
		}




	}
}
