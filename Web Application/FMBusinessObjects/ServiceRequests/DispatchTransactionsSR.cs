// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatchTransactionsSR.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the TransactionOrigin type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.ServiceRequests
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Diagnostics;
	using System.Globalization;
	using System.Linq;
	using System.Runtime.Serialization;
	using System.Security;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// A dispatch transaction service request.
	/// </summary>
	[Serializable]
	[SecuritySafeCritical]
	[DataContract]
	public class DispatchTransactionsSR : AccountingServiceRequest
	{
		public enum SubCommands
		{
			GetVersion,
			GetCount,
			GetTransferCandidates,
			None
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DispatchTransactionsSR"/> class.
		/// </summary>
		public DispatchTransactionsSR()
		{
			this.AliasNames = new List<string>();
			this.Statuses = new List<string>();
			this.Translations = new List<DispatchTranslationPair>();
			this.TransactionList = new List<string>();
		}

		/// <summary>
		/// Gets or sets the alias names.
		/// </summary>
		/// <value>
		/// The alias names.
		/// </value>
		[DataMember]
		public List<string> AliasNames { get; set; }

		/// <summary>
		/// Gets or sets the begin date.
		/// </summary>
		/// <value>
		/// The begin date.
		/// </value>
		[DataMember]
		public DateTimeOffset BeginDate { get; set; }

		/// <summary>
		/// Gets or sets the end date.
		/// </summary>
		/// <value>
		/// The end date.
		/// </value>
		[DataMember]
		public DateTimeOffset EndDate { get; set; }

		/// <summary>
		/// Gets or sets the statuses.
		/// </summary>
		/// <value>
		/// The statuses.
		/// </value>
		[DataMember]
		public List<string> Statuses { get; set; }

		/// <summary>
		/// Gets or sets the translations.
		/// </summary>
		/// <value>
		/// The translations.
		/// </value>
		[DataMember]
		public List<DispatchTranslationPair> Translations { get; set; }

		/// <summary>
		/// Gets or sets the row version.
		/// </summary>
		/// <value>
		/// The row version.
		/// </value>
		[DataMember]
		public string RowVersion { get; set; }

		/// <summary>
		/// Gets or sets a list of transaction IDs to include in the result set even if they do not
		/// meet the specified filters.
		/// </summary>
		[DataMember]
		public List<string> TransactionList { get; set; }

		[DataMember]
		public SubCommands SubCommand { get; set; }

		[DataMember]
		public DateTimeOffset TransferDateTime { get; set; }

		/// <summary>
		/// Gets the SQL.
		/// </summary>
		/// <param name="cmd">The CMD.</param>
		/// <param name="site">The site.</param>
		public void GetSQL(SqlCommand cmd, SiteClass site)
		{
			DateTimeOffset startDate = this.BeginDate.Date;
			DateTimeOffset endDate = this.EndDate.Date;

			cmd.CommandText = " SELECT " +
				"T.TransID," +
				"L.TransactionLineItemGuid," + 
				"T.AliasName," +
				"T.TransDateTime," +
				"T.FuelAdditiveFlag," +
				"T.SubmittedToAccounting," +
				"T.IssuePoint," +
				"T.IssuePointNumber," +
				"T.RadioNumber," +
				"T.OperatorID," +
				"T.OperatorName, " +
				"T.OperatorPersonnelGuid," +
				"T.LookupTransactionStatusIndex," +
				"S.TransactionStatusName AS 'TransactionStatus'," +
				"T.LookupTransTypeIndex," +
				"T.LookupTransTypeIndex AS 'TransTypeID'," +
				"T.FST," +
				"T.TimeEnd," +
				"T.TimeOut," +
				"T.RequestedDateTime," +
				"T.DispatchedDateTime," +
				"T.TimeIn," +
				"(DATEDIFF(minute,T.RequestedDateTime,T.TimeIn)) AS ResponseTime," +
				"(DATEDIFF(minute,T.FST,T.TimeEnd)) AS FuelTime," +
				"T.Number01," +
				"T.Number02," +
				"T.Number03," +
				"T.Number04," +
				"T.Number05," +
				"T.Number06," +
				"(SELECT Notes FROM tblTransactionNotes WITH (NOLOCK) WHERE TransactionGuid = T.TransactionGuid) AS Notes," +
				"U.*," +
				"ABS(dbo.udf_ConvertFromSIUnits(L.GrossQuantity,dbo.udf_ProductTypeFactor(L.ProductType,@AdditiveVolumeUnits,@VolumeUnits),dbo.udf_ProductTypeFactor(L.ProductType,@AdditiveVolumeDecimalPlaces,@VolumeDecimalPlaces))) AS GrossQuantity," +
				"ABS(dbo.udf_ConvertFromSIUnits(L.NetQuantity,dbo.udf_ProductTypeFactor(L.ProductType,@AdditiveVolumeUnits,@VolumeUnits),dbo.udf_ProductTypeFactor(L.ProductType,@AdditiveVolumeDecimalPlaces,@VolumeDecimalPlaces))) AS NetQuantity," +
				"L.Product AS ProductID," +
				"L.ProductGuid," +
				"dbo.udf_ConvertFromSIUnits(L.Variance,dbo.udf_ProductTypeFactor(L.ProductType,@AdditiveVolumeUnits,@VolumeUnits),dbo.udf_ProductTypeFactor(L.ProductType,@AdditiveVolumeDecimalPlaces,@VolumeDecimalPlaces)) AS Variance," +
				"T.Flag01," +
				"T.Flag02," +
				"T.Flag03," +
				"T.Flag04," +
				"T.Flag05," +
				"T.Flag06," +
				"T.ContactSurname," +
				"T.ShipToID," +
				"T.BillToID," +
				// Aircraft are only displayed for Refuels and Defuels. For Refuels, the aircraft is the destination equipment. record
				// For Defuels, the aircraft is the source equipment record
				"AircraftID = CASE WHEN (T.LookupTransTypeIndex = 6) THEN T.DestinationRegistrationID1 " + // Refuel
					"WHEN (T.LookupTransTypeIndex = 4) THEN T.SourceRegistrationID1 " + // Defuel
					"ELSE NULL " + 
					"END, " +
				// The Vehicle is the fueling vehicle involved in the transaction. 
				// The Vehicle is the equipment vehicle for Refuels or Return to Bulks (it's dispensing the fuel to either a plane or a fill stand) 
				// The Vehicle is the destination equipment for Defuels or FillStand transactions (it's receiving the fuel from either a plane or a fill stand) 
				"VehicleID = CASE WHEN (T.LookupTransTypeIndex = 6 OR T.LookupTransTypeIndex = 10) THEN T.SourceRegistrationID1 " + // Refuel or Return to Bulk
					"WHEN (T.LookupTransTypeIndex = 4 OR T.LookupTransTypeIndex = 7) THEN T.DestinationRegistrationID1 " + // Defuel or FillStand
					"ELSE NULL " +
					"END, " +
				// The model is the aircraft model, which only applies to Refuels or Defuels
				"Model = CASE WHEN (T.LookupTransTypeIndex = 6) THEN T.DestinationEquipmentModel1 " + // Refuel
					"WHEN (T.LookupTransTypeIndex = 4) THEN T.SourceEquipmentModel1 " + // Defuel
					"ELSE NULL " +
					"END, " +
				// The XRef displayed depends on the type of transaction. For Refuels and Defuels, it's the aircraft's Xref. The aircraft is either the source (Defuel) or Destination (Refuel)
				// For FillStands and Return to Bulks, it's the fueling vehicle's Xref. The fueling vehicle is either the source (Return to Bulk) or destination (FillStand)
				"XREF = CASE WHEN (T.LookupTransTypeIndex = 6 OR T.LookupTransTypeIndex = 7) THEN DestinationEquipment.Xref " + // Refuel or FillStand
					"WHEN (T.LookupTransTypeIndex = 4 OR T.LookupTransTypeIndex = 10) THEN SourceEquipment.Xref " + // Defuel or Return to Bulk
					"ELSE NULL " +
					"END, " +
				"SourceEquipment.IssPtNum, " +
				"T.CardNumber," +
				"T.FuelCardID," +
				"T.Site," +
				"SourceEquipment.Volume AS OnHandQuantity," +
				"U.UserData7 AS Location," +
				// Converting a Timestamp column to a string requires some legwork to get the correct string representation.
				// We must first cast the _RowVersion to a BINARY(8) column and then convert it to a string.
				"CONVERT(NVARCHAR(100), CAST(T._RowVersion AS BINARY(8)), 1) AS RowVersionString," +
				"(SELECT LastName + ',' + FirstName from tblPersonnel WITH (NOLOCK) WHERE PersonnelGuid = T.OperatorPersonnelGuid) AS OperatorName" +
				" FROM dbo.tblTransactions T WITH (NOLOCK) " + 
				" INNER JOIN dbo.tblTransactionLineItems L WITH (NOLOCK) ON T.TransactionGuid = L.TransactionGuid" +
				" LEFT JOIN dbo.tblTransactionUserData U WITH (NOLOCK) ON U.TransactionGuid = T.TransactionGuid" +
                " LEFT JOIN dbo.tblEquipment DestinationEquipment WITH (NOLOCK)" +
                " ON DestinationEquipment.EquipmentGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Equipment', T.Destination1EquipmentGuid, @SiteGuid)" +                  					
                " LEFT JOIN dbo.tblEquipment SourceEquipment WITH (NOLOCK)" +
                " ON SourceEquipment.EquipmentGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Equipment', T.Source1EquipmentGuid, @SiteGuid)" +
				" LEFT JOIN lookup.tblTransactionStatus S WITH (NOLOCK) ON S.TransactionStatusIndex = T.LookupTransactionStatusIndex";

			this.AddFilterConditions(cmd, "T", site);

			cmd.CommandText += " ORDER BY T._RowVersion";

			cmd.Parameters.Add("@StartDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@EndDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@AdditiveVolumeUnits", SqlDbType.Int);
			cmd.Parameters.Add("@AdditiveVolumeDecimalPlaces", SqlDbType.Int);
			cmd.Parameters.Add("@VolumeUnits", SqlDbType.Int);
			cmd.Parameters.Add("@VolumeDecimalPlaces", SqlDbType.Int);

			cmd.Parameters["@StartDate"].Value = startDate;
			cmd.Parameters["@EndDate"].Value = endDate.AddDays(1);
			cmd.Parameters["@SiteGuid"].Value = this.Security.SiteGuid;
			cmd.Parameters["@AdditiveVolumeUnits"].Value = site.AdditiveVolumeUnits;
			cmd.Parameters["@AdditiveVolumeDecimalPlaces"].Value = site.AdditiveVolumeDecimalPlaces;
			cmd.Parameters["@VolumeUnits"].Value = site.VolumeUnits;
			cmd.Parameters["@VolumeDecimalPlaces"].Value = site.VolumeDecimalPlaces;
		}

		/// <summary>
		/// This method returns a SQL statement suitable for use in SQLDependency.  It must explicitly include
		/// all fields that are used in the Where clause as created in AddFilterConditions.
		/// </summary>
		/// <param name="cmd">
		/// The SqlCommand object to use.
		/// </param>
		/// <param name="site">
		/// The current FuelsManager site context.
		/// </param>
		/// <param name="topVersion">The latest version to use for comparison.</param>
		public void GetUpdateCommand(SqlCommand cmd, SiteClass site, string topVersion)
		{
			cmd.CommandText = "SELECT COUNT(*) AS 'RowCount' FROM dbo.tblTransactionLineItems L WITH (NOLOCK) " +
				"JOIN dbo.tblTransactions WITH (NOLOCK) ON tblTransactions.TransactionGuid = L.TransactionGuid " +
				"LEFT JOIN lookup.tblTransactionStatus S WITH (NOLOCK) ON S.TransactionStatusIndex = tblTransactions.LookupTransactionStatusIndex";

			this.AddFilterConditions(cmd, string.Empty, site);

			cmd.CommandText += " AND dbo.tblTransactions._RowVersion > " + topVersion;

			DateTimeOffset startDate = this.BeginDate;
			DateTimeOffset endDate = this.EndDate;

			cmd.Parameters.Add("@StartDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@EndDate", SqlDbType.DateTimeOffset);

			cmd.Parameters["@StartDate"].Value = startDate.Date;
			cmd.Parameters["@EndDate"].Value = endDate.Date.AddDays(1);
		}

		/// <summary>
		/// This method will return a SQL Command object that will retrieve a count of any transactions
		/// that have a status not equal to "Completed" and a status not equal to "Cancelled" and an
		/// origin equal to "Dispatch" and a flag Ready For Accounting "Flag03" equal to false.
		/// </summary>
		/// <returns></returns>
		public SqlCommand CanTransferBeginSQL()
		{
			// Tx status Completed = 0; Tx status Cancelled = 7; Origin Dispatch = 3; Dispatch Transfer Flag (Flag03) = 0
			// means not closed out.
			string sql =
                string.Format(
                    "SELECT COUNT(*) AS TxCount FROM tblTransactions WITH(NOLOCK) " +
							 "WHERE TransactionStatus <> {0} AND TransactionStatus <> {1} AND LookupOriginApplicationIndex IN ({2}) " +
							 "AND (SubmittedToAccounting = 0 OR SubmittedToAccounting IS NULL) " +
							 "AND RequestedDateTime <= @TransferDateTime AND SiteGuid = @SiteGuid ",
							(int)TransactionStatus.Completed,
							(int)TransactionStatus.Cancelled,
							TransactionOriginExtensions.GetDispatchOriginList());

			var sqlCommand = new SqlCommand { CommandText = sql };

			var parm = new SqlParameter("@TransferDateTime", SqlDbType.DateTimeOffset) { Value = this.TransferDateTime };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@SiteGuid", SqlDbType.Int) { Value = this.Security.SiteGuid };
			sqlCommand.Parameters.Add(parm);

			return sqlCommand;
		}

		/// <summary>
		/// This method will return a SQL command object that will retrieve all the transactions that are
		/// candidates for closeout based on the transaction status of Completed or Cancelled, an origin
		/// of Dispatch, dispatch transfer flag equal to false, and is less than or equal to the closeout
		/// date time.
		/// </summary>
		/// <param name="lockoutDateTime"></param>
		/// <returns></returns>
		public SqlCommand GetTransactionTransferCandidates()
		{
			// Tx status Completed = 0; Tx status Cancelled = 7; Origin Dispatch = 3; Dispatch Transfer Flag (Flag03) = 0
			// means not closed out.
			string sql =
				string.Format(
							"SELECT TransID FROM tblTransactions WITH(NOLOCK) " +
							 "WHERE (TransactionStatus = {0} OR TransactionStatus = {1}) AND LookupOriginApplicationIndex IN ({2}) " +
							 "AND (SubmittedToAccounting = 0) " +
							 "AND RequestedDateTime <= @TransferDateTime AND SiteGuid = @SiteGuid ",
							(int)TransactionStatus.Completed,
							(int)TransactionStatus.Cancelled,
							TransactionOriginExtensions.GetDispatchOriginList());

			var sqlCommand = new SqlCommand { CommandText = sql };

			var parm = new SqlParameter("@TransferDateTime", SqlDbType.DateTimeOffset) { Value = this.TransferDateTime };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@SiteGuid", SqlDbType.Int) { Value = this.Security.SiteGuid };
			sqlCommand.Parameters.Add(parm);

			return sqlCommand;
		}

		/// <summary>
		/// Adds the filter conditions.
		/// </summary>
		/// <param name="cmd">The CMD.</param>
		/// <param name="transactionTableAlias">The transaction table alias.</param>
		/// <param name="site">The primary site.</param>
		private void AddFilterConditions(SqlCommand cmd, string transactionTableAlias, SiteClass site)
		{
			if (!this.AliasNames.Any())
			{
				cmd.CommandText += " WHERE 1 = 0 ";
				return;
			}

			// Determine transaction table alias text
			var transTableName = String.IsNullOrEmpty(transactionTableAlias) ? String.Empty : transactionTableAlias + ".";

			bool areTranslationsPresent = this.Translations.Count > 0;

			cmd.CommandText += " WHERE 1 = 1 ";

			this.AddSiteFilterCondition(cmd, site, transTableName);

			if (String.IsNullOrEmpty(transactionTableAlias))
			{
				cmd.CommandText += " AND tblTransactions.[DeleteFlag] = 0 AND tblTransactions.[RequestedDateTime] BETWEEN @StartDate AND @EndDate";
			}
			else
			{
				cmd.CommandText += String.Format(" AND {0}.[DeleteFlag] = 0 AND {0}.[RequestedDateTime] BETWEEN @StartDate AND @EndDate", transactionTableAlias);
			}

			if (this.Statuses.Count > 0)
			{
				var statusParamNames = new ArrayList();
				string sql = String.Empty;
				
				for (int i = 0; i < this.Statuses.Count; i++)
				{
					string paramName = "@Status" + i.ToString(CultureInfo.InvariantCulture);
					cmd.Parameters.Add(paramName, SqlDbType.NVarChar, 20);
					cmd.Parameters[paramName].Value = this.Statuses[i];

					statusParamNames.Add(paramName);
				}

				const string FieldValue = "[TransactionStatusName]";

				if (statusParamNames.Count > 0)
				{
					sql += " AND " + FieldValue + " IN ({0}) ";
					var paramStrings = statusParamNames.ToArray(typeof(string)) as string[];
					Debug.Assert(paramStrings != null, "Expect paramStrings != null");
					sql = String.Format(sql, String.Join(",", paramStrings));
					cmd.CommandText += sql;
				}
			}

			if (this.AliasNames.Count > 0)
			{
				var aliasParamNames = new ArrayList();
				string sql = String.Empty;

				for (int i = 0; i < this.AliasNames.Count; i++)
				{
					string translatedValue = this.AliasNames[i];
					if (areTranslationsPresent)
					{
						translatedValue = this.TranslateAlias(this.AliasNames[i]);
					}

					string paramName = "@Alias" + i.ToString(CultureInfo.InvariantCulture);
					cmd.Parameters.Add(paramName, SqlDbType.NVarChar, 32);
					cmd.Parameters[paramName].Value = translatedValue;

					aliasParamNames.Add(paramName);
				}

				if (aliasParamNames.Count > 0)
				{
					sql += " AND [AliasName] IN ({0}) ";
					var paramStrings = aliasParamNames.ToArray(typeof(string)) as string[];
					Debug.Assert(paramStrings != null, "Expect paramStrings != null");
					sql = String.Format(sql, String.Join(",", paramStrings));
					cmd.CommandText += sql;
				}
			}

			if (this.TransactionList.Count > 0)
			{
				string sql = String.Empty;

				for (int i = 0; i < this.TransactionList.Count; i++)
				{
					string paramName = "@TransID" + i.ToString(CultureInfo.InvariantCulture);

					string sqlOR = "[TransID] = " + paramName;

					if (String.IsNullOrEmpty(transactionTableAlias) == false)
					{
						sqlOR = transactionTableAlias + "." + sqlOR;
					}

					cmd.Parameters.Add(paramName, SqlDbType.NVarChar, 64);
					cmd.Parameters[paramName].Value = this.TransactionList[i];

					sql += " OR " + sqlOR;
				}

				cmd.CommandText += sql;
			}

			cmd.Parameters.Add("@LookupOriginApplicationIndex", SqlDbType.Int);

			cmd.Parameters["@LookupOriginApplicationIndex"].Value = (int)TransactionOrigin.Dispatch;
		}

		/// <summary>
		/// Adds the site filter condition.  It includes the ability to add member sites to a group site 
		/// query so as to include member site transactions in the dispatching view.
		/// </summary>
		/// <param name="cmd">The CMD being built.</param>
		/// <param name="site">The current site.</param>
		/// <param name="transTableName">Name of the trans table.</param>
		private void AddSiteFilterCondition(SqlCommand cmd, SiteClass site, string transTableName)
		{
			var siteGuids = site.SiteToSiteMapCollection.Select(x => x.ChildSiteGuid.ToString()).ToArray();

			string memberSites = String.Empty;
			if (siteGuids.Length > 0)
			{
				memberSites += ",'" + String.Join("','", siteGuids) + "'";
			}

			cmd.CommandText += String.Format(" AND {0}[SiteGuid] in (@SiteGuidParam2{1}) ", transTableName, memberSites);

			cmd.Parameters.Add("@SiteGuidParam2", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@SiteGuidParam2"].Value = this.Security.SiteGuid;
		}

		/// <summary>
		/// This method returns a SQL statement suitable for use in checking for transaction updates.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <returns>A SqlCommand object initialized to check for dispatch transaction updates.</returns>
		public SqlCommand GetUpdateCommand(SecurityClass security)
		{
			string sql = string.Format(
					"SELECT TOP 1 _RowVersion FROM tblTransactions " +
					"WHERE SiteGuid = @SiteGuid AND SubmittedToAccounting = 0 AND LookupOriginApplicationIndex IN ({0}) " +
					"ORDER BY _RowVersion DESC", TransactionOriginExtensions.GetDispatchOriginList());
			
			var cmd = new SqlCommand(sql);
			cmd.Parameters.AddWithValue("@SiteGuid", security.SiteGuid);

			return cmd;
		}

		/// <summary>
		/// Translates the alias.
		/// </summary>
		/// <param name="alias">The alias.</param>
		/// <returns>The translated alias if a translation exists, otherwise returns alias name.</returns>
		private string TranslateAlias(string alias)
		{
			foreach (DispatchTranslationPair pair in this.Translations)
			{
				if (alias.Equals(pair.DispatchName))
				{
					return pair.AccountingName;
				}
			}

			return alias;
		}

		/// <summary>
		/// A dispatch alias translation pair.
		/// </summary>
		[Serializable]
		[DebuggerDisplay("AccountingName={AccountingName}, DispatchName={DispatchName}")]
		[DataContract]
		public class DispatchTranslationPair
		{
			/// <summary>
			/// Gets or sets the name as known by accounting.
			/// </summary>
			/// <value>
			/// The name as known by accounting.
			/// </value>
			[DataMember]
			public string AccountingName { get; set; }

			/// <summary>
			/// Gets or sets the name as known by dispatch.
			/// </summary>
			/// <value>
			/// The name as known by dispatch.
			/// </value>
			[DataMember]
			public string DispatchName { get; set; }
		}
	}
}
