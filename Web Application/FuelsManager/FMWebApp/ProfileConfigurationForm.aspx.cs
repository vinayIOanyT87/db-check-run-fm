// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProfileConfigurationForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ProfileConfigurationForm.aspx.cs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;

	using global::FMWebApp;

	/// <summary>
	///    This is the main page for the profile tabs pages.
	/// </summary>
	public partial class ProfileConfigurationForm : FMAutoSubmitFormBase
	{
		#region Public Methods and Operators

		/// <summary>
		///    This method will call all the tab pages and updated the page data into the profile
		///    table which will be saved to the database.
		/// </summary>
		public void UpdateData()
		{
			try
			{
				this.ProfileGeneralSettingPage.UpdateChanges();
				this.ProfileAnalogSettingPage.UpdateChanges();
				this.ProfileValidationRuleSettingPage.UpdateChanges();
				this.ProfileFuelingEquipSettingPage.UpdateChanges();
				this.ProfileDCUSettingPage.UpdateChanges();
				this.ProfilePrinterSettingPage.UpdateChanges();
				this.ProfileCommunicationSettingPage.UpdateChanges();
				this.ProfileOpsConfigSettingPage.UpdateChanges();
				this.ProfileTransactionSettingPage.UpdateChanges();
			}
			catch (Exception ex)
			{
				string errMgs = "Error retrieving updates from pages.\n" + ex.Message;
				throw new Exception(errMgs);
			}
		}

		#endregion

		#region Methods

		/// <summary>
		///    This method will handle the cancel button event. It will remove any
		///    changes.
		/// </summary>
		/// <param name="sender">Sender object from the event.</param>
		/// <param name="e">Event arguments.</param>
		protected void CancelBtnCommand(object sender, CommandEventArgs e)
		{
			// Remove the previous profile.
			this.Session.Remove(PageSessionKeyConstants.ProfileConfigurationProfileObject);
			this.Session.Remove(PageSessionKeyConstants.ProfileConfigurationItemToEdit);

			this.Redirect("ProfileConfigSummaryForm.aspx");
		}

		/// <summary>
		///    This method will handle the new button event. It will presist the existing data,
		///    remove the information from session, create a new profile and save the table in
		///    session.
		/// </summary>
		/// <param name="sender">Sender object from the event.</param>
		/// <param name="e">Event arguments.</param>
		protected void NewBtnCommand(object sender, CommandEventArgs e)
		{
			var mobileDeviceProfile =
				this.Session[PageSessionKeyConstants.ProfileConfigurationProfileObject] as MobileDeviceProfile;

			if (mobileDeviceProfile != null)
			{
				// Save the existing profile to the database.
				this.UpdateData();

				if (mobileDeviceProfile.MobileDeviceProfileGuid == Guid.Empty)
				{
					FMChannelHelper.MakeCall<IMobileDeviceProfiles, MobileDeviceProfile>(
																	 x =>
																	 x.Add(this.Security, mobileDeviceProfile)
																);

				}
				else
				{
					FMChannelHelper.MakeCall<IMobileDeviceProfiles, MobileDeviceProfile>(
																	 x =>
																	 x.Modify(this.Security, mobileDeviceProfile)
																);
				}

				// Remove the previous profile and add a new one.
				this.Session.Remove(PageSessionKeyConstants.ProfileConfigurationProfileObject);
				this.Session.Remove(PageSessionKeyConstants.ProfileConfigurationItemToEdit);

				var newProfileObj = new MobileDeviceProfile();
				this.Session.Add(PageSessionKeyConstants.ProfileConfigurationProfileObject, newProfileObj);

				this.ProfileGeneralSettingPage.ResetFieldsForNewEvent();
				this.ProfileAnalogSettingPage.ResetFieldsForNewEvent();
				this.ProfileValidationRuleSettingPage.ResetFieldsForNewEvent();
				this.ProfileFuelingEquipSettingPage.ResetFieldsForNewEvent();
				this.ProfileDCUSettingPage.ResetFieldsForNewEvent();
				this.ProfilePrinterSettingPage.ResetFieldsForNewEvent();
				this.ProfileCommunicationSettingPage.ResetFieldsForNewEvent();
				this.ProfileOpsConfigSettingPage.ResetFieldsForNewEvent();
				this.ProfileTransactionSettingPage.ResetFieldsForNewEvent();
			}
		}

		/// <summary>
		///    This method will handle the OK button event. It will presist the existing data
		///    to the database.
		/// </summary>
		/// <param name="sender">Sender object from the event.</param>
		/// <param name="e">Event arguments.</param>
		protected void OkBtnCommand(object sender, CommandEventArgs e)
		{
			var mobileDeviceProfile =
				this.Page.Session[PageSessionKeyConstants.ProfileConfigurationProfileObject] as MobileDeviceProfile;

			if (mobileDeviceProfile != null)
			{
				try
				{
					this.UpdateData();

					if (mobileDeviceProfile.MobileDeviceProfileGuid == Guid.Empty)
					{
						FMChannelHelper.MakeCall<IMobileDeviceProfiles, MobileDeviceProfile>(
																	 x =>
																	 x.Add(this.Security, mobileDeviceProfile)
																);
					}
					else
					{
						FMChannelHelper.MakeCall<IMobileDeviceProfiles>(
																	 x =>
																	 x.Modify(this.Security, mobileDeviceProfile)
																);
					}

					this.Session.Remove(PageSessionKeyConstants.ProfileConfigurationProfileObject);
					this.Session.Remove(PageSessionKeyConstants.ProfileConfigurationItemToEdit);

					this.Redirect("ProfileConfigSummaryForm.aspx");
				}
				catch (Exception ex)
				{
					this.ErrorHandler(ex);
				}
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
			this.GetSecurity();

			this.ApplyDataDictionaryToTabs();

			if (this.Page.IsPostBack == false)
			{
				object profileObj = this.Session[PageSessionKeyConstants.ProfileConfigurationProfileObject];
				object profileGuid = this.Session[PageSessionKeyConstants.ProfileConfigurationItemToEdit];

				// If both the profile table and profile GUID are null, then we are adding a new
				// profile. Else we are going to edit an existing profile.
				if ((profileObj == null) && (profileGuid == null))
				{
					var newMobileDeviceProfile = new MobileDeviceProfile();
					this.Session.Add(PageSessionKeyConstants.ProfileConfigurationProfileObject, newMobileDeviceProfile);
				}
				else
				{
					this.RetrieveProfileData();
				}
			}

			this.DisableFields();
		}

		/// <summary>
		///    This method will appy the data dictionary to the tab text.
		/// </summary>
		private void ApplyDataDictionaryToTabs()
		{
			this.tpGeneralSettingsPage.HeaderText = this.GetDataDictionaryValueByKey(this.Security.SiteGuid, "General");
			this.tpFuelingEquipSettingsPage.HeaderText = this.GetDataDictionaryValueByKey(this.Security.SiteGuid, "Fueling Equipment");
			this.tpValidationRuleSettingsPage.HeaderText = this.GetDataDictionaryValueByKey(this.Security.SiteGuid, "Validation Rules");
			this.tpTransactionSettingsPage.HeaderText = this.GetDataDictionaryValueByKey(this.Security.SiteGuid, "Transaction");
			this.tpDCUSettingsPage.HeaderText = this.GetDataDictionaryValueByKey(this.Security.SiteGuid, "Data Capture Unit");
			this.tpAnalogSettingsPage.HeaderText = this.GetDataDictionaryValueByKey(this.Security.SiteGuid, "Analog Input");
			this.tpOpsConfigSettingPage.HeaderText = this.GetDataDictionaryValueByKey(this.Security.SiteGuid, "Operational Configuration");
			this.tpCommunicationSettingPage.HeaderText = this.GetDataDictionaryValueByKey(this.Security.SiteGuid, "Communication");
			this.tpPrinterSettingPage.HeaderText = this.GetDataDictionaryValueByKey(this.Security.SiteGuid, "Printer");
		}

		/// <summary>
		///    This method will disable all fields if the user does not have the
		///    "modify mobile device profile" right.
		/// </summary>
		private void DisableFields()
		{
			var mobileDeviceProfile =
				this.Session[PageSessionKeyConstants.ProfileConfigurationProfileObject] as MobileDeviceProfile;

			if (mobileDeviceProfile == null)
			{
				this.NewBtn.Enabled = false;
				this.OkBtn.Enabled = false;
				return;
			}

			if (mobileDeviceProfile.SiteGuid == Guid.Empty && this.Security.HasRight(RIGHT.MODIFY_MOBILE_DEVICE_PROFILES))
			{
				this.NewBtn.Enabled = true;
				this.OkBtn.Enabled = true;
			}
			else
			{
				this.NewBtn.Enabled = this.Security.HasRight(RIGHT.MODIFY_MOBILE_DEVICE_PROFILES)
				                      && (this.Security.SiteGuid == mobileDeviceProfile.SiteGuid);
				this.OkBtn.Enabled = this.Security.HasRight(RIGHT.MODIFY_MOBILE_DEVICE_PROFILES)
				                     && (this.Security.SiteGuid == mobileDeviceProfile.SiteGuid);
			}
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.NewBtn.Command += new System.Web.UI.WebControls.CommandEventHandler(this.NewBtnCommand);
			this.OkBtn.Command += new System.Web.UI.WebControls.CommandEventHandler(this.OkBtnCommand);
			this.CancelBtn.Command += new System.Web.UI.WebControls.CommandEventHandler(this.CancelBtnCommand);
		}

		/// <summary>
		///    This method will retrieve the profile data from the database based on a profile GUID.
		///    It will store the profile table in session.
		/// </summary>
		private void RetrieveProfileData()
		{
			this.Session.Remove(PageSessionKeyConstants.ProfileConfigurationProfileObject);
			var strGuid = this.Session[PageSessionKeyConstants.ProfileConfigurationItemToEdit] as string;

			if (string.IsNullOrEmpty(strGuid) == false)
			{
				Guid profileGuid = Guid.Parse(strGuid);

				MobileDeviceProfile mobileDeviceProfile = FMChannelHelper.MakeCall<IMobileDeviceProfiles, MobileDeviceProfile>(
																	 x =>
																	 x.GetByProfileGuid(this.Security, profileGuid)
																);

				this.Session.Add(PageSessionKeyConstants.ProfileConfigurationProfileObject, mobileDeviceProfile);
			}
		}

		#endregion
	}
}