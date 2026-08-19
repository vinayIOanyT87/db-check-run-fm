/******************************************************************************
	FILE NAME:	MaintenanceAddRecordForm.cs

	PURPOSE:		MaintenanceAddRecordForm

		Copyright (C) 2009 Varec, Inc.		All Rights Reserved		Norcross, GA

		This file shall not be copied or reproduced in any form without
		the express written consent of Varec, Inc.

		Date			By						Version		Reason
		----------	--------------		-------		-------------------------------
		2009-08-18	Larry Leonard		7.5.1			Creation for Iteration 7.
  
	  2009-09-25  A. Coker                      WI 7169 - Display operator name 
												rather than ID on Add Maintenance 
												Record page.  
 
	  2009-09-25  A. Coker                      WI 7140 - Display Maintenance Reason
												Description instead of ID. 
 
	  2009-09-26  A. Coker                      WI 7195 - Do not disable Operator
												(when equipment is selected)

	  2009-10-14  A. Coker                      WI 8156 - Updated security rights check
 
	  2009-10-22  A. Coker                      WI 8692 - Fixed issue with Return to Service date
												not getting set when user types in date rather
												than selecting it from the calendar.
 
	  2009-10-23  A. Coker                      Fixed null object refernece error caused by 
												an attempt to assign datetime to null Return To Service Date
 
	  2009-10-23  A.. Coker                     WI 8758 - Exclude empty Maintenance Reasons from dropdown list.
 
	  2009-10-25  A.. Coker                     WI 8757 - Moved default blank item to top of Asset ID combobox items.

*******************************************************************************/

