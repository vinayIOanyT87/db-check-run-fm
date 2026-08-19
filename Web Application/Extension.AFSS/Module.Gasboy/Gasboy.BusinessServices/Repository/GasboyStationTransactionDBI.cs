// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GasboyStationTransactionDBI.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//  Implements methods for the Gasboy Station Failed transaction functionality that interact with the database 
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Afss.Module.Gasboy.BusinessServices.Repository
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Linq;

	using FMBusinessObjects.DataObjects;

	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;

	using FuelsManager.Afss.BusinessObjects.Constants;
	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects;

	using Microsoft.SqlServer.Server;

	/// <summary>
	/// Implements methods for the Gasboy Station Failed transaction functionality that interact with the database 
	/// </summary>
	public class GasboyStationTransactionDBI : BaseDBI
	{
		/// <summary>
		/// Provides database access
		/// </summary>
		private static readonly ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		public GasboyStationTransactionDBI(string user, DateTimeOffset saveTime)
			: base(user, saveTime)
		{
		}

		/// <summary>
		/// Get a failed transaction record identified by the provided guid from the database
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="externalStationTransactionGuid">Identifies the failed transaction record to retrieve</param>
		/// <param name="includeErrorDetails">Indicates whether any related error records should be included.</param>
		/// <returns>The failed transaction record identified by the provided guid from the database, or null if it was not found</returns>
		public static GasboyStationTransaction Get(SecurityClass security, Guid externalStationTransactionGuid, bool includeErrorDetails)
		{
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "usp_ExternalStationTransactionGet";

				cmd.Parameters.Add("@ExternalStationTransactionGuid", SqlDbType.UniqueIdentifier).Value = externalStationTransactionGuid;

				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

				if (set.Tables.Count == 0 || set.Tables[0].Rows.Count == 0)
				{
					return null;
				}

				var transaction = LoadObjectFromDataRow(set.Tables[0].Rows[0]);

				if (includeErrorDetails)
				{
					transaction.TransactionErrors = GasboyStationTransactionErrorDBI.GetList(security, externalStationTransactionGuid);
				}

				return transaction;
			}
		}

		/// <summary>
		/// Search the database for failed transaction records identified by the provided parameters
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		/// <param name="siteGuid">The site to retrieve failed transaction records for</param>
		/// <param name="externalStationGuid">The station to get failed transaction for, or Guid.Empty for all stations</param>
		/// <param name="beginDate">The beginning date to get failed transaction for</param>
		/// <param name="endDate">The end date to get failed transaction for</param>
		/// <param name="transactionID">If specified, failed transactions with this id or beginning with this id will be returned</param>
		/// <returns>External station failed transactions for the site matching the provided search parameters</returns>
		public static List<GasboyStationTransaction> GetList(
			SecurityClass security,
			Guid siteGuid,
			Guid externalStationGuid,
			DateTimeOffset? beginDate,
			DateTimeOffset? endDate,
			string transactionID)
		{
			var transactions = new List<GasboyStationTransaction>();

			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "usp_ExternalStationTransactionEnumerate";

				cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = siteGuid;

				if (externalStationGuid != Guid.Empty)
				{
					cmd.Parameters.Add("@ExternalStationGuid", SqlDbType.UniqueIdentifier).Value = externalStationGuid;
				}

				cmd.Parameters.Add("@BeginDate", SqlDbType.DateTimeOffset).Value = SetOptionalValue<DateTimeOffset>(beginDate);
				cmd.Parameters.Add("@EndDate", SqlDbType.DateTimeOffset).Value = SetOptionalValue<DateTimeOffset>(endDate);

				if (!string.IsNullOrEmpty(transactionID))
				{
					cmd.Parameters.Add("@TransactionID", SqlDbType.NVarChar, 20).Value = transactionID;
				}

				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

				if (set.Tables.Count == 0 || set.Tables[0].Rows.Count == 0)
				{
					return transactions;
				}

				foreach (DataRow row in set.Tables[0].Rows)
				{
					transactions.Add(LoadObjectFromDataRow(row));
				}
			}

			return transactions;
		}

		/// <summary>
		/// Search the database for failed transaction records identified by the provided parameters
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		/// <param name="siteGuid">The site to retrieve failed transaction records for</param>
		/// <param name="externalStationGuid">The station to get failed transaction for, or Guid.Empty for all stations</param>
		/// <param name="beginDate">The beginning date to get failed transaction for</param>
		/// <param name="endDate">The end date to get failed transaction for</param>
		/// <param name="transactionID">If specified, failed transactions with this id or beginning with this id will be returned</param>
		/// <returns>External station failed transactions for the site matching the provided search parameters</returns>
		public static List<GasboyStationTransaction> GetFailedList(
			SecurityClass security,
			Guid siteGuid,
			Guid externalStationGuid,
			DateTimeOffset? beginDate,
			DateTimeOffset? endDate,
			string transactionID)
		{
			var failedTransactions = new List<GasboyStationTransaction>();

			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "usp_ExternalStationTransactionEnumerateFailed";

				cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = siteGuid;

				if (externalStationGuid != Guid.Empty)
				{
					cmd.Parameters.Add("@ExternalStationGuid", SqlDbType.UniqueIdentifier).Value = externalStationGuid;
				}

				cmd.Parameters.Add("@BeginDate", SqlDbType.DateTimeOffset).Value = SetOptionalValue<DateTimeOffset>(beginDate);
				cmd.Parameters.Add("@EndDate", SqlDbType.DateTimeOffset).Value = SetOptionalValue<DateTimeOffset>(endDate);

				if (!string.IsNullOrEmpty(transactionID))
				{
					cmd.Parameters.Add("@TransactionID", SqlDbType.NVarChar, 20).Value = transactionID;
				}

				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

				if (set.Tables.Count == 0 || set.Tables[0].Rows.Count == 0)
				{
					return failedTransactions;
				}

				foreach (DataRow row in set.Tables[0].Rows)
				{
					failedTransactions.Add(LoadObjectFromDataRow(row));
				}
			}
			
			return failedTransactions;
		}

		/// <summary>
		/// Get a gasboy transaction record using the external transaction id
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="externalStationTransactionID">Identifies the transaction record to retrieve</param>
		/// <returns>The failed transaction record identified by the provided guid from the database, or null if it was not found</returns>
		public static GasboyStationTransaction GetByTransactionID(SecurityClass security, string externalStationTransactionID)
		{
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "usp_ExternalStationTransactionGetByTransactionID";

				cmd.Parameters.Add("@ExternalStationTransactionID", SqlDbType.NVarChar, 20).Value = externalStationTransactionID;

				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

				if (set.Tables.Count == 0 || set.Tables[0].Rows.Count == 0)
				{
					return null;
				}

				var transaction = LoadObjectFromDataRow(set.Tables[0].Rows[0]);

				return transaction;
			}
		}

		/// <summary>
		/// Add failed transactions to the database
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		/// <param name="transactions">Failed transactions to save to the database</param>
		public static void Insert(SecurityClass security, List<GasboyStationTransaction> transactions)
		{
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "usp_ExternalStationTransactionInsert";

				SqlParameter tableValuedParameter = cmd.Parameters.Add("@ExternalStationTransactions", SqlDbType.Structured);
				tableValuedParameter.Value = CreateSqlDataRecords(transactions);
				tableValuedParameter.TypeName = "dbo.ExternalStationTransactionType";
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// Delete a failed transaction record identified by the provided guid from the database
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="externalStationTransactionGuid">Identifies the failed transaction record to delete</param>
		public static void Purge(SecurityClass security, Guid externalStationTransactionGuid)
		{
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "usp_ExternalStationTransactionDelete";

				cmd.Parameters.Add("@ExternalStationTransactionGuid", SqlDbType.UniqueIdentifier).Value = externalStationTransactionGuid;
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// Modify an external station failed transaction with an updated status
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		/// <param name="transactions"></param>
		public static void ModifyFailedStatus(SecurityClass security, List<GasboyStationTransaction> transactions)
		{
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_ExternalStationTransactionFailedStatusUpdate";

				SqlParameter tableValuedParameter = cmd.Parameters.Add("@ExternalStationTransactionFailedStatus", SqlDbType.Structured);
				tableValuedParameter.Value = CreateSqlDataRecordsForFailedStatusUpdate(transactions);
				tableValuedParameter.TypeName = "dbo.ExternalStationTransactionFailedStatusType";

				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// Modify an external station transaction with an updated status
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		/// <param name="localTransaction">The Transaction to Update</param>
		public static void ModifyStatus(SecurityClass security, GasboyStationTransaction localTransaction)
		{
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_ExternalStationTransactionStatusUpdate";

				SqlParameter guidParameter = cmd.Parameters.Add("@ExternalStationTransactionGuid", SqlDbType.UniqueIdentifier);
				guidParameter.Value = localTransaction.IdentityGuid;
				SqlParameter intParameter = cmd.Parameters.Add("@ExternalStationTransactionStatus", SqlDbType.Int);
				intParameter.Value = (int)localTransaction.ExternalStationTransactionStatus;

				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// Modify an external station failed transaction with an updated status
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		/// <param name="externalStation"></param>
		/// <param name="transactions"></param>
		public static List<GasboyStationTransaction> GetDuplicateTransactions(SecurityClass security, GasboyStation externalStation, List<GasboyStationTransaction> transactions)
		{
			var duplicateTransactions = new List<GasboyStationTransaction>();

			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_ExternalStationDuplicateTransactionGet";

				SqlParameter stationParameter = cmd.Parameters.Add("@ExternalStationGuid", SqlDbType.UniqueIdentifier);
				stationParameter.Value = externalStation.IdentityGuid;


				SqlParameter tableValuedParameter = cmd.Parameters.Add("@DuplicateExternalStationTransactionIDs", SqlDbType.Structured);
				tableValuedParameter.Value = CreateSqlDataRecordsForDuplicateTransactions(transactions);
				tableValuedParameter.TypeName = "dbo.FindDuplicateExternalStationTransactionType";

				ConsolidatedDA.ExecuteQuery(security, cmd);

				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

				if (set.Tables.Count > 0)
				{
					foreach (DataRow row in set.Tables[0].Rows)
					{
						var transaction = new GasboyStationTransaction
										  {
											  IdentityGuid =
												  DataObject.getValue(
													  row["ExternalStationTransactionGuid"],
													  Guid.Empty),
											  ExternalStationGuid =
												  DataObject.getValue(
													  row["ExternalStationGuid"],
													  Guid.Empty),
											  SiteGuid =
												  DataObject.getValue(row["SiteGuid"], Guid.Empty),
											  ExternalStationID =
												  DataObject.getValue(
													  row["ExternalStationID"],
													  string.Empty),
											  ID =
												  DataObject.getValue(
													  row["StationTransactionID"],
													  string.Empty),
											  CreatedDate =
												  DataObject.getValue(
													  row["CreatedDate"],
													  DateTimeOffset.Now),
											  ExternalStationTransactionStatus =
												  DataObject.getValue(
													  row["LookupExternalStationTransactionStatusIndex"],
													  ExternalStationTransactionStatus.None),
											  ExternalStationTransactionFailedStatus =
												  DataObject.getValue(
													  row["LookupExternalStationTransactionFailedStatusIndex"
												  ],
													  ExternalStationTransactionFailedStatus.None)
										  };

						duplicateTransactions.Add(transaction);
					}
				}
			}

			return duplicateTransactions;
		}

		/// <summary>
		/// Create the SqlDataRecords corresponding to the table valued parameter we pass to the insert stored procedure
		/// </summary>
		/// <param name="transactions">The transactions to create SqlDataRecords for</param>
		/// <returns>SqlDataRecords corresponding to the table valued parameter we pass to the insert stored procedure</returns>
		private static IEnumerable<SqlDataRecord> CreateSqlDataRecords(IEnumerable<GasboyStationTransaction> transactions)
		{
			SqlMetaData[] metaData = new SqlMetaData[8];

			int i = 0;
			metaData[i++] = new SqlMetaData("ExternalStationTransactionGuid", SqlDbType.UniqueIdentifier);
			metaData[i++] = new SqlMetaData("ExternalStationGuid", SqlDbType.UniqueIdentifier);
			metaData[i++] = new SqlMetaData("SiteGuid", SqlDbType.UniqueIdentifier);
			metaData[i++] = new SqlMetaData("StationTransactionID", SqlDbType.NVarChar, 20);
			metaData[i++] = new SqlMetaData("RawTransactionData", SqlDbType.NVarChar, -1);
			metaData[i++] = new SqlMetaData("CreatedUpdatedBy", SqlDbType.NVarChar, 100);
			metaData[i++] = new SqlMetaData("LookupExternalStationTransactionStatusIndex", SqlDbType.Int);
			metaData[i] = new SqlMetaData("LookupExternalStationTransactionFailedStatusIndex", SqlDbType.Int);

			SqlDataRecord record = new SqlDataRecord(metaData);

			foreach (GasboyStationTransaction transaction in transactions)
			{
				int j = 0;

				record.SetGuid(j++, transaction.IdentityGuid);
				record.SetGuid(j++, transaction.ExternalStationGuid);
				record.SetGuid(j++, transaction.SiteGuid);
				record.SetString(j++, transaction.ID);
				record.SetString(j++, transaction.RawTransactionData);
				record.SetString(j++, transaction.CreatedBy);
				record.SetInt32(j++, (int)transaction.ExternalStationTransactionStatus);
				record.SetInt32(j, (int)transaction.ExternalStationTransactionFailedStatus);

				yield return record;
			}
		}

		/// <summary>
		/// Create the SqlDataRecords corresponding to the table valued parameter we pass to the insert stored procedure
		/// </summary>
		/// <param name="failedTransactions">The failed transactions to create SqlDataRecords for</param>
		/// <returns>SqlDataRecords corresponding to the table valued parameter we pass to the insert stored procedure</returns>
		private static IEnumerable<SqlDataRecord> CreateSqlDataRecordsForFailedStatusUpdate(IEnumerable<GasboyStationTransaction> failedTransactions)
		{
			SqlMetaData[] metaData = new SqlMetaData[3];

			int i = 0;
			metaData[i++] = new SqlMetaData("ExternalStationTransactionGuid", SqlDbType.UniqueIdentifier);
			metaData[i++] = new SqlMetaData("CreatedUpdatedBy", SqlDbType.NVarChar, 100);
			metaData[i] = new SqlMetaData("LookupExternalStationTransactionFailedStatusIndex", SqlDbType.Int);


			SqlDataRecord record = new SqlDataRecord(metaData);

			foreach (GasboyStationTransaction failedTransaction in failedTransactions)
			{
				int j = 0;

				record.SetGuid(j++, failedTransaction.IdentityGuid);
				record.SetString(j++, failedTransaction.CreatedBy);
				record.SetInt32(j, (int)failedTransaction.ExternalStationTransactionFailedStatus);


				yield return record;
			}
		}

		/// <summary>
		/// Create the SqlDataRecords corresponding to the table valued parameter we pass to the insert stored procedure
		/// </summary>
		/// <param name="transactions">The transactions to create SqlDataRecords for</param>
		/// <returns>SqlDataRecords corresponding to the table valued parameter we pass to the insert stored procedure</returns>
		private static IEnumerable<SqlDataRecord> CreateSqlDataRecordsForDuplicateTransactions(IEnumerable<GasboyStationTransaction> transactions)
		{
			SqlMetaData[] metaData = new SqlMetaData[1];

			int i = 0;
			metaData[i] = new SqlMetaData("StationTransactionID", SqlDbType.NVarChar, 20);

			SqlDataRecord record = new SqlDataRecord(metaData);

			foreach (GasboyStationTransaction transaction in transactions)
			{
				int j = 0;

				record.SetString(j, transaction.ID);

				yield return record;
			}
		}

		/// <summary>
		/// Load a gasboy failed transaction record from a dataRow read from the database
		/// </summary>
		/// <param name="row">The dataRow to read failed transaction information from</param>
		/// <returns>A populated GasboyStationTransaction object</returns>
		private static GasboyStationTransaction LoadObjectFromDataRow(DataRow row)
		{
			GasboyStationTransaction transaction = new GasboyStationTransaction();

			transaction.IdentityGuid = DataObject.getValue(row["ExternalStationTransactionGuid"], Guid.Empty);
			transaction.SiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty);
			transaction.ID = DataObject.getValue(row["StationTransactionID"], string.Empty);
			transaction.ExternalStationGuid = DataObject.getValue(row["ExternalStationGuid"], Guid.Empty);
			transaction.ExternalStationID = DataObject.getValue(row["ExternalStationID"], string.Empty);
			transaction.RawTransactionData = DataObject.getValue(row["RawTransactionData"], string.Empty);
			transaction.CreatedDate = DataObject.getValue(row["CreatedDate"], DateTimeOffset.Now);
			transaction.CreatedBy = DataObject.getValue(row["CreatedBy"], BaseDataObject.ADMIN);
			transaction.UpdatedDate = DataObject.getValue(row["UpdatedDate"], DateTimeOffset.Now);
			transaction.UpdatedBy = DataObject.getValue(row["UpdatedBy"], BaseDataObject.ADMIN);
			transaction.ExternalStationTransactionStatus = DataObject.getValue(row["LookupExternalStationTransactionStatusIndex"], ExternalStationTransactionStatus.None);
			transaction.ExternalStationTransactionFailedStatus = DataObject.getValue(row["LookupExternalStationTransactionFailedStatusIndex"], ExternalStationTransactionFailedStatus.None);

			return transaction;
		}

		protected override void PrepareDeleteRemainingStatement()
		{
			return;
		}

		protected override void PrepareDeleteStatement()
		{
			return;
		}

		protected override void PrepareInsertStatement()
		{
			return;
		}

		protected override void PrepareSelectStatement()
		{
			return;
		}


	}
}