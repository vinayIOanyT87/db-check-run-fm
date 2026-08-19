 #pragma warning disable 1587
///***************************************************************************
/// Module Name:  EquipmentMeterPage.aspx.cs
/// Author:       Ryan Hill
/// Copyright (c) Varec, Inc.  All rights reserved.
///***************************************************************************
#pragma warning restore 1587

namespace FuelsManager.FMWebApp
{
	using System;
    using System.Collections.Generic;
    using System.Globalization;
    using FMBusinessObjects.DataObjects;

    /// <summary>
    /// This page allows a user to assign a meter to a piece of equipment.
    /// </summary>
    public partial class EquipmentMeterPage : EquipmentPageBase
	{
        //possibly make this configurable by site later
        private const int NumberOfDigits = 8;
        private const double MeterFactor = 1.0000;
        private const double FuelCompressionFactor = 1.0000;

        private const string __NO_METER__ = "None";
        private const string __SINGLE_METER__ = "Single";
        private const string __DUAL_METER__ = "Dual";


        private void HandleGeneralMeterAssigned()
        {
            this.MeterConfigRadioGroup.SelectedValue = __SINGLE_METER__;
            //this.EnableControls(true);
            FirstMeterEnable(true);
            SecondMeterEnable(false);

            this.MeterIDTextBox.Text = this.Equipment.Meter[0].ID;
            this.NumberOfDigitsTextBox.Text = this.Equipment.Meter[0].NumberOfDigits.ToString();
            this.RotatesBackwardCheckBox.Checked = this.Equipment.Meter[0].RotatesBackwardsFlag;
            this.ReceiptMeterCheckBox.Checked = this.Equipment.Meter[0].ReceiptMeterFlag;
            this.MeterFactorTextBox.Text = this.Equipment.Meter[0].MeterFactor?.ToString("F4") ?? string.Empty;
            this.FuelCompressionTextBox.Text = this.Equipment.Meter[0].FuelCompressionFactor?.ToString("F4") ?? string.Empty;
        }

        /// <summary>
        /// Set the controls on the screen to enabled or disabled.
        /// </summary>
        /// <param name="enable">True to enable the controls, false otherwise.</param>
        private void EnableControls(bool enable)
        {
            FirstMeterEnable(enable);
            SecondMeterEnable(enable);
        }

        #region Not Postback methods
        private void HandleDCUMeterAssigned()
        {
            // These fields were added to the meter object, but there doesn't appear to be a way to input them from the UI at the moment
            // because the controls never seem to be enabled.
            // It is also worth considering whether the data types and controls are in sync, for example, 
            // why is a DateTimeOffset represented with a text box instead of a date control?
            this.HasDcuCheckBox.Checked = !string.IsNullOrEmpty(this.Equipment.Meter[0].DcuID);
            this.DcuIDTextBox.Text = this.Equipment.Meter[0].DcuID;
            this.DcuVoltsTextBox.Text = this.Equipment.Meter[0].DcuBatteryVoltage?.ToString("0.0000") ?? string.Empty;
            this.DcuAmpsTextBox.Text = this.Equipment.Meter[0].DcuBatteryCurrent?.ToString("0.0000") ?? string.Empty;
            this.DcuTemperatureTextBox.Text = this.Equipment.Meter[0].DcuTemperature?.ToString("0.0000") ?? string.Empty;
            this.DcuResetsTextBox.Text = this.Equipment.Meter[0].DcuResets?.ToString() ?? string.Empty;
            this.DcuUpdatedTextBox.Text = this.Equipment.Meter[0].DcuUpdateDate?.ToString("d") ?? string.Empty;
            this.DcuConfigurationDateTextBox.Text = this.Equipment.Meter[0].DcuConfigurationDate?.ToString("d") ?? string.Empty;
            this.DcuFirmwareVersionTextBox.Text = this.Equipment.Meter[0].DcuFirmwareVersion;
            this.DcuBluetoothAddressTextBox.Text = this.Equipment.Meter[0].DcuBluetoothAddress;
        }

        private void HandleDualMeterAssigned()
        {
            this.MeterConfigRadioGroup.SelectedValue = __DUAL_METER__;
            this.EnableControls(true);

            this.MeterID2TextBox.Text = this.Equipment.Meter[1].ID;
            this.NumberOfDigits2TextBox.Text = this.Equipment.Meter[1].NumberOfDigits.ToString();
            this.RotatesBackward2CheckBox.Checked = this.Equipment.Meter[1].RotatesBackwardsFlag;
            this.ReceiptMeter2CheckBox.Checked = this.Equipment.Meter[1].ReceiptMeterFlag;
            this.MeterFactor2TextBox.Text = this.Equipment.Meter[1].MeterFactor?.ToString("F4") ?? string.Empty;
            this.FuelCompression2TextBox.Text = this.Equipment.Meter[1].FuelCompressionFactor?.ToString("F4") ?? string.Empty;
        }
        #endregion 

