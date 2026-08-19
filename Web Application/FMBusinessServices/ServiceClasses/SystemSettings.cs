// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemSettings.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;

	using IsolationLevel = System.Transactions.IsolationLevel;

	[SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = IsolationLevel.ReadCommitted)]
	public class SystemSettingsClass : ISystemSettings
	{
		#region Constants and Fields

		private readonly ConsolidatedDAClass consolidatedDA = new ConsolidatedDAClass();

		#endregion

		#region Public Methods and Operators

		public SystemSettingClass Get(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var systemSetting = new SystemSettingClass();
			using (var cmd = new SqlCommand())
			{
				systemSetting.SelectSQL(cmd, ContextUtil.IsInTransaction);
				DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, security);
				if (dataSet.Tables.Count != 0 && dataSet.Tables[0].Rows.Count != 0)
				{
					systemSetting.LoadObject(dataSet);
				}
				else
				{
					// Add default System Setting record to database since it doesn't exist
					using (var addCmd = new SqlCommand())
					{
						systemSetting.InsertSQL(addCmd);
						this.consolidatedDA.ExecuteQuery(security, addCmd);
					}
				}
			}
			return systemSetting;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, SystemSettingClass systemSetting)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (systemSetting == null)
			{
				throw new ArgumentNullException("systemSetting");
			}

			// Must have Modify System Settings security right
			if (security.HasRight(RIGHT.MODIFY_SYSTEM_SETTINGS) == false)
			{
				throw new FMInsufficientRightsException();
			}

			SystemSettingClass oldSystemSetting = this.Get(security);

			// If the report server password is the dummy masked password text, 
			// it has not been modified by the user and the existing value should be preserved
			if (systemSetting.ReportServerPassword == SystemSettingClass.MaskedPasswordText)
			{
				systemSetting.ReportServerPassword = oldSystemSetting.ReportServerPassword;
			}

			systemSetting.UpdatedDate = DateTimeOffset.Now;
			systemSetting.UpdatedBy = security.UserID;
			using (var cmd = new SqlCommand())
			{
				systemSetting.UpdateSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		#endregion
	}
}