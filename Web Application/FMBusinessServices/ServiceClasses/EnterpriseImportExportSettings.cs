/// Adapted from SettingsDA - not changed too much due to time
/// 

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Web;

using FMBusinessObjects.LogClient;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;

using FMBusinessServices.DataAccessLayer;

namespace FMBusinessServices.ServiceClasses
{
	/// <summary>
	/// Replaces old SettingsDA class
	/// </summary>
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class EnterpriseImportExportSettings : IEnterpriseImportExportSettings
	{
		public DataTable SelectAll (SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException( "security" );
			}

			ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();
			DataSet Set = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				this.SelectAllSQL(cmd);
				Set = ConsolidatedDA.GetDataSet(cmd, security);
			}

			if (Set.Tables != null && Set.Tables.Count > 0)
			{
				return Set.Tables[0];
			}

			return null;

		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Update(SecurityClass security, string settingKey, string settingValue)
		{
			if (security == null)
			{
				throw new ArgumentNullException( "security" );
			}

			ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

			using (SqlCommand cmd = new SqlCommand())
			{
				UpdateSQL(cmd, settingKey, settingValue);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		private void SelectAllSQL(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT [SettingID], [SettingKey], [SettingValue] FROM [tblSettings]";
		}

		private void UpdateSQL(SqlCommand cmd, string settingKey, string settingValue)
		{
			cmd.CommandText = @"IF EXISTS (SELECT 1 FROM tblSettings WHERE SettingKey = @SettingKey) 
									UPDATE tblSettings SET SettingValue = @SettingValue WHERE SettingKey = @SettingKey
								ELSE
									INSERT tblSettings (SettingKey, SettingValue) VALUES (@SettingKey, @SettingValue)";
				
			cmd.Parameters.Add("@SettingKey", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@SettingValue", SqlDbType.NVarChar, 128);

			cmd.Parameters["@SettingKey"].Value = settingKey.Trim();
			cmd.Parameters["@SettingValue"].Value = settingValue.Trim();
		}
	}
}
