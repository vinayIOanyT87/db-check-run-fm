// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaxGstDetailForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the TaxGstDetailForm type.
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

    public partial class TaxGstDetailForm : AccountingWebFormView
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

			if (this.Page.Session[PageSessionKeyConstants.TAX_GST_COMPANIES_LIST] == null)
			{
				companies = new List<TaxCompanyMapDO>();
				this.Page.Session.Add(PageSessionKeyConstants.TAX_GST_COMPANIES_LIST, companies);
			}

			companies = this.Page.Session[PageSessionKeyConstants.TAX_GST_COMPANIES_LIST] as List<TaxCompanyMapDO>;

			if (companies == null)
			{
				this.Page.Session.Remove(PageSessionKeyConstants.TAX_GST_COMPANIES_LIST);
				companies = this.Page.Session[PageSessionKeyConstants.TAX_GST_COMPANIES_LIST] as List<TaxCompanyMapDO>;
			}

			var gstCompanyMapDO = new TaxCompanyMapDO();
			companies?.Add(gstCompanyMapDO);

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
		///    in session and return back to the GST Summary page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void CancelButtonOnClick(object sender, EventArgs e)
		{
			this.RemoveSessionKeys();
			this.Redirect("TaxRateGstSummaryForm.aspx");
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

		    var companies = this.Page.Session[PageSessionKeyConstants.TAX_GST_COMPANIES_LIST] as List<TaxCompanyMapDO>;

		    if (companies != null)
		    {
		        var companyMapDO = new TaxCompanyMapDO();
		        companies.Remove(companyMapDO);

		        this.GridSizeDropDown.SetPageSize(this.DataGridAssignedCompanies, companies.Count);
		        this.DataGridAssignedCompanies.DataSource = companies;
		        this.DataGridAssignedCompanies.DataBind();
		    }

		    // Enable or disable the GST Code field based on Add or Edit mode.
			this.EnableGstCode();
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

			if (this.Page.Session[PageSessionKeyConstants.TAX_GST_COMPANIES_LIST] != null)
			{
				companies = this.Page.Session[PageSessionKeyConstants.TAX_GST_COMPANIES_LIST] as List<TaxCompanyMapDO>;

				if (companies == null)
				{
					this.ErrorHandler(new Exception("Company list object is missing."));
				}
				else
				{
				    List<TaxCompanyMapDO> deletedCompanies;
				    if (this.Page.Session[PageSessionKeyConstants.TAX_GST_DELETED_COMPANIES_LIST] == null)
					{
						deletedCompanies = new List<TaxCompanyMapDO>();
						this.Page.Session.Add(PageSessionKeyConstants.TAX_GST_DELETED_COMPANIES_LIST, deletedCompanies);
					}
					else
					{
						deletedCompanies =
							(List<TaxCompanyMapDO>)this.Page.Session[PageSessionKeyConstants.TAX_GST_DELETED_COMPANIES_LIST];
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

			// Enable or disable the GST Code field based on Add or Edit mode.
			this.EnableGstCode();
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

			if (this.Page.Session[PageSessionKeyConstants.TAX_GST_COMPANIES_LIST] != null)
			{
				companies = this.Page.Session[PageSessionKeyConstants.TAX_GST_COMPANIES_LIST] as List<TaxCompanyMapDO>;

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
					        this.Page.Session[PageSessionKeyConstants.TAX_GST_DELETED_COMPANIES_LIST] as List<TaxCompanyMapDO>;

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

			// Enable or disable the GST Code field based on Add or Edit mode.
			this.EnableGstCode();
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

		    var companies = this.Page.Session[PageSessionKeyConstants.TAX_GST_COMPANIES_LIST] as List<TaxCompanyMapDO>;

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
			bool successful = this.SaveGst();

			if (successful)
			{
				this.GstDateField.CurrentValue = DateTimeOffset.Now;
				this.GSTCodeTextBox.Text = "";
				this.GSTRateTextBox.Text = "0.0";

				// Remove all session keys related to the previous detail object.
				this.RemoveSessionKeys();

				// Create a new GST data object in place it in session.
				var gstDO = new GoodsAndServicesTaxDO();
				this.Page.Session.Add(PageSessionKeyConstants.TAX_GST_DETAIL_OBJECT, gstDO);

				// Create a new companies hash table and add it to session.
				var companies = new List<TaxCompanyMapDO>();
				this.Page.Session.Add(PageSessionKeyConstants.TAX_GST_COMPANIES_LIST, companies);

				this.GridSizeDropDown.SetPageSize(this.DataGridAssignedCompanies, companies.Count);
				this.DataGridAssignedCompanies.DataSource = companies;
				this.DataGridAssignedCompanies.DataBind();

				this.Page.Session.Add(PageSessionKeyConstants.TAX_GST_DETAIL_MODE, RequestTypes.Add);

				// Enable or disable the GST Code field based on Add or Edit mode.
				this.EnableGstCode();
			}
		}

		/// <summary>
		///    This method will handle OK event to save the GST detail to the database.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void OkButtonOnClick(object sender, EventArgs e)
		{
			bool successful = this.SaveGst();

			if (successful)
			{
				this.RemoveSessionKeys();
				this.Redirect("TaxRateGstSummaryForm.aspx");
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
		///    This is the main entry point for the GST Detail page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e)
		{
			if (this.Page.IsPostBack == false)
			{
				this.LoadGstDetails();
				this.UpdateGstCompanyGrid();
			}

			// Enable or disable the GST Code field based on Add or Edit mode.
			this.EnableGstCode();
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

			if (this.Page.Session[PageSessionKeyConstants.TAX_GST_COMPANIES_LIST] != null)
			{
				var companies = (List<TaxCompanyMapDO>)this.Page.Session[PageSessionKeyConstants.TAX_GST_COMPANIES_LIST];

				this.GridSizeDropDown.SetPageSize(this.DataGridAssignedCompanies, companies.Count);
				this.DataGridAssignedCompanies.DataSource = companies;
				this.DataGridAssignedCompanies.DataBind();
			}

			// Enable or disable the GST Code field based on Add or Edit mode.
			this.EnableGstCode();
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
			this.GSTCodeTextBox.Enabled = enable;
			this.GSTRateTextBox.Enabled = enable;
			this.GstDateField.Enabled = enable;
		}

		/// <summary>
		///    This method will enable/disable the GST Code field depending if mode is
		///    Add or Edit.
		/// </summary>
		private void EnableGstCode()
		{
			if (this.Page.Session[PageSessionKeyConstants.TAX_GST_DETAIL_MODE] == null)
			{
				this.GSTCodeTextBox.Enabled = true;
			}
			else
			{
				var requestType = (RequestTypes)this.Page.Session[PageSessionKeyConstants.TAX_GST_DETAIL_MODE];

				switch (requestType)
				{
					case RequestTypes.Add:
						this.GSTCodeTextBox.Enabled = true;
						break;
					case RequestTypes.Edit:
						this.GSTCodeTextBox.Enabled = false;
						break;
					default:
						this.GSTCodeTextBox.Enabled = true;
						break;
				}
			}
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
			else if (mode.ToUpper().Contains("EDIT"))
			{
				return RequestTypes.Edit;
			}
			else if (mode.ToUpper().Contains("ADD"))
			{
				return RequestTypes.Add;
			}
			else
			{
				return RequestTypes.NONE;
			}
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
		///    This method will load the GST details from session.
		/// </summary>
		private void LoadGstDetails()
		{
			var except = new Exception("No GST Tax object in session.");
			GoodsAndServicesTaxDO gstTaxDO;

			if (this.Page.Session[PageSessionKeyConstants.TAX_GST_DETAIL_MODE] != null)
			{
				this.Page.Session.Remove(PageSessionKeyConstants.TAX_GST_DETAIL_MODE);
			}

			switch (this.GetRequestMode())
			{
				case RequestTypes.Edit:
					if (this.Page.Session[PageSessionKeyConstants.TAX_GST_SUMMARY_OBJECT] == null)
					{
						this.ErrorHandler(except);
					}
					else
					{
						gstTaxDO = this.Page.Session[PageSessionKeyConstants.TAX_GST_SUMMARY_OBJECT] as GoodsAndServicesTaxDO;
						this.Page.Session.Add(PageSessionKeyConstants.TAX_GST_DETAIL_OBJECT, gstTaxDO);
						this.Page.Session.Remove(PageSessionKeyConstants.TAX_GST_SUMMARY_OBJECT);

						if (gstTaxDO == null)
						{
							this.ErrorHandler(except);
						}
						else
						{
							this.GSTCodeTextBox.Text = gstTaxDO.GstCode;
							this.GSTRateTextBox.Text = gstTaxDO.GstValue.ToString();
							this.GstDateField.CurrentValue = gstTaxDO.GstDate;

							//Set the title label with a key field from the bound object appended
							this.EntityExportTitleLabel.Text = this.GetTitleLabelText(this.EntityExportTitleLabel.Text, gstTaxDO.GstCode);

							this.Page.Session.Add(PageSessionKeyConstants.TAX_GST_DETAIL_MODE, RequestTypes.Edit);
						}
					}
					break;
				case RequestTypes.Add:
					if (this.Page.Session[PageSessionKeyConstants.TAX_GST_SUMMARY_OBJECT] == null)
					{
						this.ErrorHandler(except);
					}
					else
					{
						gstTaxDO = this.Page.Session[PageSessionKeyConstants.TAX_GST_SUMMARY_OBJECT] as GoodsAndServicesTaxDO;
						this.Page.Session.Add(PageSessionKeyConstants.TAX_GST_DETAIL_OBJECT, gstTaxDO);
						this.Page.Session.Remove(PageSessionKeyConstants.TAX_GST_SUMMARY_OBJECT);

						if (gstTaxDO == null)
						{
							this.ErrorHandler(except);
						}
						else
						{
							this.GSTCodeTextBox.Text = "";
							this.GSTRateTextBox.Text = "0.0";
							this.GstDateField.CurrentValue = DateTimeOffset.Now;

							this.Page.Session.Add(PageSessionKeyConstants.TAX_GST_DETAIL_MODE, RequestTypes.Add);
						}
					}
					break;
				default:
					this.ErrorHandler(except);
					break;
			}
		}

		/// <summary>
		///    This method will return a hash table that contains an associated company list of only
		///    the companies that required to be added. It compares the modified list with the original
		///    list from the database. The default is to return a null.
		/// </summary>
		/// <param name="gstDO"></param>
		/// <param name="modifyCompanyList"></param>
		/// <returns></returns>
		private List<TaxCompanyMapDO> NewAssociatedCompanies(
			GoodsAndServicesTaxDO gstDO, List<TaxCompanyMapDO> modifyCompanyList)
		{
			List<TaxCompanyMapDO> companyList = FMChannelHelper.MakeCall<IGoodsAndServices, List<TaxCompanyMapDO>>(
																	 x =>
																	 x.GetGSTCompanies(gstDO, this.security)
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
			else
			{
				return newAssociatedCompanyList;
			}
		}

		/// <summary>
		///    This method will return all the session keys associated with this
		///    detail page.
		/// </summary>
		private void RemoveSessionKeys()
		{
			if (this.Page.Session[PageSessionKeyConstants.TAX_GST_SUMMARY_OBJECT] != null)
			{
				this.Page.Session.Remove(PageSessionKeyConstants.TAX_GST_SUMMARY_OBJECT);
			}

			if (this.Page.Session[PageSessionKeyConstants.TAX_GST_DETAIL_OBJECT] != null)
			{
				this.Page.Session.Remove(PageSessionKeyConstants.TAX_GST_DETAIL_OBJECT);
			}

			if (this.Page.Session[PageSessionKeyConstants.TAX_GST_COMPANIES_LIST] != null)
			{
				this.Page.Session.Remove(PageSessionKeyConstants.TAX_GST_COMPANIES_LIST);
			}

			if (this.Page.Session[PageSessionKeyConstants.TAX_GST_DELETED_COMPANIES_LIST] != null)
			{
				this.Page.Session.Remove(PageSessionKeyConstants.TAX_GST_DELETED_COMPANIES_LIST);
			}
		}

		/// <summary>
		///    This method will return True if the GST detail information was saved
		///    successfully in the database.
		/// </summary>
		/// <returns></returns>
		private bool SaveGst()
		{
			bool successful = false;
			bool dateTimeValueChanged = false;
			DateTimeOffset originalDateTime = DateTimeOffset.Now;

			// Validate the input
			// Make sure Code is populated
			if (this.GSTCodeTextBox.Text.Trim().Length == 0)
			{
				this.ErrorHandler(new Exception("GST Code is Required."));
				return false;
			}

			// Make sure rate is populated
			if (this.GSTRateTextBox.Text.Trim().Length == 0)
			{
				this.ErrorHandler(new Exception("GST Rate is Required."));
				return false;
			}

			// Make sure Value is numeric
			double gstRate;
			try
			{
				gstRate = Convert.ToDouble(this.GSTRateTextBox.Text.Trim());
			}
			catch
			{
				this.ErrorHandler(new Exception("GST Rate Must be Numeric."));
				return false;
			}

			if (string.IsNullOrEmpty(this.GstDateField.Text))
			{
				this.ErrorHandler(new Exception("GST Date Must be a valid date."));
				return false;
			}

			GoodsAndServicesTaxDO gstTaxDO;
			List<TaxCompanyMapDO> modifiedCompanyList = null;

			if (this.Page.Session[PageSessionKeyConstants.TAX_GST_DETAIL_OBJECT] == null)
			{
				gstTaxDO = new GoodsAndServicesTaxDO();
			}
			else
			{
				gstTaxDO = this.Page.Session[PageSessionKeyConstants.TAX_GST_DETAIL_OBJECT] as GoodsAndServicesTaxDO;
			}

			if (this.Page.Session[PageSessionKeyConstants.TAX_GST_COMPANIES_LIST] != null)
			{
				modifiedCompanyList = (List<TaxCompanyMapDO>)this.Page.Session[PageSessionKeyConstants.TAX_GST_COMPANIES_LIST];
			}

			if (gstTaxDO != null && (gstTaxDO.IdentityGuid != Guid.Empty && gstTaxDO.GstDate != this.GstDateField.CurrentValue))
			{
				dateTimeValueChanged = true;
				originalDateTime = gstTaxDO.GstDate;
			}

			// Update/Insert the GST object
		    if (gstTaxDO != null)
		    {
		        gstTaxDO.GstCode = this.GSTCodeTextBox.Text.Trim();
		        gstTaxDO.GstValue = gstRate;
		        gstTaxDO.GstDate = this.GstDateField.CurrentValue;

		        try
		        {
		            if (gstTaxDO.IdentityGuid == Guid.Empty)
		            {
		                // Create a new existing GST tax object.
		                List<TaxCompanyMapDO> companyList = null;

		                if (this.Page.Session[PageSessionKeyConstants.TAX_GST_COMPANIES_LIST] != null)
		                {
		                    companyList = (List<TaxCompanyMapDO>)this.Page.Session[PageSessionKeyConstants.TAX_GST_COMPANIES_LIST];
		                }

		                gstTaxDO.IdentityGuid = FMChannelHelper.MakeCall<IGoodsAndServices, Guid>(
		                    x =>
		                        x.Add(gstTaxDO, this.security, companyList)
		                    );

		                successful = true;
		            }
		            else
		            {
		                // Update an existing GST tax object.
		                List<TaxCompanyMapDO> deletedCompanyList = null;
		                List<TaxCompanyMapDO> completeCompanyList = null;

		                if (dateTimeValueChanged)
		                {
		                    completeCompanyList = (List<TaxCompanyMapDO>)this.Page.Session[PageSessionKeyConstants.TAX_GST_COMPANIES_LIST];
		                }

		                var newAssociatedCompanyList = this.NewAssociatedCompanies(gstTaxDO, modifiedCompanyList);

		                if (this.Page.Session[PageSessionKeyConstants.TAX_GST_DELETED_COMPANIES_LIST] != null)
		                {
		                    deletedCompanyList =
		                        (List<TaxCompanyMapDO>)this.Page.Session[PageSessionKeyConstants.TAX_GST_DELETED_COMPANIES_LIST];
		                }

		                FMChannelHelper.MakeCall<IGoodsAndServices>(
		                    x =>
		                        x.Save(gstTaxDO, this.security, newAssociatedCompanyList, deletedCompanyList, completeCompanyList)
		                    );

		                successful = true;
		            }
		        }
		        catch (Exception ex)
		        {
		            successful = false;
		            if (dateTimeValueChanged)
		            {
		                gstTaxDO.GstDate = originalDateTime;
		            }
		            this.ErrorHandler(ex);
		        }
		    }

		    return successful;
		}

		/// <summary>
		///    This method will update the GST company assignment grid with companies
		///    associated to the selected GST.
		/// </summary>
		private void UpdateGstCompanyGrid()
		{
			if (this.Page.Session[PageSessionKeyConstants.TAX_GST_DETAIL_OBJECT] != null)
			{
				var gstDO = this.Page.Session[PageSessionKeyConstants.TAX_GST_DETAIL_OBJECT] as GoodsAndServicesTaxDO;

				List<TaxCompanyMapDO> companies = FMChannelHelper.MakeCall<IGoodsAndServices, List<TaxCompanyMapDO>>(
																	 x =>
																	 x.GetGSTCompanies(gstDO, this.security)
																);

				if (companies != null)
				{
					this.GridSizeDropDown.SetPageSize(this.DataGridAssignedCompanies, companies.Count);
					this.DataGridAssignedCompanies.DataSource = companies;
					this.DataGridAssignedCompanies.DataBind();

					if (this.Page.Session[PageSessionKeyConstants.TAX_GST_COMPANIES_LIST] != null)
					{
						this.Page.Session.Remove(PageSessionKeyConstants.TAX_GST_COMPANIES_LIST);
					}

					this.Page.Session.Add(PageSessionKeyConstants.TAX_GST_COMPANIES_LIST, companies);
				}
			}
		}

		#endregion
	}
}