// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FuelCardLimitSummaryForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// Displays a list of Fuel Card Limits assigned to or owned by the current site
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FuelCardWebApp
{
    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Interfaces;

    using FMControls;

    using FuelsManager.FMWebApp;

    using global::FMWebApp;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web.UI.WebControls;

    /// <summary>
    /// Displays a list of Fuel Card Limits assigned to or owned by the current site
    /// </summary>
    public partial class FuelCardLimitSummaryForm : FMFormBase, IMenuDiscovery, IEntityDiscovery
    {
        #region Form Properties
        /// <summary>
        /// Stores the text the user searched on when the Find button is pressed
        /// </summary>
        private string SessionFindTextBoxSearchString
        {
            get
            {
	            if (this.Session["FuelCardLimitsFindSearchString"] is string)
                {
                    return this.Session["FuelCardLimitsFindSearchString"] as string;
                }
	            
				return string.Empty;
            }
	        set
            {
                this.Session.Add("FuelCardLimitsFindSearchString", value);
            }
        }
        #endregion

        /// <summary>
        /// When the page loads, get the security information and display the fuel card limits currently defined.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this.GetSecurity();

                if (!this.IsPostBack)
                {
                    this.SessionFindTextBoxSearchString = string.Empty;
                    this.UpdateView();

                    this.EnableControls(true);
                }
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

        #region Form Grid Events
        /// <summary>
        /// When the user presses the delete button, delete the selected fuel card limit.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">Contains the row index the user pressed delete for.</param>
        protected void FuelCardLimitsGrid_OnRowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName.Equals("Delete", StringComparison.OrdinalIgnoreCase))
                {
                    int rowIndex = Convert.ToInt32(e.CommandArgument);

                    // Get the primary key of the record the user pressed delete from the DataKeys collection
                    DataKey dataKey = this.FuelCardLimitsGrid.DataKeys[rowIndex];
                    if (dataKey != null && dataKey.Values != null)
                    {
                        Guid fuelCardLimitGuid = (Guid)dataKey.Values["IdentityGuid"];

                        FMChannelHelper.MakeCall<IFuelCardLimits>(
							fuelCardLimitsServiceClass => fuelCardLimitsServiceClass.Purge(this.Security, fuelCardLimitGuid));
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
        /// When the user changes the page, change the page and update the view.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">Identifies the page selected by the user.</param>
        protected void FuelCardLimitsGrid_OnPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                this.EnableControls(true);
                this.FuelCardLimitsGrid.PageIndex = e.NewPageIndex;

                this.UpdateView();
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

        /// <summary>
        /// When the user edits a row, take them to the detail screen for the Fuel Card Limit they selected
        /// </summary>
        /// <param name="sender">The parameter is not used</param>
        /// <param name="e">Identifies the row the user clicked edit for</param>
        protected void FuelCardLimitsGrid_OnRowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                string fuelCardLimitGuid = string.Empty;

                // You can't access the row's data here. We use the grid's datakeys property instead to store the fuel card limit guid.
                DataKey dataKey = this.FuelCardLimitsGrid.DataKeys[e.NewEditIndex];

                if (dataKey != null && dataKey.Values != null)
                {
                    fuelCardLimitGuid = dataKey.Values["IdentityGuid"].ToString();
                }

                this.Redirect("FuelCardLimitDetailForm.aspx?FuelCardLimitGuid=" + fuelCardLimitGuid);
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

        /// <summary>
        /// When a row is bound to the grid, wire up the delete button to pass the row index of the 
        /// row being bound. This is necessary to support grid paging.
        /// </summary>
        /// <param name="sender">The parameter is not used</param>
        /// <param name="e">Identifies the row being bound</param>
        protected void FuelCardLimitsGrid_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    FMDeleteLinkButton deleteButton = e.Row.FindControl("DeleteButton") as FMDeleteLinkButton;

                    if (deleteButton != null)
                    {
                        deleteButton.CommandArgument = e.Row.RowIndex.ToString(CultureInfo.InvariantCulture);

                        // Disable the delete button if the user does not have modify rights 
                        if (!this.Security.HasRight(RIGHT.MODIFY_FUEL_CARD_LIMIT))
                        {
                            deleteButton.Enabled = false;
                        }

                        // Disable the delete button if the fuel card limit is not owned by the current site
                        if (e.Row.DataItem is FuelCardLimit)
                        {
                            FuelCardLimit limit = e.Row.DataItem as FuelCardLimit;

                            if (limit.SiteGuid != this.Security.SiteGuid)
                            {
                                deleteButton.Enabled = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }
        #endregion

        #region Form Control Events
        /// <summary>
        /// When the add button is clicked, redirect the user to the Fuel Card Limit Detail Screen so they can 
        /// add a new Fuel Card Limit
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        protected void AddButtonClick(object sender, EventArgs e)
        {
            try
            {
                this.Redirect("FuelCardLimitDetailForm.aspx");
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

        /// <summary>
        /// Update the view when the user changes the grid page size
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        protected void PageSizeDropDown_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.EnableControls(true);
                this.UpdateView();
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

        /// <summary>
        /// When the user clicks the find button, limit the results to those
        /// that contain the value the user typed into the find box
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        protected void FindButton_OnClick(object sender, EventArgs e)
        {
            try
            {
                this.SessionFindTextBoxSearchString = this.FindTextBox.Text;

                this.FuelCardLimitsGrid.PageIndex = 0;

                this.EnableControls(true);
                this.UpdateView();
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

        /// <summary>
        /// When the user clicks the Show All button, display all fuel card limits 
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        protected void ShowAllButton_OnClick(object sender, EventArgs e)
        {
            try
            {
                this.FindTextBox.Text = string.Empty;
                this.SessionFindTextBoxSearchString = string.Empty;

                this.FuelCardLimitsGrid.PageIndex = 0;

                this.EnableControls(true);
                this.UpdateView();
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }
        #endregion

        #region Form Methods
        /// <summary>
        /// Enable or disable controls on the screen.
        /// </summary>
        /// <param name="enable">True to enable, false to disable.</param>
        private void EnableControls(bool enable)
        {
            // Never let the add button be enabled if the user doesn't have modify rights
            bool enableControls = enable && this.Security.HasRight(RIGHT.MODIFY_FUEL_CARD_LIMIT);

            this.AddButton.Enabled = enableControls;
            this.AddButtonTop.Enabled = enableControls;
        }

        /// <summary>
        /// Retrieve fuel card limits from the database and display them on the grid
        /// </summary>
        private void UpdateView()
        {
            List<FuelCardLimit> fuelCardLimits;

            if (string.IsNullOrWhiteSpace(this.SessionFindTextBoxSearchString))
            {
                fuelCardLimits = FMChannelHelper.MakeCall<IFuelCardLimits, List<FuelCardLimit>>(
                    fuelCardLimitsServiceClass => fuelCardLimitsServiceClass.Enumerate(this.Security));
            }
            else
            {
                // If the user provided a value in the find box, use that value to filter the result set
                fuelCardLimits = FMChannelHelper.MakeCall<IFuelCardLimits, List<FuelCardLimit>>(
                    fuelCardLimitsServiceClass => fuelCardLimitsServiceClass.EnumerateAndFilter(this.Security, this.SessionFindTextBoxSearchString));
            }

            this.PageSizeDropDown.SetPageSize(this.FuelCardLimitsGrid, fuelCardLimits.Count);
            this.BindData(fuelCardLimits);
        }

        /// <summary>
        /// Bind the fuel card limits provided to the grid
        /// </summary>
        /// <param name="fuelCardLimits">The fuel card limits to bind to the grid</param>
        private void BindData(List<FuelCardLimit> fuelCardLimits)
        {
            this.FuelCardLimitsGrid.DataSource = fuelCardLimits;
            this.FuelCardLimitsGrid.DataBind();
        }
        #endregion

        #region FuelsManager Menu Support
        /// <summary>
        /// Gets a list of menu items that should be displayed for the current user.
        /// </summary>
        /// <param name="security">The security object of the current session</param>
        /// <param name="siteGroup">Whether the current logged-in site is a site group</param>
        /// <param name="options">Hardware key options</param>
        /// <returns>
        /// List of menu items to be displayed
        /// </returns>
        public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1,ushort word2, ushort useNewLicenseKey, uint options)
        {
            if (!security.HasRight(RIGHT.VIEW_FUEL_CARD_LIMIT) && !security.HasRight(RIGHT.MODIFY_FUEL_CARD_LIMIT))
            {
                return null;
            }

            List<FMMenuItem> items = new List<FMMenuItem>
                                         {
                                             new FMMenuItem
                                                 {
                                                     MenuItemType = FMMenuItemType.CONFIG_OTHER_FUEL_CARD_LIMITS,
                                                     RootMenuName = "Configuration",
                                                     CategoryName = "Other",
                                                     ItemName = "Fuel Card Limits",
                                                     NavigateUrl = "..\\FuelCardWebApp\\FuelCardLimitSummaryForm.aspx",
                                                     ApplyDataDictionary = ApplyDataDictionary.Apply
                                                 }
                                         };

            return items;
        }
        #endregion

        #region Entity Assignment and Ownership Support
        /// <summary>
        /// Can you assign Fuel Card Limits to sites other than the site which owns the limt? Yes you can.
        /// </summary>
        bool IEntityDiscovery.EntityAssignable
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Get the service class which supports entity assignment for Fuel Card Limits.
        /// This doesn't appear to be used in any meaningful way but must be implemented to satisfy IEntityDiscovery
        /// </summary>
        Type IEntityDiscovery.EntityEngineType
        {
            get
            {
                return typeof(IFuelCardLimits);
            }
        }

        /// <summary>
        /// The type of entity we are supporting entity assignment for (Fuel Card Limits)
        /// </summary>
        ENTITY_TYPE IEntityDiscovery.EntityType
        {
            get
            {
                return ENTITY_TYPE.FUEL_CARD_LIMIT;
            }
        }

        /// <summary>
        /// Enumerate entity maps for fuel card limits. This is used by the entity ownership form to show fuel card limits
        /// owned by the site.
        /// </summary>
        /// <param name="security">Contains security information.</param>
        /// <param name="type">The type of entity assignment to enumerate. For fuel card limits this appears to only be OWNED, which is 
        /// used by the entity ownership form</param>
        /// <returns>Entity to site mappings for fuel card limits depending on the type provided.</returns>
        EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(SecurityClass security, ENTITY_ASSIGNMENT_TYPE type)
        {
            List<FuelCardLimit> fuelCardLimits = FMChannelHelper.MakeCall<IFuelCardLimits, List<FuelCardLimit>>(
                                                                     fuelCardLimitsServiceClass =>
                                                                     fuelCardLimitsServiceClass.Enumerate(security)
                                                                );

            EntityToSiteMapCollectionClass entityToSiteMapCollection = new EntityToSiteMapCollectionClass();

            if (!security.HasRight(RIGHT.MODIFY_FUEL_CARD_LIMIT))
            {
                return entityToSiteMapCollection;
            }

            foreach (FuelCardLimit fuelCardLimit in fuelCardLimits)
            {
                if (type == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
                {
                    if (security.SiteGuid == fuelCardLimit.SiteGuid)
                    {
                        continue;
                    }

                    if (security.LoginSiteGuid != fuelCardLimit.SiteGuid)
                    {
                        continue;
                    }
                }
                else
                {
                    if (security.SiteGuid != fuelCardLimit.SiteGuid)
                    {
                        continue;
                    }
                }

                EntityToSiteMapClass entityToSiteMap = new EntityToSiteMapClass(fuelCardLimit);
                entityToSiteMapCollection.Add(entityToSiteMap);
            }

            return entityToSiteMapCollection;
        }

        /// <summary>
        /// Get the primary key (aka Identity Guid) of the Fuel Card Limit matching the provided ID
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="id">Identifies the Fuel Card Limit to retrieve</param>
        /// <returns>The primary key (aka Identity Guid) of the Fuel Card Limit matching the provided ID</returns>
        Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string id)
        {
            return FMChannelHelper.MakeCall<IFuelCardLimits, Guid>(
							fuelCardLimitsServiceClass => fuelCardLimitsServiceClass.GetIdentityGuid(security, id));
        }

        /// <summary>
        /// Modify the provided fuel card limit's siteGuid. This is used for entity ownership changes.
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="guid">Identifies the Fuel Card Limit we want to modify</param>
        /// <param name="siteGuid">Identifies the site the fuel card limit should be owned by</param>
        void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid siteGuid)
        {
            FuelCardLimit fuelCardLimit = FMChannelHelper.MakeCall<IFuelCardLimits, FuelCardLimit>(
												fuelCardLimitsServiceClass => fuelCardLimitsServiceClass.Get(security, guid));

            fuelCardLimit.SiteGuid = siteGuid;
            FMChannelHelper.MakeCall<IFuelCardLimits>(
							fuelCardLimitsServiceClass => fuelCardLimitsServiceClass.Modify(security, fuelCardLimit));
        }
        #endregion
    }
}