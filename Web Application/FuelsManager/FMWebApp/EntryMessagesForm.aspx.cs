// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntryMessagesForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the EntryMessagesForm type.
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

	using global::FMWebApp;

	/// <summary>
	///    Summary description for EntryMessagesForm.
	/// </summary>
	public partial class EntryMessagesForm : ApplicationStringsFormBase, IEntityDiscovery, IMenuDiscovery
	{
		#region Constants and Fields

		protected LinkButton DeleteButton;

		protected LinkButton EditButton;

		protected LinkButton Linkbutton1;

		protected LinkButton Linkbutton2;

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
				return ENTITY_TYPE.ENTRY_MESSAGE;
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

		protected override bool DisableEditDeleteButtons(object sender, DataGridItemEventArgs e)
		{
			// Disable the buttons if the user does not have the Modify Products right.
			return this.Security.HasRight(RIGHT.MODIFY_PRODUCTS) == false;
		}

		protected override STRING_TYPE StringType
		{
			get
			{
				return STRING_TYPE.ENTRY_MESSAGE;
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
                if ((word2 & 0x01) != 0x01)
                    return null;
            }
            else
            {
                if ((options & 0x8000) == 0)
                {
                    return null;
                }
                // Depends Upon Shared Components Config
                if ((options & 0x4000) == 0)
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
					MenuItemType = FMMenuItemType.ASSETS_PRODUCTS_PRODUCT_ENTRY_MESSAGES,
					RootMenuName = "Assets",
					CategoryName = "Products",
					ItemName = "Product Entry Messages",
					NavigateUrl = "EntryMessagesForm.aspx",
					ApplyDataDictionary = ApplyDataDictionary.Apply
				};

			menuItems.Add(menuItem);

			return menuItems;
		}

		#endregion

		#region Explicit Interface Methods

		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(
			SecurityClass Security, ENTITY_ASSIGNMENT_TYPE Type)
		{
			ApplicationStringCollectionClass applicationStringCollection;
			applicationStringCollection =
				FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
					x => x.EnumerateByType(Security, this.StringType));

			var EntityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			foreach (ApplicationStringClass applicationString in applicationStringCollection)
			{
				if (Type == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if (Security.SiteGuid == applicationString.SiteGuid)
					{
						continue;
					}

					if (Security.LoginSiteGuid != applicationString.SiteGuid)
					{
						continue;
					}
				}
				else
				{
					if (Security.SiteGuid != applicationString.SiteGuid)
					{
						continue;
					}
				}

				var EntityToSiteMap = new EntityToSiteMapClass(applicationString);
				EntityToSiteMapCollection.Add(EntityToSiteMap);
			}
			return EntityToSiteMapCollection;
		}

		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string ID)
		{
			return FMChannelHelper.MakeCall<IApplicationStrings, Guid>(x => x.GetIdentityGuid(security, this.StringType, ID));
		}

		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid SiteGuid)
		{
			ApplicationStringClass applicationString =
				FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringClass>(x => x.Get(security, guid));

			applicationString.SiteGuid = SiteGuid;
			FMChannelHelper.MakeCall<IApplicationStrings>(x => x.Modify(security, applicationString));
		}

		#endregion

		#region Methods

		protected override void EnableControls(bool enable)
		{
			this.AddButton.Enabled = enable;
			this.AddButton2.Enabled = enable;

			this.EntryMessagesFormPageSizeDropDown.Enabled = enable;
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
					if (!this.Security.HasRight(RIGHT.MODIFY_PRODUCTS))
					{
						this.AddButton.Enabled = false;
						this.AddButton2.Enabled = false;
					}

					// Enumerate 
					ApplicationStringCollectionClass ApplicationStringCollection =
						FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
							x => x.EnumerateByType(this.Security, STRING_TYPE.ENTRY_MESSAGE));
					this.Session["ApplicationStringCollection"] = ApplicationStringCollection;

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
			this.UpdateView(this.EntryMessagesFormPageSizeDropDown);
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