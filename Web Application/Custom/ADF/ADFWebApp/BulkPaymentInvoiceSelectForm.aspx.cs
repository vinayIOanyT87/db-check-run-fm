using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using FMControls;
using Accounting;
using FMWebApp;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.Exceptions;
using FMBusinessObjects.UtilityObjects;
using FMBusinessObjects.ServiceRequests;

namespace ADFWebApp
{
	public class BulkPaymentInvoiceContext : BaseContext
	{
		public BulkPaymentInvoiceContext()
			: base()
		{
			this.ResetContextProperties();
		}

		public override void ResetContextProperties()
		{
			base.ResetContextProperties();

			// initialise search variables
			this.InvoiceNumber = -1;
			this.AccountCode = "";
			this.EnteredBy = "{All}";
			this.CostCentreCode = "";
			//this.Supplier = ""; -- ignore these 2 because they're passed in from bulk payment detail
			//this.Section = "{All}";
			this.SupplierInvoiceNumber = "";
			this.CurrencyGuid = Guid.Empty;

		}

		public override string GetKey()
		{
			return BulkPaymentInvoiceContext.CONTEXT_KEY;
		}

		#region Search Properties

		public int InvoiceNumber { get; set; }
		public string AccountCode { get; set; }
		public string EnteredBy { get; set; }
		public string CostCentreCode { get; set; }
		public string Supplier { get; set; }
		public string Section { get; set; }
		public string SupplierInvoiceNumber { get; set; }
		public Guid CurrencyGuid { get; set; }
		#endregion // Properties

		protected static string CONTEXT_KEY = typeof(BulkPaymentInvoiceContext).ToString();
	}

	public partial class BulkPaymentInvoiceSelectForm : BaseContextPage<BulkPaymentInvoiceContext>
	{
		#region Contructor
		public BulkPaymentInvoiceSelectForm()
			: base(new BulkPaymentInvoiceContext())
		{
		}
		#endregion // Constructor

		#region Attributes
		protected TransactionDOCollection m_collection = new TransactionDOCollection();
		protected BulkPaymentDetailContext m_paymentContext = new BulkPaymentDetailContext();
		protected string[] m_sectionCollection = null;
		protected SiteTimeConverter m_converter = null;

		#region Constants
		public static string PARAM_SUPPLIER = "Supplier";
		public static string PARAM_SECTION = "Section";
		public static string PARAM_FOREX = "Forex";
		#endregion // Constants
		#endregion // Attributes

		protected void Page_Load(object sender, EventArgs e)
		{
			base.security = Session["Security"] as SecurityClass;
			if (base.security == null)
			{
				base.ErrorHandler(new FMSessionInvalidException());
				//throw new FMSessionInvalidException();
			}

			BulkPaymentInvoiceContext context = this.GetContext();

			// populate the context based on input parameters
			if (Request.Params[PARAM_SECTION] != null)
			{
				context.Section = Request.Params[PARAM_SECTION];
			}
			if (Request.Params[PARAM_SUPPLIER] != null)
			{
				context.Supplier = Request.Params[PARAM_SUPPLIER];
			}
			if (Request.Params[PARAM_FOREX] != null)
			{
				try
				{
					context.CurrencyGuid = Guid.Parse(Request.Params[PARAM_FOREX]);
				}
				catch (Exception)
				{
					context.CurrencyGuid = Guid.Empty;
				}
			}

			m_paymentContext = Session[m_paymentContext.GetKey()] as BulkPaymentDetailContext;

			context.AcctSite.GetUserCompanies = true;
			context.AcctSite = FMChannelHelper.MakeCall<IAccountingSites, AccountingSite>(
																	 x =>
																	 x.LoadSiteInfo(base.security, base.security.SiteGuid)
																);

			m_converter = new SiteTimeConverter(context.AcctSite.CurrentSite);
			this.StoreContext(context);

			if (!Page.IsPostBack)
			{
				this.LoadFromContext(context);
				this.PopulateFilters();
				this.EnableDisableControls();

				this.BindControls();
				this.UpdateView();
			}
		}

		protected void EnableDisableControls()
		{
			// currently this is only shown from bulk payment association, so make read only fields
			// for data which are passed in
			this.txtSupplier.Enabled = false;
			//this.ddlSection.Enabled = false; WI-14436
		}

		protected void PopulateFilters()
		{
			// need to populate two things, entered by and section

			// entered by...
			Common.PopulateFilterDropDownList(base.security, ref this.ddlEnteredBy, Common.FilterTarget.USER, true);

			// section...
			this.ddlSection.Items.Add(new ListItem(Common.DDL_ALL));
			Common.PopulateFilterDropDownList(base.security, ref this.ddlSection, Common.FilterTarget.SECTION, true);

			// set the role for supplier
			this.txtSupplier.Role = "SUPPLIER";

			BulkPaymentInvoiceContext context = this.GetContext();
			this.LoadFromContext(context);
		}

