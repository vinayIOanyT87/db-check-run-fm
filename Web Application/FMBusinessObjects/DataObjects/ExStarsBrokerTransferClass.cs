namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.Serialization;
	using System.Text;

	public class ExStarsBrokerTransferListClass : List<ExStarsBrokerTransferClass> { }

	/// <summary>
	/// Represent a transaction where ownership of product is transfered from one owner to another
	/// NOTE: in Legacy there is a condition  AND SUBTYPE_CODE_3 = 'BRKTFR'
	///       This is being presumed here
	/// </summary>
	[Serializable]
	[DataContract]
	public class ExStarsBrokerTransferClass : ExStarsBaseTransactionClass
	{
		public bool UseFromToOwners { get; protected set; }

		[DataMember]
		public Guid DebitTransGuid { get; set; }

		[DataMember]
		public Guid CreditTransGuid { get; set; }

		[DataMember]
		public string ProductId { get; set; }

		[DataMember]
		public string DocumentNumber { get; set; }

		[DataMember]
		public Guid ManagerCompanyGuid { get; set; }

		[DataMember]
		public Guid CarrierCompanyGuid { get; set; }

		[DataMember]
		public Guid ShipperCompanyGuid { get; set; }

		[DataMember]
		public Guid FromOwnerCompanyGuid { get; set; }

		[DataMember]
		public Guid ToOwnerCompanyGuid { get; set; }

		[DataMember]
		public Guid SupplierCompanyGuid { get; set; }

		[DataMember]
		public string ManagerID { get; set; }

		[DataMember]
		public string CarrierCompanyId { get; set; }

		[DataMember]
		public string ShipperCompanyId { get; set; }

		[DataMember]
		public string FromOwnerId { get; set; }

		[DataMember]
		public string ToOwnerId { get; set; }

		[DataMember]
		public string FromOwnerCode { get; set; }

		[DataMember]
		public string ToOwnerCode { get; set; }

		[DataMember]
		public string ManagerState { get; set; }

		[DataMember]
		public string SupplierId { get; set; }

		[DataMember]
		public string ManagerFederalId { get; set; }


		public ExStarsBrokerTransferClass Clone()
		{
			return (ExStarsBrokerTransferClass)this.MemberwiseClone();
		}

		public ExStarsBrokerTransferClass(bool useFromToOwner)
		{
			this.UseFromToOwners = useFromToOwner;
		}



		public ExStarsTaxInfoBrokerTransferSumCollection TaxInfoSumCollection = new ExStarsTaxInfoBrokerTransferSumCollection();

		public void AddTaxInfo(ExStarsBrokerTransferClass trx)
		{
			string key = trx.TaxInfoAmtKey();
			this.TaxInfoSumCollection.Add(key, trx);
		}

		public bool ValidateTransaction(out String errorMessage)
		{
			StringBuilder msg = new StringBuilder();
			if (Guid.Empty.Equals(this.ProductGuid))
			{
				msg.AppendLine("   No product is defined");
			}
			if (Guid.Empty.Equals(this.ToOwnerCompanyGuid))
			{
				msg.AppendLine("   No To Owner is defined");
			}
			if (Guid.Empty.Equals(this.FromOwnerCompanyGuid))
			{
				msg.AppendLine("   No From Owner is defined");
			}
			if (msg.ToString().Length > 0)
			{
				errorMessage = String.Format("Transaction {0} is missing {1}\n", msg.ToString());
				return false;
			}
			errorMessage = "";
			return true;
		}

		public string RollUpKey()
		{
			return RollUpKey(this);
		}

		public static string RollUpKey(ExStarsBrokerTransferClass brokerXfer)
		{
#if true
			if (brokerXfer.UseFromToOwners)
			{
				// ref C_ExSTARS_X12_Schedule_Detail::Generate_Bulk_Issues() ~ 3340
				return string.Format("Broker_Disbursement;{0};{1};{2};{3};{4};"
					, brokerXfer.ManagerID
					, brokerXfer.FromOwnerId
					// yes, twice
					, brokerXfer.FromOwnerId
					, brokerXfer.TaxCode
					, brokerXfer.EquipmentType
					);
			}
			else
			{
				// ref C_ExSTARS_X12_Schedule_Detail::Generate_Terminal_Receipts ~ 1716
				return string.Format("Broker_Receipt;{0};{1};{2};{3};{4};"
					, brokerXfer.ManagerID
					, brokerXfer.ToOwnerId
					, brokerXfer.FromOwnerId
					, brokerXfer.TaxCode
					, brokerXfer.EquipmentType
					);
			}
#else
			if (brokerXfer.UseFromToOwners)
			{
				// ref C_ExSTARS_X12_Schedule_Detail::Generate_Terminal_Receipts ~ 1716
				return string.Format("Broker_Disbursement;{0};{1};{2};{3};{4};"
					, brokerXfer.ManagerCompanyGuid.ToString()
					, brokerXfer.FromOwnerCompanyGuid.ToString()
					// yes, twice
					, brokerXfer.FromOwnerCompanyGuid.ToString()
					, brokerXfer.TaxCode
					, brokerXfer.EquipmentType
					);
				
			}
			else
			{
				// ref C_ExSTARS_X12_Schedule_Detail::Generate_Terminal_Receipts ~ 1716
				return string.Format("Broker_Receipt;{0};{1};{2};{3};{4};"
					, brokerXfer.ManagerCompanyGuid.ToString()
					, brokerXfer.ToOwnerCompanyGuid.ToString()
					, brokerXfer.FromOwnerCompanyGuid.ToString()
					, brokerXfer.TaxCode
					, brokerXfer.EquipmentType
					);				
			}
#endif
		}

		public string TaxInfoAmtKey()
		{
			return TaxInfoAmtKey(this);
		}

		public static string TaxInfoAmtKey(ExStarsBrokerTransferClass brokerXfer)
		{
			// ref C_ExSTARS_X12_Schedule_Detail::Generate_Terminal_Receipts ~ 1726
			// ref C_ExSTARS_X12_Schedule_Detail::Generate_Bulk_Issues ~ 3350
			return string.Format("{0}{1}{2};{3};{4};"
				, brokerXfer.ReportYear
				, brokerXfer.ReportMonth
				, brokerXfer.ReportDay
				, brokerXfer.DocumentNumber
				, brokerXfer.NetVolume < 0 ? "-" : "+"
				);
		}

	}
}