        private void HandleNotPostback()
        {
            // If the equipment has a meter assigned to it, populate the controls on the screen.
            if (this.Equipment.Meter != null && this.Equipment.Meter.Count > 0)
            {
                HandleGeneralMeterAssigned();
                HandleDCUMeterAssigned();

                //Dual Meter
                if (this.Equipment.Meter.Count > 1)
                {
                    HandleDualMeterAssigned();
                }
            }
            else
            {
                // If there is no meter, indicate that on the screen.
                // by setting the has meter check box to unchecked and disabling the controls.
                this.MeterConfigRadioGroup.SelectedValue = __NO_METER__;
                this.EnableControls(false);
            }

            this.SetFieldAccessibilityForChildRecordVersion();
        }

        /// <summary>
        /// Perform processing at the time the page is loaded
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this.MeterConfigRadioGroup.SelectedIndexChanged += MeterConfigRadioGroup_SelectedIndexChanged;

                if (!this.Page.IsPostBack)
                {
                    HandleNotPostback();
                }
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

        private void MeterConfigRadioGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            // If the dual meter box was checked, update the meter ID of the first meter by appending an 'A'
            // Set the second meter's ID to the ID of the piece of equipment appending a 'B'. 

            if (this.MeterConfigRadioGroup.SelectedValue == __NO_METER__)
            {
                //first meter
                this.MeterIDTextBox.Text = string.Empty;
                this.RotatesBackwardCheckBox.Checked = false;
                this.NumberOfDigitsTextBox.Text = string.Empty;
                this.ReceiptMeterCheckBox.Checked = false;
                this.MeterFactorTextBox.Text = string.Empty;
                this.FuelCompressionTextBox.Text = string.Empty;

                //second meter
                this.MeterID2TextBox.Text = string.Empty;
                this.RotatesBackward2CheckBox.Checked = false;
                this.NumberOfDigits2TextBox.Text = string.Empty;
                this.ReceiptMeter2CheckBox.Checked = false;
                this.MeterFactor2TextBox.Text = string.Empty;
                this.FuelCompression2TextBox.Text = string.Empty;

                FirstMeterEnable(false);
                SecondMeterEnable(false);
            }
            else if (this.MeterConfigRadioGroup.SelectedValue == __SINGLE_METER__)
            {
                FirstMeterEnable(true);
                SecondMeterEnable(false);
            }
            else if (this.MeterConfigRadioGroup.SelectedValue == __DUAL_METER__)
            {
                FirstMeterEnable(true);
                SecondMeterEnable(true);
            }
        }

        #region Update Data methods

        private void FirstMeterEnable(bool enable)
        {
            this.MeterIDTextBox.Enabled = enable;
            this.NumberOfDigitsTextBox.Enabled = enable;
            this.RotatesBackwardCheckBox.Enabled = enable;
            this.ReceiptMeterCheckBox.Enabled = enable;
            this.MeterFactorTextBox.Enabled = enable;
            this.FuelCompressionTextBox.Enabled = enable;

            if(enable)
            {
                if (String.IsNullOrEmpty(NumberOfDigitsTextBox.Text)) this.NumberOfDigitsTextBox.Text = NumberOfDigits.ToString();
                if (String.IsNullOrEmpty(MeterFactorTextBox.Text)) this.MeterFactorTextBox.Text = MeterFactor.ToString("F4", CultureInfo.InvariantCulture);
                if (String.IsNullOrEmpty(FuelCompressionTextBox.Text)) this.FuelCompressionTextBox.Text = FuelCompressionFactor.ToString("F4", CultureInfo.InvariantCulture);
            }
        }

        private void SecondMeterEnable(bool enable)
        {
            this.MeterID2TextBox.Enabled = enable;
            this.NumberOfDigits2TextBox.Enabled = enable;
            this.RotatesBackward2CheckBox.Enabled = enable;
            this.ReceiptMeter2CheckBox.Enabled = enable;
            this.MeterFactor2TextBox.Enabled = enable;
            this.FuelCompression2TextBox.Enabled = enable;

            if (enable)
            {
                if(String.IsNullOrEmpty(NumberOfDigits2TextBox.Text)) this.NumberOfDigits2TextBox.Text =  NumberOfDigits.ToString();
                if(String.IsNullOrEmpty(MeterFactor2TextBox.Text)) this.MeterFactor2TextBox.Text =  MeterFactor.ToString("F4", CultureInfo.InvariantCulture);
                if (String.IsNullOrEmpty(FuelCompression2TextBox.Text)) this.FuelCompression2TextBox.Text = FuelCompressionFactor.ToString("F4", CultureInfo.InvariantCulture);
            }
        }

