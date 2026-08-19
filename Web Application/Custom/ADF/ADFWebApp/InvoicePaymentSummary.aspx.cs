using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Reflection;

using Accounting;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.UtilityObjects;
using FMBusinessObjects.Exceptions;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.ServiceRequests;

namespace ADFWebApp
{
	#region Context
	public class InvoicePaymentContext : BaseContext
	{
		#region Properties
		public string InvoiceNumber { get; set; }
		public string FuelType { get; set; }
		public string AccountCode { get; set; }
		public string CostCentreCode { get; set; }
		public string EnteredBy { get; set; }
		public string QueryNumber { get; set; }
		public string PaymentID { get; set; }
		public string Supplier { get; set; }
		public string Section { get; set; }
		public InvoicePaymentMode Mode { get; set; }
		#endregion // Properties

		protected static string CONTEXT_KEY = typeof(InvoicePaymentContext).ToString();

		public InvoicePaymentContext()
			: base()
		{
			this.Mode = InvoicePaymentMode.NONE;

			this.ResetContextProperties();
		}

		public override void ResetContextProperties()
		{
			base.ResetContextProperties();

			this.InvoiceNumber = "";
			this.FuelType = "";
			this.AccountCode = "";
			this.CostCentreCode = "";
			this.EnteredBy = "";
			this.QueryNumber = "";
			this.PaymentID = "";
			this.Supplier = "";
			this.Section = "";
		}

		public override string GetKey()
		{
			return InvoicePaymentContext.CONTEXT_KEY;
		}
	}
	#endregion // Context

	public enum InvoicePaymentColumns : int
	{
		// must be in order
		PAYMENT_ID = 0,
		INVOICE_NUMBER,
		ORDER_NUMBER,
		QUANTITY,
		SUPPLIER,
		TOTAL_AMOUNT,
		SECTION,
		ENTERED_BY,
		INVOICE_QUERY,
		ACTION_REQUIRED,
		TRANS_ID
	}

	public enum InvoicePaymentMode
	{
		NONE,
		INVOICE = TransactionTypes.T21_AccountPayableInvoice,
		PAYMENT
	}

	public partial class InvoicePaymentSummary : BaseContextPage<InvoicePaymentContext>, IDataDictionary
	{
		#region Attributes
		public static string FILENAME = "InvoicePaymentSummary.aspx";

		protected static Hashtable ColumnNames = new Hashtable()
		{
			{InvoicePaymentColumns.PAYMENT_ID, "PaymentID"},
			{InvoicePaymentColumns.INVOICE_NUMBER, "InvoiceNumber"},
			{InvoicePaymentColumns.ORDER_NUMBER, "OrderNumber"},
			{InvoicePaymentColumns.QUANTITY, "Quantity"},
			{InvoicePaymentColumns.SUPPLIER, "Supplier"},
			{InvoicePaymentColumns.TOTAL_AMOUNT, "TotalAmount"},
			{InvoicePaymentColumns.SECTION, "Section"},
			{InvoicePaymentColumns.ENTERED_BY, "EnteredBy"},
			{InvoicePaymentColumns.INVOICE_QUERY, "InvoiceQuery"},
			{InvoicePaymentColumns.ACTION_REQUIRED, "ActionRequired" },
			{InvoicePaymentColumns.TRANS_ID, "TransID"}
		};

		protected static List<InvoicePaymentColumns> InvoiceSummaryColumns = new List<InvoicePaymentColumns>()
		{
			InvoicePaymentColumns.INVOICE_NUMBER,
			InvoicePaymentColumns.ORDER_NUMBER,
			InvoicePaymentColumns.QUANTITY,
			InvoicePaymentColumns.SUPPLIER,
			InvoicePaymentColumns.TOTAL_AMOUNT,
			InvoicePaymentColumns.SECTION,
			InvoicePaymentColumns.ENTERED_BY,
			InvoicePaymentColumns.INVOICE_QUERY,
			InvoicePaymentColumns.ACTION_REQUIRED
		};
		protected static List<InvoicePaymentColumns> PaymentSummaryColumns = new List<InvoicePaymentColumns>()
		{
			InvoicePaymentColumns.PAYMENT_ID,
			InvoicePaymentColumns.QUANTITY,
			InvoicePaymentColumns.SUPPLIER,
			InvoicePaymentColumns.TOTAL_AMOUNT,
			InvoicePaymentColumns.ENTERED_BY
		};

