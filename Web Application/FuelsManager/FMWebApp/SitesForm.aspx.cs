// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SitesForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the SitesForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Net.Sockets;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;

	using global::FMWebApp;

	/// <summary>
	///    Summary description for SitesForm.
	/// </summary>
	public partial class SitesForm : FMFormBase, IEntityDiscovery, IMenuDiscovery
	{
		private bool isAMultiSiteKey;
		private int siteCount;

		#region Explicit Interface Properties

		bool IEntityDiscovery.EntityAssignable
		{
			get
			{
				return false;
			}
		}

		Type IEntityDiscovery.EntityEngineType
		{
			get
			{
				return typeof(ISites);
			}
		}

		ENTITY_TYPE IEntityDiscovery.EntityType
		{
			get
			{
				return ENTITY_TYPE.SITE;
			}
		}

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
				// no checks for site when using a new key
         }
         else
         {
             // Depends Upon Shared Components Config
             if ((options & 0x4000) == 0)
             {
                 return null;
             }
         }
         var items = new List<FMMenuItem>();

			if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				return null;
			}

			items.Add(
				new FMMenuItem
					{
						MenuItemType = FMMenuItemType.ADMIN_SITES_SITES,
						RootMenuName = "Administration",
						CategoryName = "Sites",
						ItemName = "Sites",
						NavigateUrl = "SitesForm.aspx",
						SortOrder = 1,
						ApplyDataDictionary = ApplyDataDictionary.Apply
					});

			return items;
		}

		#endregion

		#region Explicit Interface Methods

		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(
			SecurityClass Security, ENTITY_ASSIGNMENT_TYPE Type)
		{
			SiteCollectionClass SiteCollection;
			SiteCollection = FMChannelHelper.MakeCall<ISites, SiteCollectionClass>(
																	 x =>
																	 x.Enumerate(Security)
																);


			var EntityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			foreach (SiteClass Site in SiteCollection)
			{
				if (Type == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if (Security.SiteGuid == Site.SiteGuid)
					{
						continue;
					}

					if (Security.LoginSiteGuid != Site.SiteGuid)
					{
						continue;
					}
				}
				else
				{
					if (Security.SiteGuid != Site.SiteGuid)
					{
						continue;
					}
				}

				var EntityToSiteMap = new EntityToSiteMapClass(Site);
				EntityToSiteMapCollection.Add(EntityToSiteMap);
			}

			return EntityToSiteMapCollection;
		}

		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string ID)
		{
			return Guid.Empty;
		}

		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid SiteGuid)
		{
			SiteClass Site = 
				FMChannelHelper.MakeCall<ISites, SiteClass>(
						x =>
						x.Get(security, guid, getMemberSites: true, getSchedulesAndProcessVariables: true, bGetAssociatedAliases: true)
				);

			Site.SiteGuid = SiteGuid;
			FMChannelHelper.MakeCall<ISites>(
					x =>
					x.Modify(security, DATA_TYPE.CONFIG, Site, updateDocumentNumbers: true)
			);
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

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				this.isAMultiSiteKey = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsMultipleSiteKey());
				this.siteCount = FMChannelHelper.MakeCall<ISites, int>(x => x.GetSiteCount(Security));

				if (!this.Page.IsPostBack)
				{
					// Make Delete Column Invisable if not Administrator
					if (!this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
					    || !SiteClass.IsAdminSiteGuid(this.Security.SiteGuid))
					{
						this.SitesDataGrid.Columns[4].Visible = false;
						this.AddButton.Enabled = false;
						this.AddButton2.Enabled = false;
					}
					else if(isAMultiSiteKey == false &&
						siteCount >= 2)
					{
						this.AddButton.Enabled = false;
						this.AddButton2.Enabled = false;
					}

					if (this.Session["SitesPage"] != null)
					{
						this.SitesDataGrid.CurrentPageIndex = (int)this.Session["SitesPage"];
						this.Session.Remove("SitesPage");
					}
					this.UpdateView();
				}

				// Put user code to initialize the page here
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void AddButton_Command(object sender, CommandEventArgs e)
		{
			this.Session.Remove("Site");
			this.Session.Remove("IdentityGuid");
			this.Session["SitesPage"] = this.SitesDataGrid.CurrentPageIndex;
			this.Redirect("SiteForm.aspx");
		}

		/// <summary>
		/// This method gets the SSO mode.
		/// </summary>
		/// <returns>Returns true if in SSO mode, otherwise false.</returns>
		private bool IsSsoMode()
		{
			bool ssoMode = false;

			try
			{
				var configSetting = FMChannelHelper.MakeCall<IConfigurationSettings, ConfigurationSettingDOClass>
													(x => x.GetByKey(this.Security, ConfigurationSettingDOClass.Key_SingleSignOnMode));

				// This is so that the Login page will not try and auto login the domain user.
				if (configSetting != null && string.IsNullOrEmpty(configSetting.SettingValue) == false && configSetting.SettingValue == "1")
				{
					ssoMode = true;
				}
			}
			catch (Exception)
			{
				return ssoMode;
			}

			return ssoMode;
		}

		/// <summary>
		/// This method will enumerate sites.
		/// </summary>
		/// <returns>Returns a collection of site objects.</returns>
		private ICollection EnumerateSites()
		{
			SiteCollectionClass siteCollection;
			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(
																			this.Security,
																			this.Security.SiteGuid,
																			getMemberSites: true,
																			getSchedulesAndProcessVariables: true,
																			bGetAssociatedAliases: true)
																	);
			if (site.SiteGroup)
			{
				if (site.IsAdminSite)
				{
					siteCollection = FMChannelHelper.MakeCall<ISites, SiteCollectionClass>(
																	 x =>
																	 x.Enumerate(this.Security)
																);
				}
				else
				{
					siteCollection = FMChannelHelper.MakeCall<ISites, SiteCollectionClass>(
																	 x =>
																	 x.EnumerateByParentSite(this.Security, this.Security.SiteGuid)
																);
					siteCollection.Add(site);
				}
			}
			else
			{
			    siteCollection = new SiteCollectionClass { site };
			}

            var adSiteGroupList = FMChannelHelper.MakeCall<IActiveDirectoryMappings, List<ActiveDirectorySiteGroup>>(
                                                                    x => x.EnumerateAllActiveDirectorySites(this.Security));

			// Check for Single Sign On mode.
            bool ssoMode = this.IsSsoMode();

			// This is so that the Login page will not try and auto login the domain user.
			var siteDataTable = new DataTable();

		    siteDataTable.Columns.Add("IdentityGuid", typeof(Guid));
			siteDataTable.Columns.Add("ID", typeof(string));
            siteDataTable.Columns.Add("AdSiteMapping", typeof(string));
            siteDataTable.Columns.Add("Enabled", typeof(bool));
			siteDataTable.Columns.Add("SiteGroup", typeof(bool));

			foreach (SiteClass siteRec in siteCollection)
			{
				var siteDataRow = siteDataTable.NewRow();

				siteDataRow["IdentityGuid"]     = siteRec.SiteGuid;
				siteDataRow["ID"]               = siteRec.ID;
                siteDataRow["AdSiteMapping"]    = this.GetActiveDirectoryMappingName(ssoMode, adSiteGroupList, siteRec.ActiveDirectorySiteGroupGuid);
                siteDataRow["Enabled"]          = siteRec.Enabled;
				siteDataRow["SiteGroup"]        = siteRec.SiteGroup;

				siteDataTable.Rows.Add(siteDataRow);
			}

            // If not in SSO mode, then hide the AD Site Mapping Name column.
		    if (ssoMode == false)
		    {
		        this.SitesDataGrid.Columns[3].Visible = false;
		    }

			var siteDataView = new DataView(siteDataTable);
			return siteDataView;
		}

	    /// <summary>
	    /// This method will return a matching AD site mapping name.
	    /// </summary>
	    /// <param name="ssoMode">Flag that indicates if in SSO mode.</param>
	    /// <param name="adSiteGroupList">The AD site group list.</param>
	    /// <param name="adSiteGroupGuid">The AD site Guid to compare to.</param>
	    /// <returns>Return an empty string if not found, or a AD mapping name.</returns>
	    private string GetActiveDirectoryMappingName(bool ssoMode, List<ActiveDirectorySiteGroup> adSiteGroupList, Guid adSiteGroupGuid)
	    {
	        if (ssoMode == false || adSiteGroupList.Count == 0)
	        {
	            return string.Empty;
	        }

	        var adGroup = adSiteGroupList.Find(x => x.ActiveDirectorySiteGroupGuid == adSiteGroupGuid);
	        if (adGroup == null)
	        {
	            return string.Empty;
	        }

	        return adGroup.Name;
	    }

        /// <summary>
        ///    Required method for Designer support - do not modify
        ///    the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
		{
			this.AddButton2.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);
			this.SitesDataGrid.EditCommand +=
				new System.Web.UI.WebControls.DataGridCommandEventHandler(this.SitesDataGrid_EditCommand);
			this.SitesDataGrid.PageIndexChanged +=
				new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.SitesDataGrid_PageIndexChanged);
			this.SitesDataGrid.DeleteCommand +=
				new System.Web.UI.WebControls.DataGridCommandEventHandler(this.SitesDataGrid_DeleteCommand);
			this.SitesDataGrid.ItemDataBound +=
				new System.Web.UI.WebControls.DataGridItemEventHandler(this.SitesDataGrid_ItemDataBound);
			this.AddButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);
		}

		private void SitesDataGrid_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				TableCell identityGuidCell = e.Item.Cells[1];//bds
				Guid siteGuid = Guid.Empty;
				if (!string.IsNullOrEmpty(identityGuidCell.Text))
				{
					siteGuid = new Guid(identityGuidCell.Text);
				}

				try
				{
					if (UsingLoadRack)
					{
						ILoadRackManager LoadRackManager = this.GetLoadRackManager();
						LoadRackManager.Purge(this.Security, typeof(SiteClass), siteGuid);
					}
				}
				catch (SocketException socketExcept)
				{
					if (socketExcept.ErrorCode != 10061)
					{
						throw socketExcept;
					}
				}

				FMChannelHelper.MakeCall<ISites>(
																	 x =>
																	 x.Purge(this.Security, siteGuid)
																);

				this.SitesDataGrid.SelectedIndex = -1;
				if (this.SitesDataGrid.Items.Count == 1 && this.SitesDataGrid.CurrentPageIndex > 0)
				{
					this.SitesDataGrid.CurrentPageIndex--;
				}
				this.UpdateView();
				this.Session.Remove("IdentityGuid");
				this.ucFMMenuBar.Refresh();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void SitesDataGrid_EditCommand(object source, DataGridCommandEventArgs e)
		{
			this.Session.Remove("Site");
			TableCell identityGuidCell = e.Item.Cells[1];//bds
			this.Session["IdentityGuid"] = new Guid(identityGuidCell.Text);
			this.Session["SitesPage"] = this.SitesDataGrid.CurrentPageIndex;
			this.Redirect("SiteForm.aspx");
		}

		private void SitesDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			var deleteButton = (LinkButton)e.Item.FindControl("DeleteButton");

			if (deleteButton != null)
			{
				TableCell siteGuidCell = e.Item.Cells[1];//bds
				Guid siteGuid = Guid.Parse(siteGuidCell.Text);

				if (!this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) || SiteClass.IsAdminSiteGuid(siteGuid)
				    || (this.isAMultiSiteKey && SiteClass.IsDefaultSiteGuid(siteGuid)))
				{
					deleteButton.Enabled = false;
					deleteButton.Text = "<img src=Images/Delete_un.gif border=0 align=absmiddle alt='Delete this item'>";
				}
				else if (this.isAMultiSiteKey == false && this.siteCount >= 2)
				{
					deleteButton.Enabled = false;
					deleteButton.Text = "<img src=Images/Delete_un.gif border=0 align=absmiddle alt='Delete this item'>";
				}
			}
		}

		private void SitesDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.SitesDataGrid.EditItemIndex > -1)
			{
				return;
			}
			this.SitesDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateView();
		}

		private void UpdateView()
		{
			ICollection Sites = this.EnumerateSites();

			this.SitesDataGrid.DataSource = Sites;
			this.SitesDataGrid.DataBind();
		}

		#endregion
	}
}