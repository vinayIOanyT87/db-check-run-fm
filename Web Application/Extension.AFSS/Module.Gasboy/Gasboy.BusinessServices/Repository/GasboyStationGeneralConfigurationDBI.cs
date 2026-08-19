// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GasboyStationGeneralConfigurationDBI.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//  Implements methods for the Gasboy Station General configuration functionality that interact with the database 
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

	/// <summary>
	/// Implements methods for the Gasboy Station General configuration functionality that interact with the database 
	/// </summary>
	public class GasboyStationGeneralConfigurationDBI
	{
		/// <summary>
		/// Provides database access
		/// </summary>
		private static readonly ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		/// <summary>
		/// Get a Gasboy Station General configuration record identified by the provided guid from the database
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="externalStationGeneralConfigurationGuid">Identifies the Gasboy Station General configuration record to retrieve</param>
		/// <returns>The Gasboy Station General configuration identified by the provided guid from the database, or null if it was not found</returns>
		public static GasboyStationGeneralConfiguration Get(SecurityClass security, Guid externalStationGeneralConfigurationGuid)
		{
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "usp_ExternalStationGeneralConfigurationGet";

				cmd.Parameters.Add("@ExternalStationGeneralConfigurationGuid", SqlDbType.UniqueIdentifier).Value = externalStationGeneralConfigurationGuid;

				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

				if (set.Tables.Count == 0 || set.Tables[0].Rows.Count == 0)
				{
					return null;
				}

				return LoadObjectFromDataRow(set.Tables[0].Rows[0]);
			}
		}

		/// <summary>
		/// Get a Gasboy Station General configuration record identified by the provided site guid from the database.
		/// There should only be one general configuration record for a site
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="siteGuid">Identifies the site to retrieve the Gasboy Station General configuration record for</param>
		/// <returns>The Gasboy Station General configuration record identified by the provided site guid from the database, or null if it was not found</returns>
		public static GasboyStationGeneralConfiguration GetBySiteGuid(SecurityClass security, Guid siteGuid)
		{
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "usp_ExternalStationGeneralConfigurationGet";

				cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = siteGuid;

				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

				if (set.Tables.Count == 0 || set.Tables[0].Rows.Count == 0)
				{
					return null;
				}

				return LoadObjectFromDataRow(set.Tables[0].Rows[0]);
			}
		}

		/// <summary>
		/// Search the database for all Gasboy Station General configuration records 
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		/// <returns>All configured Gasboy Station General configuration records</returns>
		public static List<GasboyStationGeneralConfiguration> GetList(SecurityClass security)
		{
			List<GasboyStationGeneralConfiguration> generalConfigurations = new List<GasboyStationGeneralConfiguration>();

			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "usp_ExternalStationGeneralConfigurationEnumerate";

				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

				if (set.Tables.Count == 0 || set.Tables[0].Rows.Count == 0)
				{
					return generalConfigurations;
				}

				foreach (DataRow row in set.Tables[0].Rows)
				{
					GasboyStationGeneralConfiguration failedTransaction = new GasboyStationGeneralConfiguration
					{
						IdentityGuid = DataObject.getValue(row["ExternalStationGeneralConfigurationGuid"], Guid.Empty),                     
						SiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty),
						RetailSaleTransactionAliasGuid = DataObject.getValue(row["RetailSaleTransactionAliasGuid"], Guid.Empty),
						RetailSaleTransactionAliasName = DataObject.getValue(row["RetailSaleTransactionAliasName"], string.Empty),
						DownloadTransactionsIntervalMinutes = DataObject.getOptionalInt(row["DownloadTransactionsIntervalMinutes"]),
						DownloadEventsIntervalMinutes = DataObject.getOptionalInt(row["DownloadEventsIntervalMinutes"]),
					};

					generalConfigurations.Add(failedTransaction);
				}
			}

			return generalConfigurations;
		}

		/// <summary>
		/// Add a Gasboy Station General configuration record to the database
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		/// <param name="generalConfiguration">A Gasboy Station General configuration record to save to the database</param>
		public static void Insert(SecurityClass security, GasboyStationGeneralConfiguration generalConfiguration)
		{
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "usp_ExternalStationGeneralConfigurationInsert";

				cmd.Parameters.Add("@ExternalStationGeneralConfigurationGuid", SqlDbType.UniqueIdentifier).Value = generalConfiguration.IdentityGuid;
				cmd.Parameters.Add("@CreatedUpdatedBy", SqlDbType.NVarChar, 100).Value = generalConfiguration.CreatedBy;

				AddCommonInsertUpdateParameters(cmd, generalConfiguration);

				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// Modify a Gasboy Station General configuration record in the database
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		/// <param name="generalConfiguration">A Gasboy Station General configuration record to modify in the database</param>
		public static void Update(SecurityClass security, GasboyStationGeneralConfiguration generalConfiguration)
		{
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "usp_ExternalStationGeneralConfigurationUpdate";

				cmd.Parameters.Add("@ExternalStationGeneralConfigurationGuid", SqlDbType.UniqueIdentifier).Value = generalConfiguration.IdentityGuid;
				cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100).Value = generalConfiguration.UpdatedBy;

				AddCommonInsertUpdateParameters(cmd, generalConfiguration);

				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// Delete a Gasboy Station General configuration record in the database
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="externalStationGeneralConfigurationGuid">Identifies the general configuration record to delete</param>
		public static void Purge(SecurityClass security, Guid externalStationGeneralConfigurationGuid)
		{
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "usp_ExternalStationGeneralConfigurationDelete";

				cmd.Parameters.Add("@ExternalStationGeneralConfigurationGuid", SqlDbType.UniqueIdentifier).Value = externalStationGeneralConfigurationGuid;
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// Load a gasboy general configuration record from a dataRow read from the database
		/// </summary>
		/// <param name="row">The dataRow to read general configuration information from</param>
		/// <returns>A populated GasboyStationGeneralConfiguration object</returns>
		private static GasboyStationGeneralConfiguration LoadObjectFromDataRow(DataRow row)
		{
			GasboyStationGeneralConfiguration generalConfiguration = new GasboyStationGeneralConfiguration();

			generalConfiguration.IdentityGuid = DataObject.getValue(row["ExternalStationGeneralConfigurationGuid"], Guid.Empty);
			generalConfiguration.SiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty);
			generalConfiguration.RetailSaleTransactionAliasGuid = DataObject.getValue<Guid?>(row["RetailSaleTransactionAliasGuid"], null);
			generalConfiguration.RetailSaleTransactionAliasName = DataObject.getValue(row["RetailSaleTransactionAliasName"], string.Empty);
			generalConfiguration.DownloadTransactionsIntervalMinutes = DataObject.getOptionalInt(row["DownloadTransactionsIntervalMinutes"]);
			generalConfiguration.DownloadEventsIntervalMinutes = DataObject.getOptionalInt(row["DownloadEventsIntervalMinutes"]);
			generalConfiguration.CreatedDate = DataObject.getValue(row["CreatedDate"], DateTimeOffset.Now);
			generalConfiguration.CreatedBy = DataObject.getValue(row["CreatedBy"], BaseDataObject.ADMIN);
			generalConfiguration.UpdatedDate = DataObject.getValue(row["UpdatedDate"], DateTimeOffset.Now);
			generalConfiguration.UpdatedBy = DataObject.getValue(row["UpdatedBy"], BaseDataObject.ADMIN);

			return generalConfiguration;
		}


		/// <summary>
		/// Add parameters that are used by both the insert and update stored procedures
		/// </summary>
		/// <param name="cmd">A SqlCommand to add parameters to</param>
		/// <param name="generalConfiguration">Contains values for the insert and update stored procedures</param>
		private static void AddCommonInsertUpdateParameters(SqlCommand cmd, GasboyStationGeneralConfiguration generalConfiguration)
		{
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = generalConfiguration.SiteGuid;
			cmd.Parameters.Add("@RetailSaleTransactionAliasGuid", SqlDbType.UniqueIdentifier).Value = generalConfiguration.RetailSaleTransactionAliasGuid ?? (object)DBNull.Value;
			cmd.Parameters.Add("@DownloadTransactionsIntervalMinutes", SqlDbType.Int).Value = generalConfiguration.DownloadTransactionsIntervalMinutes ?? (object)DBNull.Value;
			cmd.Parameters.Add("@DownloadEventsIntervalMinutes", SqlDbType.Int).Value = generalConfiguration.DownloadEventsIntervalMinutes ?? (object)DBNull.Value;
		}
	}
}