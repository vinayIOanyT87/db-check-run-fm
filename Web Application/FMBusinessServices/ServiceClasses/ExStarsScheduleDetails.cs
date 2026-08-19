
namespace FMBusinessServices.ServiceClasses
{
	using System;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	/// <summary>
	/// See ExSTARS Document: IRS Publication 3536 Rev.11-2005 - Terminal Receipts-Schedule 15A
	/// Pages 13, 76
	/// Beginning of Terminal Operator Report Schedule Detail (TOR Tax Schedule Code T2/TFS/0100/TFS01 = T3 loop)
	/// This TFS loop begins the schedule detail. It repeats when one of the following values changes:
	/// Tax Form, Tax Schedule Code, Mode Code, Product Code, Carrier, Origin, Destination, Position Holder, Exchange Party.
	/// If there are no transactions to report in this filing, it is not necessary to transmit a Schedules TFS loop.
	/// 
	/// C_ExSTARS_X12_Transaction_Set::Generate_Schedule_Details() ~ 542
	/// </summary>
	[Serializable]
	public class ExStarsScheduleDetails : ExStarsTransactionReportLoopBase
	{
		// All referenced line number refer to /FuelsManager Aviation/Release v7.1 SP5/Core/ExSTARS Reporting Utility/ExSTARS_X12_Schedule_Detail.cpp 1     5/01/13 3:31p
	
		protected int TotalScheduleTransactionCount = 0;
		protected double TotalNetReportedTiaItems;
		protected ExStarsTerminalOperatorReportConfigClass ExStarsTerminalOperatorReportConfig;

		/// <summary>
		/// Required for serialization, do not use
		/// </summary>
		public ExStarsScheduleDetails() : base() { }

		/// <summary>
		/// Standard constructor
		/// </summary>		
		public ExStarsScheduleDetails(ExStarsSiteConfigExpanded config
			, ref int totalSegmentCount
			, ref double totalNetReportedTiaItems
			, ExStarsManagerTotals managerTotals
			, ref string validationErrors)
			: base(config, "SCHEDULE DETAILS REPORT", managerTotals, ref validationErrors)
		{
			ExStarsRollUpTransaction.SetConfig(config);
			this.TotalNetReportedTiaItems = totalNetReportedTiaItems;
			this.GenerateTerminalReceipts();
			this.GenerateDisbursements();
			this.GenerateAdjustments();
			totalSegmentCount += SegmentList.CountInUse();
			// return back the updated value
			totalNetReportedTiaItems = this.TotalNetReportedTiaItems;
			if (!IsValid)
			{
				validationErrors += this.ValidationErrors;
				System.Diagnostics.Debug.WriteLine(this.ValidationErrors);
			}
		}

		// 
		//  C_ExSTARS_X12_Schedule_Detail::Generate_Book_Adjustments ~ 827
		protected void GenerateAdjustments()
		{
			ExStarsRollUpCollection trxRollUps = new ExStarsRollUpCollection();
			const EnumExStarsTrxType trxType = EnumExStarsTrxType.Adjustment;
			
			ExStarsTransactionLoader trxLoader = new ExStarsTransactionLoader(Config, trxType);
			TotalScheduleTransactionCount += trxLoader.TransactionList.Count;
			foreach (ExStarsTransactionClass transaction in trxLoader.TransactionList)
			{
				try
				{
					string message;
					if (!transaction.ValidateTransaction(out message))
					{
						this.AppendMessage(message);
						continue;
					}
					trxRollUps.Add(transaction, trxType);
					// C_ExSTARS_X12_Schedule_Detail::Generate_Book_Adjustments ~ 917
					AllMgrTotals.AddtoSum(trxType, transaction.ManagerCompanyGuid, transaction.ProductGuid, transaction.GrossVolume, transaction.NetVolume);
				}			
				catch (Exception e)
				{
					Config.AppendError(ExStarsErrorSource.Transaction, "GenerateAdjustments transaction {0} {1} ", transaction.TransId, e.Message);
				}

			}

			// ref: ~ 1161
			foreach (var rollUpRcpt in trxRollUps.Values)
			{
				if (rollUpRcpt.SummaryCount == 0)
				{
					continue;
				}
				SegmentList.Add(new ExStarsComment(this.TransactionAdjustmentDescription(rollUpRcpt.ProductId
					, rollUpRcpt.TaxCode
					, rollUpRcpt.IsNegativeDirection ? ExStarsConstants.TFS02_TerminalDisbursements : ExStarsConstants.TFS02_TerminalReceipts
					, rollUpRcpt.ManagerCompany
					, rollUpRcpt.OwnerCompany
					, rollUpRcpt.CustomerCompany
					, rollUpRcpt.CarrierCompany
					, rollUpRcpt.IrsTransportMode
					, rollUpRcpt.SummaryCount)));
				SegmentList.AddRange(GenerateReport(SegmentStateEnum.Begin, rollUpRcpt, EnumExStarsTrxType.Adjustment));

				// for a single manager, owner, customer, vendor, equipmenttype report by date
				// ~ 1289
				foreach (ExStarsTaxInfoSum taxInfoRcpt in rollUpRcpt.TaxInfoTransSums.Values)
				{
					try
					{ 
						// C_ExSTARS_X12_Schedule_Detail::Generate_Book_Adjustments
						SegmentList.AddRange(this.GenerateShippingInformation(SegmentStateEnum.ShippingDocumentNumber, rollUpRcpt, taxInfoRcpt, EnumExStarsTrxType.Issue));
					}
					catch (Exception e)
					{
						Config.AppendError(ExStarsErrorSource.Transaction, "Generate_Book_Adjustments {0}/{1}/{2} {3} ", taxInfoRcpt.ReportYear, taxInfoRcpt.ReportMonth, taxInfoRcpt.ReportDay, e.Message);
					}
				}
			}
		}



