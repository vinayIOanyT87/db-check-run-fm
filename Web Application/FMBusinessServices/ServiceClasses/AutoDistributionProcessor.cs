namespace FMBusinessServices.ServiceClasses
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.ServiceModel;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.ServiceRequests;
    using FMBusinessObjects.UtilityObjects;

    using FMBusinessServices.DataAccessLayer;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    using IsolationLevel = System.Transactions.IsolationLevel;

    /// <summary>
	/// This is for the AutoDistribion Operation Page. This calculates the thruput 
	/// and prepare the helper class which calculates the quantities and percents.
	/// </summary>
	[ServiceBehavior(TransactionIsolationLevel = IsolationLevel.ReadCommitted)]
	public class AutoDistributionProcessor : IAutoDistributionProcessor
	{
		private const string MessageInvalidSecurity = "Invalid Security";
		private const string MessageRuleNotFound = "Rule Not Found";
		private const string MessageUnexpectedError = "No DataTable returned";
		private const string MessageNoOwnerFound = "No owner found for this rule/maanger/product combination.";

		/// <summary>
		/// This method is used to calculate the thruputs from the given service request data.
		/// </summary>
		/// <param name="mySecurity">Security object used by FM</param>
		/// <param name="requestData">Request Info</param>
		/// <returns>Thruputs for each owner</returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public DataTable CalculateThruput(SecurityClass mySecurity, AutoDistributionThruputSR requestData)
		{
			Validate(mySecurity, requestData.RuleGuid);


			AutoDistributionThruputSqlInfo sqlInfo = new AutoDistributionThruputSqlInfo()
				{
					RuleGuid = requestData.RuleGuid,
					ManagerGuid = requestData.ManagerGuid,
					ProductGuid = requestData.ProductGuid,
					StartDate = requestData.StartDate,
					EndDate = requestData.EndDate
				};

			this.GatherProductUnits(mySecurity, sqlInfo);

			DataTable resultTable;
			using (SqlCommand cmd = new SqlCommand())
			{
				AutoDistributionProcessorDAC.PrepareThruputSqlCommand(cmd, mySecurity, sqlInfo);

				ConsolidatedDAClass consolidatedDA = new ConsolidatedDAClass();

				resultTable = consolidatedDA.GetDataTable(cmd, mySecurity);
			}

			if (resultTable == null)
			{
				throw new ApplicationException(MessageUnexpectedError);
			}

			if (resultTable.Rows.Count == 0)
			{
				throw new ApplicationException(MessageNoOwnerFound);
			}

			return resultTable;
		}

		/// <summary>
		/// Create the Helper object from the given informaiont
		/// </summary>
		/// <param name="mySecurity">Security object used by FM</param>
		/// <param name="siteGuid">Site Guid</param>
		/// <param name="aliasGuid">Transaciton Alias Guid</param>
		/// <param name="productGuid">Product Guid</param>
		/// <returns></returns>
		public AutoDistributionOperationHelper PrepareHelper(SecurityClass mySecurity, Guid siteGuid, Guid aliasGuid, Guid productGuid)
		{

			var siteService = new SitesClass();
			SiteClass site = siteService.Get(mySecurity, siteGuid, false);

			TransactionAliasesClass aliasService = new TransactionAliasesClass();
			TransactionAliasClass transactionAlias = aliasService.Get(mySecurity, aliasGuid, false);

			ProductsClass productService = new ProductsClass();
			ProductClass product = productService.Get(mySecurity, productGuid);

			// Get default number format information from site
			NumberFormatInfo volumeTrxNumberFormat = site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME);
			NumberFormatInfo massTrxNumberFormat = site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME);
			NumberFormatInfo volumeProductNumberFormat = site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.MASS);
			NumberFormatInfo massProductNumberFormat = site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.MASS);
			NumberFormatInfo percentNumberFormat = site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT);


			// get specific transaction decimal places information
			UnitsHelperClass unitsHelper = new UnitsHelperClass(mySecurity, site, transactionAlias, product);
			volumeTrxNumberFormat.NumberDecimalDigits = unitsHelper.VolumeDecimalPlaces;
			massTrxNumberFormat.NumberDecimalDigits = unitsHelper.MassDecimalPlaces;
			
			// get specific product decimal places information
			unitsHelper = new UnitsHelperClass(mySecurity, site, null, product);
			volumeProductNumberFormat.NumberDecimalDigits = unitsHelper.VolumeDecimalPlaces;
			massProductNumberFormat.NumberDecimalDigits = unitsHelper.MassDecimalPlaces;

			AutoDistributionOperationHelper newHelper = new AutoDistributionOperationHelper(
					volumeTrxNumberFormat,
					massTrxNumberFormat,
					volumeProductNumberFormat,
					massProductNumberFormat,
					percentNumberFormat
				);

			return newHelper;
		}

		/// <summary>
		/// This gather conversion factors and decimal places and stores the info in the AutoDistributionThruputSqlInfo structure.
		/// </summary>
		/// <param name="mySecurity">Security object used by FM</param>
		/// <param name="sqlInfo">AutoDistributionThruputSqlInfo to be populated</param>
		public void GatherProductUnits(SecurityClass mySecurity, AutoDistributionThruputSqlInfo sqlInfo)
		{
			SitesClass sites = new SitesClass();
			SiteClass currentSite = sites.Get(mySecurity, mySecurity.SiteGuid, false);

			// Use the product conversion factor and precision if the product is configured to
			// have them.
			ProductsClass products = new ProductsClass();
			ProductClass product = products.Get(mySecurity, sqlInfo.ProductGuid);

			if (product.VolumeUnits > 0)
			{
				sqlInfo.VolumeConversionFactor = GetConversionFactor(product.VolumeUnits, product.VolumeDecimalPlaces);
				sqlInfo.VolumeDecimalPlaces = Convert.ToDouble(product.VolumeDecimalPlaces);
			}
			else
			{
				if (product.ProductType == ProductType.AdditiveProduct)
				{
					sqlInfo.VolumeConversionFactor = 
								GetConversionFactor(currentSite.AdditiveVolumeUnits, Convert.ToInt32(currentSite.AdditiveVolumeDecimalPlaces));
					sqlInfo.VolumeDecimalPlaces = Convert.ToDouble(currentSite.AdditiveVolumeDecimalPlaces);
				}
				else
				{
					sqlInfo.VolumeConversionFactor = GetConversionFactor(currentSite.VolumeUnits, Convert.ToInt32(currentSite.VolumeDecimalPlaces));
					sqlInfo.VolumeDecimalPlaces = Convert.ToDouble(currentSite.VolumeDecimalPlaces);
				}
			}

			if (product.MassUnits > 0)
			{
				sqlInfo.MassConversionFactor = GetConversionFactor(product.MassUnits, product.MassDecimalPlaces);
				sqlInfo.MassDecimalPlaces = Convert.ToDouble(product.MassDecimalPlaces);
			}
			else
			{
				sqlInfo.MassConversionFactor = GetConversionFactor(currentSite.MassUnits, Convert.ToInt32(currentSite.MassDecimalPlaces));
				sqlInfo.MassDecimalPlaces = Convert.ToDouble(currentSite.MassDecimalPlaces);
			}
		}

		/// <summary>
		/// This method will calculate the correction factor.
		/// </summary>
		/// <param name="unit">Engineering unit</param>
		/// <param name="decimalPlace">Decimal place to calculate.</param>
		/// <returns>Returns a Correction Factor.</returns>
		private static double GetConversionFactor(EngineeringUnit unit, int decimalPlace)
		{
			const double ConvertFactor = 1;

			var siDouble = new SIDouble
								{
									Units = unit,
									numberDecimalDigits = Convert.ToInt32(decimalPlace),
									SIValue = ConvertFactor
								};

			return siDouble.Value;
		}

		/// <summary>
		/// Validate SecurityClass object
		/// </summary>
		/// <param name="mySecurity">Security object used by FM</param>
		private static void Validate(SecurityClass mySecurity)
		{
			if (mySecurity == null)
			{
				throw new ArgumentNullException(MessageInvalidSecurity);
			}
		}

		/// <summary>
		/// Validate Rule object
		/// </summary>
		/// <param name="mySecurity">Security object used by FM</param>
		/// <param name="ruleGuid">Guid of the rule to be validated</param>
		private static void Validate(SecurityClass mySecurity, Guid ruleGuid)
		{
			Validate(mySecurity);

			AutoDistributionRules ruleService = new AutoDistributionRules();
			AutoDistributionRuleDO ruleDO = ruleService.Get(mySecurity, ruleGuid);
			if (ruleDO == null)
			{
				throw new ArgumentException(MessageRuleNotFound);
			}
		}

	}
}