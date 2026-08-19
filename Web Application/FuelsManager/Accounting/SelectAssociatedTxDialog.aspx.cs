namespace FuelsManager.Accounting
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Data;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

	/// <summary>
	/// Summary description for SelectAssociatedTxDialog.
	/// </summary>
	public partial class SelectAssociatedTxDialog : AccountingWebFormView
	{
		protected TransactionFilterControl filterControl;

		private AccountingSite accountingSite = null;
		private TransactionDO trans;
		protected LineItemDO lineItem;
		private string product;
		private string deliveryLocation;
		private string currencyUnit;
		private TransactionTypes transType;
		protected System.Web.UI.WebControls.Button btnRefresh;

		//Even though an associated transaction may have multiple line items, on the grid they will be represented by their 
		//single parent transaction. GridTransactions will contain one AssocTxDO per transaction.
		protected BaseCollections gridTransactions = null;

		//Contains all the line items of all transactions available to the current transaction being edited.
		//This will include : (1) transaction line items known as associated in database, but not associated to another 
		//line item belonging to the current line item's transaction.(2) line items of recently disassociated transactions 
		//that used to be associated to other line items belonging to the same transaction current line item belongs.
		private BaseCollections availableTransactions = null;

		//Contains all line items of all transactions currently associated with the transaction
		//being edited in Transaction Detail page, as stated in database. It will not
		//include associations recently established during the transaction edit.
		private BaseCollections allAssociatedTransactionsBeforeTransactionEdit = null;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			this.product = this.Request.QueryString["product"];
			this.deliveryLocation = this.Request.QueryString["deliveryLocation"];
			this.currencyUnit = this.Request.QueryString["currency"];

			this.lineItem = this.Session[TransactionDetailBase.SessionLineItemObject] as LineItemDO;
			this.transType = this.trans.TransTypeID;

			if (this.deliveryLocation != null && this.deliveryLocation != "")
			{
				Guid locationGuid = FMChannelHelper.MakeCall<IIATACodes, Guid>(x => x.GetIdentityGuid(this.security, this.deliveryLocation));

				if (locationGuid != Guid.Empty)
				{
					IATACodeClass location = FMChannelHelper.MakeCall<IIATACodes, IATACodeClass>(x => x.Get(this.security, locationGuid));
					this.deliveryLocation = location.Name;
				}
			}

			// Wire up the refresh event handler
			this.filterControl.Refresh += new EventHandler(this.filterControl_Refresh);

			if (this.IsPostBack == false)
			{
				this.Session.Remove("availableTransactions");
				this.Session.Remove("gridTransactions");
			}

			if (this.Session["availableTransactions"] == null)
			{
				this.Session["availableTransactions"] = new BaseCollections();
			}

			this.availableTransactions = this.Session["availableTransactions"] as BaseCollections;

			if (this.Session["gridTransactions"] == null)
			{
				this.Session["gridTransactions"] = new BaseCollections();
			}

			this.gridTransactions = this.Session["gridTransactions"] as BaseCollections;

			if (this.Session["allAssociatedTransactionsBeforeTransactionEdit"] == null)
			{
				//First edit of a lineitem for the current transaction.
				//Store all associated transactions so that detailed information can be provided.
				this.Session["allAssociatedTransactionsBeforeTransactionEdit"] = new BaseCollections();
				this.allAssociatedTransactionsBeforeTransactionEdit = this.Session["allAssociatedTransactionsBeforeTransactionEdit"] as BaseCollections;

				foreach (LineItemDO li in this.trans.LineItems)
				{
					foreach (AssociatedTxDO atx in li.AssociatedTransactions)
					{
						this.allAssociatedTransactionsBeforeTransactionEdit.Add(atx);
					}
				}
			}

			this.allAssociatedTransactionsBeforeTransactionEdit = this.Session["allAssociatedTransactionsBeforeTransactionEdit"] as BaseCollections;

			if (this.IsPostBack == false)
			{
				this.UpdateView();
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			this.InitializeComponent();
			base.OnInit(e);
			base.Initialize();
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Init += new EventHandler(this.SelectAssociatedTxDialog_Init);
			this.dgTransactions.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.dgTransactions_ItemDataBound);
		}
		#endregion


		private void UpdateView()
		{

			// Create and populate the request object
			AssociatedTxSR sr = new AssociatedTxSR();

			sr.RequestType = AssociatedTxSR.RequestTypes.GetAvailableTransactions;
			sr.TransID = this.trans.TransID;
			sr.Security = base.security;

			sr.CurrentSiteGuid = this.accountingSite.CurrentSiteGuid;
			sr.Product = this.product;
			sr.TransactionAliasGuid = this.trans.TransactionAliasGuid;

			bool isAdf = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsADFKey());

			// Set the project type to ADF if the application is being deployed
			// to ADF.
			if (isAdf)
			{
				sr.ProjectType = AssociatedTxSR.ProjectTypes.ADF;
				sr.TransTypeID = this.trans.TransTypeID;
			}

			if (!string.IsNullOrEmpty(this.currencyUnit) && !this.currencyUnit.ToUpper().Equals("NONE"))
			{
				var currencyDOCollectionClass = FMChannelHelper.MakeCall<ICurrencies, CurrencyDOCollectionClass>(
						x =>
						x.GetCurrencies(this.security)
					);

				foreach (CurrencyDO currency in currencyDOCollectionClass)
				{
					if (currency.UnitDisplayName.ToUpper().Equals(this.currencyUnit))
					{
						sr.CurrencyGuid = currency.IdentityGuid;
					}
				}
			}

			if (this.filterControl.Manager != null && this.filterControl.Manager != "")
			{
				sr.Manager = this.filterControl.Manager;
			}

			if (this.filterControl.Owner != null && this.filterControl.Owner != "")
			{
				sr.Owner = this.filterControl.Owner;
			}

			if (this.filterControl.Supplier != null && this.filterControl.Supplier != "")
			{
				sr.Supplier = this.filterControl.Supplier;
			}

			if (this.filterControl.PONumber != null && this.filterControl.PONumber != "")
			{
				sr.PONumber = this.filterControl.PONumber;
			}

			if (this.filterControl.ShipTo != null && this.filterControl.ShipTo != "")
			{
				sr.ShipTo = this.filterControl.ShipTo;
			}

			if (this.filterControl.BillTo != null && this.filterControl.BillTo != "")
			{
				sr.BillTo = this.filterControl.BillTo;
			}

			if (this.filterControl.DocumentNumber != null && this.filterControl.DocumentNumber != "")
			{
				sr.DocumentNumber = this.filterControl.DocumentNumber;
			}

			if (this.filterControl.Product != null && this.filterControl.Product != "")
			{
				sr.Product = this.filterControl.Product;
			}

			sr.DateFilter = this.filterControl.DateFilter;
			sr.StartDate = this.filterControl.StartDate;
			sr.EndDate = this.filterControl.EndDate;

			StringCollection assocLineItemdIDsToCurrentLineItem = new StringCollection();
			StringCollection assocLineItemdIDsToOtherLineItems = new StringCollection();


			// get the associated transaction ids and line item ids
			foreach (LineItemDO txLineItem in this.trans.LineItems)
			{
				foreach (AssociatedTxDO assocTx in txLineItem.AssociatedTransactions)
				{

					if ((this.deliveryLocation != null && this.deliveryLocation != "" &&
						this.deliveryLocation.ToUpper() != assocTx.DeliveryLocation.ToUpper() &&
						this.deliveryLocation.ToUpper() != assocTx.Site.ToUpper()) ||
						this.product.ToUpper() != assocTx.Product.ToUpper())
					{
						continue;
					}

					//These will include newly unassociated and newly associated transactions not persistent yet, and associated
					//transactions that are currently persistent.
					sr.AssociatedTransactionIDs.Add(assocTx);
					if (txLineItem.TransactionLineItemGuid == this.lineItem.TransactionLineItemGuid)
					{
						assocLineItemdIDsToCurrentLineItem.Add(assocTx.TransactionLineItemGuid.ToString());
					}
					else
					{
						assocLineItemdIDsToOtherLineItems.Add(assocTx.TransactionLineItemGuid.ToString());
					}
				}
			}


			// Retrieve the list of associated transactions

			AssociatedTxListDO txList = FMChannelHelper.MakeCall<IAssociatedTxProcessor, AssociatedTxListDO>(x => x.Process(sr));

			Guid standardTypeGuid;

			if (this.trans.TransTypeID == TransactionTypes.T21_AccountPayableInvoice)
			{
				standardTypeGuid = ListViewClass.GetGuidFromStandardType(LISTVIEW_STANDARD_TYPE.ASSOCIATED_TX);
			}
			else if (this.trans.TransTypeID == TransactionTypes.T22_AccountReceivableInvoice)
			{
				standardTypeGuid = ListViewClass.GetGuidFromStandardType(LISTVIEW_STANDARD_TYPE.RECOVERY_ASSOCIATED_TX);
			}
			else if (this.trans.TransTypeID == TransactionTypes.T9_Request)
			{
				standardTypeGuid = ListViewClass.GetGuidFromStandardType(LISTVIEW_STANDARD_TYPE.ASSOCIATED_TX);
			}
			else if (this.trans.TransTypeID == TransactionTypes.T18_SupplyOrder ||
					 this.trans.TransTypeID == TransactionTypes.T8_Receipt)
			{
				standardTypeGuid = ListViewClass.GetGuidFromStandardType(LISTVIEW_STANDARD_TYPE.SUPPLY_ORDER_ASSOCIATED_TX);
			}
			else if (this.trans.TransTypeID == TransactionTypes.T17_Order)
			{
				standardTypeGuid = ListViewClass.GetGuidFromStandardType(LISTVIEW_STANDARD_TYPE.ORDER_ASSOCIATED_TX);
			}
			else
			{
				throw new ApplicationException("Unknown Associated Transaction Type ID Encountered");
			}

			ListViewDataSet grid = new ListViewDataSet(base.security, LISTVIEW_TYPE.STANDARD, standardTypeGuid, this.accountingSite);

			grid.SetDataGrid(this.dgTransactions);
			StringCollection lineItemGuidsAdded = new StringCollection();

			foreach (AssociatedTxDO atdo in this.allAssociatedTransactionsBeforeTransactionEdit)
			{
				AssociatedTxDO atx = new AssociatedTxDO();
				atx.Associated = 0;
				atx.TransID = atdo.TransID;
				atx.InventoryDate = atdo.InventoryDate;
				atx.BillToID = atdo.BillToID;
				atx.DocumentNumber = atdo.DocumentNumber;
				atx.InventoryDateTime = atdo.InventoryDateTime;
				atx.TransactionLineItemGuid = atdo.TransactionLineItemGuid;
				atx.Manager = atdo.Manager;
				atx.Owner = atdo.Owner;
				atx.PONumber = atdo.PONumber;
				atx.ShipToID = atdo.ShipToID;
				atx.SupplierID = atdo.SupplierID;
				atx.TransactionDateTime = atdo.TransactionDateTime;
				atx.TransactionDate = atdo.TransactionDate;
				atx.TransID = atdo.TransID;
				atx.TransactionAlias = atdo.TransactionAlias;
				atx.Product = atdo.Product;
				atx.DeliveryLocation = atdo.DeliveryLocation;
				atx.Site = atdo.Site;
				atx.GrossQuantity = atdo.GrossQuantity;
				atx.GST = atdo.GST;
				atx.Excise = atdo.Excise;
				atx.Markup = atdo.Markup;
				atx.TotalValue = atdo.TotalValue;
				atx.TotalPriceWithTax = atdo.TotalPriceWithTax;
				// JS20100824 WI-17000
				atx.CurrencyGuid = atdo.CurrencyGuid;
				atx.LinkedTransactionLineItemGuid = atdo.LinkedTransactionLineItemGuid;
				atx.LineItemStatus = atdo.LineItemStatus;
				atx.ProductPrice = atdo.ProductPrice;
				// JS20100907 WI-14875
				atx.LineItemRequestedDateTime = atdo.LineItemRequestedDateTime;
				// JS20100915 WI-17454
				atx.AlternativeNetVolume = atdo.AlternativeNetVolume;

				if ((this.deliveryLocation != null && this.deliveryLocation != "" &&
					this.deliveryLocation.ToUpper() != atx.DeliveryLocation.ToUpper() &&
					this.deliveryLocation.ToUpper() != atx.Site.ToUpper()) ||
					this.product.ToUpper() != atx.Product.ToUpper())
				{
					continue;
				}

				if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsADFKey()) && this.trans.TransTypeID == TransactionTypes.T18_SupplyOrder)
				{
					this.ADFAddToFuelOrderAssoc(atx, ref lineItemGuidsAdded, ref this.availableTransactions);
				}
				else if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsADFKey()) && this.trans.TransTypeID == TransactionTypes.T8_Receipt)
				{
					this.ADFAddToReceiptAssoc(atx, ref lineItemGuidsAdded, ref this.availableTransactions);
				}
				else if (assocLineItemdIDsToOtherLineItems.Contains(atx.TransactionLineItemGuid.ToString()) == false)
				{
					if (lineItemGuidsAdded.Contains(atx.TransactionLineItemGuid.ToString()) == false)
					{
						lineItemGuidsAdded.Add(atx.TransactionLineItemGuid.ToString());
						this.availableTransactions.Add(atx);
					}
				}
			}


			if (txList.AvailableTransactions.Tables.Count > 0)
			{


				//All these transactions contain lineitems that have product and delivery location 
				//values same as the current line item.
				foreach (DataRow dr in txList.AvailableTransactions.Tables[0].Rows)
				{
					AssociatedTxDO atx = this.PopulateAssociatedTxDO(dr);
					if ((this.deliveryLocation != null && this.deliveryLocation != "" &&
						this.deliveryLocation.ToUpper() != atx.DeliveryLocation.ToUpper() &&
						this.deliveryLocation.ToUpper() != atx.Site.ToUpper()) ||
						this.product.ToUpper() != atx.Product.ToUpper())
					{
						continue;
					}

					if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsADFKey()) && this.trans.TransTypeID == TransactionTypes.T18_SupplyOrder)
					{
						this.ADFAddToFuelOrderAssoc(atx, ref lineItemGuidsAdded, ref this.availableTransactions);
					}
					else if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsADFKey()) && this.trans.TransTypeID == TransactionTypes.T8_Receipt)
					{
						this.ADFAddToReceiptAssoc(atx, ref lineItemGuidsAdded, ref this.availableTransactions);
					}
					else if (assocLineItemdIDsToOtherLineItems.Contains(atx.TransactionLineItemGuid.ToString()) == false)
					{
						if (lineItemGuidsAdded.Contains(atx.TransactionLineItemGuid.ToString()) == false)
						{
							lineItemGuidsAdded.Add(atx.TransactionLineItemGuid.ToString());
							this.availableTransactions.Add(atx);
						}
					}
				}
			}

			foreach (AssociatedTxDO atx in this.lineItem.AssociatedTransactions)
			{
				if (lineItemGuidsAdded.Contains(atx.TransactionLineItemGuid.ToString()) == false)
				{
					lineItemGuidsAdded.Add(atx.TransactionLineItemGuid.ToString());
					this.availableTransactions.Add(atx);
				}
			}

			this.gridTransactions.Clear();

			// the same line item won't be added twice
			ArrayList lineItemAdded = new ArrayList();

			//Grid needs to show unique transactions.
			foreach (AssociatedTxDO atdo in this.availableTransactions)
			{
				AssociatedTxDO atx = new AssociatedTxDO();

				// JS20100604 compare the line item to enforce the same line item cannot be associated on a separate line of the transaction
				// previously this was incorrect as it compared the transaction ID.
				if (!AlreadyAssociated(this.trans, this.lineItem.TransactionLineItemGuid, atdo.TransactionLineItemGuid) &&
					!lineItemAdded.Contains(atdo.TransactionLineItemGuid))
				{
					lineItemAdded.Add(atdo.TransactionLineItemGuid);

					atx.Associated = (assocLineItemdIDsToCurrentLineItem.Contains(atdo.TransactionLineItemGuid.ToString()) ? 1 : 0);
					atx.TransID = atdo.TransID;
					atx.InventoryDate = atdo.InventoryDate;
					atx.BillToID = atdo.BillToID;
					atx.DocumentNumber = atdo.DocumentNumber;
					atx.InventoryDateTime = atdo.InventoryDateTime;
					atx.TransactionLineItemGuid = atdo.TransactionLineItemGuid;
					atx.Manager = atdo.Manager;
					atx.Owner = atdo.Owner;
					atx.PONumber = atdo.PONumber;
					atx.ShipToID = atdo.ShipToID;
					atx.SupplierID = atdo.SupplierID;
					atx.TransactionDateTime = atdo.TransactionDateTime;
					atx.TransactionDate = atdo.TransactionDate;
					atx.TransID = atdo.TransID;
					atx.TransactionAlias = atdo.TransactionAlias;
					atx.Product = atdo.Product;
					atx.DeliveryLocation = atdo.DeliveryLocation;
					atx.Site = atdo.Site;
					atx.GrossQuantity = atdo.GrossQuantity;
					atx.GST = atdo.GST;
					atx.Excise = atdo.Excise;
					atx.Markup = atdo.Markup;
					atx.TotalValue = atdo.TotalValue;
					atx.TotalPriceWithTax = atdo.TotalPriceWithTax;
					// JS20100824 WI-17000
					atx.CurrencyGuid = atdo.CurrencyGuid;
					atx.LinkedTransactionLineItemGuid = atdo.LinkedTransactionLineItemGuid;
					atx.LineItemStatus = atdo.LineItemStatus;
					atx.ProductPrice = atdo.ProductPrice;
					// JS20100907 WI-14875
					atx.LineItemRequestedDateTime = atdo.LineItemRequestedDateTime;
					// JS20100915 WI-17454
					atx.AlternativeNetVolume = atdo.AlternativeNetVolume;

					this.gridTransactions.Add(atx);
				}

			}

			grid.BindData(this.gridTransactions, QuantityDisplay.NET, this.accountingSite.CurrentSite._VolumeDecimalPlaces, this.accountingSite.CurrentSite._MassDecimalPlaces, false);
		}

		/// <summary>
		/// Returns true if the transId is already associated with the transaction
		/// </summary>
		/// <param name="transaction"></param>
		/// <returns></returns>
		private static bool AlreadyAssociated(TransactionDO transaction, Guid a_curTransactionLineItemGuid, Guid a_TransactionLineItemGuid)
		{
			foreach (LineItemDO lineItem in transaction.LineItems)
			{
				if (lineItem.TransactionLineItemGuid == a_curTransactionLineItemGuid)
				{
					// allow disassociations
					continue;
				}

				foreach (AssociatedTxDO associatedTransaction in lineItem.AssociatedTransactions)
				{
					if (associatedTransaction.TransactionLineItemGuid == a_TransactionLineItemGuid)
					{
						return true;
					}
				}
			}

			return false;

		}


		/// <summary>
		/// Performs filtering of in-memory associations.
		/// </summary>
		/// <param name="atx"></param>
		/// <returns></returns>
		protected bool PassFilter(AssociatedTxDO atx)
		{
			if (atx.Associated == 1)
				return true;

			if (this.filterControl.Manager != null && this.filterControl.Manager != "")
			{
				if (atx.Manager != this.filterControl.Manager)
					return false;
			}

			if (this.filterControl.Owner != null && this.filterControl.Owner != "")
			{
				if (atx.Owner != this.filterControl.Owner)
					return false;

			}

			if (this.filterControl.Supplier != null && this.filterControl.Supplier != "")
			{
				if (atx.SupplierID != this.filterControl.Supplier)
					return false;

			}

			if (this.filterControl.PONumber != null && this.filterControl.PONumber != "")
			{
				if (atx.PONumber != this.filterControl.PONumber)
					return false;

			}

			if (this.filterControl.ShipTo != null && this.filterControl.ShipTo != "")
			{
				if (atx.ShipToID != this.filterControl.ShipTo)
					return false;

			}

			if (this.filterControl.BillTo != null && this.filterControl.BillTo != "")
			{
				if (atx.BillToID != this.filterControl.BillTo)
					return false;
			}

			if (this.filterControl.DocumentNumber != null && this.filterControl.DocumentNumber != "")
			{
				if (atx.DocumentNumber != this.filterControl.DocumentNumber)
					return false;
			}

			if (this.filterControl.Product != null && this.filterControl.Product != "")
			{
				if (atx.Product.ToUpper() != this.filterControl.Product.ToUpper())
					return false;
			}

			if (this.filterControl.DateFilter != AssociatedTxSR.DateFilters.None)
			{
				// Both begin and end date must be provided
				if (this.filterControl.StartDate != null && this.filterControl.EndDate != null)
				{
					DateTimeOffset startDate = this.filterControl.StartDate;
					DateTimeOffset endDate = this.filterControl.EndDate;
					if (this.filterControl.DateFilter == AssociatedTxSR.DateFilters.InventoryDate)
					{

						if (atx.InventoryDateTime < startDate || atx.InventoryDateTime > endDate)
							return false;
					}
					else
					{

						if (atx.TransactionDateTime < startDate || atx.TransactionDateTime > endDate)
							return false;
					}
				}
			}
			return true;
		}

		private AssociatedTxDO PopulateAssociatedTxDO(DataRow dr)
		{
			AssociatedTxDO txDo = new AssociatedTxDO(this.accountingSite.CurrentSite.GetDateTimeFormatInfo());
			txDo.Load(dr);

			return txDo;
		}

		protected void ADFAddToFuelOrderAssoc(AssociatedTxDO a_atx, ref StringCollection lineItemGuidsAdded, ref BaseCollections availableTransactions)
		{
			// only demands which are not cancelled can be associated
			if (a_atx.LineItemStatus == TransactionStatus.Completed ||
				a_atx.LineItemStatus == TransactionStatus.Cancelled ||
				a_atx.TransStatus == TransactionStatus.Cancelled)
			{
				return;
			}

			lineItemGuidsAdded.Add(a_atx.TransactionLineItemGuid.ToString());
			availableTransactions.Add(a_atx);
		}

		protected void ADFAddToReceiptAssoc(AssociatedTxDO a_atx, ref StringCollection lineItemGuidsAdded, ref BaseCollections availableTransactions)
		{
			// completed and cancelled demands cannot be receipted
			try
			{
				if (a_atx.LineItemStatus == TransactionStatus.Completed ||
					a_atx.LineItemStatus == TransactionStatus.Cancelled ||
					a_atx.TransStatus == TransactionStatus.Cancelled)
				{
					return;
				}
			}
			catch (Exception e)
			{
				base.ErrorHandler(e);
			}

			// the current demand is associated with a supply order that is not cancelled
			try
			{
				AssociatedTxSR sr = new AssociatedTxSR();
				sr.Security = base.security;
				sr.RequestType = AssociatedTxSR.RequestTypes.GetAssociatedParentTransactions;
				sr.TransID = a_atx.TransID;

				AssociatedTxListDO result = FMChannelHelper.MakeCall<IAssociatedTxProcessor, AssociatedTxListDO>(x => x.Process(sr));

				if (result.AssociatedTransactions.Tables.Count > 0)
				{
					foreach (DataRow dr in result.AssociatedTransactions.Tables[0].Rows)
					{
						AssociatedTxDO atx = new AssociatedTxDO(this.accountingSite.CurrentSite.GetDateTimeFormatInfo());
						atx.Load(dr);
						// add it only if associated with a fuel order line item AND the fuel order is not cancelled
						if (atx.TransTypeID == TransactionTypes.T18_SupplyOrder &&
							atx.TransStatus != TransactionStatus.Cancelled &&
							atx.LineItemStatus != TransactionStatus.Cancelled)
						{
							// resolved a defect so that the user cannot associate line items which are not associated
							TransactionSR transSr = new TransactionSR();
							transSr.Security = this.security;
							transSr.TransID = atx.TransID;

							TransactionDO atxTrans = FMChannelHelper.MakeCall<ITransactionProcessor, TransactionDO>(x => x.Process(transSr));

							foreach (LineItemDO lineItem in atxTrans.LineItems)
							{
								foreach (AssociatedTxDO x in lineItem.AssociatedTransactions)
								{
									if (x.TransactionLineItemGuid == a_atx.TransactionLineItemGuid)
									{
										lineItemGuidsAdded.Add(a_atx.TransactionLineItemGuid.ToString());
										availableTransactions.Add(a_atx);
										return;
									}
								}
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				base.ErrorHandler(e);
			}
		}

		private void InitFilter()
		{
			if (this.trans.TransTypeID == TransactionTypes.T9_Request)
			{
				// Show filter options for transactions that could be
				// associated with a request (Receipts)
				this.filterControl.ShowDocumentNumber = true;
				this.filterControl.ShowSupplier = true;
			}
			else if (this.trans.TransTypeID == TransactionTypes.T21_AccountPayableInvoice)
			{
				this.filterControl.ShowSupplier = true;
				this.filterControl.ShowPONumber = true;
				this.filterControl.ShowBillTo = true;
				this.filterControl.ShowProduct = true;
			}
			else if (this.trans.TransTypeID == TransactionTypes.T22_AccountReceivableInvoice)
			{
				this.filterControl.ShowShipTo = true;
				this.filterControl.ShowBillTo = true;
				this.filterControl.ShowDocumentNumber = true;
				this.filterControl.ShowProduct = true;
			}
			else
			{
				this.filterControl.ShowBillTo = true;
				this.filterControl.ShowDocumentNumber = true;
				this.filterControl.ShowManager = true;
				this.filterControl.ShowOwner = true;
				this.filterControl.ShowPONumber = true;
				this.filterControl.ShowShipTo = true;
				this.filterControl.ShowSupplier = true;
			}

			// now check for anything configured in filterviews which will REPLACE the above settings.
			FilterViewsCollectionClass viewCollection = FMChannelHelper.MakeCall<IFilterViews, FilterViewsCollectionClass>(
							x =>
							x.EnumerateByTransTypeID(this.security, this.trans.TransTypeID)
						);

			if (viewCollection.Count > 0)
			{
				// if there is one or more fields configured then we should overwrite the original
				this.filterControl.InitialiseFromFieldView(viewCollection);
			}

			bool isAdf = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsADFKey());

			// JS20100517 WI-14270 for ADF filter control must disabled
			if (isAdf && this.trans.TransTypeID == TransactionTypes.T21_AccountPayableInvoice)
			{
				this.filterControl.SupplierState = false;
			}

			this.filterControl.PopulateControls();

			if (this.filterControl.PONumber == null)
			{
				this.filterControl.PONumber = this.trans.PONumber;
			}

			if (this.trans.ManagerID != null && this.filterControl.Manager == null)
			{
				this.filterControl.Manager = this.trans.ManagerID;
			}

			if (this.trans.OwnerID != null && this.filterControl.Owner == null)
			{
				this.filterControl.Owner = this.trans.OwnerID;
			}

			if (this.trans.SupplierID != null && this.filterControl.Supplier == null)
			{
				if ((this.trans.TransTypeID != TransactionTypes.T18_SupplyOrder)
					&& (this.trans.TransTypeID != TransactionTypes.T21_AccountPayableInvoice)
					&& (this.trans.TransTypeID != TransactionTypes.T8_Receipt))
				{
					this.filterControl.Supplier = this.trans.SupplierID;
				}
				// JS20100604 supplier ID in association needs to remain BLANK for fuel orders
				else if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsADFKey())
					&& !this.trans.Alias.ToUpper().Equals("FUEL ORDER")
					&& !this.trans.Alias.ToUpper().Equals("RECEIPT"))
				{
					this.filterControl.Supplier = this.trans.SupplierID;
				}
			}

			if (this.trans.ShipToID != null && this.filterControl.ShipTo == null)
			{
				this.filterControl.ShipTo = this.trans.ShipToID;
			}

			if (this.trans.BillToID != null && this.filterControl.BillTo == null)
			{
				this.filterControl.BillTo = this.trans.BillToID;
			}

			if (this.filterControl.DocumentNumber == null)
			{
				this.filterControl.DocumentNumber = "";// trans.DocumentNumber;
			}

			this.product = this.Request.QueryString["product"];

			if (this.product != null && this.filterControl.Product == null)
			{
				this.filterControl.Product = this.product;
			}
		}

		private void filterControl_Refresh(object sender, EventArgs e)
		{
			StringCollection selectedTransIDs = new StringCollection();
			StringCollection unselectedTransIDs = new StringCollection();

			foreach (DataGridItem item in this.dgTransactions.Items)
			{
				CheckBox chkAssociated = (CheckBox) item.FindControl("chkSelected");
				AssociatedTxDO atx = this.gridTransactions[item.ItemIndex] as AssociatedTxDO;

				if (chkAssociated != null)
				{
					if (chkAssociated.Checked)
					{
						selectedTransIDs.Add(atx.TransID);
					}
					else
					{
						unselectedTransIDs.Add(atx.TransID);
					}
				}
			}

			this.UpdateView();

			foreach (DataGridItem item in this.dgTransactions.Items)
			{
				AssociatedTxDO atx = this.gridTransactions[item.ItemIndex] as AssociatedTxDO;

				if (selectedTransIDs.Contains(atx.TransID))
				{
					CheckBox chkAssociated = (CheckBox) item.FindControl("chkSelected");
					atx.Associated = 1;
					chkAssociated.Checked = true;
				}
				else if (unselectedTransIDs.Contains(atx.TransID))
				{
					CheckBox chkAssociated = (CheckBox) item.FindControl("chkSelected");
					atx.Associated = 0;
					chkAssociated.Checked = false;
				}

				if ((this.PassFilter(atx) == false) && (atx.Associated != 1))
				{
					item.Visible = false;
				}
				else
				{
					item.Visible = true;
				}
			}
		}

		private void SelectAssociatedTxDialog_Init(object sender, EventArgs e)
		{
			base.Initialize();

			// Get site information
			this.accountingSite = FMChannelHelper.MakeCall<IAccountingSites, AccountingSite>(
				x =>
				x.LoadSiteInfo(this.security, this.security.SiteGuid)
			);

			this.accountingSite.GetUserCompanies = false;

			// Get the transaction object from memory
			this.trans = (TransactionDO) this.Session["TransactionDetailTransaction"];
			this.InitFilter();
		}

		protected virtual void OK_Clicked(object sender, System.EventArgs e)
		{
			try
			{

				if (this.Session["associatedTransactionsBeforeEdit"] == null)
				{
					//First time editing. Store associations in case need to be restored when user cancels edit.
					BaseCollections associatedTransactionsBeforeEdit = new BaseCollections();
					this.Session["associatedTransactionsBeforeEdit"] = associatedTransactionsBeforeEdit;
					foreach (AssociatedTxDO atx in this.lineItem.AssociatedTransactions)
					{
						associatedTransactionsBeforeEdit.Add(atx);
					}

				}

				this.lineItem.AssociatedTransactions.Clear();
				StringCollection newlyAssociatedTransIds = new StringCollection();

				//
				//Match associate flag settings to ones set by user on the grid.
				foreach (DataGridItem item in this.dgTransactions.Items)
				{
					CheckBox chkAssociated = (CheckBox) item.FindControl("chkSelected");
					HtmlInputHidden hidTransID = (HtmlInputHidden) item.FindControl("hidTransID");
					int associated = chkAssociated.Checked ? 1 : 0;
					string transID = hidTransID.Value;

					if (associated == 1)
					{
						if (newlyAssociatedTransIds.Contains(transID) == false)
						{
							newlyAssociatedTransIds.Add(transID);
						}
					}

				}
				StringCollection assocLineItemGuids = new StringCollection();
				foreach (AssociatedTxDO atx in this.availableTransactions)
				{
					if ((this.deliveryLocation != null && this.deliveryLocation != "" &&
						this.deliveryLocation.ToUpper() != atx.DeliveryLocation.ToUpper() &&
						this.deliveryLocation.ToUpper() != atx.Site.ToUpper()) ||
						this.product.ToUpper() != atx.Product.ToUpper())
					{
						continue;
					}

					if (newlyAssociatedTransIds.Contains(atx.TransID) && !assocLineItemGuids.Contains(atx.TransactionLineItemGuid.ToString()))
					{
						this.lineItem.AssociatedTransactions.Add(atx);
						atx.Associated = 1;
						assocLineItemGuids.Add(atx.TransactionLineItemGuid.ToString());
					}
				}


				this.Session.Remove("availableTransactions");
				this.Session.Remove("gridTransactions");

				var js = "var Result = new Array('OK_Clicked');setWindowReturnValue(Result);closeDialogWindow();";
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "AssocticationsSet", js, true);
			}
			catch
			{
				;
			}
		}

		protected void Cancel_Clicked(object sender, System.EventArgs e)
		{
			this.Session.Remove("availableTransactions");
			this.Session.Remove("gridTransactions");

            var js = "closeDialogWindow();";
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "cancelSelect", js, true);
        }


		protected virtual void dgTransactions_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{

			if (e.Item.ItemIndex != -1)
			{
				CheckBox chkAssociated = (CheckBox) e.Item.FindControl("chkSelected");

				HtmlInputHidden hidTransID = (HtmlInputHidden) e.Item.FindControl("hidTransID");
				AssociatedTxDO txDO = this.gridTransactions[e.Item.ItemIndex] as AssociatedTxDO;

				chkAssociated.Checked = (txDO.Associated == 1 ? true : false);
				hidTransID.Value = txDO.TransID;

				if ((this.PassFilter(txDO) == false) && (txDO.Associated != 1))
				{
					e.Item.Visible = false;
				}

			}

		}

	}

}
