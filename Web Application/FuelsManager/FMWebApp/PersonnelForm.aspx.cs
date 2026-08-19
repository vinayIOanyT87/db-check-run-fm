// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonnelForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for PersonnelForm.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Drawing;
    using System.Globalization;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Interfaces;
    using FMBusinessObjects.ServiceRequests;
    using FMBusinessObjects.UtilityObjects;

    using FMControls;

    using FMEnterpriseManagementBusinessObjects.BusinessInterfaces;
    using FMEnterpriseManagementBusinessObjects.ChannelFactories;

    using FMSynchronizationCommon;

    /// <summary>
	/// Code behind class for personnel form page.
	/// </summary>
	public partial class PersonnelForm : FMFormBaseAjax, IEntityDiscovery, IMenuDiscovery
	{
		#region Constants and Fields

		/// <summary>
		/// Session storage key
		/// </summary>
		private const string PersonnelFindString = "Personnel.FindString";

		/// <summary>
		/// Session storage key
		/// </summary>
		private const string SortDirection = "Personnel.SortDirection";

		/// <summary>
		/// Session storage key
		/// </summary>
		private const string SortExpression = "Personnel.SortExpression";

        /// <summary>
        /// Session enterprise inclusion key
        /// </summary>
        private const string EnterpriseSearch = "Personnel.IncludeEnterprise";

		/// <summary>
		/// Stores the current search string entered by the user
		/// </summary>
		private string searchString;

        private bool isEnterprise;

        /// <summary>
        /// Retain the state of the Show Hidden checkbox
        /// </summary>
        private bool SessionPersonnelSummaryShowHiddenChecked
        {
            get
            {
                if (this.Session["PersonnelSummaryShowHiddenChecked"] is bool)
                {
                    return (bool)this.Session["PersonnelSummaryShowHiddenChecked"];
                }
                else
                {
                    return false;
                }
            }

            set
            {
                this.Session.Add("PersonnelSummaryShowHiddenChecked", value);
            }
        }

		#endregion

		#region Explicit Interface Properties

		/// <summary>
		/// Gets a value indicating whether [entity assignable].
		/// </summary>
		/// <value>
		///   <c>true</c> if [entity assignable]; otherwise, <c>false</c>.
		/// </value>
		bool IEntityDiscovery.EntityAssignable => true;

        /// <summary>
		/// Gets the type of the entity engine.
		/// </summary>
		/// <value>
		/// The type of the entity engine.
		/// </value>
		Type IEntityDiscovery.EntityEngineType => typeof(IPersonnel);

        /// <summary>
		/// Gets the type of the entity.
		/// </summary>
		/// <value>
		/// The type of the entity.
		/// </value>
		ENTITY_TYPE IEntityDiscovery.EntityType => ENTITY_TYPE.PERSONNEL;

        #endregion

		#region Public Methods and Operators

        /// <summary>
        /// Gets a list of menu items that should be displayed for the current user.
        /// </summary>
        /// <param name="security">
        /// The security object of the current session 
        /// </param>
        /// <param name="siteGroup">
        /// Whether the current logged-in site is a site group 
        /// </param>
        /// <param name="useNewLicenseKey"></param>
        /// <param name="options">
        /// Hardware key options 
        /// </param>
        /// <param name="word1"></param>
        /// <param name="word2"></param>
        /// <returns>
        /// List of menu items to be displayed 
        /// </returns>
        public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1,ushort word2, ushort useNewLicenseKey, uint options)
		{
            if (useNewLicenseKey == 1)
            {

            }
            else
            {
                // Depends Upon Accounting
                if ((options & 0x80100) == 0)
                {
                    return null;
                }
            }

            var items = new List<FMMenuItem>();

			if (!security.HasRight(RIGHT.VIEW_PERSONNEL_DATA)
				&& !security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA)
				&& !security.HasRight(RIGHT.CONFIGURE_TRAINING))
			{
				return null;
			}

			items.Add(
				new FMMenuItem
					{
						MenuItemType = FMMenuItemType.ASSETS_PERSONNEL_PERSONNEL, 
						RootMenuName = "Assets", 
						CategoryName = "Personnel", 
						ItemName = "Personnel", 
						NavigateUrl = "PersonnelForm.aspx", 
						ApplyDataDictionary = ApplyDataDictionary.Apply, 
						SortOrder = 1
					});

			return items;
		}
		#endregion

		#region Explicit Interface Methods

		/// <summary>
		/// Enumerates the entity maps.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="type">The type.</param>
		/// <returns>A collection of entity to site maps.</returns>
		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(SecurityClass security, ENTITY_ASSIGNMENT_TYPE type)
		{
			PersonCollectionClass personCollection;

			if (type == ENTITY_ASSIGNMENT_TYPE.UNDELEGATED)
			{
				personCollection = FMChannelHelper.MakeCall<IPersonnel, PersonCollectionClass>(
																					  x =>
																					  x.EnumerateUndelegated(security)
																				);
			}
			else
			{
				personCollection = FMChannelHelper.MakeCall<IPersonnel, PersonCollectionClass>(
																					  x =>
																					  x.Enumerate(security)
																				);
			}
			
			var entityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			foreach (var person in personCollection)
			{
				if (type == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if (security.SiteGuid == person.SiteGuid)
					{
						continue;
					}

					if (security.LoginSiteGuid != person.SiteGuid)
					{
						continue;
					}
				}
				else
				{
					// For entity types supporting Record Versioning, assignments can be cascaded, 
					// irrespective of whether Record Versioning is turned on or off.
					if ((security.SiteGuid != person.SiteGuid) && (security.SiteGuid != person.AssignedToSiteGuid))
					{
						continue;
					}
				}

				//The EntityToSiteMap references Product records by their MasterRecordGuids instead of their actual ProductGuids.
				var entityToSiteMap = new EntityToSiteMapClass(person) { IdentityGuid = person.MasterRecordGuid };
				entityToSiteMapCollection.Add(entityToSiteMap);
			}

			return entityToSiteMapCollection;
		}

		/// <summary>
		/// Gets the identity GUID.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="personId">The personId.</param>
		/// <returns>The identity guid of the specified person class.</returns>
		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string personId)
		{
			return FMChannelHelper.MakeCall<IPersonnel, Guid>(x => x.GetGuidByID(security, personId));
		}

		/// <summary>
		/// Sets the site GUID.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="guid">The GUID.</param>
		/// <param name="siteGuid">The site GUID.</param>
		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid siteGuid)
		{
			FMChannelHelper.MakeCall<IPersonnel>(
				personnel =>
					{
						var person = personnel.Get(security, guid);
						person.SiteGuid = siteGuid;
						person.MasterSiteGuid = siteGuid;
						personnel.Modify(security, DATA_TYPE.CONFIG, person);
					});
		}
		#endregion

		#region Methods
		/// <summary>
		/// Fs the ind all BTN on click.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void FIndAllBtnOnClick(object sender, EventArgs e)
		{
			this.Session.Remove(PersonnelFindString);
			this.searchString = null;
			this.FindTextBox.Text = string.Empty;
			this.PersonnelDataGrid.PageIndex = 0;
			this.UpdateView();
		}

		/// <summary>
		/// Finds the BTN on click.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void FindBtnOnClick(object sender, EventArgs e)
		{
			if ((this.FindTextBox == null) || (this.FindTextBox.Text.Length < 1))
			{
				this.searchString = null;
				this.Session.Remove(PersonnelFindString);
			}
			else
			{
				this.searchString = this.FindTextBox.Text.ToUpper();
				this.FindTextBox.Text = this.searchString;
				this.Session.Add(PersonnelFindString, this.searchString);
			}

            this.Session[EnterpriseSearch] = false;

            // Update the page with the new contents.
            this.PersonnelDataGrid.PageIndex = 0;
			this.UpdateView();
		}

        protected void SearchEnterpriseBtnOnClick(object sender, EventArgs e)
        {
            if ((this.FindTextBox == null) || (this.FindTextBox.Text.Length < 1))
            {
                this.searchString = null;
                this.Session.Remove(PersonnelFindString);
            }
            else
            {
                this.searchString = this.FindTextBox.Text.ToUpper();
                this.FindTextBox.Text = this.searchString;
                this.Session.Add(PersonnelFindString, this.searchString);
            }

            this.Session[EnterpriseSearch] = true;

            // Update the page with the new contents.
            this.PersonnelDataGrid.PageIndex = 0;
            this.UpdateView();
        }

        /// <summary>
        /// Raises the <see cref="OnInit"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnInit(EventArgs e)
		{
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

			    this.isEnterprise = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsMultipleSiteKey());

				if (!this.Security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA))
				{
					this.AddButton.Enabled = false;
					this.AddButton2.Enabled = false;
				}

				if (!this.Page.IsPostBack)
				{
                    this.Session[EnterpriseSearch] = false;

                    if (this.Session["PersonRole"] == null)
					{
						this.Session["PersonRole"] = PERSON_ROLE.MAX_PERSON_ROLE;
					}

					for (var type = PERSON_ROLE.LOADER_ROLE; type <= PERSON_ROLE.MAX_PERSON_ROLE; type++)
					{
						var newRoleItem = new ListItem(PersonRoleMapClass.RoleID(type), ((int)type).ToString(CultureInfo.InvariantCulture));

						this.PersonRoleDropDownList.Items.Add(newRoleItem);
						if (this.Session["PersonRole"] != null && (PERSON_ROLE)this.Session["PersonRole"] == type)
						{
							this.PersonRoleDropDownList.SelectedIndex = this.PersonRoleDropDownList.Items.Count - 1;
						}
					}

					this.Session["PersonRole"] = (PERSON_ROLE)Convert.ToInt32(this.PersonRoleDropDownList.SelectedItem.Value);

					if (this.Session["PersonnelPage"] != null)
					{
						this.PersonnelDataGrid.PageIndex = (int)this.Session["PersonnelPage"];
						this.Session.Remove("PersonnelPage");
					}

				    this.ShowHiddenCheckBox.Checked = this.SessionPersonnelSummaryShowHiddenChecked;

				    if (this.Session[SortExpression] == null)
				    {
				        this.Session[SortExpression] = "PersonID";
				    }

				    if (this.Session[SortDirection] == null)
				    {
				        this.Session[SortDirection] = "ASC";
				    }

					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Persons the role drop down list selected index changed.  This method handles the personnel dropdown list event. 
		/// It will set the search string if there is a value in the find text box.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void PersonRoleDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				this.Session["PersonRole"] = (PERSON_ROLE)Convert.ToInt32(this.PersonRoleDropDownList.SelectedItem.Value);

				if ((this.FindTextBox == null) || (this.FindTextBox.Text.Length < 1))
				{
					this.searchString = null;
				}
				else
				{
					this.searchString = this.FindTextBox.Text.ToUpper();
					this.FindTextBox.Text = this.searchString;
				}

				this.PersonnelDataGrid.PageIndex = 0;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the RowCommand event of the PersonnelDataGrid control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.CommandEventArgs"/> instance containing the event data.</param>
		protected void PersonnelDataGridRowCommand(object sender, CommandEventArgs e)
		{
			string redirectUrl = string.Empty;

			try
			{
                this.GetSecurity();

				if (e.CommandName == "Edit")
				{
					int index = Convert.ToInt32(e.CommandArgument);
				    GridViewRow row = this.PersonnelDataGrid.Rows[index];

					var entityControl = (Literal)row.FindControl("EntityGuidText");
					string entityGuidString = entityControl.Text;
                    var personGuid = Guid.Parse(entityGuidString);
                    var masterRecordGuidString = ((Literal)row.FindControl("MasterRecordGuidText")).Text;
                    var masterRecordGuid = Guid.Parse(masterRecordGuidString);
                    var remoteString = ((Literal)row.FindControl("RemoteText")).Text;
				    var remote = bool.Parse(remoteString);

                    this.Session.Remove("PersonArrayList");
					this.Session.Remove("PersonSelectContextArrayList");

					PersonClass person;

                    if (remote && !this.IsEnterprise)
                    {
                        SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security,
                                                                                                    this.Security.SiteGuid,
                                                                                                    getMemberSites: false,
                                                                                                    getSchedulesAndProcessVariables: false,
                                                                                                    bGetAssociatedAliases: true));

                        if (!string.IsNullOrEmpty(site.EnterpriseUserId))
                        {
                            SecurityLoginRequest sr = new SecurityLoginRequest
                            {
                                UserID = site.EnterpriseUserId,
                                Password = site.EnterprisePassword,
                                SiteID = site.EnterpriseSite
                            };

                            SecurityLoginResponse enterpriseSecurityResponse =
                                EnterpriseManagementChannelHelper
                                    .MakeCall<IClientEnterpriseManagementService, SecurityLoginResponse>(x => x.Login(sr));

                            person = EnterpriseManagementChannelHelper.MakeCall<IClientEnterpriseManagementService, PersonClass>(
                                    x => x.GetPerson(enterpriseSecurityResponse.Security, masterRecordGuid));

                            EnterpriseManagementChannelHelper.MakeCall<IClientEnterpriseManagementService>(
                                x => x.Logout(enterpriseSecurityResponse.Security));
                        }
                        else
                        {
                            // We should not get here; if we don't have enterprise credentials, we shouldn't even have remote equipment retrieved
                            // if we don't have these credentials.
                            throw new Exception("Unable to retrieve credentials to access Enterprise system");
                        }
                    }
                    else
                    {
                        person = FMChannelHelper.MakeCall<IPersonnel, PersonClass>(x => x.Get(this.Security, personGuid));
                    }
                    var personArrayList = new ArrayList { Tuple.Create(person, remote) };
                    this.Session["PersonEdit"] = personGuid;
                    this.Session["PersonArrayList"] = personArrayList;
                    this.Session["PersonnelPage"] = this.PersonnelDataGrid.PageIndex;
					redirectUrl = "PersonForm.aspx";
                }
                else if (e.CommandName == "Delete")
				{
					this.GetSecurity();

					// Get Index
					int index = Convert.ToInt32(e.CommandArgument);

					var literalControl = (Literal)this.PersonnelDataGrid.Rows[index].FindControl("EntityGuidText");
					var entityGuid = new Guid(literalControl.Text);

					FMChannelHelper.MakeCall<IPersonnel>(x => x.Purge(this.Security, entityGuid));

					if (this.PersonnelDataGrid.Rows.Count == 1 && this.PersonnelDataGrid.PageIndex > 0)
					{
						this.PersonnelDataGrid.PageIndex--;
					}

					this.UpdateView();
				}
                else if (e.CommandName == "Select")
                {
                    if (!this.isEnterprise)
                    {
                        int index = Convert.ToInt32(e.CommandArgument);
                        GridViewRow row = this.PersonnelDataGrid.Rows[index];

                        // this is for requesting the assignment of enterprise equipment down to this site.
                        // Synchronization would then pull the newly assigned equipment down to the terminal.
                        var masterRecordGuidText = ((Literal)(row.FindControl("MasterRecordGuidText"))).Text;
                        Guid masterRecordGuid = Guid.Parse(masterRecordGuidText);

                        SiteClass site =
                            FMChannelHelper.MakeCall<ISites, SiteClass>(
                                x =>
                                    x.Get(
                                        this.Security,
                                        this.Security.SiteGuid,
                                        getMemberSites: false,
                                        getSchedulesAndProcessVariables: false,
                                        bGetAssociatedAliases: true));

                        if (!string.IsNullOrEmpty(site.EnterpriseUserId))
                        {
                            SecurityLoginRequest sr = new SecurityLoginRequest
                                                      {
                                                          UserID = site.EnterpriseUserId,
                                                          Password = site.EnterprisePassword,
                                                          SiteID = site.EnterpriseSite
                                                      };

                            SecurityLoginResponse enterpriseSecurityResponse =
                                EnterpriseManagementChannelHelper
                                    .MakeCall<IClientEnterpriseManagementService, SecurityLoginResponse>(
                                        x => x.Login(sr));

                            EnterpriseManagementChannelHelper.MakeCall<IClientEnterpriseManagementService>(
                                x =>
                                    x.RequestEnterprisePersonAssignment(
                                        enterpriseSecurityResponse.Security,
                                        this.Security.SiteGuid,
                                        masterRecordGuid));

                            EnterpriseManagementChannelHelper.MakeCall<IClientEnterpriseManagementService>(
                                x => x.Logout(enterpriseSecurityResponse.Security));

                            this.ForceAdHocSync();
                            this.PersonnelDataGrid.SelectedIndex = -1;
                        }
                    }

                    this.UpdateView();
                }
            }
            catch (Exception except)
			{
				this.ErrorHandler(except);
			}

			if (string.IsNullOrEmpty(redirectUrl) == false)
			{
				this.Redirect(redirectUrl);
			}
		}

		protected void PersonnelDataGridPageIndexChanging (object sender, EventArgs e) { 
			this.UpdateView();
		}

        private void ForceAdHocSync()
        {
            // Ad-hoc synchronization should only be fired at the terminal
            if (this.isEnterprise)
            {
                return;
            }

            // Determine which SiteId to use when performing the synchronization request.  
            // This is all dependent on the determine request type.
            var selectedSyncSite =
                FMChannelHelper.MakeCall<ISyncControllerProcessor, SyncSelectedSiteDO>(
                    syncControllerProcessorChannel =>
                    syncControllerProcessorChannel.GetSynchronizationSiteId(
                        this.Security, SYNCREQUESTTYPE.MANUAL));

            FMSyncServiceChannelHelper tmpHelper = new FMSyncServiceChannelHelper();
            var tmpConfig = tmpHelper.CreateChannelFactoryConfigInfo<ISynchronizationServices>();

            FMChannelFactory<ISynchronizationServices> syncServiceFactory =
                new FMChannelFactory<ISynchronizationServices>(tmpConfig);

            Func<ISynchronizationServices, SecurityClass, SyncSelectedSiteDO, byte[], SYNCREQUESTTYPE, bool> callback = (proxy,
                                                                                                             security,
                                                                                                             selectedSite,
                                                                                                             clientCert,
                                                                                                             requestType)
                                                                                                            => proxy.ManuallyInitiate(security, selectedSite, clientCert, requestType);

            // ReSharper disable once RedundantTypeArgumentsOfMethod
            FMChannelHelper.MakeCall<ISynchronizationServices, bool>(syncServiceFactory, channelProxy => callback(channelProxy, this.Security, selectedSyncSite, this.Request.ClientCertificate.Certificate, SYNCREQUESTTYPE.MANUAL));
        }

        /// <summary>
        /// Handles the RowDataBound event of the PersonnelDataGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewRowEventArgs"/> instance containing the event data.</param>
        protected void PersonnelDataGridRowDataBound(object sender, GridViewRowEventArgs e)
		{
			try
			{
				if ( e.Row.RowType == DataControlRowType.DataRow )
				{
                    var deleteButton = (FMDeleteLinkButton)e.Row.FindControl("DeleteButton");
                    // Note that the ID for a FMSelectLinkButton is always "SelectButton"
                    var assignButton = (FMSelectLinkButton)e.Row.FindControl("SelectButton");
                    var editButton = (FMEditLinkButton)e.Row.FindControl("EditButton");
                    var remoteCheckBox = (CheckBox)e.Row.FindControl("RemoteCheckBox");
                    var remote = (bool)(((DataRowView)e.Row.DataItem).Row["Remote"]);
                    var lockedOutCheckBox = (CheckBox)e.Row.FindControl("GlobalLockedOut");
                    var lockedOut = (bool)(((DataRowView)e.Row.DataItem).Row["LockedOut"]);
                    var siteGuid = (Guid)(((DataRowView)e.Row.DataItem).Row["SiteGuid"]);
                    var personnelGuid = (Guid)(((DataRowView)e.Row.DataItem).Row["IdentityGuid"]);
                    var masterRecordGuid = (Guid)(((DataRowView)e.Row.DataItem).Row["MasterRecordGuid"]);

                    if ( deleteButton != null & e.Row != null)
					{
					    if (!this.Security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA) || this.Security.SiteGuid != siteGuid || remote)
					    {
					        deleteButton.Enabled = false;
					    }

					    //Child record versions cannot be created or deleted directly. Their lifetime is controlled by the Entity-To-Site assignment only.
					    if (deleteButton.Enabled
					        && (!personnelGuid.Equals(masterRecordGuid)))
					    {
					        deleteButton.Enabled = false;
					    }

					    deleteButton.CommandArgument = e.Row.RowIndex.ToString(CultureInfo.InvariantCulture);
					}

                    // Edit button can be used to view assigned equipment even if there's no access to change it, so the rules are more permissive
                    // than for delete.  Only requirement is that the user have either view or modify access and the equipment exist on the local server.
                    // The equipment can be owned by the current site or the site group and be assigned down.
                    // 2/8/2019  Edit button should be used even to view details of equipment from the enterprise system; user will be able to subsequently 
                    // request assignment of that equipment from the detail page.
                    if ( editButton != null )
					{
                        if ((!this.Security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
                             && !this.Security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA)))
                        {
                            editButton.Enabled = false;
                        }

                        editButton.CommandArgument = e.Row.RowIndex.ToString( CultureInfo.InvariantCulture );
					}

                    // Assign button can be used to request equipment not yet assigned.  
                    // Requirement is that the user have either modify access and the equipment not exist on the local server.
                    if (assignButton != null)
                    {
                        if (!this.Security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
                             || !remote)
                        {
                            assignButton.Enabled = false;
                        }

                        assignButton.CommandArgument = e.Row.RowIndex.ToString(CultureInfo.InvariantCulture);
                    }

                    if (remoteCheckBox != null)
                    {
                        remoteCheckBox.Checked = remote;
                    }

                    if (lockedOutCheckBox != null)
                    {
                        lockedOutCheckBox.Checked = lockedOut;
                    }

                    // Change the color of the text of hidden personnel to give the user a visual indication that the person is hidden.
                    var rowView = (DataRowView)e.Row.DataItem;
				    if (rowView != null)
                    {
                        DataRowView view = rowView;
                        DateTimeOffset? hiddenDate = view.Row["HiddenDate"] as DateTimeOffset?;
                        if (hiddenDate.HasValue)
                        {
                            e.Row.ForeColor = Color.Red;
                        }
                    }
				}
			}
			catch ( Exception except )
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Personnels the data grid sort.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewSortEventArgs"/> instance containing the event data.</param>
		protected void PersonnelDataGridSort(object sender, GridViewSortEventArgs e)
		{
			var sortExpression = this.Session[SortExpression] as string;
			var sortDirection = this.Session[SortDirection] as string;

			if (e.SortExpression != sortExpression)
			{
				this.Session[SortDirection] = "ASC";
			}
			else
			{
				if (sortDirection == "DESC")
				{
					this.Session[SortDirection] = "ASC";
				}
				else
				{
					this.Session[SortDirection] = "DESC";
				}
			}

			this.Session[SortExpression] = e.SortExpression;

			this.UpdateView();
		}


        /// <summary>
        /// When the user checks or unchecks the Show Hidden checkbox, update the view
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        protected void ShowHiddenCheckBox_OnCheckedChanged(object sender, EventArgs e)
        {
            try
            {

                if (string.IsNullOrEmpty(this.FindTextBox?.Text))
                {
                    this.searchString = null;
                    this.Session.Remove(PersonnelFindString);
                }
                else
                {
                    this.searchString = this.FindTextBox.Text.ToUpper();
                    this.FindTextBox.Text = this.searchString;
                    this.Session.Add(PersonnelFindString, this.searchString);
                }

                this.SessionPersonnelSummaryShowHiddenChecked = this.ShowHiddenCheckBox.Checked;

                // Update the page with the new contents.
                this.PersonnelDataGrid.PageIndex = 0;
                this.UpdateView();
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

		/// <summary>
		/// Handles the Command event of the AddButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.CommandEventArgs"/> instance containing the event data.</param>
		private void AddButtonCommand(object sender, CommandEventArgs e)
		{
			this.Session.Remove("PersonArrayList");
			this.Session.Remove("PersonSelectContextArrayList");

			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(this.Security, this.Security.SiteGuid, false, false, true)
																);
			FMChannelHelper.MakeCall<IPersonnel>(
				personnel =>
					{
						var person = new PersonClass(site)
							{ ShortCardNumber = personnel.GetNextShortCardNumber(this.Security).ToString(CultureInfo.InvariantCulture) };

						var personArrayList = new ArrayList { person };
						this.Session["PersonArrayList"] = personArrayList;
					});

			this.Session["PersonnelPage"] = this.PersonnelDataGrid.PageIndex;
			
			this.Redirect("PersonForm.aspx");
		}

		/// <summary>
		/// Enumerates the persons.
		/// </summary>
		/// <returns>A collection of person objects.</returns>
		private ICollection EnumeratePersons()
		{
			var role = (PERSON_ROLE)Convert.ToInt32(this.PersonRoleDropDownList.SelectedValue);

			var sortExpression = this.Session[SortExpression] as string;
			var sortDirection = this.Session[SortDirection] as string;
			string orderBy = null;

			var limits = new EnumerationLimits();
			int limit = limits.GetLimit(EnumerationLimits.EnumerationOptions.PERSON);

			if (sortExpression != null && sortDirection != null)
			{
				orderBy = sortExpression + " " + sortDirection;
			}

			// Determine whether to retrieve the personnel using a filter or not.  If the user entered in 
			// find string, then use the filter method.
			var personCollection =
				FMChannelHelper.MakeCall<IPersonnel, PersonCollectionClass>(
                x => x.EnumerateByRoleAndFilter(this.Security, role, this.searchString, orderBy, hideHiddenPersonnel: !this.ShowHiddenCheckBox.Checked));


			var personDataTable = new DataTable();

			personDataTable.Columns.Add("SiteGuid", typeof(Guid));
			personDataTable.Columns.Add("IdentityGuid", typeof(Guid));
			personDataTable.Columns.Add("MasterRecordGuid", typeof(Guid));
			personDataTable.Columns.Add("PersonID", typeof(string));
			personDataTable.Columns.Add("FirstName", typeof(string));
			personDataTable.Columns.Add("MiddleName", typeof(string));
			personDataTable.Columns.Add("LastName", typeof(string));
		    
            // HiddenDate is a nullable DateTimeOffset (DateTimeOffset?) but you can't use nullable types in data tables.
            personDataTable.Columns.Add("HiddenDate", typeof(DateTimeOffset));
            personDataTable.Columns.Add("ShortCardNumber", typeof(string));
		    personDataTable.Columns.Add("LockedOut", typeof(bool));
		    personDataTable.Columns.Add("Remote", typeof(bool));

            foreach (var person in personCollection)
			{
				DataRow personDataRow = personDataTable.NewRow();

				personDataRow["SiteGuid"] = person.SiteGuid;
				personDataRow["IdentityGuid"] = person.IdentityGuid;
				personDataRow["MasterRecordGuid"] = person.MasterRecordGuid;
				personDataRow["PersonID"] = person.ID;
				personDataRow["FirstName"] = person.FirstName;
				personDataRow["MiddleName"] = person.MiddleName;
				personDataRow["LastName"] = person.LastName;
                personDataRow["HiddenDate"] = person.HiddenDate ?? (object)DBNull.Value;
                personDataRow["ShortCardNumber"] = person.ShortCardNumber;
			    personDataRow["LockedOut"] = person.LockedOut;
			    personDataRow["Remote"] = false;

                personDataTable.Rows.Add(personDataRow);
			}

		    if ((bool)this.Session[EnterpriseSearch] && !this.isEnterprise)
		    {
                SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security,
                                                                                            this.Security.SiteGuid,
                                                                                            getMemberSites: false,
                                                                                            getSchedulesAndProcessVariables: false,
                                                                                            bGetAssociatedAliases: true));

		        if (!string.IsNullOrEmpty(site.EnterpriseUserId))
		        {
		            SecurityLoginRequest sr = new SecurityLoginRequest
		                                      {
		                                          UserID = site.EnterpriseUserId,
		                                          Password = site.EnterprisePassword,
		                                          SiteID = site.EnterpriseSite
		                                      };

		            SecurityLoginResponse enterpriseSecurityResponse =
		                EnterpriseManagementChannelHelper.MakeCall<IClientEnterpriseManagementService, SecurityLoginResponse>(
		                    x => x.Login(sr));

		            personCollection =
		                EnterpriseManagementChannelHelper.MakeCall<IClientEnterpriseManagementService, PersonCollectionClass>(
		                    x =>
		                        x.EnumeratePersonnelByRoleAndFilter(
		                            enterpriseSecurityResponse.Security, role, this.searchString, orderBy, hideHiddenPersonnel: !this.ShowHiddenCheckBox.Checked));

		            EnterpriseManagementChannelHelper.MakeCall<IClientEnterpriseManagementService>(
		                x => x.Logout(enterpriseSecurityResponse.Security));

                    foreach (var person in personCollection)
                    {
                        if (personDataTable.Select($"MasterRecordGuid = '{person.MasterRecordGuid}'").Length > 0)
                        {
                            continue;
                        }

                        DataRow personDataRow = personDataTable.NewRow();

                        personDataRow["SiteGuid"] = person.SiteGuid;
                        personDataRow["IdentityGuid"] = person.IdentityGuid;
                        personDataRow["MasterRecordGuid"] = person.MasterRecordGuid;
                        personDataRow["PersonID"] = person.ID;
                        personDataRow["FirstName"] = person.FirstName;
                        personDataRow["MiddleName"] = person.MiddleName;
                        personDataRow["LastName"] = person.LastName;
                        personDataRow["HiddenDate"] = person.HiddenDate ?? (object)DBNull.Value;
                        personDataRow["ShortCardNumber"] = person.ShortCardNumber;
                        personDataRow["LockedOut"] = person.LockedOut;
                        personDataRow["Remote"] = true;

                        personDataTable.Rows.Add(personDataRow);
                    }
                }
            }
		    else
		    {
                // Only do limit check for local personnel list.  Enterprise could easily have many many personnel.
                if (personCollection.Count >= limit && limit > 0)
                {
                    this.lblWarning.Text = "Results limited to first " + limit + " records.  Use filters to narrow search.";
                    this.lblWarning.Visible = true;
                }
                else
                {
                    this.lblWarning.Visible = false;
                }
            }

		    DataView personDataView = new DataView(personDataTable);
            if (this.Session[SortExpression] != null && this.Session[SortDirection] != null)
            {
                personDataView.Sort = (string)this.Session[SortExpression] + " " + (string)this.Session[SortDirection];
            }

            return personDataView;
		}

        // ReSharper disable once InconsistentNaming
		protected void PersonnelDataGrid_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				if (e.Row.RowIndex == 0)
					e.Row.Style.Add("height", "43px");
				e.Row.Style.Add("vertical-align", "bottom");
			}
        }

        protected void PageSizeDropDown_SelectedIndexChanged(object source, EventArgs e)
        {
            this.UpdateView();
        }

        /// <summary>
        ///   Required method for Designer support - do not modify
        ///   the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
		{
			this.FindBtn.Click += this.FindBtnOnClick;
			this.ShowAllBtn.Click += this.FIndAllBtnOnClick;
			this.AddButton.Command += this.AddButtonCommand;
			this.AddButton2.Command += this.AddButtonCommand;
			this.PersonnelDataGrid.RowCommand += this.PersonnelDataGridRowCommand;
			this.PersonnelDataGrid.RowDataBound += this.PersonnelDataGridRowDataBound;
			this.PersonnelDataGrid.Sorting += this.PersonnelDataGridSort;
			this.PersonnelDataGrid.PageIndexChanging += this.PersonnelDataGridPageIndexChanging;
			this.PersonRoleDropDownList.SelectedIndexChanged += this.PersonRoleDropDownListSelectedIndexChanged;
		}

		/// <summary>
		///   This method will update the view of the personnel page.
		/// </summary>
		private void UpdateView()
		{
            bool defense = FMChannelHelper.MakeCall<IHardwareKey, bool>(
                                                                     x =>
                                                                     x.IsDescKey()
                                                                );

            // Locate the previous search string from the session. Set the set
            // string if found.
            if (this.Session[PersonnelFindString] != null)
			{
				this.FindTextBox.Text = this.Session[PersonnelFindString] as string;
				this.searchString = this.Session[PersonnelFindString] as string;
			}

			ICollection persons = this.EnumeratePersons();

            this.StationsFormPageSizeDropDown.SetPageSize(this.PersonnelDataGrid, persons.Count);

            this.PersonnelDataGrid.DataSource = persons;
			this.PersonnelDataGrid.DataBind();
			this.FindTextBox.Text = this.searchString;
			this.FindTextBox.Enabled = true;
		    this.SearchEnterpriseBtn.Visible = !defense; //if in the future we changed the Find button function, we may need to revisit
		    this.PersonnelIDLabel.Visible = !defense;
		    this.PersonnelIDSearchBox.Visible = !defense;
		    this.FirstLabel.Visible = !defense;
		    this.FirstSearchBox.Visible = !defense;
		    this.LastLabel.Visible = !defense;
		    this.LastSearchBox.Visible = !defense;
		    this.ShortCardLabel.Visible = !defense;
		    this.ShortCardSearchBox.Visible = !defense;
		    this.PersonnelDataGrid.Columns[0].Visible = !defense; //Assign
		    this.PersonnelDataGrid.Columns[7].Visible = !defense; //Short Card Number
            this.PersonnelDataGrid.Columns[8].Visible = !defense; //Global Locked Out
        } 
		#endregion
	}
}