		CollectionBase m_collection;
		BulkPaymentCollectionClass m_paymentCollection = new BulkPaymentCollectionClass();

		SiteTimeConverter m_converter;
		#endregion // Attributes

		#region Constructor
		public InvoicePaymentSummary()
			: base(new InvoicePaymentContext())
		{
		}
		#endregion

		#region Events
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				// setup accounting
				base.security = Session["Security"] as SecurityClass;
				if (base.security == null)
				{
					throw new FMSessionInvalidException();
				}
				if (null == Request.Params["mode"])
				{
					throw new System.AccessViolationException("Access Denied");
				}

				// initialise sites for context
				InvoicePaymentContext context = this.GetContext();
				context.AcctSite = FMChannelHelper.MakeCall<IAccountingSites, AccountingSite>(
																	 x =>
																	 x.LoadSiteInfo(base.security, base.security.SiteGuid)
																);

				m_converter = new SiteTimeConverter(context.AcctSite.CurrentSite);

				// initialise mode for context
				context.Mode = (InvoicePaymentMode)int.Parse(Request.Params["mode"]);
				if (InvoicePaymentMode.NONE == context.Mode)
				{
					throw new System.AccessViolationException("Access Denied");
				}
				// store it back to session
				this.StoreContext(context);

				context = this.GetContext();

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
					// populate dropdowns
					this.PopulateControls();
					this.LoadFromContext(context);

					this.BuildDictionaryLabels();

					object store = context;
					context = this.LoadToContext(ref store);

