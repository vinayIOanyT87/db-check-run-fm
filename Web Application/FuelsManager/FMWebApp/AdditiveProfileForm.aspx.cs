/******************************************************************************
	FILE NAME:		AdditiveProfileForm.aspx.cs
	PURPOSE:		Implementation of AdditiveProfileForm

	COMMENTS:
		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
		This file shall not be copied or reproduced in any form without
		the express written consent of Endress+Hauser.

	AUTHOR(S):	W. Gray
	VERSION:	1.0.0  Current version

	MODIFICATION HISTORY:
		Date:			By:					Reason:
		----------	-----------------	-------------------------------------------
		2007-02-09	Richard Panachida	Added an override method to disable/enable controls. 
												Currently, it disables/enables the Add button (CSI 4083).
  
		2009-06-08	W.Gray				7.5.1.0 - Added Description (CSI 4008)

		2009-06-08	W.Gray				7.5.1.1 - Added Treat Rate (CSI 4009)
*******************************************************************************/

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
	/// Summary description for AdditiveProfileForm.
	/// </summary>
	public partial class AdditiveProfileForm : FMAutoSubmitFormBase
	{

		private void UpdateAdditivesView()
		{
			this.AdditivesDataGrid.DataSource = this.EnumerateAdditives();
			this.AdditivesDataGrid.DataBind();
		}

		private ICollection EnumerateAdditives()
		{
			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(this.Security, this.Security.SiteGuid, getMemberSites: false, getSchedulesAndProcessVariables: false, bGetAssociatedAliases: true)
																);
			AdditiveProfileClass AdditiveProfile = (AdditiveProfileClass)this.Session["AdditiveProfile"];

			DataTable AdditiveDataTable = new DataTable();
			DataRow AdditiveDataRow;

			AdditiveDataTable.Columns.Add("Index", typeof(Int32));
			AdditiveDataTable.Columns.Add("ID", typeof(string));
			AdditiveDataTable.Columns.Add("Rate", typeof(string));
			AdditiveDataTable.Columns.Add("CycleVolume", typeof(string));
			AdditiveDataTable.Columns.Add("TreatRate", typeof(string));
            AdditiveDataTable.Columns.Add("DesiredTreatRate", typeof(string));
			AdditiveDataTable.Columns.Add("Tolerance", typeof(string));

			int Item = 0;
			foreach (ProductMapClass Additive in AdditiveProfile.AdditiveCollection)
			{
				AdditiveDataRow = AdditiveDataTable.NewRow();

				AdditiveDataRow["Index"] = Item;
				AdditiveDataRow["ID"] = Additive.AssignedID;
				AdditiveDataRow["Rate"] = Additive.AdditiveRate;
				AdditiveDataRow["CycleVolume"] = Additive.AdditiveCycleVolume;

				SIDouble AdditiveRate = new SIDouble(site.AdditiveProfileCycleAmountUnits, site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.ADDITIVE_CYCLE_AMOUNT), Additive._AdditiveRate.SIValue);
				if (AdditiveRate.Value != 0.0)
				{
					double treatRate;
					treatRate = Additive._AdditiveCycleVolume.Value / AdditiveRate.Value;
					AdditiveDataRow["TreatRate"] = treatRate.ToString("f8");
				}
				else
					AdditiveDataRow["TreatRate"] = "";

                if (Additive.DesiredTreatRate != 0.0)
                {
                    AdditiveDataRow["DesiredTreatRate"] = Additive.DesiredTreatRate.ToString("f8");
                }
                else
                {
                    AdditiveDataRow["DesiredTreatRate"] = string.Empty;
                }

                AdditiveDataRow["Tolerance"] = Additive.Tolerance.ToString(site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT));

				AdditiveDataTable.Rows.Add(AdditiveDataRow);
				Item++;
			}

			DataView AdditiveDataView = new DataView(AdditiveDataTable);
			return AdditiveDataView;
		}


		protected ListItemCollection EnumerateAdditiveProducts()
		{
			AdditiveProfileClass AdditiveProfile = (AdditiveProfileClass)this.Session["AdditiveProfile"];

			int iIndex = this.AdditivesDataGrid.CurrentPageIndex * this.AdditivesDataGrid.PageSize + this.AdditivesDataGrid.EditItemIndex;
			DataView AdditivesDataView = (DataView)this.AdditivesDataGrid.DataSource;

			ProductCollectionClass AdditiveCollection = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(
				products => products.EnumerateByType(this.Security, ProductType.AdditiveProduct));

			ListItemCollection AdditiveItems = new ListItemCollection();

			for (int iItem = 0; iItem < AdditiveCollection.Count; iItem++)
			{
				ProductClass Additive = AdditiveCollection[iItem];

				for (int iExistingItem = 0; iExistingItem < AdditiveProfile.AdditiveCollection.Count; iExistingItem++)
				{
					ProductMapClass Map = AdditiveProfile.AdditiveCollection[iExistingItem];

					if (this.AdditivesDataGrid.EditItemIndex != -1 &&
					iExistingItem == (int)AdditivesDataView[iIndex][0])
						continue;

					if (Additive.MasterRecordGuid == Map.AssignedGuid)
					{
						Additive = null;
						break;
					}
				}

				if (Additive == null)
					continue;


				AdditiveItems.Add(new ListItem(Additive.ID, Additive.MasterRecordGuid.ToString()));
			}

			if (AdditiveItems.Count == 0)
				throw new Exception("No Additives Available");

			return AdditiveItems;
		}


		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				this.GetSecurity();

				AdditiveProfileClass AdditiveProfile;

				if (!this.Page.IsPostBack)
				{
					// Get IdentityGuid
					if (this.Session["IdentityGuid"] != null)
					{
						// Get AdditiveProfile
						AdditiveProfile = FMChannelHelper.MakeCall<IAdditiveProfiles, AdditiveProfileClass>(
							additiveProfiles => additiveProfiles.Get(this.Security, Guid.Parse(this.Session["IdentityGuid"] as string)));

						this.AdditiveProfileIDTextbox.Text = AdditiveProfile.ID;
						this.AdditiveProfileDescriptionTextbox.Text = AdditiveProfile.Description;
					}
					else
						AdditiveProfile = new AdditiveProfileClass();

					this.Session["AdditiveProfile"] = AdditiveProfile;

					if (!this.Security.HasRight(RIGHT.MODIFY_PRODUCTS)
						|| (this.Security.SiteGuid != AdditiveProfile.SiteGuid
						&& AdditiveProfile.SiteGuid != Guid.Empty))
					{
						this.OK.Enabled = false;
						this.AddButton.Enabled = false;
					}

					//Set the title label with a key field from the bound object appended
					if (AdditiveProfile != null)
					{
						this.AdditiveProfileTitleLabel.Text = this.GetTitleLabelText(this.AdditiveProfileTitleLabel.Text, AdditiveProfile.ID);
					}

					this.UpdateAdditivesView();
				}
				else
					AdditiveProfile = (AdditiveProfileClass)this.Session["AdditiveProfile"];

			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// This method will enable and disable controls.
		/// </summary>
		/// <param name="enable"></param>
		private void EnableControls(bool enable)
		{
			AdditiveProfileClass AdditiveProfile;

			// Get identityGuid
			if (this.Session["IdentityGUid"] != null)
			{
				// Get AdditiveProfile
				AdditiveProfile = FMChannelHelper.MakeCall<IAdditiveProfiles, AdditiveProfileClass>(
					additiveProfiles => additiveProfiles.Get(this.Security, Guid.Parse(this.Session["IdentityGuid"] as string)));

				this.AdditiveProfileIDTextbox.Text = AdditiveProfile.ID;
			}
			else
				AdditiveProfile = new AdditiveProfileClass();

			if (this.Security.HasRight(RIGHT.MODIFY_PRODUCTS)
				&& (this.Security.SiteGuid == AdditiveProfile.SiteGuid || AdditiveProfile.SiteGuid == Guid.Empty))
			{
				this.AddButton.Enabled = enable;
				this.OK.Enabled = enable;
				this.AdditiveProfileIDTextbox.Enabled = enable;
			}

			this.Cancel.Enabled = enable;
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeAdditive();
			base.OnInit(e);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeAdditive()
		{
			this.AdditivesDataGrid.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.AdditivesDataGrid_PageIndexChanged);
			this.AdditivesDataGrid.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.AdditivesDataGrid_CancelCommand);
			this.AdditivesDataGrid.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.AdditivesDataGrid_EditCommand);
			this.AdditivesDataGrid.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.AdditivesDataGrid_UpdateCommand);
			this.AdditivesDataGrid.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.AdditivesDataGrid_DeleteCommand);
			this.AdditivesDataGrid.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.AdditivesDataGrid_ItemDataBound);
			this.AddButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);
			this.OK.Command += new System.Web.UI.WebControls.CommandEventHandler(this.OK_Command);
			this.Cancel.Command += new System.Web.UI.WebControls.CommandEventHandler(this.Cancel_Command);
		}
		#endregion

		private void AddButton_Command(object sender, System.Web.UI.WebControls.CommandEventArgs e)
		{
			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
					x =>
					x.Get(this.Security, this.Security.SiteGuid, getMemberSites: false, getSchedulesAndProcessVariables: false, bGetAssociatedAliases: true)
			);
			AdditiveProfileClass AdditiveProfile = (AdditiveProfileClass)this.Session["AdditiveProfile"];
			ProductMapClass Additive = new ProductMapClass(site);
			AdditiveProfile.AdditiveCollection.Add(Additive);

			Additive.IdentityGuid = AdditiveProfile.IdentityGuid;
			Additive.Type = PRODUCT_MAP_TYPE.ADDITIVE_PROFILE_MAP;
			Additive.Sequence = AdditiveProfile.AdditiveCollection.Count - 1;

			this.AdditivesDataGrid.CurrentPageIndex = (AdditiveProfile.AdditiveCollection.Count - 1) / this.AdditivesDataGrid.PageSize;
			this.AdditivesDataGrid.EditItemIndex = (AdditiveProfile.AdditiveCollection.Count - 1) % this.AdditivesDataGrid.PageSize;

			// Disable all controls while in line item edit mode.
			this.EnableControls(false);

			try
			{
				this.UpdateAdditivesView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				AdditiveProfile.AdditiveCollection.RemoveAt(AdditiveProfile.AdditiveCollection.Count - 1);

				if (this.AdditivesDataGrid.CurrentPageIndex > 0
					&& this.AdditivesDataGrid.EditItemIndex == 0)
				{
					this.AdditivesDataGrid.CurrentPageIndex--;
				}

				this.AdditivesDataGrid.EditItemIndex = -1;

				// Enable all controls after line item edit.
				this.EnableControls(true);
				this.UpdateAdditivesView();
			}
		}

		private void AdditivesDataGrid_CancelCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			Label IndexLabel = (Label)e.Item.FindControl("IndexLabel");

			if (IndexLabel != null)
			{
				AdditiveProfileClass AdditiveProfile = (AdditiveProfileClass)this.Session["AdditiveProfile"];
				ProductMapClass Additive = AdditiveProfile.AdditiveCollection[System.Convert.ToInt32(IndexLabel.Text)];

				if (Additive.AssignedGuid == Guid.Empty)
				{
					AdditiveProfile.AdditiveCollection.RemoveAt(System.Convert.ToInt32(IndexLabel.Text));

					if (this.AdditivesDataGrid.Items.Count == 1
						&& this.AdditivesDataGrid.CurrentPageIndex > 0)
					{
						this.AdditivesDataGrid.CurrentPageIndex--;
					}
				}

				// Enable all controls after line item edit.
				this.EnableControls(true);

				this.AdditivesDataGrid.EditItemIndex = -1;
				this.UpdateAdditivesView();
			}
		}

		private void AdditivesDataGrid_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			Label IndexLabel = (Label)e.Item.FindControl("IndexLabel");
			if (IndexLabel != null)
			{
				AdditiveProfileClass AdditiveProfile = (AdditiveProfileClass)this.Session["AdditiveProfile"];

				if (this.AdditivesDataGrid.EditItemIndex == e.Item.ItemIndex)
				{
					this.AdditivesDataGrid.EditItemIndex = -1;
					this.EnableControls(true);
				}
				else if (this.AdditivesDataGrid.EditItemIndex > e.Item.ItemIndex)
					this.AdditivesDataGrid.EditItemIndex--;

				AdditiveProfile.AdditiveCollection.RemoveAt(System.Convert.ToInt32(IndexLabel.Text));
				if (this.AdditivesDataGrid.Items.Count == 1
				&& this.AdditivesDataGrid.CurrentPageIndex > 0)
					this.AdditivesDataGrid.CurrentPageIndex--;
				this.UpdateAdditivesView();
			}
		}

		private void AdditivesDataGrid_EditCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			this.AdditivesDataGrid.EditItemIndex = e.Item.ItemIndex;
			try
			{
				// Disable all controls while in line item edit mode.
				this.EnableControls(false);
				this.UpdateAdditivesView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				this.AdditivesDataGrid.EditItemIndex = -1;

				// Enable all controls after line item edit.
				this.EnableControls(true);
				this.UpdateAdditivesView();
			}
		}

		private void AdditivesDataGrid_UpdateCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			try
			{
				Label IndexLabel = (Label)e.Item.FindControl("IndexLabel");

				if (IndexLabel != null)
				{
                    SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(sites => sites.Get(this.Security, this.Security.SiteGuid, false,  false, false));

					AdditiveProfileClass AdditiveProfile = (AdditiveProfileClass)this.Session["AdditiveProfile"];
					ProductMapClass Additive = AdditiveProfile.AdditiveCollection[System.Convert.ToInt32(IndexLabel.Text)];

					DropDownList AdditivesDropDownList = (DropDownList)e.Item.FindControl("AdditivesDropDownList");
					if (AdditivesDropDownList.SelectedIndex != -1)
					{
						Additive.AssignedGuid = Guid.Parse(AdditivesDropDownList.SelectedValue);
						Additive.AssignedID = AdditivesDropDownList.SelectedItem.Text;
					}

					TextBox RateTextBox = (TextBox)e.Item.FindControl("RateTextBox");
					Additive.AdditiveRate = RateTextBox.Text;

					TextBox CycleVolumeTextBox = (TextBox)e.Item.FindControl("CycleVolumeTextBox");
					Additive.AdditiveCycleVolume = CycleVolumeTextBox.Text;
                                     
                    TextBox DesiredTreatRateTextBox = (TextBox)e.Item.FindControl("DesiredTreatRateTextBox");
                    try
                    {
                        Additive.DesiredTreatRate = Convert.ToDouble(DesiredTreatRateTextBox.Text);
                    }
                    catch (FormatException)
                    {
                        Additive.DesiredTreatRate = 0.0;
                    }
                    TextBox ToleranceTextBox = (TextBox)e.Item.FindControl("ToleranceTextBox");
                    Additive.Tolerance = Convert.ToDouble(ToleranceTextBox.Text, site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT));

					this.AdditivesDataGrid.EditItemIndex = -1;

					// Enable all controls after line item edit.
					this.EnableControls(true);
					this.UpdateAdditivesView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void AdditivesDataGrid_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			Label IndexLabel = (Label)e.Item.FindControl("IndexLabel");
			if (IndexLabel != null)
			{
				DropDownList AdditivesDropDownList = (DropDownList)e.Item.FindControl("AdditivesDropDownList");
				if (AdditivesDropDownList != null)
				{
					AdditiveProfileClass AdditiveProfile = (AdditiveProfileClass)this.Session["AdditiveProfile"];
					ProductMapClass Additive = AdditiveProfile.AdditiveCollection[System.Convert.ToInt32(IndexLabel.Text)];
					if (Additive.AssignedGuid != Guid.Empty)
					{
						ListItemCollection Items = AdditivesDropDownList.Items;
						int Index = Items.IndexOf(Items.FindByValue(Additive.AssignedGuid.ToString()));
						AdditivesDropDownList.SelectedIndex = Index;
					}

					if (!this.Security.HasRight(RIGHT.MODIFY_PRODUCTS)
						|| (this.Security.SiteGuid != AdditiveProfile.SiteGuid
						&& AdditiveProfile.SiteGuid != Guid.Empty))
					{
						FMEditLinkButton EditButton = (FMEditLinkButton)e.Item.FindControl("EditButton");
						if (EditButton != null)
						{
							EditButton.Enabled = false;
						}

						FMDeleteLinkButton DeleteButton = (FMDeleteLinkButton)e.Item.FindControl("DeleteButton");
						if (DeleteButton != null)
						{
							DeleteButton.Enabled = false;
						}
					}
				}
			}
		}

		private void InitializeComponent()
		{
			this.Cancel.Command += new System.Web.UI.WebControls.CommandEventHandler(this.Cancel_Command);
			this.OK.Command += new System.Web.UI.WebControls.CommandEventHandler(this.OK_Command);

		}

		private void OK_Command(object sender, System.Web.UI.WebControls.CommandEventArgs e)
		{
			try
			{
				AdditiveProfileClass AdditiveProfile = (AdditiveProfileClass)this.Session["AdditiveProfile"];

				AdditiveProfile.ID = this.AdditiveProfileIDTextbox.Text;
				AdditiveProfile.Description = this.AdditiveProfileDescriptionTextbox.Text;

				FMChannelHelper.MakeCall<IAdditiveProfiles>(
					additiveProfiles =>
					{
						if (AdditiveProfile.IdentityGuid != Guid.Empty)
							additiveProfiles.Modify(this.Security, AdditiveProfile);
						else
							additiveProfiles.Add(this.Security, AdditiveProfile);
					});
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}
			this.Session.Remove("AdditiveProfile");
			this.Redirect("AdditiveProfilesForm.aspx");
		}

		private void Cancel_Command(object sender, System.Web.UI.WebControls.CommandEventArgs e)
		{
			this.Session.Remove("AdditiveProfile");
			this.Redirect("AdditiveProfilesForm.aspx");
		}

		private void AdditivesDataGrid_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.AdditivesDataGrid.EditItemIndex > -1)
				return;
			this.AdditivesDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateAdditivesView();
		}
	}
}
