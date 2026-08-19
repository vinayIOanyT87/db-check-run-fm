// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UsersForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the UsersForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;
   using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Web.UI.WebControls;

   /// <summary>
   ///    Summary description for UsersForm.
   /// </summary>
	public partial class UsersForm : FMAutoSubmitFormBase, IEntityDiscovery, IMenuDiscovery
	{
		#region Constants and Fields

		private const string SessionKeyPageIndex = "UsersPageIndex";

		private const string UserFindString = "UserFindString";

		private string searchString;

		#endregion

		public static string UsersFormUrl
		{
			get
			{
				string usersFormUrl = ConfigurationManager.AppSettings["UsersFormURL"];

				if (string.IsNullOrEmpty(usersFormUrl))
				{
					usersFormUrl = "FMWebApp/UsersForm.aspx";
				}

				return "../" + usersFormUrl;
			}
		}

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
				return typeof(IUsers);
			}
		}

		ENTITY_TYPE IEntityDiscovery.EntityType
		{
			get
			{
				return ENTITY_TYPE.USER;
			}
		}

		#endregion

		#region Explicit Interface Methods

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

			if ((security.HasRight(RIGHT.MODIFY_USERS) == false) && (security.HasRight(RIGHT.VIEW_USERS) == false))
			{
				return null;
			}

			items.Add(
				new FMMenuItem
					{
						MenuItemType = FMMenuItemType.ADMIN_SECURITY_USERS,
						RootMenuName = "Administration",
						CategoryName = "Security",
						ItemName = "Users",
						NavigateUrl = UsersForm.UsersFormUrl,
						ApplyDataDictionary = ApplyDataDictionary.Apply
					});

			return items;
		}

		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(
			SecurityClass Security, ENTITY_ASSIGNMENT_TYPE Type)
		{
			UserCollectionClass UserCollection;
			UserCollection = FMChannelHelper.MakeCall<IUsers, UserCollectionClass>(
																	 x =>
																	 x.Enumerate(Security)
																);

			var EntityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			if (Security.HasRight(RIGHT.MODIFY_USERS))
			{
				foreach (UserClass User in UserCollection)
				{
					if (Type == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
					{
						if (Security.SiteGuid == User.SiteGuid)
						{
							continue;
						}

						if (Security.LoginSiteGuid != User.SiteGuid)
						{
							continue;
						}
					}
					else
					{
						if (Security.SiteGuid != User.SiteGuid)
						{
							continue;
						}
					}

					var EntityToSiteMap = new EntityToSiteMapClass(User);
					EntityToSiteMapCollection.Add(EntityToSiteMap);
				}
			}

			return EntityToSiteMapCollection;
		}

		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string ID)
		{
			return FMChannelHelper.MakeCall<IUsers, Guid>(
																	 x =>
																	 x.GetIdentityGuid(security, ID)
																);
		}

		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid SiteGuid)
		{
			UserClass User = FMChannelHelper.MakeCall<IUsers, UserClass>(
											x =>
											x.Get(security, guid)
									);

			User.SiteGuid = SiteGuid;
			FMChannelHelper.MakeCall<IUsers>(x =>	x.Modify(security,User));
		}

		#endregion

		#region Methods

		protected void FindAllBtnOnClick(object sender, EventArgs e)
		{
			this.Session.Remove(UserFindString);
			this.searchString = null;
			this.FindTextBox.Text = string.Empty;
			this.UsersDataGrid.CurrentPageIndex = 0;
			this.UpdateView();
		}

		protected void FindBtnOnClick(object sender, EventArgs e)
		{
			if ((this.FindTextBox == null) || (this.FindTextBox.Text.Length < 1))
			{
				this.searchString = null;
				this.Session.Remove(UserFindString);
			}
			else
			{
				this.searchString = this.FindTextBox.Text.ToUpper();
				this.FindTextBox.Text = this.searchString;
				this.Session.Add(UserFindString, this.searchString);
			}

			// Update the page with the new contents.
			this.UsersDataGrid.CurrentPageIndex = 0;
			this.UpdateView();
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
					if (!this.Security.HasRight(RIGHT.MODIFY_USERS))
					{
						this.AddButton.Enabled = false;
						this.AddButton2.Enabled = false;
					}

					if (this.Session[SessionKeyPageIndex] != null)
					{
						this.UsersDataGrid.CurrentPageIndex = (int)this.Session[SessionKeyPageIndex];
						this.Session.Remove(SessionKeyPageIndex);
					}

					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void AddButtonCommand(object sender, CommandEventArgs e)
		{
			this.Session.Remove(UserForm.SessionKeyUserGuid);
			this.Session[SessionKeyPageIndex] = this.UsersDataGrid.CurrentPageIndex;
			this.Redirect(UserForm.UserFormUrl);
		}

		private Guid GetGuidFromGridArgument(DataGridItem theDataGrid)
		{
			return new Guid(theDataGrid.Cells[2].Text);//bds
		}

		private Guid GetSiteGuidFromGridArgument(DataGridItem theDataGrid)
		{
			return new Guid(theDataGrid.Cells[1].Text);//bds
        }

        private bool GetActiveDirectoryUserFlagFromGridArgument(DataGridItem theDataGrid)
        {
            string flagStr = theDataGrid.Cells[7].Text;//bds

            if (string.IsNullOrEmpty(flagStr)) return false;

            if (flagStr.ToUpper().Equals("TRUE")) return true;

            return false;
        }

        /// <summary>
        ///    Required method for Designer support - do not modify
        ///    the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
		{
			this.UsersDataGrid.EditCommand      += this.UsersDataGridEditCommand;
			this.UsersDataGrid.PageIndexChanged += this.UsersDataGridPageIndexChanged;
			this.UsersDataGrid.DeleteCommand    += this.UsersDataGridDeleteCommand;
			this.UsersDataGrid.ItemDataBound    += this.UsersDataGridItemDataBound;
			this.AddButton.Command              += this.AddButtonCommand;
			this.AddButton2.Command             += this.AddButtonCommand;
		}

		/// <summary>
		///    This method will update the user grid and reset the find string to the
		///    value in session.
		/// </summary>
		private void UpdateView()
		{
			if (!this.Security.HasRight(RIGHT.VIEW_USERS) && !this.Security.HasRight(RIGHT.MODIFY_USERS))
			{
			    var userCollection = new UserCollectionClass
			                         {
			                             FMChannelHelper.MakeCall<IUsers, UserClass>(x => x.Get(this.Security, this.Security.UserGuid))
			                         };

			    this.UsersDataGrid.DataSource = userCollection;
			}
			else
			{
				// Locate the previous search string from the session. Set the set
				// string if found.
				if (this.Session[UserFindString] != null)
				{
					this.FindTextBox.Text = this.Session[UserFindString] as string;
					this.searchString = this.Session[UserFindString] as string;
				}

				// Determine whether to retrieve the personnel using a filter or not.  If the user entered in 
				// find string, then use the filter method.
				if (string.IsNullOrEmpty(this.searchString))
				{
					this.UsersDataGrid.DataSource = FMChannelHelper.MakeCall<IUsers, UserCollectionClass>(
														x => x.Enumerate(this.Security));
				}
				else
				{
					this.UsersDataGrid.DataSource = FMChannelHelper.MakeCall<IUsers, UserCollectionClass>(
														x => x.EnumerateAndFilter(this.Security, this.searchString));
				}
			}

		    this.UsersDataGrid.DataBind();
		}

		private void UsersDataGridDeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				Guid userGuid = this.GetGuidFromGridArgument(e.Item);
            UserClass user = FMChannelHelper.MakeCall<IUsers, UserClass>(x => x.Get(this.Security, userGuid));
				if (user == null)
				{
               throw new Exception($"Cannot find user {user.ID}.");

            }
            if (user.InactivityLockout == false)
            {
               throw new Exception($"Cannot delete an active user. Please, de-activate/lockout user {user.ID} first.");
            }

            FMChannelHelper.MakeCall<IUsers>(x =>x.Purge(this.Security, userGuid));

				this.UsersDataGrid.SelectedIndex = -1;
				this.Session.Remove(UserForm.SessionKeyUserGuid);
				if (this.UsersDataGrid.Items.Count == 1 && this.UsersDataGrid.CurrentPageIndex > 0)
				{
					this.UsersDataGrid.CurrentPageIndex--;
				}
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void UsersDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
			this.Session[UserForm.SessionKeyUserGuid] = this.GetGuidFromGridArgument(e.Item);
			this.Session[SessionKeyPageIndex] = this.UsersDataGrid.CurrentPageIndex;
			this.Redirect(UserForm.UserFormUrl);
		}

		private void UsersDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			var deleteButton = (LinkButton)e.Item.FindControl("DeleteButton");

		    if (deleteButton == null)
		    {
		        return;
		    }

		    if (!this.Security.HasRight(RIGHT.MODIFY_USERS)
		        || this.Security.SiteGuid != this.GetSiteGuidFromGridArgument(e.Item)
		        || UserClass.IsAdministratorGuid(this.GetGuidFromGridArgument(e.Item))
		        || this.GetActiveDirectoryUserFlagFromGridArgument(e.Item))
		    {
		        deleteButton.Enabled = false;
		        deleteButton.Text = "<img src=Images/Delete_un.gif border=0 align=absmiddle alt='Delete this item'>";
		    }
		}

		private void UsersDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.UsersDataGrid.EditItemIndex > -1)
			{
				return;
			}

			this.UsersDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateView();
		}
		#endregion
	}
}