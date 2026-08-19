
namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	public class ExStarsRollUpCollection : SortedDictionary<string, ExStarsRollUpTransaction>
	{
		public void Add(ExStarsBrokerTransferClass transaction, EnumExStarsTrxType transType)
		{
			string key = transaction.RollUpKey();
			if (!this.ContainsKey(key))
			{
				ExStarsRollUpTransaction newRollup = new ExStarsRollUpTransaction(transaction, transType);
				base.Add(key,newRollup);
				return;
			}
			ExStarsRollUpTransaction rollup = this[key];
			rollup.AddToSum(transaction, transType);
		}  

		public void Add(ExStarsTransactionClass transaction, EnumExStarsTrxType transType)
		{
			string key = transaction.RollUpKey(transType);
			if (!this.ContainsKey(key))
			{
				ExStarsRollUpTransaction newRollup = new ExStarsRollUpTransaction(transaction, transType);
				base.Add(key,newRollup);
				return;
			}
			ExStarsRollUpTransaction rollup = this[key];
			rollup.AddToSum(transaction, transType);
		}  		
	}


	public class ExStarsRollUpTransaction
	{
		#region Properties and member variables

		public String AliasName { get; protected set; }

		// Alias Defuel has TransactionType == Issue, so that the sum together
		public EnumExStarsTrxType TransactionType { get; protected set; }

		public CompanyClass ShipperCompany { get; protected set; }

		public CompanyClass ManagerCompany { get; protected set; }

		public CompanyClass OwnerCompany { get; protected set; }

		public CompanyClass SupplierCompany { get; protected set; }

		public CompanyClass CarrierCompany { get; protected set; }

		public CompanyClass CustomerCompany { get; set; }

		public EquipmentClass Equipment { get; protected set; }

		/// <summary>
		/// SupplierFederalId for broker transfers SupplierFederalId is NOT SupplierCompany.FederalId
		/// </summary>
		public string SupplierFederalId { get; protected set; }

		public string EquipmentType { get; protected set; }

		public string IrsTransportMode { get; protected set; }

		public string EquipmentRegistrationId { get; protected set; }

		public string EquipmentSerialNumber { get; protected set; }

		public bool IsBargeOrShip { get; protected set; }

		public ExStarsTransportMode.EnumStorage StorageType { get; protected set; }

		public int ReportYear { get; protected set; }

		public int ReportMonth { get; protected set; }

		public int ReportDay { get; protected set; }

		public Guid ProductGuid { get; protected set; }

		/// The ProductId is used only for diagnostic purposes, it means nothing to the IRS.
		/// Several ProductId's may have the same TaxCode, which is meaningful to the IRS
		public string ProductId { get; protected set; }

		public string TaxCode { get; protected set; }

		public bool IsTwoPartyExchange { get; protected set; }

		public bool IsNegativeDirection { get; protected set; }

		public bool AdjustmentUsedAsReceipt { get; protected set; }

		public double GrossVolume { get; protected set; }

		public double NetVolume { get; protected set; }

		public int SummaryCount
		{
			get
			{
				return TaxInfoTransSums.Count;
			}
		}

		protected static ExStarsSiteConfigExpanded Config = null;		

		public ExStarsTaxInfoTransactionSumCollection TaxInfoTransSums { get; protected set; }

		#endregion

		#region Constructors

		public ExStarsRollUpTransaction(ExStarsTransactionClass trx, EnumExStarsTrxType transactionType)
		{
			this.TaxInfoTransSums = new ExStarsTaxInfoTransactionSumCollection();
			this.TransactionType = transactionType;
			this.AliasName = trx.AliasName;

			this.EquipmentType = trx.EquipmentType;
			this.IrsTransportMode = trx.IrsTransportMode;
			this.EquipmentRegistrationId = trx.EquipmentRegistrationId;
			this.EquipmentSerialNumber = trx.EquipmentSerialNumber;
			this.IsBargeOrShip = trx.IsBargeOrShip;

			this.StorageType = Config.LookUpIrsTransportModeByEqTypeId(trx.EquipmentType).StorageType;

			this.ShipperCompany = Config.LookUpCompany(trx.ShipperCompanyGuid, nullIsOk: transactionType != EnumExStarsTrxType.Receipt);
			this.ManagerCompany = Config.LookUpCompany(trx.ManagerCompanyGuid);
			this.OwnerCompany = Config.LookUpCompany(trx.OwnerCompanyGuid);
			this.SupplierCompany = Config.LookUpCompany(trx.SupplierCompanyGuid, nullIsOk: transactionType != EnumExStarsTrxType.Receipt);

			// adjustment type 1:Adjustment from Primary Storage (Tanks, pipeline, etc)
			// C_ExSTARS_X12_Schedule_Detail::Generate_Book_Adjustments ~ 966
			// vendor (carrier) = manager
			// customer (ship to)  = owner
			//
			// adjustment type 2:Adjustment from Secondary Storage (Tankers, fuel trucks, etc)
			// C_ExSTARS_X12_Schedule_Detail::Generate_Book_Adjustments ~ 1116
			// vendor (carrier) = owner
			// customer (ship to)  = owner
			if (transactionType == EnumExStarsTrxType.Adjustment)
			{
				this.CustomerCompany = this.OwnerCompany;
				switch (this.StorageType)
				{
					case ExStarsTransportMode.EnumStorage.Primary:
						this.CarrierCompany = this.ManagerCompany;
						break;
					case ExStarsTransportMode.EnumStorage.Secondary:
						this.CarrierCompany = this.OwnerCompany;
						break;
					default:
						throw new ExStarsBusinessException("Primary/secondary storage is not defined for EquipmentType {1}", this.EquipmentType);
				}

			}
			else
			{
				this.CarrierCompany = Config.LookUpCompany(trx.CarrierCompanyGuid, nullIsOk: transactionType == EnumExStarsTrxType.Receipt);
				this.CustomerCompany = Config.LookUpCompany(trx.ShipToCompanyGuid, nullIsOk: transactionType == EnumExStarsTrxType.Receipt);
			}

			// SupplierFederalId for receipts use the FederalId from SupplierCompany, all others transactions, use from Manager
			// ref  ~ 1785, 3407
			this.SupplierFederalId = (transactionType == EnumExStarsTrxType.Receipt)
				? this.SupplierCompany.FederalID
				: this.ManagerCompany.FederalID;
			this.ReportYear = trx.ReportYear;
			this.ReportMonth = trx.ReportMonth;
			this.ReportDay = trx.ReportDay;
			this.ProductId = trx.ProductId;
			this.ProductGuid = trx.ProductGuid;
			this.TaxCode = trx.TaxCode;
			this.IsTwoPartyExchange = IsTwoPartyExchange;
			this.IsNegativeDirection = false;
			this.AdjustmentUsedAsReceipt = false;

			this.GrossVolume = 0.0;
			this.NetVolume = 0.0;
		}

		public ExStarsRollUpTransaction(ExStarsBrokerTransferClass trx, EnumExStarsTrxType transactionType)
		{
			System.Diagnostics.Debug.Assert(transactionType != EnumExStarsTrxType.Adjustment);
			this.TaxInfoTransSums = new ExStarsTaxInfoTransactionSumCollection();
			this.TransactionType = transactionType;
			this.AliasName = trx.AliasName;
			this.OwnerCompany = Config.LookUpCompany(trx.ToOwnerCompanyGuid);
			this.ManagerCompany = Config.LookUpCompany(trx.ManagerCompanyGuid);
			this.CustomerCompany = this.OwnerCompany;
			this.SupplierCompany = Config.LookUpCompany(trx.FromOwnerCompanyGuid);
			// Shipper moves product from a refinary to manager, not defined for broker transfers
			this.ShipperCompany = null;
			switch (transactionType)
			{
				case EnumExStarsTrxType.BrokerReceipt:
					// Broker_Receipt ~ 1779
					this.CarrierCompany = Config.LookUpCompany(trx.FromOwnerCompanyGuid);
					break;
				case EnumExStarsTrxType.BrokerDisbursement:
					// Broker_Disbursement ~ 3401
					this.CarrierCompany = ManagerCompany;
					break;
				default:
					throw new ExStarsBusinessException("ExStarsRollUpTransaction switch({0})", transactionType.ToString());
			}

			// SupplierFederalId for broker transfers SupplierFederalId is NOT SupplierCompany.FederalId
			// ref  ~ 1785, 3407
			this.SupplierFederalId = ManagerCompany.FederalID;

			this.ReportYear = trx.ReportYear;
			this.ReportMonth = trx.ReportMonth;
			this.ReportDay = trx.ReportDay;
			this.ProductId = trx.ProductId;
			this.ProductGuid = trx.ProductGuid;
			this.TaxCode = trx.TaxCode;
			this.IsTwoPartyExchange = false;
			this.IsNegativeDirection = false;
			this.AdjustmentUsedAsReceipt = false;
			this.EquipmentType = trx.EquipmentType;
			this.IrsTransportMode = trx.IrsTransportMode;
			this.EquipmentRegistrationId = trx.EquipmentRegistrationId;
			this.EquipmentSerialNumber = trx.EquipmentSerialNumber;
			this.IsBargeOrShip = trx.IsBargeOrShip;

			this.GrossVolume = 0.0;
			this.NetVolume = 0.0;
		}

		
		#endregion		
		
		/// <summary>
		/// Rather than pass this parameter a bizialian times, lets do it once and be done.
		/// </summary>
		/// <param name="config"></param>
		public static void SetConfig(ExStarsSiteConfigExpanded config)
		{
			Config = config;
		}

		public new string ToString()
		{
			return string.Format("{0};{1};{2}", AliasName, ManagerCompany.ID, OwnerCompany.ID);
		}

		/// <summary>
		/// If a ExStarsTaxInfoSum object with the correct date exists for this rollup, then update it, else create it
		/// </summary>
		/// <param name="brokerXfer"></param>
		/// <param name="transactionType"></param>
		public void AddToSum(ExStarsBrokerTransferClass brokerXfer, EnumExStarsTrxType transactionType)
		{
			string key = brokerXfer.TaxInfoAmtKey();

			if (!this.TaxInfoTransSums.ContainsKey(key))
			{
				TaxInfoTransSums.Add(key, new ExStarsTaxInfoSum(brokerXfer));
				//
				// the new keyed element is populated with the value, so exit now
				//
				return;
			}
			//
			// the code below follows the logic from the legacy code, but nothing actually uses the totals
			// ref C_ExSTARS_X12_Schedule_Detail::Generate_Terminal_Receipts() ~ 1744
			//
			if (transactionType == EnumExStarsTrxType.BrokerReceipt)
			{
				this.TaxInfoTransSums[key].NetBrokerReceipts += brokerXfer.NetVolume;
				this.TaxInfoTransSums[key].GrossBrokerReceipts += brokerXfer.GrossVolume;
			}
			// 
			// Ref C_ExSTARS_X12_Schedule_Detail::Generate_Bulk_Issues() ~ 3340, 3384
			//
			else if (transactionType == EnumExStarsTrxType.BrokerDisbursement)
			{
				this.TaxInfoTransSums[key].NetBrokerIssues += brokerXfer.NetVolume;
				this.TaxInfoTransSums[key].GrossBrokerIssues += brokerXfer.GrossVolume;				
			}
		}

		public void AddToSum(ExStarsTransactionClass transaction, EnumExStarsTrxType transactionType)
		{
			bool setNegative = (transactionType == EnumExStarsTrxType.Defuel);
			EnumExStarsTrxType sumerizeByTrxType = transactionType == EnumExStarsTrxType.Defuel
														   ? EnumExStarsTrxType.Issue
														   : transactionType;
			string key = transaction.TaxInfoAmtKey(sumerizeByTrxType);
			ExStarsTransactionClass transCopy = transaction.Clone(setNegative);

			if (!this.TaxInfoTransSums.ContainsKey(key))
			{
				TaxInfoTransSums.Add(key, new ExStarsTaxInfoSum(transCopy, setNegative));
			}
			else
			{
				this.TaxInfoTransSums[key].NetReceiptVolume += transCopy.NetVolume;
				this.TaxInfoTransSums[key].GrossReceiptVolume += transCopy.GrossVolume;
			}
			// ref C_ExSTARS_X12_Schedule_Detail::Generate_Book_Adjustments ~ 1062
			// for adjustments, update HydrantDisbursement
			if (transactionType == EnumExStarsTrxType.Adjustment)
			{
				this.TaxInfoTransSums[key].NetHydrantDisbursement += transCopy.NetVolume;
				this.TaxInfoTransSums[key].GrossHydrantDisbursement += transCopy.GrossVolume;
			}
		}


	}

}