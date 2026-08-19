// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessVariableMessagesForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ProcessVariableMessagesForm type.
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
	///    Summary description for ProcessVariableMessagesForm.
	/// </summary>
	public partial class ProcessVariableMessagesForm : ApplicationStringsFormBase,
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
				return ENTITY_TYPE.PROCESS_VARIABLE_MESSAGE;
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
			if (!this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				return true;
			}
			else if (((this.StringType == STRING_TYPE.PRODUCT_MESSAGE || this.StringType == STRING_TYPE.DOT_HAZARDOUS_MESSAGE)
							&& !this.Security.HasRight(RIGHT.MODIFY_PRODUCTS))
						|| (this.StringType == STRING_TYPE.COMPANY_TYPE && !this.Security.HasRight(RIGHT.MODIFY_COMPANY_DATA))
						|| (this.StringType == STRING_TYPE.ALLOCATION_GROUP && !this.Security.HasRight(RIGHT.MODIFY_ALLOCATIONS)))
			{
				return true;
			}
			return false;
		}

		protected override STRING_TYPE StringType
		{
			get
			{
				return STRING_TYPE.PROCESS_VARIABLE_MESSAGE;
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
                // Depends upon Load Rack Service
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
            var items = new List<FMMenuItem>();

			if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				return null;
			}

			items.Add(
				new FMMenuItem
					{
						MenuItemType = FMMenuItemType.CONFIG_SITES_PROCESS_VARIABLE_MESSAGES,
						RootMenuName = "Configuration",
						CategoryName = "Sites",
						ItemName = "Process Variable Messages",
						NavigateUrl = "ProcessVariableMessagesForm.aspx",
						ApplyDataDictionary = ApplyDataDictionary.Apply
					});

			return items;
		}

		#endregion

		#region Explicit Interface Methods

		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(
			SecurityClass Security, ENTITY_ASSIGNMENT_TYPE Type)
		{
			ApplicationStringCollectionClass ApplicationStringCollection;
			ApplicationStringCollection = FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
																	 x =>
																	 x.EnumerateByType(Security, this.StringType)
																);


			var EntityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			foreach (ApplicationStringClass ApplicationString in ApplicationStringCollection)
			{
				if (Type == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if (Security.SiteGuid == ApplicationString.SiteGuid)
					{
						continue;
					}

					if (Security.SiteGuid != ApplicationString.SiteGuid)
					{
						continue;
					}
				}
				else
				{
					if (Security.SiteGuid != ApplicationString.SiteGuid)
					{
						continue;
					}
				}

				var EntityToSiteMap = new EntityToSiteMapClass(ApplicationString);
				EntityToSiteMapCollection.Add(EntityToSiteMap);
			}
			return EntityToSiteMapCollection;
		}

		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string ID)
		{
			return FMChannelHelper.MakeCall<IApplicationStrings, Guid>(
																	 x =>
																	 x.GetIdentityGuid(security, this.StringType, ID)
																);
		}

		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid SiteGuid)
		{
			ApplicationStringClass ApplicationString = FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringClass>(
																	 x =>
																	 x.Get(security, guid)
																);

			ApplicationString.SiteGuid = SiteGuid;
			FMChannelHelper.MakeCall<IApplicationStrings>(
																	 x =>
																	 x.Modify(security, ApplicationString)
																);
		}

		#endregion

		#region Methods

		protected override void EnableControls(bool enable)
		{
			this.AddButton.Enabled = enable;
			this.AddButton2.Enabled = enable;

			this.ProcessVariableMessagesFormPageSizeDropDown.Enabled = enable;
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
					if (!this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
					{
						this.AddButton.Enabled = false;
						this.AddButton2.Enabled = false;
					}

					// Enumerate 
					ApplicationStringCollectionClass ApplicationStringCollection = FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
																	 x =>
																	 x.EnumerateByType(this.Security, STRING_TYPE.PROCESS_VARIABLE_MESSAGE)
																);
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
			this.UpdateView(this.ProcessVariableMessagesFormPageSizeDropDown);
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AddButton2.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButtonCommand);
			this.ApplicationStringsDataGrid.EditCommand +=
				new System.Web.UI.WebControls.DataGridCommandEventHandler(this.ApplicationStringsDataGridEditCommand);
			this.ApplicationStringsDataGrid.PageIndexChanged +=
				new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.ApplicationStringsDataGridPageIndexChanged);
			this.ApplicationStringsDataGrid.CancelCommand +=
				new System.Web.UI.WebControls.DataGridCommandEventHandler(this.ApplicationStringsDataGridCancelCommand);
			this.ApplicationStringsDataGrid.UpdateCommand +=
				new System.Web.UI.WebControls.DataGridCommandEventHandler(this.ApplicationStringsDataGridUpdateCommand);
			this.ApplicationStringsDataGrid.DeleteCommand +=
				new System.Web.UI.WebControls.DataGridCommandEventHandler(this.ApplicationStringsDataGridDeleteCommand);
			this.ApplicationStringsDataGrid.ItemDataBound +=
				new System.Web.UI.WebControls.DataGridItemEventHandler(this.ApplicationStringsDataGridItemDataBound);
			this.AddButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButtonCommand);
		}

		#endregion
	}
}