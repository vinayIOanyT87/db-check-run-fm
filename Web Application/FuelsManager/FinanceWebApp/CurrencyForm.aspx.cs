// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CurrencyForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for CurrencyForm.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FinanceWebApp
{
	using System;
	using System.Globalization;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using FMControls;

	using FuelsManager.Accounting;

	/// <summary>
	///    Code behind for currency form.
	/// </summary>
	public partial class CurrencyForm : AccountingWebFormView
	{
		#region Constants and Fields
		/// <summary>
		///    This Guid is used for indicating that a line item is new, but still in edit mode. if the line
		///    item is accepted, the Guid is set to Guid.Empty
		/// </summary>
		protected static readonly Guid NewLineItemGuid = new Guid("B49A7304-D376-4EC2-99B5-69A7D49F903C");

		/// <summary>
		///    Flag indicating if the items is a new item.
		/// </summary>
		private bool isNew;
		#endregion

		#region Properties
		/// <summary>
		///    Gets or sets the date format.  Used for displaying date in regional settings format.
		/// </summary>
		protected string DateFormat { get; set; }

		/// <summary>
		///    Gets or sets the currency object.
		/// </summary>
		private CurrencyDO Currency { get; set; }
		#endregion

		#region Public Methods and Operators
		/// <summary>
		///    Keyses the specified security.
		/// </summary>
		/// <param name="securityObject">The security object.</param>
		/// <returns>An array of data dictionary keys.</returns>
		public string[] Keys(SecurityClass securityObject)
		{
			string[] keys = { "Country", "Currency Configuration", "Display", "Effective Date", "Name", "Rate", "Unit" };
			return keys;
		}
		#endregion

		#region Methods
		/// <summary>
		///    Populates the individual controls with values from the selected currency.
		///    Populates the grid with currency line items belonging to the currency.
		/// </summary>
		protected void BindControls()
		{
			this.txtCountry.Text = this.Currency.Country;
			this.txtName.Text = this.Currency.UnitDisplayName;
			foreach (ListItem item in this.ddlUnit.Items)
			{
				if (this.Currency.LookupCurrencyUnitIndex == Convert.ToInt32(item.Value))
				{
					item.Selected = true;
					break;
				}
			}

			this.chkDisplay.Checked = this.Currency.DisplayFlag;

			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
										x =>
										x.Get(
													this.security,
													this.security.SiteGuid,
													getMemberSites: true,
													getSchedulesAndProcessVariables: true,
													bGetAssociatedAliases: true)
										);
			DateTimeFormatInfo dateTimeFormatInfo = site.GetDateTimeFormatInfo();
			this.DateFormat = "{0:" + dateTimeFormatInfo.ShortDatePattern + "}"; // used in aspx 

			// Bind the line items
			this.dgLineItems.DataSource = this.Currency.LineItems;
			this.dgLineItems.DataBind();
		}

		/// <summary>
		///    Handles the Click event of the btnAddBottom control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">
		///    The <see cref="System.EventArgs" /> instance containing the event data.
		/// </param>
		protected void BtnAddBottomClick(object sender, EventArgs e)
		{
			try
			{
				this.AddNewLineItem();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Handles the Click event of the btnAddTop control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">
		///    The <see cref="System.EventArgs" /> instance containing the event data.
		/// </param>
		protected void BtnAddTopClick(object sender, EventArgs e)
		{
			try
			{
				this.AddNewLineItem();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Handles the Click event of the btnCancel control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">
		///    The <see cref="System.EventArgs" /> instance containing the event data.
		/// </param>
		protected void BtnCancelClick(object sender, EventArgs e)
		{
			// Return to the currency list page
			this.Redirect("CurrenciesForm.aspx");
		}

		/// <summary>
		///    Handles the Click event of the btnOK control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">
		///    The <see cref="System.EventArgs" /> instance containing the event data.
		/// </param>
		protected void BtnOkClick(object sender, EventArgs e)
		{
			try
			{
				this.Currency = (CurrencyDO)this.Session[CurrenciesForm.SessionCurrency];

				// Populate the currency object
				this.Currency.UnitDisplayName = this.txtName.Text;
				this.Currency.Country = this.txtCountry.Text;
				this.Currency.LookupCurrencyUnitIndex = Convert.ToInt32(this.ddlUnit.SelectedItem.Value);
				this.Currency.DisplayFlag = this.chkDisplay.Checked;
				this.Currency.SiteGuid = this.security.SiteGuid;

				// See if the currency is an existing or new currency
				if (this.isNew)
				{
					try
					{
						FMChannelHelper.MakeCall<ICurrencies>(
																	 x =>
																	 x.Add(this.security, this.Currency)
																);
					}
					catch (Exception ex)
					{
						this.ErrorHandler(ex);
						return;
					}
				}
				else
				{
					try
					{
						FMChannelHelper.MakeCall<ICurrencies>(
																	 x =>
																	 x.Save(this.security, this.Currency)
																);
					}
					catch (Exception ex)
					{
						this.ErrorHandler(ex);
						return;
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}

			this.Redirect("CurrenciesForm.aspx");
		}

		/// <summary>
		///    Handles the ItemDataBound event of the dgLineItems control.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="eventArgs">
		///    The <see cref="System.Web.UI.WebControls.DataGridItemEventArgs" /> instance containing the event data.
		/// </param>
		protected void DgLineItemsItemDataBound(object source, DataGridItemEventArgs eventArgs)
		{
			try
			{
				var editButton = (LinkButton)eventArgs.Item.FindControl("btnEdit");
				var deleteButton = (LinkButton)eventArgs.Item.FindControl("btnDelete");

				// Disable the edit and delete buttons if the user does not have modify rights
				if ((editButton != null) && (deleteButton != null))
				{
					if (this.security.HasRight(RIGHT.MODIFY_FINANCIAL_DATA) == false)
					{
						editButton.Enabled = false;
						deleteButton.Enabled = false;
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Raises the <see cref="OnInit" /> event.
		/// </summary>
		/// <param name="e">
		///    The <see cref="System.EventArgs" /> instance containing the event data.
		/// </param>
		protected override void OnInit(EventArgs e)
		{
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		///    Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">
		///    The <see cref="System.EventArgs" /> instance containing the event data.
		/// </param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				// Initialize Security and DataDictionary
				this.Initialize();

				if (!this.Page.IsPostBack)
				{
					// Check the session for an existing currency object.  If the session variable
					// is null then this is considered a new currency
					if (this.Session[CurrenciesForm.SessionCurrency] == null)
					{
						this.Currency = new CurrencyDO { SiteGuid = this.security.SiteGuid };
						this.Session[CurrenciesForm.SessionCurrency] = this.Currency;
						this.isNew = true;
					}
					else
					{
						this.Currency = (CurrencyDO)this.Session[CurrenciesForm.SessionCurrency];
						this.isNew = false;
					}

					if (this.security.HasRight(RIGHT.MODIFY_FINANCIAL_DATA) == false)
					{
						this.EnableControls(false);
					}

					// Populate the Currency Unit drop down
					this.PopulateUnits();

					// Set the title label with a key field from the bound object appended
					if (this.Currency != null)
					{
						this.labCurrencyConfig.Text = this.GetTitleLabelText(this.labCurrencyConfig.Text, this.Currency.UnitDisplayName);
					}

					this.BindControls();
				}
				else
				{
					// If the currency in session has an empty Guid it has not been saved
					this.Currency = (CurrencyDO)this.Session[CurrenciesForm.SessionCurrency];

					this.isNew = this.Currency.IdentityGuid == Guid.Empty;

					this.MapCurrency();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Populates the unit drop down list with units from the database
		/// </summary>
		protected void PopulateUnits()
		{
			// Get the currency units then populate the drop down list
			CurrencyUnitDOCollectionClass units = FMChannelHelper.MakeCall<ICurrencies, CurrencyUnitDOCollectionClass>(
																	 x =>
																	 x.GetCurrencyUnits(this.security)
																);

			if (units.Count == 0)
			{
				this.btnOK.Enabled = false;
				throw new ApplicationException("No currency units defined in system.");
			}

			this.ddlUnit.DataTextField = "CurrencyUnitName";
			this.ddlUnit.DataValueField = "CurrencyUnitIndex";
			this.ddlUnit.DataSource = units;
			this.ddlUnit.DataBind();
		}

		/// <summary>
		///    Adds the new line item.
		/// </summary>
		private void AddNewLineItem()
		{
			try
			{
				// Get the currency object from the session
				this.Currency = (CurrencyDO)this.Session[CurrenciesForm.SessionCurrency];

				// Add a new line item to the currency's line items
				var lineItem = new CurrencyLineItemDO { EffectiveDate = TimeConverter.Today(), IdentityGuid = NewLineItemGuid };

				// NewLineItemGuid is an indicator that this is a new line item in edit mode.
				// if the line item is accepted it will have its guid set to Guid.Empty
				this.Currency.LineItems.Add(lineItem);

				// Set the grid's edit item to the new line item
				this.dgLineItems.CurrentPageIndex = (this.Currency.LineItems.Count - 1) / this.dgLineItems.PageSize;
				this.dgLineItems.EditItemIndex = (this.Currency.LineItems.Count - 1) % this.dgLineItems.PageSize;

				// Bind the controls again
				this.BindControls();

				this.EnableControls(false);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Handles the CancelCommand event of the dgLineItems control.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="e">
		///    The <see cref="System.Web.UI.WebControls.DataGridCommandEventArgs" /> instance containing the event data.
		/// </param>
		private void DgLineItemsCancelCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				this.Currency = (CurrencyDO)this.Session[CurrenciesForm.SessionCurrency];

				// Remove the line item from the collection of line items
				// If the item has a guid of NewLineItemGuid it is a new item that has not been
				// accepted so just remove it.
				if (Guid.Parse((string)e.CommandArgument) == NewLineItemGuid)
				{
					foreach (CurrencyLineItemDO lineItem in this.Currency.LineItems)
					{
						if (lineItem.IdentityGuid == NewLineItemGuid)
						{
							this.Currency.LineItems.RemoveByIdentityGuid(lineItem);
							break;
						}
					}
				}

				// Put the item back in regular mode
				this.dgLineItems.EditItemIndex = -1;

				// Bind the controls again
				this.BindControls();

				this.EnableControls(true);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    The delete command event handler for the grid.
		/// </summary>
		/// <param name="source">
		///    The source.
		/// </param>
		/// <param name="e">
		///    The event arguments.
		/// </param>
		private void DgLineItemsDeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				this.Currency = (CurrencyDO)this.Session[CurrenciesForm.SessionCurrency];

				// Remove the line item from the currency object
				int itemIndex = (this.dgLineItems.PageSize * this.dgLineItems.CurrentPageIndex) + e.Item.ItemIndex;
				this.Currency.LineItems.RemoveAt(itemIndex);

				this.BindControls();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    The edit command event handler for the grid.
		/// </summary>
		/// <param name="source">
		///    The source.
		/// </param>
		/// <param name="e">
		///    The event args.
		/// </param>
		private void DgLineItemsEditCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				// Put the selected item into edit mode
				this.dgLineItems.EditItemIndex = e.Item.ItemIndex;

				this.Currency = (CurrencyDO)this.Session[CurrenciesForm.SessionCurrency];
				this.BindControls();

				// Disable the controls
				this.EnableControls(false);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    The page change event handler for the grid.
		/// </summary>
		/// <param name="source">
		///    The source.
		/// </param>
		/// <param name="e">
		///    The event args.
		/// </param>
		private void DgLineItemsPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			try
			{
				// if we are editing do not allow a page change
				if (this.dgLineItems.EditItemIndex > -1)
				{
					return;
				}

				this.Currency = (CurrencyDO)this.Session[CurrenciesForm.SessionCurrency];
				this.dgLineItems.CurrentPageIndex = e.NewPageIndex;
				this.BindControls();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Dgs the line items update command.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="e">
		///    The <see cref="System.Web.UI.WebControls.DataGridCommandEventArgs" /> instance containing the event data.
		/// </param>
		private void DgLineItemsUpdateCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				this.Currency = (CurrencyDO)this.Session[CurrenciesForm.SessionCurrency];

				// Determine the index of the line item using the
				// current page and datagrid line item
				int page = this.dgLineItems.CurrentPageIndex;
				int pageSize = this.dgLineItems.PageSize;
				int lineItemIndex = (page * pageSize) + e.Item.ItemIndex;

				// Get the line item
				CurrencyLineItemDO lineItem = this.Currency.LineItems[lineItemIndex];

				// Throw an exception if the line item was not found
				if (lineItem == null)
				{
					throw new ApplicationException("Could not find the line item to update.");
				}

				// Get the values from the controls
				var rate = (TextBox)e.Item.FindControl("txtRate");
				var effectiveDate = (FMDate)e.Item.FindControl("dtExpirationDate");

				// Be sure rate is populated and is numeric
				if (rate.Text.Trim().Length == 0)
				{
					var ex = new Exception("Rate is required.");
					this.ErrorHandler(ex);
					return;
				}

				// Be sure rate is numeric
				double rateValue;

				if (!double.TryParse(rate.Text, out rateValue))
				{
					var ex = new Exception("Rate must be numeric.");
					ErrorHandler(ex);
					return;
				}

				// If this is a new line item change the guid from NewLineItemGuid to Guid.Empty
				if (lineItem.IdentityGuid == NewLineItemGuid)
				{
					lineItem.IdentityGuid = Guid.Empty;
				}

				lineItem.EffectiveDate = effectiveDate.CurrentValue;
				lineItem.Rate = rateValue;
				lineItem.CurrencyGuid = this.Currency.IdentityGuid;
				lineItem.IsDirty = true;

				// Get out of edit mode
				this.dgLineItems.EditItemIndex = -1;

				this.BindControls();
				this.EnableControls(true);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Enables the controls.
		/// </summary>
		/// <param name="enable">
		///    if set to <c>true</c> [enable].
		/// </param>
		private void EnableControls(bool enable)
		{
			this.txtName.Enabled = enable;
			this.txtCountry.Enabled = enable;
			this.ddlUnit.Enabled = enable;
			this.btnAddBottom.Enabled = enable;
			this.btnAddTop.Enabled = enable;
			this.btnCancel.Enabled = enable;
			this.btnOK.Enabled = enable;
			this.chkDisplay.Enabled = enable;
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.dgLineItems.EditCommand		+= this.DgLineItemsEditCommand;
			this.dgLineItems.PageIndexChanged	+= this.DgLineItemsPageIndexChanged;
			this.dgLineItems.CancelCommand		+= this.DgLineItemsCancelCommand;
			this.dgLineItems.UpdateCommand		+= this.DgLineItemsUpdateCommand;
			this.dgLineItems.DeleteCommand		+= this.DgLineItemsDeleteCommand;
			this.dgLineItems.ItemDataBound		+= this.DgLineItemsItemDataBound;
		}

		/// <summary>
		///    This is used on post backs to capture any changes made by the user
		///    to the currency object
		/// </summary>
		private void MapCurrency()
		{
			this.Currency.Country = this.txtCountry.Text;

			if (this.ddlUnit.SelectedItem != null)
			{
				this.Currency.LookupCurrencyUnitIndex = Convert.ToInt32(this.ddlUnit.SelectedItem.Value);
			}

			this.Currency.UnitDisplayName = this.txtName.Text;
			this.Currency.DisplayFlag = this.chkDisplay.Checked;
		}
		#endregion
	}
}