		protected void GenerateDisbursements()
		{
			//
			// Hydrant Disbursements  ref: C_ExSTARS_X12_Schedule_Detail::Generate_Disbursements ~ 2324
			// Hydrant Defuels  ref: C_ExSTARS_X12_Schedule_Detail::Generate_Disbursements ~ 2521
			// C_ExSTARS_X12_Schedule_Detail::Generate_Bulk_Issues ~ 3130
			//
			// C_Rec_HydrantDisbursements
			// hydrant disbursements rollups as an AH and truck disbursements rollups as an AJ 
			// FD-Publ 3536-Motor Fuel Excise Tax EDI Guide-09
			ExStarsRollUpCollection trxRollUps = new ExStarsRollUpCollection();

			foreach (EnumExStarsTrxType trxType in new EnumExStarsTrxType[] 
				{ EnumExStarsTrxType.BulkIssue, EnumExStarsTrxType.Issue, EnumExStarsTrxType.Defuel })
			{
				// Defuels and lumped together with Issues
				EnumExStarsTrxType sumerizeByTrxType = trxType == EnumExStarsTrxType.Defuel
					                                       ? EnumExStarsTrxType.Issue
					                                       : trxType;
				ExStarsTransactionLoader trxLoader = new ExStarsTransactionLoader(Config, trxType);
				TotalScheduleTransactionCount += trxLoader.TransactionList.Count;
				foreach (ExStarsTransactionClass transaction in trxLoader.TransactionList)
				{
					string message;
					try
					{
						if (!transaction.ValidateTransaction(out message))
						{
							this.AppendMessage(message);
							continue;
						}
						trxRollUps.Add(transaction, trxType);
						// ref ~ 1618, Generate_Bulk_Issues() ~ 3230
						AllMgrTotals.AddtoSum(sumerizeByTrxType, transaction.ManagerCompanyGuid, transaction.ProductGuid, transaction.GrossVolume, transaction.NetVolume);
					}			
					catch (Exception e)
					{
						Config.AppendError(ExStarsErrorSource.Transaction, " GenerateDisbursements transaction {0} {1} ", transaction.TransId, e.Message);
					}

				}
			}

			//
			// Broker transfers C_ExSTARS_X12_Schedule_Detail::Generate_Bulk_Issues() ~ 3236
			//
			ExStarsBrokerTransferLoader brokerTrxLoader = new ExStarsBrokerTransferLoader(Config, useFromToOwner: true);
			TotalScheduleTransactionCount += brokerTrxLoader.TransactionList.Count;
			foreach (ExStarsBrokerTransferClass brokerTrx in brokerTrxLoader.TransactionList)
			{
				trxRollUps.Add(brokerTrx, EnumExStarsTrxType.BrokerDisbursement);
				AllMgrTotals.AddtoSum(EnumExStarsTrxType.BrokerDisbursement, brokerTrx.ManagerCompanyGuid, brokerTrx.ProductGuid, brokerTrx.GrossVolume, brokerTrx.NetVolume);
			} 

			// ref: 2774, loop to create TFS segments
			foreach (var rollUpRcpt in trxRollUps.Values)
			{
				if (rollUpRcpt.SummaryCount == 0)
				{
					continue;
				}
				// ref ~2834
				bool useVendorForCustomer = rollUpRcpt.CustomerCompany == null && rollUpRcpt.CarrierCompany != null;
				if (useVendorForCustomer)
				{
					rollUpRcpt.CustomerCompany = rollUpRcpt.CarrierCompany;
				}

				SegmentList.Add(new ExStarsComment(this.TransactionTotalsDescription(rollUpRcpt.ProductId
					, useVendorForCustomer
					, rollUpRcpt.TaxCode
					, ExStarsConstants.TFS02_TerminalDisbursements
					, rollUpRcpt.ManagerCompany
					, rollUpRcpt.OwnerCompany
					, rollUpRcpt.CustomerCompany
					, rollUpRcpt.CarrierCompany
					, rollUpRcpt.IrsTransportMode
					, rollUpRcpt.SummaryCount)));
				try
				{
					SegmentList.AddRange(GenerateReport(SegmentStateEnum.Begin, rollUpRcpt, EnumExStarsTrxType.Issue));
				}
				catch (Exception e)
				{
					Config.AppendError(ExStarsErrorSource.Transaction, "Generate_Disbursements: loop to create TFS segments{0}", e.Message);
				}

				// for a single manager, owner, customer, vendor, equipmenttype report by date
				// ~ 2929
				foreach (ExStarsTaxInfoSum taxInfoRcpt in rollUpRcpt.TaxInfoTransSums.Values)
				{
					try
					{
						// C_ExSTARS_X12_Schedule_Detail::Generate_Disbursements
						SegmentList.AddRange(this.GenerateShippingInformation(SegmentStateEnum.ShippingDocumentNumber, rollUpRcpt, taxInfoRcpt, EnumExStarsTrxType.Issue));
					}
					catch (Exception e)
					{
						Config.AppendError(ExStarsErrorSource.Transaction, "Generate_Disbursements{0}/{1}/{2} {3} ", taxInfoRcpt.ReportYear, taxInfoRcpt.ReportMonth, taxInfoRcpt.ReportDay, e.Message);
					}
				}
			}
		}


