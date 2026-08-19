namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Globalization;
	using System.IO;
	using System.Linq.Expressions;
	using System.Text;
	using System.Text.RegularExpressions;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	using Microsoft.SqlServer.Management.Smo;

	public class ErrorFromAck
	{
		
	}


	public class ExStars151Acknowledgement
	{
		// Motor Fuel Excise Tax EDI Guide  pg 143 -


		private const string ErrorBadFormat = "This is not a properly formatted 151 Acknowledgment file";
		private const string ErrorMissingControlNumber = "151 Acknowledgment file is missing the control number.";
		private const string ErrorStd = "151 Acknowledgment file is missing";
		private const string ErrorMissingSegment = "A required segment is missing";
		private const string MsgNoErrorsComplete = "The error information from this file is associated with ExSTARS data that has been completely verified by the IRS.  This error information will not be loaded.";
		//private const string NoErrorsIrsMsg = "There are no open errors, this period's filing is complete.\\";
//
		protected ExStarsIrsErrorCodeClassList IrsErrorCodes;
		public FileCreatingStatus Status { get; protected set; }
		public string StatusMessage { get; protected set; }

		private string ediAcknowledgementReport;
		private readonly ExStarsSiteConfigExpanded Config;
		//private ExStarsSiteConfigClass SiteIdentity;
		//private readonly SecurityClass Security = null;
		private string controlNumber;
		private PureSegmentList ediSentToIrs;
		private PureSegmentList ackSegments;
		protected StringBuilder ErrorsReport = new StringBuilder();

		//public ExStars151Acknowledgement(SecurityClass security, ExStarsSiteConfigClass siteIdentity, string pathNameOfEdiSentToIrs = null, ExStarsSiteConfigExpanded config)
		public ExStars151Acknowledgement(ExStarsSiteConfigExpanded config, string pathNameOfEdiSentToIrs = null)
		{
			this.Config = config;
			this.Status = FileCreatingStatus.Unknown;
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
				ediSentToIrs = new PureSegmentList(wholeEdiReport);
			}

			ExStarsSegment segmentIsa = this.ediSentToIrs.Find(null, 15, "ISA", ExStarsConstants.ISA01_AuthorizationInfoQualifier);
			ExStarsSegment segmentTia = segmentIsa.FindNext(2, "DTM", ExStarsConstants.DTM01_TaxPeriodEndDate);
			ExStarsSegment segmentGs = segmentIsa.FindNext(2, "GS", ExStarsConstants.GS01_TransactionSet813);

			// It's OK for segmentFgsBi to get a null, Beginning Inventory is rare
			ExStarsSegment segmentFgsBi = segmentIsa.FindNext("FGS", ExStarsConstants.FGS01_BeginningInventory);
			bool includeBeginningInventory = segmentFgsBi != null;

			this.controlNumber = segmentIsa.ElementByIndex(13).Value;
			ExStarsFilings filings = new ExStarsFilings(this.Config);
			if(  filings.ControlNumberInUse(this.controlNumber))
			{
				throw new ExStarsBusinessException("The report for control number \"{0}\" has already been added to tblExStarsFilings", pathNameOfEdiSentToIrs);
			}

			CultureInfo provider = CultureInfo.InvariantCulture;
			// use time 23:59:59.998 instead of 23:59:59.999 because of rounding
			DateTime endDate = DateTime.ParseExact(segmentTia.ElementByIndex(2).Value + "235959998", "yyyyMMddHHmmmssfff", provider);
			DateTime startDate = new DateTime( endDate.Year, endDate.Month, 1);
			DateTimeOffset createdDate = DateTime.ParseExact(
				segmentGs.ElementByIndex(4).Value + segmentGs.ElementByIndex(5).Value
				, "yyyyMMddHHmmss"
				, provider);		

			DateTimeOffset sentDate = ExStarsConstants.BeginningOfDateTimeOffset;

			ExStarsFilingClass filingRow = new ExStarsFilingClass( startDate
				, endDate
				, this.Config.ManagerCompanyGuid
				, this.Config.SiteGuid
				, reportType
				, includeBeginningInventory
				, ReportModifiersEnum.Original
				, this.controlNumber
				, FileCreatingStatus.Submitted
				, createdDate
				, sentDate
				, ediSentToIrs.ToStringEdi(false)
				, ediSentToIrs.ToStringEdi(true)
				, ediSentToIrs.ToBinary());

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
				this.controlNumber = this.ValidateFileSignature();
				ExStarsFilings filings = new ExStarsFilings(this.Config);
				ExStarsFilingClass filingsRow = filings.GetByControlNumber(this.controlNumber);
				if (null == filingsRow)
				{
					// Maybe this has been replaced
					if (filings.ControlNumberInUse(this.controlNumber, useOriginalControlNumber: true))
					{
						// ~ 201
						throw new ExStarsBusinessException("The 151 acknowledgment selected is associated with an ExSTARS file that has been replaced.");
					}
					throw new ExStarsBusinessException("Database does not contain a record for report submitted to IRS with control number \"{0}\"", this.controlNumber);
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
				this.ackSegments = new PureSegmentList(acknowledgementReport);
				InterpretAcknowledgement(filings, filingsRow);
			}
			catch (Exception exception)
			{
				// throw all the exceptions into the returned report
				ErrorsReport.AppendLine(exception.Message);
			}
			this.StatusMessage = ErrorsReport.ToString();
		}

		protected void InterpretAcknowledgement(ExStarsFilings filings, ExStarsFilingClass filingsRow)
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


			// Publish results header
			ErrorsReport.AppendFormat(
@"/{9}
+  IRS ExSTARS Acknowledgement for EDI {0} - {1} Report 
+  Submitted by        {2,-18}      FEIN: {3}
+  Created on          {4}
+  For reporting dates {5} - {6}
+  Control Number:     {7}    
+  {8} 
{9}/
"
				, ExStarsConstants.ToString(filingsRow.ReportType)
				, filingsRow.ModifierAsStr
				, Config.InfoProviderName
				, Config.FeinCode
				, filingsRow.FilingCreated.ToString("g")
				, filingsRow.FilingStartDate.ToString("d")
				, filingsRow.FilingEndDate.ToString("d")
				, filingsRow.ControlNumber
				, string.IsNullOrEmpty(filingsRow.OriginalControlNumber)
					  ? ""
					  : string.Format("Original Control Number:{0}", filingsRow.OriginalControlNumber)
				, "+".Repeat(90)
				);


			// C_ExSTARS_X12_151_Error_Processor::ProcessFile() ~161
			// pg 25: If you receive a TS-151 ACK with an “AT” in the BTA01 segment, your information return has been accepted without errors.
			// Also pg 145, 277
			if (    segmentBta.ElementByIndex(1).Value.Equals(ExStarsConstants.BTA01_Accepted)
				 || segmentQty.ElementByIndex(2).Value.Equals( "0"))
			{
				this.Status = FileCreatingStatus.FinishedNoErrors;
				// update filing
				filingsRow.FilingStatus = FileCreatingStatus.FinishedNoErrors;
				filingsRow.Acknowledgement = segmentRefFj.ElementByIndex(2).Value;
				filings.Update(filingsRow);
				this.StatusMessage = string.Format("{0}\n{1}\n"
					, MsgNoErrorsComplete
					, segmentPbi.ElementByIndex(6).Value);
				ErrorsReport.AppendLine(this.StatusMessage);
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
					segmentAckPbi = FormatErrorOrWarning(segmentSentReferencedSequence, segmentAckPbi);
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
				while (startSentSegment.IsComment)
				{
					startSentSegment = startSentSegment.Prev();
				}
				
				//
				// we overran the comments moving previous, go one element next to return to the comments
				//
				startSentSegment = startSentSegment.Next();

				do
				{
					ErrorsReport.AppendLine(startSentSegment.ToString());
					startSentSegment = startSentSegment.Next();
				}
				while (startSentSegment != null 
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


			ExStarsReportedErrorClass reportedError = ExStarsReportedErrorClass.ParseBbi(this.controlNumber, sequenceNumber, getErrorFromSegmentPbi);
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

			StringBuilder errortext = new StringBuilder();
			errortext.AppendFormat(
@" 
{0} ELEMENT {1} INFO: {0}
Segment:            {2}
Sequence Number:    {3}
{9}
{4} Description:{5}{6}
{9}
Severity:           {7}
Field ID:           {8} 
"
//--+----1----+----2----+----3----+----4
			, "--"
			, errorOrWarningText
			, this.IrsErrorCodes.GetDescription( ExStarsIrsErrorCodeClass.CodeGroupEnum.PBI04, reportedError.PBI04)
			, sequenceNumber
			, errorOrWarningText
			, " ".Repeat(7 - errorOrWarningText.Length)
			, errorFieldText
			, pbi03Secondary.Description
			, pbi01Primary.ElementId
			,"-".Repeat( 90));

			if (errorFieldValue.Length > 0)
			{
				errortext.AppendFormat("{0,-20}{1}\n", "Field Value:", errorFieldValue);
			}
			errortext.AppendFormat(
//--+----1----+----2----+----3----+----4
				@" 
Value sent to IRS:  {0}
Explanation:        {1}: {2}
Reason:			     {3}
"
				, searchStartInSentSegment.ElementValue(pbi01Primary.ElementId)
				, pbi01Primary.Description
				, pbi01Secondary.Description
				, pbi03Primary.Description);

			ErrorsReport.AppendLine(errortext.ToString());

			return nextSegment;
		}

		protected string ValidateFileSignature()
		{
			Regex hasBtaSegment = new Regex("\\BTA~");
			int startIdx = this.ediAcknowledgementReport.IndexOf("ISA~", StringComparison.InvariantCulture);
			int endIdx = this.ediAcknowledgementReport.IndexOf("~^\\", StringComparison.InvariantCulture);
			if ( !hasBtaSegment.IsMatch(this.ediAcknowledgementReport) || startIdx < 0 || endIdx < 0)
			{
				throw new ExStarsBusinessException(ErrorBadFormat);
			}
			string[] parts = this.ediAcknowledgementReport.Substring(startIdx, endIdx - startIdx).Split('~');
			if (parts.Length < 14)
			{
				throw new ExStarsBusinessException(ErrorMissingControlNumber);
			}
			return parts[13];
		}

		protected void InsertSeparationLine()
		{
			ErrorsReport.AppendLine("*".Repeat(90));
		}
	}
}
