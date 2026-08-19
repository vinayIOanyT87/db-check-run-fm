// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldLevelConfigForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FieldLevelConfigForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.ServiceModel;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;

    public partial class FieldLevelConfigForm : FMFormBase, IMenuDiscovery
	{
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
				if (((word1 & 0x01) != 0x01) && security.LoginSiteGuid != Guids.SiteAdminGuid)// master data management)
				{
					return null;
				}
			}
			else
			{
				// Depends Upon Shared Components Config
				if ((options & 0x4000) == 0)
				{
					return null;
				}
			}
			if ((security.HasRight(RIGHT.VIEW_FIELD_LEVEL_CONTROL_CONFIGURATION) == false)
				&& (security.HasRight(RIGHT.MODIFY_FIELD_LEVEL_CONTROL_CONFIGURATION) == false))
			{
				return null;
			}

			// The Login Site must be a Site Group
			if (siteGroup == false)
			{
				return null;
			}

			var menuItems = new List<FMMenuItem>();

			var menuItem = new FMMenuItem
				{
					MenuItemType = FMMenuItemType.ADMIN_FIELD_LEVEL_CONFIGURATION,
					RootMenuName = "Administration",
					CategoryName = "Sites",
					ItemName = "Field Level Control",
					NavigateUrl = "FieldLevelConfigForm.aspx",
					SortOrder = 4,
					ApplyDataDictionary = FMWebApp.ApplyDataDictionary.Apply
				};

			menuItems.Add(menuItem);

			return menuItems;
		}

		#endregion

		#region Methods

		/// <summary>
		///    This method will handle the apply button event and apply all changes to the database.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void ApplyBtn_Onclick(object sender, EventArgs e)
		{
		    if (this.SiteGroupDropdown.SelectedIndex < 0)
			{
				return;
			}
			var targetSiteGroupGuid = this.Security.SiteGuid;
			if (this.SiteGroupDropdown.SelectedValue != "{All}")
			{
				Guid.TryParse(this.SiteGroupDropdown.SelectedValue, out targetSiteGroupGuid);
			}

			FieldLevelConfigCollectionClass flcCollection = this.GetFlcUpdatedList(true);
			if ((flcCollection == null) || (flcCollection.Count == 0))
			{
				return;
			}

			try
			{
                FMChannelHelper.MakeCall<IFieldLevelConfigMaps>(
                        flcMaps =>
                        {
                            (flcMaps as IClientChannel).OperationTimeout = new TimeSpan(0, 10, 0); 
                            flcMaps.Update(this.Security, flcCollection, targetSiteGroupGuid);
                        });
			}
			catch (Exception except)
			{
                if ((Convert.ToString(this.EntityTypeDropdown.SelectedValue).ToUpper() == ENTITY_TYPE.TRANSACTION_ALIAS.ToString().ToUpper()) && 
                    (except.Message.Contains("DELETE statement conflicted with the REFERENCE constraint")) 
                    && (except.Message.Contains("tblListViewFields")))
                {
                    except = new Exception("Transaction Alias Record Versioning cannot be turned off because of existing relationships with one or more Views at the children sites/sitegroups. Please modify the View/s as necessary and try again.", except);
                }
   				this.ErrorHandler(except);
			}
			this.RefreshGrid();
		}

		protected void ControlModeDropDownSelectedIndexChanged(object sender, EventArgs e)
		{
			this.PersistFilters();
			this.RefreshGrid();
		}

		protected void EntityTypeDropdownSelectedIndexChanged(object sender, EventArgs e)
		{
			this.PersistFilters();
			//this.LoadFilterDropdown();

			this.LoadFilterValueDropdown();

			this.LoadTargetFieldDropdown();
			this.RefreshGrid();
		}

		/// <summary>
		///    Set the display and availability of controls in the grid for each individual data row as the data grid is built.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void FieldLevelConfigGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			try
			{
				if (e.Item.ItemIndex < 0)
				{
					return;
				}

				var verSpecificColCheckBox = (CheckBox)e.Item.FindControl("VerSpecificColCheckBox");
                var globalSpecificColCheckBox = (CheckBox)e.Item.FindControl("GlobalSpecificColCheckBox");
                if (verSpecificColCheckBox == null || globalSpecificColCheckBox == null)
				{
					return;
				}
			    verSpecificColCheckBox.Enabled = false;
			    globalSpecificColCheckBox.Enabled = false;

				if (!this.Security.HasRight(RIGHT.MODIFY_FIELD_LEVEL_CONTROL_CONFIGURATION))
				{
					return;
				}

				if (this.SiteGroupDropdown.SelectedIndex < 0)
				{
					return;
				}

				Guid targetSiteGroup = this.Security.SiteGuid;
				if (this.SiteGroupDropdown.SelectedValue != "{All}")
				{
					Guid.TryParse(this.SiteGroupDropdown.SelectedValue, out targetSiteGroup);
				}

				var siteGroupGuidColLabel = (Label)e.Item.FindControl("SiteGroupGuidColLabel");

				if ((siteGroupGuidColLabel == null) || (siteGroupGuidColLabel.Text != targetSiteGroup.ToString()))
				{
					return;
				}

				var inheritedControlModeColLabel = (Label)e.Item.FindControl("InheritedControlModeColLabel");
				if (inheritedControlModeColLabel == null)
				{
					return;
				}

				if (inheritedControlModeColLabel.Text.Length == 0)
				{
					verSpecificColCheckBox.Enabled = true;
				}
				else
				{
					var inheritedControlMode =
						(FieldLevelConfigClass.FIELD_CONTROL_MODE)
						Enum.Parse(typeof(FieldLevelConfigClass.FIELD_CONTROL_MODE), inheritedControlModeColLabel.Text);
					if ((inheritedControlMode == FieldLevelConfigClass.FIELD_CONTROL_MODE.VersionSpecific)
						 || (inheritedControlMode == FieldLevelConfigClass.FIELD_CONTROL_MODE.Unknown))
					{
						verSpecificColCheckBox.Enabled = true;
					}
                    if ((inheritedControlMode == FieldLevelConfigClass.FIELD_CONTROL_MODE.GlobalSpecific)
                         || (inheritedControlMode == FieldLevelConfigClass.FIELD_CONTROL_MODE.Unknown))
                    {
                        globalSpecificColCheckBox.Enabled = true;
                    }
                }

                verSpecificColCheckBox.InputAttributes.Add("onchange",$"if (this.checked) {{ document.getElementById(\"{globalSpecificColCheckBox.ClientID}\").checked = false;}} ");
                globalSpecificColCheckBox.InputAttributes.Add("onchange", $"if (this.checked) {{ document.getElementById(\"{verSpecificColCheckBox.ClientID}\").checked = false;}} ");
            }
            catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    This method will handle the sort command event. It will save the sort column in session.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		protected void FieldLevelConfigGridSortCommand(object source, DataGridSortCommandEventArgs e)
		{
			bool validSortKey = false;
			string sortExpression = e.SortExpression;

			// Can only sort on the Entity Type, Entity, or Site columns.
			if (sortExpression != null)
			{
				validSortKey = ((sortExpression.ToUpper().Equals("ENTITYTYPEID"))
									 || (sortExpression.ToUpper().Equals("SITEGROUPID"))
									 || (sortExpression.ToUpper().Equals("FILTERDISPLAYNAME"))
									 || (sortExpression.ToUpper().Equals("FILTERVALUENAME"))
									 || (sortExpression.ToUpper().Equals("TARGETFIELD"))
									 || (sortExpression.ToUpper().Equals("FORWARDCONTROLMODE")));
			}

			if (!validSortKey)
			{
				return;
			}

			var sortField = this.Page.Session[PageSessionKeyConstants.EAF_SESSION_SORT_KEY] as string;
			if (string.IsNullOrEmpty(sortField) || (sortField.IndexOf(" DESC", StringComparison.Ordinal) > 0))
			{
				sortField = sortExpression + " ASC";
			}
			else
			{
				sortField = sortExpression + " ASC";
			}
			FieldLevelConfigCollectionClass flcCollection = this.GetFlcUpdatedList(false);
			DataView dv = this.GetDataView(flcCollection);
			dv.Sort = sortField;
			this.FieldLevelConfigGrid.DataSource = dv;
			this.FieldLevelConfigGrid.DataBind();
			this.Page.Session[PageSessionKeyConstants.EAF_SESSION_SORT_KEY] = sortField;
		}

		protected void FilterDropDownSelectedIndexChanged(object sender, EventArgs e)
		{
			this.PersistFilters();
			this.LoadFilterValueDropdown();
			this.RefreshGrid();
		}

		protected void FilterValueDropDownSelectedIndexChanged(object sender, EventArgs e)
		{
			this.PersistFilters();
			this.RefreshGrid();
		}

		protected void IncludeMemberSiteGroupsCheckBoxCheckedChanged(object sender, EventArgs e)
		{
			this.PersistFilters();
			this.RefreshGrid();
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
		///    This method is the main entry point into the entity assignment page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!this.Page.IsPostBack)
				{
					this.ApplyDataDictionary();
                    this.GetRootSite();
					this.InitialiseControls();
					this.RefreshGrid();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void SiteGroupDropdownSelectedIndexChanged(object sender, EventArgs e)
		{
			this.PersistFilters();
			string filterValue = this.FilterValueDropdown.SelectedValue;
			this.LoadFilterValueDropdown();
			if (!string.IsNullOrEmpty(filterValue))
			{
				this.FilterValueDropdown.SelectedIndex =
					this.FilterValueDropdown.Items.IndexOf(this.FilterValueDropdown.Items.FindByValue(filterValue));
			}
			this.IncludeMemberSiteGroupsCheckBox.Enabled = true;
			if (this.SiteGroupDropdown.SelectedValue == "{All}")
			{
				this.IncludeMemberSiteGroupsCheckBox.Checked = true;
				this.IncludeMemberSiteGroupsCheckBox.Enabled = false;
			}
			this.RefreshGrid();
		}

		protected void TargetFieldDropDownSelectedIndexChanged(object sender, EventArgs e)
		{
			this.PersistFilters();
			this.RefreshGrid();
		}

		/// <summary>
		///    Checks all the check boxes of the VersionSpecific column that are enabled.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void TopCheckAllButtonClick(object sender, EventArgs e)
		{
		    DataGridItemCollection gridItems = this.FieldLevelConfigGrid.Items;

			if (gridItems.Count > 0)
			{
				foreach (DataGridItem item in gridItems)
				{
					try
					{
					    var checkBox = (CheckBox)item.FindControl("VerSpecificColCheckBox");
					    if (checkBox.Enabled)
						{
							checkBox.Checked = true;
						}
					}
					catch (Exception except)
					{
						this.ErrorHandler(except);
					}
				}
			}
		}

		/// <summary>
		///    Unchecks all the check boxes of the VersionSpecific column that are enabled.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void TopUncheckAllButtonClick(object sender, EventArgs e)
		{
		    DataGridItemCollection gridItems = this.FieldLevelConfigGrid.Items;

			if (gridItems.Count > 0)
			{
				foreach (DataGridItem item in gridItems)
				{
					try
					{
					    var checkBox = (CheckBox)item.FindControl("VerSpecificColCheckBox");
					    if (checkBox.Enabled)
						{
							checkBox.Checked = false;
						}
					}
					catch (Exception except)
					{
						this.ErrorHandler(except);
					}
				}
			}
		}

		/// <summary>
		///    This method will apply the data dictionary to the controls on the form.
		/// </summary>
		private void ApplyDataDictionary()
		{
			this.BottomApplyBtn.Text = this.GetTranslatedText(this.BottomApplyBtn.Text);
			this.TopApplyBtn.Text = this.GetTranslatedText(this.TopApplyBtn.Text);
			this.TopCheckAllButton.Text = this.GetTranslatedText(this.TopCheckAllButton.Text);
			this.TopUncheckAllButton.Text = this.GetTranslatedText(this.TopUncheckAllButton.Text);
			this.IncludeMemberSiteGroupsCheckBox.Text = this.GetTranslatedText(this.IncludeMemberSiteGroupsCheckBox.Text);
			this.EntityTypeLabel.Text = this.GetTranslatedText(this.EntityTypeLabel.Text);
			this.SiteGroupLabel.Text = this.GetTranslatedText(this.SiteGroupLabel.Text);
			//this.FilterLabel.Text = base.GetTranslatedText(this.FilterLabel.Text);
			this.FilterValueLabel.Text = this.GetTranslatedText(this.FilterValueLabel.Text);
			this.TargetFieldLabel.Text = this.GetTranslatedText(this.TargetFieldLabel.Text);
			this.ControlModeLabel.Text = this.GetTranslatedText(this.ControlModeLabel.Text);
		    for (int i = 0; i < this.FieldLevelConfigGrid.Columns.Count; i++)
			{
				if ((this.FieldLevelConfigGrid.Columns[i] != null) && (this.FieldLevelConfigGrid.Columns[i].HeaderText != null))
				{
				    var newText = this.GetTranslatedText(this.FieldLevelConfigGrid.Columns[i].HeaderText);
				    this.FieldLevelConfigGrid.Columns[i].HeaderText = newText;
				}
			}
		}

		/// <summary>
		///    This method will enable/disable the controls according to the user rights.
		/// </summary>
		private void ControlsAvailabilitySecurityCheck()
		{
			this.TopCheckAllButton.Enabled = false;
			this.TopUncheckAllButton.Enabled = false;
			this.TopApplyBtn.Enabled = false;
			this.BottomApplyBtn.Enabled = false;
           
            if (this.Security.HasRight(RIGHT.MODIFY_FIELD_LEVEL_CONTROL_CONFIGURATION))
			{
				this.TopCheckAllButton.Enabled = true;
				this.TopUncheckAllButton.Enabled = true;
				this.TopApplyBtn.Enabled = true;
				this.BottomApplyBtn.Enabled = true;
			}
		}

        /// <summary>
        /// Fetch the RootSite of the current server, as determined by the Sync download.
        /// </summary>
        private void GetRootSite()
        {
            string rootSiteId = null;
            bool isEnterpriseSystem = false;
            string isEnterpriseStr =
                FMChannelHelper.MakeCall<IConfigurationSettings, string>(
                    configSettingsChannel =>
                        configSettingsChannel.GetKeyValueByKey(this.Security, ConfigurationSettingDOClass.Key_IsEnterprise));
            isEnterpriseSystem = (isEnterpriseStr == "1");

            if (!isEnterpriseSystem)
            {
                SyncClientConfigurationDO clientSyncConfig = FMChannelHelper.MakeCall<ISyncClientConfigurations, SyncClientConfigurationDO>(x => x.Get(this.Security));
                if (clientSyncConfig != null && !string.IsNullOrEmpty(clientSyncConfig.RootSiteID))
                    rootSiteId = clientSyncConfig.RootSiteID;
            }
            this.Session["FLCRootSiteId"] = rootSiteId;
        }


		/// <summary>
		///    Package the FLC collection into a DataView to facilitate sorting of the datagrid
		/// </summary>
		/// <param name="flcCollection"></param>
		/// <returns></returns>
		private DataView GetDataView(FieldLevelConfigCollectionClass flcCollection)
		{
		    ENTITY_TYPE eType = ENTITY_TYPE.EQUIPMENT;

			switch (this.EntityTypeDropdown.SelectedValue)
			{
				case "Equipment":
					{
						eType = ENTITY_TYPE.EQUIPMENT;
						break;
					}
				case "Product":
					{
						eType = ENTITY_TYPE.PRODUCT;
						break;
					}
				case "Company":
					{
						eType = ENTITY_TYPE.COMPANY;
						break;
					}

				case "Transaction_Alias":
					{
						eType = ENTITY_TYPE.TRANSACTION_ALIAS;
						break;
					}
				case "Personnel":
					{
						eType = ENTITY_TYPE.PERSONNEL;
						break;
					}
			}

			var userDataFieldCollection =
				 FMChannelHelper.MakeCall<IUserDataFields, UserDataFieldCollectionClass>(
				 x => x.EnumerateByEntityType(this.Security, eType, Guid.Empty, false, false));

			var dt = new DataTable();
			dt.Columns.Add(new DataColumn("FieldLevelConfigMatrixIndex", typeof(int)));
			dt.Columns.Add(new DataColumn("EntitySegmentTemplateGuid", typeof(Guid)));
			dt.Columns.Add(new DataColumn("EntityTypeId", typeof(string)));
			dt.Columns.Add(new DataColumn("EntityTypeDisplayName", typeof(string)));
			dt.Columns.Add(new DataColumn("SiteGroupGuid", typeof(Guid)));
			dt.Columns.Add(new DataColumn("SiteGroupId", typeof(string)));
			dt.Columns.Add(new DataColumn("HierarchyLevel", typeof(int)));
			dt.Columns.Add(new DataColumn("FilterFieldName", typeof(string)));
			dt.Columns.Add(new DataColumn("FilterDisplayName", typeof(string)));
			dt.Columns.Add(new DataColumn("FilterValueGuid", typeof(Guid)));
			dt.Columns.Add(new DataColumn("FilterValueName", typeof(string)));
			dt.Columns.Add(new DataColumn("TargetField", typeof(string)));
			dt.Columns.Add(new DataColumn("TargetFieldDisplay", typeof(string)));
			dt.Columns.Add(new DataColumn("InheritedControlMode", typeof(string)));
			dt.Columns.Add(new DataColumn("ForwardControlMode", typeof(string)));
		    dt.Columns.Add(new DataColumn("IsFCMVerSpecific", typeof(bool)));
		    dt.Columns.Add(new DataColumn("IsFCMGlobalSpecific", typeof(bool)));
		    foreach (FieldLevelConfigClass t in flcCollection)
		    {
		        var dr = dt.NewRow();
		        FieldLevelConfigClass flc = t;
		        dr["FieldLevelConfigMatrixIndex"] = flc.FieldLevelConfigMatrixIndex;
		        dr["EntitySegmentTemplateGuid"] = flc.EntitySegmentTemplateGuid;
		        dr["EntityTypeId"] = flc.EntityTypeId;
		        dr["EntityTypeDisplayName"] = flc.EntityTypeDisplayName;
		        dr["SiteGroupGuid"] = flc.SiteGroupGuid;
		        dr["SiteGroupId"] = flc.SiteGroupId;
		        dr["HierarchyLevel"] = flc.HierarchyLevel;
		        dr["FilterFieldName"] = flc.FilterFieldName;
		        dr["FilterDisplayName"] = flc.FilterDisplayName;
		        dr["FilterValueGuid"] = flc.FilterValueGuid;
		        dr["FilterValueName"] = flc.FilterValueName;
		        dr["TargetField"] = flc.TargetField;

		        //replace flc.TargetField with UserData alias if available
		        var targetField = this.GetTranslatedText(flc.TargetField);
		        foreach (var fieldClass in userDataFieldCollection)
		        {
		            var udf = (UserDataFieldClass)fieldClass;
		            if (udf.DbName == flc.TargetField)
		            {
		                targetField = this.GetTranslatedText(udf.DbName) + " - " + this.GetTranslatedText(udf.DisplayName);
		                break;
		            }
		        }
		        //then replace result with data dictionary alias if available.
		        dr["TargetFieldDisplay"] = targetField;

		        //dr["TargetFieldDisplay"] = base.GetTranslatedText(this.EntityTypeDropdown.Items[this.EntityTypeDropdown.SelectedIndex].Text + targetField);
		        dr["InheritedControlMode"] = flc.InheritedControlMode;
		        dr["ForwardControlMode"] = flc.ForwardControlMode;
		        dr["IsFCMVerSpecific"] = flc.ForwardControlMode == FieldLevelConfigClass.FIELD_CONTROL_MODE.VersionSpecific;
		        dr["IsFCMGlobalSpecific"] = flc.ForwardControlMode == FieldLevelConfigClass.FIELD_CONTROL_MODE.GlobalSpecific;
		        dt.Rows.Add(dr);
		    }
			var dv = new DataView(dt);
			return dv;
		}


		/// <summary>
		///    Retrieves the current status of the collection of FLC records being displayed, as captured with the latest user changes to the grid/collection elements
		/// </summary>
		/// <param name="changesOnly">True: Returns only a collection of records changed by the user. False: Return all records.</param>
		/// <returns></returns>
		private FieldLevelConfigCollectionClass GetFlcUpdatedList(bool changesOnly)
		{
		    int matrixIndex;

			DataGridItemCollection gridItems = this.FieldLevelConfigGrid.Items;
			if (gridItems.Count == 0)
			{
				return null;
			}

			var flcChangeList = new FieldLevelConfigCollectionClass();
			foreach (DataGridItem item in gridItems)
			{
				try
				{
					var indexColLabel = (Label)item.FindControl("FieldLevelConfigMatrixIndexColLabel");
					if (indexColLabel == null)
					{
						continue;
					}
					matrixIndex = Convert.ToInt32(indexColLabel.Text);
					var flcConfigMatrix =
						this.Session[PageSessionKeyConstants.FLC_SESSION_DATA_MATRIX] as FieldLevelConfigCollectionClass;
					if ((flcConfigMatrix == null) || (flcConfigMatrix.Count == 0))
					{
						return null;
					}
					Guid targetSiteGroupGuid = this.Security.SiteGuid;
					if (this.SiteGroupDropdown.SelectedValue != "{All}")
					{
						Guid.TryParse(this.SiteGroupDropdown.SelectedValue, out targetSiteGroupGuid);
					}
					var flc = flcConfigMatrix.Find(f => f.FieldLevelConfigMatrixIndex == matrixIndex);
					if (flc == null)
					{
						continue;
					}
					if ((flc.SiteGroupGuid != targetSiteGroupGuid) && (changesOnly))
					{
						continue;
					}

					var checkVerSpecific = (CheckBox)item.FindControl("VerSpecificColCheckBox");
                    var checkGlobalSpecific = (CheckBox)item.FindControl("GlobalSpecificColCheckBox");
                    if (checkVerSpecific == null || checkGlobalSpecific == null)
					{
						continue;
					}

                    // TODO: Fix up logic here for both checkboxes.
				    if (!checkVerSpecific.Enabled && !checkGlobalSpecific.Enabled)
				    {
				        continue;
				    }

				    bool changed = !((!checkVerSpecific.Enabled) || (checkVerSpecific.Checked == (flc.ForwardControlMode == FieldLevelConfigClass.FIELD_CONTROL_MODE.VersionSpecific))) 
                                    || !((!checkGlobalSpecific.Enabled) || (checkGlobalSpecific.Checked == (flc.ForwardControlMode == FieldLevelConfigClass.FIELD_CONTROL_MODE.GlobalSpecific))) 
                                    || !changesOnly;

				    if (!changed)
				    {
				        continue;
				    }

				    if (checkVerSpecific.Checked)
				    {
				        flc.ForwardControlMode = FieldLevelConfigClass.FIELD_CONTROL_MODE.VersionSpecific;
				    }
                    else if (checkGlobalSpecific.Checked)
                    {
                        flc.ForwardControlMode = FieldLevelConfigClass.FIELD_CONTROL_MODE.GlobalSpecific;
                    }
                    else
                    {
                        flc.ForwardControlMode = FieldLevelConfigClass.FIELD_CONTROL_MODE.ParentSpecific;
                    }

                    flcChangeList.Add(flc);
				}
				catch (Exception except)
				{
					this.ErrorHandler(except);
				}
			}
			return flcChangeList;
		}

		/// <summary>
		///    Initialise the controls on the form
		/// </summary>
		private void InitialiseControls()
		{
			this.ControlsAvailabilitySecurityCheck();
			this.LoadEntityTypeDropdown();
			if (this.Page.Session[PageSessionKeyConstants.FLC_SESSION_ENTITY_TYPE_SELECT] != null)
			{
				var selectedValue = this.Page.Session[PageSessionKeyConstants.FLC_SESSION_ENTITY_TYPE_SELECT] as string;
				this.EntityTypeDropdown.SelectedValue = selectedValue;
			}

			this.IncludeMemberSiteGroupsCheckBox.Checked = false;
			if (this.Page.Session[PageSessionKeyConstants.FLC_SESSION_INCLUDE_MEMBER_SITEGROUPS_SELECT] != null)
			{
				this.IncludeMemberSiteGroupsCheckBox.Checked =
					(bool)this.Page.Session[PageSessionKeyConstants.FLC_SESSION_INCLUDE_MEMBER_SITEGROUPS_SELECT];
			}

			this.LoadSiteGroupDropdown();
			if (this.Page.Session[PageSessionKeyConstants.FLC_SESSION_SITE_GROUP_SELECT] != null)
			{
				var selectedValue = this.Page.Session[PageSessionKeyConstants.FLC_SESSION_SITE_GROUP_SELECT] as string;
				this.SiteGroupDropdown.SelectedValue = selectedValue;
			}

			//this.LoadFilterDropdown();
			//if (this.Page.Session[PageSessionKeyConstants.FLC_SESSION_FILTER_SELECT] != null)
			//{
			//    var selectedValue = this.Page.Session[PageSessionKeyConstants.FLC_SESSION_FILTER_SELECT] as string;
			//    this.FilterDropdown.SelectedValue = selectedValue;
			//}

			this.LoadFilterValueDropdown();
			if (this.Page.Session[PageSessionKeyConstants.FLC_SESSION_FILTER_VALUE_SELECT] != null)
			{
				var selectedValue = this.Page.Session[PageSessionKeyConstants.FLC_SESSION_FILTER_VALUE_SELECT] as string;
				this.FilterValueDropdown.SelectedValue = selectedValue;
			}

			this.LoadTargetFieldDropdown();
			if (this.Page.Session[PageSessionKeyConstants.FLC_SESSION_TARGET_FIELD_SELECT] != null)
			{
				var selectedValue = this.Page.Session[PageSessionKeyConstants.FLC_SESSION_TARGET_FIELD_SELECT] as string;
				this.TargetFieldDropdown.SelectedValue = selectedValue;
			}

			this.LoadControlModeDropdown();
			if (this.Page.Session[PageSessionKeyConstants.FLC_SESSION_CONTROL_MODE_SELECT] != null)
			{
				var selectedValue = this.Page.Session[PageSessionKeyConstants.FLC_SESSION_CONTROL_MODE_SELECT] as string;
				this.ControlModeDropdown.SelectedValue = selectedValue;
			}
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.TopCheckAllButton.Command += this.TopCheckAllButtonClick;
			this.TopUncheckAllButton.Command += this.TopUncheckAllButtonClick;
			this.FieldLevelConfigGrid.ItemDataBound += this.FieldLevelConfigGridItemDataBound;
		}

		/// <summary>
		///    This method will load the Control Mode dropdown list
		/// </summary>
		private void LoadControlModeDropdown()
		{
			try
			{
				this.ControlModeDropdown.Items.Clear();
			    var newItem = new ListItem("Parent Controlled", "ParentSpecific");
				this.ControlModeDropdown.Items.Insert(0, newItem);
				newItem = new ListItem("Child Controlled - Local Version", "VersionSpecific");
				this.ControlModeDropdown.Items.Insert(0, newItem);
                newItem = new ListItem("Child Controlled - Global Access", "GlobalSpecific");
                this.ControlModeDropdown.Items.Insert(0, newItem);
                newItem = new ListItem("{Configurable}", "Configurable");
				this.ControlModeDropdown.Items.Insert(0, newItem);
				newItem = new ListItem("{All}", "{All}");
				this.ControlModeDropdown.Items.Insert(0, newItem);
				this.ControlModeDropdown.SelectedIndex = 0;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    This method will load the Entity Type dropdown list with a list of all the entity types supported by Field LEvel Configuration.
		/// </summary>
		private void LoadEntityTypeDropdown()
		{
			Guid currentSiteGuid = this.Security.SiteGuid;
			Guid loginSiteGuid = this.Security.LoginSiteGuid;
			this.Security.LoginSiteGuid = currentSiteGuid;
			try
			{
				Hashtable htData = FMChannelHelper.MakeCall<IFieldLevelConfigMaps, Hashtable>(x => x.GetEntityTypes(this.Security));

				this.EntityTypeDropdown.Items.Clear();
				ListItem newItem;
				foreach (DictionaryEntry de in htData)
				{
					newItem = new ListItem(Convert.ToString(de.Value), Convert.ToString(de.Key));
					this.EntityTypeDropdown.Items.Add(newItem);
				}
				if (this.EntityTypeDropdown.Items.Count > 0)
				{
					/*
					if (this.EntityTypeDropdown.Items.Count > 1)
					{
						 newItem = new ListItem("{All}", "{All}");
						 this.EntityTypeDropdown.Items.Insert(0, newItem);
					}
					*/
					newItem = new ListItem("{None}", ((int)ENTITY_TYPE.UNKNOWN).ToString());
					this.EntityTypeDropdown.Items.Insert(0, newItem);
					this.EntityTypeDropdown.SelectedIndex = 0;
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
			finally
			{
				this.Security.SiteGuid = currentSiteGuid;
				this.Security.LoginSiteGuid = loginSiteGuid;
			}
		}

		/// <summary>
		///    This method will load the Filter Value dropdown list with all the Filters Values for the selected Filter
		/// </summary>


		private void LoadFilterValueDropdown()
		{
			string filterKey = "";

			this.FilterValueDropdown.Items.Clear();

			if (Convert.ToString(this.EntityTypeDropdown.SelectedValue) == "Equipment")
			{
				this.FilterValueLabel.Text = "Equipment Type";
				this.FieldLevelConfigGrid.Columns[3].HeaderText = "Equipment Type";
				filterKey = "EquipmentTypeGuid";
			}
			else
			{
				//default label / column header text will be "Filter Value"
				this.FilterValueLabel.Text = "Filter Value";
				this.FieldLevelConfigGrid.Columns[3].HeaderText = "Filter Value";
			}

			//Here use the GetFLCFilters to find the filter for selected EntityType
			//i.e. Equipment -> EquipmentType
			//this will be used in place of filterdropdown.selectedvalue.

			//if ((this.FilterDropdown.SelectedIndex < 0) || (this.FilterDropdown.SelectedValue == "{All}")
			//    || (this.FilterDropdown.SelectedValue == "{None}"))
			//{
			//    return;
			//}

			Guid selectedSiteGroup;
			if ((this.SiteGroupDropdown.SelectedIndex < 0) || (this.SiteGroupDropdown.SelectedValue == "{All}")
				 || (this.SiteGroupDropdown.SelectedValue == "{None}")
				 || !Guid.TryParse(this.SiteGroupDropdown.SelectedValue, out selectedSiteGroup))
			{
				ListItem newItem = new ListItem("{n/a}", "{n/a}");
				this.FilterValueDropdown.Items.Insert(0, newItem);
				this.FilterValueDropdown.SelectedIndex = 0;
				return;
			}

			Guid currentSiteGuid = this.Security.SiteGuid;
			Guid loginSiteGuid = this.Security.LoginSiteGuid;
			this.Security.LoginSiteGuid = currentSiteGuid;

			try
			{
				if (filterKey != "")
				{
					Hashtable htDataFilterValues = FMChannelHelper.MakeCall<IFieldLevelConfigMaps, Hashtable>(
															  x =>
															  x.GetFilterValues(
																	this.Security,
																	Convert.ToString(this.EntityTypeDropdown.SelectedValue),
																	selectedSiteGroup,
																	filterKey)
																);

					ListItem newItem;
					foreach (DictionaryEntry de in htDataFilterValues)
					{
						string filterFieldName = (de.Value == null) ? "{Undefined}" : Convert.ToString(de.Value);
						newItem = new ListItem(filterFieldName, Convert.ToString(de.Key));
						this.FilterValueDropdown.Items.Add(newItem);
					}
					if (this.FilterValueDropdown.Items.Count > 1)
					{
						newItem = new ListItem("{All}", "{All}");
						this.FilterValueDropdown.Items.Insert(0, newItem);
						this.FilterValueDropdown.SelectedIndex = 0;
					}

					//best place for following two lines is EntityTypeDropdown_SelectedIndexChanged, but putting them 
					//there would require an additional WCF function call to IFieldLevelConfigMaps so they are here. 
					//BMain 1/2/2013
					//this.FilterValueLabel.Text = FilterValue;
					//this.FieldLevelConfigGrid.Columns[3].HeaderText = FilterValue;
				}
				else
				{
					ListItem newItem = new ListItem("{n/a}", "{n/a}");
					this.FilterValueDropdown.Items.Insert(0, newItem);
					this.FilterValueDropdown.SelectedIndex = 0;
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
			finally
			{
				this.Security.SiteGuid = currentSiteGuid;
				this.Security.LoginSiteGuid = loginSiteGuid;
			}
		}

		/// <summary>
		///    This method will load the Site Group dropdown list with the current sitegroup context, and all its immediate children sitegroups in the site hieararchy.
		/// </summary>
		private void LoadSiteGroupDropdown()
		{
			Guid currentSiteGuid = this.Security.SiteGuid;
            string currentSiteId = this.Security.SiteID;
			Guid loginSiteGuid = this.Security.LoginSiteGuid;
			this.Security.LoginSiteGuid = currentSiteGuid;
            string rootSiteId = this.Session["FLCRootSiteId"] as string;
            try
			{
                this.SiteGroupDropdown.Items.Clear();
                SiteClass rootSite = null;
                if (rootSiteId != null)
                {
                   rootSite =
                    FMChannelHelper.MakeCall<ISites, SiteClass>(
                        sites => sites.GetByID(this.Security, rootSiteId, false));
                    if (!rootSite.SiteGroup)
                        return;
                }

                SortedList slData =
					FMChannelHelper.MakeCall<IFieldLevelConfigMaps, SortedList>(
						x => x.GetSiteHierarchy(this.Security, currentSiteGuid, 1, true));
				                                
                if ((rootSiteId != null) && (!rootSiteId.Equals(currentSiteId)))
                {
                    String siteId;
                    foreach (DictionaryEntry de in slData)
                    {
                        siteId = Convert.ToString(de.Key);
                        if (siteId.Equals(rootSiteId))
                        {
                            //RootSiteId is below the currentSiteId. Can only support FLC for the RootSiteId down.
                            //Fetch sitegroups from the RootSiteId instead of from the CurrentSiteId.                            
                            slData =
                                FMChannelHelper.MakeCall<IFieldLevelConfigMaps, SortedList>(
                                    x => x.GetSiteHierarchy(this.Security, rootSite.SiteGuid, 1, true));
                            break;
                        }
                    }
                }


                ListItem newItem;
				foreach (DictionaryEntry de in slData)
				{
					newItem = new ListItem(Convert.ToString(de.Key), Convert.ToString(de.Value));
					this.SiteGroupDropdown.Items.Add(newItem);
				}
				if (this.SiteGroupDropdown.Items.Count > 1)
				{
					newItem = new ListItem("{All}", "{All}");
					this.SiteGroupDropdown.Items.Insert(0, newItem);
				}
				for (int i = 0; i < this.SiteGroupDropdown.Items.Count; i++)
				{
					if (this.SiteGroupDropdown.Items[i].Value == Convert.ToString(currentSiteGuid))
					{
						this.SiteGroupDropdown.SelectedIndex = i;
						break;
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
			finally
			{
				this.Security.SiteGuid = currentSiteGuid;
				this.Security.LoginSiteGuid = loginSiteGuid;
			}
		}

		/// <summary>
		///    This method will load the Target Field dropdown list with all the fields for the selected Entity
		/// </summary>
		private void LoadTargetFieldDropdown()
		{
			//this.TargetFieldDropdown.Items.Clear();
			//if ((this.EntityTypeDropdown.SelectedIndex < 0) || (this.EntityTypeDropdown.SelectedValue == "{All}")
			//    || (this.EntityTypeDropdown.SelectedValue == "{None}"))
			//{
			//    return;
			//}

		    ENTITY_TYPE eType = ENTITY_TYPE.EQUIPMENT;

			switch (this.EntityTypeDropdown.SelectedValue)
			{
				case "Equipment":
					{
						eType = ENTITY_TYPE.EQUIPMENT;
						break;
					}
				case "Products":
					{
						eType = ENTITY_TYPE.PRODUCT;
						break;
					}
				case "Companies":
					{
						eType = ENTITY_TYPE.COMPANY;
						break;
					}

				case "TransactionAliases":
					{
						eType = ENTITY_TYPE.TRANSACTION_ALIAS;
						break;
					}
			}

			var userDataFieldCollection =
				 FMChannelHelper.MakeCall<IUserDataFields, UserDataFieldCollectionClass>(
				 x => x.EnumerateByEntityType(this.Security, eType, Guid.Empty, false, false));


			this.TargetFieldDropdown.Items.Clear();
			if ((this.EntityTypeDropdown.SelectedIndex < 0) || (this.EntityTypeDropdown.Items[this.EntityTypeDropdown.SelectedIndex].Text == "{None}"))
			{
				//return;
				ListItem newItem = new ListItem("{n/a}", "{n/a}");
				this.TargetFieldDropdown.Items.Insert(0, newItem);
				this.TargetFieldDropdown.SelectedIndex = 0;
				return;
			}


			Guid currentSiteGuid = this.Security.SiteGuid;
			Guid loginSiteGuid = this.Security.LoginSiteGuid;
			this.Security.LoginSiteGuid = currentSiteGuid;
			try
			{
				Hashtable htData =
					FMChannelHelper.MakeCall<IFieldLevelConfigMaps, Hashtable>(
						x => x.GetTargetFields(this.Security, Convert.ToString(this.EntityTypeDropdown.SelectedValue)));

				ListItem newItem;
				foreach (DictionaryEntry de in htData)
				{

					//replace flc.TargetField with UserData alias if available
					string nonTranslatedTargetField = Convert.ToString(de.Value);
					var targetField = this.GetTranslatedText(nonTranslatedTargetField);
					foreach (var fieldClass in userDataFieldCollection)
					{
					    var udf = (UserDataFieldClass)fieldClass;
					    if (udf.DbName == nonTranslatedTargetField)
						{
							targetField = this.GetTranslatedText(udf.DbName) + " - " + this.GetTranslatedText(udf.DisplayName);
							break;
						}
					}

				    newItem = new ListItem(targetField, Convert.ToString(de.Key));
					this.TargetFieldDropdown.Items.Add(newItem);

					//**** original code
					//newItem = new ListItem(Convert.ToString(de.Value), Convert.ToString(de.Key));
					//this.TargetFieldDropdown.Items.Add(newItem);
				}
				if (this.TargetFieldDropdown.Items.Count > 1)
				{
					newItem = new ListItem("{All}", "{All}");
					this.TargetFieldDropdown.Items.Insert(0, newItem);
					this.TargetFieldDropdown.SelectedIndex = 0;
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
			finally
			{
				this.Security.SiteGuid = currentSiteGuid;
				this.Security.LoginSiteGuid = loginSiteGuid;
			}
		}

		/// <summary>
		///    This method will persist the field level control configuration page filters.
		/// </summary>
		private void PersistFilters()
		{
			this.Page.Session[PageSessionKeyConstants.FLC_SESSION_ENTITY_TYPE_SELECT] = this.EntityTypeDropdown.SelectedValue;
			this.Page.Session[PageSessionKeyConstants.FLC_SESSION_SITE_GROUP_SELECT] = this.SiteGroupDropdown.SelectedValue;
			//this.Page.Session[PageSessionKeyConstants.FLC_SESSION_FILTER_SELECT] = this.FilterDropdown.SelectedValue;
			this.Page.Session[PageSessionKeyConstants.FLC_SESSION_FILTER_VALUE_SELECT] = this.FilterValueDropdown.SelectedValue;
			this.Page.Session[PageSessionKeyConstants.FLC_SESSION_TARGET_FIELD_SELECT] = this.TargetFieldDropdown.SelectedValue;
			this.Page.Session[PageSessionKeyConstants.FLC_SESSION_CONTROL_MODE_SELECT] = this.ControlModeDropdown.SelectedValue;
			this.Page.Session[PageSessionKeyConstants.FLC_SESSION_INCLUDE_MEMBER_SITEGROUPS_SELECT] =
				this.IncludeMemberSiteGroupsCheckBox.Checked;
		}

		/// <summary>
		///    This method refreshes the grid.
		/// </summary>
		private void RefreshGrid()
		{
			Guid currentSiteGuid = this.Security.SiteGuid;
			Guid loginSiteGuid = this.Security.LoginSiteGuid;
			this.Security.LoginSiteGuid = currentSiteGuid;

			try
			{
				var flcCollection = new FieldLevelConfigCollectionClass();

				if ((this.EntityTypeDropdown.SelectedIndex >= 0) && (this.EntityTypeDropdown.SelectedValue != "{None}")
					 && (this.SiteGroupDropdown.SelectedIndex >= 0))
				{
					string entityTypeId = (this.EntityTypeDropdown.SelectedValue == "{All}")
													 ? null
													 : this.EntityTypeDropdown.SelectedValue;
					Guid sitegroupGuid;
					if (this.SiteGroupDropdown.SelectedValue == "{All}")
					{
						sitegroupGuid = this.Security.SiteGuid;
					}
					else
					{
						Guid.TryParse(this.SiteGroupDropdown.SelectedValue, out sitegroupGuid);
					}

					//Hashtable htDataFilters = flc.GetFilters(base.Security, Convert.ToString(this.EntityTypeDropdown.SelectedValue));
					//string filterFieldName = null;
					//if (htDataFilters.Count == 1)
					//{
					//    //for each loop is necessary b/c hash table cannot be accessed by index.
					//    //this hash table should always have only one item, thus the break statement
					//    foreach (DictionaryEntry de in htDataFilters)
					//    {
					//        filterFieldName = Convert.ToString(de.Key);
					//        break;
					//    }
					//}

					string filterFieldName = null;
					if (Convert.ToString(this.EntityTypeDropdown.SelectedValue) == "Equipment")
					{
						filterFieldName = "EquipmentTypeGuid";
					}



					//string filterFieldName = ((this.FilterDropdown.SelectedIndex < 0) || (this.FilterDropdown.SelectedValue == "{All}"))
					//                             ? null
					//                             : this.FilterDropdown.SelectedValue;

					Guid filterValueGuid = Guid.Empty;
					bool ignoreFilterValues = false;
					if ((this.FilterValueDropdown.SelectedIndex >= 0) && (this.FilterValueDropdown.SelectedValue != "{All}"))
					{
						Guid.TryParse(this.FilterValueDropdown.SelectedValue, out filterValueGuid);
					}
					else
					{
						ignoreFilterValues = true;
					}
					string targetField = ((this.TargetFieldDropdown.SelectedIndex < 0)
												 || (this.TargetFieldDropdown.SelectedValue == "{All}"))
													? null
													: this.TargetFieldDropdown.SelectedValue;
					FieldLevelConfigClass.FIELD_CONTROL_MODE controlMode = ((this.ControlModeDropdown.SelectedIndex < 0)
																							  || (this.ControlModeDropdown.SelectedValue == "{All}"))
																								 ? FieldLevelConfigClass.FIELD_CONTROL_MODE.Unknown
																								 : (FieldLevelConfigClass.FIELD_CONTROL_MODE)
																									Enum.Parse(
																										typeof(FieldLevelConfigClass.FIELD_CONTROL_MODE),
																										this.ControlModeDropdown.SelectedValue);
					flcCollection =
						FMChannelHelper.MakeCall<IFieldLevelConfigMaps, FieldLevelConfigCollectionClass>(
							x =>
							x.GetFieldLevelConfigMatrix(
						this.Security,
						entityTypeId,
						sitegroupGuid,
						filterFieldName,
						ignoreFilterValues,
						filterValueGuid,
						targetField,
						controlMode,
								this.IncludeMemberSiteGroupsCheckBox.Checked));
				}

				this.FieldLevelConfigGrid.DataSource = this.GetDataView(flcCollection);
				this.Session[PageSessionKeyConstants.FLC_SESSION_DATA_MATRIX] = flcCollection;
				this.FieldLevelConfigGrid.DataBind();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
			finally
			{
				this.Security.SiteGuid = currentSiteGuid;
				this.Security.LoginSiteGuid = loginSiteGuid;
			}
		}

		#endregion
	}
}