/*****************************************************************************
AssociatedTxProcessor

Original Author: Van Thompson
Revisions: See source control comments

(C) Copyright 2008 by Varec, Inc.  All rights reserved.

Please Read:
If you want to add other filters you must also update the following files:
	AssociatedTxSR.cs
	AssociatedTxProcessor.cs

Revision History
Date:		      By:					Reason:
2009-02-06     A. Coker          Fix defect  1350. If Start and End date is not set, 
											return 1900-1-1 for start date and 2200-1-1 for end date
											so that filter spans whole history rather than just the current date.
 
2009-04-03     Richard Panachida Defect 2709: Fixed search date issue.

//*****************************************************************************/
namespace FuelsManager.Accounting
{
	using System;
	using System.Collections;
	using System.Web.UI.WebControls;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;
	using FMControls;

	/// <summary>
	/// The transaction filter control.
	/// </summary>
	public partial class TransactionFilterControl : System.Web.UI.UserControl
	{
		#region Private data members
		/// <summary>
		/// The value in the filter box which represents no date filter
		/// </summary>
		private const string NoneValue = "None";
		#endregion

		#region Protected data members
		protected FMLabel labDateFilter;
		protected FMLabel labStartDate;
		protected FMLabel labEndDate;
		protected FMLabel labManager;
		protected FMLabel labOwner;
		protected FMLabel labSupplier;
		protected FMLabel labPONumber;
		protected FMLabel labShipTo;
		protected FMLabel labBillTo;
		protected FMLabel labDocumentNumber;
		protected FMLabel labCarrier;
		protected FMLabel labProduct;
		protected DropDownList cbxDateFilter;
		protected FMDate dtStartDate;
		protected FMDate dtEndDate;
		protected FMCompanyTextBox ctbManager;
		protected FMCompanyTextBox ctbOwner;
		protected FMCompanyTextBox ctbShipTo;
		protected FMCompanyTextBox ctbBillTo;
		protected FMCompanyTextBox ctbSupplier;
		protected FMProductTextBox ctbProduct;
		protected TextBox txtPONumber;
		protected TextBox txtDocumentNumber;
		protected ArrayList controls = new ArrayList();

		protected Hashtable FilterFieldStateTable = new Hashtable();
		protected Hashtable FilterFieldEnabledTable = new Hashtable();

		protected SecurityClass Security;
		#endregion

		#region Construction
		/// <summary>
		/// Initializes a new instance of the <see cref="TransactionFilterControl"/> class.
		/// </summary>
		public TransactionFilterControl()
		{
			this.HideAllFields();
			this.EnableAllFields();
		}
		#endregion // Construction

		/// <summary>
		/// The refresh.
		/// </summary>
		public event EventHandler Refresh;

		#region Properties
		/// <summary>
		/// Gets the date filter.
		/// </summary>
		public AssociatedTxSR.DateFilters DateFilter
		{
			get
			{
				string dateFilter = this.cbxDateFilter.SelectedValue;

				if (dateFilter == "TransactionDate")
				{
					return AssociatedTxSR.DateFilters.TransactionDate;
				}

				if (dateFilter == "InventoryDate")
				{
					return AssociatedTxSR.DateFilters.InventoryDate;
				}

				return AssociatedTxSR.DateFilters.None;
			}
		}

		/// <summary>
		/// Gets the start date.
		/// </summary>
		public DateTimeOffset StartDate
		{
			get
			{
				if ( this.dtStartDate.Text == string.Empty )
				{
					return DateTimeOffset.Now;
				}

				return this.dtStartDate.CurrentValue;
			}
		}

		/// <summary>
		/// Gets the start date string.
		/// </summary>
		public string StartDateStr
		{
			get
			{
				if ( this.dtStartDate.Text == string.Empty )
				{
					return TimeConverter.MinFMDate.ToString("D");
				}

				return this.dtStartDate.CurrentValue.ToString("D");
			}
		}

		/// <summary>
		/// Gets the end date.
		/// </summary>
		public DateTimeOffset EndDate
		{
			get
			{
				if ( this.dtEndDate.Text == string.Empty )
				{
					return DateTimeOffset.Now;
				}

				return this.dtEndDate.CurrentValue;
			}
		}

		/// <summary>
		/// Gets the end date string.
		/// </summary>
		public string EndDateStr
		{
			get
			{
				if ( this.dtEndDate.Text == string.Empty )
				{
					return TimeConverter.MaxFMDate.ToString("D");
				}

				return this.dtEndDate.CurrentValue.ToString("D");
			}
		}

