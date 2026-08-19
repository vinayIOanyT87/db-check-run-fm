/// <summary>
/// File name:	FessDetailPage.cs
/// 
/// Purpose:	The purpose of this class is to handle the display of the FESS detail records.
///             
/// Comments:	Copyright (C) Varec, Inc - An SAIC Company, Norcross, GA, USA, 
///				2009.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Varec, Inc.
///				
/// Author(s):	R.Panachida & G.Kendall
///	
/// Modification History:
/// Date:			By:					   Reason:
/// ----------		--------------------	----------------------------------
///		
///</summary>

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
using System.Web.SessionState;
using System.Reflection;

using Accounting;
using FMControls;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.UtilityObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.Constants;
using FMBusinessObjects.ServiceRequests;
using FMBusinessObjects.Exceptions;

namespace ADFWebApp
{
	public class BulkPaymentDetailContext : BaseContext
	{
		#region Attributes
		public static string CONTEXT_KEY = "BulkPaymentDetailContext.Key";
		#endregion // Attributes

		public BulkPaymentDetailContext()
			: base()
		{
			this.BulkPayment = null;
			this.InvoiceCache = new Hashtable();

			this.NewTransIDs = new List<string>();
			this.ExistingTransIDs = new List<string>();
		}

		public BulkPaymentInvoiceMappingClass GetMapping(string a_transID)
		{
			BulkPaymentInvoiceMappingClass result = null;

			foreach (BulkPaymentInvoiceMappingClass mapping in BulkPayment.Mapping)
			{
				if (mapping.InvoiceTransID.ToUpper().Equals(a_transID.ToUpper()))
				{
					result = mapping;
				}
			}

			// if mapping not found then create it
			if (null == result)
			{
				result = new BulkPaymentInvoiceMappingClass();

				result.BulkPaymentID = this.BulkPayment.BulkPaymentID;
				result.InvoiceTransID = a_transID;

				BulkPayment.Mapping.Add(result);
			}

			return result;
		}

		public void SetMapping(string a_transID, BulkPaymentInvoiceMappingClass a_mapping)
		{
			foreach (BulkPaymentInvoiceMappingClass mapping in BulkPayment.Mapping)
			{
				if (mapping.InvoiceTransID.ToUpper().Equals(a_transID.ToUpper()))
				{
					BulkPayment.Mapping.Remove(mapping);
					break;
				}
			}

			BulkPayment.Mapping.Add(a_mapping);
		}

		#region Implementations
		public override void ResetContextProperties()
		{
			base.ResetContextProperties();
		}

		public override string GetKey()
		{
			return CONTEXT_KEY;
		}
		#endregion // Implementations

		#region Properties
		public BulkPaymentClass BulkPayment { get; set; }
		public Hashtable InvoiceCache { get; set; }

		public List<string> NewTransIDs { get; set; }
		public List<string> ExistingTransIDs { get; set; }
		#endregion // Properties
	}

	public partial class BulkPaymentDetailPage : BaseContextPage<BulkPaymentDetailContext>, IDataDictionary
	{
		protected enum Mode
		{
			ADD,
			EDIT
		}

		#region Attributes
		protected Mode m_mode = Mode.ADD;
		protected SiteTimeConverter m_converter;

		public static string EDIT_PARAM_NAME = "BulkPaymentID";
		public static string FILENAME = "BulkPaymentDetailPage.aspx";
		#endregion // Attributes

		#region Construction
		public BulkPaymentDetailPage()
			: base(null)
		{
		}

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
			base.init();
		}

		public override void Dispose()
		{
			base.Dispose();

			if (this.IsPostBack)
			{
				//this.StoreContext(null);
			}
		}

		protected void InitializeComponent()
		{
			this.resultGrid.ItemDataBound += new DataGridItemEventHandler(resultGrid_ItemDataBound);
			this.resultGrid.EditCommand += new DataGridCommandEventHandler(resultGrid_EditCommand);
			this.resultGrid.CancelCommand += new DataGridCommandEventHandler(resultGrid_CancelCommand);
			this.resultGrid.UpdateCommand += new DataGridCommandEventHandler(resultGrid_UpdateCommand);
			this.resultGrid.DeleteCommand += new DataGridCommandEventHandler(resultGrid_DeleteCommand);
			this.resultGrid.PageIndexChanged += new DataGridPageChangedEventHandler(resultGrid_PageIndexChanged);
			this.ddlForeignCurrency.SelectedIndexChanged += new EventHandler(ddlForeignCurrency_SelectedIndexChanged);
		}
		#endregion // Constructor