        private void ReformatMeterIDOfMeter1()
        {
            if ("A".Equals(this.MeterIDTextBox.Text.Right(1)))
            {
                this.MeterIDTextBox.Text = this.MeterIDTextBox.Text.Substring(0, this.MeterIDTextBox.Text.Length - 1);
            }
        }

        private bool HandleEquipmentMeterCountOneOrGreater(int count, string meterText, string numberOfDigitsTextBox, 
                                bool rotatesBackwardCheckBoxChecked, bool receiptMeterCheckBoxChecked,
                                string meterFactorTextBox, string fuelCompressionTextBox)
        {
            //update existing MeterClass
            this.Equipment.Meter[count - 1].ID = meterText;
            this.Equipment.Meter[count - 1].NumberOfDigits = MeterClass.ValidateNumberOfDigits(numberOfDigitsTextBox);
            this.Equipment.Meter[count - 1].RotatesBackwardsFlag = rotatesBackwardCheckBoxChecked;
            this.Equipment.Meter[count - 1].ReceiptMeterFlag = receiptMeterCheckBoxChecked;
            this.Equipment.Meter[count - 1].MeterFactor = MeterClass.ValidateMeterFactor(meterFactorTextBox);
            this.Equipment.Meter[count - 1].FuelCompressionFactor = MeterClass.ValidateFuelCompressionFactor(fuelCompressionTextBox);

            return true;
        }

        private bool HandleEquipmentMeterCountIsZero(string meterText, 
                        string numberOfDigitsTextBox, 
                        bool rotatesBackwardCheckBoxChecked, 
                        bool receiptMeterCheckBoxChecked,
                        string meterFactorTextBox, 
                        string fuelCompressionTextBox,
                        Guid siteGuid, 
                        string siteID)
        {
            MeterClass mc = new MeterClass
            {
                ID = meterText,
                NumberOfDigits = MeterClass.ValidateNumberOfDigits(numberOfDigitsTextBox),
                RotatesBackwardsFlag = rotatesBackwardCheckBoxChecked,
                ReceiptMeterFlag = receiptMeterCheckBoxChecked,
                MeterFactor = MeterClass.ValidateMeterFactor(meterFactorTextBox),
                FuelCompressionFactor = MeterClass.ValidateFuelCompressionFactor(fuelCompressionTextBox),
                SiteGuid = siteGuid,
                SiteID = siteID
            };

            this.Equipment.Meter.Add(mc);
            return true;
        }

        private bool HandleIfDualMeter()
        {
            if (string.IsNullOrEmpty(this.MeterID2TextBox.Text))
            {
                throw new ApplicationException("Meter 2 ID is required");
            }

            SecondMeterEnable(MeterConfigRadioGroup.SelectedValue == __DUAL_METER__);

            _ = (this.Equipment.Meter.Count > 1) ? 
                        this.HandleEquipmentMeterCountOneOrGreater(this.Equipment.Meter.Count,
                        this.MeterID2TextBox.Text,
                        this.NumberOfDigits2TextBox.Text,
                        this.RotatesBackward2CheckBox.Checked,
                        this.ReceiptMeter2CheckBox.Checked,
                        this.MeterFactor2TextBox.Text,
                        this.FuelCompression2TextBox.Text)
                        : 
                        this.HandleEquipmentMeterCountIsZero(this.MeterID2TextBox.Text,
                        this.NumberOfDigits2TextBox.Text,
                        this.RotatesBackward2CheckBox.Checked,
                        this.ReceiptMeter2CheckBox.Checked,
                        this.MeterFactor2TextBox.Text,
                        this.FuelCompression2TextBox.Text,
                        this.Security.SiteGuid,
                        this.Security.SiteID);

            return true;
        }

        private bool HandleIfNotDualMeter()
        {
            //if there was a second meter, remove it from the collection
            if (this.Equipment.Meter.Count > 1)
            {
                this.Equipment.Meter.RemoveAt(1);
            }
            return true;
        }