		protected void GenerateTerminalReceipts()
		{
			//
			// Receipt transactions
			//
			// C_ExSTARS_X12_Schedule_Detail::Generate_Terminal_Receipts() ~ 1505
			ExStarsRollUpCollection rollUps = new ExStarsRollUpCollection();
			ExStarsTransactionLoader receiptTrxLoader = new ExStarsTransactionLoader(Config, EnumExStarsTrxType.Receipt);
			TotalScheduleTransactionCount += receiptTrxLoader.TransactionList.Count;

			foreach (ExStarsTransactionClass receiptTrx in receiptTrxLoader.TransactionList)
			{
				string message;
				if (!receiptTrx.ValidateTransaction(out message))
				{
					this.AppendMessage(message);
				}
				else
				{
					rollUps.Add(receiptTrx, EnumExStarsTrxType.Receipt);
					// ref ~ 1618
					AllMgrTotals.AddtoSum(EnumExStarsTrxType.Receipt, receiptTrx.ManagerCompanyGuid, receiptTrx.ProductGuid, receiptTrx.GrossVolume, receiptTrx.NetVolume);
				}
			}

			//
			// Broker transfers C_ExSTARS_X12_Schedule_Detail::Generate_Terminal_Receipts() ~1685
			// Refueler & Broker Transger
			ExStarsBrokerTransferLoader brokerTrxLoader = new ExStarsBrokerTransferLoader(Config, useFromToOwner: false);
			TotalScheduleTransactionCount += brokerTrxLoader.TransactionList.Count;
			foreach (ExStarsBrokerTransferClass brokerTrx in brokerTrxLoader.TransactionList)
			{
				try
				{ 
					rollUps.Add(brokerTrx, EnumExStarsTrxType.BrokerReceipt);
					AllMgrTotals.AddtoSum(EnumExStarsTrxType.BrokerDisbursement, brokerTrx.ManagerCompanyGuid, brokerTrx.ProductGuid, brokerTrx.GrossVolume, brokerTrx.NetVolume);
				}
				catch (Exception e)
				{

					Config.AppendError(ExStarsErrorSource.Transaction, "Generate_Terminal_Receipts: brokerReceipts{0}", e.Message);
				}

			} 

			// ref: 1833, loop to create TFS segments
			foreach (ExStarsRollUpTransaction rollUpRcpt in rollUps.Values)
			{
				if (rollUpRcpt.SummaryCount == 0)
				{
					continue;
				}
				SegmentList.Add(new ExStarsComment(this.ReceiptDescription(rollUpRcpt.ProductId
					, rollUpRcpt.TaxCode
					, ExStarsConstants.TFS02_TerminalReceipts
					, rollUpRcpt.ManagerCompany
					, rollUpRcpt.SupplierCompany
					, rollUpRcpt.OwnerCompany
					, rollUpRcpt.ShipperCompany
					, rollUpRcpt.IrsTransportMode
					, rollUpRcpt.SummaryCount)));
				SegmentList.AddRange(GenerateReport(SegmentStateEnum.Begin, rollUpRcpt, EnumExStarsTrxType.Receipt));

				// for a single manager, supplier, owner, shipper, equipmenttype report by date
				foreach (ExStarsTaxInfoSum taxInfoRcpt in rollUpRcpt.TaxInfoTransSums.Values)
				{
					// _ExSTARS_X12_Schedule_Detail::Generate_Terminal_Receipts ~ 2006, 2019
					SegmentList.AddRange(this.GenerateShippingInformation(SegmentStateEnum.ShippingDocumentNumber, rollUpRcpt, taxInfoRcpt, EnumExStarsTrxType.Receipt));
				}
			}
		}



