// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompanyGroupProductsPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the CompanyGroupProductsPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMWebApp
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FMControls;
    using FMCore;

	using FuelsManager.FMWebApp;

	/// <summary>
	///    Summary description for CompanyGroupGroupProductsPage.
	/// </summary>
	public partial class CompanyGroupProductsPage : FMUserControlBase
	{
		#region Public Methods and Operators

		public void UpdateData()
		{
			var CompanyGroup = (CompanyGroupClass)this.Session["CompanyGroup"];
		}

		#endregion

		#region Methods

		protected ListItemCollection EnumerateAdditiveProfiles()
		{
			var AdditiveProfileItems = new ListItemCollection();

			try
			{
				AdditiveProfileCollectionClass AdditiveProfileCollection = 
					FMChannelHelper.MakeCall<IAdditiveProfiles, AdditiveProfileCollectionClass>(
																	 x =>
																	 x.Enumerate(this.Security)
																);


				AdditiveProfileItems.Add(new ListItem("", Guid.Empty.ToString()));

				foreach (AdditiveProfileClass AdditiveProfile in AdditiveProfileCollection)
				{
					var NewAdditiveProfileItem = new ListItem(AdditiveProfile.ID, AdditiveProfile.IdentityGuid.ToString());
					foreach (ListItem ExistingAdditiveProfileItem in AdditiveProfileItems)
					{
						if (ExistingAdditiveProfileItem.Text.CompareTo(NewAdditiveProfileItem.Text) > 0)
						{
							int Index = AdditiveProfileItems.IndexOf(ExistingAdditiveProfileItem);
							AdditiveProfileItems.Insert(Index, NewAdditiveProfileItem);
							NewAdditiveProfileItem = null;
							break;
						}
					}

					if (NewAdditiveProfileItem != null)
					{
						AdditiveProfileItems.Add(NewAdditiveProfileItem);
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
			return AdditiveProfileItems;
		}

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


			var CompanyGroup = (CompanyGroupClass)this.Session["CompanyGroup"];

			foreach (ProductClass Product in ProductCollection)
			{
				if (Product.ProductType == ProductType.AdditiveProduct)
				{
					continue;
				}

				bool Found = false;
				foreach (ProductMapClass AuthorizedProduct in CompanyGroup.AuthorizedProductCollection)
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

		protected bool HasSpecialInstruction(ProductMapClass AuthorizedProduct)
		{
			if (AuthorizedProduct.SpecialInstructions != string.Empty)
			{
				return true;
			}
			return false;
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
				var CompanyGroup = (CompanyGroupClass)this.Session["CompanyGroup"];

				if (!this.Page.IsPostBack)
				{
					if (!this.Security.HasRight(RIGHT.MODIFY_COMPANY_DATA))
					{
						this.AddProductButton.Enabled = false;
					}

					this.UpdateAuthorizedProductsView();
				}
				else
				{
					if (this.Request.GetQueryOrFormValue("__EVENTTARGET") == "InstructionsButton")
					{
						this.UpdateSpecialInstructionButton();
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void AddProductButton_Command(object sender, CommandEventArgs e)
		{
			var CompanyGroup = (CompanyGroupClass)this.Session["CompanyGroup"];
			var AuthorizedProduct = new ProductMapClass();
			AuthorizedProduct.AssignedToGuid = CompanyGroup.IdentityGuid;
			AuthorizedProduct.Type = PRODUCT_MAP_TYPE.PRODUCT_COMPANY_GROUP_MAP;
			CompanyGroup.AuthorizedProductCollection.Add(AuthorizedProduct);
			this.AuthorizedProductsDataGrid.CurrentPageIndex = (CompanyGroup.AuthorizedProductCollection.Count - 1)
			                                                   / this.AuthorizedProductsDataGrid.PageSize;
			this.AuthorizedProductsDataGrid.EditItemIndex = (CompanyGroup.AuthorizedProductCollection.Count - 1)
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
				CompanyGroup.AuthorizedProductCollection.RemoveAt(CompanyGroup.AuthorizedProductCollection.Count - 1);

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
				var CompanyGroup = (CompanyGroupClass)this.Session["CompanyGroup"];
				ProductMapClass AuthorizedProduct = CompanyGroup.AuthorizedProductCollection[Convert.ToInt32(IndexLabel.Text)];

				if (AuthorizedProduct.AssignedGuid == Guid.Empty)
				{
					CompanyGroup.AuthorizedProductCollection.RemoveAt(Convert.ToInt32(IndexLabel.Text));

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
				var CompanyGroup = (CompanyGroupClass)this.Session["CompanyGroup"];

				if (this.AuthorizedProductsDataGrid.EditItemIndex == e.Item.ItemIndex)
				{
					this.AuthorizedProductsDataGrid.EditItemIndex = -1;
					this.EnableControls(true);
				}

				else if (this.AuthorizedProductsDataGrid.EditItemIndex > e.Item.ItemIndex)
				{
					this.AuthorizedProductsDataGrid.EditItemIndex--;
				}

				CompanyGroup.AuthorizedProductCollection.RemoveAt(Convert.ToInt32(IndexLabel.Text));

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
				var ProductsDropDownList = (DropDownList)e.Item.FindControl("ProductsDropDownList");
				if (ProductsDropDownList != null)
				{
					var CompanyGroup = (CompanyGroupClass)this.Session["CompanyGroup"];
					ProductMapClass AuthorizedProduct = CompanyGroup.AuthorizedProductCollection[Convert.ToInt32(IndexLabel.Text)];

					if (AuthorizedProduct.AssignedGuid != Guid.Empty)
					{
						ListItemCollection Items = ProductsDropDownList.Items;
						int Index = Items.IndexOf(Items.FindByValue(AuthorizedProduct.AssignedGuid.ToString()));
						ProductsDropDownList.SelectedIndex = Index;
					}

					var AdditiveProfilesDropDownList = (DropDownList)e.Item.FindControl("AdditiveProfilesDropDownList");
					if (AdditiveProfilesDropDownList != null)
					{
						if (AuthorizedProduct.AdditiveProfileGuid != Guid.Empty)
						{
							ListItemCollection Items = AdditiveProfilesDropDownList.Items;
							int Index = Items.IndexOf(Items.FindByValue(AuthorizedProduct.AdditiveProfileGuid.ToString()));
							AdditiveProfilesDropDownList.SelectedIndex = Index;
						}
					}
				}

				// Inhibit Edits of Products Assigned by way of Company Groups
				if (!this.Security.HasRight(RIGHT.MODIFY_COMPANY_DATA))
				{
					var EditButton = (FMEditLinkButton)e.Item.FindControl("FMEditLinkButton1");
					if (EditButton != null)
					{
						EditButton.Enabled = false;
					}

					var DeleteButton = (FMDeleteLinkButton)e.Item.FindControl("FMDeleteLinkButton1");
					if (DeleteButton != null)
					{
						DeleteButton.Enabled = false;
					}

					var button = (HtmlInputButton)e.Item.FindControl("InstructionsButton");
					if (button != null)
					{
						if (button.Value == this.GetTranslatedText("Add")) //leave the Edit button so the user can read the content of instruction
						{
							button.Disabled = true;
						}
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
				var CompanyGroup = (CompanyGroupClass)this.Session["CompanyGroup"];
				ProductMapClass AuthorizedProduct = CompanyGroup.AuthorizedProductCollection[Convert.ToInt32(IndexLabel.Text)];

				var ProductsDropDownList = (DropDownList)e.Item.FindControl("ProductsDropDownList");
				AuthorizedProduct.AssignedGuid = Guid.Parse(ProductsDropDownList.SelectedValue);
				AuthorizedProduct.AssignedID = ProductsDropDownList.SelectedItem.Text;

				var AdditiveProfilesDropDownList = (DropDownList)e.Item.FindControl("AdditiveProfilesDropDownList");
				AuthorizedProduct.AdditiveProfileGuid = Guid.Parse(AdditiveProfilesDropDownList.SelectedValue);
				AuthorizedProduct.AdditiveProfileID = AdditiveProfilesDropDownList.SelectedItem.Text;

				var ShipToProductID = (TextBox)e.Item.FindControl("ShipToProductIDTextBox");
				AuthorizedProduct.ShipToProductID = ShipToProductID.Text;

				var ShipToProductCode = (TextBox)e.Item.FindControl("ShipToProductCodeTextBox");
				AuthorizedProduct.ShipToProductCode = ShipToProductCode.Text;

				var ShipToLoadRackDisplayText = (TextBox)e.Item.FindControl("ShipToLoadRackDisplayTextTextBox");
				AuthorizedProduct.ShipToLoadRackDisplayText = ShipToLoadRackDisplayText.Text;

				this.AuthorizedProductsDataGrid.EditItemIndex = -1;

				// Enable the add button.
				this.EnableControls(true);

				this.UpdateAuthorizedProductsView();
			}
		}

		private void EnableControls(bool enable)
		{
			if (this.Security.HasRight(RIGHT.MODIFY_COMPANY_DATA))
			{
				this.AddProductButton.Enabled = enable;
			}

			// Call the main form to disable buttons and tabs.
			var companyGroupForm = (CompanyGroupForm)this.Page;
			companyGroupForm.EnableControls(enable);
		}

		private ICollection EnumerateAuthorizedProducts()
		{
			var CompanyGroup = (CompanyGroupClass)this.Session["CompanyGroup"];

			var AuthorizedProductsDataTable = new DataTable();
			DataRow AuthorizedProductDataRow;

			AuthorizedProductsDataTable.Columns.Add("Index", typeof(Int32));
			AuthorizedProductsDataTable.Columns.Add("ProductID", typeof(string));
			AuthorizedProductsDataTable.Columns.Add("AdditiveProfileID", typeof(string));
			AuthorizedProductsDataTable.Columns.Add("ShipToProductID", typeof(string));
			AuthorizedProductsDataTable.Columns.Add("ShipToProductCode", typeof(string));
			AuthorizedProductsDataTable.Columns.Add("ShipToLoadRackDisplayText", typeof(string));
			AuthorizedProductsDataTable.Columns.Add("SpecialInstructionsText", typeof(string));
			AuthorizedProductsDataTable.Columns.Add("SpecialInstructionsClick", typeof(string));

			string AddText = this.GetTranslatedText("Add");
			string EditText = this.GetTranslatedText("Edit");

			int Item = 0;
			foreach (ProductMapClass AuthorizedProduct in CompanyGroup.AuthorizedProductCollection)
			{
				AuthorizedProductDataRow = AuthorizedProductsDataTable.NewRow();

				AuthorizedProductDataRow["Index"] = Item;
				AuthorizedProductDataRow["ProductID"] = AuthorizedProduct.AssignedID;
				AuthorizedProductDataRow["AdditiveProfileID"] = AuthorizedProduct.AdditiveProfileID;
				AuthorizedProductDataRow["ShipToProductID"] = AuthorizedProduct.ShipToProductID;
				AuthorizedProductDataRow["ShipToProductCode"] = AuthorizedProduct.ShipToProductCode;
				AuthorizedProductDataRow["ShipToLoadRackDisplayText"] = AuthorizedProduct.ShipToLoadRackDisplayText;
				AuthorizedProductDataRow["SpecialInstructionsText"] = this.HasSpecialInstruction(AuthorizedProduct)
					                                                      ? EditText
					                                                      : AddText;
				AuthorizedProductDataRow["SpecialInstructionsClick"] = "InstructionsButton_Click(" + Item + ")";

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
			this.AuthorizedProductsDataGrid.EditCommand += this.AuthorizedProductsDataGrid_EditCommand;
			this.AuthorizedProductsDataGrid.PageIndexChanged += this.AuthorizedProductsDataGrid_PageIndexChanged;
			this.AuthorizedProductsDataGrid.CancelCommand += this.AuthorizedProductsDataGrid_CancelCommand;
			this.AuthorizedProductsDataGrid.UpdateCommand += this.AuthorizedProductsDataGrid_UpdateCommand;
			this.AuthorizedProductsDataGrid.DeleteCommand += this.AuthorizedProductsDataGrid_DeleteCommand;
			this.AuthorizedProductsDataGrid.ItemDataBound += this.AuthorizedProductsDataGrid_ItemDataBound;
			this.AddProductButton.Command += this.AddProductButton_Command;
		}

		private void UpdateAuthorizedProductsView()
		{
			this.AuthorizedProductsDataGrid.DataSource = this.EnumerateAuthorizedProducts();
			this.AuthorizedProductsDataGrid.DataBind();
		}

		private void UpdateSpecialInstructionButton()
		{
			var CompanyGroup = (CompanyGroupClass)this.Session["CompanyGroup"];

			string AddText = this.GetTranslatedText("Add");
			string EditText = this.GetTranslatedText("Edit");

			foreach (DataGridItem GridItem in this.AuthorizedProductsDataGrid.Items)
			{
				var IndexLabel = (Label)GridItem.FindControl("IndexLabel");

				if (IndexLabel != null)
				{
					ProductMapClass ProductMap = CompanyGroup.AuthorizedProductCollection[Convert.ToInt32(IndexLabel.Text)];

					if (ProductMap != null)
					{
						var Button = (HtmlInputButton)GridItem.FindControl("InstructionsButton");

						if (Button != null)
						{
							Button.Value = this.HasSpecialInstruction(ProductMap) ? EditText : AddText;
						}
					}
				}
			}
		}

		#endregion
	}
}