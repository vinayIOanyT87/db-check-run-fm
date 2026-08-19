namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Data;
	using System.Collections.Generic;
	using System.Linq;
	using System.Data.SqlClient;
	using System.Text;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
    using FMBusinessObjects.UtilityObjects;
	using FMBusinessServices.DataAccessLayer;

	
	/// <summary>
	/// This class expands ExStarsSiteConfig by looking up the Guids and getting the whole associated row
	/// </summary>
	public class ExStarsSiteConfigExpanded : ExStarsSiteConfigClass
	{
		// ReSharper disable InconsistentNaming

		#region Properties

		public SiteClass Site { get; protected set; }

		public CompanyClass Manager { get; protected set; }

		public SecurityClass Security { get; protected set; }

		public ProductCollectionClass IrsProductsByProductGuid { get; protected set; }

		public ProductByTaxCodeCollectionClass IrsProductsByTaxCode { get; protected set; }

		public int SiteVolumePrecision
		{
			get
			{
				return int.Parse(Site.VolumeDecimalPlaces);
			}
		}

		public DateTime StartTransactionDateTime { get; protected set; }

		public DateTime EndTransactionDateTime { get; protected set; }

		public DateTime ReportDateTime { get; protected set; }

		public int SequenceNumber { get; protected set; }

		public ReportTypeEnum ReportType { get; protected set; }

		public ReportModifiersEnum ReportModifier { get; set; }

		public bool ForceOverwrite { get; protected set; }

		public VersionInfo Version { get; protected set; } 


		public string DunsNumber
		{
			get
			{
				return this.HelperToGetIrsSpecifiedIds.DunsNumber;
			}
		}

		public string InterchangeControlVersion
		{
			get
			{
				return this.HelperToGetIrsSpecifiedIds.InterchangeControlVersion;
			}
		}

		public string GS03_ApplicationReceiversCode
		{
			get
			{
				return this.HelperToGetIrsSpecifiedIds.GS03_ApplicationReceiversCode;
			}
		}

		public string ISA12_InterchangeControlVersion
		{
			get
			{
				return this.HelperToGetIrsSpecifiedIds.ISA12_InterchangeControlVersion;
			}
		}

		public string GS08_FuncGrpHdrVerReleaseIndustryIdCode
		{
			get
			{
				return this.HelperToGetIrsSpecifiedIds.GS08_FuncGrpHdrVerReleaseIndustryIdCode;
			}
		}

		public string ISA05Qualifier
		{
			get
			{
				return this.HelperToGetIrsSpecifiedIds.ISA05Qualifier;
			}
		}

		public int EnableDebugFeatures
		{
			get
			{
				return this.HelperToGetIrsSpecifiedIds.EnableDebugFeatures;
			}
		}



		public bool IsTest { get; protected set; }

		public bool HasErrors { get; protected set; }

		public TransactionAliasCollectionClass SecureAliasList { get; protected set; }

		public CompanyCollectionClass AllCompanies { get; protected set; }

		public ExStarsErrorsAndWarningsList ErrorsAndWarningsesAndWarnings { get; protected set; }




		#endregion

		#region Private and Protected Variables

		protected Dictionary<string, ExStarsTransportMode> TransportationModesByName = new Dictionary<string, ExStarsTransportMode>();
		
		protected Dictionary<string, ExStarsTransportMode> TransportationModesByIrsMode = new Dictionary<string, ExStarsTransportMode>();

		protected ExStarsSiteConfig HelperToGetIrsSpecifiedIds;

		private PureSegmentList acknowledgement;

		private int[] sequenceNumsInAck = null;

		#endregion

		#region Constructors

		public ExStarsSiteConfigExpanded(SecurityClass security
		                                 , ref Guid managerCompanyGuid
		                                 , ref Guid siteGuid
		                                 , bool isTest
		                                 , DateTime startTransactionDateTime
		                                 , DateTime endTransactionDateTime
		                                 , DateTime reportDateTime
		                                 , ReportTypeEnum reportType
		                                 , ReportModifiersEnum reportModifier)
			: this(security
				, ref managerCompanyGuid
				, ref siteGuid
				, reportType
				, isTest
				, false)
		{
			this.StartTransactionDateTime = startTransactionDateTime;
			this.EndTransactionDateTime = endTransactionDateTime;
			this.ReportDateTime = reportDateTime;
			this.ReportModifier = reportModifier;
		}

		public ExStarsSiteConfigExpanded(SecurityClass security
		                                 , ref Guid managerCompanyGuid
		                                 , ref Guid siteGuid
		                                 , ReportTypeEnum reportType
		                                 , bool isTest
		                                 , bool forceOverwrite) 
			:this( security, ref managerCompanyGuid, ref siteGuid)
		{
			this.ReportType = reportType;
			this.IsTest = isTest;
			this.ForceOverwrite = forceOverwrite;
		}

		public ExStarsSiteConfigExpanded(SecurityClass security
		                                 , ref Guid managerCompanyGuid
		                                 , ref Guid siteGuid
		                                 , DateTime startTransactionDateTime
		                                 , DateTime endTransactionDateTime)
			: this(security, ref managerCompanyGuid, ref siteGuid)
		{
			this.StartTransactionDateTime = startTransactionDateTime;
			this.EndTransactionDateTime = endTransactionDateTime;
		}


		public ExStarsSiteConfigExpanded(SecurityClass security
		                                 , ref Guid managerCompanyGuid
		                                 , ref Guid siteGuid)
		{
			this.HelperToGetIrsSpecifiedIds = new ExStarsSiteConfig(security);
			Version = ConsolidatedDAClass.GetVersion();
			//
			// Set default values for configuration values that are not used with 151 Acknowledgement files
			//
			this.StartTransactionDateTime = ExStarsConstants.BeginningOfDateTime;
			this.EndTransactionDateTime = ExStarsConstants.BeginningOfDateTime;
			this.ReportDateTime = ExStarsConstants.BeginningOfDateTime;
			this.ReportModifier = ReportModifiersEnum.Undefined;
			this.sequenceNumsInAck = new int[] { -1 };
			this.acknowledgement = new PureSegmentList();
			this.Security = security;
	

			ExStarsSiteConfigClass subClass = this.HelperToGetIrsSpecifiedIds.GetIrsSpecifiedIds(security, false, ref managerCompanyGuid, ref siteGuid);
			this.CopyFrom(subClass);
			TransactionAliasesClass aliases = new TransactionAliasesClass();
			this.SecureAliasList = aliases.Enumerate(this.Security);

			this.SequenceNumber = 0;
			this.ErrorsAndWarningsesAndWarnings = new ExStarsErrorsAndWarningsList();
			this.HasErrors = false;
#if false // not needed for upload /download / history
			this.universalConfig = new ConfigurationSettingsClass();
			this.DunsNumber =						this.GetString( ConfigurationSettingDOClass.Key_IrsExStars_DunsNumber_ISA08);
			this.InterchangeControlVersion =		this.GetString( ConfigurationSettingDOClass.Key_IrsExStars_InterchangeControlVersion_ISA12);
			this.GS03_ApplicationReceiversCode =	this.GetString( ConfigurationSettingDOClass.Key_IrsExStars_ApplicationReceiversCode_GS03);
			this.GS08_FuncGrpHdrVerReleaseIndustryIdCode = this.GetString(ConfigurationSettingDOClass.Key_IrsExStars_FuncGrpHdrVerReleaseIndIdCode_GS08);
			this.ISA12_InterchangeControlVersion =	this.GetString(ConfigurationSettingDOClass.Key_IrsExStars_InterchangeControlVersion_ISA12);
			this.ISA05Qualifier =					this.GetString( ConfigurationSettingDOClass.Key_IrsExStars_ISA05Qualifier);
			this.EnableDebugFeatures =				int.Parse( this.GetString(ConfigurationSettingDOClass.Key_IrsExStars_EnableDebugFeatures)));
#endif
			SitesClass sitesClass = new SitesClass();
			this.Site = sitesClass.Get(security, this.SiteGuid, false, false, false);
			this.LoadTransportationModes();
			this.LoadAllCompanies();
			ValidateSite();
			ValidateManager();
			this.LoadIrsProductsByProductGuid();
			this.BasicValidation();
		}

		#endregion

		#region public methods

		public string BaseFileName()
		{
			return BaseFileName(
				this.Manager.ID
				, this.EndTransactionDateTime
				, this.ReportType
				, this.TransSetControlNumber
				, this.IsTest
				, ReportModifierCode(this.ReportModifier));
		}

		public static string BaseFileName( 
			string managerId
			, DateTime endTransDate
			, ReportTypeEnum reportType
			, string transSetControlNumber
			, bool isTest
			, string reportModifierAsStr)
		{
				return string.Format(
					"{0}_{1}_{2}_ExSTARS_{3}_{4}_{5}.{6}"
					, managerId.Replace(" ", "")
					, endTransDate.ToString("yyyyMMdd")
					, reportType
					, transSetControlNumber
					, isTest ? "T" : "P"
					, reportModifierAsStr
					, "{0}");
		}

		public string ReportModifierCode()
		{
			return ReportModifierCode(this.ReportModifier);
		}

		public static string ReportModifierCode( ReportModifiersEnum modifier)
		{
			// CExSTARS_ExportDlg::OnExport()  ~ 676
			switch (modifier)
			{
				case ReportModifiersEnum.Original:
					return ExStarsConstants.BTI13_Original;
				case ReportModifiersEnum.Supplemental:
					return ExStarsConstants.BTI14_Supplemental;
				case ReportModifiersEnum.Correction:
					return ExStarsConstants.BTI14_Corrected;
				case ReportModifiersEnum.Replacement:
					return ExStarsConstants.BTI14_Replacement;
				default:
					//return "";
					throw new ExStarsSiteConfigException("ExStarsSiteConfigExpanded.ReportModifierCode() switch({0})", modifier);
			}
		}
		#endregion

		#region Error Reporting

		public void AppendError(ExStarsErrorSource source, string fmt, params object[] args)
		{
			this.HasErrors = true;
			AppendWarning(source, fmt, args);
		}

		public void AppendWarning(ExStarsErrorSource source, string fmt, params object[] args)
		{
			string header = string.Format("{0,-12}{1,6} ", source.ToString(), this.ErrorsAndWarningsesAndWarnings.Count + 1);
			string msg = header + String.Format(fmt, args);
			this.ErrorsAndWarningsesAndWarnings.Add(msg, msg);
		}

		public void AppendInfoMsg(string fmt, params object[] args)
		{
			string msg = String.Format(fmt, args);
			this.ErrorsAndWarningsesAndWarnings.Add(msg, msg);
		}

		public string ErrorsAndWarningsReport()
		{
			StringBuilder allError = new StringBuilder(300 * this.ErrorsAndWarningsesAndWarnings.Count);
			foreach (var msg in this.ErrorsAndWarningsesAndWarnings)
			{
				allError.Append(msg.Value);
				allError.AppendLine();
			}
			return allError.ToString();
		}


		#endregion

		#region Load internal tables

		/// <summary>
		/// Create a look-up table keyed by the EquipmentTypeId example: pipeline
		/// so that it can return the IRS code example: PL
		/// </summary>
		protected void LoadTransportationModes()
		{
			// ExSTARS IRS Transportation Modes FD-Publ 3536-Motor Fuel Excise Tax EDI Guide-09	Rev 11-2005, page 14
			// All values should be upper case and separated by "="
			var consolidatedDA = new ConsolidatedDAClass();
			string sql = "SELECT SettingValue FROM [dbo].[tblConfigurationSetting] WHERE SettingKey ='IrsExStarsIrsTransportModes'";
			using (var cmd = new SqlCommand(sql))
			{
				cmd.CommandType = CommandType.Text;
				DataSet dataSet = consolidatedDA.GetDataSet(cmd, this.Security);
				if (dataSet.Tables.Count == 0)
				{
					this.AppendError(ExStarsErrorSource.Config, "IrsExStarsIrsTransportModes is not configured in table tblConfigurationSetting");
					return;
					//throw new ExStarsBusinessException("IrsExStarsIrsTransportModes is not configured in table tblConfigurationSetting");
				}
				DataTable table = dataSet.Tables[0];
				foreach (DataRow row in table.Rows)
				{
					string rowValue = DataObject.getValue(row["SettingValue"], "");
					ExStarsTransportMode transport = new ExStarsTransportMode(rowValue);
					this.TransportationModesByName.Add(transport.Name, transport);
					// there is not a one-on-one relationship of IrsModeCode to equipment type
					// this is intended to work with "RS", but otherwise it takes the first offered
					if (!TransportationModesByIrsMode.ContainsKey(transport.IrsModeCode))
					{
						this.TransportationModesByIrsMode.Add(transport.IrsModeCode, transport);
					}
				}
			}
		}

		private void LoadIrsProductsByProductGuid()
		{
			this.IrsProductsByProductGuid = new ProductCollectionClass();
			// look up and process all products for the current site
			ProductsClass products = new ProductsClass();
			ProductCollectionClass siteProducts = products.Enumerate2(this.Security, this.Site.SiteGuid);

			// iterate through all products for the manager
			// to the IRS point of view products are unique based on TaxCode, but there may my multiple 
			// rows with tblProducts with the same ProductID, SiteGuid and TaxCode, but different ProductGuid
			foreach (ProductClass product in siteProducts)
			{
				if (!(product.SiteGuid == this.Site.SiteGuid))
				{
					continue;
				}
				if (!product.TrackedByIrs)
				{
					this.AppendWarning(ExStarsErrorSource.Config, "Warning: product {0} will not be reported because no IRS Mode has been assigned", product.ID);
					continue;
				}
				this.IrsProductsByProductGuid.Add(product);
			}
		}

		private void LoadAllCompanies()
		{
			CompaniesClass companies = new CompaniesClass();

			this.AllCompanies = new CompanyCollectionClass();
			SortedList<string, string> sortedCompanies = new SortedList<string, string>();
			foreach (var role in new COMPANY_ROLE[] { COMPANY_ROLE.MANAGER, COMPANY_ROLE.OWNER, COMPANY_ROLE.SHIPPER, COMPANY_ROLE.CARRIER, COMPANY_ROLE.SUPPLIER, COMPANY_ROLE.CUSTOMER_SHIPTO })
			{
				this.AllCompanies.AddRange(companies.EnumerateByRole(this.Security, role, byGroupCompanies: false, bLocalize: false));
			}
			foreach (var company in this.AllCompanies)
			{
				if (string.IsNullOrEmpty(company.FederalID))
				{
					company.FederalID = "999999999";
				}
				if (string.IsNullOrEmpty(company.State))
				{
					company.State = "XX";
				}
			}
#if false // used during development to debug
			foreach (var company in AllCompanies)
			{
				if (!sortedCompanies.ContainsKey(company.ID))
				{
					sortedCompanies.Add(company.ID, company.ID);
				}
			}
			foreach (var sortedCompany in sortedCompanies)
			{
				System.Diagnostics.Trace.WriteLine(sortedCompany);
			}

			CompanyClass findComp = (from c in AllCompanies
									 where c.ID == "ConocoPhillips"
									 select c).First();

			
#endif

			CompanyCollectionClass managerCollection = companies.EnumerateByRole(this.Security, COMPANY_ROLE.MANAGER, byGroupCompanies: false, site: this.Site);
			if (managerCollection.Count != 1)
			{
				throw new ExStarsBusinessException("A single manager, not a company group must be selected");
			}
			this.Manager = managerCollection[0];
		}

		public void LoadPreviousAcknowlegement(string wholeFile)
		{
			this.acknowledgement = new PureSegmentList(wholeFile, "From 151");
			// Certainly there are more segments in acknowledgement than REF~55, but it does create an upper limit.
			// by using an array of integers, this can be really fast to search, even when searching linearly
			this.sequenceNumsInAck = new int[this.acknowledgement.Count + 1];
			int seqCount = 0;
			foreach (var segment in this.acknowledgement)
			{
				if (segment.Match("REF", ExStarsConstants.REF01_SequenceNumber))
				{
					this.sequenceNumsInAck[seqCount++] = SeqNumToInt(segment.ElementByIndex(2).Value);
				}
			}
			// put on an end marker
			this.sequenceNumsInAck[seqCount] = -1;
		}

		#endregion

		#region Look-up functions

		protected bool HasSequenceNumber(int sequenceNumberToSeachFor)
		{
			//
			// It's a linear search, but likely less than 20 array items.
			//
			foreach (int sn in this.sequenceNumsInAck)
			{
				if (sn < 0)
				{
					break;
				}
				if (sequenceNumberToSeachFor == sn)
				{
					return true;
				}
			}
			return false;
		}

		public bool IsNotCorrectionOrHasReferencedError()
		{
			return this.ReportModifier != ReportModifiersEnum.Correction
			       || HasSequenceNumber(this.SequenceNumber);
		}

		public int IncSequenceNumber()
		{
			return ++this.SequenceNumber;
		}


		public CompanyClass LookUpCompany(Guid companyGuid)
		{
			return this.LookUpCompany(companyGuid, false, "", "");
		}


		public ProductClass LookUpProduct(Guid productGuid)
		{
			try
			{
				ProductClass product = (from p in this.IrsProductsByProductGuid
				                        where p.IdentityGuid == productGuid || p.MasterRecordGuid == productGuid
				                        select p).First();
				return product;
			}
			catch (Exception)
			{
				this.AppendError(ExStarsErrorSource.Site, "Product Guid {0} cannot be accessed", productGuid.ToString());
				return new ProductClass();
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="companyGuid">guid</param>
		/// /// <param name="nullIsOk">if TRUE and companyGuid is null, then return null</param>
		/// <param name="transId">transaction ID, used when lookup fails</param>
		/// <param name="fieldName">Example Suppier, used when lookup fails</param>
		/// <returns>Returns the company name</returns>
		public CompanyClass LookUpCompany(Guid companyGuid, bool nullIsOk = false, string transId = "", string fieldName = "")
		{
			if (companyGuid.Equals(Guid.Empty) && nullIsOk)
			{
				return null;
			}
			try
			{
				var company = (from c in AllCompanies
				               where c.IdentityGuid == companyGuid || c.MasterRecordGuid == companyGuid
				               select c).First();
				return company;
			}
			catch (Exception)
			{
				string transInfo = transId.Length > 0 ? "For transaction " + transId + " " : "";
				string fieldNameInfo = fieldName.Length > 0 ? "For transaction " + fieldName + " " : "";
				this.AppendError(ExStarsErrorSource.Company, "{0}{1}Company Guid {2} cannot be accessed"
					, transInfo
					, fieldNameInfo
					, companyGuid.ToString());
				return new CompanyClass();
			}
		}

#if false
		protected string GetString(string key)
		{
			ConfigurationSettingDOClass keyedValue = universalConfig.GetByKey(this.Security, key);
			return keyedValue.SettingValue;
		}
#endif





	protected int SeqNumToInt(string seqNumAsStr)
		{
			return Int32.Parse(seqNumAsStr.TrimStart('0'));
		}

		public ExStarsTransportMode LookUpIrsTransportModeByEqTypeId(string equipType)
		{
			string keyToUpper = equipType.ToUpper();
			try
			{
				return this.TransportationModesByName[keyToUpper];
			}
			catch
			{
				throw new ExStarsBusinessException("IrsExStarsIrsTransportModes for equipment type \"{0}\" is not configured in table tblConfigurationSetting", keyToUpper);
			}
		}

		/// <summary>
		/// There is not a one-on-one relationship of IrsModeCode to equipment type
		/// this is intended to work with "RS", but otherwise it takes the first offered
		/// </summary>
		/// <param name="modeCodeAsStr"></param>
		/// <returns>a named source equipment type</returns>
		public string LookUpEquipmentTypeFromIrsTransportMode(string modeCodeAsStr)
		{
			return TransportationModesByIrsMode[modeCodeAsStr].Name;
		}

		public string IrsModeCode(string equipType)
		{
			return this.LookUpIrsTransportModeByEqTypeId(equipType).IrsModeCode;
		}

		public bool IrsModeCodeKeyExists(string key)
		{
			string keyToUpper = key.ToUpper();
			return this.TransportationModesByName.ContainsKey(keyToUpper);
		}
		
		#endregion

		#region Validation

		public void BasicValidation()
		{
			if (this.SiteGuid == null || this.ManagerCompanyGuid == null)
			{
				throw new ExStarsSiteConfigException("Not initialized.");
			}

			if (this.InterchangeSenderId.Length < 2 || this.InterchangeSenderId.Length > 15)
			{
				throw new ExStarsSiteConfigException("InterchangeSenderId", 2, 15);
			}

			if (this.ApplicationSendersCode.Length < 2 || this.ApplicationSendersCode.Length > 15)
			{
				throw new ExStarsSiteConfigException("ApplicationSendersCode", 2, 15);
			}
			if (AuthorizationCode.Length < 10 || this.AuthorizationCode.Length > 10)
			{
				throw new ExStarsSiteConfigException("AuthorizationCode", 10, 10);
			}
			// ref: pg 41
			if (FeinCode.Length < 9 || FeinCode.Length > 18)
			{
				throw new ExStarsSiteConfigException("FeinCode", 9, 18);
			}
			if (SecurityCode.Length < 10 || SecurityCode.Length > 10)
			{
				throw new ExStarsSiteConfigException("SecurityCode", 10, 10);
			}
			if (InfoProviderName.Length < 2 || InfoProviderName.Length > 15)
			{
				throw new ExStarsSiteConfigException("InfoProviderName", 2, 15);
			}
			if (AbbreviatedProviderName.Length < 2 || AbbreviatedProviderName.Length > 15)
			{
				throw new ExStarsSiteConfigException("AbbreviatedProviderName", 2, 15);
			}
			if (TerminalControlNumber.Length != 9)
			{
				throw new ExStarsSiteConfigException("TerminalControlNumber", 9, 9);
			}
		}


		/// <summary>
		/// validates address fields are not blank or too short
		/// </summary>
		protected void ValidateManager()
		{
			try
			{

				if ((string.IsNullOrEmpty(this.Manager.Address1) && string.IsNullOrEmpty(this.Manager.Address2)))
				{
					throw new ExStarsBusinessException("The address of manager \"{0}\" must not be blank", this.Manager.ID);
				}
				if (string.IsNullOrEmpty(this.Manager.City) || this.Manager.City.Length < 2)
				{
					throw new ExStarsBusinessException("The city of manager \"{0}\" must not be blank or invalid", this.Manager.ID);
				}
				if (string.IsNullOrEmpty(this.Manager.State) || this.Manager.State.Length < 2)
				{
					throw new ExStarsBusinessException("The state of manager \"{0}\" must not be blank or invalid", this.Manager.ID);
				}
				if (string.IsNullOrEmpty(this.Manager.Zip) || this.Manager.Zip.Length < 3)
				{
					throw new ExStarsBusinessException("The zipcode of manager \"{0}\" must not be blank or invalid", this.Manager.ID);
				}
				if (string.IsNullOrEmpty(this.Manager.Country) || this.Manager.Country.Length < 3)
				{
					throw new ExStarsBusinessException("The country of manager \"{0}\" must not be blank or invalid", this.Manager.ID);
				}
				if (string.IsNullOrEmpty(this.Manager.Contact1Name))
				{
					throw new ExStarsBusinessException("The contact name of manager \"{0}\" must not be blank", this.Manager.ID);
				}
				if (string.IsNullOrEmpty(this.Manager.Contact1EmailAddress))
				{
					throw new ExStarsBusinessException("The contact email of manager \"{0}\" must not be blank", this.Manager.ID);
				}
			}
			catch (Exception e)
			{

				this.AppendError(ExStarsErrorSource.Config, e.Message);
			}

		}

		protected void ValidateSite()
		{
			int decimalPlaces;
			if (!Int32.TryParse(Site.VolumeDecimalPlaces, out decimalPlaces))
			{
				throw new ExStarsBusinessException("The Volume Decimal Places Setting for site \"{0}\" is invalid", this.Site.ID);
				//this.AppendError(ExStarsErrorSource.site,"The Volume Decimal Places Setting for site \"{0}\" is invalid", this.Site.ID);	
			}
		}

		
		#endregion

	}

	
}