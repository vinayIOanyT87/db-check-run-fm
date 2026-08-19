// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatchTransactionsProcessor.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the DispatchTransactionsProcessor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.Diagnostics;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

	using FMBusinessServices.DataAccessLayer;

	/// <summary>
	/// Service class for enuemrating transactions for use in Dispatch
	/// </summary>
	public class DispatchTransactionsProcessor : IDispatchTransactionsProcessor
	{
		#region Public Methods and Operators

		/// <summary>
		/// Processes the specified dispatch service request.
		/// </summary>
		/// <param name="sr">The service request object.</param>
		/// <returns>
		/// A DispatchTransactionDO object containing the requested transactions.
		/// </returns>
		public DispatchTransactionsDO Process(DispatchTransactionsSR sr)
		{
			DispatchTransactionsDO dispatchTxDO = null;

			switch ( sr.SubCommand )
			{
				case DispatchTransactionsSR.SubCommands.None:
					dispatchTxDO = this.TransValuesProcessing( sr );
					break;
				case DispatchTransactionsSR.SubCommands.GetVersion:
					dispatchTxDO = this.GetTransVersion( sr );
					break;
				case DispatchTransactionsSR.SubCommands.GetCount:
					dispatchTxDO = this.CanCloseoutProcessBegin( sr );
					break;
				case DispatchTransactionsSR.SubCommands.GetTransferCandidates:
					dispatchTxDO = this.GetCloseoutCandidates( sr );
					break;
			}

			return dispatchTxDO;
		}

		/// <summary>
		/// Gets the trans version.
		/// </summary>
		/// <param name="sr">The service request structure describing the request..</param>
		/// <returns>A DispatchTransactionsDO object.</returns>
		private DispatchTransactionsDO GetTransVersion( DispatchTransactionsSR sr )
		{
			using (var sqlCommand = sr.GetUpdateCommand(sr.Security))
			{
				var consolidatedDA = new ConsolidatedDAClass();

				DataSet dataSet = consolidatedDA.GetDataSet(sqlCommand, sr.Security);

				var dispatchTxDO = new DispatchTransactionsDO { Transactions = dataSet };

				return dispatchTxDO;
			}
		}

		/// <summary>
		/// This method will return a data object that contains the result on whether
		/// the dispatch closeout process can begin.
		/// </summary>
		/// <param name="sr">The dispatch service request structure.</param>
		/// <returns>A DispatchTransactionsDO object.</returns>
		private DispatchTransactionsDO CanCloseoutProcessBegin( DispatchTransactionsSR sr )
		{
			var consolidatedDA = new ConsolidatedDAClass();
			using (var sqlCommand = sr.CanTransferBeginSQL())
			{
				DataSet dataSet = consolidatedDA.GetDataSet(sqlCommand, sr.Security);

				var dispatchTxDO = new DispatchTransactionsDO { Transactions = dataSet };

				return dispatchTxDO;
			}
		}

		/// <summary>
		/// This method will return a data object that contains the list of transactions
		/// that are a candidate for closeout.
		/// </summary>
		/// <param name="sr">The dispatch service request structure.</param>
		/// <returns>A DispatchTransactionsDO object.</returns>
		private DispatchTransactionsDO GetCloseoutCandidates( DispatchTransactionsSR sr )
		{
			var consolidatedDA = new ConsolidatedDAClass();
			using (var sqlCommand = sr.GetTransactionTransferCandidates())
			{
				DataSet dataSet = consolidatedDA.GetDataSet(sqlCommand, sr.Security);

				var dispatchTxDO = new DispatchTransactionsDO { Transactions = dataSet };

				return dispatchTxDO;
			}
		}

		/// <summary>
		/// This method will process the Translation Value Processing.
		/// </summary>
		/// <param name="sr">The dispatch service request structure.</param>
		/// <returns>A DispatchTransactionsDO object.</returns>
		private DispatchTransactionsDO TransValuesProcessing( DispatchTransactionsSR sr )
		{
			var consolidatedDA = new ConsolidatedDAClass();

			var sites = new SitesClass();
			var site = sites.Get(sr.Security, sr.Security.SiteGuid, false, false, false);

			using (var cmd = new SqlCommand())
			{
				sr.GetSQL(cmd, site);
				var transactions = new DispatchTransactionsDO
					                   {
						                   Transactions = consolidatedDA.GetDataSet(cmd, sr.Security)
					                   };

				this.TranslateValues(sr, transactions.Transactions, site);

				return transactions;
			}
		}

		/// <summary>
		/// Gets the latest transaction line item row version.
		/// </summary>
		/// <param name="sr">
		/// The sr.
		/// </param>
		/// <param name="topVersion">
		/// The top Version.
		/// </param>
		/// <returns>
		/// Latest transaction line item row version
		/// </returns>
		public bool GetTopLineItemVersion(DispatchTransactionsSR sr, string topVersion)
		{
			var sites = new SitesClass();
			SiteClass site = sites.Get( sr.Security, sr.Security.SiteGuid, false, false, false );

			var consolidatedDA = new ConsolidatedDAClass();
			DataSet set;

			using ( var cmd = new SqlCommand() )
			{
				sr.GetUpdateCommand( cmd, site, topVersion );

				set = consolidatedDA.GetDataSet( cmd, sr.Security );
			}

			var topVer = false;

			if ( set != null && set.Tables.Count > 0 && set.Tables[0].Rows.Count > 0 )
			{
				var rowCount = (int) set.Tables[0].Rows[0]["RowCount"];
				topVer = rowCount > 0;
			}

			return topVer;
		}

		/// <summary>
		/// Gets the specified line items.
		/// </summary>
		/// <param name="sr">The service request object</param>
		/// <returns>A DispatchTransactionDO object containing the requested line items.</returns>
		public DispatchTransactionsDO GetLineItems(DispatchTransactionsSR sr)
		{
			var sites = new SitesClass();
			var site = sites.Get(
				sr.Security,
				sr.Security.SiteGuid,
				bGetMemberSites: false,
				getSchedulesAndProcessVariables: false,
				bGetAssociatedAliases: false );

			var transactions = new DispatchTransactionsDO();
			var consolidatedDA = new ConsolidatedDAClass();

			using ( var cmd = new SqlCommand() )
			{
				sr.GetSQL(cmd, site);
				transactions.Transactions = consolidatedDA.GetDataSet( cmd, sr.Security );
			}

			this.TranslateValues( sr, transactions.Transactions, site );

			return transactions;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Translates the alias name in the specified row to the user-specific alias names in Dispatch.
		/// </summary>
		/// <param name="sr">
		/// The service request.
		/// </param>
		/// <param name="row">
		/// The row to translate.
		/// </param>
		private void TranslateAliasNameInRow(DispatchTransactionsSR sr, DataRow row)
		{
			string aliasName = DataObject.getValue(row["AliasNameActual"], string.Empty);

			foreach (DispatchTransactionsSR.DispatchTranslationPair pair in sr.Translations)
			{
				if (aliasName.Equals(pair.AccountingName))
				{
					aliasName = pair.DispatchName;
					break;
				}
			}

			row["AliasName"] = aliasName;
		}

		/// <summary>
		/// Translates values in the dataset.
		/// </summary>
		/// <param name="sr">The service request</param>
		/// <param name="dataSet">The data set to translate</param>
		/// <param name="site">The current site</param>
		private void TranslateValues(DispatchTransactionsSR sr, DataSet dataSet, SiteClass site)
		{
			DataTable table = dataSet.Tables[0];

			// TransactionStatus values
			table.Columns["LookupTransactionStatusIndex"].ColumnName = "TransactionStatusInt";
			
			// ReSharper disable AssignNullToNotNullAttribute
			var stringType = Type.GetType("System.String");
			var booleanType = Type.GetType( "System.Boolean" );
			// ReSharper restore AssignNullToNotNullAttribute
			Debug.Assert( stringType != null, "stringType != null" );
			Debug.Assert( booleanType != null, "booleanType != null" );

			table.Columns.Add("LookupTransactionStatusIndex", stringType);
			table.Columns.Add("TransactionStatusCancelled", booleanType);

			Debug.Assert(sr != null, "sr != null");
			bool translationsArePresent = sr.Translations != null && sr.Translations.Count > 0;

			// AliasName
			if (translationsArePresent)
			{
				table.Columns["AliasName"].ColumnName = "AliasNameActual";
				table.Columns.Add("AliasName");
				table.Columns["AliasName"].DataType = Type.GetType("System.String");
			}

			foreach (DataRow row in table.Rows)
			{
				// Convert the date/time values to site time for display in dispatch client
				DateTimeOffset? tempDate;
				if ((tempDate = DataObject.getOptionalDateTimeOffset(row["RequestedDateTime"])) != null)
				{
					row["RequestedDateTime"] = TimeConverter.ToSiteTimeOrDate(site, tempDate.Value);
				}

				if ((tempDate = DataObject.getOptionalDateTimeOffset(row["TransDateTime"])) != null)
				{
					row["TransDateTime"] = TimeConverter.ToSiteTimeOrDate(site, tempDate.Value);
				}

				if ((tempDate = DataObject.getOptionalDateTimeOffset(row["DispatchedDateTime"])) != null)
				{
					row["DispatchedDateTime"] = TimeConverter.ToSiteTimeOrDate(site, tempDate.Value);
				}

				if ((tempDate = DataObject.getOptionalDateTimeOffset(row["FST"])) != null)
				{
					row["FST"] = TimeConverter.ToSiteTimeOrDate(site, tempDate.Value);
				}

				if ((tempDate = DataObject.getOptionalDateTimeOffset(row["TimeEnd"])) != null)
				{
					row["TimeEnd"] = TimeConverter.ToSiteTimeOrDate(site, tempDate.Value);
				}

				if ((tempDate = DataObject.getOptionalDateTimeOffset(row["TimeIn"])) != null)
				{
					row["TimeIn"] = TimeConverter.ToSiteTimeOrDate(site, tempDate.Value);
				}

				if ((tempDate = DataObject.getOptionalDateTimeOffset(row["TimeOut"])) != null)
				{
					row["TimeOut"] = TimeConverter.ToSiteTimeOrDate(site, tempDate.Value);
				}

				int transactionStatusInt = DataObject.getValue(row["TransactionStatusInt"], 0);
				row["LookupTransactionStatusIndex"] = Enum.GetName(typeof(TransactionStatus), transactionStatusInt);

				if (translationsArePresent)
				{
					this.TranslateAliasNameInRow(sr, row);
				}

				var transTypeId = DataObject.getValue<short>(row["LookupTransTypeIndex"], 0);
				if (transTypeId == 5 || transTypeId == 6)
				{
					row["GrossQuantity"] = -DataObject.getValue(row["GrossQuantity"], 0.0);

					// row["NetQuantity"] = -((double)row["NetQuantity"]);
				}

				row["TransactionStatusCancelled"] = transactionStatusInt == (int)TransactionStatus.Cancelled;

				// RecirculationType is only valid for TransTypeID 12
				// Custom Defense change
				if (transTypeId != 12)
				{
					row["Number02"] = DBNull.Value;
				}

				// Default Response Time and Fueling Time to zero
				if (row["ResponseTime"] == DBNull.Value)
				{
					row["ResponseTime"] = 0;
				}

				if (row["FuelTime"] == DBNull.Value)
				{
					row["FuelTime"] = 0;
				}
			}
		}

		#endregion
	}
}