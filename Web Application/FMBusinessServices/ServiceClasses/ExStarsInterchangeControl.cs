#pragma warning disable 0414
namespace FMBusinessServices.ServiceClasses
{
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;


	/// <summary>
	/// This is the top-most business-logic class to create ExSTARS reports
	/// </summary>
	public class ExStarsInterchangeControl : ExStarsReportsBase
	{
		// staying with the design used in FMA 7.1, the Functional group will be a subsection of InterchangeControl
		// ref: C_ExSTARS_X12_Interchange_Control::get_csInterchange_Control_Value

		protected ExStarsManagerTotals AllMgrTotals;

		#region Constructors
		/// <summary>
		/// Required by serialization, do not use this
		/// </summary>
		public ExStarsInterchangeControl() : base() { }

		/// <summary>
		/// Standard constructor
		/// </summary>
		/// <param name="config"></param>
		/// <param name="validationErrors"></param>
		public ExStarsInterchangeControl(ExStarsSiteConfigExpanded config, ref string validationErrors)
			: base(config, "INTERCHANGE GROUP SECTION", ref validationErrors)
		{
			//	See ExSTARS Document: IRS Publication 3536 Rev.11-2005 - Page 32 
			//ref C_ExSTARS_X12_Interchange_Control::Generate_Interchange_Control_Header_Segment()

			// Each submission can contain only one interchange; that is, only one ISA/ISE loop; only one
			// functional group within the interchange (GS/GE loop); and only one transaction set (ST/SE loop)
			// within the functional group.
			// ref p 25

			ExStarsFilings filings = new ExStarsFilings(this.Config);

			//
			// this presumes that ExStarsBusiness.ValidateReportModifier() has already validated
			// this modifier
			//
			if (this.Config.ReportModifier == ReportModifiersEnum.Correction)
			{
				// get the error file
				ExStarsFilingClass filingsRow = filings.GetByTransactionSetControlNumber(config.TransSetControlNumber);
				if (filingsRow.ResponseLoaded <= ExStarsConstants.BeginningOfDateTimeOffset)
				{
					throw new ExStarsElementsException("IRS acknowledgement for control number {0} and dates {1}-{2} has not been loaded"
						, filingsRow.TransSetControlNumber
						,filingsRow.FilingStartDate.ToString("d")
						,filingsRow.FilingEndDate.ToString("d"));
				}

				//
				// Throw an exception if Acknowledgement is invalid
				//
				ExStarsProcessUploadedFile.ValidateFileSignature(filingsRow.Acknowledgement);
				Config.LoadPreviousAcknowlegement(filingsRow.Acknowledgement);
			}

			// Create a new InterchangeControlNumber and confirm its uniqueness
			int retry = 0;
			do
			{
				Config.InterchangeControlNumber = ExStarsSegment.UniqueControlNumber();
				if (++retry > 100)
				{
					throw new ExStarsBusinessException("Cannot create unique InterchangeControlNumber");
				}
			}
			while (filings.TransactionSetControlNumberInUse(Config.InterchangeControlNumber));

			this.AllMgrTotals = new ExStarsManagerTotals(config);

			this.SegmentList.Add(this.CreateHeaderSegment());
			string functionGroupValidationErrors = "";
			this.SegmentList.Add(new ExStarsFunctionalGroup(this.Config, ref functionGroupValidationErrors, ref AllMgrTotals));
			//  C_ExSTARS_X12_Document::Generate_Interchange_Control() ~ 225
			this.SegmentList.Add(new ExStarsComment("\nBEGIN INTERCHANGE TRAILER SECTION"));
			this.SegmentList.Add(this.CreateTrailerSegment());
			validationErrors += functionGroupValidationErrors + this.ValidationErrors;
			this.SegmentList.Add(new ExStarsComment(this.AllMgrTotals.ReportTotals()));
		}

		#endregion

		protected ExStarsSegment CreateTrailerSegment()
		{
			ExStarsSegment trailerSegment = new ExStarsSegment("IEA", "Interchange Control Trailer");
			// ref: IRS Publication 3536 Rev.11-2005 - p 35 For ExSTARS, this should always be 1.
			trailerSegment.AddElement(1, "Number Of Transaction Sets Included", "For ExSTARS, this should always be 1.", EnumExStarsElementTypes.N0, 1, 5, "1");
			trailerSegment.AddElement(2, "Interchange Control Number", "must be identical to ISA13", EnumExStarsElementTypes.N0, 9, 9, Config.InterchangeControlNumber);
			return trailerSegment;
		}


