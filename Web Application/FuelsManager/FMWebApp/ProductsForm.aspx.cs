// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProductsForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ProductsForm type.
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
	using System.Net.Sockets;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;
	using FMBusinessObjects.UtilityObjects;

	using FMControls;

	using global::FMWebApp;

	/// <summary>
	///    Summary description for ProductsForm.
	/// </summary>
	public partial class ProductsForm : FMAutoSubmitFormBase, IEntityDiscovery, IMenuDiscovery
	{
		#region Constants and Fields
		protected FMLabel FilterLabel;

		private const string ProductFindString = "ProductFindString";
		private string searchString;

        /// <summary>
        /// Retain the state of the Show Hidden checkbox
        /// </summary>
        private bool SessionProductSummaryShowHiddenChecked
        {
            get
            {
                if (this.Session["ProductSummaryShowHiddenChecked"] is bool)
                {
                    return (bool)this.Session["ProductSummaryShowHiddenChecked"];
                }
                else
                {
                    return false;
                }
            }

            set
            {
                this.Session.Add("ProductSummaryShowHiddenChecked", value);
            }
        }

		#endregion

		#region Explicit Interface Properties
		bool IEntityDiscovery.EntityAssignable => true;

	    Type IEntityDiscovery.EntityEngineType => typeof(IProducts);

	    ENTITY_TYPE IEntityDiscovery.EntityType => ENTITY_TYPE.PRODUCT;

	    #endregion

		#region Public Methods and Operators
		/// <summary>
		///    Gets a list of menu items that should be displayed for the current user.
		/// </summary>
		/// <param name="security">The security object of the current session</param>
		/// <param name="siteGroup">Whether the current logged-in site is a site group</param>
		/// <param name="options">Hardware key options</param>
		/// <returns>
		///    List of menu items to be displayed
		/// </returns>
		public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1,ushort word2, ushort useNewLicenseKey, uint options)
		{
            if (useNewLicenseKey == 1)
            {

            }
            else
            {
                // Depends Upon Shared Components Config
                if ((options & 0x4000) == 0)
                {
                    return null;
                }
            }
            if (!security.HasRight(RIGHT.VIEW_PRODUCTS) && !security.HasRight(RIGHT.MODIFY_PRODUCTS))
			{
				return null;
			}

			var menuItems = new List<FMMenuItem>();

			var menuItem = new FMMenuItem
				{
					MenuItemType = FMMenuItemType.ASSETS_PRODUCTS_PRODUCTS,
					RootMenuName = "Assets",
					CategoryName = "Products",
					ItemName = "Products",
					NavigateUrl = "ProductsForm.aspx",
					ApplyDataDictionary = ApplyDataDictionary.Apply,
					SortOrder = 1
				};

			menuItems.Add(menuItem);

			return menuItems;
		}
		#endregion

		#region Explicit Interface Methods

		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(SecurityClass security, 
																			ENTITY_ASSIGNMENT_TYPE entityAssignmentType)
		{
			ProductCollectionClass productCollection;


            if (entityAssignmentType == ENTITY_ASSIGNMENT_TYPE.UNDELEGATED)
            {
                productCollection = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(
                                                                     x =>
                                                                     x.EnumerateUndelegated(security)
                                                                );
            }
            else
            {
                productCollection = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(
                                                                     x =>
                                                                     x.EnumerateBySite(security)
                                                                );
            }

			var entityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			foreach (ProductClass product in productCollection)
			{
				if (entityAssignmentType == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if (security.SiteGuid == product.SiteGuid)
					{
						continue;
					}

					if (security.LoginSiteGuid != product.SiteGuid)
					{
						continue;
					}
				}
				else
				{
                    //For entity types supporting Record Versioning, assignments can be cascaded, irrespective of whether Record Versioning is turned on or off.
                    if ((security.SiteGuid != product.SiteGuid) && (security.SiteGuid != product.AssignedToSiteGuid))
					{
						continue;
					}
				}

				// The EntityToSiteMap references Product records by their MasterRecordGuids 
				// instead of their actual ProductGuids.
				var entityToSiteMap = new EntityToSiteMapClass(product) { IdentityGuid = product.MasterRecordGuid };
				entityToSiteMapCollection.Add(entityToSiteMap);
			}

			return entityToSiteMapCollection;
		}

		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string entityId)
		{
			return FMChannelHelper.MakeCall<IProducts, Guid>(
																	 x =>
																	 x.GetIdentityGuid(security, entityId)
																);
		}

		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid siteGuid)
		{
			ProductClass product = FMChannelHelper.MakeCall<IProducts, ProductClass>(
																	 x =>
																	 x.Get(security, guid)
																);
			product.SiteGuid = siteGuid;
			FMChannelHelper.MakeCall<IProducts>(
																	 x =>
																	 x.Modify(security, product)
																);
		}
		#endregion

		#region Methods
		protected void FindAllOnClick(object sender, EventArgs e)
		{
			this.Session.Remove(ProductFindString);
			this.searchString = null;
			this.FindTextBox.Text = string.Empty;
			this.ProductsDataGrid.CurrentPageIndex = 0;
			this.UpdateView();
		}

		protected void FindBtnOnClick(object sender, EventArgs e)
		{
			if ((this.FindTextBox == null) || (this.FindTextBox.Text.Length < 1))
			{
				this.searchString = null;
				this.Session.Remove(ProductFindString);
			}
			else
			{
				this.searchString = this.FindTextBox.Text.ToUpper();
				this.FindTextBox.Text = this.searchString;
				this.Session.Add(ProductFindString, this.searchString);
			}

			// Update the page with the new contents.
			this.ProductsDataGrid.CurrentPageIndex = 0;
			this.UpdateView();
		}

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!this.Security.HasRight(RIGHT.MODIFY_PRODUCTS))
				{
					this.AddButton.Enabled = false;
					this.AddButton2.Enabled = false;
				}

				if (!this.Page.IsPostBack)
				{
					if (this.Session["ProductType"] == null)
					{
						this.Session["ProductType"] = ProductType.MaxProduct;
					}

					// people want the all to be first but the way this is designed it will always be last
					// that is why we are setting it first and outside of the loop below
					this.ProductTypeDropDownList.Items.Add(
						new ListItem(ProductClass.ProductTypeID(ProductType.MaxProduct), ((int)ProductType.MaxProduct).ToString()));

					for (var productType = ProductType.ComponentProduct; productType < ProductType.MaxProduct; productType++)
					{
						if (productType == ProductType.AdditizedProduct)
						{
							continue;
						}

						var newTypeItem = new ListItem(ProductClass.ProductTypeID(productType), 
														((int)productType).ToString(CultureInfo.InvariantCulture));
						this.ProductTypeDropDownList.Items.Add(newTypeItem);
						
						if (this.Session["ProductType"] != null && (ProductType)this.Session["ProductType"] == productType)
						{
							this.ProductTypeDropDownList.SelectedIndex = this.ProductTypeDropDownList.Items.Count - 1;
						}
					}

					this.Session["ProductType"] = (ProductType)Convert.ToInt32(this.ProductTypeDropDownList.SelectedItem.Value);

					if (this.Session["ProductsPage"] != null)
					{
						this.ProductsDataGrid.CurrentPageIndex = (int)this.Session["ProductsPage"];
						this.Session.Remove("ProductsPage");
					}

					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void ProductTypeDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				this.Session["ProductType"] = (ProductType)Convert.ToInt32(this.ProductTypeDropDownList.SelectedItem.Value);

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

				this.ProductsDataGrid.CurrentPageIndex = 0;
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
                if (string.IsNullOrEmpty(this.FindTextBox?.Text))
                {
                    this.searchString = null;
                }
                else
                {
                    this.searchString = this.FindTextBox.Text.ToUpper();
                    this.FindTextBox.Text = this.searchString;
                }

                this.SessionProductSummaryShowHiddenChecked = this.ShowHiddenCheckBox.Checked;

                // Update the page with the new contents.
                this.ProductsDataGrid.CurrentPageIndex = 0;
                this.UpdateView();
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

		private void AddButtonCommand(object sender, CommandEventArgs e)
		{
			this.Session.Remove("ProductArrayList");
			this.Session.Remove("ProductSelectContextArrayList");

			var product = new ProductClass(FMChannelHelper.MakeCall<ISites, SiteClass>(
												sites => sites.GetBasic(this.Security, this.Security.SiteGuid)));

			var productArrayList = new ArrayList { product };

			this.Session["ProductArrayList"] = productArrayList;
			this.Session["ProductsPage"] = this.ProductsDataGrid.CurrentPageIndex;
			this.Redirect("ProductForm.aspx");
		}

		private ICollection EnumerateProducts()
		{
			ProductCollectionClass productCollection;

			var productType = (ProductType)Convert.ToInt32(this.ProductTypeDropDownList.SelectedValue);

		    var limits = new EnumerationLimits();
			var limit = limits.GetLimit(EnumerationLimits.EnumerationOptions.PRODUCT);

			// Determine whether to retrieve the products using a filter or not.  If the user entered in 
			// find string, then use the filter method.
			if (string.IsNullOrEmpty(this.searchString))
			{
				productCollection = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(
																	 x =>
                                                                     x.EnumerateByType(this.Security, productType, hideHiddenProducts: !this.ShowHiddenCheckBox.Checked, limit : limit)
																);

			}
			else
			{
				productCollection = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(
																	 x =>
                                                                     x.EnumerateByTypeAndFilter(this.Security, productType, this.searchString, hideHiddenProducts: !this.ShowHiddenCheckBox.Checked, limit : limit)
																);

			}

            if (productCollection.Count >= limit && limit > 0)
            {
                this.lblWarning.Text = "Results limited to first " + limit + " records.  Use filters to narrow search.";
                this.lblWarning.Visible = true;
            }
            else
            {
                this.lblWarning.Visible = false;
            }

            var productDataTable = new DataTable();

			productDataTable.Columns.Add("SiteGuid", typeof(Guid));
			productDataTable.Columns.Add("IdentityGuid", typeof(Guid));
            productDataTable.Columns.Add("MasterRecordGuid", typeof(Guid));
			productDataTable.Columns.Add("ID", typeof(string));
			productDataTable.Columns.Add("Code", typeof(string));
			productDataTable.Columns.Add("Description", typeof(string));
			productDataTable.Columns.Add("Type", typeof(string));
			productDataTable.Columns.Add("VaporRecovery", typeof(bool));

            // HiddenDate is a nullable DateTimeOffset (DateTimeOffset?) but you can't use nullable types in data tables.
            productDataTable.Columns.Add("HiddenDate", typeof(DateTimeOffset));

			foreach (ProductClass product in productCollection)
			{
				DataRow productDataRow = productDataTable.NewRow();

				productDataRow["SiteGuid"] = product.SiteGuid;
				productDataRow["IdentityGuid"] = product.IdentityGuid;
                productDataRow["MasterRecordGuid"] = product.MasterRecordGuid;
				productDataRow["ID"] = product.ID;
				productDataRow["Code"] = product.Code;
				productDataRow["Description"] = product.Description;
				productDataRow["Type"] = this.GetTranslatedText(ProductClass.ProductTypeID(product.ProductType));
				productDataRow["VaporRecovery"] = product.VaporRecovery;
                productDataRow["HiddenDate"] = product.HiddenDate ?? (object)DBNull.Value;

				productDataTable.Rows.Add(productDataRow);
			}

			var productDataView = new DataView(productDataTable);
			return productDataView;
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.ProductsDataGrid.EditCommand		+= this.ProductsDataGridEditCommand;
			this.ProductsDataGrid.PageIndexChanged	+= this.ProductsDataGridPageIndexChanged;
			this.ProductsDataGrid.DeleteCommand		+= this.ProductsDataGridDeleteCommand;
			this.ProductsDataGrid.ItemDataBound		+= this.ProductsDataGridItemDataBound;
			this.AddButton.Command					+= this.AddButtonCommand;
			this.AddButton2.Command					+= this.AddButtonCommand;

            var limits = new EnumerationLimits();
            int pageLimit = limits.GetLimit(EnumerationLimits.EnumerationOptions.PRODUCT);
            this.ProductSummaryPageSizeDropDown.SetLimit(pageLimit);
            this.ProductsDataGrid.PageSize = pageLimit;
        }

		private void ProductsDataGridDeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				this.GetSecurity();

				// Get identityGuid
				TableCell identityGuidCell = e.Item.Cells[2]; //bds
				Guid identityGuid = Guid.Parse(identityGuidCell.Text);

				try
				{
					if (UsingLoadRack)
					{
						ILoadRackManager loadRackManager = this.GetLoadRackManager();
						loadRackManager.Purge(this.Security, typeof(ProductClass), identityGuid);
					}
				}
				catch (SocketException socketExcept)
				{
					if (socketExcept.ErrorCode != 10061)
					{
						throw;
					}
				}

				FMChannelHelper.MakeCall<IProducts>(x => x.Purge(this.Security, identityGuid));

				this.ProductsDataGrid.SelectedIndex = -1;
				this.Session.Remove("IdentityGuid");

				if (this.ProductsDataGrid.Items.Count == 1 && this.ProductsDataGrid.CurrentPageIndex > 0)
				{
					this.ProductsDataGrid.CurrentPageIndex--;
				}
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void ProductsDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				this.Session.Remove("ProductArrayList");
				this.Session.Remove("ProductSelectContextArrayList");

				TableCell identityGuidCell = e.Item.Cells[2]; //bds

				ProductClass product = 
					FMChannelHelper.MakeCall<IProducts, ProductClass>(
							x =>
							x.GetByProductAuthorizedCompanies(this.Security, Guid.Parse(identityGuidCell.Text), true)
					);

				var productArrayList = new ArrayList { product };

				this.Session["ProductArrayList"] = productArrayList;
				this.Session["ProductsPage"] = this.ProductsDataGrid.CurrentPageIndex;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}

			this.Redirect("ProductForm.aspx");
		}

		private void ProductsDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			var deleteButton = (LinkButton)e.Item.FindControl("DeleteButton");

		    if (deleteButton != null)
		    {
		        TableCell siteGuidCell = e.Item.Cells[1];  //bds

				if (!this.Security.HasRight(RIGHT.MODIFY_PRODUCTS) || this.Security.SiteGuid != Guid.Parse(siteGuidCell.Text))
		        {
		            deleteButton.Enabled = false;
		        }

		        //Child record versions cannot be created or deleted directly. Their lifetime is controlled by the Entity-To-Site assignment only.
		        if (deleteButton.Enabled)
		        {
		            int index = this.ProductsDataGrid.CurrentPageIndex * this.ProductsDataGrid.PageSize + e.Item.ItemIndex;
		            var dv = (DataView)this.ProductsDataGrid.DataSource;

		            if (!dv.Table.Rows[index]["IdentityGuid"].Equals(dv.Table.Rows[index]["MasterRecordGuid"]))
		            {
		                deleteButton.Enabled = false;
		            }
		        }
		    }

		    // Change the color of the text of hidden products to give the user a visual indication that the product is hidden.
		    var view = e.Item.DataItem as DataRowView;
		    DateTimeOffset? hiddenDate = view?.Row["HiddenDate"] as DateTimeOffset?;
		    if (hiddenDate != null)
		    {
		        e.Item.ForeColor = Color.Red;
		    }
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

        private void ProductsDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			try
			{
				// if we are editing do not allow a page change
				if (this.ProductsDataGrid.EditItemIndex > -1)
				{
					return;
				}

				this.ProductsDataGrid.CurrentPageIndex = e.NewPageIndex;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    This method will update the product grid and reset the find string to the
		///    value in session.
		/// </summary>
		private void UpdateView()
		{
			// Locate the previous search string from the session. Set the set
			// string if found.
			if (this.Session[ProductFindString] != null)
			{
				this.FindTextBox.Text = this.Session[ProductFindString] as string;
				this.searchString = this.Session[ProductFindString] as string;
			}

            this.ShowHiddenCheckBox.Checked = this.SessionProductSummaryShowHiddenChecked;

            var productType = (ProductType)Convert.ToInt32(this.ProductTypeDropDownList.SelectedValue);

			// Don't show Vapor Recovery for Additives
			this.ProductsDataGrid.Columns[7].Visible = (productType != ProductType.AdditiveProduct); //bds

			ICollection products = this.EnumerateProducts();

            this.ProductSummaryPageSizeDropDown.SetPageSize(this.ProductsDataGrid, products.Count);
            this.ProductsDataGrid.DataSource = products;
			this.ProductsDataGrid.DataBind();
        }
		#endregion

		//*************************************************************************************************
		// This method is called when the find button is pressed. It will retrieve data from the find
		// text box and set the search string. If there is no data, then the search string is set to null.
		//*************************************************************************************************
	}
}