// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IATACodesForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Code behind for IATACodesForm.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;
	using FMBusinessObjects.UtilityObjects;

	/// <summary>
	/// Code behind for IATACodesForm.
	/// </summary>
	public partial class IATACodesForm : FMAutoSubmitFormBase, IEntityDiscovery, IMenuDiscovery
	{
		#region Constants and Fields

		/// <summary>
		/// Session key
		/// </summary>
		private const string IATACodeFindString = "IATACodeFindString";

		/// <summary>
		/// Session key
		/// </summary>
		private const string SortDirection = "IATACodesForm.SortDirection";

		/// <summary>
		/// Session key
		/// </summary>
		private const string SortExpression = "IATACodesForm.SortExpression";

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

		#endregion

		#region Explicit Interface Properties

		/// <summary>
		/// Gets a value indicating whether [entity assignable].
		/// </summary>
		/// <value>
		///   <c>true</c> if [entity assignable]; otherwise, <c>false</c>.
		/// </value>
		bool IEntityDiscovery.EntityAssignable => true;

	    /// <summary>
		/// Gets the type of the entity engine.
		/// </summary>
		/// <value>
		/// The type of the entity engine.
		/// </value>
		Type IEntityDiscovery.EntityEngineType => typeof(IIATACodes);

	    /// <summary>
		/// Gets the type of the entity.
		/// </summary>
		/// <value>
		/// The type of the entity.
		/// </value>
		ENTITY_TYPE IEntityDiscovery.EntityType => ENTITY_TYPE.IATA_CODE;

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
	    /// <param name="useNewLicenseKey"></param>
	    /// <param name="options">
	    /// Hardware key options
	    /// </param>
	    /// <param name="word1"></param>
	    /// <param name="word2"></param>
	    /// <returns>
	    /// List of menu items to be displayed
	    /// </returns>
	    public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1, ushort word2, ushort useNewLicenseKey, uint options)
        {
			if (useNewLicenseKey != 1)
            {
                // Depends Upon Shared Components and an ADF key for the ADF
                if ((options & 0x8004000) == 0)
                {
                    return null;
                }
            }

            var menuItems = new List<FMMenuItem>();

            if (!security.HasRight(RIGHT.VIEW_TICKETING_DATA) && !security.HasRight(RIGHT.MODIFY_TICKETING_DATA))
            {
                return null;
            }

            var menuItem = new FMMenuItem
            {
                MenuItemType = FMMenuItemType.CONFIG_SITES_IATA_CODES,
                RootMenuName = "Configuration",
                CategoryName = "Sites",
                ItemName = "Delivery Locations",
                NavigateUrl = "IATACodesForm.aspx",
                ApplyDataDictionary = ApplyDataDictionary.Apply
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
            IATACodeCollectionClass iataCodeCollection = null;
            iataCodeCollection = FMChannelHelper.MakeCall<IIATACodes, IATACodeCollectionClass>(x => x.Enumerate(security));

			var entityToSiteMapCollection = new EntityToSiteMapCollectionClass();

            foreach (IATACodeClass iataCode in iataCodeCollection)
			{
				if (type == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
                    if (security.SiteGuid == iataCode.SiteGuid)
					{
						continue;
					}

                    if (security.LoginSiteGuid != iataCode.SiteGuid)
					{
						continue;
					}
				}
				else
				{
                    //For entity types supporting Record Versioning, assignments can be cascaded, irrespective of whether Record Versioning is turned on or off.
                    if ((security.SiteGuid != iataCode.SiteGuid))
					{
						continue;
					}
				}

			    var entityToSiteMap = new EntityToSiteMapClass(iataCode) { IdentityGuid = iataCode.IdentityGuid };
			    entityToSiteMapCollection.Add(entityToSiteMap);
			}

            return entityToSiteMapCollection;
            //throw new NotImplementedException("IEntityDiscovery.EnumerateEntityMaps not imlpemented for IATACodes");
		}

		/// <summary>
		/// Gets the identity GUID.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="id">The id.</param>
		/// <returns>The identity guid.</returns>
		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string id)
		{
			return FMChannelHelper.MakeCall<IIATACodes, Guid>(x => x.GetIdentityGuid(security, id));
		}

		/// <summary>
		/// Sets the site GUID.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="guid">The GUID.</param>
		/// <param name="siteGuid">The site GUID.</param>
		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid siteGuid)
		{
			var iataCode = FMChannelHelper.MakeCall<IIATACodes, IATACodeClass>(x => x.Get(security, guid));

            iataCode.SiteGuid = siteGuid;
            FMChannelHelper.MakeCall<IIATACodes>(x => x.Modify(security, iataCode));
		}

		#endregion

		#region Methods

		/// <summary>
		/// Handles the OnClick event of the FindAllBtn control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void FindAllBtn_OnClick(object sender, EventArgs e)
		{
            this.Session.Remove(IATACodeFindString);
			this.searchString = null;
			this.FindTextBox.Text = string.Empty;
            this.IATACodesDataGrid.CurrentPageIndex = 0;
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
                this.Session.Remove(IATACodeFindString);
			}
			else
			{
				this.searchString = this.FindTextBox.Text.ToUpper();
				this.FindTextBox.Text = this.searchString;
                this.Session.Add(IATACodeFindString, this.searchString);
			}

			// Update the page with the new contents.
            this.IATACodesDataGrid.CurrentPageIndex = 0;
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
					if (!this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
					{
						this.AddButton.Enabled = false;
						this.AddButton2.Enabled = false;
					}

					if (this.Session["IATACodesPage"] != null)
					{
                        this.IATACodesDataGrid.CurrentPageIndex = (int)this.Session["IATACodesPage"];
                        this.Session.Remove("IATACodesPage");
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
			this.Session.Remove("IATACodeArrayList");
            this.Session.Remove("IATACodeSelectContextArrayList");

            var iataCode = new IATACodeClass();

            var iataCodeArrayList = new ArrayList { iataCode };
			this.Session["IATACodeArrayList"] = iataCodeArrayList;

            this.Session["IATACodesPage"] = this.IATACodesDataGrid.CurrentPageIndex;
            this.Redirect("IATACodeMainForm.aspx");
		}

		/// <summary>
        /// Handles the DeleteCommand event of the IATACodesDataGrid control.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="e">The <see cref="DataGridCommandEventArgs" /> instance containing the event data.</param>
        private void IataCodesDataGridDeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				this.GetSecurity();

				// Get Guid
				TableCell guidCell = e.Item.Cells[2];//bds
			    Guid guid = Guid.Parse(guidCell.Text);
				FMChannelHelper.MakeCall<IIATACodes>(x => x.Purge(this.Security, guid));

                this.IATACodesDataGrid.SelectedIndex = -1;
				this.Session.Remove("IdentityGuid");
                if (this.IATACodesDataGrid.Items.Count == 1 && this.IATACodesDataGrid.CurrentPageIndex > 0)
				{
                    this.IATACodesDataGrid.CurrentPageIndex--;
				}

				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
        /// Handles the EditCommand event of the IATACodesDataGrid control.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="e">The <see cref="DataGridCommandEventArgs" /> instance containing the event data.</param>
        private void IataCodesDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				this.Session.Remove("IATACodeArrayList");
                this.Session.Remove("IATACodeSelectContextArrayList");

				TableCell guidCell = e.Item.Cells[2];//bds

                // Get IATACode
                IATACodeClass iataCode =
                    FMChannelHelper.MakeCall<IIATACodes, IATACodeClass>(x => x.Get(this.Security, Guid.Parse(guidCell.Text)));

                var iataCodeArrayList = new ArrayList { iataCode };
                this.Session["IATACodeArrayList"] = iataCodeArrayList;

				this.Session["IATACodesPage"] = this.IATACodesDataGrid.CurrentPageIndex;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}

			// moved outside of the catch expression since response.redirect calls response.end which will raise a thread exception error by design
			this.Redirect("IATACodeMainForm.aspx");
		}

		/// <summary>
        /// Handles the ItemDataBound event of the IATACodesDataGrid control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="DataGridItemEventArgs" /> instance containing the event data.</param>
        private void IataCodesDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			var deleteButton = (LinkButton)e.Item.FindControl("DeleteButton");

			if (deleteButton != null)
			{
				TableCell siteGuidCell = e.Item.Cells[1];//bds

				deleteButton.Enabled = this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
				                       && (this.Security.SiteGuid == Guid.Parse(siteGuidCell.Text));
			}
		}

		/// <summary>
        /// Handles the PageIndexChanged event of the IATACodesDataGrid control.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="e">The <see cref="DataGridPageChangedEventArgs" /> instance containing the event data.</param>
        private void IataCodesDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			try
			{
				// if we are editing do not allow a page change
                if (this.IATACodesDataGrid.EditItemIndex > -1)
				{
					return;
				}

                this.IATACodesDataGrid.CurrentPageIndex = e.NewPageIndex;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
        /// Handles the SortCommand event of the IATACodesDataGrid control.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="e">The <see cref="DataGridSortCommandEventArgs" /> instance containing the event data.</param>
        private void IataCodesDataGridSortCommand(object source, DataGridSortCommandEventArgs e)
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

                this.IATACodesDataGrid.CurrentPageIndex = 0;
				this.Session.Remove("IATACodesPage");
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
			this.AddButton2.Command                 += this.AddButtonCommand;
            this.IATACodesDataGrid.EditCommand      += this.IataCodesDataGridEditCommand;
            this.IATACodesDataGrid.PageIndexChanged += this.IataCodesDataGridPageIndexChanged;
            this.IATACodesDataGrid.DeleteCommand    += this.IataCodesDataGridDeleteCommand;
            this.IATACodesDataGrid.ItemDataBound    += this.IataCodesDataGridItemDataBound;
			this.AddButton.Command                  += this.AddButtonCommand;
            this.IATACodesDataGrid.SortCommand      += this.IataCodesDataGridSortCommand;

			var limits = new EnumerationLimits();
            int pageLimit = limits.GetLimit(EnumerationLimits.EnumerationOptions.DEFAULT);
			this.IATACodeSummaryPageSizeDropDown.SetLimit(pageLimit);
            this.IATACodesDataGrid.PageSize = pageLimit;
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		private void UpdateView()
		{
			var limits = new EnumerationLimits();
			int limit = limits.GetLimit(EnumerationLimits.EnumerationOptions.DEFAULT);

			// Locate the previous search string from the session. Set the set
			// string if found.
            if (this.Session[IATACodeFindString] != null)
			{
                this.FindTextBox.Text = this.Session[IATACodeFindString] as string;
                this.searchString = this.Session[IATACodeFindString] as string;
			}

		    //var tmp1 = "test";
            //var tmp2 = "te" + "st";
            //if (tmp1 == tmp2) throw new NotImplementedException("Search not implemented");
            // Determine if the user entered in a filter to narrow the IATACode list. If so,
            // then call the method in IATACodes that will use the filter. Otherwise, use the
            // original method to get IATACodes.
            this.ds = FMChannelHelper.MakeCall<IIATACodes, DataSet>(x => x.EnumerateWithFilter(this.Security, this.searchString));

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
                this.IATACodeSummaryPageSizeDropDown.SetPageSize(this.IATACodesDataGrid, this.ds.Tables[0].Rows.Count);
                this.IATACodesDataGrid.DataSource = this.dv;
                this.IATACodesDataGrid.DataBind();
			}
		}

		#endregion
	}
}