		protected void BindControls()
		{
			// bind form events
			this.btnRefresh.Click += new EventHandler(btnRefresh_Click);
			this.btnShowAll.Click += new EventHandler(btnShowAll_Click);
			this.ddlPageSize.SelectedIndexChanged += new EventHandler(ddlPageSize_SelectedIndexChanged);
		}

		protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.UpdateView();
		}

		protected void btnShowAll_Click(object sender, EventArgs e)
		{
			BulkPaymentInvoiceContext context = this.GetContext();

			txtInvoiceNumber.Value = "";
			txtAccountCode.Value = "";
			ddlEnteredBy.SelectByText(Common.DDL_ALL);
			txtCostCentreCode.Value = "";
			txtSupplierInvoiceNumber.Value = "";
			ddlSection.SelectByText(Common.DDL_ALL);

			Object co = context;

			LoadToContext(ref co);
			context = co as BulkPaymentInvoiceContext;

			this.StoreContext(context);
			this.LoadFromContext(context);

			this.btnRefresh_Click(sender, e);
		}

		protected void btnRefresh_Click(object sender, EventArgs e)
		{
			try
			{
				Common.RefreshPreProcessing(ref this.InvoiceDataGrid,
					new Common.GetContext(GetContext),
					new Common.LoadToContext(this.LoadToContext),
					new Common.StoreContext(StoreContext));

				this.UpdateView();
			}
			catch (Exception ex)
			{
				base.ErrorHandler(ex);
			}
		}

		protected void UpdateView()
		{
			try
			{
				BulkPaymentInvoiceContext context = this.GetContext();

				// get the collection
				this.m_collection = this.EnumerateByContext(context);

				DataView dw = this.BuildDataView(m_collection);

				this.ddlPageSize.SetPageSize(this.InvoiceDataGrid, m_collection.Count);

				this.InvoiceDataGrid.DataSource = dw;
				this.InvoiceDataGrid.DataBind();
			}
			catch (Exception e)
			{
				base.ErrorHandler(e);
			}
		}

		public TransactionFilterSR BuildTransactionFilter(ICustomContext a_context)
		{
			BulkPaymentInvoiceContext context = a_context as BulkPaymentInvoiceContext;
			if (null == context)
			{
				throw new Exception("could not convert " + a_context.ToString() + " to BulkPaymentInvoiceContext");
			}

			TransactionFilterSR sr = new TransactionFilterSR();

			sr.Security = base.security;
			sr.TransTypeID = TransactionTypes.T21_AccountPayableInvoice;
			sr.StartDateInventory = context.StartDate;
			sr.EndDateInventory = context.EndDate.AddDays(1.0);
			sr.SupplierID = context.Supplier;
			sr.UpdatedBy = context.EnteredBy;
			sr.UseDate = TransactionFilterSR.DateType.INVENTORYDATE;

			return sr;
		}

		public bool FilterResults(TransactionDO trans)
		{
			// JS20100103 CCP-042 first created method, need to add last edit criteria
			bool returnVal = false;

			BulkPaymentInvoiceContext a_context = base.GetContext();

			// JS20100208 CCP-042 check it's not already associated with another bulk payment
			BulkPaymentInvoiceMappingCollectionClass existingMappingCollection =
				FMChannelHelper.MakeCall<IBulkPaymentInvoiceMappings, BulkPaymentInvoiceMappingCollectionClass>(
																	 x =>
																	 x.Enumerate(base.security)
																);

			foreach (BulkPaymentInvoiceMappingClass mapping in existingMappingCollection)
			{
				if (mapping.InvoiceTransID.ToUpper().Equals(trans.TransID.ToUpper()))
				{
					return false;
				}
			}

			// failsafe
			if (trans.UserData[Common.USERDATA_SUPPLIERINVOICENUMBER_KEY] == null)
			{
				trans.UserData[Common.USERDATA_SUPPLIERINVOICENUMBER_KEY] = "";
			}

			if (
				((trans.UserData[Common.USERDATA_COSTCENTRE_KEY].ToString()).Contains(a_context.CostCentreCode) || a_context.CostCentreCode == "") && // cost centre code
				((trans.UserData[Common.USERDATA_ACCTCODE_KEY].ToString()).Contains(a_context.AccountCode) || a_context.AccountCode == "") && // account code
				(trans.UserData[Common.USERDATA_SECTION_KEY].ToString().ToUpper().Equals(a_context.Section.ToUpper()) || a_context.Section == "" || a_context.Section.Equals(Common.DDL_ALL)) && // section
				(trans.SupplierID.Equals(a_context.Supplier) || a_context.Supplier == "") && // supplier
				(trans.Flag05) && // Ready For Payment
				(!IsAssociated(trans.TransID)) && // not already selected
				(trans.UserData[Common.USERDATA_SUPPLIERINVOICENUMBER_KEY].ToString().ToUpper().Equals(a_context.SupplierInvoiceNumber.ToUpper()) || a_context.SupplierInvoiceNumber.Length == 0) // supplier invoice number

				)
			{
				if (trans.LineItems.Count > 0)
				{
					returnVal = (a_context.CurrencyGuid == Guid.Empty && (trans.LineItems[0] as LineItemDO).CurrencyGuid == Guid.Empty); // or both AUD

					if ((trans.LineItems[0] as LineItemDO).CurrencyGuid != Guid.Empty && !returnVal)
					{
						returnVal = a_context.CurrencyGuid == (trans.LineItems[0] as LineItemDO).CurrencyGuid; // matching forex unit
					}

				}
				else
				{
					returnVal = true; // no product to match, just add it
				}
			}

			return returnVal;
		}

