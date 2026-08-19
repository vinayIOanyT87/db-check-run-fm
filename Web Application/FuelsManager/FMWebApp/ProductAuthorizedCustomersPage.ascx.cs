// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProductAuthorizedCustomersPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ProductAuthorizedCustomersPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
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
	///    Summary description for ProductAuthorizedCustomersPage.
	/// </summary>
	public partial class ProductAuthorizedCustomersPage : ProductPageBase
	{
		#region Enums
		public enum ASSIGNMENT_TYPE
		{
			COMPANIES = 0,
			COMPANY_GROUPS = 1,
		};
		#endregion

		#region Public Methods and Operators
		public void UpdateData()
		{
		}
		#endregion

		#region Methods
		/// <summary>
		///    This method enables/disables controls.
		/// </summary>
		/// <param name="enable"></param>
		protected void EnableControls(bool enable)
		{
			this.AddButton.Enabled = enable;
			this.TypeDropDownList.Enabled = enable;

			// Call the main form to disable buttons and tabs.
			var productForm = (ProductForm)this.Page;
			productForm.EnableControls(enable);
		}

		protected ListItemCollection EnumerateAdditiveProfiles()
		{
			AdditiveProfileCollectionClass additiveProfileCollection = 
				FMChannelHelper.MakeCall<IAdditiveProfiles, AdditiveProfileCollectionClass>(
																	 x =>
																	 x.Enumerate(this.Security)
																);

			var additiveProfileItems = new ListItemCollection { new ListItem(string.Empty, Guid.Empty.ToString()) };

			for (int iItem = 0; iItem < additiveProfileCollection.Count; iItem++)
			{
				AdditiveProfileClass additiveProfile = additiveProfileCollection.Items(iItem);
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

			return additiveProfileItems;
		}

		protected ListItemCollection EnumerateCustomers()
		{
			var companyItems = new ListItemCollection();
			var authorizedCustomersDataView = (DataView)this.AuthorizedCustomersDataGrid.DataSource;

			int item = this.AuthorizedCustomersDataGrid.EditItemIndex
			           + this.AuthorizedCustomersDataGrid.CurrentPageIndex * this.AuthorizedCustomersDataGrid.PageSize;

			var assignmentType = (ASSIGNMENT_TYPE)Convert.ToInt32(this.TypeDropDownList.SelectedValue);

			// When Type is COMPANIES the FMCompanyTextBox is used and the CompanyGroupDropDownList is invisible
			if (assignmentType == ASSIGNMENT_TYPE.COMPANIES)
			{
				return companyItems;
			}

			CompanyGroupCollectionClass companyGroupCollection = FMChannelHelper.MakeCall<ICompanyGroups, CompanyGroupCollectionClass>(
				x =>
					x.Enumerate(this.Security)
				);
			ProductMapCollectionClass authorizedCustomerGroupCollections = this.Product.AuthorizedCustomerGroupCollection;

			foreach (CompanyGroupClass companyGroup in companyGroupCollection)
			{
				bool found = false;

				foreach (ProductMapClass authorizedCustomerGroup in authorizedCustomerGroupCollections)
				{
					if (companyGroup.ID != (string)authorizedCustomersDataView[item][1]
					    && companyGroup.IdentityGuid == authorizedCustomerGroup.IdentityGuid)
					{
						found = true;
						break;
					}
				}

				if (found)
				{
					continue;
				}

				var newCompanyItem = new ListItem(companyGroup.ID, companyGroup.IdentityGuid.ToString());

				foreach (ListItem existingCompanyItem in companyItems)
				{
					if (String.Compare(existingCompanyItem.Text, newCompanyItem.Text, StringComparison.Ordinal) > 0)
					{
						int index = companyItems.IndexOf(existingCompanyItem);
						companyItems.Insert(index, newCompanyItem);
						newCompanyItem = null;
						break;
					}
				}

				if (newCompanyItem != null)
				{
					companyItems.Add(newCompanyItem);
				}
			}

			if (companyItems.Count == 0)
			{
				throw (new Exception("No Company Groups Available."));
			}

			return companyItems;
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
				if (!this.Page.IsPostBack)
				{
					if (!this.Security.HasRight(RIGHT.MODIFY_PRODUCTS))
					{
						this.AddButton.Enabled = false;
					}

					// Populate the TypeDropDownList
					ASSIGNMENT_TYPE[] assignmentTypes = { ASSIGNMENT_TYPE.COMPANIES, ASSIGNMENT_TYPE.COMPANY_GROUPS };

					foreach (ASSIGNMENT_TYPE assignmentType in assignmentTypes)
					{
						var item = new ListItem(this.AssignmentTypeID(assignmentType), ((int)assignmentType).ToString());
						this.TypeDropDownList.Items.Add(item);
					}

					this.UpdateView();
                    this.SetFieldAccessibilityForChildRecordVersion();
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

		protected void TypeDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			this.UpdateView();
		}

		private void AddButtonCommand(object sender, CommandEventArgs e)
		{
			var authorizedCustomer = new ProductMapClass { AssignedGuid = this.Product.IdentityGuid };

			var assignmentType = (ASSIGNMENT_TYPE)Convert.ToInt32(this.TypeDropDownList.SelectedValue);
			ProductMapCollectionClass productMapCollection;

			if (assignmentType == ASSIGNMENT_TYPE.COMPANIES)
			{
				authorizedCustomer.Type = PRODUCT_MAP_TYPE.PRODUCT_COMPANY_MAP;
				productMapCollection = this.Product.AuthorizedCustomerCollection;
			}
			else
			{
				authorizedCustomer.Type = PRODUCT_MAP_TYPE.PRODUCT_COMPANY_GROUP_MAP;
				productMapCollection = this.Product.AuthorizedCustomerGroupCollection;
			}

			authorizedCustomer.Sequence = productMapCollection.Count - 1;
			productMapCollection.Add(authorizedCustomer);

			this.AuthorizedCustomersDataGrid.CurrentPageIndex = (productMapCollection.Count - 1)
			                                                    / this.AuthorizedCustomersDataGrid.PageSize;
			this.AuthorizedCustomersDataGrid.EditItemIndex = (productMapCollection.Count - 1)
			                                                 % this.AuthorizedCustomersDataGrid.PageSize;

			try
			{
				this.EnableControls(false);
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				productMapCollection.RemoveAt(productMapCollection.Count - 1);

				if (this.AuthorizedCustomersDataGrid.CurrentPageIndex > 0
					&& this.AuthorizedCustomersDataGrid.EditItemIndex == 0)
				{
					this.AuthorizedCustomersDataGrid.CurrentPageIndex--;
				}

				this.AuthorizedCustomersDataGrid.EditItemIndex = -1;
				this.EnableControls(true);
				this.UpdateView();
			}
		}

		private string AssignmentTypeID(ASSIGNMENT_TYPE assignmentType)
		{
			switch (assignmentType)
			{
				case ASSIGNMENT_TYPE.COMPANIES:
					return "Companies";

				case ASSIGNMENT_TYPE.COMPANY_GROUPS:
					return "Company Groups";

				default:
					return string.Empty;
			}
		}

		private void AuthorizedCustomersDataGridCancelCommand(object source, DataGridCommandEventArgs e)
		{
			var indexLabel = (Label)e.Item.FindControl("IndexLabel");

			if (indexLabel != null)
			{
				var assignmentType = (ASSIGNMENT_TYPE)Convert.ToInt32(this.TypeDropDownList.SelectedValue);
				ProductMapCollectionClass productMapCollection;
				
				if (assignmentType == ASSIGNMENT_TYPE.COMPANIES)
				{
					productMapCollection = this.Product.AuthorizedCustomerCollection;
				}
				else
				{
					productMapCollection = this.Product.AuthorizedCustomerGroupCollection;
				}

				ProductMapClass authorizedCustomer = productMapCollection[Convert.ToInt32(indexLabel.Text)];

				// If the user has not clicked the green check yet, delete the row.
				if (authorizedCustomer.AssignedToGuid == Guid.Empty)
				{
					productMapCollection.RemoveAt(Convert.ToInt32(indexLabel.Text));

					if ((this.AuthorizedCustomersDataGrid.Items.Count == 1) && (this.AuthorizedCustomersDataGrid.CurrentPageIndex > 0))
					{
						this.AuthorizedCustomersDataGrid.CurrentPageIndex--;
					}
				}

				this.EnableControls(true);
				this.AuthorizedCustomersDataGrid.EditItemIndex = -1;
				this.UpdateView();
			}
		}

		private void AuthorizedCustomersDataGridDeleteCommand(object source, DataGridCommandEventArgs e)
		{
			var indexLabel = (Label)e.Item.FindControl("IndexLabel");
			
			if (indexLabel != null)
			{
				var assignmentType = (ASSIGNMENT_TYPE)Convert.ToInt32(this.TypeDropDownList.SelectedValue);
				ProductMapCollectionClass productMapCollection;

				if (assignmentType == ASSIGNMENT_TYPE.COMPANIES)
				{
					productMapCollection = this.Product.AuthorizedCustomerCollection;
				}
				else
				{
					productMapCollection = this.Product.AuthorizedCustomerGroupCollection;
				}

				if (this.AuthorizedCustomersDataGrid.EditItemIndex == e.Item.ItemIndex)
				{
					this.AuthorizedCustomersDataGrid.EditItemIndex = -1;
					this.EnableControls(true);
				}
				else if (this.AuthorizedCustomersDataGrid.EditItemIndex > e.Item.ItemIndex)
				{
					this.AuthorizedCustomersDataGrid.EditItemIndex--;
				}

				productMapCollection.RemoveAt(Convert.ToInt32(indexLabel.Text));

				if (this.AuthorizedCustomersDataGrid.Items.Count == 1 && this.AuthorizedCustomersDataGrid.CurrentPageIndex > 0)
				{
					this.AuthorizedCustomersDataGrid.CurrentPageIndex--;
				}

				this.UpdateView();
			}
		}

		private void AuthorizedCustomersDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
			this.AuthorizedCustomersDataGrid.EditItemIndex = e.Item.ItemIndex;
			this.EnableControls(false);
			this.UpdateView();
		}

		private void AuthorizedCustomersDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			var indexLabel = (Label)e.Item.FindControl("IndexLabel");

			if (indexLabel != null)
			{
				var assignmentType = (ASSIGNMENT_TYPE)Convert.ToInt32(this.TypeDropDownList.SelectedValue);
				ProductMapCollectionClass productMapCollection;

				if (assignmentType == ASSIGNMENT_TYPE.COMPANIES)
				{
					productMapCollection = this.Product.AuthorizedCustomerCollection;
				}
				else
				{
					productMapCollection = this.Product.AuthorizedCustomerGroupCollection;
				}

				ProductMapClass authorizedCustomer = productMapCollection[Convert.ToInt32(indexLabel.Text)];

				var buttonInstructions = (HtmlInputButton)e.Item.FindControl("InstructionsButton");
				if (buttonInstructions != null)
				{
					buttonInstructions.Disabled = (assignmentType != ASSIGNMENT_TYPE.COMPANIES);
				}

                
                var recordEditBtn = (FMEditLinkButton)e.Item.FindControl("RecordEditBtn");
                var recordDeleteBtn = (FMDeleteLinkButton)e.Item.FindControl("RecordDeleteBtn");
                
                //For child record versions, set the datagrid buttons availability according to the Product FLC configuration on the "AuthorizedCustomers" field
                bool currentSiteOwnsRecordVersion = (this.Product.SiteGuid == this.Security.SiteGuid);

                if (!((this.Product.IdentityGuid.Equals(Guid.Empty)) 
					|| (currentSiteOwnsRecordVersion && this.Product.IdentityGuid.Equals(this.Product.MasterRecordGuid)) 
					|| (this.VersionSpecificFields == null) || (this.VersionSpecificFields.Count == 0)))
                {
                    if ((buttonInstructions != null) && !buttonInstructions.Disabled)
                    {
                        string addText = this.GetTranslatedText("Add");

	                    if (buttonInstructions.Value == addText)
	                    {
		                    buttonInstructions.Disabled = !currentSiteOwnsRecordVersion && (!this.VersionSpecificFields.Contains("AuthorizedCustomers"));
	                    }
                    }

	                if (recordEditBtn != null)
	                {
		                recordEditBtn.Enabled = recordEditBtn.Enabled && this.VersionSpecificFields.Contains("AuthorizedCustomers");
	                }

	                if (recordDeleteBtn != null)
	                {
		                recordDeleteBtn.Enabled = recordDeleteBtn.Enabled && this.VersionSpecificFields.Contains("AuthorizedCustomers");
	                }
                }

				if (e.Item.ItemIndex == this.AuthorizedCustomersDataGrid.EditItemIndex)
				{
					var companyGroupDropDownList = (FMDropDownList)e.Item.FindControl("CompanyGroupDropDownList");
					var companyTextBox = (FMCompanyTextBox)e.Item.FindControl("CompanyTextBox");

					if (companyGroupDropDownList != null && companyTextBox != null)
					{
						companyTextBox.Visible = (assignmentType == ASSIGNMENT_TYPE.COMPANIES);
						companyGroupDropDownList.Visible = (assignmentType != ASSIGNMENT_TYPE.COMPANIES);

						if (authorizedCustomer.IdentityGuid != Guid.Empty)
						{
							if (assignmentType != ASSIGNMENT_TYPE.COMPANIES)
							{
								ListItemCollection items = companyGroupDropDownList.Items;
								int index = items.IndexOf(items.FindByValue(authorizedCustomer.IdentityGuid.ToString()));
								companyGroupDropDownList.SelectedIndex = index;
							}
						}

						var additiveProfilesDropDownList = (DropDownList)e.Item.FindControl("AdditiveProfilesDropDownList");
						
						if (additiveProfilesDropDownList != null)
						{
							if (authorizedCustomer.AdditiveProfileGuid != Guid.Empty)
							{
								ListItemCollection items = additiveProfilesDropDownList.Items;
								int index = items.IndexOf(items.FindByValue(authorizedCustomer.AdditiveProfileGuid.ToString()));
								additiveProfilesDropDownList.SelectedIndex = index;
							}

							if (additiveProfilesDropDownList.Items.Count == 0)
							{
								additiveProfilesDropDownList.Visible = false;
							}
						}
					}
				}
				else
				{
					if (assignmentType == ASSIGNMENT_TYPE.COMPANIES)
					{
						var companyIDLabel = (Label)e.Item.FindControl("CompanyIDLabel");
						
						if (companyIDLabel != null)
						{
							companyIDLabel.ToolTip = authorizedCustomer.AssignedToToolTip;
						}
					}
				}
			}
		}

		private void AuthorizedCustomersDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.AuthorizedCustomersDataGrid.EditItemIndex > -1)
			{
				return;
			}

			this.AuthorizedCustomersDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateView();
		}

		private void AuthorizedCustomersDataGridUpdateCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				var indexLabel = (Label)e.Item.FindControl("IndexLabel");

				if (indexLabel != null)
				{
					var assignmentType = (ASSIGNMENT_TYPE)Convert.ToInt32(this.TypeDropDownList.SelectedValue);
					ProductMapCollectionClass productMapCollection;

					if (assignmentType == ASSIGNMENT_TYPE.COMPANIES)
					{
						productMapCollection = this.Product.AuthorizedCustomerCollection;
					}
					else
					{
						productMapCollection = this.Product.AuthorizedCustomerGroupCollection;
					}

					int rowIndex = Convert.ToInt32(indexLabel.Text);
					ProductMapClass authorizedCustomer = productMapCollection[rowIndex];

					if (assignmentType == ASSIGNMENT_TYPE.COMPANIES)
					{
						var companyTextBox = (FMCompanyTextBox)e.Item.FindControl("CompanyTextBox");
						if (companyTextBox.Text != string.Empty)
						{
							Guid companyGuid = FMChannelHelper.MakeCall<ICompanies, Guid>(
																	 x => x.GetIdentityGuid(this.Security, companyTextBox.Text));

							// check to see if this company is already an authorized customer, and if so, return an error
							if (this.FindDuplicate(productMapCollection, companyGuid, rowIndex))
							{
								this.ErrorHandler(new Exception("The company selected is already assigned to the product"));
								return;
							}

							authorizedCustomer.AssignedToGuid = companyGuid;
							authorizedCustomer.AssignedToID = companyTextBox.Text;
							CompanyClass company = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
													companies => companies.Get(this.Security, authorizedCustomer.IdentityGuid, false));

							authorizedCustomer.AssignedToName = company.Name;
							authorizedCustomer.AssignedToAddress = company.Address1;
							authorizedCustomer.AssignedToCity = company.City;
							authorizedCustomer.AssignedToState = company.State;
						}
						else
						{
							productMapCollection.RemoveAt(Convert.ToInt32(indexLabel.Text));
							if (this.AuthorizedCustomersDataGrid.Items.Count == 1 && this.AuthorizedCustomersDataGrid.CurrentPageIndex > 0)
							{
								this.AuthorizedCustomersDataGrid.CurrentPageIndex--;
							}
						}
					}
					else
					{
						var companyGroupDropDownList = (FMDropDownList)e.Item.FindControl("CompanyGroupDropDownList");

						if (companyGroupDropDownList.SelectedIndex != -1)
						{
							Guid companyGroupGuid = Guid.Parse(companyGroupDropDownList.SelectedValue);

							//check to see if this company group is already an authorized customer, and if so, return an error
							if (this.FindDuplicate(productMapCollection, companyGroupGuid, rowIndex))
							{
								this.ErrorHandler(new Exception("The company group selected is already assigned to the product"));
								return;
							}

							authorizedCustomer.AssignedToGuid = companyGroupGuid;
							authorizedCustomer.AssignedToID = companyGroupDropDownList.SelectedItem.Text;
						}
					}

					var additiveProfilesDropDownList = (DropDownList)e.Item.FindControl("AdditiveProfilesDropDownList");

					if (additiveProfilesDropDownList.SelectedIndex != -1)
					{
						authorizedCustomer.AdditiveProfileGuid = Guid.Parse(additiveProfilesDropDownList.SelectedValue);
						authorizedCustomer.AdditiveProfileID = additiveProfilesDropDownList.SelectedItem.Text;
					}

					var shipToProductID = (TextBox)e.Item.FindControl("ShipToProductIDTextBox");
					authorizedCustomer.ShipToProductID = shipToProductID.Text;

					var shipToProductCode = (TextBox)e.Item.FindControl("ShipToProductCodeTextBox");
					authorizedCustomer.ShipToProductCode = shipToProductCode.Text;

					var shipToLoadRackDisplayText = (TextBox)e.Item.FindControl("ShipToLoadRackDisplayTextTextBox");
					authorizedCustomer.ShipToLoadRackDisplayText = shipToLoadRackDisplayText.Text;

					this.EnableControls(true);
					this.AuthorizedCustomersDataGrid.EditItemIndex = -1;
					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private ICollection EnumerateAuthorizedCustomers()
		{
			ProductMapCollectionClass authorizedCustomerCollection;
			var assignmentType = (ASSIGNMENT_TYPE)Convert.ToInt32(this.TypeDropDownList.SelectedValue);

			if (assignmentType == ASSIGNMENT_TYPE.COMPANIES)
			{
				authorizedCustomerCollection = this.Product.AuthorizedCustomerCollection;
			}
			else
			{
				authorizedCustomerCollection = this.Product.AuthorizedCustomerGroupCollection;
			}

			var authorizedCustomersDataTable = new DataTable();

			authorizedCustomersDataTable.Columns.Add("Index", typeof(Int32));
			authorizedCustomersDataTable.Columns.Add("CustomerID", typeof(string));
			authorizedCustomersDataTable.Columns.Add("AdditiveProfileID", typeof(string));
			authorizedCustomersDataTable.Columns.Add("ShipToProductID", typeof(string));
			authorizedCustomersDataTable.Columns.Add("ShipToProductCode", typeof(string));
			authorizedCustomersDataTable.Columns.Add("ShipToLoadRackDisplayText", typeof(string));
			authorizedCustomersDataTable.Columns.Add("SpecialInstructionsText", typeof(string));
			authorizedCustomersDataTable.Columns.Add("SpecialInstructionsClick", typeof(string));

			string addText = this.GetTranslatedText("Add");
			string editText = this.GetTranslatedText("Edit");
            
            bool isAuthCustomersVs = true;
            bool currentSiteOwnsRecordVersion = (this.Product.SiteGuid == this.Security.SiteGuid);
            if (!((this.Product.IdentityGuid.Equals(Guid.Empty))
				  || (currentSiteOwnsRecordVersion && this.Product.IdentityGuid.Equals(this.Product.MasterRecordGuid)) 
                  || (this.VersionSpecificFields == null))
               )
			{
				isAuthCustomersVs = this.VersionSpecificFields.Contains("AuthorizedCustomers");
			}

			if (authorizedCustomerCollection != null)
			{
				for (int iItem = 0; iItem < authorizedCustomerCollection.Count; iItem++)
				{
					DataRow authorizedCustomerDataRow = authorizedCustomersDataTable.NewRow();

					ProductMapClass authorizedCustomer = authorizedCustomerCollection[iItem];
					authorizedCustomerDataRow["Index"] = iItem;
					authorizedCustomerDataRow["CustomerID"] = authorizedCustomer.AssignedToID;
					authorizedCustomerDataRow["AdditiveProfileID"] = authorizedCustomer.AdditiveProfileID;
					authorizedCustomerDataRow["ShipToProductID"] = authorizedCustomer.ShipToProductID;
					authorizedCustomerDataRow["ShipToProductCode"] = authorizedCustomer.ShipToProductCode;
					authorizedCustomerDataRow["ShipToLoadRackDisplayText"] = authorizedCustomer.ShipToLoadRackDisplayText;
					authorizedCustomerDataRow["SpecialInstructionsText"] = this.HasSpecialInstruction(authorizedCustomer)
						                                                       ? editText
						                                                       : addText;
					if (isAuthCustomersVs)
					{
						authorizedCustomerDataRow["SpecialInstructionsClick"] = "InstructionsButton_Click(" + iItem + ")";
					}
					else 
					{
						//For child record versions for which the AuthorisedCustomers field is ParentSpecific, 
						//set Special Instructions form in read-only mode.
						authorizedCustomerDataRow["SpecialInstructionsClick"] = "InstructionsReadOnlyButton_Click(" + iItem + ")";
					}

					// Sort the DataTable by CustomerID except for an Added entry
					// which has a null CustomerID that messes up the sort order
					// relative to the EditItemIndex
					if (authorizedCustomer.IdentityGuid != Guid.Empty)
					{
						int iRow = 0;
						foreach (DataRow row in authorizedCustomersDataTable.Rows)
						{
							if (String.Compare(((string)row["CustomerID"]), (string)authorizedCustomerDataRow["CustomerID"], StringComparison.Ordinal) > 0)
							{
								authorizedCustomersDataTable.Rows.InsertAt(authorizedCustomerDataRow, iRow);
								authorizedCustomerDataRow = null;
								break;
							}

							iRow++;
						}
					}

					if (authorizedCustomerDataRow != null)
					{
						authorizedCustomersDataTable.Rows.Add(authorizedCustomerDataRow);
					}
				}
			}
			
			var authorizedCustomersDataView = new DataView(authorizedCustomersDataTable);
			return authorizedCustomersDataView;
		}

		/// <summary>
		///    Search the existing product maps to see if any are assigned to the same company
		///    The data grid index of the product map you are checking for duplicates of is required so
		///    that it does not count as a duplicate of itself
		/// </summary>
		/// <param name="productMaps">A productMapCollection to search</param>
		/// <param name="companyOrCompanyGroupGuid">The Guid of the company or company group to search for duplicates of</param>
		/// <param name="gridIndex">the data grid index of the product map we are searching for duplicates of, so it doesn't get counted as a duplicate of itself</param>
		/// <returns>True if a duplicate is found. False otherwise.</returns>
		private bool FindDuplicate(ProductMapCollectionClass productMaps, Guid companyOrCompanyGroupGuid, int gridIndex)
		{
			if (productMaps != null)
			{
				for (int i = 0; i < productMaps.Count; ++i)
				{
					ProductMapClass existingProductMap = productMaps[i];

					//if the identity guids are the same, and the index in the test collection is not the same, 
					//then we have a duplicate
					if (existingProductMap.AssignedToGuid == companyOrCompanyGroupGuid && gridIndex != i)
					{
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AuthorizedCustomersDataGrid.EditCommand += this.AuthorizedCustomersDataGridEditCommand;
			this.AuthorizedCustomersDataGrid.PageIndexChanged += this.AuthorizedCustomersDataGridPageIndexChanged;
			this.AuthorizedCustomersDataGrid.CancelCommand += this.AuthorizedCustomersDataGridCancelCommand;
			this.AuthorizedCustomersDataGrid.UpdateCommand += this.AuthorizedCustomersDataGridUpdateCommand;
			this.AuthorizedCustomersDataGrid.DeleteCommand += this.AuthorizedCustomersDataGridDeleteCommand;
			this.AuthorizedCustomersDataGrid.ItemDataBound += this.AuthorizedCustomersDataGridItemDataBound;
			this.AddButton.Command += this.AddButtonCommand;
		}

		private void UpdateSpecialInstructionButton()
		{
			string addText = this.GetTranslatedText("Add");
			string editText = this.GetTranslatedText("Edit");

			foreach (DataGridItem gridItem in this.AuthorizedCustomersDataGrid.Items)
			{
				var indexLabel = (Label)gridItem.FindControl("IndexLabel");

				if (indexLabel != null)
				{
					ProductMapClass productMap = this.Product.AuthorizedCustomerCollection[Convert.ToInt32(indexLabel.Text)];

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

		private void UpdateView()
		{
			this.AuthorizedCustomersDataGrid.DataSource = this.EnumerateAuthorizedCustomers();
			this.AuthorizedCustomersDataGrid.DataBind();
		}

        private void SetFieldAccessibilityForChildRecordVersion()
        {
            bool currentSiteOwnsRecordVersion = (this.Product.SiteGuid == this.Security.SiteGuid);

            if ((this.Product.IdentityGuid.Equals(Guid.Empty) 
                 || (currentSiteOwnsRecordVersion && this.Product.IdentityGuid.Equals(this.Product.MasterRecordGuid))
                 || (this.VersionSpecificFields == null)))
            {
                return;
            }

            this.AddButton.Enabled = (this.AddButton.Enabled && this.VersionSpecificFields.Contains("AuthorizedCustomers"));
        }
		#endregion
	}
}