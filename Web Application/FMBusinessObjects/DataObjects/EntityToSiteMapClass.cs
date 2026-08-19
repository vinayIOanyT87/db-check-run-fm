// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityToSiteMapClass.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	Defines the EntityToSiteMapClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Diagnostics;
	using System.Runtime.CompilerServices;
	using System.Runtime.Serialization;

	public enum ENTITY_TYPE
	{
		UNKNOWN = 0,

		UNDEFINED,

		ADDITIVE_PROFILE,

		ALARM_AND_EVENT,

		ALARM_EVENT_CATEGORY,

		ALARM_PRIORITY,

		ALLOCATION,

		ALLOCATION_GROUP,

		ALLOCATION_LINE_ITEM,

		APPOINTMENT_EQUIPMENT,

		APPOINTMENT_PERSONNEL,

		APPOINTMENT_TANK,

		ASSOCIATED_PAYMENT,

		AUTODISTRIBUTION_REASONCODE,

		AUTODISTRIBUTION_RULE,

		BULK_PAYMENT,

		BULK_PAYMENT_INVOICE_MAPPING,

		CHANGE_QUEUE_RECORD,

		CLOSEOUT,

		COMPANY,

		COMPANY_GROUP,

		COMPANY_ROLE,

		COMPANY_TYPE,

		CONTROLLER_LOG,

		DATA_DICTIONARY,

		DATA_SYNCHRONIZATION_CONFIG,

		DATA_TRANSMISSION,

		DISPATCH_CONFIGURATION,

		DOT_HAZARDOUS_MESSAGE,

		EMAIL_ADDRESS,

		EMAIL_GROUP,

		ENTRY_MESSAGE,

		EQUIPMENT,

		EQUIPMENT_MAINTENANCE_LOG,

		EQUIPMENT_QUALITY_TAG_LOG,

		EQUIPMENT_TYPE,

		EXIT_MESSAGE,

		EXPORT_RESULT,

		EXPORT_RESULT_DETAIL,

		EXTERNAL_STATION,

		EXTERNAL_STATION_DEVICE,

		FESS_REBATE,

		FESS_SUMMARY,

		FILTER_VIEW,

		FOOTNOTE,

		FUEL_CARD,

		FUEL_CARD_LIMIT,

		GATE,

		GROUP,

		HOUSE_CARD,

		IATA_CODE,

		INVOICE_QUERY,

		LEDGER_AGGREGATE_COLUMN,

		LEDGER_VIEW,

		LIST_VIEW,

		LIST_VIEW_FIELD,

		LOAD_ARM,

		MAINTENANCE_REASON,

		MESSAGE,

		MESSAGE_LOG,

		METER,

		NONE,

		NOTE,

		OPC_CONNECTION,

		PERSONNEL,

		PERSONNEL_INFO,

		PIDX_PROFILE,

		PIDX_PROFILE_COMPANY_MAP,

		PROCESS_VARIABLE,

		PROCESS_VARIABLE_MESSAGE,

		PRODUCT,

		PRODUCT_GROUP,

		PRODUCT_MAP_ADDITIVE_PROFILE,

		PRODUCT_MAP_BLEND_COMPONENT,

		PRODUCT_MAP_COMPANY,

		PRODUCT_MAP_COMPANY_GROUP,

		PRODUCT_MAP_COMPANY_SUPPLIER,

		PRODUCT_MAP_COMPANY_UNAVAILABLE_INVENTORY,

		PRODUCT_MAP_PRESET_COMPONENT,

		PRODUCT_MAP_PRESET_EXTERNAL_COMPONENT,

		PRODUCT_MAP_PRESET_INJECTOR,

		PRODUCT_MAP_PRESET_RECIPE,

		PRODUCT_MAP_PRODUCT_GROUP,

		PRODUCT_MAP_TRANSACTION_ALIAS_EXCLUSION,

		PRODUCT_MAP_UNDEFINED,

		PRODUCT_MESSAGE,

		QUALIFICATION_COMPANY_CERTIFICATE_AND_PERMIT,

		QUALIFICATION_EQUIPMENT_TEST_AND_INSPECTION,

		QUALIFICATION_EQUIPMENT_TAG_AND_LICENSE,

		QUALIFICATION_PERSON_QUALIFICATION,

		QUALIFICATION_PERSON_LICENSE,

		QUALIFICATION_PERSON_TRAINING,

		QUALIFICATION_MAP,

		QUALIFICATION_MAP_PERSON_QUALIFICATION_TO_EQUIPMENT_TYPE,

		QUALIFICATION_MAP_PERSON_TRAINING_TO_EQUIPMENT_TYPE,

		QUALIFICATION_MAP_PERSON_QUALIFICATION_TO_STATION,

		QUALIFICATION_MAP_PERSON_TRAINING_TO_STATION,

		QUALIFICATION_MAP_EQUIPMENT_TEST_AND_INSPECTION_TO_STATION,

		QUALITY_TAG,

		QUERY,

		QUERY_DEFAULT,

		QUERY_DEFAULT_FIELD,

		QUERY_GROUP_ASSIGNMENT,

		REPORT_APPROVAL,

		REPORT_CONFIGURATION_SETTINGS,

		RESERVE_LEVEL,

		SCHEDULE_COMPANY_ACCESS,

		SCHEDULE_HOLIDAY,

		SCHEDULE_PERSON_ACCESS,

		SCHEDULE_TERMINAL_OPERATIONS,

		SEQUENCE,

		SITE,

		SITE_TO_SITE,

		STANDING_OFFER,

		STATION,

		SYSTEM_SETTING,

		TANK,

		TANK_GROUP,

		TANK_MAP,

		TANK_MAINTENANCE_LOG,

		TANK_QUALITY_TAG_LOG,

		TEST,

		TEST_EQUIPMENT_RESULT,

		TEST_SET,

		TEST_SET_EQUIPMENT_RESULT,

		TEST_SET_TANK_RESULT,

		TEST_TANK_RESULT,

		TRANSACTION,

		TRANSACTION_ALIAS,

		TRANSACTION_ALIAS_FIELD,

		TRANSACTION_ALIAS_LINE_ITEM,

		TRANSACTION_ALIAS_NAME,

		USER,

		USER_DATA_FIELD,

		WEIGHT_AVERAGE_COST,

		MOBILE_DEVICE_PROFILE,

		SHIPTO_STATE,

		FUEL_CARD_TYPE,

		PRODUCT_MAP_PRESET_FLOW_CONTROLLED_ADDITIVE,

		PRODUCT_MAP_OFFLOAD_EXTERNAL_METER,

		ASSET_TRACKING_DEVICE,

		ASSET_TRACKING_MAP_CONFIGURATION,

		QUALIFICATION_MAP_EQUIPMENT_TAG_AND_LICENSE_TO_STATION,

		POINT_TEMPLATE_TYPE,

		POINT_TEMPLATE,

		SITE_CERTIFICATE,

		POINT_CATEGORY,

		MODULE

	};

	public static class EntityExtensions
	{
		/// <summary>
		/// Verifies if a given entity type supports mapping of the individual entity records.
		/// Some Entity Types, e.g. Alarm & Events, are not mapped individually, but are mapped as a whole.
		/// </summary>
		/// <param name="entityType"></param>
		/// <returns>True: Entity Type supports individual mapping.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsEntityTypeSupportsIndividualEntityMapping( this ENTITY_TYPE entityType )
		{
			bool result = true;

			switch ( entityType )
			{
				case ENTITY_TYPE.ALARM_AND_EVENT:
				case ENTITY_TYPE.DATA_DICTIONARY:
				case ENTITY_TYPE.QUERY:
				case ENTITY_TYPE.QUERY_DEFAULT:
				case ENTITY_TYPE.QUERY_DEFAULT_FIELD:
				case ENTITY_TYPE.REPORT_CONFIGURATION_SETTINGS:
				case ENTITY_TYPE.USER_DATA_FIELD:
				case ENTITY_TYPE.EXTERNAL_STATION_DEVICE:
					{
					result = false; 
					break;
				}
			}

			return result;
		}
	}

	[Serializable]
	[CollectionDataContract]
	public class EntityToSiteMapCollectionClass : List<EntityToSiteMapClass>
	{
	}

	[EntityImportExportWorksheetAttribute("ASSIGNMENTS")]
	[DataContract]
	[Serializable]
	public class EntityToSiteMapClass : BaseDataObject
	{
		#region Constructors and Destructors

		public EntityToSiteMapClass()
        {
            this.DisableSelection = false;
        }

		public EntityToSiteMapClass(BaseDataObject dataObject)
		{
			this.TypeID                 = dataObject.EntityType;
			this.SiteGuid               = dataObject.SiteGuid;
			this.AssignedFromSiteGuid   = dataObject.SiteGuid;
			this.IdentityGuid           = dataObject.IdentityGuid;
		    this.DisableSelection       = false;

			//ID refers to Entity's ID and not EntityToSiteMap since it doesn't have one.
			this.ID = (string.IsNullOrEmpty(dataObject.ID) ? "" : dataObject.ID);
		}


		/// <summary>
		///	Initializes a new instance of the <see cref="EntityToSiteMapClass" /> class.
		/// </summary>
		/// <param name="entityId">
		///	The entity Id.
		/// </param>
		/// <param name="entityType">
		///	The entity Type.
		/// </param>
		/// <param name="siteGuid">
		///	The site Guid.
		/// </param>
		/// <param name="identityGuid">
		///	The identity Guid.
		/// </param>
		public EntityToSiteMapClass(string entityId, ENTITY_TYPE entityType, Guid siteGuid, Guid identityGuid)
		{
			this.TypeID             = entityType;
			this.SiteGuid           = siteGuid;
			this.IdentityGuid       = identityGuid;
            this.DisableSelection   = false;

            // ID refers to Entity's ID and not EntityToSiteMap since it doesn't have one.
            this.ID = string.IsNullOrEmpty(entityId) ? string.Empty : entityId;
		}

		#endregion

		#region Public Properties
		[EntityImportExportAttribute("ASSIGNEDID*", 105, "ID")]
		public override string ID
		{
			get { return _ID; }
			set { _ID = value; }
		}

		[EntityImportExportAttribute("SITEID*", 105, "SITE ID")]
		public override string SiteID
		{
			get { return _SiteID; }
			set { _SiteID = value; }
		}


		[DataMember]
		public Guid AssignedFromSiteGuid { get; set; }

		[DataMember]
		public string AssignedFromSiteId { get; set; }

		[DataMember]
		public bool IsAssigned { get; set; }

		[DataMember]
		public bool IsOwner { get; set; }

		[DataMember]
		[EntityImportExportAttribute("TYPEID*", 105, "TypeID")]
		public ENTITY_TYPE TypeID { get; set; }

        [DataMember]
        public bool DisableSelection { get; set; }

		#endregion

		#region Public Methods and Operators

		public static string GetEntityTypeID(ENTITY_TYPE entityType)
		{
			switch (entityType)
			{
				case ENTITY_TYPE.UNDEFINED:
					return "Undefined";
				case ENTITY_TYPE.ADDITIVE_PROFILE:
					return "Additive Profiles";
				case ENTITY_TYPE.ALARM_AND_EVENT:
					return "Alarm And Events";
				case ENTITY_TYPE.ALARM_EVENT_CATEGORY:
					return "Alarm Event Category";
				case ENTITY_TYPE.ALARM_PRIORITY:
					return "Alarm Priorities";
				case ENTITY_TYPE.ALLOCATION_GROUP:
					return "Allocation Groups";
				case ENTITY_TYPE.ALLOCATION_LINE_ITEM:
					return "Allocation Line Item";
				case ENTITY_TYPE.APPOINTMENT_EQUIPMENT:
				case ENTITY_TYPE.APPOINTMENT_PERSONNEL:
				case ENTITY_TYPE.APPOINTMENT_TANK:
					return "Appointment";
				case ENTITY_TYPE.ASSET_TRACKING_DEVICE:
					return "Asset Tracking Device";
				case ENTITY_TYPE.ASSET_TRACKING_MAP_CONFIGURATION:
					return "Asset Tracking Map Configuration";
				case ENTITY_TYPE.ASSOCIATED_PAYMENT:
					return "FESS Associated Payment";
				case ENTITY_TYPE.AUTODISTRIBUTION_REASONCODE:
					return "Auto Distribution Reason Code";
				case ENTITY_TYPE.AUTODISTRIBUTION_RULE:
					return "Auto Distribution Rule";
				case ENTITY_TYPE.BULK_PAYMENT:
					return "Bulk Payment";
				case ENTITY_TYPE.BULK_PAYMENT_INVOICE_MAPPING:
					return "Bulk Payment Invoice Mapping";
				case ENTITY_TYPE.CHANGE_QUEUE_RECORD:
					return string.Empty; //override in the class returns the empty string
				case ENTITY_TYPE.CLOSEOUT:
					return "Close out";
				case ENTITY_TYPE.COMPANY:
					return "Companies";
				case ENTITY_TYPE.COMPANY_GROUP:
					return "Company Groups";
				case ENTITY_TYPE.COMPANY_ROLE:
					return "Company Roles";
				case ENTITY_TYPE.COMPANY_TYPE:
					return "Company Types";
				case ENTITY_TYPE.CONTROLLER_LOG:
					return "Controller Log";
				case ENTITY_TYPE.DATA_DICTIONARY:
					return "Data Dictionary";
				case ENTITY_TYPE.DATA_SYNCHRONIZATION_CONFIG:
					return "Data Synchronization Config";
				case ENTITY_TYPE.DATA_TRANSMISSION:
					return "Data Transmission";
				case ENTITY_TYPE.DISPATCH_CONFIGURATION:
					return "Dispatch Configurations";
				case ENTITY_TYPE.DOT_HAZARDOUS_MESSAGE:
					return "DOT Hazardous Messages";
				case ENTITY_TYPE.EMAIL_ADDRESS:
					return "E-mail Address";
				case ENTITY_TYPE.EMAIL_GROUP:
					return "E-mail Groups";
				case ENTITY_TYPE.ENTRY_MESSAGE:
					return "Entry Message";
				case ENTITY_TYPE.EQUIPMENT:
					return "Equipment";
				case ENTITY_TYPE.EQUIPMENT_MAINTENANCE_LOG:
					return "Equipment Maintenance Log Class";
				case ENTITY_TYPE.EQUIPMENT_QUALITY_TAG_LOG:
					return "Equipment Quality Tag Log Class";
				case ENTITY_TYPE.EQUIPMENT_TYPE:
					return "Equipment Type";
				case ENTITY_TYPE.EXIT_MESSAGE:
					return "Exit Message";
				case ENTITY_TYPE.EXPORT_RESULT:
					return "Export Results";
				case ENTITY_TYPE.EXPORT_RESULT_DETAIL:
					return "Export Result Details";
				case ENTITY_TYPE.EXTERNAL_STATION_DEVICE:
					return "External Station Device";
				case ENTITY_TYPE.FESS_REBATE:
					return "FESS Rebate";
				case ENTITY_TYPE.FESS_SUMMARY:
					return "FESS Summary";
				case ENTITY_TYPE.FILTER_VIEW:
					return "Filter View";
				case ENTITY_TYPE.FOOTNOTE:
					return "Footnotes";
				case ENTITY_TYPE.FUEL_CARD:
					return "Fuel Card";
				case ENTITY_TYPE.FUEL_CARD_LIMIT:
					return "Fuel Card Limit";
				case ENTITY_TYPE.FUEL_CARD_TYPE:
					return "Fuel Card Types";
				case ENTITY_TYPE.GATE:
					return "Gates";
				case ENTITY_TYPE.GROUP:
					return "User Groups";
				case ENTITY_TYPE.HOUSE_CARD:
					return "House Cards";
				case ENTITY_TYPE.IATA_CODE:
					return "Delivery Locations";
				case ENTITY_TYPE.INVOICE_QUERY:
					return "Invoice Query";
				case ENTITY_TYPE.LEDGER_AGGREGATE_COLUMN:
					return "Ledger Aggregate Column";
				case ENTITY_TYPE.LEDGER_VIEW:
					return "Ledger Views";
				case ENTITY_TYPE.LIST_VIEW:
					return "List Views"; //varies on the type of list view. Need to investigate further
				case ENTITY_TYPE.LIST_VIEW_FIELD:
					return "List View Fields";
				case ENTITY_TYPE.LOAD_ARM:
					return "Load Arms";
				case ENTITY_TYPE.MAINTENANCE_REASON:
					return "Maintenance Reason";
				case ENTITY_TYPE.MESSAGE:
					return "Message";
				case ENTITY_TYPE.MESSAGE_LOG:
					return "MessageLog";
				case ENTITY_TYPE.METER:
					return "Meter";
				case ENTITY_TYPE.NOTE:
					return "Note";
				case ENTITY_TYPE.OPC_CONNECTION:
					return "OPC Connections";
				case ENTITY_TYPE.PERSONNEL:
					return "Personnel";
				case ENTITY_TYPE.PERSONNEL_INFO:
					return "Person Info Class";
				case ENTITY_TYPE.PIDX_PROFILE:
					return "PIDX Profile";
				case ENTITY_TYPE.PIDX_PROFILE_COMPANY_MAP:
					return "PIDX Profile Company Map";
				case ENTITY_TYPE.PROCESS_VARIABLE:
					return "Process Variables";
				case ENTITY_TYPE.PROCESS_VARIABLE_MESSAGE:
					return "Process Variable Message";
				case ENTITY_TYPE.PRODUCT:
					return "Products";
				case ENTITY_TYPE.PRODUCT_GROUP:
					return "Product Groups";
				case ENTITY_TYPE.PRODUCT_MAP_ADDITIVE_PROFILE:
					return "Additive Profile Assignment";
				case ENTITY_TYPE.PRODUCT_MAP_BLEND_COMPONENT:
					return "Blend Component Assignment";
				case ENTITY_TYPE.PRODUCT_MAP_COMPANY:
					return "Company Product Assignment";
				case ENTITY_TYPE.PRODUCT_MAP_COMPANY_GROUP:
					return "Company Group Product Assignment";
				case ENTITY_TYPE.PRODUCT_MAP_COMPANY_SUPPLIER:
					return "Company Supplier Product Assignment";
				case ENTITY_TYPE.PRODUCT_MAP_COMPANY_UNAVAILABLE_INVENTORY:
					return "Company Unavailable Inventory Assignment";
				case ENTITY_TYPE.PRODUCT_MAP_PRESET_COMPONENT:
					return "Preset Component Assignment";
				case ENTITY_TYPE.PRODUCT_MAP_PRESET_EXTERNAL_COMPONENT:
					return "Preset External Component Assignment";
				case ENTITY_TYPE.PRODUCT_MAP_PRESET_FLOW_CONTROLLED_ADDITIVE:
					return "Preset Flow Controlled Additive Assignment";
				case ENTITY_TYPE.PRODUCT_MAP_OFFLOAD_EXTERNAL_METER:
					return "Offload External Meter Assignment";
				case ENTITY_TYPE.PRODUCT_MAP_PRESET_INJECTOR:
					return "Preset Injector Assignment";
				case ENTITY_TYPE.PRODUCT_MAP_PRESET_RECIPE:
					return "Preset Recipe Assignment";
				case ENTITY_TYPE.PRODUCT_MAP_PRODUCT_GROUP:
					return "Product Group Assignment";
				case ENTITY_TYPE.PRODUCT_MAP_TRANSACTION_ALIAS_EXCLUSION:
					return "Transaction Alias Product Exclusion Assignment";
				case ENTITY_TYPE.PRODUCT_MAP_UNDEFINED:
					return "Undefined Product Assignment";
				case ENTITY_TYPE.PRODUCT_MESSAGE:
					return "Product Messages";
				case ENTITY_TYPE.QUALIFICATION_COMPANY_CERTIFICATE_AND_PERMIT:
					return "Company Certificates and Permits";
				case ENTITY_TYPE.QUALIFICATION_EQUIPMENT_TEST_AND_INSPECTION:
					return "Equipment Tests and Inspections";
				case ENTITY_TYPE.QUALIFICATION_EQUIPMENT_TAG_AND_LICENSE:
					return "Equipment Tags and Licenses";
				case ENTITY_TYPE.QUALIFICATION_PERSON_QUALIFICATION:
					return "Personnel Qualifications";
				case ENTITY_TYPE.QUALIFICATION_PERSON_LICENSE:
					return "Personnel Licenses";
				case ENTITY_TYPE.QUALIFICATION_PERSON_TRAINING:
					return "Personnel Training";
				case ENTITY_TYPE.QUALIFICATION_MAP:
					return "Qualification Maps";
				case ENTITY_TYPE.QUALIFICATION_MAP_PERSON_QUALIFICATION_TO_EQUIPMENT_TYPE:
					return "Equipment Type Personnel Qualification";
				case ENTITY_TYPE.QUALIFICATION_MAP_PERSON_TRAINING_TO_EQUIPMENT_TYPE:
					return "Equipment Type Personnel Training";
				case ENTITY_TYPE.QUALIFICATION_MAP_PERSON_QUALIFICATION_TO_STATION:
					return "Station Personnel Qualification";
				case ENTITY_TYPE.QUALIFICATION_MAP_PERSON_TRAINING_TO_STATION:
					return "Station Personnel Training";
				case ENTITY_TYPE.QUALIFICATION_MAP_EQUIPMENT_TEST_AND_INSPECTION_TO_STATION:
					return "Station Equipment Test & Inspection";
				case ENTITY_TYPE.QUALIFICATION_MAP_EQUIPMENT_TAG_AND_LICENSE_TO_STATION:
					return "Station Equipment Tag & Licenses";
				case ENTITY_TYPE.QUALITY_TAG:
					return "Quality Tag";
				case ENTITY_TYPE.QUERY:
					return string.Empty; //override in the class returns the empty string
				case ENTITY_TYPE.QUERY_DEFAULT:
					return "Query Settings";
				case ENTITY_TYPE.QUERY_DEFAULT_FIELD:
					return "Query Settings"; //returns the same as QueryDefault
				case ENTITY_TYPE.REPORT_CONFIGURATION_SETTINGS:
					return "Report Configuration Settings";
				case ENTITY_TYPE.QUERY_GROUP_ASSIGNMENT:
					return "Query Group Assignments";
				case ENTITY_TYPE.REPORT_APPROVAL:
					return string.Empty; //override in the class returns the empty string
				case ENTITY_TYPE.RESERVE_LEVEL:
					return "Reserve Level";
				case ENTITY_TYPE.SCHEDULE_COMPANY_ACCESS:
					return "Company Access Schedule";
				case ENTITY_TYPE.SCHEDULE_HOLIDAY:
					return "Holiday Schedule";
				case ENTITY_TYPE.SCHEDULE_PERSON_ACCESS:
					return "Person Access Schedule";
				case ENTITY_TYPE.SCHEDULE_TERMINAL_OPERATIONS:
					return "Terminal Operations Schedule";
				case ENTITY_TYPE.SEQUENCE:
					return "Sequence";
				case ENTITY_TYPE.SITE:
					return "Sites";
				case ENTITY_TYPE.SITE_TO_SITE:
					return "site To site Maps";
				case ENTITY_TYPE.STANDING_OFFER:
					return "Price List Entry";
				case ENTITY_TYPE.STATION:
					return "Stations";
				case ENTITY_TYPE.SYSTEM_SETTING:
					return "System Setting";
				case ENTITY_TYPE.TANK:
					return "Tanks";
				case ENTITY_TYPE.TANK_GROUP:
					return "Tank Groups";
				case ENTITY_TYPE.TANK_MAP:
					return "Tank Group Assignment";
				case ENTITY_TYPE.TANK_MAINTENANCE_LOG:
					return "TankMaintenanceLogClass";
				case ENTITY_TYPE.TANK_QUALITY_TAG_LOG:
					return "TankQualityTagLogClass";
				case ENTITY_TYPE.TEST:
					return "Test";
				case ENTITY_TYPE.TEST_EQUIPMENT_RESULT:
					return string.Empty; //override in the class returns the empty string
				case ENTITY_TYPE.TEST_SET:
					return "Test Set";
				case ENTITY_TYPE.TEST_SET_EQUIPMENT_RESULT:
					return string.Empty; //override in the class returns the empty string
				case ENTITY_TYPE.TEST_SET_TANK_RESULT:
					return string.Empty; //override in the class returns the empty string
				case ENTITY_TYPE.TEST_TANK_RESULT:
					return string.Empty; //override in the class returns the empty string
				case ENTITY_TYPE.TRANSACTION:
					return "Transactions";
				case ENTITY_TYPE.TRANSACTION_ALIAS:
					return "Transaction Aliases";
				case ENTITY_TYPE.TRANSACTION_ALIAS_FIELD:
					return "Transaction Alias Field";
				case ENTITY_TYPE.TRANSACTION_ALIAS_LINE_ITEM:
					return "Transaction Alias Line Item";
				case ENTITY_TYPE.TRANSACTION_ALIAS_NAME:
					return "Transaction Aliases"; //same as transaction alias
				case ENTITY_TYPE.USER:
					return "Users";
				case ENTITY_TYPE.USER_DATA_FIELD:
					return "User Data";
				case ENTITY_TYPE.WEIGHT_AVERAGE_COST:
					return string.Empty; //override in the class returns the empty string
				case ENTITY_TYPE.MOBILE_DEVICE_PROFILE:
					return "Mobile Device Profile";
				case ENTITY_TYPE.SHIPTO_STATE:
					return "Ship To State";
				case ENTITY_TYPE.POINT_TEMPLATE:
					return "Point Templates";
				case ENTITY_TYPE.POINT_TEMPLATE_TYPE:
					return "Point Types";
				case ENTITY_TYPE.POINT_CATEGORY:
					return "Point Categories";
				case ENTITY_TYPE.MODULE:
					return "Modules";

				default:
					Debug.Assert(false, "Entity Type ID not found.");
					return "Unknown";
			}
		}

		public static string GetMappingTableName(ENTITY_TYPE entityType)
		{
			const string SchemaPrefix = "map.";

			switch (entityType)
			{
				case ENTITY_TYPE.ADDITIVE_PROFILE:
					return SchemaPrefix + "tblEntityAdditiveProfileToSite";
				case ENTITY_TYPE.ALARM_AND_EVENT:
					return SchemaPrefix + "tblEntityAlarmAndEventToSite";
				case ENTITY_TYPE.ALARM_EVENT_CATEGORY:
					return SchemaPrefix + "tblEntityAlarmAndEventCategoryToSite";
				case ENTITY_TYPE.ALARM_PRIORITY:
					return SchemaPrefix + "tblEntityAlarmPriorityToSite";
				case ENTITY_TYPE.ALLOCATION_GROUP:
					return SchemaPrefix + "tblEntityAllocationGroupToSite";
				case ENTITY_TYPE.APPOINTMENT_EQUIPMENT:
					return SchemaPrefix + "tblEntityAppointmentEquipmentToSite";
				case ENTITY_TYPE.APPOINTMENT_PERSONNEL:
					return SchemaPrefix + "tblEntityAppointmentPersonnelToSite";
				case ENTITY_TYPE.APPOINTMENT_TANK:
					return SchemaPrefix + "tblEntityAppointmentTankToSite";
				case ENTITY_TYPE.ASSET_TRACKING_DEVICE:
					return SchemaPrefix + "tblEntityAssetTrackingDeviceToSite";
				case ENTITY_TYPE.ASSET_TRACKING_MAP_CONFIGURATION:
					return SchemaPrefix + "tblEntityAssetTrackingMapConfigurationToSite";
				case ENTITY_TYPE.AUTODISTRIBUTION_REASONCODE:
					return SchemaPrefix + "tblEntityAutoDistributionReasonCodeToSite";
				case ENTITY_TYPE.AUTODISTRIBUTION_RULE:
					return SchemaPrefix + "tblEntityAutoDistributionRuleToSite";
				case ENTITY_TYPE.COMPANY:
					return SchemaPrefix + "tblEntityCompanyToSite";
				case ENTITY_TYPE.COMPANY_GROUP:
					return SchemaPrefix + "tblEntityCompanyGroupToSite";
				case ENTITY_TYPE.COMPANY_TYPE:
					return SchemaPrefix + "tblEntityCompanyTypeToSite";
				case ENTITY_TYPE.DATA_DICTIONARY:
					return SchemaPrefix + "tblEntityDataDictionaryToSite";
				case ENTITY_TYPE.DISPATCH_CONFIGURATION:
					return SchemaPrefix + "tblEntityDispatchConfigurationToSite";
				case ENTITY_TYPE.DOT_HAZARDOUS_MESSAGE:
					return SchemaPrefix + "tblEntityDotHazardousMessagesToSite";
				case ENTITY_TYPE.EMAIL_ADDRESS:
					return SchemaPrefix + "tblEntityEmailAddressToSite";
				case ENTITY_TYPE.EMAIL_GROUP:
					return SchemaPrefix + "tblEntityEmailGroupToSite";
				case ENTITY_TYPE.ENTRY_MESSAGE:
					return SchemaPrefix + "tblEntityEntryMessageToSite";
				case ENTITY_TYPE.EQUIPMENT:
					return SchemaPrefix + "tblEntityEquipmentToSite";
				case ENTITY_TYPE.EQUIPMENT_TYPE:
					return SchemaPrefix + "tblEntityEquipmentTypeToSite";
				case ENTITY_TYPE.EXIT_MESSAGE:
					return SchemaPrefix + "tblEntityExitMessageToSite";
				case ENTITY_TYPE.EXTERNAL_STATION:
					return SchemaPrefix + "tblEntityExternalStationToSite";
				case ENTITY_TYPE.FOOTNOTE:
					return SchemaPrefix + "tblEntityFootNoteToSite";
				case ENTITY_TYPE.FUEL_CARD:
					return SchemaPrefix + "tblEntityFuelCardToSite";
				case ENTITY_TYPE.FUEL_CARD_LIMIT:
					return SchemaPrefix + "tblEntityFuelCardLimitToSite";
				case ENTITY_TYPE.FUEL_CARD_TYPE:
					return SchemaPrefix + "tblEntityFuelCardTypeToSite";
				case ENTITY_TYPE.GROUP:
					return SchemaPrefix + "tblEntityUserGroupToSite";
				case ENTITY_TYPE.IATA_CODE:
					return SchemaPrefix + "tblEntityIATACodeToSite";
				case ENTITY_TYPE.LEDGER_AGGREGATE_COLUMN:
					return SchemaPrefix + "tblEntityLedgerAggregateColumnToSite";
				case ENTITY_TYPE.LEDGER_VIEW:
					return SchemaPrefix + "tblEntityLedgerViewToSite";
				case ENTITY_TYPE.LIST_VIEW:
					return SchemaPrefix + "tblEntityListViewToSite";
				case ENTITY_TYPE.MAINTENANCE_REASON:
					return SchemaPrefix + "tblEntityMaintenanceReasonToSite";
				case ENTITY_TYPE.PERSONNEL:
					return SchemaPrefix + "tblEntityPersonnelToSite";
				case ENTITY_TYPE.PROCESS_VARIABLE_MESSAGE:
					return SchemaPrefix + "tblEntityProcessVariableMessageToSite";
				case ENTITY_TYPE.PRODUCT:
					return SchemaPrefix + "tblEntityProductToSite";
				case ENTITY_TYPE.PRODUCT_GROUP:
					return SchemaPrefix + "tblEntityProductGroupToSite";
				case ENTITY_TYPE.PRODUCT_MESSAGE:
					return SchemaPrefix + "tblEntityProductMessageToSite";
				case ENTITY_TYPE.QUALIFICATION_COMPANY_CERTIFICATE_AND_PERMIT:
					return SchemaPrefix + "tblEntityCompanyCertificateAndPermitToSite";
				case ENTITY_TYPE.QUALIFICATION_EQUIPMENT_TEST_AND_INSPECTION:
					return SchemaPrefix + "tblEntityEquipmentTestAndInspectionToSite";
				case ENTITY_TYPE.QUALIFICATION_EQUIPMENT_TAG_AND_LICENSE:
					return SchemaPrefix + "tblEntityEquipmentTagAndLicenseToSite";
				case ENTITY_TYPE.QUALIFICATION_PERSON_QUALIFICATION:
					return SchemaPrefix + "tblEntityPersonnelQualificationToSite";
				case ENTITY_TYPE.QUALIFICATION_PERSON_LICENSE:
					return SchemaPrefix + "tblEntityPersonnelLicenseToSite";
				case ENTITY_TYPE.QUALIFICATION_PERSON_TRAINING:
					return SchemaPrefix + "tblEntityPersonnelTrainingToSite";
				case ENTITY_TYPE.QUALITY_TAG:
					return SchemaPrefix + "tblEntityQualityTagToSite";
				case ENTITY_TYPE.QUERY:
					return SchemaPrefix + "tblEntityQuerySettingToSite";
				case ENTITY_TYPE.QUERY_DEFAULT:
					return SchemaPrefix + "tblEntityQuerySettingToSite";
				case ENTITY_TYPE.QUERY_DEFAULT_FIELD:
					return SchemaPrefix + "tblEntityQuerySettingToSite";
				case ENTITY_TYPE.REPORT_CONFIGURATION_SETTINGS:
					return SchemaPrefix + "tblEntityReportConfigurationSettingsToSite";
				case ENTITY_TYPE.STANDING_OFFER:
					return SchemaPrefix + "tblEntityStandingOfferToSite";
				case ENTITY_TYPE.TEST:
					return SchemaPrefix + "tblEntityTestToSite";
				case ENTITY_TYPE.TEST_SET:
					return SchemaPrefix + "tblEntityTestSetToSite";
				case ENTITY_TYPE.TRANSACTION_ALIAS:
				case ENTITY_TYPE.TRANSACTION_ALIAS_NAME:
					return SchemaPrefix + "tblEntityTransactionAliasToSite";
				case ENTITY_TYPE.USER:
					return SchemaPrefix + "tblEntityUserToSite";
				case ENTITY_TYPE.USER_DATA_FIELD:
					return SchemaPrefix + "tblEntityUserDataToSite";
				case ENTITY_TYPE.MOBILE_DEVICE_PROFILE:
					return SchemaPrefix + "tblEntityMobileDeviceProfileToSite";
				case ENTITY_TYPE.POINT_TEMPLATE:
					return SchemaPrefix + "tblEntityPointTemplateToSite";
				case ENTITY_TYPE.POINT_TEMPLATE_TYPE:
					return SchemaPrefix + "tblEntityPointTemplateTypeToSite";
				case ENTITY_TYPE.POINT_CATEGORY:
					return SchemaPrefix + "tblEntityPointCategoryToSite";
				case ENTITY_TYPE.MODULE:
					return SchemaPrefix + "tblEntityModuleToSite";
				default:
					Debug.Assert(false, "Entity to site mapping table name not found.");
					return "Unknown";
			}
		}

		public void Load(DataSet set)
		{
			if (set == null)
			{
				throw new ArgumentNullException("set");
			}

			//do not allow the typeID to be reset
			ENTITY_TYPE typeID = this.TypeID;

			this.Reset();

			DataTable table = set.Tables[0];
			if (table.Rows.Count == 0)
			{
				return;
			}

			DataRow row = table.Rows[0];
			this.TypeID = typeID;
			this.SiteGuid = DataObject.getValue<Guid>(row["AssignedToSiteGuid"], Guid.Empty);
			this.IdentityGuid = DataObject.getValue<Guid>(row["EntityRecordGuid"], Guid.Empty);
			this.CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
			this.CreatedBy = DataObject.getValue<string>(row["CreatedBy"], ADMIN);
			if (table.Columns.IndexOf("EntityId") >= 0)
			{
				this.ID = DataObject.getValue<string>(row["EntityId"], "");
			}
			if (table.Columns.IndexOf("AssignedToSiteId") >= 0)
			{
				this.SiteID = DataObject.getValue<string>(row["AssignedToSiteId"], "");
			}
			if (table.Columns.IndexOf("AssignedFromSiteGuid") >= 0)
			{
				this.AssignedFromSiteGuid = DataObject.getValue<Guid>(row["AssignedFromSiteGuid"], Guid.Empty);
			}
			if (table.Columns.IndexOf("AssignedFromSiteId") >= 0)
			{
				this.AssignedFromSiteId = DataObject.getValue<string>(row["AssignedFromSiteId"], "");
			}
			if (this.AssignedFromSiteGuid == this.SiteGuid)
			{
				this.IsOwner = true;
			}
			this.IsAssigned = true;
		}

		public override void Reset()
		{
			base.Reset();
			this.IsAssigned = true;
			this.IsOwner = true;
			this.TypeID = ENTITY_TYPE.UNKNOWN;
		}

		#endregion
	}
}