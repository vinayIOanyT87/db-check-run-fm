// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMSecurityValidationBase.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FMSecurityValidationBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.InternalClasses
{
	using System;
	using System.Collections;
	using System.Text;
	using System.Text.RegularExpressions;
	using System.Web.Security;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	public abstract class FMSecurityValidationBase
	{
		#region Constants and Fields

		protected int StrongPwdUse;

		protected bool checkForPreviousPwds;

		protected string currentPassword;

		protected int daysUntilExpiration;

		protected int expirationInDays;

		protected bool inactivityLockout;

		protected int inactivityPeriod;

		protected DateTimeOffset lastLoginDate;

		protected DateTimeOffset lastPasswordChangeTimestamp;

		protected int lockoutThreshold;

		protected int minCharacterLength;

		protected int minTimeAllowedToChange;

		protected int pwdHistoryCount;

		protected ArrayList pwdList;

		protected string userID;

		protected bool userLockedOut;

		private const int MIN_PASSWORD_DIFF = 8;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="FMSecurityValidationBase"/> class. 
		///   This is the default constructor for the FM Security Validation base class;
		/// </summary>
		protected FMSecurityValidationBase()
		{
			this.Initialize();
		}

		#endregion

		#region Public Properties

		public StrongPasswordUsage StrongPassword
		{
			get
			{
				return (StrongPasswordUsage)this.StrongPwdUse;
			}
		}

		/// <summary>
		/// Gets the number of days until the Password expires.
		/// </summary>
		public int DaysUntilExpiration
		{
			get
			{
				return this.daysUntilExpiration;
			}
		}

		/// <summary>
		/// Gets the minimum number of characters.
		/// </summary>
		public int MinimumCharacterLength
		{
			get
			{
				return this.minCharacterLength;
			}
		}

		/// <summary>
		/// Gets the minimum time allowed to change Password.
		/// </summary>
		public int MinimumTimeAllowedToChangePassword
		{
			get
			{
				return this.minTimeAllowedToChange;
			}
		}

        /// <summary>
        /// Gets minimum number of password differences allowed
        /// Miriam 11.12.18 
        /// </summary>
        public int MinimumPasswordDiff
        {
            get
            {
                return MIN_PASSWORD_DIFF;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///   This method will return true if the current Password timestamp exceeds the number of days allowed for a Password to exist before forcing a change. Otherwise, it returns false.
        /// </summary>
        /// <returns> true: The password has expired false: The password has not yet expired </returns>
        /// <remarks>
        ///   As a side effect, DaysUntilExpiration is set to the number of days left before the password expires. A maximum password age of 0 means that passwords never expire. DaysUntilExpiration is set to 999.
        /// </remarks>
        public bool ExceededPasswordAge()
		{
			bool result = false;

			// If expiration is set to be zero days, that means that passwords
			// should not expire.
			if (this.expirationInDays == 0)
			{
				this.daysUntilExpiration = 999;
				return false;
			}

			// Remove the time from the date/time in order to determine days only.
			DateTimeOffset newPwdChangeDate = TimeConverter.ToDate(this.lastPasswordChangeTimestamp);

			// Add the expiration number in order to compare to the current date.
			newPwdChangeDate = newPwdChangeDate.AddDays(this.expirationInDays);

			// Get the current date without the time.
			DateTimeOffset currentDate = TimeConverter.Today();

			// Determine the days until expiration.
			TimeSpan expirationTimeSpan = newPwdChangeDate - currentDate;
			this.daysUntilExpiration = expirationTimeSpan.Days;

			if (this.daysUntilExpiration <= 0)
			{
				result = true;
			}

			return result;
		}

		/// <summary>
		/// This method will return true if the current password timestamp exceeds the number of inactive days allowed before the user is locked out. Otherwise, it returns false.
		/// </summary>
		/// <returns>
		/// The System.Boolean.
		/// </returns>
		public bool InactivityInExcess()
		{
			bool result = false;

			// If the period is set to 0, let it be unlimited
			if (this.inactivityPeriod == 0)
			{
				return false;
			}

			DateTimeOffset lastLoginSpan = this.lastLoginDate.AddDays(this.inactivityPeriod);

			if (this.userID != "Administrator" && (this.inactivityLockout || lastLoginSpan < DateTimeOffset.Now))
			{
				result = true;
			}

			return result;
		}

		/// <summary>
		/// This method will return true if the user is locked out.
		/// </summary>
		/// <param name="numberOfTries">
		/// The number of tries so far.
		/// </param>
		/// <returns>
		/// The System.Boolean.
		/// </returns>
		public virtual bool LockedOut(int numberOfTries)
		{
			if ((this.lockoutThreshold > 0) && (numberOfTries >= this.lockoutThreshold))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// This method returns true if the Password meets the strong Password criterion. If it does not, then false is returned. If the strong Password flag in the configuration settings is set to false, meaning do not perform the check, then true is returned.
		/// </summary>
		/// <param name="pwd">
		/// The password to evaluate.
		/// </param>
		/// <returns>
		/// The System.Boolean.
		/// </returns>
		public bool MeetsStrongPassword(string pwd)
		{
			bool result = true;

			if (this.StrongPwdUse == (int)StrongPasswordUsage.Strong)
			{
				result = FMCore.FuelsManagerExtensions.IsStrongPassword(pwd);
			}
			else if (this.StrongPwdUse == (int)StrongPasswordUsage.Enhanced)
			{
				if (string.IsNullOrEmpty(pwd) || (pwd.Length <= 0))
				{
					result = false;
				}
				else
				{
					result = FMCore.FuelsManagerExtensions.IsEnhancedStrongPassword(pwd);
				}
			}
				return result;
		}

		/// <summary>
		/// This method returns true if the Password has the minimum number of characters. Otherwise, it returns false.
		/// </summary>
		/// <param name="pwd">
		/// The pwd.
		/// </param>
		/// <returns>
		/// The System.Boolean.
		/// </returns>
		public bool MinimumOfCharacters(string pwd)
		{
			bool result = !(string.IsNullOrEmpty(pwd) || (pwd.Length < this.minCharacterLength));
			return result;
		}

		/// <summary>
		/// This method returns true if the minimum number of days to change a Password is less than the number of days since the last Password change occurred. Otherwise, it returns false.
		/// </summary>
		/// <returns>
		/// The System.Boolean.
		/// </returns>
		public bool MinimumTimeAllowedToChange()
		{
			bool result = true;

			if (this.MinimumTimeAllowedToChangePassword > 0)
			{
				if (DateTimeOffset.Now < this.lastPasswordChangeTimestamp.AddDays(this.minTimeAllowedToChange))
				{
					result = false;
				}
			}

			return result;
		}

		/// <summary>
		/// Parses the configuration.
		/// </summary>
		public abstract void ParseConfiguration();

		/// <summary>
		///   These abstract methods must be implemented by the derived classes. Each application has a different User Info and Configuration objects. These methods parse the specific data objects and setup the validation data members.
		/// </summary>
		public abstract void ParseUserInfo();

		/// <summary>
		/// This method will return true if the new Password matches any of the previous used passwords. Otherwise, it return false. If the flag to check previous used passwords is not set (false) to check, then true is returned.
		/// </summary>
		/// <param name="pwd">
		/// The new password to check for previous existence.
		/// </param>
		/// <param name="oldPassword">
		/// The old Password.
		/// </param>
		/// <param name="checkAlmostMatch">
		/// The check Almost Match.
		/// </param>
		/// <returns>
		/// The System.Boolean.
		/// </returns>
		public bool PreviouslyExisted(string pwd, string oldPassword, bool checkAlmostMatch)
		{
			bool result = false;

			if (this.checkForPreviousPwds)
			{
				if ((!string.IsNullOrEmpty(pwd)) && (!string.IsNullOrEmpty(this.userID)))
				{
					if (this.ComparePasswords(oldPassword, pwd, checkAlmostMatch))
					{
						result = true;
					}
					else
					{
						for (int nextPwd = 0; nextPwd < this.pwdHistoryCount; nextPwd++)
						{
							var previousPwd = (string)this.pwdList[nextPwd];

							if (this.ComparePasswords(pwd, previousPwd, checkAlmostMatch))
							{
								result = true;
								break;
							}
						}
					}
				}
			}

			return result;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Compares the passwords.
		/// </summary>
		/// <param name="pwd">The PWD.</param>
		/// <param name="previousPwd">The previous PWD.</param>
		/// <param name="checkAlmostMatch">if set to <c>true</c> [check almost match].</param>
		/// <returns>True if passwords match.</returns>
		private bool ComparePasswords(string pwd, string previousPwd, bool checkAlmostMatch)
		{
			bool result = false;

			if (!string.IsNullOrEmpty(previousPwd))
			{
				if (checkAlmostMatch)
				{
					if (this.IsClose(pwd, previousPwd))
					{
						result = true;
					}
				}
				else
				{
					if (pwd.Equals(previousPwd))
					{
						result = true;
					}
				}
			}

			return result;
		}

		/// <summary>
		///   This method initialize the FM Security Validation object to its initial state.
		/// </summary>
		private void Initialize()
		{
			this.lastPasswordChangeTimestamp = DateTimeOffset.Now;
			this.lastLoginDate = DateTimeOffset.Now;
			this.minTimeAllowedToChange = 0;    // Units in days
			this.minCharacterLength = UserClass.UserDataCount;  // Number of characters
			this.currentPassword = string.Empty;
			this.userID = string.Empty;
			this.expirationInDays = 999;        // Units in days
			this.daysUntilExpiration = this.expirationInDays;
			this.lockoutThreshold = 0;          // Number of Password tries before locking out.
			this.userLockedOut = false;         // User is locked out if true.
			this.StrongPwdUse = (int)StrongPasswordUsage.None;
			this.checkForPreviousPwds = false;
			this.pwdList = new ArrayList();
			this.pwdHistoryCount = 0;
			this.inactivityLockout = false;
		}

		/// <summary>
		/// Determines whether the specified password one is close.
		/// </summary>
		/// <param name="passwordOne">The password one.</param>
		/// <param name="passwordTwo">The password two.</param>
		/// <returns>
		///   <c>true</c> if the specified password one is close to password two; otherwise, <c>false</c>.
		/// </returns>
		private bool IsClose(string passwordOne, string passwordTwo)
		{
			int rotation;

			// Check that there are sufficient differences
			// First pad with spaces to get to same length
			if (passwordTwo.Length < passwordOne.Length)
			{
				passwordTwo = passwordTwo.PadRight(passwordOne.Length, ' ');
			}
			else
			{
				passwordOne = passwordOne.PadRight(passwordTwo.Length, ' ');
			}

			int newPwdSize = passwordTwo.Length;

			for (rotation = 0; rotation < newPwdSize; rotation++)
			{
				int differences = 0;
				for (int index = 0; index < newPwdSize; index++)
				{
					if (passwordOne.Substring(index, 1) != passwordTwo.Substring((index + rotation) % newPwdSize, 1))
					{
						differences++;
					}
				}

				if (MIN_PASSWORD_DIFF > differences)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// This method will return a temporary password with the following criteria
		/// At least 8 characters long
		/// One symbol
		/// At least one uppercase character
		/// At least one number
		/// </summary>
		/// <returns></returns>
		public string GenerateTemporaryPassword(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			//	At least 8 characters long with at least one symbol minimum
			string retVal = Membership.GeneratePassword(8, 1);
			StringBuilder sb = new StringBuilder(retVal);
			Random rand = new Random();

			// Must contain at least one uppercase character, if none exist will overwrite 
			// the character in the third position of the string with an uppercase letter.
			Regex regex = new Regex(@"[A-Z]");
			
			// %%%%%%%%, 
			MatchCollection matches = regex.Matches(sb.ToString().Substring(0, 7));
			if (matches.Count < 1)
			{
				// Uppercase characters in ASCII table only
				int index = rand.Next(65, 90);
				sb[3] = Convert.ToChar(index);
			}

			// Must contain at least one number, if none exist will overwrite the last character in the 
			// string with a number.
			regex = new Regex(@"\d");
			matches = regex.Matches(sb.ToString());
			if (matches.Count < 1)
			{
				// Numbers in ASCII table
				int newNumber = rand.Next(48, 57);
				sb[7] = Convert.ToChar(newNumber);
			}

			retVal = sb.ToString();

			return retVal;
		}
		#endregion
	}
}
