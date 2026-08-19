using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Resources;



namespace FuelsManagerEnterpriseExportWndSvs
{
	public class CloseoutDependencyClass
	{
		private DataSet m_DataSet;
		private SqlConnection m_connection;
		private SqlCommand m_command;
		private string m_strConnectionString = "";
		private string m_strSQLQuery = "";
		private EventLog m_eventLog;


		public CloseoutDependencyClass()
		{
		}


		public Boolean InitializeCloseoutDependencyClass(EventLog log)
		{

			string strFunctionName = "InitializeCloseoutDependencyClass((EventLog log)";
			Boolean bResults = false;
			m_connection = null;
			m_DataSet = null;
			m_connection = null;
			m_command = null;
			m_strConnectionString = String.Empty;
			m_strSQLQuery = String.Empty;

			try
			{
				m_eventLog = log;

				m_strConnectionString = ReadConnectionStringFromRegistry();
				//m_strQueueName = "NewCloseoutAddedQueue";
				m_strSQLQuery = "select [CloseOutInventoryID] from dbo.tblCloseoutInventory order by CloseOutInventoryID"; // just want to select on ID


				m_connection = new SqlConnection(m_strConnectionString);


				if (m_connection == null)
				{
					string msg = String.Format("Warning: 'FuelsManager Enterprise Export Windows Service' is unable to make a connection to SQL server."); // Attempt number: {0} out of {1} attempts.", nTimesToTry.ToString(), nTimesToTry.ToString());
					m_eventLog.WriteEntry(msg);

				}

			}
			catch (Exception ex)
			{
				String strAdditionalErrorInfo = String.Format(System.Globalization.CultureInfo.CurrentCulture, "Exception in object: {0}, Function {1}, Message: {2}.", this.ToString(), strFunctionName, ex.Message);
				m_eventLog.WriteEntry(strAdditionalErrorInfo, EventLogEntryType.Error);
				throw (ex);
			}
			return bResults;
		}



		~CloseoutDependencyClass()
		{
			Closing();
		}


		protected void Dispose()
		{
			if (m_DataSet != null)
			{
				m_DataSet.Clear();
				m_DataSet.Dispose();
			}

			if (m_connection != null)
			{
				m_connection.Close();
				m_connection.Dispose();
			}

			if (m_command != null)
			{
				m_command.Dispose();
			}
		}

		public bool EnoughPermission()
		{
			SqlClientPermission perm = new SqlClientPermission(System.Security.Permissions.PermissionState.Unrestricted);
			try
			{
				perm.Demand();
				return true;
			}
			catch (System.Exception)
			{
				throw;
			}
		}


		public Boolean DependsRun()
		{
			string strFunctionName = "DependsRun()";
			try
			{
				//int nTimesToTry = 50;
				//int nSecondsToWaitBetweenStartUpTries = 60;   // 60 seconds
				//int nSecondsToWaitBetweenTries = 30; // 30 seconds
				//System.Threading.Thread.Sleep(nSecondsToWaitBetweenStartUpTries * 1000); // wait  and try again SQL Server service may not be running yet.
				//for (int i = 1; i < nTimesToTry; i++)
				//{
					//try
					//{
						// Remove any existing dependency connection, then create a new one. 

						SqlDependency.Stop(m_strConnectionString);
						SqlDependency.Start(m_strConnectionString);

						if (m_connection == null)
						{
							m_connection = new SqlConnection(m_strConnectionString);
						}
						if (m_command == null)
						{
							m_command = new SqlCommand(m_strSQLQuery, m_connection);
						}
						if (m_DataSet == null)
						{
							m_DataSet = new DataSet();
							m_DataSet.Locale = System.Globalization.CultureInfo.CurrentCulture;
						}


						//break; // if I got here then I made a connection and the SQL server is running 

					
					//}
					//catch (Exception ex)					{

					//    string msg = String.Format("Exception starting in object: {0}, function: {1}, Exception: {2}, Try number: {3} out of {4} attemps, will wait {5} seconds and try again.", this.ToString(), strFunctionName, ex.Message, i.ToString(), nTimesToTry.ToString(), nSecondsToWaitBetweenTries.ToString());
					//    m_eventLog.WriteEntry(msg, EventLogEntryType.Warning);
					//    System.Threading.Thread.Sleep(nSecondsToWaitBetweenTries * 1000); // wait  and try again SQL Server service may not be running yet.
					//}

				//}

				SubscriptToInsertDependencyOnTblTblCloseoutInventory();

			}
			catch (Exception ex)
			{
				String strAdditionalErrorInfo = String.Format(System.Globalization.CultureInfo.CurrentCulture, "Exception in object: {0}, Function {1}, Message: {2}.", this.ToString(), strFunctionName, ex.Message);
				m_eventLog.WriteEntry(strAdditionalErrorInfo, EventLogEntryType.Error);
				return false;
			}
			return true;
		}



