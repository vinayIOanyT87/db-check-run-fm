// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProfileDCUSettingPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ProfileDCUSettingPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;

	using global::FMWebApp;

	/// <summary>
	///    This class handles the functionality for the Profile DCU tab page.
	/// </summary>
	public partial class ProfileDCUSettingPage : FMUserControlBase
	{
		#region Constants and Fields

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
			this.LoadData();
		}

		/// <summary>
		///    This method will update the profile configuration table from the general page.
		/// </summary>
		public void UpdateChanges()
		{
			this.mobileDeviceProfile =
				this.Session[PageSessionKeyConstants.ProfileConfigurationProfileObject] as MobileDeviceProfile;

			if (this.mobileDeviceProfile != null)
			{
				this.mobileDeviceProfile.HasDCU = this.HasDcuCB.Checked;
				this.mobileDeviceProfile.BluetoothDcu = this.BluetoothDcuCB.Checked;
				this.mobileDeviceProfile.LogDCUActions = this.LogDcuActionsCB.Checked;
				this.mobileDeviceProfile.HasAveryHardoll = this.HasAveryHardollCB.Checked;

				// Convert to integer and check range.
				this.mobileDeviceProfile.DcuReadRetry = this.ConvertToIntAndTestRange(
					this.DcuReadyRetryTB.Text, 1, 100, "DCU Read Retry");
				this.mobileDeviceProfile.DcuDisconnectDelay = this.ConvertToIntAndTestRange(
					this.DcuDisconnectDelayTB.Text, 1, 100, "DCU Disconnect Delay");
				this.mobileDeviceProfile.DcuCommunicationFailRestart =
					this.ConvertToIntAndTestRange(this.DcuCommunicationFailRestartTB.Text, 1, 100, "DCU Communication Fail Restart");

				this.mobileDeviceProfile.DcuComPort = null;
				if (this.DcuComPortDD.SelectedIndex != 0)
				{
					this.mobileDeviceProfile.DcuComPort = this.DcuComPortDD.SelectedItem.Value;
				}

				this.mobileDeviceProfile.AveryHardollComPort = null;
				if (this.AveryHardollComPortDD.SelectedIndex != 0)
				{
					this.mobileDeviceProfile.AveryHardollComPort = this.AveryHardollComPortDD.SelectedItem.Value;
				}
			}
		}

		#endregion

		#region Methods

		/// <summary>
		///    This method handles the Has Avery checkbox change event.
		/// </summary>
		/// <param name="sender">
		///    The sender.
		/// </param>
		/// <param name="e">
		///    The e.
		/// </param>
		protected void HasAveryCheckedChange(object sender, EventArgs e)
		{
			this.mobileDeviceProfile =
				this.Session[PageSessionKeyConstants.ProfileConfigurationProfileObject] as MobileDeviceProfile;

			if (this.mobileDeviceProfile == null)
			{
				return;
			}

			if (this.HasAveryHardollCB.Checked)
			{
				this.ClearDcuFields();
				this.EnableDisableDcuFields(false);
				this.EnableDisableAveryFields(true);
			}
			else
			{
				this.EnableDisableAveryFields(false);
				this.ClearAveryHardollFields();
			}

			// If neither the Has DCU checkbox and the Has Avery Hardoll checkbox are not
			// checked, then enable both.
			if (this.HasDcuCB.Checked == false && this.HasAveryHardollCB.Checked == false)
			{
				this.HasDcuCB.Enabled = true;
				this.HasAveryHardollCB.Enabled = true;
			}
		}

		/// <summary>
		///    This method handles the Has DCU checkbox change event.
		/// </summary>
		/// <param name="sender">
		///    The sender.
		/// </param>
		/// <param name="e">
		///    The e.
		/// </param>
		protected void HasDcuCheckedChange(object sender, EventArgs e)
		{
			this.mobileDeviceProfile =
				this.Session[PageSessionKeyConstants.ProfileConfigurationProfileObject] as MobileDeviceProfile;

			if (this.mobileDeviceProfile == null)
			{
				return;
			}

			if (this.HasDcuCB.Checked)
			{
				this.ClearAveryHardollFields();
				this.EnableDisableAveryFields(false);
				this.EnableDisableDcuFields(true);
			}
			else
			{
				this.EnableDisableDcuFields(false);
				this.ClearDcuFields();
			}

			// If neither the Has DCU checkbox and the Has Avery Hardoll checkbox are not
			// checked, then enable both.
			if (this.HasDcuCB.Checked == false && this.HasAveryHardollCB.Checked == false)
			{
				this.HasDcuCB.Enabled = true;
				this.HasAveryHardollCB.Enabled = true;
			}
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
			this.ApplyDataDictionaryToPanes();

			if (this.Page.IsPostBack == false)
			{
				this.LoadData();
			}

			this.DisableFields();
		}

		/// <summary>
		///    This method applies the data dictionary to the Grouping Pane text.
		/// </summary>
		private void ApplyDataDictionaryToPanes()
		{
			this.DCUPanel.GroupingText = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.Security.SiteGuid, "DCU")
																);

			this.AveryHardollPanel.GroupingText = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.Security.SiteGuid, "Avery Hardoll")
																);
		}

		/// <summary>
		///    This method will clear all the Avery Hardoll fields and the corresponding
		///    fields in the data object.
		/// </summary>
		private void ClearAveryHardollFields()
		{
			this.HasAveryHardollCB.Checked = false;
			this.AveryHardollComPortDD.SelectedIndex = 0;
			this.AveryHardollMeterIDTB.Text = string.Empty;

			this.mobileDeviceProfile.HasAveryHardoll = false;
			this.mobileDeviceProfile.AveryHardollComPort = string.Empty;
			this.mobileDeviceProfile.AveryHardollMeterId = string.Empty;
		}

		/// <summary>
		///    This method will clear all the DCU fields and the corresponding
		///    fields in the data object.
		/// </summary>
		private void ClearDcuFields()
		{
			this.HasDcuCB.Checked = false;
			this.BluetoothDcuCB.Checked = false;
			this.LogDcuActionsCB.Checked = false;
			this.DcuComPortDD.SelectedIndex = 0;
			this.DcuReadyRetryTB.Text = string.Empty;
			this.DcuDisconnectDelayTB.Text = string.Empty;
			this.DcuCommunicationFailRestartTB.Text = string.Empty;

			this.mobileDeviceProfile.HasDCU = false;
			this.mobileDeviceProfile.BluetoothDcu = false;
			this.mobileDeviceProfile.LogDCUActions = false;
			this.mobileDeviceProfile.DcuComPort = string.Empty;
			this.mobileDeviceProfile.DcuReadRetry = null;
			this.mobileDeviceProfile.DcuDisconnectDelay = null;
			this.mobileDeviceProfile.DcuCommunicationFailRestart = null;
		}

		/// <summary>
		///    This method converts a string to integer and test the range. It will return null
		///    if the text value is null.
		/// </summary>
		/// <param name="textValue">
		///    The text value.
		/// </param>
		/// <param name="startRange">
		///    The start range.
		/// </param>
		/// <param name="endRange">
		///    The end range.
		/// </param>
		/// <param name="errMessage">
		///    The err message.
		/// </param>
		/// <returns>
		///    The System.Nullable`1[T -&gt; System.Int32].
		/// </returns>
		/// <exception cref="ApplicationException">
		///    Thows an out of range or non-numeric exception.
		/// </exception>
		private int? ConvertToIntAndTestRange(string textValue, int startRange, int endRange, string errMessage)
		{
			int? returnValue;
			var mustBe1Str = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.Security.SiteGuid, "must be between")
																);
			var mustBe2Str = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.Security.SiteGuid, "must be a number between")
																);

			string mustBe1 = " " + mustBe1Str + " ";
			string mustBe2 = " " + mustBe2Str + " ";

			string err1 = mustBe1 + startRange.ToString(CultureInfo.InvariantCulture) + " and "
			              + endRange.ToString(CultureInfo.InvariantCulture) + ".";
			string err2 = mustBe2 + startRange.ToString(CultureInfo.InvariantCulture) + " and "
			              + endRange.ToString(CultureInfo.InvariantCulture) + ".";
			string appErrorStr = null;

			if (string.IsNullOrEmpty(textValue))
			{
				return null;
			}

			try
			{
				int intValue = Convert.ToInt32(textValue);

				if (intValue < startRange || intValue > endRange)
				{
					throw new ApplicationException(errMessage + err1);
				}

				returnValue = intValue;
			}
			catch (FormatException)
			{
				appErrorStr = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.Security.SiteGuid, errMessage)
																);

				throw new ApplicationException(appErrorStr + err2);
			}
			catch (OverflowException)
			{
				throw new ApplicationException(appErrorStr + err2);
			}

			return returnValue;
		}

		/// <summary>
		///    This method will disable all fields if the user does not have the
		///    "modify mobile device profile" right.
		/// </summary>
		private void DisableFields()
		{
			this.HasDcuCB.Enabled = this.HasPermission();
			this.BluetoothDcuCB.Enabled = this.HasPermission();
			this.LogDcuActionsCB.Enabled = this.HasPermission();
			this.DcuComPortDD.Enabled = this.HasPermission();
			this.DcuReadyRetryTB.Enabled = this.HasPermission();
			this.DcuDisconnectDelayTB.Enabled = this.HasPermission();
			this.DcuCommunicationFailRestartTB.Enabled = this.HasPermission();
			this.HasAveryHardollCB.Enabled = this.HasPermission();
			this.AveryHardollComPortDD.Enabled = this.HasPermission();
			this.AveryHardollMeterIDTB.Enabled = this.HasPermission();
		}

		/// <summary>
		///    This method will either enable or disable Avery fields.
		/// </summary>
		/// <param name="setting">
		///    The setting.
		/// </param>
		private void EnableDisableAveryFields(bool setting)
		{
			this.HasAveryHardollCB.Enabled = setting;
			this.AveryHardollComPortDD.Enabled = setting;
			this.AveryHardollMeterIDTB.Enabled = setting;
			this.AveryHardollComPortLB.Enabled = setting;
			this.AveryHardollMeterIDLB.Enabled = setting;
		}

		/// <summary>
		///    This method will either enable or disable DCU fields.
		/// </summary>
		/// <param name="setting">
		///    The setting.
		/// </param>
		private void EnableDisableDcuFields(bool setting)
		{
			this.HasDcuCB.Enabled = setting;
			this.BluetoothDcuCB.Enabled = setting;
			this.LogDcuActionsCB.Enabled = setting;
			this.BluetoothDcuCB.Enabled = setting;
			this.DcuComPortDD.Enabled = setting;
			this.DcuReadyRetryTB.Enabled = setting;
			this.DcuDisconnectDelayTB.Enabled = setting;
			this.DcuCommunicationFailRestartTB.Enabled = setting;
			this.DcuComPortLB.Enabled = setting;
			this.DcuReadyRetryLB.Enabled = setting;
			this.DcuDisconnectDelayLB.Enabled = setting;
			this.DcuCommunicationFailRestartLbl.Enabled = setting;
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
		///    This method will load the com port dropdowns and selected the appropriate item.
		/// </summary>
		/// <param name="dcuComPortStr">
		///    The dcu com port str.
		/// </param>
		/// <param name="averyComPortStr">
		///    The avery com port str.
		/// </param>
		private void LoadComDropdowns(string dcuComPortStr, string averyComPortStr)
		{
			var compareList = new Dictionary<string, int>();
			int itemIndex = 0;

			const string DropdownListItem = "None,COM1,COM2,COM3,COM4,COM5,COM6,COM7,COM8,COM9";
			string[] dropdownListItems = DropdownListItem.Split(',');

			var dropdownList = new List<ListItem>();

			foreach (string item in dropdownListItems)
			{
				if (item.Equals("None"))
				{
					dropdownList.Add(new ListItem { Text = this.GetDictionaryValueByKey(this.Security.SiteGuid, item), Value = item });
				}
				else
				{
					dropdownList.Add(new ListItem { Text = item, Value = item });
				}

				compareList.Add(item, itemIndex++);
			}

			this.DcuComPortDD.DataSource = dropdownList;
			this.DcuComPortDD.DataTextField = "Text";
			this.DcuComPortDD.DataValueField = "Value";
			this.DcuComPortDD.Sort = false;
			this.DcuComPortDD.DataBind();
			this.DcuComPortDD.SelectedIndex = 0;

			this.AveryHardollComPortDD.DataSource = dropdownList;
			this.AveryHardollComPortDD.DataTextField = "Text";
			this.AveryHardollComPortDD.DataValueField = "Value";
			this.AveryHardollComPortDD.Sort = false;
			this.AveryHardollComPortDD.DataBind();
			this.AveryHardollComPortDD.SelectedIndex = 0;

			int selectedIndex;

			if (compareList.TryGetValue(dcuComPortStr, out selectedIndex))
			{
				this.DcuComPortDD.SelectedIndex = selectedIndex;
			}

			if (compareList.TryGetValue(averyComPortStr, out selectedIndex))
			{
				this.AveryHardollComPortDD.SelectedIndex = selectedIndex;
			}
		}

		private string GetDictionaryValueByKey(Guid guid, string item)
		{
			return FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(guid, item)
																);
		}

		/// <summary>
		///    This method will load the profile generate page with the data from the database.
		/// </summary>
		private void LoadData()
		{
			this.mobileDeviceProfile =
				this.Session[PageSessionKeyConstants.ProfileConfigurationProfileObject] as MobileDeviceProfile;

			if (this.mobileDeviceProfile != null)
			{
				this.HasDcuCB.Checked = this.mobileDeviceProfile.HasDCU;
				this.BluetoothDcuCB.Checked = this.mobileDeviceProfile.BluetoothDcu;
				this.LogDcuActionsCB.Checked = this.mobileDeviceProfile.LogDCUActions;
				this.HasAveryHardollCB.Checked = this.mobileDeviceProfile.HasAveryHardoll;

				this.AveryHardollMeterIDTB.Text = this.mobileDeviceProfile.AveryHardollMeterId;
				this.DcuReadyRetryTB.Text = string.Empty;
				this.DcuDisconnectDelayTB.Text = string.Empty;
				this.DcuCommunicationFailRestartTB.Text = string.Empty;

				if (this.mobileDeviceProfile.DcuReadRetry != null)
				{
					this.DcuReadyRetryTB.Text = this.mobileDeviceProfile.DcuReadRetry.Value.ToString(CultureInfo.InvariantCulture);
				}

				if (this.mobileDeviceProfile.DcuDisconnectDelay != null)
				{
					this.DcuDisconnectDelayTB.Text =
						this.mobileDeviceProfile.DcuDisconnectDelay.Value.ToString(CultureInfo.InvariantCulture);
				}

				if (this.mobileDeviceProfile.DcuCommunicationFailRestart != null)
				{
					this.DcuCommunicationFailRestartTB.Text =
						this.mobileDeviceProfile.DcuCommunicationFailRestart.Value.ToString(CultureInfo.InvariantCulture);
				}

				// The load the COM dropdowns and select the appropriate item.
				this.LoadComDropdowns(this.mobileDeviceProfile.DcuComPort, this.mobileDeviceProfile.AveryHardollComPort);

				if (this.mobileDeviceProfile.HasDCU)
				{
					this.HasDcuCB.Checked = true;
					this.EnableDisableDcuFields(true);
					this.EnableDisableAveryFields(false);
				}
				else
				{
					this.HasDcuCB.Checked = false;
					this.EnableDisableDcuFields(false);
				}

				if (this.mobileDeviceProfile.HasAveryHardoll)
				{
					this.HasAveryHardollCB.Checked = true;
					this.EnableDisableAveryFields(true);
					this.EnableDisableDcuFields(false);
				}
				else
				{
					this.HasAveryHardollCB.Checked = false;
					this.EnableDisableAveryFields(false);
				}

				// If neither the Has DCU flag or Has Avery Hardoll flag, then enable both
				// checkboxes for selection.
				if (this.mobileDeviceProfile.HasDCU == false && this.mobileDeviceProfile.HasAveryHardoll == false)
				{
					this.HasAveryHardollCB.Enabled = true;
					this.HasDcuCB.Enabled = true;
				}
			}
		}

		#endregion
	}
}