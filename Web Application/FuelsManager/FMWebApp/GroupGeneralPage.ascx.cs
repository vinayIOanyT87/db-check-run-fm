// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupGeneralPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the GroupGeneralPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	///    Summary description for GroupGeneralPage.
	/// </summary>
	public partial class GroupGeneralPage : FMUserControlBase
	{
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
				var userGroup = (GroupClass)this.Session[GroupForm.SESSION_KEY_GROUP];

				if (!this.Page.IsPostBack)
				{
					if (!this.Security.HasRight(RIGHT.MODIFY_USER_GROUPS))
					{
						this.AssignUsersButton.Enabled = false;
						this.UnassignUsersButton.Enabled = false;
					}

					this.Name.Text = userGroup.ID;
					this.Description.Text = userGroup.Description;
					this.SessionTimeout.Text = userGroup.SessionTimeout.ToString();

					if (userGroup.SessionTimeout == 0)
					{
						this.SessionTimeoutEnabled.Checked = false;
						this.SessionTimeout.Enabled = false;
					}
					else
					{
						this.SessionTimeoutEnabled.Checked = true;
						this.SessionTimeout.Enabled = true;
					}

					// Populate AssignedUsersListBox
					foreach (UserGroupMapClass userGroupMap in userGroup.UserGroupMapCollection)
					{
						var unassignedUserItem = new ListItem(userGroupMap.UserID, userGroupMap.UserGuid.ToString());

						foreach (ListItem assignedUserItem in this.AssignedUsersListBox.Items)
						{
							if (assignedUserItem.Text.CompareTo(unassignedUserItem.Text) > 0)
							{
								int index = this.AssignedUsersListBox.Items.IndexOf(assignedUserItem);
								this.AssignedUsersListBox.Items.Insert(index, unassignedUserItem);
								unassignedUserItem = null;
								break;
							}
						}

						if (unassignedUserItem != null)
						{
							this.AssignedUsersListBox.Items.Add(unassignedUserItem);
						}
					}

                    // Populate the active directory user group dropdown.
                    this.PopulateActiveDirectoryUserGroupNames(userGroup.ActiveDirectoryUserGroupGuid);

					// Populate UnassignedUsersListBox
					UserCollectionClass userCollection = FMChannelHelper.MakeCall<IUsers, UserCollectionClass>(x => x.Enumerate(this.Security));

					foreach (UserClass user in userCollection)
					{
						if (null == this.AssignedUsersListBox.Items.FindByValue(user.IdentityGuid.ToString()))
						{
							var assignedUserItem = new ListItem(user.ID, user.IdentityGuid.ToString());

							foreach (ListItem unassignedUserItem in this.UnassignedUsersListBox.Items)
							{
								if (unassignedUserItem.Text.CompareTo(assignedUserItem.Text) > 0)
								{
									int index = this.UnassignedUsersListBox.Items.IndexOf(unassignedUserItem);
									this.UnassignedUsersListBox.Items.Insert(index, assignedUserItem);
									assignedUserItem = null;
									break;
								}
							}

							if (assignedUserItem != null)
							{
								this.UnassignedUsersListBox.Items.Add(assignedUserItem);
							}
						}
					}
				}
				else
				{
					userGroup.ID = this.Name.Text;
					userGroup.Description = this.Description.Text;
					userGroup.SessionTimeout = Int32.Parse(this.SessionTimeout.Text);
				}

				// Disable a the controls and buttons on the page
				this.DisablePageControls();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// This method will disable all the fields on the General page based on the 
		/// GroupForm.DisableAllControls flag.
		/// </summary>
		private void DisablePageControls()
        {
			if(GroupForm.DisableAllControls)
            {
				this.Name.Enabled = false;
				this.Description.Enabled = false;
				this.SessionTimeout.Enabled = false;
				this.AdGrpDropdownList.Enabled = false;
				this.AssignUsersButton.Enabled = false;
				this.UnassignUsersButton.Enabled = false;
				this.SessionTimeoutEnabled.Enabled = false;
            }
        }

		private void AssignUsersButtonCommand(object sender, CommandEventArgs e)
		{
			ListItem unassignedUserItem;
			while ((unassignedUserItem = this.UnassignedUsersListBox.SelectedItem) != null)
			{
                bool activerDirectoryUser = false;
                Guid userGuid;
                var selectedItem = this.UnassignedUsersListBox.SelectedItem;

                if (Guid.TryParse(selectedItem.Value, out userGuid))
                {
                    activerDirectoryUser = this.IsUserAnActiverDirectoryUser(userGuid);
                }

                if (activerDirectoryUser)
                {
                    this.ErrorHandler(new Exception("Cannot assign an active directory user."));
                    return;
                }

                this.UnassignedUsersListBox.Items.Remove(unassignedUserItem);
				unassignedUserItem.Selected = false;

				foreach (ListItem assignedUserItem in this.AssignedUsersListBox.Items)
				{
					if (assignedUserItem.Text.CompareTo(unassignedUserItem.Text) > 0)
					{
						int index = this.AssignedUsersListBox.Items.IndexOf(assignedUserItem);
						this.AssignedUsersListBox.Items.Insert(index, unassignedUserItem);
						unassignedUserItem = null;
						break;
					}
				}

				if (unassignedUserItem != null)
				{
					this.AssignedUsersListBox.Items.Add(unassignedUserItem);
				}
			}

			this.UpdateUserCollection();
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AssignUsersButton.Command += this.AssignUsersButtonCommand;
			this.UnassignUsersButton.Command += this.UnassignUsersButtonCommand;
			this.SessionTimeoutEnabled.CheckedChanged += this.SessionTimeoutEnabled_CheckedChanged;
		}


		private void SessionTimeoutEnabled_CheckedChanged(object sender, EventArgs e)
		{
			if (this.SessionTimeoutEnabled.Checked)
			{
				this.SessionTimeout.Enabled = true;
				if(this.SessionTimeout.Text == "0")
				{
					this.SessionTimeout.Text = "20";
				}
			}
			else
			{
				this.SessionTimeout.Enabled = false;
				// set at 0. a value of 0 indicates that the session timeout is not enabled. Requires no database change plus we have far to many extra columns in our database.
				this.SessionTimeout.Text = "0";
			}
		}


		private void UnassignUsersButtonCommand(object sender, CommandEventArgs e)
		{
			ListItem assignedUserItem;
			while ((assignedUserItem = this.AssignedUsersListBox.SelectedItem) != null)
			{
			    bool activerDirectoryUser = false;
			    Guid userGuid;
			    var selectedItem = this.AssignedUsersListBox.SelectedItem;

			    if (Guid.TryParse(selectedItem.Value, out userGuid))
			    {
			        activerDirectoryUser = this.IsUserAnActiverDirectoryUser(userGuid);
			    }

			    if (activerDirectoryUser)
			    {
                    this.ErrorHandler(new Exception("Cannot un-assign an active directory user."));
                    return;
                }
                
				this.AssignedUsersListBox.Items.Remove(assignedUserItem);
				assignedUserItem.Selected = false;

				foreach (ListItem unassignedUserItem in this.UnassignedUsersListBox.Items)
				{
					if (unassignedUserItem.Text.CompareTo(assignedUserItem.Text) > 0)
					{
						int index = this.UnassignedUsersListBox.Items.IndexOf(unassignedUserItem);
						this.UnassignedUsersListBox.Items.Insert(index, assignedUserItem);
						assignedUserItem = null;
						break;
					}
				}

				if (assignedUserItem != null)
				{
					this.UnassignedUsersListBox.Items.Add(assignedUserItem);
				}
			}

			this.UpdateUserCollection();
		}

		private void UpdateUserCollection()
		{
			var group = (GroupClass)this.Session[GroupForm.SESSION_KEY_GROUP];

			var userGroupMapCollection = new UserGroupMapCollectionClass();
			foreach (ListItem assignedUserItem in this.AssignedUsersListBox.Items)
			{
			    var userGroupMap = new UserGroupMapClass
			                       {
			                           UserGuid = new Guid(assignedUserItem.Value),
			                           SiteGuid = this.Security.SiteGuid
			                       };
			    userGroupMapCollection.Add(userGroupMap);
			}

			group.UserGroupMapCollection = userGroupMapCollection;

			var groupRightslPage =
				(GroupRightsPage) ((GroupForm)this.Page).FindControl("tcGroupTabs").FindControl("tpRightsPage").FindControl("GroupRightsPage");

			var unassignedRightsListBox = (ListBox)groupRightslPage.FindControl("UnassignedRightsListBox");

		    if (this.UnassignedUsersListBox.Items.FindByValue(Guids.UserAdminGuid.ToString()) != null)
			{
				if (unassignedRightsListBox.Items.FindByValue(((int)RIGHT.VIEW_OPERATE_ONLY).ToString()) == null)
				{
					var unassignedRightItem = new ListItem(SecurityClass.RightID(RIGHT.VIEW_OPERATE_ONLY), ((int)RIGHT.VIEW_OPERATE_ONLY).ToString());
					unassignedRightsListBox.Items.Add(unassignedRightItem);
				}
			}
			else if (this.AssignedUsersListBox.Items.FindByValue(Guids.UserAdminGuid.ToString()) != null)
			{
				var unasssignedViewOperateOnlyRight = unassignedRightsListBox.Items.FindByValue(((int)RIGHT.VIEW_OPERATE_ONLY).ToString());
				if (unasssignedViewOperateOnlyRight != null)
				{
					unassignedRightsListBox.Items.Remove(unasssignedViewOperateOnlyRight);
				}
			}
		}

		/// <summary>
		/// This method gets the SSO mode.
		/// </summary>
		/// <returns>Returns true if in SSO mode, otherwise false.</returns>
		private bool IsSsoMode()
        {
			bool isSsoMode = false;

			try
			{
				var configSetting = FMChannelHelper.MakeCall<IConfigurationSettings, ConfigurationSettingDOClass>
													(x => x.GetByKey(this.Security, ConfigurationSettingDOClass.Key_SingleSignOnMode));

				// This is so that the Login page will not try and auto login the domain user.
				if (configSetting != null && string.IsNullOrEmpty(configSetting.SettingValue) == false && configSetting.SettingValue == "1")
				{
					isSsoMode = true;
				}
			}
			catch(Exception)
            {
				return isSsoMode;
            }

			return isSsoMode;
		}

        /// <summary>
        /// This method will populate the Active Directory dropdown list with the 
        /// List of active directory site groups.
        /// </summary>
        private void PopulateActiveDirectoryUserGroupNames(Guid activeDirectoryUserGroupGuid)
        {
            int selectionIndex = 0;
            var adUserGroupList = FMChannelHelper.MakeCall<IActiveDirectoryMappings, List<ActiveDirectoryUserGroup>>(
                                                                x => x.EnumerateActiveDirectoryUserList(this.Security, activeDirectoryUserGroupGuid));

			if (this.IsSsoMode() == false)
            {
                this.AdGrpDropdownList.Enabled = false;
            }

            var adGrpUserListItem = new ListItem { Text = "None", Value = Guid.Empty.ToString() };
            this.AdGrpDropdownList.Items.Add(adGrpUserListItem);
            this.AdGrpDropdownList.SelectedIndex = selectionIndex;

            if (adUserGroupList == null || adUserGroupList.Count == 0)
            {
                return;
            }

            foreach (var adUserGroup in adUserGroupList)
            {
                selectionIndex++;
                adGrpUserListItem = new ListItem { Text = adUserGroup.Name, Value = adUserGroup.ActiveDirectoryUserGroupGuid.ToString() };
                this.AdGrpDropdownList.Items.Add(adGrpUserListItem);

                if (adUserGroup.ActiveDirectoryUserGroupGuid == activeDirectoryUserGroupGuid)
                {
                    this.AdGrpDropdownList.SelectedIndex = selectionIndex;
                }
            }
        }

        /// <summary>
        /// This method will return whether the user is an active directory user or not.
        /// </summary>
        /// <param name="userGuid">The User's guid to retrieve the user.</param>
        /// <returns>Return True is active directory user, otherwise false.</returns>
	    private bool IsUserAnActiverDirectoryUser(Guid userGuid)
	    {
	        if (userGuid == Guid.Empty) return false;

	        try
	        {
	            var userObj = FMChannelHelper.MakeCall<IUsers, UserClass>(x => x.Get(this.Security, userGuid));
	            if (userObj == null) return false;

	            return userObj.ActiveDirectoryUser;
	        }
	        catch (Exception)
	        {
	            return false;
	        }
	    }

	    public void UpdateActiveDirectorySelection()
        {
            var group = (GroupClass)this.Session[GroupForm.SESSION_KEY_GROUP];

            Guid selectedItemGuid;
            ListItem selectedAdUserGrp = this.AdGrpDropdownList.SelectedItem;
            group.ActiveDirectoryUserGroupGuid = Guid.Empty;

            if (Guid.TryParse(selectedAdUserGrp.Value, out selectedItemGuid))
            {
                group.ActiveDirectoryUserGroupGuid = selectedItemGuid;
            }
        }
        #endregion
    }
}