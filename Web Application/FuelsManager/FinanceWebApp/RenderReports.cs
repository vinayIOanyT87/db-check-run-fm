// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RenderReports.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the RenderReports type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FinanceWebApp
{
	using System;
	using System.Net;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ReportExecutionSvr2005;

	public class RenderReports
	{
		#region Constants and Fields

		public const string REPORT_FORMAT_EXCEL = "EXCEL";

		public const string REPORT_FORMAT_IMAGE = "IMAGE";

		public const string REPORT_FORMAT_PDF = "PDF";

		private readonly string deviceInfo1;

		private readonly string deviceInfo2;

		private string deviceInfo;

		private string reportFormat;

		private string reportName;

		private ParameterValue[] reportParameters;

		private ReportExecutionService reportingService;

		private string reportingServiceUrl;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		///    This is the default constructor for the Report Service Render class.
		/// </summary>
		public RenderReports()
		{
			this.reportName = null;

			this.deviceInfo1 = "<DeviceInfo><Toolbar>";
			this.deviceInfo2 = "</Toolbar></DeviceInfo>";
			this.reportFormat = REPORT_FORMAT_EXCEL;
		}

		#endregion

		#region Public Properties

		/// <summary>
		///    This property will set and get the device information settings.
		/// </summary>
		public string DeviceInfo
		{
			get
			{
				return this.deviceInfo;
			}
			set
			{
				this.deviceInfo = value;

				if ((this.deviceInfo == null) || (this.deviceInfo.Length <= 0))
				{
					this.deviceInfo = this.deviceInfo1 + "False" + this.deviceInfo2;
				}
			}
		}

		/// <summary>
		///    This property will get the error message attribute.
		/// </summary>
		public string ErrorMessage
		{
			get
			{
				return this.ErrorMessage;
			}
		}

		/// <summary>
		///    This property will set and get the report format.
		/// </summary>
		public string ReportFormat
		{
			get
			{
				return this.reportFormat;
			}
			set
			{
				this.reportFormat = value;
				//Default to Excel report format
				if ((this.reportFormat == null) || (this.reportFormat.Length <= 0)
				    || ((this.reportFormat.Equals(REPORT_FORMAT_EXCEL) == false)
				        && (this.reportFormat.Equals(REPORT_FORMAT_IMAGE) == false)
				        && (this.reportFormat.Equals(REPORT_FORMAT_PDF) == false)))
				{
					this.reportFormat = REPORT_FORMAT_EXCEL;
				}
			}
		}

		/// <summary>
		///    This property will set and get the report name.
		/// </summary>
		public string ReportName
		{
			get
			{
				return this.reportName;
			}
			set
			{
				this.reportName = value;

				if ((this.reportName == null) || (this.reportFormat.Length <= 0))
				{
					this.reportName = "JournalEntryTransactionReport";
				}
			}
		}

		/// <summary>
		///    This property will set the reportParameters attribute.
		/// </summary>
		public ParameterValue[] ReportParameters
		{
			set
			{
				this.reportParameters = value;
			}
		}

		/// <summary>
		///    This property will set and get the reportingservice URL.
		/// </summary>
		public string ReportingServiceUrl
		{
			get
			{
				return this.reportingServiceUrl;
			}
			set
			{
				this.reportingServiceUrl = value;

				if ((this.reportingServiceUrl == null) || (this.reportingServiceUrl.Length <= 0))
				{
					this.reportingServiceUrl = "http://localhost/ReportServer/";
				}
			}
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		///    This method will call for the report to be render and saved to a temporary file.
		/// </summary>
		/// <returns></returns>
		public byte[] RenderReport(SecurityClass security)
		{
			byte[] result = null;
			result = this.Render(security);
			return result;
		}

		#endregion

		#region Methods

		/// <summary>
		///    This method will render the entire report to discover the number of pages the report
		///    contains.  It will also set the printer name, the number of copies to be printed, the
		///    event handler to handle the printing, and request the report to be printed.
		/// </summary>
		private byte[] Render(SecurityClass security)
		{
			try
			{
				SystemSettingClass systemSetting = FMChannelHelper.MakeCall<ISystemSettings, SystemSettingClass>(
																	 x =>
																	 x.Get(security)
																);

				// Create the proxy object and set credentials to Windows Authentication (default).
				this.reportingService = new ReportExecutionService();
				this.reportingService.Url = this.reportingServiceUrl;
				if (!string.IsNullOrEmpty(systemSetting.ReportServerUserName))
				{
					string[] userName = systemSetting.ReportServerUserName.Split('\\');
					if (userName.Length > 1)
					{
						reportingService.Credentials = new NetworkCredential(userName[1], systemSetting.ReportServerPassword, userName[0]);
					}
					else
					{
						reportingService.Credentials = new NetworkCredential(userName[0], systemSetting.ReportServerPassword, ".");
					}
				}
				else
				{
					reportingService.Credentials = CredentialCache.DefaultCredentials;
				}


				byte[] result;
				string[] streamIDs;
				string optionalString = null;
				string extension = null;

				// Create a device info request that will indicate that no toolbar is to be displayed.
				Warning[] warnings = null;

				// Render the entire report to find out how many pages it contains.
				string historyID = null;
				ExecutionInfo executionInfo = this.reportingService.LoadReport(this.ReportName, historyID);
				if (executionInfo != null)
				{
					string parameterLanguage = "en-us";
					this.reportingService.SetExecutionParameters(this.reportParameters, parameterLanguage);

					if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDescKey()))
					{
						var credentialsList = new DataSourceCredentials[1];
						credentialsList[0] = new DataSourceCredentials();
						credentialsList[0].Password = FMChannelHelper.MakeCall<IDBAccess, string>(
																	 x =>
																	 x.GetDBPassword(security.Password)
																);

						credentialsList[0].UserName = security.UserID;
						credentialsList[0].DataSourceName = "ConsolidatedDBDataSource";
						this.reportingService.SetExecutionCredentials(credentialsList);
					}
				}

				result = this.reportingService.Render(
					this.reportFormat,
					this.deviceInfo,
					out extension,
					// Extension
					out optionalString,
					// MimeType
					out optionalString,
					// Encoding
					out warnings,
					// Warning objects
					out streamIDs); // The stream identifiers                                                          

				return result;
			}
			catch (Exception ex)
			{
				throw new Exception("Error rendering report. " + ex.Message);
			}
		}

		#endregion
	}
}