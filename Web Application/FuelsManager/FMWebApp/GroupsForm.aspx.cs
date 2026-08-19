// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupsForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the GroupsForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Data;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;

	using global::FMWebApp;

	/// <summary>
	///    Summary description for GroupsForm.
	/// </summary>
	public partial class GroupsForm : FMFormBase, IEntityDiscovery, IMenuDiscovery
	{
		#region Constants and Fields

		private const string SESSION_KEY_GROUPS_PAGEINDEX = "GroupsPageIndex";

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
				return typeof(IGroups);
			}
		}

		ENTITY_TYPE IEntityDiscovery.EntityType
		{
			get
			{
				return ENTITY_TYPE.GROUP;
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
                // Depends Upon Shared Components Config
                if ((options & 0x4000) == 0)
                {
                    return null;
                }
            }
            var items = new List<FMMenuItem>();

			if (!security.HasRight(RIGHT.VIEW_USER_GROUPS) && !security.HasRight(RIGHT.MODIFY_USER_GROUPS))
			{
				return null;
			}

			items.Add(
				new FMMenuItem
					{
						MenuItemType = FMMenuItemType.ADMIN_SECURITY_USER_GROUPS,
						RootMenuName = "Administration",
						CategoryName = "Security",
						ItemName = "User Groups",
						NavigateUrl = GroupsForm.GroupsFormURL,
						ApplyDataDictionary = ApplyDataDictionary.Apply
					});

			return items;
		}

		#endregion

		#region Explicit Interface Methods

		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(
			SecurityClass Security, ENTITY_ASSIGNMENT_TYPE Type)
		{
			GroupCollectionClass GroupCollection;
			GroupCollection = FMChannelHelper.MakeCall<IGroups, GroupCollectionClass>(x => x.Enumerate(Security));

			var EntityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			foreach (GroupClass Group in GroupCollection)
			{
				if (Type == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if (Security.SiteGuid == Group.SiteGuid)
					{
						continue;
					}

					if (Security.LoginSiteGuid != Group.SiteGuid)
					{
						continue;
					}
				}
				else
				{
					if (Security.SiteGuid != Group.SiteGuid)
					{
						continue;
					}
				}

				var EntityToSiteMap = new EntityToSiteMapClass(Group);
				EntityToSiteMapCollection.Add(EntityToSiteMap);
			}
			return EntityToSiteMapCollection;
		}

		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string ID)
		{
			return FMChannelHelper.MakeCall<IGroups, Guid>(x => x.GetIdentityGuid(security, ID));
		}

		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid targetGroupGuid, Guid SiteGuid)
		{
			GroupClass Group = FMChannelHelper.MakeCall<IGroups, GroupClass>(x => x.Get(security, targetGroupGuid));

			Group.SiteGuid = SiteGuid;
			FMChannelHelper.MakeCall<IGroups>(x => x.Modify(security, Group));
		}

		#endregion

		#region Methods

		public static string GroupsFormURL
		{
			get
			{
				string groupsFormURL = ConfigurationManager.AppSettings["GroupsFormURL"];
				if (string.IsNullOrEmpty(groupsFormURL))
				{
					groupsFormURL = "FMWebApp/GroupsForm.aspx";
				}
				return "../" + groupsFormURL;
			}
		}

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
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
					if (!this.Security.HasRight(RIGHT.MODIFY_USER_GROUPS))
					{
						this.AddButton.Enabled = false;
						this.AddButton2.Enabled = false;
					}

					if (this.Session[SESSION_KEY_GROUPS_PAGEINDEX] != null)
					{
						this.GroupsDataGrid.CurrentPageIndex = (int)this.Session[SESSION_KEY_GROUPS_PAGEINDEX];
						this.Session.Remove(SESSION_KEY_GROUPS_PAGEINDEX);
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
			this.Session.Remove(GroupForm.SESSION_KEY_GROUP_GUID);
			this.Session[SESSION_KEY_GROUPS_PAGEINDEX] = this.GroupsDataGrid.CurrentPageIndex;
			this.Redirect(GroupForm.GroupFormURL);
		}

		private Guid GetGuidFromGridArgument(DataGridItem theDataGrid)
		{
			return new Guid(theDataGrid.Cells[2].Text);
		}

		private Guid GetSiteGuidFromGridArgument(DataGridItem theDataGrid)
		{
			return new Guid(theDataGrid.Cells[1].Text);
		}

		private void GroupsDataGrid_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				FMChannelHelper.MakeCall<IGroups>(x => x.Purge(this.Security, this.GetGuidFromGridArgument(e.Item)));

				this.GroupsDataGrid.SelectedIndex = -1;
				this.Session.Remove(GroupForm.SESSION_KEY_GROUP_GUID);
				if (this.GroupsDataGrid.Items.Count == 1 && this.GroupsDataGrid.CurrentPageIndex > 0)
				{
					this.GroupsDataGrid.CurrentPageIndex--;
				}
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void GroupsDataGrid_EditCommand(object source, DataGridCommandEventArgs e)
		{
			this.Session[GroupForm.SESSION_KEY_GROUP_GUID] = this.GetGuidFromGridArgument(e.Item);
			this.Session[SESSION_KEY_GROUPS_PAGEINDEX] = this.GroupsDataGrid.CurrentPageIndex;
			this.Redirect(GroupForm.GroupFormURL);
		}

		private void GroupsDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			this.GetSecurity();
			var EditButton = (LinkButton)e.Item.FindControl("EditButton");
			var DeleteButton = (LinkButton)e.Item.FindControl("DeleteButton");
			if (EditButton != null && DeleteButton != null)
			{
				Guid groupGuid = this.GetGuidFromGridArgument(e.Item);
				Guid siteGuid = this.GetSiteGuidFromGridArgument(e.Item);

				if (!this.Security.HasRight(RIGHT.MODIFY_USER_GROUPS) || (this.Security.SiteGuid != siteGuid)
				    || GroupClass.IsAdminGroupGuid(groupGuid))
				{
					DeleteButton.Enabled = false;
				}
			}
		}

		private void GroupsDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.GroupsDataGrid.EditItemIndex > -1)
			{
				return;
			}
			this.GroupsDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateView();
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AddButton2.Command += this.AddButton_Command;
			this.GroupsDataGrid.EditCommand += this.GroupsDataGrid_EditCommand;
			this.GroupsDataGrid.PageIndexChanged += this.GroupsDataGrid_PageIndexChanged;
			this.GroupsDataGrid.DeleteCommand += this.GroupsDataGrid_DeleteCommand;
			this.GroupsDataGrid.ItemDataBound += this.GroupsDataGrid_ItemDataBound;
			this.AddButton.Command += this.AddButton_Command;
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
			catch(Exception)
            {
				return ssoMode;
            }

			return ssoMode;
		}

		private void UpdateView()
		{
			GroupCollectionClass groupCollection =
				FMChannelHelper.MakeCall<IGroups, GroupCollectionClass>(x => x.Enumerate(this.Security));

			this.GroupsFormPageSizeDropDown.SetPageSize(this.GroupsDataGrid, groupCollection.Count);

            var groupdDataTable = new DataTable();

            groupdDataTable.Columns.Add("SiteGuid", typeof(Guid));
            groupdDataTable.Columns.Add("IdentityGuid", typeof(Guid));
            groupdDataTable.Columns.Add("ID", typeof(string));
            groupdDataTable.Columns.Add("AdUserMapping", typeof(string));
            groupdDataTable.Columns.Add("Description", typeof(string));

            var adUserGroupList = FMChannelHelper.MakeCall<IActiveDirectoryMappings, List<ActiveDirectoryUserGroup>>(
                                                                    x => x.EnumerateAllActiveDirectoryUser(this.Security));

            bool ssoMode = this.IsSsoMode();

            foreach (GroupClass group in groupCollection)
            {
                var groupDataRow = groupdDataTable.NewRow();

                groupDataRow["SiteGuid"]        = group.SiteGuid;
                groupDataRow["IdentityGuid"]    = group.IdentityGuid;
                groupDataRow["ID"]              = group.ID;
                groupDataRow["AdUserMapping"]   = this.GetActiveDirectoryMappingName(ssoMode, adUserGroupList, group.ActiveDirectoryUserGroupGuid);
                groupDataRow["Description"]     = group.Description;

                groupdDataTable.Rows.Add(groupDataRow);
            }

            // If not in SSO mode, then hide the AD User Mapping Name column.
            if (ssoMode == false)
            {
                this.GroupsDataGrid.Columns[4].Visible = false;
            }

            this.GroupsDataGrid.DataSource = new DataView(groupdDataTable); ;
			this.GroupsDataGrid.DataBind();
		}

        /// <summary>
        /// This method will return a matching AD User mapping name.
        /// </summary>
        /// <param name="ssoMode">Flag that indicates if in SSO mode.</param>
        /// <param name="adUserGroupList">The AD user group list.</param>
        /// <param name="adGroupGuid">The AD user group Guid</param>
        /// <returns>Return an empty string if not found, or a AD mapping name.</returns>
        private string GetActiveDirectoryMappingName(bool ssoMode, List<ActiveDirectoryUserGroup> adUserGroupList, Guid adGroupGuid)
        {
            if (ssoMode == false || adUserGroupList.Count == 0)
            {
                return string.Empty;
            }

            var adGroup = adUserGroupList.Find(x => x.ActiveDirectoryUserGroupGuid == adGroupGuid);
            if (adGroup == null)
            {
                return string.Empty;
            }

            return adGroup.Name;
        }
        #endregion
    }
}