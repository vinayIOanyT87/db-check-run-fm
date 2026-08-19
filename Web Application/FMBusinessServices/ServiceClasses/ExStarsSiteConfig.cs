
namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessServices.DataAccessLayer;

	/// <summary>
	/// Helper class to get the configured identification values from the database based on manager and site.  
	/// These are used to send reports to IRS-ExSTARS.
	/// </summary>
	public class ExStarsSiteConfig : /*FMServiceBase, */ ExStarsUniversalConfig, IExStarsSiteConfig
	{
		#region Constants and Fields

		private readonly ConsolidatedDAClass consolidatedDa = new ConsolidatedDAClass();

#if false
		public SecurityClass Security { get; protected set; }
		public string DunsNumber { get; protected set; }
		public string InterchangeControlVersion { get; protected set; }
		public string GS03_ApplicationReceiversCode { get; protected set; }
		public string ISA12_InterchangeControlVersion { get; protected set; }
		public string GS08_FuncGrpHdrVerReleaseIndustryIdCode { get; protected set; }
		public string ISA05Qualifier { get; protected set; }
		public int EnableDebugFeatures { get; protected set; }

		private readonly ConfigurationSettingsClass universalConfig;

#endif

		#endregion

		public ExStarsSiteConfig( SecurityClass security) : base(security)
		{

			//this.Security = security;
			//this.universalConfig = new ConfigurationSettingsClass();
			//this.DunsNumber =						this.GetString( ConfigurationSettingDOClass.Key_IrsExStars_DunsNumber_ISA08);
			//this.InterchangeControlVersion =		this.GetString( ConfigurationSettingDOClass.Key_IrsExStars_InterchangeControlVersion_ISA12);
			//this.GS03_ApplicationReceiversCode =	this.GetString( ConfigurationSettingDOClass.Key_IrsExStars_ApplicationReceiversCode_GS03);
			//this.GS08_FuncGrpHdrVerReleaseIndustryIdCode = this.GetString(ConfigurationSettingDOClass.Key_IrsExStars_FuncGrpHdrVerReleaseIndIdCode_GS08);
			//this.ISA12_InterchangeControlVersion =	this.GetString(ConfigurationSettingDOClass.Key_IrsExStars_InterchangeControlVersion_ISA12);
			//this.ISA05Qualifier =					this.GetString( ConfigurationSettingDOClass.Key_IrsExStars_ISA05Qualifier);
			//this.EnableDebugFeatures =				int.Parse( this.GetString(ConfigurationSettingDOClass.Key_IrsExStars_EnableDebugFeatures));
		}

		//protected string GetString(string key)
		//{
		//	ConfigurationSettingDOClass keyedValue = universalConfig.GetByKey(this.Security, key);
		//	return keyedValue.SettingValue;
		//}


		public ExStarsSiteConfigClass GetIrsSpecifiedIds(SecurityClass security, bool isTest, ref Guid managerCompanyGuid, ref Guid siteGuid)
		{
			this.Security = security;
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}
			if (!isTest)
			{		
				if (managerCompanyGuid == null)
				{
					throw new ArgumentNullException("managerCompanyGuid");
				}

				if (siteGuid == null)
				{
					throw new ArgumentNullException("siteGuid");
				}
			}

			// If a user has the authority to view ExSTARS reports, it is presumed they have the right to see the
			// company data on that report.
			if (!security.HasRight(RIGHT.VIEW_IRS_EXSTARS_REPORT))
			{
				throw new FMInsufficientRightsException();
			}

			ExStarsSiteConfigClass siteConfigData = new ExStarsSiteConfigClass();
			
			var consolidatedDA = new ConsolidatedDAClass();
			using (SqlCommand cmd = new SqlCommand())
			{
				try
				{
					siteConfigData.GetExStarsConfigSql(cmd, managerCompanyGuid, siteGuid);
					DataSet dataSet = this.consolidatedDa.GetDataSet(cmd, security);

					if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
					{
						throw new ExStarsBusinessException("No ExSTARS configuration has been created for the selected company and site ( {0}, {1}", managerCompanyGuid.ToString(), siteGuid.ToString());
					}
					DataTable table = dataSet.Tables[0];
					LoadResults(table, siteConfigData);
					if (isTest)
					{
						managerCompanyGuid = siteConfigData.ManagerCompanyGuid;
						siteGuid = siteConfigData.SiteGuid;
					}
				}
				catch (ExStarsBusinessException)
				{
					// pass the exception up
					throw;
				}
				catch (Exception e)
				{
					throw new ExStarsSqlException(e, "SQL error: {0}", cmd.CommandText);
				}
			}

			return siteConfigData;
		}

		private static void LoadResults(DataTable table, ExStarsSiteConfigClass configData)
		{
			DataRow row = table.Rows[0];
			configData.SiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty);
			configData.ManagerCompanyGuid = DataObject.getValue(row["ManagerCompanyGuid"], Guid.Empty);
			configData.InterchangeSenderId = DataObject.getValue(row["InterchangeSenderId"], string.Empty).Trim();
			configData.ApplicationSendersCode = DataObject.getValue(row["ApplicationSendersCode"], string.Empty).Trim();
			configData.AuthorizationCode = DataObject.getValue(row["AuthorizationCode"], string.Empty).Trim();
			configData.FeinCode = DataObject.getValue(row["FeinCode"], string.Empty).Trim();
			configData.SecurityCode = DataObject.getValue(row["SecurityCode"], string.Empty).Trim();
			configData.InfoProviderName = DataObject.getValue(row["InfoProviderName"], string.Empty).Trim();
			configData.AbbreviatedProviderName = DataObject.getValue(row["AbbreviatedProviderName"], string.Empty).Trim();
			configData.InterchangeControlNumber = DataObject.getValue(row["GroupControlNumber"], string.Empty).Trim();
			configData.IRS_637Registration = DataObject.getValue(row["IRS_637Registration"], string.Empty).Trim();
			configData.TerminalControlNumber = DataObject.getValue(row["TerminalControlNumber"], string.Empty).Trim();
		}
	}
}