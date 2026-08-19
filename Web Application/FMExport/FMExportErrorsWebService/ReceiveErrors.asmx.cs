// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReceiveErrors.asmx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FMExportErrors web service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMExportErrorsWebService
{
	using System;
	using System.ComponentModel;
	using System.IO;
	using System.Web.Services;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	[Serializable]
	public class ReceiveErrorResponse
	{
		public bool OperationSucessful;
		public string OperationErrorText;

		public ReceiveErrorResponse()
		{
			this.OperationSucessful = true;
			this.OperationErrorText = "OK";
		}
	}

	/// <summary>
	/// Summary description for Service1
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[ToolboxItem(false)]
	public class ReceiveErrorsClass : WebService
	{
		/// <summary>
		/// The FuelsManager security object
		/// </summary>
		private SecurityClass security;

		private ReceiveErrorResponse ValidateConfig(ExportSiteEmailAddressClass config)
		{
			var response = new ReceiveErrorResponse();
			string strResponseText = string.Empty;

			// Verify that Configuration Exists
			if (config == null)
			{
				response.OperationSucessful = false;
				response.OperationErrorText = "There is no email configuration record for this site.";
				return response;
			}

			// Verify that folder is not empty
			if (config.ErrorFolder == string.Empty)
			{
				response.OperationSucessful = false;
				strResponseText += Environment.NewLine + "The error folder for this site has not been configured.";
			}
			else
			{
				// Verify that folder exists
				if (!Directory.Exists(config.ErrorFolder))
				{
					response.OperationSucessful = false;
					strResponseText += Environment.NewLine + "The error folder for this site does not exist on the system.";
				}
			}

			// Verify that emails are configured for this site
			if (config.EmailAddresses.Count == 0)
			{
				response.OperationSucessful = false;
				strResponseText += Environment.NewLine + "There are no email addresses configured for this site.";
			}

			if (!string.IsNullOrEmpty(strResponseText))
			{
				response.OperationErrorText = strResponseText;
			}

			return response;
		}

		private bool ValidateFileName(string fileName)
		{
			// Filename cannot be empty
			if (string.IsNullOrWhiteSpace(fileName))
			{
				return false;
			}

			// Filename cannot have invalid characters
			if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
			{
				return false;
			}

			return true;
		}

		[WebMethod]
		public ReceiveErrorResponse Receive(string site, string fileName, string data)
		{
			var response = new ReceiveErrorResponse();
			if (string.IsNullOrEmpty(data))
			{
				response.OperationSucessful = false;
				response.OperationErrorText = "Data is empty or null";
				return response;
			}

			try
			{
				string updatedFileName = string.Empty;
				int index = fileName.LastIndexOf('.');
				if (index == -1)
				{
					updatedFileName = fileName;
				}
				else
				{
					updatedFileName = fileName.Substring(0, index) + ".txt";
				}

				ExportSiteEmailAddressClass emailConfig =
					FMChannelHelper.MakeCall<IExportSiteEmailAddresses, ExportSiteEmailAddressClass>(
						exportSiteEmailAddresses => exportSiteEmailAddresses.GetConfigBySiteId(this.security, site));

				response = this.ValidateConfig(emailConfig);
				if (!response.OperationSucessful)
				{
					return response;
				}

				string fullFilePath = emailConfig.ErrorFolder + updatedFileName;
				if (!this.ValidateFileName(updatedFileName))
				{
					response.OperationSucessful = false;
					response.OperationErrorText = "\"" + updatedFileName + "\" is an invalid file name.";
					return response;
				}

				FileStream fs = File.Open(fullFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
				var writer = new StreamWriter(fs);
				string reformatedData = data.Replace("\n", Environment.NewLine);
				writer.Write(reformatedData);
				writer.Close();
			}
			catch (Exception ex)
			{
				response.OperationSucessful = false;
				response.OperationErrorText = ex.Message;
			}

			return response;
		}

		[WebMethod]
		public ReceiveErrorResponse SiteConfigured(string site)
		{
			var response = new ReceiveErrorResponse();
			try
			{
				ExportSiteEmailAddressClass emailConfig =
					FMChannelHelper.MakeCall<IExportSiteEmailAddresses, ExportSiteEmailAddressClass>(
						exportSiteEmailAddresses => exportSiteEmailAddresses.GetConfigBySiteId(this.security, site));

				response = this.ValidateConfig(emailConfig);
			}
			catch (Exception ex)
			{
				response.OperationSucessful = false;
				response.OperationErrorText = ex.Message;
			}

			return response;
		}
	}
}
