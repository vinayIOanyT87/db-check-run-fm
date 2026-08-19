using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Data;


namespace FuelsManagerEnterpriseExportWndSvs
{
	class Settings
	{
		private string m_strConnectionString;
		private int m_nSettingID;
		private string m_strSettingKey;
		private string m_strSettingValue;
	


		public Settings()
		{

		}

		public Settings(string strConnectionString)
		{
			m_strConnectionString = strConnectionString;
		}

		public string SettingValue
		{
			get { return m_strSettingValue; }
			set { m_strSettingValue = value; }
		}

		public string ConnectionString
		{
			get { return m_strConnectionString; }
			set { m_strConnectionString = value; }
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
		



		public string ReadSettingValue(string strKey)
		{

			m_strSettingValue = "";
			string strFunctionName = "ReadSettingValue ( string strKey )";
			SqlConnection myConnection = new SqlConnection();
			try
			{

				myConnection.ConnectionString = m_strConnectionString;

				SqlDataAdapter myAdapter = new SqlDataAdapter();
				myAdapter.TableMappings.Add("Table", "tblSettings");
				myConnection.Open();

				string strSqlSelect = GetSelectSQL(strKey);

				SqlCommand myCommand = new SqlCommand(strSqlSelect, myConnection);

				myCommand.CommandType = CommandType.Text;

				myAdapter.SelectCommand = myCommand;

				DataSet ds = new DataSet("Settings");

				myAdapter.Fill(ds);
				myConnection.Close();

				DataTable tblSettings = ds.Tables["tblSettings"];
				if (tblSettings.Rows.Count > 0)
				{
					DataRow dr = tblSettings.Rows[0];

					m_strSettingValue = (String)dr["SettingValue"];

					m_nSettingID = (int)dr["SettingID"];

					m_strSettingKey = (String)dr["SettingKey"];


				}
			}
			catch (Exception ex)
			{
				if (myConnection.State == ConnectionState.Open)
				{
					myConnection.Close();
				}

				String strAdditionalErrorInfo = String.Format("Error in object: {0}, Function: {1}, Message: {0}", this.ToString(), strFunctionName, ex.Message);
				System.Diagnostics.EventLog eventLog = new System.Diagnostics.EventLog("Application", ".", "DataExport");
				eventLog.WriteEntry(strAdditionalErrorInfo, EventLogEntryType.Error);
				throw (ex);
			}


			return m_strSettingValue;
		}


		protected string GetSelectSQL(string strKey)
		{

			string strSQL = "";
			string strFunctionName = "GetSelectSQL (string strKey)";
			try
			{

				strSQL = String.Format("Select SettingID, SettingKey, SettingValue from tblSettings where [SettingKey] = '{0}'", strKey);

			}
			catch (Exception ex)
			{
				String strAdditionalErrorInfo = String.Format("Error in object: {0}, Function: {1}, Message: {0}", this.ToString(), strFunctionName, ex.Message);
				System.Diagnostics.EventLog eventLog = new System.Diagnostics.EventLog("Application", ".", "DataExport");
				eventLog.WriteEntry(strAdditionalErrorInfo, EventLogEntryType.Error);
				throw (ex);
			}

			return strSQL;
		}

	}
}
