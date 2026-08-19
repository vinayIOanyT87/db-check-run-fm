#define use_the_code
#pragma warning disable 0168, 0169,0414,0649

namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Linq;
	using System.Text;

	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.DataObjects;

	using System.IO;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.UtilityObjects;

	using FMBusinessServices.DataAccessLayer;

	/// <summary>
	/// Give the progress after a request to create an EDI file.  While the file is being created
	/// the status is working. When completem but before downloading the status is either
	/// FinishedNoErrors or FinishedWithErrors.  Once the EDI file or the error list has been downloaded
	/// the status is unknown
	/// </summary>

	public class ExStarsBusiness : FMServiceBase, IExStarsBusiness
	{
		#region private and protected attributes and variables

		protected ExStarsInterchangeControl InterchangeControl;

		protected ExStarsSiteConfigExpanded Config;

		protected string EdiRaw;

		protected string EdiReadable;

		protected string EdiFilePath;

		protected string EasyReadFilePath;

		protected StringBuilder RequestValidation;

		
		#endregion


		#region Create Reports

		public string CreateExStarsReportTest1(SecurityClass security
		                                        , bool isTest
		                                        , Guid siteGuid
		                                        , Guid managerGuid
		                                        , string reportType
		                                        , string reportMode)

		{
			return CreateExStarsReportTest2(security
				, isTest
				, siteGuid
				, managerGuid
				, reportType
				, reportMode
				, "20130731");
		}

		/// <summary>
		/// This interface is intended for testing only, it supplies default parameters that should
		/// not be assumed in a real-world situation
		/// </summary>
		/// <param name="security"></param>
		/// /// <param name="siteGuid"></param>
		/// <param name="isTest"></param>
		/// <param name="managerGuid"></param>
		/// <param name="reportType">valid values:"Standard", "Outgoing Manger", "Incoming Manager"</param>
		/// <param name="reportMode">valid values:Original, Replacement, Supplemental, Correction</param>
		/// <param name="endDateyyyymm">year,month </param>
		/// <returns></returns>
		public string CreateExStarsReportTest2(
			SecurityClass security,
			bool isTest,
			Guid siteGuid,
			Guid managerGuid,
			string reportType,
			string reportMode,
			string endDateyyyymm)
		{
			int year = Int32.Parse(endDateyyyymm.Left(4));
			int month = Int32.Parse(endDateyyyymm.Substring(4, 2));
			DateTime startDate = new DateTime(year, month, 1);
			DateTime endDate = startDate.AddMonths(1).AddDays(-1);
			string userErrors;
			bool ableToCreateReport;
			CreateExStarsReport(
				security,
				siteGuid,
				managerGuid,
				isTest,
				startDate,
				endDate,
				false,
				reportType,
				reportMode,
				out userErrors,
				out ableToCreateReport
				);
			return this.EasyReadFilePath;
		}

		/// <summary>
		/// Create the EDI and EDI readable ExSTARS report files
		/// </summary>
		/// <param name="security"></param>
		/// <param name="siteGuid"></param>
		/// <param name="managerGuid"></param>
		/// <param name="isTest"></param>
		/// <param name="startTransactionDateTime">Almost always this is the first day of the month</param>
		/// <param name="endDateTime">Almost always this is the last day of the month</param>
		/// <param name="recreateReport">when true, mark the existing filing for the same site & date as "replaced"</param>
		/// <param name="reportTypeAsStr">accepted values: StdMonthlyReport, OutgoingManger, IncomingManager</param>
		/// <param name="reportModifierAsStr">accepted values: Original, Replacement, Supplemental, Correction</param>
		/// <param name="userErrors">A list of errors caused by what the user selected</param>
		/// <param name="reportCreated">returs true if appl was able to create reports without errors</param>
		/// <returns></returns>
		public string CreateExStarsReport(
			SecurityClass security
			, Guid siteGuid
			, Guid managerGuid
			, bool isTest
			, DateTime startTransactionDateTime
			, DateTime endDateTime
			, bool recreateReport
			, string reportTypeAsStr
			, string reportModifierAsStr
			, out string userErrors
			, out bool reportCreated)
		{
			this.RequestValidation = new StringBuilder();
			ReportTypeEnum reportType = BasicValidation(security, siteGuid, managerGuid, reportTypeAsStr, isTest);
			ReportModifiersEnum reportModifier = this.TranslateReportModifier(reportModifierAsStr);

#if false
			if (isTest)
			{
				// for testing, incoming manager will be assumed to be days 1-15 of the month
				// and outgoing manager day 16 through end of month
				switch (reportType)
				{
					case ReportTypeEnum.IncomingManager:
						startTransactionDateTime = startTransactionDateTime.AddDays(15);
						break;
					case ReportTypeEnum.OutgoingManger:
						endDateTime = startTransactionDateTime.AddDays(16);
						break;
					default:
						break;
				}
			}
#endif

			ExStarsComment.Reset();
			//Tests();

			this.Config = new ExStarsSiteConfigExpanded(
				security
				, ref managerGuid
				, ref siteGuid
				, isTest
				, startTransactionDateTime
				, endDateTime
				, DateTime.Now
				, reportType
				, reportModifier);

			this.EdiRaw = "";
			this.EdiReadable = "";
			this.CreateReport(recreateReport);
			string errorsAndWarnings;
			userErrors = RequestValidation.ToString();
			reportCreated = !this.Config.HasErrors && RequestValidation.Length == 0;
			if (reportCreated)
			{
				errorsAndWarnings = "The report is complete, with no errors or warnings.";
			}
			else
			{
				errorsAndWarnings = string.Format("{0}\n{1}", userErrors, this.Config.ErrorsAndWarningsReport());
			}
			return errorsAndWarnings;  
		}

		#endregion
		#region Private and Protected Members

		private ReportModifiersEnum TranslateReportModifier(string reportModifierAsStr)
		{
			ReportModifiersEnum modifier;
			if (!ReportModifiersEnum.TryParse(reportModifierAsStr, true, out modifier))
			{
				throw new ExStarsBusinessException("{0} is an invalid report modifer, valid values are: Original, Replacement, Supplemental, Correction");
			}

			return modifier;
		}

		protected ReportTypeEnum ParseReportType(string reportType)
		{
			if (reportType.ToUpper().StartsWith("OUTGOING"))
			{
				return ReportTypeEnum.OutgoingManger;
			}
			if (reportType.ToUpper().StartsWith("INCOMING"))
			{
				return ReportTypeEnum.IncomingManager;
			}
			if (reportType.ToUpper().StartsWith("STANDARD"))
			{
				return ReportTypeEnum.StdMonthly;
			}
			if (reportType.ToUpper().StartsWith("ACKNOWLEDGEMENT"))
			{
				return ReportTypeEnum.Acknowledgement;
			}
			ReportTypeEnum reportTypeEnum;
			if (ReportTypeEnum.TryParse(reportType, true, out reportTypeEnum))
			{
				return reportTypeEnum;
			}

			throw new ExStarsBusinessException("Invalid report type: \"{0}\", acceptible values are: \"Standard\", \"Outgoing Manager\", \"Incoming Manager\", \"Acknowledgement\"", reportType);
		}

		/// <summary>
		/// Write file file to the default location, create any necessary folders while doing so
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="outputString"></param>
		/// <returns>Path name of file</returns>
		public  static string WriteToFile(string fileName, string outputString)
		{
			string outputDir = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
				, "Varec"   
				, "ExSTARS");

			if (!Directory.Exists(outputDir))
			{
				Directory.CreateDirectory(outputDir);
			}

			var outputPath = Path.Combine(outputDir, fileName);
			using (StreamWriter sw = new StreamWriter(outputPath, false))
			{
				sw.Write(outputString);
			}
			return outputPath;
		}


		/// <summary>
		/// Recursively go through the ouput and strip it down to segments  only
		/// </summary>
		/// <param name="outputList"></param>
		/// <param name="inputMixedData"></param>
		protected void ToPureSegmentList(PureSegmentList outputList, object inputMixedData)
		{
			if (inputMixedData is ExStarsReportsBase)
			{
				ToPureSegmentList(outputList, (inputMixedData as ExStarsReportsBase).SegmentList);
			}
			else if (inputMixedData is ExStarsSegment)
			{
				outputList.Add(inputMixedData as ExStarsSegment);
			}
			else if (inputMixedData is ExStarsComment)
			{
				ExStarsComment seg = inputMixedData as ExStarsComment;
				if (seg.IsComment)
				{
					outputList.Add(seg);
				}
			}
			else if (inputMixedData is SegmentList)
			{
				foreach (var segment in inputMixedData as SegmentList)
				{
					ToPureSegmentList(outputList, segment);
				}
			}
		}

		public int EnableDebugFeatures(SecurityClass security)
		{
			ExStarsUniversalConfig universalConfig = new ExStarsUniversalConfig(security);
			return universalConfig.EnableDebugFeatures;
		}

		#endregion

		#region Generate Output

		public string GetEdiReadable()
		{
			return this.EdiReadable;
		}

		public string GetEdiRaw()
		{
			return this.EdiReadable;
		}


		public void CreateReport(bool recreateReport)
		{
			this.EasyReadFilePath = "";
			this.EdiFilePath = "";

			ExStarsFilings filings = new ExStarsFilings(this.Config);

			ExStarsFilingStatusClass lastFilingStatus = filings.GetLastStatus();
			if (!recreateReport)
			{
				// do nothing
			}
			else if (lastFilingStatus != null)
			{
				if (lastFilingStatus.IsResponseLoaded)
				{
					const string error = "ExSTARS reports can only be recreated when no 151 file has been loaded.";
					this.RequestValidation.AppendLine(error);
					throw new ExStarsBusinessException(error);
				}
				else
				{
					filings.DeleteFiling(lastFilingStatus.ExStarsFilingsGuid);
					lastFilingStatus = filings.GetLastStatus();
				}
			}

			// query ExStarsFilings for duplicate
			ValidateReportStatus(lastFilingStatus);
			ExStarsFilingStatusClass originalFiling = this.ValidateReportModifier(filings);
			
			// 
			// Having just validiated  the modifier, now use it
			// ref CExSTARS_ExportDlg::RecordFiling() ~1619
			if (   originalFiling != null
				&& (    this.Config.ReportModifier == ReportModifiersEnum.Replacement 
				     || this.Config.ReportModifier == ReportModifiersEnum.Correction))
			{
				Config.OriginalTransSetControlNumber = originalFiling.TransSetControlNumber;
				System.Diagnostics.Debug.Assert(originalFiling != null);
				//  mark the previously sent report as replaced
				filings.UpdateStatus(originalFiling.ExStarsFilingsGuid, FileCreatingStatus.Replaced);
			}

			// Validate Config
			ValidateProduct();
			// Each submission can contain only one interchange; that is, only one ISA/ISE loop; only one
			// functional group within the interchange (GS/GE loop); and only one transaction set (ST/SE loop)
			// within the functional group.
			// ref p 25

			string validationErrors = this.RequestValidation.ToString();

			// Get all the data..
			this.InterchangeControl = new ExStarsInterchangeControl(this.Config, ref validationErrors); 
			string outpath1 = "none";
			string outpath2 = "none";

			if (this.EdiFilePath != null)
			{
				this.EdiRaw = this.InterchangeControl.ToStringEdi();
				outpath1 = WriteToFile(string.Format( Config.BaseFileName(), "edi"), this.EdiRaw);
				this.EdiFilePath = outpath1;
			}

			if (this.EasyReadFilePath != null)
			{
				StringBuilder fullReport = new StringBuilder();
				fullReport.AppendFormat(
					"ExSTARS {0} {1} Report for \"{2}\"\n" +
					"     Period: {3} - {4}\n"+
					"     Transaction Set Control Number: {5}\n" +
					"     Created by Varec FuelsManager® Version: {6}\n"
					, this.Config.ReportModifier
					, ExStarsConstants.ToString(this.Config.ReportType)
					, this.Config.Manager.ID
					, this.Config.StartTransactionDateTime.ToString("MMMM dd, yyyy")
					, this.Config.EndTransactionDateTime.ToString("MMMM dd, yyyy")
					, this.Config.TransSetControlNumber
					, this.Config.Version.ToString(4)
					);
				fullReport.AppendLine("     Complies with IRS document \"Motor Fuel Excise Tax EDI Guide\" Publication 3536 (Rev. 3-2010)");
				fullReport.AppendLine(validationErrors);
				fullReport.AppendLine(this.Config.ErrorsAndWarningsReport());
				fullReport.AppendLine(this.InterchangeControl.ToStringEasyRead());

				this.EdiReadable = fullReport.ToString();
				outpath2 = WriteToFile(string.Format(Config.BaseFileName(), "EasyRead.txt"), this.EdiReadable);
				this.EasyReadFilePath = outpath2;
			}

			if (!this.Config.HasErrors)
			{
				if (this.Config.IsTest)
				{
					filings.DeleteEntry();
				}
				string serializedData = "";
				try
				{
					// by getting only the ExStarsSegment objects, less has to be  serialized
					PureSegmentList pureSegments = new PureSegmentList();
					this.ToPureSegmentList(pureSegments, this.InterchangeControl);
					serializedData = pureSegments.ToBinary();

#if false // Only use for testing serialization
					WriteToFile(string.Format(baseName, "Serialized.BIN"), serializedData);
					PureSegmentList testDeSerialize = PureSegmentList.FromBinary(serializedData);
					WriteToFile(string.Format(baseName, "PURE1.txt"), pureSegments.ToString());
					WriteToFile(string.Format(baseName, "PURE2.txt"), testDeSerialize.ToString());
					WriteToFile(string.Format(baseName, "DeserializedEasyRead.txt"), testDeSerialize.ToStringEdi(true));				
#endif
				}
				catch (Exception ex)
				{
					throw;
				}
				filings.InsertFilingRecord(outpath1, outpath2, this.EdiRaw, this.EdiReadable, serializedData);
			}
		}


		#endregion

		#region Upload, Download and History

		/// <summary>
		/// return values that helps the UI determine what the default parameters should be
		/// </summary>
		/// <param name="security"></param>
		/// <param name="siteGuid"></param>
		/// <param name="managerGuid"></param>
		/// <param name="expectedStartDate"></param>
		/// <param name="awaitingIrsResponse"></param>
		/// <param name="lastSubmissionHasErrors"></param>
		public string GetExpectedParameters(
			SecurityClass security
			, Guid siteGuid
			, Guid managerGuid
			, out DateTime expectedStartDate
			, out bool awaitingIrsResponse
			, out bool lastSubmissionHasErrors)
		{
			BasicValidation(security);
			this.Config = new ExStarsSiteConfigExpanded(
				security
				, ref managerGuid
				, ref siteGuid);
			ExStarsFilings filings = new ExStarsFilings(this.Config);
			ExStarsFilingStatusClass lastStatus = filings.GetLastStatus();
			if (!lastStatus.IsResponseLoaded)
			{
				awaitingIrsResponse = true;
				lastSubmissionHasErrors = false;
				expectedStartDate = lastStatus.FilingStartDate;
			}
			else if (lastStatus.UnresolvedErrors > 0)
			{
				awaitingIrsResponse = false;
				lastSubmissionHasErrors = true;
				expectedStartDate = lastStatus.FilingStartDate;				
			}
			else
			{
				awaitingIrsResponse = false;
				lastSubmissionHasErrors = false;
				expectedStartDate = lastStatus.ExpectedStartingDate;
			}
			return "";
		}

		public string DownloadReport(
			SecurityClass security
			, Guid siteGuid
			, Guid managerGuid
			, DateTime transactionMonthAndYear
			, string reportTypeAsStr			
			, string exStarsFileFormatAsStr
			, out string defaultFileName)
		{
			BasicValidation(security);
			DateTime startTransactionDateTime = new DateTime(transactionMonthAndYear.Year, transactionMonthAndYear.Month, 1);
			DateTime endTransactionDateTime = startTransactionDateTime.AddMonths(1).AddDays(-1);
			ReportTypeEnum reportType;
			if (!ReportTypeEnum.TryParse(reportTypeAsStr, true, out reportType))
			{
				throw new ExStarsBusinessException("Invalid ReportTypeEnum:{0}", reportTypeAsStr);
			}

			this.Config = new ExStarsSiteConfigExpanded(
				security
				, ref managerGuid
				, ref siteGuid
				, false
				, startTransactionDateTime
				, endTransactionDateTime
				, DateTime.Now
				, reportType
				, ReportModifiersEnum.AllTypes);

			ExStarsFilings filings = new ExStarsFilings(this.Config);
			ExStarsFilingClass filingsData = filings.QueryByDate();

			return GetRequestedReport(out defaultFileName, filingsData, exStarsFileFormatAsStr);
		}


		public string DownloadReportByGuid(
			SecurityClass security
			, Guid siteGuid
			, Guid managerGuid
			, string filingGuidAsStr
			, string exStarsFileFormatAsStr
			, out string defaultFileName
			)
		{
			BasicValidation(security);
			this.Config = new ExStarsSiteConfigExpanded(security, ref managerGuid, ref siteGuid);
			ExStarsFilings filings = new ExStarsFilings(this.Config);
			ExStarsFilingClass filingsData = filings.QueryByFlingGuid(filingGuidAsStr);
			return GetRequestedReport(out defaultFileName, filingsData, exStarsFileFormatAsStr);
		}

		private static string GetRequestedReport(out string defaultFileName, ExStarsFilingClass filingsData, string exStarsFileFormatAsStr)
		{
			if (filingsData == null)
			{
				defaultFileName = "";
				return null;
			}

			ExStarsFileFormat fileFormat = ExStarsFileFormat.unknown;
			if (!ExStarsFileFormat.TryParse(exStarsFileFormatAsStr, true, out fileFormat))
			{
				throw new ExStarsBusinessException("Invalid ExStarsFileFormat:{0}", exStarsFileFormatAsStr);
			}

			string reportText = "";

			switch (fileFormat)
			{
				case ExStarsFileFormat.edi:
					reportText = filingsData.EdiReport;
					defaultFileName = filingsData.RawDataFileName;
					break;

				case ExStarsFileFormat.easyread:
					reportText = filingsData.EasyReadReport;
					defaultFileName = filingsData.EasyReadFileName;
					break;

				case ExStarsFileFormat.errorReport:
					reportText = filingsData.AckEasyRead;
					defaultFileName = filingsData.RawDataFileName + ".151.txt";
					break;

				default:
					throw new ExStarsBusinessException("ExStarsBusiness.DownloadReport() switch ({0})", fileFormat);
			}

			return WriteToFile(defaultFileName, reportText);
		}


		public string UploadFile(SecurityClass security
								 , out string message
								 , Guid managerGuid
								 , string filePath
								 , string reportTypeAsStr
								 , bool forceOverwrite)
		{
			ReportTypeEnum reportType = this.BasicValidation(security, reportTypeAsStr);
			Guid siteGuid = security.SiteGuid;

			this.Config = new ExStarsSiteConfigExpanded(
				security
				, ref managerGuid
				, ref siteGuid
				, reportType
				, false
				, forceOverwrite);

			ExStarsProcessUploadedFile edi151Ack = new ExStarsProcessUploadedFile(this.Config);

			if (reportType == ReportTypeEnum.Acknowledgement)
			{
				edi151Ack.ParseAcknowledgementFile(filePath);
			}
			else
			{
				edi151Ack.LoadReportSentToIrs(filePath, reportType);
			}

			message = edi151Ack.StatusMessage;

			return WriteToFile("ErrorLog.txt", edi151Ack.StatusMessage);
		}
		



		public string ViewHistory(SecurityClass security
		                          , Guid managerGuid
		                          , DateTime startTransactionDate
		                          , DateTime endTransactionDateTime
		                          , out ExStarsReportHistoryList sortedHistoryList
			)
		{
			HasViewPermission(security);
			Guid siteGuid = security.SiteGuid;
			this.Config = new ExStarsSiteConfigExpanded(
							security
							, ref managerGuid
							, ref siteGuid
							, startTransactionDate
							, endTransactionDateTime);
			ExStarsFilings filings =  new ExStarsFilings(this.Config);
			ExStarsFilingStatusListClass statusList = filings.GetHistory();
			CompanyClass managerObject = Config.LookUpCompany(managerGuid);
			sortedHistoryList = new ExStarsReportHistoryList(statusList, managerObject);
			sortedHistoryList.Sort();
			return sortedHistoryList.Count.ToString();
		}

		#endregion

		#region Unit Tests

		protected void Tests()
		{
			ExStarsTransportMode.Test();
			ExStarsTransactionSet.Test_SplitInfoProviderName();
			TestUniqueControlNumber();
		}

		private static void TestUniqueControlNumber()
		{
			string s1, s2, s3, s4, s5, s6;
			DateTime endOfTime = new DateTime(2012, 12, 31, 23, 59, 59, 999);
			DateTime beginningOfTime = new DateTime(2010, 1, 1);
			DateTime exactly3YearsLater = new DateTime(2013, 1, 1);
			for (int i = 0; i < 10; i++)
			{
				s1 = ExStarsSegment.UniqueControlNumber(beginningOfTime);
				s2 = ExStarsSegment.UniqueControlNumber(beginningOfTime);
				s3 = ExStarsSegment.UniqueControlNumber(endOfTime);
				s4 = ExStarsSegment.UniqueControlNumber(endOfTime);
				// Test roll-over
				int maxInterations = 100;
				while (maxInterations < 0 && "999999999" != ExStarsSegment.UniqueControlNumber(endOfTime))
				{
					// do nothing except call UniqueControlNumber()
					System.Diagnostics.Debug.Assert(--maxInterations > 0, "Infinite Loop");
				}
				// expect s5 = "000000001"
				s5 = ExStarsSegment.UniqueControlNumber(exactly3YearsLater);
				s6 = ExStarsSegment.UniqueControlNumber(exactly3YearsLater);
				System.Diagnostics.Debug.Assert(s1 == "000000001");
				System.Diagnostics.Debug.Assert(s2 == "000000002");
				System.Diagnostics.Debug.Assert(s3.StartsWith("999999"));
				System.Diagnostics.Debug.Assert(s4.StartsWith("999999"));
				System.Diagnostics.Debug.Assert(s3 != s4);
				System.Diagnostics.Debug.Assert(s5 != s6);
			}
		}


		protected bool ValidateProduct()
		{
			int countOfValid = 0;
			foreach (ProductClass product in this.Config.IrsProductsByProductGuid)
			{
				if (product.ValidateIrsProductCode())
				{
					countOfValid++;
				}
			}
			if (countOfValid == 0)
			{
				this.Config.AppendError(ExStarsErrorSource.Config, "No products are configured with an IRS product code");
				return false;
				//throw new ExStarsBusinessException("No products are configured with an IRS product code");
			}

			return true;
		}

		#endregion


		#region Validation

		private void ValidateReportStatus(ExStarsFilingStatusClass lastFilingStatus)
		{
			if (lastFilingStatus == null)
			{
				// There has never been a report for this site and manager
				if (this.Config.ReportModifier == ReportModifiersEnum.Replacement
					|| this.Config.ReportModifier == ReportModifiersEnum.Supplemental
					|| this.Config.ReportModifier == ReportModifiersEnum.Correction)
				{
					string error = string.Format("{0} has not been created, therefore Replacement, Supplemental and Correction files are not appropriate."
						, this.StdExceptionHeader());
					this.RequestValidation.AppendLine(error);
					throw new ExStarsBusinessException(error);
				}
				if (this.Config.ReportType != ReportTypeEnum.StdMonthly)
				{
					string error = string.Format("{0} has not been previouly created.  The report must be a Standard Monthly Report."
						, this.StdExceptionHeader());
					this.RequestValidation.AppendLine(error);
					throw new ExStarsBusinessException(error);
				}
				return;
			}

			switch (this.Config.ReportType)
			{
				case ReportTypeEnum.StdMonthly:
				case ReportTypeEnum.OutgoingManger:
					{
						this.ValidateExpectedStartingReportDate(lastFilingStatus);
						this.ValidateNotAwaitingAcknowledgement(lastFilingStatus);

						if (this.Config.ManagerCompanyGuid != lastFilingStatus.ManagerCompanyGuid)
						{
							string error = string.Format("{0} must use the same manager as the previous report or create an \"Incoming-Manager Report\"."
								, this.StdExceptionHeader());
							this.RequestValidation.AppendLine(error);
							throw new ExStarsBusinessException(error);
						}
					}
					break;

				case ReportTypeEnum.IncomingManager:
					this.ValidateExpectedStartingReportDate(lastFilingStatus);
					this.ValidateNotAwaitingAcknowledgement(lastFilingStatus);
					break;
			}
		}

		private void ValidateNotAwaitingAcknowledgement(ExStarsFilingStatusClass lastFilingStatus)
		{
			// It's OK to re-create  the report if it has not been sent
			if (lastFilingStatus.FilingStatus == FileCreatingStatus.Submitted)
			{
				string error = string.Format("{0}, has been submitted, but the 151 acknowledgement file has not been loaded ."
					, this.StdExceptionHeader());
				this.RequestValidation.AppendLine(error);
				throw new ExStarsBusinessException(error);
			}
		}

		private void ValidateExpectedStartingReportDate(ExStarsFilingStatusClass lastFilingStatus)
		{
			DateTime expectedStartDate;
			string error;
			switch (this.Config.ReportModifier)
			{
				case ReportModifiersEnum.Original:
					expectedStartDate = lastFilingStatus.ExpectedStartingDate;
					break;
				case ReportModifiersEnum.Replacement:
				case ReportModifiersEnum.Supplemental:
				case ReportModifiersEnum.Correction:
					expectedStartDate = lastFilingStatus.FilingStartDate; 
					break;
				default:
					// this should never happen
					error = "In ValidateExpectedStartingReportDate() Invalid this.Config.ReportModifier";
					this.RequestValidation.AppendLine(error);
					throw new ExStarsBusinessException(error);
			}

			if (expectedStartDate != this.Config.StartTransactionDateTime)
			{
				error = string.Format("{0}, does not match the expected starting date of {1}."
					, this.StdExceptionHeader()
					, expectedStartDate.ToString("d"));
				this.RequestValidation.AppendLine(error);
				throw new ExStarsBusinessException(error);
			}
		}

		/// <summary>
		/// Validate that if this is replacement or supplemental that an original exists, search by control number
		/// supplemental files cannot be created unless there is a valid prior file.
		/// An Exception is thrown if not valid.
		/// </summary>
		/// <param name="filings"></param>
		/// <returns>the guid of the original fling if there is one, else null</returns>
		private ExStarsFilingStatusClass ValidateReportModifier(ExStarsFilings filings)
		{
			ExStarsFilingStatusListClass allFilings = filings.GetStatus(ReportModifiersEnum.AllTypes, skipReplaced: true);
			if (allFilings.Count == 0)
			{
				switch (this.Config.ReportModifier)
				{
					case ReportModifiersEnum.Original:
						// Everything is new, thats good
						return null;
					case ReportModifiersEnum.Replacement:
						ThrowExceptionInvalidReplacement();
						break;
					case ReportModifiersEnum.Correction:
						ThrowExceptionInvalidCorrection();
						return null;
					case ReportModifiersEnum.Supplemental:
						ThrowExceptionInvalidSupplemental();
						return null;
					default:
						throw new ExStarsBusinessException("ValidateReportModifier() 1: switch({0})", this.Config.ReportModifier.ToString());
				}
			}
			// else these dates have already been submitted

			var completedFilings = (from filing in allFilings
									where filing.FilingStatus == FileCreatingStatus.FinishedNoErrors
									select filing).FirstOrDefault();


			if (completedFilings != null)
			{
				switch (this.Config.ReportModifier)
				{
					case ReportModifiersEnum.Original:
						this.ThrowExceptionAllreadySubmitted();
						return null;
					case ReportModifiersEnum.Replacement:
						ThrowExceptionInvalidReplacement();
						return null;
					case ReportModifiersEnum.Correction:
						ThrowExceptionInvalidCorrection();
						return null;
					case ReportModifiersEnum.Supplemental:
						// Supplemental is only valid if there is a completed filing
						this.Config.OriginalTransSetControlNumber = completedFilings.TransSetControlNumber;
						return completedFilings;
					default:
						throw new ExStarsBusinessException("ExStarsBusiness.CreateReport() :2 invalid switch({0}"
							, this.Config.ReportModifier);
				}
			}


			var returnedWithErrors = (from filing in allFilings
									  where filing.FilingStatus == FileCreatingStatus.FinishedWithErrors
									  select filing).FirstOrDefault();

			if (returnedWithErrors != null)
			{
				switch (this.Config.ReportModifier)
				{
					case ReportModifiersEnum.Original:
						this.ThrowExceptionAllreadySubmitted();
						return null;
					case ReportModifiersEnum.Replacement:
					case ReportModifiersEnum.Correction:
						this.Config.OriginalTransSetControlNumber = returnedWithErrors.TransSetControlNumber;
						return returnedWithErrors;
					case ReportModifiersEnum.Supplemental:
						ThrowExceptionInvalidSupplemental();
						return null;
					default:
						throw new ExStarsBusinessException("ExStarsBusiness.CreateReport() :3 invalid switch({0}"
							, this.Config.ReportModifier);
				}
			}

			var submittedReports = (from filing in allFilings
									where filing.FilingStatus == FileCreatingStatus.Submitted || filing.FilingStatus == FileCreatingStatus.Created
									select filing).FirstOrDefault();

			if (submittedReports != null)
			{
				string error = string.Format("{0} has not been acknowledged by the IRS.  " +
					"  You must do one of the following: \n"+
					"(1) Load the 151 acknowledgenent file. OR \n"+
					"(2) Check the \"Re-Create File\" check box.  Use recreate, if the file has NOT been sent to the IRS or "+
					"there are errors that prevent a 151 file from being created."
				, this.StdExceptionHeader());
				this.RequestValidation.AppendLine(error);
				throw new ExStarsBusinessException(error);
			}

			return null;
		}

		private void ThrowExceptionAllreadySubmitted()
		{
			string error = string.Format("{0} has already been created.", StdExceptionHeader());
			this.RequestValidation.AppendLine(error);
			throw new ExStarsBusinessException(error);
		}

		private string StdExceptionHeader()
		{
			return string.Format("An ExSTARS report for manager \"{0}\" covering dates {1}-{2}"
								, this.Config.Manager.ID
								, this.Config.StartTransactionDateTime.ToString("d")
								, this.Config.EndTransactionDateTime.ToString("d"));
		}

		private void ThrowExceptionInvalidSupplemental()
		{
			string error = string.Format(
				"A supplemental report may only be sent when the original submission has been accepted " +
				"and there is a need to transmit new or additional data not included in an initial or " +
				"modified report.");
			this.RequestValidation.AppendLine(error);
			throw new ExStarsBusinessException(error);

		}

		private void ThrowExceptionInvalidCorrection()
		{
			string error = string.Format(
				"A correction report may only be sent when adjusting or correcting original or " +
				"modified filing. This code should be used in response to the receipt of a 151 Data " +
				" Acknowledgement that does not contain BTA01=AT (Accepted code).");
			this.RequestValidation.AppendLine(error);
			throw new ExStarsBusinessException(error);

		}

		private void ThrowExceptionInvalidReplacement()
		{
			string error = string.Format(
				"A replacement report may only be sent when the original submission resulted in \"must fix\" " +
				" errors and a complete replacement of the file to correct errors is submitted. This cannot " +
				" be used when the original file for the filing period resulted in warning messages only. " +
				"This cannot be used when the original file resulted in a \"file rejected\"");
			this.RequestValidation.AppendLine(error);
			throw new ExStarsBusinessException(error);

		}

		private void HasViewPermission(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}
			if (!security.HasRight(RIGHT.VIEW_IRS_EXSTARS_REPORT))
			{
				throw new FMInsufficientRightsException();
			}
		}

		private void BasicValidation(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}
			// If a user has the authority to view ExSTARS reports, it is presumed they have the right to see the
			// company data on that report.
			if (!security.HasRight(RIGHT.CREATE_IRS_EXSTARS_REPORT))
			{
				throw new FMInsufficientRightsException();
			}
		}



		private ReportTypeEnum BasicValidation(SecurityClass security, string reportTypeAsStr)
		{
			BasicValidation(security);
			ReportTypeEnum reportType = this.ParseReportType(reportTypeAsStr);
			return reportType;
		}

		private ReportTypeEnum BasicValidation(SecurityClass security, Guid siteGuid, Guid managerGuid, string reportTypeAsStr, bool isTest)
		{
			if (!isTest)
			{
				if (managerGuid == null)
				{
					throw new ArgumentNullException("managerGuid");
				}

				if (siteGuid == null)
				{
					throw new ArgumentNullException("siteGuid");
				}
			}
			return BasicValidation(security, reportTypeAsStr);


		}

		#endregion


	}
}