		protected string TransactionTotalsDescription(string productId, bool useVendorForCustomer, string taxCode, string irsScheduleType
			, CompanyClass manager
			, CompanyClass owner
			, CompanyClass shipToCompany
			, CompanyClass carrier
			, string equipmentType
			, int count
			)
		{


			return string.Format(
@"--------------------------------------------------------------------------------------------------
This section represents {11} daily transaction totals for a {12}consortium customer.
The system reports the {13} as the cosignor for these daily transaction totals in the raw EDI file.
----------------------------------------------------------------------------------------------------
// ProductID: {0}
// IRS Product Code: {1}
// IRS Schedule Type Code: {2}
// Manager: {3}
// Manager TCN: {4}
// Manager State: {5}
// Supplier: {6}
// Supplier State: {7}
// {12}Consortium Customer: {8}
// Carrier: {9}
// IRS Transaction Type Mode Code: {10}
....................................................................................................",
				productId,
				taxCode,
				irsScheduleType,
				manager.ID,
				Config.TerminalControlNumber,
				manager.State,
				owner.ID,
				owner.State,
				shipToCompany.ID,
				carrier.ID,
				equipmentType,
				count,
				useVendorForCustomer? "Non-" : "",
				useVendorForCustomer? "vendor" : "customer"
				);
		}

		protected string TransactionAdjustmentDescription(string productId
			, string taxCode
			, string irsScheduleType
			, CompanyClass manager
			, CompanyClass owner
			, CompanyClass customer
			, CompanyClass carrier
			, string equipmentType
			, int count)
		{
			// adjustment type 1:
			// vendor (carrier) = manager
			// customer (ship to)  = owner
			//
			// adjustment type 2:
			// vendor (carrier) = owner
			// customer (ship to)  = owner
			return string.Format(
@"---------------------------------------------------------------------------------
This section represents {10} daily Book Adjustment totals for a consortium owner.
----------------------------------------------------------------------------------
// ProductID: {0}
// IRS Product Code: {1}
// IRS Schedule Type Code: {2}
// Manager: {3}
// Manager TCN: {4}
// Manager State: {5}
// Owner: {6}
// Cosignor: {7}
// Carrier: {8}
// IRS Transaction Type Mode Code: {9}
..................................................................................",
				productId,
				taxCode,
				irsScheduleType,
				manager.ID,
				Config.TerminalControlNumber,
				manager.State,
				owner.ID,
				customer.ID,
				carrier.ID,
				equipmentType,
				count
				);
		}


		protected string ReceiptDescription( string productId,  string taxCode, string irsScheduleType
			, CompanyClass manager
			, CompanyClass supplier
			, CompanyClass owner
			, CompanyClass shipper
			, string equipmentType
			, int count
			)
		{
			return string.Format(
@"--------------------------------------------------------
This section represents {11} receipt transactions for a consortium customer.
The system reports the owner as the cosignor for these transactions in the raw EDI file.
-------------------------------------------------------
// ProductID: {0}
// IRS Product Code: {1}
// IRS Schedule Type Code: {2}
// Manager: {3}
// Manager TCN: {4}
// Manager State: {5}
// Supplier: {6}
// Supplier State: {7}
// Consortium Owner: {8}
// Shipper: {9}
// IRS Transaction Type Mode Code: {10}
.......................................................",
				productId,
				taxCode,
				irsScheduleType,
				manager.ID,
				Config.TerminalControlNumber,
				manager.State,
				supplier.ID,
				supplier.State,
				owner.ID,
				shipper.ID,
				equipmentType,
				count);
		}


