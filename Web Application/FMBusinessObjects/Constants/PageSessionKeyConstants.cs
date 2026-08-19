// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PageSessionKeyConstants.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the PageSessionKeyConstants type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.Constants
{
	/// <summary>
	/// The page session key constants.
	/// </summary>
	public static class PageSessionKeyConstants
	{
		// Make sure to always prefix the session key for the page by the page name.

		// Company Role Assignment Form session keys.
		public const string CRAF_SESSION_COMPANY_SELECT			= "CompanyRoleAssignmentForm.CompanySelection";
		public const string CRAF_SESSION_COMPANY_ROLE_SELECT	= "CompanyRoleAssignmentForm.CompanyRoleSelection";
		public const string CRAF_SESSION_SITE_SELECT			= "CompanyRoleAssignmentForm.SiteSelection";
		public const string CRAF_SESSION_FIND_STRING			= "CompanyRoleAssignmentForm.FindString";
		public const string CRAF_SESSION_INCLUDE_MEMBERS		= "CompanyRoleAssignmentForm.IncludeMemberSites";
		public const string CRAF_SESSION_ROLE_DATA_LIST			= "CompanyRoleAssignmentForm.RoleDataList";
		public const string CRAF_SESSION_ROLE_MANAGER_COUNT		= "CompanyRoleAssignmentForm.ManagerCount";
		public const string CRAF_SESSION_ROLE_OWNER_COUNT		= "CompanyRoleAssignmentForm.OwnerCount";
		public const string CRAF_SESSION_ROLE_CARRIER_COUNT		= "CompanyRoleAssignmentForm.CarrierCount";
		public const string CRAF_SESSION_ROLE_SHIPTO_COUNT		= "CompanyRoleAssignmentForm.ShipToCount";
		public const string CRAF_SESSION_ROLE_BILLTO_COUNT		= "CompanyRoleAssignmentForm.BillToCount";
		public const string CRAF_SESSION_ROLE_SHIPPER_COUNT		= "CompanyRoleAssignmentForm.ShipperCount";
		public const string CRAF_SESSION_ROLE_SUPPLIER_COUNT	= "CompanyRoleAssignmentForm.SupplierCount";
		public const string CRAF_SESSION_APPLY_ALL_SETTING		= "CompanyRoleAssignmentForm.ApplyAllSettings";
		public const string CRAF_SESSION_SORT_KEY				= "CompanyRoleAssignmentForm.SortKey";

		// Entity Assignment Form session keys.
		public const string EAF_SESSION_SORT_KEY				= "EntityAssignmentForm.SortKey";
		public const string EAF_SESSION_INCLUDE_MEMBERS			= "EntityAssignmentForm.IncludeMemberSites";
		public const string EAF_SESSION_SITE_SELECT				= "EntityAssignmentForm.SiteSelection";
		public const string EAF_SESSION_ENTITY_SELECT			= "EntityAssignmentForm.EntitySelection";
		public const string EAF_SESSION_ENTITY_TYPE_SELECT		= "EntityAssignmentForm.EntityTypeSelection";
		public const string EAF_SESSION_ENTITY_ENTITY_ENGINE	= "EntityAssignmentForm.EntityEngineType";
		public const string EAF_SESSION_ENTITY_ASSIGNMENTS		= "EntityAssignmentForm.EntityModifiedAssignments";

		// IATA Codes
		public const string IATA_CODE_COLLECTION = "IATACodeCollection";

		// Menu constants
		public const string FM_MENU_DATA = "MenuData";

		// Ledger form session keys.
		public const string LEDGER_MONTH_SELECTION		= "Ledger.MonthSelection";
		public const string LEDGER_MANAGER_SELECTION	= "Ledger.ManagerSelection";
		public const string LEDGER_OWNER_SELECTION		= "Ledger.OwnerSelection";
		public const string LEDGER_PRODUCT_SELECTION	= "Ledger.ProductSelection";
		public const string LEDGER_SHOW_COST_SELECTION	= "Ledger.ShowCostSelection";
		public const string LEDGER_GROSS_NET_SELECTION	= "LedgerGrossNetSelection";
		public const string LEDGER_DATE_TYPE_SELECTION	= "Ledger.DateTypeSelection";
		public const string LEDGER_VOLUME_UNIT_SELECTION = "Ledger.VolumeUnitSelection";

		// Standing Offers (aka Price List) form session keys.
		public const string SOP_SUPPLIER			= "StandingOfferPriceForm.SOP_Supplier";
		public const string SOP_PRODUCT				= "StandingOfferPriceForm.SOP_Product";
		public const string SOP_LOCATION			= "StandingOfferPriceForm.SOP_Location";
		public const string SOP_EFFECTIVE_DATE		= "StandingOfferPriceForm.SOP_EffectiveDate";
		public const string SOP_EFF_END_DATE		= "StandingOfferPriceForm.SOP_EffectiveEndDate";
		public const string SOP_COLLECTION			= "StandingOfferPriceForm.SOP_Collection";
		public const string SOP_REFERENCE_NUMBER	= "StandingOfferPriceForm.SOP_ReferenceNumber";

		// Ledger Aggregate Columns Keys
		public const string LEDGER_AGGREGATE_COLUMN_OBJECT = "LedgerAggregateColumn.Object";

		// Ledger View Keys
		public const string LEDGER_VIEW_OBJECT		= "LedgerViewsForm.Object";
		public const string LEDGER_VIEW_COLLECTION	= "Ledger.ViewCollection";
		public const string LEDGER_VIEW_SELECTION	= "Ledger.ViewSelection";

		// Login values
		public const int MAXIMUM_SESSION_TIMEOUT = 525600;

		// Application BSME Version key
		public const string APPLICATION_IS_BSME_VERSION = "Application.IsBsmeVersion";


		// Inventory Reconciliation Session Keys
		public const string INVENTORY_RECONCILIATION_CONTEXT_KEY = "InventoryReconciliationPage.ContextKey";

		// Tax configuration keys
		public const string TAX_GST_SUMMARY_OBJECT				= "TaxRateGstSummaryForm.GSTEditObject";
		public const string TAX_GST_DETAIL_OBJECT				= "TaxRateGstDetailForm.GSTDetailObject";
		public const string TAX_GST_COMPANIES_LIST				= "TaxRateGstDetailForm.GSTCompaniesList";
		public const string TAX_GST_DELETED_COMPANIES_LIST		= "TaxRateGstDetailForm.GSTDeletedCompaniesList";
		public const string TAX_GST_DETAIL_MODE					= "TaxRateGSTDetailForm.Mode";
		public const string TAX_EXCISE_SUMMARY_OBJECT			= "TaxRateExciseSummaryForm.ExciseEditObject";
		public const string TAX_EXCISE_DETAIL_OBJECT			= "TaxRateExciseDetailForm.ExciseDetailObject";
		public const string TAX_EXCISE_COMPANIES_LIST			= "TaxRateExciseDetailForm.ExciseCompaniesList";
		public const string TAX_EXCISE_DELETED_COMPANIES_LIST	= "TaxRateExciseDetailForm.ExciseDeletedCompaniesList";
		public const string TAX_EXCISE_SUMMARY_PRODUCT_ID		= "TaxRateExciseSummaryForm.ProductID";
		public const string TAX_EXCISE_SUMMARY_STARTDATE		= "TaxRateExciseSummaryForm.StartDate";
		public const string TAX_EXCISE_SUMMARY_ENDDATE			= "TaxRateExciseSummaryForm.EndDate";
		public const string TAX_EXCISE_DETAIL_MODE				= "TaxRateExciseDetailForm.Mode";
		public const string TAX_MARKUP_SUMMARY_OBJECT			= "TaxRateMarkupSummaryForm.MarkupEditObject";
		public const string TAX_MARKUP_DETAIL_OBJECT			= "TaxRateMarkupDetailForm.MarkupDetailObject";
		public const string TAX_MARKUP_COMPANIES_LIST			= "TaxRateMarkupDetailForm.MarkupCompaniesList";
		public const string TAX_MARKUP_DELETED_COMPANIES_LIST	= "TaxRateMarkupDetailForm.MarkupDeletedCompaniesList";
		public const string TAX_MARKUP_DETAIL_MODE				= "TaxRateMarkupDetailForm.Mode";

		// Transaction List Page session keys
		public const string TRANSACTION_LIST_PAGE_TRANS_TYPE_INDEX	= "TransactionList.TransactionTypeIndex";
		public const string TRANSACTION_LIST_PAGE_TRANS_TYPE_LIST	= "TransactionList.TransactionTypeList";
		public const string TRANSACTION_LIST_PAGE_EXPORT_FORMAT		= "TransactionList.ExportFormat";

		// Fuel Card Selection Form page session keys
		public const string FUEL_CARD_SELECTION_PAGE_CONTEXT_ARRAY_LIST = "FuelCardSelectionForm.ContextArrayList";
		public const string FUEL_CARD_SELECTION_PAGE_SELECT_CONTEXT		= "FuelCardSelectionForm.SelectContext";

		// Fuel Card Session Keys
		public const string FUEL_CARD_ARRAY_LIST = "FuelCardArrayList";

        // AssetDevice Session Keys
        public const string ASSET_TRACKING_DEVICE_IDENTITY_GUID = "AssetTrackingDeviceSummaryForm.IdentityGuid";

        // Accounting General form page session keys
        public const string ACCOUNTING_GENERAL_PAGE_DATA_OBJECT = "GeneralConfiguration.DataObject";

		// Auto Distribution
		public const string AutoDistributionRule						= "AutoDistributionRule";
		public const string AutoDistributionRulesFormPageName			= "AutoDistributionRulesForm";
		public const string AutoDistributionRulesFormDataList			= AutoDistributionRulesFormPageName + "List";
		public const string AutoDistributionRulesFormPageIndex			= AutoDistributionRulesFormPageName + "PageIndex";
		public const string AutoDistributionRulesFormFindString			= AutoDistributionRulesFormPageName + ".SortExpression";
		public const string AutoDistributionRulesFormSortDisplayName	= AutoDistributionRulesFormPageName + ".SortExpression";
		public const string AutoDistributionRulesFormIsSortAscending	= AutoDistributionRulesFormPageName + ".SortDirection";
		public const string AutoDistributionOperationPageName			= "AutoDistributionOperation";
		public const string AutoDistributionOperationDistributionList	= AutoDistributionOperationPageName + "DistributionList";
		public const string AutoDistributionOperationRuleList			= AutoDistributionOperationPageName + "RuleList";
		public const string AutoDistributionOperationRuleGuid			= AutoDistributionOperationPageName + "RuleGuid";
		public const string AutoDistributionOperationSortExpression		= AutoDistributionOperationPageName + "SortExpression";
		public const string AutoDistributionOperationSortDirection		= AutoDistributionOperationPageName + "SortDirection";
		public const string AutoDistributionOperationWarningMsg			= AutoDistributionOperationPageName + "OutOfBalanceWarning";
		public const string AutoDistributionOperationBalanceFlag		= AutoDistributionOperationPageName + "VolumeBalanceFlag";


		// IntoPlane Profile Configuration
		public const string ProfileConfigurationItemToEdit    = "ProfileConfigSummaryForm.ItemToEdit";
		public const string ProfileConfigurationProfileObject = "ProfileConfigSummaryForm.MobileDeviceProfileObject";
		public const string ProfileConfigurationFindString    = "ProfileConfigSummaryForm.FindString";


        // Field Level Configuration session keys.
        public const string FLC_SESSION_DATA_MATRIX							= "FieldLevelConfigForm.FLCMatrix";
        public const string FLC_SESSION_ENTITY_TYPE_SELECT					= "FieldLevelConfigForm.EntityTypeSelection";
        public const string FLC_SESSION_SITE_GROUP_SELECT					= "FieldLevelConfigForm.SiteGroupSelection";
        public const string FLC_SESSION_FILTER_SELECT						= "FieldLevelConfigForm.FilterSelection";
        public const string FLC_SESSION_FILTER_VALUE_SELECT					= "FieldLevelConfigForm.FilterValueSelection";
        public const string FLC_SESSION_TARGET_FIELD_SELECT					= "FieldLevelConfigForm.TargetFieldSelection";
        public const string FLC_SESSION_CONTROL_MODE_SELECT					= "FieldLevelConfigForm.ControlModeSelection";
        public const string FLC_SESSION_INCLUDE_MEMBER_SITEGROUPS_SELECT	= "FieldLevelConfigForm.IncludeMemberSitegroupsSelection";
        public const string FLC_SESSION_SORT_KEY							= "FieldLevelConfigForm.SortKey";        

		// Mobile Device Configuration
		public const string MobileDeviceConfigurationItemToEdit	= "MobileDeviceConfigSummaryPage.ItemToEdit";
		public const string MobileDeviceConfigurationObject		= "MobileDeviceConfigurationPage.AdcDeviceObject";
		public const string MobileDeviceConfigurationFindString	= "MobileDeviceConfigSummaryPage.FindString";

		// User Permission Assignment Configuration
		public const string UGAF_SESSION_SITE_SELECT		= "UserPermissionAssignmentForm.SiteSelection";
		public const string UGAF_SESSION_SITEGROUP_SELECT	= "UserPermissionAssignmentForm.SiteGroupSelection";
		public const string UGAF_SESSION_USER_SELECT		= "UserPermissionAssignmentForm.UserSelection";

		// Transaction Error Summary Form page session keys
		public const string TRANS_ERROR_SUMMARY_START_DATE		= "TransactionErrorSummaryForm.StartDate";
		public const string TRANS_ERROR_SUMMARY_END_DATE		= "TransactionErrorSummaryForm.EndDate";
		public const string TRANS_ERROR_SUMMARY_USE_DATE_FILTER = "TransactionErrorSummaryForm.UseDateFilter";
		public const string TRANS_ERROR_SUMMARY_SELECTED_SITE	= "TransactionErrorSummaryForm.SelectedSite";
		public const string TRANS_ERROR_SUMMARY_SORT_DIRECTION	= "TransactionErrorSummaryForm.SortDirection";
		public const string TRANS_ERROR_SUMMARY_SORT_EXPRESSION	= "TransactionErrorSummaryForm.SortExpression";
		public const string TRANS_ERROR_SUMMARY_STATUS_CODE		= "TransactionErrorSummaryForm.StatusCode";
		public const string TRANS_ERROR_EXPORT_FORMAT			= "TransactionErrorSummaryForm.ExportFormat";

		// unacknowledged transaction summary page
		public const string TRANS_UNACK_SUMMARY_START_DATE		= "UnacknowledgedTransactionSummary.StartDate";
		public const string TRANS_UNACK_SUMMARY_END_DATE		= "UnacknowledgedTransactionSummary.EndDate";
		public const string TRANS_UNACK_SUMMARY_USE_DATE_FILTER = "UnacknowledgedTransactionSummary.UseDateFilter";
		public const string TRANS_UNACK_SUMMARY_SELECTED_SITE	= "UnacknowledgedTransactionSummary.SelectedSite";
		public const string TRANS_UNACK_SUMMARY_SORT_DIRECTION	= "UnacknowledgedTransactionSummary.SortDirection";
		public const string TRANS_UNACK_SUMMARY_SORT_EXPRESSION = "UnacknowledgedTransactionSummary.SortExpression";

		// Transaction History Popup page session keys
		public const string TRANS_HISTORY_POPUP_START_DATE		= "TransactionHistoryPopup.StartDate";
		public const string TRANS_HISTORY_POPUP_END_DATE		= "TransactionHistoryPopup.EndDate";
		public const string TRANS_HISTORY_POPUP_USE_DATE_FILTER = "TransactionHistoryPopup.UseDateFilter";
		public const string TRANS_HISTORY_POPUP_SORT_DIRECTION	= "TransactionHistoryPopup.SortDirection";
		public const string TRANS_HISTORY_POPUP_SORT_EXPRESSION = "TransactionHistoryPopup.SortExpression";

        // Transaction Alias page session keys
        public const string TRANS_ALIAS_VERSION_SPECIFIC_FIELDS = "TransactionAlias.VersionSpecificFields";

        // Synchronization page session keys
        public const string SYNC_CONFIG_CLIENT_SETTINGS				= "SynchronizationConfiguration.ClientSettings";
        public const string SYNC_CONFIG_SERVER_SETTINGS				= "SynchronizationConfiguration.ServerSettings";
        public const string SYNC_CONFIG_SITE_SETTINGS					= "SynchronizationConfiguration.SiteSettings";
        public const string SYNC_CONFIG_SITE_SETTINGS_MODIFIED		= "SynchronizationConfiguration.SiteSettings.Modified";
        public const string SYNC_ONLINE_SERVICE_STATE					= "OnlineSynchronization.ServiceState";
        public const string SYNC_OFFLINE_SERVICE_STATE				= "OfflineSynchronization.ServiceState";
		public const string SYNC_DATA_STORE_ID						= "SynchronizationConfiguration.DataStoreID";
		public const string SYNC_DATA_STORE_NAME					= "SynchronizationConfiguration.DataStoreName";

	    public const string SYNC_WINSERVICE_BINDING_TYPE				= "SyncServiceBindingType";
        public const string SYNC_WINSERVICE_BINDING_CONFIGURATION		= "SyncServiceBindingConfiguration";
        public const string SYNC_WINSERVICE_BINDING_END_POINT_ADDRESS	= "SyncServiceBindingEndPointAddress";

        public const string SYNC_MANUAL_REQUEST_TYPE			= "ManualSyncRequestType";
        public const string SYNC_MANUAL_AUTOREFRESH_INTERVAL	= "ManualSyncAutoRefreshInterval";
        public const string SYNC_MANUAL_SELECTED_SITEID			= "ManualSyncSelectedSiteID";

        public const string EOM_APPROVAL_REPORT_CHECKSUM = "EndOfMonthApprovalReportChecksum";
	}
}
