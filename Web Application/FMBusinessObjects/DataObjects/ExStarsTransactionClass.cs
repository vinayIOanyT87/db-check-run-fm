
namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Text;
	using System.Collections;
	using System.Collections.Generic;
	using System.Runtime.Serialization;
	using FMBusinessObjects.Exceptions;

	public class ExStarsTransactionListClass : List<ExStarsTransactionClass> { }


	[Serializable]
	[DataContract]
	public class ExStarsTransactionClass : ExStarsBaseTransactionClass
	{
		public const string TwoPartyExchangeCode = "2-PARTY EXCH";

		[DataMember]
		public Guid TransactionGuid { get; set; }

		[DataMember]
		public String SubType { get; set; }
				
		[DataMember]
		public string TransId { get; set; }

		[DataMember]
		public string BillOfLadingNumber { get; set; }

		[DataMember]
		public string ProductId { get; set; }

		[DataMember]
		public bool AviationFuelFlag { get; set; }

		[DataMember]
		public bool GroundFuel { get; set; }

		[DataMember]
		public Guid CarrierCompanyGuid { get; set; }

		[DataMember]
		public Guid ShipperCompanyGuid { get; set; }

		[DataMember]
		public Guid ManagerCompanyGuid { get; set; }
		
		[DataMember]
		public Guid OwnerCompanyGuid { get; set; }

		[DataMember]
		public Guid SupplierCompanyGuid { get; set; }

		[DataMember]
		public Guid ShipToCompanyGuid { get; set; }

		[DataMember]
		public string CarrierCompanyId { get; set; }

		[DataMember]
		public string ShipperCompanyId { get; set; }

		[DataMember]
		public string ManagerId { get; set; }
		
		[DataMember]
		public string OwnerId { get; set; }

		[DataMember]
		public string SupplierId { get; set; }

		[DataMember]
		public string ShipToId { get; set; }

		[DataMember]
		public string ManagerFederalId { get; set; }

		[DataMember]
		public string SupplierFederalId { get; set; }

		[DataMember]
		public string ShipToFederalId { get; set; }

		[DataMember]
		public string ShipToState { get; set; }

		public bool IsTwoPartyExchange { get { return this.SubType.Equals(TwoPartyExchangeCode, StringComparison.OrdinalIgnoreCase); } }

		public bool IsPositive { get { return this.NetVolume >= 0.0;} }


		public ExStarsTransactionClass Clone(bool negateVolumeAmount)
		{
			ExStarsTransactionClass newTrans = (ExStarsTransactionClass)this.MemberwiseClone();
			if (negateVolumeAmount)
			{
				newTrans.GrossVolume *= -1;
				newTrans.NetVolume *= -1;
			}
			return newTrans;
		}


		public bool ValidateTransaction(out String errorMessage)
		{
			StringBuilder msg = new StringBuilder();
			if (Guid.Empty.Equals(this.ProductGuid))
			{
				msg.AppendLine("   No product is defined");
			}
			if (Guid.Empty.Equals(this.OwnerCompanyGuid))
			{
				msg.AppendLine("   No Owner is defined");
			}
			if (this.AliasName.Equals("Receipt", StringComparison.OrdinalIgnoreCase))
			{
				if (string.IsNullOrEmpty(EquipmentType))
				{
					msg.AppendLine("   EquipmentType is not defined");
				}
				if (Guid.Empty.Equals(this.SupplierCompanyGuid))
				{
					msg.AppendLine("   No Supplier is defined");
				}
				if (Guid.Empty.Equals(this.ShipperCompanyGuid))
				{
					msg.AppendLine("   No Shipper is defined");
				}
				if (this.BillOfLadingNumber == "")
				{
					msg.AppendLine("   No Bill Of Lading is defined");
				}
				if (string.IsNullOrEmpty(EquipmentType))
				{
					msg.AppendLine("   No shipping mode (equipment type) is defined");
				}
				else if (IsBargeOrShip)
				{
					if (this.EquipmentRegistrationId == "")
					{
						msg.AppendLine("   No ship name is defined");
					}
					if (this.EquipmentSerialNumber == "")
					{
						msg.AppendLine("   No ship registration number is defined");
					}

				}
			} else if (	   this.AliasName.Equals("Issue", StringComparison.OrdinalIgnoreCase)
						|| this.AliasName.Equals("Defuel", StringComparison.OrdinalIgnoreCase))
			{
				if (Guid.Empty.Equals(this.ShipToCompanyGuid))
				{
					msg.AppendLine("   No ShipTo company is defined");
				}
				if (Guid.Empty.Equals(this.CarrierCompanyGuid))
				{
					msg.AppendLine("   No Carrier company is defined");
				}
			}
			else if (this.AliasName.Equals("Adjustment", StringComparison.OrdinalIgnoreCase))
			{
				if (string.IsNullOrEmpty(EquipmentType))
				{
					msg.AppendLine("   No shipping mode (equipment type) is defined");
				}
				if (Guid.Empty.Equals(this.OwnerCompanyGuid))
				{
					msg.AppendLine("   No Owner company is defined");
				}
			}

			if (msg.ToString().Length > 0)
			{
				errorMessage = String.Format("Transaction {0} is missing {1}\n", TransId, msg.ToString());
				return false;
			}
			errorMessage = "";
			return true;
		}



		public string RollUpKey(EnumExStarsTrxType trxType)
		{
			switch (trxType)
			{
#if true
	
				case EnumExStarsTrxType.Receipt:
					// C_ExSTARS_X12_Schedule_Detail::Generate_Terminal_Receipts() ~ 1572
					return string.Format("{0};{1};{2};{3};{4};{5};"
						, this.OwnerId
						, this.ManagerId
						, this.ShipperCompanyId
						, this.TaxCode
						, this.EquipmentType
						, this.EquipmentRegistrationId
						);
				case EnumExStarsTrxType.Issue:
					return string.Format("{0};{1};{2};{3};{4};{5};"
						, this.ManagerId
						, this.OwnerId
						, this.CarrierCompanyId
						, this.ShipToId
						, this.TaxCode
						, this.EquipmentType
						);
				case EnumExStarsTrxType.BulkIssue:
					return string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};"
						, this.ManagerId
						, this.OwnerId
						, this.CarrierCompanyId
						, this.ShipToId
						, this.TaxCode
						, this.EquipmentType
						, this.IsTwoPartyExchange
						// C_ExSTARS_X12_Schedule_Detail::Generate_Bulk_Issues() ~ 3278
						, this.IsTwoPartyExchange? this.ShipToState : ""
						// C_ExSTARS_X12_Schedule_Detail::Generate_Bulk_Issues() ~ 3272
						, this.IsBargeOrShip? this.EquipmentRegistrationId : ""
						);
				case EnumExStarsTrxType.Adjustment:
					// C_ExSTARS_X12_Schedule_Detail::Generate_Book_Adjustments ~ 892
					return string.Format("{0};{1};{2};{3};{4};"
						, this.OwnerId
						, this.ManagerId
						, this.EquipmentType
						, this.TaxCode
						, IsPositive ? "+" : "-");
						


#else
				case EnumExStarsTrxType.Receipt:
					// C_ExSTARS_X12_Schedule_Detail::Generate_Terminal_Receipts() ~ 1572
					return string.Format("{0};{1};{2};{3};{4};{5};"
						, this.OwnerCompanyGuid.ToString()
						, this.ManagerCompanyGuid.ToString()
						, this.ShipperCompanyGuid.ToString()
						, this.TaxCode
						, this.EquipmentType
						, this.EquipmentRegistrationId
						);
				case EnumExStarsTrxType.Issue:
					return string.Format("{0};{1};{2};{3};{4};{5};"
						, this.ManagerCompanyGuid.ToString()
						, this.OwnerCompanyGuid.ToString()
						, this.CarrierCompanyGuid.ToString()
						, this.ShipToCompanyGuid.ToString()
						, this.TaxCode
						, this.EquipmentType
						);
				case EnumExStarsTrxType.BulkIssue:
					return string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};"
						, this.ManagerCompanyGuid.ToString()
						, this.OwnerCompanyGuid.ToString()
						, this.CarrierCompanyGuid.ToString()
						, this.ShipToCompanyGuid.ToString()
						, this.TaxCode
						, this.EquipmentType
						, this.IsTwoPartyExchange
						, this.ShipToState
						,this.EquipmentRegistrationId
						);

