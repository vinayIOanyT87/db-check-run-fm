using FMBusinessObjects.UtilityObjects;

namespace FuelsManager.Accounting
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Data;
	using System.Diagnostics.CodeAnalysis;
	using System.Drawing;
	using System.Globalization;
	using System.Linq;
	using System.Threading;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;
	using FMBusinessObjects.ServiceRequests;

	using FMControls;

    using FMCore;

	#region Transaction List Context class
	[Serializable]
	public class TransactionListContext
	{
		#region Constants and Fields
		private string manager;
		private string month;
		private string owner;
		private string product;
		private string returnUrl;
		private string site;
		private string transactionListReturnUrl;
		#endregion

		#region Constructors and Destructors
		/// <summary>
		///    This is the default constructor for the Transaction List Context class
		/// </summary>
		public TransactionListContext()
		{
			this.site = string.Empty;
			this.manager = string.Empty;
			this.owner = string.Empty;
			this.product = string.Empty;
			this.month = string.Empty;
			this.returnUrl = string.Empty;
			this.transactionListReturnUrl = string.Empty;
		}
		#endregion

		#region Public Properties
		public string Manager
		{
			get
			{
				return this.manager;
			}
			set
			{
				this.manager = value;

				if (string.IsNullOrEmpty(this.manager))
				{
					this.manager = string.Empty;
				}
			}
		}

		public string Month
		{
			get
			{
				return this.month;
			}
			set
			{
				this.month = value;

				if (string.IsNullOrEmpty(this.month))
				{
					this.month = string.Empty;
				}
			}
		}

		public string Owner
		{
			get
			{
				return this.owner;
			}
			set
			{
				this.owner = value;

				if (string.IsNullOrEmpty(this.owner))
				{
					this.owner = string.Empty;
				}
			}
		}

		public string Product
		{
			get
			{
				return this.product;
			}
			set
			{
				this.product = value;

				if (string.IsNullOrEmpty(this.product))
				{
					this.product = string.Empty;
				}
			}
		}

		public string ReturnURL
		{
			get
			{
				return this.returnUrl;
			}
			set
			{
				this.returnUrl = value;

				if (string.IsNullOrEmpty(this.returnUrl))
				{
					this.returnUrl = string.Empty;
				}
			}
		}

		public string Site
		{
			get
			{
				return this.site;
			}
			set
			{
				this.site = value;

				if (string.IsNullOrEmpty(this.site))
				{
					this.site = string.Empty;
				}
			}
		}

		public string TransactionListReturnURL
		{
			get
			{
				return this.transactionListReturnUrl;
			}
			set
			{
				this.transactionListReturnUrl = value;

				if (string.IsNullOrEmpty(this.transactionListReturnUrl))
				{
					this.transactionListReturnUrl = string.Empty;
				}
			}
		}
		#endregion
	}
	#endregion

	#region Transaction Type Class
	/// <summary>
	///    The purpose of this class is to contain a list of transaction aliases
	///    and aggregate names to be displayed in the the transaction type
	///    dropdown list.
	/// </summary>
	[Serializable]
	public class TransactionTypeClass
	{
		#region Constants and Fields
		private LedgerAggregateColumnClass aggregateColumnObj;
		private TransactionAliasClass aliasObj;
		private bool isAggregate;
		private int itemIndex;
		#endregion

		#region Constructors and Destructors
		/// <summary>
		///    This is the default constructor for the Transaction Type Class.
		/// </summary>
		public TransactionTypeClass()
		{
			this.itemIndex = 0;
			this.isAggregate = false;
		}
		#endregion

		#region Public Properties

		public LedgerAggregateColumnClass AggregateColumn
		{
			get
			{
				return this.aggregateColumnObj;
			}
			set
			{
				this.aggregateColumnObj = value;
				this.isAggregate = true;
			}
		}

		/// <summary>
		/// Gets or sets the alias.
		/// </summary>
		public TransactionAliasClass Alias
		{
			get
			{
				return this.aliasObj;
			}

			set
			{
				this.aliasObj = value;
			}
		}

		/// <summary>
		///    This property will return either a transaction alias ID or
		///    an aggregate column ID.
		/// </summary>
		public string ID
		{
			get
			{
				return (this.aliasObj != null) ? this.aliasObj.ID : this.AggregateColumn.ID;
			}
		}

		/// <summary>
		/// This property will return True if the object is containing
		/// an aggregate view.
		/// </summary>
		public bool IsAggregate
		{
			get
			{
				return this.isAggregate;
			}
		}

		/// <summary>
		///    This property will return the item index which is stored in the dropdown
		///    value property.
		/// </summary>
		public int ItemIndex
		{
			get
			{
				return this.itemIndex;
			}

			set
			{
				this.itemIndex = value;
			}
		}
		#endregion
	}
	#endregion

	#region Transaction Type List Class
	/// <summary>
	///    The purpose of this class is to contain a list of transaction
	///    type classes.
	/// </summary>
	[Serializable]
	public class TransactionTypeListClass : List<TransactionTypeClass>
	{
	}
	#endregion

	#region Transaction List class
	/// <summary>
	///    The purpose of this class is to handle the Transaction List page functionality.
	/// </summary>
	public partial class TransactionList : AccountingWebFormView, IEntityDiscovery
	{
		#region Constants and Fields
		/// <summary>
		/// The current site.
		/// </summary>
		protected SiteClass CurrentSite;
		#endregion

		#region Explicit Interface Properties
		/// <summary>
		/// Gets a value indicating whether entity assignable.
		/// </summary>
		bool IEntityDiscovery.EntityAssignable
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Gets the entity engine type.
		/// </summary>
		Type IEntityDiscovery.EntityEngineType
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Gets the entity type.
		/// </summary>
		ENTITY_TYPE IEntityDiscovery.EntityType
		{
			get
			{
				return ENTITY_TYPE.TRANSACTION;
			}
		}

		private TransactionAliasNameCollectionClass AliasNames { get; set; }
		#endregion

		#region Explicit Interface Methods
		/// <summary>
		/// The enumerate entity maps.
		/// </summary>
		/// <param name="inSecurity">
		/// The security.
		/// </param>
		/// <param name="inType">
		/// The type.
		/// </param>
		/// <returns>
		/// The <see cref="EntityToSiteMapCollectionClass"/>.
		/// </returns>
		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(SecurityClass inSecurity, ENTITY_ASSIGNMENT_TYPE inType)
		{
			var entityToSiteMapCollection = new EntityToSiteMapCollectionClass();
			return entityToSiteMapCollection;
		}

		/// <summary>
		/// The get identity GUID.
		/// </summary>
		/// <param name="inSecurity">
		/// The in security.
		/// </param>
		/// <param name="id">
		/// The ID.
		/// </param>
		/// <returns>
		/// The <see cref="Guid"/>.
		/// </returns>
		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass inSecurity, string id)
		{
			return Guid.Empty;
		}

		/// <summary>
		/// The set site GUID.
		/// </summary>
		/// <param name="inSecurity">
		/// The in security.
		/// </param>
		/// <param name="guid">
		/// The GUID.
		/// </param>
		/// <param name="siteGuid">
		/// The site GUID.
		/// </param>
		void IEntityDiscovery.SetSiteGuid(SecurityClass inSecurity, Guid guid, Guid siteGuid)
		{
		}
		#endregion

		#region Methods
		/// <summary>
		/// The on initialization.
		/// </summary>
		/// <param name="e">
		/// The event arguments.
		/// </param>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed. Suppression is OK here.")]
		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// The page initialization.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The event arguments.
		/// </param>
		protected void Page_Init(object sender, EventArgs e)
		{
			this.security = (SecurityClass)this.Session["Security"];

			if (this.IsPostBack)
			{
				this.AliasNames = (TransactionAliasNameCollectionClass) this.Session["TransactionAliasListAliasNames"];
			}
			else
			{
				this.AliasNames =
					FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasNameCollectionClass>(
						x => x.EnumerateNamesOnly(this.security, byUser: true));

				this.Session["TransactionAliasListAliasNames"] = this.AliasNames;
			}

			this.CurrentSite = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(
																			this.security,
																			this.security.SiteGuid,
																			getMemberSites: false,
																			getSchedulesAndProcessVariables: false,
																			bGetAssociatedAliases: true)
																	);
			this.Initialize( );

			if (this.Page.IsPostBack)
			{
				this.InitializeTransactionDataGrid();
			}
		}

		/// <summary>
		/// The page_ load.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				// If the Security is null, then remove all objects from the 
				// session and display the accounting error page.
				if (this.Session["Security"] == null)
				{
					this.Session.RemoveAll();
					this.DisplayErrorPage();
					return;
				}

				if (this.Page.IsPostBack == false)
				{
					if (string.IsNullOrEmpty(Request.GetQueryOrFormValue("TrackReturn")))
					{
						Session.Remove("MovementCallStack");
					}
					else if (Request.GetQueryOrFormValue("TrackReturn") == "true")
					{
						var callStack = Session["MovementCallStack"] as Stack;
						if (callStack != null)
						{
							if (callStack.Count == 0 ||
								string.IsNullOrEmpty(callStack.Peek() as string) ||
								callStack.Peek() as string != Request.RawUrl)
							{

								callStack.Push(Request.RawUrl);
							}
						}
					}

					string column = this.Page.Server.UrlDecode(this.Request.GetQueryOrFormValue("Column"));

					if (string.IsNullOrEmpty(column))
					{
						column = this.Request.QueryString["Column"];
						column = column.Trim();
					}

					string row = this.Request.GetQueryOrFormValue("Row");
					var transactionListContext = (TransactionListContext)this.Session["TransactionListContext"];
					if (transactionListContext == null)
					{
						throw new Exception("Invalid TransactionListContext.");
					}

					if (this.Session["TransactionList.SortExpression"] == null
						|| this.Request.UrlReferrer == null
						|| System.IO.Path.GetFileName(this.Request.UrlReferrer.LocalPath).Equals("Ledger.aspx"))
					{
						//this sets the sorting to chronological with related transactions (Reversed, updates) grouped together
						this.Session["TransactionList.SortExpression"] = "AliasName";
						this.Session["TransactionList.SortDirection"] = "ASC";
					}

					DateTimeOffset dateValue =
						DateTimeOffset.Parse(transactionListContext.Month, this.CurrentSite.GetDateTimeFormatInfo())
									  .AddDays(Convert.ToDouble(row, CultureInfo.InvariantCulture));
					this.DateValueLabel.Text = dateValue.ToString("d", this.CurrentSite.GetDateTimeFormatInfo());
					this.SiteValueLabel.Text = transactionListContext.Site;
					this.ManagerValueLabel.Text = transactionListContext.Manager;
					this.OwnerValueLabel.Text = transactionListContext.Owner;
					this.ProductValueLabel.Text = transactionListContext.Product;

					this.PopulateTransactionTypeDropDownList(column);

					if (this.Session["TransactionList.CurrentPageIndex"] != null)
					{
						this.TransactionDataGrid.CurrentPageIndex = (int)this.Session["TransactionList.CurrentPageIndex"];
					}

					ProductClass product =
						FMChannelHelper.MakeCall<IProducts, ProductClass>(x => x.GetByID(this.security, this.ProductValueLabel.Text));
					this.Session["TransactionList.Product"] = product;

					this.ApplyDictionary();
					this.RefreshButtonCommand(null, null);

					this.ChangeInventoryDateColumnLabel();

					var displayDateTypeControls = this.IsBsme && this.security.HasRight(RIGHT.ACCESS_RECONCILIATION_VIEWS);
					this.DisplayDateTypeControls(displayDateTypeControls);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// The transaction type drop down list selected index changed.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void TransactionTypeDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			this.TransactionDataGrid.CurrentPageIndex = 0;
		}

		private void ChangeInventoryDateColumnLabel()
		{
			var columns = this.TransactionDataGrid.Columns;
			if (columns.Count <= 0)
			{
				return;
			}
			var translatedText = GetTranslatedText(BsmeLedgerDateType.InventoryDateType);
			var dataGridColumns = columns.Cast<DataGridColumn>().ToList();
			var inventoryDateColumn = dataGridColumns.FirstOrDefault(c => c.HeaderText == translatedText);
			if (inventoryDateColumn != null)
			{
				var displayText = BsmeLedgerDateType.GetDisplayText(Session[PageSessionKeyConstants.LEDGER_DATE_TYPE_SELECTION]);
				inventoryDateColumn.HeaderText = displayText;
			}
		}

		private void DisplayDateTypeControls(bool visible)
		{
			var displayText = BsmeLedgerDateType.GetDisplayText(Session[PageSessionKeyConstants.LEDGER_DATE_TYPE_SELECTION]);
			this.DateTypeValueLabel.Text = displayText;
			this.DateTypeLabel.Visible = visible;
			this.DateTypeValueLabel.Visible = visible;
		}

		private void ExportData(string dataFormat)
		{
			if (this.Session["TransactionList.ExportDataSet"] == null)
			{
				return;
			}

			var dataSet = this.Session["TransactionList.ExportDataSet"] as DataSet;
			const string ReportName = "Accounting Transaction List";

			var exportHelper = new DataTableExportHelper(dataSet);
			exportHelper.ExportData(dataFormat, ReportName);
		}

		/// <summary>
		/// The add button command.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		private void AddButtonCommand(object sender, CommandEventArgs e)
		{
			// Create session object for TransactionDetail list of transactions.
			// Indicate the return URL for when the TransactionDetail Close button is clicked.
			var detailList = new TransactionDetailList
			                 {
				                 ReturnURL =
					                 "../Accounting/TransactionList.aspx?Row="
					                 + this.Request.GetQueryOrFormValue("Row") + "&Column="
					                 + this.Request.GetQueryOrFormValue("Column")
			                 };

			// Put the object into session and transfer to the TransactionDetail.
			this.Session[TransactionDetailList.TransactionDetailListKey] = detailList;

			// Escape the alias name for any URL special characters (i.e. & ' / ? ! # $ * + , : ; = @ [ ])
			string aliasName = Uri.EscapeDataString(this.TransactionTypeDropDownList.SelectedItem.Text);

			// Read the TransactionDetail URL from the Web.config file (06-Jul-2009 IGO)
			string transactionDetailUrl = ConfigurationManager.AppSettings["AccountingTransactionDetailURL"];
			string redirect = "../" + transactionDetailUrl + "?" + TransactionDetailBase.ModeKey + "=ADD&TransAlias="
							   + aliasName;

			if (this.ManagerValueLabel.Text != string.Empty)
			{
				redirect += "&Manager=" + this.ManagerValueLabel.Text;
			}

			if (this.OwnerValueLabel.Text != string.Empty)
			{
				redirect += "&Owner=" + this.OwnerValueLabel.Text;
			}

			if (this.ProductValueLabel.Text != string.Empty)
			{
				redirect += "&Product=" + this.ProductValueLabel.Text;
			}

			if (this.DateValueLabel.Text != string.Empty)
			{
				redirect += "&InventoryDate=" + this.DateValueLabel.Text;
			}

			this.Redirect(redirect);
		}

		/// <summary>
		///    This method will apply the data dictionary for the items that are not FMControl
		///    objects.
		/// </summary>
		private void ApplyDictionary()
		{
			string newName = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
				x => x.Get(this.CurrentSite.SiteGuid, this.CloseButton.Text));

			this.CloseButton.Text = newName;

			newName = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
									x => x.Get(this.CurrentSite.SiteGuid, this.RefreshButton.Text));

			this.RefreshButton.Text = newName;

			newName = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
									x => x.Get(this.CurrentSite.SiteGuid, this.Label2.Text.Trim()));

			this.Label2.Text = newName;

			// Data dictionary the Edit column header name. All other columns are dynamically create and
			// dictionaried in the InitializeTransactionDataGrid method.
			DataGridColumnCollection columns = this.TransactionDataGrid.Columns;
			newName = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
								x => x.Get(this.CurrentSite.SiteGuid, columns[0].HeaderText));

			columns[0].HeaderText = newName;
		}

		/// <summary>
		/// The close button command.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		private void CloseButtonCommand(object sender, CommandEventArgs e)
		{
			this.Session.Remove("TransactionList.DataSet");
			var transactionListContext = (TransactionListContext)this.Session["TransactionListContext"];

			if (transactionListContext.Equals(null) || transactionListContext.TransactionListReturnURL.Equals(string.Empty))
			{
				this.Redirect(ResolveUrl("~/FMWebApp/FuelsManagerForm.aspx"));
			}
			else
			{
				var returnAddresses = Session["MovementCallStack"] as Stack;

				if (returnAddresses != null && returnAddresses.Count > 0)
				{
					returnAddresses.Pop();
					var r = returnAddresses.Pop() as string;
					string returnToPage = "Ledger.aspx";

					if (!string.IsNullOrEmpty(r))
					{
						returnToPage = r;
					}

					this.Redirect(returnToPage);
					Context.ApplicationInstance.CompleteRequest();
				}
				else
				{
					this.Redirect(transactionListContext.TransactionListReturnURL);
				}
			}
		}

		/// <summary>
		/// This method will enable or disable the Add buttons based the the user's rights and
		/// site group.
		/// </summary>
		/// <param name="column">
		/// The column.
		/// </param>
		private void EnableAddButton(string column)
		{
			// Check rules for disabling Add buttons.
			if (this.security.HasModifyTransactionRightByAliasName(column))
			{
				this.AddButton.Enabled = true;
				this.AddButton1.Enabled = true;
			}
			else
			{
				this.AddButton.Enabled = false;
				this.AddButton1.Enabled = false;
			}

			TransactionAliasNameClass transactionAlias = this.AliasNames.Find(name => name.AliasName == column);

			if (transactionAlias == null)
			{
				throw new ApplicationException("Transaction alias not found for enable processing.");
			}
			
			// Get Site Group information.
			bool correctType = false;

			if (this.CurrentSite.SiteGroup)
			{
				// These are the only transaction types are allowable for a site group.
				switch (transactionAlias.TransTypeID)
				{
					case TransactionTypes.T9_Request:
					case TransactionTypes.T18_SupplyOrder:
					case TransactionTypes.T17_Order:
					case TransactionTypes.T21_AccountPayableInvoice:
					case TransactionTypes.T22_AccountReceivableInvoice:
						correctType = true;
						break;
				}
			}
			else
			{
				correctType = true;
			}

			const ENTITY_TYPE AliasEntityType = ENTITY_TYPE.TRANSACTION_ALIAS;

			bool isAssigned =
				FMChannelHelper.MakeCall<IEntityToSiteMaps, bool>(
					x => x.IsAssigned(this.security, AliasEntityType, this.security.SiteGuid, transactionAlias.IdentityGuid));

			this.AddButton.Enabled = this.AddButton.Enabled && isAssigned && correctType;
			this.AddButton1.Enabled = this.AddButton1.Enabled && isAssigned && correctType;
		}

		/// <summary>
		/// The get tool tip.
		/// </summary>
		/// <param name="index">
		/// The index.
		/// </param>
		/// <param name="view">
		/// The view.
		/// </param>
		/// <param name="role">
		/// The role.
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		private string GetToolTip(int index, DataView view, COMPANY_ROLE role)
		{
			string companyRole = null;
			string toolTip = string.Empty;

			switch (role)
			{
				case COMPANY_ROLE.MANAGER:
					companyRole = "Manager";
					break;
				case COMPANY_ROLE.OWNER:
					companyRole = "Owner";
					break;
				case COMPANY_ROLE.SHIPPER:
					companyRole = "Shipper";
					break;
				case COMPANY_ROLE.CUSTOMER_BILLTO:
					companyRole = "BillTo";
					break;
				case COMPANY_ROLE.CUSTOMER_SHIPTO:
					companyRole = "ShipTo";
					break;
				case COMPANY_ROLE.CARRIER:
					companyRole = "Carrier";
					break;
				case COMPANY_ROLE.SUPPLIER:
					companyRole = "Supplier";
					break;
			}

			var name = view[index][companyRole + "Name"] as string;
			if (!string.IsNullOrEmpty(name))
			{
				toolTip += name;
			}

			var address = view[index][companyRole + "Address"] as string;
			if (!string.IsNullOrEmpty(address))
			{
				toolTip += ", " + address;
			}

			var city = view[index][companyRole + "City"] as string;
			if (!string.IsNullOrEmpty(city))
			{
				toolTip += ", " + city;
			}

			var state = view[index][companyRole + "State"] as string;
			if (!string.IsNullOrEmpty(state))
			{
				toolTip += ", " + state;
			}

			return toolTip;
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.TransactionDataGrid.UseDataDictionary = false;
			this.RefreshButton.Command					+= this.RefreshButtonCommand;
			this.CloseButton.Command					+= this.CloseButtonCommand;
			this.AddButton.Command						+= this.AddButtonCommand;
			this.AddButton1.Command						+= this.AddButtonCommand;
			this.TransactionDataGrid.ItemCreated		+= this.TransactionDataGridItemCreated;
			this.TransactionDataGrid.PageIndexChanged	+= this.TransactionDataGridPageIndexChanged;
			this.TransactionDataGrid.EditCommand		+= this.TransactionDataGridEditCommand;
			this.TransactionDataGrid.SortCommand		+= this.TransactionDataGridSortCommand;
			this.TransactionDataGrid.ItemDataBound		+= this.TransactionDataGridItemDataBound;
		}

		/// <summary>
		///    This method builds the transaction list grid and populates the grid with
		///    data.
		/// </summary>
		private void InitializeTransactionDataGrid()
		{
			// Remove all but the first column which is Edit
			while (this.TransactionDataGrid.Columns.Count > 1)
			{
				this.TransactionDataGrid.Columns.RemoveAt(1);
			}

			this.security = (SecurityClass)this.Session["Security"];
			this.Security = this.security; // the data dictionary (this.GetTranslatedText()) uses the upper case Security field.

			TransactionTypeClass transType;

			bool isAggregate;

			if (this.Page.Session[PageSessionKeyConstants.TRANSACTION_LIST_PAGE_TRANS_TYPE_INDEX] != null)
			{
				var selectedTransTypeIndex = (int)this.Page.Session[PageSessionKeyConstants.TRANSACTION_LIST_PAGE_TRANS_TYPE_INDEX];

				if (this.Page.Session[PageSessionKeyConstants.TRANSACTION_LIST_PAGE_TRANS_TYPE_LIST] != null)
				{
					var transTypeList = this.Page.Session[PageSessionKeyConstants.TRANSACTION_LIST_PAGE_TRANS_TYPE_LIST] as TransactionTypeListClass;

					if (transTypeList != null)
					{
						transType = transTypeList[selectedTransTypeIndex];
						isAggregate = transType.IsAggregate;
					}
					else
					{
						throw new Exception("Transaction Type List is null.");
					}
				}
				else
				{
					// No alias/aggregate transaction type list.
					return;
				}
			}
			else
			{
				// No selected index
				return;
			}

			var transactionAliasCollection = new List<TransactionAliasClass>();
			ListViewClass listView;
			Guid aliasGuid;
			Guid listViewGuid;

			// Get either the transaction list type view or aggregate type view.
			if (isAggregate == false)
			{
				aliasGuid = transType.Alias.MasterRecordGuid;
				listViewGuid =
					FMChannelHelper.MakeCall<IListViews, Guid>(
						x => x.GetIdentityGuid(this.security, LISTVIEW_TYPE.TRANSACTION_LIST, aliasGuid));

				transactionAliasCollection.Add(
					FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(
						x => x.Get(this.security, transType.Alias.MasterRecordGuid, false)));

				// Return if there are no views.
				if (listViewGuid == Guid.Empty)
				{
					return;
				}

				listView =
					FMChannelHelper.MakeCall<IListViews, ListViewClass>(
						x => x.Get(this.security, LISTVIEW_TYPE.TRANSACTION_LIST, listViewGuid));
			}
			else
			{
				aliasGuid = transType.AggregateColumn.IdentityGuid;
				listViewGuid =
					FMChannelHelper.MakeCall<IListViews, Guid>(
						x => x.GetIdentityGuid(this.security, LISTVIEW_TYPE.AGGREGATE, aliasGuid));

				// Return if there are no views.
				if (listViewGuid == Guid.Empty)
				{
					return;
				}

				listView =
					FMChannelHelper.MakeCall<IListViews, ListViewClass>(
						x => x.Get(this.security, LISTVIEW_TYPE.AGGREGATE, listViewGuid));

				LedgerAggregateColumnClass aggregateColumn =
					FMChannelHelper.MakeCall<ILedgerAggregateColumns, LedgerAggregateColumnClass>(
						x => x.GetByColumnGuid(this.security, listView.TypeGuid));

				FMChannelHelper.MakeCall<ITransactionAliases>(
					transactionAliases =>
						{
							foreach (LedgerAggregateColumnMapClass columnMap in aggregateColumn.Aliases)
							{
								transactionAliasCollection.Add(transactionAliases.Get(this.security, columnMap.TransactionAliasGuid, byUser: false));
							}
						});
			}

			bool transIdPresent = false;

			foreach (ListViewFieldClass listViewField in listView.ListViewFieldCollection)
			{
				// Stip out "From" and "To" from conjoined transaction fields.
				if ((listViewField.DataPath.StartsWith("From") || listViewField.DataPath.StartsWith("To"))
				    && (listViewField.DataPath.Contains("ManagerID") || listViewField.DataPath.Contains("OwnerID")
				        || listViewField.DataPath.Contains("ShipperID") || listViewField.DataPath.Contains("BillToID")
				        || listViewField.DataPath.Contains("ShipToID") || listViewField.DataPath.Contains("SupplierID")
				        || listViewField.DataPath.Contains("CarrierID") || listViewField.DataPath.Contains("Product")
				        || listViewField.DataPath.Contains("StorageLocationID")))
				{
					string conjoinedDataPath;

					if (listViewField.DataPath.StartsWith("From"))
					{
						conjoinedDataPath = "To" + listViewField.DataPath.Substring(4);
					}
					else
					{
						conjoinedDataPath = "From" + listViewField.DataPath.Substring(2);
					}

					if (listViewField.DataPath.StartsWith("From"))
					{
						listViewField.DataPath = listViewField.DataPath.Substring(4);
					}
					else
					{
						listViewField.DataPath = listViewField.DataPath.Substring(2);
					}

					foreach (TransactionAliasClass transactionAlias in transactionAliasCollection)
					{
						FieldClass[] fields;

						if ((listViewField.DataPath.Contains("Product") || listViewField.DataPath.Contains("StorageLocationID"))
						    && transactionAlias.MultipleLineItems)
						{
							fields = transactionAlias.DisplayOrder(TRANSACTION_SECTION_TYPE.LINE_ITEMS);
						}
						else
						{
							fields = transactionAlias.DisplayOrder(TRANSACTION_SECTION_TYPE.BODY);
						}

						foreach (FieldClass field in fields)
						{
							if (field.DbName == conjoinedDataPath)
							{
								if (conjoinedDataPath.StartsWith("From"))
								{
									listViewField.ID = field.DisplayName + "/" + listViewField.ID;
								}
								else
								{
									listViewField.ID = listViewField.ID + "/" + field.DisplayName;
								}
								break;
							}
						}
						break;
					}
				}

				if (listViewField.DataPath == "ManagerID" || listViewField.DataPath == "OwnerID"
				    || listViewField.DataPath == "ShipperID" || listViewField.DataPath == "BillToID"
				    || listViewField.DataPath == "ShipToID" || listViewField.DataPath == "SupplierID"
				    || listViewField.DataPath == "CarrierID")
				{
					var column = new TemplateColumn
						             {
							             HeaderText = this.GetTranslatedText(listViewField.ID),
							             SortExpression = listViewField.DataPath,
							             ItemTemplate = new CompanyLabel(listViewField.DataPath)
						             };

					this.TransactionDataGrid.Columns.Add(column);
				}
				else
				{
					// Do not add EBS Post Date column if date type selection is by EBS Post Date since the column will be added subsequently
					var dateType = (BsmeLedgerDateType.DateProcessTypes)Convert.ToInt32(this.Session[PageSessionKeyConstants.LEDGER_DATE_TYPE_SELECTION]);
					if (dateType == BsmeLedgerDateType.DateProcessTypes.ByEbsPostDate)
					{
						if (listViewField.ID == "EBS Post Date")
						{
							continue;
						}
					}

					var column = new BoundColumn { HeaderText = this.GetTranslatedText(listViewField.ID) };

					string dataPath = listViewField.DataPath;

					// JS20100517 WI-14232 & WI-14266 allows custom fields to display
					if (listViewField.DataPath.Equals("DeliveryLocationLabel"))
					{
						dataPath = "DeliveryLocation";
					}
					else if (listViewField.DataPath.Equals("ADFTransactionDateTime"))
					{
						dataPath = "TransDateTime";
					}
					else if (listViewField.DataPath.Equals("BaseCost"))
					{
						dataPath = "Number01";
					}
					else if (listViewField.DataPath.Equals("InvoiceQuery"))
					{
						dataPath = "Number04";
					}
					else if (listViewField.DataPath.Equals("SelectedQuality"))
					{
						dataPath = "Quality";
					}
					else if (listViewField.DataPath.Equals("BulkPaymentNumber") || listViewField.DataPath.Equals("ParentDocumentNumber")
					         || listViewField.DataPath.Equals("ParentUserData03") || listViewField.DataPath.Equals("ParentReceiptNumber")
					         || listViewField.DataPath.Equals("ParentFuelOrderNumber"))
					{
						// JS20100517 limitation cannot be done without some extra work, since outside of CCP-042
						// I'll treat them the same as virtual fields (skip)
						continue;
					}

					column.DataField = column.SortExpression = dataPath;

					if ((column.DataField == "GrossQuantity") || (column.DataField == "NetQuantity")
					    || (column.DataField == "LineFill") || (column.DataField == "BottomVolume")
					    || (column.DataField == "NetCapacity") || (column.DataField == "ReceiptVariance")
					    || (column.DataField == "LoadRackVariance"))
					{
						var decimalFormat = new String('0', this.CurrentSite._VolumeDecimalPlaces);

						ProductClass product = this.Session["TransactionList.Product"] as ProductClass;
						if (product != null)
						{
							if (product.ProductType == ProductType.AdditiveProduct)
							{
								decimalFormat = new String('0', this.CurrentSite._AdditiveVolumeDecimalPlaces);
							}
							else
							{
								if (product.VolumeUnits == 0)
								{
									decimalFormat = new String('0', this.CurrentSite._VolumeDecimalPlaces);
								}
								else
								{
									decimalFormat = new String('0', product.VolumeDecimalPlaces);
								}
							}
						}

						column.DataFormatString = string.IsNullOrEmpty(decimalFormat)
														? "{0:#,0;(#,0)}" : "{0:#,0." + decimalFormat + ";(#,0." + decimalFormat + ")}";
					}
					else if (column.DataField == "Temperature" || column.DataField == "FreezePoint")
					{
						var decimalFormat = new String('0', this.CurrentSite._TemperatureDecimalPlaces);
						column.DataFormatString = string.IsNullOrEmpty(decimalFormat)
														? "{0:#,0;(#,0)}" : "{0:#,0." + decimalFormat + ";(#,0." + decimalFormat + ")}";

					}
					else if (column.DataField == "Density")
					{
						var decimalFormat = new String('0', this.CurrentSite._DensityDecimalPlaces);
						column.DataFormatString = string.IsNullOrEmpty(decimalFormat)
														? "{0:#,0;(#,0)}" : "{0:#,0." + decimalFormat + ";(#,0." + decimalFormat + ")}";

					}
					else if (column.DataField == "DifferentialPressure")
					{
						var decimalFormat = new String('0', this.CurrentSite._PressureDecimalPlaces);
						column.DataFormatString = string.IsNullOrEmpty(decimalFormat)
														? "{0:#,0;(#,0)}" : "{0:#,0." + decimalFormat + ";(#,0." + decimalFormat + ")}";

					}
					else if (column.DataField == "InventoryDate")
					{
						column.DataFormatString = "{0:" + this.CurrentSite.GetDateTimeFormatInfo().ShortDatePattern + "}";
					}

					this.TransactionDataGrid.Columns.Add(column);

					if (column.DataField == "TransID")
					{
						transIdPresent = true;
					}
				}
			}

			if (transIdPresent == false)
			{
				var column = new BoundColumn { HeaderText = "ID", DataField = "TransID", Visible = false };
				this.TransactionDataGrid.Columns.Add(column);
			}
		}

		/// <summary>
		/// This method will return True if the alias meets all the user right security. True
		/// means we can add the alias to the list.
		/// </summary>
		/// <param name="alias">The transaction alias to evaluate.</param>
		/// <returns>True if it is ok to add the alias.</returns>
		private bool OkToAddAlias( TransactionAliasClass alias )
		{
			var aliasName = new TransactionAliasNameClass { ID = alias.ID, TransTypeID = alias.TransTypeID };
			return this.OkToAddAlias( aliasName );
		}

		/// <summary>
		/// This method will return True if the alias meets all the user right security. True
		///    means we can add the alias to the list.
		/// </summary>
		/// <param name="alias">
		/// The alias name class to evaluate.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		private bool OkToAddAlias( TransactionAliasNameClass alias )
		{
			// Exclude Payment and Recovery from the drop down list if modify and view security rights are missing.
			if (alias.TransTypeID == TransactionTypes.T22_AccountReceivableInvoice)
			{
				if (!this.security.HasViewTransactionRightByAliasName(alias.ID))
				{
					return false;
				}
			}
			else if (alias.TransTypeID == TransactionTypes.T21_AccountPayableInvoice)
			{
				if (!this.security.HasViewTransactionRightByAliasName(alias.ID))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// This method will populate the transaction alias dropdown list and select the item
		/// that matches the incoming alias.
		/// </summary>
		/// <param name="inColumn">The column name.</param>
		private void PopulateTransactionTypeDropDownList(string inColumn)
		{
			int itemCount = 0;
			int defaultSelectValue = 0;
			this.security = (SecurityClass)this.Session["Security"];

			var transactionTypeList = new TransactionTypeListClass();

			var transactionAliasCollection =
				FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasCollectionClass>(x => x.Enumerate(this.security));

			foreach (TransactionAliasClass transactionAlias in transactionAliasCollection)
			{
				if (this.OkToAddAlias(transactionAlias) == false)
				{
					continue;
				}

				var transactionType = new TransactionTypeClass { Alias = transactionAlias };

				transactionTypeList.Add(transactionType);
			}

			LedgerAggregateColumnCollectionClass columnCollection =
				FMChannelHelper.MakeCall<ILedgerAggregateColumns, LedgerAggregateColumnCollectionClass>(
					x => x.Enumerate(this.security));

			var aliasInfo =
				FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasNameCollectionClass>(
					x => x.EnumerateNamesOnly(this.security, byUser: false));

			foreach (LedgerAggregateColumnClass column in columnCollection)
			{
				bool isOkToAdd = true;

				LedgerAggregateColumnClass column1 = column;
				var columnMaps =
					FMChannelHelper.MakeCall<ILedgerAggregateColumnMaps, LedgerAggregateColumnMapCollectionClass>(
						x => x.Enumerate(this.security, column1.IdentityGuid));

				// Add the aliases that comprise the aggregate to the class.
				column.Aliases = columnMaps;

				// Check each alias in the column definition
				FMChannelHelper.MakeCall<ITransactionAliases>(
					transactionAliases =>
						{
							foreach (LedgerAggregateColumnMapClass aliasMap in columnMaps)
							{
								var alias = aliasInfo.Find( x => x.IdentityGuid == aliasMap.TransactionAliasGuid );

								// Don't add this aggregate if any of the associated aliases are not ok to add
								if (this.OkToAddAlias(alias) == false)
								{
									isOkToAdd = false;
									break;
								}
							}
						});

				if (isOkToAdd)
				{
					var transactionType = new TransactionTypeClass { AggregateColumn = column };

					transactionTypeList.Add(transactionType);
				}
			}

			// Sort the Transaction Type dropdown list based on the Alias name.
			transactionTypeList.Sort(
				(transTypeClass1, transTypeClass2) => Comparer<string>.Default.Compare(transTypeClass1.ID, transTypeClass2.ID));

			// Set the transaction type class item index based on the entry in the list.
			// Also set the default selected item based on the column passed in.
			foreach (TransactionTypeClass transTypeClass in transactionTypeList)
			{
				transTypeClass.ItemIndex = itemCount;

				// Identify the transaction type item to select in the list.
				if (transTypeClass.ID.ToUpper().Equals(inColumn.ToUpper()))
				{
					defaultSelectValue = itemCount;
				}

				itemCount++;
			}

			this.TransactionTypeDropDownList.DataTextField = "ID";
			this.TransactionTypeDropDownList.DataValueField = "ItemIndex";
			this.TransactionTypeDropDownList.DataSource = transactionTypeList;
			this.TransactionTypeDropDownList.DataBind();

			this.TransactionTypeDropDownList.SelectedValue = defaultSelectValue.ToString(CultureInfo.InvariantCulture);

			if (this.Page.Session[PageSessionKeyConstants.TRANSACTION_LIST_PAGE_TRANS_TYPE_INDEX] != null)
			{
				this.Page.Session.Remove(PageSessionKeyConstants.TRANSACTION_LIST_PAGE_TRANS_TYPE_INDEX);
			}

			if (this.Page.Session[PageSessionKeyConstants.TRANSACTION_LIST_PAGE_TRANS_TYPE_LIST] != null)
			{
				this.Page.Session.Remove(PageSessionKeyConstants.TRANSACTION_LIST_PAGE_TRANS_TYPE_LIST);
			}

			int selectedValue = Convert.ToInt32(this.TransactionTypeDropDownList.SelectedValue, CultureInfo.InvariantCulture);
			this.Page.Session.Add(PageSessionKeyConstants.TRANSACTION_LIST_PAGE_TRANS_TYPE_INDEX, selectedValue);
			this.Page.Session.Add(PageSessionKeyConstants.TRANSACTION_LIST_PAGE_TRANS_TYPE_LIST, transactionTypeList);
		}

		/// <summary>
		/// This method will handle the refresh button event or when the page is first loaded.
		/// It gets the data for the transaction list grid.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		private void RefreshButtonCommand(object sender, CommandEventArgs e)
		{
			try
			{
				this.Session.Remove(PageSessionKeyConstants.TRANSACTION_LIST_PAGE_EXPORT_FORMAT);

				string[] statusString = Enum.GetNames(typeof(TransactionStatus));
				string[] qualityString = Enum.GetNames(typeof(TransactionQuality));

				if ((this.Page.Session["UseDataDictionary"] == null) || ((bool)this.Page.Session["UseDataDictionary"]))
				{
					FMChannelHelper.MakeCall<IDataDictionariesClass>(
						dictionaries =>
							{
								if (this.Page.Session["SiteGuid"] != null)
								{
									var siteGuid = (Guid)this.Page.Session["SiteGuid"];
									int index = 0;

									foreach (string status in statusString)
									{
										statusString[index++] = dictionaries.Get(siteGuid, status);
									}

									index = 0;

									foreach (string quality in qualityString)
									{
										qualityString[index++] = dictionaries.Get(siteGuid, quality);
									}
								}
							});
				}

				if (this.Page.Session[PageSessionKeyConstants.TRANSACTION_LIST_PAGE_TRANS_TYPE_INDEX] != null)
				{
					this.Page.Session.Remove(PageSessionKeyConstants.TRANSACTION_LIST_PAGE_TRANS_TYPE_INDEX);
				}

				int selectedValue = Convert.ToInt32(this.TransactionTypeDropDownList.SelectedValue, CultureInfo.InvariantCulture);
				this.Page.Session.Add(PageSessionKeyConstants.TRANSACTION_LIST_PAGE_TRANS_TYPE_INDEX, selectedValue);

				// Get the appropriate alias columns to build the grid.
				this.InitializeTransactionDataGrid();

				this.ChangeInventoryDateColumnLabel();

				string selectedTransTypeName = string.Empty;

				this.security = (SecurityClass)this.Session["Security"];
				var transactionListSr = new TransactionListSR
					{
						Security = this.security,
						TransactionDate = DateTime.Parse(this.DateValueLabel.Text,  this.CurrentSite.GetDateTimeFormatInfo()),
						Manager = this.ManagerValueLabel.Text,
						Owner = this.OwnerValueLabel.Text,
						Product = this.ProductValueLabel.Text,
						DateType = (BsmeLedgerDateType.DateProcessTypes)Convert.ToInt32(this.Session[PageSessionKeyConstants.LEDGER_DATE_TYPE_SELECTION])
					};

				TransactionTypeClass transType = null;

				if (!string.IsNullOrEmpty(Request.GetQueryOrFormValue("NOMINATION_KEY")))
				{
					transactionListSr.NominationKey = Request.GetQueryOrFormValue("NOMINATION_KEY");
                    selectedTransTypeName = "Movement";
				}
				else
				{
					transactionListSr.NominationKey = string.Empty;

					if (this.Page.Session[PageSessionKeyConstants.TRANSACTION_LIST_PAGE_TRANS_TYPE_INDEX] != null)
					{
						var selectedTransTypeIndex =
							(int)this.Page.Session[PageSessionKeyConstants.TRANSACTION_LIST_PAGE_TRANS_TYPE_INDEX];

						if (this.Page.Session[PageSessionKeyConstants.TRANSACTION_LIST_PAGE_TRANS_TYPE_LIST] != null)
						{
							var transTypeList =
								this.Page.Session[PageSessionKeyConstants.TRANSACTION_LIST_PAGE_TRANS_TYPE_LIST] as TransactionTypeListClass;

							if (transTypeList != null)
							{
								transType = transTypeList[selectedTransTypeIndex];
							}

							if (transType != null)
							{
								if (transType.IsAggregate)
								{
									selectedTransTypeName = transType.AggregateColumn.ID;
									LedgerAggregateColumnMapCollectionClass aggregateAliases = transType.AggregateColumn.Aliases;

									foreach (LedgerAggregateColumnMapClass aggregateColumn in aggregateAliases)
									{
										transactionListSr.AddAliasNames(aggregateColumn.AliasName);
									}
								}
								else
								{
									transactionListSr.AddAliasNames(transType.ID);
									selectedTransTypeName = transType.ID;
								}
							}
						}
					}
				}

				// check the configuration for show deleted transactions (IGO 22-Jun-2009)
				var genConfigSr = new GeneralConfigSR
					{
						Security = this.security,
						Request = GeneralConfigSR.GeneralConfigurationRequests.GET_CONFIGURATION_EXCLUDE_ALIASES
					};

				GeneralConfigDO genConfigDo =
					FMChannelHelper.MakeCall<IGeneralConfigProcessor, GeneralConfigDO>(x => x.Get(genConfigSr));

				transactionListSr.ShowDeletedTransactions = genConfigDo.ShowDeletedTransactions;

				// request the transaction list 
				TransactionListDO transactionListDO =
					FMChannelHelper.MakeCall<ITransactionListProcessor, TransactionListDO>(x => x.Process(transactionListSr));

				this.Session["TransactionList.DataSet"] = transactionListDO.TransactionDataSet;

				// Must negate certain transaction types - perhaps in the future
				// quantity data will be stored as it is entered and this can be
				// eliminated.  This could be done by testing the TransTypeID before
				// but it isn't readily available, also by doing it regardless the
				// performance is more deterministic
				if (transactionListDO.TransactionDataSet.Tables[0].Columns["LookupTransactionStatusIndex"] == null)
				{
					transactionListDO.TransactionDataSet.Tables[0].Columns.Add("LookupTransactionStatusIndex");
				}

				transactionListDO.TransactionDataSet.Tables[0].Columns["LookupTransactionStatusIndex"].ColumnName =
					"TransactionStatusInt";
				transactionListDO.TransactionDataSet.Tables[0].Columns.Add("LookupTransactionStatusIndex");
				transactionListDO.TransactionDataSet.Tables[0].Columns["LookupTransactionStatusIndex"].DataType =
					Type.GetType("System.String");

				if (transactionListDO.TransactionDataSet.Tables[0].Columns["ItemTransactionStatus"] == null)
				{
					transactionListDO.TransactionDataSet.Tables[0].Columns.Add("ItemTransactionStatus");
				}

				transactionListDO.TransactionDataSet.Tables[0].Columns["ItemTransactionStatus"].ColumnName =
					"ItemTransactionStatusInt";
				transactionListDO.TransactionDataSet.Tables[0].Columns.Add("ItemTransactionStatus");
				transactionListDO.TransactionDataSet.Tables[0].Columns["LookupTransactionStatusIndex"].DataType =
					Type.GetType("System.String");

				if (transactionListDO.TransactionDataSet.Tables[0].Columns["LookupQualityIndex"] == null)
				{
					transactionListDO.TransactionDataSet.Tables[0].Columns.Add("LookupQualityIndex");
				}

				transactionListDO.TransactionDataSet.Tables[0].Columns["LookupQualityIndex"].ColumnName = "QualityInt";
				transactionListDO.TransactionDataSet.Tables[0].Columns.Add("LookupQualityIndex");
				transactionListDO.TransactionDataSet.Tables[0].Columns["LookupQualityIndex"].DataType = Type.GetType(
					"System.String");

				foreach (DataRow row in transactionListDO.TransactionDataSet.Tables[0].Rows)
				{
					if (row["TransactionStatusInt"] != DBNull.Value)
					{
						row["LookupTransactionStatusIndex"] = statusString[(int)row["TransactionStatusInt"]];
					}

					if (row["ItemTransactionStatusInt"] != DBNull.Value)
					{
						row["ItemTransactionStatus"] = statusString[(int)row["ItemTransactionStatusInt"]];
					}

					if (row["QualityInt"] != DBNull.Value)
					{
						row["LookupQualityIndex"] = qualityString[(int)row["QualityInt"]];
					}

					var transTypeId = DataObject.getValue<short>(row["LookupTransTypeIndex"], 0);

					if ((transTypeId == 5) || (transTypeId == 6))
					{
						row["GrossQuantity"] = -DataObject.getValue(row["GrossQuantity"], 0.0);
						row["NetQuantity"] = -DataObject.getValue(row["NetQuantity"], 0.0);
					}

					// Perform data dictionary on the equipment type columns for a given row.
					this.DataDictionaryEquipmentType(row);
				}

				var transactionTypeList =
					this.Page.Session[PageSessionKeyConstants.TRANSACTION_LIST_PAGE_TRANS_TYPE_LIST] as TransactionTypeListClass;

				var transIndex = Convert.ToInt32(this.TransactionTypeDropDownList.SelectedValue, CultureInfo.InvariantCulture);

				// aggregate column types do not have an Alias property, so check before accessing it
				if (transactionTypeList != null && transactionTypeList[transIndex].Alias != null)
				{
					var identityGuid = transactionTypeList[transIndex].Alias.IdentityGuid;
					
					var alias = this.AliasNames.Find(x => x.IdentityGuid == identityGuid);

					if ( alias == null || alias.TransTypeID == TransactionTypes.T14_PhysicalInventory )
					{
						this.OwnerLabel.Visible = false;
						this.OwnerValueLabel.Visible = false;
					}
				}

				// Insure CurrentPageIndex does not exceed Row Count
				int rowCount = transactionListDO.TransactionDataSet.Tables[0].Rows.Count;

				if (this.TransactionDataGrid.CurrentPageIndex * this.TransactionDataGrid.PageSize > rowCount)
				{
					this.TransactionDataGrid.CurrentPageIndex = rowCount / this.TransactionDataGrid.PageSize;
				}

				// Enable or disable the Add buttons based on the alias name.
				if (transType != null && transType.IsAggregate)
				{
					this.AddButton.Enabled = false;
					this.AddButton1.Enabled = false;
				}
				else
				{
					this.EnableAddButton(selectedTransTypeName);
				}

				// Disable Add buttons if BSM-E version and product is not active
				bool isBsme = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDescKey());
				if (isBsme && this.AddButton.Enabled)
				{
					ProductClass product = this.Session["TransactionList.Product"] as ProductClass;
					if (product != null && product.UserData6.ToUpper() != "YES")
					{
						this.AddButton.Enabled = false;
						this.AddButton1.Enabled = false;
					}
				}

				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void ExportDropDownOnChanged(object sender, EventArgs e)
		{
			var control = sender as FMDropDownList;
			if (control != null)
			{
				var selectedFormat = control.SelectedValue;
				if (!string.IsNullOrEmpty(selectedFormat))
				{
					Page.Session[PageSessionKeyConstants.TRANSACTION_LIST_PAGE_EXPORT_FORMAT] = this.ExportDropDown.SelectedValue;
				}
				else
				{
					Page.Session.Remove(PageSessionKeyConstants.TRANSACTION_LIST_PAGE_EXPORT_FORMAT);
				}
			}
		}

		protected void ExportButtonOnClick(object sender, EventArgs e)
		{
			var selectedFormat = Page.Session[PageSessionKeyConstants.TRANSACTION_LIST_PAGE_EXPORT_FORMAT] as string;
			if (!string.IsNullOrEmpty(selectedFormat))
			{
				this.ExportData(selectedFormat);
			}
		}

		/// <summary>
		/// This method will data dictionary the equipment type column values.
		/// </summary>
		/// <param name="row">Row to perform the data dictionary.</param>
		private void DataDictionaryEquipmentType(DataRow row)
		{
			var columnNames = new List<string> {"DestinationEquipmentType1", 
												"DestinationEquipmentType2",
												"DestinationEquipmentType3",
												"SourceEquipmentType1",
												"SourceEquipmentType2",
												"SourceEquipmentType3",
												"DestinationEquipmentType",
												"SourceEquipmentType"};

			foreach (string columnName in columnNames)
			{
				if (row[columnName] != DBNull.Value)
				{
					var equipType = (string) row[columnName];
					row[columnName] = this.GetTranslatedText(equipType);
				}
			}
		}

		/// <summary>
		/// The transaction data grid edit command.
		/// </summary>
		/// <param name="source">
		/// The source.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		private void TransactionDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
			if (this.Session["TransactionList.DataSet"] == null)
			{
				this.RefreshButtonCommand(null, null);
			}

			// Create session object for TransactionDetail list of transactions.
			var detailList = new TransactionDetailList();

			// First, sort the data set as it was displayed, so that the Previous/Next buttons operate correctly.
			var view = new DataView(((DataSet)this.Session["TransactionList.DataSet"]).Tables[0]);

			if (this.Session["TransactionList.SortExpression"] != null && this.Session["TransactionList.SortDirection"] != null)
			{
				view.Sort = (string)this.Session["TransactionList.SortExpression"] + " "
				            + (string)this.Session["TransactionList.SortDirection"];
			}

			// Put each transaction ID into the list for Previous/Next buttons.
			foreach (DataRowView row in view)
			{
				var transId = row[0] as string;
				detailList.TransactionIDList.Add(transId);
			}

			// Indicate which transaction id in the list is the one to initially display.
			detailList.CurrentIndex = e.Item.DataSetIndex;

            var selectedTransactionGuid= view[detailList.CurrentIndex]["TransactionGuid"];
            var validatedTransactionGuid = selectedTransactionGuid == null ? "" : selectedTransactionGuid.ToString();
            Guid parsedSelectedTransactionGuid;
            if (Guid.TryParse(validatedTransactionGuid, out parsedSelectedTransactionGuid))
            {
                detailList.SelectedTransactionGuid = parsedSelectedTransactionGuid;
            }
            var possiblyNull = view[detailList.CurrentIndex]["AliasName"];
		    if (possiblyNull != null)
		    {
		        detailList.SelectedTransactionAliasID = possiblyNull.ToString();
		    }

            // Escape the alias name for any URL special characters (i.e. & ' / ? ! # $ * + , : ; = @ [ ])
            string columnName = Uri.EscapeDataString(this.Request.GetQueryOrFormValue("Column"));

			// Indicate the return URL for when the TransactionDetail Close button is clicked.
			detailList.ReturnURL = "../Accounting/TransactionList.aspx?Row=" + this.Request.GetQueryOrFormValue("Row") + "&Column="
								   + columnName;

			// Put the object into session and transfer to the TransactionDetail.
			this.Session[TransactionDetailList.TransactionDetailListKey] = detailList;

			// Read the TransactionDetail URL from the Web.config file (06-Jul-2009 IGO)
			string transactionDetailUrl = ConfigurationManager.AppSettings["AccountingTransactionDetailURL"];

			if (Request.GetQueryOrFormValue("TrackReturn") == "true")
			{
				if (transactionDetailUrl.IndexOf('?') > 0)
				{
					transactionDetailUrl += "&TrackReturn=true";
				}
				else
				{
					transactionDetailUrl += "?TrackReturn=true";
				}
			}

			this.Redirect("../" + transactionDetailUrl);
		}

		/// <summary>
		/// The transaction data grid item created.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		private void TransactionDataGridItemCreated(object sender, DataGridItemEventArgs e)
		{
			ListItemType elemType = e.Item.ItemType;

			if (elemType == ListItemType.Header)
			{
				var sortExpression = (string)this.Session["TransactionList.SortExpression"];
				var sortDirection = (string)this.Session["TransactionList.SortDirection"];

				if (sortExpression != null)
				{
					int index = 0;

					foreach (DataGridColumn column in this.TransactionDataGrid.Columns)
					{
						if (column.SortExpression == sortExpression)
						{
							TableCell cell = e.Item.Cells[index];
							var sortedLabel = new Label();
							sortedLabel.Font.Name = "webdings";
							sortedLabel.Font.Size = FontUnit.XSmall;

							if (string.IsNullOrEmpty(sortDirection) || sortDirection == "ASC")
							{
								sortedLabel.Text = "6";
							}
							else
							{
								sortedLabel.Text = "5";
							}

							cell.Controls.Add(sortedLabel);
							break;
						}

						index++;
					}
				}
			}
		}

		/// <summary>
		/// The transaction data grid item data bound.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		private void TransactionDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemIndex >= 0)
			{
				var view = (DataView)this.TransactionDataGrid.DataSource;

				string[] companyId = { "ManagerID", "OwnerID", "ShipperID", "BillToID", "ShipToID", "SupplierID", "CarrierID" };

				COMPANY_ROLE[] role =
					{
						COMPANY_ROLE.MANAGER, 
						COMPANY_ROLE.OWNER, 
						COMPANY_ROLE.SHIPPER, 
						COMPANY_ROLE.CUSTOMER_BILLTO, 
						COMPANY_ROLE.CUSTOMER_SHIPTO, 
						COMPANY_ROLE.SUPPLIER, 
						COMPANY_ROLE.CARRIER
					};

				int index = 0;

				foreach (string id in companyId)
				{
					var companyLabel = e.Item.FindControl(id) as Label;

					if (companyLabel != null)
					{
						companyLabel.Text = view[e.Item.DataSetIndex][id] as string;
						companyLabel.ToolTip = this.GetToolTip(e.Item.DataSetIndex, view, role[index]);
					}

					index++;
				}

				// Update the the EditLinkButton if ShowDeletedEnabled and the row contains a deleted item (25-Jun-2009 IGO)
				DataRow datarow = view.Table.Rows[e.Item.ItemIndex];
				bool deleteflag = DataObject.getValue(datarow["DeleteFlag"], false);

				if (deleteflag)
				{
					var editButton = e.Item.FindControl("EditButton") as FMEditLinkButton;

					if (editButton != null)
					{
						editButton.ShowDeleted = true;
					}
				}

				string aliasName = DataObject.getValue(datarow["aliasName"], string.Empty);

				if (!string.IsNullOrEmpty(aliasName))
				{
					if (!this.security.HasViewTransactionRightByAliasName(aliasName))
					{
						e.Item.Visible = false;
					}
				}

				bool errorFlag = DataObject.getValue(datarow["ErrorFlag"], false);

				if (errorFlag)
				{
					e.Item.ForeColor = Color.Red;
					e.Item.BackColor = Color.FromArgb(255, 204, 204);
				}
			}
		}

		/// <summary>
		/// The transaction data grid page index changed.
		/// </summary>
		/// <param name="source">
		/// The source.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		private void TransactionDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.TransactionDataGrid.EditItemIndex > -1)
			{
				return;
			}

			this.TransactionDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateView();
		}

		/// <summary>
		/// This method updates the grid based on the value selected in the number of records
		/// to show dropdown.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void GridSizeDropdownOnChange(object sender, EventArgs e)
		{
			// if we are editing do not allow a page change
			if ( this.TransactionDataGrid.EditItemIndex > -1 )
			{
				return;
			}

			this.UpdateView();
		}

		/// <summary>
		/// The transaction data grid sort command.
		/// </summary>
		/// <param name="source">
		/// The source.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		private void TransactionDataGridSortCommand(object source, DataGridSortCommandEventArgs e)
		{
			if (this.Session["TransactionList.SortExpression"] != null)
			{
				if ((string)this.Session["TransactionList.SortExpression"] != e.SortExpression)
				{
					this.Session["TransactionList.SortExpression"] = e.SortExpression;
				}
				else
				{
					if (this.Session["TransactionList.SortDirection"] != null
					    && (string)this.Session["TransactionList.SortDirection"] == "ASC")
					{
						this.Session["TransactionList.SortDirection"] = "DESC";
					}
					else
					{
						this.Session["TransactionList.SortDirection"] = "ASC";
					}
				}
			}
			else
			{
				this.Session["TransactionList.SortExpression"] = e.SortExpression;
				this.Session["TransactionList.SortDirection"] = "ASC";
			}

			this.UpdateView();
		}

		/// <summary>
		/// This method will update the transaction list grid view.
		/// </summary>
		private void UpdateView()
		{
			var view = new DataView(((DataSet)this.Session["TransactionList.DataSet"]).Tables[0]);

			if ((this.Session["TransactionList.SortExpression"] != null) && (this.Session["TransactionList.SortDirection"] != null))
			{
				string sortExpr = (string)this.Session["TransactionList.SortExpression"];
				string sortDir = (string)this.Session["TransactionList.SortDirection"];
				view.Sort = sortExpr + " " + sortDir;
			}

			var culture = new CultureInfo(CultureInfo.CurrentCulture.LCID)
				              {
					              NumberFormat =
						              {
							              NumberDecimalSeparator = this.CurrentSite.NumberDecimalSeparator,
							              NumberGroupSeparator = this.CurrentSite.NumberGroupSeparator,
							              NumberGroupSizes = this.CurrentSite.GetNumberGroupSizes(),
							              NumberNegativePattern = 0,
							              NegativeSign = string.Empty
						              },
					              DateTimeFormat = this.CurrentSite.GetDateTimeFormatInfo()
				              };

			Thread.CurrentThread.CurrentCulture = culture;

			// Set column formats for dates
			this.SetColumnFormatsForDates(view);

			this.TransactionDataGrid.DataSource = view;
			this.TransactionDataGrid.DataBind();

			Session["TransactionList.ExportDataSet"] = this.ConvertDisplayDataToDataSet(view.ToTable());
		}

		private DataSet ConvertDisplayDataToDataSet(DataTable dataTable)
		{
			// The incoming dataTable is the n-number of rows and ~370 columns of underlying ledger data.

			// Dict holds the header text from the display grid and index of the column from ledger data table.
			var columnDictionary = new Dictionary<string, int>();

			// New table holds the pared-down set of data.  Only export what appears in the transaction list grid.
			var exportDataTable = new DataTable();

			foreach (DataGridColumn column in TransactionDataGrid.Columns)
			{
				var headerText = column.HeaderText;
				var columnVisible = column.Visible;

				if (headerText == "Edit" || !columnVisible)
					continue;

				var columnName = column.SortExpression;  // The underlying name of the column in the gridview datatable.
				var columnIndex = dataTable.Columns[columnName].Ordinal;
				var dataType = dataTable.Columns[columnIndex].DataType;

				exportDataTable.Columns.Add(new DataColumn(headerText, dataType));

				columnDictionary.Add(headerText, columnIndex);
			}

			foreach (DataRow row in dataTable.Rows)
			{
				var rowValues = new List<object>();
				foreach (var keyValuePair in columnDictionary)
				{
					var columnIndex = keyValuePair.Value;
					var columnValue = row[columnIndex];
					rowValues.Add(columnValue);
				}
				exportDataTable.Rows.Add(rowValues.ToArray());
			}

			var exportDataSet = new DataSet();
			exportDataSet.Tables.Add(exportDataTable);
			return exportDataSet;
		}

		private void SetColumnFormatsForDates(DataView view)
		{
			if (view.Table == null)
			{
				return;
			}

			var formatInfo = this.CurrentSite.GetDateTimeFormatInfo();
			var datePattern = "{0:" + formatInfo.ShortDatePattern + " " + formatInfo.ShortTimePattern + "}";

			foreach (DataColumn column in view.Table.Columns)
			{
				if (column.DataType.Name == "DateTimeOffset" 
					&& column.ColumnName.NotEquals("InventoryDate", StringComparison.InvariantCultureIgnoreCase))
				{
					foreach (var gridColumn in this.TransactionDataGrid.Columns)
					{
						if (gridColumn is BoundColumn)
						{
							if ((gridColumn as BoundColumn).DataField == column.ColumnName)
							{
								(gridColumn as BoundColumn).DataFormatString = datePattern;
								break;
							}
						}
					}
				}
			}
		}

		#endregion
	}
	#endregion

	#region Company Label class
	/// <summary>
	/// The company label.
	/// </summary>
	public class CompanyLabel : ITemplate
	{
		#region Constants and Fields
		/// <summary>
		/// The id.
		/// </summary>
		private readonly string id;
		#endregion

		#region Constructors and Destructors
		/// <summary>
		/// Initializes a new instance of the <see cref="CompanyLabel"/> class.
		/// </summary>
		/// <param name="inId">
		/// The id.
		/// </param>
		public CompanyLabel(string inId)
		{
			this.id = inId;
		}
		#endregion

		#region Public Methods and Operators
		/// <summary>
		/// The instantiate in.
		/// </summary>
		/// <param name="container">
		/// The container.
		/// </param>
		public void InstantiateIn(Control container)
		{
			var label = new Label { ID = this.id };
			container.Controls.Add(label);
		}
		#endregion
	}
	#endregion
}