		/// <summary>
		/// Gets or sets the manager.
		/// </summary>
		public string Manager
		{
			get
			{
				if (this.ctbManager == null)
				{
					return null;
				}

				if (this.ctbManager.Text.Trim().Length == 0)
				{
					return null;
				}

				return this.ctbManager.Text;
			}

			set
			{
				if (this.ctbManager != null)
				{
					this.ctbManager.Text = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the owner.
		/// </summary>
		public string Owner
		{
			get
			{
				if (this.ctbOwner == null)
				{
					return null;
				}

				if (this.ctbOwner.Text.Trim().Length == 0)
				{
					return null;
				}

				return this.ctbOwner.Text;
			}

			set
			{
				if (this.ctbOwner != null)
				{
					this.ctbOwner.Text = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the supplier.
		/// </summary>
		public string Supplier
		{
			get
			{
				if (this.ctbSupplier == null)
				{
					return null;
				}

				if (this.ctbSupplier.Text.Trim().Length == 0)
				{
					return null;
				}

				return this.ctbSupplier.Text;
			}

			set
			{
				if (this.ctbSupplier != null)
				{
					this.ctbSupplier.Text = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the product.
		/// </summary>
		public string Product
		{
			get
			{
				if (this.ctbProduct == null)
				{
					return null;
				}

				if (this.ctbProduct.Text.Trim().Length == 0)
				{
					return null;
				}

				return this.ctbProduct.Text;
			}

			set
			{
				if (this.ctbProduct != null)
				{
					this.ctbProduct.Text = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the purchase order number.
		/// </summary>
		public string PONumber
		{
			get
			{
				if (this.txtPONumber == null)
				{
					return null;
				}

				if (this.txtPONumber.Text.Trim().Length == 0)
				{
					return null;
				}

				return this.txtPONumber.Text.Trim( );
			}

			set
			{
				if (this.txtPONumber != null)
				{
					this.txtPONumber.Text = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the ship to.
		/// </summary>
		public string ShipTo
		{
			get
			{
				if (this.ctbShipTo == null)
				{
					return null;
				}

				if (this.ctbShipTo.Text.Trim().Length == 0)
				{
					return null;
				}

				return this.ctbShipTo.Text;
			}

			set
			{
				if (this.ctbShipTo != null)
				{
					this.ctbShipTo.Text = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the bill to.
		/// </summary>
		public string BillTo
		{
			get
			{
				if (this.ctbBillTo == null)
				{
					return null;
				}

				if (this.ctbBillTo.Text.Trim().Length == 0)
				{
					return null;
				}

				return this.ctbBillTo.Text;
			}

			set
			{
				if (this.ctbBillTo != null)
				{
					this.ctbBillTo.Text = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the document number.
		/// </summary>
		public string DocumentNumber
		{
			get
			{
				if (this.txtDocumentNumber == null)
				{
					return null;
				}

				if (this.txtDocumentNumber.Text.Trim().Length == 0)
				{
					return null;
				}

				return this.txtDocumentNumber.Text.Trim( );
			}

			set
			{
				if (this.txtDocumentNumber != null)
				{
					this.txtDocumentNumber.Text = value;
				}
			}
		}

		// enable/disable properties

		/// <summary>
		/// Gets or sets a value indicating whether manager state.
		/// </summary>
		public bool ManagerState
		{
			get { return (bool) this.FilterFieldEnabledTable[FilterViewClass.FilterFields.MANAGER]; }
			set { this.FilterFieldEnabledTable[FilterViewClass.FilterFields.MANAGER] = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether owner state.
		/// </summary>
		public bool OwnerState
		{
			get { return (bool) this.FilterFieldEnabledTable[FilterViewClass.FilterFields.OWNER]; }
			set { this.FilterFieldEnabledTable[FilterViewClass.FilterFields.OWNER] = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether supplier state.
		/// </summary>
		public bool SupplierState
		{
			get { return (bool) this.FilterFieldEnabledTable[FilterViewClass.FilterFields.SUPPLIER]; }
			set { this.FilterFieldEnabledTable[FilterViewClass.FilterFields.SUPPLIER] = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether purchase order number state.
		/// </summary>
		public bool PONumberState
		{
			get { return (bool) this.FilterFieldEnabledTable[FilterViewClass.FilterFields.PONUMBER]; }
			set { this.FilterFieldEnabledTable[FilterViewClass.FilterFields.PONUMBER] = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether ship to state.
		/// </summary>
		public bool ShipToState
		{
			get { return (bool) this.FilterFieldEnabledTable[FilterViewClass.FilterFields.SHIPTO]; }
			set { this.FilterFieldEnabledTable[FilterViewClass.FilterFields.SHIPTO] = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether bill to state.
		/// </summary>
		public bool BillToState
		{
			get { return (bool) this.FilterFieldEnabledTable[FilterViewClass.FilterFields.BILLTO]; }
			set { this.FilterFieldEnabledTable[FilterViewClass.FilterFields.BILLTO] = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether document number state.
		/// </summary>
		public bool DocumentNumberState
		{
			get { return (bool) this.FilterFieldEnabledTable[FilterViewClass.FilterFields.DOCUMENTNUMBER]; }
			set { this.FilterFieldEnabledTable[FilterViewClass.FilterFields.DOCUMENTNUMBER] = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether product state.
		/// </summary>
		public bool ProductState
		{
			get { return (bool) this.FilterFieldEnabledTable[FilterViewClass.FilterFields.PRODUCT]; }
			set { this.FilterFieldEnabledTable[FilterViewClass.FilterFields.PRODUCT] = value; }
		}

		// visibility properties

		/// <summary>
		/// Gets or sets a value indicating whether show manager.
		/// </summary>
		public bool ShowManager
		{
			get { return (bool) this.FilterFieldStateTable[FilterViewClass.FilterFields.MANAGER]; }
			set { this.FilterFieldStateTable[FilterViewClass.FilterFields.MANAGER] = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether show owner.
		/// </summary>
		public bool ShowOwner
		{
			get { return (bool) this.FilterFieldStateTable[FilterViewClass.FilterFields.OWNER]; }
			set { this.FilterFieldStateTable[FilterViewClass.FilterFields.OWNER] = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether show supplier.
		/// </summary>
		public bool ShowSupplier
		{
			get { return (bool) this.FilterFieldStateTable[FilterViewClass.FilterFields.SUPPLIER]; }
			set { this.FilterFieldStateTable[FilterViewClass.FilterFields.SUPPLIER] = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether show po number.
		/// </summary>
		public bool ShowPONumber
		{
			get { return (bool) this.FilterFieldStateTable[FilterViewClass.FilterFields.PONUMBER]; }
			set { this.FilterFieldStateTable[FilterViewClass.FilterFields.PONUMBER] = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether show ship to.
		/// </summary>
		public bool ShowShipTo
		{
			get { return (bool) this.FilterFieldStateTable[FilterViewClass.FilterFields.SHIPTO]; }
			set { this.FilterFieldStateTable[FilterViewClass.FilterFields.SHIPTO] = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether show bill to.
		/// </summary>
		public bool ShowBillTo
		{
			get { return (bool) this.FilterFieldStateTable[FilterViewClass.FilterFields.BILLTO]; }
			set { this.FilterFieldStateTable[FilterViewClass.FilterFields.BILLTO] = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether show document number.
		/// </summary>
		public bool ShowDocumentNumber
		{
			get { return (bool) this.FilterFieldStateTable[FilterViewClass.FilterFields.DOCUMENTNUMBER]; }
			set { this.FilterFieldStateTable[FilterViewClass.FilterFields.DOCUMENTNUMBER] = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether show product.
		/// </summary>
		public bool ShowProduct
		{
			get { return (bool) this.FilterFieldStateTable[FilterViewClass.FilterFields.PRODUCT]; }
			set { this.FilterFieldStateTable[FilterViewClass.FilterFields.PRODUCT] = value; }
		}
		#endregion

		/// <summary>
		/// The page load.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void Page_Load(object sender, EventArgs e)
		{
		}

		/// <summary>
		/// The is field showing.
		/// </summary>
		/// <param name="field">
		/// The a field.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool IsFieldShowing(FilterViewClass.FilterFields field)
		{
			return (bool)this.FilterFieldStateTable[field];
		}

		/// <summary>
		/// The is field enabled.
		/// </summary>
		/// <param name="field">
		/// The a field.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool IsFieldEnabled(FilterViewClass.FilterFields field)
		{
			return (bool)this.FilterFieldEnabledTable[field];
		}

		/// <summary>
		/// The enable field.
		/// </summary>
		/// <param name="field">
		/// The field.
		/// </param>
		/// <param name="enable">
		/// The enable.
		/// </param>
		public void EnableField(FilterViewClass.FilterFields field, bool enable)
		{
			this.FilterFieldEnabledTable[field] = enable;
		}

		/// <summary>
		/// The show field.
		/// </summary>
		/// <param name="field">
		/// The field.
		/// </param>
		/// <param name="show">
		/// The show.
		/// </param>
		public void ShowField(FilterViewClass.FilterFields field, bool show)
		{
			this.FilterFieldStateTable[field] = show;
		}

		/// <summary>
		/// The enable all fields.
		/// </summary>
		public void EnableAllFields()
		{
			this.EnableDisableFields(true);
		}

		/// <summary>
		/// The disable all fields.
		/// </summary>
		public void DisableAllFields()
		{
			this.EnableDisableFields(false);
		}

		/// <summary>
		/// The show all fields.
		/// </summary>
		public void ShowAllFields()
		{
			this.ShowHideFields(true);
		}

		/// <summary>
		/// The hide all fields.
		/// </summary>
		public void HideAllFields()
		{
			this.ShowHideFields(false);
		}

		/// <summary>
		/// The initialize from field view.
		/// </summary>
		/// <param name="collection">
		/// The collection.
		/// </param>
		public void InitialiseFromFieldView(FilterViewsCollectionClass collection)
		{
			if (collection.Count == 0)
			{
				return; // failsafe
			}

			this.HideAllFields();

			// now go through each element in the collection to show the fields that exist
			foreach (FilterViewClass filter in collection)
			{
				this.ShowField(filter.FilterFieldID, true);
			}
		}

		/// <summary>
		/// The get translated text.
		/// </summary>
		/// <param name="siteGuid">
		/// The site GUID.
		/// </param>
		/// <param name="key">
		/// The key.
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		public string GetTranslatedText(Guid siteGuid, string key)
		{
			return FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(siteGuid, key));
		}

		/// <summary>
		/// The show hide fields.
		/// </summary>
		/// <param name="show">
		/// The show.
		/// </param>
		protected void ShowHideFields(bool show)
		{
			foreach (FilterViewClass.FilterFields field in Enum.GetValues(typeof(FilterViewClass.FilterFields)))
			{
				this.FilterFieldStateTable[field] = show;
			}
		}

		/// <summary>
		/// The enable disable fields.
		/// </summary>
		/// <param name="enable">
		/// The enable.
		/// </param>
		protected void EnableDisableFields(bool enable)
		{
			foreach (FilterViewClass.FilterFields field in Enum.GetValues(typeof(FilterViewClass.FilterFields)))
			{
				this.FilterFieldEnabledTable[field] = enable;
			}
		}

		/// <summary>
		/// The transaction filter control initialize.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void TransactionFilterControlInit(object sender, EventArgs e)
		{
		}

		/// <summary>
		/// The initialize controls.
		/// </summary>
		private void InitControls()
		{
			const string CssClass = "formfieldtitle";
			const string CssClassText = "formfield";

			// Labels
			// This is always visible so don't add it to the control collection
			this.labDateFilter = new FMLabel
				                     {
					                     Text = this.GetTranslatedText(this.Security.SiteGuid, "Date Filter") + ":",
					                     CssClass = CssClass
				                     };

			// This is always visible so don't add it to the control collection
			this.labStartDate = new FMLabel
				                    {
										Text = this.GetTranslatedText(this.Security.SiteGuid, "Start Date") + ":",
					                    CssClass = CssClass
				                    };

			// This is always visible so don't add it to the control collection
			this.labEndDate = new FMLabel
				                  {
									  Text = this.GetTranslatedText(this.Security.SiteGuid, "End Date") + ":",
					                  CssClass = CssClass
				                  };

			// These are always visible so don't add them to the control collection
			this.dtEndDate = new FMDate { CssClass = CssClassText, ID = "dtEndDate" };

			this.dtStartDate = new FMDate { CssClass = CssClassText, ID = "dtStartDate", Enabled = false };

			// The date controls should start as disabled since the date filter's starting 
			// value is None, and when it's None we disable the controls
			this.dtEndDate.Enabled = false;

			// This is always visible so don't add it to the control collection
			this.cbxDateFilter = new DropDownList { CssClass = CssClassText };
			this.cbxDateFilter.Items.Add(new ListItem("None", NoneValue));
			this.cbxDateFilter.Items.Add(new ListItem("Transaction Date", "TransactionDate"));
			this.cbxDateFilter.Items.Add(new ListItem("Inventory Date", "InventoryDate"));
			this.cbxDateFilter.AutoPostBack = true;
			this.cbxDateFilter.SelectedIndexChanged += this.CbxDateFilterSelectedIndexChanged;
			this.cbxDateFilter.Items[0].Selected = true;

			// Populate the controls collection
			// The controls are added label first followed by
			// the control.
			if (this.ShowDocumentNumber)
			{
				this.labDocumentNumber = new FMLabel
					                         {
												 Text = this.GetTranslatedText(this.Security.SiteGuid, "Document Number") + ":",
						                         CssClass = CssClass
					                         };
				this.controls.Add(this.labDocumentNumber);

				this.txtDocumentNumber = new TextBox { CssClass = CssClassText, Enabled = this.DocumentNumberState };
				this.controls.Add(this.txtDocumentNumber);
			}

			if (this.ShowPONumber)
			{
				this.labPONumber = new FMLabel
					                   {
										   Text = this.GetTranslatedText(this.Security.SiteGuid, "PO Number") + ":",
						                   CssClass = CssClass
					                   };
				this.controls.Add(this.labPONumber);

				this.txtPONumber = new TextBox { CssClass = CssClassText, Enabled = this.PONumberState };
				this.controls.Add(this.txtPONumber);
			}

			if (this.ShowBillTo)
			{
				this.labBillTo = new FMLabel
					                 {
										 Text = this.GetTranslatedText(this.Security.SiteGuid, "Bill To") + ":",
						                 CssClass = CssClass
					                 };
				this.controls.Add(this.labBillTo);

				this.ctbBillTo = new FMCompanyTextBox
					                 {
						                 Role = "CUSTOMER_BILLTO",
						                 CssClass = CssClassText,
						                 ID = "ctbBillTo",
						                 Enabled = this.BillToState
					                 };
				this.controls.Add(this.ctbBillTo);
			}

			if (this.ShowManager)
			{
				this.labManager = new FMLabel
					                  {
										  Text = this.GetTranslatedText(this.Security.SiteGuid, "Manager") + ":",
						                  CssClass = CssClass
					                  };
				this.controls.Add(this.labManager);

				this.ctbManager = new FMCompanyTextBox
					                  {
						                  Role = "MANAGER",
						                  CssClass = CssClassText,
						                  ID = "ctbManager",
						                  Enabled = this.ManagerState
					                  };
				this.controls.Add(this.ctbManager);
			}

			if (this.ShowOwner)
			{
				this.labOwner = new FMLabel
					                {
										Text = this.GetTranslatedText(this.Security.SiteGuid, "Owner") + ":",
						                CssClass = CssClass
					                };
				this.controls.Add(this.labOwner);

				this.ctbOwner = new FMCompanyTextBox
					                {
						                Role = "OWNER",
						                CssClass = CssClassText,
						                ID = "ctbOwner",
						                Enabled = this.OwnerState
					                };
				this.controls.Add(this.ctbOwner);
			}

			if (this.ShowShipTo)
			{
				this.labShipTo = new FMLabel
					                 {
										 Text = this.GetTranslatedText(this.Security.SiteGuid, "Ship To") + ":",
						                 CssClass = CssClass
					                 };
				this.controls.Add(this.labShipTo);

				this.ctbShipTo = new FMCompanyTextBox
					                 {
						                 Role = "CUSTOMER_SHIPTO",
						                 CssClass = CssClassText,
						                 ID = "ctbShipTo",
						                 Enabled = this.ShipToState
					                 };
				this.controls.Add(this.ctbShipTo);
			}

			if (this.ShowSupplier)
			{
				this.labSupplier = new FMLabel
					                   {
										   Text = this.GetTranslatedText(this.Security.SiteGuid, "Supplier") + ":",
						                   CssClass = CssClass
					                   };
				this.controls.Add(this.labSupplier);

				this.ctbSupplier = new FMCompanyTextBox
					                   {
						                   Role = "SUPPLIER",
						                   CssClass = CssClassText,
						                   ID = "ctbSupplier",
						                   Enabled = this.SupplierState
					                   };
				this.controls.Add(this.ctbSupplier);
			}

			if (this.ShowProduct)
			{
				this.labProduct = new FMLabel
					                  {
										  Text = this.GetTranslatedText(this.Security.SiteGuid, "Product") + ":",
						                  CssClass = CssClass
					                  };
				this.controls.Add(this.labProduct);

				this.ctbProduct = new FMProductTextBox { CssClass = CssClassText, ID = "ctbProduct", Enabled = this.ProductState };
				this.controls.Add(this.ctbProduct);
			}
		}

		/// <summary>
		/// The populate controls.
		/// </summary>
		public void PopulateControls()
		{
			this.InitControls();

			// The table will have 4 columns.  Columns 1 and 3 will contain labels.
			// columns 2 and 4 will contain controls for collecting input
			this.tblFilter.Rows.Clear();

			// The first row will contain the date filter and start date controls
			var dateFilter = new TableRow();
			var row1Cell1 = new TableCell { Wrap = false };
			row1Cell1.Controls.Add(this.labDateFilter);
			dateFilter.Cells.Add(row1Cell1);

			var row1Cell2 = new TableCell { Wrap = false };
			row1Cell2.Controls.Add(this.cbxDateFilter);
			dateFilter.Cells.Add(row1Cell2);

			var row1Cell3 = new TableCell { Wrap = false };
			row1Cell3.Controls.Add(this.labStartDate);
			dateFilter.Cells.Add(row1Cell3);

			var row1Cell4 = new TableCell { Wrap = false };
			row1Cell4.Controls.Add(this.dtStartDate);
			dateFilter.Cells.Add(row1Cell4);

			// Add the date filter row to the table
			this.tblFilter.Rows.Add(dateFilter);

			// Create the next row that will contain the end date label
			// and control.
			var endDate = new TableRow();
			var row2Cell1 = new TableCell { Wrap = false };
			row2Cell1.Controls.Add(this.labEndDate);
			endDate.Cells.Add(row2Cell1);

			var row2Cell2 = new TableCell { Wrap = false };
			row2Cell2.Controls.Add(this.dtEndDate);
			endDate.Cells.Add(row2Cell2);

			// If no other filter controls are supposed to display
			// add the row that contains the end date controls
			int controlCount = this.controls.Count;
			int controlNumber = 1;

			if (controlCount == 0)
			{
				this.tblFilter.Rows.Add(endDate);
			}
			else
			{
				TableRow currentRow = endDate;
				int cellNumber = 3;

				// Add the filter controls to the table
				foreach (WebControl control in this.controls)
				{
					if (cellNumber == 1)
					{
						currentRow = new TableRow();
					}

					var cell = new TableCell { Wrap = false };
					cell.Controls.Add(control);
					currentRow.Cells.Add(cell);

					// If this is the last control go ahead and
					// add the row
					if (controlNumber == controlCount)
					{
						this.tblFilter.Rows.Add(currentRow);
						break;
					}

					if (cellNumber == 4)
					{
						this.tblFilter.Rows.Add(currentRow);
						cellNumber = 1;
					}
					else
					{
						cellNumber++;
					}

					// Increase the control number
					controlNumber++;
				}
			}
		}

		/// <summary>
		/// The transaction filter control pre-render.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void TransactionFilterControlPreRender(object sender, EventArgs e)
		{	
		}

		/// <summary>
		/// When the user selects "None" as the date filter value, disable the start and end dates
		/// </summary>
		/// <param name="sender">The parameter is not used.</param>
		/// <param name="e">The parameter is not used.</param>
		protected void CbxDateFilterSelectedIndexChanged(object sender, EventArgs e)
		{
			bool enableDateControls = !string.IsNullOrEmpty(this.cbxDateFilter.SelectedValue) && this.cbxDateFilter.SelectedValue != NoneValue;
			this.dtStartDate.Enabled = enableDateControls;
			this.dtEndDate.Enabled = enableDateControls;
		}

		/// <summary>
		/// The on refresh.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		protected virtual void OnRefresh(object sender)
		{
			if (this.Refresh != null)
			{
				this.Refresh(sender, new EventArgs());
			}
		}

		/// <summary>
		/// The button refresh click.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void BtnRefreshClick(object sender, System.EventArgs e)
		{
			this.OnRefresh(sender);
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent( );
			base.OnInit(e);
			this.Security = this.Session["Security"] as SecurityClass;
		}

		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent( )
		{
			this.Init += new System.EventHandler(this.TransactionFilterControlInit);
			this.PreRender += new System.EventHandler(this.TransactionFilterControlPreRender);
		}
		#endregion
	}
}