		protected SegmentList GenerateReport(SegmentStateEnum startingSegment, ExStarsRollUpTransaction rollupTransaction, EnumExStarsTrxType trxType)
		{
			SegmentList segmentList = new SegmentList();

			// ref C_ExSTARS_X12_Schedule_Detail::Generate_Shipping_Information() ~ 817
			for (SegmentStateEnum segmentStateEnum = startingSegment;
				 segmentStateEnum != SegmentStateEnum.Finish;
				 segmentStateEnum = this.NextSegmentState(segmentStateEnum))
			{
				switch (segmentStateEnum)
				{
					//--- VVV Start Summary by company hierarchy reglardless of date
					case SegmentStateEnum.Begin:
						SegmentList.Add(this.CreateTaxFormSegment(rollupTransaction, trxType));
						break;

					case SegmentStateEnum.RelationshipToTheInformation:
						switch (trxType)
						{
							case EnumExStarsTrxType.Receipt:
                                //For receipts it is likely that users will not know the TCN for the supplying company.
                                //So passing null for the fromCompany parameter will ensure that the From State element
                                //does not get added.
								SegmentList.Add(this.CreateRelationshipToTheInformation(null, rollupTransaction.ManagerCompany)); // , transaction));
								break;
							case EnumExStarsTrxType.Issue:
								SegmentList.Add(this.CreateRelationshipToTheInformation(rollupTransaction.ManagerCompany, rollupTransaction.CustomerCompany)); //, transaction));
								break;
							default:
								throw new ExStarsBusinessException("ExStarsScheduleDetails.GenerateReport switch({0}", trxType.ToString());
						}
						break;

					case SegmentStateEnum.SequenceErrorToIdNumberRtti:
						SegmentList.Add(CreateSequenceErrorToIdNumberRtti());
						break;

					case SegmentStateEnum.PositionHolderFein:
						// For Terminal Receipts  of Product Code E00, Bxx, or D00, the Position Holder 
						// must be reported using one iteration of the N1 segment
						// ref p 80
						if (rollupTransaction.TaxCode[0] == 'E' || rollupTransaction.TaxCode[0] == 'B' || rollupTransaction.TaxCode[0] == 'D')
						{
							SegmentList.Add(this.CreateCompanyNameSegment(ExStarsConstants.N101_PositionHolder, rollupTransaction.CarrierCompany, rollupTransaction.SupplierFederalId, "PositionHolderFein", "Carrier"));
						}
						break;

					case SegmentStateEnum.Option1OriginTerminal:
						// C_ExSTARS_X12_Schedule_Detail::~C_ExSTARS_X12_Schedule_Detail() ~225
						// ref p 53
						if (trxType != EnumExStarsTrxType.Receipt)
						{
							SegmentList.Add(this.CreateOriginTerminalSegment());							
						}
						break;

					case SegmentStateEnum.CarrierInformation:
						// EDI Guide pg 52,  What the IRS calls a carrier we call a shipper
						// SupplierFederalId for receipts use the FederalId from SupplierCompany, all others transactions, use from Manager
						// ref  ~ 1785, 3407
						switch (trxType)
						{
							case EnumExStarsTrxType.Receipt:
								// ~ 1945
								SegmentList.Add(this.CreateCompanyNameSegment(ExStarsConstants.N101_Carrier, rollupTransaction.ShipperCompany, rollupTransaction.SupplierFederalId, "Receipt", "Shipper"));
								break;
							case EnumExStarsTrxType.Issue:
								// ~ 2844
								{
									SegmentList.Add(this.CreateCompanyNameSegment(ExStarsConstants.N101_Carrier, rollupTransaction.CarrierCompany, rollupTransaction.CarrierCompany.FederalID, "Issue", "Carrier"));
								}
								break;
							case EnumExStarsTrxType.Adjustment:
								SegmentList.Add(this.CreateCompanyNameSegment(ExStarsConstants.N101_Carrier, rollupTransaction.CarrierCompany, rollupTransaction.CarrierCompany.FederalID, "Adjustment", "Carrier"));
								break;
							default:
								throw new ExStarsBusinessException("ExStarsScheduleDetails.GenerateReport() switch ({0)", trxType.ToString());
						}
						break;

					case SegmentStateEnum.TwoPartyExchange:
						// ref: C_ExSTARS_X12_Schedule_Detail::Generate_Bulk_Issues()  ~3541
						// ref: C_ExSTARS_X12_Schedule_Detail::Generate_Segment() ~278
						if (rollupTransaction.IsTwoPartyExchange)
						{
							SegmentList.Add(this.CreateCompanyNameSegment(ExStarsConstants.N101_Exchanger, rollupTransaction.CustomerCompany, rollupTransaction.CustomerCompany.FederalID, "Exchanger", "Customer")); // broker.customer));
						}
						break;

					case SegmentStateEnum.Option2DestinationState:
						// ref C_ExSTARS_X12_Schedule_Detail::Generate_Segment() ~ 326
						// per email from Patricia Shiamone ExSTARS EDI Helpdesk 3/10/2011
						// N1~ST is used for NON BULK Disbursements only.  Non-bulk transactions are by Truck or Rail. 
						// (update 3/18/2011 from Patricia Shiamone)  ,.. or by hydrant
						ExStarsTransportMode irsModeCode = Config.LookUpIrsTransportModeByEqTypeId(rollupTransaction.EquipmentType);
						if (trxType == EnumExStarsTrxType.Issue
							&& ( irsModeCode.IsIrsRail
								|| irsModeCode.IsIrsHydrant
								|| irsModeCode.IsIrsTruck))
						{
							SegmentList.Add(this.CreateNameShipToStateSegment(rollupTransaction.CustomerCompany.State));
						}
						break;

					case SegmentStateEnum.Option1DestinationTerminal:
						// ref ~ 309, receipts only
						SegmentList.Add(this.CreateDestinationTerminalSegment());
						break;

					//--- ^^^ End of Summary by company hierarchy reglardless of date
				}
			}
			return segmentList;

		}
		protected SegmentList GenerateShippingInformation(SegmentStateEnum startingSegment, ExStarsRollUpTransaction rollUpTrx,  ExStarsTaxInfoSum taxInfoSum, EnumExStarsTrxType trxType) // ExStarsBaseTransactionClass baseTransaction)
		{
			SegmentList segmentList = new SegmentList();

			// ref C_ExSTARS_X12_Schedule_Detail::Generate_Shipping_Information() ~ 817
			for (SegmentStateEnum segmentStateEnum = startingSegment;
				 segmentStateEnum != SegmentStateEnum.Finish;
				 segmentStateEnum = this.NextSegmentState(segmentStateEnum))
			{
				double grossAmountsAreNotTotaled = 0.0;
				switch (segmentStateEnum)
				{
					//--- VVV Shipping seqment Sequence by date ----

					case SegmentStateEnum.ShippingDocumentNumber:
						SegmentList.Add(this.CreateFormGroupSegment(taxInfoSum.BillOfLadingNumber));
						break;

					case SegmentStateEnum.SequenceErrorIdNumberSdn:
						SegmentList.Add(CreateSequenceErrorToIdNumberRtti());
						break;

					case SegmentStateEnum.ShippingDocumentDate:
						if (this.Config.IsNotCorrectionOrHasReferencedError())
						{
							SegmentList.Add(new ExStarsDateTimeSegment(ExStarsConstants.DTM01_BillOfLadingDate, taxInfoSum.ReportYear, taxInfoSum.ReportMonth, taxInfoSum.ReportDay));
						}
						break;

					case SegmentStateEnum.ShippingVesselName:
						if (rollUpTrx.IsBargeOrShip)
						{
							// ref pg 109
							SegmentList.Add(CreateNameSegment(
								ExStarsConstants.N101_VesselName
								, rollUpTrx.EquipmentRegistrationId.ToUpper()
								, ExStarsConstants.N103_TransportShipperCode
								, rollUpTrx.EquipmentSerialNumber.ToUpper()
								, minIdLength: 1
								, maxIdLength: 9));
						}
						break;

					case SegmentStateEnum.ShippingDocumentNetGallons:
						SegmentList.Add(CreateAmountSegment(MeasurementBeingTaxed.Net, taxInfoSum.NetReceiptVolume, ref this.TotalNetReportedTiaItems));
						break;

					case SegmentStateEnum.ShippingDocumentGrossGallons:
						SegmentList.Add(CreateAmountSegment(MeasurementBeingTaxed.Gross, taxInfoSum.GrossReceiptVolume, ref grossAmountsAreNotTotaled));
						break;
				}
			}
			return segmentList;

		}