		#region Data Dictionary
		string[] IDataDictionary.Keys(SecurityClass a_security)
		{
			string[] keys = 
			{
				// header
				"Bulk Payment Details",
				"Bulk Payment ID",
				"ROMAN Payment Number",
				"Section",
				"Last Edited By",
				"Payment Due Date",
				"Transaction Date",
				"Excise Total",
				"On-Cost Total",
				"GST Total",
				"Location",
				"Payment Type",
				"Supplier",
				"Foreign Currency",
				"Foreign Currency Rate",
				"Discount Rate",
				"Total Foreign Price",
				"Total AUD",
				"Total AUD Paid",
				// line items
				"Excise Value",
				"On-Cost Value",
				"GST Value",
				"Rebate Flag",
				"Rebate Number",
				"Foreign Total",
				"Total Price",
				"Invoice Number",
				"Vol on Delivery Docket",
				// buttons
				"Associate"
			};

			return keys;
		}

		protected void BuildDictionaryLabels()
		{

			lblHeading.Text = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(base.security.SiteGuid, "Bulk Payment Details")
																);
			lblBulkPaymentID.Text =
				FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(base.security.SiteGuid, "Bulk Payment ID"));
			lblRomanNumber.Text =
				FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(base.security.SiteGuid, "ROMAN Payment Number"));
			lblSection.Text =
				FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(base.security.SiteGuid, "Section"));
			lblLastEditedBy.Text =
				FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(base.security.SiteGuid, "Last Edited By"));
			lblPaymentDueDate.Text =
				FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(base.security.SiteGuid, "Payment Due Date"));
			lblTransactionDate.Text =
				FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(base.security.SiteGuid, "Transaction Date"));
			lblExcise.Text =
				FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(base.security.SiteGuid, "Excise Total"));
			lblOnCost.Text =
				FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(base.security.SiteGuid, "On-Cost Total"));
			lblGST.Text =
				FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(base.security.SiteGuid, "GST Total"));
			lblLocation.Text =
				FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(base.security.SiteGuid, "Location"));
			lblPaymentType.Text =
				FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(base.security.SiteGuid, "Payment Type"));
			lblSupplier.Text =
				FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(base.security.SiteGuid, "Supplier"));
			lblForeignCurrency.Text =
				FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(base.security.SiteGuid, "Foreign Currency"));
			lblForeignRate.Text =
				FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
					x => x.Get(base.security.SiteGuid, "Foreign Currency Rate"));
			lblDiscountRate.Text =
				FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(base.security.SiteGuid, "Discount Rate"));
			lblTotalForeign.Text =
				FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(base.security.SiteGuid, "Total Foreign Price"));
			lblTotal.Text =
				FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(base.security.SiteGuid, "Total AUD"));
			lblTotalPaid.Text =
				FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(base.security.SiteGuid, "Total AUD Paid"));
		}
		#endregion // Data Dictionary

		#region Event Handling
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				// security
				base.security = Session["Security"] as SecurityClass;
				if (base.security == null)
				{
					throw new FMSessionInvalidException();
				}
				// accounting
				BulkPaymentDetailContext context = this.GetContext();

				context.AcctSite = FMChannelHelper.MakeCall<IAccountingSites, AccountingSite>(
																	 x =>
																	 x.LoadSiteInfo(base.security, base.security.SiteGuid)
																);

				m_converter = new SiteTimeConverter(context.AcctSite.CurrentSite);

				// check input parameter for edit flag
				if (Request.Params[BulkPaymentDetailPage.EDIT_PARAM_NAME] != null)
				{
					m_mode = Mode.EDIT;

					if (null == context.BulkPayment)
					{
						int id = int.Parse(Request.Params[BulkPaymentDetailPage.EDIT_PARAM_NAME]);
						context.BulkPayment = FMChannelHelper.MakeCall<IBulkPayments, BulkPaymentClass>(
																	 x =>
																	 x.GetByID(base.security, id)
																);


						// load the mappings
						context.BulkPayment.Mapping = FMChannelHelper.MakeCall<IBulkPaymentInvoiceMappings, BulkPaymentInvoiceMappingClass>(
																	 x =>
																	 x.EnumerateByBulkPaymentID(base.security, context.BulkPayment.BulkPaymentID)
																);
					}
				}
				else
				{
					if (null == context.BulkPayment)
					{
						context.BulkPayment = new BulkPaymentClass();
						context.BulkPayment.BulkPaymentID = FMChannelHelper.MakeCall<IBulkPayments, BulkPaymentClass>(
																	 x =>
																	 x.GetNextBulkPaymentID(base.security)
																);

					}

					this.btnViewPrintable.Enabled = false; // disabled until saved
				}
				this.StoreContext(context);

				// setup event handling...
				this.BindControls();

				// check security
				bool ok = this.SecurityProcessing();
				if (!ok)
				{
					throw new System.AccessViolationException("Access Denied");
				}

				if (!this.IsPostBack)
				{
					if (m_mode == Mode.EDIT)
					{
						this.CreateInvoiceCache();
					}
					this.SetDefaultFieldState();
					this.PopulateControls();
					this.LoadFromContext(context);

					this.BuildDictionaryLabels();

					// CCP-??? disable roman number field if the field contains ANYTHING
					if (this.tbRomanNumber.Text.Length > 0)
					{
						Control arg = tbRomanNumber as Control;
						Common.SetTextboxState(false, ref arg);
						this.tbRomanNumber = arg as TextBox;
					}

					UpdateView();
				}
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
				BulkPaymentDetailContext context = this.GetContext();

				DataView dw = this.BuildDataView(context);

				this.StoreContext(context);

				this.ddlPageSize.SetPageSize(this.resultGrid, context.NewTransIDs.Count + context.ExistingTransIDs.Count + 1);

				this.resultGrid.DataSource = dw;
				this.resultGrid.DataBind();
			}
			catch (Exception e)
			{
				base.ErrorHandler(e);
			}
		}

		protected void InvSelectionTextBox_TextChanged(object sender, EventArgs e)
		{
			this.InvSelectionTextBox.Text = "";

			this.UpdateView();
		}

		protected void ddlForeignCurrency_SelectedIndexChanged(object sender, EventArgs e)
		{
			// pull the most current exchange rate
			Guid currencyGuid = Guid.Empty;

			try
			{
				currencyGuid = Guid.Parse(ddlForeignCurrency.SelectedValue);
			}
			catch (Exception) { }

			if (currencyGuid != Guid.Empty)
			{
				CurrencyDO currency = FMChannelHelper.MakeCall<ICurrencies, CurrencyDO>(
																	 x =>
																	 x.Get(this.security, currencyGuid)
																);

				if (currency != null)
				{
					// go through the list of currency line items to find the most recent exchange rate
					DateTimeOffset mostRecent = DateTimeOffset.MinValue;
					CurrencyLineItemDO found = null;

					foreach (CurrencyLineItemDO currencyLineItem in currency.LineItems)
					{
						if (currencyLineItem.CurrencyGuid == currencyGuid &&
							currencyLineItem.EffectiveDate > mostRecent)
						{
							mostRecent = currencyLineItem.EffectiveDate;
							found = currencyLineItem;
						}
					}

					if (found != null)
					{
						tbForeignRate.Text = found.Rate.ToString();

						// force recalculation of AUD paid
						AggregateFinancialControls();
					}
				}
			}
		}

		protected void resultGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			try
			{
				Common.PageChangePreProcessing(ref this.resultGrid, e);

				this.UpdateView();
			}
			catch (Exception ex)
			{
				base.ErrorHandler(ex);
			}
		}

		protected void EnableDisableControls(bool a_editing)
		{
			this.tbDiscountRate.ReadOnly =
				this.tbForeignRate.ReadOnly =
				this.tbLocation.ReadOnly =
				this.tbRomanNumber.ReadOnly =
				this.tbSupplier.ReadOnly =
				a_editing;

			this.ddlForeignCurrency.Enabled = this.dtPaymentDueDate.Enabled = this.dtTransactionDate.Enabled =
				this.ddlPageSize.Enabled = this.ddlPaymentType.Enabled = this.ddlSection.Enabled =
				this.btnApply.Enabled = this.btnCancel.Enabled = this.btnViewPrintable.Enabled =
				!a_editing;

			if (a_editing)
			{
				this.tbDiscountRate.BackColor =
				 this.tbRomanNumber.BackColor =
				 this.tbSupplier.BackColor =
					System.Drawing.Color.FromArgb(211, 211, 211);
			}
			else
			{
				this.tbDiscountRate.BackColor =
				 this.tbRomanNumber.BackColor =
				 this.tbSupplier.BackColor =
					System.Drawing.Color.White;
			}
		}

		protected void SetDefaultFieldState()
		{
			tbBulkPaymentID.BackColor =
				tbOnCost.BackColor =
				tbLocation.BackColor =
				tbExcise.BackColor =
				tbGST.BackColor =
				tbLastEdit.BackColor =
				tbTotal.BackColor =
				tbTotalForeign.BackColor =
				tbTotalPaid.BackColor =
				tbForeignRate.BackColor =
					System.Drawing.Color.FromArgb(211, 211, 211);

			tbBulkPaymentID.ReadOnly =
				tbOnCost.ReadOnly =
				tbLocation.ReadOnly =
				tbExcise.ReadOnly =
				tbGST.ReadOnly =
				tbLastEdit.ReadOnly =
				tbTotal.ReadOnly =
				tbTotalForeign.ReadOnly =
				tbTotalPaid.ReadOnly =
				tbForeignRate.ReadOnly =
				  true;
		}

		protected void resultGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
		}

		protected void resultGrid_EditCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				this.resultGrid.EditItemIndex = e.Item.ItemIndex;
				this.EnableDisableControls(true);
				this.UpdateView();
			}
			catch (Exception ex)
			{
				base.ErrorHandler(ex);
			}
		}

		protected void resultGrid_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			string transID = e.CommandArgument.ToString();

			// remove it from our context so on next enumeration in UpdateView it will be gone
			BulkPaymentDetailContext context = this.GetContext();
			BulkPaymentInvoiceMappingClass mapping = context.GetMapping(transID);

			for (int i = 0; i < context.BulkPayment.Mapping.Count; ++i)
			{
				BulkPaymentInvoiceMappingClass map = context.BulkPayment.Mapping.Item(i);

				if (map.InvoiceTransID.ToUpper().Equals(transID.ToUpper()))
				{
					context.BulkPayment.Mapping.Remove(i);
					break;
				}
			}
			if (context.ExistingTransIDs.Contains(transID))
			{
				context.ExistingTransIDs.Remove(transID);
			}
			else if (context.NewTransIDs.Contains(transID))
			{
				context.NewTransIDs.Remove(transID);
			}
			this.StoreContext(context);

			this.resultGrid.SelectedIndex = -1;

			if ((this.resultGrid.Items.Count == 1) && (this.resultGrid.CurrentPageIndex > 0))
			{
				this.resultGrid.CurrentPageIndex--;
			}

			this.UpdateView();
		}

		protected void resultGrid_CancelCommand(object source, DataGridCommandEventArgs e)
		{
			this.EnableDisableControls(false);

			this.resultGrid.EditItemIndex = -1;

			this.UpdateView();
		}

		protected void resultGrid_UpdateCommand(object source, DataGridCommandEventArgs e)
		{
			string transID = e.CommandArgument.ToString();

			BulkPaymentDetailContext context = this.GetContext();

			BulkPaymentInvoiceMappingClass mapping = context.GetMapping(transID);

			// populate mapping with the stuff on the current line item
			FMCheckBox cbRebate = e.Item.FindControl("cbRebateEdit") as FMCheckBox;
			if (cbRebate.Checked)
			{
				TextBox tbRebate = e.Item.FindControl("tbLiRebateNumber") as TextBox;
				mapping.RebateNumber = tbRebate.Text;
			}

			context.SetMapping(transID, mapping);
			this.StoreContext(context);

			this.EnableDisableControls(false);
			this.resultGrid.EditItemIndex = -1;

			this.UpdateView();
		}

		protected void BindControls()
		{
			this.btnDelete.Click += new EventHandler(btnDelete_Click);
			this.InvSelectionTextBox.TextChanged += new EventHandler(InvSelectionTextBox_TextChanged);
			this.btnApply.Click += new EventHandler(btnApply_Click);
			this.btnCancel.Click += new EventHandler(btnCancel_Click);
			this.btnViewPrintable.Click += new EventHandler(btnViewPrintable_Click);
			this.ddlPageSize.SelectedIndexChanged += new EventHandler(ddlPageSize_SelectedIndexChanged);
		}

		protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.UpdateView();
		}

		protected void btnViewPrintable_Click(object sender, EventArgs e)
		{
			string bulkRptType = ((int)ReportTypesClass.ReportTypes.ADF_BULK_RPT).ToString();
			string rptURL = "../FMReporting/ReportLandingPage.aspx?ReportType=" + bulkRptType;
			string reportName = "FESSPayment";
			rptURL = rptURL + "&ReportName=" + reportName;
			rptURL = rptURL + "&BulkPaymentID=" + this.GetContext().BulkPayment.BulkPaymentID.ToString();

			string javascriptPopupReport = "<script type='text/javascript'>\n<!-- \n" +
											"window.open('" + rptURL + "', " +
											"'Reports', " +
											"'status=0, toolbar=0, menubar=1, resizable=1, scrollbars=1, height=700, width=700'" +
											"); \n" +
											"-->\n</script>";

			Response.Cookies.Add(new HttpCookie("Token", Session["Token"] as String));
			ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "RPT_POPUP_NEW_BROWSER", javascriptPopupReport, false);
		}

		protected void btnDelete_Click(object sender, EventArgs e)
		{
			BulkPaymentDetailContext context = this.GetContext();

			if (m_mode != Mode.ADD)
			{
				BulkPaymentInvoiceMappingCollectionClass origMappings = 
					FMChannelHelper.MakeCall<IBulkPaymentInvoiceMappings, BulkPaymentInvoiceMappingCollectionClass>(
							x =>
							x.EnumerateByBulkPaymentID(base.security,context.BulkPayment.BulkPaymentID)
					);
				
				// first delete all the line items
				foreach (BulkPaymentInvoiceMappingClass mapping in origMappings)
				{
					this.RemoveMappings(base.security, mapping);
				}

				// now delete the bulk payment itself
				FMChannelHelper.MakeCall<IBulkPaymentInvoiceMappings>(
																	 x =>
																	 x.Remove(base.security, context.BulkPayment)
																);

			}

			// close the window - same as cancel behaviour
			btnCancel_Click(sender, e);
		}

		private void RemoveMappings(SecurityClass securityClass, BulkPaymentInvoiceMappingClass mapping)
		{
			FMChannelHelper.MakeCall<IBulkPaymentInvoiceMappings>(
																	 x =>
																	 x.Remove(securityClass, mapping)
																);

		}

		protected void btnCancel_Click(object sender, EventArgs e)
		{
			// clear the context
			this.StoreContext(new BulkPaymentDetailContext());

			// go back to the bulk payment summary
			this.Redirect("InvoicePaymentSummary.aspx?mode=" + (int)InvoicePaymentMode.PAYMENT);
		}

		protected void btnApply_Click(object sender, EventArgs e)
		{
			try
			{
				BulkPaymentDetailContext context = this.GetContext();

				// CCP-??? remember the date the roman payment number was changed
				if (context.BulkPayment.RomanNumber.Length == 0 && this.tbRomanNumber.Text.Length > 0)
				{
					context.BulkPayment.RomanNumberDate = DateTime.UtcNow;
				}

				// if there is a roman number, then pull in the latest currency price
				if (this.tbRomanNumber.Text.Length > 0)
				{
					this.ddlForeignCurrency_SelectedIndexChanged(sender, e);
				}

				object contextObj = context;
				context = this.LoadToContext(ref contextObj);
				this.StoreContext(context);

				if (m_mode == Mode.ADD)
				{
					FMChannelHelper.MakeCall<IBulkPaymentInvoiceMappings>(
																	 x =>
																	 x.Add(base.security, context.BulkPayment)
																);

					foreach (BulkPaymentInvoiceMappingClass mapping in context.BulkPayment.Mapping)
					{
						// fix it up
						mapping.BulkPaymentID = context.BulkPayment.BulkPaymentID;
						this.AddInvoiceMappings(base.security, mapping);
					}
				}
				else
				{
					// retrieve original and compare for missing existing line items
					BulkPaymentInvoiceMappingCollectionClass origMappings = 
						FMChannelHelper.MakeCall<IBulkPaymentInvoiceMappings, BulkPaymentInvoiceMappingCollectionClass>(
								x =>
								x.EnumerateByBulkPaymentID(base.security, context.BulkPayment.BulkPaymentID)
						);

					FMChannelHelper.MakeCall<IBulkPayments>(
																	 x =>
																	 x.Update(base.security, context.BulkPayment)
																);

					// now check which line items needs to be added, updated or deleted

					// ADDED
					foreach (string transID in context.NewTransIDs)
					{
						BulkPaymentInvoiceMappingClass mapping = context.GetMapping(transID);
						this.AddInvoiceMappings(base.security, mapping);
					}

					// UPDATE
					foreach (string transID in context.ExistingTransIDs)
					{
						BulkPaymentInvoiceMappingClass mapping = context.GetMapping(transID);
						this.UpdateInvoiceMappings(base.security, mapping);
					}

					// DELETE
					foreach (BulkPaymentInvoiceMappingClass mapping in origMappings)
					{
						// if original doesn't exist in existing transaction IDs then it should be deleted
						if (!context.ExistingTransIDs.Contains(mapping.InvoiceTransID))
						{
							this.RemoveInvoiceMappings(base.security, mapping);
						}
					}
				}

				// should reload to refresh changes and set the transaction mode to edit
				this.Redirect(BulkPaymentDetailPage.FILENAME + "?" + BulkPaymentDetailPage.EDIT_PARAM_NAME + "=" + context.BulkPayment.BulkPaymentID);
			}
			catch (Exception ex)
			{
				this.StoreContext(new BulkPaymentDetailContext());

				base.ErrorHandler(ex);
			}
		}

		private void RemoveInvoiceMappings(SecurityClass securityClass, BulkPaymentInvoiceMappingClass mapping)
		{
			FMChannelHelper.MakeCall<IBulkPaymentInvoiceMappings>(
																	 x =>
																	 x.Remove(securityClass, mapping)
																);
		}

		private void UpdateInvoiceMappings(SecurityClass securityClass, BulkPaymentInvoiceMappingClass mapping)
		{
			FMChannelHelper.MakeCall<IBulkPaymentInvoiceMappings>(
																	 x =>
																	 x.Update(securityClass, mapping)
																);
		}

		private void AddInvoiceMappings(SecurityClass securityClass, BulkPaymentInvoiceMappingClass mapping)
		{
			FMChannelHelper.MakeCall<IBulkPaymentInvoiceMappings>(
																	 x =>
																	 x.Add(securityClass, mapping)
																);
		}
		#endregion // Event Handling

		#region Helper Methods
		protected void CreateInvoiceCache()
		{
			BulkPaymentDetailContext context = this.GetContext();

			if (context.ExistingTransIDs.Count > 0)
			{
				return;
			}

			context.InvoiceCache.Clear();
			context.ExistingTransIDs.Clear();

			// caches all the invoice transactions associated within the bulk payment
			foreach (BulkPaymentInvoiceMappingClass mapping in context.BulkPayment.Mapping)
			{
				try
				{
					string transID = mapping.InvoiceTransID;
					context.ExistingTransIDs.Add(transID);

					TransactionSR sr = new TransactionSR();
					sr.Security = base.security;
					sr.TransID = transID;

					TransactionDO invoiceTransaction = FMChannelHelper.MakeCall<ITransactionProcessor, TransactionDO>(
																	 x =>
																	 x.Process(sr)
																);

					context.InvoiceCache.Add(transID, invoiceTransaction);

					this.StoreContext(context);
				}
				catch (Exception e)
				{
					base.ErrorHandler(e);
				}
			}
		}

		protected void PopulateControls()
		{
			BulkPaymentDetailContext context = this.GetContext();

			// association buttons
			//btnAssociateTop.Attributes["onclick"] = "javascript:InvoiceSelect('InvSelectionTextBox')";
			//btnAssociateBottom.Attributes["onclick"] = "javascript:alert('blah')";

			// Bulk Payment ID
			tbBulkPaymentID.Text = context.BulkPayment.BulkPaymentID.ToString();

			// Location
			tbLocation.Text = base.security.SiteID;

			// section
			Common.PopulateFilterDropDownList(base.security, ref this.ddlSection, Common.FilterTarget.SECTION, false);

			// Payment Type
			ddlPaymentType.Items.Add("None");
			ddlPaymentType.Items.Add("CP");
			ddlPaymentType.Items.Add("DC");
			ddlPaymentType.SelectByText("None");

			// Last Edited By
			if (m_mode != Mode.ADD)
				tbLastEdit.Text = context.BulkPayment.UpdatedBy;
			else
				tbLastEdit.Text = base.security.UserID;

			// Foreign Currency
			CurrencyDOCollectionClass currencyCol = FMChannelHelper.MakeCall<ICurrencies, CurrencyDOCollectionClass>(
																	 x =>
																	 x.GetCurrencies(this.security)
																);

			ddlForeignCurrency.Items.Add(new ListItem("None", Guid.Empty.ToString()));
			foreach (CurrencyDO currency in currencyCol)
			{
				ddlForeignCurrency.Items.Add(new ListItem(currency.UnitDisplayName, currency.IdentityGuid.ToString()));
			}

			// Payment Due Date & Transaction Date
			// fill dates with current date by default
			// convert date for display
			dtPaymentDueDate.Text = context.AcctSite.FormatDateTime(m_converter.ConvertToSiteTime(DateTime.UtcNow));
			dtTransactionDate.Text = dtPaymentDueDate.Text;

			// hide the transaction id column
			this.resultGrid.Columns[2].Visible = false;

			// set shipping
			this.tbSupplier.Role = "SUPPLIER";

			// foreign currency should be disabled if transaction has line items
			if (context.BulkPayment.Mapping.Count > 0)
			{
				this.ddlForeignCurrency.Enabled = false;
			}

			// if the bulk payment has been paid (i.e. roman number entered) then no more associations or applies
			if (context.BulkPayment.RomanNumberDate != DateTime.MaxValue)
			{
				btnApply.Enabled = false;
				//btnAssociateTop.Enabled = false;
				//btnAssociateBottom.Enabled = false;
			}

			// fields which require aggregation
			this.AggregateFinancialControls();
		}

		protected TransactionDO GetTransaction(BulkPaymentDetailContext a_context, string a_transID)
		{
			TransactionDO result = null;

			// first check the invoice cache
			if (a_context.InvoiceCache.ContainsKey(a_transID))
			{
				result = a_context.InvoiceCache[a_transID] as TransactionDO;
			}
			else
			{
				// retrieve and add to cache
				try
				{
					TransactionSR sr = new TransactionSR();
					sr.Security = base.security;
					sr.TransID = a_transID;

					TransactionDO trans = FMChannelHelper.MakeCall<ITransactionProcessor, TransactionDO>(
																	 x =>
																	 x.Process(sr)
																);

					// remember this in the cache so we don't have to retrieve it again in the future
					a_context.InvoiceCache[a_transID] = trans;
					result = trans;
				}
				catch (Exception e)
				{
					base.ErrorHandler(e);
				}
			}

			return result;
		}

		protected void AggregateFinancialControls()
		{
			double totalExcise = 0.0;
			double totalGST = 0.0;
			double totalOnCost = 0.0;
			double totalGrand = 0.0;
			double totalForeign = 0.0;
			double totalPaid = 0.0;

			string productName_junk = "";
			double quantity_junk = 0.0;

			BulkPaymentDetailContext context = this.GetContext();

			List<string> allTransIDList = new List<string>(context.ExistingTransIDs);
			allTransIDList.AddRange(context.NewTransIDs);

			foreach (string curTransID in allTransIDList)
			{
				TransactionDO trans = this.GetTransaction(context, curTransID);

				double curExcise = 0.0;
				double curGST = 0.0;
				double curOnCost = 0.0;
				double curGrand = 0.0;
				double foreign = 0.0;

				AggregateInvoiceLineItems(trans, security, ref productName_junk, ref quantity_junk, ref foreign,
					ref curExcise, ref curGST, ref curOnCost, ref curGrand);

				totalExcise += curExcise;
				totalGST += curGST;
				totalOnCost += curOnCost;
				totalGrand += curGrand;
				totalForeign += foreign;
			}

			// set the fields
			tbExcise.Text = Math.Round(totalExcise, 2, MidpointRounding.AwayFromZero).ToString();
			tbGST.Text = Math.Round(totalGST, 2, MidpointRounding.AwayFromZero).ToString();
			tbOnCost.Text = Math.Round(totalOnCost, 2, MidpointRounding.AwayFromZero).ToString();
			tbTotal.Text = Math.Round(totalGrand, 2, MidpointRounding.AwayFromZero).ToString();
			tbTotalForeign.Text = Math.Round(totalForeign, 2, MidpointRounding.AwayFromZero).ToString();

			// work out total AUD paid
			try
			{
				if (context.BulkPayment.ForeignRate != 0)
				{
					totalPaid = totalForeign / context.BulkPayment.ForeignRate;
					tbTotalPaid.Text = Math.Round(totalPaid, 2, MidpointRounding.AwayFromZero).ToString();
				}
			}
			catch (Exception) { }
		}

		public static void AggregateInvoiceLineItems(TransactionDO a_invoiceTrans, SecurityClass a_security,
			ref string a_productNames, ref double a_quantity, ref double a_foreignTotal,
			ref double a_excise, ref double a_gst, ref double a_oncost, ref double a_totalAmount)
		{
			a_totalAmount = 0.0;
			a_oncost = 0.0;
			for (int i = 0; i < a_invoiceTrans.LineItems.Count; ++i)
			{
				LineItemDO resultLineItem = Common.AggregateLineItemValues(a_security, a_invoiceTrans, i);

				if (a_productNames.Length > 0)
					a_productNames += ", ";
				a_productNames += resultLineItem.Product;

				a_quantity += resultLineItem.Quantity.Gross;
				a_excise += resultLineItem.Tax1.Value;
				a_gst += resultLineItem.Tax2.Value;
				a_totalAmount += resultLineItem.TotalPriceWithTax;

				try
				{
					a_foreignTotal += double.Parse(resultLineItem.UserData["TALUD3"].ToString());
				}
				catch (Exception) { }
				try
				{
					a_oncost += double.Parse(resultLineItem.UserData["TALUD14"].ToString());
				}
				catch (Exception) { }
			}
		}

		protected DataView BuildDataView(BulkPaymentDetailContext a_context)
		{
			DataView result = new DataView();

			DataTable table = new DataTable();
			table.Columns.Add("InvoiceTransID", typeof(string));
			table.Columns.Add("Product", typeof(string));
			table.Columns.Add("InvoiceNumber", typeof(string));
			table.Columns.Add("Quantity", typeof(string));
			table.Columns.Add("RebateChecked", typeof(bool));
			table.Columns.Add("RebateNumber", typeof(string));
			table.Columns.Add("AccountCode", typeof(string));
			table.Columns.Add("CostCentreCode", typeof(string));
			table.Columns.Add("ForeignTotal", typeof(string));
			table.Columns.Add("Excise", typeof(string));
			table.Columns.Add("GST", typeof(string));
			table.Columns.Add("OnCost", typeof(string));
			table.Columns.Add("TotalPrice", typeof(string));

			List<string> allTransIDList = a_context.ExistingTransIDs;
			allTransIDList.AddRange(a_context.NewTransIDs);

			foreach (string transID in allTransIDList)
			{
				TransactionDO invoiceTrans = this.GetTransaction(a_context, transID);

				DataRow row = table.NewRow();

				// get product, quantity, foreign total, excise, gst, oncost and total from the line items
				string productNames = "";
				double quantity = 0.0;
				double foreignTotal = 0.0;
				double excise = 0.0;
				double gst = 0.0;
				double oncost = 0.0;
				double totalAmount = 0.0;

				AggregateInvoiceLineItems(invoiceTrans, security,
						ref productNames, ref quantity, ref foreignTotal, ref excise, ref gst, ref oncost, ref totalAmount);

				// some stuff is stored in the mappings
				BulkPaymentInvoiceMappingClass mapping = a_context.GetMapping(transID);

				row["InvoiceTransID"] = transID;
				row["Product"] = productNames;
				//if (null == invoiceTrans.UserData3) // invoice number
				if (null == invoiceTrans.DocumentNumber)
					row["InvoiceNumber"] = "";
				else
					row["InvoiceNumber"] = invoiceTrans.DocumentNumber;
				//row["InvoiceNumber"] = invoiceTrans.UserData3;
				row["Quantity"] = quantity.ToString();

				// check if rebate exists for the current item
				row["RebateChecked"] = mapping.RebateNumber.Length > 0;
				row["RebateNumber"] = mapping.RebateNumber;
				row["AccountCode"] = invoiceTrans.UserData13; // account code
				row["CostCentreCode"] = invoiceTrans.UserData1; // cost centre code
				row["ForeignTotal"] = String.Format("{0:c}", Math.Round(foreignTotal, 2, MidpointRounding.AwayFromZero));
				row["Excise"] = String.Format("{0:c}", Math.Round(excise, 2, MidpointRounding.AwayFromZero));
				row["GST"] = String.Format("{0:c}", Math.Round(gst, 2, MidpointRounding.AwayFromZero));
				row["OnCost"] = String.Format("{0:c}", Math.Round(oncost, 2, MidpointRounding.AwayFromZero));
				row["TotalPrice"] = String.Format("{0:c}", Math.Round(totalAmount, 2, MidpointRounding.AwayFromZero));

				table.Rows.Add(row);
			}

			result = new DataView(table);

			return result;
		}

		protected bool SecurityProcessing()
		{
			if (null == Session["Security"] || null == base.security)
			{
				Session.RemoveAll();
				this.DisplayErrorPage();
			}
			bool returnVal = security.HasViewTransactionRightByTransTypeID(TransactionTypes.T21_AccountPayableInvoice);
			return returnVal;
		}
		#endregion // Helper Methods

		#region Context Overrides
		public override BulkPaymentDetailContext GetContext()
		{
			BulkPaymentDetailContext existingContext = Session[BulkPaymentDetailContext.CONTEXT_KEY] as BulkPaymentDetailContext;
			if (null == existingContext)
			{
				existingContext = new BulkPaymentDetailContext();
			}

			return existingContext;
		}

		public override void StoreContext(object a_context)
		{
			BulkPaymentDetailContext context = a_context as BulkPaymentDetailContext;
			if (null != context)
			{
				// always remove duplicates from context
				List<string> existing = new List<string>();
				List<string> added = new List<string>();

				foreach (string transID in context.ExistingTransIDs)
				{
					if (!existing.Contains(transID) && !context.NewTransIDs.Contains(transID))
						existing.Add(transID);
				}

				foreach (string transID in context.NewTransIDs)
				{
					if (!added.Contains(transID))
						added.Add(transID);
				}

				context.ExistingTransIDs = existing;
				context.NewTransIDs = added;

				Session[context.GetKey()] = context;
			}
		}

		public override void LoadFromContext(BulkPaymentDetailContext a_context)
		{
			// shortcut
			BulkPaymentClass payment = a_context.BulkPayment;

			// Bulk Payment ID
			tbBulkPaymentID.Text = payment.BulkPaymentID.ToString();

			// Location
			//tbLocation.Text = payment.SiteID;

			// Payment Type
			if (payment.PaymentType.Length > 0)
				ddlPaymentType.SelectByText(payment.PaymentType);

			// Section
			if (payment.Section.Length > 0)
				ddlSection.SelectByText(payment.Section);

			// Foreign Currency Unit
			if (payment.ForeignUnit.Length > 0)
				ddlForeignCurrency.SelectByText(payment.ForeignUnit);

			// Foreign Currency Rate
			tbForeignRate.Text = payment.ForeignRate.ToString();

			// Payment Due Date & Transaction Date
			dtPaymentDueDate.Text = a_context.AcctSite.FormatDateTime(
				  m_converter.ConvertToSiteTime(payment.PaymentDueDate));
			dtTransactionDate.Text = a_context.AcctSite.FormatDateTime(
				  m_converter.ConvertToSiteTime(payment.TransactionDate));

			// ROMAN
			tbRomanNumber.Text = payment.RomanNumber;

			// Discount Rate
			tbDiscountRate.Text = payment.DiscountRate.ToString();

			// Supplier
			tbSupplier.Text = payment.Supplier;
		}

		public override BulkPaymentDetailContext LoadToContext(ref object a_context)
		{
			// shortcut
			BulkPaymentClass payment = (a_context as BulkPaymentDetailContext).BulkPayment;

			if (ddlPaymentType.SelectedIndex != -1)
				payment.PaymentType = ddlPaymentType.SelectedItem.Text;
			if (ddlSection.SelectedIndex != -1)
				payment.Section = ddlSection.SelectedItem.Text;
			if (ddlForeignCurrency.SelectedIndex != -1)
				payment.ForeignUnit = ddlForeignCurrency.SelectedItem.Text;
			if (tbForeignRate.Text.Length > 0)
				payment.ForeignRate = double.Parse(tbForeignRate.Text);
			if (dtPaymentDueDate.Text.Length > 0)
				payment.PaymentDueDate = m_converter.ConvertFromSiteTime(dtPaymentDueDate.CurrentValue);
			if (dtTransactionDate.Text.Length > 0)
				payment.TransactionDate = m_converter.ConvertFromSiteTime(dtTransactionDate.CurrentValue);
			if (tbDiscountRate.Text.Length > 0)
				payment.DiscountRate = double.Parse(tbDiscountRate.Text);

			payment.RomanNumber = tbRomanNumber.Text;
			payment.SiteID = tbLocation.Text;
			payment.Supplier = tbSupplier.Text;

			BulkPaymentDetailContext context = a_context as BulkPaymentDetailContext;
			context.BulkPayment = payment;

			return context;
		}
		#endregion // Context Overrides
	}
}
