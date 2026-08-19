// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FuelCardLimitGeneralPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// The General tab for the Fuel Card Limit Detail Form. Displays information about the Fuel Card Limit like the ID and the line items.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FuelCardWebApp
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FMControls;

	using FuelsManager.FMWebApp;

	/// <summary>
    /// The General tab for the Fuel Card Limit Detail Form. Displays information about the Fuel Card Limit like the ID and the line items.
    /// </summary>
    public partial class FuelCardLimitGeneralPage : FuelCardLimitPageBase
    {
        /// <summary>
        /// Update the fuel card limit object with any data the user has specified on the page
        /// </summary>
        public void UpdateData()
        {
            if (string.IsNullOrWhiteSpace(this.IDTextBox.Text))
            {
                throw new Exception("ID must be provided");
            }

            this.FuelCardLimit.ID = this.IDTextBox.Text;
        }

        /// <summary>
        /// When the page loads, update the view to display the details of the Fuel Card Limit
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!this.IsPostBack)
                {
                    this.UpdateView();

                    this.EnableControls(true);
                }
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

        #region Grid Events

        /// <summary>
        /// When the user clicks the cancel edit button, either remove the line item from the list if it's a new one,
        /// or cancel the edits on the current line item.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">Identifies the row the edit was cancelled for.</param>
        protected void LineItemsGridRowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                this.EnableControls(true);
                this.LineItemsGrid.EditIndex = -1;

                List<FuelCardLimitLineItem> lineItems = this.FuelCardLimit.LineItems;

                int position = this.GetLineItemIndexFromRowIndex(e.RowIndex);
                FuelCardLimitLineItem lineItem = lineItems[position];

                // Get the identity guid of the object associated with the row
                DataKey dataKey = this.LineItemsGrid.DataKeys[e.RowIndex];

                if (dataKey != null 
					&& dataKey.Value is Guid 
					&& (Guid)dataKey.Value == Guid.Empty 
					&& lineItem != null && lineItem.Limit.Value == 0)
                {
                    // If the line item is a new one, cancel should remove it from the list rather than just cancelling the edit
                    // Keep in mind that new line items are added to the end of the list
                    lineItems.RemoveAt(lineItems.Count - 1);
                }

                this.BindData(this.FuelCardLimit.LineItems);
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

        /// <summary>
        /// When the user edits a row, disable the add button and edit the row.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">Identifies the row being edited.</param>
        protected void LineItemsGridRowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                this.EnableControls(false);
                this.LineItemsGrid.EditIndex = e.NewEditIndex;
                this.BindData(this.FuelCardLimit.LineItems);
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

        /// <summary>
        /// When a row is bound to the grid wire up the delete button and select the appropriate values in the drop downs.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">Identifies the row being bound</param>
        protected void LineItemsGridRowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {                    
                    FMDeleteLinkButton deleteButton = e.Row.FindControl("DeleteButton") as FMDeleteLinkButton;

                    bool disableControls = !this.Security.HasRight(RIGHT.MODIFY_FUEL_CARD_LIMIT)
                                           || (this.FuelCardLimit.SiteGuid != Guid.Empty
                                               && this.FuelCardLimit.SiteGuid != this.Security.SiteGuid);

                    if (deleteButton != null)
                    {
                        // Set the delete button's command argument to the row index so that delete will work properly with paging
                        deleteButton.CommandArgument = e.Row.RowIndex.ToString(CultureInfo.InvariantCulture);

                        // Disable the delete button in the grid if the user doesn't have modify rights or if the limit is not owned by the current site
                        if (disableControls)
                        {
                            deleteButton.Enabled = false;                   
                        }
                    }

                    // Disable the edit button in the grid if the user doesn't have modify rights or if the limit is not owned by the current site
                    if (e.Row.Cells.Count >= 1 && disableControls)
                    {
                        e.Row.Cells[0].Enabled = false;//bds
                    }

                    if (e.Row.DataItem is FuelCardLimitLineItem)
                    {
                        FuelCardLimitLineItem lineItem = e.Row.DataItem as FuelCardLimitLineItem;

                        // Set the type drop down to the type of line item
                        FMDropDownList lineItemTypeDropDownList = e.Row.FindControl("TypeDropDownList") as FMDropDownList;

                        if (lineItemTypeDropDownList != null)
                        {
                            lineItemTypeDropDownList.SelectedValue = lineItem.LineItemType.ToString();
                        }

                        // Hide or show the product and product group drop downs depending on the type of line item
                        FMDropDownList productDropDownList = e.Row.FindControl("ProductDropDownList") as FMDropDownList;
                        FMDropDownList productGroupDropDownList = e.Row.FindControl("ProductGroupDropDownList") as FMDropDownList;

                        if (productDropDownList != null && productGroupDropDownList != null)
                        {
                            switch (lineItem.LineItemType)
                            {
                                case FuelCardLimitLineItemType.Product:
                                    productDropDownList.Visible = true;
                                    productDropDownList.SelectedValue = lineItem.ProductGuid.ToString();
                                    productGroupDropDownList.Visible = false;
                                    break;
                                case FuelCardLimitLineItemType.ProductGroup:
                                    productDropDownList.Visible = false;
                                    productGroupDropDownList.SelectedValue = lineItem.ProductGroupApplicationStringGuid.ToString();
                                    productGroupDropDownList.Visible = true;
                                    break;
                                default:
                                    productDropDownList.Visible = false;
                                    productGroupDropDownList.Visible = false;
                                    break;
                            }
                        }

                        // Set the period drop down depending to the period specified for the line item
                        FMDropDownList periodDropDownList = e.Row.FindControl("PeriodDropDownList") as FMDropDownList;

                        if (periodDropDownList != null)
                        {
                            periodDropDownList.SelectedValue = lineItem.Period.ToString();
                        }

                        // Apply the data dictionary to the labels representing drop down values in the grid rows.
                        // This is necessary because FMLabel doesn't do translation properly when in a grid.
                        if (FMFormBase.GetDataDictionaryFlag())
                        {
                            FMLabel periodLabel = e.Row.FindControl("PeriodLabel") as FMLabel;

                            if (periodLabel != null)
                            {
                                periodLabel.Text = GetDataDictionaryValueByKey(this.Security.SiteGuid, lineItem.Period.ToString());
                            }

                            FMLabel lineItemTypeLabel = e.Row.FindControl("TypeLabel") as FMLabel;

                            if (lineItemTypeLabel != null)
                            {
                                lineItemTypeLabel.Text = GetDataDictionaryValueByKey(this.Security.SiteGuid, lineItem.UserFriendlyLineItemType);
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

        /// <summary>
        /// Fires when the user presses the delete button and deletes a line item from the grid.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">Identifies the row the user pressed delete on</param>
        protected void LineItemsGridRowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName.Equals("Delete", StringComparison.OrdinalIgnoreCase))
                {
                    int rowIndex = Convert.ToInt32(e.CommandArgument);

                    List<FuelCardLimitLineItem> lineItems = this.FuelCardLimit.LineItems;

                    int position = this.GetLineItemIndexFromRowIndex(rowIndex);
                    lineItems.RemoveAt(position);

                    this.EnableControls(true);
                    this.LineItemsGrid.EditIndex = -1;

                    this.FuelCardLimit.LineItems = lineItems;
                    this.BindData(this.FuelCardLimit.LineItems);
                }
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

        /// <summary>
        /// When the user saves edits on a row, use the information provided to update a line item record.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">Identifies the row being saved</param>
        protected void LineItemsGridRowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                // Get the identity guid of the record contained in the row updated by the user
                Guid identityGuidUpdated = Guid.Empty;

                if (this.LineItemsGrid.DataKeys[e.RowIndex] != null && this.LineItemsGrid.DataKeys[e.RowIndex].Value is Guid)
                {
                    identityGuidUpdated = (Guid)this.LineItemsGrid.DataKeys[e.RowIndex].Value;
                }

                List<FuelCardLimitLineItem> lineItems = this.FuelCardLimit.LineItems;

                FuelCardLimitLineItem lineItem;

                // Get the object we have associated with the row
                if (identityGuidUpdated != Guid.Empty)
                {
                    lineItem = lineItems.FirstOrDefault(matchingLineItem => matchingLineItem.IdentityGuid == identityGuidUpdated);
                }
                else
                {
                    int position = this.GetLineItemIndexFromRowIndex(e.RowIndex);
                    lineItem = lineItems[position];
                }

                if (lineItem != null)
                {
                    // Get the row
                    GridViewRow row = this.LineItemsGrid.Rows[e.RowIndex];

                    // Save the data the user entered
                    FuelCardLimitLineItemType lineItemType = FuelCardLimitLineItemType.AllProducts;
                    FMDropDownList limitTypeDropDownList = row.Cells[1].Controls[1] as FMDropDownList;//bds

                    if (limitTypeDropDownList != null 
						&& limitTypeDropDownList.SelectedItem != null
                        && !string.IsNullOrEmpty(limitTypeDropDownList.SelectedItem.Value))
                    {
                        Enum.TryParse(limitTypeDropDownList.SelectedValue, out lineItemType);
                    }

                    // The entity the user has selected depends on the type of line item.
                    // If the user selected the Product type, we want the product drop down's value.
                    // If the user selected the Product Group type, we want the product group drop down's value.
                    // Otherwise, for the All products type, we want no value.
                    Guid entityGuid = Guid.Empty;
                    string entityID = string.Empty;

                    FMDropDownList entityDropDown = null;

                    switch (lineItemType)
                    {
                        case FuelCardLimitLineItemType.Product:
                            entityDropDown = row.Cells[2].Controls[1] as FMDropDownList;//bds
                            break;
                        case FuelCardLimitLineItemType.ProductGroup:
                            entityDropDown = row.Cells[2].Controls[3] as FMDropDownList;//bds
                            break;
                    }

                    if (entityDropDown != null
                        && entityDropDown.SelectedItem != null
                        && !string.IsNullOrEmpty(entityDropDown.SelectedItem.Value))
                    {
                        string entityGuidString = entityDropDown.SelectedItem.Value;
                        Guid.TryParse(entityGuidString, out entityGuid);

                        entityID = entityDropDown.SelectedItem.Text;
                    }

                    if ((lineItemType == FuelCardLimitLineItemType.Product || lineItemType == FuelCardLimitLineItemType.ProductGroup) 
						&& entityGuid == Guid.Empty)
                    {
                        throw new ApplicationException("Entity is required when the line item type is Product or Product Group");
                    }
                    
                    if (lineItemType == FuelCardLimitLineItemType.AllProducts && entityGuid != Guid.Empty)
                    {
                        throw new ApplicationException("Entity is not allowed when the line item type is All Products");
                    }

                    if (lineItemType == FuelCardLimitLineItemType.Product)
                    {
                        lineItem.ProductGuid = entityGuid;
                        lineItem.ProductGroupApplicationStringGuid = Guid.Empty;
                    }
                    else if (lineItemType == FuelCardLimitLineItemType.ProductGroup)
                    {
                        lineItem.ProductGroupApplicationStringGuid = entityGuid;
                        lineItem.ProductGuid = Guid.Empty;
                    }
                    else
                    {
                        lineItem.ProductGroupApplicationStringGuid = Guid.Empty;
                        lineItem.ProductGuid = Guid.Empty;
                    }

                    lineItem.AssignedProductGroupOrProductID = entityID;

                    // Validate and set the limit
                    FMTextBox limitTextBox = row.Cells[3].Controls[1] as FMTextBox;//bds
                    string limitString = string.Empty;
                    double limit;

                    if (limitTextBox != null)
                    {
                        limitString = limitTextBox.Text;
                    }

                    if (string.IsNullOrEmpty(limitString))
                    {
                        throw new Exception("Limit must be provided");
                    }

                    if (!double.TryParse(limitString, NumberStyles.Number, lineItem.Limit.Format, out limit))
                    {
                        throw new Exception("Limit must be numeric");
                    }
                    
                    if (limit <= 0)
                    {
                        throw new Exception("Limit must be greater than zero");
                    }

                    lineItem.Limit.Value = limit;

                    // Set the period
                    FuelCardLimitPeriod period = FuelCardLimitPeriod.Day;
                    FMDropDownList periodDropDown = row.Cells[4].Controls[1] as FMDropDownList;//bds

                    if (periodDropDown != null 
						&& periodDropDown.SelectedItem != null 
						&& !string.IsNullOrEmpty(periodDropDown.SelectedItem.Value))
                    {
                        Enum.TryParse(periodDropDown.SelectedValue, out period);
                    }

                    lineItem.Period = period;
                }

                this.EnableControls(true);

                // Reset the edit index
                this.LineItemsGrid.EditIndex = -1;

                this.FuelCardLimit.LineItems = lineItems;

                // Bind data to the grid control
                this.BindData(this.FuelCardLimit.LineItems);
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
        protected void LineItemsGrid_OnPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                this.EnableControls(true);
                this.LineItemsGrid.EditIndex = -1;
                this.LineItemsGrid.PageIndex = e.NewPageIndex;

                this.UpdateView();
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

        #endregion

        /// <summary>
        /// When the user changes the type of line item, change the ID drop down to display the appropriate entities (products or product groups)
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        protected void TypeDropDownList_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Get the grid row we're editing
                int rowIndex = this.LineItemsGrid.EditIndex;

                if (rowIndex < 0 || rowIndex > this.LineItemsGrid.Rows.Count - 1)
                {
                    return;
                }

                GridViewRow row = this.LineItemsGrid.Rows[rowIndex];

                // Find the type the user has selected
                FuelCardLimitLineItemType lineItemType = FuelCardLimitLineItemType.AllProducts;

                FMDropDownList limitTypeDropDownList = sender as FMDropDownList;

                if (limitTypeDropDownList != null && limitTypeDropDownList.SelectedItem != null
                    && !string.IsNullOrEmpty(limitTypeDropDownList.SelectedItem.Value))
                {
                    Enum.TryParse(limitTypeDropDownList.SelectedValue, out lineItemType);
                }

                // Find the product and product group drop downs in the row. We will hide or show these depending 
                // on the type of line item the user has selected
                FMDropDownList productDropDownList = row.FindControl("ProductDropDownList") as FMDropDownList;
                FMDropDownList productGroupDropDownList = row.FindControl("ProductGroupDropDownList") as FMDropDownList;

                if (productDropDownList != null && productGroupDropDownList != null)
                {
                    switch (lineItemType)
                    {
                        case FuelCardLimitLineItemType.Product:
                            productDropDownList.Visible = true;
                            productGroupDropDownList.Visible = false;
                            break;
                        case FuelCardLimitLineItemType.ProductGroup:
                            productDropDownList.Visible = false;
                            productGroupDropDownList.Visible = true;
                            break;
                        default:
                            productDropDownList.Visible = false;
                            productGroupDropDownList.Visible = false;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

        /// <summary>
        /// When the add button is clicked, add a new line item record to the grid
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        protected void AddButtonClick(object sender, EventArgs e)
        {
            try
            {
                List<FuelCardLimitLineItem> lineItems = this.FuelCardLimit.LineItems;

                SiteClass site =
                    FMChannelHelper.MakeCall<ISites, SiteClass>(
                        sites => sites.Get(this.Security, this.Security.SiteGuid, false, false, false));

                FuelCardLimitLineItem lineItem = new FuelCardLimitLineItem(site);

                lineItems.Add(lineItem);
                this.EnableControls(false);

                // The newly added row in the grid should be in edit.
                // We must set the page index to the last page. 
                // To calculate this, we divide the number of line items by the page size and round up. 
                // We must subtract one since the page index is zero based.
                this.LineItemsGrid.PageIndex = (int)Math.Ceiling((double)lineItems.Count / this.LineItemsGrid.PageSize) - 1;

                // If there is no remainder when dividing by the page size
                // Then the row added is the last record in the grid.
                // Otherwise, the row added is the remainder when dividing the count of line items by the page size
                // Keep in mind that the EditIndex is zero based, so we have to subtract one.
                if (lineItems.Count % this.LineItemsGrid.PageSize == 0)
                {
                    this.LineItemsGrid.EditIndex = this.LineItemsGrid.PageSize - 1;
                }
                else
                {
                    this.LineItemsGrid.EditIndex = (lineItems.Count % this.LineItemsGrid.PageSize) - 1;
                }

                this.FuelCardLimit.LineItems = lineItems;

                this.BindData(this.FuelCardLimit.LineItems);
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

        #region Drop Down List Population
        /// <summary>
        /// Get the types of line items to show in the Type drop down (Product, Product Group, All Products)
        /// </summary>
        /// <returns>The types of line items to show in the Type drop down</returns>
        protected ICollection EnumerateLineItemTypes()
        {
            Dictionary<FuelCardLimitLineItemType, string> translatedLimitLineItemTypes = new Dictionary<FuelCardLimitLineItemType, string>();

            try
            {
                List<FuelCardLimitLineItemType> lineItemTypes = 
					Enum.GetValues(typeof(FuelCardLimitLineItemType)).OfType<FuelCardLimitLineItemType>().ToList();

                bool useDataDictionary = FMFormBase.GetDataDictionaryFlag();

                // Add each line item type value to the dictionary. The dictionary key is the enumeration value itself, while the dictionary value is the display value
                // Apply the data dictionary to the display values if the system is configured to do so
                foreach (FuelCardLimitLineItemType lineItemType in lineItemTypes)
                {
                    string translatedValue = FuelCardLimitLineItem.GetUserFriendlyLineItemTypeEnumString(lineItemType);

                    if (useDataDictionary)
                    {
                        string localValue = translatedValue;
                        localValue = GetDataDictionaryValueByKey(Security.SiteGuid, localValue);
                    }

                    translatedLimitLineItemTypes.Add(lineItemType, translatedValue);
                }
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }

            return translatedLimitLineItemTypes;
        }

        /// <summary>
        /// Get the types of periods that can be selected for a line item (day, month, transactional, etc)
        /// </summary>
        /// <returns>The types of periods that can be selected for a line item</returns>
        protected ICollection EnumeratePeriods()
        {
            Dictionary<FuelCardLimitPeriod, string> translatedPeriodTypes = new Dictionary<FuelCardLimitPeriod, string>();

            try
            {
                List<FuelCardLimitPeriod> limitPeriodTypes = 
					Enum.GetValues(typeof(FuelCardLimitPeriod)).OfType<FuelCardLimitPeriod>().ToList();

                bool useDataDictionary = FMFormBase.GetDataDictionaryFlag();

                // Add each Period value to the dictionary. The dictionary key is the enumeration value itself, while the dictionary value is the display value
                // Apply the data dictionary to the display values if the system is configured to do so
                foreach (FuelCardLimitPeriod period in limitPeriodTypes)
                {
                    string translatedValue = period.ToString();

                    if (useDataDictionary)
                    {
                        FuelCardLimitPeriod localPeriod = period;
                        translatedValue = GetDataDictionaryValueByKey(Security.SiteGuid, localPeriod.ToString());
                    }

                    translatedPeriodTypes.Add(period, translatedValue);
                }
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }

            return translatedPeriodTypes;
        }

        /// <summary>
        /// Get the products to display in the product drop down when the type of line item is Product
        /// </summary>
        /// <returns>Products owned or assigned to the site</returns>
        protected ICollection EnumerateProducts()
        {
            ProductCollectionClass products = new ProductCollectionClass();

            try
            {
                products = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(
							productsClient => productsClient.EnumerateByFilterAndLocalize(this.Security, string.Empty, false));
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }

            return products;
        }

        /// <summary>
        /// Get the products to display in the product drop down when the type of line item is Product Group
        /// </summary>
        /// <returns>Product groups owned or assigned to the site</returns>
        protected ICollection EnumerateProductGroups()
        {
            ProductGroupCollectionClass productGroups = new ProductGroupCollectionClass();

            try
            {
                productGroups = FMChannelHelper.MakeCall<IProductGroups, ProductGroupCollectionClass>(
									productGroupsClient => productGroupsClient.Enumerate(this.Security));
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }

            return productGroups;
        }
        #endregion

        /// <summary>
        /// Update the data displayed on the form
        /// </summary>
        private void UpdateView()
        {
            try
            {
                FuelCardLimit fuelCardLimit = this.FuelCardLimit;

                if (fuelCardLimit != null)
                {
                    this.IDTextBox.Text = fuelCardLimit.ID;
                    this.BindData(fuelCardLimit.LineItems);
                }
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

        /// <summary>
        /// Bind the line items provided to the grid
        /// </summary>
        /// <param name="lineItems">The line items to bind to the grid</param>
        private void BindData(List<FuelCardLimitLineItem> lineItems)
        {
            this.LineItemsGrid.DataSource = lineItems;
            this.LineItemsGrid.DataBind();
        }

        /// <summary>
        /// Enable or disable controls on the screen.
        /// </summary>
        /// <param name="enable">True to enable, false to disable.</param>
        private void EnableControls(bool enable)
        {
            this.AddButton.Enabled = enable && this.Security.HasRight(RIGHT.MODIFY_FUEL_CARD_LIMIT)
                                  && (this.FuelCardLimit.SiteGuid == Guid.Empty
                                      || this.FuelCardLimit.SiteGuid == this.Security.SiteGuid);

            // Hide or show the grid pager too. We don't want users switching pages while a grid row is in edit.
            this.LineItemsGrid.PagerSettings.Visible = enable;

            // Call the main form to disable the OK button
            FuelCardLimitDetailForm detailForm = (FuelCardLimitDetailForm)this.Page;
            detailForm.EnableControls(enable);
        }

        /// <summary>
        /// Using the provided rowIndex in the grid, calculate the position in the line items collection using the pagesize and pageindex.
        /// </summary>
        /// <param name="rowIndex">The rowIndex to get the line items collection index for</param>
        /// <returns>The position in the line items collection</returns>
        private int GetLineItemIndexFromRowIndex(int rowIndex)
        {
            return ((this.LineItemsGrid.PageIndex + 1) * this.LineItemsGrid.PageSize) - (this.LineItemsGrid.PageSize - rowIndex);
        }
    }
}