		protected ExStarsSegment CreateTaxFormSegment(ExStarsRollUpTransaction transaction, EnumExStarsTrxType trxType)
		{
			// Ref C_ExSTARS_X12_Base_Segments::Generate_Tax_Form_Segment() ~1488
			if (!this.Config.IsNotCorrectionOrHasReferencedError())
			{
				return null;
			}
			// ~2389, ~2278
			string irsTransportModeCode = Config.IrsModeCode( transaction.EquipmentType);
			string tfs02Value;
			switch (trxType)
			{
				case EnumExStarsTrxType.Receipt:
					tfs02Value = ExStarsConstants.TFS02_TerminalReceipts;
					break;
				case EnumExStarsTrxType.Issue:
				case EnumExStarsTrxType.Defuel:
				case EnumExStarsTrxType.BulkIssue:
					tfs02Value = ExStarsConstants.TFS02_TerminalDisbursements;
					break;
				default:
					throw new ExStarsBusinessException("ExStarsScheduleDetails.CreateTaxFormSegment() switch ({0)", trxType.ToString());
			}

			// ExSTARS pg 76, C_ExSTARS_X12_Schedule_Detail::Generate_Terminal_Receipts() ~ 1915
			ExStarsSegment segment = new ExStarsSegment("TFS", "TFS loop begins the schedule detail");
			segment.AddElement(1, "Reference Identification Qualifier", "", EnumExStarsElementTypes.ID, 2, 2, ExStarsConstants.TFS01_TaxScheduleCode);
			segment.AddElement(2, "Reference Identification", "", EnumExStarsElementTypes.AN, 3, 3, tfs02Value);
			segment.AddElement(3, "Reference Identification Qualifier", "", EnumExStarsElementTypes.ID, 2, 2, ExStarsConstants.FGS02_ProductGroup);
			segment.AddElement(4, "Reference Identification", "", EnumExStarsElementTypes.AN, 3, 3, transaction.TaxCode);
			segment.AddElement(5, "ID Code Qualifier", "", EnumExStarsRequired.X, EnumExStarsElementTypes.ID, 2, 2, ExStarsConstants.TFS05_IdentCodeQualifier);
			segment.AddElement(6, "ID Code", "IRS Transaction Type Mode Code", EnumExStarsRequired.X, EnumExStarsElementTypes.AN, 2, 9, irsTransportModeCode);
			return segment;
		}

