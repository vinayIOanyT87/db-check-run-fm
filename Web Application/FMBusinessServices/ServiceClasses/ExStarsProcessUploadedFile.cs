namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Globalization;
	using System.IO;
	using System.Text;
	using System.Text.RegularExpressions;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	public class ExStarsProcessUploadedFile
	{
		// Motor Fuel Excise Tax EDI Guide  pg 143 -

		#region Properties
		private const string ErrorBadFormat = "This is not a properly formatted 151 Acknowledgment file";
		private const string ErrorInterchangeMissingControlNumber = "151 Acknowledgment file is missing the interchange control number.";
		private const string ErrorMissingControlNumber = "151 Acknowledgment file is missing the transaction set control number.";
		private const string MsgNoErrorsComplete = "The error information from this file is associated with ExSTARS data that has been completely verified by the IRS.  This error information will not be loaded.";
		protected ExStarsIrsErrorCodeClassList IrsErrorCodes;
		public FileCreatingStatus Status { get; protected set; }
		public string StatusMessage { get; protected set; }

		private string ediAcknowledgementReport;
		private readonly ExStarsSiteConfigExpanded Config;
		private string interchangeControlNumber;
		private string transactionSetcontrolNumber;
		private PureSegmentList ediSentToIrs;
		private PureSegmentList ackSegments;
		protected StringBuilder ErrorsReport = new StringBuilder();
		private readonly CultureInfo cultureInfoInvariant = CultureInfo.InvariantCulture;
		private readonly ExStarsFilings filings;
		private ExStarsFilingClass filingsRow = null;
		#endregion

		public ExStarsProcessUploadedFile(ExStarsSiteConfigExpanded config, string pathNameOfEdiSentToIrs = null)
		{
			this.Config = config;
			this.Status = FileCreatingStatus.Unknown;
			filings = new ExStarsFilings(this.Config);
			ExStarsIrsErrorCodeLoader errorCodeLoader = new ExStarsIrsErrorCodeLoader(this.Config.Security);
			this.IrsErrorCodes = errorCodeLoader.GetAll();
			this.ediSentToIrs = null;
		}

		public void LoadReportSentToIrs(string pathNameOfEdiSentToIrs, ReportTypeEnum reportType)
		{
			if (!File.Exists(pathNameOfEdiSentToIrs))
			{
				throw new ExStarsBusinessException("File \"{0}\" containing EDI sent to IRS does not exist", pathNameOfEdiSentToIrs);
			}
			else
			{
				string wholeEdiReport;
				using (StreamReader sr = new StreamReader(pathNameOfEdiSentToIrs))
				{
					wholeEdiReport = sr.ReadToEnd();
				}
				ediSentToIrs = new PureSegmentList(wholeEdiReport, "");
			}

			ExStarsSegment segmentIsa = this.ediSentToIrs.Find(null, 15, "ISA", ExStarsConstants.ISA01_AuthorizationInfoQualifier);
			ExStarsSegment segmentDtmTaxPeriodEnd = segmentIsa.FindNext(2, "DTM", ExStarsConstants.DTM01_TaxPeriodEndDate);
			ExStarsSegment segmentDtmPropertyAquired = segmentIsa.FindNext(0, "DTM", ExStarsConstants.DTM01_DatePropertyAquired);
			ExStarsSegment segmentDtmPropertySold = segmentIsa.FindNext(0, "DTM", ExStarsConstants.DTM01_DatePropertySold);
			ExStarsSegment segmentGs = segmentIsa.FindNext(5, "GS", ExStarsConstants.GS01_TransactionSet813);
			ExStarsSegment segmentSt = segmentIsa.FindNext(3, "ST", ExStarsConstants.ST01_TransactionSetCode);
			ExStarsSegment segmentBti = segmentSt.FindNext(13, "BTI");
			ExStarsSegment segmentRefFj = segmentSt.FindNext(0, "REF", "FJ");

			// It's OK for segmentFgsBi to get a null, Beginning Inventory is rare
			ExStarsSegment segmentFgsBi = segmentIsa.FindNext("FGS", ExStarsConstants.FGS01_BeginningInventory);
			bool includeBeginningInventory = segmentFgsBi != null;
			if (segmentDtmPropertySold != null)
			{
				reportType = ReportTypeEnum.OutgoingManger;
			}
			else if (segmentDtmPropertyAquired != null)
			{
				reportType = ReportTypeEnum.IncomingManager;
			}
			else
			{
				reportType = ReportTypeEnum.StdMonthly;
			}

			ReportModifiersEnum modifier = ReportModifiersEnum.Undefined;
			if (segmentBti.ElementByIndex(13).Value == ExStarsConstants.BTI13_Original)
			{
				modifier = ReportModifiersEnum.Original;
			}
			else 
			{
				switch (segmentBti.ElementByIndex(14).Value)
				{
					case ExStarsConstants.BTI14_Corrected:
						modifier = ReportModifiersEnum.Correction;
						break;
					case ExStarsConstants.BTI14_Replacement:
						modifier = ReportModifiersEnum.Replacement;
						break;
					case ExStarsConstants.BTI14_Supplemental:
						modifier = ReportModifiersEnum.Supplemental;
						break;
				}
			}
			if (modifier == ReportModifiersEnum.Undefined)
			{
				throw new ExStarsBusinessException("Segment BTI does not a have valid transaction type code in elements 13 or 14");
			}


			this.interchangeControlNumber = segmentIsa.ElementByIndex(13).Value;
			this.transactionSetcontrolNumber = segmentSt.ElementByIndex(2).Value;
			if (!filings.TransactionSetControlNumberInUse(this.transactionSetcontrolNumber))
			{
				// do nothing
			} 
			else if (this.Config.ForceOverwrite)
			{
				filings.DeleteEntry();
				ExStarsReportedErrors reportedErrors = new ExStarsReportedErrors(this.Config);
				reportedErrors.DeleteEntry(this.transactionSetcontrolNumber);				
			}
			else
			{
				throw new ExStarsOverwriteException("The report for control number \"{0}\" has already been loaded, do you wish to replace it?", this.transactionSetcontrolNumber);
			}

			// use time 23:59:59.998 instead of 23:59:59.999 because of rounding
			DateTime endDate = DateTime.ParseExact(segmentDtmTaxPeriodEnd.ElementByIndex(2).Value + "235959998", "yyyyMMddHHmmmssfff", this.cultureInfoInvariant);
			DateTime startDate = new DateTime( endDate.Year, endDate.Month, 1);
			DateTimeOffset createdDate = DateTime.ParseExact(
				segmentGs.ElementByIndex(4).Value + segmentGs.ElementByIndex(5).Value
				, "yyyyMMddHHmmss"
				, this.cultureInfoInvariant);		

			DateTimeOffset sentDate = ExStarsConstants.BeginningOfDateTimeOffset;

#if true
			string formatStringForFileName = ExStarsSiteConfigExpanded.BaseFileName(
				Config.Manager.Name
				, endDate
				, reportType
				, this.transactionSetcontrolNumber
				, segmentIsa.ElementByIndex(15).Value == ExStarsConstants.ISA15_TestData
				, ExStarsSiteConfigExpanded.ReportModifierCode(modifier));
#endif

			string originalCtrlNum = segmentRefFj == null
				                         ? ""
				                         : segmentRefFj.ElementByIndex(2).Value;

			ExStarsFilingClass filingRow = new ExStarsFilingClass( startDate
				, endDate
				, this.Config.ManagerCompanyGuid
				, this.Config.SiteGuid
				, reportType
				, includeBeginningInventory
				, ReportModifiersEnum.Original
				, this.interchangeControlNumber
				, this.transactionSetcontrolNumber
				, originalCtrlNum
				, FileCreatingStatus.Submitted
				, ExStarsConstants.PrependDefaultPathToFileName( string.Format(formatStringForFileName, "edi"))
				, ExStarsConstants.PrependDefaultPathToFileName( string.Format(formatStringForFileName, "EasyRead.txt"))
				, createdDate
				, sentDate
				, DateTimeOffset.MinValue
				, this.ediSentToIrs.ToStringEdi(false)
				, this.ediSentToIrs.ToStringEdi(true)
				, this.ediSentToIrs.ToBinary()
				, new Guid());

			filings.InsertFilingRecord(filingRow);

			if (ErrorsReport.Length == 0)
			{
				ErrorsReport.AppendLine("File loaded successfully, no errors found.");
			}

			this.StatusMessage = ErrorsReport.ToString();
		}


		public void ParseAcknowledgementFile(string pathOfAcknowledgementReport)
		{
			if (! File.Exists(pathOfAcknowledgementReport))
			{
				throw new ExStarsBusinessException("File \"{0}\" containing IRS 151 Acknowledgement does not exist", pathOfAcknowledgementReport);
			}
			string acknowledgementReport = null;
			using (StreamReader sr = new StreamReader(pathOfAcknowledgementReport))
			{
				acknowledgementReport = sr.ReadToEnd();
			}
			ParseAcknowledgement(acknowledgementReport);
		}


		public void ParseAcknowledgement(string acknowledgementReport)
		{
			try
			{
				this.ediAcknowledgementReport = acknowledgementReport;
				this.transactionSetcontrolNumber = ValidateFileSignature(this.ediAcknowledgementReport);
				filingsRow = filings.GetByTransactionSetControlNumber(this.transactionSetcontrolNumber);
				if (filingsRow == null)
				{
					// Maybe this has been replaced
					if (filings.TransactionSetControlNumberInUse(this.transactionSetcontrolNumber, useOriginalControlNumber: true))
					{
						// ~ 201
						throw new ExStarsBusinessException("The 151 acknowledgment selected is associated with an ExSTARS file that has been replaced.");
					}
					throw new ExStarsBusinessException("Database does not contain a record for report submitted to IRS with transaction set control number \"{0}\"", this.transactionSetcontrolNumber);
				}

				// ~ 213
				if (filingsRow.FilingStatus == FileCreatingStatus.FinishedNoErrors)
				{
					throw new ExStarsBusinessException("The error information from this file is associated with ExSTARS data that has been completely verified by the IRS.  This error information will not be loaded.");
				}
				// We got this far so it must have come back
				this.Status = FileCreatingStatus.Created;
				// C_ExSTARS_X12_151_Error_Processor::ProcessFile() ~ 289
				if (null == this.ediSentToIrs)
				{
					// this has not been previously loaded from a file, so get it from the database
					this.ediSentToIrs = PureSegmentList.FromBinary(filingsRow.SerializedData);
				}

				this.ackSegments = new PureSegmentList(acknowledgementReport, "From 151");
				InterpretAcknowledgement();
			}
			catch (Exception exception)
			{
				// throw all the exceptions into the returned report
				ErrorsReport.AppendLine(exception.Message);
			}
			this.StatusMessage = ErrorsReport.ToString();
		}

		protected void InterpretAcknowledgement()
		{
			// Process the 151 Header, ref pg 143
			ExStarsSegment segmentGs = this.ackSegments.Find(null, "GS", "TA");
			ExStarsSegment nextSegment = segmentGs.Find("ST", "151");
			ExStarsSegment segmentBta = nextSegment.Find("BTA");
			nextSegment = segmentBta.Find("DTM", ExStarsConstants.DTM01_TaxPeriodEndDate);
			ExStarsSegment segmentRefFj = segmentBta.Find("REF", "FJ");
			// C_ExSTARS_X12_151_Error_Processor::ProcessFile ~ 158
			ExStarsSegment segmentQty = segmentRefFj.Find("QTY", "86");
			ExStarsSegment segmentPbi = segmentQty.Find("PBI");
			this.transactionSetcontrolNumber = filingsRow.TransSetControlNumber.Trim();
			string origControlNumber = filingsRow.OriginalControlNumber.Trim();

			this.CreateResultsHeader( origControlNumber);
			ExStarsReportedErrors reportedErrors = new ExStarsReportedErrors(this.Config);

			// C_ExSTARS_X12_151_Error_Processor::ProcessFile() ~161
			// pg 25: If you receive a TS-151 ACK with an “AT” in the BTA01 segment, your information return has been accepted without errors.
			// Also pg 145, 277
			if (    segmentBta.ElementByIndex(1).Value.Equals(ExStarsConstants.BTA01_Accepted)
				 || segmentQty.ElementByIndex(2).Value.Equals( "0"))
			{
				this.Status = FileCreatingStatus.FinishedNoErrors;
				// update filing
				filingsRow.FilingStatus = FileCreatingStatus.FinishedNoErrors;
				filingsRow.Acknowledgement = this.ediAcknowledgementReport;
				this.StatusMessage = string.Format("{0}\n{1}\n"
					, MsgNoErrorsComplete
					, segmentPbi.ElementByIndex(6).Value);
				ErrorsReport.AppendLine(this.StatusMessage);
				filingsRow.AckEasyRead = this.ErrorsReport.ToString();
				filings.UpdateForIrsAcknowledgement(filingsRow);
				return;
			}

			var next813TransactionSet = this.LogTheStartOfTheErrorReport(segmentPbi);

			// 
			// Log the error details, group by reference
			//
			ExStarsSegment next151TfsSegment = segmentPbi.Find("TFS");
			while(  next151TfsSegment != null)
			{
				next151TfsSegment =  ProblemReportedPerReference(next151TfsSegment, next813TransactionSet);
			}
			filingsRow.FilingStatus = FileCreatingStatus.FinishedWithErrors;
			filingsRow.Acknowledgement = this.ediAcknowledgementReport;
			filingsRow.AckEasyRead = this.ErrorsReport.ToString();
			filings.UpdateForIrsAcknowledgement(filingsRow);
		}

		private void CreateResultsHeader( string origControlNumber)
		{
			// Publish results header
			this.ErrorsReport.AppendFormat(
				"/{9} \n" +
				"+  IRS ExSTARS Acknowledgement for EDI {0} - {1} Report  \n" +
				"+  Submitted by:        {2,-18}      FEIN: {3} \n" +
				"+  Created on:          {4} \n" +
				"+  For reporting dates: {5} - {6} \n" +
				"+  Control Number:      {7}     \n" +
				"{8}  \n" +
				"{9}/ \n"
				, ExStarsConstants.ToString(filingsRow.ReportType)
				, filingsRow.ModifierAsStr
				, this.Config.InfoProviderName
				, this.Config.FeinCode
				, filingsRow.FilingCreated.ToString("g")
				, filingsRow.FilingStartDate.ToString("d")
				, filingsRow.FilingEndDate.ToString("d")
				, this.transactionSetcontrolNumber
				, string.IsNullOrEmpty(origControlNumber)
					  ? ""
					  : string.Format("+  Orig Control Number:  {0}", origControlNumber)
				, "+".Repeat(90)
				);
		}

		// A one-time PBI 
		// ref C_ExSTARS_X12_151_Error_Processor::ProcessFile ~ 158
		private ExStarsSegment LogTheStartOfTheErrorReport(ExStarsSegment segmentPbi)
		{
			this.InsertSeparationLine();

			ExStarsSegment next813TransactionSet = this.ediSentToIrs.Find(null, "ST");
			this.FormatErrorOrWarning(next813TransactionSet, segmentPbi);

			// Append all the segments orginally sent to the IRS
			for (ExStarsSegment next813Segment = this.ediSentToIrs.Next(null);
			     next813Segment != null && !next813Segment.Match("TFS");
			     next813Segment = next813Segment.Next())
			{
				this.ErrorsReport.AppendLine(next813Segment.ToString());
			}

			this.ErrorsReport.AppendLine();
			this.InsertSeparationLine();
			return next813TransactionSet;
		}

		protected ExStarsSegment ProblemReportedPerReference(ExStarsSegment currentAckSegment, ExStarsSegment startTransactionSet)
		{
			// The segment in the 151 ACK file after the "TFS" segment is expected to be REF~55
			// throw an exception if this is not true
			ExStarsSegment segmentAckReferencedSequence = currentAckSegment.Next();
			if (!segmentAckReferencedSequence.Match("REF", ExStarsConstants.REF01_SequenceNumber))
			{
				throw new ExStarsBusinessException("Expected REF~55, got {0}", segmentAckReferencedSequence.ToString());
			}

			// Using the reference marker from the 151 ACK file, find the matching reference in the 813 submittion
			ExStarsSegment segmentSentReferencedSequence = startTransactionSet.Find( "REF", ExStarsConstants.REF01_SequenceNumber, segmentAckReferencedSequence.ElementByIndex(2).Value);
			if (null == segmentSentReferencedSequence)
			{
				throw new ExStarsBusinessException("There is no matching reference for {0}", segmentAckReferencedSequence.ToString());
			}

			// Loop through all the PBI's
			ExStarsSegment segmentAckPbi = currentAckSegment.Find("PBI");
			if (null != segmentAckPbi)
			{
				// 
				// There may be multiple PBI segments per reference
				//
				do
				{
					segmentAckPbi = FormatErrorOrWarning(segmentSentReferencedSequence, segmentAckPbi, segmentSentReferencedSequence);
				}
				while (segmentAckPbi != null && segmentAckPbi.Match("PBI"));

				// 
				// Loop through all the segments originally sent for this reference.
				//
				ExStarsSegment startSentSegment = segmentSentReferencedSequence;
				while (!startSentSegment.Match("TFS") && !startSentSegment.Match("FGS"))
				{
					startSentSegment = startSentSegment.Prev();
				}
				ExStarsSegment segmentSentTfsOrFgs = startSentSegment;

				//
				// There may be some comments prior to TFS/FSG
				//
				startSentSegment = startSentSegment.Prev();
				bool foundComment = false;
				while (startSentSegment.IsComment)
				{
					foundComment = true;
					startSentSegment = startSentSegment.Prev();
				}
				
				//
				// we overran the comments moving previous, go one element next to return to the comments
				//
				if (foundComment)
				{
					startSentSegment = startSentSegment.Next();
				}

				do
				{
					ErrorsReport.AppendLine(startSentSegment.ToString());
					startSentSegment = startSentSegment.Next();
				}
				while (    (startSentSegment != null )
						&& (   ( ReferenceEquals(segmentSentTfsOrFgs, startSentSegment ))
							|| ( !startSentSegment.Match("TFS") && !startSentSegment.Match("FGS")))
						);
				ErrorsReport.AppendLine();
				InsertSeparationLine();
			}
			return currentAckSegment.FindNext("TFS");
		}


		protected ExStarsSegment FormatErrorOrWarning(ExStarsSegment searchStartInSentSegment, ExStarsSegment getErrorFromSegmentPbi, ExStarsSegment matchToSegmentReferencedSequence = null)
		{
			ExStarsSegment nextSegment = searchStartInSentSegment.Next();
			string sequenceNumber = (matchToSegmentReferencedSequence == null) || (matchToSegmentReferencedSequence.Elements.ByIndex(1).Value != ExStarsConstants.REF01_SequenceNumber)
				                        ? "N/A"
				                        : matchToSegmentReferencedSequence.Elements.ByIndex(2).Value;

			ExStarsReportedErrors allReportedErrors = new ExStarsReportedErrors(this.Config);

			ExStarsReportedErrorClass reportedError = new ExStarsReportedErrorClass (
				this.Config.Security
				, this.Config.ManagerCompanyGuid
				, this.Config.SiteGuid
				, this.filingsRow.ExStarsFilingsGuid
				, sequenceNumber
				, getErrorFromSegmentPbi);
			allReportedErrors.InsertErrorRecord( reportedError);
			string errorOrWarningText = reportedError.MustCorrect ? "WARNING" : "ERROR";
			string errorFieldValue = "";
			string errorFieldText = "";
		
			int dashIdx = reportedError.IrsErrorText.IndexOf("-", StringComparison.InvariantCulture);
			if (dashIdx > 0)
			{
				errorFieldValue = reportedError.IrsErrorText.Left(dashIdx);
				errorFieldText = reportedError.IrsErrorText.Substring(dashIdx + 1);
			}
			else
			{
				errorFieldText = reportedError.IrsErrorText;
			}

			ExStarsIrsErrorCodeClass pbi01Primary = this.IrsErrorCodes.LookUp(ExStarsIrsErrorCodeClass.CodeGroupEnum.PBI01_Primary, reportedError.PBI01_Primary);
			ExStarsIrsErrorCodeClass pbi01Secondary = this.IrsErrorCodes.LookUp(ExStarsIrsErrorCodeClass.CodeGroupEnum.PBI01_Secondary, reportedError.PBI01_Secondary);
			ExStarsIrsErrorCodeClass pbi03Primary = this.IrsErrorCodes.LookUp(ExStarsIrsErrorCodeClass.CodeGroupEnum.PBI03_Primary, reportedError.PBI03_Primary);
			ExStarsIrsErrorCodeClass pbi03Secondary = this.IrsErrorCodes.LookUp(ExStarsIrsErrorCodeClass.CodeGroupEnum.PBI03_Secondary, reportedError.PBI03_Secondary);
			string segmentIdWhereErrorOccured = ExStarsSegment.SegmentId(pbi01Primary.ElementId);
			searchStartInSentSegment = this.ediSentToIrs.Find(searchStartInSentSegment, segmentIdWhereErrorOccured);
			ExStarsElement elementWithError = searchStartInSentSegment.ElementByKey(pbi01Primary.ElementId);
			string tip = string.IsNullOrEmpty(elementWithError.Description) 
				?""
				:string.Format("Tip:                {0}", elementWithError.Description);

			//if( searchStartInSentSegment.Description)

			StringBuilder errortext = new StringBuilder();
			errortext.AppendFormat(
				"{0} ELEMENT {1} INFO: {0} \n" +
				"Segment:            {2} \n" +
				"Sequence Number:    {3} \n" +
				"{10} \n" +
				"{4} Description:{5}{6} \n" +
				"{7} \n" +
				"{10} \n" +
				"Severity:           {8} \n" +
				"Field ID:           {9}  \n"
			, "--"
			, errorOrWarningText
			, this.IrsErrorCodes.GetDescription( ExStarsIrsErrorCodeClass.CodeGroupEnum.PBI04, reportedError.PBI04)
			, sequenceNumber
			, errorOrWarningText
			, " ".Repeat(7 - errorOrWarningText.Length)
			, errorFieldText
			, tip
			, pbi03Secondary.Description
			, pbi01Primary.ElementId
			,"-".Repeat( 90));

			if (errorFieldValue.Length > 0)
			{
				errortext.AppendFormat("{0,-20}{1}\n", "Field Value:", errorFieldValue);
			}
			errortext.AppendFormat(
				"Value sent to IRS:  {0} \n" +
				"Explanation:        {1}: {2} \n" +
				"Reason:			     {3} \n"
				, searchStartInSentSegment.ElementValue(pbi01Primary.ElementId)
				, pbi01Primary.Description
				, pbi01Secondary.Description
				, pbi03Primary.Description);

			ErrorsReport.AppendLine(errortext.ToString());

			return nextSegment;
		}

		public static string ValidateFileSignature(string ediAcknowledgementReport)
		{
			Regex hasBtaSegment = new Regex("\\BTA~");
			Regex hasSt151Segment = new Regex("\\ST~151~");
			// 
			// Validate the ISA segement, even though we are not using it
			//
			int startIdx = ediAcknowledgementReport.IndexOf("ISA~", StringComparison.InvariantCulture);
			if (startIdx < 0)
			{
				{
					throw new ExStarsBusinessException(ErrorBadFormat);
				}				
			}
			int endIdx = ediAcknowledgementReport.IndexOf("~^\\", startIdx, StringComparison.InvariantCulture);
			if (   !hasBtaSegment.IsMatch(ediAcknowledgementReport) 
				|| !hasSt151Segment.IsMatch(ediAcknowledgementReport) 
				|| endIdx < 0)
			{
				throw new ExStarsBusinessException(ErrorBadFormat);
			}
			string[] isaParts = ediAcknowledgementReport.Substring(startIdx, endIdx - startIdx).Split('~');
			if (isaParts.Length < 14)
			{
				throw new ExStarsBusinessException(ErrorInterchangeMissingControlNumber);
			}

			//
			// Validate the REF~FJ and get the transaction set control number which must match to the original
			// EDI file  ref pg 95
			//
			startIdx = ediAcknowledgementReport.IndexOf("REF~FJ~", endIdx, StringComparison.InvariantCulture);
			endIdx = ediAcknowledgementReport.IndexOf("\\", startIdx, StringComparison.InvariantCulture);
			if (startIdx < 0 || endIdx < 0)
			{
				throw new ExStarsBusinessException(ErrorBadFormat);
			}
			string[] refFjParts = ediAcknowledgementReport.Substring(startIdx, endIdx - startIdx).Split('~');
			if (refFjParts.Length < 2)
			{
				throw new ExStarsBusinessException(ErrorMissingControlNumber);
			}

			return refFjParts[2];
		}

		protected void InsertSeparationLine()
		{
			ErrorsReport.AppendLine("*".Repeat(90));
		}
	}
}
