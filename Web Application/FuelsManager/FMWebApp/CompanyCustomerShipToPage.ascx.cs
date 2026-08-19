// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompanyCustomerShipToPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the CompanyCustomerShipToPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;

    using FMControls;

    using FMCore;

    /// <summary>
	///    Summary description for ShipToShipToPage.
	/// </summary>
	public partial class CompanyCustomerShipToPage : CompanyPageBase
	{
		#region Constants and Fields

		protected TextBox CompaniesTextBox;

		#endregion

		#region Properties

		private string JavascriptStartup
		{
			get
			{
				string script = @"
				<script type='text/javascript'>
				<!--
					// Set Assign and Unassign Button values according to Data Dictionary
					var AssignButton=document.getElementById('CompanyCustomerShipToPage_AssignButton');
					if(AssignButton != null)
						AssignButton.value='" + ((FMFormBase)this.Page).GetTranslatedText("Assign") + @"';
					var UnassignButton=document.getElementById('CompanyCustomerShipToPage_UnassignButton');
					if(UnassignButton != null)
						UnassignButton.value='" + ((FMFormBase)this.Page).GetTranslatedText("Unassign") + @"';
				//-->
				</script>
				";
				return script;
			}
		}

		#endregion

		#region Public Methods and Operators

		public void AddSelf()
		{
            CompanyMapClass companyMap = CompanyMapClass.CreateCompanyMap(COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP);
		    companyMap.AssignedID = this.Company.ID;
		    companyMap.AssignedGuid = this.Company.IdentityGuid;
		    companyMap.AssignedName = this.Company.Name;
		    companyMap.AssignedAddress = this.Company.Address1;
		    companyMap.AssignedCity = this.Company.City;
		    companyMap.AssignedState = this.Company.State;

			this.Company.AuthorizedCarrierCollection.Add(companyMap);

			this.UpdateAuthorizedCarriersView();
		}

		public void RemoveSelf()
		{
			int index = 0;
			foreach (CompanyMapClass companyMap in this.Company.AuthorizedCarrierCollection)
			{
				if (companyMap.AssignedID == this.Company.ID)
				{
					this.Company.AuthorizedCarrierCollection.Remove(index);
					break;
				}

				index++;
			}

			this.UpdateAuthorizedCarriersView();
		}

		public void SetInstructionValue(int itemIndex, int instructionIndex)
		{
		}

		public void UpdateData()
		{
			if (!this.Company.HasRole(COMPANY_ROLE.CUSTOMER_SHIPTO))
			{
				return;
			}

			if (this.TypeDropDownList.SelectedIndex != -1)
			{
				this.Company.CustomerShipToTypeApplicationStringGuid = Guid.Parse(this.TypeDropDownList.SelectedValue);
				this.Company.CustomerShipToTypeID = this.TypeDropDownList.SelectedItem.Text;
			}
			else
			{
				this.Company.CustomerShipToTypeApplicationStringGuid = Guid.Empty;
				this.Company.CustomerShipToTypeID = "{None}";
			}

			this.Company.PurchaseOrderRequired = this.PurchaseOrderRequiredCheckBox.Checked;
			this.Company.DisableShipToAllocationsCheck = this.DisableShipToAllocationsCheckCheckBox.Checked;
			this.Company.DisableBillToAllocationsCheck = this.DisableBillToAllocationsCheckCheckBox.Checked;
			this.Company.DisableShipperAllocationsCheck = this.DisableShipperAllocationsCheckCheckBox.Checked;
			this.Company.DisableOwnerAllocationsCheck = this.DisableOwnerAllocationsCheckCheckBox.Checked;
		}

		#endregion

		#region Methods

		/// <summary>
		///    Handles the TextChanged event of the AssignCompaniesTextBox control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">
		///    The <see cref="EventArgs" /> instance containing the event data.
		/// </param>
		protected void AssignCompaniesTextBoxTextChanged(object sender, EventArgs e)
		{
			try
			{
				string[] companyIDs = this.AssignCompaniesTextBox.Text.Split('|');
				this.AssignCompaniesTextBox.Text = string.Empty;

				// First remove {All} from the target collection
				this.RemoveAllCompanyMap(this.Company);

				foreach (string companyID in companyIDs)
				{
					if (companyID == "|")
					{
						continue;
					}

					CompanyMapClass companyMap = CompanyMapClass.CreateCompanyMap(COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP);

					if (companyID == "{All}")
					{
						companyMap.AssignedID = "{All}";
						this.Company?.AuthorizedCarrierCollection.Clear();
						this.Company?.AuthorizedCarrierCollection.Add(companyMap);

						// Stop processing since we only want to have the {All} option in the list
						break;
					}

					CompanyClass carrier = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
																	companies => companies.Get(this.Security, companies.GetIdentityGuid(this.Security, companyID), false));

				    companyMap.AssignedID = carrier.ID;
				    companyMap.AssignedGuid = carrier.IdentityGuid;
				    companyMap.AssignedName = carrier.Name;
				    companyMap.AssignedAddress = carrier.Address1;
				    companyMap.AssignedCity = carrier.City;
				    companyMap.AssignedState = carrier.State;

					this.Company.AuthorizedCarrierCollection.Add(companyMap);

					if (carrier.IdentityGuid == this.Company.IdentityGuid)
					{
						var carrierPage =
							(CompanyCarrierPage)
							this.Page.FindControl("tcCompanyTabs").FindControl("tpCarrierPage").FindControl("CompanyCarrierPage");

						carrierPage.AddSelf();
					}
				}

				this.Company.AuthorizedCarrierCollection.Sort(COMPANY_MAP_SORT_CRITERIA.ASSIGNED);

				this.UpdateAuthorizedCarriersView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected ListItemCollection EnumerateAdditiveProfiles()
		{
			var additiveProfileItems = new ListItemCollection();

			try
			{
				AdditiveProfileCollectionClass additiveProfileCollection =
					FMChannelHelper.MakeCall<IAdditiveProfiles, AdditiveProfileCollectionClass>(x => x.Enumerate(this.Security));

				additiveProfileItems.Add(new ListItem(string.Empty, Guid.Empty.ToString()));

				foreach (AdditiveProfileClass additiveProfile in additiveProfileCollection)
				{
					var newAdditiveProfileItem = new ListItem(additiveProfile.ID, additiveProfile.IdentityGuid.ToString());
					foreach (ListItem existingAdditiveProfileItem in additiveProfileItems)
					{
						if (String.Compare(existingAdditiveProfileItem.Text, newAdditiveProfileItem.Text, StringComparison.Ordinal) > 0)
						{
							int index = additiveProfileItems.IndexOf(existingAdditiveProfileItem);
							additiveProfileItems.Insert(index, newAdditiveProfileItem);
							newAdditiveProfileItem = null;
							break;
						}
					}

					if (newAdditiveProfileItem != null)
					{
						additiveProfileItems.Add(newAdditiveProfileItem);
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}

			return additiveProfileItems;
		}

		protected ListItemCollection EnumerateProducts()
		{
			var productItems = new ListItemCollection();

			var authorizedProductsDataView = (DataView)this.AuthorizedProductsDataGrid.DataSource;

			int item = this.AuthorizedProductsDataGrid.EditItemIndex + this.AuthorizedProductsDataGrid.CurrentPageIndex * this.AuthorizedProductsDataGrid.PageSize;

			ProductCollectionClass productCollection = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(x => x.Enumerate(this.Security));

			foreach (ProductClass product in productCollection)
			{
				if (product.ProductType == ProductType.AdditiveProduct)
				{
					continue;
				}

				bool found = false;
				foreach (ProductMapClass authorizedProduct in this.Company.AuthorizedProductCollection)
				{
					if (product.ID != (string)authorizedProductsDataView[item][1]
					    && product.IdentityGuid == authorizedProduct.AssignedGuid)
					{
						found = true;
						break;
					}
				}

				if (found)
				{
					continue;
				}

				var newProductItem = new ListItem(product.ID, product.IdentityGuid.ToString());
				foreach (ListItem existingProductItem in productItems)
				{
					if (String.Compare(existingProductItem.Text, newProductItem.Text, StringComparison.Ordinal) > 0)
					{
						int index = productItems.IndexOf(existingProductItem);
						productItems.Insert(index, newProductItem);
						newProductItem = null;
						break;
					}
				}

				if (newProductItem != null)
				{
					productItems.Add(newProductItem);
				}
			}

			if (productItems.Count == 0)
			{
				throw new Exception("No Products Available");
			}

			return productItems;
		}

		protected bool HasSpecialInstruction(ProductMapClass authorizedProduct)
		{
			if (authorizedProduct.SpecialInstructions != string.Empty)
			{
				return true;
			}
			return false;
		}

		protected override void OnInit(EventArgs e)
		{
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			this.InitializeComponent();

			this.AuthorizedProductsDataGrid.EditCommand += this.AuthorizedProductsDataGridEditCommand;
			this.AuthorizedProductsDataGrid.PageIndexChanged += this.AuthorizedProductsDataGridPageIndexChanged;
			this.AuthorizedProductsDataGrid.CancelCommand += this.AuthorizedProductsDataGridCancelCommand;
			this.AuthorizedProductsDataGrid.UpdateCommand += this.AuthorizedProductsDataGridUpdateCommand;
			this.AuthorizedProductsDataGrid.DeleteCommand += this.AuthorizedProductsDataGridDeleteCommand;
			this.AuthorizedProductsDataGrid.ItemDataBound += this.AuthorizedProductsDataGridItemDataBound;

			base.OnInit(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				if (!this.Company.HasRole(COMPANY_ROLE.CUSTOMER_SHIPTO))
				{
					return;
				}

				// Checks the user rights and sets the controls.
				this.SetUserRights();

				if (!this.Page.IsPostBack)
				{
					// TypeDropDownList

					ApplicationStringCollectionClass types =
						FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
							x => x.EnumerateByType(this.Security, STRING_TYPE.COMPANY_TYPE));

				    // ReSharper disable once ForCanBeConvertedToForeach
					for (int iItem = 0; iItem < types.Count; iItem++)
					{
						ApplicationStringClass type = types[iItem];

						var newTypeItem = new ListItem(type.ID, type.IdentityGuid.ToString());

						foreach (ListItem existingTypeItem in this.TypeDropDownList.Items)
						{
							if (String.Compare(existingTypeItem.Text, newTypeItem.Text, StringComparison.Ordinal) > 0)
							{
								int index = this.TypeDropDownList.Items.IndexOf(existingTypeItem);
								this.TypeDropDownList.Items.Insert(index, newTypeItem);
								if (type.IdentityGuid == this.Company.CustomerShipToTypeApplicationStringGuid)
								{
									this.TypeDropDownList.SelectedIndex = index;
								}

								newTypeItem = null;
								break;
							}
						}

						if (newTypeItem != null)
						{
							this.TypeDropDownList.Items.Add(newTypeItem);
							if (type.IdentityGuid == this.Company.CustomerShipToTypeApplicationStringGuid)
							{
								this.TypeDropDownList.SelectedIndex = this.TypeDropDownList.Items.Count - 1;
							}
						}
					}

                    this.TypeDropDownList.Items.Insert(0, new ListItem(this.GetTranslatedText("{None}"), Guid.Empty.ToString()));

                    this.PurchaseOrderRequiredCheckBox.Checked = this.Company.PurchaseOrderRequired;
					this.DisableShipToAllocationsCheckCheckBox.Checked = this.Company.DisableShipToAllocationsCheck;
					this.DisableBillToAllocationsCheckCheckBox.Checked = this.Company.DisableBillToAllocationsCheck;
					this.DisableShipperAllocationsCheckCheckBox.Checked = this.Company.DisableShipperAllocationsCheck;
					this.DisableOwnerAllocationsCheckCheckBox.Checked = this.Company.DisableOwnerAllocationsCheck;

					this.UpdateAuthorizedProductsView();

					this.UpdateAuthorizedCarriersView();
				    this.SetFieldAccessibilityForChildRecordVersion();
				}
				else
				{
					if (this.Request.GetQueryOrFormValue("__EVENTTARGET") == "InstructionsButton")
					{
						this.UpdateSpecialInstructionButton();
					}
				}

				this.Page.ClientScript.RegisterStartupScript(
					this.GetType(), "CompanyCustomerShipToPageScriptBlock", this.JavascriptStartup);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void UnassignCompaniesTextBoxTextChanged(object sender, EventArgs e)
		{
			try
			{
				string[] companyIDs = this.UnassignCompaniesTextBox.Text.Split('|');
				this.UnassignCompaniesTextBox.Text = string.Empty;

				foreach (string companyID in companyIDs)
				{
					if (companyID == "|")
					{
						continue;
					}

					int index = 0;
					foreach (CompanyMapClass companyMap in this.Company.AuthorizedCarrierCollection)
					{
						if (companyMap.AssignedID == companyID)
						{
							this.Company.AuthorizedCarrierCollection.Remove(index);

							if (companyMap.AssignedToID == this.Company.ID)
							{
								var carrierPage =
									(CompanyCarrierPage)
									this.Page.FindControl("tcCompanyTabs").FindControl("tpCarrierPage").FindControl("CompanyCarrierPage");
								carrierPage.RemoveSelf();
							}

							break;
						}

						index++;
					}
				}

				this.UpdateAuthorizedCarriersView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void AddProductButtonCommand(object sender, CommandEventArgs e)
		{
			var authorizedProduct = new ProductMapClass
			                        {
				                        AssignedToGuid = this.Company.IdentityGuid,
				                        Type = PRODUCT_MAP_TYPE.PRODUCT_COMPANY_MAP
			                        };
			this.Company.AuthorizedProductCollection.Add(authorizedProduct);
			this.AuthorizedProductsDataGrid.CurrentPageIndex = (this.Company.AuthorizedProductCollection.Count - 1)
			                                                   / this.AuthorizedProductsDataGrid.PageSize;
			this.AuthorizedProductsDataGrid.EditItemIndex = (this.Company.AuthorizedProductCollection.Count - 1)
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
				this.Company.AuthorizedProductCollection.RemoveAt(this.Company.AuthorizedProductCollection.Count - 1);

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

		private void AuthorizedCarriersDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemIndex != -1)
			{
				CompanyMapClass authorizedCarrier = this.Company.AuthorizedCarrierCollection[e.Item.DataSetIndex];
				var idLabel = (Label)e.Item.FindControl("IDLabel");
				idLabel.Text = authorizedCarrier.AssignedID;
				idLabel.ToolTip = authorizedCarrier.AssignedToolTip;
			}
		}

		private void AuthorizedCarriersDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.AuthorizedCarriersDataGrid.EditItemIndex > -1)
			{
				return;
			}

			this.AuthorizedCarriersDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateAuthorizedCarriersView();
		}

		private void AuthorizedProductsDataGridCancelCommand(object source, DataGridCommandEventArgs e)
		{
			var indexLabel = (Label)e.Item.FindControl("IndexLabel");

			if (indexLabel != null)
			{
				ProductMapClass authorizedProduct = this.Company.AuthorizedProductCollection[Convert.ToInt32(indexLabel.Text)];

				if (authorizedProduct.AssignedGuid == Guid.Empty)
				{
					this.Company.AuthorizedProductCollection.RemoveAt(Convert.ToInt32(indexLabel.Text));

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

		private void AuthorizedProductsDataGridDeleteCommand(object source, DataGridCommandEventArgs e)
		{
			var indexLabel = (Label)e.Item.FindControl("IndexLabel");
			if (indexLabel != null)
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

				this.Company.AuthorizedProductCollection.RemoveAt(Convert.ToInt32(indexLabel.Text));

				if (this.AuthorizedProductsDataGrid.Items.Count == 1 && this.AuthorizedProductsDataGrid.CurrentPageIndex > 0)
				{
					this.AuthorizedProductsDataGrid.CurrentPageIndex--;
				}

				this.UpdateAuthorizedProductsView();
			}
		}

		private void AuthorizedProductsDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
			this.AuthorizedProductsDataGrid.EditItemIndex = e.Item.ItemIndex;

			// Disable the add button.
			this.EnableControls(false);

			this.UpdateAuthorizedProductsView();
		}

		private void AuthorizedProductsDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			var indexLabel = (Label)e.Item.FindControl("IndexLabel");
			if (indexLabel != null)
			{
				ProductMapClass authorizedProduct = this.Company.AuthorizedProductCollection[Convert.ToInt32(indexLabel.Text)];

				var productsDropDownList = (DropDownList)e.Item.FindControl("ProductsDropDownList");
				if (productsDropDownList != null)
				{
					if (authorizedProduct.AssignedGuid != Guid.Empty)
					{
						ListItemCollection items = productsDropDownList.Items;
						int index = items.IndexOf(items.FindByValue(authorizedProduct.AssignedGuid.ToString()));
						productsDropDownList.SelectedIndex = index;
					}

					var additiveProfilesDropDownList = (DropDownList)e.Item.FindControl("AdditiveProfilesDropDownList");
					if (additiveProfilesDropDownList != null)
					{
						if (authorizedProduct.AdditiveProfileGuid != Guid.Empty)
						{
							ListItemCollection items = additiveProfilesDropDownList.Items;
							int index = items.IndexOf(items.FindByValue(authorizedProduct.AdditiveProfileGuid.ToString()));
							additiveProfilesDropDownList.SelectedIndex = index;
						}
					}
				}
                var editButton = (FMEditLinkButton)e.Item.FindControl("EditButton");
                var deleteButton = (FMDeleteLinkButton)e.Item.FindControl("DeleteButton");
                var instructionsButton = (HtmlInputButton)e.Item.FindControl("InstructionsButton");
				// Inhibit Edits of Products Assigned by way of Company Groups
				if ((authorizedProduct.Type == PRODUCT_MAP_TYPE.PRODUCT_COMPANY_GROUP_MAP)
				    || (this.Security.HasRight(RIGHT.MODIFY_COMPANY_DATA) == false))
				{
					if (editButton != null)
					{
						editButton.Enabled = false;
					}

					if (deleteButton != null)
					{
						deleteButton.Enabled = false;
					}

                    if (instructionsButton != null)
					{
                        if (instructionsButton.Value == this.GetTranslatedText("Add"))
						{
							// leave the Edit button so the user can read the content of instruction
                            instructionsButton.Disabled = true;
						}
					}
				}

                //Set the availability of the Grid editing buttons for child record versions
                bool currentSiteOwnsRecordVersion = (this.Company.SiteGuid == this.Security.SiteGuid);
				if ((this.Company.IdentityGuid.Equals(Guid.Empty))
				    || (currentSiteOwnsRecordVersion && this.Company.IdentityGuid.Equals(this.Company.MasterRecordGuid)))
				{
					return;
				}

                if ((this.VersionSpecificFields == null) || !this.VersionSpecificFields.Contains("ShipToAuthorizedProducts"))
                {
	                if (editButton != null)
	                {
		                editButton.Enabled = false;
	                }
	                if (deleteButton != null)
	                {
		                deleteButton.Enabled = false;
	                }
	                if (instructionsButton != null)
	                {
		                instructionsButton.Disabled = true;
	                }
                }
			}
		}

		private void AuthorizedProductsDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.AuthorizedProductsDataGrid.EditItemIndex > -1)
			{
				return;
			}

			this.AuthorizedProductsDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateAuthorizedProductsView();
		}

		private void AuthorizedProductsDataGridUpdateCommand(object source, DataGridCommandEventArgs e)
		{
			var indexLabel = (Label)e.Item.FindControl("IndexLabel");
			if (indexLabel != null)
			{
				ProductMapClass authorizedProduct = this.Company.AuthorizedProductCollection[Convert.ToInt32(indexLabel.Text)];

				var productsDropDownList = (DropDownList)e.Item.FindControl("ProductsDropDownList");
				authorizedProduct.AssignedGuid = Guid.Parse(productsDropDownList.SelectedValue);
				authorizedProduct.AssignedID = productsDropDownList.SelectedItem.Text;

				var additiveProfilesDropDownList = (DropDownList)e.Item.FindControl("AdditiveProfilesDropDownList");
				authorizedProduct.AdditiveProfileGuid = Guid.Parse(additiveProfilesDropDownList.SelectedValue);
				authorizedProduct.AdditiveProfileID = additiveProfilesDropDownList.SelectedItem.Text;

				var shipToProductID = (TextBox)e.Item.FindControl("ShipToProductIDTextBox");
				authorizedProduct.ShipToProductID = shipToProductID.Text;

				var shipToProductCode = (TextBox)e.Item.FindControl("ShipToProductCodeTextBox");
				authorizedProduct.ShipToProductCode = shipToProductCode.Text;

				var shipToLoadRackDisplayText = (TextBox)e.Item.FindControl("ShipToLoadRackDisplayTextTextBox");
				authorizedProduct.ShipToLoadRackDisplayText = shipToLoadRackDisplayText.Text;

				this.AuthorizedProductsDataGrid.EditItemIndex = -1;

				// Enable the add button.
				this.EnableControls(true);

				this.UpdateAuthorizedProductsView();
			}
		}

		/// <summary>
		///    This method will enable and disable controls.
		/// </summary>
		/// <param name="enable">
		/// </param>
		private void EnableControls(bool enable)
		{
			this.AddProductButton.Enabled = enable;
			this.TypeDropDownList.Enabled = enable;
			this.PurchaseOrderRequiredCheckBox.Enabled = enable;

			// Call the main form to disable buttons and tabs.
			var companyForm = (CompanyForm)this.Page;
			companyForm.EnableControls(enable);
            this.SetFieldAccessibilityForChildRecordVersion();
		}

		private ICollection EnumerateAuthorizedProducts()
		{
			var authorizedProductsDataTable = new DataTable();

		    authorizedProductsDataTable.Columns.Add("Index", typeof(int));
			authorizedProductsDataTable.Columns.Add("ProductID", typeof(string));
			authorizedProductsDataTable.Columns.Add("AdditiveProfileID", typeof(string));
			authorizedProductsDataTable.Columns.Add("ShipToProductID", typeof(string));
			authorizedProductsDataTable.Columns.Add("ShipToProductCode", typeof(string));
			authorizedProductsDataTable.Columns.Add("ShipToLoadRackDisplayText", typeof(string));
			authorizedProductsDataTable.Columns.Add("SpecialInstructionsText", typeof(string));
			authorizedProductsDataTable.Columns.Add("SpecialInstructionsClick", typeof(string));

			string addText = this.GetTranslatedText("Add");
			string editText = this.GetTranslatedText("Edit");

			int item = 0;
			foreach (ProductMapClass authorizedProduct in this.Company.AuthorizedProductCollection)
			{
				DataRow authorizedProductDataRow = authorizedProductsDataTable.NewRow();

				authorizedProductDataRow["Index"] = item;
				authorizedProductDataRow["ProductID"] = authorizedProduct.AssignedID;
				authorizedProductDataRow["AdditiveProfileID"] = authorizedProduct.AdditiveProfileID;
				authorizedProductDataRow["ShipToProductID"] = authorizedProduct.ShipToProductID;
				authorizedProductDataRow["ShipToProductCode"] = authorizedProduct.ShipToProductCode;
				authorizedProductDataRow["ShipToLoadRackDisplayText"] = authorizedProduct.ShipToLoadRackDisplayText;
				authorizedProductDataRow["SpecialInstructionsText"] = this.HasSpecialInstruction(authorizedProduct)
					                                                      ? editText
					                                                      : addText;
				authorizedProductDataRow["SpecialInstructionsClick"] = "InstructionsButton_Click(" + item + ")";

				// Sort the DataTable by ProductID except for an Added entry
				// which has a null ProductID that messes up the sort order
				// relative to the EditItemIndex
				if (authorizedProduct.AssignedGuid != Guid.Empty)
				{
					int iRow = 0;
					foreach (DataRow row in authorizedProductsDataTable.Rows)
					{
						if (String.Compare(((string)row["ProductID"]), (string)authorizedProductDataRow["ProductID"], StringComparison.Ordinal) > 0)
						{
							authorizedProductsDataTable.Rows.InsertAt(authorizedProductDataRow, iRow);
							authorizedProductDataRow = null;
							break;
						}

						iRow++;
					}
				}

				if (authorizedProductDataRow != null)
				{
					authorizedProductsDataTable.Rows.Add(authorizedProductDataRow);
				}

				item++;
			}

			var authorizedProductsDataView = new DataView(authorizedProductsDataTable);
			return authorizedProductsDataView;
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AddProductButton.Command += this.AddProductButtonCommand;
			this.AuthorizedCarriersDataGrid.PageIndexChanged += this.AuthorizedCarriersDataGridPageIndexChanged;
			this.AuthorizedCarriersDataGrid.ItemDataBound += this.AuthorizedCarriersDataGridItemDataBound;
		}

		/// <summary>
		///    This method checks the user rights and sets the controls to be disabled.
		/// </summary>
		private void SetUserRights()
		{
			if (this.Security.HasRight(RIGHT.MODIFY_COMPANY_DATA) == false)
			{
				this.AddProductButton.Enabled = false;
				this.TypeDropDownList.Enabled = false;
				this.PurchaseOrderRequiredCheckBox.Enabled = false;
				this.AssignCompaniesTextBox.Enabled = false;
				this.UnassignCompaniesTextBox.Enabled = false;
			}
		}

		private void UpdateAuthorizedCarriersView()
		{
			this.AuthorizedCarriersDataGrid.DataSource = this.Company.AuthorizedCarrierCollection;

		    var companyMapClasses = (List<CompanyMapClass>)this.AuthorizedCarriersDataGrid.DataSource;
		    if (companyMapClasses != null)
		    {
		        int count = companyMapClasses.Count;
		        if ((count - 1) / this.AuthorizedCarriersDataGrid.PageSize < this.AuthorizedCarriersDataGrid.CurrentPageIndex)
		        {
		            this.AuthorizedCarriersDataGrid.CurrentPageIndex = (count - 1) / this.AuthorizedCarriersDataGrid.PageSize;
		        }
		    }

		    this.AuthorizedCarriersDataGrid.DataBind();
		}

		private void UpdateAuthorizedProductsView()
		{
			this.AuthorizedProductsDataGrid.DataSource = this.EnumerateAuthorizedProducts();
			this.AuthorizedProductsDataGrid.DataBind();
		}

		private void UpdateSpecialInstructionButton()
		{
			string addText = this.GetTranslatedText("Add");
			string editText = this.GetTranslatedText("Edit");

			foreach (DataGridItem gridItem in this.AuthorizedProductsDataGrid.Items)
			{
				var indexLabel = (Label)gridItem.FindControl("IndexLabel");

				if (indexLabel != null)
				{
					ProductMapClass productMap = this.Company.AuthorizedProductCollection[Convert.ToInt32(indexLabel.Text)];

					if (productMap != null)
					{
						var button = (HtmlInputButton)gridItem.FindControl("InstructionsButton");

						if (button != null)
						{
							button.Value = this.HasSpecialInstruction(productMap) ? editText : addText;
						}
					}
				}
			}
		}

        private void SetFieldAccessibilityForChildRecordVersion()
        {
            bool currentSiteOwnsRecordVersion = (this.Company.SiteGuid == this.Security.SiteGuid);
            if ((this.Company.IdentityGuid.Equals(Guid.Empty)
                 || (currentSiteOwnsRecordVersion && this.Company.IdentityGuid.Equals(this.Company.MasterRecordGuid))
                 || (this.VersionSpecificFields == null)))
            {
                return;
            }

            if ((!this.CompanyCustomerShipToPage_AssignButton.Disabled) && this.VersionSpecificFields.Contains("AuthorizedCarriers"))
                this.CompanyCustomerShipToPage_AssignButton.Disabled = false;
            else
                this.CompanyCustomerShipToPage_AssignButton.Disabled = true;
            if ((!this.CompanyCustomerShipToPage_UnassignButton.Disabled) && this.VersionSpecificFields.Contains("AuthorizedCarriers"))
                this.CompanyCustomerShipToPage_UnassignButton.Disabled = false;
            else
                this.CompanyCustomerShipToPage_UnassignButton.Disabled = true;
            
            this.AddProductButton.Enabled = (this.AddProductButton.Enabled 
                                               && this.VersionSpecificFields.Contains("ShipToAuthorizedProducts"));
            this.TypeDropDownList.Enabled = (this.TypeDropDownList.Enabled 
                                        && this.VersionSpecificFields.Contains("CustomerShipToTypeApplicationStringGuid"));
            this.PurchaseOrderRequiredCheckBox.Enabled = (this.PurchaseOrderRequiredCheckBox.Enabled 
                                         && this.VersionSpecificFields.Contains("PurchaseOrderRequired"));
            this.DisableShipToAllocationsCheckCheckBox.Enabled = (this.DisableShipToAllocationsCheckCheckBox.Enabled 
                                        && this.VersionSpecificFields.Contains("DisableShipToAllocationsCheck"));
            this.DisableBillToAllocationsCheckCheckBox.Enabled = (this.DisableBillToAllocationsCheckCheckBox.Enabled 
                                                && this.VersionSpecificFields.Contains("DisableBillToAllocationsCheck"));
            this.DisableShipperAllocationsCheckCheckBox.Enabled = (this.DisableShipperAllocationsCheckCheckBox.Enabled 
                                                          && this.VersionSpecificFields.Contains("DisableShipperAllocationsCheck"));
            this.DisableOwnerAllocationsCheckCheckBox.Enabled = (this.DisableOwnerAllocationsCheckCheckBox.Enabled 
                                                   && this.VersionSpecificFields.Contains("DisableOwnerAllocationsCheck"));            
        }


		private void RemoveAllCompanyMap(CompanyClass company)
		{
			for (int nLoop = 0; nLoop < company.AuthorizedCarrierCollection.Count; ++nLoop)
			{
				if (company.AuthorizedCarrierCollection[nLoop].AssignedID == "{All}")
				{
					company.AuthorizedCarrierCollection.Clear();
					break;
				}
			}
		}

		#endregion
	}
}