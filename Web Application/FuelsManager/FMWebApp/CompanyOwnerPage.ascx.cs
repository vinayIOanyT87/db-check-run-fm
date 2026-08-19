// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompanyOwnerPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the CompanyOwnerPage type.
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

	using FuelsManager.FMWebApp;

	/// <summary>
	///    Summary description for CompanyOwnerPage.
	/// </summary>
	public partial class CompanyOwnerPage : CompanyPageBase
	{
		#region Public Methods and Operators

		public void UpdateData()
		{
			if (!this.Company.HasRole(COMPANY_ROLE.OWNER))
			{
				return;
			}
		}

		#endregion

		#region Methods

		protected ListItemCollection EnumerateProducts()
		{
			var ProductItems = new ListItemCollection();

			var UnavailableInventoriesDataView = (DataView)this.UnavailableInventoriesDataGrid.DataSource;

			int Item = this.UnavailableInventoriesDataGrid.EditItemIndex
			           + this.UnavailableInventoriesDataGrid.CurrentPageIndex * this.UnavailableInventoriesDataGrid.PageSize;

			ProductCollectionClass ProductCollection = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(
																	 x =>
																	 x.Enumerate(this.Security)
																);

			foreach (ProductClass	Product in ProductCollection)
			{
				if (Product.ProductType == ProductType.AdditiveProduct)
				{
					continue;
				}

				bool Found = false;
				foreach (ProductMapClass UnavailableInventory in this.Company.UnavailableInventoryCollection)
				{
					if (Product.ID != (string)UnavailableInventoriesDataView[Item][1]
					    && Product.IdentityGuid == UnavailableInventory.AssignedGuid)
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
			this.InitializeComponent();
			base.OnInit(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				if (!this.Company.HasRole(COMPANY_ROLE.OWNER))
				{
					return;
				}

				if (! this.Page.IsPostBack)
				{
					this.UpdateUnavailableInventoriesView();
                    SetFieldAccessibilityForChildRecordVersion();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void AddProductButton_Command(object sender, CommandEventArgs e)
		{
			var UnavailableInventory = new ProductMapClass();
			UnavailableInventory.AssignedToGuid = this.Company.IdentityGuid;
			UnavailableInventory.Type = PRODUCT_MAP_TYPE.UNAVAILABLE_INVENTORY_COMPANY_MAP;
			this.Company.UnavailableInventoryCollection.Add(UnavailableInventory);
			this.UnavailableInventoriesDataGrid.CurrentPageIndex = (this.Company.UnavailableInventoryCollection.Count - 1)
			                                                       / this.UnavailableInventoriesDataGrid.PageSize;
			this.UnavailableInventoriesDataGrid.EditItemIndex = (this.Company.UnavailableInventoryCollection.Count - 1)
			                                                    % this.UnavailableInventoriesDataGrid.PageSize;

			try
			{
				// Disable the add button.
				this.EnableControls(false);

				this.UpdateUnavailableInventoriesView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				this.Company.UnavailableInventoryCollection.RemoveAt(this.Company.UnavailableInventoryCollection.Count - 1);

				if (this.UnavailableInventoriesDataGrid.CurrentPageIndex > 0
				    && this.UnavailableInventoriesDataGrid.EditItemIndex == 0)
				{
					this.UnavailableInventoriesDataGrid.CurrentPageIndex--;
				}

				this.UnavailableInventoriesDataGrid.EditItemIndex = -1;

				// Enable the add button.
				this.EnableControls(true);

				this.UpdateUnavailableInventoriesView();
			}
		}

		/// <summary>
		///    This method will enable and disable controls.
		/// </summary>
		/// <param name="enable"></param>
		private void EnableControls(bool enable)
		{
			this.AddProductButton.Enabled = enable;

			// Call the main form to disable buttons and tabs.
			var companyForm = (CompanyForm)this.Page;
			companyForm.EnableControls(enable);
		}

		private ICollection EnumerateUnavailableInventories()
		{
			var UnavailableInventoriesDataTable = new DataTable();
			DataRow UnavailableInventoriesDataRow;

			UnavailableInventoriesDataTable.Columns.Add("Index", typeof(Int32));
			UnavailableInventoriesDataTable.Columns.Add("ProductID", typeof(string));
			UnavailableInventoriesDataTable.Columns.Add("Gross", typeof(string));
			UnavailableInventoriesDataTable.Columns.Add("Net", typeof(string));

			string AddText = this.GetTranslatedText("Add");
			string EditText = this.GetTranslatedText("Edit");

			int Item = 0;
			foreach (ProductMapClass UnavailableInventory in this.Company.UnavailableInventoryCollection)
			{
				UnavailableInventoriesDataRow = UnavailableInventoriesDataTable.NewRow();

				UnavailableInventoriesDataRow["Index"] = Item;
				UnavailableInventoriesDataRow["ProductID"] = UnavailableInventory.AssignedID;
				UnavailableInventoriesDataRow["Gross"] = UnavailableInventory.UnavailableInventoryGross;
				UnavailableInventoriesDataRow["Net"] = UnavailableInventory.UnavailableInventoryNet;

				// Sort the DataTable by ProductID except for an Added entry
				// which has a null ProductID that messes up the sort order
				// relative to the EditItemIndex
				if (UnavailableInventory.AssignedGuid != Guid.Empty)
				{
					int iRow = 0;
					foreach (DataRow Row in UnavailableInventoriesDataTable.Rows)
					{
						if (((string)Row["ProductID"]).CompareTo((string)UnavailableInventoriesDataRow["ProductID"]) > 0)
						{
							UnavailableInventoriesDataTable.Rows.InsertAt(UnavailableInventoriesDataRow, iRow);
							UnavailableInventoriesDataRow = null;
							break;
						}
						iRow++;
					}
				}
				if (UnavailableInventoriesDataRow != null)
				{
					UnavailableInventoriesDataTable.Rows.Add(UnavailableInventoriesDataRow);
				}

				Item++;
			}

			var UnavailableInventoriesDataView = new DataView(UnavailableInventoriesDataTable);
			return UnavailableInventoriesDataView;
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AddProductButton.Command += this.AddProductButton_Command;
			this.UnavailableInventoriesDataGrid.EditCommand += this.UnavailableInventoriesDataGrid_EditCommand;
			this.UnavailableInventoriesDataGrid.PageIndexChanged += this.UnavailableInventoriesDataGrid_PageIndexChanged;
			this.UnavailableInventoriesDataGrid.CancelCommand += this.UnavailableInventoriesDataGrid_CancelCommand;
			this.UnavailableInventoriesDataGrid.UpdateCommand += this.UnavailableInventoriesDataGrid_UpdateCommand;
			this.UnavailableInventoriesDataGrid.DeleteCommand += this.UnavailableInventoriesDataGrid_DeleteCommand;
			this.UnavailableInventoriesDataGrid.ItemDataBound += this.UnavailableInventoriesDataGrid_ItemDataBound;
		}

		private void UnavailableInventoriesDataGrid_CancelCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				var IndexLabel = (Label)e.Item.FindControl("IndexLabel");

				if (IndexLabel != null)
				{
					ProductMapClass UnavailableInventory =
						this.Company.UnavailableInventoryCollection[Convert.ToInt32(IndexLabel.Text)];

					if (UnavailableInventory.AssignedGuid == Guid.Empty)
					{
						this.Company.UnavailableInventoryCollection.RemoveAt(Convert.ToInt32(IndexLabel.Text));

						if (this.UnavailableInventoriesDataGrid.Items.Count == 1
						    && this.UnavailableInventoriesDataGrid.CurrentPageIndex > 0)
						{
							this.UnavailableInventoriesDataGrid.CurrentPageIndex--;
						}
					}

					this.UnavailableInventoriesDataGrid.EditItemIndex = -1;

					// Enable the add button.
					this.EnableControls(true);

					this.UpdateUnavailableInventoriesView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void UnavailableInventoriesDataGrid_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				var IndexLabel = (Label)e.Item.FindControl("IndexLabel");
				if (IndexLabel != null)
				{
					if (this.UnavailableInventoriesDataGrid.EditItemIndex == e.Item.ItemIndex)
					{
						this.UnavailableInventoriesDataGrid.EditItemIndex = -1;
						this.EnableControls(true);
					}

					else if (this.UnavailableInventoriesDataGrid.EditItemIndex > e.Item.ItemIndex)
					{
						this.UnavailableInventoriesDataGrid.EditItemIndex--;
					}

					this.Company.UnavailableInventoryCollection.RemoveAt(Convert.ToInt32(IndexLabel.Text));

					if (this.UnavailableInventoriesDataGrid.Items.Count == 1
					    && this.UnavailableInventoriesDataGrid.CurrentPageIndex > 0)
					{
						this.UnavailableInventoriesDataGrid.CurrentPageIndex--;
					}

					this.UpdateUnavailableInventoriesView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void UnavailableInventoriesDataGrid_EditCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				this.UnavailableInventoriesDataGrid.EditItemIndex = e.Item.ItemIndex;

				// Disable the add button.
				this.EnableControls(false);

				this.UpdateUnavailableInventoriesView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void UnavailableInventoriesDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			try
			{
				var IndexLabel = (Label)e.Item.FindControl("IndexLabel");
				if (IndexLabel != null)
				{
					ProductMapClass UnavailableInventory =
						this.Company.UnavailableInventoryCollection[Convert.ToInt32(IndexLabel.Text)];

					var ProductsDropDownList = (DropDownList)e.Item.FindControl("ProductsDropDownList");
					if (ProductsDropDownList != null)
					{
						if (UnavailableInventory.AssignedGuid != Guid.Empty)
						{
							ListItemCollection Items = ProductsDropDownList.Items;
							int Index = Items.IndexOf(Items.FindByValue(UnavailableInventory.AssignedGuid.ToString()));
							ProductsDropDownList.SelectedIndex = Index;
						}
					}
					//Set the availability of the Grid editing buttons for child record versions
					bool currentSiteOwnsRecordVersion = (Company.SiteGuid == Security.SiteGuid);
					if ((Company.IdentityGuid.Equals(Guid.Empty))
					    || (currentSiteOwnsRecordVersion && Company.IdentityGuid.Equals(Company.MasterRecordGuid)))
                        return;
					LinkButton EditButton = (LinkButton)e.Item.FindControl("EditButton");
					LinkButton DeleteButton = (LinkButton)e.Item.FindControl("DeleteButton");
					if ((EditButton != null) && (DeleteButton != null))
					{
						if ((VersionSpecificFields == null) || !VersionSpecificFields.Contains("UnavailableInventories"))
						{
							EditButton.Enabled = false;
							DeleteButton.Enabled = false;
						}
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void UnavailableInventoriesDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			try
			{
				// if we are editing do not allow a page change
				if (this.UnavailableInventoriesDataGrid.EditItemIndex > -1)
				{
					return;
				}
				this.UnavailableInventoriesDataGrid.CurrentPageIndex = e.NewPageIndex;
				this.UpdateUnavailableInventoriesView();
			}
			catch ( Exception except )
			{
				this.ErrorHandler( except );
			}
		}

		private void UnavailableInventoriesDataGrid_UpdateCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				var IndexLabel = (Label)e.Item.FindControl("IndexLabel");
				if (IndexLabel != null)
				{
					ProductMapClass UnavailableInventory =
						this.Company.UnavailableInventoryCollection[Convert.ToInt32(IndexLabel.Text)];

					var ProductsDropDownList = (DropDownList)e.Item.FindControl("ProductsDropDownList");
					UnavailableInventory.AssignedGuid = Guid.Parse(ProductsDropDownList.SelectedValue);
					UnavailableInventory.AssignedID = ProductsDropDownList.SelectedItem.Text;

					var GrossTextBox = (TextBox)e.Item.FindControl("GrossTextBox");
					UnavailableInventory.UnavailableInventoryGross = GrossTextBox.Text;
					var NetTextBox = (TextBox)e.Item.FindControl("NetTextBox");
					UnavailableInventory.UnavailableInventoryNet = NetTextBox.Text;

					this.UnavailableInventoriesDataGrid.EditItemIndex = -1;

					// Enable the add button.
					this.EnableControls(true);

					this.UpdateUnavailableInventoriesView();
				}
			}
			catch ( Exception except )
			{
				this.ErrorHandler( except );
			}
		}

		private void UpdateUnavailableInventoriesView()
		{
			this.UnavailableInventoriesDataGrid.DataSource = this.EnumerateUnavailableInventories();
			this.UnavailableInventoriesDataGrid.DataBind();
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
                                               && this.VersionSpecificFields.Contains("UnavailableInventories"));
        }

		#endregion
	}
}