// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaxExciseDetailForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the TaxExciseDetailForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FinanceWebApp
{
    using System;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;

    using Accounting;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;

    using FMControls;

    using FMCore;

    public partial class TaxExciseDetailForm : AccountingWebFormView
	{
		#region Enums

		private enum RequestTypes
		{
			Edit,

			Add,

			NONE
		};

		#endregion

		#region Methods

		/// <summary>
		///    This method will handle the Add button events. It will add a new item
		///    to the grid and the companies list hash table.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void AddBottomButtonOnClick(object sender, EventArgs e)
		{
			List<TaxCompanyMapDO> companies;

			if (this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_COMPANIES_LIST] == null)
			{
				companies = new List<TaxCompanyMapDO>();
				this.Page.Session.Add(PageSessionKeyConstants.TAX_EXCISE_COMPANIES_LIST, companies);
			}

			companies = this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_COMPANIES_LIST] as List<TaxCompanyMapDO>;

			if (companies == null)
			{
				this.Page.Session.Remove(PageSessionKeyConstants.TAX_EXCISE_COMPANIES_LIST);
				companies = this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_COMPANIES_LIST] as List<TaxCompanyMapDO>;
			}

			var taxCompanyMapDO = new TaxCompanyMapDO();
			companies?.Add(taxCompanyMapDO);

		    if (companies != null)
		    {
		        this.DataGridAssignedCompanies.CurrentPageIndex = (companies.Count - 1) / this.DataGridAssignedCompanies.PageSize;
		        this.DataGridAssignedCompanies.EditItemIndex = (companies.Count - 1) % this.DataGridAssignedCompanies.PageSize;

		        // Disable controls while in edit mode.
		        this.EnableControls(false);

		        this.GridSizeDropDown.SetPageSize(this.DataGridAssignedCompanies, companies.Count);
		        this.DataGridAssignedCompanies.DataSource = companies;
		    }
		    this.DataGridAssignedCompanies.DataBind();
		}

		/// <summary>
		///    This method will handle the Add button events. It will add a new item
		///    to the grid and the companies list hash table.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void AddTopButtonOnClick(object sender, EventArgs e)
		{
			this.AddBottomButtonOnClick(sender, e);
		}

		/// <summary>
		///    This method handles the cancel button event. It will cleanup items
		///    in session and return back to the Excise Summary page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void CancelButtonOnClick(object sender, EventArgs e)
		{
			this.RemoveSessionKeys();
			this.Redirect("TaxRateExciseSummaryForm.aspx");
		}

		/// <summary>
		///    This method handles the cancel event on the assign companies grid. It cancels the
		///    editing.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		protected void DataGridAssignedCompaniesCancelCommand(object source, DataGridCommandEventArgs e)
		{
			this.EnableControls(true);
			this.DataGridAssignedCompanies.EditItemIndex = -1;

		    var companies = this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_COMPANIES_LIST] as List<TaxCompanyMapDO>;

		    if (companies != null)
		    {
		        var companyMapDO = new TaxCompanyMapDO();
		        companies.Remove(companyMapDO);

		        this.GridSizeDropDown.SetPageSize(this.DataGridAssignedCompanies, companies.Count);
		        this.DataGridAssignedCompanies.DataSource = companies;
		        this.DataGridAssignedCompanies.DataBind();
		    }

		    // Enable or disable the Product field based on Add or Edit mode.
			this.EnableProductField();
		}

		/// <summary>
		///    This method handles the delete assign companies event.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		protected void DataGridAssignedCompaniesDeleteCommand(object source, DataGridCommandEventArgs e)
		{
			List<TaxCompanyMapDO> companies = null;

		    // Validate the controls
			var company = (FMLabel)e.Item.FindControl("labCompany");
			if (company.Text.Trim().Length == 0)
			{
				this.ErrorHandler(new Exception("Invalid company name."));
				return;
			}

		    var companyMapDO = new TaxCompanyMapDO { CompanyID = company.Text.Trim() };

		    // Get the select company's guid
			companyMapDO.CompanyGuid = FMChannelHelper.MakeCall<ICompanies, Guid>(
																	 x =>
																	 x.GetIdentityGuid(this.security, companyMapDO.CompanyID)
																);


			if (this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_COMPANIES_LIST] != null)
			{
				companies = this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_COMPANIES_LIST] as List<TaxCompanyMapDO>;

				if (companies == null)
				{
					this.ErrorHandler(new Exception("Company list object is missing."));
				}
				else
				{
				    List<TaxCompanyMapDO> deletedCompanies;
				    if (this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_DELETED_COMPANIES_LIST] == null)
					{
						deletedCompanies = new List<TaxCompanyMapDO>();
						this.Page.Session.Add(PageSessionKeyConstants.TAX_EXCISE_DELETED_COMPANIES_LIST, deletedCompanies);
					}
					else
					{
						deletedCompanies =
							(List<TaxCompanyMapDO>)this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_DELETED_COMPANIES_LIST];
					}

					companies.Remove(companyMapDO);

					if (deletedCompanies == null)
					{
						this.ErrorHandler(new Exception("Deleted company list object is missing."));
					}
					else if (deletedCompanies.Contains(companyMapDO) == false)
					{
						deletedCompanies.Add(companyMapDO);
					}
				}
			}

			// Add the company to the company collection
			this.DataGridAssignedCompanies.EditItemIndex = -1;
			this.DataGridAssignedCompanies.Columns[0].HeaderText = "Delete";
		    if (companies != null)
		    {
		        this.DataGridAssignedCompanies.CurrentPageIndex = (companies.Count - 1) / this.DataGridAssignedCompanies.PageSize;
		        this.GridSizeDropDown.SetPageSize(this.DataGridAssignedCompanies, companies.Count);
		        this.DataGridAssignedCompanies.DataSource = companies;
		    }
		    this.DataGridAssignedCompanies.DataBind();

			this.EnableControls(true);

			// Enable or disable the Product field based on Add or Edit mode.
			this.EnableProductField();
		}

		/// <summary>
		///    This method handles the grid's item data binding. It will disable the delete button in
		///    the grid if the user does not have premissions.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="eventArgs"></param>
		protected void DataGridAssignedCompaniesItemDataBound(object source, DataGridItemEventArgs eventArgs)
		{
			try
			{
				var deleteButton = (LinkButton)eventArgs.Item.FindControl("btnDelete");

				// Disable the edit and delete buttons if the user does not have modify rights
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

		/// <summary>
		///    This method handles the assigned companies update event. The update is kept in session
		///    until the user selects OK or New.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		protected void DataGridAssignedCompaniesUpdateCommand(object source, DataGridCommandEventArgs e)
		{
			List<TaxCompanyMapDO> companies = null;

			// Validate the controls
			var txtCompany = (FMCompanyTextBox)e.Item.FindControl("txtCompany");
			if (txtCompany.Text.Trim().Length == 0)
			{
				this.ErrorHandler(new Exception("Company is Required"));
				return;
			}

			// Get the select company's guid
		    var companyMapDO = new TaxCompanyMapDO { CompanyID = txtCompany.Text.Trim() };

		    companyMapDO.CompanyGuid = FMChannelHelper.MakeCall<ICompanies, Guid>(
																	 x =>
																	 x.GetIdentityGuid(this.security, companyMapDO.CompanyID)
																);

			if (this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_COMPANIES_LIST] != null)
			{
				companies = this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_COMPANIES_LIST] as List<TaxCompanyMapDO>;

				if (companies == null)
				{
					this.ErrorHandler(new Exception("Company list object is missing."));
				}
				else
				{
					if (companies.Contains(companyMapDO))
					{
						this.ErrorHandler(new Exception("The company selected is already assigned."));
						return;
					}
					else
					{
						var emptyCompanyMapDO = new TaxCompanyMapDO();
						companies.Remove(emptyCompanyMapDO);

						companies.Add(companyMapDO);

					    var deletedCompanies =
					        this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_DELETED_COMPANIES_LIST] as List<TaxCompanyMapDO>;

					    if (deletedCompanies != null && deletedCompanies.Contains(companyMapDO))
					    {
					        deletedCompanies.Remove(companyMapDO);
					    }
					}
				}
			}

			// Add the company to the company collection
			this.DataGridAssignedCompanies.EditItemIndex = -1;
			this.DataGridAssignedCompanies.Columns[0].HeaderText = "Delete";
		    if (companies != null)
		    {
		        this.GridSizeDropDown.SetPageSize(this.DataGridAssignedCompanies, companies.Count);
		        this.DataGridAssignedCompanies.DataSource = companies;
		    }
		    this.DataGridAssignedCompanies.DataBind();

			this.EnableControls(true);

			// Enable or disable the Product field based on Add or Edit mode.
			this.EnableProductField();
		}

		/// <summary>
		///    This method handles the grid size dropdown change. It will update the
		///    grid size accordingly.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void GridSizeDropdownOnChange(object sender, EventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.DataGridAssignedCompanies.EditItemIndex > -1)
			{
				return;
			}

		    var companies = this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_COMPANIES_LIST] as List<TaxCompanyMapDO>;

		    if (companies != null)
		    {
		        var companyMapDO = new TaxCompanyMapDO();
		        companies.Remove(companyMapDO);

		        this.GridSizeDropDown.SetPageSize(this.DataGridAssignedCompanies, companies.Count);
		        this.DataGridAssignedCompanies.DataSource = companies;
		        this.DataGridAssignedCompanies.DataBind();
		    }
		}

		/// <summary>
		///    This method handles the New button event. It saves the data to the database
		///    and initializes the page for a new entry.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void NewButtonOnClick(object sender, EventArgs e)
		{
			bool successful = this.SaveExcise();

			if (successful)
			{
				this.ExciseDateField.CurrentValue = DateTimeOffset.Now;
				this.ProductSelectControl.Text = "";
				this.ExciseRateTextBox.Text = this.RateFormatter(0.0);

				// Remove all session keys related to the previous detail object.
				this.RemoveSessionKeys();

				// Create a new Excise data object in place it in session.
				var exciseDO = new ExciseTaxDO();
				this.Page.Session.Add(PageSessionKeyConstants.TAX_EXCISE_DETAIL_OBJECT, exciseDO);

				// Create a new companies list and add it to session.
				var companies = new List<TaxCompanyMapDO>();
				this.Page.Session.Add(PageSessionKeyConstants.TAX_EXCISE_COMPANIES_LIST, companies);

				this.GridSizeDropDown.SetPageSize(this.DataGridAssignedCompanies, companies.Count);
				this.DataGridAssignedCompanies.DataSource = companies;
				this.DataGridAssignedCompanies.DataBind();

				this.Page.Session.Add(PageSessionKeyConstants.TAX_EXCISE_DETAIL_MODE, RequestTypes.Add);

				// Enable or disable the Product field based on Add or Edit mode.
				this.EnableProductField();
			}
		}

		/// <summary>
		///    This method will handle OK event to save the Excise detail to the database.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void OkButtonOnClick(object sender, EventArgs e)
		{
			bool successful = this.SaveExcise();

			if (successful)
			{
				this.RemoveSessionKeys();
				this.Redirect("TaxRateExciseSummaryForm.aspx");
			}
		}

		/// <summary>
		///    This method will handle the On Init event for the page. It will initialize the base
		///    page OnInit and setup event handlers.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInit(EventArgs e)
		{
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		///    This is the main entry point for the Excise Detail page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e)
		{
			if (this.Page.IsPostBack == false)
			{
				this.LoadExciseDetails();
				this.UpdateExciseCompanyGrid();
			}

			// Enable or disable the Product field based on Add or Edit mode.
			this.EnableProductField();
		}

		/// <summary>
		///    This method will handle the page index change. It will update the view to the
		///    new page.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		private void DataGridAssignedCompaniesPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.DataGridAssignedCompanies.EditItemIndex > -1)
			{
				return;
			}

			this.DataGridAssignedCompanies.CurrentPageIndex = e.NewPageIndex;

			if (this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_COMPANIES_LIST] != null)
			{
				var companies = (List<TaxCompanyMapDO>)this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_COMPANIES_LIST];

				this.GridSizeDropDown.SetPageSize(this.DataGridAssignedCompanies, companies.Count);
				this.DataGridAssignedCompanies.DataSource = companies;
				this.DataGridAssignedCompanies.DataBind();
			}

			// Enable or disable the Product field based on Add or Edit mode.
			this.EnableProductField();
		}

		/// <summary>
		///    This method will enable and disable controls.
		/// </summary>
		/// <param name="enable"></param>
		private void EnableControls(bool enable)
		{
			this.AddBottomButton.Enabled = enable;
			this.AddTopButton.Enabled = enable;
			this.NewButton.Enabled = enable;
			this.OkButton.Enabled = enable;
			this.CancelButton.Enabled = enable;
			this.ProductSelectControl.Enabled = enable;
			this.ExciseRateTextBox.Enabled = enable;
			this.ExciseDateField.Enabled = enable;
		}

		/// <summary>
		///    This method will enable/disable the Product field depending if mode is
		///    Add or Edit.
		/// </summary>
		private void EnableProductField()
		{
			if (this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_DETAIL_MODE] == null)
			{
				this.ProductSelectControl.Enabled = true;
			}
			else
			{
				var requestType = (RequestTypes)this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_DETAIL_MODE];

				switch (requestType)
				{
					case RequestTypes.Add:
						this.ProductSelectControl.Enabled = true;
						break;
					case RequestTypes.Edit:
						this.ProductSelectControl.Enabled = false;
						break;
					default:
						this.ProductSelectControl.Enabled = true;
						break;
				}
			}
		}

		/// <summary>
		///    This method will return the product guid given a product ID. It will return
		///    zero if the product ID is not found.
		/// </summary>
		/// <param name="productID"></param>
		/// <returns></returns>
		private Guid GetProductGuid(string productID)
		{
			Guid productGuid = Guid.Empty;

			if (!string.IsNullOrEmpty(productID))
			{
				productGuid = FMChannelHelper.MakeCall<IProducts, Guid>(
																	 x =>
																	 x.GetIdentityGuid(this.security,productID)
																);

			}

			return productGuid;
		}

		/// <summary>
		///    This method will return the master record guid given a product ID. It will return
		///    zero if the product ID is not found.
		/// </summary>
		/// <param name="productID"></param>
		/// <returns></returns>
		private Guid GetMasterRecordGuid(string productID)
		{
			Guid masterRecordGuid = Guid.Empty;

			if (!string.IsNullOrEmpty(productID))
			{
				masterRecordGuid = FMChannelHelper.MakeCall<IProducts, Guid>(
																	 x =>
																	 x.GetMasterRecordGuidFromID(this.security, productID)
																);

			}

			return masterRecordGuid;
		}

		/// <summary>
		///    This method will return the request type of Edit, Add, or None.
		///    The default is None.
		/// </summary>
		/// <returns></returns>
		private RequestTypes GetRequestMode()
		{
			string mode = this.Request.GetQueryOrFormValue("Mode");

			if (string.IsNullOrEmpty(mode))
			{
				return RequestTypes.NONE;
			}
		    if (mode.ToUpper().Contains("EDIT"))
		    {
		        return RequestTypes.Edit;
		    }
		    if (mode.ToUpper().Contains("ADD"))
		    {
		        return RequestTypes.Add;
		    }
		    return RequestTypes.NONE;
		}

		/// <summary>
		///    This method will initialize event handles.
		/// </summary>
		private void InitializeComponent()
		{
			this.DataGridAssignedCompanies.CancelCommand +=
				this.DataGridAssignedCompaniesCancelCommand;
			this.DataGridAssignedCompanies.UpdateCommand +=
				this.DataGridAssignedCompaniesUpdateCommand;
			this.DataGridAssignedCompanies.DeleteCommand +=
				this.DataGridAssignedCompaniesDeleteCommand;
			this.DataGridAssignedCompanies.ItemDataBound +=
				this.DataGridAssignedCompaniesItemDataBound;
			this.DataGridAssignedCompanies.PageIndexChanged +=
				this.DataGridAssignedCompaniesPageIndexChanged;
		}

		/// <summary>
		///    This method will load the Excise details from session.
		/// </summary>
		private void LoadExciseDetails()
		{
			var except = new Exception("No Excise Tax object in session.");
			ExciseTaxDO exciseTaxDO;

			if (this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_DETAIL_MODE] != null)
			{
				this.Page.Session.Remove(PageSessionKeyConstants.TAX_EXCISE_DETAIL_MODE);
			}

			switch (this.GetRequestMode())
			{
				case RequestTypes.Edit:
					if (this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_SUMMARY_OBJECT] == null)
					{
						this.ErrorHandler(except);
					}
					else
					{
						exciseTaxDO = this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_SUMMARY_OBJECT] as ExciseTaxDO;
						this.Page.Session.Add(PageSessionKeyConstants.TAX_EXCISE_DETAIL_OBJECT, exciseTaxDO);
						this.Page.Session.Remove(PageSessionKeyConstants.TAX_EXCISE_SUMMARY_OBJECT);

						if (exciseTaxDO == null)
						{
							this.ErrorHandler(except);
						}
						else
						{
							this.ExciseRateTextBox.Text = this.RateFormatter(exciseTaxDO.ExciseRate);
							this.ExciseDateField.CurrentValue = exciseTaxDO.ExciseDate;
							this.ProductSelectControl.Text = exciseTaxDO.Product;
							this.Page.Session.Add(PageSessionKeyConstants.TAX_EXCISE_DETAIL_MODE, RequestTypes.Edit);
						}
					}
					break;
				case RequestTypes.Add:
					if (this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_SUMMARY_OBJECT] == null)
					{
						this.ErrorHandler(except);
					}
					else
					{
						exciseTaxDO = this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_SUMMARY_OBJECT] as ExciseTaxDO;
						this.Page.Session.Add(PageSessionKeyConstants.TAX_EXCISE_DETAIL_OBJECT, exciseTaxDO);
						this.Page.Session.Remove(PageSessionKeyConstants.TAX_EXCISE_SUMMARY_OBJECT);

						if (exciseTaxDO == null)
						{
							this.ErrorHandler(except);
						}
						else
						{
							this.ExciseRateTextBox.Text = this.RateFormatter(0.0);
							this.ExciseDateField.CurrentValue = DateTimeOffset.Now;
							this.ProductSelectControl.Text = "";
							this.Page.Session.Add(PageSessionKeyConstants.TAX_EXCISE_DETAIL_MODE, RequestTypes.Add);
						}
					}
					break;
				default:
					this.ErrorHandler(except);
					break;
			}
		}

		/// <summary>
		///    This method will return a List  that contains an associated company list of only
		///    the companies that required to be added. It compares the modified list with the original
		///    list from the database. The default is to return a null.
		/// </summary>
		/// <param name="exciseDO"></param>
		/// <param name="modifyCompanyList"></param>
		/// <returns></returns>
		private List<TaxCompanyMapDO> NewAssociatedCompanies(ExciseTaxDO exciseDO, List<TaxCompanyMapDO> modifyCompanyList)
		{
			List<TaxCompanyMapDO> companyList = FMChannelHelper.MakeCall<IExcises, List<TaxCompanyMapDO>>(
																	 x =>
																	 x.GetExciseCompanies(exciseDO, this.security)
																);

			var newAssociatedCompanyList = new List<TaxCompanyMapDO>();

			// Return the modified company list since there were no originals
			// in the database.
			if ((companyList == null) || (companyList.Count <= 0))
			{
				return modifyCompanyList;
			}

			if ((modifyCompanyList != null) && (modifyCompanyList.Count > 0))
			{
				foreach (TaxCompanyMapDO modifiedCompanyMap in modifyCompanyList)
				{
					if (companyList.Contains(modifiedCompanyMap) == false)
					{
					    var newCompanyMapDO = new TaxCompanyMapDO
					                          {
					                              CompanyID = modifiedCompanyMap.CompanyID,
					                              CompanyGuid = modifiedCompanyMap.CompanyGuid
					                          };

					    newAssociatedCompanyList.Add(newCompanyMapDO);
					}
				}
			}

			if (newAssociatedCompanyList.Count <= 0)
			{
			    return null;
			}
		    return newAssociatedCompanyList;
		}

		/// <summary>
		///    This method will return the Rate formatted as follows:
		///    2,300.00000. It will always have five decimal places.
		/// </summary>
		/// <param name="inRate"></param>
		/// <returns></returns>
		private string RateFormatter(double inRate)
		{
			return inRate.ToString("#,###.00000");
		}

		/// <summary>
		///    This method will return all the session keys associated with this
		///    detail page.
		/// </summary>
		private void RemoveSessionKeys()
		{
			if (this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_SUMMARY_OBJECT] != null)
			{
				this.Page.Session.Remove(PageSessionKeyConstants.TAX_EXCISE_SUMMARY_OBJECT);
			}

			if (this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_DETAIL_OBJECT] != null)
			{
				this.Page.Session.Remove(PageSessionKeyConstants.TAX_EXCISE_DETAIL_OBJECT);
			}

			if (this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_COMPANIES_LIST] != null)
			{
				this.Page.Session.Remove(PageSessionKeyConstants.TAX_EXCISE_COMPANIES_LIST);
			}

			if (this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_DELETED_COMPANIES_LIST] != null)
			{
				this.Page.Session.Remove(PageSessionKeyConstants.TAX_EXCISE_DELETED_COMPANIES_LIST);
			}

			if (this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_DETAIL_MODE] != null)
			{
				this.Page.Session.Remove(PageSessionKeyConstants.TAX_EXCISE_DETAIL_MODE);
			}
		}

		/// <summary>
		///    This method will return True if the Excise detail information was saved
		///    successfully in the database.
		/// </summary>
		/// <returns></returns>
		private bool SaveExcise()
		{
			bool successful = false;

			// Validate the input
			// Make sure Product is populated
			if (this.ProductSelectControl.Text.Trim().Length == 0)
			{
				this.ErrorHandler(new Exception("Product is Required."));
				return false;
			}

			// Make sure rate is populated
			if (this.ExciseRateTextBox.Text.Trim().Length == 0)
			{
				this.ErrorHandler(new Exception("Excise Rate is Required."));
				return false;
			}

			// Make sure Value is numeric
			double exciseRate;
			try
			{
				exciseRate = Convert.ToDouble(this.ExciseRateTextBox.Text.Trim());
			}
			catch
			{
				this.ErrorHandler(new Exception("Excise Rate Must be Numeric."));
				return false;
			}

			if (string.IsNullOrEmpty(this.ExciseDateField.Text))
			{
				this.ErrorHandler(new Exception("Excise Date Must be a valid date."));
				return false;
			}

			ExciseTaxDO exciseTaxDO;
			List<TaxCompanyMapDO> modifiedCompanyList = null;

			if (this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_DETAIL_OBJECT] == null)
			{
				exciseTaxDO = new ExciseTaxDO();
			}
			else
			{
				exciseTaxDO = this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_DETAIL_OBJECT] as ExciseTaxDO;
			}

			if (this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_COMPANIES_LIST] != null)
			{
				modifiedCompanyList = (List<TaxCompanyMapDO>)this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_COMPANIES_LIST];
			}

			// Update/Insert the Excise object
		    if (exciseTaxDO != null)
		    {
		        exciseTaxDO.Product = this.ProductSelectControl.Text.Trim();
		        exciseTaxDO.ExciseRate = exciseRate;
		        exciseTaxDO.ExciseDate = this.ExciseDateField.CurrentValue;

		        try
		        {
		            if (exciseTaxDO.IdentityGuid == Guid.Empty)
		            {
		                exciseTaxDO.ProductGuid = this.GetProductGuid(exciseTaxDO.Product);
		                exciseTaxDO.ProductGuid = this.GetMasterRecordGuid(exciseTaxDO.Product);

		                // Ensure that a product guid can be found.
		                if (exciseTaxDO.ProductGuid == Guid.Empty)
		                {
		                    string errorMsg = "Could not find Product: '" + exciseTaxDO.Product + "'";
		                    this.ErrorHandler(new Exception(errorMsg));
		                }
		                else
		                {
		                    // Create a new existing Excise tax object.
		                    List<TaxCompanyMapDO> companyList = null;

		                    if (this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_COMPANIES_LIST] != null)
		                    {
		                        companyList = (List<TaxCompanyMapDO>)this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_COMPANIES_LIST];
		                    }

		                    exciseTaxDO.IdentityGuid = FMChannelHelper.MakeCall<IExcises, Guid>(
		                        x =>
		                            x.Add(exciseTaxDO,this.security,companyList)
		                        );

		                    successful = true;
		                }
		            }
		            else
		            {
		                // Update an existing Excise tax object.
		                List<TaxCompanyMapDO> deletedCompanyList = null;
		                List<TaxCompanyMapDO> newAssociatedCompanyList = this.NewAssociatedCompanies(exciseTaxDO, modifiedCompanyList);

		                if (this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_DELETED_COMPANIES_LIST] != null)
		                {
		                    deletedCompanyList =
		                        (List<TaxCompanyMapDO>)this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_DELETED_COMPANIES_LIST];
		                }

		                FMChannelHelper.MakeCall<IExcises>(
		                    x =>
		                        x.Save(exciseTaxDO, this.security, newAssociatedCompanyList, deletedCompanyList)
		                    );

		                successful = true;
		            }
		        }
		        catch (Exception ex)
		        {
		            successful = false;
		            this.ErrorHandler(ex);
		        }
		    }

		    return successful;
		}

		/// <summary>
		///    This method will update the Excise company assignment grid with companies
		///    associated to the selected Excise.
		/// </summary>
		private void UpdateExciseCompanyGrid()
		{
			if (this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_DETAIL_OBJECT] != null)
			{
				var exciseDO = this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_DETAIL_OBJECT] as ExciseTaxDO;
				List<TaxCompanyMapDO> companies = FMChannelHelper.MakeCall<IExcises, List<TaxCompanyMapDO>>(
																	 x =>
																	 x.GetExciseCompanies(exciseDO, this.security)
																);

				if (companies != null)
				{
					this.GridSizeDropDown.SetPageSize(this.DataGridAssignedCompanies, companies.Count);
					this.DataGridAssignedCompanies.DataSource = companies;
					this.DataGridAssignedCompanies.DataBind();

					if (this.Page.Session[PageSessionKeyConstants.TAX_EXCISE_COMPANIES_LIST] != null)
					{
						this.Page.Session.Remove(PageSessionKeyConstants.TAX_EXCISE_COMPANIES_LIST);
					}

					this.Page.Session.Add(PageSessionKeyConstants.TAX_EXCISE_COMPANIES_LIST, companies);
				}
			}
		}

		#endregion
	}
}