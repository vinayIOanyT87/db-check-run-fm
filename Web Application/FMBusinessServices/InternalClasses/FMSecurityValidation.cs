// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMSecurityValidation.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FMSecurityValidation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.InternalClasses
{
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Class for validating passwords according to the current configuration settings.
	/// </summary>
	public class FMSecurityValidation : FMSecurityValidationBase
	{
		#region Constants and Fields

		/// <summary>
		/// Site context for validation
		/// </summary>
		private SiteClass site;

		/// <summary>
		/// The user to validate.
		/// </summary>
		private UserClass user;

		#endregion

		#region Constructors and Destructors
		/// <summary>
		/// Initializes a new instance of the <see cref="FMSecurityValidation"/> class. 
		/// This is the default constructor for the FM Security Validation class;
		/// </summary>
		public FMSecurityValidation()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FMSecurityValidation"/> class. 
		/// This constructor for the FM Security Validation allows for passing an instance of a UserClass and SiteClass.
		/// </summary>
		/// <param name="inUser">
		/// The in User.
		/// </param>
		/// <param name="inSite">
		/// The in Site.
		/// </param>
		public FMSecurityValidation(UserClass inUser, SiteClass inSite)
		{
			this.User = inUser;
			this.Site = inSite;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the Site Class object. This site object must be the site that contains the Password configuration.
		/// </summary>
		public SiteClass Site
		{
			get
			{
				return this.site;
			}

			set
			{
				this.site = value ?? new SiteClass();
			}
		}

		/// <summary>
		/// Gets or sets the User Class object.
		/// </summary>
		public UserClass User
		{
			get
			{
				return this.user;
			}

			set
			{
				this.user = value ?? new UserClass();
			}
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Checks the number of tries for password guess.
		/// </summary>
		/// <param name="numberOfTries">The number of tries.</param>
		/// <returns>This method will return true if the user is locked out due to the number of password entry tries.</returns>
		public bool CheckNumberOfTries(int numberOfTries)
		{
			return base.LockedOut(numberOfTries);
		}

		/// <summary>
		/// This method will return true if the user is locked out.
		/// </summary>
		/// <param name="numberOfTries">The number of tries attempted in a row.</param>
		/// <returns>Returns if the user is locked out.</returns>
		public override bool LockedOut(int numberOfTries)
		{
			// The Administrator account will not be locked out by design.  So to deter hackers trying to 
			// guess the password, we misdirect them by saying the user is actually locked out.
			if ((this.user.IdentityGuid == Guids.UserAdminGuid) && (this.site.SiteID == "SiteAdmin"))
			{
				return true;
			}

			if (this.user.InactivityLockout)
			{
				return true;
			}

			this.user.InactivityLockout = base.LockedOut(numberOfTries);
			return this.user.InactivityLockout;
		}

		/// <summary>
		/// Parses the configuration information for use in validation.
		/// </summary>
		public override void ParseConfiguration()
		{
			this.minTimeAllowedToChange = this.site.MinTimeAllowedToChangePassword;
			this.minCharacterLength = this.site.MinPasswordCharacterLength;
			this.expirationInDays = this.site.PasswordExpirationInDays;
			this.lockoutThreshold = this.site.PasswordLockoutThreshold;
			this.checkForPreviousPwds = this.site.CheckForPreviousPassword;
			this.StrongPwdUse = this.site.StrongPasswordUse;
			this.pwdHistoryCount = this.site.PasswordHistoryCount;
			this.inactivityPeriod = this.site.InactivityDisablePeriod;

			// Set the number of previous passwords in the base array.
			if (this.site.PasswordHistoryCount > 0)
			{
				this.pwdList.Add(this.user.PasswordHistory1);
				this.pwdList.Add(this.user.PasswordHistory2);
				this.pwdList.Add(this.user.PasswordHistory3);
				this.pwdList.Add(this.user.PasswordHistory4);
				this.pwdList.Add(this.user.PasswordHistory5);
				this.pwdList.Add(this.user.PasswordHistory6);
				this.pwdList.Add(this.user.PasswordHistory7);
				this.pwdList.Add(this.user.PasswordHistory8);
				this.pwdList.Add(this.user.PasswordHistory9);
				this.pwdList.Add(this.user.PasswordHistory10);
				this.pwdList.Add(this.user.PasswordHistory11);
				this.pwdList.Add(this.user.PasswordHistory12);
				this.pwdList.Add(this.user.PasswordHistory13);
				this.pwdList.Add(this.user.PasswordHistory14);
				this.pwdList.Add(this.user.PasswordHistory15);
				this.pwdList.Add(this.user.PasswordHistory16);
				this.pwdList.Add(this.user.PasswordHistory17);
				this.pwdList.Add(this.user.PasswordHistory18);
				this.pwdList.Add(this.user.PasswordHistory19);
				this.pwdList.Add(this.user.PasswordHistory20);
				this.pwdList.Add(this.user.PasswordHistory21);
				this.pwdList.Add(this.user.PasswordHistory22);
				this.pwdList.Add(this.user.PasswordHistory23);
				this.pwdList.Add(this.user.PasswordHistory24);
			}
		}

		/// <summary>
		/// This method parses the User information to retrieve all the Password information related to the user. It sets data members for future validation. This method is used only for FuelsManager 7.1. A new parse method would be created for other applications. If the User Class is null, then the parse method defaults the settings.
		/// </summary>
		public override void ParseUserInfo()
		{
			this.lastPasswordChangeTimestamp = this.user.PasswordTimestamp;
			this.lastLoginDate = this.user.LastLoginDate;
			this.currentPassword = this.user.Password;
			this.userID = this.user.ID;
			this.inactivityLockout = this.user.InactivityLockout;
		}

		#endregion
	}
}