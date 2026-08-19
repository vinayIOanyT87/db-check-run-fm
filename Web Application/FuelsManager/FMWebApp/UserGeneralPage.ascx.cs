using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FuelsManager.FMWebApp
{
	using System.Collections;
	using System.Globalization;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.Interfaces;
	using FMBusinessObjects.UtilityObjects;

	using FMCore;

	public partial class UserGeneralPage : FMUserControlBase, IDataDictionary
	{
		string[] IDataDictionary.Keys(SecurityClass inSecurity)
		{
			string[] keys = {	"Re-enter Password",
								"Password must be periodically changed",
								"Password Aging",
								"days",
								"Assigned Groups",
								"Unassigned Groups",
								"User Configuration",
								"Password vs. Re-enter Password does not match",
								"Locked Out"
							};
			return keys;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			PasswordPopupBubble();

			if (!this.Page.IsPostBack)
			{

				UserClass user = this.User;

				this.Name.Text = user.ID;
				this.FullNameTextBox.Text = user.Name;
				this.EmailAddressTextBox.Text = user.EmailAddress;
				this.PhoneNumberTextbox.Text = user.PhoneNumber;
				this.AccountExpirationDate.CurrentValue = user.AccountExpirationDate;
				this.PasswordTextBox.Attributes.Add("value", "**********");
				this.ReenterPasswordTextBox.Attributes.Add("value", "**********");

				// Not allowed to lock out the main system user admin
				if (user.IdentityGuid == Guids.UserAdminGuid)
				{
					this.LockedOutCheckBox.Enabled = false;
				}

				// Populate AssignedGroupsListBox
				UserGroupMapCollectionClass userGroupMapCollection = user.UserGroupMapCollection;
				List<UserGroupMapClass> userGroupMapSorted = userGroupMapCollection.OrderBy(x => x.GroupID).ToList();

				this.AssignedGroupsListBox.Items.Clear();

				foreach (UserGroupMapClass userGroupMap in userGroupMapSorted)
				{
					var userGroupItem = new ListItem(userGroupMap.GroupID, userGroupMap.GroupGuid.ToString());
					this.AssignedGroupsListBox.Items.Add(userGroupItem);
				}

				// Determine if the user is an Administrator at the Current Site
				bool administrativeUser = false;
				if (user.IdentityGuid != this.Security.UserGuid)
				{
					var currentUserGroups = FMChannelHelper.MakeCall<IUserGroupMaps, UserGroupMapCollectionClass>(
							 x =>
							 x.EnumerateByUserAndSite(this.Security, this.Security.UserGuid, this.Security.SiteGuid));

					if (currentUserGroups.Find(x => x.GroupGuid == Guids.GroupAdminGuid) != null)
					{
						administrativeUser = true;
					}
				}
				else
				{

					if (user.UserGroupMapCollection.Find(x => x.GroupGuid == Guids.GroupAdminGuid) != null)
					{
						administrativeUser = true;
					}
				}

				this.ChangePasswordCheckBox.Checked = user.ChangePassword;

				Guid siteGuid = this.Security.SiteGuid;
				SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																 sites => sites.GetBasic(this.Security, siteGuid));

				this.LockedOutCheckBox.Checked = user.InactivityLockout;
				if (site.EnablePasswordHint)
				{
					this.PasswordHintTextBox.Text = user.PasswordHint;
					this.PasswordHintTextBox.Visible = true;
					this.PasswordHintLabel.Visible = true;
				}
				else
				{
					this.PasswordHintTextBox.Visible = false;
					this.PasswordHintLabel.Visible = false;
				}

				// Populate UnassignedGroupsListBox
				if (this.Security.HasRight(RIGHT.VIEW_USER_GROUPS)
				|| this.Security.HasRight(RIGHT.VIEW_USERS)
				|| this.Security.HasRight(RIGHT.MODIFY_USER_GROUPS)
				|| this.Security.HasRight(RIGHT.MODIFY_USERS))
				{
					var groupCollection = FMChannelHelper.MakeCall<IGroups, GroupCollectionClass>(
																 x =>
																 x.Enumerate(this.Security)
															);

					List<GroupClass> groupCollectionSorted = groupCollection.OrderBy(x => x.ID).ToList();
					this.UnassignedGroupsListBox.Items.Clear();

					foreach (GroupClass group in groupCollectionSorted)
					{
						if (group.IdentityGuid == Guids.GroupAdminGuid && !administrativeUser)
						{
							continue;
						}

						if (this.AssignedGroupsListBox.Items.FindByValue(group.IdentityGuid.ToString()) == null)
						{
							var assignedGroupItem = new ListItem(group.ID, group.IdentityGuid.ToString());
							this.UnassignedGroupsListBox.Items.Add(assignedGroupItem);
						}
					}

					if (this.HasEntityAssignmentRights(site.SiteGroup))
					{
						this.EntityAssignment.Enabled = true;
					}

					if (this.HasGroupAssignmentRights(site.SiteGroup))
					{
						this.SiteAssignment.Enabled = true;
					}
				}

				if (!this.Security.HasRight(RIGHT.MODIFY_USERS)
				|| user.SiteGuid.IsNotEmptyAndNotEqualTo(this.Security.SiteGuid))
				{
					this.Name.Enabled = false;
					this.ChangePasswordCheckBox.Enabled = false;
					this.FullNameTextBox.Enabled = false;
					this.EmailAddressTextBox.Enabled = false;
					this.AssignGroupsButton.Enabled = false;
					this.UnassignGroupsButton.Enabled = false;

					this.PasswordTextBox.Enabled = false;
					this.ReenterPasswordTextBox.Enabled = false;
				}
			}
			else
			{
				this.PasswordTextBox.Attributes.Add("value", this.PasswordTextBox.Text);
				this.ReenterPasswordTextBox.Attributes.Add("value", this.ReenterPasswordTextBox.Text);
			}
		}

		public void ClearPassword()
		{
			this.PasswordTextBox.Text = "";
			this.ReenterPasswordTextBox.Text = "";
		}

		public bool PasswordNotChanged
		{
			get
			{
				string oldPassword = this.Session[UserForm.SessionKeyOldPassword] as string;
				return this.PasswordTextBox.Text == "**********" || oldPassword == this.PasswordTextBox.Text;
			}
		}

		public bool UserGroupChanged
		{
			get;
			private set;
		}

		public void UpdateData()
		{

			if (this.Session[UserForm.SessionKeyUserGuid] == null)
			{

				if (string.IsNullOrEmpty(this.PasswordTextBox.Text))
				{
					throw new Exception("Must enter in a Password");
				}
			}

			if (this.PasswordTextBox.Text != this.ReenterPasswordTextBox.Text)
			{
				throw new Exception("Password vs. Re-enter Password does not match.");
			}

			if (this.EmailAddressTextBox.Text.IsValidEmailAddressSyntax() == false)
			{
				throw new FMEmailFormatException();
			}

			UserClass user = this.User;

			user.ID = this.Name.Text;
			if (this.PasswordTextBox.Text != "**********")
			{
				user.Password = this.PasswordTextBox.Text;
			}

			user.ChangePassword = this.ChangePasswordCheckBox.Checked;
			user.Name = this.FullNameTextBox.Text;
			user.EmailAddress = this.EmailAddressTextBox.Text;
			user.PhoneNumber = this.PhoneNumberTextbox.Text;

			try
			{
				// Validate that this is a validate date.
				string tempDate = AccountExpirationDate.Text;
				AccountExpirationDate.Text = tempDate;
			}
			catch (Exception)
			{
				this.AccountExpirationDate.CurrentValue = this.AccountExpirationDate.CurrentValue.Date;
				const string Msg = "Account Expiration Date Format is invalid.";
				throw new Exception(Msg);
			}
			if( !string.IsNullOrEmpty (AccountExpirationDate.Text) ) { 
				user.AccountExpirationDate = this.AccountExpirationDate.CurrentValue.Date;
			}

			// Create an Assigned GroupCollection
			var userGroupMapCollection = new UserGroupMapCollectionClass();

			foreach (ListItem assignedGroupItem in this.AssignedGroupsListBox.Items)
			{
				var userGroupMap = new UserGroupMapClass
				{
					GroupGuid = new Guid(assignedGroupItem.Value),
					SiteGuid = this.Security.SiteGuid
				};
				userGroupMapCollection.Add(userGroupMap);
			}

			// Determine if userGroup assignements changed for current user
			bool userGroupChange = false;
			if (user.IdentityGuid == this.Security.UserGuid)
			{
				foreach (UserGroupMapClass userGroupMap in user.UserGroupMapCollection)
				{
					if (userGroupMapCollection.Find(x => x.GroupGuid == userGroupMap.GroupGuid) == null)
					{
						userGroupChange = true;
						break;
					}
				}

				if (!userGroupChange)
				{
					foreach (UserGroupMapClass userGroupMap in userGroupMapCollection)
					{
						if (user.UserGroupMapCollection.Find(x => x.GroupGuid == userGroupMap.GroupGuid) == null)
						{
							userGroupChange = true;
							break;
						}
					}
				}
			}

			this.UserGroupChanged = userGroupChange;

			user.UserGroupMapCollection = userGroupMapCollection;

			Guid siteGuid = this.Security.SiteGuid;
			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
						x =>
						x.Get(this.Security, siteGuid, getMemberSites: false, getSchedulesAndProcessVariables: false,
						bGetAssociatedAliases: false)
				);

			if (site.EnablePasswordHint)
			{
				if (this.PasswordHintTextBox.Text == user.Password)
				{
					throw new Exception("Password hint cannot be the same as the password.");
				}
				user.PasswordHint = this.PasswordHintTextBox.Text;
			}

			if (string.IsNullOrEmpty(user.PasswordHint))
			{
				user.PasswordHint = "No hint available";
			}

			if (!this.LockedOutCheckBox.Checked)
			{
				user.PasswordLockoutCount = 0;
				user.LastLoginDate = DateTimeOffset.Now;
				user.LastLogoffDate = DateTimeOffset.Now;
				user.InactivityLockout = false;
			}
			else
			{
				user.InactivityLockout = true;
			}

		}


		private void AssignGroupsButtonCommand(object sender, CommandEventArgs e)
		{
			ListItem unassignedGroupItem;
			while ((unassignedGroupItem = this.UnassignedGroupsListBox.SelectedItem) != null)
			{
				this.UnassignedGroupsListBox.Items.Remove(unassignedGroupItem);
				unassignedGroupItem.Selected = false;

				foreach (ListItem assignedGroupItem in this.AssignedGroupsListBox.Items)
				{
					if (assignedGroupItem.Text.CompareTo(unassignedGroupItem.Text) > 0)
					{
						int index = this.AssignedGroupsListBox.Items.IndexOf(assignedGroupItem);
						this.AssignedGroupsListBox.Items.Insert(index, unassignedGroupItem);
						unassignedGroupItem = null;
						break;
					}
				}

				if (unassignedGroupItem != null)
				{
					this.AssignedGroupsListBox.Items.Add(unassignedGroupItem);
				}
			}
		}

		private void UnassignGroupsButtonCommand(object sender, CommandEventArgs e)
		{
			ListItem assignedGroupItem;
			while ((assignedGroupItem = this.AssignedGroupsListBox.SelectedItem) != null)
			{
				this.AssignedGroupsListBox.Items.Remove(assignedGroupItem);
				assignedGroupItem.Selected = false;

				foreach (ListItem unassignedGroupItem in this.UnassignedGroupsListBox.Items)
				{
					if (unassignedGroupItem.Text.CompareTo(assignedGroupItem.Text) > 0)
					{
						int index = this.UnassignedGroupsListBox.Items.IndexOf(unassignedGroupItem);
						this.UnassignedGroupsListBox.Items.Insert(index, assignedGroupItem);
						assignedGroupItem = null;
						break;
					}
				}

				if (assignedGroupItem != null)
				{
					this.UnassignedGroupsListBox.Items.Add(assignedGroupItem);
				}
			}
		}


		private void EntityAssignmentCommand(object sender, CommandEventArgs e)
		{
			try
			{
				SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
							x =>
							x.Get(this.Security, this.Security.SiteGuid, getMemberSites: false, getSchedulesAndProcessVariables: false,
							bGetAssociatedAliases: false)
					);

				if (!this.HasEntityAssignmentRights(site.SiteGroup))
				{
					throw new Exception("Unable to perfom Entity Assignment due to lack of rights or because this is not a site group");
				}

				UserForm parentPage = this.Page as UserForm;

				if (parentPage != null && parentPage.OkButton.Enabled && !parentPage.SaveUser())
				{
					return;
				}

				IEntityDiscovery usersForm = new UsersForm();
				string entityType = ((int)usersForm.EntityType).ToString(CultureInfo.InvariantCulture);

				if (this.Page.Session[PageSessionKeyConstants.EAF_SESSION_ENTITY_TYPE_SELECT] == null)
				{
					this.Page.Session.Add(PageSessionKeyConstants.EAF_SESSION_ENTITY_TYPE_SELECT, entityType);
				}
				else
				{
					this.Page.Session[PageSessionKeyConstants.EAF_SESSION_ENTITY_TYPE_SELECT] = entityType;
				}

				if (this.Page.Session[PageSessionKeyConstants.EAF_SESSION_ENTITY_SELECT] == null)
				{
					this.Page.Session.Add(PageSessionKeyConstants.EAF_SESSION_ENTITY_SELECT, Convert.ToString(this.Session[UserForm.SessionKeyUserGuid]));
				}
				else
				{
					this.Page.Session[PageSessionKeyConstants.EAF_SESSION_ENTITY_SELECT] = Convert.ToString(this.Session[UserForm.SessionKeyUserGuid]);
				}

				if (this.Page.Session[PageSessionKeyConstants.EAF_SESSION_SITE_SELECT] == null)
				{
					this.Page.Session.Add(PageSessionKeyConstants.EAF_SESSION_SITE_SELECT, site.SiteID);
				}
				else
				{
					this.Page.Session[PageSessionKeyConstants.EAF_SESSION_SITE_SELECT] = site.SiteID;
				}

				if (this.Page.Session[PageSessionKeyConstants.EAF_SESSION_INCLUDE_MEMBERS] == null)
				{
					this.Page.Session.Add(PageSessionKeyConstants.EAF_SESSION_INCLUDE_MEMBERS, true);
				}
				else
				{
					this.Page.Session[PageSessionKeyConstants.EAF_SESSION_INCLUDE_MEMBERS] = true;
				}

				var entityEngineHshTbl = new Hashtable { { usersForm.EntityType, usersForm.EntityEngineType.GUID } };
				if (this.Page.Session[PageSessionKeyConstants.EAF_SESSION_ENTITY_ENTITY_ENGINE] == null)
				{
					this.Page.Session.Add(PageSessionKeyConstants.EAF_SESSION_ENTITY_ENTITY_ENGINE, entityEngineHshTbl);
				}
				else
				{
					this.Page.Session[PageSessionKeyConstants.EAF_SESSION_ENTITY_ENTITY_ENGINE] = entityEngineHshTbl;
				}

				this.Redirect("EntityToSiteAssignmentForm.aspx?Mode=User");
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}


		protected void SiteAssignmentCommand(object sender, EventArgs e)
		{
			try
			{
				SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(sites => sites.Get(this.Security, this.Security.SiteGuid, false, false, false));

				if (!this.HasGroupAssignmentRights(site.SiteGroup))
				{
					throw new Exception("Unable to perfom Group Assignment due to lack of rights or because this is not a site group");
				}

				UserForm parentPage = this.Page as UserForm;

				if ( parentPage != null && parentPage.OkButton.Enabled && !parentPage.SaveUser())
				{
					return;
				}

				if (this.Page.Session[PageSessionKeyConstants.UGAF_SESSION_USER_SELECT] == null)
				{
					this.Page.Session.Add(PageSessionKeyConstants.UGAF_SESSION_USER_SELECT, this.Name.Text);
				}
				else
				{
					this.Page.Session[PageSessionKeyConstants.UGAF_SESSION_USER_SELECT] = this.Name.Text;
				}

				if (this.Page.Session[PageSessionKeyConstants.UGAF_SESSION_SITE_SELECT] == null)
				{
					this.Page.Session.Add(PageSessionKeyConstants.UGAF_SESSION_SITE_SELECT, this.Security.SiteID);
				}
				else
				{
					this.Page.Session[PageSessionKeyConstants.UGAF_SESSION_SITE_SELECT] = this.Security.SiteID;
				}

				if (this.Page.Session[PageSessionKeyConstants.UGAF_SESSION_SITEGROUP_SELECT] == null)
				{
					this.Page.Session.Add(PageSessionKeyConstants.UGAF_SESSION_SITEGROUP_SELECT, true);
				}
				else
				{
					this.Page.Session[PageSessionKeyConstants.UGAF_SESSION_SITEGROUP_SELECT] = true;
				}

				this.Redirect("UserPermissionAssignmentForm.aspx?Mode=User");
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		protected UserClass User
		{
			get
			{
				return ((UserForm)this.Page).FMUser;
			}
		}

		/// <summary>
		/// Return true if this is a site group and the user has entity assignment rights
		/// </summary>
		/// <param name="isSiteGroup">true if this is a site group</param>
		/// <returns>True if this is a site group and the user has entity assignment rights</returns>
		private bool HasEntityAssignmentRights(bool isSiteGroup)
		{
			return isSiteGroup && (this.Security.HasRight(RIGHT.VIEW_ENTITY_ASSIGNMENTS) || this.Security.HasRight(RIGHT.MODIFY_ENTITY_ASSIGNMENTS));
		}

		/// <summary>
		/// Return true if this is a site group and the user has group assignment rights (rights to see the user permission form)
		/// </summary>
		/// <param name="isSiteGroup">true if this is a site group</param>
		/// <returns>True if this is a site group and the user has group assignment rights</returns>
		private bool HasGroupAssignmentRights(bool isSiteGroup)
		{
			return isSiteGroup && (this.Security.HasRight(RIGHT.VIEW_USERS) || this.Security.HasRight(RIGHT.MODIFY_USERS));
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.UnassignGroupsButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.UnassignGroupsButtonCommand);
			this.AssignGroupsButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AssignGroupsButtonCommand);
			this.EntityAssignment.Command += new System.Web.UI.WebControls.CommandEventHandler(this.EntityAssignmentCommand);
			this.SiteAssignment.Command += new System.Web.UI.WebControls.CommandEventHandler(this.SiteAssignmentCommand);
		}

		#endregion

		/// <summary>
		/// Displays the password policy in a popup bubble.
		/// </summary>
		protected void PasswordPopupBubble()
		{
			Guid siteGuid = this.Security.SiteGuid;
			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(sites => sites.Get(this.Security, siteGuid, false, false, false));

			string passwordPopupMessageEnhanced = "Must be at least " + site.MinPasswordCharacterLength + " characters long and contain at least two lower case letters, two upper case letters, two digits, and two special characters (@#$%^&+.,!=).";
			string passwordPopupMessageStrong = "Must be at least " + site.MinPasswordCharacterLength + " characters long and contain at least one lower case letter, one upper case letter, one digit, and one special character (@#$%^&+.,!=).";
			string passwordPopupMessageRegular = "Must be at least " + site.MinPasswordCharacterLength + " characters long.";

			string temp = "";

			switch ((StrongPasswordUsage)site.StrongPasswordUse)
			{
				case StrongPasswordUsage.Enhanced:
					temp = passwordPopupMessageEnhanced;
					break;
				case StrongPasswordUsage.Strong:
					temp = passwordPopupMessageStrong;
					break;
				case StrongPasswordUsage.None:
					temp = passwordPopupMessageRegular;
					break;
			}
			this.PasswordPopupBubbleLabel.Attributes["title"] = temp;
		}
	}
}