#endif
				default:
					throw new ExStarsBusinessException("Not Implemented: ExStarsTransactionListClass.RollUpKey({0})", trxType.ToString());
			}			
		}


		public string TaxInfoAmtKey( EnumExStarsTrxType transactionType)
		{
			switch (transactionType)
			{
				case EnumExStarsTrxType.Receipt:
					// C_ExSTARS_X12_Schedule_Detail::Generate_Terminal_Receipts() ~ 1584
					return string.Format("{0:0000}{1:00}{2:00};{3};{4};"
						, this.ReportYear
						, this.ReportMonth
						, this.ReportDay
						, this.BillOfLadingNumber
						, this.NetVolume < 0? "-" : "+"
						);
				case EnumExStarsTrxType.Issue:
				case EnumExStarsTrxType.BulkIssue:
					return string.Format("{0:0000}{1:00}{2:00};"
						, this.ReportYear
						, this.ReportMonth
						, this.ReportDay
						);
				default:
					throw new ExStarsBusinessException("Not Implemented: ExStarsTransactionListClass.TaxInfoAmtKey({0})", transactionType.ToString());
			}
		}



		public void TestIfValid()
		{
			// Trasaction validation:
			// Validate receipts for barge and ship
			//	GBL (ticket number) must not be blank
			//  must identify as ship or barge
			// Validate refueling
			//   Must have 2 char state indentifiction
			// Validate Bulk Issues
			//   ref: FD-Publ 3536-Motor Fuel Excise Tax EDI Guide-09	Rev 11-2005, page 14
			//   must have vessel name and registration ID
			// Validate shipping mode: must have valid IRS delivery mode or vehicle
			//	Shipping mode must be TRUCK or PIPELINE
			//	Carrier must be specified
			//	Carrier must be in database
			// Validate vehicle registration
			//	Eq must be in database
			// Validate broker trx
			//	must have ticket number
			//	must have shipping mode
			//	must have IRS delivery mode
			//
			// TBD: more checks
			//
			if (this.AliasIs(EnumExStarsTrxType.Receipt))
			{
				if (string.IsNullOrEmpty(this.BillOfLadingNumber))
				{
					throw new ExStarsReceiptTransactionException("Bill Of Lading Number (BillOfLadingNumber) must not be empty. Transaction \"{0}\"", this.TransactionGuid);
				}

				if ((this.EquipmentType.StartsWith("BARGE", StringComparison.CurrentCultureIgnoreCase)
				     || this.EquipmentType.StartsWith("SHIP", StringComparison.CurrentCultureIgnoreCase)))
				{
					if (string.IsNullOrEmpty(this.EquipmentRegistrationId))
					{
						throw new ExStarsReceiptTransactionException("For ships & barges, Registration ID must not be empty. Transaction \"{0}\"", this.TransactionGuid);
					}
					if (string.IsNullOrEmpty(this.EquipmentSerialNumber))
					{
						throw new ExStarsReceiptTransactionException("For ships & barges, Equipment Serial Number must not be emptyTransaction \"{0}\"", this.TransactionGuid);
					}
				}
			}
		}


	}
}
