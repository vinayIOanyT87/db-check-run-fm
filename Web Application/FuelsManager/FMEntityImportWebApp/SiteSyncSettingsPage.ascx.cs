// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SiteSyncSettingsPage.ascx.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	Defines the SiteSyncSettingsPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMEntityImportWebApp
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;

	using FMControls;

	using FuelsManager.FMWebApp;

	/// <summary>
	///	Summary description for SiteSyncSettingsPage.
	/// </summary>
	public partial class SiteSyncSettingsPage : FMUserControlBase
	{
		#region Properties
		/// <summary>
		/// Get the Site Sync Configuration object from Session.
		/// </summary>
		private SiteCollectionClass SessionSiteSyncSettings
		{
			get
			{
				if (this.Session[PageSessionKeyConstants.SYNC_CONFIG_SITE_SETTINGS] != null 
				&& this.Session[PageSessionKeyConstants.SYNC_CONFIG_SITE_SETTINGS] is SiteCollectionClass)
				{
					return (SiteCollectionClass)this.Session[PageSessionKeyConstants.SYNC_CONFIG_SITE_SETTINGS];
				}
					
				return null;
			}
			set
			{
				this.Session.Add(PageSessionKeyConstants.SYNC_CONFIG_SITE_SETTINGS, value);
			}
		}
		#endregion Properties

		#region Page Event Handlers and Overrides
		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		///	Required method for Designer support - do not modify
		///	the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.SiteSyncSettingDataGrid.CancelCommand += this.SiteSyncSettingDataGridCancelCommand;
			this.SiteSyncSettingDataGrid.EditCommand += this.SiteSyncSettingDataGridEditCommand;
			this.SiteSyncSettingDataGrid.UpdateCommand += this.SiteSyncSettingDataGridUpdateCommand;
			this.SiteSyncSettingDataGrid.ItemDataBound += this.SiteSyncSettingDataGridItemDataBound;
			this.Disable.Command += this.DisableCommand;
			this.Enable.Command += this.EnableCommand;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				if (!this.Page.IsPostBack)
				{
					if (!this.Security.HasRight(RIGHT.MODIFY_SYNC_CONFIG_SITE_SETTINGS)
					|| !this.IsEnterprise)
					{
						this.Disable.Enabled = false;
						this.Enable.Enabled = false;
					}

					this.UpdateSiteSyncSettingView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}
		#endregion Page Event Handlers and Overrides

		#region Methods
		/// <summary>
		///	This method enables and disables controls.
		/// </summary>
		/// <param name="enable"></param>
		protected void EnableControls(bool enable)
		{
			// Call the main form to disable buttons and tabs.
			var SiteForm = (SynchronizationConfigForm)this.Page;
			SiteForm.EnableControls(enable);

			if (this.Security.HasRight(RIGHT.MODIFY_SYNC_CONFIG_SITE_SETTINGS)
			&& this.IsEnterprise)
			{
				this.Disable.Enabled = enable;
				this.Enable.Enabled = enable;
			}
		}
		#endregion Methods

		#region DataGrid Methods and Event Handlers
		private ICollection EnumerateSiteSyncSetting()
		{
			var SiteSyncIntervalDataTable = new DataTable();

			SiteSyncIntervalDataTable.Columns.Add("SiteID", typeof(string));
			SiteSyncIntervalDataTable.Columns.Add("SiteGuid", typeof(Guid));
			SiteSyncIntervalDataTable.Columns.Add("DisableSyncTransferFlag", typeof(bool));
			SiteSyncIntervalDataTable.Columns.Add("EnablePeriodicSyncFlag", typeof(bool));
			SiteSyncIntervalDataTable.Columns.Add("PeriodicSyncIntervalMinutes", typeof(int));

			if (null != this.SessionSiteSyncSettings)
			{
				foreach (SiteClass site in this.SessionSiteSyncSettings)
				{
					var SiteSyncIntervalDataRow = SiteSyncIntervalDataTable.NewRow();

					SiteSyncIntervalDataRow["SiteGuid"] = site.IdentityGuid;
					SiteSyncIntervalDataRow["SiteID"] = site.ID;
					SiteSyncIntervalDataRow["DisableSyncTransferFlag"] = site.DisableSyncTransferFlag;
					SiteSyncIntervalDataRow["EnablePeriodicSyncFlag"] = site.EnablePeriodicSyncFlag;
					SiteSyncIntervalDataRow["PeriodicSyncIntervalMinutes"] = site.PeriodicSyncIntervalMinutes;

					SiteSyncIntervalDataTable.Rows.Add(SiteSyncIntervalDataRow);
				}
			}

			var SiteSyncSettingDataView = new DataView(SiteSyncIntervalDataTable);
			return (SiteSyncSettingDataView);
		}

		private void SiteSyncSettingDataGridCancelCommand(object source, DataGridCommandEventArgs e)
		{
				var SiteSyncSettingDataGrid = (DataGrid)source;
				SiteSyncSettingDataGrid.EditItemIndex = -1;

				// Enable controls when completing line item editing;
				this.EnableControls(true);
				this.UpdateSiteSyncSettingView();
		}

		private void SiteSyncSettingDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
				var SiteSyncSettingDataGrid = (DataGrid)source;
				SiteSyncSettingDataGrid.EditItemIndex = e.Item.ItemIndex;

				// Disable controls while in line item edit mode;
				this.EnableControls(false);
				this.UpdateSiteSyncSettingView();
		}

		private void SiteSyncSettingDataGridUpdateCommand(object source, DataGridCommandEventArgs e)
		{
				try
				{
					DataGrid SiteSyncSettingDataGrid = (DataGrid)source;

					var siteIdLabelRo = (Label)e.Item.FindControl("SiteIDLabelRO");

					if (siteIdLabelRo != null)
					{
						SiteClass siteEdit = null;

						string siteID = siteIdLabelRo.Text;

						if (null != this.SessionSiteSyncSettings)
						{
								var siteList = from s in
													this.SessionSiteSyncSettings
													where s.ID == siteID
													select s;

							var siteClasses = siteList as SiteClass[] ?? siteList.ToArray();

							if ((siteClasses.Any()))
							{
								siteEdit = siteClasses.FirstOrDefault();
							}

							if (null != siteEdit)
							{
								var enableCheckBox = (CheckBox)e.Item.FindControl("EnablePeriodicSyncFlag");
								siteEdit.EnablePeriodicSyncFlag = enableCheckBox.Checked;

								var syncInterval = (TextBox)e.Item.FindControl("PeriodicSyncIntervalMinutes");
								siteEdit.PeriodicSyncIntervalMinutes = string.IsNullOrEmpty(syncInterval.Text) ? 0 : Convert.ToInt32(syncInterval.Text);

								var disableSiteTransferCheckBox = (CheckBox)e.Item.FindControl("DisableSyncTransferFlag");
								siteEdit.DisableSyncTransferFlag = disableSiteTransferCheckBox.Checked;
							}
						}

						SiteSyncSettingDataGrid.EditItemIndex = -1;

						this.Session.Remove(PageSessionKeyConstants.SYNC_CONFIG_SITE_SETTINGS_MODIFIED);
						this.Session.Add(PageSessionKeyConstants.SYNC_CONFIG_SITE_SETTINGS_MODIFIED, true);

						// Enable controls when completing line item editing;
						this.EnableControls(true);
						this.UpdateSiteSyncSettingView();
					}
				}
				catch (Exception except)
				{
					this.ErrorHandler(except);
				}
		}

		private void SiteSyncSettingDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			Label indexLabel = (Label)e.Item.FindControl("IndexLabel");

			if (indexLabel != null)
			{
				if (!this.Security.HasRight(RIGHT.MODIFY_SYNC_CONFIG_SITE_SETTINGS))
				{
					FMEditLinkButton editBtn = (FMEditLinkButton)e.Item.FindControl("FMEditLinkButton");

					if (null != editBtn)
					{
						editBtn.Enabled = false;
					}

					FMUpdateLinkButton updateBtn = (FMUpdateLinkButton)e.Item.FindControl("FMUpdateLinkButton");

					if (null != updateBtn)
					{
						updateBtn.Enabled = false;
					}

					FMCancelLinkButton cancelBtn = (FMCancelLinkButton)e.Item.FindControl("FMCancelLinkButton");

					if (null != cancelBtn)
					{
						cancelBtn.Enabled = false;
					}
				}

				var disableSyncTransferFlagCheckBox = (CheckBox) e.Item.FindControl("DisableSyncTransferFlag");

				if(null != disableSyncTransferFlagCheckBox
				&& !this.IsEnterprise)
				{
					disableSyncTransferFlagCheckBox.Enabled = false;
				}
			}
		}

		private void DisableCommand(object sender, CommandEventArgs e)
		{
			foreach (SiteClass site in this.SessionSiteSyncSettings)
			{
				site.DisableSyncTransferFlag = true;
			}

			this.UpdateSiteSyncSettingView();
		}

		private void EnableCommand(object sender, CommandEventArgs e)
		{
			foreach (SiteClass site in this.SessionSiteSyncSettings)
			{
				site.DisableSyncTransferFlag = false;
			}

			this.UpdateSiteSyncSettingView();
		}


		private void UpdateSiteSyncSettingView()
		{
				this.SiteSyncSettingDataGrid.DataSource = this.EnumerateSiteSyncSetting();
				this.SiteSyncSettingDataGrid.DataBind();
		}
		#endregion DataGrid Methods and Event Handlers
	}
}