// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProfileValidationRuleSettingPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ProfileValidationRuleSettingPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;

	using FMControls;

	using global::FMWebApp;

	/// <summary>
	///    This class handles the functionality for the Profile Validation Rule Setting tab page.
	/// </summary>
	public partial class ProfileValidationRuleSettingPage : FMUserControlBase
	{
		#region Constants and Fields

		private const string DropdownDisplay1Option1 = "Not Displayed, Compared to User Entry";

		private const string DropdownDisplay1Option2 = "Displayed, no edit";

		private const string DropdownDisplay1Option3 = "Displayed, user may edit";

		private const string DropdownDisplay2Option1 = "Not Required";

		private const string DropdownDisplay2Option2 = "Required";

		private const string DropdownDisplay2Option3 = "Required & Allow Override";

		private const string DropdownDisplay3Option1 = "Display & Non-Edit";

		private const string DropdownDisplay3Option2 = "User Entry with Overriding";

		private const string DropdownDisplay3Option3 = "User Entry without Overriding";

		private const string DropdownDisplay3Option4 = "Editing with Overriding";

		private const string DropdownDisplay3Option5 = "Editing without Overriding";

		private const string DropdownDisplay4Option1 = "Not Required";

		private const string DropdownDisplay4Option2 = "Required";

		private const string DropdownDisplay4Option3 = "Required & Allow Override";

		private const string DropdownDisplay5Option1 = "Not Required";

		private const string DropdownDisplay5Option2 = "Required with Override";

		private const string DropdownDisplay5Option3 = "Required without Override";

		private const string OffStr = "Off";

		private const string OnStr = "On";

		private const string OnWithEaStr = "On with Error Action";

		/// <summary>
		///    The mobile device profile.
		/// </summary>
		private MobileDeviceProfile mobileDeviceProfile;

		#endregion

		#region Public Methods and Operators

		/// <summary>
		///    This method will reset all the fields when the new button is
		///    selected.
		/// </summary>
		public void ResetFieldsForNewEvent()
		{
			this.LoadRuleDropdowns();
			this.SelectTheDropdownItem();
			this.SetOddDropdownSelections();
			this.SetErrorActionCheckboxes();

			this.DisableErrorActionControls();
			this.DisableFields();
		}

		/// <summary>
		///    This method retrieves the data from the page.  It is called by the Profile
		///    Configuration form.
		/// </summary>
		public void UpdateChanges()
		{
			if (this.mobileDeviceProfile != null)
			{
				this.RetrieveDataFromPage();
			}
		}

		#endregion

		#region Methods

		/// <summary>
		///    This method handles the Aircraft Type Verification dropdown on change event.
		///    It will enable or disable the associated Error Action checkbox.
		/// </summary>
		/// <param name="sender">
		///    The sender.
		/// </param>
		/// <param name="e">
		///    The e.
		/// </param>
		protected void AircraftTypeOnChange(object sender, EventArgs e)
		{
			string selectItemValue = this.AircraftTypeVerificationDD.SelectedItem.Value;

			if (string.IsNullOrEmpty(selectItemValue) == false)
			{
				try
				{
					this.mobileDeviceProfile.AircraftTypeVerification = Convert.ToInt32(selectItemValue);
				}
				catch (FormatException)
				{
					// Ignore
				}
				catch (OverflowException)
				{
					// Ignore
				}
			}

			this.DisableErrorActionControls();
		}

		/// <summary>
		///    This method handles the Destination dropdown on change event.
		///    It will enable or disable the associated Error Action checkbox.
		/// </summary>
		/// <param name="sender">
		///    The sender.
		/// </param>
		/// <param name="e">
		///    The e.
		/// </param>
		protected void DestinationOnChange(object sender, EventArgs e)
		{
			string selectItemValue = this.DestinationDD.SelectedItem.Value;

			if (string.IsNullOrEmpty(selectItemValue) == false)
			{
				try
				{
					this.mobileDeviceProfile.Destination = Convert.ToInt32(selectItemValue);
				}
				catch (FormatException)
				{
					// Ignore
				}
				catch (OverflowException)
				{
					// Ignore
				}
			}

			this.DisableErrorActionControls();
		}

		/// <summary>
		///    This method handles the Gate dropdown on change event.
		///    It will enable or disable the associated Error Action checkbox.
		/// </summary>
		/// <param name="sender">
		///    The sender.
		/// </param>
		/// <param name="e">
		///    The e.
		/// </param>
		protected void GateOnChange(object sender, EventArgs e)
		{
			string selectItemValue = this.GateDD.SelectedItem.Value;

			if (string.IsNullOrEmpty(selectItemValue) == false)
			{
				try
				{
					this.mobileDeviceProfile.Gate = Convert.ToInt32(selectItemValue);
				}
				catch (FormatException)
				{
					// Ignore
				}
				catch (OverflowException)
				{
					// Ignore
				}
			}

			this.DisableErrorActionControls();
		}

		/// <summary>
		///    This method handles the Meter Total dropdown on change event.
		///    It will enable or disable the associated Error Action checkbox.
		/// </summary>
		/// <param name="sender">
		///    The sender.
		/// </param>
		/// <param name="e">
		///    The e.
		/// </param>
		protected void MeterTotalOnChange(object sender, EventArgs e)
		{
			string selectItemValue = this.MeterTotalDD.SelectedItem.Value;

			if (string.IsNullOrEmpty(selectItemValue) == false)
			{
				try
				{
					this.mobileDeviceProfile.MeterTotal = Convert.ToInt32(selectItemValue);
				}
				catch (FormatException)
				{
					// Ignore
				}
				catch (OverflowException)
				{
					// Ignore
				}
			}

			this.DisableErrorActionControls();
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
			this.mobileDeviceProfile =
				this.Session[PageSessionKeyConstants.ProfileConfigurationProfileObject] as MobileDeviceProfile;

			if (this.Page.IsPostBack == false)
			{
				this.LoadRuleDropdowns();
				this.SelectTheDropdownItem();
				this.SetOddDropdownSelections();
				this.SetErrorActionCheckboxes();
			}

			this.DisableErrorActionControls();
			this.DisableFields();
		}

		/// <summary>
		///    This method handles the Ship Number dropdown on change event.
		///    It will enable or disable the associated Error Action checkbox.
		/// </summary>
		/// <param name="sender">
		///    The sender.
		/// </param>
		/// <param name="e">
		///    The e.
		/// </param>
		protected void ShipNumberOnChange(object sender, EventArgs e)
		{
			string selectItemValue = this.ShipNumberDD.SelectedItem.Value;

			if (string.IsNullOrEmpty(selectItemValue) == false)
			{
				try
				{
					this.mobileDeviceProfile.ShipNumber = Convert.ToInt32(selectItemValue);
				}
				catch (FormatException)
				{
					// Ignore
				}
				catch (OverflowException)
				{
					// Ignore
				}
			}

			this.DisableErrorActionControls();
		}

		/// <summary>
		///    This method handles the Tank Capacity dropdown on change event.
		///    It will enable or disable the associated Error Action checkbox.
		/// </summary>
		/// <param name="sender">
		///    The sender.
		/// </param>
		/// <param name="e">
		///    The e.
		/// </param>
		protected void TankCapacityOnChange(object sender, EventArgs e)
		{
			string selectItemValue = this.TankCapacityDD.SelectedItem.Value;

			if (string.IsNullOrEmpty(selectItemValue) == false)
			{
				try
				{
					this.mobileDeviceProfile.TankCapacity = Convert.ToInt32(selectItemValue);
				}
				catch (FormatException)
				{
					// Ignore
				}
				catch (OverflowException)
				{
					// Ignore
				}
			}

			this.DisableErrorActionControls();
		}

		/// <summary>
		///    This method handles the Tank Position Balance dropdown on change event.
		///    It will enable or disable the associated Error Action checkbox.
		/// </summary>
		/// <param name="sender">
		///    The sender.
		/// </param>
		/// <param name="e">
		///    The e.
		/// </param>
		protected void TankPosBalanceOnChange(object sender, EventArgs e)
		{
			string selectItemValue = this.CheckTanksDifferenceDD.SelectedItem.Value;

			if (string.IsNullOrEmpty(selectItemValue) == false)
			{
				try
				{
					this.mobileDeviceProfile.TankPositionBalanceVerification = Convert.ToInt32(selectItemValue);
				}
				catch (FormatException)
				{
					// Ignore
				}
				catch (OverflowException)
				{
					// Ignore
				}
			}

			this.DisableErrorActionControls();
		}

		/// <summary>
		///    This method handles the Ticket Printing dropdown on change event.
		///    It will enable or disable the associated Error Action checkbox.
		/// </summary>
		/// <param name="sender">
		///    The sender.
		/// </param>
		/// <param name="e">
		///    The e.
		/// </param>
		protected void TicketPrintingOnChange(object sender, EventArgs e)
		{
			string selectItemValue = this.TicketPrintingDD.SelectedItem.Value;

			if (string.IsNullOrEmpty(selectItemValue) == false)
			{
				try
				{
					this.mobileDeviceProfile.TicketPrinting = Convert.ToInt32(selectItemValue);
				}
				catch (FormatException)
				{
					// Ignore
				}
				catch (OverflowException)
				{
					// Ignore
				}
			}

			this.DisableErrorActionControls();
		}

		/// <summary>
		///    This method handles the Volume Pumped dropdown on change event.
		///    It will enable or disable the associated Error Action checkbox.
		/// </summary>
		/// <param name="sender">
		///    The sender.
		/// </param>
		/// <param name="e">
		///    The e.
		/// </param>
		protected void VolumePumpedOnChange(object sender, EventArgs e)
		{
			string selectItemValue = this.VolumePumpedDD.SelectedItem.Value;

			if (string.IsNullOrEmpty(selectItemValue) == false)
			{
				try
				{
					this.mobileDeviceProfile.VolumePumped = Convert.ToInt32(selectItemValue);
				}
				catch (FormatException)
				{
					// Ignore
				}
				catch (OverflowException)
				{
					// Ignore
				}
			}

			this.DisableErrorActionControls();
		}

		/// <summary>
		///    This method enables/disables the error action checkboxes based on the
		///    settings of the corresponding dropdown selections.
		/// </summary>
		private void DisableErrorActionControls()
		{
			string destinationValueItem = this.DestinationDD.SelectedItem.Value;
			string ticketPrintValueItem = this.TicketPrintingDD.SelectedItem.Value;
			string aircraftTypeValueItem = this.AircraftTypeVerificationDD.SelectedItem.Value;
			string shipNumValueItem = this.ShipNumberDD.SelectedItem.Value;
			string tankDiffValueItem = this.CheckTanksDifferenceDD.SelectedItem.Value;
			string gateValueItem = this.GateDD.SelectedItem.Value;
			string meterTotalValueItem = this.MeterTotalDD.SelectedItem.Value;
			string volumePumpValueItem = this.VolumePumpedDD.SelectedItem.Value;
			string tankCapValueItem = this.TankCapacityDD.SelectedItem.Value;

			this.EaDestinationCB.Enabled = true;
			this.EaTicketPrintingCB.Enabled = true;
			this.EaAircraftTypeVerificationCB.Enabled = true;
			this.EaShipNumberCB.Enabled = true;
			this.EaCheckTanksDifferenceCB.Enabled = true;
			this.EaGateCB.Enabled = true;
			this.EaMeterTotalCB.Enabled = true;
			this.EaVolumePumpedCB.Enabled = true;
			this.EaTankCapacityCB.Enabled = true;

			if (string.IsNullOrEmpty(destinationValueItem) == false)
			{
				if (destinationValueItem.Equals("1"))
				{
					this.EaDestinationCB.Checked = false;
					this.EaDestinationCB.Enabled = false;
					this.mobileDeviceProfile.EaDestination = false;
				}
			}

			if (string.IsNullOrEmpty(ticketPrintValueItem) == false)
			{
				if (ticketPrintValueItem.Equals("1"))
				{
					this.EaTicketPrintingCB.Checked = false;
					this.EaTicketPrintingCB.Enabled = false;
					this.mobileDeviceProfile.EaTicketPrinting = false;
				}
			}

			if (string.IsNullOrEmpty(aircraftTypeValueItem) == false)
			{
				if (aircraftTypeValueItem.Equals("1"))
				{
					this.EaAircraftTypeVerificationCB.Checked = false;
					this.EaAircraftTypeVerificationCB.Enabled = false;
					this.mobileDeviceProfile.EaAircraftType = false;
				}
			}

			if (string.IsNullOrEmpty(shipNumValueItem) == false)
			{
				if (shipNumValueItem.Equals("1"))
				{
					this.EaShipNumberCB.Enabled = false;
					this.EaShipNumberCB.Checked = false;
					this.mobileDeviceProfile.EaShipNumber = false;
				}
			}

			if (string.IsNullOrEmpty(tankDiffValueItem) == false)
			{
				this.TankPositionBalanceTB.ReadOnly = false;

				if (tankDiffValueItem.Equals("1"))
				{
					this.EaCheckTanksDifferenceCB.Enabled = false;
					this.EaCheckTanksDifferenceCB.Checked = false;
					this.mobileDeviceProfile.EaTankDiffPercentage = false;
					this.TankPositionBalanceTB.ReadOnly = true;
				}
			}

			if (string.IsNullOrEmpty(gateValueItem) == false)
			{
				if (gateValueItem.Equals("1"))
				{
					this.EaGateCB.Enabled = false;
					this.EaGateCB.Checked = false;
					this.mobileDeviceProfile.EaGateNumber = false;
				}
			}

			if (string.IsNullOrEmpty(meterTotalValueItem) == false)
			{
				if (meterTotalValueItem.Equals("1"))
				{
					this.EaMeterTotalCB.Enabled = false;
					this.EaMeterTotalCB.Checked = false;
					this.mobileDeviceProfile.EaMeterTotal = false;
				}
			}

			if (string.IsNullOrEmpty(volumePumpValueItem) == false)
			{
				if (volumePumpValueItem.Equals("1"))
				{
					this.EaVolumePumpedCB.Enabled = false;
					this.EaVolumePumpedCB.Checked = false;
					this.mobileDeviceProfile.EaVolumePumped = false;
				}
			}

			if (string.IsNullOrEmpty(tankCapValueItem) == false)
			{
				if (tankCapValueItem.Equals("1"))
				{
					this.EaTankCapacityCB.Enabled = false;
					this.EaTankCapacityCB.Checked = false;
					this.mobileDeviceProfile.EaTankCapacity = false;
				}
			}
		}

		/// <summary>
		///    This method will disable all fields if the user does not have the
		///    "modify mobile device profile" right.
		/// </summary>
		private void DisableFields()
		{
			this.StrictUserValidationDD.Enabled = this.HasPermission();
			this.VerifyFuelingEquipmentDD.Enabled = this.HasPermission();
			this.AllowEditRequiredFuelDD.Enabled = this.HasPermission();
			this.AllowBackAfterArrivalDD.Enabled = this.HasPermission();
			this.AllowBackAfterTicketDD.Enabled = this.HasPermission();
			this.RequirePrintDD.Enabled = this.HasPermission();
			this.TotalFuelLoadCheckDD.Enabled = this.HasPermission();
			this.VolumetricThresholdValidationDD.Enabled = this.HasPermission();
			this.ValidateShipNumberDD.Enabled = this.HasPermission();
			this.AllowFlightGateModificationDD.Enabled = this.HasPermission();
			this.BypassDistributionToleranceDD.Enabled = this.HasPermission();
			this.VehicleIdCheckDD.Enabled = this.HasPermission();
			this.GseFuelMustMatchDD.Enabled = this.HasPermission();
			this.AllowManualMeterDD.Enabled = this.HasPermission();
			this.UseValidationLogicForGaTransactionDD.Enabled = this.HasPermission();
			this.AllowShipNumberModificationDD.Enabled = this.HasPermission();
			this.AllowAircraftTypeModificationDD.Enabled = this.HasPermission();
			this.AllowDestinationModificationDD.Enabled = this.HasPermission();
			this.AllowVtoModificationDD.Enabled = this.HasPermission();
			this.OverrideWingBalancePercentVerficationDD.Enabled = this.HasPermission();
			this.DestinationDD.Enabled = this.HasPermission();
			this.TicketPrintingDD.Enabled = this.HasPermission();
			this.AircraftTypeVerificationDD.Enabled = this.HasPermission();
			this.ShipNumberDD.Enabled = this.HasPermission();
			this.CheckTanksDifferenceDD.Enabled = this.HasPermission();
			this.EaDestinationCB.Enabled = this.HasPermission();
			this.EaTicketPrintingCB.Enabled = this.HasPermission();
			this.EaAircraftTypeVerificationCB.Enabled = this.HasPermission();
			this.EaShipNumberCB.Enabled = this.HasPermission();
			this.EaCheckTanksDifferenceCB.Enabled = this.HasPermission();
			this.GateDD.Enabled = this.HasPermission();
			this.MeterTotalDD.Enabled = this.HasPermission();
			this.VolumePumpedDD.Enabled = this.HasPermission();
			this.TankCapacityDD.Enabled = this.HasPermission();
			this.TankPositionBalanceTB.Enabled = this.HasPermission();
			this.EaGateCB.Enabled = this.HasPermission();
			this.EaMeterTotalCB.Enabled = this.HasPermission();
			this.EaVolumePumpedCB.Enabled = this.HasPermission();
			this.EaTankCapacityCB.Enabled = this.HasPermission();
		}

		/// <summary>
		///    This method sets the dropdown to the appropriate selection based on the validation rule
		///    and validate rule error action values.  The default setting is "Off".
		/// </summary>
		/// <param name="dropdown">The dropdown to be set.</param>
		/// <param name="valRuleSetting">Validation rule setting value (true or false).</param>
		/// <param name="valRuleEaSetting">Validation rule error action setting value (true or false).</param>
		private void GenericDropdownSelect(FMDropDownList dropdown, bool valRuleSetting, bool valRuleEaSetting)
		{
			// Selection set to "Off"
			dropdown.SelectedIndex = 0;

			if (valRuleSetting && valRuleEaSetting)
			{
				// Selection set to "On with Error Action"
				dropdown.SelectedIndex = 2;
			}
			else if (valRuleSetting)
			{
				// Selection set to "On"
				dropdown.SelectedIndex = 1;
			}
		}

		/// <summary>
		///    This method returns true if the user has the MODIFY_MOBILE_DEVICE_PROFILES right and the
		///    entity has not been assigned down.
		/// </summary>
		/// <returns>
		///    The System.Boolean.
		/// </returns>
		private bool HasPermission()
		{
			this.mobileDeviceProfile =
				this.Session[PageSessionKeyConstants.ProfileConfigurationProfileObject] as MobileDeviceProfile;

			if (this.mobileDeviceProfile == null)
			{
				return false;
			}

			if (this.mobileDeviceProfile.SiteGuid == Guid.Empty && this.Security.HasRight(RIGHT.MODIFY_MOBILE_DEVICE_PROFILES))
			{
				return true;
			}

			return this.Security.HasRight(RIGHT.MODIFY_MOBILE_DEVICE_PROFILES)
					 && (this.Security.SiteGuid == this.mobileDeviceProfile.SiteGuid);
		}

		/// <summary>
		///    This method loads the validation rule dropdowns with the appropriate values.
		/// </summary>
		private void LoadRuleDropdowns()
		{
			var dropdownValues = new List<ListItem>();

			string off = this.GetDataDictionaryValueByKey(this.Security.SiteGuid, OffStr);
			string on = this.GetDataDictionaryValueByKey(this.Security.SiteGuid, OnStr);
			string onWithEa = this.GetDataDictionaryValueByKey(this.Security.SiteGuid, OnWithEaStr);

			var listItem = new ListItem(off, "0");
			dropdownValues.Add(listItem);

			listItem = new ListItem(on, "1");
			dropdownValues.Add(listItem);

			listItem = new ListItem(onWithEa, "2");
			dropdownValues.Add(listItem);

			this.StrictUserValidationDD.DataSource = dropdownValues;
			this.StrictUserValidationDD.DataTextField = "Text";
			this.StrictUserValidationDD.DataValueField = "Value";
			this.StrictUserValidationDD.DataBind();

			this.VerifyFuelingEquipmentDD.DataSource = dropdownValues;
			this.VerifyFuelingEquipmentDD.DataTextField = "Text";
			this.VerifyFuelingEquipmentDD.DataValueField = "Value";
			this.VerifyFuelingEquipmentDD.DataBind();

			this.AllowEditRequiredFuelDD.DataSource = dropdownValues;
			this.AllowEditRequiredFuelDD.DataTextField = "Text";
			this.AllowEditRequiredFuelDD.DataValueField = "Value";
			this.AllowEditRequiredFuelDD.DataBind();

			this.AllowBackAfterArrivalDD.DataSource = dropdownValues;
			this.AllowBackAfterArrivalDD.DataTextField = "Text";
			this.AllowBackAfterArrivalDD.DataValueField = "Value";
			this.AllowBackAfterArrivalDD.DataBind();

			this.AllowBackAfterTicketDD.DataSource = dropdownValues;
			this.AllowBackAfterTicketDD.DataTextField = "Text";
			this.AllowBackAfterTicketDD.DataValueField = "Value";
			this.AllowBackAfterTicketDD.DataBind();

			this.RequirePrintDD.DataSource = dropdownValues;
			this.RequirePrintDD.DataTextField = "Text";
			this.RequirePrintDD.DataValueField = "Value";
			this.RequirePrintDD.DataBind();

			this.TotalFuelLoadCheckDD.DataSource = dropdownValues;
			this.TotalFuelLoadCheckDD.DataTextField = "Text";
			this.TotalFuelLoadCheckDD.DataValueField = "Value";
			this.TotalFuelLoadCheckDD.DataBind();

			this.VolumetricThresholdValidationDD.DataSource = dropdownValues;
			this.VolumetricThresholdValidationDD.DataTextField = "Text";
			this.VolumetricThresholdValidationDD.DataValueField = "Value";
			this.VolumetricThresholdValidationDD.DataBind();

			this.ValidateShipNumberDD.DataSource = dropdownValues;
			this.ValidateShipNumberDD.DataTextField = "Text";
			this.ValidateShipNumberDD.DataValueField = "Value";
			this.ValidateShipNumberDD.DataBind();

			this.AllowVtoModificationDD.DataSource = dropdownValues;
			this.AllowVtoModificationDD.DataTextField = "Text";
			this.AllowVtoModificationDD.DataValueField = "Value";
			this.AllowVtoModificationDD.DataBind();

			this.AllowFlightGateModificationDD.DataSource = dropdownValues;
			this.AllowFlightGateModificationDD.DataTextField = "Text";
			this.AllowFlightGateModificationDD.DataValueField = "Value";
			this.AllowFlightGateModificationDD.DataBind();

			this.OverrideWingBalancePercentVerficationDD.DataSource = dropdownValues;
			this.OverrideWingBalancePercentVerficationDD.DataTextField = "Text";
			this.OverrideWingBalancePercentVerficationDD.DataValueField = "Value";
			this.OverrideWingBalancePercentVerficationDD.DataBind();

			this.BypassDistributionToleranceDD.DataSource = dropdownValues;
			this.BypassDistributionToleranceDD.DataTextField = "Text";
			this.BypassDistributionToleranceDD.DataValueField = "Value";
			this.BypassDistributionToleranceDD.DataBind();

			this.VehicleIdCheckDD.DataSource = dropdownValues;
			this.VehicleIdCheckDD.DataTextField = "Text";
			this.VehicleIdCheckDD.DataValueField = "Value";
			this.VehicleIdCheckDD.DataBind();

			this.GseFuelMustMatchDD.DataSource = dropdownValues;
			this.GseFuelMustMatchDD.DataTextField = "Text";
			this.GseFuelMustMatchDD.DataValueField = "Value";
			this.GseFuelMustMatchDD.DataBind();

			this.AllowManualMeterDD.DataSource = dropdownValues;
			this.AllowManualMeterDD.DataTextField = "Text";
			this.AllowManualMeterDD.DataValueField = "Value";
			this.AllowManualMeterDD.DataBind();

			this.UseValidationLogicForGaTransactionDD.DataSource = dropdownValues;
			this.UseValidationLogicForGaTransactionDD.DataTextField = "Text";
			this.UseValidationLogicForGaTransactionDD.DataValueField = "Value";
			this.UseValidationLogicForGaTransactionDD.DataBind();

			this.AllowShipNumberModificationDD.DataSource = dropdownValues;
			this.AllowShipNumberModificationDD.DataTextField = "Text";
			this.AllowShipNumberModificationDD.DataValueField = "Value";
			this.AllowShipNumberModificationDD.DataBind();

			this.AllowAircraftTypeModificationDD.DataSource = dropdownValues;
			this.AllowAircraftTypeModificationDD.DataTextField = "Text";
			this.AllowAircraftTypeModificationDD.DataValueField = "Value";
			this.AllowAircraftTypeModificationDD.DataBind();

			this.AllowDestinationModificationDD.DataSource = dropdownValues;
			this.AllowDestinationModificationDD.DataTextField = "Text";
			this.AllowDestinationModificationDD.DataValueField = "Value";
			this.AllowDestinationModificationDD.DataBind();

			// Populate Destination dropdown
			dropdownValues.Clear();
			listItem = new ListItem(this.GetDataDictionaryValueByKey(this.Security.SiteGuid, DropdownDisplay1Option1), "1");
			dropdownValues.Add(listItem);
			listItem = new ListItem(this.GetDataDictionaryValueByKey(this.Security.SiteGuid, DropdownDisplay1Option2), "2");
			dropdownValues.Add(listItem);
			listItem = new ListItem(this.GetDataDictionaryValueByKey(this.Security.SiteGuid, DropdownDisplay1Option3), "3");
			dropdownValues.Add(listItem);

			this.DestinationDD.DataSource = dropdownValues;
			this.DestinationDD.DataTextField = "Text";
			this.DestinationDD.DataValueField = "Value";
			this.DestinationDD.DataBind();

			// Populate Gate dropdown
			dropdownValues.Clear();
			listItem = new ListItem(this.GetDataDictionaryValueByKey(this.Security.SiteGuid, DropdownDisplay1Option1), "1");
			dropdownValues.Add(listItem);
			listItem = new ListItem(this.GetDataDictionaryValueByKey(this.Security.SiteGuid, DropdownDisplay1Option2), "2");
			dropdownValues.Add(listItem);
			listItem = new ListItem(this.GetDataDictionaryValueByKey(this.Security.SiteGuid, DropdownDisplay1Option3), "3");
			dropdownValues.Add(listItem);

			this.GateDD.DataSource = dropdownValues;
			this.GateDD.DataTextField = "Text";
			this.GateDD.DataValueField = "Value";
			this.GateDD.DataBind();

			// Populate Ticket Printing dropdown
			dropdownValues.Clear();
			listItem = new ListItem(this.GetDataDictionaryValueByKey(this.Security.SiteGuid, DropdownDisplay2Option1), "1");
			dropdownValues.Add(listItem);
			listItem = new ListItem(this.GetDataDictionaryValueByKey(this.Security.SiteGuid, DropdownDisplay2Option2), "2");
			dropdownValues.Add(listItem);
			listItem = new ListItem(this.GetDataDictionaryValueByKey(this.Security.SiteGuid, DropdownDisplay2Option3), "3");
			dropdownValues.Add(listItem);

			this.TicketPrintingDD.DataSource = dropdownValues;
			this.TicketPrintingDD.DataTextField = "Text";
			this.TicketPrintingDD.DataValueField = "Value";
			this.TicketPrintingDD.DataBind();

			// Populate Aircraft Type Verification dropdown
			dropdownValues.Clear();
			listItem = new ListItem(this.GetDataDictionaryValueByKey(this.Security.SiteGuid, DropdownDisplay2Option1), "1");
			dropdownValues.Add(listItem);
			listItem = new ListItem(this.GetDataDictionaryValueByKey(this.Security.SiteGuid, DropdownDisplay2Option2), "2");
			dropdownValues.Add(listItem);
			listItem = new ListItem(this.GetDataDictionaryValueByKey(this.Security.SiteGuid, DropdownDisplay2Option3), "3");
			dropdownValues.Add(listItem);

			this.AircraftTypeVerificationDD.DataSource = dropdownValues;
			this.AircraftTypeVerificationDD.DataTextField = "Text";
			this.AircraftTypeVerificationDD.DataValueField = "Value";
			this.AircraftTypeVerificationDD.DataBind();

			// Populate Ship Number dropdown
			dropdownValues.Clear();
			listItem = new ListItem(this.GetDataDictionaryValueByKey(this.Security.SiteGuid, DropdownDisplay3Option1), "1");
			dropdownValues.Add(listItem);
			listItem = new ListItem(this.GetDataDictionaryValueByKey(this.Security.SiteGuid, DropdownDisplay3Option2), "2");
			dropdownValues.Add(listItem);
			listItem = new ListItem(this.GetDataDictionaryValueByKey(this.Security.SiteGuid, DropdownDisplay3Option3), "3");
			dropdownValues.Add(listItem);
			listItem = new ListItem(this.GetDataDictionaryValueByKey(this.Security.SiteGuid, DropdownDisplay3Option4), "4");
			dropdownValues.Add(listItem);
			listItem = new ListItem(this.GetDataDictionaryValueByKey(this.Security.SiteGuid, DropdownDisplay3Option5), "5");
			dropdownValues.Add(listItem);

			this.ShipNumberDD.DataSource = dropdownValues;
			this.ShipNumberDD.DataTextField = "Text";
			this.ShipNumberDD.DataValueField = "Value";
			this.ShipNumberDD.DataBind();

			// Populate Meter Total dropdown
			dropdownValues.Clear();
			listItem = new ListItem(this.GetDataDictionaryValueByKey(this.Security.SiteGuid, DropdownDisplay3Option1), "1");
			dropdownValues.Add(listItem);
			listItem = new ListItem(this.GetDataDictionaryValueByKey(this.Security.SiteGuid, DropdownDisplay3Option2), "2");
			dropdownValues.Add(listItem);
			listItem = new ListItem(this.GetDataDictionaryValueByKey(this.Security.SiteGuid, DropdownDisplay3Option3), "3");
			dropdownValues.Add(listItem);
			listItem = new ListItem(this.GetDataDictionaryValueByKey(this.Security.SiteGuid, DropdownDisplay3Option4), "4");
			dropdownValues.Add(listItem);
			listItem = new ListItem(this.GetDataDictionaryValueByKey(this.Security.SiteGuid, DropdownDisplay3Option5), "5");
			dropdownValues.Add(listItem);

			this.MeterTotalDD.DataSource = dropdownValues;
			this.MeterTotalDD.DataTextField = "Text";
			this.MeterTotalDD.DataValueField = "Value";
			this.MeterTotalDD.DataBind();

			// Populate Volume Pumped dropdown
			dropdownValues.Clear();
			listItem = new ListItem(this.GetDataDictionaryValueByKey(this.Security.SiteGuid, DropdownDisplay4Option1), "1");
			dropdownValues.Add(listItem);
			listItem = new ListItem(this.GetDataDictionaryValueByKey(this.Security.SiteGuid, DropdownDisplay4Option2), "2");
			dropdownValues.Add(listItem);
			listItem = new ListItem(this.GetDataDictionaryValueByKey(this.Security.SiteGuid, DropdownDisplay4Option3), "3");
			dropdownValues.Add(listItem);

			this.VolumePumpedDD.DataSource = dropdownValues;
			this.VolumePumpedDD.DataTextField = "Text";
			this.VolumePumpedDD.DataValueField = "Value";
			this.VolumePumpedDD.DataBind();

			// Populate Tank Capacity dropdown
			dropdownValues.Clear();
			listItem = new ListItem(this.GetDataDictionaryValueByKey(this.Security.SiteGuid, DropdownDisplay4Option1), "1");
			dropdownValues.Add(listItem);
			listItem = new ListItem(this.GetDataDictionaryValueByKey(this.Security.SiteGuid, DropdownDisplay4Option2), "2");
			dropdownValues.Add(listItem);
			listItem = new ListItem(this.GetDataDictionaryValueByKey(this.Security.SiteGuid, DropdownDisplay4Option3), "3");
			dropdownValues.Add(listItem);

			this.TankCapacityDD.DataSource = dropdownValues;
			this.TankCapacityDD.DataTextField = "Text";
			this.TankCapacityDD.DataValueField = "Value";
			this.TankCapacityDD.DataBind();

			// Populate Tank position balance verification dropdown
			dropdownValues.Clear();
			listItem = new ListItem(this.GetDataDictionaryValueByKey(this.Security.SiteGuid, DropdownDisplay5Option1), "1");
			dropdownValues.Add(listItem);
			listItem = new ListItem(this.GetDataDictionaryValueByKey(this.Security.SiteGuid, DropdownDisplay5Option2), "2");
			dropdownValues.Add(listItem);
			listItem = new ListItem(this.GetDataDictionaryValueByKey(this.Security.SiteGuid, DropdownDisplay5Option3), "3");
			dropdownValues.Add(listItem);

			this.CheckTanksDifferenceDD.DataSource = dropdownValues;
			this.CheckTanksDifferenceDD.DataTextField = "Text";
			this.CheckTanksDifferenceDD.DataValueField = "Value";
			this.CheckTanksDifferenceDD.DataBind();
		}

		/// <summary>
		///    This method will retrieve the page values and load the mobile
		///    device profile data object with the new data.
		/// </summary>
		private void RetrieveDataFromPage()
		{
			if (string.IsNullOrEmpty(this.StrictUserValidationDD.SelectedItem.Value) == false)
			{
				this.mobileDeviceProfile.StrictUserValidation = false;
				this.mobileDeviceProfile.EaStrictUserValidation = false;

				if (this.StrictUserValidationDD.SelectedItem.Value.Equals("1"))
				{
					this.mobileDeviceProfile.StrictUserValidation = true;
					this.mobileDeviceProfile.EaStrictUserValidation = false;
				}
				else if (this.StrictUserValidationDD.SelectedItem.Value.Equals("2"))
				{
					this.mobileDeviceProfile.StrictUserValidation = true;
					this.mobileDeviceProfile.EaStrictUserValidation = true;
				}
			}

			if (string.IsNullOrEmpty(this.VerifyFuelingEquipmentDD.SelectedItem.Value) == false)
			{
				this.mobileDeviceProfile.VerifyFuelingEquipment = false;
				this.mobileDeviceProfile.EaVerifyFuelingEquipment = false;

				if (this.VerifyFuelingEquipmentDD.SelectedItem.Value.Equals("1"))
				{
					this.mobileDeviceProfile.VerifyFuelingEquipment = true;
					this.mobileDeviceProfile.EaVerifyFuelingEquipment = false;
				}
				else if (this.VerifyFuelingEquipmentDD.SelectedItem.Value.Equals("2"))
				{
					this.mobileDeviceProfile.VerifyFuelingEquipment = true;
					this.mobileDeviceProfile.EaVerifyFuelingEquipment = true;
				}
			}

			if (string.IsNullOrEmpty(this.AllowEditRequiredFuelDD.SelectedItem.Value) == false)
			{
				this.mobileDeviceProfile.AllowEditRequiredFuelLoad = false;
				this.mobileDeviceProfile.EaAllowEditOfRequiredFuelLoad = false;

				if (this.AllowEditRequiredFuelDD.SelectedItem.Value.Equals("1"))
				{
					this.mobileDeviceProfile.AllowEditRequiredFuelLoad = true;
					this.mobileDeviceProfile.EaAllowEditOfRequiredFuelLoad = false;
				}
				else if (this.AllowEditRequiredFuelDD.SelectedItem.Value.Equals("2"))
				{
					this.mobileDeviceProfile.AllowEditRequiredFuelLoad = true;
					this.mobileDeviceProfile.EaAllowEditOfRequiredFuelLoad = true;
				}
			}

			if (string.IsNullOrEmpty(this.AllowBackAfterArrivalDD.SelectedItem.Value) == false)
			{
				this.mobileDeviceProfile.AllowBackAfterArrivalScreen = false;
				this.mobileDeviceProfile.EaAllowBackAfterArrivalScreen = false;

				if (this.AllowBackAfterArrivalDD.SelectedItem.Value.Equals("1"))
				{
					this.mobileDeviceProfile.AllowBackAfterArrivalScreen = true;
					this.mobileDeviceProfile.EaAllowBackAfterArrivalScreen = false;
				}
				else if (this.AllowBackAfterArrivalDD.SelectedItem.Value.Equals("2"))
				{
					this.mobileDeviceProfile.AllowBackAfterArrivalScreen = true;
					this.mobileDeviceProfile.EaAllowBackAfterArrivalScreen = true;
				}
			}

			if (string.IsNullOrEmpty(this.AllowBackAfterTicketDD.SelectedItem.Value) == false)
			{
				this.mobileDeviceProfile.AllowBackAfterTicketPrinted = false;
				this.mobileDeviceProfile.EaAllowBackAfterTicketPrinted = false;

				if (this.AllowBackAfterTicketDD.SelectedItem.Value.Equals("1"))
				{
					this.mobileDeviceProfile.AllowBackAfterTicketPrinted = true;
					this.mobileDeviceProfile.EaAllowBackAfterTicketPrinted = false;
				}
				else if (this.AllowBackAfterTicketDD.SelectedItem.Value.Equals("2"))
				{
					this.mobileDeviceProfile.AllowBackAfterTicketPrinted = true;
					this.mobileDeviceProfile.EaAllowBackAfterTicketPrinted = true;
				}
			}

			if (string.IsNullOrEmpty(this.RequirePrintDD.SelectedItem.Value) == false)
			{
				this.mobileDeviceProfile.RequirePrint = false;
				this.mobileDeviceProfile.EaRequirePrint = false;

				if (this.RequirePrintDD.SelectedItem.Value.Equals("1"))
				{
					this.mobileDeviceProfile.RequirePrint = true;
					this.mobileDeviceProfile.EaRequirePrint = false;
				}
				else if (this.RequirePrintDD.SelectedItem.Value.Equals("2"))
				{
					this.mobileDeviceProfile.RequirePrint = true;
					this.mobileDeviceProfile.EaRequirePrint = true;
				}
			}

			if (string.IsNullOrEmpty(this.TotalFuelLoadCheckDD.SelectedItem.Value) == false)
			{
				this.mobileDeviceProfile.TotalFuelLoadCheck = false;
				this.mobileDeviceProfile.EaTotalFuelLoad = false;

				if (this.TotalFuelLoadCheckDD.SelectedItem.Value.Equals("1"))
				{
					this.mobileDeviceProfile.TotalFuelLoadCheck = true;
					this.mobileDeviceProfile.EaTotalFuelLoad = false;
				}
				else if (this.TotalFuelLoadCheckDD.SelectedItem.Value.Equals("2"))
				{
					this.mobileDeviceProfile.TotalFuelLoadCheck = true;
					this.mobileDeviceProfile.EaTotalFuelLoad = true;
				}
			}

			if (string.IsNullOrEmpty(this.VolumetricThresholdValidationDD.SelectedItem.Value) == false)
			{
				this.mobileDeviceProfile.VolumetricThresholdValidation = false;
				this.mobileDeviceProfile.EaVolumetricThresholdValidation = false;

				if (this.VolumetricThresholdValidationDD.SelectedItem.Value.Equals("1"))
				{
					this.mobileDeviceProfile.VolumetricThresholdValidation = true;
					this.mobileDeviceProfile.EaVolumetricThresholdValidation = false;
				}
				else if (this.VolumetricThresholdValidationDD.SelectedItem.Value.Equals("2"))
				{
					this.mobileDeviceProfile.VolumetricThresholdValidation = true;
					this.mobileDeviceProfile.EaVolumetricThresholdValidation = true;
				}
			}

			if (string.IsNullOrEmpty(this.ValidateShipNumberDD.SelectedItem.Value) == false)
			{
				this.mobileDeviceProfile.ValidateShipNumber = false;
				this.mobileDeviceProfile.EaValidateShipNumber = false;

				if (this.ValidateShipNumberDD.SelectedItem.Value.Equals("1"))
				{
					this.mobileDeviceProfile.ValidateShipNumber = true;
					this.mobileDeviceProfile.EaValidateShipNumber = false;
				}
				else if (this.ValidateShipNumberDD.SelectedItem.Value.Equals("2"))
				{
					this.mobileDeviceProfile.ValidateShipNumber = true;
					this.mobileDeviceProfile.EaValidateShipNumber = true;
				}
			}

			if (string.IsNullOrEmpty(this.AllowFlightGateModificationDD.SelectedItem.Value) == false)
			{
				this.mobileDeviceProfile.AllowFlightGateModification = false;
				this.mobileDeviceProfile.EaAllowFlightGateModification = false;

				if (this.AllowFlightGateModificationDD.SelectedItem.Value.Equals("1"))
				{
					this.mobileDeviceProfile.AllowFlightGateModification = true;
					this.mobileDeviceProfile.EaAllowFlightGateModification = false;
				}
				else if (this.AllowFlightGateModificationDD.SelectedItem.Value.Equals("2"))
				{
					this.mobileDeviceProfile.AllowFlightGateModification = true;
					this.mobileDeviceProfile.EaAllowFlightGateModification = true;
				}
			}

			if (string.IsNullOrEmpty(this.BypassDistributionToleranceDD.SelectedItem.Value) == false)
			{
				this.mobileDeviceProfile.BypassDistributionTolerance = false;
				this.mobileDeviceProfile.EaBypassDistributionTolerance = false;

				if (this.BypassDistributionToleranceDD.SelectedItem.Value.Equals("1"))
				{
					this.mobileDeviceProfile.BypassDistributionTolerance = true;
					this.mobileDeviceProfile.EaBypassDistributionTolerance = false;
				}
				else if (this.BypassDistributionToleranceDD.SelectedItem.Value.Equals("2"))
				{
					this.mobileDeviceProfile.BypassDistributionTolerance = true;
					this.mobileDeviceProfile.EaBypassDistributionTolerance = true;
				}
			}

			if (string.IsNullOrEmpty(this.VehicleIdCheckDD.SelectedItem.Value) == false)
			{
				this.mobileDeviceProfile.VehicleIdCheck = false;
				this.mobileDeviceProfile.EaVehicleIdCheck = false;

				if (this.VehicleIdCheckDD.SelectedItem.Value.Equals("1"))
				{
					this.mobileDeviceProfile.VehicleIdCheck = true;
					this.mobileDeviceProfile.EaVehicleIdCheck = false;
				}
				else if (this.VehicleIdCheckDD.SelectedItem.Value.Equals("2"))
				{
					this.mobileDeviceProfile.VehicleIdCheck = true;
					this.mobileDeviceProfile.EaVehicleIdCheck = true;
				}
			}

			if (string.IsNullOrEmpty(this.GseFuelMustMatchDD.SelectedItem.Value) == false)
			{
				this.mobileDeviceProfile.GseFuelMustMatch = false;
				this.mobileDeviceProfile.EaGseFuelMustMatch = false;

				if (this.GseFuelMustMatchDD.SelectedItem.Value.Equals("1"))
				{
					this.mobileDeviceProfile.GseFuelMustMatch = true;
					this.mobileDeviceProfile.EaGseFuelMustMatch = false;
				}
				else if (this.GseFuelMustMatchDD.SelectedItem.Value.Equals("2"))
				{
					this.mobileDeviceProfile.GseFuelMustMatch = true;
					this.mobileDeviceProfile.EaGseFuelMustMatch = true;
				}
			}

			if (string.IsNullOrEmpty(this.AllowManualMeterDD.SelectedItem.Value) == false)
			{
				this.mobileDeviceProfile.AllowManualMeter = false;
				this.mobileDeviceProfile.EaAllowManualMeter = false;

				if (this.AllowManualMeterDD.SelectedItem.Value.Equals("1"))
				{
					this.mobileDeviceProfile.AllowManualMeter = true;
					this.mobileDeviceProfile.EaAllowManualMeter = false;
				}
				else if (this.AllowManualMeterDD.SelectedItem.Value.Equals("2"))
				{
					this.mobileDeviceProfile.AllowManualMeter = true;
					this.mobileDeviceProfile.EaAllowManualMeter = true;
				}
			}

			if (string.IsNullOrEmpty(this.UseValidationLogicForGaTransactionDD.SelectedItem.Value) == false)
			{
				this.mobileDeviceProfile.UseValidLogicGaTrans = false;
				this.mobileDeviceProfile.EaUseValidationLogicGaTrans = false;

				if (this.UseValidationLogicForGaTransactionDD.SelectedItem.Value.Equals("1"))
				{
					this.mobileDeviceProfile.UseValidLogicGaTrans = true;
					this.mobileDeviceProfile.EaUseValidationLogicGaTrans = false;
				}
				else if (this.UseValidationLogicForGaTransactionDD.SelectedItem.Value.Equals("2"))
				{
					this.mobileDeviceProfile.UseValidLogicGaTrans = true;
					this.mobileDeviceProfile.EaUseValidationLogicGaTrans = true;
				}
			}

			if (string.IsNullOrEmpty(this.AllowShipNumberModificationDD.SelectedItem.Value) == false)
			{
				this.mobileDeviceProfile.AllowShipNumberModification = false;
				this.mobileDeviceProfile.EaAllowShipNumberModification = false;

				if (this.AllowShipNumberModificationDD.SelectedItem.Value.Equals("1"))
				{
					this.mobileDeviceProfile.AllowShipNumberModification = true;
					this.mobileDeviceProfile.EaAllowShipNumberModification = false;
				}
				else if (this.AllowShipNumberModificationDD.SelectedItem.Value.Equals("2"))
				{
					this.mobileDeviceProfile.AllowShipNumberModification = true;
					this.mobileDeviceProfile.EaAllowShipNumberModification = true;
				}
			}

			if (string.IsNullOrEmpty(this.AllowAircraftTypeModificationDD.SelectedItem.Value) == false)
			{
				this.mobileDeviceProfile.AllowAircraftTypeModification = false;
				this.mobileDeviceProfile.EaAllowAircraftTypeModification = false;

				if (this.AllowAircraftTypeModificationDD.SelectedItem.Value.Equals("1"))
				{
					this.mobileDeviceProfile.AllowAircraftTypeModification = true;
					this.mobileDeviceProfile.EaAllowAircraftTypeModification = false;
				}
				else if (this.AllowAircraftTypeModificationDD.SelectedItem.Value.Equals("2"))
				{
					this.mobileDeviceProfile.AllowAircraftTypeModification = true;
					this.mobileDeviceProfile.EaAllowAircraftTypeModification = true;
				}
			}

			if (string.IsNullOrEmpty(this.AllowDestinationModificationDD.SelectedItem.Value) == false)
			{
				this.mobileDeviceProfile.AllowDestinationModification = false;
				this.mobileDeviceProfile.EaAllowDestinationModification = false;

				if (this.AllowDestinationModificationDD.SelectedItem.Value.Equals("1"))
				{
					this.mobileDeviceProfile.AllowDestinationModification = true;
					this.mobileDeviceProfile.EaAllowDestinationModification = false;
				}
				else if (this.AllowDestinationModificationDD.SelectedItem.Value.Equals("2"))
				{
					this.mobileDeviceProfile.AllowDestinationModification = true;
					this.mobileDeviceProfile.EaAllowDestinationModification = true;
				}
			}

			if (string.IsNullOrEmpty(this.AllowVtoModificationDD.SelectedItem.Value) == false)
			{
				this.mobileDeviceProfile.AllowVtoModification = false;
				this.mobileDeviceProfile.EaAllowVtoModification = false;

				if (this.AllowVtoModificationDD.SelectedItem.Value.Equals("1"))
				{
					this.mobileDeviceProfile.AllowVtoModification = true;
					this.mobileDeviceProfile.EaAllowVtoModification = false;
				}
				else if (this.AllowVtoModificationDD.SelectedItem.Value.Equals("2"))
				{
					this.mobileDeviceProfile.AllowVtoModification = true;
					this.mobileDeviceProfile.EaAllowVtoModification = true;
				}
			}

			if (string.IsNullOrEmpty(this.OverrideWingBalancePercentVerficationDD.SelectedItem.Value) == false)
			{
				this.mobileDeviceProfile.OverrideWingBalancePercentVar = false;
				this.mobileDeviceProfile.EaWingBalancePercentage = false;

				if (this.OverrideWingBalancePercentVerficationDD.SelectedItem.Value.Equals("1"))
				{
					this.mobileDeviceProfile.OverrideWingBalancePercentVar = true;
					this.mobileDeviceProfile.EaWingBalancePercentage = false;
				}
				else if (this.OverrideWingBalancePercentVerficationDD.SelectedItem.Value.Equals("2"))
				{
					this.mobileDeviceProfile.OverrideWingBalancePercentVar = true;
					this.mobileDeviceProfile.EaWingBalancePercentage = true;
				}
			}

			if (string.IsNullOrEmpty(this.DestinationDD.SelectedItem.Value) == false)
			{
				try
				{
					this.mobileDeviceProfile.Destination = Convert.ToInt32(this.DestinationDD.SelectedItem.Value);
				}
				catch (FormatException)
				{
					// Ignore
				}
				catch (OverflowException)
				{
					// Ignore
				}
			}

			if (string.IsNullOrEmpty(this.TicketPrintingDD.SelectedItem.Value) == false)
			{
				try
				{
					this.mobileDeviceProfile.TicketPrinting = Convert.ToInt32(this.TicketPrintingDD.SelectedItem.Value);
				}
				catch (FormatException)
				{
					// Ignore
				}
				catch (OverflowException)
				{
					// Ignore
				}
			}

			if (string.IsNullOrEmpty(this.AircraftTypeVerificationDD.SelectedItem.Value) == false)
			{
				try
				{
					this.mobileDeviceProfile.AircraftTypeVerification =
						Convert.ToInt32(this.AircraftTypeVerificationDD.SelectedItem.Value);
				}
				catch (FormatException)
				{
					// Ignore
				}
				catch (OverflowException)
				{
					// Ignore
				}
			}

			if (string.IsNullOrEmpty(this.ShipNumberDD.SelectedItem.Value) == false)
			{
				try
				{
					this.mobileDeviceProfile.ShipNumber = Convert.ToInt32(this.ShipNumberDD.SelectedItem.Value);
				}
				catch (FormatException)
				{
					// Ignore
				}
				catch (OverflowException)
				{
					// Ignore
				}
			}

			if (string.IsNullOrEmpty(this.CheckTanksDifferenceDD.SelectedItem.Value) == false)
			{
				try
				{
					this.mobileDeviceProfile.TankPositionBalanceVerification =
						Convert.ToInt32(this.CheckTanksDifferenceDD.SelectedItem.Value);
				}
				catch (FormatException)
				{
					// Ignore
				}
				catch (OverflowException)
				{
					// Ignore
				}
			}

			if (string.IsNullOrEmpty(this.GateDD.SelectedItem.Value) == false)
			{
				try
				{
					this.mobileDeviceProfile.Gate = Convert.ToInt32(this.GateDD.SelectedItem.Value);
				}
				catch (FormatException)
				{
					// Ignore
				}
				catch (OverflowException)
				{
					// Ignore
				}
			}

			if (string.IsNullOrEmpty(this.MeterTotalDD.SelectedItem.Value) == false)
			{
				try
				{
					this.mobileDeviceProfile.MeterTotal = Convert.ToInt32(this.MeterTotalDD.SelectedItem.Value);
				}
				catch (FormatException)
				{
					// Ignore
				}
				catch (OverflowException)
				{
					// Ignore
				}
			}

			if (string.IsNullOrEmpty(this.VolumePumpedDD.SelectedItem.Value) == false)
			{
				try
				{
					this.mobileDeviceProfile.VolumePumped = Convert.ToInt32(this.VolumePumpedDD.SelectedItem.Value);
				}
				catch (FormatException)
				{
					// Ignore
				}
				catch (OverflowException)
				{
					// Ignore
				}
			}

			if (string.IsNullOrEmpty(this.TankCapacityDD.SelectedItem.Value) == false)
			{
				try
				{
					this.mobileDeviceProfile.TankCapacity = Convert.ToInt32(this.TankCapacityDD.SelectedItem.Value);
				}
				catch (FormatException)
				{
					// Ignore
				}
				catch (OverflowException)
				{
					// Ignore
				}
			}

			if (string.IsNullOrEmpty(this.TankPositionBalanceTB.Text))
			{
				this.mobileDeviceProfile.TankPositionBalancePercentage = null;
			}
			else
			{
				try
				{
					this.mobileDeviceProfile.TankPositionBalancePercentage = Convert.ToDouble(this.TankPositionBalanceTB.Text);
				}
				catch (FormatException)
				{
					// Ignore
				}
				catch (OverflowException)
				{
					// Ignore
				}
			}

			this.mobileDeviceProfile.EaDestination = this.EaDestinationCB.Checked;
			this.mobileDeviceProfile.EaTicketPrinting = this.EaTicketPrintingCB.Checked;
			this.mobileDeviceProfile.EaAircraftType = this.EaAircraftTypeVerificationCB.Checked;
			this.mobileDeviceProfile.EaShipNumber = this.EaShipNumberCB.Checked;
			this.mobileDeviceProfile.EaTankDiffPercentage = this.EaCheckTanksDifferenceCB.Checked;
			this.mobileDeviceProfile.EaGateNumber = this.EaGateCB.Checked;
			this.mobileDeviceProfile.EaMeterTotal = this.EaMeterTotalCB.Checked;
			this.mobileDeviceProfile.EaVolumePumped = this.EaVolumePumpedCB.Checked;
			this.mobileDeviceProfile.EaTankCapacity = this.EaTankCapacityCB.Checked;
		}

		/// <summary>
		///    This method selects the appropriate item in the dropdown based on the
		///    database values.
		/// </summary>
		private void SelectTheDropdownItem()
		{
			if (this.mobileDeviceProfile == null)
			{
				return;
			}

			this.GenericDropdownSelect(
				this.StrictUserValidationDD,
				this.mobileDeviceProfile.StrictUserValidation,
				this.mobileDeviceProfile.EaStrictUserValidation);
			this.GenericDropdownSelect(
				this.VerifyFuelingEquipmentDD,
				this.mobileDeviceProfile.VerifyFuelingEquipment,
				this.mobileDeviceProfile.EaVerifyFuelingEquipment);
			this.GenericDropdownSelect(
				this.AllowEditRequiredFuelDD,
				this.mobileDeviceProfile.AllowEditRequiredFuelLoad,
				this.mobileDeviceProfile.EaAllowEditOfRequiredFuelLoad);
			this.GenericDropdownSelect(
				this.AllowBackAfterArrivalDD,
				this.mobileDeviceProfile.AllowBackAfterArrivalScreen,
				this.mobileDeviceProfile.EaAllowBackAfterArrivalScreen);
			this.GenericDropdownSelect(
				this.AllowBackAfterTicketDD,
				this.mobileDeviceProfile.AllowBackAfterTicketPrinted,
				this.mobileDeviceProfile.EaAllowBackAfterTicketPrinted);
			this.GenericDropdownSelect(
				this.RequirePrintDD, this.mobileDeviceProfile.RequirePrint, this.mobileDeviceProfile.EaRequirePrint);
			this.GenericDropdownSelect(
				this.TotalFuelLoadCheckDD, this.mobileDeviceProfile.TotalFuelLoadCheck, this.mobileDeviceProfile.EaTotalFuelLoad);
			this.GenericDropdownSelect(
				this.VolumetricThresholdValidationDD,
				this.mobileDeviceProfile.VolumetricThresholdValidation,
				this.mobileDeviceProfile.EaVolumetricThresholdValidation);
			this.GenericDropdownSelect(
				this.ValidateShipNumberDD,
				this.mobileDeviceProfile.ValidateShipNumber,
				this.mobileDeviceProfile.EaValidateShipNumber);
			this.GenericDropdownSelect(
				this.AllowVtoModificationDD,
				this.mobileDeviceProfile.AllowVtoModification,
				this.mobileDeviceProfile.EaAllowVtoModification);
			this.GenericDropdownSelect(
				this.AllowFlightGateModificationDD,
				this.mobileDeviceProfile.AllowFlightGateModification,
				this.mobileDeviceProfile.EaAllowFlightGateModification);
			this.GenericDropdownSelect(
				this.OverrideWingBalancePercentVerficationDD,
				this.mobileDeviceProfile.OverrideWingBalancePercentVar,
				this.mobileDeviceProfile.EaWingBalancePercentage);
			this.GenericDropdownSelect(
				this.BypassDistributionToleranceDD,
				this.mobileDeviceProfile.BypassDistributionTolerance,
				this.mobileDeviceProfile.EaBypassDistributionTolerance);
			this.GenericDropdownSelect(
				this.VehicleIdCheckDD, this.mobileDeviceProfile.VehicleIdCheck, this.mobileDeviceProfile.EaVehicleIdCheck);
			this.GenericDropdownSelect(
				this.GseFuelMustMatchDD, this.mobileDeviceProfile.GseFuelMustMatch, this.mobileDeviceProfile.EaGseFuelMustMatch);
			this.GenericDropdownSelect(
				this.AllowManualMeterDD, this.mobileDeviceProfile.AllowManualMeter, this.mobileDeviceProfile.EaAllowManualMeter);
			this.GenericDropdownSelect(
				this.UseValidationLogicForGaTransactionDD,
				this.mobileDeviceProfile.UseValidLogicGaTrans,
				this.mobileDeviceProfile.EaUseValidationLogicGaTrans);
			this.GenericDropdownSelect(
				this.AllowShipNumberModificationDD,
				this.mobileDeviceProfile.AllowShipNumberModification,
				this.mobileDeviceProfile.EaAllowShipNumberModification);
			this.GenericDropdownSelect(
				this.AllowAircraftTypeModificationDD,
				this.mobileDeviceProfile.AllowAircraftTypeModification,
				this.mobileDeviceProfile.EaAllowAircraftTypeModification);
			this.GenericDropdownSelect(
				this.AllowDestinationModificationDD,
				this.mobileDeviceProfile.AllowDestinationModification,
				this.mobileDeviceProfile.EaAllowDestinationModification);
		}

		/// <summary>
		///    This method sets the Error Action checkboxes to their appropriate
		///    state.
		/// </summary>
		private void SetErrorActionCheckboxes()
		{
			if (this.mobileDeviceProfile == null)
			{
				return;
			}

			this.EaDestinationCB.Checked = this.mobileDeviceProfile.EaDestination;
			this.EaTicketPrintingCB.Checked = this.mobileDeviceProfile.EaTicketPrinting;
			this.EaAircraftTypeVerificationCB.Checked = this.mobileDeviceProfile.EaAllowAircraftTypeModification;
			this.EaShipNumberCB.Checked = this.mobileDeviceProfile.EaShipNumber;
			this.EaCheckTanksDifferenceCB.Checked = this.mobileDeviceProfile.EaTankDiffPercentage;
			this.EaGateCB.Checked = this.mobileDeviceProfile.EaGateNumber;
			this.EaMeterTotalCB.Checked = this.mobileDeviceProfile.EaMeterTotal;
			this.EaVolumePumpedCB.Checked = this.mobileDeviceProfile.EaVolumePumped;
			this.EaTankCapacityCB.Checked = this.mobileDeviceProfile.EaTankCapacity;
		}

		/// <summary>
		///    This method will set the dropdown selections that have different information in their
		///    list.  It will set the value based on the mobile device profile data object.
		/// </summary>
		private void SetOddDropdownSelections()
		{
			if (this.mobileDeviceProfile == null)
			{
				return;
			}

			switch (this.mobileDeviceProfile.Destination)
			{
				case 1:
					// "Not Displayed, Compared to User Entry"
					this.DestinationDD.SelectedIndex = 0;
					break;
				case 2:
					// "Displayed, no edit"
					this.DestinationDD.SelectedIndex = 1;
					break;
				case 3:
					// "Displayed, user may edit"
					this.DestinationDD.SelectedIndex = 2;
					break;
				default:
					// "Not Displayed, Compared to User Entry"
					this.DestinationDD.SelectedIndex = 0;
					break;
			}

			switch (this.mobileDeviceProfile.TicketPrinting)
			{
				case 1:
					// "Not Required"
					this.TicketPrintingDD.SelectedIndex = 0;
					break;
				case 2:
					// "Required"
					this.TicketPrintingDD.SelectedIndex = 1;
					break;
				case 3:
					// "Required & Allow Override"
					this.TicketPrintingDD.SelectedIndex = 2;
					break;
				default:
					// "Not Required"
					this.TicketPrintingDD.SelectedIndex = 0;
					break;
			}

			switch (this.mobileDeviceProfile.AircraftTypeVerification)
			{
				case 1:
					// "Not Required"
					this.AircraftTypeVerificationDD.SelectedIndex = 0;
					break;
				case 2:
					// "Required"
					this.AircraftTypeVerificationDD.SelectedIndex = 1;
					break;
				case 3:
					// "Required & Allow Override"
					this.AircraftTypeVerificationDD.SelectedIndex = 2;
					break;
				default:
					// "Not Required"
					this.AircraftTypeVerificationDD.SelectedIndex = 0;
					break;
			}

			switch (this.mobileDeviceProfile.ShipNumber)
			{
				case 1:
					// "Display & Non-Edit"
					this.ShipNumberDD.SelectedIndex = 0;
					break;
				case 2:
					// "User Entry with Overriding"
					this.ShipNumberDD.SelectedIndex = 1;
					break;
				case 3:
					// "User Entry without Overriding"
					this.ShipNumberDD.SelectedIndex = 2;
					break;
				case 4:
					// "Editing with Overriding"
					this.ShipNumberDD.SelectedIndex = 3;
					break;
				case 5:
					// "Editing without Overriding"
					this.ShipNumberDD.SelectedIndex = 4;
					break;
				default:
					// "Display & Non-Edit"
					this.ShipNumberDD.SelectedIndex = 0;
					break;
			}

			switch (this.mobileDeviceProfile.TankPositionBalanceVerification)
			{
				case 1:
					// "Not Required"
					this.CheckTanksDifferenceDD.SelectedIndex = 0;
					break;
				case 2:
					// "Required with Override"
					this.CheckTanksDifferenceDD.SelectedIndex = 1;
					break;
				case 3:
					// "Required without Override"
					this.CheckTanksDifferenceDD.SelectedIndex = 2;
					break;
				default:
					// "Not Required"
					this.CheckTanksDifferenceDD.SelectedIndex = 0;
					break;
			}

			switch (this.mobileDeviceProfile.Gate)
			{
				case 1:
					// "Not Displayed, Compared to User Entry"
					this.GateDD.SelectedIndex = 0;
					break;
				case 2:
					// "Displayed, no edit"
					this.GateDD.SelectedIndex = 1;
					break;
				case 3:
					// "Displayed, user may edit"
					this.GateDD.SelectedIndex = 2;
					break;
				default:
					// "Not Displayed, Compared to User Entry"
					this.GateDD.SelectedIndex = 0;
					break;
			}

			switch (this.mobileDeviceProfile.MeterTotal)
			{
				case 1:
					// "Display & Non-Edit"
					this.MeterTotalDD.SelectedIndex = 0;
					break;
				case 2:
					// "User Entry with Overriding"
					this.MeterTotalDD.SelectedIndex = 1;
					break;
				case 3:
					// "User Entry without Overriding"
					this.MeterTotalDD.SelectedIndex = 2;
					break;
				case 4:
					// "Editing with Overriding"
					this.MeterTotalDD.SelectedIndex = 3;
					break;
				case 5:
					// "Editing without Overriding"
					this.MeterTotalDD.SelectedIndex = 4;
					break;
				default:
					// "Display & Non-Edit"
					this.MeterTotalDD.SelectedIndex = 0;
					break;
			}

			switch (this.mobileDeviceProfile.VolumePumped)
			{
				case 1:
					// "Not Required"
					this.VolumePumpedDD.SelectedIndex = 0;
					break;
				case 2:
					// "Required"
					this.VolumePumpedDD.SelectedIndex = 1;
					break;
				case 3:
					// "Required & Allow Override"
					this.VolumePumpedDD.SelectedIndex = 2;
					break;
				default:
					// "Not Required"
					this.VolumePumpedDD.SelectedIndex = 0;
					break;
			}

			switch (this.mobileDeviceProfile.TankCapacity)
			{
				case 1:
					// "Not Required"
					this.TankCapacityDD.SelectedIndex = 0;
					break;
				case 2:
					// "Required"
					this.TankCapacityDD.SelectedIndex = 1;
					break;
				case 3:
					// "Required & Allow Override"
					this.TankCapacityDD.SelectedIndex = 2;
					break;
				default:
					// "Not Required"
					this.TankCapacityDD.SelectedIndex = 0;
					break;
			}
		}

		#endregion
	}
}