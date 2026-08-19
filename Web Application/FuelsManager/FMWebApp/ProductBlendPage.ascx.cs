// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProductBlendPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ProductBlendPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FMControls;

	/// <summary>
	///    Summary description for ProductComponentsPage.
	/// </summary>
	public partial class ProductBlendPage : ProductPageBase
	{
		#region Constants and Fields
		protected SiteClass CurrentSite;
		#endregion

		#region Enums
		/// <summary>
		///    Indicates the direction to shift the selected component within the data grid list.
		/// </summary>
		private enum ShiftDirection
		{
			Up,
			Down
		};
		#endregion

		#region Public Methods and Operators
		public void UpdateData()
		{
			if (this.Product.ProductType != ProductType.BlendProduct)
			{
				return;
			}

			this.Product.ComponentTolerance = this.AllowableToleranceTextbox.Text;
		}
		#endregion

		#region Methods
		protected ListItemCollection EnumerateComponentProducts()
		{
			ProductCollectionClass availableComponentCollection = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(
																	 x =>
																	 x.EnumerateByType(this.Security, ProductType.ComponentProduct)
																);

			var componentItems = new ListItemCollection();

			foreach (ProductClass availableComponent in availableComponentCollection)
			{
				// Exclude self
				if (this.Product.IdentityGuid != Guid.Empty && this.Product.IdentityGuid == availableComponent.MasterRecordGuid)
				{
					continue;
				}

				// Exclude Components that already part of the blend
				bool inUse = false;
				int item = 0;

				foreach (ProductMapClass component in this.Product.ComponentCollection)
				{
					if (item
					    != this.ComponentsDataGrid.EditItemIndex
					       + this.ComponentsDataGrid.CurrentPageIndex * this.ComponentsDataGrid.PageSize
					    && component.AssignedGuid == availableComponent.MasterRecordGuid)
					{
						inUse = true;
						break;
					}

					item++;
				}

				if (inUse)
				{
					continue;
				}

				var newComponentItem = new ListItem(availableComponent.ID, availableComponent.MasterRecordGuid.ToString());  //Product Blend-to-Component mapping is not under Record Versioning, i.e. the MasterRecordGuid of the Products must be used in the mapping.
				
				foreach (ListItem existingComponentItem in componentItems)
				{
					if (String.Compare(existingComponentItem.Text, newComponentItem.Text, StringComparison.Ordinal) > 0)
					{
						int index = componentItems.IndexOf(existingComponentItem);
						componentItems.Insert(index, newComponentItem);
						newComponentItem = null;
						break;
					}
				}

				if (newComponentItem != null)
				{
					componentItems.Add(newComponentItem);
				}
			}

			if (componentItems.Count == 0)
			{
				throw new Exception("No Available Components");
			}

			return componentItems;
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
				this.CurrentSite = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(this.Security, this.Security.SiteGuid, false, false, true)
																);
				if (this.Product.ProductType != ProductType.BlendProduct)
				{
					return;
				}

				if (!this.Page.IsPostBack)
				{
					this.UpdateComponentsView();
					this.AllowableToleranceTextbox.Text = this.Product.ComponentTolerance;
                    this.SetFieldAccessibilityForChildRecordVersion();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void AddButtonCommand(object sender, CommandEventArgs e)
		{
			ProductMapCollectionClass componentCollection = this.Product.ComponentCollection;
			var component = new ProductMapClass
			                {
				                AssignedToGuid = this.Product.IdentityGuid,
				                Type = PRODUCT_MAP_TYPE.BLEND_COMPONENT_MAP,
				                Sequence = componentCollection.Count
			                };

			componentCollection.Add(component);
			this.ComponentsDataGrid.CurrentPageIndex = (componentCollection.Count - 1) / this.ComponentsDataGrid.PageSize;
			this.ComponentsDataGrid.EditItemIndex = (componentCollection.Count - 1) % this.ComponentsDataGrid.PageSize;
			
			try
			{
				this.EnableControls(false);
				this.UpdateComponentsView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				componentCollection.RemoveAt(componentCollection.Count - 1);
				
				if (this.ComponentsDataGrid.CurrentPageIndex > 0 && this.ComponentsDataGrid.EditItemIndex == 0)
				{
					this.ComponentsDataGrid.CurrentPageIndex--;
				}
				
				this.ComponentsDataGrid.EditItemIndex = -1;
				this.UpdateComponentsView();
				this.EnableControls(true);
			}
		}

		private void ComponentsDataGridCancelCommand(object source, DataGridCommandEventArgs e)
		{
			var indexLabel = (Label)e.Item.FindControl("IndexLabel");
			
			if (indexLabel != null)
			{
				ProductMapCollectionClass componentCollection = this.Product.ComponentCollection;
				ProductMapClass component = componentCollection[Convert.ToInt32(indexLabel.Text)];
				
				if (component.AssignedGuid == Guid.Empty)
				{
					componentCollection.RemoveAt(Convert.ToInt32(indexLabel.Text));
					
					if (this.ComponentsDataGrid.Items.Count == 1 && this.ComponentsDataGrid.CurrentPageIndex > 0)
					{
						this.ComponentsDataGrid.CurrentPageIndex--;
					}
				}
				
				this.ComponentsDataGrid.EditItemIndex = -1;
				this.EnableControls(true);
				this.UpdateComponentsView();
			}
		}

		private void ComponentsDataGridDeleteCommand(object source, DataGridCommandEventArgs e)
		{
			var indexLabel = (Label)e.Item.FindControl("IndexLabel");
			
			if (indexLabel != null)
			{
				ProductMapCollectionClass componentCollection = this.Product.ComponentCollection;

				if (this.ComponentsDataGrid.EditItemIndex == e.Item.ItemIndex)
				{
					this.ComponentsDataGrid.EditItemIndex = -1;
					this.EnableControls(true);
				}
				else if (this.ComponentsDataGrid.EditItemIndex > e.Item.ItemIndex)
				{
					this.ComponentsDataGrid.EditItemIndex--;
				}

				componentCollection.RemoveAt(Convert.ToInt32(indexLabel.Text));
				
				if (this.ComponentsDataGrid.Items.Count == 1 && this.ComponentsDataGrid.CurrentPageIndex > 0)
				{
					this.ComponentsDataGrid.CurrentPageIndex--;
				}
				
				this.UpdateComponentsView();
			}
		}

		private void ComponentsDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				this.EnableControls(false);
				this.ComponentsDataGrid.EditItemIndex = e.Item.ItemIndex;
				this.UpdateComponentsView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				this.ComponentsDataGrid.EditItemIndex = -1;
				this.UpdateComponentsView();
				this.EnableControls(true);
			}
		}

		private void ComponentsDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			var indexLabel = (Label)e.Item.FindControl("IndexLabel");
			
			if (indexLabel != null)
			{
				var productsDropDownList = (DropDownList)e.Item.FindControl("ProductsDropDownList");
				
				if (productsDropDownList != null)
				{
					ProductMapCollectionClass componentCollection = this.Product.ComponentCollection;
					ProductMapClass component = componentCollection[Convert.ToInt32(indexLabel.Text)];

					// If Edit then must add Item back in to list as it was omitted
					// during EnumerateComponentProducts to prevent duplication
					if (component.AssignedGuid != Guid.Empty)
					{
						var newComponentItem = new ListItem(component.AssignedID, component.AssignedGuid.ToString());
						
						foreach (ListItem existingComponentItem in productsDropDownList.Items)
						{
							if (String.Compare(existingComponentItem.Text, newComponentItem.Text, StringComparison.Ordinal) > 0)
							{
								int index = productsDropDownList.Items.IndexOf(existingComponentItem);
								productsDropDownList.Items.Insert(index, newComponentItem);
								productsDropDownList.SelectedIndex = index;
								newComponentItem = null;
								break;
							}
						}

						if (newComponentItem != null)
						{
							productsDropDownList.Items.Add(newComponentItem);
							productsDropDownList.SelectedIndex = productsDropDownList.Items.Count - 1;
						}
					}
				}
                //For child record versions, disable the datagrid editing buttons. The components of a Blend product cannot be changed on child record versions.
                bool currentSiteOwnsRecordVersion = (this.Product.SiteGuid == this.Security.SiteGuid);
                if (!(this.Product.IdentityGuid.Equals(Guid.Empty) 
					|| (currentSiteOwnsRecordVersion && this.Product.IdentityGuid.Equals(this.Product.MasterRecordGuid))
					|| this.VersionSpecificFields == null))
                {
                    var recordEditBtn = (FMEditLinkButton)e.Item.FindControl("RecordEditBtn");
                    var recordDeleteBtn = (FMDeleteLinkButton)e.Item.FindControl("RecordDeleteBtn");

	                if (recordEditBtn != null)
	                {
		                recordEditBtn.Enabled = false;
	                }

	                if (recordDeleteBtn != null)
	                {
		                recordDeleteBtn.Enabled = false;
	                }
                }
			}
		}

		private void ComponentsDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.ComponentsDataGrid.EditItemIndex > -1)
			{
				return;
			}

			this.ComponentsDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateComponentsView();
		}

		private void ComponentsDataGridUpdateCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				var indexLabel = (Label)e.Item.FindControl("IndexLabel");
				
				if (indexLabel != null)
				{
					ProductMapCollectionClass componentCollection = this.Product.ComponentCollection;
					ProductMapClass component = componentCollection[Convert.ToInt32(indexLabel.Text)];

					var productsDropDownList = (DropDownList)e.Item.FindControl("ProductsDropDownList");
					component.AssignedGuid = Guid.Parse(productsDropDownList.SelectedValue);
					component.AssignedID = productsDropDownList.SelectedItem.Text;

					var percentTextBox = (TextBox)e.Item.FindControl("PercentTextBox");
					component.BlendPercentage = Convert.ToDouble(percentTextBox.Text, this.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT));
					this.ComponentsDataGrid.EditItemIndex = -1;
					this.EnableControls(true);
					this.UpdateComponentsView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void DownButtonCommand(object sender, CommandEventArgs e)
		{
			try
			{
				this.ShiftComponent(ShiftDirection.Down);
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		///    This method enables/disables controls.
		/// </summary>
		/// <param name="enable"></param>
		private void EnableControls(bool enable)
		{
			this.AddButton.Enabled = enable;
			this.UpButton.Enabled = enable;
			this.DownButton.Enabled = enable;
			this.AllowableToleranceTextbox.Enabled = enable;

			// Call the main form to disable buttons and tabs.
			var productForm = (ProductForm)this.Page;
			productForm.EnableControls(enable);
		}

		private ICollection EnumerateComponents()
		{
			ProductMapCollectionClass componentCollection = this.Product.ComponentCollection;

			var componentDataTable = new DataTable();

			componentDataTable.Columns.Add("Index", typeof(Int32));
			componentDataTable.Columns.Add("ID", typeof(string));
			componentDataTable.Columns.Add("Percent", typeof(string));

			if (componentCollection != null)
			{
				int item = 0;
				
				foreach (ProductMapClass component in componentCollection)
				{
					DataRow componentDataRow = componentDataTable.NewRow();

					componentDataRow["Index"] = item;
					componentDataRow["ID"] = component.AssignedID;
					componentDataRow["Percent"] = component.BlendPercentage.ToString(this.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT));

					componentDataTable.Rows.Add(componentDataRow);
					item++;
				}
			}

			return new DataView(componentDataTable);
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.DownButton.Command						+= this.DownButtonCommand;
			this.UpButton.Command						+= this.UpButtonCommand;
			this.ComponentsDataGrid.EditCommand			+= this.ComponentsDataGridEditCommand;
			this.ComponentsDataGrid.PageIndexChanged	+= this.ComponentsDataGridPageIndexChanged;
			this.ComponentsDataGrid.CancelCommand		+= this.ComponentsDataGridCancelCommand;
			this.ComponentsDataGrid.UpdateCommand		+= this.ComponentsDataGridUpdateCommand;
			this.ComponentsDataGrid.DeleteCommand		+= this.ComponentsDataGridDeleteCommand;
			this.ComponentsDataGrid.ItemDataBound		+= this.ComponentsDataGridItemDataBound;
			this.AddButton.Command						+= this.AddButtonCommand;
		}

		/// <summary>
		///    Shift the selected component in the specified direction.  Wrap-around is enabled so that
		///    a component at the top of the list will move to the bottom when shifted up.  Likewise a
		///    component at the bottom of the list will move to the top when shifted down.  Multiple
		///    data grid pages exist when the total number of components is greater than the page size.
		///    Shifting is limited to the components on the current data grid page.
		/// </summary>
		/// <param name="direction">The direction to shift, either Up or Down</param>
		private void ShiftComponent(ShiftDirection direction)
		{
			int selectedIndex = this.ComponentsDataGrid.SelectedIndex;

			// Compute the number of items on current page.  Only the last page may have fewer items than the page size.
			int numItems = this.ComponentsDataGrid.PageSize;

			// if last page
			if (this.ComponentsDataGrid.CurrentPageIndex == this.ComponentsDataGrid.PageCount - 1) 
			{
				numItems = this.Product.ComponentCollection.Count
				           - this.ComponentsDataGrid.CurrentPageIndex * this.ComponentsDataGrid.PageSize;
			}

			if (selectedIndex == -1 || numItems == 1)
			{
				return;
			}

			// Compute the component index based on the selected index, page size and current page
			int componentIndex = selectedIndex + this.ComponentsDataGrid.CurrentPageIndex * this.ComponentsDataGrid.PageSize;

			// Compute the new index based on the shift direction, selected index and current number of items
			int newIndex;
			if (direction == ShiftDirection.Up)
			{
				if (selectedIndex == 0)
				{
					newIndex = numItems - 1;
				}
				else
				{
					newIndex = selectedIndex - 1;
				}
			}
			else
			{
				if (selectedIndex == numItems - 1)
				{
					newIndex = 0;
				}
				else
				{
					newIndex = selectedIndex + 1;
				}
			}

			// Compute the new component index based on the new index, page size and current page
			int newComponentIndex = newIndex + this.ComponentsDataGrid.CurrentPageIndex * this.ComponentsDataGrid.PageSize;

			ProductMapClass selectedComponent = this.Product.ComponentCollection[componentIndex];
			this.Product.ComponentCollection.RemoveAt(componentIndex);
			this.Product.ComponentCollection.Insert(newComponentIndex, selectedComponent);
			int sequence = 0;

			foreach (ProductMapClass component in this.Product.ComponentCollection)
			{
				component.Sequence = sequence++;
			}

			this.ComponentsDataGrid.SelectedIndex = newIndex;
			this.UpdateComponentsView();
		}

		private void UpButtonCommand(object sender, CommandEventArgs e)
		{
			try
			{
				this.ShiftComponent(ShiftDirection.Up);
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		private void UpdateComponentsView()
		{
			this.ComponentsDataGrid.DataSource = this.EnumerateComponents();
			this.ComponentsDataGrid.DataBind();
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

            this.AllowableToleranceTextbox.Enabled = (this.AllowableToleranceTextbox.Enabled && this.VersionSpecificFields.Contains("ComponentTolerance"));

            this.AddButton.Enabled = false;
            this.UpButton.Enabled = false;
            this.DownButton.Enabled = false;
        }
		#endregion
	}
}