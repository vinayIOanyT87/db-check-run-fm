// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportResults.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ExportResultsClass type.
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

	/// <summary>
	/// The export results class.
	/// </summary>
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class ExportResultsClass : IExportResults
	{
		/// <summary>
		/// The consolidated da.
		/// </summary>
		private readonly ConsolidatedDAClass consolidatedDA;

		/// <summary>
		/// Initializes a new instance of the <see cref="ExportResultsClass"/> class.
		/// </summary>
		public ExportResultsClass()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
		}

		/// <summary>
		/// This method will only insert the Export Result and not the Export Result Detail
		/// items. It is used for importing results from the enterprise to the base level
		/// system.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="exportResult">
		/// The export result.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Null argument exception.
		/// </exception>
		/// <exception cref="Exception">
		/// Access denied exception.
		/// </exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void AddFromImport(SecurityClass security, ExportResultClass exportResult)
		{
			if ( security == null )
			{
				throw new ArgumentNullException("security");
			}

			if ( exportResult == null )
			{
				throw new ArgumentNullException("exportResult");
			}

			if ( (security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT) == false) &&
				  (security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA) == false) &&
				  (security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) == false) )
			{
				throw new FMInsufficientRightsException();
			}

			using ( var sqlCommand = new SqlCommand( ) )
			{
				exportResult.InsertSql(sqlCommand);
				this.consolidatedDA.ExecuteQuery(security, sqlCommand);
			}
		}

		/// <summary>
		/// This method will only insert the Export Result and not the Export Result Detail
		/// items.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="exportResult">
		/// The export result.
		/// </param>
		/// <returns>
		/// The <see cref="Guid"/>.
		/// </returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, ExportResultClass exportResult)
		{
			return this.AddWithUserInfo(security, exportResult, useSecurityUserInfo: true);
		}

		/// <summary>
		/// This method will only insert the Export Result and not the Export Result Detail
		/// items.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="exportResult">
		/// The export result.
		/// </param>
		/// <param name="useSecurityUserInfo">
		/// The use security user info.
		/// </param>
		/// <returns>
		/// The <see cref="Guid"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">Null argument exception.
		/// </exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid AddWithUserInfo(SecurityClass security, ExportResultClass exportResult, bool useSecurityUserInfo)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (exportResult == null)
			{
				throw new ArgumentNullException("exportResult");
			}

			if ( (security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT) == false) &&
				  (security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA) == false) &&
				  (security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) == false) )
			{
				throw new FMInsufficientRightsException();
			}

			exportResult.SiteGuid		= security.SiteGuid;
			exportResult.CreatedDate	= DateTimeOffset.UtcNow;
			exportResult.UpdatedDate	= exportResult.CreatedDate;
			exportResult.IdentityGuid	= Guid.NewGuid();
            exportResult.CreatedBy      = string.Empty;
            exportResult.UpdatedBy      = string.Empty;

			if ( useSecurityUserInfo )
			{
				exportResult.CreatedBy = security.UserID;
				exportResult.UpdatedBy = security.UserID;
			}

			using (var cmd = new SqlCommand())
			{
				exportResult.InsertSql(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}

			if (exportResult.ExportResultDetailCollection != null && exportResult.ExportResultDetailCollection.Count != 0)
			{
				var exportResultDetails = new ExportResultDetailsClass();

				foreach (ExportResultDetailClass exportResultDetail in exportResult.ExportResultDetailCollection)
				{
					exportResultDetail.ExportResultGuid = exportResult.IdentityGuid;
					exportResultDetail.IdentityGuid = exportResultDetails.Add(security, exportResultDetail);
				}
			}

			return exportResult.IdentityGuid;
		}

		/// <summary>
		/// The get most recent.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="type">
		/// The type.
		/// </param>
		/// <param name="interfaceName">
		/// The interface name.
		/// </param>
		/// <returns>
		/// The <see cref="ExportResultClass"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">Null argument exception
		/// </exception>
		/// <exception cref="Exception">Access denied.
		/// </exception>
        public ExportResultClass GetMostRecent(SecurityClass security, EXPORT_RESULT_TYPE type, string interfaceName)
        {
			var exportResult = new ExportResultClass { Type = type, InterfaceName = interfaceName, SiteGuid = security.SiteGuid };
            return GetMostRecent(security, type, interfaceName,  exportResult);
        }
        public ExportResultClass GetMostRecent(SecurityClass security, EXPORT_RESULT_TYPE type, string interfaceName, ExportResultClass exportResult)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if ( (security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT) == false) &&
				  (security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA) == false) &&
				  (security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) == false) &&
				  (security.HasRight(RIGHT.VIEW_TRANSACTION_DATA) == false) )
			{
				throw new FMInsufficientRightsException();
			}


			using (var cmd = new SqlCommand())
			{
				exportResult.SelectMostRecentSql(cmd);
				exportResult.Load(this.consolidatedDA.GetDataSet(cmd, security));
			}

			return exportResult;
		}

		/// <summary>
		/// The get max trans version.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="type">
		/// The type.
		/// </param>
		/// <param name="interfaceName">
		/// The interface name.
		/// </param>
		/// <returns>
		/// The <see cref="ExportResultClass"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">Null argument exception.
		/// </exception>
		/// <exception cref="Exception">Access denied exception.
		/// </exception>
        public ExportResultClass GetMaxTransVersion(SecurityClass security, EXPORT_RESULT_TYPE type, string interfaceName)
        {
            var exportResult = new ExportResultClass { Type = type, InterfaceName = interfaceName, SiteGuid = security.SiteGuid };
            return GetMaxTransVersion(security, type, interfaceName, exportResult);
        }

        public ExportResultClass GetMaxTransVersion(SecurityClass security, EXPORT_RESULT_TYPE type, string interfaceName, ExportResultClass exportResult)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if ( (security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT) == false) &&
				  (security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA) == false) &&
				  (security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) == false) &&
				  (security.HasRight(RIGHT.VIEW_TRANSACTION_DATA) == false) )
			{
				throw new FMInsufficientRightsException();
			}

			using (var cmd = new SqlCommand())
			{
				exportResult.SelectMaxTransVersionSql(cmd);
				exportResult.Load(this.consolidatedDA.GetDataSet(cmd, security));
			}

			return exportResult;
		}

		/// <summary>
		/// This method will return the Export Result Index based on the interface name. It
		/// will return null if not found.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="interfaceName">
		/// The interface name.
		/// </param>
		/// <returns>
		/// The <see cref="Guid"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">Null argument exception.
		/// </exception>
		/// <exception cref="Exception">Access denied exception.
		/// </exception>
		public Guid GetGuidByInterfaceName(SecurityClass security, string interfaceName)
		{
			Guid exportResultGuid = Guid.Empty;

			if ( security == null )
			{
				throw new ArgumentNullException("security");
			}

			if ( (security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT) == false) &&
				  (security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA) == false) &&
				  (security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) == false) &&
				  (security.HasRight(RIGHT.VIEW_TRANSACTION_DATA) == false) )
			{
				throw new FMInsufficientRightsException();
			}

			var exportResult = new ExportResultClass { InterfaceName = interfaceName };
			DataSet dataSet;

			using ( var sqlCommand = new SqlCommand( ) )
			{
				exportResult.GetGuidByInterfaceNameSql(sqlCommand);
				dataSet = this.consolidatedDA.GetDataSet(sqlCommand, security);
			}
			
			if ( (dataSet != null) && (dataSet.Tables.Count > 0) )
			{
				DataTable table = dataSet.Tables[0];

				if ( (table != null) && (table.Rows != null) && (table.Rows.Count > 0) )
				{
					DataRow row = table.Rows[0];
					exportResultGuid = row.IsNull("ExportResultGuid") ? Guid.Empty : (Guid) row["ExportResultGuid"];
				}
			}

			return exportResultGuid;
		}
	}
}
