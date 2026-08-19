/// <summary>
/// File name:	ReportingDA.cs
/// Purpose:	To handle connecting to the database and executing a given
///				SQL statement. For a query, this class returns a data set of
///				the resulting query.
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///	Author(s):	Richard R. Panachida
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:			By:						Reason:
///		----------		--------------------	----------------------------------
///		
/// </summary>

using System.Data.SqlClient;
using FMCommon;
using Microsoft.Win32;


namespace ReportingDAL
{
	public class ReportingDA
	{
		#region Attributes
		private string connect;
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor for the reporting data access object.
		/// </summary>
		public ReportingDA()
		{
			this.FindConnectionString();
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property returns the connection string to the database.
		/// </summary>
		public string ConnectionString
		{
			get { return this.connect; }
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// This method will connect to the database and return a data set for the given
		/// query.
		/// </summary>
		/// <param name="sql"></param>
		/// <returns></returns>
		public System.Data.DataSet GetDataSet(string sql)
		{
			System.Data.DataSet                 resultDataSet = new System.Data.DataSet();
			System.Data.SqlClient.SqlConnection	connection    = new SqlConnection(ConnectionString);
			SqlDataAdapter	                    Adapter       = new SqlDataAdapter(sql, connection); 

			Adapter.Fill(resultDataSet);
			connection.Close();

			return resultDataSet;
		}

		/// <summary>
		/// This method will execute an update, add, or delete SQL command for a 
		/// given SQL statement.
		/// </summary>
		/// <param name="sql"></param>
		public void ExecuteQuery(SecurityClass security, string sql)
		{
			string sSecurityToken = "";
			sql = SecurityClass.CreateChangeLogPreambleString(security, ref sSecurityToken) + sql;


			SqlConnection	connection = new SqlConnection(ConnectionString);
			SqlCommand		command    = new SqlCommand(sql, connection); 

			command.Connection.Open();
			command.ExecuteNonQuery();
			command.Connection.Close();
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will search for the connection string in the windows registry.  If it is
		/// not found, then it is set to a default value.
		/// </summary>
		private void FindConnectionString()
		{
			string	    valueString = "ConnectString";

			System.Security.Permissions.RegistryPermission regPermissions = new 
				System.Security.Permissions.RegistryPermission(System.Security.Permissions.RegistryPermissionAccess.AllAccess,
															   "HKEY_LOCAL_MACHINE\\SOFTWARE\\Varec\\ReportingServices");

			regPermissions.Assert();

			RegistryKey key         = Registry.LocalMachine.OpenSubKey("Software\\Varec\\ReportingServices", false);

			this.connect = null;

			if (key != null)
				this.connect = (string) key.GetValue(valueString);

			if (this.connect == null)
			{
				this.connect = "Data Source=127.0.0.1;Initial Catalog=ConsolidatedDB;Integrated Security=SSPI";
			}

			System.Security.Permissions.RegistryPermission.RevertAssert();
		}
		#endregion
	}
}
