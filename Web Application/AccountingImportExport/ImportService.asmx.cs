// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportService.asmx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Accounting web service for import
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace AccountingImportExport
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.IO.Compression;
    using System.ServiceModel.Channels;
    using System.Web.Services;
    using System.Xml.Serialization;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.ServiceRequests;

    using Microsoft.Web.Services2;
    using Microsoft.Web.Services2.Attachments;
    using Microsoft.Web.Services2.Security.Tokens;

    using Unity;
    using XMLImport;
    using FMDepedencyManager;

    using EventLog = Microsoft.Web.Services2.Diagnostics.EventLog;
    using FMWebAPIBusinessLogic.Interfaces.FMProxy;
    using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic;

    /// <summary>
    /// Accounting web service for import
    /// </summary>
    public class ImportService : WebService
	{
		/// <summary>
		/// The event log source for logging.
		/// </summary>
		public const string EventLogSource = "FuelsManager";
		
		#region Fields

		/// <summary>
		/// Required by the Web Services Designer 
		/// </summary>
		private readonly IContainer components = null;

        #endregion

        private readonly XMLImportProcessor _xmlImportProcessor;
        private readonly ICurrentRequestContext _currentUserSecurity;
        private readonly ISiteProxy _siteProxy;

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportService"/> class.
        /// </summary>
        public ImportService()
		{
            //Using Service Locator like this is an antipattern, but ASMX is not designed for DI.  It must have a parameterless constructor.
            this._xmlImportProcessor = FMServiceLocator.Container.Resolve<XMLImportProcessor>();
            this._currentUserSecurity = FMServiceLocator.Container.Resolve<ICurrentRequestContext>();
            this._siteProxy = FMServiceLocator.Container.Resolve<ISiteProxy>();
			// CODEGEN: This call is required by the ASP.NET Web Services Designer
			this.InitializeComponent();
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Exports this instance.
		/// </summary>
		/// <returns>The standard "export" name string.</returns>
		[WebMethod]
		public string Export()
		{
			return "Export";
		}

		/// <summary>
		/// Imports this instance.
		/// </summary>
		/// <returns>A array of import results.</returns>
		/// <exception cref="System.Exception">Missing attachment in ImportService.Import() SOAP message.</exception>
		[WebMethod]
		[XmlInclude(typeof(TransactionValidationResult))]
		public ArrayList Import()
		{
			SecurityClass security = null;

			try
			{
				SoapContext requestContext = RequestSoapContext.Current;

				string siteID;
				security = this.Login(out siteID);

				if (requestContext.Attachments.Count != 1)
				{
					throw new Exception("Missing attachment in ImportService.Import() SOAP message.");
				}

				Attachment attachment = requestContext.Attachments[0];

                // Decompress the stream provided on the attachment
			    using (MemoryStream decompressedStream = new MemoryStream())
			    {
			        using (GZipStream compressedStream = new GZipStream(attachment.Stream, CompressionMode.Decompress))
			        {
			            compressedStream.CopyTo(decompressedStream);
			        }

                    // We have to reset the position before the stream's xml is read
			        decompressedStream.Position = 0;

			        //var importer = new XMLImportProcessor();
			        List<TransactionValidationResult> result = _xmlImportProcessor.Import(
			            security,
			            siteID,
			            decompressedStream,
			            filter: null);

			        // We must convert to an ArrayList to stay compatible with the legacy application,
			        // otherwise the legacy application will report no errors when there are errors
			        var resultAsArrayList = new ArrayList(result);
			        return resultAsArrayList;
			    }
			}
			catch (Exception e)
			{
				var result = new TransactionValidationResult();
				var resultList = new List<TransactionValidationResult>();

				XMLImportProcessor.LogError(e.ToString());
				result.ErrorList.Add(e.Message);
				result.AliasName = "N/A";
				result.TransID = "N/A";
				resultList.Add(result);

				// We must convert to an ArrayList to stay compatible with the legacy application,
				// otherwise the legacy application will report no errors when there are errors
				var resultAsArrayList = new ArrayList(resultList);
				return resultAsArrayList;
			}
			finally
			{
				this.Logout(security);
			}
		}

		// E. Simmons
		// 09-28-2007 Added to support CSI #5186
		[WebMethod]
		public SaveTransmitTranListResultDO ImportEnterpriseCompressedData(byte[] compressedxml)
		{
			// E. Simmons
			// 10-03-2007
			// This function is not implemented because the code below does not compress properly.  
			// Instead of spending time on figuring this problem out, it would be better that
			// I move on with my task of completing CSI #5186 and come back to this later.
			// MemoryStream stream = 
			// new MemoryStream(compressedxml);
			// BZip2InputStream zisUncompressed = new BZip2InputStream(stream);
			// byte[] bytesBuffer = new byte[zisUncompressed.Length];
			// zisUncompressed.Read(bytesBuffer, 0, bytesBuffer.Length);
			// zisUncompressed.Close();
			// stream.Close();
			// string xml = Encoding.ASCII.GetString(bytesBuffer);
			// return _importEnterprise(xml);
			throw new NotImplementedException();
		}

		/// <summary>
		/// This web method is specifically targeted at supporting import of transmission data from FuelsManager 7.5SP2 clients in order
		/// to facilitate transition from 7.5 to a later version enterprise system.  It uses references to 7.5SP2 assemblies to allow
		/// deserialization of the transmission record.
		/// </summary>
		/// <param name="xml">The XML of the data transmission.</param>
		/// <returns>A response object that describes the results of the import.</returns>
		/// [WebMethod(EnableSession = true)]
		[WebMethod]
		public SaveTransmitTranListResultDO ImportEnterpriseData(string xml)
		{
			return this.ImportEnterprise(xml);
		}

		/// <summary>
		/// This web method is specifically targeted at supporting import of transmission data from FuelsManager 7.5SP2 clients in order
		/// to facilitate transition from 7.5 to a later version enterprise system.  It uses references to 7.5SP2 assemblies to allow
		/// deserialization of the transmission record.
		/// </summary>
		/// <param name="xml">The XML of the data transmission.</param>
		/// <returns>A response object that describes the results of the import.</returns>
		/// [WebMethod(EnableSession = true)]
		[WebMethod]
		public FM7Accounting.EntityDataImportResponseDO ImportEntityData( string xml )
		{
			var import = new ImportFuelsManager75(this.Context);
			return import.Import(xml);
		}

		/// <summary>
		/// Tests the login.
		/// </summary>
		/// <returns>True if the login is valid.</returns>
		[WebMethod]
		public bool TestLogin()
		{
			SecurityClass security = null;

			try
			{
				string siteID;
				security = this.Login(out siteID);
			}
			catch (Exception e)
			{
				XMLImportProcessor.LogError(e.ToString());
				return false;
			}
			finally
			{
				this.Logout(security);
			}

			return security != null;
		}

		/// <summary>
		/// Web method used by FuelsManager Service to ping to keep alive
		/// </summary>
		/// <returns></returns>
		[WebMethod]
		public void PingApplicationServer()
		{
			return;
		}

		/// <summary>
		/// Indicate that we support sending import error emails through the alarm and event log system.
		/// This method is here to support the legacy aviation client
		/// </summary>
		/// <returns>True to indicate that we support sending import error emails</returns>
		[WebMethod]
		public bool SupportsAlarmAndEventLogEmails()
		{
			return true;
		}

		/// <summary>
		/// Write text received to the alarm and event log so it can be emailed out
		/// </summary>
		/// <param name="errorText">Text to write to the alarm and event log</param>
		[WebMethod]
		public void LogImportErrorText(string errorText)
		{
			SecurityClass security = null;

			try
			{
				string siteID;
				security = this.Login(out siteID);

				// You must provide some text to email
				if (string.IsNullOrEmpty(errorText))
				{
					throw new ArgumentNullException("errorText");
				}

				// Add the siteID to the error text so that it can be used later
				// when an email is sent. The site goes in the subject line
				// Web service calls can change \r\n to \n, and \n will not appear correctly as a newline in the file.  
				// We do some replaces to ensure that the newlines appear correctly.
				siteID = siteID.PadRight(30);
				string errorTextAndSite = siteID + errorText.Replace(Environment.NewLine, "\n").Replace("\n", Environment.NewLine);

				this.WriteAlarmAndEventLog(security, errorTextAndSite);
			}
			catch (Exception e)
			{
				XMLImportProcessor.LogError(e.ToString());
				throw;
			}
			finally
			{
				this.Logout(security);
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">
		/// The disposing.
		/// </param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}

			base.Dispose(disposing);
		}

		/// <summary>
		/// Validates the login.
		/// </summary>
		/// <param name="siteID">The site ID.</param>
		/// <returns>A validated security object.</returns>
		private SecurityClass Login(out string siteID)
		{
			siteID = null;

			// Read the hardware key first, otherwise someone will have to log in through the user interface
			// for the login to succeed
			FMChannelHelper.MakeCall<IHardwareKey>(hardwareKeyChannel => hardwareKeyChannel.ReadHardwareKey());

			SoapContext requestContext = RequestSoapContext.Current;

			foreach (UsernameToken token in requestContext.Security.Tokens)
			{
				char[] separatorList = { '|', '\0' };
				string[] stringList = token.Username.Split(separatorList, 2);
				siteID = stringList[0];
				string userName = stringList[1];

				SecurityClass security = null;
							
				bool changePassword;
				int daysUntilExpiration;
				
				var loginRequest = new SecurityLoginRequest
					                   {
						                   SiteID = siteID, 
										   UserID = userName,
										   Password = token.Password,
										   TimeOut = -1
					                   };

				string loginResult = FMChannelHelper.MakeCall<ISites, string>(sites => sites.Login(out changePassword, out daysUntilExpiration, out security, loginRequest));

				if (!string.IsNullOrEmpty(loginResult)
					&& (loginResult.StartsWith("User", StringComparison.InvariantCultureIgnoreCase)
						|| loginResult.StartsWith("LOGIN FAILED", StringComparison.InvariantCultureIgnoreCase)))
				{
					throw new Exception(loginResult);
				}

                _currentUserSecurity.SetCurrentSecurityContext(security);
                var currentSite = _siteProxy.Get(security.SiteGuid, false, false, false);
                this._currentUserSecurity.SetCurrentSite(currentSite);

                return security;
			}

			return null;
		}

		/// <summary>
		/// Log the user out of FuelsManager
		/// </summary>
		/// <param name="security">Contains information about the user that should be logged out</param>
		private void Logout(SecurityClass security)
		{
			try
			{
				if (security != null)
				{
					FMChannelHelper.MakeCall<ISites>(sites => sites.Logout(security));
				}
			}
			catch (Exception ex)
			{
				XMLImportProcessor.LogError(ex.ToString());
			}
		}

		/// <summary>
		///     Required method for Designer support - do not modify
		///     the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// Imports enterprise transactions.
		/// </summary>
		/// <param name="xml">The XML.</param>
		/// <returns>A result object.</returns>
		private SaveTransmitTranListResultDO ImportEnterprise(string xml)
		{
			string siteID;
			SecurityClass security = this.Login(out siteID);

			var transmitTranListDO = new TransmitTranListDO { XSDPath = this.Server.MapPath(".\\") };

			transmitTranListDO.LoadDataSetsFromXML(xml);

			var sr = new SaveTransmitTranListSR
			{
				Security = security,
				Transactions = transmitTranListDO
			};

			var result = FMChannelHelper.MakeCall<ISaveTransmitTranListProcessor, SaveTransmitTranListResultDO>(x => x.Process(sr));
			
			this.Logout(security);

			return result;
		}

		/// <summary>
		/// Write an entry to the alarm and event log with any errors from the import
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="errorText">The results of the import</param>
		private void WriteAlarmAndEventLog(SecurityClass security, string errorText)
		{
			if (string.IsNullOrEmpty(errorText))
			{
				return;
			}

			var alarmAndEventLog = new AlarmAndEventLogClass(TransactionAlarmEventDO.FMAEInterfaceImportErrorEventDescriptor)
				                       {
										   AssociatedData = errorText
				                       };

			FMChannelHelper.MakeCall<IAlarmAndEventLogs>(alarmAndEventLogs => alarmAndEventLogs.Add(security, alarmAndEventLog));
		}

		#endregion
	}
}