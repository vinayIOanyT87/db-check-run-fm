using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.UtilityObjects;

using FMBusinessServices.DataAccessLayer;

namespace FMBusinessServices.ServiceClasses
{
	/// Common classes and functions used in the Enterprise Export Windows Service and and Import Web service.	
	/// Prerequisit:  connection string in registry, settings in database table tblSettings. 
	public class EnterpriseExportImportClass : IEnterpriseExportImport
	{
		private AlarmAndEventLogClass alarmAndEventLog;
		private EventLog eventLog;
		private SecurityClass security;

		private EnterpriseExportImportDO enterpriseExportImportDO;

		public EnterpriseExportImportClass()
		{

		}

		public EnterpriseExportImportClass(SecurityClass inSecurity, String strEventlogSource)
		{
			Init(inSecurity, strEventlogSource);
		}

		private void Init(SecurityClass inSecurity, String strEventlogSource)
		{
			security = inSecurity;

			if (string.IsNullOrEmpty(strEventlogSource))
			{
				strEventlogSource = "FuelsManager";
			}

			if (EventLog.SourceExists(strEventlogSource) == false)
			{
				EventLog.CreateEventSource(strEventlogSource, "Application");
			}

			eventLog = new EventLog("Application", ".", strEventlogSource);

			alarmAndEventLog = new AlarmAndEventLogClass();
			alarmAndEventLog.Source = strEventlogSource;
		}

		public EnterpriseExportImportDO ReadSettings(SecurityClass inSecurity, String strEventlogSource)
		{
			Init(inSecurity, strEventlogSource);

			string strFunctionName = "ReadSettings()";
			this.enterpriseExportImportDO = new EnterpriseExportImportDO();

			try
			{
				Settings setting = new Settings();
				setting.AlarmAndEventSourceName = alarmAndEventLog.Source;

				string strEnterpriseDataSendAttempts = setting.ReadSettingValue(this.security, "EnterpriseDataSendAttempts");

				if (strEnterpriseDataSendAttempts.Length > 0)
				{
					this.enterpriseExportImportDO.EnterpriseDataSendAttempts = Convert.ToInt32(strEnterpriseDataSendAttempts);
				}

				this.enterpriseExportImportDO.ExportArchiveDir = setting.ReadSettingValue(security, "ExportArchiveDir");
				this.enterpriseExportImportDO.ImportArchiveDir = setting.ReadSettingValue(security, "ImportArchiveDir");
				this.enterpriseExportImportDO.ExportingSiteGuid = setting.ReadSettingValue(security, "ExportingSiteGuid");

				string strEnterpriseDataIntervalBetweenSendAttemptsInMinutes = setting.ReadSettingValue(security, "EnterpriseDataIntervalBetweenSendAttemptsInMinutes");

				if (strEnterpriseDataIntervalBetweenSendAttemptsInMinutes.Length > 0)
				{
					int enterpriseDataIntervalBetweenSendAttemptsInMinutes = Convert.ToInt32(strEnterpriseDataIntervalBetweenSendAttemptsInMinutes);
					this.enterpriseExportImportDO.EnterpriseDataIntervalBetweenSendAttemptsInSeconds = enterpriseDataIntervalBetweenSendAttemptsInMinutes * 60;
				}

				this.enterpriseExportImportDO.URLofEnterpriseDataWebService = setting.ReadSettingValue(security, "URLofEnterpriseDataWebService");
				this.enterpriseExportImportDO.LogImportProcessInformation = Convert.ToBoolean(setting.ReadSettingValue(security, "LogImportProcessRunInformation"));
			}
			catch (Exception ex)
			{
				String strAdditionalMessage = String.Format("Exception in object: {0}, Function {1}, Message: {2}.", this.ToString(), strFunctionName, ex.Message);
				WriteToEventLogs(inSecurity, strEventlogSource, strAdditionalMessage, EventLogEntryType.Error);
				throw ex;
			}

			return this.enterpriseExportImportDO;
		}

		public void WriteToEventLogs(SecurityClass security, string eventLogSource, string strMessage, EventLogEntryType eventLogEntryType)
		{
			Init( security, eventLogSource );

			if (string.IsNullOrEmpty(strMessage) == true)
			{
				return;
			}

			this.eventLog.Source = "FuelsManager";
			eventLog.WriteEntry(strMessage, eventLogEntryType);

			alarmAndEventLog.Alarm = true;
			int maxlengthOfMessage = 1000; // this is a large as the associated data can hold.

			string strSQLSafeMessage = strMessage.Replace("'", "''");  //escape the single quote that shows up in some error messages. 

			if (strSQLSafeMessage.Length > maxlengthOfMessage)
			{
				alarmAndEventLog.AssociatedData = strSQLSafeMessage.Remove(maxlengthOfMessage);
			}
			else
			{
				alarmAndEventLog.AssociatedData = strSQLSafeMessage;
			}

			alarmAndEventLog.SiteGuid = security.SiteGuid;
			alarmAndEventLog.UpdatedDate = DateTimeOffset.Now;
			alarmAndEventLog.UpdatedBy = security.UserID ?? "FuelsManager";
			alarmAndEventLog.CreatedDate = DateTimeOffset.Now;
			alarmAndEventLog.CreatedBy = security.UserID ?? "FuelsManager";
			alarmAndEventLog.ID = "Enterprise Export Import Event";

			AlarmAndEventLogsClass alarmAndEventLogs = new AlarmAndEventLogsClass();
			alarmAndEventLogs.Add(security, alarmAndEventLog);
		}