		private void SubscriptToInsertDependencyOnTblTblCloseoutInventory()
		{
			string strFunctionName = "SubscriptToInsertDependencyOnTblTblCloseoutInventory()";
			try
			{
				if (m_DataSet != null)
				{
					m_DataSet.Clear();
				}
				// Ensure the command object does not have a notification object.
				m_command.Notification = null;
				// Create and bind the SqlDependency object to the command object.
				//qlDependency dependency = new SqlDependency(m_command);
				SqlDependency dependency = new SqlDependency(m_command, null, 0);   // 0 means accept default. 
				dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);

				
				//int numofColumns = Convert.ToInt32(m_command.ExecuteScalar());
				//msg = String.Format("Number of Records: {0}", numofColumns);

				// you have to do a fill in order to make the dependency work. 
				using (SqlDataAdapter adapter = new SqlDataAdapter(m_command))
				{
				    adapter.Fill(m_DataSet, "CloseoutInventory");
				    DataTable tblCloseoutInventory = m_DataSet.Tables["CloseoutInventory"];
				//    // for testing 
					//int i = 0;
					//msg = "";
					//for (i = 0; i < tblCloseoutInventory.Rows.Count; i++)
					//{
					//    DataRow row = tblCloseoutInventory.Rows[i];
					//    msg = String.Format(System.Globalization.CultureInfo.CurrentCulture, "CloseoutInventoryId: {0}.",
					//    row["CloseoutInventoryId"].ToString());
						//msg = String.Format("CloseoutInventoryId: {0}, Site: {1}, Siteindex: {2}.",
						// row["CloseoutInventoryId"].ToString(),
						// row["site"].ToString(),
						// Convert.ToString(row["SiteIndex"]));
						// Console.WriteLine(msg);
					//}
					//m_eventLog.WriteEntry(msg, EventLogEntryType.Information); // for testing 
					// Console.WriteLine(msg); // write last line
				}

				// if you get here then it is set up
				string msg = "Windows service 'FuelsManager Enterprise Export Window Service' set up an 'insert' dependency on the table 'tblCloseoutInventory'.";
				m_eventLog.WriteEntry(msg, EventLogEntryType.Information); 


			}

			catch (Exception ex)
			{
				String strAdditionalErrorInfo = String.Format(System.Globalization.CultureInfo.CurrentCulture, "Exception in object: {0}, Function {1}, Message: {2}.", this.ToString(), strFunctionName, ex.Message);
				m_eventLog.WriteEntry(strAdditionalErrorInfo, EventLogEntryType.Error);
				throw;
			}
		}



		//delegate void UIDelegate();
		private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
		{
			string strFunctionName = "dependency_OnChange(object sender, SqlNotificationEventArgs e)";
			try
			{
				string msg = "A Data Export in the windows service 'FuelsManager Enterprise Export Window Service' has been triggered by an Insert into the table 'tblCloseoutInventory'.";
				m_eventLog.WriteEntry(msg, EventLogEntryType.Information);
				if (e.Info == System.Data.SqlClient.SqlNotificationInfo.Insert)
				{
					DataExport de = new DataExport();
					de.Export();
				}
				//Remove the handler as it is used for a single notification.
				SqlDependency dependency = (SqlDependency)sender;
				dependency.OnChange -= dependency_OnChange;

				msg = "Insert dependency on the table 'tblCloseoutInventory' has been removed in the windows service 'FuelsManager Enterprise Export Window Service'.";
				m_eventLog.WriteEntry(msg, EventLogEntryType.Information);

				SubscriptToInsertDependencyOnTblTblCloseoutInventory();

			}
			catch (Exception ex)
			{
				String strAdditionalErrorInfo = String.Format(System.Globalization.CultureInfo.CurrentCulture, "Exception in object: {0}, Function {1}, Message: {2}.", this.ToString(), strFunctionName, ex.Message);
				m_eventLog.WriteEntry(strAdditionalErrorInfo, EventLogEntryType.Error);
				throw;
			}
		}


		//private void RefreshData()
		//{
		// Console.WriteLine("Database had some changes." );

		// // Reload the dataset that is bound to the grid.
		// GetCloseoutInventoryData();
		//}


		private void Closing()
		{
			SqlDependency.Stop(m_strConnectionString);
			if (m_connection != null)
			{
				m_connection.Close();
			}
			Dispose();
		}


		private string ReadConnectionStringFromRegistry()
		{
			string strFunctionName = "ReadConnectionStringFromRegistry()";
			string strConnectionString = "";
			try
			{
				// Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.LocalMachine;
				string strSubKey = "SOFTWARE\\Varec\\Accounting\\";
				Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(strSubKey, true);
				if (key != null)
				{
					strConnectionString = (string)key.GetValue("ConnectString", "");
					key.Close();
				}
			}
			catch (Exception ex)
			{
				String strAdditionalErrorInfo = String.Format(System.Globalization.CultureInfo.CurrentCulture, "Exception in object: {0}, Function {1}, Message: {2}.", this.ToString(), strFunctionName, ex.Message);
				m_eventLog.WriteEntry(strAdditionalErrorInfo, EventLogEntryType.Error);
				throw;
			}
			return strConnectionString;
		}
	}
}
