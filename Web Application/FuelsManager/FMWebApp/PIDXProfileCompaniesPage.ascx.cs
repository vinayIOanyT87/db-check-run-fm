// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PIDXProfileCompaniesPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the PIDXProfileCompaniesPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using global::FMWebApp;

	/// <summary>
	///    Summary description for PIDXProfileCompaniesPage.
	/// </summary>
	public partial class PIDXProfileCompaniesPage : FMUserControlBase
	{
		#region Public Methods and Operators

		public void ItemShipToTextBox_TextChanged(object sender, EventArgs e)
		{
			try
			{
				DataGridItem Item = this.PIDXProfileCompaniesDataGrid.Items[this.PIDXProfileCompaniesDataGrid.EditItemIndex];
				var ShipToTextBox = Item.FindControl("ShipToTextBox") as TextBox;
				var LoadIDDropDownList = Item.FindControl("LoadIDDropDownList") as DropDownList;

				if (ShipToTextBox == null || LoadIDDropDownList == null)
				{
					throw new Exception("Invalid Controls");
				}

				var PIDXProfile = this.Session["PIDXProfile"] as PIDXProfileClass;
				PIDXProfileCompanyMapCollectionClass PIDXProfileCompanyMapCollection = PIDXProfile.PIDXProfileCompanyMapCollection;
				int Index = this.PIDXProfileCompaniesDataGrid.CurrentPageIndex * this.PIDXProfileCompaniesDataGrid.PageSize
				            + this.PIDXProfileCompaniesDataGrid.EditItemIndex;
				PIDXProfileCompanyMapClass PIDXProfileCompanyMap = PIDXProfileCompanyMapCollection[Index];

				PIDXProfileCompanyMap.ShipToID = ShipToTextBox.Text;

				Guid shipToMasterGuid = FMChannelHelper.MakeCall<ICompanies, Guid>(
																	 x =>
																	 x.GetMasterRecordGuid(this.Security, PIDXProfileCompanyMap.ShipToID)
																);
				if (shipToMasterGuid == Guid.Empty)
				{
					throw new Exception("Invalid ShipTo Company");
				}
				CompanyMapCollectionClass ShipToBillToMapCollection = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
																	 x =>
																	 x.EnumerateByAssignedGuidAndType(this.Security, shipToMasterGuid, COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP)
																);
				LoadIDDropDownList.Items.Clear();

				bool SetCompanyData = true;

				foreach (CompanyMapClass ShipToBillToMap in ShipToBillToMapCollection)
				{
					if (SetCompanyData)
					{
						PIDXProfileCompanyMap.ShipToName = ShipToBillToMap.AssignedName;
						PIDXProfileCompanyMap.ShipToState = ShipToBillToMap.AssignedState;
						PIDXProfileCompanyMap.ShipToCity = ShipToBillToMap.AssignedCity;
						PIDXProfileCompanyMap.ShipToAddress = ShipToBillToMap.AssignedAddress;
						SetCompanyData = false;
					}

					CompanyMapCollectionClass LoadIDToShipToMapCollection = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
																	 x =>
																	 x.EnumerateByAssignedToGuidAndType(this.Security, ShipToBillToMap.IdentityGuid, COMPANY_MAP_TYPE.LOADID_SHIPTO_MAP)
																);

					foreach (CompanyMapClass LoadIDToShipToMap in LoadIDToShipToMapCollection)
					{
						LoadIDDropDownList.Items.Add(new ListItem(LoadIDToShipToMap.MapID, LoadIDToShipToMap.IdentityGuid.ToString()));
					}
				}
			}

			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		#endregion

		#region Methods

		protected void AddButton_Command(object sender, CommandEventArgs e)
		{
			var PIDXProfile = this.Session["PIDXProfile"] as PIDXProfileClass;
			PIDXProfileCompanyMapCollectionClass PIDXProfileCompanyMapCollection = PIDXProfile.PIDXProfileCompanyMapCollection;
			var PIDXProfileCompanyMap = new PIDXProfileCompanyMapClass();
			PIDXProfileCompanyMapCollection.Add(PIDXProfileCompanyMap);
			this.PIDXProfileCompaniesDataGrid.CurrentPageIndex = (PIDXProfileCompanyMapCollection.Count - 1)
			                                                     / this.PIDXProfileCompaniesDataGrid.PageSize;
			this.PIDXProfileCompaniesDataGrid.EditItemIndex = (PIDXProfileCompanyMapCollection.Count - 1)
			                                                  % this.PIDXProfileCompaniesDataGrid.PageSize;
			this.EnableControls(false);
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

		protected void PIDXProfileCompaniesDataGrid_CancelCommand(object source, DataGridCommandEventArgs e)
		{
			var PIDXProfile = this.Session["PIDXProfile"] as PIDXProfileClass;
			PIDXProfileCompanyMapCollectionClass PIDXProfileCompanyMapCollection = PIDXProfile.PIDXProfileCompanyMapCollection;
			int Index = this.PIDXProfileCompaniesDataGrid.CurrentPageIndex * this.PIDXProfileCompaniesDataGrid.PageSize
			            + e.Item.ItemIndex;
			PIDXProfileCompanyMapClass PIDXProfileCompanyMap = PIDXProfileCompanyMapCollection[Index];

			// If the user has not clicked the green check yet, delete the row.
			if (PIDXProfileCompanyMap.ID.Length == 0)
			{
				PIDXProfileCompanyMapCollection.Remove(Index);
				if (this.PIDXProfileCompaniesDataGrid.Items.Count == 1 && this.PIDXProfileCompaniesDataGrid.CurrentPageIndex > 0)
				{
					this.PIDXProfileCompaniesDataGrid.CurrentPageIndex--;
				}
			}
			this.PIDXProfileCompaniesDataGrid.EditItemIndex = -1;
			this.EnableControls(true);
			this.UpdateView();
		}

		protected void PIDXProfileCompaniesDataGrid_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			var PIDXProfile = this.Session["PIDXProfile"] as PIDXProfileClass;
			PIDXProfileCompanyMapCollectionClass PIDXProfileCompanyMapCollection = PIDXProfile.PIDXProfileCompanyMapCollection;
			int Index = this.PIDXProfileCompaniesDataGrid.CurrentPageIndex * this.PIDXProfileCompaniesDataGrid.PageSize
			            + e.Item.ItemIndex;
			PIDXProfileCompanyMapClass PIDXProfileCompanyMap = PIDXProfileCompanyMapCollection[Index];

			if (this.PIDXProfileCompaniesDataGrid.EditItemIndex == e.Item.ItemIndex)
			{
				this.PIDXProfileCompaniesDataGrid.EditItemIndex = -1;
				this.EnableControls(true);
			}
			else if (this.PIDXProfileCompaniesDataGrid.EditItemIndex > e.Item.ItemIndex)
			{
				this.PIDXProfileCompaniesDataGrid.EditItemIndex--;
			}

			PIDXProfileCompanyMapCollection.Remove(Index);
			if (this.PIDXProfileCompaniesDataGrid.Items.Count == 1 && this.PIDXProfileCompaniesDataGrid.CurrentPageIndex > 0)
			{
				this.PIDXProfileCompaniesDataGrid.CurrentPageIndex--;
			}

			this.UpdateView();
		}

		protected void PIDXProfileCompaniesDataGrid_EditCommand(object source, DataGridCommandEventArgs e)
		{
			this.PIDXProfileCompaniesDataGrid.EditItemIndex = e.Item.ItemIndex;
			this.EnableControls(false);
			this.UpdateView();
		}

		protected void PIDXProfileCompaniesDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.PIDXProfileCompaniesDataGrid.EditItemIndex > -1)
			{
				return;
			}
			this.PIDXProfileCompaniesDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateView();
		}

		protected void PIDXProfileCompaniesDataGrid_UpdateCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				var PIDXProfile = this.Session["PIDXProfile"] as PIDXProfileClass;
				PIDXProfileCompanyMapCollectionClass PIDXProfileCompanyMapCollection = PIDXProfile.PIDXProfileCompanyMapCollection;
				int Index = this.PIDXProfileCompaniesDataGrid.CurrentPageIndex * this.PIDXProfileCompaniesDataGrid.PageSize
				            + e.Item.ItemIndex;
				PIDXProfileCompanyMapClass PIDXProfileCompanyMap = PIDXProfileCompanyMapCollection[Index];

				var LoadIDDropDownList = e.Item.FindControl("LoadIDDropDownList") as DropDownList;
				if (LoadIDDropDownList == null)
				{
					throw new Exception("Invalid LoadIDDropDownList");
				}

				if (LoadIDDropDownList.SelectedIndex != -1)
				{
					CompanyMapClass LoadIDCompanyMap = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapClass>(
																	 x =>
																	 x.Get(this.Security, Guid.Parse(LoadIDDropDownList.SelectedValue), COMPANY_MAP_TYPE.LOADID_SHIPTO_MAP)
																);

					PIDXProfileCompanyMap.CompanyPersonnelToShipToBillToGuid = Guid.Parse(LoadIDDropDownList.SelectedValue);
					PIDXProfileCompanyMap.ID = LoadIDDropDownList.SelectedItem.Text;

					var SellerIDTextBox = (TextBox)e.Item.FindControl("SellerIDTextBox");
					PIDXProfileCompanyMap.SellerID = SellerIDTextBox.Text;

					var ShipperIDTextBox = (TextBox)e.Item.FindControl("ShipperIDTextBox");
					PIDXProfileCompanyMap.ShipperID = ShipperIDTextBox.Text;

					var ConsigneeNumberTextBox = (TextBox)e.Item.FindControl("ConsigneeNumberTextBox");
					PIDXProfileCompanyMap.ConsigneeNumber = ConsigneeNumberTextBox.Text;

					var DenialOverrideCheckBox = (CheckBox)e.Item.FindControl("DenialOverrideCheckBox");
					PIDXProfileCompanyMap.DenialOverride = DenialOverrideCheckBox.Checked;

					var UnavailableOverrideCheckBox = (CheckBox)e.Item.FindControl("UnavailableOverrideCheckBox");
					PIDXProfileCompanyMap.UnavailableOverride = UnavailableOverrideCheckBox.Checked;
				}
				else
				{
					PIDXProfileCompanyMapCollection.Remove(Index);
				}

				this.EnableControls(true);
				this.PIDXProfileCompaniesDataGrid.EditItemIndex = -1;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void PageSizeDropDown_SelectedIndexChanged(object source, EventArgs e)
		{
			this.UpdateView();
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				if (!this.Page.IsPostBack)
				{
					if (!this.Security.HasRight(RIGHT.MODIFY_PIDX_PROFILES)) // vthompson 5773
					{
						this.AddButton.Enabled = false;
						this.AddButton2.Enabled = false;
					}

					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void EnableControls(bool enable)
		{
			this.AddButton2.Enabled = enable;
			this.AddButton.Enabled = enable;

			// Call the main form to disable buttons and tabs.
			var pidxProfileForm = (PIDXProfileForm)this.Page;
			pidxProfileForm.EnableControls(enable);
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AddButton2.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);
			this.PIDXProfileCompaniesDataGrid.EditCommand +=
				new System.Web.UI.WebControls.DataGridCommandEventHandler(this.PIDXProfileCompaniesDataGrid_EditCommand);
			this.PIDXProfileCompaniesDataGrid.PageIndexChanged +=
				new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.PIDXProfileCompaniesDataGrid_PageIndexChanged);
			this.PIDXProfileCompaniesDataGrid.CancelCommand +=
				new System.Web.UI.WebControls.DataGridCommandEventHandler(this.PIDXProfileCompaniesDataGrid_CancelCommand);
			this.PIDXProfileCompaniesDataGrid.UpdateCommand +=
				new System.Web.UI.WebControls.DataGridCommandEventHandler(this.PIDXProfileCompaniesDataGrid_UpdateCommand);
			this.PIDXProfileCompaniesDataGrid.DeleteCommand +=
				new System.Web.UI.WebControls.DataGridCommandEventHandler(this.PIDXProfileCompaniesDataGrid_DeleteCommand);
			this.PIDXProfileCompaniesDataGrid.ItemDataBound +=
				new System.Web.UI.WebControls.DataGridItemEventHandler(this.PIDXProfileCompaniesDataGrid_ItemDataBound);
			this.AddButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);
		}

		private void PIDXProfileCompaniesDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			try
			{
				if (e.Item.ItemIndex != -1)
				{
					var EditButton = e.Item.FindControl("Fmeditlinkbutton1") as LinkButton;
					var DeleteButton = e.Item.FindControl("Fmdeletelinkbutton1") as LinkButton;
					if (EditButton != null && DeleteButton != null)
					{
						EditButton.Enabled = this.Security.HasRight(RIGHT.MODIFY_PIDX_PROFILES); //vthompson CSI 5773
						DeleteButton.Enabled = this.Security.HasRight(RIGHT.MODIFY_PIDX_PROFILES); //vthompson CSI 5773
					}

					var PIDXProfile = this.Session["PIDXProfile"] as PIDXProfileClass;
					PIDXProfileCompanyMapCollectionClass PIDXProfileCompanyMapCollection = PIDXProfile.PIDXProfileCompanyMapCollection;
					int Index = this.PIDXProfileCompaniesDataGrid.CurrentPageIndex * this.PIDXProfileCompaniesDataGrid.PageSize
					            + e.Item.ItemIndex;
					PIDXProfileCompanyMapClass PIDXProfileCompanyMap = PIDXProfileCompanyMapCollection[Index];

					if (this.PIDXProfileCompaniesDataGrid.EditItemIndex != e.Item.ItemIndex)
					{
						var CompanyLabel = (Label)e.Item.FindControl("CompanyLabel");
						CompanyLabel.ToolTip = PIDXProfileCompanyMap.ShipToToolTip;

						var DenialOverrideCheckBox = e.Item.FindControl("DenialOverrideCheckBox") as CheckBox;
						DenialOverrideCheckBox.Enabled = false;

						var UnavailableOverrideCheckBox = e.Item.FindControl("UnavailableOverrideCheckBox") as CheckBox;
						UnavailableOverrideCheckBox.Enabled = false;
					}
					else
					{
						Guid shipToGuid = FMChannelHelper.MakeCall<ICompanies, Guid>(
																	 x =>
																	 x.GetMasterRecordGuid(this.Security, PIDXProfileCompanyMap.ShipToID)
																);

						if (shipToGuid != Guid.Empty)
						{
							var ShipToTextBox = e.Item.FindControl("ShipToTextBox") as TextBox;
							ShipToTextBox.Text = PIDXProfileCompanyMap.ShipToID;
							ShipToTextBox.ToolTip = PIDXProfileCompanyMap.ShipToToolTip;

							var LoadIDDropDownList = e.Item.FindControl("LoadIDDropDownList") as DropDownList;

							CompanyMapCollectionClass ShipToBillToMapCollection = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
																	 x =>
																	 x.EnumerateByAssignedGuidAndType(this.Security, shipToGuid, COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP)
																);


							foreach (CompanyMapClass ShipToBillToMap in ShipToBillToMapCollection)
							{
								CompanyMapCollectionClass LoadIDToShipToMapCollection =
									FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
																	 x =>
																	 x.EnumerateByAssignedToGuidAndType(this.Security, ShipToBillToMap.IdentityGuid, COMPANY_MAP_TYPE.LOADID_SHIPTO_MAP)
																);

								foreach (CompanyMapClass LoadIDToShipToMap in LoadIDToShipToMapCollection)
								{
									LoadIDDropDownList.Items.Add(new ListItem(LoadIDToShipToMap.MapID, LoadIDToShipToMap.IdentityGuid.ToString()));
									if (LoadIDToShipToMap.IdentityGuid == PIDXProfileCompanyMap.CompanyPersonnelToShipToBillToGuid)
									{
										LoadIDDropDownList.SelectedIndex = LoadIDDropDownList.Items.Count - 1;
									}
								}
							}
						}
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
				var PIDXProfile = this.Session["PIDXProfile"] as PIDXProfileClass;

				PIDXProfileCompanyMapCollectionClass PIDXProfileCompanyMapCollection = PIDXProfile.PIDXProfileCompanyMapCollection;

				this.PIDXProfileCompaniesPageSizeDropDown.SetPageSize(
					this.PIDXProfileCompaniesDataGrid, PIDXProfileCompanyMapCollection.Count);

				this.PIDXProfileCompaniesDataGrid.DataSource = PIDXProfileCompanyMapCollection;
				this.PIDXProfileCompaniesDataGrid.DataBind();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		#endregion
	}
}