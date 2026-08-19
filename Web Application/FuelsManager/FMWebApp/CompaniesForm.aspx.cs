// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompaniesForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Code behind for CompaniesForm.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Drawing;
	using System.Globalization;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;
	using FMBusinessObjects.UtilityObjects;

	using global::FMWebApp;

	/// <summary>
	/// Code behind for CompaniesForm.
	/// </summary>
	public partial class CompaniesForm : FMAutoSubmitFormBase, IEntityDiscovery, IMenuDiscovery
	{
		#region Constants and Fields

		/// <summary>
		/// Session key
		/// </summary>
		private const string CompanyFindString = "CompanyFindString";

		/// <summary>
		/// Session key
		/// </summary>
		private const string SortDirection = "CompaniesForm.SortDirection";

		/// <summary>
		/// Session key
		/// </summary>
		private const string SortExpression = "CompaniesForm.SortExpression";

		/// <summary>
		/// The dataset for the form
		/// </summary>
		private DataSet ds = new DataSet();

		/// <summary>
		/// The data view for the form
		/// </summary>
		private DataView dv = new DataView();

		/// <summary>
		/// The search string for the form.
		/// </summary>
		private string searchString;

        /// <summary>
        /// Retain the state of the Show Hidden checkbox
        /// </summary>
        private bool SessionCompanySummaryShowHiddenChecked
        {
            get
            {
                if (this.Session["CompanySummaryShowHiddenChecked"] is bool)
                {
                    return (bool)this.Session["CompanySummaryShowHiddenChecked"];
                }
                else
                {
                    return false;
                }
            }

            set
            {
                this.Session.Add("CompanySummaryShowHiddenChecked", value);
            }
        }

		#endregion

		#region Explicit Interface Properties

		/// <summary>
		/// Gets a value indicating whether [entity assignable].
		/// </summary>
		/// <value>
		///   <c>true</c> if [entity assignable]; otherwise, <c>false</c>.
		/// </value>
		bool IEntityDiscovery.EntityAssignable
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Gets the type of the entity engine.
		/// </summary>
		/// <value>
		/// The type of the entity engine.
		/// </value>
		Type IEntityDiscovery.EntityEngineType
		{
			get
			{
				return typeof(ICompanies);
			}
		}

		/// <summary>
		/// Gets the type of the entity.
		/// </summary>
		/// <value>
		/// The type of the entity.
		/// </value>
		ENTITY_TYPE IEntityDiscovery.EntityType
		{
			get
			{
				return ENTITY_TYPE.COMPANY;
			}
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Gets a list of menu items that should be displayed for the current user.
		/// </summary>
		/// <param name="security">
		/// The security object of the current session
		/// </param>
		/// <param name="siteGroup">
		/// Whether the current logged-in site is a site group
		/// </param>
		/// <param name="options">
		/// Hardware key options
		/// </param>
		/// <returns>
		/// List of menu items to be displayed
		/// </returns>
		public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1,ushort word2, ushort useNewLicenseKey, uint options)
		{
            if (useNewLicenseKey == 1)
            {

            }
            else
            {
                // Depends Upon Accounting
                if ((options & 0x80100) == 0)
                {
                    return null;
                }
            }
            if (!security.HasRight(RIGHT.VIEW_COMPANY_DATA) && !security.HasRight(RIGHT.MODIFY_COMPANY_DATA))
			{
				return null;
			}

			var menuItems = new List<FMMenuItem>();

			var menuItem = new FMMenuItem
				{
					MenuItemType = FMMenuItemType.ACCOUNTING_COMPANIES_COMPANIES, 
					RootMenuName = "Accounting", 
					CategoryName = "Companies", 
					ItemName = "Companies", 
					NavigateUrl = "CompaniesForm.aspx", 
					ApplyDataDictionary = ApplyDataDictionary.Apply, 
					SortOrder = 1
				};

			menuItems.Add(menuItem);

			return menuItems;
		}

		#endregion

		#region Explicit Interface Methods


		/// <summary>
		/// Enumerates the entity maps.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="type">The type.</param>
		/// <returns>A collection of entity to site maps</returns>
		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(SecurityClass security, ENTITY_ASSIGNMENT_TYPE type)
		{
            CompanyCollectionClass companyCollection = null;
            if (type == ENTITY_ASSIGNMENT_TYPE.UNDELEGATED)
            {
                companyCollection = FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(x => x.EnumerateUndelegated(security));
            }
            else
            {
                companyCollection = FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(x => x.EnumerateBySite(security));
            }

			var entityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			foreach (CompanyClass company in companyCollection)
			{
				if (type == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if (security.SiteGuid == company.SiteGuid)
					{
						continue;
					}

					if (security.LoginSiteGuid != company.SiteGuid)
					{
						continue;
					}
				}
				else
				{
                    //For entity types supporting Record Versioning, assignments can be cascaded, irrespective of whether Record Versioning is turned on or off.
                    if ((security.SiteGuid != company.SiteGuid) && (security.SiteGuid != company.AssignedToSiteGuid))
					{
						continue;
					}
				}

				var entityToSiteMap = new EntityToSiteMapClass(company);
                entityToSiteMap.IdentityGuid = company.MasterRecordGuid; //The EntityToSiteMap references Company records by their MasterRecordGuids instead of their actual CompanyGuids.
				entityToSiteMapCollection.Add(entityToSiteMap);
			}

			return entityToSiteMapCollection;
		}

		/// <summary>
		/// Gets the identity GUID.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="id">The id.</param>
		/// <returns>The identity guid.</returns>
		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string id)
		{
			return FMChannelHelper.MakeCall<ICompanies, Guid>(x => x.GetIdentityGuid(security, id));
		}

		/// <summary>
		/// Sets the site GUID.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="guid">The GUID.</param>
		/// <param name="siteGuid">The site GUID.</param>
		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid siteGuid)
		{
			var company = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(x => x.Get(security, guid));

			company.SiteGuid = siteGuid;
			FMChannelHelper.MakeCall<ICompanies>(x => x.Modify(security, DATA_TYPE.CONFIG, company));
		}

		#endregion

		#region Methods

		/// <summary>
		/// Handles the SelectedIndexChanged event of the CompanyRoleDropDownList control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void CompanyRoleDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				this.Session["CompanyRole"] = (COMPANY_ROLE)Convert.ToInt32(this.CompanyRoleDropDownList.SelectedItem.Value);
				this.CompaniesDataGrid.CurrentPageIndex = 0;

				// Set the search string to either null or what is in the find text box.
				if ((this.FindTextBox == null) || (this.FindTextBox.Text.Length < 1))
				{
					this.searchString = null;
				}
				else
				{
					this.searchString = this.FindTextBox.Text.ToUpper();
					this.FindTextBox.Text = this.searchString;
				}

				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the OnClick event of the FindAllBtn control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void FindAllBtn_OnClick(object sender, EventArgs e)
		{
			this.Session.Remove(CompanyFindString);
			this.searchString = null;
			this.FindTextBox.Text = string.Empty;
			this.CompaniesDataGrid.CurrentPageIndex = 0;
			this.UpdateView();
		}

		/// <summary>
		/// Handles the OnClick event of the FindBtn control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void FindBtn_OnClick(object sender, EventArgs e)
		{
			if ((this.FindTextBox == null) || (this.FindTextBox.Text.Length < 1))
			{
				this.searchString = null;
				this.Session.Remove(CompanyFindString);
			}
			else
			{
				this.searchString = this.FindTextBox.Text.ToUpper();
				this.FindTextBox.Text = this.searchString;
				this.Session.Add(CompanyFindString, this.searchString);
			}

			// Update the page with the new contents.
			this.CompaniesDataGrid.CurrentPageIndex = 0;
			this.UpdateView();
		}

		/// <summary>
		/// Raises the <see cref="OnInit" /> event.
		/// </summary>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Handles the SelectedIndexChanged event of the PageSizeDropDown control.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void PageSizeDropDownSelectedIndexChanged(object source, EventArgs e)
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

        /// <summary>
        /// When the user checks or unchecks the Show Hidden checkbox, update the view
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        protected void ShowHiddenCheckBox_OnCheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.FindTextBox == null || string.IsNullOrEmpty(this.FindTextBox.Text))
                {
                    this.searchString = null;
                    this.Session.Remove(CompanyFindString);
                }
                else
                {
                    this.searchString = this.FindTextBox.Text.ToUpper();
                    this.FindTextBox.Text = this.searchString;
                    this.Session.Add(CompanyFindString, this.searchString);
                }

                this.SessionCompanySummaryShowHiddenChecked = this.ShowHiddenCheckBox.Checked;

                // Update the page with the new contents.
                this.CompaniesDataGrid.CurrentPageIndex = 0;
                this.UpdateView();
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (this.Page.IsPostBack == false)
				{
					if (!this.Security.HasRight(RIGHT.MODIFY_COMPANY_DATA))
					{
						this.AddButton.Enabled = false;
						this.AddButton2.Enabled = false;
					}

					if (this.Session["CompanyRole"] == null)
					{
						this.Session["CompanyRole"] = COMPANY_ROLE.MAX_COMPANY_ROLE;
					}

					for (var type = COMPANY_ROLE.MANAGER; type <= COMPANY_ROLE.MAX_COMPANY_ROLE; type++)
					{
						var newRoleItem = new ListItem(CompanyRoleMapClass.RoleID(type), ((int)type).ToString(CultureInfo.InvariantCulture));
						this.CompanyRoleDropDownList.Items.Add(newRoleItem);

						if (this.Session["CompanyRole"] != null && (COMPANY_ROLE)this.Session["CompanyRole"] == type)
						{
							this.CompanyRoleDropDownList.SelectedIndex = this.CompanyRoleDropDownList.Items.Count - 1;
						}
					}

					this.Session["CompanyRole"] = (COMPANY_ROLE)Convert.ToInt32(this.CompanyRoleDropDownList.SelectedItem.Value);

				    this.ShowHiddenCheckBox.Checked = this.SessionCompanySummaryShowHiddenChecked;

					if (this.Session["CompaniesPage"] != null)
					{
						this.CompaniesDataGrid.CurrentPageIndex = (int)this.Session["CompaniesPage"];
						this.Session.Remove("CompaniesPage");
					}

					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the Command event of the AddButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="CommandEventArgs" /> instance containing the event data.</param>
		private void AddButtonCommand(object sender, CommandEventArgs e)
		{
			this.Session.Remove("CompanyArrayList");
			this.Session.Remove("CompanySelectContextArrayList");

			var siteClass = FMChannelHelper.MakeCall<ISites, SiteClass>(
                                                    sites => sites.Get(
														this.Security, 
														this.Security.SiteGuid, 
														getMemberSites: false,
														getSchedulesAndProcessVariables: false,
														bGetAssociatedAliases: false));

			var company = new CompanyClass(siteClass);

			var companyArrayList = new ArrayList { company };
			this.Session["CompanyArrayList"] = companyArrayList;

			this.Session["CompaniesPage"] = this.CompaniesDataGrid.CurrentPageIndex;
			this.Redirect("CompanyForm.aspx");
		}

		/// <summary>
		/// Handles the DeleteCommand event of the CompaniesDataGrid control.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="e">The <see cref="DataGridCommandEventArgs" /> instance containing the event data.</param>
		private void CompaniesDataGridDeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				this.GetSecurity();

				// Get Guid
				TableCell guidCell = e.Item.Cells[2];//bds
				FMChannelHelper.MakeCall<ICompanies>(x => x.Purge(this.Security, Guid.Parse(guidCell.Text)));

				this.CompaniesDataGrid.SelectedIndex = -1;
				this.Session.Remove("IdentityGuid");
				if (this.CompaniesDataGrid.Items.Count == 1 && this.CompaniesDataGrid.CurrentPageIndex > 0)
				{
					this.CompaniesDataGrid.CurrentPageIndex--;
				}

				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the EditCommand event of the CompaniesDataGrid control.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="e">The <see cref="DataGridCommandEventArgs" /> instance containing the event data.</param>
		private void CompaniesDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				this.Session.Remove("CompanyArrayList");
				this.Session.Remove("CompanySelectContextArrayList");

				TableCell guidCell = e.Item.Cells[2];//bds

				// Get Company
				CompanyClass company =
					FMChannelHelper.MakeCall<ICompanies, CompanyClass>(x => x.Get(this.Security, Guid.Parse(guidCell.Text)));

				var companyArrayList = new ArrayList { company };
				this.Session["CompanyArrayList"] = companyArrayList;

				this.Session["CompaniesPage"] = this.CompaniesDataGrid.CurrentPageIndex;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}

			// moved outside of the catch expression since response.redirect calls response.end which will raise a thread exception error by design
			this.Redirect("CompanyForm.aspx");
		}

		/// <summary>
		/// Handles the ItemDataBound event of the CompaniesDataGrid control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="DataGridItemEventArgs" /> instance containing the event data.</param>
		private void CompaniesDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			var deleteButton = (LinkButton)e.Item.FindControl("DeleteButton");

			if (deleteButton != null)
			{
				TableCell siteGuidCell = e.Item.Cells[1];//bds

				deleteButton.Enabled = this.Security.HasRight(RIGHT.MODIFY_COMPANY_DATA)
				                       && (this.Security.SiteGuid == Guid.Parse(siteGuidCell.Text));
                //Child record versions cannot be created or deleted directly. Their lifetime is controlled by the Entity-To-Site assignment only.
                if (deleteButton.Enabled)
                {
                    int Index = CompaniesDataGrid.CurrentPageIndex * CompaniesDataGrid.PageSize + e.Item.ItemIndex;
                    DataView dv = (DataView)CompaniesDataGrid.DataSource;
                    if (!dv.Table.Rows[Index]["IdentityGuid"].Equals(dv.Table.Rows[Index]["_MasterRecordGuid"]))
                        deleteButton.Enabled = false;
                }			 
			}

            // Change the color of the text of hidden companies to give the user a visual indication that the company is hidden.
		    if (e.Item.DataItem is DataRowView)
		    {
		        DataRowView view = e.Item.DataItem as DataRowView;
		        DateTimeOffset? hiddenDate = view.Row["HiddenDate"] as DateTimeOffset?;
		        if (hiddenDate.HasValue)
		        {
		            e.Item.ForeColor = Color.Red;
		        }
		    }
		}

		/// <summary>
		/// Handles the PageIndexChanged event of the CompaniesDataGrid control.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="e">The <see cref="DataGridPageChangedEventArgs" /> instance containing the event data.</param>
		private void CompaniesDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			try
			{
				// if we are editing do not allow a page change
				if (this.CompaniesDataGrid.EditItemIndex > -1)
				{
					return;
				}

				this.CompaniesDataGrid.CurrentPageIndex = e.NewPageIndex;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the SortCommand event of the CompaniesDataGrid control.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="e">The <see cref="DataGridSortCommandEventArgs" /> instance containing the event data.</param>
		private void CompaniesDataGridSortCommand(object source, DataGridSortCommandEventArgs e)
		{
			try
			{
				var sortExpression = this.Session[SortExpression] as string;
				var sortDirection = this.Session[SortDirection] as string;

				if (e.SortExpression != sortExpression)
				{
					this.Session[SortExpression] = e.SortExpression;
					this.Session[SortDirection] = "ASC";
				}
				else
				{
					if (sortDirection == "DESC")
					{
						this.Session[SortDirection] = "ASC";
					}
					else
					{
						this.Session[SortDirection] = "DESC";
					}
				}

				this.CompaniesDataGrid.CurrentPageIndex = 0;
				this.Session.Remove("CompaniesPage");
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///     Required method for Designer support - do not modify
		///     the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AddButton2.Command += this.AddButtonCommand;
			this.CompaniesDataGrid.EditCommand += this.CompaniesDataGridEditCommand;
			this.CompaniesDataGrid.PageIndexChanged += this.CompaniesDataGridPageIndexChanged;
			this.CompaniesDataGrid.DeleteCommand += this.CompaniesDataGridDeleteCommand;
			this.CompaniesDataGrid.ItemDataBound += this.CompaniesDataGridItemDataBound;
			this.AddButton.Command += this.AddButtonCommand;
			this.CompaniesDataGrid.SortCommand += this.CompaniesDataGridSortCommand;

			var limits = new EnumerationLimits();
			int pageLimit = limits.GetLimit(EnumerationLimits.EnumerationOptions.COMPANY);
			this.CompanySummaryPageSizeDropDown.SetLimit(pageLimit);
			this.CompaniesDataGrid.PageSize = pageLimit;
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		private void UpdateView()
		{
			var role = (COMPANY_ROLE)Convert.ToInt32(this.CompanyRoleDropDownList.SelectedValue);

			var limits = new EnumerationLimits();
			int limit = limits.GetLimit(EnumerationLimits.EnumerationOptions.COMPANY);

			// Locate the previous search string from the session. Set the set
			// string if found.
			if (this.Session[CompanyFindString] != null)
			{
				this.FindTextBox.Text = this.Session[CompanyFindString] as string;
				this.searchString = this.Session[CompanyFindString] as string;
			}

			// Determine if the user entered in a filter to narrow the company list. If so,
			// then call the method in companies that will use the filter. Otherwise, use the
			// original method to get companies.
			if (string.IsNullOrEmpty(this.searchString))
			{
				this.ds =
					FMChannelHelper.MakeCall<ICompanies, DataSet>(x => x.EnumerateByRoleCompanyGrid(this.Security, role, false, hideHiddenCompanies: !this.ShowHiddenCheckBox.Checked));
			}
			else
			{
				this.ds =
					FMChannelHelper.MakeCall<ICompanies, DataSet>(
                        x => x.EnumerateByRoleAndFilterCompanyGrid(this.Security, role, this.searchString, false, hideHiddenCompanies: !this.ShowHiddenCheckBox.Checked));
			}

			this.dv = new DataView(this.ds.Tables[0]);
			if (this.Session[SortExpression] != null && this.Session[SortDirection] != null)
			{
				this.dv.Sort = string.Format("{0} {1}", this.Session[SortExpression], this.Session[SortDirection]);
			}

			if (this.dv.Count >= limit && limit > 0)
			{
				this.lblWarning.Text = string.Format("Results limited to first {0} records.  Use filters to narrow search.", limit);
				this.lblWarning.Visible = true;
			}
			else
			{
				this.lblWarning.Visible = false;
			}

			if (this.ds.Tables.Count > 0)
			{
				this.CompanySummaryPageSizeDropDown.SetPageSize(this.CompaniesDataGrid, this.ds.Tables[0].Rows.Count);
				this.CompaniesDataGrid.DataSource = this.dv;
				this.CompaniesDataGrid.DataBind();
			}
		}

		#endregion

		// *************************************************************************************************
		// This method is called when the find button is pressed. It will retrieve data from the find
		// text box and set the search string. If there is no data, then the search string is set to null.
		// *************************************************************************************************
	}
}