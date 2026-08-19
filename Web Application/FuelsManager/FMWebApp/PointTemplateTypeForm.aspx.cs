namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;

	/// <summary>
	///    Summary description for PointTemplateTypeForm.
	/// </summary>
	public partial class PointTemplateTypeForm : ApplicationStringsFormBase,
																IEntityDiscovery,
																IMenuDiscovery
	{
		#region Public Methods and Operators
		/// <summary>
		///    Gets a list of menu items that should be displayed for the current user.
		/// </summary>
		/// <param name="security">The security object of the current session</param>
		/// <param name="siteGroup">Whether the current logged-in site is a site group</param>
		/// <returns>
		///    List of menu items to be displayed
		/// </returns>
		public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1, ushort word2, ushort useNewLicenseKey, uint options)
		{
			if ((word1 & 0x80) != 0x80)
				return null;

			if (!security.HasRight(RIGHT.VIEW_POINT_TYPES))
			{
				return null;
			}

			var menuItems = new List<FMMenuItem>();

			var menuItem = new FMMenuItem
			               {
				               MenuItemType = FMMenuItemType.CONFIG_INVMGR_POINT_TEMPLATE_TYPE,
				               RootMenuName = "Configuration",
				               CategoryName = "Inventory Management",
				               ItemName = "Point Types",
				               NavigateUrl = "PointTemplateTypeForm.aspx",
				               ApplyDataDictionary = ApplyDataDictionary.Apply
			               };

			menuItems.Add(menuItem);

			return menuItems;
		}

		#endregion

		#region Explicit Interface Properties
		bool IEntityDiscovery.EntityAssignable
		{
			get { return true; }
		}

		Type IEntityDiscovery.EntityEngineType
		{
			get { return typeof( IApplicationStrings ); }
		}

		ENTITY_TYPE IEntityDiscovery.EntityType
		{
			get { return ENTITY_TYPE.POINT_TEMPLATE_TYPE; }
		}
		#endregion

		#region Properties
		protected override DataGrid ApplicationDataGrid
		{
			get { return this.ApplicationStringsDataGrid; }
		}

		protected override STRING_TYPE StringType
		{
			get { return STRING_TYPE.POINT_TEMPLATE_TYPE; }
		}
		#endregion

		#region Private data members
		private List<Guid> protectedPointTypeGuids;
		private bool isMovementKey;
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
		public List<FMMenuItem> GetMenuItems( SecurityClass security, bool siteGroup, uint options, ushort word1, ushort word2, ushort useNewLicenseKey)
		{
			if ((word1 & 0x80) != 0x80)
				return null;

			if ( !security.HasRight( RIGHT.VIEW_POINT_TYPES ) )
			{
				return null;
			}


			var menuItems = new List<FMMenuItem>();

			var menuItem = new FMMenuItem
			{
				MenuItemType = FMMenuItemType.CONFIG_INVMGR_POINT_TEMPLATE_TYPE,
				RootMenuName = "Configuration",
				CategoryName = "Inventory Management",
				ItemName = "Point Types",
				NavigateUrl = "PointTemplateTypeForm.aspx",
				ApplyDataDictionary = ApplyDataDictionary.Apply
			};

			menuItems.Add( menuItem );

			return menuItems;
		}

		#endregion

		#region Explicit Interface Methods

		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(SecurityClass security, ENTITY_ASSIGNMENT_TYPE entityAssignmentType )
		{
			var applicationStringCollection = FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
																					x => x.EnumerateByType( security, this.StringType ) );

			var entityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			foreach ( ApplicationStringClass applicationString in applicationStringCollection )
			{
				if (entityAssignmentType == ENTITY_ASSIGNMENT_TYPE.ASSIGNED )
				{
					if ( security.SiteGuid == applicationString.SiteGuid )
					{
						continue;
					}

					if ( security.SiteGuid != applicationString.SiteGuid )
					{
						continue;
					}
				}
				else
				{
					if ( security.SiteGuid != applicationString.SiteGuid )
					{
						continue;
					}
				}

				var entityToSiteMap = new EntityToSiteMapClass( applicationString );
				entityToSiteMapCollection.Add( entityToSiteMap );
			}
			return entityToSiteMapCollection;
		}

		Guid IEntityDiscovery.GetIdentityGuid( SecurityClass security, string id )
		{
			return FMChannelHelper.MakeCall<IApplicationStrings, Guid>( x => x.GetIdentityGuid( security, this.StringType, id ) );
		}

		void IEntityDiscovery.SetSiteGuid( SecurityClass security, Guid guid, Guid siteGuid )
		{
			ApplicationStringClass applicationString = FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringClass>( x => x.Get( security, guid ) );
			applicationString.SiteGuid = siteGuid;
			FMChannelHelper.MakeCall<IApplicationStrings>( x => x.Modify( security, applicationString ) );
		}
		#endregion

		#region Methods
		protected override void EnableControls( bool bEnable )
		{
			this.AddButton.Enabled = bEnable;
			this.AddButton2.Enabled = bEnable;

			this.PointTemplateTypePageSizeDropDown.Enabled = bEnable;
		}

		protected override bool DisableEditDeleteButtons(object sender, DataGridItemEventArgs e)
		{
			// Disable the buttons if the user does not have the Modify Point Types right.
			if(!this.Security.HasRight(RIGHT.MODIFY_POINT_TYPES))
			{
				return true;
			}

			Guid? appStringGuid = ((DataRowView)e.Item.DataItem).Row.ItemArray[1] as Guid?;

			bool found = false;

			if (appStringGuid != Guid.Empty)
			{

				foreach (Guid protectedGuid in this.protectedPointTypeGuids)
				{
					if (appStringGuid == protectedGuid)
					{
						found = true;
						break;
					}
				}
			}

			return found;
		}


        protected override void ApplicationStringsDataGridItemDataBound(object sender, DataGridItemEventArgs e)
        {
            this.GetSecurity();

            var siteGuidLabel = (Label)e.Item.FindControl("SiteGuidLabel");

            if (siteGuidLabel != null)
            {
                bool disable = this.DisableEditDeleteButtons(sender, e) || (this.Security.SiteGuid != Guid.Parse(siteGuidLabel.Text));

                // Update the edit button setting text and image file based on "enabled" status
                var editButton = (LinkButton)e.Item.FindControl("EditButton");

                if (editButton != null)
                {
                    editButton.Enabled = disable == false;

                    //There is a single exception. The 'System' type can be edited, but not deleted. 
                    if (disable)
                    {
                        Guid? appStringGuid = ((DataRowView)e.Item.DataItem).Row.ItemArray[1] as Guid?;
                        if (appStringGuid != Guid.Empty && appStringGuid == Guid.Parse("2DDEB3E0-545C-444B-B1BF-9CAB048F21B7") && this.Security.SiteGuid == Guid.Parse(siteGuidLabel.Text))
                        {
                            editButton.Enabled = true;                       }
                        }

                }

                // Update the delete button setting text and image file based on "enabled" status
                var deleteButton = (LinkButton)e.Item.FindControl("DeleteButton");

                if (deleteButton != null)
                {
                    deleteButton.Enabled = disable == false;
                }
            }

            // The select and delete buttons need to be disabled when in edit mode.
            if (this.ApplicationDataGrid != null && this.ApplicationDataGrid.EditItemIndex != -1)
            {
                var control = e.Item.FindControl("SelectButton") as LinkButton;
                if (control != null)
                {
                    control.Enabled = false;
                }

                control = e.Item.FindControl("DeleteButton") as LinkButton;
                if (control != null)
                {
                    control.Enabled = false;
                }

                control = e.Item.FindControl("EditButton") as LinkButton;
                if (control != null)
                {
                    control.Enabled = false;
                }
            }

            if ((this.ApplicationDataGrid != null && this.ApplicationDataGrid.EditItemIndex == e.Item.ItemIndex)
                || this.PriorEditItemIndex == e.Item.ItemIndex)
            {
                // Now set the focus to the edit control
                Control ctrl;

                var applicationDataGrid = this.ApplicationDataGrid;
                if (applicationDataGrid != null && applicationDataGrid.EditItemIndex == e.Item.ItemIndex)
                {
                    ctrl = e.Item.FindControl("StringTextBox");
                }
                else
                {
                    ctrl = e.Item.FindControl("EditButton");
                }

                if (ctrl != null)
                {
                    const string Script = @"<script language='javascript'> document.getElementById('{0}').focus(); </script>";
                    this.Page.ClientScript.RegisterStartupScript(
                       this.GetType(), "page_set_focus", string.Format(Script, ctrl.ClientID));
                }
            }
        }


        protected override void OnInit( EventArgs e )
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit( e );
		}

		protected void Page_Load( object sender, EventArgs e )
		{
			try
			{
				this.isMovementKey = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsMovementKey());

				this.GetSecurity();

				// Load the protected Point Type GUIDs
				this.LoadProtectedPointTypeGuids();

				if ( !this.Page.IsPostBack )
				{
					if ( !this.Security.HasRight( RIGHT.MODIFY_POINT_TYPES) )
					{
						this.AddButton.Enabled = false;
						this.AddButton2.Enabled = false;
					}

					// Enumerate 
					ApplicationStringCollectionClass applicationStringCollection =
						FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
							x => x.EnumerateByType( this.Security, STRING_TYPE.POINT_TEMPLATE_TYPE ) );

					// Remove movement items if the movement hardware key is not set.
					ApplicationStringCollectionClass updatedAppStrCollection = this.RemoveMovementBasedOnHardwareKey(applicationStringCollection);

					this.Session["ApplicationStringCollection"] = updatedAppStrCollection;

					this.UpdateView();
				}
			}
			catch ( Exception except )
			{
				this.ErrorHandler( except );
			}
		}

		/// <summary>
		/// This method will remove the movement items if the movement hardware key is not set.
		/// </summary>
		/// <param name="applicationStringCollection"></param>
		/// <returns>Returns a new list of application strings.</returns>
		private ApplicationStringCollectionClass RemoveMovementBasedOnHardwareKey(ApplicationStringCollectionClass applicationStringCollection)
        {
			if(applicationStringCollection == null || applicationStringCollection.Count == 0 || this.isMovementKey)
            {
				return applicationStringCollection;
            }

			var applicationStrList = new ApplicationStringCollectionClass();

			foreach(ApplicationStringClass appStr in applicationStringCollection)
            {
				if(appStr.ID.ToUpper() != "MOVEMENT" && appStr.ID.ToUpper() != "MOVEMENT NODE")
                {
					applicationStrList.Add(appStr);
                }
            }

			return applicationStrList;
        }

		/// <summary>
		/// This method will load the protected Point Type GUIDs into a collection.
		/// </summary>
		private void LoadProtectedPointTypeGuids()
		{
			// These GUIDs must match what is in the database in order to disable the delete
			// icon button.
			this.protectedPointTypeGuids = new List<Guid>
			                               {
												Guid.Parse("E78CD406-4C19-4978-8940-FA4E404E3E53"),	// Tank GUID
												Guid.Parse("E33A769F-3EFC-46C6-A50F-A103454BFE97"),	// Valve GUID
												Guid.Parse("1135AA41-525B-4024-BF3D-6BF2D55A034B"),	// Pump GUID
												Guid.Parse("9403A36F-33F6-4DCC-857D-F53C8DC66196"),	// Meter GUID
												Guid.Parse("7EA082F3-6FBF-4136-A2D7-8A3670E9A9EF"),	// Preset GUID
												Guid.Parse("55F0E8B8-3A74-40D0-8B8C-675A4B6A478C"),	// Pipe GUID
												Guid.Parse("2DDEB3E0-545C-444B-B1BF-9CAB048F21B7")  // System GUID
											};

			if(this.isMovementKey)
            {
				// Movement GUID
				this.protectedPointTypeGuids.Add(Guid.Parse("A89562CE-FB16-47D3-9BD6-C33AD3BD2141"));

				// Movement Node GUID
				this.protectedPointTypeGuids.Add(Guid.Parse("E8CA745C-2C38-4B52-B15C-EF738AD41305"));
			}
		}

        protected override void UpdateView()
		{
			this.UpdateView( this.PointTemplateTypePageSizeDropDown );
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
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
		}
		#endregion

	}
}