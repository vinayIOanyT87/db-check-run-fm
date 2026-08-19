// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportResultDetails.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ExportResultDetailsClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	using FMBusinessServices.DataAccessLayer;

	/// <summary>
	/// The export result details class.
	/// </summary>
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class ExportResultDetailsClass : IExportResultDetails
	{
		/// <summary>
		/// The consolidated data access.
		/// </summary>
		private readonly ConsolidatedDAClass consolidatedDA;

		/// <summary>
		/// Initializes a new instance of the <see cref="ExportResultDetailsClass"/> class.
		/// </summary>
		public ExportResultDetailsClass()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
		}

		/// <summary>
		/// The save from import.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="exportResultDetail">
		/// The export result detail.
		/// </param>
		/// <returns>
		/// The <see cref="Guid"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">Null argument exception.
		/// </exception>
		/// <exception cref="Exception">Access denied exception.
		/// </exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid SaveFromImport(SecurityClass security, ExportResultDetailClass exportResultDetail)
		{
			if ( security == null )
			{
				throw new ArgumentNullException("security");
			}

			if ( exportResultDetail == null )
			{
				throw new ArgumentNullException("exportResultDetail");
			}

			if ( (security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT) == false) &&
				  (security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA) == false) &&
				  (security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) == false) )
			{
				throw new FMInsufficientRightsException();
			}

			exportResultDetail.CreatedBy = security.UserID;
			exportResultDetail.UpdatedBy = security.UserID;

			if ( exportResultDetail.ExportResultGuid == Guid.Empty )
			{
				DataSet dataSetIdentity;
				using (var sqlCommand = new SqlCommand())
				{
					exportResultDetail.InsertSql(sqlCommand);

					dataSetIdentity = this.consolidatedDA.GetDataSet(sqlCommand, security);
				}

				if ( (dataSetIdentity != null) && (dataSetIdentity.Tables.Count > 0) )
				{
					DataTable table = dataSetIdentity.Tables[0];

					if ( (table != null) && (table.Rows != null) && (table.Rows.Count > 0) )
					{
						DataRow row = table.Rows[0];
						Guid newGuid = row.IsNull(0) ? Guid.Empty : (Guid)row[0];

						if ( newGuid != Guid.Empty )
						{
							exportResultDetail.ExportResultGuid = newGuid;
						}
					}
				}
			}
			else
			{
				using (var sqlCommand = new SqlCommand())
				{
					exportResultDetail.ModifySql(sqlCommand);
					this.consolidatedDA.ExecuteQuery(security, sqlCommand);
				}
			}

			if ( exportResultDetail.IdentityGuid == Guid.Empty )
			{
				throw new Exception("Invalid index return from Insert.");
			}

			// Update transaction flags and status
			using (var sqlCommand = new SqlCommand())
			{
				exportResultDetail.UpdateTransactionFlagsAndStatus(sqlCommand);
				this.consolidatedDA.ExecuteQuery(security, sqlCommand);
			}

			return exportResultDetail.IdentityGuid;
		}

		/// <summary>
		/// This method will insert the export result detail record in the database based
		/// and return the new GUID.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="exportResultDetail">
		/// The export result detail.
		/// </param>
		/// <returns>
		/// The <see cref="Guid"/>.
		/// </returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, ExportResultDetailClass exportResultDetail)
		{
			return this.AddWithUserInfo(security, exportResultDetail, useSecurityUserInfo: true);
		}

		/// <summary>
		/// This method will insert the export result detail record in the database based
		/// and return the new GUID.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="exportResultDetail">
		/// The export result detail.
		/// </param>
		/// <param name="useSecurityUserInfo">
		/// The use security user info.
		/// </param>
		/// <returns>
		/// The <see cref="Guid"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">Null argument exception.
		/// </exception>
		/// <exception cref="Exception">Access denied exception.
		/// </exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid AddWithUserInfo(SecurityClass security, ExportResultDetailClass exportResultDetail, bool useSecurityUserInfo)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (exportResultDetail == null)
			{
				throw new ArgumentNullException("exportResultDetail");
			}

			if ( (security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT) == false) &&
				  (security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA) == false) &&
				  (security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) == false) )
			{
				throw new FMInsufficientRightsException();
			}

			exportResultDetail.CreatedDate	= DateTimeOffset.UtcNow;
			exportResultDetail.UpdatedDate	= exportResultDetail.CreatedDate;
			exportResultDetail.IdentityGuid = Guid.NewGuid();

			if ( useSecurityUserInfo )
			{
				exportResultDetail.CreatedBy = security.UserID;
				exportResultDetail.UpdatedBy = security.UserID;
			}

			DataSet dataSetIdentity;

			using (var sqlCommand = new SqlCommand())
			{
				exportResultDetail.InsertSql(sqlCommand);
				dataSetIdentity = this.consolidatedDA.GetDataSet(sqlCommand, security);
			}

			if ( (dataSetIdentity != null) && (dataSetIdentity.Tables.Count > 0) )
			{
				DataTable table = dataSetIdentity.Tables[0];

				if ( (table != null) && (table.Rows != null) && (table.Rows.Count > 0) )
				{
					DataRow row = table.Rows[0];
					Guid newGuid = row.IsNull(0) ? Guid.Empty : (Guid)row[0];

					if ( newGuid != Guid.Empty )
					{
						exportResultDetail.IdentityGuid = newGuid;
					}
				}
			}

			if ( exportResultDetail.IdentityGuid == Guid.Empty )
			{
				throw new Exception("Invalid index return from Insert.");
			}

			return exportResultDetail.IdentityGuid;
		}

		/// <summary>
		/// This method will update the export result detail record in the database based
		/// on the ExportResultDetail object's GUID field.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="exportResultDetail">
		/// The export result detail.
		/// </param>
		/// <exception cref="ArgumentNullException">Null argument exception.
		/// </exception>
		/// <exception cref="Exception">Access denied exception.
		/// </exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, ExportResultDetailClass exportResultDetail)
		{
			if ( security == null )
			{
				throw new ArgumentNullException("security");
			}

			if ( exportResultDetail == null )
			{
				throw new ArgumentNullException("exportResultDetail");
			}

			if ( (security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT) == false) &&
				  (security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA) == false) &&
				  (security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) == false) )
			{
				throw new FMInsufficientRightsException();
			}

			exportResultDetail.UpdatedDate	= DateTimeOffset.UtcNow;
			exportResultDetail.UpdatedBy	= security.UserID;

			using (var sqlCommand = new SqlCommand())
			{
				exportResultDetail.ModifySql(sqlCommand);
				this.consolidatedDA.ExecuteQuery(security, sqlCommand);
			}
		}

		/// <summary>
		/// This method will delete an export result detail record from the database based
		/// on the ExportResultDetail object's GUID field.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="exportResultDetailGuid">
		/// The export result detail GUID.
		/// </param>
		/// <exception cref="ArgumentNullException">Null argument exception.
		/// </exception>
		/// <exception cref="Exception">Access denied exception.
		/// </exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid exportResultDetailGuid)
		{
			if ( security == null )
			{
				throw new ArgumentNullException("security");
			}

			if ( (security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT) == false) &&
				  (security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA) == false) &&
				  (security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) == false) )
			{
				throw new FMInsufficientRightsException();
			}

			ExportResultDetailClass exportResultDetail = this.Get(security, exportResultDetailGuid);

			if ( exportResultDetail.IdentityGuid == Guid.Empty )
			{
				throw new Exception("ExportResultDetail Not Found.");
			}

			using (var sqlCommand = new SqlCommand())
			{
				exportResultDetail.PurgeSql(sqlCommand);
				this.consolidatedDA.ExecuteQuery(security, sqlCommand);
			}
		}


		/// <summary>
		/// This method will return an Export Result Detail object based on the
		/// GUID field.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="exportResultDetailGuid">
		/// The export result detail GUID.
		/// </param>
		/// <returns>
		/// The <see cref="ExportResultDetailClass"/>.
		/// </returns>
		public ExportResultDetailClass Get(SecurityClass security, Guid exportResultDetailGuid)
		{
			this.CheckSecurity(security);
            var exportResultDetail = new ExportResultDetailClass { IdentityGuid = exportResultDetailGuid };

			using (var sqlCommand = new SqlCommand())
			{
				exportResultDetail.GetByGuidSql(sqlCommand);
				DataSet dataSet = this.consolidatedDA.GetDataSet(sqlCommand, security);
				exportResultDetail.Load(dataSet);
			}

			return exportResultDetail;
		}

		/// <summary>
		/// This method will return a data set containing a history list for a given
		/// transaction.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="recordId">
		/// The record ID.
		/// </param>
		/// <param name="startDate">
		/// The start date.
		/// </param>
		/// <param name="endDate">
		/// The end date.
		/// </param>
		/// <param name="orderBy">
		/// The order by.
		/// </param>
		/// <returns>
		/// The <see cref="DataSet"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">Null argument exception.
		/// </exception>
		public DataSet GetTransHistoryByRecordId(
			SecurityClass security,
			string recordId,
			DateTime? startDate,
			DateTime? endDate,
			string orderBy)
		{
			if ( string.IsNullOrEmpty(recordId) )
			{
				throw new ArgumentNullException("recordId");
			}

			this.CheckSecurity(security);

            var exportResultDetail =  new ExportResultDetailClass { RecordId = recordId };

			DataSet dataSet;

			using (var sqlCommand = new SqlCommand())
			{
				exportResultDetail.GetTransHistoryByRecordId(sqlCommand, startDate, endDate, orderBy);

				dataSet = this.consolidatedDA.GetDataSet(sqlCommand, security);
			}

			return dataSet;
		}

		/// <summary>
		/// This method will return an Export Result Detail object based on the
		/// record ID field and the most current record using updated date.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="recordId">
		/// The record id.
		/// </param>
		/// <param name="transVersion">
		/// The trans version.
		/// </param>
		/// <returns>
		/// The <see cref="ExportResultDetailClass"/>.
		/// </returns>
		public ExportResultDetailClass GetByRecordIdAndTransVersion(SecurityClass security, string recordId, long transVersion)
		{
			this.CheckSecurity(security);

			List<string> interfaceNameList = this.GetInterfaceNames(security);
			var exportResultDetail =  new ExportResultDetailClass
                                                {
                                                    RecordId = recordId,
                                                    TransVersion = transVersion,
                                                    InterfaceNameList = interfaceNameList
                                                };

			using (var sqlCommand = new SqlCommand())
			{
				exportResultDetail.GetByRecordIdAndTransVersion(sqlCommand);

				DataSet dataSet = this.consolidatedDA.GetDataSet(sqlCommand, security);
				exportResultDetail.Load(dataSet);
			}

			return exportResultDetail;
		}

		/// <summary>
		/// This method will return an Export Result Detail object based on the
		/// record ID field and the most current record using updated date.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="recordId">
		/// The record id.
		/// </param>
		/// <returns>
		/// The <see cref="ExportResultDetailClass"/>.
		/// </returns>
		public ExportResultDetailClass GetByRecordIdAndCurrent(SecurityClass security, string recordId)
		{
			this.CheckSecurity(security);
            var exportResultDetail = new ExportResultDetailClass { RecordId = recordId };

			using (var sqlCommand = new SqlCommand())
			{
				exportResultDetail.GetByRecordIdAndMostCurrent(sqlCommand);

				DataSet dataSet = this.consolidatedDA.GetDataSet(sqlCommand,security);
				exportResultDetail.Load(dataSet);
			}

			return exportResultDetail;
		}

		/// <summary>
		/// This method will return an error summary of all the transactions that are in
		/// error along with the error text.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="startDate">
		/// The start date.
		/// </param>
		/// <param name="endDate">
		/// The end date.
		/// </param>
		/// <param name="siteList">
		/// The site list.
		/// </param>
		/// <param name="orderBy">
		/// The order by.
		/// </param>
		/// <returns>
		/// The <see cref="DataSet"/>.
		/// </returns>
		public DataSet GetErrorTransactionsAndTexts(
			SecurityClass security,
			DateTime? startDate,
			DateTime? endDate,
			List<Guid> siteList,
			string orderBy)
		{
			this.CheckSecurity(security);

			DataSet errorTransDataSet;
            var exportResultDetail =  new ExportResultDetailClass( );

			using (var sqlCommand = new SqlCommand())
			{
				exportResultDetail.GetErrorTransactions(sqlCommand, null, startDate, endDate, siteList, orderBy, security.UserGuid);
				errorTransDataSet = this.consolidatedDA.GetDataSet(sqlCommand, security);
			}

			return errorTransDataSet;
		}

	    /// <summary>
	    /// This method will return an error summary of all the transactions that are in
	    /// error along with the error text for a single interface.
	    /// </summary>
	    /// <param name="security">
	    /// The security.
	    /// </param>
	    /// <param name="interfaceName">
	    /// The interface name
	    /// </param>
	    /// <param name="startDate">
	    /// The start date.
	    /// </param>
	    /// <param name="endDate">
	    /// The end date.
	    /// </param>
	    /// <param name="siteList">
	    /// The site list.
	    /// </param>
	    /// <param name="orderBy">
	    /// The order by.
	    /// </param>
	    /// <returns>
	    /// The <see cref="DataSet"/>.
	    /// </returns>
	    public DataSet GetErrorTransactionsAndTextsByInterface(
	        SecurityClass security,
	        string interfaceName,
	        DateTime? startDate,
	        DateTime? endDate,
	        List<Guid> siteList,
	        string orderBy)
	    {
            this.CheckSecurity(security);

            DataSet errorTransDataSet;
            var exportResultDetail =  new ExportResultDetailClass();

            using (var sqlCommand = new SqlCommand())
            {
                exportResultDetail.GetErrorTransactions(sqlCommand, interfaceName, startDate, endDate, siteList, orderBy, security.UserGuid);
                errorTransDataSet = this.consolidatedDA.GetDataSet(sqlCommand, security);
            }

            return errorTransDataSet;
	    }

	    /// <summary>
		/// This method will return the result detail information GUID based on the Record ID and
		/// TransVersion combination. It will return null if not found.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="resultDetail">
		/// The result detail.
		/// </param>
		/// <returns>
		/// The <see cref="Guid"/>.
		/// </returns>
		/// <exception cref="ArgumentException">Null argument exception.
		/// </exception>
		public Guid GetGuidByRecordIdAndTransVersion(SecurityClass security, ExportResultDetailClass resultDetail)
		{
			Guid resultDetailGuid = Guid.Empty;

			this.CheckSecurity(security);

			if ( resultDetail == null )
			{
				throw new ArgumentException("resultDetail");
			}

			DataSet resultDetailDataSet;

			using (var sqlCommand = new SqlCommand())
			{
				resultDetail.GetGuidByRecordIdAndTransVersionSql(sqlCommand);
				resultDetailDataSet = this.consolidatedDA.GetDataSet(sqlCommand, security);
			}

			if ( (resultDetailDataSet != null) && (resultDetailDataSet.Tables.Count > 0) )
			{
				DataTable table = resultDetailDataSet.Tables[0];

				if ( (table != null) && (table.Rows != null) && (table.Rows.Count > 0) )
				{
					DataRow row = table.Rows[0];
					resultDetailGuid = row.IsNull("ExportResultDetailGuid") ? Guid.Empty : (Guid) row["ExportResultDetailGuid"];
				}
			}

			return resultDetailGuid;
		}

		/// <summary>
		/// This method retrieves the unacknowledged transactions.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="startDate">The transaction start date.</param>
		/// <param name="endDate">The transaction end date.</param>
		/// <param name="siteList">The list of sites.</param>
		/// <param name="orderBy">Order by column.</param>
		/// <returns>Returns a data set containing the unacknowledged transactions</returns>
		public DataSet GetUnacknowledgedTransactions(SecurityClass security,
													DateTime? startDate,
													DateTime? endDate,
													List<Guid> siteList,
													string orderBy)
		{
			this.CheckSecurity(security);

            var exportResultDetail =  new ExportResultDetailClass();

			var sqlCommand = new SqlCommand();
			exportResultDetail.GetUnacknowledgedTransactions(sqlCommand, startDate, endDate, siteList, orderBy, security.UserGuid);

			DataSet unacknowledgedTransDataSet = this.consolidatedDA.GetDataSet( sqlCommand, security);

			return unacknowledgedTransDataSet;
		}

		/// <summary>
		/// This method will return a collection of Export Result Detail classes base on the
		/// site GUID.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <returns>
		/// The <see cref="ExportResultDetailCollectionClass"/>.
		/// </returns>
		public ExportResultDetailCollectionClass Enumerate(SecurityClass security)
		{
			this.CheckSecurity(security);

			var exportResultDetailCollection = new ExportResultDetailCollectionClass( );
            var exportResultDetail =  new ExportResultDetailClass( );
			DataSet dataSet;

			using (var sqlCommand = new SqlCommand())
			{
				exportResultDetail.EnumerateSql(sqlCommand, security.SiteGuid);
				dataSet = this.consolidatedDA.GetDataSet(sqlCommand, security);
			}

			DataTable table = dataSet.Tables[0];

			while ( table.Rows.Count != 0 )
			{
                exportResultDetail =  new ExportResultDetailClass( );
				exportResultDetail.Load(dataSet);
				exportResultDetailCollection.Add(exportResultDetail);
				table.Rows.RemoveAt(0);
			}

			return exportResultDetailCollection;
		}

		/// <summary>
		/// This method will retreive the list of external export results interface names
		/// in order to retrieve the actual export results record.
		/// </summary>
		/// <param name="security">The FuelsManager serurity object.</param>
		/// <returns>Returns a collection of external export results interface names.</returns>
		public List<string> GetInterfaceNames(SecurityClass security)
		{
			var interfaceNameList = new List<string>();
			var configSettings = new ConfigurationSettingsClass();

			string interfaceNames = configSettings.GetKeyValueByKey(security, ConfigurationSettingDOClass.Key_External_ExportResults_InterfaceNames);

			string[] interfaceNameStrings = interfaceNames.Split(';');

			if (interfaceNameStrings.Length > 0)
			{
				foreach (string name in interfaceNameStrings)
				{
					if (string.IsNullOrEmpty(name) == false)
					{
						interfaceNameList.Add(name);
					}
				}
			}

			return interfaceNameList;
		}

		/// <summary>
		/// This method ensures the security is correct.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <exception cref="ArgumentNullException">Null argument exception.
		/// </exception>
		/// <exception cref="Exception">Access denied exception.
		/// </exception>
		private void CheckSecurity(SecurityClass security)
		{
			if ( security == null )
			{
				throw new ArgumentNullException("security");
			}

			if ( (security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT) == false) &&
				  (security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA) == false) &&
				  (security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) == false) &&
				  (security.HasRight(RIGHT.VIEW_TRANSACTION_DATA) == false) &&
				  (security.HasRight(RIGHT.IMPORT_TRANSACTION) == false))
			{
				throw new FMInsufficientRightsException();
			}
		}
	}
}
