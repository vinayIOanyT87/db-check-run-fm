// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SiteGeneralPage.ascx.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	Defines the SiteGeneralPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Drawing;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	using FMControls;

	using Areas.AssetTrackingArea.Controllers;

	using FMCore;

	/// <summary>
	///	Summary description for SiteGeneralForm.
	/// </summary>
	public partial class SiteGeneralPage : FMUserControlBase
	{
		#region Constants and Fields
		public bool IsDefense = false;
		protected Calendar Calendar1;
		protected FMLabel Label6;
		private bool coordinateValidationError;
		#endregion

		public bool SiteGroup => this.GroupCheckBox.Checked;
		public bool Enterprise => this.EnterpriseCheckbox.Checked;


		#region Public Methods and Operators

		/// <summary>
		///	This method handles the updating the Site data object with the information on the
		///	Site General page.
		/// </summary>
		public void UpdateData()
		{
			var localSite = (SiteClass)this.Session["Site"];

			if (this.PasswordTextBox.Text != this.ReenterPasswordTextBox.Text)
			{
				throw new Exception("Password vs. Re-enter Password does not match");
			}

			if ( this.EmailAddressTextbox.Text.IsValidEmailAddressSyntax() == false )
			{
				throw new FMEmailFormatException( "Email Address" );
			}

			localSite.ID						= this.Identifier.Text;
			localSite.Enabled					= this.EnabledCheckBox.Checked;
			localSite.Number					= this.NumberTextbox.Text;
			localSite.SPLCCode					= this.SPLCCodeTextbox.Text;
			localSite.Address1					= this.Address1Textbox.Text;
			localSite.Address2					= this.Address2Textbox.Text;
			localSite.City						= this.CityTextbox.Text;
			localSite.State						= this.StateTextbox.Text;
			localSite.Zip						= this.ZipTextbox.Text;
			localSite.Country					= this.CountryTextbox.Text;
			localSite.Phone						= this.PhoneTextbox.Text;
			localSite.Fax						= this.FaxTextbox.Text;
			localSite.EmergencyContact			= this.EmergencyContactTextbox.Text;
			localSite.EmergencyPhone			= this.EmergencyPhoneTextbox.Text;
			localSite.EmailAddress				= this.EmailAddressTextbox.Text;
			localSite.TimeZone					= this.TimeZoneDropDownList.SelectedItem.Value;
			localSite.TerminalControlNumber	= this.TerminalControlNumberTextbox.Text;
			localSite.InhibitSiteLedgerRollup	= this.InhibitSiteLedgerRollupCheckbox.Checked;
			localSite.OperateTabGroups = this.OperateTagGroupsCheckBox.Checked;

			localSite.MaxOperateTabsAllowed = SiteClass.ValidateMaxOperateTabsAllowed(MaxOperateTabsAllowed.Text);

			localSite.IATAGuid					= new Guid(this.IATADropDownList.SelectedValue);

			this.ValidateCoordinates();
			if (this.coordinateValidationError == false)
			{
				localSite.LatitudeStr = this.SiteLatitudeTextBox.Text;
				localSite.LongitudeStr = this.SiteLongitudeTextBox.Text;
				localSite.ZoomStr = this.SiteZoomTextBox.Text;
			}
			else
			{
				localSite.Latitude = null;
				localSite.Longitude = null;
				localSite.Zoom = null;
			}

			if (!string.IsNullOrEmpty(this.UserName.Text))
			{
				var user = new UserClass { ID = this.UserName.Text, Password = this.PasswordTextBox.Text };
				this.Session["User"] = user;
			}
			else
			{
				this.Session["User"] = null;
			}

		    Guid selectedItemGuid;
		    var selectedItem = this.AdGrpDropdownList.SelectedItem;
		    localSite.ActiveDirectorySiteGroupGuid = Guid.Empty;

            if (Guid.TryParse(selectedItem.Value, out selectedItemGuid))
		    {
		        localSite.ActiveDirectorySiteGroupGuid = selectedItemGuid;
		    }
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
				var localSite = (SiteClass)this.Session["Site"];

				if (this.Page.IsPostBack == false)
				{
					this.Identifier.Text				= localSite.ID;
					this.EnabledCheckBox.Checked		= localSite.Enabled;
					this.NumberTextbox.Text				= localSite.Number;
					this.SPLCCodeTextbox.Text			= localSite.SPLCCode;
					this.Address1Textbox.Text			= localSite.Address1;
					this.Address2Textbox.Text			= localSite.Address2;
					this.CityTextbox.Text				= localSite.City;
					this.StateTextbox.Text				= localSite.State;
					this.ZipTextbox.Text				= localSite.Zip;
					this.CountryTextbox.Text			= localSite.Country;
					this.PhoneTextbox.Text				= localSite.Phone;
					this.FaxTextbox.Text				= localSite.Fax;
					this.EmergencyContactTextbox.Text	= localSite.EmergencyContact;
					this.EmergencyPhoneTextbox.Text		= localSite.EmergencyPhone;
					this.EmailAddressTextbox.Text		= localSite.EmailAddress;
					this.SiteLatitudeTextBox.Text		= localSite.LatitudeStr;
					this.SiteLongitudeTextBox.Text		= localSite.LongitudeStr;
					this.SiteZoomTextBox.Text			= localSite.ZoomStr;
					this.TerminalControlNumberTextbox.Text = localSite.TerminalControlNumber;

					// Update the session information with the site coordinate
					// information.
					this.SetCoordinateInfoInSession(localSite);

					var localsecurity = new SecurityClass
											{
												UserID = this.Security.UserID,
												UserGuid = this.Security.UserGuid,
												Password = this.Security.Password,
												Token = this.Security.Token
											};

					// copy relevant memeber into local security
					localsecurity.CloneRights(this.Security);
					localsecurity.SiteID = this.Security.SiteID;
					localsecurity.SiteGuid = this.Security.SiteGuid;

					// Force the SiteGuid to the Site.IdentityGuid such that enumerations will
					// be in the correct context.
					localsecurity.SiteGuid = localSite.IdentityGuid;
					localsecurity.SiteID = localSite.ID;

					// Populate the IATADropDownList
					this.IATADropDownList.Items.Add(new ListItem(this.GetTranslatedText("{None}"), Guid.Empty.ToString()));
					IATACodeCollectionClass iataCodeCollection = FMChannelHelper.MakeCall<IIATACodes, IATACodeCollectionClass>(
																	x =>
																	x.Enumerate(localsecurity)
																);

					foreach (IATACodeClass iataCode in iataCodeCollection)
					{
						this.IATADropDownList.Items.Add(new ListItem(iataCode.ID, iataCode.IdentityGuid.ToString()));
						if (localSite.IATAGuid == iataCode.IdentityGuid)
						{
							this.IATADropDownList.SelectedIndex = this.IATADropDownList.Items.Count - 1;
						}
					}

                    // Populate the active directory group site dropdown list.
                    this.PopulateActiveDirectorySiteGroupNames(localSite);

					// Populate TimeZoneDropDownList
					this.LoadTimeZoneDropdown(localSite);

					this.InhibitSiteLedgerRollupCheckbox.Checked = localSite.InhibitSiteLedgerRollup;
					this.EnterpriseCheckbox.Checked = localSite.Enterprise;
					this.OperateTagGroupsCheckBox.Checked = localSite.OperateTabGroups;
					this.MaxOperateTabsAllowed.Text = localSite.MaxOperateTabsAllowed.ToString();
					
					if (localSite.SiteGuid != Guid.Empty)
					{
						this.UserName.Enabled = false;
						this.UserName.BackColor = Color.LightGray;
						this.PasswordTextBox.Enabled = false;
						this.PasswordTextBox.BackColor = Color.LightGray;
						this.ReenterPasswordTextBox.Enabled = false;
						this.ReenterPasswordTextBox.BackColor = Color.LightGray;
						this.GroupCheckBox.Enabled = false;
					}
					else
					{
						// When Adding a Site, if Single Site Key allow only addition of Sites.
						if (! FMChannelHelper.MakeCall<IHardwareKey, bool>(x =>x.IsMultipleSiteKey() ))
						{
							this.GroupCheckBox.Enabled = false;
						}
					}

					this.GroupCheckBox.Checked = localSite.SiteGroup;

					this.ShowHideRequiredFieldLabels();
					IsDefense = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDescKey());
            }
            else
				{
					// Update the session information with the site coordinate
					// information.
					this.SetCoordinateInfoInSession();

					this.PasswordTextBox.Attributes.Add("value", this.PasswordTextBox.Text);
					this.ReenterPasswordTextBox.Attributes.Add("value", this.ReenterPasswordTextBox.Text);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///	Required method for Designer support - do not modify
		///	the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// This method will validate the coordinate information.
		/// </summary>
		private void ValidateCoordinates()
		{
			this.coordinateValidationError = false;

			if (string.IsNullOrEmpty(this.SiteLatitudeTextBox.Text) 
				&& string.IsNullOrEmpty(this.SiteLongitudeTextBox.Text)
				&& string.IsNullOrEmpty(this.SiteZoomTextBox.Text))
			{
				return;
			}

			if ((string.IsNullOrEmpty(this.SiteLatitudeTextBox.Text) == false && string.IsNullOrEmpty(this.SiteLongitudeTextBox.Text))
				|| (string.IsNullOrEmpty(this.SiteLongitudeTextBox.Text) == false && string.IsNullOrEmpty(this.SiteLatitudeTextBox.Text)))
			{
				this.coordinateValidationError = true;
				throw new Exception("Must have both Latitude and Longitude.");
			}

			if ((string.IsNullOrEmpty(this.SiteLatitudeTextBox.Text) == false || string.IsNullOrEmpty(this.SiteLongitudeTextBox.Text) == false)
				&& string.IsNullOrEmpty(this.SiteZoomTextBox.Text))
			{
				this.coordinateValidationError = true;
				throw new Exception("Must have a zoom value.");
			}

			double latOut;
			double longOut;
			int zoomOut;

			if (double.TryParse(this.SiteLatitudeTextBox.Text, out latOut) == false)
			{
				this.coordinateValidationError = true;
				throw new Exception("Must be numeric.");
			}

			if (double.TryParse(this.SiteLongitudeTextBox.Text, out longOut) == false)
			{
				this.coordinateValidationError = true;
				throw new Exception("Must be numeric.");
			}

			if (int.TryParse(this.SiteZoomTextBox.Text, out zoomOut) == false)
			{
				this.coordinateValidationError = true;
				throw new Exception("Must be numeric.");
			}

			if (latOut < -90 || latOut > 90)
			{
				this.coordinateValidationError = true;
				throw new Exception("Must be between -90 and 90 degrees.");
			}

			if (longOut < -180 || longOut > 180)
			{
				this.coordinateValidationError = true;
				throw new Exception("Must be between -180 and 180 degrees.");
			}

			if (zoomOut < 0 || zoomOut > 25)
			{
				this.coordinateValidationError = true;
				throw new Exception("Must be between 0 and 25.");
			}
		}

		/// <summary>
		/// This method will set the coordinate info into session for the popup.
		/// </summary>
		private void SetCoordinateInfoInSession()
		{
			if (string.IsNullOrEmpty(this.SiteLatitudeTextBox.Text) == false)
			{
				this.Session[AssetCalculateCoordinatesController.SessionCalculateCoordinateLatitude] = this.SiteLatitudeTextBox.Text;
			}

			if (string.IsNullOrEmpty(this.SiteLongitudeTextBox.Text) == false)
			{
				this.Session[AssetCalculateCoordinatesController.SessionCalculateCoordinateLongitude] = this.SiteLongitudeTextBox.Text;
			}

			if (string.IsNullOrEmpty(this.SiteZoomTextBox.Text) == false)
			{
				this.Session[AssetCalculateCoordinatesController.SessionCalculateCoordinateZoom] = this.SiteZoomTextBox.Text;
			}
		}

		/// <summary>
		/// This method will set the coordinate info into session for the popup.
		/// </summary>
		private void SetCoordinateInfoInSession(SiteClass inSite)
		{
			if (this.Session[AssetCalculateCoordinatesController.SessionCalculateCoordinateZoom] == null)
			{
				this.Session.Add(AssetCalculateCoordinatesController.SessionCalculateCoordinateZoom, inSite.Zoom.ToString());
			}

			if (inSite.Latitude != null)
			{
				this.Session.Add(AssetCalculateCoordinatesController.SessionCalculateCoordinateLatitude, inSite.Latitude.ToString());
			}

			if (inSite.Longitude != null)
			{
				this.Session.Add(AssetCalculateCoordinatesController.SessionCalculateCoordinateLongitude, inSite.Longitude.ToString());
			}

		}
		/// <summary>
		///	This method will populate the time zone dropdown list based on the GetSystemTimeZone
		///	method in TimeZoneInfo.  The list is based on the host computer.
		/// </summary>
		/// <param name="site"></param>
		private void LoadTimeZoneDropdown(SiteClass site)
		{
			// Populate TimeZoneDropDownList 
			ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();

			foreach (TimeZoneInfo timeZoneInfo in timeZones)
			{
				var item = new ListItem(timeZoneInfo.DisplayName, timeZoneInfo.Id);
				this.TimeZoneDropDownList.Items.Add(item);

				if (timeZoneInfo.Id.Equals(site.TimeZone))
				{
					this.TimeZoneDropDownList.SelectedIndex = this.TimeZoneDropDownList.Items.Count - 1;
				}
			}
		}

		private void ShowHideRequiredFieldLabels()
		{
			var localSite = (SiteClass)this.Session["Site"];

			bool isVisible = false; // default: Always hide labels in every version except DESC

			if (FMChannelHelper.MakeCall<IHardwareKey, Boolean>(x =>x.IsDescKey() ))
			{
				// Show/Hide Labels based on Site Group and Index for DESC
				isVisible = !localSite.SiteGroup && !localSite.IsAdminSite;
			}

			this.RequiredFMLABEL0.Visible = isVisible;
			this.RequiredFMLABEL1.Visible = isVisible;
			this.RequiredFMLABEL2.Visible = isVisible;
			this.RequiredFMLABEL3.Visible = isVisible;
			this.RequiredFMLABEL4.Visible = isVisible;
			this.RequiredFMLABEL5.Visible = isVisible;
		}

		/// <summary>
		/// This method gets the SSO mode.
		/// </summary>
		/// <returns>Returns true if in SSO mode, otherwise false.</returns>
		private bool IsSsoMode()
		{
			bool ssoMode = false;

			try
			{
				var configSetting = FMChannelHelper.MakeCall<IConfigurationSettings, ConfigurationSettingDOClass>
													(x => x.GetByKey(this.Security, ConfigurationSettingDOClass.Key_SingleSignOnMode));

				// This is so that the Login page will not try and auto login the domain user.
				if (configSetting != null && string.IsNullOrEmpty(configSetting.SettingValue) == false && configSetting.SettingValue == "1")
				{
					ssoMode = true;
				}
			}
			catch (Exception)
			{
				return ssoMode;
			}

			return ssoMode;
		}

		/// <summary>
		/// This method will populate the Active Directory dropdown list with the 
		/// List of active directory site groups.
		/// </summary>
		private void PopulateActiveDirectorySiteGroupNames(SiteClass localSite)
        {
            int selectionIndex = 0;
            var adSiteGroupList = FMChannelHelper.MakeCall<IActiveDirectoryMappings, List<ActiveDirectorySiteGroup>>(
                                                                x => x.EnumerateActiveDirectorySiteList(this.Security, localSite.ActiveDirectorySiteGroupGuid));

            // This is so that the Login page will not try and auto login the domain user.
			if (this.IsSsoMode() == false)
            {
                this.AdGrpDropdownList.Enabled = false;
            }

            var adGrpSiteListItem = new ListItem { Text = "None", Value = Guid.Empty.ToString() };
            this.AdGrpDropdownList.Items.Add(adGrpSiteListItem);
            this.AdGrpDropdownList.SelectedIndex = selectionIndex;

            if (adSiteGroupList == null || adSiteGroupList.Count == 0)
            {
                return;
            }

            foreach (var adSiteGroup in adSiteGroupList)
            {
                selectionIndex++;
                adGrpSiteListItem = new ListItem { Text = adSiteGroup.Name, Value = adSiteGroup.ActiveDirectorySiteGroupGuid.ToString() };
                this.AdGrpDropdownList.Items.Add(adGrpSiteListItem);

                if (adSiteGroup.ActiveDirectorySiteGroupGuid == localSite.ActiveDirectorySiteGroupGuid)
                {
                    this.AdGrpDropdownList.SelectedIndex = selectionIndex;
                }
            }
        }
        #endregion
    }
}