					UpdateView();
				}
			}
			catch (Exception ex)
			{
				base.ErrorHandler(ex);
			}
		}

		protected void btnShowAll_Click(object sender, EventArgs e)
		{
			InvoicePaymentContext context = this.GetContext();

			context.InvoiceNumber = "";
			context.FuelType = "";
			context.AccountCode = "";
			context.CostCentreCode = "";
			context.EnteredBy = "";
			context.QueryNumber = "";
			context.PaymentID = "";
			context.Supplier = "";
			context.Section = "";

			this.LoadFromContext(context);
			this.StoreContext(context);

			this.UpdateView();
		}

		protected void btnRefresh_Click(object sender, EventArgs e)
		{
			try
			{
				Common.RefreshPreProcessing(ref this.resultGrid,
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

		protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.UpdateView();
		}

		protected void BindControls()
		{
			this.btnRefresh.Click += new EventHandler(btnRefresh_Click);
			this.ddlPageSize.SelectedIndexChanged += new EventHandler(ddlPageSize_SelectedIndexChanged);
			this.btnAddTop.Click += new EventHandler(btnAddTop_Click);
			this.btnAddBottom.Click += new EventHandler(btnAddBottom_Click);
		}

		protected void btnAddBottom_Click(object sender, EventArgs e)
		{
			this.AddButtonProcessing(sender, e);
		}

		protected void btnAddTop_Click(object sender, EventArgs e)
		{
			this.AddButtonProcessing(sender, e);
		}

		protected void AddButtonProcessing(object sender, EventArgs e)
		{
			InvoicePaymentContext context = this.GetContext();

			if (security.HasModifyTransactionRightByTransTypeID(TransactionTypes.T21_AccountPayableInvoice))
			{
				if (context.Mode == InvoicePaymentMode.PAYMENT)
				{
					Session[BulkPaymentDetailContext.CONTEXT_KEY] = new BulkPaymentDetailContext();
					this.Redirect("../ADFWebApp/" + BulkPaymentDetailPage.FILENAME);
				}
				else if (context.Mode == InvoicePaymentMode.INVOICE)
				{
					this.Redirect("../ADFWebApp/TransactionDetail.aspx?TransactionDetailMode=ADD&TransAlias=Invoice");
				}
				else
				{
					base.ErrorHandler(new Exception("Unsupported mode of operation)"));
				}
			}
		}

		#region Line Items
		protected void InitializeComponent()
		{
			// bind data grid events
			this.resultGrid.ItemDataBound += new DataGridItemEventHandler(resultGrid_ItemDataBound);
			this.resultGrid.PageIndexChanged += new DataGridPageChangedEventHandler(resultGrid_PageIndexChanged);
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

		protected void resultGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			// add the edit button
			try
			{
				if (e.Item.ItemIndex != -1)
				{
					FMControls.FMEditLinkButton btn = e.Item.FindControl("EditLinkButton") as FMControls.FMEditLinkButton;
					if (btn != null)
					{
						InvoicePaymentContext context = this.GetContext();

						string transID = (string)((System.Data.DataRowView)(((System.Web.UI.WebControls.DataGridItem)(e.Item.Cells[8].BindingContainer)).DataItem)).Row.ItemArray[this.resultGrid.Columns.Count - 2];

						if (context.Mode == InvoicePaymentMode.PAYMENT)
						{
							Session[BulkPaymentDetailContext.CONTEXT_KEY] = new BulkPaymentDetailContext();
							btn.Attributes["href"] = "BulkPaymentDetailPage.aspx?" + BulkPaymentDetailPage.EDIT_PARAM_NAME + "=" + transID;
						}
						else
						{
							btn.Attributes["href"] = "TransactionDetail.aspx" +
								"?TransID=" + transID +
								"&" + TransactionDetail.CUSTOM_REDIRECT_PARAM + "=InvoicePaymentSummary.aspx?mode=" + (int)context.Mode;
						}
					}
				}
			}
			catch (Exception ex)
			{
				base.ErrorHandler(ex);
			}
		}

		#endregion // Line Items

		#endregion // Events

		#region Data Dictionary
		string[] IDataDictionary.Keys(SecurityClass a_security)
		{
			string[] keys = 
			{
				// header
				"Invoice Summary",
				"Bulk Payment Summary",
				"FM Invoice Number",
				"Product",
				"Account Code",
				"Entered By",
				"Start Date",
				"End Date",
				"Invoice Query",
				"Payment ID",
				"Supplier",
				"Section",
				// line item
				"Edit",
				"Payment ID",
				"Cost Centre Code",
				"Invoice Number",
				"Order Number",
				"Quantity",
				"Total Amount",
				"Section",
				"Action Required",
				// buttons
				"Add",
				"Show All",
				"Refresh"
			};
			return keys;
		}

		protected void BuildDictionaryLabels()
		{
			InvoicePaymentContext context = GetContext();
			if (context.Mode == InvoicePaymentMode.INVOICE)
			{
				lblHeading.Text = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(base.security.SiteGuid, "Invoice Summary")
																);

				lblPaymentID.Text = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(base.security.SiteGuid, "Cost Centre Code")
																);

			}
			else if (context.Mode == InvoicePaymentMode.PAYMENT)
			{
				lblHeading.Text = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(base.security.SiteGuid, "Bulk Payment Summary")
																);

				lblPaymentID.Text = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(base.security.SiteGuid, "Payment ID")
																);

			}

			lblInvoiceNumber.Text = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(base.security.SiteGuid, "FM Invoice Number")
																);

			lblFuelType.Text = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(base.security.SiteGuid, "Product")
																);

			lblAccountCode.Text = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(base.security.SiteGuid, "Account Code")
																);

			lblEnteredBy.Text = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(base.security.SiteGuid, "Entered By")
																);

			lblStartDate.Text = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(base.security.SiteGuid, "Start Date")
																);

			lblEndDate.Text = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(base.security.SiteGuid, "End Date")
																);

			lblInvoiceQuery.Text = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
                                                    x =>
                                                    x.Get(base.security.SiteGuid, "Invoice Query") 
                                                );

			lblSupplier.Text = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
                                                    x =>
                                                    x.Get(base.security.SiteGuid, "Supplier") 
                                                );

			lblSection.Text = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
                                                    x =>
                                                    x.Get(base.security.SiteGuid, "Section") 
                                                );
		}
		#endregion // Data Dictionary

		#region Update View
		protected void UpdateView()
		{
			try
			{
				InvoicePaymentContext context = this.GetContext();

				// manage labels depending on summary type
				if (context.Mode == InvoicePaymentMode.INVOICE)
				{
					//lblPaymentID.Text = "Cost Centre Code";
					//lblHeading.Text = "Invoice Summary";

					tbPaymentID.Visible = false;
					ddlCostCentreCode.Visible = true;
				}
				else
				{
					//lblPaymentID.Text = "Payment ID";
					//lblHeading.Text = "Bulk Payment Summary";

					tbPaymentID.Visible = true;
					ddlCostCentreCode.Visible = false;
				}

				// get the collection using the context
				int collectionSize = 0;
				if (context.Mode == InvoicePaymentMode.INVOICE)
				{
					this.m_collection = this.EnumerateByContext(context);
					collectionSize = (m_collection as TransactionDOCollection).Count;
				}
				else
				{
					this.m_collection = this.EnumeratePayments(context);
					collectionSize = (m_collection as BulkPaymentCollectionClass).Count;
				}

				DataView dw = this.BuildDataView(m_collection);

				this.ddlPageSize.SetPageSize(this.resultGrid, collectionSize);

				this.resultGrid.DataSource = dw;
				this.resultGrid.DataBind();
			}
			catch (Exception e)
			{
				base.ErrorHandler(e);
			}
		}

		public TransactionFilterSR FilterBuilder(ICustomContext a_context)
		{
			InvoicePaymentContext context = (InvoicePaymentContext)a_context;
			if (null == context)
			{
				throw new Exception("could not convert " + a_context.ToString() + " to InvoicePaymentContext");
			}

			TransactionFilterSR sr = new TransactionFilterSR();
			DateTime utcStartDate = m_converter.ConvertToSiteTime(context.StartDate).AddDays(1).AddMilliseconds(-1);
			DateTime utcEndDate = m_converter.ConvertToSiteTime(context.EndDate).AddDays(2).AddMilliseconds(-1);

			// note to self, need to do this properly eventually on the db
			sr.Security = base.security;
			sr.TransTypeID = (TransactionTypes)context.Mode;
			sr.StartDateInventory = m_converter.ConvertFromSiteTime(utcStartDate);
			sr.EndDateInventory = m_converter.ConvertFromSiteTime(utcEndDate); // wind to end of day +1
			sr.SupplierID = context.Supplier;
			if (context.QueryNumber.Length > 0)
			{
				sr.InvoiceQuery = int.Parse(context.QueryNumber);
			}

			sr.UpdatedBy = context.EnteredBy;

			if (context.Mode == InvoicePaymentMode.INVOICE)
				sr.DocumentNumber = context.InvoiceNumber;
			else if (context.Mode == InvoicePaymentMode.PAYMENT)
				sr.DocumentNumber = context.PaymentID;

			return sr;
		}

		public bool FilterResults(TransactionDO a_trans)
		{
			InvoicePaymentContext context = this.GetContext();

			// payment ID field is cost centre code in invoice summary
			bool returnVal = true;

			// section
			returnVal &= context.Section.Length == 0 ||
				a_trans.UserData[Common.USERDATA_SECTION_KEY].ToString().ToUpper().Contains(context.Section.ToUpper());

			// account code
			returnVal &= context.AccountCode.Length == 0 || context.AccountCode.Equals(Common.DDL_ALL) ||
				a_trans.UserData[Common.USERDATA_ACCTCODE_KEY].ToString().ToUpper().Contains(context.AccountCode.ToUpper());

			// cost centre code
			returnVal &= context.CostCentreCode.Length == 0 || context.CostCentreCode.Equals(Common.DDL_ALL) ||
					a_trans.UserData[Common.USERDATA_COSTCENTRE_KEY].ToString().ToUpper().Contains(context.CostCentreCode.ToUpper());

			// supplier
			returnVal &= context.Supplier.Length == 0 ||
				a_trans.SupplierID.ToString().ToUpper().Equals(context.Supplier.ToUpper());

			// last edited by
			returnVal &= context.EnteredBy.Length == 0 || context.EnteredBy.Equals("{All}") ||
				a_trans.UpdatedBy.ToUpper().Equals(context.EnteredBy.ToUpper());

			// if invoice summary, should match cost centre code in user data
			if (context.Mode == InvoicePaymentMode.INVOICE)
			{
				// invoice query
				if (a_trans.Number04 != null)
				{
					try
					{
						returnVal &= context.QueryNumber.Length == 0 ||
							a_trans.Number04.Value == double.Parse(context.QueryNumber);
					}
					catch (Exception) { }
				}

				returnVal &= context.InvoiceNumber.Length == 0 ||
					a_trans.DocumentNumber.Equals(context.InvoiceNumber);
			}

			// product
			if (context.FuelType.Length != 0)
			{
				bool found = false;
				foreach (LineItemDO lineItem in a_trans.LineItems)
				{
					if (lineItem.Product.ToUpper().Equals(context.FuelType.ToUpper()))
					{
						found = true;
						break;
					}
				}

				returnVal = found;
			}

			if (!returnVal)
			{
				bool foundPaymentID = context.Mode != InvoicePaymentMode.PAYMENT;
				bool foundFuelType = false;

				bool foundAll = false;

				foreach (LineItemDO li in a_trans.LineItems)
				{
					// if bulk payment, then should match invoice number to line items
					if (context.Mode == InvoicePaymentMode.PAYMENT && !foundPaymentID)
					{
						foundPaymentID = li.InvoiceNumber.ToUpper().Equals(context.InvoiceNumber.ToUpper());
					}
					// fuel type
					if (!foundFuelType)
					{
						foundFuelType = li.Product.ToUpper().Equals(context.FuelType.ToUpper());
					}

					foundAll = foundPaymentID && foundFuelType;
					if (foundAll)
					{
						break;
					}
				}

				returnVal &= foundAll;
			}

			return returnVal;
		}

		protected BulkPaymentCollectionClass EnumeratePayments(InvoicePaymentContext a_context)
		{
			BulkPaymentCollectionClass result = new BulkPaymentCollectionClass();

			BulkPaymentFilter filter = new BulkPaymentFilter();

			// create the filter from our context
			filter.InvoiceNumber = a_context.InvoiceNumber;
			filter.FuelType = a_context.FuelType;
			filter.AccountCode = a_context.AccountCode;
			filter.EnteredBy = a_context.EnteredBy;
			filter.InvoiceNumber = a_context.QueryNumber;
			filter.PaymentID = a_context.PaymentID;
			filter.Supplier = a_context.Supplier;

			filter.StartDate = a_context.StartDate;
			filter.EndDate = a_context.EndDate.AddDays(1.0);

			BulkPaymentCollectionClass col = FMChannelHelper.MakeCall<IBulkPayments, BulkPaymentCollectionClass>(
																	 x =>
																	 x.EnumerateByFilter(base.security, filter)
																);
			// check filter matching
			foreach (BulkPaymentClass payment in col)
			{
				bool addToResult = true;

				addToResult &= (payment.Supplier.ToUpper().Equals(a_context.Supplier.ToUpper()) || a_context.Supplier.Length == 0) &&
					(payment.BulkPaymentID.ToString().Equals(a_context.PaymentID) || a_context.PaymentID.Length == 0) &&
					// invoice & acct code & fuel type & invoice number done separately
					(payment.CreatedBy.ToUpper().Equals(a_context.EnteredBy.ToUpper()) || a_context.EnteredBy.Length == 0 || a_context.EnteredBy.Equals("{All}")) &&
					(payment.Section.ToUpper().Equals(a_context.Section.ToUpper()) || a_context.Section.Length == 0);


				// load the mappings for the bulk payment
				payment.Mapping = FMChannelHelper.MakeCall<IBulkPaymentInvoiceMappings , BulkPaymentInvoiceMappingCollectionClass>(
																	 x =>
																	 x.EnumerateByBulkPaymentID(base.security, payment.BulkPaymentID)
																);

				addToResult &= payment.Mapping.Count > 0 ||
					(a_context.AccountCode.Length == 0 && a_context.FuelType.Length == 0 && a_context.InvoiceNumber.Length == 0);
				foreach (BulkPaymentInvoiceMappingClass mapping in payment.Mapping)
				{
					try
					{
						TransactionSR sr = new TransactionSR();
						sr.Security = base.security;
						sr.TransID = mapping.InvoiceTransID;

						TransactionDO invoiceTrans = this.ProcessTransProcessor(sr);
						if (invoiceTrans != null)
						{
							addToResult &= ((invoiceTrans.DocumentNumber.Equals(a_context.InvoiceNumber) || a_context.InvoiceNumber.Length == 0) &&
								(invoiceTrans.UserData[Common.USERDATA_ACCTCODE_KEY].ToString().ToUpper().Equals(a_context.AccountCode.ToUpper()) || a_context.AccountCode.Equals(Common.DDL_ALL) || a_context.AccountCode.Length == 0));


							// now check the invoice line items for product
							if (a_context.FuelType.Length != 0)
							{
								bool found = false;
								foreach (LineItemDO lineItem in invoiceTrans.LineItems)
								{
									if (lineItem.Product.ToString().ToUpper().Equals(a_context.FuelType.ToUpper()))
									{
										found = true;
									}
								}

								addToResult = found;
							}
						}
					}
					catch (Exception) { }
				}

				if (addToResult)
				{
					result.Add(payment);
				}
			}

			return result;
		}

		private TransactionDO ProcessTransProcessor(TransactionSR sr)
		{
			return FMChannelHelper.MakeCall<ITransactionProcessor, TransactionDO>(
																	 x =>
																	 x.Process(sr)
																);
		}

		protected TransactionDOCollection EnumerateByContext(InvoicePaymentContext a_context)
		{
			TransactionDOCollection result = new TransactionDOCollection();

			try
			{
				result = Common.EnumerateByContext(a_context, base.security, a_context.AcctSite,
						new Common.FilterBuilderDelegate(FilterBuilder),
						new Common.InlineFilterDelegate(FilterResults));
			}
			catch (Exception e)
			{
				base.ErrorHandler(e);
			}

			return result;
		}

		protected DataView BuildDataView(CollectionBase a_collection)
		{
			DataView result = null;

			// grab the context, we'll need the mode of operation
			InvoicePaymentContext context = this.GetContext();

			// create a matching table
			DataTable table = new DataTable();

			// work out shown columns
			List<InvoicePaymentColumns> shownColumns = new List<InvoicePaymentColumns>();
			if (context.Mode == InvoicePaymentMode.PAYMENT)
			{
				shownColumns = InvoicePaymentSummary.PaymentSummaryColumns;
			}
			else if (context.Mode == InvoicePaymentMode.INVOICE)
			{
				shownColumns = InvoicePaymentSummary.InvoiceSummaryColumns;
			}
			shownColumns.Sort(); // default sort should sort the keys in ASC order

			//foreach (InvoicePaymentColumns col in shownColumns)
			foreach (InvoicePaymentColumns enumValue in Enum.GetValues(typeof(InvoicePaymentColumns)))
			{
				table.Columns.Add(ColumnNames[enumValue].ToString(), typeof(string));
				this.resultGrid.Columns[(int)enumValue + 1].Visible = shownColumns.Contains(enumValue);
			}
			foreach (Object obj in a_collection)
			{
				DataRow row = table.NewRow();

				TransactionDO trans = null;

				if (context.Mode == InvoicePaymentMode.INVOICE)
					trans = obj as TransactionDO;

				double totalQuantity = 0.0;
				double totalAmount = 0.0;

				// calculate total amounts
				if (context.Mode == InvoicePaymentMode.INVOICE)
				{
					List<string> alreadySummed = new List<string>();

					// need the take price from associated receipt/dfp/commercial
					for (int index = 0; index < trans.LineItems.Count; ++index)
					{
						LineItemDO resultLineItem = Common.AggregateLineItemValues(security, trans, index);

						totalQuantity += resultLineItem.Quantity.Gross;
						totalAmount += resultLineItem.TotalPriceWithTax;
					}
				}
				else // bulk payments
				{
					BulkPaymentClass payment = obj as BulkPaymentClass;

					payment.Mapping = FMChannelHelper.MakeCall<IBulkPaymentInvoiceMappings, BulkPaymentInvoiceMappingCollectionClass>(
																	 x =>
																	 x.EnumerateByBulkPaymentID(base.security, payment.BulkPaymentID)
																);
					// workout nets
					foreach (BulkPaymentInvoiceMappingClass mapping in payment.Mapping)
					{
						try
						{
							TransactionSR sr = new TransactionSR();
							sr.Security = base.security;
							sr.TransID = mapping.InvoiceTransID;

							TransactionDO invoiceTrans = FMChannelHelper.MakeCall<ITransactionAliasListProcessor, TransactionDO>(
																	 x =>
																	 x.Process(sr)
																);

							string dummyString = "";
							double dummyDouble = 0.0;
							double quantity = 0.0;
							double amount = 0.0;

							BulkPaymentDetailPage.AggregateInvoiceLineItems(invoiceTrans, security,
								ref dummyString, ref quantity, ref dummyDouble, ref dummyDouble, ref dummyDouble, ref dummyDouble, ref amount);

							totalQuantity += quantity;
							totalAmount += amount;
						}
						catch (Exception ex)
						{
							base.ErrorHandler(ex);
						}
					}
				}

				// bind data to columns
				foreach (InvoicePaymentColumns enumValue in Enum.GetValues(typeof(InvoicePaymentColumns)))
				{
					if (!shownColumns.Contains(enumValue))
					{
						// invisible columns doesn't need anything
						row[ColumnNames[enumValue] as string] = "";
						continue;
					}

					// else...

					if (enumValue == InvoicePaymentColumns.QUANTITY)
					{
						row[ColumnNames[enumValue] as string] = Math.Round(totalQuantity, 0, MidpointRounding.AwayFromZero).ToString();
					}
					else if (enumValue == InvoicePaymentColumns.TOTAL_AMOUNT)
					{
						row[ColumnNames[enumValue] as string] = String.Format("{0:c}", Math.Round(totalAmount, 2, MidpointRounding.AwayFromZero));
					}
					else
					{
						// couldn't get dynamic evokes working so here we go...
						if (enumValue == InvoicePaymentColumns.ACTION_REQUIRED)
						{
							row[ColumnNames[enumValue] as string] = trans.UserData[Common.USERDATA_ACTIONREQUIRED_KEY].ToString();
						}
						else if (enumValue == InvoicePaymentColumns.ENTERED_BY)
						{
							row[ColumnNames[enumValue] as string] =
								context.Mode == InvoicePaymentMode.PAYMENT ?
								(obj as BulkPaymentClass).UpdatedBy :
								trans.UpdatedBy.ToString();
						}
						else if (enumValue == InvoicePaymentColumns.INVOICE_NUMBER)
						{
							row[ColumnNames[enumValue] as string] = trans.DocumentNumber.ToString();
						}
						else if (enumValue == InvoicePaymentColumns.INVOICE_QUERY)
						{
							string invoiceQuery = "";
							if (trans.Number04 != null)
							{
								if (0 == trans.Number04.Value)
								{
									invoiceQuery = "";
								}
								else
								{
									invoiceQuery = trans.Number04.Value.ToString();
								}
							}
							row[ColumnNames[enumValue] as string] = invoiceQuery;
						}
						else if (enumValue == InvoicePaymentColumns.ORDER_NUMBER)
						{
							row[ColumnNames[enumValue] as string] = trans.PONumber.ToString();
						}
						else if (enumValue == InvoicePaymentColumns.PAYMENT_ID)
						{
							row[ColumnNames[enumValue] as string] =
								context.Mode == InvoicePaymentMode.PAYMENT ?
								(obj as BulkPaymentClass).BulkPaymentID.ToString()
								:
								trans.DocumentNumber.ToString();

						}
						else if (enumValue == InvoicePaymentColumns.SECTION)
						{
							row[ColumnNames[enumValue] as string] = trans.UserData[Common.USERDATA_SECTION_KEY].ToString();
						}
						else if (enumValue == InvoicePaymentColumns.SUPPLIER)
						{
							row[ColumnNames[enumValue] as string] =
								context.Mode == InvoicePaymentMode.PAYMENT ?
								(obj as BulkPaymentClass).Supplier
								:
								trans.SupplierID.ToString();
						}
					}
				}

				row[ColumnNames[InvoicePaymentColumns.TRANS_ID] as string] =
					context.Mode == InvoicePaymentMode.PAYMENT ?
					(obj as BulkPaymentClass).BulkPaymentID.ToString()
					:
					trans.TransID;

				table.Rows.Add(row);
			}

			result = new DataView(table);

			return result;
		}

		#endregion // Updated View

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

		#region Helper methods
		protected void PopulateControls()
		{
			Common.PopulateFilterDropDownList(base.security, ref this.ddlEnteredBy, Common.FilterTarget.USER, true);
			Common.PopulateFilterDropDownList(base.security, ref this.ddlSection, Common.FilterTarget.SECTION, true);
			Common.PopulateFilterDropDownList(base.security, ref this.ddlAccountCode, Common.FilterTarget.INVOICE_ACCOUNT_CODE, true);
			Common.PopulateFilterDropDownList(base.security, ref this.ddlCostCentreCode, Common.FilterTarget.INVOICE_COST_CENTRE_CODE, true);

			this.tbSupplier.Role = "SUPPLIER";
		}

		protected bool SecurityProcessing()
		{
			bool returnVal = false;

			if (null == Session["Security"] || null == base.security)
			{
				Session.RemoveAll();
				this.DisplayErrorPage();
			}
			returnVal = security.HasModifyTransactionRightByTransTypeID(TransactionTypes.T21_AccountPayableInvoice);
			this.btnAddBottom.Enabled = this.btnAddTop.Enabled = returnVal;

			return security.HasViewTransactionRightByTransTypeID(TransactionTypes.T21_AccountPayableInvoice);
		}

		#endregion // Helper Methods

		#region Context Implementations
		public override InvoicePaymentContext LoadToContext(ref Object a_context)
		{
			InvoicePaymentContext context = a_context as InvoicePaymentContext;
			if (null == context)
			{
				throw new InvalidCastException("Could not cast " + a_context.ToString() + " to " + typeof(InvoicePaymentContext).ToString());
			}

			context.InvoiceNumber = this.tbInvoiceNumber.Text.Trim();
			context.PaymentID = this.tbPaymentID.Text.Trim();

			context.FuelType = this.tbProduct.Text.Trim();
			context.Supplier = this.tbSupplier.Text.Trim();
			context.QueryNumber = this.tbInvoiceQuery.Text.Trim();

			// convert from display back to UTC
			context.StartDate = m_converter.ConvertFromSiteTime(this.startDateCtrl.CurrentValue);
			context.EndDate = m_converter.ConvertFromSiteTime(this.endDateCtrl.CurrentValue);

			context.Section = Common.GetValueForFiltering(ddlSection).Trim();
			context.EnteredBy = Common.GetValueForFiltering(ddlEnteredBy).Trim();
			context.AccountCode = Common.GetValueForFiltering(ddlAccountCode).Trim();
			context.CostCentreCode = Common.GetValueForFiltering(ddlCostCentreCode).Trim();

			a_context = context;

			return a_context as InvoicePaymentContext;
		}

		public override void LoadFromContext(InvoicePaymentContext context)
		{
			this.tbInvoiceNumber.Text = context.InvoiceNumber;
			this.tbPaymentID.Text = context.PaymentID;

			this.tbProduct.Text = context.FuelType;
			this.tbSupplier.Text = context.Supplier;
			this.tbInvoiceQuery.Text = context.QueryNumber;

			if (context.EnteredBy.Length > 0)
				this.ddlEnteredBy.SelectByText(context.EnteredBy);

			if (context.Section.Length > 0)
				this.ddlSection.SelectByText(context.Section);

			if (context.AccountCode.Length > 0)
				this.ddlAccountCode.SelectByText(context.AccountCode);

			if (context.CostCentreCode.Length > 0)
				this.ddlCostCentreCode.SelectByText(context.CostCentreCode);

			System.Globalization.DateTimeFormatInfo formatInfo = this.startDateCtrl.FormatInfo;

			// convert start and end dates from UTC to display
			DateTime dispStartDate = m_converter.ConvertToSiteTime(context.StartDate);
			DateTime dispEndDate = m_converter.ConvertToSiteTime(context.EndDate);

			this.startDateCtrl.Text = dispStartDate.ToString(formatInfo);
			this.endDateCtrl.Text = dispEndDate.ToString(formatInfo);
		}
		#endregion // Context Implementations
	}
}
