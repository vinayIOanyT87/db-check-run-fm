// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the GroupForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Configuration;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;
	using System.Web.Configuration;

	/// <summary>
	///    Summary description for GroupForm.
	/// </summary>
	public partial class GroupForm : FMFormBase
	{
		#region Constants and Fields

		public static readonly string SESSION_KEY_GROUP_GUID = "GroupFormGroupGuid";

		public static readonly string SESSION_KEY_GROUP = "GroupFormGroupObject";

		#endregion

		#region Methods

		public static string GroupFormURL
		{
			get
			{
				string groupFormURL = ConfigurationManager.AppSettings["GroupFormURL"];
				if (string.IsNullOrEmpty(groupFormURL))
				{
					groupFormURL = "FMWebApp/GroupForm.aspx";
				}
				return "../" + groupFormURL;
			}
		}

		public static bool DisableAllControls { get; set; }

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
				DisableAllControls = false;

				if (!this.Page.IsPostBack)
				{
					GroupClass userGroup;

					// Get Guid
					if (this.Session[SESSION_KEY_GROUP_GUID] != null && !((Guid)this.Session[SESSION_KEY_GROUP_GUID]).IsEmpty())
					{
						// Get Group
						userGroup =
							FMChannelHelper.MakeCall<IGroups, GroupClass>(
								x => x.Get(this.Security, (Guid)this.Session[SESSION_KEY_GROUP_GUID]));
					}
					else
					{
                        userGroup = new GroupClass { SessionTimeout = GetSessionTimeout() };
                    }

					this.Session[SESSION_KEY_GROUP] = userGroup;

					if (!this.Security.HasRight(RIGHT.MODIFY_USER_GROUPS)
					    || (userGroup.SiteGuid.IsNotEmptyAndNotEqualTo(this.Security.SiteGuid)))
					{
						this.OK.Enabled = false;
						DisableAllControls = true;
					}

					//Set the title label with a key field from the bound object appended
					if (userGroup != null)
					{
						this.UserGroupTitleLabel.Text = this.GetTitleLabelText(this.UserGroupTitleLabel.Text, userGroup.ID);
					}
				}
				else
				{
					if (this.Session[SESSION_KEY_GROUP] == null)
					{
						throw new Exception("Group not in Session");
					}
				}
				
				// Apply the data dictionary to the tab page header text
				this.tpGeneralPage.HeaderText = this.GetTranslatedText("General");
				this.tpRightsPage.HeaderText = this.GetTranslatedText("Security Rights");
				this.tpCompaniesPage.HeaderText = this.GetTranslatedText("Companies");
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				this.Response.End();
			}
		}

		private void CancelCommand(object sender, CommandEventArgs e)
		{
			this.Session.Remove(SESSION_KEY_GROUP);
			this.Redirect(GroupsForm.GroupsFormURL);
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.OK.Command += this.OkCommand;
			this.Cancel.Command += this.CancelCommand;
		}

		private void OkCommand(object sender, CommandEventArgs e)
		{
			try
			{
				this.GetSecurity();
                this.GroupGeneralPage.UpdateActiveDirectorySelection();

				var userGroup = (GroupClass)this.Session[SESSION_KEY_GROUP];

				if (!userGroup.IdentityGuid.IsEmpty())
				{
					FMChannelHelper.MakeCall<IGroups>(x => x.Modify(this.Security, userGroup));
				}
				else
				{
					FMChannelHelper.MakeCall<IGroups>(x => x.Add(this.Security, userGroup));
				}

				// Update the Session Security if the current user is a member of the group
				if (userGroup.UserGroupMapCollection.Find(x => x.UserGuid == this.Security.UserGuid) != null)
				{
					this.Security.RightCollection = FMChannelHelper.MakeCall<IRights, RightCollectionClass> (x => x.EnumerateByUserBySite(this.Security, this.Security.UserGuid, this.Security.SiteGuid));
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}
			this.Session.Remove(SESSION_KEY_GROUP);

			this.ucFMMenuBar.Refresh();
			this.Redirect(GroupsForm.GroupsFormURL);
		}

		#endregion
	}
}