		protected ExStarsSegment CreateRelationshipToTheInformation(CompanyClass fromCompany, CompanyClass toCompany) //Guid fromCompanyGuid, Guid toCompanyGuid, ExStarsTransactionClass receipt)
		{
			// C_ExSTARS_X12_Base_Segments::Generate_Relationship_to_the_Information_Segment() ~195
			// ref page 77
			// 
			// When reporting terminal non-bulk disbursements use composite REF04-02 to report the state 
			// in which the terminal is located (Manager) and composite REF04-04 to report the state in which 
			// the product is delivered (ShipTo).
			// If either REF04-03 or REF04-04 is present, then the other is required.
			//CompanyClass fromCompany = Config.LookUpCompany(fromCompanyGuid, receipt.TransId, receipt.AliasName + " fromCompany");
			//CompanyClass toCompany = Config.LookUpCompany(toCompanyGuid, receipt.TransId, receipt.AliasName + " toCompany");

			// ref C_ExSTARS_X12_Base_Segments::Generate_Relationship_to_the_Information_Segment() ~ 222
			if (this.Config.IsNotCorrectionOrHasReferencedError())
			{
				ExStarsSegment segment = new ExStarsSegment("REF", "Reference Identification");
				segment.AddElement(1, "Reference Identification Qualifier", "SU = Specific Processing", EnumExStarsElementTypes.ID, 2, 2, ExStarsConstants.REF01_SpecialProcessing);
				segment.AddElement(2, "Reference Identification", "IRS = IRS Data or N/A = if not applicable to IRS", EnumExStarsElementTypes.AN, 3, 3, "IRS");
				// not used: #3
				segment.AddElement(4, "Reference Identification", "", EnumExStarsRequired.O, EnumExStarsElementTypes.MultiPart);
                if (fromCompany != null)
                {
				segment.AppendSubElement(4, 1, "Reference Identification Qualifier", "S0 = Special Approval", EnumExStarsRequired.M, EnumExStarsElementTypes.ID, 2, 2, ExStarsConstants.REF0401_SpecialApproval, null);
				segment.AppendSubElement(4, 2, "Reference Identification ", "State (Abbreviation) to which the terminal is located.", EnumExStarsRequired.M, EnumExStarsElementTypes.AN, 2, 2, fromCompany.State, fromCompany);
				segment.AppendSubElement(4, 3, "Reference Identification Qualifier", "S0 = Special Approval", EnumExStarsRequired.M, EnumExStarsElementTypes.ID, 2, 2, ExStarsConstants.REF0401_SpecialApproval, null);
				segment.AppendSubElement(4, 4, "Reference Identification ", "State (Abbreviation) to which the product is delivered", EnumExStarsRequired.M, EnumExStarsElementTypes.AN, 2, 2, toCompany.State, toCompany);
                }
                else
                {
                    segment.AppendSubElement(4, 1, "Reference Identification Qualifier", "S0 = Special Approval", EnumExStarsRequired.M, EnumExStarsElementTypes.ID, 2, 2, ExStarsConstants.REF0401_SpecialApproval, null);
                    segment.AppendSubElement(4, 2, "Reference Identification ", "State (Abbreviation) to which the product is delivered", EnumExStarsRequired.M, EnumExStarsElementTypes.AN, 2, 2, toCompany.State, toCompany);
                }

				// for bulk issues subelemets 3,4 will be set, C_ExSTARS_X12_Schedule_Detail::Generate_Bulk_Issues() ~ 3163
				return segment;
			}
			return null;
		}

		protected ExStarsSegment CreateCompanyNameSegment(string companyRoleDesignator, CompanyClass company,  string idCode, string transId, string fieldName)
		{
			// ref pg 79, 80
			return CreateNameSegment(companyRoleDesignator, company.ID, ExStarsConstants.N103_EIN, idCode);
		}


		protected ExStarsSegment CreateNameSegment(string entityIdCode, string name, string idCodeQualifier, string idCode, int minIdLength = 9, int maxIdLength = 18)
		{
			if (!this.Config.IsNotCorrectionOrHasReferencedError())
			{
				return null;
			}
			ExStarsSegment segment = new ExStarsSegment("N1", "Name");
			segment.AddElement(1, "Entity Identification Code", "", EnumExStarsElementTypes.ID, 2, 2, entityIdCode);
			segment.AddElement(2, "Name", "First 4 of company Name", EnumExStarsElementTypes.AN, 1, 35, name.Left(4));
			segment.AddElement(3, "Identification Code Qualifier", "", EnumExStarsElementTypes.ID, 2, 2, idCodeQualifier);
			segment.AddElement(4, "Identification Code: EIN", name, EnumExStarsElementTypes.AN, minIdLength, maxIdLength, idCode);
			return segment;
		}


