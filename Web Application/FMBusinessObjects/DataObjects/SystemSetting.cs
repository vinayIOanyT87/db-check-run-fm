namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Runtime.Serialization;
	using FMBusinessObjects.Constants;

	[Serializable]
    [DataContract]
	public class SystemSettingClass : BaseDataObject
	{
		#region Public constants data member
		/// <summary>
		/// The default text to display in the password text box on the system settings page.
		/// This value is checked when modifying a system settings record to see if the password was
		/// actually changed by the user
		/// </summary>
		public const string MaskedPasswordText = "**********";

		public const string ProhibitUpdatingLinkedEquipmentText = "Prohibit Deletion/ID Change of Linked Equipment";
		public const string UserDataListDefaultToFirstValueText = "List type User Data defaults to the first value on the list";
		#endregion

		#region Private data members
		[DataMember] private string reportServerUrl;
		[DataMember] private int stationMessageTimeout;
		[DataMember] private int stationPromptTimeout;
        [DataMember] private string reportServerPassword;
        [DataMember] private string reportServerUserName;
		[DataMember] private bool prohibitUpdatingLinkedEquipment;
		[DataMember] private bool userDataListDefaultToFirstValue;
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public SystemSettingClass()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public string ReportServerUrl
		{
			get { return this.reportServerUrl; }
			set { SetString("Report Server URL ", 80, value, ref this.reportServerUrl); }
		}

        public string ReportServerUserName
        {
            get { return this.reportServerUserName; }
            set { SetString("Report Server User Name ", 80, value, ref this.reportServerUserName); }
        }

        public string ReportServerPassword
        {
            get { return this.reportServerPassword; }
            set { SetString("Report Server Password ", 80, value, ref this.reportServerPassword); }
        }

		public int StationMessageTimeout
		{
			get { return this.stationMessageTimeout; }
			set { this.stationMessageTimeout = value; }
		}

		public int StationPromptTimeout
		{
			get { return this.stationPromptTimeout; }
			set { this.stationPromptTimeout = value; }
		}

		public bool ProhibitUpdatingLinkedEquipment
		{
			get { return this.prohibitUpdatingLinkedEquipment; }
			set { this.prohibitUpdatingLinkedEquipment = value; }
		}

		public bool UserDataListDefaultToFirstValue
		{
			get { return this.userDataListDefaultToFirstValue; }
			set { this.userDataListDefaultToFirstValue = value; }
        }

        public override ENTITY_TYPE EntityType
		{
			get { return ENTITY_TYPE.SYSTEM_SETTING; }
		}

		public override ENTITY_TYPE ParentEntityType
		{
			get { return ENTITY_TYPE.NONE; }
		}
		#endregion

		#region Public methods
		public override void Reset()
		{
			this.Init();
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method sets the object to its default values.
		/// </summary>
		private void Init()
		{
			base.Reset();

			this._IdentityGuid						= Guids.SystemSettingsGuid;
			this.reportServerUrl					= "http://localhost/ReportServer";
			this.stationMessageTimeout				= 2;
			this.stationPromptTimeout				= 60;
			this.prohibitUpdatingLinkedEquipment	= false;
			this.userDataListDefaultToFirstValue	= false;
		}
		#endregion
	}
}
