// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PasswordConfigurationForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the PasswordConfigurationForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FMControls;

	using global::FMWebApp;

	using Unity.Exceptions;

    /// <summary>
	///    Summary description for PasswordConfigurationForm.
	/// </summary>
	public partial class PasswordConfigurationForm : FMFormBase, IMenuDiscovery
	{
		#region Constants and Fields

		private const string ERR_MSG001 = "Must be positive";

		private const string ERR_MSG002 = "Must be less than or equal to 24";

		private const string ERR_MSG003 = "Error saving site configuration";

		private const string ERR_MSG004 = "Must be numeric";

		private const string ERR_MSG005 = "Error loading Site configuration data";

		private const string ERR_MSG006 = "Error saving Site configuration data";

        private const string ERR_MSG007 = "Minimum number of characters must be equal to or greater than ";

        private SiteClass site;

		#endregion

		#region Public Methods and Operators

		/// <summary>
		///    Gets a list of menu items that should be displayed for the current user.
		/// </summary>
		/// <param name="security">The security object of the current session</param>
		/// <param name="siteGroup">Whether the current logged-in site is a site group</param>
		/// <param name="options">Hardware key options</param>
		/// <returns>
		///    List of menu items to be displayed
		/// </returns>
		public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1,ushort word2, ushort useNewLicenseKey, uint options)
		{
            if (useNewLicenseKey == 1)
            {

            }
            else
            {
                // Depends Upon Shared Components Config
                if ((options & 0x4000) == 0)
                {
                    return null;
                }
            }
            var items = new List<FMMenuItem>();

			if ((!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS))
				 && (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)))
			{
				return null;
			}


			items.Add(
				new FMMenuItem
					{
						MenuItemType = FMMenuItemType.ADMIN_SECURITY_PASSWORD_SETTINGS,
						RootMenuName = "Administration",
						CategoryName = "Security",
						ItemName = "Security Settings",
						NavigateUrl = "PasswordConfigurationForm.aspx",
						ApplyDataDictionary = ApplyDataDictionary.Apply
					});

			return items;
		}

		#endregion


		#region Methods

		/// <summary>
		///    This method saves the Password configuration to the database. It throws
		///    an exceptions if the values are invalid.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void ApplyBtnOnClick(object sender, EventArgs e)
		{
			bool errFlag = false;
			int minTimeAllowed = 0;
			int minNumOfChar = 0;
			int pwdAging = 999;
			int lockoutTreshold = 0;
			int howMany = 0;
			int inactivityPeriod = 0;
			int disablePeriod = 0;

			try
			{
				if (this.DisableArchivePeriodTextbox.Text.Length > 0)
				{
					disablePeriod = Convert.ToInt32(this.DisableArchivePeriodTextbox.Text);

					if (disablePeriod < 0)
					{
						errFlag = true;
						base.ErrorHandler(new Exception(ERR_MSG001));
					}
					else
					{
						this.site.DisableArchivePeriod = disablePeriod;
					}
				}

				if (this.InactivityDisplayPeriodTextbox.Text.Length > 0)
				{
					inactivityPeriod = Convert.ToInt32(this.InactivityDisplayPeriodTextbox.Text);

					if (inactivityPeriod < 0)
					{
						errFlag = true;
						base.ErrorHandler(new Exception(ERR_MSG001));
					}
					else
					{
						if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDescKey()) && inactivityPeriod == 0)
						{
							throw new ApplicationException("Inactivity period cannot be greater than zero.");
						}

						this.site.InactivityDisablePeriod = inactivityPeriod;
					}
				}

				if (this.MinTimeAllowedTextbox.Text.Length > 0)
				{
					minTimeAllowed = Convert.ToInt32(this.MinTimeAllowedTextbox.Text);

					if (minTimeAllowed < 0)
					{
						errFlag = true;
						base.ErrorHandler(new Exception(ERR_MSG001));
					}
					else
					{
						this.site.MinTimeAllowedToChangePassword = minTimeAllowed;
					}
				}
                //<summary>
                //  Minimum # of characters option in security settings is dependent on MIN_PASSWORD_DIFF
                //  Users will get an error prompt if they enter a negative number or a number less than MIN_PASSWORD_DIFF 
                //  (Miriam 11.12.18)
                //</summary>
                if (this.MinNumOfCharTextbox.Text.Length > 0)
				{
					minNumOfChar = Convert.ToInt32(this.MinNumOfCharTextbox.Text);

                    if (minNumOfChar < 0)
					{
						errFlag = true;
						base.ErrorHandler(new Exception(ERR_MSG001));
					}
                    else if (minNumOfChar < UserClass.MIN_PASSWORD_DIFF) 
                    {
                        errFlag = true;
                        base.ErrorHandler(new Exception(ERR_MSG007 + UserClass.MIN_PASSWORD_DIFF + "."));
                    }
					else
					{
						this.site.MinPasswordCharacterLength = minNumOfChar;
					}
				}

				if (this.PwdAgingTextBox.Text.Length > 0)
				{
					pwdAging = Convert.ToInt32(this.PwdAgingTextBox.Text);

					if (pwdAging < 0)
					{
						errFlag = true;
						base.ErrorHandler(new Exception(ERR_MSG001));
					}
					else
					{
						this.site.PasswordExpirationInDays = pwdAging;
					}
				}

				if (this.LockoutThresholdTextbox.Text.Length > 0)
				{
					lockoutTreshold = Convert.ToInt32(this.LockoutThresholdTextbox.Text);

					if (lockoutTreshold < 0)
					{
						errFlag = true;
						base.ErrorHandler(new Exception(ERR_MSG001));
					}
					else
					{
						this.site.PasswordLockoutThreshold = lockoutTreshold;
					}
				}

				if (this.PreviousPwdCheckBox.Checked)
				{
					if (this.HowManyTextbox.Text.Length > 0)
					{
						howMany = Convert.ToInt32(this.HowManyTextbox.Text);

						if (howMany < 0)
						{
							errFlag = true;
							base.ErrorHandler(new Exception(ERR_MSG001));
						}
						else if (howMany > 24)
						{
							errFlag = true;
							base.ErrorHandler(new Exception(ERR_MSG002));
						}
						else
						{
							this.site.PasswordHistoryCount = howMany;
						}
					}
				}

				if (errFlag == false)
				{
					this.site.CheckForPreviousPassword = this.PreviousPwdCheckBox.Checked;

					if (false == this.StrongPwdCheckBox.Checked && false == this.EnhancedStrongPwdCheckBox.Checked)
					{
						this.site.StrongPasswordUse = (int)StrongPasswordUsage.None;
					}
					else if (this.StrongPwdCheckBox.Checked)
					{
						this.site.StrongPasswordUse = (int)StrongPasswordUsage.Strong;
					}
					else if (this.EnhancedStrongPwdCheckBox.Checked)
					{
						this.site.StrongPasswordUse = (int)StrongPasswordUsage.Enhanced;
					}

					// Set the apply to all member sites only if visible.
					if (this.ApplySettingToMemSitesCheckbox.Visible)
					{
						this.site.ApplyToAllSiteMembers = this.ApplySettingToMemSitesCheckbox.Checked;
					}
					else
					{
						this.site.ApplyToAllSiteMembers = false;
					}
					this.site.EnablePasswordHint = this.EnablePasswordHintCheckbox.Checked ? true : false;
					this.site.EnablePasswordReset = this.EnablePasswordResetCheckbox.Checked ? true : false;
					this.site.AllowUseOfSpecialChars = this.AllowSpecialCharsCheckbox.Checked ? true : false;
				}

				try
				{
					// Save the configuration settings if there were no errors.
					if (errFlag == false)
					{
						FMChannelHelper.MakeCall<ISites>(x => x.Modify(base.Security, DATA_TYPE.CONFIG, this.site, true));

						// Update all site members if the this site is a site group and the user
						// has indicated that they want all the site members to be updated.
						if (this.site.SiteGroup && this.ApplySettingToMemSitesCheckbox.Visible
							 && this.ApplySettingToMemSitesCheckbox.Checked)
						{
							this.UpdateAllMemberSites();
						}

						this.UpdateView();
					}
				}
				catch (Exception except)
				{
					base.ErrorHandler(new Exception(ERR_MSG003 + " - " + except.Message));
				}
			}
			catch (Exception)
			{
				base.ErrorHandler(new Exception(ERR_MSG004));
			}
		}

		/// <summary>
		///    This method handles the on change of the check box being checked and unchecked.
		///    If checked, then it will enable the How Many text box.  Otherwise, it will disable
		///    it.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void ChkPrevPwdOnChange(object sender, EventArgs e)
		{
			if (this.PreviousPwdCheckBox.Checked)
			{
				this.HowManyTextbox.Enabled = true;
				this.HowManyLabel.Enabled = true;
			}
			else
			{
				this.HowManyTextbox.Text = "0";
				this.HowManyTextbox.Enabled = false;
				this.HowManyLabel.Enabled = false;
			}
		}

		protected void EnhancedStrongPwdCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			if (this.EnhancedStrongPwdCheckBox.Checked)
			{
				this.StrongPwdCheckBox.Enabled = false;
			}
			else
			{
				this.StrongPwdCheckBox.Enabled = true;
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
		///    This method is the main entry point for this page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();
				this.GetConfigurationInfo();

				// Only display apply to all member sites checkbox if the
				// site is a site group.
				if (this.site.SiteGroup)
				{
					this.ApplySettingToMemSitesCheckbox.Visible = true;
				}
				else
				{
					this.ApplySettingToMemSitesCheckbox.Visible = false;
				}

				if (this.Page.IsPostBack == false)
				{
					this.UpdateView();
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		protected void StrongPwdCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			if (this.StrongPwdCheckBox.Checked)
			{
				this.EnhancedStrongPwdCheckBox.Enabled = false;
			}
			else
			{
				this.EnhancedStrongPwdCheckBox.Enabled = true;
			}
		}

		/// <summary>
		///    This method retrieves the site, which contains the Password configuration.
		/// </summary>
		private void GetConfigurationInfo()
		{
			try
			{
				Guid siteGuid = base.Security.SiteGuid;
				this.site =
					FMChannelHelper.MakeCall<ISites, SiteClass>(
							x =>
							x.Get(base.Security, siteGuid, getMemberSites: true, getSchedulesAndProcessVariables: true, 
										bGetAssociatedAliases: true)
					);
			}
			catch (Exception)
			{
				base.ErrorHandler(new Exception(ERR_MSG005));
			}
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		///    This method will update all the child sites with the with the site groups
		///    Password configuration settings.
		/// </summary>
		private void UpdateAllMemberSites()
		{
			SiteToSiteMapCollectionClass siteList = this.site.SiteToSiteMapCollection;

			if (siteList != null)
			{
				foreach (SiteToSiteMapClass siteMap in siteList)
				{
					SiteClass childSite = this.GetChildSite(
						base.Security,
						siteMap.ChildSiteGuid,
						getMemberSites: true,
						getSchedulesAndProcessVariables: true,
						bGetAssociatedAliases: true);

					childSite.MinTimeAllowedToChangePassword = this.site.MinTimeAllowedToChangePassword;
					childSite.MinPasswordCharacterLength = this.site.MinPasswordCharacterLength;
					childSite.PasswordExpirationInDays = this.site.PasswordExpirationInDays;
					childSite.PasswordLockoutThreshold = this.site.PasswordLockoutThreshold;
					childSite.PasswordHistoryCount = this.site.PasswordHistoryCount;
					childSite.CheckForPreviousPassword = this.site.CheckForPreviousPassword;
					childSite.StrongPasswordUse = this.site.StrongPasswordUse;
					childSite.InactivityDisablePeriod = this.site.InactivityDisablePeriod;
					childSite.DisableArchivePeriod = this.site.DisableArchivePeriod;
					childSite.ApplyToAllSiteMembers = false;
					childSite.EnablePasswordHint = this.site.EnablePasswordHint;
					childSite.EnablePasswordReset = this.site.EnablePasswordReset;
					childSite.AllowUseOfSpecialChars = this.site.AllowUseOfSpecialChars;

					this.ModifySite(base.Security, DATA_TYPE.CONFIG, childSite, true);
				}
			}
		}

		private void ModifySite(SecurityClass securityClass, DATA_TYPE dataType, SiteClass childSite, bool param)
		{
			FMChannelHelper.MakeCall<ISites>(
																x =>
																x.Modify(securityClass, dataType, childSite, param)
														);
		}

		private SiteClass GetChildSite(SecurityClass securityClass, Guid guid, bool getMemberSites,
			bool getSchedulesAndProcessVariables, bool bGetAssociatedAliases)
		{
			return FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(
																			securityClass,
																			guid,
																			getMemberSites,
																			getSchedulesAndProcessVariables,
																			bGetAssociatedAliases)
																	 );
		}

		/// <summary>
		///    This method will update the view with the most recent data.
		/// </summary>
		private void UpdateView()
		{
			// If the user has modify rights, then enable the controls.
			// Otherwise, disable all the controls.
			if (this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				this.StrongPwdCheckBox.Enabled = true;
				this.EnhancedStrongPwdCheckBox.Enabled = true;
				this.PreviousPwdCheckBox.Enabled = true;
				this.MinNumOfCharTextbox.Enabled = true;
				this.MinTimeAllowedTextbox.Enabled = true;
				this.LockoutThresholdTextbox.Enabled = true;
				this.PwdAgingTextBox.Enabled = true;
				this.InactivityDisplayPeriodTextbox.Enabled = true;

				this.MinNumOfCharLabel.Enabled = true;
				this.MinTimeAllowedLabel.Enabled = true;
				this.LockoutThresholdLabel.Enabled = true;
				this.PwdAgingLabel.Enabled = true;
				this.Days1Label.Enabled = true;
				this.Days2Label.Enabled = true;
				this.Days3Label.Enabled = true;
				this.ApplyBtn.Enabled = true;
				this.InactivityLabel.Enabled = true;
				if (this.ApplySettingToMemSitesCheckbox.Visible)
				{
					this.ApplySettingToMemSitesCheckbox.Enabled = true;
				}
				this.EnablePasswordHintCheckbox.Enabled = true;
				this.EnablePasswordResetCheckbox.Enabled = true;
			}
			else
			{
				this.StrongPwdCheckBox.Enabled = false;
				this.EnhancedStrongPwdCheckBox.Enabled = false;
				this.PreviousPwdCheckBox.Enabled = false;
				this.MinNumOfCharTextbox.Enabled = false;
				this.MinTimeAllowedTextbox.Enabled = false;
				this.LockoutThresholdTextbox.Enabled = false;
				this.PwdAgingTextBox.Enabled = false;
				this.HowManyTextbox.Enabled = false;
				this.InactivityDisplayPeriodTextbox.Enabled = false;

				this.MinNumOfCharLabel.Enabled = false;
				this.MinTimeAllowedLabel.Enabled = false;
				this.LockoutThresholdLabel.Enabled = false;
				this.PwdAgingLabel.Enabled = false;
				this.HowManyLabel.Enabled = false;
				this.Days1Label.Enabled = false;
				this.Days2Label.Enabled = false;
				this.Days3Label.Enabled = false;
				this.ApplyBtn.Enabled = false;
				this.InactivityLabel.Enabled = false;

				if (this.ApplySettingToMemSitesCheckbox.Visible)
				{
					this.ApplySettingToMemSitesCheckbox.Enabled = false;
				}
				this.EnablePasswordHintCheckbox.Enabled = false;
				this.EnablePasswordResetCheckbox.Enabled = false;
			}

			if (StrongPasswordUsage.None == (StrongPasswordUsage)this.site.StrongPasswordUse)
			{
				this.StrongPwdCheckBox.Checked = false;
				this.StrongPwdCheckBox.Enabled = true;
				this.EnhancedStrongPwdCheckBox.Checked = false;
				this.EnhancedStrongPwdCheckBox.Enabled = true;
			}
			else if (StrongPasswordUsage.Strong == (StrongPasswordUsage)this.site.StrongPasswordUse)
			{
				this.StrongPwdCheckBox.Checked = true;
				this.EnhancedStrongPwdCheckBox.Checked = false;
				this.EnhancedStrongPwdCheckBox.Enabled = false;
			}
			else if (StrongPasswordUsage.Enhanced == (StrongPasswordUsage)this.site.StrongPasswordUse)
			{
				this.EnhancedStrongPwdCheckBox.Checked = true;
				this.StrongPwdCheckBox.Checked = false;
				this.StrongPwdCheckBox.Enabled = false;
			}

			this.PreviousPwdCheckBox.Checked = this.site.CheckForPreviousPassword;
			this.MinNumOfCharTextbox.Text = this.site.MinPasswordCharacterLength.ToString();
			this.MinTimeAllowedTextbox.Text = this.site.MinTimeAllowedToChangePassword.ToString();
			this.LockoutThresholdTextbox.Text = this.site.PasswordLockoutThreshold.ToString();
			this.PwdAgingTextBox.Text = this.site.PasswordExpirationInDays.ToString();
			this.InactivityDisplayPeriodTextbox.Text = this.site.InactivityDisablePeriod.ToString();
			this.DisableArchivePeriodTextbox.Text = this.site.DisableArchivePeriod.ToString();

			if (this.PreviousPwdCheckBox.Checked)
			{
				this.HowManyTextbox.Enabled = true;
				this.HowManyTextbox.Text = this.site.PasswordHistoryCount.ToString();
				this.HowManyLabel.Enabled = true;
			}
			else
			{
				this.HowManyTextbox.Text = "0";
				this.HowManyTextbox.Enabled = false;
				this.HowManyLabel.Enabled = false;
			}

			// Set the apply to all member sites checkbox if it is visible.
			if (this.ApplySettingToMemSitesCheckbox.Visible)
			{
				this.ApplySettingToMemSitesCheckbox.Checked = this.site.ApplyToAllSiteMembers;
			}
			this.EnablePasswordHintCheckbox.Checked = this.site.EnablePasswordHint;
			this.EnablePasswordResetCheckbox.Checked = this.site.EnablePasswordReset;
			this.AllowSpecialCharsCheckbox.Checked = this.site.AllowUseOfSpecialChars;

            // Adding descriptions so the users will know the password policy
            this.StrongPasswordDescriptionLabel.Text = "Must be at least " + UserClass.UserDataCount + " characters long and contain at least one lower case letter, one upper case letter, one digit, and one special character (@#$%^&+.,!=).";
            this.EnhancedPasswordDescriptionLabel.Text = "Must be at least " + UserClass.UserDataCount + " characters long and contain at least two lower case letters, two upper case letters, two digits, and two special characters (@#$%^&+.,!=).";
		    this.MinNumOfCharLabel.Text = "Minimum number of characters: ";
		    this.MinNumOfCharLabel2.Text = "Must be at least " + UserClass.UserDataCount + " characters";

		}

        #endregion
	}
}