// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionSummaryController.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Controller for the Transaction Summary page.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Areas.AccountingArea.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Globalization;
	using System.ServiceModel;
	using System.Linq;
	using System.Web.Mvc;
	using System.Web.UI.WebControls;

	using DataTables.Mvc;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using FuelsManager.Areas.AccountingArea.ViewModels;
	using FuelsManager.Areas.Controllers;
	using FuelsManager.Areas.FieldHelpers;

	[RouteArea("AccountingArea")]
	[RoutePrefix("TransactionSummary")]
	[Route("{action}")]
	public class TransactionSummaryController : FMBaseController
	{
		#region Public Methods and Operators


		/// <summary>
		/// Show the Transaction Detail page for the Transaction with a TransID matching the ID provided
		/// </summary>
		/// <param name="id">Identifies the TransID of the transaction to display on the Transaction Detail Screen</param>
		/// <returns>A view containing the Transaction Detail page</returns>
		[HttpGet]
		[Route("TransactionDetail/{id}")]
		public ActionResult TransactionDetail(string id)
		{
			string url = string.Empty;

			try
			{
				// Create session object for TransactionDetail list of transactions.
				var detailList = new TransactionDetailList();

				var transactions = (List<TransactionSummaryClass>)this.Session["TransactionSummaryResults"];

				// Put each transaction ID into the list for Previous/Next buttons.
				for (int index = 0; index < transactions.Count; ++index)
				{
					TransactionSummaryClass tx = transactions[index];

					string transId = tx.TransID;
					detailList.TransactionIDList.Add(transId);

					if (id == transId)
					{
						// Indicate which transaction id in the list is the one to initially display.
						detailList.CurrentIndex = index;
					}
				}

				// Indicate the return URL for when the TransactionDetail Close button is clicked.
				detailList.ReturnURL =
					"../MenuBar/FMMenuBar.aspx?target=../AccountingArea/TransactionSummary/TransactionSummaryIndex";

				// Put the object into session and transfer to the TransactionDetail.
				this.Session[TransactionDetailList.TransactionDetailListKey] = detailList;

				// Read the TransactionDetail URL from the Web.config file
				string transactionDetailUrl = ConfigurationManager.AppSettings["AccountingTransactionDetailURL"];

				url = "../../../" + transactionDetailUrl + "?" + this.Security.CSRFTokenWithParamName;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}

			return this.RedirectWithPleaseWait(url);
		}

		/// <summary>
		/// This method is called by Datatables to get transactions to display in the table.
		/// </summary>
		/// <param name="requestModel">Contains parameters sent by datatables</param>
		/// <returns>JSON-formatted transactions and optionally an error message if something went wrong</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public JsonResult GetData([ModelBinder(typeof(TransactionSummaryDatatablesBinder))] TransactionSummaryDatatablesRequest requestModel)
		{
			try
			{
				var oldContext = this.Session["TransactionSummaryContext"] as TransactionSummaryFilterContext;

				var model = new TransactionSummaryViewModel
				{
					BeginDate = requestModel.BeginDate,
					EndDate = requestModel.EndDate,
					ShortDatePattern = requestModel.ShortDatePattern,
					SelectedAlias = requestModel.AliasName
				};

				var context = new TransactionSummaryFilterContext(model);
				this.Session["TransactionSummaryContext"] = context;

				// Do not retrieve transaction data if the selected alias has changed.
				// The displayed columns have changed and the page needs to be reloaded.
				if (oldContext != null && oldContext.SelectedAlias != requestModel.AliasName)
				{
					return this.Json(new DataTablesResponse(requestModel.Draw, new List<TransactionSummaryClass>(), 0, 0, string.Empty));
				}

				// Make sure the beginning and ending inventory date were provided
				if (string.IsNullOrEmpty(model.BeginDate))
				{
					throw new Exception("Beginning inventory date must be provided");
				}

				if (string.IsNullOrEmpty(model.EndDate))
				{
					throw new Exception("Ending inventory date must be provided");
				}

				// Make sure the beginning and ending inventory date are formatted correctly according to the site's date format settings
				DateTimeOffset beginDate;

				if (!DateTimeOffset.TryParseExact(
					 requestModel.BeginDate,
					 requestModel.ShortDatePattern,
					 CultureInfo.InvariantCulture,
					 DateTimeStyles.None,
					 out beginDate))
				{
					throw new Exception("Format of Beginning inventory date is invalid. Date format is " + model.ShortDatePattern);
				}

				DateTimeOffset endDate;

				if (!DateTimeOffset.TryParseExact(
					 requestModel.EndDate,
					 requestModel.ShortDatePattern,
					 CultureInfo.InvariantCulture,
					 DateTimeStyles.None,
					 out endDate))
				{
					throw new Exception("Format of Ending inventory date is invalid. Date format is " + model.ShortDatePattern);
				}

				// Make sure that the beginning inventory date is not after the ending date
				if (beginDate > endDate)
				{
					throw new Exception("Beginning date must be earlier than or equal to ending date.");
				}

				int recordCount = 0;

				List<TransactionSummaryClass> transactions = new List<TransactionSummaryClass>();

				var sortedColumns = requestModel.Columns.GetSortedColumns();

				// Get transactions matching the search criteria
				transactions = FMChannelHelper.MakeCall<ITransactionSummary, List<TransactionSummaryClass>>(
							transactionSummaryService => transactionSummaryService.Enumerate(this.Security, beginDate, endDate, requestModel.AliasName, requestModel.Search.Value, requestModel.Start, requestModel.Length, sortedColumns.ToList(), out recordCount));

				// Give each transaction the date and time formatting info. This is done so that properties in the objects can be used
				// to get the correctly formatted date + time by datatables
				transactions.ForEach(
					 transaction =>
					 {
						 transaction.AliasName = Server.HtmlEncode(transaction.AliasName);
                   transaction.DocumentNumber = Server.HtmlEncode(transaction.DocumentNumber);
                   transaction.ManagerID = Server.HtmlEncode(transaction.ManagerID);
                   transaction.OwnerID = Server.HtmlEncode(transaction.OwnerID);
						 transaction.ProductID = Server.HtmlEncode(transaction.ProductID);
                   transaction.ShipToID = Server.HtmlEncode(transaction.ShipToID);
						 transaction.ShortDatePattern = requestModel.ShortDatePattern;
						 transaction.TimePattern = requestModel.TimePattern;
                });

				this.Session["TransactionSummaryResults"] = transactions;

				return this.Json(new DataTablesResponse(requestModel.Draw, transactions, recordCount, recordCount, string.Empty));
			}
			catch (Exception ex)
			{
				return this.Json(new DataTablesResponse(requestModel.Draw, new List<TransactionSummaryClass>(), 0, 0, ex.Message));
			}
		}

		/// <summary>
		/// Main get action of the Transaction Summary page.
		/// </summary>
		/// <returns>The initial transaction summary view.</returns>
		[HttpGet]
		[Route("TransactionSummaryIndex")]
		public ActionResult TransactionSummaryIndex()
		{
			var context = this.Session["TransactionSummaryContext"] as TransactionSummaryFilterContext;
			var model = new TransactionSummaryViewModel(context);
			try
			{
				this.GetModelInfo(model);

				// If no persisted filter, set default date filter values.
				if (context == null)
				{
					model.BeginDate = model.NowText;
					model.EndDate = model.NowText;
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}

			return this.View(model);
		}

		/// <summary>
		///     Post method of the Transaction Summary page.
		/// </summary>
		/// <param name="model">The model object with bound fields populated from page.</param>
		/// <returns>The updated view after post.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult TransactionSummaryIndex(TransactionSummaryViewModel model)
		{
			try
			{
				if (this.ModelState.IsValid)
				{
					var context = new TransactionSummaryFilterContext(model);
					this.Session["TransactionSummaryContext"] = context;

					this.GetModelInfo(model);

					context.ShortDatePattern = model.ShortDatePattern;

					this.ValidateDates(model);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}

			return this.View(model);
		}

		#endregion

		#region Methods

		[NonAction]
		private void GetModelInfo(TransactionSummaryViewModel model)
		{
			SiteClass site =
				FMChannelHelper.MakeCall<ISites, SiteClass>(
					x =>
						x.Get(
							this.Security,
							this.Security.SiteGuid,
							getMemberSites: false,
							getSchedulesAndProcessVariables: false,
							bGetAssociatedAliases: false));

			var converter = new SiteTimeConverter(site);
			model.NowText = converter.Today().Date.ToString(site.ShortDatePattern, CultureInfo.InvariantCulture);

			if (string.IsNullOrEmpty(model.ShortDatePattern) == false && model.ShortDatePattern != site.ShortDatePattern)
			{
				model.BeginDate = model.NowText;
				model.EndDate = model.NowText;
			}

			model.ShortDatePattern = site.ShortDatePattern;
			model.TimePattern = site.TimePattern;
			model.VolumeDecimalPlaces = site.GetSiteDecimalPlaces(SITE_VARIABLE_TYPE.VOLUME);
			model.MassDecimalPlaces = site.GetSiteDecimalPlaces(SITE_VARIABLE_TYPE.MASS);

			var aliasNameCollectionClass = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasNameCollectionClass>(
																			x => x.EnumerateNamesOnly(this.Security, byUser: true));

			// If site group then we only want to present the transactions that can be created
			// at the site group.
			if (site.SiteGroup)
			{
				var siteGroupAliasNameCollection = new TransactionAliasNameCollectionClass();

				foreach (TransactionAliasNameClass alias in aliasNameCollectionClass)
				{
					if (alias.TransTypeID != TransactionTypes.T9_Request &&
						alias.TransTypeID != TransactionTypes.T18_SupplyOrder &&
						alias.TransTypeID != TransactionTypes.T17_Order)
					{
						continue;
					}

					siteGroupAliasNameCollection.Add(alias);
				}

				model.TransactionAliasNames = siteGroupAliasNameCollection;
			}
			else
			{
				model.TransactionAliasNames = aliasNameCollectionClass;
			}

			model.ListViewAliasColumnNames = this.GetListViewAliasColumnNames(model.SelectedAlias);

			string columnNames = string.Empty;
			foreach (var column in model.ListViewAliasColumnNames)
			{
				string dbName = column.Key;
				string displayName = column.Value;

				if (columnNames == string.Empty)
				{
					columnNames = dbName;
				}
				else
				{
					columnNames += ',' + dbName;
				}
			}

			model.ColumnDisplayNames = columnNames;

			model.AllOptionText = this.GetTranslatedText("{All}");
		}

		[NonAction]
		private List<TransactionSummaryClass> GetTransactions(TransactionSummaryViewModel model, out int recordCount)
		{
			string findText = model.FindText;

			var beginDateText = model.BeginDate;
			var endDateText = model.EndDate;

			DateTimeOffset beginDate = DateTimeOffset.Now;
			DateTimeOffset endDate = DateTimeOffset.Now;

			List<DataTablesColumn> sortedColumns = new List<DataTablesColumn>();
			int count = 0;

			List<TransactionSummaryClass> transactions =
				 FMChannelHelper.MakeCall<ITransactionSummary, List<TransactionSummaryClass>>(
					  x =>
					  {
						  ((IClientChannel)x).OperationTimeout = new TimeSpan(0, 2, 1);
						  return x.Enumerate(this.Security, beginDate, endDate, model.SelectedAlias, findText, 0, 1, sortedColumns, out count);
					  });

			recordCount = 0;
			if (transactions.Count > 0)
			{
				// Record count returned in first summary record.
				recordCount = transactions[0].RecordCount;
			}

			this.Session["TransactionSummaryResults"] = transactions;
			return transactions;
		}

		[NonAction]
		private Dictionary<string, string> GetListViewAliasColumnNames(string selectedAliasName)
		{
			var defaultColumnNames = new Dictionary<string, string>();
			defaultColumnNames["InventoryDate"] = "Inventory Date";
			defaultColumnNames["TransDateTime"] = "Transaction Date";
			defaultColumnNames["DocumentNumber"] = "Document Number";
			defaultColumnNames["AliasName"] = "Alias";
			defaultColumnNames["TransactionStatus"] = "Status";
			defaultColumnNames["OwnerID"] = "Owner";
			defaultColumnNames["ManagerID"] = "Manager";
			defaultColumnNames["ShipToID"] = "ShipTo";
			defaultColumnNames["Product"] = "Product";
			defaultColumnNames["GrossQuantity"] = "Gross";
			defaultColumnNames["NetQuantity"] = "Net";

			var visibleColumnNames = new Dictionary<string, string>();
			if (string.IsNullOrEmpty(selectedAliasName) || selectedAliasName == "{All}")
			{
				visibleColumnNames = defaultColumnNames;
			}
			else
			{
				visibleColumnNames = FMChannelHelper.MakeCall<ITransactionSummary, Dictionary<string, string>>(
																				x => x.GetListViewAssignedColumns(this.Security, selectedAliasName));

				if (visibleColumnNames.Count < 1)
				{
					visibleColumnNames = defaultColumnNames;
				}
			}

			return visibleColumnNames;
		}

		/// <summary>
		///     Validates the dates in the provided view model.
		/// </summary>
		/// <param name="model">The view model to validate.</param>
		private void ValidateDates(TransactionSummaryViewModel model)
		{
			if (string.IsNullOrEmpty(model.BeginDate))
			{
				throw new Exception("Beginning inventory date must be specified.");
			}

			if (string.IsNullOrEmpty(model.EndDate))
			{
				throw new Exception("Ending inventory date must be specified.");
			}

			DateTime beginDate;
			if (DateTime.TryParseExact(
				model.BeginDate,
				model.ShortDatePattern,
				CultureInfo.InvariantCulture,
				DateTimeStyles.NoCurrentDateDefault,
				out beginDate) == false)
			{
				throw new Exception("Format of Beginning inventory date is invalid.  Date format is " + model.ShortDatePattern);
			}

			DateTime endDate;
			if (DateTime.TryParseExact(
				model.EndDate,
				model.ShortDatePattern,
				CultureInfo.InvariantCulture,
				DateTimeStyles.NoCurrentDateDefault,
				out endDate) == false)
			{
				throw new Exception("Format of Ending inventory date is invalid.  Date format is " + model.ShortDatePattern);
			}

			if (beginDate > endDate)
			{
				throw new Exception("Beginning date must be earlier than ending date.");
			}
		}

		#endregion
	}
}