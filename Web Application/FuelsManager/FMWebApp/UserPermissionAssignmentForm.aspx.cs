// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserPermissionAssignmentForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the UserPermissionAssignmentForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using FMCore;

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Diagnostics;
	using System.Linq;
	using System.Text;
	using System.Web.UI;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using global::FMWebApp;
	using FMControls;

    /// <summary>
	/// Code behind for the user permission assignment form.
	/// </summary>
	public partial class UserPermissionAssignmentForm : FMFormBase, IMenuDiscovery
	{
		#region Private data members
		private const string ViewstateKeySortDirection = "SORTDIRECTION";
		private const string ViewstateKeySortField = "SORTFIELD";
		private const string ViewstateKeySortField2 = "SORTFIELD2";
		private const string ViewstateFindString = "FINDSTRINGFIELD";

		private const string SessionUserPermissionFlattenGroups = "UserPermissionFlattenGroups";

		private GroupCollectionClass flattenedGroups;

		private bool ActiveDirectoryUser;
		#endregion

		/// <summary>
		/// Gets a value indicating whether this instance is from user form.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is from user form; otherwise, <c>false</c>.
		/// </value>
		private bool IsFromUserForm
		{
			get { return this.Page.Request.GetQueryOrFormValue("Mode") != null && this.Page.Request.GetQueryOrFormValue("Mode").Equals( "User" ); }
		}

		#region Menu

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
			
			if (!siteGroup)
			{
				return null;
			}

			// DLA Defense systems have custom form; don't display this form
			if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDescKey()))
			{
				return null;
			}

			if (security.HasRight(RIGHT.MODIFY_USERS) == false
				&& security.HasRight(RIGHT.VIEW_USERS) == false)
			{
				return null;
			}

			items.Add(
				new FMMenuItem
				{
					MenuItemType = FMMenuItemType.ADMIN_SECURITY_USER_PERMIMISSIONS,
					RootMenuName = "Administration",
					CategoryName = "Security",
					ItemName = "User Permissions",
					NavigateUrl = "UserPermissionAssignmentForm.aspx",
					ApplyDataDictionary = ApplyDataDictionary.Apply
				});

			return items;
		}

		#endregion

		/// <summary>
		/// Handles the Init event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		protected void Page_Init(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (this.Page.IsPostBack)
				{
					this.flattenedGroups = (GroupCollectionClass)this.Session[SessionUserPermissionFlattenGroups];

					var groupColumnArrayList = this.Session["UPGColumns"] as ArrayList;
					if (groupColumnArrayList != null)
					{
						foreach (Guid groupGuid in groupColumnArrayList)
						{
							var customField = new TemplateColumn();

							var index = this.flattenedGroups.FindIndex(x => x.IdentityGuid == groupGuid);
							GroupClass grp = index >= 0 ? this.flattenedGroups[index] : null;

							if (grp == null)
							{
								continue;
							}

							// Create the dynamic templates and assign them to 
							// the appropriate template property.
							customField.ItemTemplate = new GroupCheckBoxColumn(DataControlRowType.DataRow, groupGuid, grp.ID, index, grp.RightCollection.Count == 1, this.ActiveDirectoryUser);
							customField.HeaderTemplate = new GroupCheckBoxColumn(DataControlRowType.Header, groupGuid, grp.ID, index, grp.RightCollection.Count == 1);

							// Add the field column to the Columns collection of the
							// GridView control.
							this.UPG.Columns.Add(customField);
						}
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				// Disable the Apply buttons unless the user has modify company data rights. 
				this.ApplyBtnSecurityCheck();

				if (!this.Page.IsPostBack)
				{
					this.flattenedGroups = FMChannelHelper.MakeCall<IGroups, GroupCollectionClass>( x => x.EnumerateAllForGrid( this.Security ) );
					this.Session[SessionUserPermissionFlattenGroups] = this.flattenedGroups;

					this.ViewState[ViewstateKeySortField] = "SiteID";
					this.ViewState[ViewstateKeySortDirection] = "ASC";
					this.ViewState[ViewstateKeySortField2] = ", UserID ASC";
					
					this.FindTextBox.Text = string.Empty;
					this.LoadSiteGroupDropDown();
					this.LoadSiteDropDown();
					this.LoadUserDropDown();
					this.SetFilterFields();
					this.PersistFilters();

					if (this.IsFromUserForm)
					{
						// Hide the user name column
						this.UPG.Columns[1].Visible = false;

						this.TopCloseBtn.Visible = true;
						this.BottomCloseBtn.Visible = true;
					}
					else
					{
						this.UPG.DataSource = new DataView();
						this.UPG.DataBind();

						this.TopCloseBtn.Visible = false;
						this.BottomCloseBtn.Visible = false;
					}

					this.ReloadGrid();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the Onclick event of the CloseBtn control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		protected void CloseBtn_Onclick(object sender, EventArgs e)
		{
			this.Redirect(UserForm.UserFormUrl);
		}

		#region Private methods

		/// <summary>
		/// This method will disable the Apply buttons if the user does not have rights.
		/// </summary>
		private void ApplyBtnSecurityCheck()
		{
			this.TopApplyButton.Enabled = false;
			this.BottomApplyButton.Enabled = false;

			if (this.Security.HasRight(RIGHT.MODIFY_USERS))
			{
				this.TopApplyButton.Enabled = true;
				this.BottomApplyButton.Enabled = true;
			}
		}

		/// <summary>
		/// This method will load the site dropdown list with a list of sites if the login site
		/// is a site group or just one site if not a site group.
		/// </summary>
		private void LoadSiteDropDown()
		{
			this.SiteDropDown.Items.Clear();
			//this.ClearCombobox(this.SiteDropDown);

			var selectedSiteGroup = new Guid(this.SiteGroupDropDown.SelectedValue);

			var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.GetBasic(this.Security, selectedSiteGroup));
			var isDesc = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDescKey());

			if (site != null)
			{
				if (site.SiteGroup)
				{
					//this.SiteDropDown.Items.Add(new ListItem("{None}", "{None}"));
					this.SiteDropDown.Items.Add(new ListItem("{All}", "{All}"));

					var siteCollection = FMChannelHelper.MakeCall<ISites, SiteCollectionClass>(x => x.EnumerateByParentSiteCurrentUserAssigned(this.Security, selectedSiteGroup));
					this.SiteDropDown.Items.Add(new ListItem(site.ID, site.IdentityGuid.ToString()));

					foreach (SiteClass nextSite in siteCollection)
					{
						string siteName = nextSite.ID;

						if (isDesc)
						{
							if (false == String.IsNullOrEmpty(nextSite.Number))
							{
								siteName = string.Format("{0} - {1}", nextSite.ID, nextSite.Number);
							}
						}

						if (nextSite.SiteGuid != site.SiteGuid)
						{
							this.SiteDropDown.Items.Add(new ListItem(siteName, nextSite.IdentityGuid.ToString()));
						}
					}
				}
				else
				{
					this.SiteDropDown.Items.Add(new ListItem(site.ID, site.IdentityGuid.ToString()));
				}

				//if (this.IsFromUserForm)
				//{
					this.SiteDropDown.SelectByText("{All}");
				//}
				//else
				//{
				//	// Initially set the default to none so we dont load the grid with a bunch of invalid data.
				//	this.SiteDropDown.SelectByText("{None}");
				//}
			}
		}

		/// <summary>
		/// Loads the site group drop down.
		/// </summary>
		private void LoadSiteGroupDropDown()
		{
			this.SiteGroupDropDown.Items.Clear();

			var site =
				FMChannelHelper.MakeCall<ISites, SiteClass>(
					x =>
					x.Get(
						this.Security,
						this.Security.SiteGuid,
						getMemberSites: true,
						getSchedulesAndProcessVariables: false,
						bGetAssociatedAliases: false));

			if (site != null)
			{
				var siteGroups = new List<ListItem>();

				if (site.SiteGroup)
				{
					// The site we came in at
					siteGroups.Add(new ListItem("{All}", this.Security.SiteGuid.ToString())); 
					siteGroups.Add(new ListItem(site.ID, site.IdentityGuid.ToString()));

					foreach (SiteToSiteMapClass siteToSiteMap in site.SiteToSiteMapCollection)
					{
						if (siteToSiteMap.ChildGroup)
						{
							siteGroups.Add(new ListItem(siteToSiteMap.ChildSiteID, siteToSiteMap.ChildSiteGuid.ToString()));
						}
					}
				}

				// Sort the list
				this.SiteGroupDropDown.DataTextField = "Text";
				this.SiteGroupDropDown.DataValueField = "Value";
				this.SiteGroupDropDown.DataSource = siteGroups.OrderBy(x => x.Text).ToList();
				this.SiteGroupDropDown.DataBind();

				if (this.IsFromUserForm)
				{
					this.SiteGroupDropDown.SelectByText(this.Session[PageSessionKeyConstants.UGAF_SESSION_SITE_SELECT].ToString());
				}
				else
				{
					// Initially set the default to the first item in the list.
					this.SiteGroupDropDown.SelectByText("{All}");
				}
			}
		}

		/// <summary>
		/// Loads the user drop down.
		/// </summary>
		private void LoadUserDropDown()
		{
			this.UserDropdown.Items.Clear();

			// Get the single sign on mode flag.
		    bool ssoMode = this.IsSsoMode();

		    if (ssoMode == false)
		    {
		        this.UserDropdown.Items.Add(new ListItem("{All}", Guid.Empty.ToString()));
		    }
		    else
		    {
		        this.UserDropdown.Items.Add(new ListItem("{None}", "FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF"));
		    }

		    // this loads users across the site group and any of its children sites
			UserCollectionClass userCollection =
				FMChannelHelper.MakeCall<IUsers, UserCollectionClass>(
					x => x.EnumerateForParentSiteByAssignedUser(this.Security, this.Security.SiteGuid));


			foreach (UserClass nextUser in userCollection)
			{
				this.UserDropdown.Items.Add(new ListItem(nextUser.ID, nextUser.IdentityGuid.ToString()));
			}

			if (this.IsFromUserForm)
			{
				var userName = this.Session[PageSessionKeyConstants.UGAF_SESSION_USER_SELECT].ToString();

				var user = FMChannelHelper.MakeCall<IUsers, UserClass>(x => x.GetByID(this.Security, userName));

				var userGuid = Guid.Empty;
				if (user != null && user.IdentityGuid != Guid.Empty)
				{
					userGuid = user.IdentityGuid;
					this.ActiveDirectoryUser = user.ActiveDirectoryUser;
				}
				if (userGuid != Guid.Empty)
				{
					if (this.UserDropdown.SelectByText(userName) < 0)
					{
						this.UserDropdown.Items.Add(new ListItem(userName, userGuid.ToString()));
						this.UserDropdown.SelectByText(userName);
					}
				}
				else
				{
					this.UserDropdown.SelectByText(userName);
				}

				this.UserDropdown.Enabled = false;
			}
			else
			{
				// Initially set the default to the first item in the list.
				this.UserDropdown.SelectByText("{All}");
				this.UserDropdown.Enabled = true;
			}
		}

		protected void PageSizeDropDownSelectedIndexChanged(object source, EventArgs e)
		{
			ReloadGrid();
		}

		/// <summary>
		/// This method will retrieve new company role data and bind the data to
		/// the company role grid.  The data will be retrieved based on the filting
		/// criterion.
		/// </summary>
		private void UpdateView()
		{
			string findString = this.FindTextBox.Text;
			string sortKey = "SiteID ASC";

			if (!string.IsNullOrEmpty(findString))
			{
				findString = findString.ToUpper();
			}

			if (this.ViewState[ViewstateKeySortField] != null)
			{
				sortKey = (string) this.ViewState[ViewstateKeySortField];
			}

			if (sortKey != string.Empty && this.ViewState[ViewstateKeySortDirection] != null)
			{
				sortKey += " " + (string)this.ViewState[ViewstateKeySortDirection];
			}

			if (this.ViewState[ViewstateKeySortField2] != null)
			{
				sortKey += (string)this.ViewState[ViewstateKeySortField2];
			}

			try
			{
				Guid userGuid = Guid.Empty;
				Guid siteGuid;

				bool loadChildrenSites = this.SiteDropDown.SelectedValue == "{All}";
				if (loadChildrenSites)
				{
					siteGuid = new Guid(this.SiteGroupDropDown.SelectedValue);
				}
				else
				{
					siteGuid = new Guid(this.SiteDropDown.SelectedValue);
				}

                if (this.UserDropdown.SelectedItem.Text != "{All}")
				{
					userGuid = new Guid(this.UserDropdown.SelectedValue);
                    var user = FMChannelHelper.MakeCall<IUsers, UserClass>(x => x.Get(this.Security, userGuid));
				    this.ActiveDirectoryUser = user.ActiveDirectoryUser;
				}

				while (this.UPG.Columns.Count > 4)
				{
					this.UPG.Columns.RemoveAt(this.UPG.Columns.Count - 1);
				}

				var dataSet =
					FMChannelHelper.MakeCall<IUserGroupMaps, DataSet>(
						x => x.EnumerateByUserPermissionGrid(this.Security, userGuid, siteGuid, loadChildrenSites, findString));


				if (dataSet.Tables.Count == 1)
				{
					var groupColumnArrayList = new ArrayList();

					foreach (DataColumn column in dataSet.Tables[0].Columns)
					{
						if (column.ColumnName == "SiteGuid" || column.ColumnName == "UserGuid" || column.ColumnName == "SiteID"
						    || column.ColumnName == "UserID" || column.ColumnName == "UserOwnerSiteGuid" || column.ColumnName == "OwnedBy")
						{
							continue;
						}

						var customField = new TemplateColumn();
						
						var groupGuid = new Guid(column.ColumnName);
						var index = this.flattenedGroups.FindIndex(x => x.IdentityGuid == groupGuid);
						GroupClass grp = index >= 0 ? this.flattenedGroups[index] : null;

						if (grp == null)
						{
							continue;
						}

						groupGuid = grp.IdentityGuid;

						// Create the dynamic templates and assign them to 
						// the appropriate template property.
						customField.ItemTemplate = new GroupCheckBoxColumn(DataControlRowType.DataRow, groupGuid, grp.ID, index, grp.RightCollection.Count == 1, this.ActiveDirectoryUser);
						customField.HeaderTemplate = new GroupCheckBoxColumn(DataControlRowType.Header, groupGuid, grp.ID, index, grp.RightCollection.Count == 1);

						// Add the field column to the Columns collection of the
						// GridView control.
						this.UPG.Columns.Add(customField);
						groupColumnArrayList.Add(groupGuid);
					}


					// Bind the data to the grid.
					var dv = new DataView(dataSet.Tables[0]) { Sort = sortKey };

					CompanySummaryPageSizeDropDown.SetPageSize(UPG, dv.Count);

					this.UPG.DataSource = dv;
					this.UPG.DataBind();

					this.ShowResults(true);

					this.Session["UPGColumns"] = groupColumnArrayList;

				}
				else
				{
					this.ShowResults(false);
				}
			}
			catch (Exception except)
			{
				const string ErrMsg = "Error retrieving User Permissions.";
				this.ErrorHandler(new ApplicationException(ErrMsg, except));
			}
		}

		private void ShowResults(bool areResults)
		{
			UPG.Visible = areResults;
			TopApplyButton.Enabled = areResults;
			BottomApplyButton.Enabled = areResults;
			noResultsLabel.Visible = !areResults;
		}
		
		/// <summary>
		/// This method will persist the company role page filters.
		/// </summary>
		private void PersistFilters()
		{
			if (this.ViewState[ViewstateFindString] == null)
			{
				if ((this.FindTextBox != null) && (!string.IsNullOrEmpty(this.FindTextBox.Text)))
				{
					this.ViewState.Add(ViewstateFindString, this.FindTextBox.Text.ToUpper());
				}
				else
				{
					this.ViewState.Add(ViewstateFindString, string.Empty);
				}
			}
			else
			{
				if ((this.FindTextBox != null) && (!string.IsNullOrEmpty(this.FindTextBox.Text)))
				{
					this.ViewState[ViewstateFindString] = this.FindTextBox.Text.ToUpper();
				}
				else
				{
					this.ViewState[ViewstateFindString] = string.Empty;
				}
			}
		}

		/// <summary>
		/// This method will set all the Filters to their previous values.
		/// </summary>
		private void SetFilterFields()
		{
			if (this.ViewState[ViewstateFindString] != null)
			{
				this.FindTextBox.Text = this.ViewState[ViewstateFindString].ToString();
			}
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// This method handles the on initialize event from ASP.NET.
		/// </summary>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		protected override void OnInit( EventArgs e )
		{
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			this.InitializeComponent();
			base.OnInit(e);

			this.GetSecurity();
		}

		/// <summary>
		/// This method initialize and adds events to the page.
		/// </summary>
		private void InitializeComponent()
		{
			this.UPG.PageIndexChanged += this.UserPermissionGridPageIndexChanged;
			this.UPG.ItemCreated += UpgOnItemCreated;
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

				if (configSetting != null && string.IsNullOrEmpty(configSetting.SettingValue) == false && configSetting.SettingValue == "1")
				{
					isSsoMode = true;
				}
			}
			catch (Exception)
			{
				return isSsoMode;
			}

			return isSsoMode;
		}

		private void UpgOnItemCreated(object sender, DataGridItemEventArgs dataGridItemEventArgs)
		{
			if (dataGridItemEventArgs.Item.ItemType == ListItemType.Item && dataGridItemEventArgs.Item.ItemIndex == 0)
			{
				var applyRow = new DataGridItem(1, 0, ListItemType.Item);
				var blankRow = new DataGridItem(0, 0, ListItemType.Header);

				var cell = new TableCell { Text = "Apply To Page" };
				cell.Font.Bold = true;
				applyRow.Cells.Add(cell);

				cell = new TableCell { Text = string.Empty };
				//cell.BackColor = headerColor;
				blankRow.Cells.Add(cell);

				for (int i = 1; i < UPG.Columns.Count; i++)
				{
					var dgColTemp = UPG.Columns[i] as TemplateColumn;

					if (dgColTemp != null)
					{
						cell = new TableCell();
						var headerCheck = new  FMCheckBox();
						headerCheck.Attributes.Add("onClick", "javascript:SelectAllCheckboxes(this, '" + ((GroupCheckBoxColumn)(dgColTemp.HeaderTemplate)).GroupID + "');");
					    headerCheck.Attributes.Add("class", "determinate");

						// Get the single sign on mode flag.
					    bool ssoMode = this.IsSsoMode();

					    if (ssoMode)
					    {
					        headerCheck.Enabled = false;
					    }

                        cell.Controls.Add(headerCheck);
                        applyRow.Cells.Add(cell);
                    }
                    else
					{
						cell = new TableCell { Text = string.Empty };
						applyRow.Cells.Add(cell);
					}

					cell = new TableCell { Text = "&nbsp;" };
					//cell.BackColor = headerColor;
					blankRow.Cells.Add(cell);
				}

				this.UPG.Controls[0].Controls.Add(applyRow);
				this.UPG.Controls[0].Controls.Add(blankRow);
			}
		}

		/// <summary>
		/// This method handles the Find Button on click event. It will update
		/// the filters and update the view with new data.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		protected void FindBtn_OnClick(object sender, EventArgs e)
		{
			try
			{
				this.ReloadGrid();
			}
			catch ( Exception except )
			{
				this.ErrorHandler( except );
			}
		}

		/// <summary>
		/// This method handles the Show All Button on click event. It will update
		/// the filters and update the view with new data.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		protected void ShowAllBtn_OnClick(object sender, EventArgs e)
		{
			try
			{
				this.FindTextBox.Text = string.Empty;
				this.ReloadGrid();
			}
			catch ( Exception except )
			{
				this.ErrorHandler( except );
			}
		}

		protected void ReloadGrid()
		{
			UPG.CurrentPageIndex = 0;
			this.PersistFilters();
			this.UpdateView();
		}


		/// <summary>
		/// Users the selection change.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		protected void UserSelectionChange(object sender, EventArgs e)
		{
			try
			{
				////// When the event is null, that means it was called on the initial page
				////// load and there is no need to refresh the grid.
				if ( e != null )
				{
					// Refresh the grid based on the filter settings.
					this.ReloadGrid();
				}
			}
			catch ( Exception except )
			{
				this.ErrorHandler( except );
			}
		}

		/// <summary>
		/// This method will handle the page index change event.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="eventArgs"></param>
		protected void UserPermissionGridPageIndexChanged( object source, DataGridPageChangedEventArgs eventArgs )
		{
			try
			{
				if (eventArgs.NewPageIndex != -1)
				{
					this.UPG.CurrentPageIndex = eventArgs.NewPageIndex;
					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// This method will handle the Apply button event. It will save all the changes to the
		/// database.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		protected void ApplyBtn_Onclick(object sender, EventArgs e)
		{
			bool failed = false;
			this.PersistFilters();

			try
			{
				var gridItems = this.UPG.Items;

				var groupGuidList = new List<Guid>();

				// Build a list of available groups
				foreach (var col in this.UPG.Columns)
				{
					var templateColumn = col as TemplateColumn;
					if (templateColumn != null)
					{
						var template = templateColumn.ItemTemplate as GroupCheckBoxColumn;

						if (template != null)
						{
							groupGuidList.Add(template.GroupGuid);
						}
					}
				}

				if (gridItems.Count > 0)
				{
					var toRemove = new List<UserGroupMapClass>();
					var toAdd = new List<UserGroupMapClass>();
					var toUpdate = new List<UserGroupMapClass>();
					var userGuidString = this.UserDropdown.SelectedValue;

					// get the user record based on selected value of the dropdown
					// then save off the ActiveDirectoryUser flag
					Guid userGuid;
					if (Guid.TryParse(userGuidString, out userGuid) == false)
					{
						return;
					}
					var user = FMChannelHelper.MakeCall<IUsers, UserClass>(x => x.Get(this.Security, userGuid));
					this.ActiveDirectoryUser = user.ActiveDirectoryUser;

					foreach (DataGridItem item in gridItems)
					{
						foreach (Guid grpGuid in groupGuidList)
						{
							// Find the index
							var index = this.flattenedGroups.FindIndex(x => x.IdentityGuid == grpGuid);

							// find the checkbox control
							var checkBox = item.FindControl("c" + index) as CheckBox;

							// find the hidden tristate control
							//var triStateControl = item.FindControl("triState_c" + index);
							//var triStateControl = this.FindControlRecursiveEndsWith(this, "c" + index + "_triState");
							//var triStateValue = this.Page.Request.Form["UPG:_ctl5:c35_triState"];
							if (checkBox != null && checkBox.Enabled)
							{
								// if we can't get/parse the bool value of the originalValue attribute, skip
								int originalValue;
								if (int.TryParse(checkBox.Attributes["OV"], out originalValue) == false)
								{
									continue;
								}

								// if we can't get/parse the bool value of the triState control, skip
								string triStateString = this.Page.Request.Form[checkBox.UniqueID + "_triState"];
								bool triStateValue;
								if (this.ActiveDirectoryUser)
								{
									//if (bool.TryParse(((HtmlInputHidden)triStateControl).Value, out triStateValue) == false)
									if (bool.TryParse(triStateString, out triStateValue) == false)
									{
										continue;
									}
								}
								else
								{
									triStateValue = false;
								}

								bool checkValue = checkBox.Checked;

								// instantiate a mapping
								var userGroupMap = new UserGroupMapClass
								                   {
									                   GroupGuid = grpGuid,
									                   SiteGuid = new Guid(item.Cells[2].Text),
									                   UserGuid = new Guid(item.Cells[3].Text)
								                   };

								//if (this.ActiveDirectoryUser == true && originalValue == 2)
								if (originalValue == 2)
								{
									userGroupMap.DenyAdPermission = triStateValue;
									toUpdate.Add(userGroupMap);
								}
								if (originalValue == 0 && triStateValue == false && checkValue == true)
								{
									if (index == 35)
									{
										Debug.WriteLine($"Control:{checkBox.ClientID}  OV:{originalValue}  TS:{triStateValue}  CHK:{checkValue}    ");
										Debug.WriteLine("Action: Add, Deny=false");
									}
									toAdd.Add(userGroupMap);
									continue;
								}

								if (originalValue == 0 && triStateValue == true && checkValue == false)
								{
									if (index == 35)
									{
										Debug.WriteLine($"Control:{checkBox.ClientID}  OV:{originalValue}  TS:{triStateValue}  CHK:{checkValue}    ");
										Debug.WriteLine("Action: Add, Deny=false");
									}
									userGroupMap.DenyAdPermission = true;
									toAdd.Add(userGroupMap);
									continue;
								}

								if (originalValue == 2 && triStateValue == false && checkValue == true)
								{
									if (index == 35)
									{
										Debug.WriteLine($"Control:{checkBox.ClientID}  OV:{originalValue}  TS:{triStateValue}  CHK:{checkValue}    ");
										Debug.WriteLine("Action: Add, Deny=false");
									}
									toUpdate.Add(userGroupMap);
									continue;
								}

								if (originalValue == 2 && triStateValue == false && checkValue == false)
								{
									if (index == 35)
									{
										Debug.WriteLine($"Control:{checkBox.ClientID}  OV:{originalValue}  TS:{triStateValue}  CHK:{checkValue}    ");
										Debug.WriteLine("Action: Update, Deny=false");
									}
									userGroupMap.DenyAdPermission = false;
									toRemove.Add(userGroupMap);
									continue;
								}

								if (originalValue == 1 && triStateValue == true && checkValue == false)
								{
									if (index == 35)
									{
										Debug.WriteLine($"Control:{checkBox.ClientID}  OV:{originalValue}  TS:{triStateValue}  CHK:{checkValue}    ");
										Debug.WriteLine("Action: Update, Deny=true");
									}
									userGroupMap.DenyAdPermission = true;
									toUpdate.Add(userGroupMap);
									continue;
								}

								if (originalValue == 1 && triStateValue == false && checkValue == false)
								{
									if (index == 35)
									{
										Debug.WriteLine($"Control:{checkBox.ClientID}  OV:{originalValue}  TS:{triStateValue}  CHK:{checkValue}    ");
										Debug.WriteLine("Action: Remove, Deny=false");
									}
									toRemove.Add(userGroupMap);
									continue;
								}
							}
						}
					}

					FMChannelHelper.MakeCall<IUserGroupMaps>(
						ugm =>
						{
							// Delete removed items
							foreach (UserGroupMapClass delGrpMap in toRemove)
							{
								ugm.Purge(this.Security, delGrpMap.UserGuid, delGrpMap.GroupGuid, delGrpMap.SiteGuid);
							}

							// Add newly checked items
							foreach (UserGroupMapClass addGrpMap in toAdd)
							{
								ugm.Add(this.Security, addGrpMap);
							}

							foreach (UserGroupMapClass updateGrpMap in toUpdate)
							{
								ugm.UpdateDenyFlag(this.Security, updateGrpMap);
							}

						});
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				failed = true;
			}

			this.UpdateView();

			if (!failed)
			{
				// Just let user know that it was successful
				this.RenderErrorMessage("The changes were saved successfully.");
			}
		}

		/// <summary>
		/// This method will handle the sort command event. It will save the sort column in session.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="e">The <see cref="DataGridSortCommandEventArgs"/> instance containing the event data.</param>
		protected void UserPermissionGridSortCommand( object source, DataGridSortCommandEventArgs e )
		{
			bool validSortKey = false;
			string sortExpression = e.SortExpression;

			// Can only sort on the User or Site columns.
			if (sortExpression != null)
			{
			    validSortKey = sortExpression.ToUpper().Equals("SITEID") || sortExpression.ToUpper().Equals("USERID");
			}

			if (validSortKey)
			{
				if ((string)this.ViewState[ViewstateKeySortField] == sortExpression)
				{
					if ((string)this.ViewState[ViewstateKeySortDirection] == "ASC")
					{
						this.ViewState[ViewstateKeySortDirection] = "DESC";
					}
					else
					{
						this.ViewState[ViewstateKeySortDirection] = "ASC";
					}
				}
				else
				{
					if (this.ViewState[ViewstateKeySortField2] != null)
					{
						this.ViewState[ViewstateKeySortField2] = "," + this.ViewState[ViewstateKeySortField] + " ASC";
					}

					this.ViewState[ViewstateKeySortDirection] = "ASC";
					this.ViewState[ViewstateKeySortField] = sortExpression;
				}
				
				this.PersistFilters();
				this.UpdateView();
			}
		}

		/// <summary>
		/// This method handles the company role selection change event. It will update
		/// the grid based on the filters.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		protected void SiteDropDownSelectChange(object sender, EventArgs e)
		{
			try
			{
				this.LoadUserDropDown();
				this.ReloadGrid();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		#endregion

		/// <summary>
		/// Performs refresh when site group selection changes.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		protected void SiteGroupSelectionChange(object sender, EventArgs e)
		{
			try
			{
				this.LoadSiteDropDown();
				this.LoadUserDropDown();
				this.ReloadGrid();
			}
			catch ( Exception except )
			{
				this.ErrorHandler( except );
			}
		}
	}

	/// <summary>
	/// Create a template class to represent a dynamic template column.
	/// </summary>
	public class GroupCheckBoxColumn : ITemplate
	{
		private readonly DataControlRowType templateType;
		private readonly Guid grpGuid;
		private readonly string groupId;

		private readonly int groupIndex;
		private bool hasViewOperateOnly;

		public bool HasViewOperateOnly { get { return this.hasViewOperateOnly; } }

		public Guid GroupGuid { get { return this.grpGuid; } }
		public string GroupID { get { return this.groupId; } }

	    private bool ActiveDirectoryUser;

        public GroupCheckBoxColumn(DataControlRowType type, Guid groupGuid, string groupID, int groupIndex, bool hasViewOperateOnly, bool activeDirectoryUser = false)
		{
			this.templateType = type;
			this.grpGuid = groupGuid;
			this.groupId = groupID;
			this.groupIndex = groupIndex;
			this.hasViewOperateOnly = hasViewOperateOnly;
            this.ActiveDirectoryUser = activeDirectoryUser;
		}

        public void InstantiateIn(Control container)
		{
			// Create the content for the different row types.
			switch (this.templateType)
			{
				case DataControlRowType.Header:
					// Create the controls to put in the header
					// section and set their properties.
					var lc = new Label { Text = this.groupId, CssClass = "VertiColumn", ViewStateMode = ViewStateMode.Disabled };

					container.Controls.Add(lc);
					break;

				case DataControlRowType.DataRow:
					var chkGroup = new FMCheckBox { ID = "c" + this.groupIndex };
					chkGroup.Attributes.Add("OV", "0");
					chkGroup.DataBinding += this.ChkGroupDataBinding;
					chkGroup.InputAttributes["class"] = "UserGroupMapCheckBox determinate";

					// Add the controls to the Controls collection
					// of the container.
					container.Controls.Add(chkGroup);

                    // add a hidden input field to hold the triState flag
                    // this field will be set to true if the checkbox control
                    // is in tristate mode and false if the checkbox control
                    // is Checked or Unchecked
			        if (this.ActiveDirectoryUser)
			        {
			            var hiddenInput = new HtmlInputHidden();
			            hiddenInput.ID = chkGroup.ID + "_triState";
			            hiddenInput.Value = "false";
			            container.Controls.Add(hiddenInput);
			        }
			        break;
			}
		}

		private void ChkGroupDataBinding(object sender, EventArgs e)
		{
			// Get the Label control to bind the value. The Label control
			// is contained in the object that raised the DataBinding 
			// event (the sender parameter).
			var checkBoxControl = (CheckBox)sender;

			// Get the GridViewRow object that contains the Label control. 
			var row = (DataGridItem)checkBoxControl.NamingContainer;

			var hasPermissions = DataBinder.Eval( row.DataItem, this.GroupGuid.ToString());

			var UserGuid = new Guid(row.Cells[3].Text);

            var ov = string.Empty;

            // -1 means no permissions/not available
            if (hasPermissions is DBNull || (int)hasPermissions == -1)
		    {
                // not assigned to the group
                checkBoxControl.Visible = false;

		        // No need to spend space for this if the control is invalid for this cell.
		        checkBoxControl.Attributes.Remove("OV");
                checkBoxControl.EnableViewState = false;
		    }
		    else if ((int)hasPermissions == 2)
		    {
                // 2 means deny flag has been set
                // this should never happen for non AD users
		        if (this.ActiveDirectoryUser)
		        {
		            ov = "2";
		        }
		    }
		    else
		    {
                // it should be either a 1 or 0 at this point
                // 1 means assigned (checked)
                // 0 means unassigned (unchecked)
                checkBoxControl.Checked = Convert.ToBoolean(hasPermissions);
                ov = checkBoxControl.Checked ? "1" : "0";
		    }

            // only save the original value (OV) if we have set it.
            if (string.IsNullOrWhiteSpace(ov) == false)
		    {
                checkBoxControl.Attributes["OV"] = ov;
            }
		    checkBoxControl.Attributes.Add("onClick", "javascript:permissionCheckBoxClick(this);");
		    checkBoxControl.Enabled = (this.HasViewOperateOnly && UserGuid == Guids.UserAdminGuid) ? false : true;
        }
    }
}
