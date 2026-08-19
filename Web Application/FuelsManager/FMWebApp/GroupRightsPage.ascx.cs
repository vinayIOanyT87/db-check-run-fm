// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupRightsPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the GroupRightsPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	///    Summary description for GroupRightsPage.
	/// </summary>
	public partial class GroupRightsPage : FMUserControlBase
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
				var Group = (GroupClass)this.Session[GroupForm.SESSION_KEY_GROUP];

				if (! this.Page.IsPostBack)
				{
					if (!this.Security.HasRight(RIGHT.MODIFY_USER_GROUPS))
					{
						this.AssignRightsButton.Enabled = false;
						this.UnassignRightsButton.Enabled = false;
					}
					// Populate AssignedRightsListBox
					foreach (RIGHT Right in Group.RightCollection)
					{
						var UnassignedRightItem = new ListItem(SecurityClass.RightID(Right), ((int)Right).ToString());

						if (UnassignedRightItem.Text == SecurityClass.UndefinedRightText)
						{
							continue;
						}

						this.AssignedRightsListBox.Items.Add(UnassignedRightItem);
					}

					// Populate UnassignedRightsListBox
					RightCollectionClass RightCollection =
						FMChannelHelper.MakeCall<IRights, RightCollectionClass>(x => x.Enumerate(this.Security));

					var GroupGeneralPage =
						(GroupGeneralPage)
						((GroupForm)this.Page).FindControl("tcGroupTabs")
						.FindControl("tpGeneralPage")
						.FindControl("GroupGeneralPage");

					var AssignedUserListBox = (ListBox)GroupGeneralPage.FindControl("AssignedUsersListBox");

					foreach (RIGHT Right in RightCollection)
					{
						if ((Group.IsAdminGroup
						|| AssignedUserListBox.Items.FindByValue(Guids.UserAdminGuid.ToString()) != null)
						&& Right == RIGHT.VIEW_OPERATE_ONLY)
						{
							continue;
						}

						if (null == this.AssignedRightsListBox.Items.FindByValue(((int)Right).ToString()))
						{
							var AssignedRightItem = new ListItem(SecurityClass.RightID(Right), ((int)Right).ToString());

							this.UnassignedRightsListBox.Items.Add(AssignedRightItem);
						}
					}
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
			if (GroupForm.DisableAllControls)
			{
				this.AssignRightsButton.Enabled = false;
				this.UnassignRightsButton.Enabled = false;
			}
		}

		private void AssignRightsButton_Command(object sender, CommandEventArgs e)
		{
			ListItem UnassignedRightItem;
			while ((UnassignedRightItem = this.UnassignedRightsListBox.SelectedItem) != null)
			{
				this.UnassignedRightsListBox.Items.Remove(UnassignedRightItem);
				UnassignedRightItem.Selected = false;

				foreach (ListItem AssignedRightItem in this.AssignedRightsListBox.Items)
				{
					if (AssignedRightItem.Text.CompareTo(UnassignedRightItem.Text) > 0)
					{
						int Index = this.AssignedRightsListBox.Items.IndexOf(AssignedRightItem);
						this.AssignedRightsListBox.Items.Insert(Index, UnassignedRightItem);
						UnassignedRightItem = null;
						break;
					}
				}

				if (UnassignedRightItem != null)
				{
					this.AssignedRightsListBox.Items.Add(UnassignedRightItem);
				}
			}
			this.UpdateGroupRights();
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.UnassignRightsButton.Command += this.UnassignRightsButton_Command;
			this.AssignRightsButton.Command += this.AssignRightsButton_Command;
		}

		private void UnassignRightsButton_Command(object sender, CommandEventArgs e)
		{
			ListItem AssignedRightItem;
			while ((AssignedRightItem = this.AssignedRightsListBox.SelectedItem) != null)
			{
				this.AssignedRightsListBox.Items.Remove(AssignedRightItem);
				AssignedRightItem.Selected = false;

				foreach (ListItem UnassignedRightItem in this.UnassignedRightsListBox.Items)
				{
					if (UnassignedRightItem.Text.CompareTo(AssignedRightItem.Text) > 0)
					{
						int Index = this.UnassignedRightsListBox.Items.IndexOf(UnassignedRightItem);
						this.UnassignedRightsListBox.Items.Insert(Index, AssignedRightItem);
						AssignedRightItem = null;
						break;
					}
				}

				if (AssignedRightItem != null)
				{
					this.UnassignedRightsListBox.Items.Add(AssignedRightItem);
				}
			}
			this.UpdateGroupRights();
		}

		private void UpdateGroupRights()
		{
			var Group = (GroupClass)this.Session[GroupForm.SESSION_KEY_GROUP];

			var RightCollection = new RightCollectionClass();
			foreach (ListItem AssignedRightItem in this.AssignedRightsListBox.Items)
			{
				RIGHT Right;
				Right = (RIGHT)Convert.ToInt32(AssignedRightItem.Value);
				RightCollection.Add(Right);
			}
			Group.RightCollection = RightCollection;

			var GroupGeneralPage =
							(GroupGeneralPage)
							((GroupForm)this.Page).FindControl("tcGroupTabs")
							.FindControl("tpGeneralPage")
							.FindControl("GroupGeneralPage");

			var UnassignedUsersListBox = (ListBox)GroupGeneralPage.FindControl("UnassignedUsersListBox");
			var AssignedUserListBox = (ListBox)GroupGeneralPage.FindControl("AssignedUsersListBox");

			if (Group.RightCollection.Contains(RIGHT.VIEW_OPERATE_ONLY))
			{
				var adminListItem = UnassignedUsersListBox.Items.FindByValue(Guids.UserAdminGuid.ToString());
				if (adminListItem != null)
				{
					UnassignedUsersListBox.Items.Remove(adminListItem);
				}
			}
			else
			{
				var adminListItem = UnassignedUsersListBox.Items.FindByValue(Guids.UserAdminGuid.ToString());
				if (adminListItem == null
				&& AssignedUserListBox.Items.FindByValue(Guids.UserAdminGuid.ToString()) == null)
				{
					var administratorUser = FMChannelHelper.MakeCall<IUsers, UserClass>(x => x.Get(this.Security, Guids.UserAdminGuid));

					if (administratorUser != null
					&& administratorUser.IdentityGuid == Guids.UserAdminGuid)
					{
						var UnassignedUserItem = new ListItem(administratorUser.ID, administratorUser.IdentityGuid.ToString());
						UnassignedUsersListBox.Items.Add(UnassignedUserItem);
					}
				}
			}
		}

		#endregion
	}
}