		protected ExStarsSegment CreateNameShipToStateSegment(string companyState)
		{
			if (!this.Config.IsNotCorrectionOrHasReferencedError())
			{
				return null;
			}
			ExStarsSegment segment = new ExStarsSegment("N1", "Name");
			segment.AddElement(1, "Entity Identification Code", "", EnumExStarsElementTypes.ID, 2, 2, ExStarsConstants.N101_ShipTo);
			segment.AddElement(2, "Name", "2 char state of ship-to company", EnumExStarsElementTypes.AN, 1, 35, companyState);
			return segment;
		}

		protected ExStarsSegment CreateDestinationTerminalSegment()
		{
			if (!this.Config.IsNotCorrectionOrHasReferencedError())
			{
				return null;
			}
			// ref pg 106
			ExStarsSegment segment = new ExStarsSegment("N1", "Name");
			segment.AddElement(1, "Entity Identification Code", "", EnumExStarsElementTypes.ID, 2, 2, ExStarsConstants.N101_DestinationTerminal);
			// there is no #2
			segment.AddElement(3, "Identification Code Qualifier", "", EnumExStarsElementTypes.ID, 2, 2, ExStarsConstants.N103_IrsFacilityCode);
			segment.AddElement(4, "Identification Code", "Terminal Control Number", EnumExStarsElementTypes.AN, 9, 9, Config.TerminalControlNumber);
			return segment;
		}

		protected ExStarsSegment CreateOriginTerminalSegment()
		{
			if (!this.Config.IsNotCorrectionOrHasReferencedError())
			{
				return null;
			}
			// ref pg 53
			ExStarsSegment segment = new ExStarsSegment("N1", "Name");
			segment.AddElement(1, "Entity Identification Code", "", EnumExStarsElementTypes.ID, 2, 2, ExStarsConstants.N101_OriginTerminal);
			// there is no #2
			segment.AddElement(3, "Identification Code Qualifier", "", EnumExStarsElementTypes.ID, 2, 2, ExStarsConstants.N103_IrsFacilityCode);
			segment.AddElement(4, "Identification Code", "Terminal Control Number", EnumExStarsElementTypes.AN, 9, 9, Config.TerminalControlNumber);
			return segment;
		}

		protected ExStarsSegment CreateFormGroupSegment(string billOfLadingNumber)
		{
			// ref C_ExSTARS_X12_Base_Segments::Generate_Form_Group_Segment() ~509
			if (!this.Config.IsNotCorrectionOrHasReferencedError())
			{
				return null;
			}
			//  C_ExSTARS_X12_Schedule_Detail::Generate_Segment()
			// ref page 107
			ExStarsSegment segment = new ExStarsSegment("FGS", "Form Group");
			segment.AddElement(1, "Assigned Identification", "D = Schedule Detail", EnumExStarsElementTypes.ID, 1, 2, ExStarsConstants.FGS01_ScheduleDetail);
			segment.AddElement(2, "Reference Identification Qualifier", "PG = Product Group", EnumExStarsRequired.X, EnumExStarsElementTypes.ID, 2, 2, ExStarsConstants.FGS02_BillOfLading);
			segment.AddElement(3, "Reference Identification", "IRS Product Group", EnumExStarsRequired.X, EnumExStarsElementTypes.AN, 1, 15, billOfLadingNumber.Left(15));
			return segment;
		}




		private SegmentStateEnum NextSegmentState(SegmentStateEnum current)
		{
			// For any match to current, use the next value in the list, its OK to have the same value multiple times because:
			// Abort -> Finish
			// Finish -> Finish
			SegmentStateEnum[] stateSequence = new SegmentStateEnum[]
				                                   {
					                                   SegmentStateEnum.Abort,
					                                   SegmentStateEnum.Finish,
					                                   SegmentStateEnum.Finish,
					                                   SegmentStateEnum.Begin,
					                                   SegmentStateEnum.RelationshipToTheInformation,
					                                   SegmentStateEnum.SequenceErrorToIdNumberRtti,
													   SegmentStateEnum.PositionHolderFein,
													   SegmentStateEnum.Option1OriginTerminal,
													   SegmentStateEnum.CarrierInformation,
													   SegmentStateEnum.TwoPartyExchange,
													   SegmentStateEnum.Option2DestinationState,
													   SegmentStateEnum.Option1DestinationTerminal,
					                                   SegmentStateEnum.Finish,
													   SegmentStateEnum.ShippingDocumentNumber,
													   SegmentStateEnum.SequenceErrorIdNumberSdn,
													   SegmentStateEnum.ShippingDocumentDate,
													   SegmentStateEnum.ShippingVesselName,
													   SegmentStateEnum.ShippingDocumentNetGallons,
													   SegmentStateEnum.ShippingDocumentGrossGallons,
													   SegmentStateEnum.Finish
				                                   };
			for (int i = 0; i < stateSequence.Length; i++)
			{
				if (current == stateSequence[i])
				{
					return stateSequence[i + 1];
				}
			}

			throw new ExStarsBusinessException("nextSegmentState() failed for {0} ", current.ToString());
		}
	}
}