		protected ExStarsSegment CreateHeaderSegment()
		{
			// ref: IRS Publication 3536 Rev.11-2005 - p 32 
			string currentDate = this.Config.ReportDateTime.ToString("yyMMdd");
			string currentTime = this.Config.ReportDateTime.ToString("HHmm");

			ExStarsSegment headerSegment = new ExStarsSegment("ISA", "Interchange Control Header");
			headerSegment.AddElement(1, "Authorization Information Qualifer", "Additional Data Identification", EnumExStarsElementTypes.ID, 2, 2, ExStarsConstants.ISA01_AuthorizationInfoQualifier);
			headerSegment.AddElement(2, "Authorization Information", "Ten-Digit Authorization Code issued by the IRS", EnumExStarsElementTypes.AN, 10, 10, this.Config.AuthorizationCode);
			headerSegment.AddElement(3, "Security Information Qualifier", "New field is Password", EnumExStarsElementTypes.ID, 2, 2, ExStarsConstants.ISA03_Password);
			headerSegment.AddElement(4, "Security Information", "Ten-digit Security Code provided by your company in the LOA", EnumExStarsElementTypes.AN, 10, 10, this.Config.SecurityCode);
			// Ref: CExSTARS_ExportApp::GetISA05Qualifier()
			// “ZZ” = ID qualifier published by the sender. Used to designate the sender ID element being qualified.
			headerSegment.AddElement(5, "Interchange ID Qualifier", "ZZ = ID qualifier published by the sender", EnumExStarsElementTypes.ID, 2, 2, Config.ISA05Qualifier);
			headerSegment.AddElement(6, "Interchange Sender ID", "ID published by the sender", EnumExStarsElementTypes.AN, 15, 15, this.Config.InterchangeSenderId.PadRight(15));
			headerSegment.AddElement(7, "Interchange ID Qualifier", "01 = DUNS Number", EnumExStarsElementTypes.ID, 2, 2, ExStarsConstants.ISA07_DunsNumber);
			headerSegment.AddElement(8, "Interchange Receiver ID", "IRS DUNS number padded to right", EnumExStarsElementTypes.AN, 15, 15, this.Config.DunsNumber.PadRight(15));
			headerSegment.AddElement(9, "Interchange Date", "YYMMDD = Date of Interchange", EnumExStarsElementTypes.DT, 6, 6, currentDate);
			headerSegment.AddElement(10, "Interchange Time", "HHMM = Date of Interchange", EnumExStarsElementTypes.TM, 4, 4, currentTime);
			headerSegment.AddElement(11, "Repetition Seperator", "\"|\" = ASCII - 7C HEX", EnumExStarsElementTypes.none, 1, 1, "|");
			headerSegment.AddElement(12, "Interchange Control Version Number", "\"00403\".  This version number covers the interchange control segements", EnumExStarsElementTypes.ID, 5, 5, this.Config.InterchangeControlVersion);
			headerSegment.AddElement(13, "Interchange Control Number", "The Interchange Control Number Uniquely Identifies the Interchange Data to the Information Provider/Transmitter.  The Information Provider/Transmitter Assigns This Number", EnumExStarsElementTypes.N0, 9, 9, Config.InterchangeControlNumber);
			headerSegment.AddElement(14, "Acknowledgment Requested", "0 = No Acknowledgement Required", EnumExStarsElementTypes.ID, 1, 1, ExStarsConstants.ISA14_NoAckRequired);
			headerSegment.AddElement(15, "Usage Indicator", "T = Test Data  P = Production Data", EnumExStarsElementTypes.ID, 1, 1, this.Config.IsTest ? ExStarsConstants.ISA15_TestData : ExStarsConstants.ISA15_ProductionData);
			headerSegment.AddElement(16, "Component Sub-Element Separator", "^(caret) = EBCDIC - 5F HEX or ASCII - 5E HEX", EnumExStarsElementTypes.none, 1, 1, ExStarsConstants.SubElementSeparator);
			return headerSegment;
		}
	}

}