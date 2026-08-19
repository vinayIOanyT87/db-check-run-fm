// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MobileDeviceConfigurationPage.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the MobileDeviceConfigurationPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;

	using global::FMWebApp;

	/// <summary>
	///    This is the main page for the mobile device tabs pages.
	/// </summary>
	public partial class MobileDeviceConfigurationPage : FMAutoSubmitFormBase
	{
		#region Public Methods and Operators

		/// <summary>
		///    This method will call all the tab pages and updated the page data into the mobile
		///    device table which will be saved to the database.
		/// </summary>
		public void UpdateData()
		{
			try
			{
				this.MobileDeviceGeneralSettingPage.UpdateChanges();
			}
			catch (Exception)
			{
				string errMgs = "Error retrieving updates from pages.";
				throw new Exception(errMgs);
			}
		}

		#endregion

		#region Methods

		/// <summary>
		///    This method will handle the cancel button event. It will remove any
		///    changes.
		/// </summary>
		/// <param name="sender">
		///    The sender.
		/// </param>
		/// <param name="e">
		///    The e.
		/// </param>
		protected void CancelButtonOnClick(object sender, EventArgs e)
		{
			// Remove the previous mobile.
			this.Session.Remove(PageSessionKeyConstants.MobileDeviceConfigurationObject);
			this.Session.Remove(PageSessionKeyConstants.MobileDeviceConfigurationItemToEdit);

			this.Redirect("MobileDeviceSummaryPage.aspx");
		}

		/// <summary>
		///    This method will handle the new button event. It will presist the existing data,
		///    remove the information from session, create a new mobile device and save the table in
		///    session.
		/// </summary>
		/// <param name="sender">
		///    The sender.
		/// </param>
		/// <param name="e">
		///    The e.
		/// </param>
		protected void NewButtonOnClick(object sender, EventArgs e)
		{
			var mobileDevice = this.Session[PageSessionKeyConstants.MobileDeviceConfigurationObject] as MobileDeviceClass;

			if (mobileDevice != null)
			{
				// Save the existing mobile device to the database.
				this.UpdateData();

				if (mobileDevice.MobileDeviceGuid == Guid.Empty)
				{
					FMChannelHelper.MakeCall<IMobileDevices>(
																	 x =>
																	 x.Add(this.Security, mobileDevice)
																);
				}
				else
				{
					FMChannelHelper.MakeCall<IMobileDevices>(
																	 x =>
																	 x.Modify(this.Security, mobileDevice)
																);
				}

				// Remove the previous mobile device and add a new one.
				this.Session.Remove(PageSessionKeyConstants.MobileDeviceConfigurationObject);
				this.Session.Remove(PageSessionKeyConstants.MobileDeviceConfigurationItemToEdit);

				var newMobileDeviceObj = new MobileDeviceClass();
				this.Session.Add(PageSessionKeyConstants.MobileDeviceConfigurationObject, newMobileDeviceObj);
			}
		}

		/// <summary>
		///    This method will handle the OK button event. It will presist the existing data
		///    to the database.
		/// </summary>
		/// <param name="sender">
		///    The sender.
		/// </param>
		/// <param name="e">
		///    The e.
		/// </param>
		protected void OkButtonOnClick(object sender, EventArgs e)
		{
			var mobileDevice = this.Page.Session[PageSessionKeyConstants.MobileDeviceConfigurationObject] as MobileDeviceClass;

			if (mobileDevice != null)
			{
				try
				{
					this.UpdateData();

					if (mobileDevice.MobileDeviceGuid == Guid.Empty)
					{
						FMChannelHelper.MakeCall<IMobileDevices, MobileDeviceClass>(
																	 x =>
																	 x.Add(this.Security,mobileDevice)
																);
					}
					else
					{
						FMChannelHelper.MakeCall<IMobileDevices>(
																	 x =>
																	 x.Modify(this.Security, mobileDevice)
																);
					}

					this.Session.Remove(PageSessionKeyConstants.MobileDeviceConfigurationObject);
					this.Session.Remove(PageSessionKeyConstants.MobileDeviceConfigurationItemToEdit);

					this.Redirect("MobileDeviceSummaryPage.aspx");
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
				object mobileDeviceObj = this.Session[PageSessionKeyConstants.MobileDeviceConfigurationObject];
				object mobileDeviceGuid = this.Session[PageSessionKeyConstants.MobileDeviceConfigurationItemToEdit];

				// If both the mobile table and mobile device GUID are null, then we are adding a new
				// mobile device. Else we are going to edit an existing mobile device.
				if ((mobileDeviceObj == null) && (mobileDeviceGuid == null))
				{
					var newMobileDevice = new MobileDeviceClass();
					this.Session.Add(PageSessionKeyConstants.MobileDeviceConfigurationObject, newMobileDevice);
				}
				else
				{
					this.RetrieveMobileDeviceData();
				}
			}

			this.DisableFields();
		}

		/// <summary>
		///    This method will appy the data dictionary to the tab text.
		/// </summary>
		private void ApplyDataDictionaryToTabs()
		{
			this.tpGeneralSettingsPage.HeaderText = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.Security.SiteGuid, "General")
																);

		}

		/// <summary>
		///    This method will disable all fields if the user does not have the
		///    "modify mobile device" right.
		/// </summary>
		private void DisableFields()
		{
			if (this.Security.HasRight(RIGHT.MODIFY_MOBILE_DEVICES) == false)
			{
				this.NewBtn.Enabled = false;
				this.OkBtn.Enabled = false;
			}
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.NewBtn.Command += new System.Web.UI.WebControls.CommandEventHandler(this.NewButtonOnClick);
			this.OkBtn.Command += new System.Web.UI.WebControls.CommandEventHandler(this.OkButtonOnClick);
			this.CancelBtn.Command += new System.Web.UI.WebControls.CommandEventHandler(this.CancelButtonOnClick);
		}

		/// <summary>
		///    This method will retrieve the mobile device data from the database based on the
		///    mobile device GUID. It will store the mobile device in session.
		/// </summary>
		private void RetrieveMobileDeviceData()
		{
			this.Session.Remove(PageSessionKeyConstants.MobileDeviceConfigurationObject);
			var strGuid = this.Session[PageSessionKeyConstants.MobileDeviceConfigurationItemToEdit] as string;

			if (string.IsNullOrEmpty(strGuid) == false)
			{
				Guid mobileDeviceGuid = Guid.Parse(strGuid);

				MobileDeviceClass mobileDevice = FMChannelHelper.MakeCall<IMobileDevices, MobileDeviceClass>(
																	 x =>
																	 x.GetByMobileDeviceGuid(this.Security, mobileDeviceGuid)
																);

				this.Session.Add(PageSessionKeyConstants.MobileDeviceConfigurationObject, mobileDevice);
			}
		}

		#endregion
	}
}