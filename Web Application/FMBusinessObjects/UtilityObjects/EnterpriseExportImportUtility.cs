// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnterpriseExportImportUtility.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the EnterpriseExportImportUtility type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.UtilityObjects
{
	using System;
	using System.Diagnostics;
	using System.IO;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	public class EnterpriseExportImportUtility
	{
		private EnterpriseExportImportDO enterpriseExportImportDO;	// set from Database tblSettings

		public SecurityClass Security { get; set; }

		public EnterpriseExportImportUtility(SecurityClass inSecurity, String strEventlogSource)
		{
			this.Security = inSecurity;

			this.enterpriseExportImportDO = new EnterpriseExportImportDO();
			this.enterpriseExportImportDO.AlarmAndEventSourceName = strEventlogSource;

			this.ReadSettings();
		}

		#region Properties
		public String AlarmAndEventSourceName
		{
			get { return this.enterpriseExportImportDO.AlarmAndEventSourceName; }
			set { this.enterpriseExportImportDO.AlarmAndEventSourceName = value; }
		}

		public String ExportArchiveDir
		{
			get { return this.enterpriseExportImportDO.ExportArchiveDir; }
			set { this.enterpriseExportImportDO.ExportArchiveDir = value; }
		}

		public String ImportArchiveDir
		{
			get { return this.enterpriseExportImportDO.ImportArchiveDir; }
			set { this.enterpriseExportImportDO.ImportArchiveDir = value; }
		}
		public String URLofEnterpriseDataWebService
		{
			get { return this.enterpriseExportImportDO.URLofEnterpriseDataWebService; }
			set { this.enterpriseExportImportDO.URLofEnterpriseDataWebService = value; }
		}

		public String ExportingSiteGuid
		{
			get { return this.enterpriseExportImportDO.SiteGuid.ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value) == false)
				{
					try
					{
						this.enterpriseExportImportDO.SiteGuid = Guid.Parse(value);
					}
					catch (Exception)
					{
						this.enterpriseExportImportDO.SiteGuid = Guid.Empty;
					}
				}
				else
				{
					this.enterpriseExportImportDO.SiteGuid = Guid.Empty;
				}
			}
		}

		public int EnterpriseDataIntervalBetweenSendAttemptsInSeconds
		{
			get { return this.enterpriseExportImportDO.EnterpriseDataIntervalBetweenSendAttemptsInSeconds; }
		}

		public int EnterpriseDataSendAttempts
		{
			get { return this.enterpriseExportImportDO.EnterpriseDataSendAttempts; }
		}

		public bool LogImportProcessInformation
		{
			get { return this.enterpriseExportImportDO.LogImportProcessInformation; }
		}
		#endregion

		#region Private methods
		private void ReadSettings()
		{
			try
			{
				this.enterpriseExportImportDO =
					FMChannelHelper.MakeCall<IEnterpriseExportImport, EnterpriseExportImportDO>(
						x => x.ReadSettings(this.Security, this.enterpriseExportImportDO.AlarmAndEventSourceName));
			}
			catch (Exception ex)
			{
				String strAdditionalMessage = String.Format("Exception in object: {0}, Function {1}, Message: {2}.", this.ToString(), "Reading Settings", ex.Message);
				WriteToEventLogs(strAdditionalMessage, EventLogEntryType.Error);
				throw ex;
			}
		}
		#endregion

		public void WriteToEventLogs(string strMessage)
		{
			this.WriteToEventLogs(strMessage, EventLogEntryType.Error);
		}

		public void WriteToEventLogs(string strMessage, EventLogEntryType eventLogEntryType)
		{
			if (string.IsNullOrEmpty(strMessage) == true)
			{
				return;
			}

			FMChannelHelper.MakeCall<IEnterpriseExportImport>(
				x =>
				x.WriteToEventLogs(
					this.Security, this.enterpriseExportImportDO.AlarmAndEventSourceName, strMessage, eventLogEntryType));
		}

		public string WriteStreamToFile(MemoryStream stream, string strDirPathToWriteTo)
		{
			string result = null;

			try
			{
				result =
					FMChannelHelper.MakeCall<IEnterpriseExportImport, string>(
						x =>
						x.WriteStreamToFile(
							this.Security, this.enterpriseExportImportDO.AlarmAndEventSourceName, stream, strDirPathToWriteTo));

				return result;
			}
			catch (Exception ex)
			{
				String strAdditionalMessage = String.Format("Exception in object: {0}, Function {1}, Message: {2}.", this.ToString(), "WriteStreamFile()", ex.Message);
				throw new Exception(strAdditionalMessage);
			}
		}
	}
}
