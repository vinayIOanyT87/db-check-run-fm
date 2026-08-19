// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AllocationGroupsForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Code behind for Allocation Groups form
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;
	using FMBusinessObjects.UtilityObjects;

	using global::FMWebApp;

	/// <summary>
	/// Code behind for Allocation Groups form
	/// </summary>
	public partial class AllocationGroupsForm : ApplicationStringsFormBase, 
	                                            IEntityDiscovery, 
	                                            IMenuDiscovery
	{
		#region Explicit Interface Properties

		bool IEntityDiscovery.EntityAssignable
		{
			get
			{
				return true;
			}
		}

		Type IEntityDiscovery.EntityEngineType
		{
			get
			{
				return typeof(IApplicationStrings);
			}
		}

		ENTITY_TYPE IEntityDiscovery.EntityType
		{
			get
			{
				return ENTITY_TYPE.ALLOCATION_GROUP;
			}
		}

		#endregion

		#region Properties

		protected override DataGrid ApplicationDataGrid
		{
			get
			{
				return this.ApplicationStringsDataGrid;
			}
		}

		protected override STRING_TYPE StringType
		{
			get
			{
				return STRING_TYPE.ALLOCATION_GROUP;
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
                // Depends Upon LoadRackService
                if ((options & 0x8000) == 0)
                {
                    return null;
                }
            }


            if (!security.HasRight(RIGHT.VIEW_ALLOCATIONS) && !security.HasRight(RIGHT.MODIFY_ALLOCATIONS))
			{
				return null;
			}

			var menuItems = new List<FMMenuItem>();

			var menuItem = new FMMenuItem
				               {
					               MenuItemType = FMMenuItemType.CONFIG_LOAD_RACK_ALLOCATION_GROUPS, 
					               RootMenuName = "Configuration", 
					               CategoryName = "Load Rack", 
					               ItemName = "Allocation Groups", 
					               NavigateUrl = "AllocationGroupsForm.aspx", 
					               ApplyDataDictionary = ApplyDataDictionary.Apply
				               };

			menuItems.Add(menuItem);

			return menuItems;
		}

		#endregion

		#region Explicit Interface Methods


		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(
			SecurityClass security, ENTITY_ASSIGNMENT_TYPE type)
		{
			ApplicationStringCollectionClass applicationStringCollection =
				FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
					x => x.EnumerateByType(security, this.StringType));

			var entityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			foreach (ApplicationStringClass applicationString in applicationStringCollection)
			{
				if (type == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if (security.SiteGuid == applicationString.SiteGuid)
					{
						continue;
					}

					if (security.LoginSiteGuid != applicationString.SiteGuid)
					{
						continue;
					}
				}
				else
				{
					if (security.SiteGuid != applicationString.SiteGuid)
					{
						continue;
					}
				}

				var entityToSiteMap = new EntityToSiteMapClass(applicationString);
				entityToSiteMapCollection.Add(entityToSiteMap);
			}

			return entityToSiteMapCollection;
		}

		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string id)
		{
			return FMChannelHelper.MakeCall<IApplicationStrings, Guid>(x => x.GetIdentityGuid(security, this.StringType, id));
		}

		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid siteGuid)
		{
			ApplicationStringClass applicationString =
				FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringClass>(x => x.Get(security, guid));

			applicationString.SiteGuid = siteGuid;
			FMChannelHelper.MakeCall<IApplicationStrings>(x => x.Modify(security, applicationString));
		}

		#endregion

		#region Methods

		/// <summary>
		/// Enables the controls during edit/save.
		/// </summary>
		/// <param name="enable">if set to <c>true</c> [b enable].</param>
		protected override void EnableControls(bool enable)
		{
			this.AddButton.Enabled = enable;
			this.AddButton2.Enabled = enable;

			this.AllocationGroupsFormPageSizeDropDown.Enabled = enable;

			if (enable == false)
			{
				this.ApplicationStringsDataGrid.SelectedIndex = -1;
			    this.ResetButton.Enabled = false;
                this.ViewState.Remove("AppStringLastSelect");
			}
		}

		/// <summary>
		/// Raises the <see cref="OnInit" /> event.
		/// </summary>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

			    if (!this.Page.IsPostBack)
			    {
			        if (!this.Security.HasRight(RIGHT.MODIFY_ALLOCATIONS))
			        {
			            this.AddButton.Enabled = false;
			            this.AddButton2.Enabled = false;
			        }

			        // Enumerate 
			        ApplicationStringCollectionClass applicationStringCollection =
			            FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
			                x => x.EnumerateByType(this.Security, STRING_TYPE.ALLOCATION_GROUP));

			        this.Session["ApplicationStringCollection"] = applicationStringCollection;

			        this.UpdateView();

			        this.ResetButton.Enabled = false;
			    }		 
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected override void UpdateView()
		{
		    this.ApplicationStringsDataGrid.SelectedIndex = -1;
		    this.ResetButton.Enabled = false;
            this.ViewState.Remove("AppStringLastSelect");

			this.UpdateView(this.AllocationGroupsFormPageSizeDropDown);
		}

		/// <summary>
		///     Required method for Designer support - do not modify
		///     the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AddButton2.Command += this.AddButtonCommand;
			this.ApplicationStringsDataGrid.EditCommand += this.ApplicationStringsDataGridEditCommand;
			this.ApplicationStringsDataGrid.PageIndexChanged += this.ApplicationStringsDataGridPageIndexChanged;
			this.ApplicationStringsDataGrid.CancelCommand += this.ApplicationStringsDataGridCancelCommand;
			this.ApplicationStringsDataGrid.UpdateCommand += this.ApplicationStringsDataGridUpdateCommand;
			this.ApplicationStringsDataGrid.DeleteCommand += this.ApplicationStringsDataGridDeleteCommand;
			this.ApplicationStringsDataGrid.ItemDataBound += this.ApplicationStringsDataGridItemDataBound;
			this.AddButton.Command += this.AddButtonCommand;
			this.ResetButton.Command += this.ResetButton_Command;
		}

		private void ResetButton_Command(object sender, CommandEventArgs e)
		{
			try
			{
			    if (this.ApplicationStringsDataGrid.SelectedItem == null)
			    {
			        return;
			    }

				var identityGuidLabel = (Label)this.ApplicationStringsDataGrid.SelectedItem.FindControl("IdentityGuidLabel");
				if (identityGuidLabel != null)
				{
					ApplicationStringCollectionClass applicationStringCollection = (ApplicationStringCollectionClass)this.Session["ApplicationStringCollection"];

					ApplicationStringClass applicationString = applicationStringCollection.Find(x => x.IdentityGuid == Guid.Parse(identityGuidLabel.Text));

					AllocationCollectionClass allocationCollection =
						FMChannelHelper.MakeCall<IAllocations, AllocationCollectionClass>(
							x => x.EnumerateByAllocationGroupGuid(this.Security, applicationString.IdentityGuid));

					SiteClass site =
						FMChannelHelper.MakeCall<ISites, SiteClass>(
							x => x.GetByMemberAndProcessVariables(this.Security, this.Security.SiteGuid, false, false));
					DateTimeOffset siteTimeToday = TimeConverter.Today(site);

					foreach (AllocationClass allocation in allocationCollection)
					{
						allocation.LastAllocationResetDate.Value = siteTimeToday;

						foreach (AllocationLineItemClass lineItem in allocation.LineItemCollection)
						{
							lineItem.SetResetDate(allocation._EffectiveDate.Value, allocation._ExpirationDate.Value, siteTimeToday);
						}

						FMChannelHelper.MakeCall<IAllocations>(x => x.Modify(this.Security, allocation));
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

        /// <summary>
        /// When a user selects an allocation group by clicking the "Select" column for a row in the grid, enable the Reset button.
        /// If the user is deselecting an item, disable the reset button
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        protected override void ApplicationDataGridSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                base.ApplicationDataGridSelectedIndexChanged(sender, e);

                if (this.Security.HasRight(RIGHT.MODIFY_ALLOCATIONS) && this.ApplicationStringsDataGrid.SelectedIndex != -1)
                {
                    this.ResetButton.Enabled = true;
                }
                else
                {
                    this.ResetButton.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

		#endregion
	}
}