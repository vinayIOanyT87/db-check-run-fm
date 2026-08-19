// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PIDXProfilesForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the PIDXProfilesForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FMControls;

	using global::FMWebApp;

	/// <summary>
	///    Summary description for PIDXProfilesForm.
	/// </summary>
	public partial class PIDXProfilesForm : FMAutoSubmitFormBase, IMenuDiscovery
	{
		#region Constants and Fields

		protected FMDeleteLinkButton Fmdeletelinkbutton1;

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
                if ((word2 & 0x01) != 0x01)
                    return null;
            }
            else
            {
                // Depends Upon Load Rack Service
                if ((options & 0x8000) == 0)
                {
                    return null;
                }
            }

            var items = new List<FMMenuItem>();

			if (siteGroup)
			{
				return null;
			}

			if (!security.HasRight(RIGHT.VIEW_PIDX_PROFILES) && !security.HasRight(RIGHT.MODIFY_PIDX_PROFILES))
			{
				return null;
			}

			items.Add(
				new FMMenuItem
					{
						MenuItemType = FMMenuItemType.CONFIG_LOAD_RACK_DATA_EXCHANGE_PROFILE,
						RootMenuName = "Configuration",
						CategoryName = "Load Rack",
						ItemName = "Data Exchange Profiles",
						NavigateUrl = "PIDXProfilesForm.aspx",
						ApplyDataDictionary = ApplyDataDictionary.Apply
					});

			return items;
		}

		#endregion

		#region Methods

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

		protected void PIDXProfilesDataGrid_SelectedIndexChanged(object sender, EventArgs e)
		{
		}

		protected void PageSizeDropDown_SelectedIndexChanged(object source, EventArgs e)
		{
			this.UpdateView();
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!this.Page.IsPostBack)
				{
					if (!this.Security.HasRight(RIGHT.MODIFY_PIDX_PROFILES)) // vthompson CSI 5773
					{
						this.AddButton.Enabled = false;
						this.AddButton2.Enabled = false;
					}

					if (this.Session["PIDXProfilesPage"] != null)
					{
						this.PIDXProfilesDataGrid.CurrentPageIndex = (int)this.Session["PIDXProfilesPage"];
						this.Session.Remove("PIDXProfilesPage");
					}

					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void AddButton_Command(object sender, CommandEventArgs e)
		{
			this.Session.Remove("IdentityGuid");
			this.Session["PIDXProfilesPage"] = this.PIDXProfilesDataGrid.CurrentPageIndex;
			this.Redirect("PIDXProfileForm.aspx");
		}

		private ICollection EnumeratePIDXProfiles()
		{
			PIDXProfileCollectionClass PIDXProfileCollection = FMChannelHelper.MakeCall<IPIDXProfiles, PIDXProfileCollectionClass >(
																	 x =>
																	 x.Enumerate(this.Security)
																);
			;

			var PIDXProfileDataTable = new DataTable();
			DataRow PIDXProfileDataRow;

			PIDXProfileDataTable.Columns.Add("SiteGuid", typeof(Guid));
			PIDXProfileDataTable.Columns.Add("IdentityGuid", typeof(Guid));
			PIDXProfileDataTable.Columns.Add("Type", typeof(string));
			PIDXProfileDataTable.Columns.Add("ID", typeof(string));
			PIDXProfileDataTable.Columns.Add("Enabled", typeof(bool));
			PIDXProfileDataTable.Columns.Add("Log Enabled", typeof(bool));

			foreach (PIDXProfileClass PIDXProfile in PIDXProfileCollection)
			{
				PIDXProfileDataRow = PIDXProfileDataTable.NewRow();

				PIDXProfileDataRow["SiteGuid"] = PIDXProfile.SiteGuid;
				PIDXProfileDataRow["IdentityGuid"] = PIDXProfile.IdentityGuid;
				PIDXProfileDataRow["Type"] = PIDXProfileClass.TypeID(PIDXProfile.Type);
				PIDXProfileDataRow["ID"] = PIDXProfile.ID;
				PIDXProfileDataRow["Enabled"] = PIDXProfile.Enabled;
				PIDXProfileDataRow["Log Enabled"] = PIDXProfile.LoggingEnabled;

				PIDXProfileDataTable.Rows.Add(PIDXProfileDataRow);
			}
			var PIDXProfileDataView = new DataView(PIDXProfileDataTable);
			return PIDXProfileDataView;
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AddButton2.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);
			this.PIDXProfilesDataGrid.EditCommand +=
				new System.Web.UI.WebControls.DataGridCommandEventHandler(this.PIDXProfilesDataGrid_EditCommand);
			this.PIDXProfilesDataGrid.PageIndexChanged +=
				new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.PIDXProfilesDataGrid_PageIndexChanged);
			this.PIDXProfilesDataGrid.DeleteCommand +=
				new System.Web.UI.WebControls.DataGridCommandEventHandler(this.PIDXProfilesDataGrid_DeleteCommand);
			this.PIDXProfilesDataGrid.ItemDataBound +=
				new System.Web.UI.WebControls.DataGridItemEventHandler(this.PIDXProfilesDataGrid_ItemDataBound);
			this.AddButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);
		}

		private void PIDXProfilesDataGrid_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				// Get IdentityGuid
				TableCell identityGuidCell = e.Item.Cells[2];//bds

				FMChannelHelper.MakeCall<IPIDXProfiles>(
																	 x =>
																	 x.Purge(this.Security, Guid.Parse(identityGuidCell.Text))
																);

				this.PIDXProfilesDataGrid.SelectedIndex = -1;
				this.Session.Remove("IdentityGuid");
				if (this.PIDXProfilesDataGrid.Items.Count == 1 && this.PIDXProfilesDataGrid.CurrentPageIndex > 0)
				{
					this.PIDXProfilesDataGrid.CurrentPageIndex--;
				}
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void PIDXProfilesDataGrid_EditCommand(object source, DataGridCommandEventArgs e)
		{
			this.Session.Remove("Product");
			TableCell identityGuidCell = e.Item.Cells[2];//bds
			this.Session["IdentityGuid"] = identityGuidCell.Text;
			this.Session["PIDXProfilesPage"] = this.PIDXProfilesDataGrid.CurrentPageIndex;
			this.Redirect("PIDXProfileForm.aspx");
		}

		private void PIDXProfilesDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			var DeleteButton = (LinkButton)e.Item.FindControl("DeleteButton");
			if (DeleteButton != null)
			{
				DeleteButton.Enabled = this.Security.HasRight(RIGHT.MODIFY_PIDX_PROFILES); // vthompson CSI 5773
			}
		}

		private void PIDXProfilesDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			try
			{
				// if we are editing do not allow a page change
				if (this.PIDXProfilesDataGrid.EditItemIndex > -1)
				{
					return;
				}
				this.PIDXProfilesDataGrid.CurrentPageIndex = e.NewPageIndex;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void UpdateView()
		{
			ICollection PIDXProfiles = this.EnumeratePIDXProfiles();

			this.PIDXProfilesFormPageSizeDropDown.SetPageSize(this.PIDXProfilesDataGrid, PIDXProfiles.Count);

			this.PIDXProfilesDataGrid.DataSource = PIDXProfiles;
			this.PIDXProfilesDataGrid.DataBind();
		}

		#endregion
	}
}