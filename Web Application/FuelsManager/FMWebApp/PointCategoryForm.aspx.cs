namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;

	/// <summary>
	///    Summary description for PointCategoryForm.
	/// </summary>
	public partial class PointCategoryForm : ApplicationStringsFormBase, IEntityDiscovery, IMenuDiscovery
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
				return typeof( IApplicationStrings );
			}
		}

		ENTITY_TYPE IEntityDiscovery.EntityType
		{
			get
			{
				return ENTITY_TYPE.POINT_CATEGORY;
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
				return STRING_TYPE.POINT_CATEGORY;
			}
		}

		#endregion

		#region Public Methods and Operators

			/// <summary>
			///    Gets a list of menu items that should be displayed for the current user.
			/// </summary>
			/// <param name="security">The security object of the current session</param>
			/// <param name="siteGroup">Whether the current logged-in site is a site group</param>
			/// <returns>
			///    List of menu items to be displayed
			/// </returns>
			/// 

		public List<FMMenuItem> GetMenuItems( SecurityClass security, bool siteGroup, ushort word1, ushort word2, ushort useNewLicenseKey, uint options)
		{
			if ((word1 & 0x80) != 0x80)
				return null;

			if ( !security.HasRight( RIGHT.VIEW_POINT_CATEGORIES) )
			{
				return null;
			}

			var menuItems = new List<FMMenuItem>();

			var menuItem = new FMMenuItem
			{
				MenuItemType = FMMenuItemType.CONFIG_INVMGR_POINT_CATEGORY,
				RootMenuName = "Configuration",
				CategoryName = "Inventory Management",
				ItemName = "Point Categories",
				NavigateUrl = "PointCategoryForm.aspx",
				ApplyDataDictionary = ApplyDataDictionary.Apply
			};

			menuItems.Add( menuItem );

			return menuItems;
		}

		#endregion

		#region Explicit Interface Methods

		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(
			SecurityClass Security, ENTITY_ASSIGNMENT_TYPE Type )
		{
			ApplicationStringCollectionClass ApplicationStringCollection;
			ApplicationStringCollection =
				FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
					x => x.EnumerateByType( Security, this.StringType ) );

			var EntityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			foreach ( ApplicationStringClass ApplicationString in ApplicationStringCollection )
			{
				if ( Type == ENTITY_ASSIGNMENT_TYPE.ASSIGNED )
				{
					if ( Security.SiteGuid == ApplicationString.SiteGuid )
					{
						continue;
					}

					if ( Security.SiteGuid != ApplicationString.SiteGuid )
					{
						continue;
					}
				}
				else
				{
					if ( Security.SiteGuid != ApplicationString.SiteGuid )
					{
						continue;
					}
				}

				var EntityToSiteMap = new EntityToSiteMapClass( ApplicationString );
				EntityToSiteMapCollection.Add( EntityToSiteMap );
			}
			return EntityToSiteMapCollection;
		}

		Guid IEntityDiscovery.GetIdentityGuid( SecurityClass security, string ID )
		{
			return FMChannelHelper.MakeCall<IApplicationStrings, Guid>( x => x.GetIdentityGuid( security, this.StringType, ID ) );
		}

		void IEntityDiscovery.SetSiteGuid( SecurityClass security, Guid guid, Guid SiteGuid )
		{
			ApplicationStringClass applicationString =
				FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringClass>( x => x.Get( security, guid ) );
			applicationString.SiteGuid = SiteGuid;
			FMChannelHelper.MakeCall<IApplicationStrings>( x => x.Modify( security, applicationString ) );
		}

		#endregion

		#region Methods

		protected override void EnableControls( bool bEnable )
		{
			this.AddButton.Enabled = bEnable;
			this.AddButton2.Enabled = bEnable;

			this.PointCategoryPageSizeDropDown.Enabled = bEnable;
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
				this.GetSecurity();

				if ( !this.Page.IsPostBack )
				{
					if ( !this.Security.HasRight( RIGHT.MODIFY_POINT_CATEGORIES ) )
					{
						this.AddButton.Enabled = false;
						this.AddButton2.Enabled = false;

					}

					// Enumerate 
					ApplicationStringCollectionClass ApplicationStringCollection =
						FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
							x => x.EnumerateByType( this.Security, STRING_TYPE.POINT_CATEGORY ) );

					this.Session["ApplicationStringCollection"] = ApplicationStringCollection;

					this.UpdateView();
				}
			}
			catch ( Exception except )
			{
				this.ErrorHandler( except );
			}
		}



		protected override void UpdateView()
		{
			this.UpdateView( this.PointCategoryPageSizeDropDown );
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