        private void HandleHasMeterCheckBoxChecked()
        {
            if (string.IsNullOrEmpty(this.MeterIDTextBox.Text))
            {
                throw new ApplicationException("Meter 1 ID is required");
            }

            if (this.Equipment.Meter.Count > 0)
            {
                this.HandleEquipmentMeterCountOneOrGreater(1, 
                this.MeterIDTextBox.Text,
                this.NumberOfDigitsTextBox.Text,
                this.RotatesBackwardCheckBox.Checked,
                this.ReceiptMeterCheckBox.Checked,
                this.MeterFactorTextBox.Text,
                this.FuelCompressionTextBox.Text);
            }
            else
            {
                this.HandleEquipmentMeterCountIsZero(this.MeterIDTextBox.Text,
                this.NumberOfDigitsTextBox.Text,
                this.RotatesBackwardCheckBox.Checked,
                this.ReceiptMeterCheckBox.Checked,
                this.MeterFactorTextBox.Text,
                this.FuelCompressionTextBox.Text,
                Guid.Empty,
                "");
            }

            //Dual Meter
            _ = this.MeterConfigRadioGroup.SelectedValue == __DUAL_METER__ ? this.HandleIfDualMeter() : this.HandleIfNotDualMeter();
        }

        /// <summary>
        /// Reads the meter data the user entered on the form and updates the equipment object with it.
        /// </summary>
        public void UpdateData()
		{
			if (this.MeterConfigRadioGroup.SelectedValue != __NO_METER__)
			{
                this.HandleHasMeterCheckBoxChecked();
            }
			else
			{
				//the user indicated that there is no meter assigned to the equipment, so set meter list to empty list.
				this.Equipment.Meter = new List<MeterClass>();
			}
		}

        private void SetFieldAccessibilityForChildRecordVersion()
        {
            bool currentSiteOwnsRecordVersion = (this.Equipment.SiteGuid == this.Security.SiteGuid);

			if (this.Equipment.IdentityGuid.Equals(Guid.Empty)
				|| (currentSiteOwnsRecordVersion && this.Equipment.IdentityGuid.Equals(this.Equipment.MasterRecordGuid)))
			{
				return;
			}

            //Meter attributes are not covered by Equipment Record Versioning
            this.MeterConfigRadioGroup.Enabled = false;
            this.EnableControls(false);
        }

        #endregion Update Data methods

        #region Auto Generate Meter Functions

        private void AutoGenerateSingleMeterGeneralSetup(EquipmentTypeChangedEventArgs e)
        {
            this.MeterConfigRadioGroup.SelectedValue = __SINGLE_METER__;
            MeterConfigRadioGroup_SelectedIndexChanged(this, EventArgs.Empty);
            this.MeterIDTextBox.Text = e.EquipmentID;
            this.NumberOfDigitsTextBox.Text = NumberOfDigits.ToString();
            this.MeterFactorTextBox.Text = MeterFactor.ToString("F4", CultureInfo.InvariantCulture);
            this.FuelCompressionTextBox.Text = FuelCompressionFactor.ToString("F4", CultureInfo.InvariantCulture);
        }

        public void AutoGenerateSingleMeter(object sender, EquipmentTypeChangedEventArgs e)
        {
            if (e.AutoCreateMeter)
            {
                //if there's already something there, don't overwrite it, probably
                if (this.MeterConfigRadioGroup.SelectedValue == __NO_METER__)
                {
                    AutoGenerateSingleMeterGeneralSetup(e);

                    // need to add a meter to the collection
                    MeterClass mc = new MeterClass
                    {
                        ID = e.EquipmentID,
                        NumberOfDigits = NumberOfDigits,
                        MeterFactor = MeterFactor,
                        FuelCompressionFactor = FuelCompressionFactor,
                        SiteGuid = this.Security.SiteGuid,
                        SiteID = this.Security.SiteID
                    };

                    this.Equipment.Meter.Add(mc);
                }
            }
        }

        private void AutoGenerateDualMeterUpdateUI()
        {
            this.MeterID2TextBox.Text = this.MeterIDTextBox.Text + "B";
            this.MeterIDTextBox.Text = this.MeterIDTextBox.Text + "A";
            this.NumberOfDigits2TextBox.Text = NumberOfDigits.ToString();
            this.MeterFactor2TextBox.Text = MeterFactor.ToString("F4", CultureInfo.InvariantCulture);
            this.FuelCompression2TextBox.Text = FuelCompressionFactor.ToString("F4", CultureInfo.InvariantCulture);
        }

        public void AutoGenerateDualMeter()
        {
            ////I need to add a meter to the collection
            MeterClass mc = new MeterClass
            {
                ID = this.MeterIDTextBox.Text + "B",
                NumberOfDigits = NumberOfDigits,
                MeterFactor = MeterFactor,
                FuelCompressionFactor = FuelCompressionFactor,
                SiteGuid = this.Security.SiteGuid,
                SiteID = this.Security.SiteID
            };

            this.Equipment.Meter.Add(mc);

            //update UI
            AutoGenerateDualMeterUpdateUI();
        }

        #endregion
    }
}