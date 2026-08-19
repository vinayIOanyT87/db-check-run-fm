// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserClass.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Runtime.Serialization;
	using System.Security;

	using Crypt;

	using FMBusinessObjects.Constants;

	[Serializable]
	[CollectionDataContract]
	public class UserCollectionClass : List<UserClass>
	{
		#region Public Properties

		[DataMember]
		public Guid SiteGuid { get; set; }

		#endregion
	}

	/// <summary>
	///     Data object representing FuelsManager users
	/// </summary>
	[Serializable]
	[DataContract]
	[DebuggerDisplay("(UserClass IdentityGuid={IdentityGuid}, ID='{ID}', Name='{Name}')")]
	[SecuritySafeCritical]
	[KnownType(typeof(GroupCollectionClass))]
	public class UserClass : FMBaseDataObjectWithUserData, IAlarmAndEventDiscovery
	{
		#region Constants and Fields

		public static AlarmAndEventDescriptorClass LoggedInEventDescriptor = new AlarmAndEventDescriptorClass(
			false,
			SystemKey, UserLoggedInKey);

		public static AlarmAndEventDescriptorClass LoggedOutEventDescriptor = new AlarmAndEventDescriptorClass(
			false,
			SystemKey,
			UserLoggedOutKey);

		public static AlarmAndEventDescriptorClass LoginFailureEventDescriptor = new AlarmAndEventDescriptorClass(
			false,
			SystemKey,
			UserLoginFailureKey);

		public static AlarmAndEventDescriptorClass LoginNoGroupEventDescriptor = new AlarmAndEventDescriptorClass(
			false,
			SystemKey,
			UserLoginFailureNoGroupKey);

		[DataMember]
		public UserGroupMapCollectionClass UserGroupMapCollection;

		[DataMember]
		protected bool _ChangePassword;

		[DataMember]
		protected string _EmailAddress;

		[DataMember]
		protected string _PhoneNumber;

		[DataMember]
		protected DateTime _AccountExpirationDate;

        [DataMember]
	    protected bool activeDirectoryUser;

		[DataMember]
		protected DateTimeOffset _LastLoginDate;

		[DataMember]
		protected DateTimeOffset _LastLogoffDate;

		[DataMember]
		protected string _Name;

		[DataMember]
		protected string _Password;

		[DataMember]
		protected DateTimeOffset _PasswordTimestamp;

	    

		[DataMember]
		protected bool inactivityLockout = false;

		[DataMember]
		protected string passwordHint = "";

		[DataMember]
		protected string passwordHistory1 = "";

		[DataMember]
		protected string passwordHistory10 = "";

		[DataMember]
		protected string passwordHistory11 = "";

		[DataMember]
		protected string passwordHistory12 = "";

		[DataMember]
		protected string passwordHistory13 = "";

		[DataMember]
		protected string passwordHistory14 = "";

		[DataMember]
		protected string passwordHistory15 = "";

		[DataMember]
		protected string passwordHistory16 = "";

		[DataMember]
		protected string passwordHistory17 = "";

		[DataMember]
		protected string passwordHistory18 = "";

		[DataMember]
		protected string passwordHistory19 = "";

		[DataMember]
		protected string passwordHistory2 = "";

		[DataMember]
		protected string passwordHistory20 = "";

		[DataMember]
		protected string passwordHistory21 = "";

		[DataMember]
		protected string passwordHistory22 = "";

		[DataMember]
		protected string passwordHistory23 = "";

		[DataMember]
		protected string passwordHistory24 = "";

		[DataMember]
		protected string passwordHistory3 = "";

		[DataMember]
		protected string passwordHistory4 = "";

		[DataMember]
		protected string passwordHistory5 = "";

		[DataMember]
		protected string passwordHistory6 = "";

		[DataMember]
		protected string passwordHistory7 = "";

		[DataMember]
		protected string passwordHistory8 = "";

		[DataMember]
		protected string passwordHistory9 = "";

		[DataMember]
		protected int passwordLockoutCount = 0;

		private const string UserLoginCategory = "UserLogin";

		private static readonly byte[] DummyData = (new Guid("4BE74006-F456-4399-86C5-03613D7FB234")).ToByteArray();

		private static readonly byte[] Seed = (new Guid("1488AE9C-6813-49AE-AF08-155A53D99CE6")).ToByteArray();

		private static readonly AESCrypt encryptor = new AESCrypt();

		private const string UserLoggedInKey = "User Logged In";

		private const string UserLoggedOutKey = "User Logged Out";

		private const string UserLoginFailureKey = "User Login Failed. ";

		private const string UserLoginFailureNoGroupKey = "User has no Group Membership.";

		public const int UserDataCount = 8;
		public const int MIN_PASSWORD_DIFF = 8;  //The number represents the length required and the number of differences from new to previous passwords.

		#endregion

		#region Constructors and Destructors

		public UserClass()
		{
			this.Reset();
		}

		#endregion

		#region Public Properties

		public bool ChangePassword
		{
			get
			{
				return this._ChangePassword;
			}
			set
			{
				this._ChangePassword = value;
			}
		}

		public string EmailAddress
		{
			get
			{
				return this._EmailAddress;
			}
			set
			{
				this.SetString("E-mail Address", 50, value, ref this._EmailAddress);
			}
		}

		public string PhoneNumber
		{
			get
			{
				return this._PhoneNumber;
			}
			set
			{
				this.SetString("Phone Number", 20, value, ref this._PhoneNumber);
			}
		}

		public DateTime AccountExpirationDate
		{
			get
			{
				return this._AccountExpirationDate;
			}
			set
			{
				this._AccountExpirationDate = value;
			}
		}

	    public bool ActiveDirectoryUser
	    {
	        get { return this.activeDirectoryUser; }
	        set { this.activeDirectoryUser = value; }
	    }

	    public string ActiveDirectoryUserStr
	    {
	        get
	        {
	            if (activeDirectoryUser)
	            {
	                return "Yes";
	            }

	            return "No";
	        }
	    }

		public override ENTITY_TYPE EntityType
		{
			get
			{
				return ENTITY_TYPE.USER;
			}
		}

		public override string ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				this.SetString("ID", 100, value, ref this._ID);
			}
		}

		public bool InactivityLockout
		{
			get
			{
				return this.inactivityLockout;
			}
			set
			{
				// The system admin is not supposed to be locked out ever.
				if (this.IdentityGuid != Guids.UserAdminGuid)
				{
					this.inactivityLockout = value;
				}
			}
		}

		public bool IsAdministrator
		{
			get
			{
				return IsAdministratorGuid(this.IdentityGuid);
			}
		}

		public DateTimeOffset LastLoginDate
		{
			get
			{
				return this._LastLoginDate;
			}
			set
			{
				this._LastLoginDate = value;
			}
		}

		public DateTimeOffset LastLogoffDate
		{
			get
			{
				return this._LastLogoffDate;
			}
			set
			{
				this._LastLogoffDate = value;
			}
		}

		public AlarmAndEventLogClass LoggedInEvent
		{
			get
			{
				var AlarmAndEventLog = new AlarmAndEventLogClass(LoggedInEventDescriptor);
				AlarmAndEventLog.AssociatedData = this.ID;
				AlarmAndEventLog.CategoryID = UserLoginCategory;
				return AlarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass LoggedOutEvent
		{
			get
			{
				var AlarmAndEventLog = new AlarmAndEventLogClass(LoggedOutEventDescriptor);
				AlarmAndEventLog.AssociatedData = this.ID;
				AlarmAndEventLog.CategoryID = UserLoginCategory;
				return AlarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass LoginFailedEvent
		{
			get
			{
				var AlarmAndEventLog = new AlarmAndEventLogClass(LoginFailureEventDescriptor);
				AlarmAndEventLog.AssociatedData = "UserID: " + this.ID + ", Site ID:" + this.SiteID;
				return AlarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass LoginFailedNoGroupEvent
		{
			get
			{
				var AlarmAndEventLog = new AlarmAndEventLogClass(LoginNoGroupEventDescriptor);
				AlarmAndEventLog.AssociatedData = "UserID: " + this.ID + ", Site ID:" + this.SiteID;
				return AlarmAndEventLog;
			}
		}

		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				this.SetString("Name", 50, value, ref this._Name);
			}
		}

		public override ENTITY_TYPE ParentEntityType
		{
			get
			{
				return ENTITY_TYPE.NONE;
			}
		}

		public string Password
		{
			get
			{
				return this._Password;
			}
			set
			{
				this._Password = value;
			}
		}

		public string PasswordHint
		{
			get
			{
				return this.passwordHint;
			}
			set
			{
				this.passwordHint = value;
			}
		}

		public string PasswordHistory1
		{
			get
			{
				return this.passwordHistory1;
			}
			set
			{
				this.passwordHistory1 = value;
			}
		}

		public string PasswordHistory10
		{
			get
			{
				return this.passwordHistory10;
			}
			set
			{
				this.passwordHistory10 = value;
			}
		}

		public string PasswordHistory11
		{
			get
			{
				return this.passwordHistory11;
			}
			set
			{
				this.passwordHistory11 = value;
			}
		}

		public string PasswordHistory12
		{
			get
			{
				return this.passwordHistory12;
			}
			set
			{
				this.passwordHistory12 = value;
			}
		}

		public string PasswordHistory13
		{
			get
			{
				return this.passwordHistory13;
			}
			set
			{
				this.passwordHistory13 = value;
			}
		}

		public string PasswordHistory14
		{
			get
			{
				return this.passwordHistory14;
			}
			set
			{
				this.passwordHistory14 = value;
			}
		}

		public string PasswordHistory15
		{
			get
			{
				return this.passwordHistory15;
			}
			set
			{
				this.passwordHistory15 = value;
			}
		}

		public string PasswordHistory16
		{
			get
			{
				return this.passwordHistory16;
			}
			set
			{
				this.passwordHistory16 = value;
			}
		}

		public string PasswordHistory17
		{
			get
			{
				return this.passwordHistory17;
			}
			set
			{
				this.passwordHistory17 = value;
			}
		}

		public string PasswordHistory18
		{
			get
			{
				return this.passwordHistory18;
			}
			set
			{
				this.passwordHistory18 = value;
			}
		}

		public string PasswordHistory19
		{
			get
			{
				return this.passwordHistory19;
			}
			set
			{
				this.passwordHistory19 = value;
			}
		}

		public string PasswordHistory2
		{
			get
			{
				return this.passwordHistory2;
			}
			set
			{
				this.passwordHistory2 = value;
			}
		}

		public string PasswordHistory20
		{
			get
			{
				return this.passwordHistory20;
			}
			set
			{
				this.passwordHistory20 = value;
			}
		}

		public string PasswordHistory21
		{
			get
			{
				return this.passwordHistory21;
			}
			set
			{
				this.passwordHistory21 = value;
			}
		}

		public string PasswordHistory22
		{
			get
			{
				return this.passwordHistory22;
			}
			set
			{
				this.passwordHistory22 = value;
			}
		}

		public string PasswordHistory23
		{
			get
			{
				return this.passwordHistory23;
			}
			set
			{
				this.passwordHistory23 = value;
			}
		}

		public string PasswordHistory24
		{
			get
			{
				return this.passwordHistory24;
			}
			set
			{
				this.passwordHistory24 = value;
			}
		}

		public string PasswordHistory3
		{
			get
			{
				return this.passwordHistory3;
			}
			set
			{
				this.passwordHistory3 = value;
			}
		}

		public string PasswordHistory4
		{
			get
			{
				return this.passwordHistory4;
			}
			set
			{
				this.passwordHistory4 = value;
			}
		}

		public string PasswordHistory5
		{
			get
			{
				return this.passwordHistory5;
			}
			set
			{
				this.passwordHistory5 = value;
			}
		}

		public string PasswordHistory6
		{
			get
			{
				return this.passwordHistory6;
			}
			set
			{
				this.passwordHistory6 = value;
			}
		}

		public string PasswordHistory7
		{
			get
			{
				return this.passwordHistory7;
			}
			set
			{
				this.passwordHistory7 = value;
			}
		}

		public string PasswordHistory8
		{
			get
			{
				return this.passwordHistory8;
			}
			set
			{
				this.passwordHistory8 = value;
			}
		}

		public string PasswordHistory9
		{
			get
			{
				return this.passwordHistory9;
			}
			set
			{
				this.passwordHistory9 = value;
			}
		}

		public int PasswordLockoutCount
		{
			get
			{
				return this.passwordLockoutCount;
			}
			set
			{
				this.passwordLockoutCount = value;
			}
		}

		public DateTimeOffset PasswordTimestamp
		{
			get
			{
				return this._PasswordTimestamp;
			}
			set
			{
				this._PasswordTimestamp = value;
			}
		}

		public string UserData1
		{
			get
			{
				return UserData[0];
			}
			set
			{
				UserData[0] = value;
			}
		}

		public string UserData2
		{
			get
			{
				return UserData[1];
			}
			set
			{
				UserData[1] = value;
			}
		}

		public string UserData3
		{
			get
			{
				return UserData[2];
			}
			set
			{
				UserData[2] = value;
			}
		}

		public string UserData4
		{
			get
			{
				return UserData[3];
			}
			set
			{
				UserData[3] = value;
			}
		}

		public string UserData5
		{
			get
			{
				return UserData[4];
			}
			set
			{
				UserData[4] = value;
			}
		}

		public string UserData6
		{
			get
			{
				return UserData[5];
			}
			set
			{
				UserData[5] = value;
			}
		}

		public string UserData7
		{
			get
			{
				return UserData[6];
			}
			set
			{
				UserData[6] = value;
			}
		}

		public string UserData8
		{
			get
			{
				return UserData[7];
			}
			set
			{
				UserData[7] = value;
			}
		}

		#endregion

		#region Explicit Interface Properties

		AlarmAndEventDescriptorClass[] IAlarmAndEventDiscovery.AlarmAndEvents
		{
			get
			{
				AlarmAndEventDescriptorClass[] Descriptors =
				{
					LoggedInEventDescriptor, LoggedOutEventDescriptor,
					LoginFailureEventDescriptor
				};
				return Descriptors;
			}
		}

		#endregion

		#region Public Methods and Operators

		public static bool IsAdministratorGuid(Guid userGuid)
		{
			return userGuid == Guids.UserAdminGuid;
		}

		public static string decode(byte[] encodedData, Guid siteGuid)
		{
			using (AESKey key = GetKey(siteGuid))
			{
				return encryptor.DecryptToText(encodedData, key);
			}
		}

		//Eric Simmons (1-6-2010)
		//Updated access level of function to public
		public static byte[] encode(string plaintextData, Guid siteGuid)
		{
			using (AESKey key = GetKey(siteGuid))
			{
				return encryptor.Encrypt(plaintextData, key);
			}
		}

		/// <summary>
		///     This method resets the object to its initial state.
		/// </summary>
		public override void Reset()
		{
			base.Reset();
			this._Password = string.Empty;
			this._LastLoginDate = DateTimeOffset.Now;
			this._LastLogoffDate = DateTimeOffset.Now;
			this._ChangePassword = false;
			this._PasswordTimestamp = DateTimeOffset.Now;
			this._Name = string.Empty;
			this._EmailAddress = string.Empty;
			this._PhoneNumber = string.Empty;
			this._AccountExpirationDate = DateTime.Today.AddYears(1).Date;
			this.UserGroupMapCollection = new UserGroupMapCollectionClass();
			this.passwordHistory1 = string.Empty;
			this.passwordHistory2 = string.Empty;
			this.passwordHistory3 = string.Empty;
			this.passwordHistory4 = string.Empty;
			this.passwordHistory5 = string.Empty;
			this.passwordHistory6 = string.Empty;
			this.passwordHistory7 = string.Empty;
			this.passwordHistory8 = string.Empty;
			this.passwordHistory9 = string.Empty;
			this.passwordHistory10 = string.Empty;
			this.passwordHistory11 = string.Empty;
			this.passwordHistory12 = string.Empty;
			this.passwordHistory13 = string.Empty;
			this.passwordHistory14 = string.Empty;
			this.passwordHistory15 = string.Empty;
			this.passwordHistory16 = string.Empty;
			this.passwordHistory17 = string.Empty;
			this.passwordHistory18 = string.Empty;
			this.passwordHistory19 = string.Empty;
			this.passwordHistory20 = string.Empty;
			this.passwordHistory21 = string.Empty;
			this.passwordHistory22 = string.Empty;
			this.passwordHistory23 = string.Empty;
			this.passwordHistory24 = string.Empty;
			this.passwordLockoutCount = 0;
			this.inactivityLockout = false;
		    this.activeDirectoryUser = false;
			this.passwordHint = string.Empty;

			UserData = new UserDataClass();
		}

		#endregion

		#region Methods

		private static AESKey GetKey(Guid siteGuid)
		{
			var newSeed = new byte[Seed.Length + DummyData.Length];
			Buffer.BlockCopy(Seed, 0, newSeed, 0, Seed.Length);
			Buffer.BlockCopy(DummyData, 0, newSeed, Seed.Length, DummyData.Length);
			return new AESKey(newSeed, siteGuid.ToByteArray());
		}

		#endregion
	}
}
