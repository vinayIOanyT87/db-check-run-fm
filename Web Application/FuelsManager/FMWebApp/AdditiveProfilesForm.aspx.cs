// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdditiveProfilesForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for AdditiveProfilesForm.
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
	using FMBusinessObjects.Interfaces;

	using global::FMWebApp;

	/// <summary>
	/// Code behind for additive profiles form
	/// </summary>
	public partial class AdditiveProfilesForm : FMFormBase, IEntityDiscovery, IMenuDiscovery
	{
		#region Explicit Interface Properties

		/// <summary>
		/// Gets a value indicating whether [entity assignable].
		/// </summary>
		/// <value>
		///   <c>true</c> if [entity assignable]; otherwise, <c>false</c>.
		/// </value>
		bool IEntityDiscovery.EntityAssignable
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Gets the type of the entity engine.
		/// </summary>
		/// <value>
		/// The type of the entity engine.
		/// </value>
		Type IEntityDiscovery.EntityEngineType
		{
			get
			{
				return typeof(IAdditiveProfiles);
			}
		}

		/// <summary>
		/// Gets the type of the entity.
		/// </summary>
		/// <value>
		/// The type of the entity.
		/// </value>
		ENTITY_TYPE IEntityDiscovery.EntityType
		{
			get
			{
				return ENTITY_TYPE.ADDITIVE_PROFILE;
			}
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Gets a list of menu items that should be displayed for the current user.
		/// </summary>
		/// <param name="security">
		/// The security object of the current session 
		/// </param>
		/// <param name="siteGroup">
		/// Whether the current logged-in site is a site group 
		/// </param>
		/// <param name="options">
		/// Hardware key options 
		/// </param>
		/// <returns>
		/// List of menu items to be displayed 
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
                // Depends Upon Shared Components Config and Load Rack Service
                if ((options & 0x4000) == 0 || (options & 0x8000) == 0)
                {
                    return null;
                }
            }


            if (!security.HasRight(RIGHT.VIEW_PRODUCTS) && !security.HasRight(RIGHT.MODIFY_PRODUCTS))
			{
				return null;
			}

			var menuItems = new List<FMMenuItem>();

			var menuItem = new FMMenuItem
				{
					MenuItemType = FMMenuItemType.ASSETS_PRODUCTS_ADDITIVE_PROFILES, 
					RootMenuName = "Assets", 
					CategoryName = "Products", 
					ItemName = "Additive Profiles", 
					NavigateUrl = "AdditiveProfilesForm.aspx", 
					ApplyDataDictionary = ApplyDataDictionary.Apply
				};

			menuItems.Add(menuItem);

			return menuItems;
		}

		#endregion

		#region Explicit Interface Methods

		/// <summary>
		/// Enumerates the entity maps.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="entityAssignmentType">The type.</param>
		/// <returns>A collection of entity to site maps for additive profiles</returns>
		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(SecurityClass security, ENTITY_ASSIGNMENT_TYPE entityAssignmentType)
		{
			var additiveProfileCollection =
				FMChannelHelper.MakeCall<IAdditiveProfiles, AdditiveProfileCollectionClass>(x => x.Enumerate(security));
			
			var entityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			foreach (AdditiveProfileClass additiveProfile in additiveProfileCollection)
			{
				if ( entityAssignmentType == ENTITY_ASSIGNMENT_TYPE.ASSIGNED )
				{
					if (security.SiteGuid == additiveProfile.SiteGuid)
					{
						continue;
					}

					if (security.LoginSiteGuid != additiveProfile.SiteGuid)
					{
						continue;
					}
				}
				else
				{
					if (security.SiteGuid != additiveProfile.SiteGuid)
					{
						continue;
					}
				}

				var entityToSiteMap = new EntityToSiteMapClass(additiveProfile);
				entityToSiteMapCollection.Add(entityToSiteMap);
			}

			return entityToSiteMapCollection;
		}

		/// <summary>
		/// Gets the identity GUID.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="additiveProfileId">The ID.</param>
		/// <returns>The identity guid of the specified additive profiles.</returns>
		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string additiveProfileId)
		{
			return FMChannelHelper.MakeCall<IAdditiveProfiles, Guid>( x => x.GetIdentityGuid( security, additiveProfileId ) );
		}

		/// <summary>
		/// Sets the site GUID.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="guid">The GUID.</param>
		/// <param name="siteGuid">The site GUID.</param>
		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid siteGuid)
		{
			FMChannelHelper.MakeCall<IAdditiveProfiles>(x => this.SetSiteGuidForAdditiveProfile(x, security, guid, siteGuid));
		}

		/// <summary>
		/// Sets the site GUID for additive profile.
		/// </summary>
		/// <param name="additiveProfiles">The additive profiles.</param>
		/// <param name="security">The security.</param>
		/// <param name="guid">The GUID.</param>
		/// <param name="siteGuid">The site GUID.</param>
		protected void SetSiteGuidForAdditiveProfile(IAdditiveProfiles additiveProfiles, SecurityClass security, Guid guid, Guid siteGuid )
		{
			var additiveProfile = additiveProfiles.Get( security, guid );
			additiveProfile.SiteGuid = siteGuid;
			additiveProfiles.Modify( security, additiveProfile );
		}

		#endregion

		#region Methods

		/// <summary>
		/// Raises the <see cref="OnInit"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Handles the SelectedIndexChanged event of the PageSizeDropDown control.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void PageSizeDropDownSelectedIndexChanged(object source, EventArgs e)
		{
			this.UpdateView();
		}

		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!this.Page.IsPostBack)
				{
					if (!this.Security.HasRight(RIGHT.MODIFY_PRODUCTS))
					{
						this.AddButton.Enabled = false;
						this.AddButton2.Enabled = false;
					}

					if (this.Session["AdditiveProfilesPage"] != null)
					{
						this.AdditiveProfilesDataGrid.CurrentPageIndex = (int)this.Session["AdditiveProfilesPage"];
						this.Session.Remove("AdditiveProfilesPage");
					}

					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the Command event of the AddButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.CommandEventArgs"/> instance containing the event data.</param>
		private void AddButtonCommand(object sender, CommandEventArgs e)
		{
			this.Session.Remove("IdentityGuid");
			this.Session["AdditiveProfilesPage"] = this.AdditiveProfilesDataGrid.CurrentPageIndex;
			this.Redirect("AdditiveProfileForm.aspx");
		}

		/// <summary>
		/// Handles the DeleteCommand event of the AdditiveProfilesDataGrid control.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.DataGridCommandEventArgs"/> instance containing the event data.</param>
		private void AdditiveProfilesDataGridDeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				// Get identityGuid
				TableCell identityGuidCell = e.Item.Cells[2];// bds

				FMChannelHelper.MakeCall<IAdditiveProfiles>(x => x.Purge(this.Security, Guid.Parse(identityGuidCell.Text)));

				this.AdditiveProfilesDataGrid.SelectedIndex = -1;
				this.Session.Remove("IdentityGuid");
				if (this.AdditiveProfilesDataGrid.Items.Count == 1 && this.AdditiveProfilesDataGrid.CurrentPageIndex > 0)
				{
					this.AdditiveProfilesDataGrid.CurrentPageIndex--;
				}

				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the EditCommand event of the AdditiveProfilesDataGrid control.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.DataGridCommandEventArgs"/> instance containing the event data.</param>
		private void AdditiveProfilesDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
			TableCell identityGuidCell = e.Item.Cells[2];	//bds
			this.Session["IdentityGuid"] = identityGuidCell.Text;
			this.Session["AdditiveProfilesPage"] = this.AdditiveProfilesDataGrid.CurrentPageIndex;
			this.Redirect("AdditiveProfileForm.aspx");
		}

		/// <summary>
		/// Handles the ItemDataBound event of the AdditiveProfilesDataGrid control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.DataGridItemEventArgs"/> instance containing the event data.</param>
		private void AdditiveProfilesDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			var deleteButton = (LinkButton)e.Item.FindControl("DeleteButton");
			if (deleteButton != null)
			{
				var siteGuidCell = e.Item.Cells[1];
				if (!this.Security.HasRight(RIGHT.MODIFY_PRODUCTS) || this.Security.SiteGuid != Guid.Parse(siteGuidCell.Text))
				{
					deleteButton.Enabled = false;
					deleteButton.Text = "<img src=Images/Delete_un.gif border=0 align=absmiddle alt='Delete this item'>";
				}
			}
		}

		/// <summary>
		/// Handles the PageIndexChanged event of the AdditiveProfilesDataGrid control.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.DataGridPageChangedEventArgs"/> instance containing the event data.</param>
		private void AdditiveProfilesDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.AdditiveProfilesDataGrid.EditItemIndex > -1)
			{
				return;
			}

			this.AdditiveProfilesDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateView();
		}

		/// <summary>
		/// Enumerates the additive profiles.
		/// </summary>
		/// <returns>A collection of additive profiles</returns>
		private ICollection EnumerateAdditiveProfiles()
		{
			var additiveProfileCollection =
				FMChannelHelper.MakeCall<IAdditiveProfiles, AdditiveProfileCollectionClass>(x => x.Enumerate(this.Security));

			var additiveProfileDataTable = new DataTable();

			additiveProfileDataTable.Columns.Add("SiteGuid", typeof(Guid));
			additiveProfileDataTable.Columns.Add("IdentityGuid", typeof(Guid));
			additiveProfileDataTable.Columns.Add("ID", typeof(string));
			additiveProfileDataTable.Columns.Add("Description", typeof(string));

			foreach (AdditiveProfileClass additiveProfile in additiveProfileCollection)
			{
				DataRow additiveProfileDataRow = additiveProfileDataTable.NewRow();

				additiveProfileDataRow["SiteGuid"] = additiveProfile.SiteGuid;
				additiveProfileDataRow["IdentityGuid"] = additiveProfile.IdentityGuid;
				additiveProfileDataRow["ID"] = additiveProfile.ID;
				additiveProfileDataRow["Description"] = additiveProfile.Description;

				additiveProfileDataTable.Rows.Add(additiveProfileDataRow);
			}

			return new DataView(additiveProfileDataTable);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AddButton2.Command += this.AddButtonCommand;
			this.AdditiveProfilesDataGrid.EditCommand += this.AdditiveProfilesDataGridEditCommand;
			this.AdditiveProfilesDataGrid.PageIndexChanged += this.AdditiveProfilesDataGridPageIndexChanged;
			this.AdditiveProfilesDataGrid.DeleteCommand += this.AdditiveProfilesDataGridDeleteCommand;
			this.AdditiveProfilesDataGrid.ItemDataBound += this.AdditiveProfilesDataGridItemDataBound;
			this.AddButton.Command += this.AddButtonCommand;
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		private void UpdateView()
		{
			var profiles = this.EnumerateAdditiveProfiles();

			this.AdditiveProfilesFormPageSizeDropDown.SetPageSize(this.AdditiveProfilesDataGrid, profiles.Count);

			this.AdditiveProfilesDataGrid.DataSource = profiles;
			this.AdditiveProfilesDataGrid.DataBind();
		}

		#endregion
	}
}