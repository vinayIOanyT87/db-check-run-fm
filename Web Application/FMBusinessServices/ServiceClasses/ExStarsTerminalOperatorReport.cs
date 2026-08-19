namespace FMBusinessServices.ServiceClasses
{
	using System;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	/// <summary>
	/// See ExSTARS Document: IRS Publication 3536 Rev.11-2005 - Page 61-62, 70
	/// User story 46782 task 46783
	/// </summary>
	[Serializable]
	public class ExStarsTerminalOperatorReport : ExStarsTransactionReportLoopBase
	{

		protected double TotalNetReportedTiaItems;
		protected ExStarsTerminalOperatorReportConfigClass ExStarsTerminalOperatorReportConfig;
		/// <summary>
		/// Required for serialization, do not use
		/// </summary>
		public ExStarsTerminalOperatorReport() : base() { }

		/// <summary>
		/// Standard constructor
		/// </summary>
		/// <param name="config"></param>
		/// <param name="totalSegmentCount"></param>
		/// <param name="totalNetReportedTiaItems">total of all segments reported as TIA~5002 
		/// or TIA~5005 items.  BI, EI, PL, Net Issue - it's added add to this </param>
		/// <param name="managerTotals"></param>
		/// <param name="validationErrors"></param>
		public ExStarsTerminalOperatorReport(ExStarsSiteConfigExpanded config
			, ref int totalSegmentCount
			, ref double totalNetReportedTiaItems
			,  ExStarsManagerTotals managerTotals
			, ref string validationErrors)
			: base(config, "TERMINAL OPERATOR REPORT", managerTotals, ref validationErrors)
		{
			this.ExStarsTerminalOperatorReportConfig = null;
			this.TotalNetReportedTiaItems = totalNetReportedTiaItems;
			GenerateTerminalOperatorReports();
			totalSegmentCount += SegmentList.CountInUse();
			validationErrors += this.ValidationErrors;
			// return back the updated value
			totalNetReportedTiaItems = this.TotalNetReportedTiaItems;
		}

		private SegmentStateEnum NextSegmentState(SegmentStateEnum current)
		{
			bool isTransferOfOwnership = Config.ReportType == ReportTypeEnum.IncomingManager
			                             || Config.ReportType == ReportTypeEnum.OutgoingManger;

			if (current == SegmentStateEnum.NoBusinessActivity)
			{
				return isTransferOfOwnership
					       ? SegmentStateEnum.DateOfTransfer
					       : SegmentStateEnum.EndingInventoryDate;
			}
			// For any match to current, use the next value in the list, its OK to have the same value multiple times because:
			// Abort -> Finish
			// Finish -> Finish
			// EndingInventoryLoopByProductCode -> Finish
			SegmentStateEnum[] stateSequence = new SegmentStateEnum[]
				                                   {
					                                   SegmentStateEnum.Abort,
					                                   SegmentStateEnum.Finish,
					                                   SegmentStateEnum.Finish,
					                                   SegmentStateEnum.Begin,
					                                   SegmentStateEnum.RelationshipToTheInformation,
					                                   SegmentStateEnum.SequenceErrorToIdNumberRtti,
													   SegmentStateEnum.NoBusinessActivity,
					                                   SegmentStateEnum.DateOfTransfer,
					                                   SegmentStateEnum.EndingInventoryDate,
					                                   SegmentStateEnum.EndingInventoryLoopByProductCode,
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

		/// <summary>
		/// 
		/// </summary>
		/// <returns>true if no error</returns>
		public bool GenerateTerminalOperatorReports()
		{
			// Reg p36 "813 - Table 2 - Body" 
			this.ExStarsTerminalOperatorReportConfig = new ExStarsTerminalOperatorReportConfigClass(
				Config.EndTransactionDateTime);
			// if validation fails it throws an exception
			this.ValidateExistanceOfPhysicalInventory();
			// in c++ calling C_ExSTARS_X12_Terminal_Operator_Report() does the same as initializing 
			// exStarsTerminalOperatorReportConfig

			// C_ExSTARS_X12_Transaction_Set::Generate_Terminal_Operator_Report() ~ 457
			for (SegmentStateEnum segmentStateEnum = SegmentStateEnum.Begin;
			     segmentStateEnum != SegmentStateEnum.Finish;
			     segmentStateEnum = this.NextSegmentState(segmentStateEnum))
			{
				// C_ExSTARS_X12_Terminal_Operator_Report::Generate_Segment() ~ 128
				switch (segmentStateEnum)
				{
					case SegmentStateEnum.Begin:
						SegmentList.Add( this.CreateTaxFormSegment());
						break;

					case SegmentStateEnum.RelationshipToTheInformation:
						SegmentList.Add( this.CreateRelationshipToTheInformation());
						break;

					case SegmentStateEnum.SequenceErrorToIdNumberRtti:
						SegmentList.Add( this.CreateSequenceErrorToIdNumberRtti());
						break;

					case SegmentStateEnum.NoBusinessActivity:
						// proceed to the next state
						break;

					case SegmentStateEnum.DateOfTransfer:
						SegmentList.Add( this.CreateDateOfTransfer());
						break;

					case SegmentStateEnum.EndingInventoryDate:
						if (this.Config.IsNotCorrectionOrHasReferencedError())
						{
							SegmentList.Add(new ExStarsDateTimeSegment(ExStarsConstants.DTM01_InventoryDate, this.Config.EndTransactionDateTime));
						}
						break;

					case SegmentStateEnum.EndingInventoryLoopByProductCode:
						CreateEndingInventoryLoopByProductCode();
						break;
					default:
						throw new NotImplementedException("GenerateTerminalOperatorReports() SegmentState");
				}
			}

			return true;
		}


		protected void CreateEndingInventoryLoopByProductCode()
		{
			// ref C_ExSTARS_X12_Terminal_Operator_Report::Generate_Product_Code_Loop_For_Ending_Inventory() ~ 298
			// FGS pg 73
			ExStarsProductInventory beginningInventory = new ExStarsProductInventory(Config,  Config.StartTransactionDateTime.AddDays(-1));
			ExStarsProductInventory endingInventory = new ExStarsProductInventory(Config, Config.EndTransactionDateTime);
			foreach (ExStarsProductInventoryClass productInventory in endingInventory.ProductInventoryList.Values)
			{
				string taxCode = productInventory.TaxCode;
				if ( taxCode == "")
				{
					// The IRS is not interested in this
					continue;
				}
				double netBeginningInventory = beginningInventory.ProductInventoryList[taxCode].NetVolume;
				double grossBeginningInventory = beginningInventory.ProductInventoryList[taxCode].GrossVolume;

				double netEndingInventory = endingInventory.ProductInventoryList[taxCode].NetVolume;
				double grossEndingInventory = endingInventory.ProductInventoryList[taxCode].GrossVolume;
				AllMgrTotals.AddtoSum(EnumExStarsTrxType.BeginningInventory, productInventory.ManagerCompanyGuid, productInventory.ProductGuid, grossBeginningInventory, netBeginningInventory, beginningInventory.ProductInventoryList[taxCode].Count);
				AllMgrTotals.AddtoSum(EnumExStarsTrxType.EndingInventory, productInventory.ManagerCompanyGuid, productInventory.ProductGuid, grossEndingInventory, netEndingInventory, endingInventory.ProductInventoryList[taxCode].Count);

				// ref CExSTARSTabControl::ActivateTabDialogs() ~ 119
				bool isBuyingParty = Config.ReportType == ReportTypeEnum.IncomingManager;
				// ref: C_ExSTARS_X12_Terminal_Operator_Report::Generate_Product_Code_Loop_For_Ending_Inventory() ~ 355
				if (isBuyingParty || !productInventory.PriorInventoryExists)
				{
					// set PriorInventoryExists
					endingInventory.SetBeginningInventoryRecorded(productInventory);
					SegmentList.Add(CreateFormGroupSegment(ExStarsConstants.FGS01_BeginningInventory, productInventory));
					SegmentList.Add(CreateSequenceErrorToIdNumberRtti());
					SegmentList.Add(CreateAmountSegment(MeasurementBeingTaxed.NetPhysicalInventory, netBeginningInventory, ref this.TotalNetReportedTiaItems));
					// C_ExSTARS_X12_Terminal_Operator_Report::Generate_Product_Code_Loop_For_Ending_Inventory() ~ 343
				}
				try
				{ 
					SegmentList.Add(CreateFormGroupSegment(ExStarsConstants.FGS01_EndingInventory, productInventory));
					SegmentList.Add(CreateSequenceErrorToIdNumberRtti());
					double totalNetEndingInventory = productInventory.NetVolume;
					SegmentList.Add(CreateAmountSegment(MeasurementBeingTaxed.NetPhysicalInventory, netEndingInventory, ref this.TotalNetReportedTiaItems));
					// ref ~ 408
					SegmentList.Add(CreateFormGroupSegment(ExStarsConstants.FGS01_GainsLosses, productInventory));
					SegmentList.Add(CreateSequenceErrorToIdNumberRtti());
					// ref ~ 423, in the old code alway zero? Seems odd
					double netGainLoss = totalNetEndingInventory - netBeginningInventory;
					SegmentList.Add(CreateAmountSegment(MeasurementBeingTaxed.NetPhysicalInventory, netGainLoss, ref this.TotalNetReportedTiaItems ));
					// for readaility purposes, insert a blank segment, later do not count this
					SegmentList.Add(new ExStarsSegment());
				}
				catch (Exception e)
				{
					Config.AppendError(ExStarsErrorSource.Transaction, "productInventory " + e.Message);
				}
			}
		}


		protected ExStarsSegment CreateFormGroupSegment(
			string assignedIdentification,
			ExStarsProductInventoryClass productInventory)
		{
			// ref C_ExSTARS_X12_Base_Segments::Generate_Form_Group_Segment() ~ 509
			if (this.Config.IsNotCorrectionOrHasReferencedError())
			{
				// ref page 73
				ExStarsSegment segment = new ExStarsSegment("FGS", "Form Group");
				string taxCode = productInventory.TaxCode;
				try
				{
					string description;
					switch (assignedIdentification)
					{
						case ExStarsConstants.FGS01_BeginningInventory:
							description = "BI = Beginning Inventory";
							break;
						case ExStarsConstants.FGS01_EndingInventory:
							description = "BE = Ending Inventory";
							break;
						case ExStarsConstants.FGS01_GainsLosses:
							description = "GL = Gains and Losses";
							break;
						default:
							throw new ExStarsBusinessException("CreateFormGroupSegment(assignedIdentification)");
					}
					segment.AddElement(1, "Assigned Identification", description, EnumExStarsElementTypes.AN, 1, 2, assignedIdentification);
					segment.AddElement(2, "Reference Identification Qualifier", "PG = Product Group", EnumExStarsRequired.X, EnumExStarsElementTypes.ID, 2, 2, ExStarsConstants.FGS02_ProductGroup);
					segment.AddElement(3, "Reference Identification", "IRS Product Group", EnumExStarsRequired.X, EnumExStarsElementTypes.AN, 3, 3, taxCode);
				}
				catch (Exception e)
				{
					Config.AppendError(ExStarsErrorSource.Transaction, "productInventory " + assignedIdentification + " " + e.Message);
				}

				return segment;
			}
			return null;
		}

		protected ExStarsSegment CreateDateOfTransfer()
		{
			ExStarsSegment segment = null;
			try
			{ 
				// ref C_ExSTARS_X12_Base_Segments::Generate_Date_Time_Segment() ~ 951; pg 73
				if (!this.Config.IsNotCorrectionOrHasReferencedError())
				{
					return null;
				}
				
				if (Config.ReportType == ReportTypeEnum.IncomingManager)
				{
					segment = new ExStarsDateTimeSegment(ExStarsConstants.DTM01_DatePropertyAquired, this.Config.StartTransactionDateTime);
				}
				else if (Config.ReportType == ReportTypeEnum.OutgoingManger)
				{
					segment = new ExStarsDateTimeSegment(ExStarsConstants.DTM01_DatePropertySold, this.Config.EndTransactionDateTime);
				}
				else
				{
					segment = new ExStarsDateTimeSegment(ExStarsConstants.DTM01_InventoryDate, this.Config.EndTransactionDateTime);
				}
			}
			catch (Exception e)
			{
				Config.AppendError(ExStarsErrorSource.Transaction, e.Message);
			}

			return segment;
		}



		protected ExStarsSegment CreateRelationshipToTheInformation()
		{
			// ref C_ExSTARS_X12_Base_Segments::Generate_Relationship_to_the_Information_Segment() ~ 222
			if (!this.Config.IsNotCorrectionOrHasReferencedError())
			{
				return null;
			}

			// ref page 70
			ExStarsSegment segment = new ExStarsSegment("REF", "Reference Identification");
			try
			{ 
				segment.AddElement(1,"Reference Identification Qualifier","SU = Specific Processing",EnumExStarsElementTypes.ID,2,2,ExStarsConstants.REF01_SpecialProcessing);
				segment.AddElement(2,"Reference Identification","IRS = IRS Data or N/A = if not applicable to IRS",EnumExStarsElementTypes.AN,3,3,"IRS");
				// not used: #3
				segment.AddElement(4, "Reference Identification", "", EnumExStarsRequired.O, EnumExStarsElementTypes.MultiPart);
				segment.AppendSubElement(4,1,"Reference Identification Qualifier","S0 = Special Approval",EnumExStarsRequired.M,EnumExStarsElementTypes.ID,2,2,"S0", null);
				segment.AppendSubElement(4, 2, "Reference Identification", "State Abbreviation for state which data belongs to.", EnumExStarsRequired.M, EnumExStarsElementTypes.AN, 2, 2, Config.Manager.State, Config.Manager);
				// for bulk issues subelemets 3,4 will be set, C_ExSTARS_X12_Schedule_Detail::Generate_Bulk_Issues() ~ 3163
			}
			catch (Exception e)
			{
				Config.AppendError(ExStarsErrorSource.Transaction, e.Message);
			}
			return segment;
		}


		protected ExStarsSegment CreateTaxFormSegment()
		{
			if (!this.Config.IsNotCorrectionOrHasReferencedError())
			{
				return null;
			}

			// pg 70
			ExStarsSegment segment = new ExStarsSegment("TFS", "Terminal Operator Report - TOR");
			try
			{ 
				segment.AddElement(1, "Reference Identification Qualifier", "", EnumExStarsElementTypes.ID, 2, 2, "T2");
				segment.AddElement(2, "Reference Identification", "", EnumExStarsElementTypes.AN, 3, 3, "TOR");
				// no #3, #4
				segment.AddElement(5, "ID Code Qualifier", "", EnumExStarsRequired.X, EnumExStarsElementTypes.ID, 2, 2, "TC");
				segment.AddElement(6,"ID Code","",EnumExStarsRequired.X,EnumExStarsElementTypes.AN,9,9,this.Config.TerminalControlNumber);
			}
			catch (Exception e)
			{
				Config.AppendError(ExStarsErrorSource.Transaction, e.Message);
			}

			return segment;
		}


		/// <summary>
		/// Validate the configuration
		/// </summary>
		/// <returns>null is there are no errors, returns a description of the error if there is</returns>
		private string ValidateExistanceOfPhysicalInventory()
		{
			ExStarsProductInventory endingInventory = new ExStarsProductInventory(Config, Config.EndTransactionDateTime);

			if (endingInventory.HasInventory == ExStarsInventoryStatus.noActivity)
			{
				this.ExStarsTerminalOperatorReportConfig.NoBusinessActivity = true;
				string msg = string.Format(
					"No ending physical inventory transaction exists for site \"{0}\", managing company \"{1}\" on date={2}\n" +
					"The IRS requires an ending physical inventory in order to report fueling transactions.\n"
					,this.Config.Site.ID
					,this.Config.Manager.ID
					,Config.EndTransactionDateTime.ToString("MMMM d, yyyy"));
				Config.AppendError(ExStarsErrorSource.Transaction,msg);
				throw new ExStarsDataValidationException(msg);
			}
			ExStarsProductInventory beginningInventory = new ExStarsProductInventory(Config, Config.StartTransactionDateTime.AddDays(-1));
			if (beginningInventory.HasInventory == ExStarsInventoryStatus.noActivity)
			{
				this.ExStarsTerminalOperatorReportConfig.NoBusinessActivity = true;
				string msg = string.Format(
					"No beginning physical inventory transaction exists for site \"{0}\", managing company \"{1}\" on date={2}\n" +
					"The IRS requires a beginning physical inventory in order to report fueling transactions for the first time.\n"
					,this.Config.Site.ID
					,this.Config.Manager.ID
					,Config.StartTransactionDateTime.ToString("MMMM d, yyyy"));
				Config.AppendError(ExStarsErrorSource.Transaction, msg);
				throw new ExStarsDataValidationException(msg);
			}
			this.ExStarsTerminalOperatorReportConfig.NoBusinessActivity = false;
			return null;
		}
	}

}