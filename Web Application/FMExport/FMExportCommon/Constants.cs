// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Constants.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Common constants used by the FMExport service and custom aviation interfaces.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMExportService
{
	/// <summary>
	/// Common constants used by the FMExport service and custom aviation interfaces.
	/// </summary>
	public class Constants
	{
		/// <summary>
		/// Indicates liter unit of measurement
		/// </summary>
		public const string LiterUom = "LT";

		/// <summary>
		/// Indicates all customers
		/// </summary>
		public static readonly string AllCustomers = "<All>";

        public const string NO_LEGACY_MAPPING_VALUE = "**NO_MAPPING**";
        public const string LEGACY_FMAE_TRANSLATIONS_CONFIGURATION_FILE_SECTION_NAME = "FMAECompanyTranslations";
        public const string SITELIST_CONFIGURATION_FILE_SECTION_NAME = "SiteList";
        public const string ALIASLIST_CONFIGURATION_FILE_SECTION_NAME = "AliasList";
        public const string PRODUCTLIST_CONFIGURATION_FILE_SECTION_NAME = "ProductList";
        public const string ALL_CUSTOMERS = "<All>";

        public const string WEBSERVICE_PLUGIN_FOLDER = "WebServicePlugins";
    }
}
