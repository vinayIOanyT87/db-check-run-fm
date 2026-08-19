using System;
using System.ServiceModel;
using FMBusinessObjects.DataObjects;


namespace FMBusinessObjects.BusinessInterfaces
{
	[ServiceContract]
	public interface IExStarsBusiness
	{
		[OperationContract]
		string CreateExStarsReportTest1(SecurityClass security, bool isTest, Guid siteGuid, Guid managerGuid, string reportType, string reportMode);

		[OperationContract]
		string CreateExStarsReportTest2(SecurityClass security
			, bool isTest
			, Guid siteGuid
			, Guid managerGuid
			, string reportType
			, string reportMode
			, string endDateyyyymm);

		[OperationContract]
		string CreateExStarsReport(
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
			, out bool reportCreated);

		/// <summary>
		/// For a particular site and managers, get values that are useful in setting the UI paramemters
		/// </summary>
		/// <param name="security"></param>
		/// <param name="siteGuid"></param>
		/// <param name="managerGuid"></param>
		/// <param name="expectedStartDate">What date should next be reported</param>
		/// <param name="awaitingIrsResponse">Has a response been recieved from the IRS?</param>
		/// <param name="lastSubmissionHasErrors">Did the IRS send back errors that must be corrected?</param>
		[OperationContract]
		string GetExpectedParameters(
			SecurityClass security
			, Guid siteGuid
			, Guid managerGuid
			, out DateTime expectedStartDate
			, out bool awaitingIrsResponse
			, out bool lastSubmissionHasErrors
			);

		[OperationContract]
		string DownloadReport(
			SecurityClass security
			, Guid siteGuid
			, Guid managerGuid
			, DateTime transactionMonthAndYear
			, string reportTypeAsStr
			, string exStarsFileFormatAsStr
			, out string defaultFileName
			);


		[OperationContract]
		string DownloadReportByGuid(
			SecurityClass security
			, Guid siteGuid
			, Guid managerGuid
			, string filingGuidAsStr
			, string exStarsFileFormatAsStr
			, out string defaultFileName
			);

		/// <summary>
		/// Upload either a previously sent EDI file (EasyRead or not) or a 151 file from the IRS
		/// </summary>
		/// <param name="security"></param>
		/// <param name="message">A message to display to the caller</param>
		/// <param name="managerGuid"></param>
		/// <param name="filePath">path name of the file</param>
		/// <param name="reportTypeAsStr">type of report</param>
		/// <param name="forceOverwrite">if true this will replace any existing entry</param>
		/// <returns></returns>
		[OperationContract]
		string UploadFile(SecurityClass security
			, out string message
			, Guid managerGuid
			, string filePath
			, string reportTypeAsStr
			, bool forceOverwrite);

		[OperationContract]
		string ViewHistory(SecurityClass security
			, Guid managerGuid
			, DateTime startTransactionDate
			, DateTime endTransactionDateTime
			, out ExStarsReportHistoryList historyList
			);

		[OperationContract]
		int EnableDebugFeatures(SecurityClass security);


	}
}
