// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProfileCommunicationSettingPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ProfileCommunicationSettingPage type.
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
	///    This class is the code behind to handle the control of the Profile Communication
	///    page that is part of a multi-tab page.
	/// </summary>
	public partial class ProfileCommunicationSettingPage : FMUserControlBase
	{
		#region Constants and Fields

		/// <summary>
		///    The connection types.
		/// </summary>
		private const string ConnectionTypes = "None,Online,Batch,Stand Alone";

		/// <summary>
		///    The connection type list.
		/// </summary>
		private string[] connectionTypeList;

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
			this.UpdateView();
		}

		/// <summary>
		///    This method will update the profile configuration table from the general page.
		/// </summary>
		public void UpdateChanges()
		{
			this.mobileDeviceProfile =
				this.Session[PageSessionKeyConstants.ProfileConfigurationProfileObject] as MobileDeviceProfile;

			if (this.mobileDeviceProfile == null)
			{
				return;
			}

			this.mobileDeviceProfile.CommunicationTimeoutSeconds = this.ConvertToInt(
				this.CommunicationTimeoutSecondsTB.Text, "Communication Timeout Seconds");
			this.mobileDeviceProfile.ConnectionRetries = this.ConvertToInt(this.ConnectionRetriesTB.Text, "Connection Retries");
			this.mobileDeviceProfile.UpdateInterval = this.ConvertToInt(this.UpdateIntervalTB.Text, "Update Interval");
			this.mobileDeviceProfile.VehicleUpdateInterval = this.ConvertToInt(
				this.VehicleUpdateIntervalTB.Text, "Vehicle Update Interval");
			this.mobileDeviceProfile.ConnectionRetryTimeout = this.ConvertToInt(
				this.ConnectionRetryTimeoutTB.Text, "Connection Retry Timeout");
			this.mobileDeviceProfile.PresubmitDelay = this.ConvertToInt(this.PresubmitDelayTB.Text, "Presubmit Delay");
			this.mobileDeviceProfile.VerificationIpAddress = this.ValidateIpAddress(this.VerificationIpAddressTB.Text);
			this.mobileDeviceProfile.PingVerificationIpAddress = this.PingVerificationIpAddressCB.Checked;
			this.mobileDeviceProfile.ConnectionType = this.ConnectionTypeDD.SelectedIndex;
		}

		#endregion

		#region Methods

		/// <summary>
		///    This method will handle the page load event for the communication page.
		/// </summary>
		/// <param name="sender">
		///    The sender.
		/// </param>
		/// <param name="e">
		///    The e.
		/// </param>
		protected void Page_Load(object sender, EventArgs e)
		{
			this.connectionTypeList = ConnectionTypes.Split(',');

			if (this.Page.IsPostBack == false)
			{
				this.UpdateView();
			}

			this.DisableFields();
		}

		/// <summary>
		///    This method handles the Ping Verfication IP Address check box change.
		/// </summary>
		/// <param name="sender">
		///    The sender.
		/// </param>
		/// <param name="e">
		///    The e.
		/// </param>
		protected void PingIpAddressCheckedChange(object sender, EventArgs e)
		{
			this.mobileDeviceProfile =
				this.Session[PageSessionKeyConstants.ProfileConfigurationProfileObject] as MobileDeviceProfile;

			if (this.mobileDeviceProfile == null)
			{
				return;
			}

			if (this.PingVerificationIpAddressCB.Checked)
			{
				this.VerificationIpAddressTB.Enabled = true;
				this.VerificationIpAddressLbl.Enabled = true;

				this.mobileDeviceProfile.PingVerificationIpAddress = true;
			}
			else
			{
				this.VerificationIpAddressTB.Text = string.Empty;
				this.VerificationIpAddressTB.Enabled = false;
				this.VerificationIpAddressLbl.Enabled = false;

				this.mobileDeviceProfile.VerificationIpAddress = string.Empty;
				this.mobileDeviceProfile.PingVerificationIpAddress = false;
			}
		}

		/// <summary>
		///    This method will convert a string into an integer value. If the value is not
		///    numeric, then an exception is thrown.
		/// </summary>
		/// <param name="inStr">
		///    The in str.
		/// </param>
		/// <param name="fieldName">
		///    The field name.
		/// </param>
		/// <returns>
		///    The System.Nullable`1[T -&gt; System.Int32].
		/// </returns>
		/// <exception cref="Exception">
		///    Non numeric exception will be thrown.
		/// </exception>
		private int? ConvertToInt(string inStr, string fieldName)
		{
			int? outValue;

			if (string.IsNullOrEmpty(inStr))
			{
				return null;
			}

			try
			{
				outValue = Convert.ToInt32(inStr);
			}
			catch (Exception)
			{
				string errMsg = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.Security.SiteGuid, "Field must be a numeric value.")
																);


				if (string.IsNullOrEmpty(fieldName) == false)
				{
					errMsg =
						FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
								x =>
								x.Get(this.Security.SiteGuid, fieldName) + " " + x.Get(this.Security.SiteGuid, 
								"field must be a numeric value.")
						);
				}

				throw new Exception(errMsg);
			}

			return outValue;
		}

		/// <summary>
		///    This method will disable all fields if the user does not have the
		///    "modify mobile device profile" right.
		/// </summary>
		private void DisableFields()
		{
			this.CommunicationTimeoutSecondsTB.Enabled = this.HasPermission();
			this.ConnectionRetriesTB.Enabled = this.HasPermission();
			this.UpdateIntervalTB.Enabled = this.HasPermission();
			this.VehicleUpdateIntervalTB.Enabled = this.HasPermission();
			this.ConnectionRetryTimeoutTB.Enabled = this.HasPermission();
			this.PresubmitDelayTB.Enabled = this.HasPermission();
			this.PingVerificationIpAddressCB.Enabled = this.HasPermission();
			this.VerificationIpAddressTB.Enabled = this.HasPermission();
			this.ConnectionTypeDD.Enabled = this.HasPermission();
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
		///    This method will load the profile communication page with the data from the database.
		/// </summary>
		private void UpdateView()
		{
			this.mobileDeviceProfile =
				this.Session[PageSessionKeyConstants.ProfileConfigurationProfileObject] as MobileDeviceProfile;

			if (this.mobileDeviceProfile == null)
			{
				return;
			}

			this.CommunicationTimeoutSecondsTB.Text = this.mobileDeviceProfile.CommunicationTimeoutSeconds == null
				                                          ? string.Empty
				                                          : this.mobileDeviceProfile.CommunicationTimeoutSeconds.Value.ToString(
					                                          CultureInfo.InvariantCulture);

			this.ConnectionRetriesTB.Text = this.mobileDeviceProfile.ConnectionRetries == null
				                                ? string.Empty
				                                : this.mobileDeviceProfile.ConnectionRetries.Value.ToString(
					                                CultureInfo.InvariantCulture);

			this.UpdateIntervalTB.Text = this.mobileDeviceProfile.UpdateInterval == null
				                             ? string.Empty
				                             : this.mobileDeviceProfile.UpdateInterval.Value.ToString(CultureInfo.InvariantCulture);

			this.VehicleUpdateIntervalTB.Text = this.mobileDeviceProfile.VehicleUpdateInterval == null
				                                    ? string.Empty
				                                    : this.mobileDeviceProfile.VehicleUpdateInterval.Value.ToString(
					                                    CultureInfo.InvariantCulture);

			this.ConnectionRetryTimeoutTB.Text = this.mobileDeviceProfile.ConnectionRetryTimeout == null
				                                     ? string.Empty
				                                     : this.mobileDeviceProfile.ConnectionRetryTimeout.Value.ToString(
					                                     CultureInfo.InvariantCulture);

			this.PresubmitDelayTB.Text = this.mobileDeviceProfile.PresubmitDelay == null
				                             ? string.Empty
				                             : this.mobileDeviceProfile.PresubmitDelay.Value.ToString(CultureInfo.InvariantCulture);

			this.PingVerificationIpAddressCB.Checked = this.mobileDeviceProfile.PingVerificationIpAddress;
			this.VerificationIpAddressTB.Text = string.Empty;
			this.VerificationIpAddressLbl.Enabled = false;
			this.VerificationIpAddressTB.Enabled = false;

			if (this.mobileDeviceProfile.PingVerificationIpAddress)
			{
				this.VerificationIpAddressTB.Text = this.mobileDeviceProfile.VerificationIpAddress;
				this.VerificationIpAddressLbl.Enabled = true;
				this.VerificationIpAddressTB.Enabled = true;
			}

			var dropdownItemList = new List<ListItem>();
			int itemIndex = 0;

			foreach (string dropdownItem in this.connectionTypeList)
			{
				var item = new ListItem
					{
						Text = this.GetDropDownItemText(this.Security.SiteGuid, dropdownItem),
						Value = itemIndex.ToString(CultureInfo.InvariantCulture)
					};
				dropdownItemList.Add(item);
				itemIndex++;
			}

			this.ConnectionTypeDD.DataSource = dropdownItemList;
			this.ConnectionTypeDD.DataTextField = "Text";
			this.ConnectionTypeDD.DataValueField = "Value";
			this.ConnectionTypeDD.Sort = false;
			this.ConnectionTypeDD.DataBind();

			this.ConnectionTypeDD.SelectedIndex = this.mobileDeviceProfile.ConnectionType == null
				                                      ? 0
				                                      : this.mobileDeviceProfile.ConnectionType.Value;
		}

		private string GetDropDownItemText(Guid guid, string dropdownItem)
		{
			return FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(guid, dropdownItem)
																);
		}

		/// <summary>
		///    This method will validate the IP address. It will throw and exception
		///    is the IP is invalid.
		/// </summary>
		/// <param name="ipAddress">
		///    The ip address.
		/// </param>
		/// <returns>
		///    The System.String.
		/// </returns>
		/// <exception cref="Exception">
		///    Invalid IP Address.
		/// </exception>
		private string ValidateIpAddress(string ipAddress)
		{
			if (string.IsNullOrEmpty(ipAddress))
			{
				return null;
			}

			string errMsg = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.Security.SiteGuid, "Invalid IP Address format.")
																);

			string[] ipAddressList = ipAddress.Split('.');

			if (ipAddressList.Length < 4)
			{
				throw new Exception(errMsg);
			}

			foreach (string ipSegment in ipAddressList)
			{
				if (string.IsNullOrEmpty(ipSegment) || ipSegment.Length > 3)
				{
					throw new Exception(errMsg);
				}

				try
				{
					Convert.ToInt32(ipSegment);
				}
				catch (Exception)
				{
					throw new Exception(errMsg);
				}
			}

			return ipAddress;
		}

		#endregion
	}
}