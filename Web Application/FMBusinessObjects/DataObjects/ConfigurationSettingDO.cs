// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationSettingDO.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ConfigurationSettingDO type
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Data.SqlClient;
	using System.Runtime.Serialization;
	using System.Text;

	using FMBusinessObjects.Constants;
	using FMBusinessObjects.UtilityObjects;

    #region Configuration Setting Data Object Collection Class
	[Serializable]
	[CollectionDataContract]
	[KnownType(typeof(ConfigurationSettingDOClass))]
	public class ConfigurationSettingDOCollectionClass : CollectionBase
	{
		public void Add ( ConfigurationSettingDOClass configSetting )
		{
			List.Add ( configSetting );
		}

		public void Remove ( int index )
		{
			if (index > Count - 1 || index < 0)
			{
				throw ( new Exception ( "Invalid Index" ) );
			}
			else
			{
				List.RemoveAt ( index );
			}
		}

		public void Remove ( ConfigurationSettingDOClass configSetting )
		{
			int index = 0;

			foreach (ConfigurationSettingDOClass Item in List)
			{
				if (Item.ConfigurationSettingGuid == configSetting.ConfigurationSettingGuid)
				{
					List.RemoveAt ( index );
					return;
				}

				index++;
			}
		}

		public ConfigurationSettingDOClass Item ( int Index )
		{
			return (ConfigurationSettingDOClass) List[Index];
		}
	}
	#endregion

	[DataContract]
	[Serializable]
	public class ConfigurationSettingDOClass : BaseLineItemDO
	{
		#region Public data members
		public const string Key_DataDictionaryAssemblies				= "DataDictionaryAssemblies";
		public const string Key_IDiscoveryAssemblies					= "IDiscoveryAssemblies";
		public const string Key_IDependencyAssemblies					= "IDependencyAssemblies";
		public const string Key_OnlineAdminDoc							= "OnlineAdminDoc";
		public const string Key_OnlineAdminTutorialDoc					= "OnlineAdminTutorialDoc";
		public const string Key_OnlineTutorialDoc						= "OnlineTutorialDoc";
		public const string Key_OnlineHelpDoc							= "OnlineHelpDoc";
		public const string Key_LoadRackInstalled						= "LoadRackInstalled";
		public const string Key_LoadRackPort							= "LoadRackPort";
		public const string Key_LR_QualityAssuranceInterface			= "LR_QualityAssuranceInterface";
		public const string Key_CustomClientScriptName					= "CustomClientScriptName";
		public const string Key_CustomTransactionFieldAssemblyPath		= "CustomTransactionFieldAssemblyPath";
		public const string Key_MaxConcurrentSessionsPerUser			= "MaxConcurrentSessionsPerUser";
        public const string Key_MaxConcurrentUsersPerServer				= "MaxConcurrentUsersPerServer";
        public const string Key_AccountingEnterpriseInterface			= "AccountingEnterpriseInterface";
		public const string Key_ISecurityAssemblies						= "ISecurityAssemblies";
		public const string Key_BKUtility_Certificate					= "BKUtility_Certificate";
		public const string Key_BKUtility_Ticks							= "BKUtility_Ticks";
		public const string Key_BKUtility_LogFilePath					= "BKUtility_LogFilePath";
		public const string Key_BKUtility_ZipFilePath					= "BKUtility_ZipFilePath";
		public const string Key_BKUtility_AdditionalFilesPaths			= "BKUtility_AdditionalFilesPaths";
		public const string Key_BKUtility_SQLDataRoot					= "BKUtility_SQLDataRoot";
		public const string Key_BKUtility_SQLTraceFolder				= "BKUtility_SQLTraceFolder";
		public const string Key_BKUtility_Project						= "BKUtility_Project";
		public const string Key_BKUtility_CurrDB						= "BKUtility_CurrDB";
		public const string Key_BKUtility_SyncTechSystemHome			= "BKUtility_SyncTechSystemHome";
		public const string Key_BKUtility_LogFileFullPath				= "BKUtility_LogFileFullPath";
		public const string Key_BKUtility_xPosition						= "BKUtility_xPosition";
		public const string Key_BKUtility_yPosition						= "BKUtility_yPosition";
		public const string Key_BKUtility_zxWidth						= "BKUtility_zxWidth";
		public const string Key_BKUtility_zyWidth						= "BKUtility_zyWidth";
		public const string Key_BKUtility_BUC							= "BKUtility_BUC";
		public const string Key_DISPATCH_PollTime						= "DISPATCH_PollTime";
		public const string Key_CAC_Enable								= "Common Access Card (CAC) Enable";
		public const string Key_ForceCloseoutButtonDisable				= "ForceCloseoutButtonDisable";
		public const string Key_ChangeQueueEnabled						= "ChangeQueueEnabled";
		public const string Key_AlertSessionLogoutEnabled				= "AlertSessionLogoutEnabled";
		public const string Key_FMService_ReindexEnabled				= "FMService_ReindexEnabled";
		public const string Key_FMService_ReindexScheduledTime			= "FMService_ReindexScheduledTime";
		public const string Key_SingleSignOnMode						= "SingleSignOnMode";
		public const string Key_ActiveDirectoryDomainName               = "ActiveDirectoryDomainName";
		public const string Key_ActiveDirectorySitesOrganizationalUnitPath = "ActiveDirectorySitesOrganizationalUnitPath";
		public const string Key_ActiveDirectoryUserGroupsOrganizationalUnitPath = "ActiveDirectoryUserGroupsOrganizationalUnitPath";
		public const string Key_UseNewTransactionAliasScreen = "UseNewTransactionAliasConfigScreen";
		public const string Key_ValidateDestinationEquipment = "ValidateDestinationEquipment";


		/// <summary>
		/// The key for the setting which describes the site group that the FMAE Translations page should be visible for.
		/// </summary>
		public const string Key_FMAETranslationsConfigurationSiteGroup = "FMAETranslationsConfigurationSiteGroup";

		public const string Key_InstallDetails_EnterpriseCertificateName = "InstallDetailsEnterpriseCertificateName";
		public const string Key_InstallDetails_ARTSCertificateName = "InstallDetailsARTSCertificateName";
		public const string Key_InstallDetails_DataExchangeServiceCertificateName = "InstallDetailsDataExchangeServiceCertificateName";

		public const string Key_LogSessionMemoryState = "LogSessionMemoryState";


		// IRS ExSTARS settings
		public const string Key_IrsExStars_ProductCodesRegEx					= "IrsProductCodesRegEx";
		public const string Key_IrsExStars_DunsNumber_ISA08						= "IrsExStarsDunsNumber_ISA08";
		public const string Key_IrsExStars_ApplicationReceiversCode_GS03		= "IrsExStarsApplicationReceiversCode_GS03";
		public const string Key_IrsExStars_FuncGrpHdrVerReleaseIndIdCode_GS08	= "IrsExStarsFuncGrpHdrVerReleaseIndustryIdCode_GS08";		
		public const string Key_IrsExStars_InterchangeControlVersion_ISA12		= "IrsExStarsInterchangeControlVersion_ISA12";
		public const string Key_IrsExStars_ISA14Value							= "IrsExStarsISA14Value";
		public const string Key_IrsExStars_ISA05Qualifier						= "IrsExStarsISA05Qualifier";
		public const string Key_IrsExStars_EnableDebugFeatures					= "IrsExStarsEnableDebugFeatures";

		// Cassandra Settings
		public const string Key_Cassandra_Configuration = "Cassandra_Configuration";
		public const string Key_Cassandra_ReplicationFactor = "Cassandra_ReplicationFactor";
		public const string Key_Cassandra_ConsistencyLevel = "Cassandra_ConsistencyLevel";
		public const string Key_Cassandra_Username = "Cassandra_Username";
		public const string Key_Cassandra_Password = "Cassandra_Password";


		// Common Sync Settings
		public const string Key_InstallDetails_SyncNodeGuid = "InstallDetailsSynchronizationNodeGuid";
		public const string Key_InstallDetails_SyncNodeName = "InstallDetailsSynchronizationNodeName";
		public const string Key_InstallDetails_SyncProfileID = "InstallDetailsSynchronizationProfileID";

		// External export results interface names
		public const string Key_External_ExportResults_InterfaceNames = "ExternalExportResultsInterfaceName";

		// FMExport Settings
		public const string Key_FMExport_ExcludeEmptyFiles = "FMExport_ExcludeEmptyFiles";
		public const string Key_FMExport_SqlConnectionRetryCount = "FMExport_SqlConnectionRetryCount";
		public const string Key_FMExport_SMTPServer = "FMExport_SMTPServer";

		public const string Key_IsEnterprise = "IsEnterprise";

		public const string Key_DefaultHelpURL = "DefaultHelpURL";

		public const string Key_WarnMinutesBeforeSessionExpire = "WarnMinutesBeforeSessionExpire";

		public const string Key_EnterpriseVisibilityOpcUaServerUrl = "EnterpriseVisibilityOpcUaServerUrl";
		public const string Key_EnterpriseVisibilitySecurityMode = "EnterpriseVisibilitySecurityMode";
		public const string Key_EnterpriseVisibilitySecurityPolicy = "EnterpriseVisibilitySecurityPolicy";
		public const string Key_EnterpriseVisibilityMessageEncoding = "EnterpriseVisibilityMessageEncoding";
		public const string Key_EnterpriseVisibilityUserIdentity = "EnterpriseVisibilityUserIdentity";
		public const string Key_EnterpriseVisibilityUserName = "EnterpriseVisibilityUserName";
		public const string Key_EnterpriseVisibilityUserPassword = "EnterpriseVisibilityPassword";
		public const string Key_EnterpriseVisibilityCertificatePath = "EnterpriseVisibilityCertificatePath";
		public const string Key_EnterpriseVisibilityPushPeriod = "EnterpriseVisibilityPushPeriod";
		public const string Key_EnterpriseVisibilityPushEnabled = "EnterpriseVisibilityPushEnabled";
		public const string Key_EnterpriseVisibilityTagsPerCall = "EnterpriseVisibilityTagsPerCall";

		// used to control wether we show bad or lask known good on a failure
		public const string Key_UseLastKnownGoodStatus = "UseLastKnownGoodStatus";

		public const string Key_Type_DWORD = "DWORD";
		public const string Key_Type_MULTI_SZ = "MULTI_SZ";
		public const string Key_Type_SZ = "SZ";
      public const string Key_Type_Password = "PWD";
      public const string Key_Type_Unknown = "UNKNOWN";
		public const string Key_Type_Time = "TIME";

		// Price Calculator
		public const string Key_PriceCalculatorInterface = "IPriceCalculatorDiscovery";

		public const string Key_QueryWriterAssemblies = "QueryWriterAssemblies";

		// External movement notifications
		public const string Key_MovementNotifyInterface = "MovementNotifyAssembly";

		public const string Key_PointCalculatorRowVisibilityConfig = "PointCalculatorRowVisibilityConfig";

		public const string Key_SynchronizedSettings = "SynchronizedSettings";
		#endregion

		#region Private data members
		[DataMember] private Guid configurationSettingGuid;
		[DataMember] private string keyType;
		[DataMember] private string settingKey;
		[DataMember] private string settingValue;
		[DataMember] private string createdBy;
		[DataMember] private string updatedBy;
		[DataMember] private DateTimeOffset? createdDate;
		[DataMember] private DateTimeOffset? updatedDate;
		#endregion

		#region Constructors
		public ConfigurationSettingDOClass()
		{
			this.Reset ( );
		}
		#endregion

		#region Properties
		public Guid ConfigurationSettingGuid
		{
			get { return this.configurationSettingGuid; }
			set { this.configurationSettingGuid = value; }
		}

		public string KeyType
		{
			get { return this.keyType; }
			set { this.keyType = value; }
		}

		public string SettingKey
		{
			get { return this.settingKey; }
			set { this.settingKey = value; }
		}

		public string SettingValue
		{
			get { return this.settingValue; }
			set { this.settingValue = value; }
		}

		public string CreatedBy
		{
			get { return this.createdBy; }
			set { this.createdBy = value; }
		}

		public string UpdatedBy
		{
			get { return this.updatedBy; }
			set { this.updatedBy = value; }
		}

		public DateTimeOffset? CreatedDate
		{
			get { return this.createdDate; }
			set { this.createdDate = value; }
		}

		public DateTimeOffset? UpdatedDate
		{
			get { return this.updatedDate; }
			set { this.updatedDate = value; }
		}
		#endregion

        #region Helper Methods
		public void GetKeyValueByKeySQL(SqlCommand cmd, string inKey)
		{
			if (string.IsNullOrEmpty ( inKey ) == true)
			{
				return;
			}

			cmd.CommandText = "  SELECT ConfigurationSettingGuid, "
				+ "			KeyType, "
				+ "			SettingKey, "
				+ "			SettingValue, "
				+ "			CreatedDate, "
				+ "			CreatedBy, "
				+ "			UpdatedDate, "
				+ "			UpdatedBy, "
				+ "			UpdatedDate "
				+ "	FROM tblConfigurationSetting "
				+ "	WHERE SettingKey = @Key ";

			SqlParameter indexParm = cmd.Parameters.Add ( "@Key", SqlDbType.NVarChar, 50 );
			indexParm.Value = inKey;
		}

		public void GetKeyValueByGuidSQL(SqlCommand cmd, string uniqueIdentify)
		{
			if (string.IsNullOrEmpty ( uniqueIdentify ) == true)
			{
				return;
			}

			cmd.CommandText = "  SELECT ConfigurationSettingGuid, "
				+ "			KeyType, "
				+ "			SettingKey, "
				+ "			SettingValue, "
				+ "			CreatedDate, "
				+ "			CreatedBy, "
				+ "			UpdatedDate, "
				+ "			UpdatedBy, "
				+ "			UpdatedDate "
				+ "	FROM tblConfigurationSetting " 
				+ "	WHERE ConfigurationSettingGuid = @Guid ";

			SqlParameter indexParm = cmd.Parameters.Add ( "@Guid", SqlDbType.NVarChar, 50 );
			indexParm.Value = uniqueIdentify;
		}

		public void EnumerateSQL ( SqlCommand cmd )
		{
			cmd.CommandText = "  SELECT ConfigurationSettingGuid, "
				+ "			KeyType, "
				+ "			SettingKey, "
				+ "			SettingValue, "
				+ "			CreatedDate, "
				+ "			CreatedBy, "
				+ "			UpdatedDate, "
				+ "			UpdatedBy, "
				+ "			UpdatedDate "
				+ "	FROM tblConfigurationSetting ORDER BY SettingKey";
		}

		public void Load ( DataTable dataTable )
		{
			if (dataTable != null)
			{
				if (( dataTable.Rows != null ) && ( dataTable.Rows.Count > 0 ))
				{
					DataRow row = dataTable.Rows[0];
					this.Load(row);
				}
			}
		}

		public void Load (DataRow row)
		{
			if (row != null)
			{
				this.configurationSettingGuid = DataObject.getValue<Guid>(row["ConfigurationSettingGuid"], Guid.Empty);
				this.keyType = DataObject.getValue<string>(row["KeyType"], "");
				this.settingKey = DataObject.getValue<string>(row["SettingKey"], "");
				this.settingValue = DataObject.getValue<string>(row["SettingValue"], "");
                if (this.settingValue != "")
                    this.settingValue = (string.Compare(this.keyType, Key_Type_Password, StringComparison.CurrentCultureIgnoreCase) == 0 ?
                                    CryptoHelper.DecryptAesSymmetric(Convert.FromBase64String(this.settingValue), Guids.SiteAdminGuid) :
                                    this.settingValue);
				this.createdDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
				this.createdBy = DataObject.getValue<string>(row["CreatedBy"], BaseDataObject.ADMIN);
				this.updatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], createdDate.Value);
				this.updatedBy = DataObject.getValue<string>(row["UpdatedBy"], BaseDataObject.ADMIN);
			}
		}

		public byte[] GetByteArray()
		{
			if (( string.IsNullOrEmpty ( this.keyType ) == false ) && ( this.keyType.ToUpper ( ).Equals ( Key_Type_SZ ) == true ) &&
				( string.IsNullOrEmpty ( this.settingValue ) == false ))
			{
				char[] charArray = this.settingValue.ToCharArray ( );
				byte[] strBytes = new byte[charArray.Length];
				int nextChar = 0;

				foreach (char usrChar in charArray)
				{
					strBytes[nextChar] = (byte) usrChar;
					nextChar++;
				}

				return strBytes;
			}
			else
			{
				return null;
			}
		}

		public string[] GetStringArray ( )
		{
			if (( string.IsNullOrEmpty ( this.keyType ) == false ) && ( this.keyType.ToUpper ( ).Equals ( Key_Type_MULTI_SZ ) == true ) &&
				( string.IsNullOrEmpty ( this.settingValue ) == false ))
			{
				char[] separator = { ';' };
				string[] strArray = this.settingValue.Split ( separator, StringSplitOptions.RemoveEmptyEntries );

				return strArray;
			}
			else
			{
				return null;
			}
		}

		public int? GetIntegerValue()
		{
			int? returnValue = null;

			if (( string.IsNullOrEmpty ( this.keyType ) == false ) && ( this.keyType.ToUpper ( ).Equals ( Key_Type_DWORD ) == true ) &&
				( string.IsNullOrEmpty ( this.settingValue ) == false ))
			{
				try
				{
					returnValue = Convert.ToInt32 ( this.settingValue );
				}
				catch (Exception)
				{

				}
			}

			return returnValue;
		}

		public double? GetDoubleValue ( )
		{
			double? returnValue = null;

			if (( string.IsNullOrEmpty ( this.keyType ) == false ) && ( this.keyType.ToUpper ( ).Equals ( Key_Type_DWORD ) == true ) &&
				( string.IsNullOrEmpty ( this.settingValue ) == false ))
			{
				try
				{
					returnValue = Convert.ToDouble ( this.settingValue );
				}
				catch (Exception)
				{

				}
			}

			return returnValue;
		}
		#endregion Helper Methods

		#region Public methods
		public void Reset ()
		{
			this.configurationSettingGuid	= Guid.Empty;
			this.keyType					= "";
			this.settingKey					= "";
			this.settingValue				= "";
			this.createdBy					= "";
			this.updatedBy					= "";
			this.createdDate				= null;
			this.updatedDate				= null;
		}
        #endregion Public methods

        #region Overrides
        public override string getSelectCommand ( )
		{
			return null;
		}
		public override string getDeleteCommand ( )
		{
			return null;
		}
		public override string getInsertCommand ( )
		{
			return null;
		}
		public override string getUpdateCommand ( )
		{
			return null;
		}
		#endregion
	}
}
