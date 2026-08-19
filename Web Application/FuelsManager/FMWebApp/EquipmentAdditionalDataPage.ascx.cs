// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EquipmentAdditionalDataPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the EquipmentAdditionalDataPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
    using System;
    using System.Globalization;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    /// <summary>
	///    Summary description for EquipmentAdditionalDataPage.
	/// </summary>
	public partial class EquipmentAdditionalDataPage : EquipmentPageBase
	{
		#region Constants and Fields

		protected SiteClass CurrentSite;

		#endregion

		#region Public Methods and Operators

		public void UpdateData()
		{
			if (this.IssPtNumTextbox.Text.Trim().Length > 0)
			{
				this.Equipment.IssPtNum = this.IssPtNumTextbox.Text.Trim();
			}
			else
			{
				//If Issue Point not specified, use last two characters of the equipment id.
				string equipmentID = this.Equipment.ID.Trim();
				if (equipmentID.Length > 2)
				{
					this.Equipment.IssPtNum = equipmentID.Substring(equipmentID.Length - 2);
				}
				else
				{
					this.Equipment.IssPtNum = equipmentID;
				}
			}

			string fieldName = "";

			this.Equipment.FuelAdditiveFlag = this.FuelAdditiveCheckBox.Checked;
			this.Equipment.SecondaryStorageFlag = this.SecondaryStorageCheckBox.Checked;
			this.Equipment.ManagedEquipmentFlag = this.ManagedEquipmentCheckBox.Checked;

			try
			{
				fieldName = "RatedGPM";
                this.Equipment.RatedGPM = Convert.ToDouble(this.RatedGPMTextbox.Text, this.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.FLOW));

				fieldName = "ActualGPM";
                this.Equipment.ActualGPM = Convert.ToDouble(this.ActualGPMTextbox.Text, this.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.FLOW));

				fieldName = "Volume Units";
				this.Equipment.VolumeUnits = (EngineeringUnit)Convert.ToInt32(this.VolumeUnitsDropDownList.SelectedValue);

				fieldName = "Temperature Units";
				this.Equipment.TemperatureUnits = (EngineeringUnit)Convert.ToInt32(this.TemperatureUnitsDropDownList.SelectedValue);

				fieldName = "Density Units";
				this.Equipment.DensityUnits = (EngineeringUnit)Convert.ToInt32(this.DensityUnitsDropDownList.SelectedValue);

				fieldName = "Mass Units";
				this.Equipment.MassUnits = (EngineeringUnit)Convert.ToInt32(this.MassUnitsDropDownList.SelectedValue);

				fieldName = "Volume Decimal Places";
				this.Equipment.VolumeDecimalPlaces = Convert.ToInt16(this.VolumeDecimalPlacesTextbox.Text);

				fieldName = "Temperature Decimal Places";
				this.Equipment.TemperatureDecimalPlaces = Convert.ToByte(this.TemperatureDecimalPlacesTextbox.Text);

				fieldName = "Density Decimal Places";
				this.Equipment.DensityDecimalPlaces = Convert.ToByte(this.DensityDecimalPlacesTextbox.Text);

				fieldName = "Mass Decimal Places";
				this.Equipment.MassDecimalPlaces = Convert.ToByte(this.MassDecimalPlacesTextbox.Text);

				bool isDescKey = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDescKey());
				if (isDescKey)
				{
					EquipmentTypeClass equipmentType = FMChannelHelper.MakeCall<IEquipmentTypes, EquipmentTypeClass>(
															x => x.Get(this.Security, this.Equipment.EquipmentTypeGuid));
					if (equipmentType.Attribute == EQUIPMENT_TYPE.TANK_TYPE)
					{
						fieldName = "Tank Bottom";
						this.Equipment.LowStockWarning = string.IsNullOrEmpty(this.TankBottomTextbox.Text) ? "0" : this.TankBottomTextbox.Text;
					}
				}
			}
			catch (Exception)
			{
				string errMsg = "Field: '" + fieldName + "' must be numeric.";
				throw new Exception(errMsg);
			}

			this.Equipment.ManufactureDate = this.ManufactureDate.Text;
			this.Equipment.InstallationDate = this.InstallationDate.Text;
			this.Equipment.InspectionDate = this.InspectionDate.Text;
			this.Equipment.CalibrationDate = this.CalibrationDate.Text;
		}

		#endregion

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
				this.CurrentSite = FMChannelHelper.MakeCall<ISites, SiteClass>(
																 x =>
																 x.GetBasic(this.Security, this.Security.SiteGuid)
															);

				bool isDescKey = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDescKey());
				EquipmentTypeClass equipmentType = new EquipmentTypeClass();
				if (isDescKey)
				{
					this.IssPtNumTextbox.MaxLength = 2;
					equipmentType = FMChannelHelper.MakeCall<IEquipmentTypes, EquipmentTypeClass>(
															x => x.Get(this.Security, this.Equipment.EquipmentTypeGuid));
				}


				if (!this.Page.IsPostBack)
				{
					this.IssPtNumTextbox.Text = this.Equipment.IssPtNum;
					this.IssptTextbox.Text = this.Equipment.IssPt;
					this.RatedGPMTextbox.Text = this.Equipment.RatedGPM.ToString(this.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.FLOW));
                    this.ActualGPMTextbox.Text = this.Equipment.ActualGPM.ToString(this.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.FLOW));
					this.FuelAdditiveCheckBox.Checked = this.Equipment.FuelAdditiveFlag;
					this.SecondaryStorageCheckBox.Checked = this.Equipment.SecondaryStorageFlag;

					if (isDescKey && equipmentType.Attribute == EQUIPMENT_TYPE.TANK_TYPE)
					{
						this.TankBottomTextbox.Text = this.Equipment.LowStockWarning;
					}

					this.InitializeUnitsDropDownList(
						this.VolumeUnitsDropDownList, EngineeringUnit.FmvCm3, EngineeringUnit.FmvKl, this.Equipment.VolumeUnits);
					this.InitializeUnitsDropDownList(
						this.TemperatureUnitsDropDownList,
						EngineeringUnit.FmtDegC,
						EngineeringUnit.FmtDegR,
						this.Equipment.TemperatureUnits);
					this.InitializeUnitsDropDownList(
						this.DensityUnitsDropDownList, EngineeringUnit.FmdGcm3, EngineeringUnit.FmdSTnYd3, this.Equipment.DensityUnits);
					this.InitializeUnitsDropDownList(
						this.MassUnitsDropDownList, EngineeringUnit.FmmGram, EngineeringUnit.FmmMlbs, this.Equipment.MassUnits);

					this.VolumeDecimalPlacesTextbox.Text = this.Equipment.VolumeDecimalPlaces.ToString(CultureInfo.InvariantCulture);
					this.TemperatureDecimalPlacesTextbox.Text = this.Equipment.TemperatureDecimalPlaces.ToString(CultureInfo.InvariantCulture);
					this.DensityDecimalPlacesTextbox.Text = this.Equipment.DensityDecimalPlaces.ToString(CultureInfo.InvariantCulture);
					this.MassDecimalPlacesTextbox.Text = this.Equipment.MassDecimalPlaces.ToString(CultureInfo.InvariantCulture);

					DateTimeFormatInfo dateTimeFormatInfo = this.CurrentSite.GetDateTimeFormatInfo();

					if (this.Equipment._ManufactureDate.Value != DateTimeOffset.MinValue)
					{
						this.ManufactureDate.CurrentValue = DateTimeOffset.Parse(this.Equipment.ManufactureDate, dateTimeFormatInfo);
					}

					if (this.Equipment._InstallationDate.Value != DateTimeOffset.MinValue)
					{
						this.InstallationDate.CurrentValue = DateTimeOffset.Parse(this.Equipment.InstallationDate, dateTimeFormatInfo);
					}

					if (this.Equipment._InspectionDate.Value != DateTimeOffset.MinValue)
					{
						this.InspectionDate.CurrentValue = DateTimeOffset.Parse(this.Equipment.InspectionDate, dateTimeFormatInfo);
					}

					if (this.Equipment._CalibrationDate.Value != DateTimeOffset.MinValue)
					{
						this.CalibrationDate.CurrentValue = DateTimeOffset.Parse(this.Equipment.CalibrationDate, dateTimeFormatInfo);
					}

					this.ManufactureDate.FormatInfo = dateTimeFormatInfo;
					this.InstallationDate.FormatInfo = dateTimeFormatInfo;
					this.InspectionDate.FormatInfo = dateTimeFormatInfo;
					this.CalibrationDate.FormatInfo = dateTimeFormatInfo;

					if (this.Equipment.IdentityGuid == Guid.Empty && isDescKey )
					{
						this.Equipment.ManagedEquipmentFlag = this.Session[EquipmentForm.EquipmentAdditionalTabVisible] != null;

					}

					this.ManagedEquipmentCheckBox.Checked = this.Equipment.ManagedEquipmentFlag;

				    var pv = this.Session["ProcessVariable"] as ProcessVariableClass;
				    if (pv != null
					&& pv.ProcessVariableType == this.Equipment.VolumeProcessVariable.ProcessVariableType
					&& pv.InstanceNumber == this.Equipment.VolumeProcessVariable.InstanceNumber)
					{
						var editedProcessVariable = pv;
						this.Equipment.VolumeProcessVariable.Load(editedProcessVariable);
						this.Session.Remove("ProcessVariable");
					}
	
					
					
					this.VolumeHostNameTextbox.Text = this.Equipment.VolumeHostName;
					this.VolumeProgIDTextbox.Text = this.Equipment.VolumeProgID;
					this.VolumeItemIDTextbox.Text = this.Equipment.VolumeItemID;
				}
				else
				{
					string fieldName = "";

					try
					{
						fieldName = "Volume Units";
						this.Equipment.VolumeUnits = (EngineeringUnit)Convert.ToInt32(this.VolumeUnitsDropDownList.SelectedValue);

						fieldName = "Volume Decimal Places";
						this.Equipment.VolumeDecimalPlaces = Convert.ToInt16(this.VolumeDecimalPlacesTextbox.Text);

						if (isDescKey && equipmentType.Attribute == EQUIPMENT_TYPE.TANK_TYPE)
						{
							fieldName = "Tank Bottom";
							this.Equipment.LowStockWarning = string.IsNullOrEmpty(this.TankBottomTextbox.Text) ? "0" : this.TankBottomTextbox.Text;
						}

						foreach (EquipmentClass compartment in this.Equipment.CompartmentCollection)
						{
							compartment.VolumeUnits = this.Equipment.VolumeUnits;
							compartment.VolumeDecimalPlaces = this.Equipment.VolumeDecimalPlaces;
						}
					}
					catch (Exception)
					{
						string errMsg = "Field: " + fieldName + " must be numeric.";
						throw new Exception(errMsg);
					}
				}

				if (isDescKey && equipmentType.Attribute == EQUIPMENT_TYPE.TANK_TYPE)
				{
					this.TankBottomLabel.Visible = true;
					this.TankBottomTextbox.Visible = true;
				}
				else
				{
					this.TankBottomLabel.Visible = false;
					this.TankBottomTextbox.Visible = false;
				}

				this.SetFieldAccessibilityForChildRecordVersion();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void VolumeOutputButtonCommand(object sender, CommandEventArgs e)
		{
			this.Session["UnitForm"] = "EquipmentForm.aspx";
			var equipmentForm = this.Page as EquipmentForm;
			if (equipmentForm != null)
			{
				equipmentForm.UpdateData();
				this.Session["EquipmentFormTabIndex"] = equipmentForm.ActiveTabIndex;
			}
			this.Session["ProcessVariable"] = this.Equipment.VolumeProcessVariable;

			bool currentSiteOwnsRecordVersion = (this.Equipment.SiteGuid == this.Security.SiteGuid);
			if ((this.Equipment.IdentityGuid.Equals(Guid.Empty))
			    || (currentSiteOwnsRecordVersion && this.Equipment.IdentityGuid.Equals(this.Equipment.MasterRecordGuid)))
			{
				this.Redirect("OPCConnectionForm.aspx");
			}
			else if ((this.VersionSpecificFields != null) && (this.VersionSpecificFields.Count > 0))
            {
				this.Redirect("OPCConnectionForm.aspx?ISCHILDRECORDVERSION=true");
			}
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		private void SetFieldAccessibilityForChildRecordVersion()
		{
			bool currentSiteOwnsRecordVersion = (this.Equipment.SiteGuid == this.Security.SiteGuid);
			if (this.Equipment.IdentityGuid.Equals(Guid.Empty)
			    || (currentSiteOwnsRecordVersion && this.Equipment.IdentityGuid.Equals(this.Equipment.MasterRecordGuid))
			    || (this.VersionSpecificFields == null))
			{
				return;
			}

			this.ManagedEquipmentCheckBox.Enabled = (this.ManagedEquipmentCheckBox.Enabled 
			                                         && this.VersionSpecificFields.Contains("ManagedEquipmentFlag"));

			//Process Variable attributes are not covered by Equipment Record Versioning
			this.VolumeHostNameTextbox.Enabled = false;
			this.VolumeProgIDTextbox.Enabled = false;
			this.VolumeItemIDTextbox.Enabled = false;

			this.IssPtNumTextbox.Enabled = (this.IssPtNumTextbox.Enabled 
			                                && this.VersionSpecificFields.Contains("IssPtNum"));
			this.IssptTextbox.Enabled = false;
				//Cannot change the Equipment Type attributes of a child record version, because the Equipment Type is used as a filter field in the definition of the Equipment entity segment template.
			this.RatedGPMTextbox.Enabled = (this.RatedGPMTextbox.Enabled 
			                                && this.VersionSpecificFields.Contains("RatedGPM"));
			this.ActualGPMTextbox.Enabled = (this.ActualGPMTextbox.Enabled 
			                                 && this.VersionSpecificFields.Contains("ActualGPM"));
			this.FuelAdditiveCheckBox.Enabled = (this.FuelAdditiveCheckBox.Enabled 
			                                     && this.VersionSpecificFields.Contains("FuelAdditiveFlag"));
			this.SecondaryStorageCheckBox.Enabled = (this.VolumeItemIDTextbox.Enabled 
			                                         && this.VersionSpecificFields.Contains("SecondaryStorageFlag"));

			this.VolumeDecimalPlacesTextbox.Enabled = (this.VolumeDecimalPlacesTextbox.Enabled 
			                                           && this.VersionSpecificFields.Contains("VolumeDecimalPlaces"));
			this.TemperatureDecimalPlacesTextbox.Enabled = (this.TemperatureDecimalPlacesTextbox.Enabled
			                                                && this.VersionSpecificFields.Contains("TemperatureDecimalPlaces"));
			this.DensityDecimalPlacesTextbox.Enabled = (this.DensityDecimalPlacesTextbox.Enabled 
			                                            && this.VersionSpecificFields.Contains("DensityDecimalPlaces"));
			this.MassDecimalPlacesTextbox.Enabled = (this.MassDecimalPlacesTextbox.Enabled 
			                                         && this.VersionSpecificFields.Contains("MassDecimalPlaces"));

			this.VolumeUnitsDropDownList.Enabled = (this.VolumeUnitsDropDownList.Enabled 
			                                        && this.VersionSpecificFields.Contains("VolumeUnitIndex"));
			this.TemperatureUnitsDropDownList.Enabled = (this.TemperatureUnitsDropDownList.Enabled
			                                             && currentSiteOwnsRecordVersion
			                                             && this.VersionSpecificFields.Contains("TemperatureUnitIndex"));
			this.DensityUnitsDropDownList.Enabled = (this.DensityUnitsDropDownList.Enabled 
			                                         && this.VersionSpecificFields.Contains("DensityUnitIndex"));
			this.MassUnitsDropDownList.Enabled = (this.MassUnitsDropDownList.Enabled 
			                                      && this.VersionSpecificFields.Contains("MassUnitIndex"));

			this.ManufactureDate.Enabled = (this.ManufactureDate.Enabled 
			                                && this.VersionSpecificFields.Contains("ManufactureDate"));
			this.InstallationDate.Enabled = (this.InstallationDate.Enabled 
			                                 && this.VersionSpecificFields.Contains("InstallationDate"));
			this.InspectionDate.Enabled = (this.InspectionDate.Enabled 
			                               && this.VersionSpecificFields.Contains("InspectionDate"));
			this.CalibrationDate.Enabled = (this.CalibrationDate.Enabled 
			                                && this.VersionSpecificFields.Contains("CalibrationDate"));
		}

		#endregion
	}
}