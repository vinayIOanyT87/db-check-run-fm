namespace FuelsManager.FuelCardWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;

	using FMControls;

	using FMWebApp;

	using global::FMWebApp;

	/// <summary>
    ///    Summary description for FCRC_FuelCardTypesForm.
	/// </summary>
	public partial class FCRC_FuelCardTypesForm : ApplicationStringsFormBase, IEntityDiscovery, IMenuDiscovery
	{
		#region Constants and Fields
        public const string NavigateUrl = "../FuelCardWebApp/FCRC_FuelCardTypesForm.aspx";
		protected FMButton Add;
		#endregion

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
				return ENTITY_TYPE.FUEL_CARD_TYPE;
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
				return STRING_TYPE.FUEL_CARD_TYPE;
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

            }
            else
            {
                // Depends Upon Accounting
                if ((options & 0x80100) == 0)
                {
                    return null;
                }
            }
            if (!security.HasRight(RIGHT.VIEW_FUEL_CARD_DATA) && !security.HasRight(RIGHT.MODIFY_FUEL_CARD_DATA))
			{
				return null;
			}


			var menuItems = new List<FMMenuItem>();

			var menuItem = new FMMenuItem
				{
					MenuItemType = FMMenuItemType.CONFIG_OTHER_FUEL_CARD_TYPES,
                    RootMenuName = "Configuration",
                    CategoryName = "Other",
                    ItemName = "Fuel Card Types",
                    NavigateUrl = NavigateUrl,
					ApplyDataDictionary = ApplyDataDictionary.Apply
				};

			menuItems.Add(menuItem);

			return menuItems;
		}
		#endregion

		#region Explicit Interface Methods
		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(SecurityClass inSecurity, 
																			ENTITY_ASSIGNMENT_TYPE entityAssignmentType)
		{
			ApplicationStringCollectionClass applicationStringCollection = 
				FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
																	 x =>
																	 x.EnumerateByType(
				inSecurity, this.StringType));


			var entityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			foreach (ApplicationStringClass applicationString in applicationStringCollection)
			{
				if (entityAssignmentType == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if (inSecurity.SiteGuid == applicationString.SiteGuid)
					{
						continue;
					}

					if (inSecurity.LoginSiteGuid != applicationString.SiteGuid)
					{
						continue;
					}
				}
				else
				{
					if (inSecurity.SiteGuid != applicationString.SiteGuid)
					{
						continue;
					}
				}

				var entityToSiteMap = new EntityToSiteMapClass(applicationString);
				entityToSiteMapCollection.Add(entityToSiteMap);
			}

			return entityToSiteMapCollection;
		}

		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string ID)
		{
			return FMChannelHelper.MakeCall<IApplicationStrings, Guid>(
																	 x =>
																	 x.GetIdentityGuid(security,this.StringType, ID)
																);
		}

		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid siteGuid)
		{
			ApplicationStringClass applicationString = FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringClass>(
																	 x =>
																	 x.Get(security, guid)
																);

			applicationString.SiteGuid = siteGuid;
			FMChannelHelper.MakeCall<IApplicationStrings>(
																	 x =>
																	 x.Modify(security,applicationString)
																);

		}
		#endregion

		#region Methods
		protected override void EnableControls(bool enable)
		{
			this.AddButton.Enabled = enable;
			this.AddButton2.Enabled = enable;

			this.FuelCardTypesFormPageSizeDropDown.Enabled = enable;
		}

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

				if (!this.Page.IsPostBack)
				{
					if (!this.Security.HasRight(RIGHT.MODIFY_FUEL_CARD_DATA))
					{
						this.AddButton.Enabled = false;
						this.AddButton2.Enabled = false;
					}

					// Enumerate 
					ApplicationStringCollectionClass applicationStringCollection = 
						FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
							x =>
							x.EnumerateByType(this.Security, STRING_TYPE.FUEL_CARD_TYPE)
						);

					this.Session["ApplicationStringCollection"] = applicationStringCollection;

					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected override void UpdateView()
		{
            this.UpdateView(this.FuelCardTypesFormPageSizeDropDown);
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AddButton2.Command								+= this.AddButtonCommand;
			this.ApplicationStringsDataGrid.EditCommand			+= this.ApplicationStringsDataGridEditCommand;
			this.ApplicationStringsDataGrid.PageIndexChanged	+= this.ApplicationStringsDataGridPageIndexChanged;
			this.ApplicationStringsDataGrid.CancelCommand		+= this.ApplicationStringsDataGridCancelCommand;
			this.ApplicationStringsDataGrid.UpdateCommand		+= this.ApplicationStringsDataGridUpdateCommand;
			this.ApplicationStringsDataGrid.DeleteCommand		+= this.ApplicationStringsDataGridDeleteCommand;
			this.ApplicationStringsDataGrid.ItemDataBound		+= this.ApplicationStringsDataGridItemDataBound;
			this.AddButton.Command								+= this.AddButtonCommand;
		}
		#endregion
	}
}