// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionSummary.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Transaction Summary service class.  
//   Provides data services to the Transaction Summary Page.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	using FMBusinessServices.DataAccessLayer;

	public class TransactionSummary : ITransactionSummary
	{
		#region Constants and Fields

		/// <summary>
		/// Provides access to the database
		/// </summary>
		private readonly ConsolidatedDAClass consolidatedDa = new ConsolidatedDAClass();

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Select the transaction summary matching the search criteria from the DB
		/// </summary>
		/// <param name="security">Contains security information.</param>
		/// <param name="beginDate">The beginning inventory date specified by the user</param>
		/// <param name="endDate">The ending inventory date specified by the user</param>
		/// <param name="aliasName">The alias name to match on specified by the user</param>
		/// <param name="searchString">Anything provided in the Find String/Search text box to match results on</param>
		/// <param name="pageStart">The first record to retrieve in the result set</param>
		/// <param name="pageLength">The number of records to retrieve from the result set</param>
		/// <param name="sortedColumns">The columns the user has sorted the results on</param>
		/// <param name="recordCount">The total number of records macthed by the sql - not the length of the paged result set.
		/// This is used to display the "Showing records x of y" information on the page.</param>
		public List<TransactionSummaryClass> Enumerate(
			SecurityClass security,
			DateTimeOffset beginDate,
			DateTimeOffset endDate,
			string aliasName,
			string searchString,
			int pageStart,
			int pageLength,
			List<DataTablesColumn> sortedColumns,
			out int recordCount)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_TRANSACTION_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			recordCount = 0;

			var summary = new List<TransactionSummaryClass>();
			var txSummary = new TransactionSummaryClass();

			using (var cmd = new SqlCommand())
			{
				txSummary.EnumerateSql(security, cmd, beginDate, endDate, aliasName, searchString, pageStart, pageLength, sortedColumns);
				cmd.CommandTimeout = 120;

				DataSet dataSet = this.consolidatedDa.GetDataSet(cmd, security);

				if (dataSet.Tables.Count > 0)
				{
					DataTable table = dataSet.Tables[0];
					recordCount = LoadResults(table, summary);
				}
			}

			return summary;
		}

		/// <summary>
		/// Get the configured transaction list view columns for the specified transaction alias
		/// </summary>
		/// <param name="security">Contains security information.</param>
		/// <param name="aliasName">The alias name to match on specified by the user</param>
		/// <returns>A dictionary collection of list view columns with key = DbName and value = DisplayName</returns>
		public Dictionary<string, string> GetListViewAssignedColumns(SecurityClass security, string aliasName)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (aliasName == null)
			{
				throw new ArgumentNullException("aliasName");
			}

			if (!security.HasRight(RIGHT.VIEW_TRANSACTION_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			var displayColumns = new Dictionary<string, string>();

			using (var cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "usp_GetTransactionSummaryColumns";

				cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = security.SiteGuid;
				cmd.Parameters.Add("@AliasName", SqlDbType.NVarChar, 32).Value = aliasName;

				DataSet dataSet = this.consolidatedDa.GetDataSet(cmd, security);

				if (dataSet.Tables.Count > 0)
				{
					DataTable table = dataSet.Tables[0];

					foreach (DataRow row in table.Rows)
					{
						string dbName = (string)row["DbName"];
						string displayName = (string)row["DisplayName"];

						displayColumns[dbName] = displayName;
					}
				}
			}

			return displayColumns;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Get the results from the result set and use them to populate TransactionSummaryClass records
		/// </summary>
		/// <param name="table">Contains the results of our SQL query</param>
		/// <param name="summary">The collection of Transaction Summary records to populate</param>
		/// <returns>The total number of results that matched our query - since the results are paged, this is not the same as the length of the
		/// list we populate.</returns>
		private static int LoadResults(DataTable table, ICollection<TransactionSummaryClass> summary)
		{
			int recordCount = 0;

			foreach (DataRow row in table.Rows)
			{
				// The record count is returned for each result, but we only need to get it once
				if (recordCount == 0)
				{
					recordCount = DataObject.getValue(row["RecordCount"], 0);
				}

				var transactionSummaryRecord = new TransactionSummaryClass
				{
					InventoryDate = (DateTime)row["InventoryDate"],
					TransDateTime = DataObject.getValue<DateTimeOffset>(row["TransDateTime"], DateTimeOffset.MinValue),
					DocumentNumber = DataObject.getValue<string>(row["DocumentNumber"], string.Empty),
					ProductID = DataObject.getValue<string>(row["Product"], string.Empty),
					TransID = DataObject.getValue<string>(row["TransID"], string.Empty),
					TransactionStatus = DataObject.getValue<string>(row["TransactionStatus"], string.Empty),
					AliasName = DataObject.getValue<string>(row["AliasName"], string.Empty),
               GrossQuantity = DataObject.getValue<double>(row["GrossQuantity"], 0),
               NetQuantity = DataObject.getValue<double>(row["NetQuantity"], 0),
               DeliveredGrossQuantity = DataObject.getValue<double>(row["DeliveredGrossQuantity"], 0),
               DeliveredNetQuantity = DataObject.getValue<double>(row["DeliveredNetQuantity"], 0),
               DeliveredGrossManualValueFlag = DataObject.getValue<bool>(row["DeliveredGrossManualValueFlag"], false),
               DeliveredNetManualValueFlag = DataObject.getValue<bool>(row["DeliveredNetManualValueFlag"], false),
               Pressure = DataObject.getValue<double>(row["Pressure"], 0),
               ShipToID = DataObject.getValue<string>(row["ShipToID"], string.Empty),
					ManagerID = DataObject.getValue<string>(row["ManagerID"], string.Empty),
					OwnerID = DataObject.getValue<string>(row["OwnerID"], string.Empty),
					DeleteFlag = DataObject.getValue<bool>(row["DeleteFlag"], false),
					MassQuantity = DataObject.getValue<double>(row["MassQuantity"], 0),
					AutoComplete = DataObject.getValue<bool>(row["AutoComplete"], false),
					BillToID = DataObject.getValue<string>(row["BillToID"], string.Empty),
					CarrierID = DataObject.getValue<string>(row["CarrierID"], string.Empty),
					ConjoinTransID = DataObject.getValue<string>(row["ConjoinTransID"], string.Empty),
					DestinationRegistrationID1 = DataObject.getValue<string>(row["DestinationRegistrationID1"], string.Empty),
					DestinationRegistrationID2 = DataObject.getValue<string>(row["DestinationRegistrationID2"], string.Empty),
					DestinationSerialNumber1 = DataObject.getValue<string>(row["DestinationSerialNumber1"], string.Empty),
					DestinationSerialNumber2 = DataObject.getValue<string>(row["DestinationSerialNumber2"], string.Empty),
					EffectiveDate = DataObject.getValue<DateTimeOffset>(row["EffectiveDate"], DateTimeOffset.MinValue),
					ExpirationDate = DataObject.getValue<DateTimeOffset>(row["ExpirationDate"], DateTimeOffset.MinValue),
					Flag01 = DataObject.getValue<bool>(row["Flag01"], false),
					LegacyNumber = DataObject.getValue<string>(row["LegacyNumber"], string.Empty),
					OperatorID = DataObject.getValue<string>(row["OperatorID"], string.Empty),
					PONumber = DataObject.getValue<string>(row["PONumber"], string.Empty),
					RequestedDeliveryDate = DataObject.getValue<DateTimeOffset>(row["RequestedDeliveryDate"], DateTimeOffset.MinValue),
					ReversalType = DataObject.getValue<string>(row["ReversalType"], string.Empty),
					ScheduledDate = DataObject.getValue<DateTimeOffset>(row["ScheduledDate"], DateTimeOffset.MinValue),
					ShipmentNumber = DataObject.getValue<string>(row["ShipmentNumber"], string.Empty),
					ShipperID = DataObject.getValue<string>(row["ShipperID"], string.Empty),
					Site = DataObject.getValue<string>(row["Site"], string.Empty),
					SupplierID = DataObject.getValue<string>(row["SupplierID"], string.Empty),
					TimeIn = DataObject.getValue<DateTimeOffset>(row["TimeIn"], DateTimeOffset.MinValue),
					TimeOut = DataObject.getValue<DateTimeOffset>(row["TimeOut"], DateTimeOffset.MinValue),
					ArmNumber = DataObject.getValue<int>(row["ArmNumber"], 0),
					AdditiveProfileID = DataObject.getValue<string>(row["AdditiveProfileID"], string.Empty),
					BatchNumber = DataObject.getValue<string>(row["BatchNumber"], string.Empty),
					Density = DataObject.getValue<double>(row["Density"], 0),
					DestinationCompartmentID = DataObject.getValue<string>(row["DestinationCompartmentID"], string.Empty),
					DestinationRegistrationID = DataObject.getValue<string>(row["DestinationRegistrationID"], string.Empty),
					InvoiceLineNumber = DataObject.getValue<string>(row["InvoiceLineNumber"], string.Empty),
					InvoiceNumber = DataObject.getValue<string>(row["InvoiceNumber"], string.Empty),
					LineNumber = DataObject.getValue<int>(row["LineNumber"], 0),
					LoadingLocationID = DataObject.getValue<string>(row["LoadingLocationID"], string.Empty),
					MeterID = DataObject.getValue<string>(row["MeterID"], string.Empty),
					MeterStart = DataObject.getValue<double>(row["MeterStart"], 0),
					MeterStartDateTime = DataObject.getValue<DateTimeOffset>(row["MeterStartDateTime"], DateTimeOffset.MinValue),
					MeterStop = DataObject.getValue<double>(row["MeterStop"], 0),
					MeterStopDateTime = DataObject.getValue<DateTimeOffset>(row["MeterStopDateTime"], DateTimeOffset.MinValue),
					PresetAmount = DataObject.getValue<double>(row["PresetAmount"], 0),
					StorageLocationID = DataObject.getValue<string>(row["StorageLocationID"], string.Empty),
					Temperature = DataObject.getValue<double>(row["Temperature"], 0),
					Vcf = DataObject.getValue<double>(row["Vcf"], 0),
					Notes = DataObject.getValue<string>(row["Notes"], string.Empty),
				};

				summary.Add(transactionSummaryRecord);
			}

			return recordCount;
		}

		#endregion
	}
}