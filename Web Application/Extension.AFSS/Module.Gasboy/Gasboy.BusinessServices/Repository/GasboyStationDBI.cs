// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GasboyStationDBI.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// Implements database interaction for the gasboy station functionality
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
	/// Implements database interaction for the gasboy station functionality
	/// </summary>
	public class GasboyStationDBI : DataAccessLayerDBI
	{
		#region Attributes
		#endregion Attributes

		/// <summary>
		/// Construct a new GasboyStationDBI object and set the user to the provided parameter
		/// </summary>
		/// <param name="user">
		/// The user executing this operation
		/// </param>
		public GasboyStationDBI(string user)
			: base(user)
		{
		}

		#region Standard Data Access Methods

		/// <summary>
		/// Gets a list of all the Gasboy stations associated with the specified Site.
		/// </summary>
		/// <param name="security">Contains security credentials</param>
		/// <param name="siteGuid">Identifies the site to retrieve the records for</param>
		/// <param name="id">The ID to search for matches on</param>
		/// <returns>All the Gasboy stations associated with the specified Site, filtered by ID if it was provided.</returns>
		public List<GasboyStation> GetList(SecurityClass security, Guid siteGuid, string id)
		{
			var dataObjects = new List<GasboyStation>();

			DataSet dataSet = this.Load(security, null, siteGuid, id, null, false);

			if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return dataObjects;
			}

			foreach (DataRow row in dataSet.Tables[0].Rows)
			{
				var dataObject = new GasboyStation
				{
					IdentityGuid = DataObject.getValue(row["ExternalStationGuid"], Guid.Empty),
					SiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty),
					ID = DataObject.getValue(row["ID"], string.Empty),
					BillingID = DataObject.getValue(row["BillingID"], string.Empty),
					Status = DataObject.getValue(row["LookupExternalStationStatusIndex"], ExternalStationStatus.Inactive),
					LastConnectionAttempt = DataObject.getValue<DateTimeOffset?>(row["LastConnectionAttempt"], null),
					LastSuccessfulConnection = DataObject.getValue<DateTimeOffset?>(row["LastSuccessfulConnection"], null),
					LastTransactionID = DataObject.getValue<long?>(row["LastTransactionID"], null),
					LastDeviceCount = DataObject.getValue<int?>(row["LastDeviceCount"],null),
					DownloadTransactionsAutomatically = DataObject.getValue(row["DownloadTransactionsAutomatically"],false),
					SiteCode = DataObject.getOptionalInt(row["SiteCode"]),
					UserName = DataObject.getValue(row["UserName"], string.Empty),
					IpAddress = DataObject.getValue(row["IPAddress"], string.Empty)
			};

				dataObject.Password =
					DataObject.getValue(
						row["Password"] == DBNull.Value ? string.Empty : UserClass.decode((byte[])row["Password"], dataObject.SiteGuid),
						string.Empty);

				dataObjects.Add(dataObject);
			}

			return dataObjects;
		}

		/// <summary>
		/// Gets a specific Gasboy station that has the specified ID for the specified site.
		/// </summary>
		/// <param name="security">Contains security credentials</param>
		/// <param name="siteGuid">Identifies the site to retrieve the records for</param>
		/// <param name="id">The ID to search for matches on</param>
		/// <returns>The Gasboy Station identified by the provided ID.</returns>
		public GasboyStation GetByID(SecurityClass security, Guid siteGuid, string id)
		{
			if (siteGuid == Guid.Empty)
			{
				throw new Exception("SiteGuid parameter cannot be empty.");
			}

			if (string.IsNullOrEmpty(id))
			{
				throw new ArgumentException("id");
			}

			DataSet dataSet = this.Load(security, null, siteGuid, id, null, true);

			if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return null;
			}

			var dataObject = this.GetDataObjectFromDataRow(dataSet.Tables[0].Rows[0]);

			return dataObject;
		}

		/// <summary>
		/// Gets a specific Gasboy station that has the specified SiteCode for the specified site.
		/// </summary>
		/// <param name="security">Contains security credentials</param>
		/// <param name="siteGuid">Identifies the site to retrieve the records for</param>
		/// <param name="siteCode">The siteCode to search for matches on</param>
		/// <returns>The Gasboy Station identified by the provided ID.</returns>
		public GasboyStation GetBySiteCode(SecurityClass security, Guid siteGuid, string siteCode)
		{
			if (siteGuid == Guid.Empty)
			{
				throw new Exception("SiteGuid parameter cannot be empty.");
			}

			if (string.IsNullOrEmpty(siteCode))
			{
				throw new ArgumentException("siteCode");
			}

			DataSet dataSet = this.Load(security, null, siteGuid, null, siteCode, true);

			if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return null;
			}

			var dataObject = this.GetDataObjectFromDataRow(dataSet.Tables[0].Rows[0]);

			return dataObject;
		}
		
		/// <summary>
		/// Gets the current <seealso cref="GasboyStation"/> record.
		/// </summary>
		/// <param name="security">Contains security credentials</param>
		/// <param name="identityGuid">Identifies the record to retrieve</param>
		/// <returns>The Gasboy Station identified by the provided identityGuid</returns>
		public GasboyStation Get(SecurityClass security, Guid identityGuid)
		{
			if (identityGuid == Guid.Empty)
			{
				throw new Exception("IdentityGuid parameter cannot be empty.");
			}

			DataSet dataSet = this.Load(security, identityGuid, null, null, null, false);

			if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return null;
			}

			var dataObject = this.GetDataObjectFromDataRow(dataSet.Tables[0].Rows[0]);

			return dataObject;
		}

		/// <summary>
		/// Saves the passed in <seealso cref="GasboyStation"/> record.
		/// </summary>
		/// <param name="security">Contains security credentials</param>
		/// <param name="dataObject">The Gasboy Station business object to save.</param>
		public void Insert(SecurityClass security, GasboyStation dataObject)
		{
			using (SqlCommand cmd = this.PrepareInsertStatement())
			{
				this.BindCommonInsertUpdateParameterValues(cmd, dataObject);

				cmd.Parameters["@CreatedUpdatedBy"].Value = this.CreatedBy;

				this.ConsolidatedDA.ExecuteQuery(security, cmd);   
			}
		}

		/// <summary>
		/// Saves the passed in <seealso cref="GasboyStation"/> record.
		/// </summary>
		/// <param name="security">Contains security credentials</param>
		/// <param name="dataObject">The Gasboy Station object to modify in the database</param>
		public void Update(SecurityClass security, GasboyStation dataObject)
		{
			using (SqlCommand cmd = this.PrepareUpdateStatement())
			{
				this.BindCommonInsertUpdateParameterValues(cmd, dataObject);

				cmd.Parameters["@UpdatedBy"].Value = this.UpdatedBy;

				this.ConsolidatedDA.ExecuteQuery(security, cmd); 
			}          
		}

		/// <summary>
		/// Deletes the passed in <seealso cref="GasboyStation"/> record
		/// </summary>
		/// <param name="security">Contains security credentials</param>
		/// <param name="dataObject">The Gasboy Station object to delete</param>
		public void Delete(SecurityClass security, GasboyStation dataObject)
		{
			using (var cmd = this.PrepareDeleteStatement())
			{
				cmd.Parameters["@IdentityGuid"].Value = dataObject.IdentityGuid;

				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		#endregion Standard Data Access Methods

		#region Custom Data Access Methods

		/// <summary>
		/// Update the connection status information only for the provided external stations
		/// </summary>
		/// <param name="cmd">A sqlCommand object to populate</param>
		/// <param name="externalStations">External stations with the updated connection status information</param>
		public static void UpdateConnectionInformationSQL(SqlCommand cmd, List<GasboyStation> externalStations)
		{
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "usp_ExternalStationUpdateConnectionInformation";

			SqlParameter tableValuedParameter = cmd.Parameters.Add("@ExternalStationConnectionInformation", SqlDbType.Structured);
			tableValuedParameter.Value = CreateSqlDataRecords(externalStations);
			tableValuedParameter.TypeName = "dbo.ExternalStationConnectionInformationType";
		}

		#endregion Custom Data Access Methods

		#region Private Persistence Methods

		/// <summary>
		/// Gets the <seealso cref="GasboyStation"/> object(s) from the database identified by the provided parameters
		/// </summary>
		/// <param name="security">Contains security credentials</param>
		/// <param name="identityGuid">Contains the identity guid of a specific record to load.</param>
		/// <param name="siteGuid">Contains the SiteGuid to restrict the results to.</param>
		/// <param name="id">Contains the station ID to load, must be combined with the SiteGuid.</param>
		/// <param name="siteCode">Contains the station siteCode to load, must be combined with the SiteGuid.</param>
		/// <param name="exactIdMatch">If true, only stations that match the site guid AND exact id value will be returned</param>
		/// <returns>A dataset populated with matching gasboy stations or null if no matches exist.</returns>
		private DataSet Load(SecurityClass security, Guid? identityGuid, Guid? siteGuid, string id, string siteCode, bool exactIdMatch = true)
		{
			DataSet dataSet = null;

			if (identityGuid.HasValue || exactIdMatch)
			{
				using (var cmd = this.PrepareSelectStatement())
				{
					if (exactIdMatch)
					{
						cmd.Parameters["@SiteGuid"].Value = this.SetOptionalValue<Guid>(siteGuid);
						cmd.Parameters["@ID"].Value = this.SetOptionalValue<string>(id);
						cmd.Parameters["@SiteCode"].Value = this.SetOptionalValue<string>(siteCode);
					}
					else
					{
						cmd.Parameters["@IdentityGuid"].Value = this.SetOptionalValue<Guid>(identityGuid);
					}

					dataSet = this.ConsolidatedDA.GetDataSet(cmd, security);
				}
			}
			else
			{
				using (var cmd = this.PrepareEnumerateStatement())
				{
					cmd.Parameters["@SiteGuid"].Value = this.SetOptionalValue<Guid>(siteGuid);
					cmd.Parameters["@ID"].Value = this.SetOptionalValue<string>(id);

					dataSet = this.ConsolidatedDA.GetDataSet(cmd, security);
				}
			}

			return dataSet;
		}

		#endregion Private Persistence Methods

		#region Override Implementations for Prepare Methods

		protected override SqlCommand PrepareSelectStatement()
		{
			var cmd = new SqlCommand
						  {
							  CommandType = CommandType.StoredProcedure,
							  CommandText = "usp_ExternalGasboyStationGet"
						  };

			cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@SiteCode", SqlDbType.NVarChar, 50);

			return cmd;
		}

		protected override SqlCommand PrepareInsertStatement()
		{
			var cmd = new SqlCommand
						  {
							  CommandType = CommandType.StoredProcedure,
							  CommandText = "usp_ExternalGasboyStationInsert"
						  };

			cmd.Parameters.Clear();

			cmd.Parameters.Add("@CreatedUpdatedBy", SqlDbType.NVarChar, 100);

			this.AddCommonInsertUpdateParameters(cmd);

			return cmd;
		}

		protected override SqlCommand PrepareUpdateStatement()
		{
			var cmd = new SqlCommand
						  {
							  CommandType = CommandType.StoredProcedure,
							  CommandText = "usp_ExternalGasboyStationUpdate"
						  };

			cmd.Parameters.Clear();

			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);

			this.AddCommonInsertUpdateParameters(cmd);

			return cmd;
		}

		protected override SqlCommand PrepareDeleteStatement()
		{
			var cmd = new SqlCommand
						  {
							  CommandType = CommandType.StoredProcedure,
							  CommandText = "usp_ExternalGasboyStationDelete"
						  };

			cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);

			return cmd;
		}

		#endregion Override Implementations for Prepare Methods

		#region Private Static Support Methods

		/// <summary>
		/// Create sqlDataRecords for use by the connection status update stored procedure
		/// </summary>
		/// <param name="externalStations">External Stations with the updated connection information</param>
		/// <returns>SqlDataRecords for use by the connection status update stored procedure</returns>
		private static IEnumerable<SqlDataRecord> CreateSqlDataRecords(IEnumerable<GasboyStation> externalStations)
		{
			SqlMetaData[] metaData = new SqlMetaData[6];

			int i = 0;
			metaData[i++] = new SqlMetaData("ExternalStationGuid", SqlDbType.UniqueIdentifier);
			metaData[i++] = new SqlMetaData("LookupExternalStationStatusIndex", SqlDbType.Int);
			metaData[i++] = new SqlMetaData("LastSuccessfulConnection", SqlDbType.DateTimeOffset);
			metaData[i++] = new SqlMetaData("LastConnectionAttempt", SqlDbType.DateTimeOffset);
			metaData[i++] = new SqlMetaData("LastTransactionID", SqlDbType.BigInt);
			metaData[i] = new SqlMetaData("UpdatedBy", SqlDbType.NVarChar, 100);

			SqlDataRecord record = new SqlDataRecord(metaData);

			foreach (GasboyStation externalStation in externalStations)
			{
				int j = 0;

				record.SetGuid(j++, externalStation.IdentityGuid);
				record.SetInt32(j++, (int)externalStation.Status);

				if (externalStation.LastSuccessfulConnection.HasValue)
				{
					record.SetDateTimeOffset(j++, externalStation.LastSuccessfulConnection.Value);
				}
				else
				{
					record.SetDBNull(j++);
				}

				if (externalStation.LastConnectionAttempt.HasValue)
				{
					record.SetDateTimeOffset(j++, externalStation.LastConnectionAttempt.Value);
				}
				else
				{
					record.SetDBNull(j++);
				}

				if (externalStation.LastTransactionID.HasValue)
				{
					record.SetInt64(j++, externalStation.LastTransactionID.Value);
				}
				else
				{
					record.SetDBNull(j++);
				}

				record.SetString(j, externalStation.UpdatedBy);

				yield return record;
			}
		}

		#endregion Private Static Support Methods

		#region Private Support Methods

		protected SqlCommand PrepareEnumerateStatement()
		{
			var cmd = new SqlCommand
						  {
							  CommandType = CommandType.StoredProcedure,
							  CommandText = "usp_ExternalStationEnumerate"
						  };

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 50);

			return cmd;
		}
		
		/// <summary>
		/// Add parameters that are used by both the insert and update stored procedures
		/// </summary>
		/// <param name="cmd">A SqlCommand to add parameters to</param>
		private void AddCommonInsertUpdateParameters(SqlCommand cmd)
		{
			cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@BillingID", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@SiteCode", SqlDbType.NVarChar, 6);
			cmd.Parameters.Add("@UserName", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@Password", SqlDbType.VarBinary, 256);
			cmd.Parameters.Add("@IPAddress", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@DownloadTransactionsAutomatically", SqlDbType.Bit);
			cmd.Parameters.Add("@LookupExternalStationStatusIndex", SqlDbType.Int);
			cmd.Parameters.Add("@LastConnectionAttempt", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@LastSuccessfulConnection", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@LastTransactionID", SqlDbType.BigInt);
			cmd.Parameters.Add("@LastDeviceCount", SqlDbType.Int);
		}

		/// <summary>
		/// Bind parameter values that are used by both the insert and update stored procedures
		/// </summary>
		/// <param name="cmd">A SqlCommand to add parameters to</param>
		/// <param name="dataObject">The Gasboy Station that needs to be persisted.</param>
		private void BindCommonInsertUpdateParameterValues(SqlCommand cmd, GasboyStation dataObject)
		{
			cmd.Parameters["@IdentityGuid"].Value = dataObject.IdentityGuid;
			cmd.Parameters["@ID"].Value = dataObject.ID;
			cmd.Parameters["@SiteGuid"].Value = dataObject.SiteGuid;
			cmd.Parameters["@BillingID"].Value = dataObject.BillingID;
			cmd.Parameters["@SiteCode"].Value = dataObject.SiteCode;
			cmd.Parameters["@UserName"].Value = dataObject.UserName;
			cmd.Parameters["@Password"].Value = !string.IsNullOrEmpty(dataObject.Password) ? UserClass.encode(dataObject.Password, dataObject.SiteGuid) : (object)DBNull.Value;
			cmd.Parameters["@IPAddress"].Value = dataObject.IpAddress;
			cmd.Parameters["@DownloadTransactionsAutomatically"].Value = dataObject.DownloadTransactionsAutomatically ? 1 : 0;           
			cmd.Parameters["@LookupExternalStationStatusIndex"].Value = (int)dataObject.Status;
			cmd.Parameters["@LastConnectionAttempt"].Value = dataObject.LastConnectionAttempt ?? (object)DBNull.Value;
			cmd.Parameters["@LastSuccessfulConnection"].Value = dataObject.LastSuccessfulConnection ?? (object)DBNull.Value;
			cmd.Parameters["@LastTransactionID"].Value = dataObject.LastTransactionID ?? (object)DBNull.Value;
			cmd.Parameters["@LastDeviceCount"].Value = dataObject.LastDeviceCount ?? (object)DBNull.Value;
		}

		/// <summary>
		/// The get data object from data row.
		/// </summary>
		/// <param name="row">
		/// The row containing the station record
		/// </param>
		/// <returns>
		/// The <see cref="GasboyStation"/>.
		/// </returns>
		private GasboyStation GetDataObjectFromDataRow(DataRow row)
		{
			var dataObject = new GasboyStation();

			dataObject.IdentityGuid = DataObject.getValue(row["ExternalStationGuid"], Guid.Empty);
			dataObject.SiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty);
			dataObject.ID = DataObject.getValue(row["ID"], string.Empty);
			dataObject.BillingID = DataObject.getValue(row["BillingID"], string.Empty);
			dataObject.SiteCode = DataObject.getOptionalInt(row["SiteCode"]);
			dataObject.UserName = DataObject.getValue(row["UserName"], string.Empty);
			dataObject.Password = DataObject.getValue(row["Password"] == DBNull.Value ? string.Empty : UserClass.decode((byte[])row["Password"], dataObject.SiteGuid), string.Empty);
			dataObject.IpAddress = DataObject.getValue(row["IPAddress"], string.Empty);
			dataObject.DownloadTransactionsAutomatically = DataObject.getValue(row["DownloadTransactionsAutomatically"], false);
			dataObject.Status = DataObject.getValue(row["LookupExternalStationStatusIndex"], ExternalStationStatus.Inactive);
			dataObject.LastConnectionAttempt = DataObject.getValue<DateTimeOffset?>(row["LastConnectionAttempt"], null);
			dataObject.LastSuccessfulConnection = DataObject.getValue<DateTimeOffset?>(row["LastSuccessfulConnection"], null);
			dataObject.LastTransactionID = DataObject.getValue<long?>(row["LastTransactionID"], null);
			dataObject.LastDeviceCount = DataObject.getValue<int?>(row["LastDeviceCount"], null);
			dataObject.CreatedDate = DataObject.getValue(row["CreatedDate"], this.CreatedDateTime);
			dataObject.CreatedBy = DataObject.getValue(row["CreatedBy"], this.CreatedBy);
			dataObject.UpdatedDate = DataObject.getValue(row["UpdatedDate"], this.UpdatedDateTime);
			dataObject.UpdatedBy = DataObject.getValue(row["UpdatedBy"], this.UpdatedBy);
		   
			return dataObject;
		}

		#endregion Private Support Methods
	}
}
