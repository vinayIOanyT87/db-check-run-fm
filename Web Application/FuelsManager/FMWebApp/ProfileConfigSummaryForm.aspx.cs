// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProfileConfigSummaryForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ProfileConfigSummaryForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Globalization;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;

	using FMControls;

	using global::FMWebApp;

	/// <summary>
	///    This class handles the code behind functionality of the page.
	/// </summary>
	public partial class ProfileConfigSummaryForm : FMAutoSubmitFormBase, IMenuDiscovery, IEntityDiscovery
	{
		#region Explicit Interface Properties

		/// <summary>
		///    Gets a value indicating whether entity assignable.
		/// </summary>
		bool IEntityDiscovery.EntityAssignable
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		///    Gets the entity engine type.
		/// </summary>
		Type IEntityDiscovery.EntityEngineType
		{
			get
			{
				return typeof(IMobileDeviceProfiles);
			}
		}

		/// <summary>
		///    Gets the entity type.
		/// </summary>
		ENTITY_TYPE IEntityDiscovery.EntityType
		{
			get
			{
				return ENTITY_TYPE.MOBILE_DEVICE_PROFILE;
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
			var items = new List<FMMenuItem>();

			//TODO: Temporary commented out so that QA does not test profile features.
			//if (security.HasRight(RIGHT.VIEW_MOBILE_DEVICE_PROFILES) == false
			//	&& security.HasRight(RIGHT.MODIFY_MOBILE_DEVICE_PROFILES) == false)
			//{
			//	return null;
			//}

			//items.Add(
			//	new FMMenuItem
			//		{
			//			MenuItemType = FMMenuItemType.CONFIG_OTHER_PROFILES,
			//			RootMenuName = "Configuration",
			//			CategoryName = "Other",
			//			ItemName = "Profiles",
			//			NavigateUrl = "ProfileConfigSummaryForm.aspx",
			//			ApplyDataDictionary = ApplyDataDictionary.Apply
			//		});

			return items;
		}

		#endregion

		#region Explicit Interface Methods

		/// <summary>
		///    This method returns an entity to site map collection of mobile device profiles. It is
		///    used for the entity to site assignment.
		/// </summary>
		/// <param name="security">
		///    The security.
		/// </param>
		/// <param name="type">
		///    The type.
		/// </param>
		/// <returns>
		///    The FMBusinessObjects.DataObjects.EntityToSiteMapCollectionClass.
		/// </returns>
		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(
			SecurityClass security, ENTITY_ASSIGNMENT_TYPE type)
		{
			DataSet dataSet = FMChannelHelper.MakeCall<IMobileDeviceProfiles, DataSet>(
																	 x =>
																	 x.EnumerateAll(security)
																);
			var profileCollection = new MobileDeviceProfileCollection();
			profileCollection.Load(dataSet);

			var entityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			foreach (MobileDeviceProfile profile in profileCollection)
			{
				if (type == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if (security.SiteGuid == profile.SiteGuid || security.LoginSiteGuid != profile.SiteGuid)
					{
						continue;
					}
				}
				else
				{
					if (security.SiteGuid != profile.SiteGuid)
					{
						continue;
					}
				}

				var entityToSiteMap = new EntityToSiteMapClass
					{
						ID = profile.ProfileId,
						SiteGuid = profile.SiteGuid,
						IdentityGuid = profile.MobileDeviceProfileGuid,
						TypeID = profile.EntityType
					};

				entityToSiteMapCollection.Add(entityToSiteMap);
			}

			return entityToSiteMapCollection;
		}

		/// <summary>
		///    This method will return the profile GUID of the entity based on the
		///    profile ID.
		/// </summary>
		/// <param name="security">
		///    The security.
		/// </param>
		/// <param name="id">
		///    The id.
		/// </param>
		/// <returns>
		///    The System.Guid.
		/// </returns>
		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string id)
		{
			return FMChannelHelper.MakeCall<IMobileDeviceProfiles, Guid>(
																	 x =>
																	 x.GetGuid(security, id)
																);
		}

		/// <summary>
		///    This method will set the site GUID on the Profile entity.
		/// </summary>
		/// <param name="security">
		///    The security.
		/// </param>
		/// <param name="guid">
		///    The guid.
		/// </param>
		/// <param name="siteGuid">
		///    The site guid.
		/// </param>
		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid siteGuid)
		{
			MobileDeviceProfile profile = FMChannelHelper.MakeCall<IMobileDeviceProfiles, MobileDeviceProfile>(
																	 x =>
																	 x.GetByProfileGuid(security, guid)
																);

			profile.SiteGuid = siteGuid;
			FMChannelHelper.MakeCall<IMobileDeviceProfiles>(
																	 x =>
																	 x.Modify(security, profile)
																);
		}

		#endregion

		#region Methods

		/// <summary>
		///    This method will handle the Add button event and redirect to the Profile
		///    Configuration detail page.
		/// </summary>
		/// <param name="sender">Sender object for the event.</param>
		/// <param name="e">Event arguments.</param>
		protected void AddBtnOnClick(object sender, EventArgs e)
		{
			this.Session.Remove(PageSessionKeyConstants.ProfileConfigurationItemToEdit);
			this.Session.Remove(PageSessionKeyConstants.ProfileConfigurationProfileObject);

			this.Redirect("ProfileConfigurationForm.aspx");
		}

		/// <summary>
		///    This method handles the find button click. It will add the find string
		///    and get the profiles based on the find string.
		/// </summary>
		/// <param name="sender">
		///    The sender.
		/// </param>
		/// <param name="e">
		///    The e.
		/// </param>
		protected void FindButtonOnClick(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(this.FindTextBox.Text))
			{
				this.Session.Remove(PageSessionKeyConstants.ProfileConfigurationFindString);
			}
			else
			{
				this.Session.Add(PageSessionKeyConstants.ProfileConfigurationFindString, this.FindTextBox.Text);
			}

			DataSet profileDataSet = this.RetrieveProfiles();
			this.LoadProfileGrid(profileDataSet);
		}

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		///    This method handles the page size dropdown change selection.
		/// </summary>
		/// <param name="source">
		///    The source.
		/// </param>
		/// <param name="e">
		///    The e.
		/// </param>
		protected void PageSizeDropDown_SelectedIndexChanged(object source, EventArgs e)
		{
			DataSet profileDataSet = this.RetrieveProfiles();
			this.LoadProfileGrid(profileDataSet);
		}

		/// <summary>
		///    This method handles the page load event.
		/// </summary>
		/// <param name="sender">
		///    The sender.
		/// </param>
		/// <param name="e">
		///    The e.
		/// </param>
		protected void Page_Load(object sender, EventArgs e)
		{
			this.GetSecurity();

			DataSet profileDataSet = this.RetrieveProfiles();
			this.LoadProfileGrid(profileDataSet);

			this.DisableFields();
		}

		/// <summary>
		///    This method will handle the delete event.  It will remove only one mobile device profile
		///    entry from the database based on the profile GUID.
		/// </summary>
		/// <param name="source">Object source of the event.</param>
		/// <param name="e">Event arguments object.</param>
		protected void ProfileDataGrid_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			TableCell profileGuidCell = e.Item.Cells[1];//bds

			Guid profileGuid = Guid.Parse(profileGuidCell.Text);
			FMChannelHelper.MakeCall<IMobileDeviceProfiles>(
																	 x =>
																	 x.Purge(this.Security, profileGuid)
																);

			// Update the grid with new data.
			DataSet profileDataSet = this.RetrieveProfiles();
			this.LoadProfileGrid(profileDataSet);

			this.DisableFields();
		}

		/// <summary>
		///    This method handles the edit event.  It will identify the row to be edited, save the
		///    the items GUID in session, and redirect to the profile configuration form.
		/// </summary>
		/// <param name="source">Source object</param>
		/// <param name="e">Event object</param>
		protected void ProfileDataGrid_EditCommand(object source, DataGridCommandEventArgs e)
		{
			this.Session.Remove(PageSessionKeyConstants.ProfileConfigurationItemToEdit);

			TableCell profileGuidCell = e.Item.Cells[1];//bds
			this.Session.Add(PageSessionKeyConstants.ProfileConfigurationItemToEdit, profileGuidCell.Text);

			this.Redirect("ProfileConfigurationForm.aspx");
		}

		/// <summary>
		///    This method handles the profile summary item data bound. It will disable the
		///    delete link button if the user does not have the "modify mobile device profiles"
		///    right.
		/// </summary>
		/// <param name="sender">
		///    The sender.
		/// </param>
		/// <param name="e">
		///    The e.
		/// </param>
		protected void ProfileSummaryItemDataBound(object sender, DataGridItemEventArgs e)
		{
			var deleteLinkButton = e.Item.FindControl("DeleteLinkButton") as FMDeleteLinkButton;

			if (deleteLinkButton != null)
			{
				TableCell siteGuidCell = e.Item.Cells[2];//bds
				Guid siteGuid = Guid.Empty;

				if (string.IsNullOrEmpty(siteGuidCell.Text) == false)
				{
					siteGuid = Guid.Parse(siteGuidCell.Text);
				}

				deleteLinkButton.Enabled = this.Security.HasRight(RIGHT.MODIFY_MOBILE_DEVICE_PROFILES)
				                           && (this.Security.SiteGuid == siteGuid);
			}
		}

		/// <summary>
		///    This method handles the show all button click. It will remove the find string
		///    and get all the profiles.
		/// </summary>
		/// <param name="sender">
		///    The sender.
		/// </param>
		/// <param name="e">
		///    The e.
		/// </param>
		protected void ShowAllButtonOnClick(object sender, EventArgs e)
		{
			this.FindTextBox.Text = string.Empty;
			this.Session.Remove(PageSessionKeyConstants.ProfileConfigurationFindString);

			DataSet profileDataSet = this.RetrieveProfiles();
			this.LoadProfileGrid(profileDataSet);
		}

		/// <summary>
		///    This method will disable all fields if the user does not have the
		///    "modify mobile device profile" right.
		/// </summary>
		private void DisableFields()
		{
			if (this.Security.HasRight(RIGHT.MODIFY_MOBILE_DEVICE_PROFILES) == false)
			{
				this.AddBottomBtn.Enabled = false;
				this.AddTopBtn.Enabled = false;
			}
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.ProfileDataGrid.EditCommand += new DataGridCommandEventHandler(this.ProfileDataGrid_EditCommand);
			this.ProfileDataGrid.DeleteCommand += new DataGridCommandEventHandler(this.ProfileDataGrid_DeleteCommand);
			this.ProfileDataGrid.ItemDataBound += new DataGridItemEventHandler(this.ProfileSummaryItemDataBound);

			int pageLimit = 10;
			this.ProfileSummaryPageSizeDropDown.SetLimit(pageLimit);
			this.ProfileDataGrid.PageSize = pageLimit;
		}

		/// <summary>
		///    This method loads the profile grid with information from tblMobileDeviceProfile table.
		/// </summary>
		/// <param name="profileDataSet">
		///    The profile Data Set.
		/// </param>
		private void LoadProfileGrid(DataSet profileDataSet)
		{
			var gridTable = new DataTable("ProfileTable");

			var column = new DataColumn("MobileDeviceProfileGuid", Type.GetType("System.String"));
			gridTable.Columns.Add(column);

			column = new DataColumn("SiteGuid", Type.GetType("System.String"));
			gridTable.Columns.Add(column);

			column = new DataColumn("ProfileID", Type.GetType("System.String"));
			gridTable.Columns.Add(column);

			column = new DataColumn("ProfileDescription", Type.GetType("System.String"));
			gridTable.Columns.Add(column);

			column = new DataColumn("DeviceCount", Type.GetType("System.String"));
			gridTable.Columns.Add(column);

			if ((profileDataSet != null) && (profileDataSet.Tables.Count > 0))
			{
				DataTable table = profileDataSet.Tables[0];

				if ((table != null) && (table.Rows.Count > 0))
				{
					foreach (DataRow profileRow in table.Rows)
					{
						if (profileRow != null)
						{
							DataRow row = gridTable.NewRow();
							row["MobileDeviceProfileGuid"] = profileRow["MobileDeviceProfileGuid"];
							row["SiteGuid"] = profileRow["SiteGuid"];
							row["ProfileID"] = profileRow["ProfileID"];
							row["ProfileDescription"] = profileRow["Description"];

							row["DeviceCount"] = "0";
							if (profileRow.IsNull("DeviceCount") == false)
							{
								var deviceCount = (int)profileRow["DeviceCount"];
								row["DeviceCount"] = deviceCount.ToString(CultureInfo.InvariantCulture);
							}

							gridTable.Rows.Add(row);
						}
					}
				}
			}

			var view = new DataView(gridTable);
			this.ProfileDataGrid.DataSource = view;
			this.ProfileDataGrid.DataBind();
		}

		/// <summary>
		///    This method will retrieve the profile data from the database based on a find filter
		///    or an empty filter.
		/// </summary>
		/// <returns>
		///    The System.Data.DataSet.
		/// </returns>
		private DataSet RetrieveProfiles()
		{
			// Check to see if there is a find string to filter the list of profiles.
			string findStr = string.Empty;
			if (this.Page.Session[PageSessionKeyConstants.ProfileConfigurationFindString] != null)
			{
				findStr = this.Page.Session[PageSessionKeyConstants.ProfileConfigurationFindString] as string;
			}

			if (string.IsNullOrEmpty(findStr))
			{
				DataSet dataSet = FMChannelHelper.MakeCall<IMobileDeviceProfiles, DataSet>(
																	 x =>
																	 x.EnumerateAll(this.Security)
																);

				return dataSet;
			}
			else
			{
				DataSet dataSet = FMChannelHelper.MakeCall<IMobileDeviceProfiles, DataSet>(
																	 x =>
																	 x.EnumerateByFindFilter(this.Security, findStr)
																);

				return dataSet;
			}
		}

		#endregion
	}
}