		protected TransactionDOCollection EnumerateByContext(BulkPaymentInvoiceContext a_context)
		{
			TransactionDOCollection result = new TransactionDOCollection();

			try
			{
				result = Common.EnumerateByContext(a_context, base.security, a_context.AcctSite,
						new Common.FilterBuilderDelegate(BuildTransactionFilter),
						new Common.InlineFilterDelegate(FilterResults));
			}
			catch (Exception e)
			{
				base.ErrorHandler(e);
			}

			return result;
		}

		protected DataView BuildDataView(TransactionDOCollection a_collection)
		{
			DataView result = null;

			// create a matching table
			DataTable table = new DataTable();
			table.Columns.Add("InvoiceNumber", typeof(string));
			table.Columns.Add("SupplierInvoiceNumber", typeof(string));
			table.Columns.Add("OrderNumber", typeof(string));
			table.Columns.Add("Quantity", typeof(string));
			table.Columns.Add("Supplier", typeof(string));
			table.Columns.Add("TotalAmount", typeof(string));
			table.Columns.Add("Section", typeof(string));
			table.Columns.Add("EnteredBy", typeof(string));
			table.Columns.Add("ActionRequired", typeof(string));
			table.Columns.Add("TransID", typeof(string));

			foreach (TransactionDO trans in a_collection)
			{
				DataRow row = table.NewRow();

				double totalQuantity = 0.0;
				double totalAmount = 0.0;
				for (int index = 0; index < trans.LineItems.Count; ++index)
				{
					LineItemDO resultLineItem = Common.AggregateLineItemValues(security, trans, index);
					totalQuantity += resultLineItem.Quantity.Gross;
					totalAmount += resultLineItem.TotalPriceWithTax;
				}

				row["InvoiceNumber"] = trans.DocumentNumber;
				row["SupplierInvoiceNumber"] = trans.UserData[Common.USERDATA_SUPPLIERINVOICENUMBER_KEY];
				row["OrderNumber"] = trans.PONumber;
				row["Quantity"] = Math.Round(totalQuantity, 0, MidpointRounding.AwayFromZero).ToString();
				row["Supplier"] = trans.SupplierID;
				row["TotalAmount"] = String.Format("{0:c}", Math.Round(totalAmount, 2, MidpointRounding.AwayFromZero));
				row["Section"] = trans.UserData[Common.USERDATA_SECTION_KEY];
				row["EnteredBy"] = ""; // JS20100103 CCP-042 TBC - wait for ATL to finish the last edit field
				row["ActionRequired"] = trans.UserData[Common.USERDATA_ACTIONREQUIRED_KEY];
				row["TransID"] = trans.TransID;

				table.Rows.Add(row);
			}

			result = new DataView(table);

			return result;
		}

		#region Context operations

		public override BulkPaymentInvoiceContext LoadToContext(ref Object a_context)
		{
			BulkPaymentInvoiceContext context = a_context as BulkPaymentInvoiceContext;

			try
			{

				if (txtInvoiceNumber.Value.Length > 0)
					context.InvoiceNumber = int.Parse(this.txtInvoiceNumber.Value);

				context.AccountCode = this.txtAccountCode.Value.Trim();
				// convert back to utc from display
				context.StartDate = m_converter.ConvertFromSiteTime(this.startDateCtrl.CurrentValue);
				context.EndDate = m_converter.ConvertFromSiteTime(this.endDateCtrl.CurrentValue);
				context.CostCentreCode = this.txtCostCentreCode.Value.Trim();
				context.Supplier = this.txtSupplier.Text;
				context.SupplierInvoiceNumber = this.txtSupplierInvoiceNumber.Value.Trim();

				context.EnteredBy = Common.GetValueForFiltering(this.ddlEnteredBy);
				context.Section = Common.GetValueForFiltering(this.ddlSection);
			}
			catch (Exception e)
			{
				base.ErrorHandler(e);
			}

			a_context = context;

			return a_context as BulkPaymentInvoiceContext;
		}

