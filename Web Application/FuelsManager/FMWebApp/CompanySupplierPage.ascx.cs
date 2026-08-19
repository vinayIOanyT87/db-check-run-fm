// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompanySupplierPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the CompanySupplierPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMWebApp
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FMControls;

	using FuelsManager.FMWebApp;

	/// <summary>
	///    Summary description for CompanySupplierPage.
	/// </summary>
	public partial class CompanySupplierPage : CompanyPageBase
	{
		#region Public Methods and Operators

		public void UpdateData()
		{
			if (!this.Company.HasRole(COMPANY_ROLE.SUPPLIER))
			{
				return;
			}
		}

		#endregion

		#region Methods

		protected ListItemCollection EnumerateProducts()
		{
			var ProductItems = new ListItemCollection();

			var AuthorizedProductsDataView = (DataView)this.AuthorizedProductsDataGrid.DataSource;

			int Item = this.AuthorizedProductsDataGrid.EditItemIndex
			           + this.AuthorizedProductsDataGrid.CurrentPageIndex * this.AuthorizedProductsDataGrid.PageSize;

			ProductCollectionClass ProductCollection = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(
																	 x =>
																	 x.Enumerate(this.Security)
																);


			foreach (ProductClass Product in ProductCollection)
			{
				if (Product.ProductType == ProductType.AdditiveProduct)
				{
					continue;
				}

				bool Found = false;
				foreach (ProductMapClass AuthorizedProduct in this.Company.SupplierAuthorizedProductCollection)
				{
					if (Product.ID != (string)AuthorizedProductsDataView[Item][1]
					    && Product.IdentityGuid == AuthorizedProduct.AssignedGuid)
					{
						Found = true;
						break;
					}
				}

				if (Found)
				{
					continue;
				}

				var NewProductItem = new ListItem(Product.ID, Product.IdentityGuid.ToString());
				foreach (ListItem ExistingProductItem in ProductItems)
				{
					if (ExistingProductItem.Text.CompareTo(NewProductItem.Text) > 0)
					{
						int Index = ProductItems.IndexOf(ExistingProductItem);
						ProductItems.Insert(Index, NewProductItem);
						NewProductItem = null;
						break;
					}
				}

				if (NewProductItem != null)
				{
					ProductItems.Add(NewProductItem);
				}
			}

			if (ProductItems.Count == 0)
			{
				throw new Exception("No Products Available");
			}

			return ProductItems;
		}

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!this.Company.HasRole(COMPANY_ROLE.SUPPLIER))
			{
				return;
			}
			// Put user code to initialize the page here
			this.SetUserRights();

			if (!this.Page.IsPostBack)
			{
				this.UpdateAuthorizedProductsView();
                SetFieldAccessibilityForChildRecordVersion();
			}
		}

		private void AddProductButton_Command(object sender, CommandEventArgs e)
		{
			var AuthorizedProduct = new ProductMapClass();
			AuthorizedProduct.AssignedToGuid = this.Company.IdentityGuid;
			AuthorizedProduct.Type = PRODUCT_MAP_TYPE.SUPPLIER_PRODUCT_COMPANY_MAP;
			this.Company.SupplierAuthorizedProductCollection.Add(AuthorizedProduct);
			this.AuthorizedProductsDataGrid.CurrentPageIndex = (this.Company.SupplierAuthorizedProductCollection.Count - 1)
			                                                   / this.AuthorizedProductsDataGrid.PageSize;
			this.AuthorizedProductsDataGrid.EditItemIndex = (this.Company.SupplierAuthorizedProductCollection.Count - 1)
			                                                % this.AuthorizedProductsDataGrid.PageSize;

			try
			{
				// Disable the add button.
				this.EnableControls(false);

				this.UpdateAuthorizedProductsView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				this.Company.SupplierAuthorizedProductCollection.RemoveAt(
					this.Company.SupplierAuthorizedProductCollection.Count - 1);

				if (this.AuthorizedProductsDataGrid.CurrentPageIndex > 0 && this.AuthorizedProductsDataGrid.EditItemIndex == 0)
				{
					this.AuthorizedProductsDataGrid.CurrentPageIndex--;
				}

				this.AuthorizedProductsDataGrid.EditItemIndex = -1;

				// Enable the add button.
				this.EnableControls(true);

				this.UpdateAuthorizedProductsView();
			}
		}

		private void AuthorizedProductsDataGrid_CancelCommand(object source, DataGridCommandEventArgs e)
		{
			var IndexLabel = (Label)e.Item.FindControl("IndexLabel");

			if (IndexLabel != null)
			{
				ProductMapClass AuthorizedProduct =
					this.Company.SupplierAuthorizedProductCollection[Convert.ToInt32(IndexLabel.Text)];

				if (AuthorizedProduct.AssignedGuid == Guid.Empty)
				{
					this.Company.SupplierAuthorizedProductCollection.RemoveAt(Convert.ToInt32(IndexLabel.Text));

					if (this.AuthorizedProductsDataGrid.Items.Count == 1 && this.AuthorizedProductsDataGrid.CurrentPageIndex > 0)
					{
						this.AuthorizedProductsDataGrid.CurrentPageIndex--;
					}
				}

				this.AuthorizedProductsDataGrid.EditItemIndex = -1;

				// Enable the add button.
				this.EnableControls(true);

				this.UpdateAuthorizedProductsView();
			}
		}

		private void AuthorizedProductsDataGrid_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			var IndexLabel = (Label)e.Item.FindControl("IndexLabel");
			if (IndexLabel != null)
			{
				if (this.AuthorizedProductsDataGrid.EditItemIndex == e.Item.ItemIndex)
				{
					this.AuthorizedProductsDataGrid.EditItemIndex = -1;
					this.EnableControls(true);
				}

				else if (this.AuthorizedProductsDataGrid.EditItemIndex > e.Item.ItemIndex)
				{
					this.AuthorizedProductsDataGrid.EditItemIndex--;
				}

				this.Company.SupplierAuthorizedProductCollection.RemoveAt(Convert.ToInt32(IndexLabel.Text));

				if (this.AuthorizedProductsDataGrid.Items.Count == 1 && this.AuthorizedProductsDataGrid.CurrentPageIndex > 0)
				{
					this.AuthorizedProductsDataGrid.CurrentPageIndex--;
				}

				this.UpdateAuthorizedProductsView();
			}
		}

		private void AuthorizedProductsDataGrid_EditCommand(object source, DataGridCommandEventArgs e)
		{
			this.AuthorizedProductsDataGrid.EditItemIndex = e.Item.ItemIndex;

			// Disable the add button.
			this.EnableControls(false);

			this.UpdateAuthorizedProductsView();
		}

		private void AuthorizedProductsDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			var IndexLabel = (Label)e.Item.FindControl("IndexLabel");
			if (IndexLabel != null)
			{
				ProductMapClass AuthorizedProduct =
					this.Company.SupplierAuthorizedProductCollection[Convert.ToInt32(IndexLabel.Text)];

				var ProductsDropDownList = (DropDownList)e.Item.FindControl("ProductsDropDownList");
				if (ProductsDropDownList != null)
				{
					if (AuthorizedProduct.AssignedGuid != Guid.Empty)
					{
						ListItemCollection Items = ProductsDropDownList.Items;
						int Index = Items.IndexOf(Items.FindByValue(AuthorizedProduct.AssignedGuid.ToString()));
						ProductsDropDownList.SelectedIndex = Index;
					}
				}

                var EditButton = (FMEditLinkButton)e.Item.FindControl("EditButton");
                var DeleteButton = (FMDeleteLinkButton)e.Item.FindControl("DeleteButton");
				// Inhibit Edits of Products Assigned by way of Company Groups
				if ((AuthorizedProduct.Type == PRODUCT_MAP_TYPE.PRODUCT_COMPANY_GROUP_MAP)
				    || (this.Security.HasRight(RIGHT.MODIFY_COMPANY_DATA) == false))
				{					
					if (EditButton != null)
					{
						EditButton.Enabled = false;
					}
					
					if (DeleteButton != null)
					{
						DeleteButton.Enabled = false;
					}
				}
                //Set the availability of the Grid editing buttons for child record versions
                bool currentSiteOwnsRecordVersion = (Company.SiteGuid == Security.SiteGuid);
                if ((Company.IdentityGuid.Equals(Guid.Empty)) || (currentSiteOwnsRecordVersion && Company.IdentityGuid.Equals(Company.MasterRecordGuid)))
                    return;
                if ((EditButton != null) && (DeleteButton != null))
                {
                    if ((VersionSpecificFields == null) || !VersionSpecificFields.Contains("SupplierAuthorizedProducts"))
                    {
                        EditButton.Enabled = false;
                        DeleteButton.Enabled = false;
                    }
                }
			}
		}

		private void AuthorizedProductsDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.AuthorizedProductsDataGrid.EditItemIndex > -1)
			{
				return;
			}
			this.AuthorizedProductsDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateAuthorizedProductsView();
		}

		private void AuthorizedProductsDataGrid_UpdateCommand(object source, DataGridCommandEventArgs e)
		{
			var IndexLabel = (Label)e.Item.FindControl("IndexLabel");
			if (IndexLabel != null)
			{
				ProductMapClass AuthorizedProduct =
					this.Company.SupplierAuthorizedProductCollection[Convert.ToInt32(IndexLabel.Text)];

				var ProductsDropDownList = (DropDownList)e.Item.FindControl("ProductsDropDownList");
				AuthorizedProduct.AssignedGuid = Guid.Parse(ProductsDropDownList.SelectedValue);
				AuthorizedProduct.AssignedID = ProductsDropDownList.SelectedItem.Text;

				AuthorizedProduct.ShipToProductID = "";

				AuthorizedProduct.ShipToProductCode = "";

				AuthorizedProduct.ShipToLoadRackDisplayText = "";

				this.AuthorizedProductsDataGrid.EditItemIndex = -1;

				// Enable the add button.
				this.EnableControls(true);

				this.UpdateAuthorizedProductsView();
			}
		}

		/// <summary>
		///    This method will enable and disable controls.
		/// </summary>
		/// <param name="enable"></param>
		private void EnableControls(bool enable)
		{
			this.AddProductButton.Enabled = enable;

			// JS20100803 WI-16554 Call the main form to disable buttons and tabs.
			var companyForm = (CompanyForm)this.Page;
			companyForm.EnableControls(enable);
		}

		private ICollection EnumerateAuthorizedProducts()
		{
			var AuthorizedProductsDataTable = new DataTable();
			DataRow AuthorizedProductDataRow;

			AuthorizedProductsDataTable.Columns.Add("Index", typeof(Int32));
			AuthorizedProductsDataTable.Columns.Add("ProductID", typeof(string));

			string AddText = this.GetTranslatedText("Add");
			string EditText = this.GetTranslatedText("Edit");

			int Item = 0;
			foreach (ProductMapClass AuthorizedProduct in this.Company.SupplierAuthorizedProductCollection)
			{
				AuthorizedProductDataRow = AuthorizedProductsDataTable.NewRow();

				AuthorizedProductDataRow["Index"] = Item;
				AuthorizedProductDataRow["ProductID"] = AuthorizedProduct.AssignedID;

				// Sort the DataTable by ProductID except for an Added entry
				// which has a null ProductID that messes up the sort order
				// relative to the EditItemIndex
				if (AuthorizedProduct.AssignedGuid != Guid.Empty)
				{
					int iRow = 0;
					foreach (DataRow Row in AuthorizedProductsDataTable.Rows)
					{
						if (((string)Row["ProductID"]).CompareTo((string)AuthorizedProductDataRow["ProductID"]) > 0)
						{
							AuthorizedProductsDataTable.Rows.InsertAt(AuthorizedProductDataRow, iRow);
							AuthorizedProductDataRow = null;
							break;
						}
						iRow++;
					}
				}

				if (AuthorizedProductDataRow != null)
				{
					AuthorizedProductsDataTable.Rows.Add(AuthorizedProductDataRow);
				}

				Item++;
			}
			var AuthorizedProductsDataView = new DataView(AuthorizedProductsDataTable);
			return AuthorizedProductsDataView;
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AuthorizedProductsDataGrid.EditCommand +=
				new System.Web.UI.WebControls.DataGridCommandEventHandler(this.AuthorizedProductsDataGrid_EditCommand);
			this.AuthorizedProductsDataGrid.PageIndexChanged +=
				new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.AuthorizedProductsDataGrid_PageIndexChanged);
			this.AuthorizedProductsDataGrid.CancelCommand +=
				new System.Web.UI.WebControls.DataGridCommandEventHandler(this.AuthorizedProductsDataGrid_CancelCommand);
			this.AuthorizedProductsDataGrid.UpdateCommand +=
				new System.Web.UI.WebControls.DataGridCommandEventHandler(this.AuthorizedProductsDataGrid_UpdateCommand);
			this.AuthorizedProductsDataGrid.DeleteCommand +=
				new System.Web.UI.WebControls.DataGridCommandEventHandler(this.AuthorizedProductsDataGrid_DeleteCommand);
			this.AddProductButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddProductButton_Command);
			this.AuthorizedProductsDataGrid.ItemDataBound +=
				new System.Web.UI.WebControls.DataGridItemEventHandler(this.AuthorizedProductsDataGrid_ItemDataBound);
		}

		private void SetUserRights()
		{
			if (this.Security.HasRight(RIGHT.MODIFY_COMPANY_DATA) == false)
			{
				this.AddProductButton.Enabled = false;
			}
		}

		private void UpdateAuthorizedProductsView()
		{
			this.AuthorizedProductsDataGrid.DataSource = this.EnumerateAuthorizedProducts();
			this.AuthorizedProductsDataGrid.DataBind();
		}


        private void SetFieldAccessibilityForChildRecordVersion()
        {
            bool currentSiteOwnsRecordVersion = (this.Company.SiteGuid == this.Security.SiteGuid);
            if ((this.Company.IdentityGuid.Equals(Guid.Empty)
                 || (currentSiteOwnsRecordVersion && this.Company.IdentityGuid.Equals(this.Company.MasterRecordGuid))))
            {
                return;
            }

            this.AddProductButton.Enabled = (this.AddProductButton.Enabled && (this.VersionSpecificFields != null)
                                               && this.VersionSpecificFields.Contains("SupplierAuthorizedProducts"));
        }

		#endregion
	}
}