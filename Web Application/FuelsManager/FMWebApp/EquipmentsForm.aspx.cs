// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EquipmentsForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the EquipmentsForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Net.Sockets;
    using System.Web;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Interfaces;
    using FMBusinessObjects.ServiceRequests;
    using FMBusinessObjects.UtilityObjects;

    using FMEnterpriseManagementBusinessObjects.BusinessInterfaces;
    using FMEnterpriseManagementBusinessObjects.ChannelFactories;

    using FMSynchronizationCommon;
	using FMControls;

    /// <summary>
    ///    Summary description for EquipmentsForm.
    /// </summary>
    public partial class EquipmentsForm : FMAutoSubmitFormBase, IEntityDiscovery, IMenuDiscovery
	{
		#region Constants and Fields

		protected EquipmentCollectionClass EquipmentCollection;

		private const string SortDirection = "EquipmentsForm.SortDirection";
		private const string SortExpression = "EquipmentsForm.SortExpression";

        private DataTable dt;
		private DataView dv = new DataView();

		private FilterSettings filterSettings;

        private bool isEnterprise;
		#endregion

		#region Explicit Interface Properties

		bool IEntityDiscovery.EntityAssignable => true;

        Type IEntityDiscovery.EntityEngineType => typeof(IEquipments);

        //	#region Private attributes
		//	private string searchString = null;
		//	private const string EQUIPMENT_FIND_STRING = "EquipmentFindString";
		//	#endregion

		ENTITY_TYPE IEntityDiscovery.EntityType => ENTITY_TYPE.EQUIPMENT;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///    Gets a list of menu items that should be displayed for the current user.
        /// </summary>
        /// <param name="security">The security object of the current session</param>
        /// <param name="siteGroup">Whether the current logged-in site is a site group</param>
        /// <param name="useNewLicenseKey">True if we're using an FM10 keys, false if using an FM9 key</param>
        /// <param name="options">Hardware key options from FM9 key</param>
        /// <param name="word1">Licence key options from FM10 key</param>
        /// <param name="word2">Additional licence key options from FM10 key</param>
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
                // Depends Upon Accounting
                if ((options & 0x80100) == 0)
                {
                    return null;
                }
            }

            var menuItems = new List<FMMenuItem>();

			if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) && !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA))
			{
				return null;
			}

            menuItems.Add(
				new FMMenuItem
					{
						MenuItemType = FMMenuItemType.ASSETS_EQUIPMENT_EQUIPMENT,
						RootMenuName = "Assets",
						CategoryName = "Equipment",
						ItemName = "Equipment",
						NavigateUrl = "EquipmentsForm.aspx",
						ApplyDataDictionary = ApplyDataDictionary.Apply,
						SortOrder = 1
					});

			return menuItems;
		}

		#endregion

		#region Explicit Interface Methods
		private EquipmentInfo[] GetEquipmentInfoArray(SecurityClass inSecurity, ENTITY_ASSIGNMENT_TYPE inType)
		{
			EquipmentInfo[] equipmentInfoArray;

			return equipmentInfoArray = (inType == ENTITY_ASSIGNMENT_TYPE.UNDELEGATED) ?
					equipmentInfoArray = FMChannelHelper.MakeCall<IEquipments, EquipmentInfo[]>(x => x.EnumerateInfoUndelegated(inSecurity, true)) :
					equipmentInfoArray = FMChannelHelper.MakeCall<IEquipments, EquipmentInfo[]>(x => x.EnumerateInfo(inSecurity));
		}

		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(SecurityClass security, ENTITY_ASSIGNMENT_TYPE type)
		    {
                EquipmentInfo[] equipmentInfoArray;
				equipmentInfoArray = GetEquipmentInfoArray(security, type);
				var equipment = new EquipmentClass();
				var entityToSiteMapCollection = new EntityToSiteMapCollectionClass();

				foreach (EquipmentInfo equipmentInfo in equipmentInfoArray)
				{
					if ((type.Equals(ENTITY_ASSIGNMENT_TYPE.ASSIGNED) &&
						(security.SiteGuid == equipmentInfo.siteGuid || security.LoginSiteGuid != equipmentInfo.siteGuid))
						|| (security.SiteGuid != equipmentInfo.siteGuid && security.SiteGuid != equipmentInfo.AssignedToSiteGuid))
						if (security.SiteGuid == equipmentInfo.siteGuid)
						{
							continue;
						}
					
						var entityToSiteMap = new EntityToSiteMapClass
						{
							ID = equipmentInfo.ID,
							SiteGuid = equipmentInfo.siteGuid,
							IdentityGuid = equipmentInfo.masterRecordGuid,
							TypeID = equipment.EntityType,
							AssignedFromSiteGuid = equipmentInfo.AssignedFromSiteGuid,
							AssignedFromSiteId = equipmentInfo.AssignedFromSiteId
						};

						entityToSiteMapCollection.Add(entityToSiteMap);
				}
			return entityToSiteMapCollection;
		}

		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string id)
		{
			return FMChannelHelper.MakeCall<IEquipments, Guid>(x => x.GetIdentityGuid(security, id));
		}

		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid siteGuid)
		{
			EquipmentClass equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(equipments => equipments.Get(security, guid));
			equipment.SiteGuid = siteGuid;
			//Automatically extend the Site Ownership change to the children compartments of the equipment
			if ((equipment.CompartmentCollection != null) && (equipment.CompartmentCollection.Count > 0))
			{
				for (int i = 0; i < equipment.CompartmentCollection.Count; i++)
				{
					EquipmentClass compartment = equipment.CompartmentCollection[i];
					compartment.SiteGuid = siteGuid;
					// If the compartment has a meter, change the site that the meter belongs to as well
					if (compartment.Meter.Count > 0)
					{
						foreach (MeterClass meter in compartment.Meter)
						{
							meter.SiteGuid = siteGuid;
						}
					}
				}
			}

			// If the equipment has a meter, change the site that the meter belongs to as well
			if (equipment.Meter.Count > 0)
			{
				foreach (MeterClass meter in equipment.Meter)
				{
					meter.SiteGuid = siteGuid;
				}
			}
			FMChannelHelper.MakeCall<IEquipments>(equipments => equipments.Modify(security, equipment));
		}
		#endregion

		//********************************************************************************************************
		// This method will handled the dropdown change event. It will look at the find text box to see if there
		// is a search string to be used when retrieving the data.
		//********************************************************************************************************

		#region Methods
		protected void EquipmentTypeClassDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				this.filterSettings.EquipmentClass = this.EquipmentTypeClassDropDownList.SelectedItem.Text;
				this.EquipmentsDataGrid.CurrentPageIndex = 0;
				this.Session.Remove("EnumerateDataSet");
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void EquipmentTypeDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				this.filterSettings.EquipmentType = this.EquipmentTypeDropDownList.SelectedItem.Text;
				this.EquipmentsDataGrid.CurrentPageIndex = 0;
				this.Session.Remove("EnumerateDataSet");
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void EquipmentsDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			try
			{
				// if we are editing do not allow a page change
				if (this.EquipmentsDataGrid.EditItemIndex > -1)
				{
					return;
				}
				this.EquipmentsDataGrid.CurrentPageIndex = e.NewPageIndex;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		//*************************************************************************************************
		// This method is called when the find/Filter button is pressed. It will retrieve data from the find
		// text box and set the search string. If there is no data, then the search string is set to null.
		//*************************************************************************************************
		protected void FindBtnOnClick(object sender, EventArgs e)
        {
            // Update the page with the new contents.
            // Locate the previous search string from the session. Set the set
            // string if found.
            PersistFilterSettings();
            this.EquipmentsDataGrid.CurrentPageIndex = 0;
            this.Session["IncludeEnterpriseEquipment"] = false;
            this.Session.Remove("EnumerateDataSet");
            this.UpdateView();
        }

        private void PersistFilterSettings()
        {
            this.filterSettings.FindString = this.FindTextBox.Text;
            this.filterSettings.TrailerID = this.TrailerIDSearchBox.Text;
            this.filterSettings.Product = this.ProductSearchBox.Text;
            this.filterSettings.Company = this.CompanySearchBox.Text;
            this.filterSettings.CompanyEquipmentID = this.CompanyEquipmentIDSearchBox.Text;
        }

        protected string GetCompanyToolTip(DataRow row)
		{
			string toolTip;
			_ = string.IsNullOrEmpty(row["CompanyName"].ToString()) == false ?
					toolTip = row["CompanyName"].ToString() :
					toolTip = row["CompanyID"].ToString();

			_ = string.IsNullOrEmpty(row["CompanyAddress"].ToString()) == false ?
					toolTip += ", " + row["CompanyAddress"] :
					toolTip += ", " + row["CompanyCity"];

			if (string.IsNullOrEmpty(row["CompanyAddress"].ToString()) == false)
			{
				toolTip += ", " + row["CompanyAddress"];
			}
			if (string.IsNullOrEmpty(row["CompanyCity"].ToString()) == false)
			{
				toolTip += ", " + row["CompanyCity"];
			}
			if (string.IsNullOrEmpty(row["CompanyState"].ToString()) == false)
			{
				toolTip += ", " + row["CompanyState"];
			}
			return toolTip;
		}

		//**************************************************************************************************
		// This method is called when the show all button is pressed. It will set the search string to null
		// indicating that we do not want to use the filter on finding companies.  In addition, the find
		// text box is cleared.
		//**************************************************************************************************

		protected void ManagedEquipmentCheckBoxCheckedChanged(object sender, EventArgs e)
		{
			this.filterSettings.ShowManagedEquipmentOnly = this.ManagedEquipmentCheckBox.Checked;
			this.Session.Remove("EnumerateDataSet");
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
                this.filterSettings.FindString = this.FindTextBox.Text;
                this.filterSettings.ShowHidden = this.ShowHiddenCheckBox.Checked;
                this.EquipmentsDataGrid.CurrentPageIndex = 0;
                this.Session.Remove("EnumerateDataSet");
                this.UpdateView();
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
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

		protected void PageSizeDropDownSelectedIndexChanged(object source, EventArgs e)
		{
			this.UpdateView();
		}

		protected void Page_Init(object sender, EventArgs e)
		{
			if (this.IsPostBack == false)
			{
				if (this.Session["EquipmentEdit"] == null && this.Session["EquipmentAdd"] == null)
				{
					this.Session[SortExpression] = "ID";
					this.Session[SortDirection] = "ASC";
				}
			}
		}

		private void HandleNullFilterSettings()
		{
			bool isDefense = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDescKey());

			this.filterSettings = new FilterSettings();
			this.filterSettings.Reset();
			//Managed equipment checkbox always defaults to TRUE if product is DEFENSE.
			this.filterSettings.ShowManagedEquipmentOnly = (isDefense || this.filterSettings.ShowManagedEquipmentOnly);
			this.Session["FilterSettings"] = this.filterSettings;
		}

		private void HandleDoesNotHaveModifyEquipmentData()
		{
			this.AddButton.Enabled = false;
			this.AddButton2.Enabled = false;
		}

		private void SetEquipmentTypeClassDropDownListValues()
		{
			EquipmentTypeCollectionClass equipmentTypeColl = FMChannelHelper.MakeCall<IEquipmentTypes, EquipmentTypeCollectionClass>(x => x.Enumerate(this.Security, null, null));

			this.EquipmentTypeClassDropDownList.Items.Clear();
			this.EquipmentTypeClassDropDownList.DataTextField = "ID";
			this.EquipmentTypeClassDropDownList.DataValueField = "IdentityGuid";
			this.EquipmentTypeClassDropDownList.DataSource = equipmentTypeColl;
			this.EquipmentTypeClassDropDownList.DataBind();
			this.EquipmentTypeClassDropDownList.Items.Insert(0, new ListItem(this.GetTranslatedText("{All}"), Guids.AllFilterGuid.ToString()));
			this.EquipmentTypeClassDropDownList.Items.Insert(1, new ListItem(this.GetTranslatedText("{Unassigned}"), Guid.Empty.ToString()));

			for (var type = EQUIPMENT_TYPE.TRAILER_TYPE; type < EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE; type++)
			{
				if (type == EQUIPMENT_TYPE.COMPARTMENT_TYPE)
				{
					continue;
				}

				var item = new ListItem(EquipmentTypeClass.TypeID(type), ((int)type).ToString(CultureInfo.InvariantCulture));
				this.EquipmentTypeDropDownList.Items.Add(item);
			}

			this.EquipmentTypeDropDownList.Items.Insert(0, new ListItem(this.GetTranslatedText(EquipmentTypeClass.TypeID(EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE)),
																((int)EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE).ToString(CultureInfo.InvariantCulture)));
		}

		private void SetSessionEquipmentsPageValues()
		{
			this.EquipmentsDataGrid.CurrentPageIndex = (int)this.Session["EquipmentsPage"];
			this.Session.Remove("EquipmentsPage");
		}

		private void SetFilterSettingsEquipmentClass()
		{
			this.filterSettings.EquipmentClass = this.GetTranslatedText("{All}");
			this.EquipmentTypeClassDropDownList.SelectByText(this.GetTranslatedText("{All}"));
		}

		private void SetFilterSettingsEquipmentType()
		{
			this.filterSettings.EquipmentType = EquipmentTypeClass.TypeID(EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE);
			this.EquipmentTypeDropDownList.SelectByText(this.filterSettings.EquipmentClass);
		}

		private void SetOtherControlsValues()
		{
			this.ManagedEquipmentCheckBox.Checked = this.filterSettings.ShowManagedEquipmentOnly;
			this.SecondaryStorageCheckBox.Checked = this.filterSettings.ShowSecondStorageOnly;
			this.EquipmentTypeDropDownList.SelectByText(this.filterSettings.EquipmentType);
			this.EquipmentTypeClassDropDownList.SelectByText(this.filterSettings.EquipmentClass);
			this.FindTextBox.Text = this.filterSettings.FindString;
			this.ShowHiddenCheckBox.Checked = this.filterSettings.ShowHidden;
		}

		private void SessionEquipmentEditValue()
		{
			this.Session.Remove("EnumerateDataSet");
		}

		private void HandleNotPostBack()
		{
			this.Session["IncludeEnterpriseEquipment"] = false;
			if (!this.Security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA))
			{
				HandleDoesNotHaveModifyEquipmentData();
			}

			SetEquipmentTypeClassDropDownListValues();
			if (this.Session["EquipmentsPage"] != null)
			{
				SetSessionEquipmentsPageValues();
			}

			if (this.filterSettings.EquipmentClass == "")
			{
				SetFilterSettingsEquipmentClass();
			}

			if (this.filterSettings.EquipmentType == "")
			{
				SetFilterSettingsEquipmentType();
			}

			SetOtherControlsValues();

			if (this.Session["EquipmentEdit"] == null && this.Session["EquipmentAdd"] == null)
			{
				SessionEquipmentEditValue();
			}

			this.UpdateView();
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();
			    this.isEnterprise = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsMultipleSiteKey());

				if (this.Session["FilterSettings"] == null)
				{
					HandleNullFilterSettings();
				}
				this.filterSettings = this.Session["FilterSettings"] as FilterSettings;

                if (!this.Page.IsPostBack)
                {
					HandleNotPostBack();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void SecondaryStorageCheckBoxCheckedChanged(object sender, EventArgs e)
		{
			this.filterSettings.ShowSecondStorageOnly = this.SecondaryStorageCheckBox.Checked;
			this.Session.Remove("EnumerateDataSet");
			this.UpdateView();
		}

		protected void ShowAllBtnOnClick(object sender, EventArgs e)
		{
			this.filterSettings.Reset();
			this.FindTextBox.Text = this.filterSettings.FindString;
			this.EquipmentTypeClassDropDownList.SelectByText(this.GetTranslatedText("{All}"));
			this.EquipmentTypeDropDownList.SelectByText(this.GetTranslatedText("{All}"));
			this.ManagedEquipmentCheckBox.Checked = this.filterSettings.ShowManagedEquipmentOnly;
			this.SecondaryStorageCheckBox.Checked = this.filterSettings.ShowSecondStorageOnly;
			this.EquipmentsDataGrid.CurrentPageIndex = 0;
			this.Session.Remove("EnumerateDataSet");
			this.UpdateView();
		}

        protected void SearchEnterpriseBtnOnClick(object sender, EventArgs e)
        {
			PersistFilterSettings();
			this.Session["IncludeEnterpriseEquipment"] = true;
            this.EquipmentsDataGrid.CurrentPageIndex = 0;
            this.Session.Remove("EnumerateDataSet");
            this.UpdateView();
        }

        private void AddButtonCommand(object sender, CommandEventArgs e)
		{
			this.Session.Remove("EquipmentArrayList");
			this.Session.Remove("EquipmentSelectContextArrayList");

			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security,
				this.Security.SiteGuid,
				getMemberSites: false,
				getSchedulesAndProcessVariables: false,
				bGetAssociatedAliases: true));

			var equipment = new EquipmentClass(site)
			{
				VolumeUnits = site.VolumeUnits,
				TemperatureUnits = site.TemperatureUnits,
				DensityUnits = site.DensityUnits,
				MassUnits = site.MassUnits,
				VolumeDecimalPlaces = site._VolumeDecimalPlaces,
				TemperatureDecimalPlaces = site._TemperatureDecimalPlaces,
				DensityDecimalPlaces = site._DensityDecimalPlaces,
				MassDecimalPlaces = site._MassDecimalPlaces,
				InServiceFlag = true
			};
			equipment.InUse = equipment.InServiceFlag;

			var equipmentArrayList = new ArrayList { Tuple.Create(equipment, false) };
			this.Session["EquipmentArrayList"] = equipmentArrayList;
			this.Session["EquipmentsPage"] = this.EquipmentsDataGrid.CurrentPageIndex;
			this.Session.Remove("EquipmentFormTabIndex");
			this.Redirect("EquipmentForm.aspx");
		}

		private void EquipmentsDataGridItemCommand(object source, DataGridCommandEventArgs e)
		{
		    try
		    {
		        this.GetSecurity();

		        if (e.CommandName == "Edit")
		        {
                    // Note that we can not use the cell using the RemoteCheckbox to determine if the euqipment is 
                    // remote only because that checkbox is disabled, so its "Checked" state always returns false
		            TableCell guidCell = e.Item.Cells[3];
		            TableCell masterGuidCell = e.Item.Cells[25];
		            TableCell remoteCell = e.Item.Cells[26];
		            Guid masterGuid = Guid.Parse(masterGuidCell.Text);
		            bool remote = bool.Parse(remoteCell.Text);

                    Guid equipmentGuid = Guid.Parse(guidCell.Text);
		            try
		            {
		                this.Session.Remove("EquipmentArrayList");
		                this.Session.Remove("EquipmentSelectContextArrayList");

		                EquipmentClass equipment;
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
		                        SecurityLoginResponse enterpriseSecurityResponse = EnterpriseManagementChannelHelper.MakeCall<IClientEnterpriseManagementService, SecurityLoginResponse>(x => x.Login(sr));

		                        equipment = EnterpriseManagementChannelHelper.MakeCall<IClientEnterpriseManagementService, EquipmentClass>(x => x.GetEquipment(enterpriseSecurityResponse.Security, 
												masterGuid));

		                        EnterpriseManagementChannelHelper.MakeCall<IClientEnterpriseManagementService>(x => x.Logout(enterpriseSecurityResponse.Security));
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
		                    equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(x => x.Get(this.Security, equipmentGuid));
		                }
		                var equipmentArrayList = new ArrayList { Tuple.Create(equipment, remote) };
		                this.Session["EquipmentEdit"] = equipmentGuid;
		                this.Session["EquipmentArrayList"] = equipmentArrayList;
		                this.Session["EquipmentsPage"] = this.EquipmentsDataGrid.CurrentPageIndex;
		            }
		            catch (Exception except)
		            {
		                this.ErrorHandler(except);
		                return;
		            }
		            this.Session.Remove("EquipmentFormTabIndex");
		            this.Redirect("EquipmentForm.aspx");
		        }
		        else if (e.CommandName == "Delete")
		        {
					TableCell guidCell = e.Item.Cells[3];
		            Guid equipmentGuid = Guid.Parse(guidCell.Text);

					try
					{
		                if (UsingLoadRack)
						// this clears the meter information from the load rack's cache so that when the meter is actually deleted from
						// the application there won't be any mismatches between the main app and the load rack
						{
							ILoadRackManager loadRackManager = this.GetLoadRackManager();
							loadRackManager.Purge(this.Security, typeof(EquipmentClass), equipmentGuid);
		                }
		            }
		            catch (SocketException socketExcept)
		            {
		                if (socketExcept.ErrorCode != 10061)
		                {
		                    throw;
		                }
		            }

					FMChannelHelper.MakeCall<IEquipments>(x => x.Purge(this.Security, equipmentGuid));
					this.EquipmentsDataGrid.SelectedIndex = -1;

					ArrayList equipmentArrayList = (ArrayList)this.Session["EquipmentArrayList"];

					this.UpdateView();
				}
		        else if (e.CommandName == "Select")
		        {
                    if (!this.isEnterprise)
                    { 
                        // this is for requesting the assignment of enterprise equipment down to this site.
                        // Synchronization would then pull the newly assigned equipment down to the terminal.
                        //
                        //This operation should only happen from the Terminal; at the enterprise this would be done directly.
                        TableCell guidCell = e.Item.Cells[26];
                        var masterRecordGuid = Guid.Parse(guidCell.Text);

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

                            SecurityLoginResponse enterpriseSecurityResponse = EnterpriseManagementChannelHelper.MakeCall<IClientEnterpriseManagementService, 
								SecurityLoginResponse>(x => x.Login(sr));
                            EnterpriseManagementChannelHelper.MakeCall<IClientEnterpriseManagementService>(x => x.RequestEnterpriseEquipmentAssignment(enterpriseSecurityResponse.Security,
                                this.Security.SiteGuid,
                                masterRecordGuid));
                            EnterpriseManagementChannelHelper.MakeCall<IClientEnterpriseManagementService>(x => x.Logout(enterpriseSecurityResponse.Security));

                            this.ForceAdHocSync();
                        }
                    }
                    this.UpdateView();
                }
            }
		    catch (Exception except)
		    {
		        this.ErrorHandler(except);
		    }
		}

        private void ForceAdHocSync()
        {
            // We should only force the sync on a terminal, not at the enterprise
            if (this.isEnterprise)
            {
                return;
            }

            // Determine which SiteId to use when performing the synchronization request.  
            // This is all dependent on the determine request type.
            var selectedSyncSite = FMChannelHelper.MakeCall<ISyncControllerProcessor, SyncSelectedSiteDO>(syncControllerProcessorChannel => 
				syncControllerProcessorChannel.GetSynchronizationSiteId(this.Security, SYNCREQUESTTYPE.MANUAL));

            FMSyncServiceChannelHelper tmpHelper = new FMSyncServiceChannelHelper();
            var tmpConfig = tmpHelper.CreateChannelFactoryConfigInfo<ISynchronizationServices>();

            FMChannelFactory<ISynchronizationServices> syncServiceFactory = new FMChannelFactory<ISynchronizationServices>(tmpConfig);

            Func<ISynchronizationServices, SecurityClass, SyncSelectedSiteDO, byte[], SYNCREQUESTTYPE, bool> callback = (proxy,
				security,
				selectedSite,
				clientCert,
				requestType) => proxy.ManuallyInitiate(security, selectedSite, clientCert, requestType);

            // ReSharper disable once RedundantTypeArgumentsOfMethod
            FMChannelHelper.MakeCall<ISynchronizationServices, bool>(syncServiceFactory, 
				channelProxy => callback(channelProxy, 
				this.Security, selectedSyncSite, this.Request.ClientCertificate.Certificate, SYNCREQUESTTYPE.MANUAL));
        }

		private void UpdateTankerTypeDeleteButtonText(FMDeleteLinkButton inDeleteButton, int inIndex)
		{
			if (dv.Table.Rows[inIndex].ItemArray[25].GetType().IsInstanceOfType("string"))
			{
				int tankerType = (int)dv.Table.Rows[inIndex].ItemArray[26];
				int meterCount = (int)dv.Table.Rows[inIndex].ItemArray[28];
				string equipMentID = this.dv[inIndex].Row.ItemArray.GetValue(2).ToString();

				if (tankerType == (int)EQUIPMENT_TYPE.TANKER_TYPE)
                {
					if (meterCount == 0)
					{
						inDeleteButton.ConfirmationText = "Are you sure you wish to delete " + equipMentID + "?";
						// add the onClick handler in case this method gets called after a postback has occurred
						// without this the default confirmation text will be output
						inDeleteButton.Attributes.Add("onClick", "return confirm(\"" + inDeleteButton.ConfirmationText + "\");");
					}
					if (meterCount == 1)
					{
						inDeleteButton.ConfirmationText = "Are you sure you wish to delete " + equipMentID + "? " + equipMentID + " has 1 meter attached to it. Deleting " + equipMentID + " will also delete the meter.";
						// add the onClick handler in case this method gets called after a postback has occurred
						// without this the default confirmation text will be output
						inDeleteButton.Attributes.Add("onClick", "return confirm(\"" + inDeleteButton.ConfirmationText + "\");");
					}
					if (meterCount == 2)
					{
						inDeleteButton.ConfirmationText = "Are you sure you wish to delete " + equipMentID + "? " + equipMentID + " has 2 meters attached to it. Deleting " + equipMentID + " will also delete the meters.";
						// add the onClick handler in case this method gets called after a postback has occurred
						// without this the default confirmation text will be output
						inDeleteButton.Attributes.Add("onClick", "return confirm(\"" + inDeleteButton.ConfirmationText + "\");");
					}
				}

			}
		}

		private void HandleDeleteButtonNotNull(DataGridItemEventArgs inE, FMDeleteLinkButton inDeleteButton, bool inRemote, DataView inDataView, int inIndex)
		{
			TableCell siteGuidCell = inE.Item.Cells[2];
			if (!this.Security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA) || this.Security.SiteGuid != Guid.Parse(siteGuidCell.Text) || inRemote)
			{
				inDeleteButton.Enabled = false;
			}

			//Child record versions cannot be created or deleted directly. Their lifetime is controlled by the Entity-To-Site assignment only.
			if (inDeleteButton.Enabled && (!inDataView.Table.Rows[inIndex]["EquipmentGuid"].Equals(inDataView.Table.Rows[inIndex]["_MasterRecordGuid"])))
			{
				inDeleteButton.Enabled = false;
			}

			UpdateTankerTypeDeleteButtonText(inDeleteButton, inIndex);
		}

		private void HandleEditButtonNotNull(LinkButton inEditButton)
		{
			if (!this.Security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA) && !this.Security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA))
			{
				inEditButton.Enabled = false;
			}
		}

		private void HandleAssignButtonNotNull(bool inRemote, LinkButton inassignButton)
		{
			if (!this.Security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA) || !inRemote)
			{
				inassignButton.Enabled = false;
			}
		}

		private void HandleOtherAssignments(string inCompanyToolTip, Label inCompanyLabel, DataView inDataView,
			int inIndex, DataGridItemEventArgs inE, CheckBox inRemoteCheckBox, bool inRemote, CheckBox inLockedOutCheckBox)
		{
			inCompanyToolTip = this.GetCompanyToolTip(inDataView.Table.Rows[inIndex]);

			inCompanyLabel = (Label)inE.Item.FindControl("CompanyLabel");
			if (inCompanyLabel != null)
			{
				inCompanyLabel.Text = HttpUtility.HtmlEncode(inCompanyLabel.Text);
				inCompanyLabel.ToolTip = inCompanyToolTip;
			}

			if (inRemoteCheckBox != null)
			{
				inRemoteCheckBox.Checked = inRemote;
			}

			if (inLockedOutCheckBox != null)
			{
				string lockedOut = (string)(((DataRowView)inE.Item.DataItem).Row["LockedOut"]);
				inLockedOutCheckBox.Checked = lockedOut == "Yes";
			}

			// Change the color of the text of hidden products to give the user a visual indication that the product is hidden.
			var view = inE.Item.DataItem as DataRowView;
			DateTimeOffset? hiddenDate = view?.Row["HiddenDate"] as DateTimeOffset?;
			if (hiddenDate != null)
			{
				inE.Item.ForeColor = System.Drawing.Color.Red;
			}
		}

		private void EquipmentsDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemIndex != -1)
			{
				int index = this.EquipmentsDataGrid.CurrentPageIndex * this.EquipmentsDataGrid.PageSize + e.Item.ItemIndex;
				var dataView = (DataView)this.EquipmentsDataGrid.DataSource;

				FMDeleteLinkButton deleteLinkButton = (FMDeleteLinkButton)e.Item.FindControl("DeleteButton");
                // Note that the ID for a FMSelectLinkButton is always "SelectButton"
                var assignButton = (LinkButton)e.Item.FindControl("SelectButton");
                var editButton = (LinkButton)e.Item.FindControl("EditButton");
                var remoteCheckBox = (CheckBox)e.Item.FindControl("RemoteCheckBox");
                var remote = (bool)(((DataRowView)e.Item.DataItem).Row["Remote"]);
			    var lockedOutCheckBox = (CheckBox)e.Item.FindControl("GlobalLockedOut");
                var lockedOut = (string)(((DataRowView)e.Item.DataItem).Row["LockedOut"]);

                if (deleteLinkButton != null)
				{
                    HandleDeleteButtonNotNull(e, deleteLinkButton, remote, dataView, index);
                }

                // Edit button can be used to view assigned equipment even if there's no access to change it, so the rules are more permissive
                // than for delete.  Only requirement is that the user have either view or modify access and the equipment exist on the local server.
                // The equipment can be owned by the current site or the site group and be assigned down.
                // 2/8/2019  Edit button should be used even to view details of equipment from the enterprise system; user will be able to subsequently 
                // request assignment of that equipment from the detail page.
                if (editButton != null)
			    {
					HandleEditButtonNotNull(editButton);
			    }

                // Assign button can be used to request equipment not yet assigned.  
                // Requirement is that the user have either modify access and the equipment not exist on the local server.
                if (assignButton != null)
                {
					HandleAssignButtonNotNull(remote, assignButton);
                }

                string companyToolTip = this.GetCompanyToolTip(dataView.Table.Rows[index]);
				var companyLabel = (Label)e.Item.FindControl("CompanyLabel");

				HandleOtherAssignments(companyToolTip, companyLabel, dataView, index, e, remoteCheckBox, remote, lockedOutCheckBox);
			}
		}

		private void EquipmentsDataGridSortCommand(object source, DataGridSortCommandEventArgs e)
		{
			try
			{
				var sortExpression = this.Session[SortExpression] as string;
				var sortDirection = this.Session[SortDirection] as string;

				if (e.SortExpression != sortExpression)
				{
					this.Session[SortExpression] = e.SortExpression;
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
				this.EquipmentsDataGrid.CurrentPageIndex = 0;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.EquipmentsDataGrid.ItemCommand += this.EquipmentsDataGridItemCommand;
			this.EquipmentsDataGrid.SortCommand += this.EquipmentsDataGridSortCommand;
			this.EquipmentsDataGrid.PageIndexChanged += this.EquipmentsDataGridPageIndexChanged;
			this.EquipmentsDataGrid.ItemDataBound += this.EquipmentsDataGridItemDataBound;

			var limits = new EnumerationLimits();
			int pageLimit = limits.GetLimit(EnumerationLimits.EnumerationOptions.EQUIPMENT);
			this.EquipmentSummaryPageSizeDropDown.SetLimit(pageLimit);
			this.EquipmentsDataGrid.PageSize = pageLimit;


			this.AddButton.Command += this.AddButtonCommand;
			this.AddButton2.Command += this.AddButtonCommand;
		}

		private DataSet HandleIncludeEnterpriseEquipment(bool inDefense)
		{
			Guid selectedEquipmentTypeClassGuid = Guid.Parse(this.EquipmentTypeClassDropDownList.SelectedValue);
			var equipmentType = (EQUIPMENT_TYPE)Convert.ToInt32(this.EquipmentTypeDropDownList.SelectedValue);
			string findString = this.FindTextBox.Text.Trim().ToUpper();

			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security,
								this.Security.SiteGuid,
								getMemberSites: false,
								getSchedulesAndProcessVariables: false,
								bGetAssociatedAliases: true));

			DataSet ds = new DataSet();

			if (!string.IsNullOrEmpty(site.EnterpriseUserId))
			{
				SecurityLoginRequest sr = new SecurityLoginRequest
				{
					UserID = site.EnterpriseUserId,
					Password = site.EnterprisePassword,
					SiteID = site.EnterpriseSite
				};

				SecurityLoginResponse enterpriseSecurityResponse = EnterpriseManagementChannelHelper.MakeCall<IClientEnterpriseManagementService,
					SecurityLoginResponse>(x => x.Login(sr));

				var limits = new EnumerationLimits();
				int limit = limits.GetLimit(EnumerationLimits.EnumerationOptions.EQUIPMENT);

				ds = EnterpriseManagementChannelHelper.MakeCall<IClientEnterpriseManagementService, DataSet>(x => x.EnumerateEquipmentDataSet(enterpriseSecurityResponse.Security,
						this.ManagedEquipmentCheckBox.Checked,
						this.SecondaryStorageCheckBox.Checked,
						selectedEquipmentTypeClassGuid,
						equipmentType,
						this.GetTranslatedText("{Unassigned}"),
						findString,
						inDefense,
						!this.ShowHiddenCheckBox.Checked,
						limit));

				EnterpriseManagementChannelHelper.MakeCall<IClientEnterpriseManagementService>(x => x.Logout(enterpriseSecurityResponse.Security));

				foreach (DataRow dr in ds.Tables[0].Rows)
				{

					if (this.dt.Select($"_MasterRecordGuid = '{((Guid)dr["_MasterRecordGuid"])}'").Length > 0)
					{
						continue;
					}

					DataRow newRow = this.dt.NewRow();
					newRow["SiteGuid"] = dr["SiteGuid"];
					newRow["EquipmentGuid"] = dr["EquipmentGuid"];
					newRow["ID"] = dr["ID"];
					newRow["_MasterRecordGuid"] = dr["_MasterRecordGuid"];
					newRow["Volume"] = dr["Volume"];
					newRow["QCDate"] = dr["QCDate"];
					newRow["ReturnToServiceDate"] = dr["ReturnToServiceDate"];
					newRow["InServiceFlag"] = dr["InServiceFlag"];
					newRow["LockedOut"] = dr["LockedOut"];
					newRow["Capacity"] = dr["Capacity"];
					newRow["VolumeUnit"] = dr["VolumeUnit"];
					newRow["CompanyEquipmentID"] = dr["CompanyEquipmentID"];
					newRow["Company"] = dr["Company"];
					newRow["ProductID"] = dr["ProductID"];
					newRow["HiddenDate"] = dr["HiddenDate"];
					newRow["CompanyName"] = dr["CompanyName"];
					newRow["CompanyID"] = dr["CompanyID"];
					newRow["CompanyAddress"] = dr["CompanyAddress"];
					newRow["CompanyCity"] = dr["CompanyCity"];
					newRow["CompanyState"] = dr["CompanyState"];
					newRow["SerialNumber"] = dr["SerialNumber"];
					newRow["Description"] = dr["Description"];
					newRow["Make"] = dr["Make"];
					newRow["Model"] = dr["Model"];
					newRow["Year"] = dr["Year"];
					newRow["EqTypeName"] = dr["EqTypeName"];	// 25
					newRow["LookupEquipmentTypeIndex"] = dr["LookupEquipmentTypeIndex"];	// 26
					newRow["FuelCardID"] = dr["FuelCardID"];	// 27
					newRow["MeterCount"] = dr["MeterCount"];	// 28
					newRow["Remote"] = true;	// 29
					this.dt.Rows.Add(newRow);
				}
			}

			return ds;
		}

		private void UpdateView()
		{
			this.PrepareDataTable();

			bool defense = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDescKey());
			bool tfmd = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsTFMDKey());

			this.EquipmentsDataGrid.Columns[0].Visible = !defense; //Assign button
			this.EquipmentsDataGrid.Columns[6].Visible = defense; //QC Due date
			this.EquipmentsDataGrid.Columns[7].Visible = defense; //Return to Service Date
			this.EquipmentsDataGrid.Columns[8].Visible = defense && !tfmd; //In Service Flag
			this.EquipmentsDataGrid.Columns[9].Visible = tfmd; // LockedOut
			this.EquipmentsDataGrid.Columns[10].Visible = !defense; //Capacity
			this.EquipmentsDataGrid.Columns[11].Visible = !defense; //Units
			this.EquipmentsDataGrid.Columns[12].Visible = !defense; //Company Equipment ID 
			this.EquipmentsDataGrid.Columns[15].Visible = !defense; //TAS's Global (or Terminal)Locked Out

			this.TrailerIDSearchBox.Visible = !defense;
			this.TrailerIDLabel.Visible = !defense;
			this.ProductSearchBox.Visible = !defense;
			this.ProductLabel.Visible = !defense;
			this.CompanySearchBox.Visible = !defense;
			this.CompanyLabel.Visible = !defense;
			this.CompanyEquipmentIDSearchBox.Visible = !defense;
			this.CompanyEquipmentIDLabel.Visible = !defense;
			this.SearchEnterpriseBtn.Visible = !defense;


			Guid selectedEquipmentTypeClassGuid = Guid.Parse(this.EquipmentTypeClassDropDownList.SelectedValue);
			var equipmentType = (EQUIPMENT_TYPE)Convert.ToInt32(this.EquipmentTypeDropDownList.SelectedValue);
			string findString = this.FindTextBox.Text.Trim().ToUpper();

			DataSet ds = new DataSet();

			if ((bool)this.Session["IncludeEnterpriseEquipment"] && !this.isEnterprise)
			{
				ds = this.HandleIncludeEnterpriseEquipment(defense);
			}
			else
			{
				var limits = new EnumerationLimits();
				int limit = limits.GetLimit(EnumerationLimits.EnumerationOptions.EQUIPMENT);

				ds = FMChannelHelper.MakeCall<IEquipments, DataSet>(x => x.EnumerateDataSet(this.Security,
								 this.ManagedEquipmentCheckBox.Checked,
								 this.SecondaryStorageCheckBox.Checked,
								 selectedEquipmentTypeClassGuid,
								 equipmentType,
								 this.GetTranslatedText("{Unassigned}"),
								 findString,
								 defense,
								 !this.ShowHiddenCheckBox.Checked,
								 limit));

				if (ds.Tables[0].Rows.Count >= limit && limit > 0)
				{
					this.lblWarning.Text = string.Format("Results limited to first {0} records.  Use filters to narrow search.", limit);
					this.lblWarning.Visible = true;
				}
				else
				{
					this.lblWarning.Visible = false;
				}


				foreach (DataRow dr in ds.Tables[0].Rows)
				{
					DataRow newRow = this.dt.NewRow();
					newRow["SiteGuid"] = dr["SiteGuid"];
					newRow["EquipmentGuid"] = dr["EquipmentGuid"];
					newRow["ID"] = dr["ID"];
					newRow["_MasterRecordGuid"] = dr["_MasterRecordGuid"];
					newRow["Volume"] = dr["Volume"];
					newRow["QCDate"] = dr["QCDate"];
					newRow["ReturnToServiceDate"] = dr["ReturnToServiceDate"];
					newRow["InServiceFlag"] = dr["InServiceFlag"];
					newRow["LockedOut"] = dr["LockedOut"];
					newRow["Capacity"] = dr["Capacity"];
					newRow["VolumeUnit"] = dr["VolumeUnit"];
					newRow["CompanyEquipmentID"] = dr["CompanyEquipmentID"];
					newRow["Company"] = dr["Company"];
					newRow["ProductID"] = dr["ProductID"];
					newRow["HiddenDate"] = dr["HiddenDate"];
					newRow["CompanyName"] = dr["CompanyName"];
					newRow["CompanyID"] = dr["CompanyID"];
					newRow["CompanyAddress"] = dr["CompanyAddress"];
					newRow["CompanyCity"] = dr["CompanyCity"];
					newRow["CompanyState"] = dr["CompanyState"];
					newRow["SerialNumber"] = dr["SerialNumber"];
					newRow["Description"] = dr["Description"];
					newRow["Make"] = dr["Make"];
					newRow["Model"] = dr["Model"];
					newRow["Year"] = dr["Year"];
					newRow["EqTypeName"] = dr["EqTypeName"];	// 25
					newRow["LookupEquipmentTypeIndex"] = dr["LookupEquipmentTypeIndex"];	// 26
					newRow["FuelCardID"] = dr["FuelCardID"];	// 27
					newRow["MeterCount"] = dr["MeterCount"];	// 28
					newRow["Remote"] = false;	// 29
					this.dt.Rows.Add(newRow);
				}
			}

			if (this.Session[SortExpression] != null && this.Session[SortDirection] != null)
			{
				this.dv = new DataView(this.dt)
				{
					Sort = (string)this.Session[SortExpression] + " " + (string)this.Session[SortDirection]
				};
			}
			else
			{
				this.dv = this.dt.DefaultView;
			}

			this.EquipmentSummaryPageSizeDropDown.SetPageSize(this.EquipmentsDataGrid, this.dt.Rows.Count);
			this.EquipmentsDataGrid.DataSource = this.dv;
			FMDeleteLinkButton deleteLinkButton = (FMDeleteLinkButton)this.EquipmentsDataGrid.FindControl("Delete");
			this.EquipmentsDataGrid.DataBind();
			deleteLinkButton = (FMDeleteLinkButton)this.EquipmentsDataGrid.FindControl("Delete");
		}

		private void PrepareDataTable()
        {
            this.dt = new DataTable();

            this.dt.Columns.Add("SiteGuid", typeof(Guid));
            this.dt.Columns.Add("EquipmentGuid", typeof(Guid));
            this.dt.Columns.Add("ID", typeof(string));
            this.dt.Columns.Add("_MasterRecordGuid", typeof(Guid));
            this.dt.Columns.Add("Volume", typeof(double));
            this.dt.Columns.Add("QCDate", typeof(DateTimeOffset));
            this.dt.Columns.Add("ReturnToServiceDate", typeof(DateTimeOffset));
            this.dt.Columns.Add("InServiceFlag", typeof(string));
            this.dt.Columns.Add("LockedOut", typeof(string));
            this.dt.Columns.Add("Capacity", typeof(double));
            this.dt.Columns.Add("VolumeUnit", typeof(string));
            this.dt.Columns.Add("CompanyEquipmentID", typeof(string));
            this.dt.Columns.Add("Company", typeof(string));
            this.dt.Columns.Add("ProductID", typeof(string));
            this.dt.Columns.Add("HiddenDate", typeof(DateTimeOffset));
            this.dt.Columns.Add("CompanyName", typeof(string));
            this.dt.Columns.Add("CompanyID", typeof(string));
            this.dt.Columns.Add("CompanyAddress", typeof(string));
            this.dt.Columns.Add("CompanyCity", typeof(string));
            this.dt.Columns.Add("CompanyState", typeof(string));
            this.dt.Columns.Add("SerialNumber", typeof(string));
            this.dt.Columns.Add("Description", typeof(string));
            this.dt.Columns.Add("Make", typeof(string));
            this.dt.Columns.Add("Model", typeof(string));
            this.dt.Columns.Add("Year", typeof(string));
            this.dt.Columns.Add("EqTypeName", typeof(string));
			this.dt.Columns.Add("LookupEquipmentTypeIndex", typeof(int));
			this.dt.Columns.Add("FuelCardID", typeof(string));
			this.dt.Columns.Add("MeterCount", typeof(int));
			this.dt.Columns.Add("Remote", typeof(bool));
        }
        #endregion
    }

    [Serializable]
	internal class FilterSettings
	{
		#region Constants and Fields
		public string EquipmentClass = "";
		public string EquipmentType = "";
		public string FindString = "";
		public bool ShowManagedEquipmentOnly;
		public bool ShowSecondStorageOnly;

        /// <summary>
        /// If true, equipment records that are marked as hidden will be displayed
        /// </summary>
	    public bool ShowHidden;
        public string TrailerID = "";
        public string Product = "";
        public string Company = "";
        public string CompanyEquipmentID = "";
		#endregion

		#region Public Methods and Operators
		public void Reset()
		{
			this.FindString = "";
			this.EquipmentClass = "";
			this.EquipmentType = "";
            this.TrailerID = "";
            this.Product = "";
            this.Company = "";
            this.CompanyEquipmentID = "";

			if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDescKey()))
			{
				// WI 18419 - We do not want to default the checkboxes to true if this is a TFMD key
				if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x =>x.IsTFMDKey()) == false)
				{
					this.ShowManagedEquipmentOnly = true;
					this.ShowSecondStorageOnly = true;
				}
			}

		    this.ShowHidden = false;
		}
		#endregion
	}
}