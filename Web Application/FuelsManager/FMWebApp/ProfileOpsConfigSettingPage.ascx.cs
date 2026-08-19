// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProfileOpsConfigSettingPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ProfileOpsConfigSettingPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.FMWebApp
{
	using System;
	using System.Globalization;

	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;

	using global::FMWebApp;

	/// <summary>
	/// This class is the code behind to handle the control of the Profile OPs
	/// page that is part of a multi-tab page.
	/// </summary>
	public partial class ProfileOpsConfigSettingPage : FMUserControlBase
	{
		#region Private data members
		/// <summary>
		/// The mobile device profile.
		/// </summary>
		private MobileDeviceProfile mobileDeviceProfile;
		#endregion

		/// <summary>
		/// This method will handle the page load event for the OPs page.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void Page_Load ( object sender, EventArgs e )
		{
			if ( Page.IsPostBack == false )
			{
				this.UpdateView();
			}

			this.DisableFields();
		}

		#region Public methods
		/// <summary>
		/// This method will reset all the fields when the new button is
		/// selected.
		/// </summary>
		public void ResetFieldsForNewEvent( )
		{
			this.UpdateView( );
		}

		/// <summary>
		/// This method will update the profile configuration table from the OPs page.
		/// </summary>
		public void UpdateChanges( )
		{
			this.mobileDeviceProfile = this.Session[PageSessionKeyConstants.ProfileConfigurationProfileObject] as MobileDeviceProfile;

			if ( this.mobileDeviceProfile == null )
			{
				return;
			}

			this.mobileDeviceProfile.GseWaitMsecForGetMeter			= this.ConvertToInt(this.GseWaitMsecGetMeterTB.Text, "GSE Wait MSec For Get Meter");
			this.mobileDeviceProfile.GseInactiveLogoutMinutes		= this.ConvertToInt(this.GseInactiveLogoutTB.Text, "GSE Inactive Logout Minutes");
			this.mobileDeviceProfile.GseInactiveTimeout				= this.ConvertToInt(this.GseInactivityTimeoutTB.Text, "GSE Inactivity Timeout");
			this.mobileDeviceProfile.BarcodeInvalidWarningSeconds	= this.ConvertToInt(this.BarcodeInvalidWarningTB.Text, "Barcode Invalid Warning Seconds");

			this.mobileDeviceProfile.ConfirmFuelCaps				= this.ConfirmFuelCapsCB.Checked;
			this.mobileDeviceProfile.VtoEnabled						= this.VtoEnableCB.Checked;
			this.mobileDeviceProfile.EnabledInOpGauges				= this.EnableInOpGaugesCB.Checked;
			this.mobileDeviceProfile.UseDispensingVehicleGseTrans	= this.UseDispensingVehicleGseCB.Checked;

			this.mobileDeviceProfile.DeIceBlendDefault = null;
			if ( string.IsNullOrEmpty(this.DeIceBlendDefaultTB.Text) == false )
			{
				try
				{
					this.mobileDeviceProfile.DeIceBlendDefault = Convert.ToDouble(this.DeIceBlendDefaultTB.Text);
				}
				catch (Exception)
				{
					string errMsg = "De-Ice Blend Default must be numeric.";
					throw new Exception(errMsg);
				}

				if ( this.mobileDeviceProfile.DeIceBlendDefault > 100.0 )
				{
					string errMsg = "De-Ice Blend Default cannot be greater than 100%.";
					throw new Exception(errMsg);
				}
			}
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method returns true if the user has the MODIFY_MOBILE_DEVICE_PROFILES right and the
		/// entity has not been assigned down.
		/// </summary>
		/// <returns>
		/// The System.Boolean.
		/// </returns>
		private bool HasPermission( )
		{
			this.mobileDeviceProfile = Session[PageSessionKeyConstants.ProfileConfigurationProfileObject] as MobileDeviceProfile;

			if ( this.mobileDeviceProfile == null )
			{
				return false;
			}

			if ( this.mobileDeviceProfile.SiteGuid == Guid.Empty && this.Security.HasRight(RIGHT.MODIFY_MOBILE_DEVICE_PROFILES) )
			{
				return true;
			}

			return this.Security.HasRight(RIGHT.MODIFY_MOBILE_DEVICE_PROFILES) && (this.Security.SiteGuid == this.mobileDeviceProfile.SiteGuid);
		}

		/// <summary>
		/// This method will disable all fields if the user does not have the
		/// "modify mobile device profile" right.
		/// </summary>
		private void DisableFields( )
		{
			this.GseWaitMsecGetMeterTB.Enabled		= this.HasPermission();
			this.GseInactiveLogoutTB.Enabled		= this.HasPermission( );
			this.GseInactivityTimeoutTB.Enabled		= this.HasPermission( );
			this.BarcodeInvalidWarningTB.Enabled	= this.HasPermission( );
			this.DeIceBlendDefaultTB.Enabled		= this.HasPermission( );
			this.ConfirmFuelCapsCB.Enabled			= this.HasPermission( );
			this.VtoEnableCB.Enabled				= this.HasPermission( );
			this.EnableInOpGaugesCB.Enabled			= this.HasPermission( );
			this.UseDispensingVehicleGseCB.Enabled	= this.HasPermission( );
		}

		/// <summary>
		/// This method will convert a string into an integer value. If the value is not
		/// numeric, then an exception is thrown.
		/// </summary>
		/// <param name="inStr">
		/// The in str.
		/// </param>
		/// <param name="fieldName">
		/// The field name.
		/// </param>
		/// <returns>
		/// The System.Nullable`1[T -&gt; System.Int32].
		/// </returns>
		/// <exception cref="Exception">Non numeric exception will be thrown.
		/// </exception>
		private int? ConvertToInt(string inStr, string fieldName)
		{
			int? outValue;

			if ( string.IsNullOrEmpty(inStr) )
			{
				return null;
			}

			try
			{
				outValue = Convert.ToInt32(inStr);
			}
			catch ( Exception )
			{
				string errMsg = "Field must be a numeric value.";

				if ( string.IsNullOrEmpty(fieldName) == false )
				{
					errMsg = fieldName + " field must be a numeric value.";
				}

				throw new Exception(errMsg);
			}

			return outValue;
		}

		/// <summary>
		/// This method will load the profile communication page with the data from the database.
		/// </summary>
		private void UpdateView( )
		{
			this.mobileDeviceProfile = this.Session[PageSessionKeyConstants.ProfileConfigurationProfileObject] as MobileDeviceProfile;

			if ( this.mobileDeviceProfile == null )
			{
				return;
			}

			this.GseWaitMsecGetMeterTB.Text = this.mobileDeviceProfile.GseWaitMsecForGetMeter == null
														? string.Empty
														: this.mobileDeviceProfile.GseWaitMsecForGetMeter.Value.ToString(CultureInfo.InvariantCulture);

			this.GseInactiveLogoutTB.Text = this.mobileDeviceProfile.GseInactiveLogoutMinutes == null
												? string.Empty
												: this.mobileDeviceProfile.GseInactiveLogoutMinutes.Value.ToString(CultureInfo.InvariantCulture);

			this.GseInactivityTimeoutTB.Text = this.mobileDeviceProfile.GseInactiveTimeout == null
												? string.Empty
												: this.mobileDeviceProfile.GseInactiveTimeout.Value.ToString(CultureInfo.InvariantCulture);

			this.BarcodeInvalidWarningTB.Text = this.mobileDeviceProfile.BarcodeInvalidWarningSeconds == null
												? string.Empty
												: this.mobileDeviceProfile.BarcodeInvalidWarningSeconds.Value.ToString(CultureInfo.InvariantCulture);

			this.ConfirmFuelCapsCB.Checked			= this.mobileDeviceProfile.ConfirmFuelCaps;
			this.VtoEnableCB.Checked				= this.mobileDeviceProfile.VtoEnabled;
			this.EnableInOpGaugesCB.Checked			= this.mobileDeviceProfile.EnabledInOpGauges;
			this.UseDispensingVehicleGseCB.Checked	= this.mobileDeviceProfile.UseDispensingVehicleGseTrans;

			this.DeIceBlendDefaultTB.Text = this.mobileDeviceProfile.DeIceBlendDefault == null
			                                	? string.Empty
			                                	: this.mobileDeviceProfile.DeIceBlendDefault.Value.ToString(CultureInfo.InvariantCulture);
		}
		#endregion
	}
}