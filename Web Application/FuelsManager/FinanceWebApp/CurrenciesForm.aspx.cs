// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Varec, Inc." file="CurrenciesForm.aspx.cs">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for CurrenciesForm.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FinanceWebApp
{
	using System;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FuelsManager.Accounting;

	/// <summary>
	///    Summary description for CurrenciesForm.
	/// </summary>
	public partial class CurrenciesForm : AccountingWebFormView
	{
		#region Constants and Fields
		public const string SessionCurrency = "CurrenciesForm.Currency";
		#endregion

		#region Public Methods and Operators
		public string[] Keys(SecurityClass inSecurity)
		{
			string[] keys = { "Currency Configuration", "Unit Display Name", "Country" };
			return keys;
		}
		#endregion

		#region Methods
		protected override void OnInit(EventArgs e)
		{
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			this.InitializeComponent();
			base.OnInit(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			// Initialize Security and DataDictionary
			this.Initialize();

			// Enable/disable controls based on the user's rights
			this.CheckPermissions();

			this.UpdateView();
		}

		protected void BtnAddBottomClick(object sender, EventArgs e)
		{
			try
			{
				this.AddNewCurrency();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void BtnAddTopClick(object sender, EventArgs e)
		{
			try
			{
				this.AddNewCurrency();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void DdlPageSizeSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void DgCurrenciesItemDataBound(object source, DataGridItemEventArgs eventArgs)
		{
			try
			{
				var deleteButton = (LinkButton)eventArgs.Item.FindControl("linkDelete");

				// Disable the delete button if the user does not have modify rights
				if (deleteButton != null)
				{
					if (this.security.HasRight(RIGHT.MODIFY_FINANCIAL_DATA) == false)
					{
						deleteButton.Enabled = false;
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void AddNewCurrency()
		{
			this.Session[SessionCurrency] = null;

			this.Redirect("CurrencyForm.aspx");
		}

		/// <summary>
		///    This method will check for permission for modifying. If the site is not a site group or the
		///    user does not have modify rights, then the editing is disabled.
		/// </summary>
		private void CheckPermissions()
		{
			if (this.security.HasRight(RIGHT.MODIFY_FINANCIAL_DATA) == false)
			{
				this.EnableControls(false);
			}
			else
			{
				this.EnableControls(true);
			}
		}

		private void EnableControls(bool enabled)
		{
			this.btnAddBottom.Enabled = enabled;
			this.btnAddTop.Enabled = enabled;
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.dgCurrencies.EditCommand		+= this.DgCurrenciesEditCommand;
			this.dgCurrencies.PageIndexChanged	+= this.DgCurrenciesPageIndexChanged;
			this.dgCurrencies.DeleteCommand		+= this.DgCurrenciesDeleteCommand;
			this.dgCurrencies.ItemDataBound		+= this.DgCurrenciesItemDataBound;
		}

		/// <summary>
		///    Updates the view presented on the data grid.
		/// </summary>
		private void UpdateView()
		{
			// Get the currencies
			CurrencyDOCollectionClass currencies = FMChannelHelper.MakeCall<ICurrencies, CurrencyDOCollectionClass>(
																	 x =>
																	 x.GetCurrencies(this.security)
																);

			this.ddlPageSize.SetPageSize(this.dgCurrencies, currencies.Count);

			this.dgCurrencies.DataSource = currencies;
			this.dgCurrencies.DataBind();
		}

		/// <summary>
		///    The currencies grid delete command handler
		/// </summary>
		/// <param name="source">
		///    The source.
		/// </param>
		/// <param name="e">
		///    The event args.
		/// </param>
		private void DgCurrenciesDeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				FMChannelHelper.MakeCall<ICurrencies>(
													x =>
													x.Remove(this.security, Guid.Parse(e.CommandArgument.ToString()))
											);
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}

			this.UpdateView();
		}

		private void DgCurrenciesEditCommand(object source, DataGridCommandEventArgs e)
		{
			// Get the guid of the currency to be edited
			Guid currencyGuid = Guid.Parse(e.CommandArgument.ToString());

			// Load the currency object
			CurrencyDO currency;

			try
			{
				currency = FMChannelHelper.MakeCall<ICurrencies, CurrencyDO>(
																	 x =>
																	 x.Get(this.security,currencyGuid)
																);
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
				return;
			}

			this.Session[SessionCurrency] = currency;

			this.Redirect("CurrencyForm.aspx");
		}

		private void DgCurrenciesPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			try
			{
				// if we are editing do not allow a page change
				if (this.dgCurrencies.EditItemIndex > -1)
				{
					return;
				}

				this.dgCurrencies.CurrentPageIndex = e.NewPageIndex;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}
		#endregion
	}
}