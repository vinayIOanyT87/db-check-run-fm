// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GasboyStationFailedTransactionErrorDBI.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//  Implements methods for the Gasboy Station Failed transaction error functionality that interact with the database 
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Afss.Module.Gasboy.BusinessServices.Repository
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;

	using FMBusinessObjects.DataObjects;

	using FMBusinessServices.DataAccessLayer;

	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects;

	using Microsoft.SqlServer.Server;

	/// <summary>
	/// Implements methods for the Gasboy Station Failed transaction error functionality that interact with the database 
	/// </summary>
	public static class GasboyStationTransactionErrorDBI
	{
		/// <summary>
		/// Provides database access
		/// </summary>
		private static readonly ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		/// <summary>
		/// Search the database for failed transaction error records associated with the provided guid identifying a failed transaction
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		/// <param name="externalStationTransactionGuid">The station to get failed transaction for, or Guid.Empty for all stations</param>
		/// <returns>External station failed transactions for the site matching the provided search parameters</returns>
		public static List<GasboyStationTransactionError> GetList(SecurityClass security, Guid externalStationTransactionGuid)
		{
			List<GasboyStationTransactionError> failedTransactionErrors = new List<GasboyStationTransactionError>();

			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "usp_ExternalStationTransactionErrorEnumerate";

				cmd.Parameters.Add("@ExternalStationTransactionGuid", SqlDbType.UniqueIdentifier).Value = externalStationTransactionGuid;

				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

				if (set.Tables.Count == 0 || set.Tables[0].Rows.Count == 0)
				{
					return failedTransactionErrors;
				}

				foreach (DataRow row in set.Tables[0].Rows)
				{
					GasboyStationTransactionError error = LoadObjectFromDataRow(row);
					failedTransactionErrors.Add(error);
				}
			}

			return failedTransactionErrors;
		}

		/// <summary>
		/// Add failed transaction errors to the database
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="errors">Failed transactions errors to save to the database</param>
		public static void Insert(SecurityClass security, List<GasboyStationTransactionError> errors)
		{
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "usp_ExternalStationTransactionErrorInsert";

				SqlParameter tableValuedParameter = cmd.Parameters.Add("@ExternalStationTransactionErrors", SqlDbType.Structured);
				tableValuedParameter.Value = (errors.Count > 0) ? CreateSqlDataRecords(errors) : (object)DBNull.Value;
				tableValuedParameter.TypeName = "dbo.ExternalStationTransactionErrorType";
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// Clear transaction errors of a given External Station Transaction from the database
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="externalStationTransactionGuid">The Guid of the external station transaction from which to clear the errors from. </param>
		public static void Clear(SecurityClass security, Guid externalStationTransactionGuid)
		{
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "usp_ExternalStationTransactionErrorClear";

				cmd.Parameters.Add("@ExternalStationTransactionGuid", SqlDbType.UniqueIdentifier).Value = externalStationTransactionGuid;
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// Create the SqlDataRecords corresponding to the table valued parameter we pass to the insert stored procedure
		/// </summary>
		/// <param name="errors">The failed transaction errors to create SqlDataRecords for</param>
		/// <returns>SqlDataRecords corresponding to the table valued parameter we pass to the insert stored procedure</returns>
		private static IEnumerable<SqlDataRecord> CreateSqlDataRecords(IEnumerable<GasboyStationTransactionError> errors)
		{
			SqlMetaData[] metaData = new SqlMetaData[4];

			int i = 0;
			metaData[i++] = new SqlMetaData("ExternalStationTransactionErrorGuid", SqlDbType.UniqueIdentifier);
			metaData[i++] = new SqlMetaData("ExternalStationTransactionGuid", SqlDbType.UniqueIdentifier);
			metaData[i++] = new SqlMetaData("Error", SqlDbType.NVarChar, 1000);
			metaData[i] = new SqlMetaData("CreatedUpdatedBy", SqlDbType.NVarChar, 100);

			SqlDataRecord record = new SqlDataRecord(metaData);

			foreach (GasboyStationTransactionError error in errors)
			{
				int j = 0;

				record.SetGuid(j++, error.IdentityGuid);
				record.SetGuid(j++, error.ExternalStationTransactionGuid);
				record.SetString(j++, error.ErrorMessage);
				record.SetString(j, error.CreatedBy);

				yield return record;
			}
		}

		/// <summary>
		/// Load a gasboy failed transaction error record from a dataRow read from the database
		/// </summary>
		/// <param name="row">The dataRow to read failed transaction error information from</param>
		/// <returns>A populated GasboyStationFailedTransactionError object</returns>
		private static GasboyStationTransactionError LoadObjectFromDataRow(DataRow row)
		{
			GasboyStationTransactionError transactionError = new GasboyStationTransactionError();

			transactionError.IdentityGuid = DataObject.getValue(row["ExternalStationTransactionErrorGuid"], Guid.Empty);
			transactionError.ExternalStationTransactionGuid = DataObject.getValue(row["ExternalStationTransactionGuid"], Guid.Empty);
			transactionError.ErrorMessage = DataObject.getValue(row["Error"], string.Empty);
			transactionError.CreatedDate = DataObject.getValue(row["CreatedDate"], DateTimeOffset.Now);
			transactionError.CreatedBy = DataObject.getValue(row["CreatedBy"], BaseDataObject.ADMIN);
			transactionError.UpdatedDate = DataObject.getValue(row["UpdatedDate"], DateTimeOffset.Now);
			transactionError.UpdatedBy = DataObject.getValue(row["UpdatedBy"], BaseDataObject.ADMIN);

			return transactionError;
		}
	}
}