namespace FuelsManager.MaintenanceWebApp
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.UtilityObjects;

    using FMCore;

	using FuelsManager.FMWebApp;

	public partial class MaintenanceAddRecordForm : FMFormBaseAjax
	{
		#region Data
		public static string MaintenancelogSessionKey = "MaintenanceAddRecord.MaintenanceAddRecordForm.MaintenanceLog";

		private const string EquipmentString = "EQUIPMENT";
		private const string TankString = "TANK";

		#endregion

		protected string Mode
		{
			get { return (string)this.ViewState["Mode"]; }
			set { this.ViewState["Mode"] = value.DefaultIfNullOrEmpty("VIEW"); }
		}

		/// <summary>
		/// Override to distinguish how the page is being used
		/// </summary>
		/// <returns>Key for lookup into tblHelpMapping</returns>
		public override string GetHelpContextKey()
		{
			return base.GetHelpContextKey() + "|" + this.Mode;
		}

      public override List<string> GetHelpContextKeys()
      {
         List<string> list = new List<string>() { base.GetHelpContextKey() + "|" + Mode };
         return list;
      }
        public EquipmentMaintenanceLogClass EquipmentMaintenanceLog
		{
			get
			{
				return this.Session[MaintenancelogSessionKey] as EquipmentMaintenanceLogClass;
			}

			set
			{
				this.Session[MaintenancelogSessionKey] = value;
			}
		}

		public TankMaintenanceLogClass TankMaintenanceLog
		{
			get
			{
				return this.Session[MaintenancelogSessionKey] as TankMaintenanceLogClass;
			}

			set
			{
				this.Session[MaintenancelogSessionKey] = value;
			}
		}

		#region Page State Management
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				// This is the first time through.
				if (!this.IsPostBack)
				{
					if (string.IsNullOrEmpty(this.Request.GetQueryOrFormValue("QUERYEDIT")) == false)
					{
						Guid entityGuid = Guid.Parse(this.Request.GetQueryOrFormValue("QUERYEDIT").Substring(1));
						char entityType = this.Request.GetQueryOrFormValue("QUERYEDIT")[0];

						if (entityType == 'E')
						{
							this.EquipmentMaintenanceLog = FMChannelHelper.MakeCall<IEquipmentMaintenanceLogs, EquipmentMaintenanceLogClass>(
								logs => logs.Get(this.Security, entityGuid));
						}
						else
						{
							this.TankMaintenanceLog = FMChannelHelper.MakeCall<ITankMaintenanceLogs, TankMaintenanceLogClass>(
								logs => logs.Get(this.Security, entityGuid));
						}

					}

					if (this.EquipmentMaintenanceLog == null && this.TankMaintenanceLog == null)
					{
						this.EquipmentMaintenanceLog = new EquipmentMaintenanceLogClass();
					}

					// Load the controls that use the View State.
					this.LoadAssetTypes();
					this.LoadMaintenanceReasons();

					// If an AJAX control, also check for callback.
					if (!this.IsCallback)
					{
						this.LoadEquipmentIdsOrTankIds();
						this.LoadPersonnelID();
					}

					this.Mode = this.Request.GetQueryOrFormValue("MODE");

					if (this.IsFromQueryWriter)
					{
						this.Mode = "VIEW";
					}

					this.TitleLabel.Text = "View Maintenance Record";
					this.AssetTypeDropdown.Enabled = false;
					this.AssetIDComboBox.Enabled = false;
					this.OkButton.Enabled = false;
					this.InServiceFMCheckBox.Enabled = false;
					this.EstimatedReturnFMDATE.Enabled = false;
					this.MaintenanceReasonFMCombobox.Enabled = false;
					this.MemoTextBox.Enabled = false;
					this.MaintenanceReasonFMCombobox.Enabled = false;
					this.WorkOrderTextBox.Enabled = false;
					this.PersonnelIDComboBox.Enabled = false;

					if (this.Security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD))
					{
						bool addMode = (this.Mode == "ADD");
						this.OkButton.Enabled = addMode;
						this.AssetTypeDropdown.Enabled = addMode;
						this.AssetIDComboBox.Enabled = addMode;
						this.MemoTextBox.Enabled = addMode;
						this.InServiceFMCheckBox.Enabled = addMode;
						this.WorkOrderTextBox.Enabled = addMode;
						this.PersonnelIDComboBox.Enabled = addMode;

						//Disable asset type and asset id dropdowns if equipment id has 
						//been provided.
						if (this.EquipmentMaintenanceLog != null)
						{
							if (!string.IsNullOrEmpty(this.EquipmentMaintenanceLog.EquipmentID))
							{
								this.AssetTypeDropdown.Enabled = false;
								this.AssetIDComboBox.Enabled = false;
							}
						}
						else if (this.TankMaintenanceLog != null)
						{
							if (!string.IsNullOrEmpty(this.TankMaintenanceLog.TankID))
							{
								this.AssetTypeDropdown.Enabled = false;
								this.AssetIDComboBox.Enabled = false;
							}
						}
					}
					else
					{
						this.Mode = "VIEW";
					}

					if (this.Mode == "ADD")
					{
						this.TitleLabel.Text = "Add Maintenance Record";
					}

					this.UpdateView();
				}

				SetModalDialog();
			}

			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}


		protected void UpdateView()
		{

			this.AssetTypeValueLabel.Text = "";

			if (this.EquipmentMaintenanceLog != null)
			{
				this.AssetTypeDropdown.SelectByText(EquipmentString);
			}
			else if (this.TankMaintenanceLog != null)
			{
				this.AssetTypeDropdown.SelectByText(TankString);
			}

			this.AssetIDLabel.Text = GetDataDictionaryValueByKey(this.Security.SiteGuid, this.AssetTypeDropdown.SelectedItem.Value);

			this.LoadHoursPassedLabel();
			this.UpdateForSelectedAsset();
			this.ApplyDataDictionary();
		}

		protected EquipmentClass Equipment
		{
			get
			{
				if (Session["ReturnPageFromMaintenanceAddRecordForm"] != null)
				{
					var equipment = Session["EQUIPMENT_SESSION_KEY"] as EquipmentClass;

					if (equipment != null)
					{
						return equipment;
					}
				}

				if (!string.IsNullOrEmpty(AssetIDComboBox.SelectedValue))
				{
					var assetGuid = Guid.Parse(this.AssetIDComboBox.SelectedValue);
					if (assetGuid != Guid.Empty)
					{
						return FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(x => x.Get(Security, assetGuid));
					}
				}
				return null;
			}
		}

		protected void SetModalDialog()
		{
			EquipmentClass equipment = Equipment;
			bool enableConfirmation = (!this.InServiceFMCheckBox.Checked && equipment != null && equipment.Type == EQUIPMENT_TYPE.TANK_TYPE);
			ModalPopupExtender1.Enabled = enableConfirmation;
			UpdatePanelModalDialog.Update();
		}

		protected void UpdateForSelectedAsset()
		{

			if (this.AssetTypeDropdown.SelectedItem.Text.Equals(EquipmentString))
			{
				this.PopulateControlsFromEquipmentMaintenanceLog();

			}
			else
			{
				this.PopulateControlsFromTankMaintenanceLog();
			}


		}

		// Overwrite the controls' text from the Dictionary.
		protected void ApplyDataDictionary()
		{
			this.AssetTypeLabel.Text = this.GetTranslatedText("Asset Type:");
			this.PersonnelIDLabel.Text = this.GetTranslatedText("Operator ID:");
			this.WorkorderLabel.Text = this.GetTranslatedText("Work Order:");
			this.AssetTypeLabel.Text = this.GetTranslatedText("Type:");
			this.InServiceFMCheckBox.Text = this.GetTranslatedText("In Service");
			this.MaintenanceReasonLabel.Text = this.GetTranslatedText("Maintenance Reason:");
			this.EstimatedReturnToServiceLabel.Text = this.GetTranslatedText("Estimated Return to Service:");
			this.MemoLabel.Text = this.GetTranslatedText("Memo:");
			this.DenotesLabel.Text = this.GetTranslatedText("* Denotes Required Field");
			this.OkButton.Text = this.GetTranslatedText("OK");
			this.CancelButton.Text = this.GetTranslatedText("Cancel");

			// Has to be built dynamically.
			// HoursPassedLabel.Text = this.m_Dictionaries.Get(Security.SiteGuid,"...
		}

		// Used for coming from Equipment Maintenance Log Form.
		private void PopulateControlsFromEquipmentMaintenanceLog()
		{
			EquipmentMaintenanceLogClass oEquipmentMaintenanceLog = this.EquipmentMaintenanceLog;

			this.AssetTypeDropdown.SelectByText(EquipmentString);


			this.WorkOrderTextBox.Text = oEquipmentMaintenanceLog.WorkOrder;
			this.MemoTextBox.Text = oEquipmentMaintenanceLog.Memo;
			this.InServiceFMCheckBox.Checked = (oEquipmentMaintenanceLog.InServiceFlag == 1);

			if (oEquipmentMaintenanceLog.OperatorPersonnelGuid != Guid.Empty)
			{
				foreach (ListItem li in this.PersonnelIDComboBox.Items)
				{
					var personnelGuid = Guid.Parse(li.Value);
					if (personnelGuid.Equals(oEquipmentMaintenanceLog.OperatorPersonnelGuid))
					{
						PersonClass person = FMChannelHelper.MakeCall<IPersonnel, PersonClass>(
							personnel => personnel.Get(this.Security, personnelGuid));
						this.PersonnelIDComboBox.SelectByText(person.FullName);
						break;
					}
				}
			}
			else
			{
				this.PersonnelIDComboBox.SelectedIndex = 0;
			}

			this.AssetIDComboBox.SelectByText(oEquipmentMaintenanceLog.EquipmentID);
			this.AssetTypeValueLabel.Text = oEquipmentMaintenanceLog.EquipmentType;
			this.UpdateForInServiceState();
		}

		// Used for coming from Tank Maintenance Log Form.??
		private void PopulateControlsFromTankMaintenanceLog()
		{
			TankMaintenanceLogClass oTankMaintenanceLog = this.TankMaintenanceLog;

			this.AssetTypeDropdown.SelectByText(TankString);

			this.WorkOrderTextBox.Text = oTankMaintenanceLog.WorkOrder;
			this.MemoTextBox.Text = oTankMaintenanceLog.Memo;
			this.InServiceFMCheckBox.Checked = (oTankMaintenanceLog.InServiceFlag.Equals(1));

			this.PersonnelIDComboBox.SelectByText("");
			if (oTankMaintenanceLog.OperatorPersonnelGuid != Guid.Empty)
			{
				foreach (ListItem li in this.PersonnelIDComboBox.Items)
				{
					var personnelGuid = Guid.Parse(li.Value);
					if (personnelGuid == oTankMaintenanceLog.OperatorPersonnelGuid)
					{
						PersonClass person = FMChannelHelper.MakeCall<IPersonnel, PersonClass>(
							personnel => personnel.Get(this.Security, personnelGuid));
						this.PersonnelIDComboBox.SelectByText(person.FullName);
						break;
					}
				}
			}

			this.AssetIDComboBox.SelectByText(oTankMaintenanceLog.TankID);
			this.AssetTypeValueLabel.Text = oTankMaintenanceLog.VesselType;
			this.UpdateForInServiceState();
		}

		protected void ResetControls()
		{
			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
					x =>
					x.Get(this.Security, this.Security.SiteGuid, getMemberSites: true, getSchedulesAndProcessVariables: true,
							bGetAssociatedAliases: true)
			);


			// Clear controls' text. They want the AssetType combobox to have a value.
			this.AssetIDLabel.Text = "";
			this.AssetIDComboBox.SelectByText("");
			this.AssetTypeValueLabel.Text = "";
			this.PersonnelIDComboBox.SelectedIndex = -1;
			this.WorkOrderTextBox.Text = "";
			this.HoursPassedLabel.Text = "";
			this.MemoTextBox.Text = "";
			this.InServiceFMCheckBox.Checked = true;
			this.MaintenanceReasonFMCombobox.SelectByText("");
			this.EstimatedReturnFMDATE.CurrentValue = TimeConverter.Now(site);

			// Clear session objects.
			this.Session[MaintenancelogSessionKey] = new EquipmentMaintenanceLogClass();
		}

		// Saving from controls to objects, thence to database.
		protected void WriteToDatabase()
		{
			string sAssetTypeSelected = this.AssetTypeDropdown.SelectedItem.Text;

			if (this.PersonnelIDComboBox.SelectedItem == null)
			{
				ErrorHandler(new Exception("Must select personnel ID."));
			}
			else
			{

				var personnelGuid = Guid.Parse(this.PersonnelIDComboBox.SelectedItem.Value);
				PersonClass person =
					FMChannelHelper.MakeCall<IPersonnel, PersonClass>(personnel => personnel.Get(this.Security, personnelGuid));

				var maintenanceReasonGuid = Guid.Parse(this.MaintenanceReasonFMCombobox.SelectedItem.Value);
				MaintenanceReasonClass reason =
					FMChannelHelper.MakeCall<IMaintenanceReasons, MaintenanceReasonClass>(
						reasons => reasons.Get(this.Security, maintenanceReasonGuid));

				ListItem li = this.AssetIDComboBox.SelectedItem;

				if (String.IsNullOrEmpty(li.Text))
				{
					return;
				}

				if (sAssetTypeSelected.Equals(EquipmentString))
				{
					this.WriteEquipmentMaintenanceLogToDatabase(person, reason);
				}
				else if (sAssetTypeSelected.Equals(TankString))
				{
					this.WriteTankMaintenanceLogToDatabase(person, reason);
				}
			}
		}

		protected void WriteEquipmentMaintenanceLogToDatabase(PersonClass person, MaintenanceReasonClass reason)
		{
			EquipmentMaintenanceLogClass oEquipmentMaintenanceLog = this.EquipmentMaintenanceLog;

			if (person != null)
			{
				oEquipmentMaintenanceLog.OperatorPersonnelGuid = person.MasterRecordGuid;
				oEquipmentMaintenanceLog.OperatorID = person.ID;
			}

			if (this.InServiceFMCheckBox.Checked)
			{
				oEquipmentMaintenanceLog.MaintenanceReasonGuid = Guid.Empty;
				oEquipmentMaintenanceLog.MaintenanceReason = "";
			}
			else if (reason != null)
			{
				oEquipmentMaintenanceLog.MaintenanceReasonGuid = reason.IdentityGuid;
				oEquipmentMaintenanceLog.MaintenanceReason = reason.Description;
			}

			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
					x =>
					x.Get(this.Security, this.Security.SiteGuid, getMemberSites: true, getSchedulesAndProcessVariables: true,
							bGetAssociatedAliases: true)
				);

			DateTimeOffset siteTimeNow = TimeConverter.Now(site);

			oEquipmentMaintenanceLog.InServiceFlag = (this.InServiceFMCheckBox.Checked) ? (byte)1 : (byte)0;
			oEquipmentMaintenanceLog.EstReturnToServiceDate = (this.InServiceFMCheckBox.Checked) ? siteTimeNow : this.EstimatedReturnFMDATE.CurrentValue;

			oEquipmentMaintenanceLog.WorkOrder = this.WorkOrderTextBox.Text;
			oEquipmentMaintenanceLog.Memo = this.MemoTextBox.Text;
			oEquipmentMaintenanceLog.ChangeDate = siteTimeNow;

			oEquipmentMaintenanceLog.IdentityGuid = Guid.Empty;
			FMChannelHelper.MakeCall<IEquipmentMaintenanceLogs>(logs => logs.Add(this.Security, oEquipmentMaintenanceLog));
		}

		protected void WriteTankMaintenanceLogToDatabase(PersonClass person, MaintenanceReasonClass reason)
		{
			TankMaintenanceLogClass oTankMaintenanceLog = this.TankMaintenanceLog;

			if (person != null)
			{
				oTankMaintenanceLog.OperatorPersonnelGuid = person.MasterRecordGuid;
				oTankMaintenanceLog.OperatorID = person.ID;
			}

			if (this.InServiceFMCheckBox.Checked)
			{
				oTankMaintenanceLog.MaintenanceReasonGuid = Guid.Empty;
				oTankMaintenanceLog.MaintenanceReason = "";
			}
			else if (reason != null)
			{
				oTankMaintenanceLog.MaintenanceReasonGuid = reason.IdentityGuid;
				oTankMaintenanceLog.MaintenanceReason = reason.Description;
			}

			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
					x =>
					x.Get(this.Security, this.Security.SiteGuid, getMemberSites: true, 
							getSchedulesAndProcessVariables: true, bGetAssociatedAliases: true)
			);

			DateTimeOffset siteTimeNow = TimeConverter.Now(site);

			oTankMaintenanceLog.InServiceFlag = (this.InServiceFMCheckBox.Checked) ? 1 : 0;
			oTankMaintenanceLog.EstReturnToServiceDate = (this.InServiceFMCheckBox.Checked) ? siteTimeNow : this.EstimatedReturnFMDATE.CurrentValue;

			oTankMaintenanceLog.WorkOrder = this.WorkOrderTextBox.Text;
			oTankMaintenanceLog.Memo = this.MemoTextBox.Text;
			oTankMaintenanceLog.ChangeDate = siteTimeNow;

			oTankMaintenanceLog.IdentityGuid = Guid.Empty;
			FMChannelHelper.MakeCall<ITankMaintenanceLogs>(logs => logs.Add(this.Security, oTankMaintenanceLog));
		}

		#endregion

		#region AssetType - Loads the Asset Type combobox ("EQUIPMENT" or "TANK").
		protected void LoadAssetTypes()
		{
			var assetTypes = new ArrayList { new ListItem(EquipmentString, "Equipment ID:"), new ListItem(TankString, "Tank ID:") };

			this.AssetTypeDropdown.Items.Clear();
			this.AssetTypeDropdown.DataSource = assetTypes;
			this.AssetTypeDropdown.DataTextField = "Text";
			this.AssetTypeDropdown.DataValueField = "Value";
			this.AssetTypeDropdown.DataBind();

			if (this.TankMaintenanceLog != null)
				this.AssetTypeDropdown.SelectByText(TankString);
			else
				this.AssetTypeDropdown.SelectByText(EquipmentString);

		}

		// Changed from "EQUIPMENT" to "TANK", or vv.
		protected void AssetTypeDropdownSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				if (this.AssetTypeDropdown.SelectedItem.Text == EquipmentString)
				{
					this.EquipmentMaintenanceLog = new EquipmentMaintenanceLogClass();
				}
				else
				{
					this.TankMaintenanceLog = new TankMaintenanceLogClass();
				}
				this.LoadEquipmentIdsOrTankIds();
				this.AssetIDComboBoxSelectedIndexChanged(sender, e);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		#endregion

		#region EquipmentOrTankID - Loads the Equipment IDs.

		// The big list of model numbers has changed selection.
		protected void AssetIDComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				this.Session.Remove(MaintenancelogSessionKey);

				this.LoadPersonnelID();

				if (this.AssetTypeDropdown.SelectedItem.Text == EquipmentString)
				{
					this.AssetIDComboBoxEquipmentSelectedIndexChanged();
				}
				else
				{
					// Tank.
					this.AssetIDComboBoxTankSelectedIndexChanged();
				}

				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}

		}

		protected void AssetIDComboBoxEquipmentSelectedIndexChanged()
		{
			Guid assetGuid = Guid.Parse(this.AssetIDComboBox.SelectedValue);

			EquipmentMaintenanceLogClass equipmentMaintenanceLog = assetGuid == Guid.Empty || this.Mode.Equals("ADD")
				? null : FMChannelHelper.MakeCall<IEquipmentMaintenanceLogs, EquipmentMaintenanceLogClass>(
							logs => logs.Get(this.Security, assetGuid));

			if (equipmentMaintenanceLog == null)
			{
				EquipmentClass equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
					equipments => equipments.Get(this.Security, assetGuid));
				if (equipment != null)
				{
					equipmentMaintenanceLog = new EquipmentMaintenanceLogClass
					                          {
						                          EquipmentGuid = equipment.MasterRecordGuid,
						                          EquipmentType = equipment.EqTypeName,
						                          EquipmentID = equipment.ID,
						                          InServiceFlag =
							                          Convert.ToByte(equipment.InServiceFlag)
					                          };

					// set the in service flage based on the equipment status
					this.InServiceFMCheckBox.Checked = equipment.InServiceFlag;
				}
			}
			else
			{
				equipmentMaintenanceLog.IdentityGuid = Guid.Empty;
			}

			if (equipmentMaintenanceLog == null)
			{
				this.EquipmentMaintenanceLog = new EquipmentMaintenanceLogClass();
			}
			else
			{
				this.EquipmentMaintenanceLog = equipmentMaintenanceLog;
			}
		}

		protected void AssetIDComboBoxTankSelectedIndexChanged()
		{
			// Tank.
			var assetGuid = Guid.Parse(this.AssetIDComboBox.SelectedValue);

			TankMaintenanceLogClass tankMaintenanceLog = assetGuid == Guid.Empty || this.Mode.Equals("ADD")
				? null : FMChannelHelper.MakeCall<ITankMaintenanceLogs, TankMaintenanceLogClass>(
					logs => logs.GetByTankGuid(this.Security, assetGuid));
			if (tankMaintenanceLog == null)
			{
				TankClass tank = FMChannelHelper.MakeCall<ITanks, TankClass>(
					tanks => tanks.Get(this.Security, assetGuid));
				if (tank != null)
				{
					tankMaintenanceLog = new TankMaintenanceLogClass();
					tankMaintenanceLog.TankGuid = tank.IdentityGuid;
					tankMaintenanceLog.LookupVesselTypeIndex = tank.VesselType;
					tankMaintenanceLog.VesselType = TankClass.VesselTypeID(tank.VesselType);
					tankMaintenanceLog.TankID = tank.ID;
				}
			}
			else
			{
				tankMaintenanceLog.IdentityGuid = Guid.Empty;
			}

			if (tankMaintenanceLog == null)
			{
				this.TankMaintenanceLog = new TankMaintenanceLogClass();
			}
			else
			{
				this.TankMaintenanceLog = tankMaintenanceLog;
			}
		}

		//
		protected void LoadEquipmentIdsOrTankIds()
		{
			string sAssetTypeSelected = this.AssetTypeDropdown.SelectedItem.Text;

			// Create the collection of Equipments or Tanks.
			if (sAssetTypeSelected == EquipmentString)
			{
				this.LoadEquipmentIds();

			}
			else if (sAssetTypeSelected == TankString)
			{
				this.LoadTankIds();

			}
			else
			{
				throw new Exception("Asset type " + sAssetTypeSelected + " not found.");
			}


		}

		protected void LoadEquipmentIds()
		{
			this.AssetIDComboBox.Clear();
			Guid equipmentGuid = FMChannelHelper.MakeCall<IEquipments, Guid>(
				equipments => equipments.GetIdentityGuid(this.Security, this.EquipmentMaintenanceLog.EquipmentID));

			if (equipmentGuid == Guid.Empty && this.EquipmentMaintenanceLog.EquipmentID.Trim().Length > 0)
				//Implies the equipment has been removed
				this.Mode = "VIEW";

			ListItem li;

			if (this.Mode != "VIEW")
			{
				EquipmentCollectionClass oEquipmentCollectionClass = FMChannelHelper.MakeCall<IEquipments, EquipmentCollectionClass>(
					equipments => equipments.EnumerateManagedEquipment(this.Security));

				// And add them to the droplist.
				li = new ListItem("", Guid.Empty.ToString());
				this.AssetIDComboBox.Items.Add(li);

				foreach (EquipmentClass oEquipment in oEquipmentCollectionClass)
				{
					li = new ListItem(oEquipment.ID, oEquipment.MasterRecordGuid.ToString());
					this.AssetIDComboBox.Items.Add(li);
				}
			}
			else
			{
				if (equipmentGuid == Guid.Empty)
				{
					li = new ListItem(this.EquipmentMaintenanceLog.EquipmentID, Guid.Empty.ToString());
				}
				else
				{
					EquipmentClass oEquipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
						equipments => equipments.Get(this.Security, equipmentGuid));
					li = new ListItem(oEquipment.ID, oEquipment.MasterRecordGuid.ToString());
				}

				this.AssetIDComboBox.Items.Add(li);

			}

			// Set to empty initially.
			this.AssetIDComboBox.SelectByText("");


		}

		protected void LoadTankIds()
		{
			this.AssetIDComboBox.Clear();

			Guid identityGuid = FMChannelHelper.MakeCall<ITanks, Guid>(
				tanks => tanks.GetIdentityGuid(this.Security, this.TankMaintenanceLog.TankID));

			if (identityGuid == Guid.Empty && this.TankMaintenanceLog.TankID.Trim().Length > 0)
			{
				this.Mode = "VIEW";
			}

			ListItem li;

			if (this.Mode != "VIEW")
			{
				TankCollectionClass oTankCollectionClass = FMChannelHelper.MakeCall<ITanks, TankCollectionClass>(
					tanks => tanks.Enumerate(this.Security));

				// And add them to the droplist.
				li = new ListItem("", Guid.Empty.ToString());
				this.AssetIDComboBox.Items.Add(li);

				foreach (TankClass oTank in oTankCollectionClass)
				{
					li = new ListItem(oTank.ID, oTank.IdentityGuid.ToString());
					this.AssetIDComboBox.Items.Add(li);
				}
			}
			else
			{
				if (identityGuid == Guid.Empty)
				{
					li = new ListItem(this.TankMaintenanceLog.TankID, Guid.Empty.ToString()); //was "-1"
				}
				else
				{
					TankClass oTank = FMChannelHelper.MakeCall<ITanks, TankClass>(tanks => tanks.Get(this.Security, identityGuid));
					li = new ListItem(oTank.ID, oTank.IdentityGuid.ToString());
				}

				this.AssetIDComboBox.Items.Add(li);
			}

			// Set to empty initially.
			this.AssetIDComboBox.SelectByText("");

		}


		#endregion
		#region PersonnelID
		protected void LoadPersonnelID()
		{
			// Collection of PersonClass objects.
			var oPersonnelCollectionClass = (PersonCollectionClass)this.Session["Maint.oPersonnelCollectionClass"];
			if (oPersonnelCollectionClass == null)
			{
				oPersonnelCollectionClass = FMChannelHelper.MakeCall<IPersonnel, PersonCollectionClass>(
					personnel => personnel.EnumerateByRoleSortByName(this.Security, PERSON_ROLE.MAX_PERSON_ROLE));
				this.Session["Maint.oPersonnelCollectionClass"] = oPersonnelCollectionClass;
			}

			// And add them to the droplist.
			this.PersonnelIDComboBox.DropDownStyle = AjaxControlToolkit.ComboBoxStyle.DropDownList;
			this.PersonnelIDComboBox.Clear();
			this.PersonnelIDComboBox.DataTextField = "FullName";
			this.PersonnelIDComboBox.DataValueField = "IdentityGuid";
			this.PersonnelIDComboBox.DataSource = oPersonnelCollectionClass;
			this.PersonnelIDComboBox.DataBind();

			this.PersonnelIDComboBox.Items.Insert(0, new ListItem(" ", Guid.Empty.ToString()));

		}

		#endregion

		#region Hours Passed Since Last Change
		// Relies on _LoadEquipmentIdsOrTankIds() to get EquipmentGuid.
		protected void LoadHoursPassedLabel()
		{
			string sAssetTypeSelected = this.AssetTypeDropdown.SelectedItem.Text;
			int nHoursPassed = 0;

			if (sAssetTypeSelected == EquipmentString)
			{
				nHoursPassed = FMChannelHelper.MakeCall<IEquipmentMaintenanceLogs, int>(
					logs => logs.GetHoursPassed(this.Security, this.EquipmentMaintenanceLog));
			}
			else if (sAssetTypeSelected == TankString)
			{
				nHoursPassed = FMChannelHelper.MakeCall<ITankMaintenanceLogs, int>(
					logs => logs.GetHoursPassed(this.Security, this.TankMaintenanceLog));
			}

			Debug.Assert(0 <= nHoursPassed, "Can't be negative - what's going on?");

			this.HoursPassedLabel.Text = nHoursPassed + " hour"
				+ (nHoursPassed == 1 ? " " : "(s) ")
				+ (nHoursPassed == 1 ? "has" : "have")
				+ " passed since the last change in maintenance status";
		}

		#endregion

		#region In Service
		protected void InServiceFMCheckBoxCheckedChanged(object sender, EventArgs e)
		{
			try
			{
				this.UpdateForInServiceState();
			}

			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		protected void UpdateForInServiceState()
		{
			string sAssetTypeSelected = this.AssetTypeDropdown.SelectedItem.Text;
			if (sAssetTypeSelected == EquipmentString)
			{
				this.UpdateForEquipmentInServiceState();
			}
			else//TANK
			{

				this.UpdateForTankInServiceState();
			}

			if (this.Mode == "ADD")
			{
				// If the In Service checkbox is not checked, then these controls be enabled; else, disabled.
				this.MaintenanceReasonFMCombobox.Enabled = !this.InServiceFMCheckBox.Checked;
				this.EstimatedReturnFMDATE.Enabled = !this.InServiceFMCheckBox.Checked;
				this.MaintenanceReasonStar.Enabled = !this.InServiceFMCheckBox.Checked;
			}
		}


		protected void UpdateForEquipmentInServiceState()
		{
			MaintenanceReasonClass reason = null;
			EquipmentMaintenanceLogClass oEquipmentMaintenanceLog = this.EquipmentMaintenanceLog;
			oEquipmentMaintenanceLog.InServiceFlag = (byte)(this.InServiceFMCheckBox.Checked ? 1 : 0);
			this.MaintenanceReasonFMCombobox.SelectByText("");
			if (!this.InServiceFMCheckBox.Checked)
			{
				foreach (ListItem li in this.MaintenanceReasonFMCombobox.Items)
				{
					Guid maintenanceReasonGuid = Guid.Parse(li.Value);
					if (maintenanceReasonGuid == oEquipmentMaintenanceLog.MaintenanceReasonGuid)
					{
						reason = FMChannelHelper.MakeCall<IMaintenanceReasons, MaintenanceReasonClass>(
							reasons => reasons.Get(this.Security, maintenanceReasonGuid));
						if (reason != null)
						{
							this.MaintenanceReasonFMCombobox.SelectByText(reason.Description);
							break;
						}
					}
				}
			}

			if (oEquipmentMaintenanceLog.InServiceFlag == 0)
			{
				this.EstimatedReturnFMDATE.CurrentValue = oEquipmentMaintenanceLog.EstReturnToServiceDate;
				if (reason == null && this.Mode == "VIEW")
				{
					this.MaintenanceReasonFMCombobox.Items.Add(oEquipmentMaintenanceLog.MaintenanceReason);
					this.MaintenanceReasonFMCombobox.SelectByText(oEquipmentMaintenanceLog.MaintenanceReason);
				}
			}
		}

		protected void UpdateForTankInServiceState()
		{
			MaintenanceReasonClass reason = null;
			TankMaintenanceLogClass oTankMaintenanceLog = this.TankMaintenanceLog;
			oTankMaintenanceLog.InServiceFlag = (byte)(this.InServiceFMCheckBox.Checked ? 1 : 0);
			this.MaintenanceReasonFMCombobox.SelectByText("");
			if (!this.InServiceFMCheckBox.Checked)
			{
				foreach (ListItem li in this.MaintenanceReasonFMCombobox.Items)
				{
					Guid maintenanceReasonGuid = Guid.Parse(li.Value);
					if (maintenanceReasonGuid == oTankMaintenanceLog.MaintenanceReasonGuid)
					{
						reason = FMChannelHelper.MakeCall<IMaintenanceReasons, MaintenanceReasonClass>(
							reasons => reasons.Get(this.Security, maintenanceReasonGuid));
						this.MaintenanceReasonFMCombobox.SelectByText(reason.Description);
						break;
					}
				}
			}

			if (oTankMaintenanceLog.InServiceFlag == 0)
			{
				this.EstimatedReturnFMDATE.CurrentValue = oTankMaintenanceLog.EstReturnToServiceDate;
				if (reason == null && this.Mode == "VIEW")
				{
					this.MaintenanceReasonFMCombobox.Items.Add(oTankMaintenanceLog.MaintenanceReason);
					this.MaintenanceReasonFMCombobox.SelectByText(oTankMaintenanceLog.MaintenanceReason);
				}
			}
		}



		#endregion

		#region MaintenanceReasons
		// Load the combobox from the database table tblMaintenanceReasons.
		protected void LoadMaintenanceReasons()
		{
			MaintenanceReasonCollectionClass oMaintenanceReasonCollectionClass =
				FMChannelHelper.MakeCall<IMaintenanceReasons, MaintenanceReasonCollectionClass>(
					reasons => reasons.EnumerateBySite(this.Security));

			// And add them to the droplist.
			this.MaintenanceReasonFMCombobox.Clear();
			ListItem li;
			foreach (MaintenanceReasonClass oMaintenanceReason in oMaintenanceReasonCollectionClass)
			{
				if (oMaintenanceReason.Description != null && oMaintenanceReason.Description.Trim().Length > 0)
				{
					li = new ListItem(oMaintenanceReason.Description, oMaintenanceReason.IdentityGuid.ToString());
					this.MaintenanceReasonFMCombobox.Items.Add(li);
				}
			}

			li = new ListItem("", Guid.Empty.ToString());
			this.MaintenanceReasonFMCombobox.Items.Add(li);
			this.MaintenanceReasonFMCombobox.SelectByText("");
		}

		#endregion

		#region Button Row Controls Message Handlers

		protected void YesButtonClick(object sender, EventArgs e)
		{
			this.OkButtonCompleteProcessing("YES");
		}
		protected void NoButtonClick(object sender, EventArgs e)
		{
			this.OkButtonCompleteProcessing("NO");
		}

		protected void OkButtonClick(object sender, EventArgs e)
		{
			try
			{
				OkButtonCompleteProcessing(string.Empty);
			}
			catch (Exception exception)
			{
				ErrorHandler(exception);
			}
		}

		
		private void OkButtonCompleteProcessing(string result)
		{
			string returnTo;
			try
			{
				this.WriteToDatabase();
				returnTo = this.Session["ReturnPageFromMaintenanceAddRecordForm"] as string;
				if (returnTo != null)
				{
					this.Session.Remove("ReturnPageFromMaintenanceAddRecordForm");
					var equipment = this.Session["EQUIPMENT_SESSION_KEY"] as EquipmentClass;
					if (equipment != null)
					{
						equipment.InServiceFlag = this.InServiceFMCheckBox.Checked;
						if (equipment._ReturnToServiceDate == null)
						{
							equipment._ReturnToServiceDate = new Date();
						}
						if (!equipment.InServiceFlag)
						{
							equipment._ReturnToServiceDate.Value = this.EstimatedReturnFMDATE.CurrentValue;
						}
						else
						{
							SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
										x =>
										x.Get(this.Security, this.Security.SiteGuid, getMemberSites: true,
												getSchedulesAndProcessVariables: true, bGetAssociatedAliases: true)
								);

							equipment._ReturnToServiceDate.Value = TimeConverter.Now(site);
						}

						equipment.InServiceFlag = this.InServiceFMCheckBox.Checked;
						equipment.MaintenanceNote = this.MemoTextBox.Text;
						equipment.StatusDescription = this.MaintenanceReasonFMCombobox.SelectedItem.Text;
						
						if (result != string.Empty)
						{
							equipment.InUse = result.Equals("YES");
						}
						else
						{
							//If you are here equipment is either not a Tank or it is in service.
							equipment.InUse = this.InServiceFMCheckBox.Checked;	
						}
						FMChannelHelper.MakeCall<IEquipments>(x => x.Modify(this.Security, equipment));

					}

					this.ResetControls();

				}
				else
				{
						EquipmentClass equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
							x => x.Get(Security, Guid.Parse(AssetIDComboBox.SelectedValue)));

					if (equipment != null)
					{
						if (result != string.Empty)
						{
							equipment.InUse = result.Equals("YES");
						}
						else
						{
							//If you are here equipment is either not a Tank or it is in service.
							equipment.InUse = this.InServiceFMCheckBox.Checked;
						}
						FMChannelHelper.MakeCall<IEquipments>(x => x.Modify(this.Security, equipment));
					}

					this.ResetControls();
					returnTo = "MaintenanceAddRecordForm.aspx?MODE=ADD";
				}
			}
			catch (Exception exception)
			{
				this.ErrorHandler(exception);
				return;
			}

			if (this.IsFromQueryWriter)
			{
				this.Redirect("..\\QueryWriterWebApp\\QueryResultsForm.aspx?Mode=Returning");
			}
			else
			{
				this.Redirect(returnTo);
			}

		}

		protected void CancelButtonClick(object sender, EventArgs e)
		{
			try
			{
				this.ResetControls();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}

			if (this.IsFromQueryWriter)
			{
				this.Redirect("..\\QueryWriterWebApp\\QueryResultsForm.aspx?Mode=Returning");
			}
			else
			{
				var returnTo = this.Session["ReturnPageFromMaintenanceAddRecordForm"] as string;
				if (returnTo != null)
				{
					this.Session.Remove("ReturnPageFromMaintenanceAddRecordForm");
					this.Redirect(returnTo);
				}
				else
				{
					if (this.OkButton.Enabled)
					{
						this.Redirect("MaintenanceAddRecordForm.aspx?MODE=ADD");
					}
					else
					{
						this.Redirect("MaintenanceAddRecordForm.aspx");
					}
				}
			}

		}

		#endregion

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();

			// Want to ignore the disabling of inputs on post backs.
			this.IgnoreInputDisable = true;
			base.OnInit(e);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.YesButton.OnClientClick = String.Format("fnModalBtnClick('{0}','{1}')", this.YesButton.UniqueID, "");
			this.NoButton.OnClientClick = String.Format("fnModalBtnClick('{0}','{1}')", this.NoButton.UniqueID, "");
		}

		#endregion
	}
}
