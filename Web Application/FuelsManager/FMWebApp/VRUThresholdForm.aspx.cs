/******************************************************************************
	FILE NAME:		VRUTrackingForm.aspx.cs
	PURPOSE:		Implementation of VCU/VRU threshold configuration and tracking

	COMMENTS:
		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
		This file shall not be copied or reproduced in any form without
		the express written consent of Endress+Hauser.

	AUTHOR(S):	Francisco Martin-Manzano
	VERSION:	1.0.0  Current version

	MODIFICATION HISTORY:
		Date:			By:				Reason:
		----------	-----------------	-------------------------------------------

*******************************************************************************/

/**** TODO, SQL function CheckProductMap needs to be modified to include the new product map type */
namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Globalization;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;


	/// <summary>
	/// Implementation vru tracking form.
	/// </summary>
	public partial class VruThresholdForm : FMAutoSubmitFormBase, IMenuDiscovery
	{
		public static string VruThresholdFormUrl
		{
			get
			{
				string vruTrackingFormUrl = ConfigurationManager.AppSettings["VruTrackingFormURL"];
				if (string.IsNullOrEmpty(vruTrackingFormUrl))
				{
					vruTrackingFormUrl = "FMWebApp/VRUThresholdForm.aspx";
				}
				return "../" + vruTrackingFormUrl;
			}
		}

		public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1, ushort word2, ushort useNewLicenseKey, uint options)
		{
			if (useNewLicenseKey == 1)
			{
				if ((word2 & 0x01) != 0x01)
					return null;
			}
			else
			{
				// Depends Upon Load Rack
				if ((options & 0x8000) == 0)
				{
					return null;
				}
			}
			if (siteGroup)
			{
				return null;
			}

			if (!security.HasRight(RIGHT.VIEW_LOAD_RACK_DATA) && !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA))
			{
				return null;
			}

			var items = new List<FMMenuItem>
								{
									 new FMMenuItem
									 {
										  MenuItemType = FMMenuItemType.VRU_THRESHOLD_CONFIG,
										  RootMenuName = "Operations",
										  CategoryName = "Load Rack",
										  ItemName = "Throughput Monitor",
										  NavigateUrl = VruThresholdFormUrl,
										  ApplyDataDictionary = ApplyDataDictionary.Apply
									 }
								};

			return items;
		}


		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AddButton2.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButtonCommand);
			this.VRUConfigurationDataGrid.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.VRUConfigurationDataGrid_EditCommand);
			this.VRUConfigurationDataGrid.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.VRUConfigurationDataGrid_PageIndexChanged);
			this.VRUConfigurationDataGrid.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.VRUConfigurationDataGrid_CancelCommand);
			this.VRUConfigurationDataGrid.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.VRUConfigurationDataGrid_UpdateCommand);
			this.VRUConfigurationDataGrid.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.VRUConfigurationDataGrid_DeleteCommand);
			this.VRUConfigurationDataGrid.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.VRUConfigurationDataGrid_ItemDataBound);
			this.VRUConfigurationDataGrid.ItemCommand += new DataGridCommandEventHandler(this.VRUConfigurationDataGrid_ItemCommand);
			this.AddButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButtonCommand);
			this.RefreshButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.RefreshButtonCommand);
			this.UnassignProductsButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.UnassignProductsButtonCommand);
			this.AssignProductsButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AssignProductsButtonCommand);

		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!this.Page.IsPostBack)
				{
					if (!this.Security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA))
					{
						this.AddButton.Enabled = false;
						this.AddButton2.Enabled = false;
					}

					this.UpdateView();

					// Populate AssignedProductsListBox
					foreach (ProductMapClass productMap in FMChannelHelper.MakeCall<IProductMaps, ProductMapCollectionClass>(x => x.EnumerateByType(this.Security, PRODUCT_MAP_TYPE.VRU_VCU_TRACKING)))
					{
						var unlistedProductItem = new ListItem(productMap.AssignedID, productMap.AssignedGuid.ToString());

						foreach (ListItem assignedProductItem in this.AssignedProductsListBox.Items)
						{
							if (string.Compare(assignedProductItem.Text, unlistedProductItem.Text, StringComparison.Ordinal) > 0)
							{
								int index = this.AssignedProductsListBox.Items.IndexOf(assignedProductItem);
								this.AssignedProductsListBox.Items.Insert(index, unlistedProductItem);
								unlistedProductItem = null;
								break;
							}
						}

						if (unlistedProductItem != null)
						{
							this.AssignedProductsListBox.Items.Add(unlistedProductItem);
						}
					}

					// Populate UnassignedProductsListBox
					ProductCollectionClass productCollection = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(x => x.Enumerate(this.Security));
					foreach (ProductClass product in productCollection)
					{
						if (product.ProductType != ProductType.ComponentProduct)
						{
							continue;
						}

						if (null == this.AssignedProductsListBox.Items.FindByValue(product.MasterRecordGuid.ToString()))
						{
							var unlistedProductItem = new ListItem(product.ID, product.MasterRecordGuid.ToString());

							foreach (ListItem unassignedProductItem in this.UnassignedProductsListBox.Items)
							{
								if (string.Compare(unassignedProductItem.Text, unlistedProductItem.Text, StringComparison.Ordinal) > 0)
								{
									int index = this.UnassignedProductsListBox.Items.IndexOf(unassignedProductItem);
									this.UnassignedProductsListBox.Items.Insert(index, unlistedProductItem);
									unlistedProductItem = null;
									break;
								}
							}

							if (unlistedProductItem != null)
							{
								this.UnassignedProductsListBox.Items.Add(unlistedProductItem);
							}
						}
					}

					if (!this.Security.HasRight(RIGHT.MODIFY_PRODUCTS))
					{
						this.AssignProductsButton.Enabled = false;
						this.UnassignProductsButton.Enabled = false;
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void UpdateView()
		{
			try
			{
				this.EnableControls(false);

				// Need to always refresh the collection here on non-postback, as it may have been updated in the database by the scheduled job
				// Postbacks come from command actions, which themselves either clear or reload the collection
				if (!this.IsPostBack || this.Session["VRUTrackingCollection"] == null)
				{ 
					this.Session["VRUTrackingCollection"] = FMChannelHelper.MakeCall<IVruTrackings, VRUTrackingCollectionClass>(x => x.Enumerate(this.Security));
				}
				var vruTrackingCollection =
					 (VRUTrackingCollectionClass)this.Session["VRUTrackingCollection"];

				this.VRUTrackingFormPageSizeDropDown.SetPageSize(this.VRUConfigurationDataGrid, vruTrackingCollection.Count);

				this.VRUConfigurationDataGrid.DataSource = vruTrackingCollection;
				this.VRUConfigurationDataGrid.DataBind();

				if (vruTrackingCollection.Count > 0)
				{
					this.FMLabelupdatedDate.Text = vruTrackingCollection[0].LastCalculationDate?.Value.ToString("G", CultureInfo.CurrentUICulture) ?? "Unknown";
				}
				else
				{
					this.FMLabelupdatedDate.Text = "Unknown";
				}

				int? calculationInterval = FMChannelHelper.MakeCall<IVruTrackings, int?>(x => x.GetAutoCalculationInterval(this.Security));
				this.FMLabelAutoIntervalMinutes.Text = calculationInterval?.ToString() ?? "Unknown";
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
			finally
			{
				this.EnableControls(true);
			}
		}

		protected void AddButtonCommand(object sender, CommandEventArgs e)
		{
			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));
			var vruTrackingCollection = (VRUTrackingCollectionClass)this.Session["VRUTrackingCollection"];
			var vruTracking = new VruTrackingClass
			{
				Limit =
					new SIDouble(
					site.GetSiteUnits(SITE_VARIABLE_TYPE.VOLUME),
					site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME),
					0),
				CurrentValue =
					new SIDouble(
					site.GetSiteUnits(SITE_VARIABLE_TYPE.VOLUME),
					site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME),
					0),
				Enabled = true,
				Tolerance = 0.0
			};

			vruTrackingCollection.Add(vruTracking);

			this.VRUConfigurationDataGrid.CurrentPageIndex = (vruTrackingCollection.Count - 1) / this.VRUConfigurationDataGrid.PageSize;
			this.VRUConfigurationDataGrid.EditItemIndex = (vruTrackingCollection.Count - 1) % this.VRUConfigurationDataGrid.PageSize;
			this.EnableControls(false);
			this.CalculateRunningTotals();
			this.UpdateView();
		}

		protected void RefreshButtonCommand(object sender, CommandEventArgs e)
		{
			try
			{
				this.EnableControls(false);
				this.Session["VRUTrackingCollection"] = null;
				this.CalculateRunningTotals();
				this.UpdateView();
			}
			finally
			{
				this.EnableControls(true);
			}
		}

		private void AssignProductsButtonCommand(object sender, CommandEventArgs e)
		{
			try
			{
				ListItem unassignedProductItem;
				while ((unassignedProductItem = this.UnassignedProductsListBox.SelectedItem) != null)
				{
					this.UnassignedProductsListBox.Items.Remove(unassignedProductItem);
					unassignedProductItem.Selected = false;

					foreach (ListItem assignedProductItem in this.AssignedProductsListBox.Items)
					{
						if (string.Compare(assignedProductItem.Text, unassignedProductItem.Text, StringComparison.Ordinal) > 0)
						{
							int index = this.AssignedProductsListBox.Items.IndexOf(assignedProductItem);
							this.AssignedProductsListBox.Items.Insert(index, unassignedProductItem);

							this.SaveProductAssignment(unassignedProductItem);
							unassignedProductItem = null;
							break;
						}
					}

					if (unassignedProductItem != null)
					{
						this.AssignedProductsListBox.Items.Add(unassignedProductItem);
						this.SaveProductAssignment(unassignedProductItem);
					}
				}

				// reload the data to recalculate the current values
				this.Session["VRUTrackingCollection"] = null;
				this.CalculateRunningTotals();
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void SaveProductAssignment(ListItem unassignedProductItem)
		{
			FMChannelHelper.MakeCall<IProductMaps>(x => x.Add(
				 this.Security,
				 new ProductMapClass
				 {
					 AssignedToGuid = this.Security.SiteGuid,
					 AssignedGuid = Guid.Parse(unassignedProductItem.Value),
					 AssignedID = unassignedProductItem.ToString(),
					 Type = PRODUCT_MAP_TYPE.VRU_VCU_TRACKING
				 }));
		}

		private void UnassignProductsButtonCommand(object sender, CommandEventArgs e)
		{
			try
			{
				ListItem assignedProductItem;
				while ((assignedProductItem = this.AssignedProductsListBox.SelectedItem) != null)
				{
					this.AssignedProductsListBox.Items.Remove(assignedProductItem);
					assignedProductItem.Selected = false;

					foreach (ListItem unassignedProductItem in this.UnassignedProductsListBox.Items)
					{
						if (string.Compare(
							 unassignedProductItem.Text,
							 assignedProductItem.Text,
							 StringComparison.Ordinal) > 0)
						{
							int index = this.UnassignedProductsListBox.Items.IndexOf(unassignedProductItem);
							this.UnassignedProductsListBox.Items.Insert(index, assignedProductItem);

							this.SaveProductUnassignment(assignedProductItem);

							assignedProductItem = null;
							break;
						}
					}

					if (assignedProductItem != null)
					{
						this.UnassignedProductsListBox.Items.Add(assignedProductItem);
						this.SaveProductUnassignment(assignedProductItem);
					}
				}

				// reload the data to recalculate the current values
				this.Session["VRUTrackingCollection"] = null;
				this.CalculateRunningTotals();
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void SaveProductUnassignment(ListItem assignedProductItem)
		{
			var unassignProductGuid = FMChannelHelper.MakeCall<IProductMaps, Guid>(x => x.GetIdentityGuid(
				this.Security,
				this.Security.SiteGuid,
				Guid.Parse(assignedProductItem.Value),
				PRODUCT_MAP_TYPE.VRU_VCU_TRACKING));
			if (unassignProductGuid != Guid.Empty)
			{
				FMChannelHelper.MakeCall<IProductMaps>(x => x.Purge(this.Security, unassignProductGuid, PRODUCT_MAP_TYPE.VRU_VCU_TRACKING));
			}
		}

		protected ListItemCollection EnumerateIntervals()
		{
			var typeItems = new ListItemCollection();

			for (var type = VRU_INTERVAL_TYPE.Minute; type <= VRU_INTERVAL_TYPE.Year; type++)
			{
				string frequencyID = this.GetTranslatedText(VruTrackingClass.IntervalTypeID(type));
				var newTypeItem = new ListItem(frequencyID, ((int)type).ToString(CultureInfo.InvariantCulture));
				typeItems.Add(newTypeItem);
			}

			return typeItems;
		}

		// ReSharper disable once InconsistentNaming
		protected void PageSizeDropDown_SelectedIndexChanged(object source, EventArgs e)
		{
			this.UpdateView();
		}

		// ReSharper disable once InconsistentNaming
		private void VRUConfigurationDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemIndex != -1)
			{
				var editButton = (LinkButton)e.Item.FindControl("EditButton");
				var deleteButton = (LinkButton)e.Item.FindControl("DeleteButton");
				if (editButton != null
					&& deleteButton != null)
				{
					editButton.Enabled = this.Security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA);
					deleteButton.Enabled = this.Security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA);
				}

				var vruTrackingCollection = (VRUTrackingCollectionClass)this.Session["VRUTrackingCollection"];
				int index = (this.VRUConfigurationDataGrid.CurrentPageIndex * this.VRUConfigurationDataGrid.PageSize) + e.Item.ItemIndex;
				VruTrackingClass vru = vruTrackingCollection[index];

				if (this.VRUConfigurationDataGrid.EditItemIndex == e.Item.ItemIndex)
				{
					var intervalTypeDropDownList = (DropDownList)e.Item.FindControl("DropDownTypeList");
					ListItem item = intervalTypeDropDownList.Items.FindByValue(((int)vru.IntervalType).ToString(CultureInfo.InvariantCulture));
					intervalTypeDropDownList.SelectedIndex = intervalTypeDropDownList.Items.IndexOf(item);
				}
				else
				{
					var intervalTypeLabel = (Label)e.Item.FindControl("IntervalTypeLabel");
					if (intervalTypeLabel != null)
					{
						intervalTypeLabel.Text = VruTrackingClass.IntervalTypeID(vru.IntervalType);
					}
				}
			}
		}

		// ReSharper disable once InconsistentNaming
		protected void VRUConfigurationDataGrid_EditCommand(object source, DataGridCommandEventArgs e)
		{
			this.VRUConfigurationDataGrid.EditItemIndex = e.Item.ItemIndex;
			this.EnableControls(false);

			this.UpdateView();
		}

		// ReSharper disable once InconsistentNaming
		protected void VRUConfigurationDataGrid_CancelCommand(object source, DataGridCommandEventArgs e)
		{
			var vruTrackingCollection = (VRUTrackingCollectionClass)this.Session["VRUTrackingCollection"];
			int index = (this.VRUConfigurationDataGrid.CurrentPageIndex * this.VRUConfigurationDataGrid.PageSize) + e.Item.ItemIndex;
			VruTrackingClass vruTracking = vruTrackingCollection[index];
			if (vruTracking.IdentityGuid == Guid.Empty)
			{
				vruTrackingCollection.RemoveAt(index);
				if (this.VRUConfigurationDataGrid.Items.Count == 1 && this.VRUConfigurationDataGrid.CurrentPageIndex > 0)
				{
					this.VRUConfigurationDataGrid.CurrentPageIndex--;
				}
			}

			this.VRUConfigurationDataGrid.EditItemIndex = -1;
			this.EnableControls(true);
			this.Session.Remove("VRUTrackingCollection");
			this.UpdateView();
		}

		// ReSharper disable once InconsistentNaming
		protected void VRUConfigurationDataGrid_UpdateCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				var vruTrackingCollection =
					 (VRUTrackingCollectionClass)this.Session["VRUTrackingCollection"];

				int index = (this.VRUConfigurationDataGrid.CurrentPageIndex * this.VRUConfigurationDataGrid.PageSize) + this.VRUConfigurationDataGrid.EditItemIndex;
				VruTrackingClass vruTracking = vruTrackingCollection[index];

				var intervalTextBox = (TextBox)e.Item.FindControl("IntervalTextBox");
				int interval;
				if (!int.TryParse(intervalTextBox.Text, out interval))
				{
					throw new FormatException("Interval only accepts integers.");
				}

				vruTracking.Interval = interval;

				// vruTracking.Interval = int.Parse(intervalTextBox.Text);
				var limitTextBox = (TextBox)e.Item.FindControl("LimitTextBox");

				double limit;
				if (!double.TryParse(limitTextBox.Text, out limit))
				{
					throw new FormatException("Invalid numeric format for Limit.");
				}

				if (limit <= 0)
				{
					throw new FormatException("Limit must be a number greater than zero.");
				}

				vruTracking.Limit.Value = limit;

				var toleranceTextBox = (TextBox)e.Item.FindControl("ToleranceTextBox");
				double tolerance;
				if (!double.TryParse(toleranceTextBox.Text, out tolerance))
				{
					throw new FormatException("Invalid numeric format for Tolerance.");
				}

				if (tolerance < 0 || tolerance >= 100)
				{
					throw new FormatException("Tolerance must be between zero and 100.");
				}

				vruTracking.Tolerance = tolerance;

				var enabledEditCheckbox = (CheckBox)e.Item.FindControl("EnabledEditCheckbox");
				vruTracking.Enabled = enabledEditCheckbox.Checked;

				var typeListDropDown = (DropDownList)e.Item.FindControl("DropDownTypeList");
				vruTracking.IntervalType = (VRU_INTERVAL_TYPE)int.Parse(typeListDropDown.SelectedItem.Value);

				vruTracking.ID = vruTracking.Interval.ToString(CultureInfo.InvariantCulture) + " "
									  + VruTrackingClass.IntervalTypeID(vruTracking.IntervalType);

				if (vruTracking.IdentityGuid == Guid.Empty)
				{
					vruTracking.IdentityGuid = FMChannelHelper.MakeCall<IVruTrackings, Guid>(x => x.Add(this.Security, vruTracking));
				}
				else
				{
					FMChannelHelper.MakeCall<IVruTrackings>(x => x.Modify(this.Security, vruTracking));
				}

				this.EnableControls(true);
				this.VRUConfigurationDataGrid.EditItemIndex = -1;
				this.Session.Remove("VRUTrackingCollection");
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}

			try
			{
				this.CalculateRunningTotals();
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		// ReSharper disable once InconsistentNaming
		protected void VRUConfigurationDataGrid_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				var vruTrackingCollection = (VRUTrackingCollectionClass)this.Session["VRUTrackingCollection"];

				int index = (this.VRUConfigurationDataGrid.CurrentPageIndex * this.VRUConfigurationDataGrid.PageSize) + e.Item.ItemIndex;
				VruTrackingClass vruTracking = vruTrackingCollection[index];

				if (this.VRUConfigurationDataGrid.EditItemIndex == e.Item.ItemIndex)
				{
					this.VRUConfigurationDataGrid.EditItemIndex = -1;
					this.EnableControls(true);
				}
				else if (this.VRUConfigurationDataGrid.EditItemIndex > e.Item.ItemIndex)
				{
					this.VRUConfigurationDataGrid.EditItemIndex--;
				}

				// Non Zero Index indicates Message has been committed to database
				if (vruTracking.IdentityGuid != Guid.Empty)
				{
					FMChannelHelper.MakeCall<IVruTrackings>(x => x.Purge(this.Security, vruTracking.IdentityGuid));
				}

				vruTrackingCollection.RemoveAt(index);
				if (this.VRUConfigurationDataGrid.Items.Count == 1 && this.VRUConfigurationDataGrid.CurrentPageIndex > 0)
				{
					this.VRUConfigurationDataGrid.CurrentPageIndex--;
				}

				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		// ReSharper disable once InconsistentNaming
		protected void VRUConfigurationDataGrid_ItemCommand(object source, DataGridCommandEventArgs e)
		{

			if (e.CommandName == "ResetDate")
			{
				Guid id = Guid.Parse(e.CommandArgument.ToString());
				FMChannelHelper.MakeCall<IVruTrackings>(x => x.UpdateResetDate(this.Security, id));

				this.Session.Remove("VRUTrackingCollection");
				this.UpdateView();
			}
		}

		// ReSharper disable once InconsistentNaming
		protected void VRUConfigurationDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.VRUConfigurationDataGrid.EditItemIndex > -1)
			{
				return;
			}

			this.VRUConfigurationDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateView();
		}

		protected void EnableControls(bool enable)
		{
			this.AddButton.Enabled = enable;
			this.AddButton2.Enabled = enable;
			this.RefreshButton.Enabled = enable;
			this.VRUTrackingFormPageSizeDropDown.Enabled = enable;
			this.AssignProductsButton.Enabled = enable;
			this.UnassignProductsButton.Enabled = enable;
			this.AssignedProductsListBox.Enabled = enable;
			this.UnassignedProductsListBox.Enabled = enable;
		}

		protected void CalculateRunningTotals()
		{
			FMChannelHelper.MakeCall<IVruTrackings>(x => x.CalculateRunningTotals(this.Security));
		}
	}
}
