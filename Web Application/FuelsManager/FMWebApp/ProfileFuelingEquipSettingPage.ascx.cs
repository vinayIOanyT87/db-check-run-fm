// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProfileFuelingEquipSettingPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   This class handles the functionality for the Profile Fueling Equipment tab page.
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
	/// This class handles the functionality for the Profile Fueling Equipment tab page.
	/// </summary>
	public partial class ProfileFuelingEquipSettingPage : FMUserControlBase
	{
		#region Private data members
		/// <summary>
		/// The mobile device profile.
		/// </summary>
		private MobileDeviceProfile mobileDeviceProfile;
		#endregion

		/// <summary>
		/// This method handles the page load event.
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
				this.LoadData();
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
			this.LoadData( );
		}

		/// <summary>
		/// This method will update the profile configuration table from the fueling equipment page.
		/// </summary>
		public void UpdateChanges( )
		{
			this.mobileDeviceProfile = this.Session[PageSessionKeyConstants.ProfileConfigurationProfileObject] as MobileDeviceProfile;

			if ( this.mobileDeviceProfile != null )
			{
				this.mobileDeviceProfile.RtdTemperatureRangeMax = this.ConvertToDouble(this.RTDTempRangeMaxTB.Text, "RTD Temperature Range Max value");
				this.mobileDeviceProfile.RtdTemperatureRangeMin = this.ConvertToDouble(this.RTDTempRangeMinTB.Text, "RTD Temperature Range Min value");
				this.mobileDeviceProfile.DefaultTemperature     = this.ConvertToDouble(this.DefaultTemperatureTB.Text, "Default Temperature value");
			}
		}
		#endregion

		#region Private Methods
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
			this.RTDTempRangeMaxTB.Enabled		= this.HasPermission();
			this.RTDTempRangeMinTB.Enabled		= this.HasPermission();
			this.DefaultTemperatureTB.Enabled	= this.HasPermission();
		}

		/// <summary>
		/// This method will load the profile fueling equipment page with the data from the database.
		/// </summary>
		private void LoadData( )
		{
			this.mobileDeviceProfile = this.Session[PageSessionKeyConstants.ProfileConfigurationProfileObject] as MobileDeviceProfile;

			if ( this.mobileDeviceProfile != null )
			{
				this.RTDTempRangeMaxTB.Text    = this.mobileDeviceProfile.RtdTemperatureRangeMax == null ? string.Empty : this.mobileDeviceProfile.RtdTemperatureRangeMax.Value.ToString(CultureInfo.InvariantCulture);
				this.RTDTempRangeMinTB.Text    = this.mobileDeviceProfile.RtdTemperatureRangeMin == null ? string.Empty : this.mobileDeviceProfile.RtdTemperatureRangeMin.Value.ToString(CultureInfo.InvariantCulture);
				this.DefaultTemperatureTB.Text = this.mobileDeviceProfile.DefaultTemperature == null ? string.Empty : this.mobileDeviceProfile.DefaultTemperature.Value.ToString(CultureInfo.InvariantCulture);
			}
		}

		/// <summary>
		/// This method will convert floating point number string to a double. It will
		/// throw an exception if the value is not a number.
		/// </summary>
		/// <param name="doubleStr">
		/// The double str.
		/// </param>
		/// <param name="errMessage">
		/// The err message.
		/// </param>
		/// <returns>
		/// The System.Nullable`1[T -&gt; System.Double].
		/// </returns>
		/// <exception cref="ApplicationException">Floating point conversion error.
		/// </exception>
		private double? ConvertToDouble(string doubleStr, string errMessage)
		{
			double? returnValue;
			const string Err1 = " must be a floating point number.";

			if ( string.IsNullOrEmpty(doubleStr) )
			{
				return null;
			}

			try
			{
				returnValue = Convert.ToDouble(doubleStr);
			}
			catch ( FormatException )
			{
				throw new ApplicationException(errMessage + Err1);
			}
			catch ( OverflowException )
			{
				throw new ApplicationException(errMessage + Err1);
			}

			return returnValue;
		}
		#endregion
	}
}