		public override void LoadFromContext(BulkPaymentInvoiceContext a_context)
		{
			if (a_context.InvoiceNumber < 0)
				this.txtInvoiceNumber.Value = "";
			else
				this.txtInvoiceNumber.Value = a_context.InvoiceNumber.ToString();

			this.txtAccountCode.Value = a_context.AccountCode;
			this.startDateCtrl.CurrentValue = m_converter.ConvertToSiteTime(a_context.StartDate);
			this.endDateCtrl.CurrentValue = m_converter.ConvertToSiteTime(a_context.EndDate);
			this.txtCostCentreCode.Value = a_context.CostCentreCode;
			this.txtSupplier.Text = a_context.Supplier;
			this.txtSupplierInvoiceNumber.Value = a_context.SupplierInvoiceNumber;

			try
			{
				if (a_context.EnteredBy.Length > 0)
				{
					this.ddlEnteredBy.Text = a_context.EnteredBy;
				}
				this.ddlSection.Text = a_context.Section;
			}
			catch (Exception) { }
		}

		#endregion // Context operations

		#region Overrides
		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
			//base.init();
		}
		#endregion // Overrides

		#region Line Items
		private void InitializeComponent()
		{
			// bind data grid events
			this.InvoiceDataGrid.ItemDataBound += new DataGridItemEventHandler(InvoiceDataGrid_ItemDataBound);
			this.InvoiceDataGrid.PageIndexChanged += new DataGridPageChangedEventHandler(InvoiceDataGrid_PageIndexChanged);
		}

		protected void InvoiceDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			try
			{
				Common.PageChangePreProcessing(ref this.InvoiceDataGrid, e);

				this.UpdateView();
			}
			catch (Exception ex)
			{
				base.ErrorHandler(ex);
			}
		}

		protected void InvoiceDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			try
			{
				// add the select button
				if (e.Item.ItemIndex != -1)
				{
					// set checkbox value to remember item checked
					FMControls.FMCheckBox cb = e.Item.FindControl("cbSelect") as FMControls.FMCheckBox;
					HtmlInputHidden hiddenInput = e.Item.FindControl("hiddenSelect") as HtmlInputHidden;
					if (cb != null)
					{
						string transID = (string)((System.Data.DataRowView)(((System.Web.UI.WebControls.DataGridItem)(e.Item.Cells[9].BindingContainer)).DataItem)).Row.ItemArray[9];
						hiddenInput.Value = transID;
					}
				}
			}
			catch (Exception except)
			{
				base.ErrorHandler(except);
			}
		}
		#endregion // Line Items

		protected void Cancel_Clicked(object sender, EventArgs e)
		{
			this.Response.Write("<script language=\"JavaScript\">window.close()</script>");
		}

		protected void OK_Clicked(object sender, EventArgs e)
		{
			foreach (DataGridItem item in this.InvoiceDataGrid.Items)
			{
				FMCheckBox cb = (FMCheckBox)item.FindControl("cbSelect");
				HtmlInputHidden hiddenInput = (HtmlInputHidden)item.FindControl("hiddenSelect");

				if (cb.Checked)
				{

					LineItemDO li = new LineItemDO();
					li.TransactionLineItemGuid = -(m_paymentContext.NewTransIDs.Count + 1);

					//string defaultStatusString = Request.Params["defaultStatus"];
					//li.Status = (TransactionStatus) int.Parse(defaultStatusString);

					try
					{
						BulkPaymentInvoiceContext context = this.GetContext();

						string transID = hiddenInput.Value;

						m_paymentContext.NewTransIDs.Add(transID);

						Session[m_paymentContext.GetKey()] = m_paymentContext;
					}
					catch (Exception ex)
					{
						base.ErrorHandler(ex);
					} // create the associated transaction for the line item (one transaction per line item)
				} // item is checked
			} // loop through all the 

			this.Response.Write("<script language=\"JavaScript\">window.returnValue = new Array(\"OK_Clicked\");window.close()</script>");
		}

		protected bool IsAssociated(string transID)
		{
			return m_paymentContext.NewTransIDs.Contains(transID);

			/*

		List<string> allTransList = new List<string>(m_paymentContext.NewTransIDs);

		foreach

		foreach (LineItemDO li in m_trans.LineItems)
		{
			foreach (AssociatedTxDO tx in li.AssociatedTransactions)
			{
				if (tx.TransID.ToUpper().Equals(transID.ToUpper()))
				{
					return true;
				}
			}
		}

		return false;*/
		}
	}
}