		public string WriteStreamToFile(SecurityClass security, string eventLogSource, MemoryStream stream, string strDirPathToWriteTo)
		{
			Init(security, eventLogSource);

			string result = null;
			string strFunctionName = "WriteStreamToFile(MemoryStream stream, String strDirPathToWriteTo)";

			try
			{
				SitesClass sites = new SitesClass();
				SiteClass site = sites.Get(security, security.SiteGuid, true, true, true);

				DateTimeOffset siteTimeNow = TimeConverter.Now(site);
				string fileName = site.ID + siteTimeNow.ToString("yyyyMMdd_HHmmssfff") + ".xml";

				// If the Export Archive Directory exist save the file to the directory.			
				if (strDirPathToWriteTo != null)
				{
					string strPath = strDirPathToWriteTo.Trim();

					if (strPath.Length > 0)
					{
						DirectoryInfo directoryInfo = new DirectoryInfo(strPath);

						if (!directoryInfo.Exists)
						{
							throw new Exception("Archive Directory Error, check Site configuration.");
						}

						string strBackSlash = "\\";

						if (!(strPath.EndsWith(strBackSlash)))
						{
							strPath += strBackSlash;
						}

						string strArchiveDirAndFileName = strPath + fileName;
						result = strArchiveDirAndFileName;
						FileStream fstream = new FileStream(strArchiveDirAndFileName, FileMode.Create);
						stream.WriteTo(fstream);
						fstream.Flush();
						fstream.Close();
					}
				}

				return result;
			}
			catch (Exception ex)
			{
				String strAdditionalMessage = String.Format("Exception in object: {0}, Function {1}, Message: {2}.", this.ToString(), strFunctionName, ex.Message);
				WriteToEventLogs(security, eventLogSource, strAdditionalMessage, EventLogEntryType.Error);
				throw ex;
			}
		}
	}

	class Settings
	{
		private int m_nSettingID;
		private string m_strSettingKey;
		private string m_strSettingValue;
		private string m_AlarmAndEventSourceName;

		public Settings()
		{
		}

		public String AlarmAndEventSourceName
		{
			get { return m_AlarmAndEventSourceName; }

			set { m_AlarmAndEventSourceName = value; }
		}

		public string SettingValue
		{
			get { return m_strSettingValue; }
			set { m_strSettingValue = value; }
		}

		public string stringSettingKey
		{
			get { return m_strSettingKey; }
			set { m_strSettingKey = value; }
		}

		public int SettingID
		{
			get { return m_nSettingID; }
		}

		public string ReadSettingValue(SecurityClass security, string strKey)
		{
			ConsolidatedDAClass da = new ConsolidatedDAClass();

			m_strSettingValue = string.Empty;
			string strFunctionName = "ReadSettingValue ( string strKey )";
			try
			{
				using (SqlCommand myCommand = new SqlCommand())
				{
					GetSelectSQL(myCommand, strKey);

					DataSet ds = da.GetDataSet(myCommand, security);

					myCommand.CommandType = CommandType.Text;

					if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
					{
						DataRow dr = ds.Tables[0].Rows[0];

						this.m_strSettingValue = DataObject.getValue<string>(dr["SettingValue"], string.Empty);

						this.m_nSettingID = DataObject.getValue<int>(dr["SettingID"], 0);

						this.m_strSettingKey = DataObject.getValue<string>(dr["SettingKey"], string.Empty);
					}
				}
			}
			catch (Exception ex)
			{
				String strAdditionalErrorInfo = String.Format("Error in object: {0}, Function: {1}, Message: {2}", this.ToString(), strFunctionName, ex.ToString());
				System.Diagnostics.EventLog eventLog = new System.Diagnostics.EventLog("Application", ".", m_AlarmAndEventSourceName);
				eventLog.WriteEntry(strAdditionalErrorInfo, EventLogEntryType.Error);
				throw;
			}

			return m_strSettingValue;
		}


		protected void GetSelectSQL(SqlCommand cmd, string strKey)
		{
			string strFunctionName = "GetSelectSQL (string strKey)";
			try
			{
				cmd.CommandText = String.Format("Select SettingID, SettingKey, SettingValue from tblSettings where [SettingKey] = @SettingKey", strKey);

				cmd.Parameters.AddWithValue("@SettingKey", strKey);
			}
			catch (Exception ex)
			{
				String strAdditionalErrorInfo = String.Format("Error in object: {0}, Function: {1}, Message: {2}", this.ToString(), strFunctionName, ex.ToString());
				System.Diagnostics.EventLog eventLog = new System.Diagnostics.EventLog("Application", ".", m_AlarmAndEventSourceName);
				eventLog.WriteEntry(strAdditionalErrorInfo, EventLogEntryType.Error);
				throw;
			}
		}
	}
}
