// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XMLImportProcessor.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// The purpose of the XML import processor is to handle the import of transaction data
// from the base sites. It will throw an exception if the user does not have the proper
// rights.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XMLImport
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.ServiceModel;
    using System.Xml;
    using System.Xml.XPath;
    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Exceptions;
    using FMBusinessObjects.ServiceRequests;
    using FMBusinessObjects.UtilityObjects;

    using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic;
    using FMWebAPIBusinessLogic.Interfaces.FMProxy;

    /// <summary>
    /// Imports transactions represented in XML 
    /// </summary>
    public class XMLImportProcessor
	{
		const string StartTransactionElementName = "TrxnS";

		#region Attributes

		/// <summary>
		/// The default prefix of user data field names
		/// </summary>
		private const string DefaultUserDataFieldPrefix = "UserData";

		/// <summary>
		/// The maximum number of user data fields supported by the transaction header
		/// </summary>
		private const int MaximumNumberOfUserDataFields = 24;

		private XmlReader xmlReader;
		private bool readResult;

		private TransactionFactory factory;
		internal SecurityClass security;

		private readonly int transactionChunkSize;
		private readonly List<TransactionDO> transactionList;
		private TransactionDO transaction;
		private List<TransactionValidationResult> importResults;

		private Dictionary<string, Guid> siteTable;
		private Dictionary<string, Dictionary<string, CompanyClass>> companyTable;
		private Dictionary<string, Dictionary<string, ProductClass>> productTable;
		private Dictionary<string, Dictionary<string, Guid>> equipmentTable;
		private Dictionary<string, Dictionary<string, Guid>> tankTable;
		private Dictionary<string, Dictionary<string, Guid>> transactionAliasTable;
		private Dictionary<string, Dictionary<Guid, Dictionary<string, string>>> transactionAliasUserFieldsTable;
		private List<string> defaultUserDataFieldNames;
		private Dictionary<string, Dictionary<string, Guid>> personnelTable;
		private Dictionary<string, Dictionary<string, Guid>> stationTable;
		private Dictionary<string, Guid> companyTranslationTable;
		private Dictionary<string, Guid> productTranslationTable;
		private Dictionary<string, GeneralConfigDO> forceCloseoutTable;
		private Dictionary<string, List<CloseoutDO>> closeoutTable;

        private readonly ICurrentRequestContext _currentRequestContext;
        private readonly ITransactionAliasesProxy _transactionAliasProxy;
        private readonly ITransactionPipeline _transactionPipeline;

        #endregion Attributes

        public XMLImportProcessor(ICurrentRequestContext currentRequestContext, ITransactionAliasesProxy transactionAliasProxy, ITransactionPipeline transactionPipeline)
		{
			this.transactionList = new List<TransactionDO>();
            this._currentRequestContext = currentRequestContext;
            this._transactionAliasProxy = transactionAliasProxy;
            this._transactionPipeline = transactionPipeline;

			// Read the maximum number of transactions to save at once from the web.config file
			this.transactionChunkSize = AppSettingsHelper.GetKeyValue("TransactionChunkSize", 100);
		}

		/// <summary>
		/// Log an error using the FMEventLog
		/// </summary>
		/// <param name="message">The message to log</param>
		public static void LogError(string message)
		{
			try
			{
				FMChannelHelper.MakeCall<IFMEventLog>(fmEventLog => fmEventLog.WriteEntry(message, FMEventLogEntryType.Error));
			}
			catch (Exception ex)
			{
				// If for whatever reason we can't log using the FMEventLog, make sure we log something
				using (EventLog eventLog = new EventLog("Application", ".", "FuelsManager"))
				{
					eventLog.WriteEntry("An error occurred while trying to log the message: " + message + ". The error was" + ex, EventLogEntryType.Error);
				}
			}
		}

		/// <summary>
		/// This method ensures that the user importing data has the Execute Import/Export right.  If not, then throw an exception.
		/// </summary>
		private void CheckUserPermission()
		{
			if ( !this.security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT) 
				&& !this.security.HasRight(RIGHT.IMPORT_TRANSACTION))
			{
				throw new System.Security.SecurityException("User \"" + this.security.UserID + "\" " + "not authorized to modify transaction data.");
			}
		}

		public List<TransactionValidationResult> Import(SecurityClass sc, string site, Stream stream, ImportFilter filter)
		{
			this.security = sc;
			this.CheckUserPermission();
			this.GetLookupTables();

			this.factory = new TransactionFactory(this);

			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings { IgnoreComments = true, IgnoreWhitespace = true };

			stream.Position = 0;
			this.xmlReader = XmlReader.Create(new XmlTextReader(stream), xmlReaderSettings);

			this.readResult = true;
			while (this.readResult)
			{
				if (this.xmlReader.LocalName == StartTransactionElementName)
				{
					break;
				}
				this.readResult = this.xmlReader.Read();
			}

			while (this.xmlReader.LocalName != StartTransactionElementName && this.readResult)
			{
				this.readResult = this.xmlReader.Read();
			}

			if (this.xmlReader.LocalName != StartTransactionElementName)
			{
				throw new Exception("Import file does not contain a TrxnS section.");
			}

			this.importResults = new List<TransactionValidationResult>();

			this.readResult = this.xmlReader.Read();

			while (this.ReadTransactions(filter) > 0)
			{
				try
				{
					SaveTransactionsResultDO saveResult = this.SaveTransactions();
					this.importResults.AddRange(saveResult.Results);
				}
				catch (FaultException<SaveTransactionsException> e)
				{
					foreach (TransactionValidationResult transResult in e.Detail.Results)
					{
						if (transResult.HasWarnings || transResult.IsValid == false)
						{
							this.importResults.Add(transResult);
						}
					}
				}
				catch (Exception e)
				{
					// We must handle a case where an unexpected exception occurs when saving a batch of transactions.
					// For example, perhaps the database was temporarily unavailable. 
					// Keep in mind that although the batch will be rolled back when an exception occurs, 
					// it's possible that one batch saved successfully and another did not.
					// All of the transactions that were in that batch must be reported as failures or otherwise the 
					// aviation client will misreport the number of transactions that failed
					LogError(e.ToString());

					foreach (TransactionValidationResult failedResult in this.transactionList.Select(failedTransaction => new TransactionValidationResult
					{
						TransID = failedTransaction.TransID,
						AliasName = failedTransaction.Alias
					}))
					{
						failedResult.ErrorList.Add(e.ToString());
						this.importResults.Add(failedResult);
					}
				}

				this.transactionList.Clear();
			}

			this.xmlReader.Close();
			return this.importResults;
		}

		private int ReadTransactions(ImportFilter filter)
		{
			while (this.readResult && this.xmlReader.LocalName != StartTransactionElementName)
			{
				try
				{
					this.ReadTransaction(filter);

					this.readResult = this.xmlReader.Read();
				}
				catch (NullReferenceException)
				{
					// This happens when there is no transaction to be read from the XML file. (Zero transactions in file.)
					// Simply fall out of the while loop and return.
					break;
				}
				catch (XmlException e)
				{
					LogError(e.ToString());
					TransactionValidationResult transResult = new TransactionValidationResult { TransID = string.Empty };
					transResult.ErrorList.Add(e.Message);
					transResult.ErrorList.Add("All transactions after Line " + e.LineNumber + ", Position " +
						e.LinePosition + " will not be processed.");
					this.importResults.Add(transResult);
					return this.transactionList.Count;
				}
				catch (Exception e)
				{
					LogError(e.ToString());
					TransactionValidationResult transResult = new TransactionValidationResult { TransID = string.Empty };
					transResult.ErrorList.Add(e.Message);
					this.importResults.Add(transResult);
					this.readResult = this.xmlReader.Read();
				}
			}

			return this.transactionList.Count;
		}

		private void ReadTransaction(ImportFilter filter)
		{
			XmlReader singleTransactionReader = this.xmlReader.ReadSubtree();
			XPathDocument doc = new XPathDocument(singleTransactionReader);

			TransactionValidationResult transactionValidationResult;

			this.transaction = this.factory.CreateTransaction(this.security, doc, out transactionValidationResult);

			Debug.WriteLine("Read trans {0} result={1} ", this.transaction, transactionValidationResult);

			// If the transaction meets the configured Import Filter criterion, add it to the list.
			if (this.FilterTransaction(filter, this.transaction))
			{
				bool noErrors = transactionValidationResult.ErrorList == null || (transactionValidationResult.ErrorList.Count == 0);

				Debug.WriteLine("Filter trans {0} ErrCount={1} Err={2} ", this.transaction
					, transactionValidationResult.ErrorList.Count
					, noErrors ? "" : transactionValidationResult.ErrorList[0]);
				// If the transaction doesn't have any validation errors, add it to the list of transaction to save
				if (transactionValidationResult == null || noErrors)
				{
					this.AddTransaction(this.transaction);
				}
				else
				{
					this.importResults.Add(transactionValidationResult);
				}
			}
		}

		private void AddTransaction(TransactionDO transactionToAdd)
		{
			this.transactionList.Add(transactionToAdd);
		}

		private bool FilterTransaction(ImportFilter filter, TransactionDO trans)
		{
			if (filter == null)
			{
				return true;
			}

			if ((filter.FromDate != null) &&
				(trans.InventoryDate < filter.FromDate.Value))
			{
				return false;
			}

			if ((filter.ToDate != null) &&
				(trans.InventoryDate > filter.ToDate.Value))
			{
				return false;
			}

			if ((filter.IncludeDeletedTransactions == false) &&
				(trans.DeleteFlag = true))
			{
				return false;
			}

			if ((filter.AliasList != null) &&
				(filter.AliasList.Contains(trans.Alias) == false))
			{
				return false;
			}

			if (!string.IsNullOrEmpty(trans.ManagerID) &&
				(filter.ManagerList != null) &&
				(filter.ManagerList.Contains(trans.ManagerID) == false))
			{
				return false;
			}

			if (!string.IsNullOrEmpty(trans.OwnerID) &&
				(filter.OwnerList != null) &&
				(filter.OwnerList.Contains(trans.OwnerID) == false))
			{
				return false;
			}

			if (!string.IsNullOrEmpty(trans.SupplierID) &&
				(filter.SupplierList != null) &&
				(filter.SupplierList.Contains(trans.SupplierID) == false))
			{
				return false;
			}

			if (!string.IsNullOrEmpty(trans.CarrierID) &&
				(filter.CarrierList != null) &&
				(filter.CarrierList.Contains(trans.CarrierID) == false))
			{
				return false;
			}

			if (!string.IsNullOrEmpty(trans.ShipToID) &&
				(filter.ConsumerList != null) &&
				(filter.ConsumerList.Contains(trans.ShipToID) == false) &&
				(filter.ConsumerList.Contains(trans.BillToID) == false))
			{
				return false;
			}

			if (filter.ProductList != null)
			{
				bool productMatch = false;
				foreach (LineItemDO lineItem in trans.LineItems)
				{
					if (filter.ProductList.Contains(lineItem.Product) == true)
					{
						productMatch = true;
						break;
					}

					foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
					{
						if (filter.ProductList.Contains(subLineItem.Product) == true)
						{
							productMatch = true;
							break;
						}
					}
				}

				if (productMatch == false)
				{
					return false;
				}
			}

			return true;
		}

		private SaveTransactionsResultDO SaveTransactions()
		{
			// The save transactions processor relies on guids being present to determine whether to insert, update, or delete. We must populate the guids
			// before we save the transactions, or we'll end up always inserting new records.
			List<TransactionDO> transactionsWithPrimaryKeys = FMChannelHelper.MakeCall<ITransactionImportProcessor, List<TransactionDO>>(
																	 importProcessor => importProcessor.PopulateKeyTransactionGuids(this.security, this.transactionList));

			SaveTransactionsSR sr = new SaveTransactionsSR
			{
				Security = this.security,
				CurrentSiteGuid = this.security.SiteGuid,
				ConvertUnits = false,
				UseAutoComplete = true,
				BypassValidation = true
			};

			SaveTransactionsResultDO result = new SaveTransactionsResultDO();

			//This for loop will send each transaction one instead of all at once.  If we have a large
			//set of transactions in the sr.Transactions list, it can cause the SQL database transaction 
			//that envelopes the SaveTransactionProcessor.SaveTranasctions() function to take too long and
			//case other who need to write transaction data to timeout.
			//By sending one transaction at a time, it allow for better interleaving of the SQL database tranasction
			//waiting.

			List<TransactionDO> smallList = new List<TransactionDO>();

			int iIter = 0;

			foreach (TransactionDO trans in transactionsWithPrimaryKeys)
			{
                //get the transaction alias class of the inbound transaction
                var transactionAlias = this._transactionAliasProxy.Get(trans.TransactionAliasGuid, false);
                //pass each transaction through inbound pipeline to 
                var inboundPipeline = this._transactionPipeline.Inbound();
                foreach (var pipe in inboundPipeline)
                {
                    pipe.Execute(trans, transactionAlias);
                }


                smallList.Add(trans);
				iIter++;
				if ((iIter % this.transactionChunkSize) == 0 || iIter == transactionsWithPrimaryKeys.Count)
				{
					sr.Transactions = smallList;
					SaveTransactionsResultDO resultTemp = FMChannelHelper.MakeCall<ISaveTransactionsProcessor, SaveTransactionsResultDO>(
																saveTransactionsProcessor =>
																saveTransactionsProcessor.SaveTransactions(sr));
					if (resultTemp != null && resultTemp.Results != null && resultTemp.Results.Count > 0)
					{
						foreach (TransactionValidationResult tvr in resultTemp.Results)
						{
							result.Results.Add(tvr);
						}
					}
					resultTemp = null;
					smallList.Clear();
				}

			}

			return result;
		}

		private void GetLookupTables()
		{
			this.companyTable = new Dictionary<string, Dictionary<string, CompanyClass>>(StringComparer.OrdinalIgnoreCase);
			this.productTable = new Dictionary<string, Dictionary<string, ProductClass>>(StringComparer.OrdinalIgnoreCase);
			this.equipmentTable = new Dictionary<string, Dictionary<string, Guid>>(StringComparer.OrdinalIgnoreCase);
			this.tankTable = new Dictionary<string, Dictionary<string, Guid>>(StringComparer.OrdinalIgnoreCase);
			this.transactionAliasTable = new Dictionary<string, Dictionary<string, Guid>>(StringComparer.OrdinalIgnoreCase);
			this.personnelTable = new Dictionary<string, Dictionary<string, Guid>>(StringComparer.OrdinalIgnoreCase);
			this.stationTable = new Dictionary<string, Dictionary<string, Guid>>(StringComparer.OrdinalIgnoreCase);
			this.transactionAliasUserFieldsTable = new Dictionary<string, Dictionary<Guid, Dictionary<string, string>>>(StringComparer.OrdinalIgnoreCase);
			this.closeoutTable = new Dictionary<string, List<CloseoutDO>>(StringComparer.OrdinalIgnoreCase);
			this.forceCloseoutTable = new Dictionary<string, GeneralConfigDO>(StringComparer.OrdinalIgnoreCase);

			// Get a list of strings that are "UserData" + x where x is a number 1 through the maximum number of user data fields
			// This is used when populating user data on the transaction record.
			this.defaultUserDataFieldNames = Enumerable.Range(1, MaximumNumberOfUserDataFields).Select(number => DefaultUserDataFieldPrefix + number.ToString(CultureInfo.InvariantCulture)).ToList();

			Guid originalSiteGuid = this.security.SiteGuid;

			SiteClass parentSite = FMChannelHelper.MakeCall<ISites, SiteClass>(sites => sites.Get(this.security, originalSiteGuid, true, false, false));
			this.siteTable = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);

			siteTable.Add(parentSite.SiteID, parentSite.SiteGuid);

			foreach (SiteToSiteMapClass siteToSiteMap in parentSite.SiteToSiteMapCollection)
			{
				this.siteTable.Add(siteToSiteMap.ChildSiteID, siteToSiteMap.ChildSiteGuid);
			}

			foreach (KeyValuePair<string, Guid> site in this.siteTable)
			{
				this.security.SiteGuid = site.Value;

				// Get Stations (loading locations) for the site.
				StationCollectionClass stationList = EnumerateStations(this.security);
				Dictionary<string, Guid> siteStations = stationList.ToDictionary(station => station.ID, station => station.IdentityGuid);

				this.stationTable.Add(site.Key, siteStations);

				// Get personnel for the site.
				IEnumerable<PersonClass> personnelList = EnumeratePersonnel(this.security);
				Dictionary<string, Guid> sitePersonnel = personnelList.ToDictionary(person => person.ID, person => person.MasterRecordGuid);

				this.personnelTable.Add(site.Key, sitePersonnel);

				// Get aliases for the site.
				TransactionAliasNameCollectionClass aliasList = EnumerateTransactionAliases(this.security);
				Dictionary<string, Guid> siteTransactionAliases = aliasList.ToDictionary(alias => alias.AliasName, alias => alias.MasterRecordGuid);

				this.transactionAliasTable.Add(site.Key, siteTransactionAliases);

				// Get alias fields for the site
				List<Guid> transactionAliasGuids = aliasList.Select(transactionAliasName => transactionAliasName.IdentityGuid).ToList();

				Dictionary<Guid, Dictionary<string, string>> userDataFields = EnumerateTransactionAliasesUserDataFields(this.security, transactionAliasGuids);

				this.transactionAliasUserFieldsTable.Add(site.Key, userDataFields);

				// Get Companies for the Site. Lookups for companies are case-insensitive to reduce the number of FMAE Translations required.
				CompanyCollectionClass companyList = EnumerateCompanies(this.security);
				Dictionary<string, CompanyClass> siteCompanies = companyList.ToDictionary(company => company.ID, StringComparer.OrdinalIgnoreCase);

				this.companyTable.Add(site.Key, siteCompanies);

				// Get Products for the Site. Lookups for products are case-insensitive to reduce the number of FMAE Translations required.
				ProductCollectionClass productList = EnumerateProducts(this.security);
				Dictionary<string, ProductClass> siteProducts = productList.ToDictionary(product => product.ID, StringComparer.OrdinalIgnoreCase);

				this.productTable.Add(site.Key, siteProducts);

				// Get Equipment for the Site
				List<EquipmentInfo> equipmentList = EnumerateEquipment(this.security);
				Dictionary<string, Guid> siteEquipment = equipmentList.ToDictionary(equipment => equipment.ID, equipment => equipment.masterRecordGuid);

				this.equipmentTable.Add(site.Key, siteEquipment);

				// Get Tanks for the Site
				TankCollectionClass tankList = EnumerateTanks(this.security);
				Dictionary<string, Guid> siteTanks = tankList.ToDictionary(tank => tank.ID, tank => tank.IdentityGuid);

				this.tankTable.Add(site.Key, siteTanks);

				GeneralConfigDO accountingConfig =
					FMChannelHelper.MakeCall<ITransactionValidator, GeneralConfigDO>(
						validator => validator.GetForcedCloseout(this.security, this.security.SiteID, this.security.SiteGuid));

				this.forceCloseoutTable.Add(site.Key, accountingConfig);

				List<Guid> productNamelist = productList.Select(product => product.MasterRecordGuid).ToList();

				List<CloseoutDO> siteCloseouts =
					FMChannelHelper.MakeCall<ITransactionValidator, List<CloseoutDO>>(
						validator => validator.GetCloseoutDates(this.security, this.security.SiteID, this.security.SiteGuid, Guid.Empty, productNamelist));

				this.closeoutTable.Add(site.Key, siteCloseouts);
			}

			this.security.SiteGuid = originalSiteGuid;

			// Translations do not depend on a particular site so we only need to get them once
			this.companyTranslationTable = EnumerateTranslationValues(this.security, FMAETranslationType.Company);

			this.productTranslationTable = EnumerateTranslationValues(this.security, FMAETranslationType.Product);
		}

		#region Methods to Enumerate Reference Data

		private static StationCollectionClass EnumerateStations(SecurityClass securityClass)
		{
			return FMChannelHelper.MakeCall<IStations, StationCollectionClass>(stations => stations.Enumerate(securityClass));
		}

		private static IEnumerable<PersonClass> EnumeratePersonnel(SecurityClass securityClass)
		{
			return FMChannelHelper.MakeCall<IPersonnel, PersonCollectionClass>(personnel => personnel.EnumerateBasicInformationOnly(securityClass));
		}

		private static TransactionAliasNameCollectionClass EnumerateTransactionAliases(SecurityClass securityClass)
		{
			return FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasNameCollectionClass>(aliasNames => aliasNames.EnumerateNamesOnly(securityClass, false));
		}

		/// <summary>
		/// Get transaction alias user data fields. These are sometimes needed if the aviation client provides user data
		/// </summary>
		/// <param name="securityClass">Contains Security Information</param>
		/// <param name="transactionAliasGuids">A list of alias guids to retrieve user data fields for</param>
		/// <returns>User data fields for the provided site</returns>
		private static Dictionary<Guid, Dictionary<string, string>> EnumerateTransactionAliasesUserDataFields(SecurityClass securityClass, List<Guid> transactionAliasGuids)
		{
			Dictionary<Guid, Dictionary<string, string>> siteUserDataFields = new Dictionary<Guid, Dictionary<string, string>>();

			// For every alias guid provided, get the user data fields
			foreach (Guid transactionAliasGuid in transactionAliasGuids)
			{
				Guid localTransactionAliasGuid = transactionAliasGuid;

				UserDataFieldCollectionClass transactionAliasUserDataFields = FMChannelHelper.MakeCall<IUserDataFields, UserDataFieldCollectionClass>(
					userDataFields =>
					userDataFields.EnumerateByEntityType(securityClass, ENTITY_TYPE.TRANSACTION_ALIAS, localTransactionAliasGuid, false, false));

				// Add each user data field to a collection for the alias, 
				// and then add that collection to a collection of aliases for the site
				foreach (UserDataFieldClass userDataField in transactionAliasUserDataFields)
				{
					Dictionary<string, string> userDataFieldValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

					if (!userDataFieldValues.ContainsKey(userDataField.DisplayName))
					{
						userDataFieldValues.Add(userDataField.DisplayName, userDataField.DbName);
					}

					siteUserDataFields.Add(localTransactionAliasGuid, userDataFieldValues);
				}
			}

			return siteUserDataFields;
		}

		private static TankCollectionClass EnumerateTanks(SecurityClass securityClass)
		{
			return FMChannelHelper.MakeCall<ITanks, TankCollectionClass>(tanks => tanks.EnumerateBasicInformation(securityClass));
		}

		private static List<EquipmentInfo> EnumerateEquipment(SecurityClass securityClass)
		{
			return FMChannelHelper.MakeCall<IEquipments, EquipmentInfo[]>(equipments => equipments.EnumerateInfo(securityClass)).ToList();
		}

		private static ProductCollectionClass EnumerateProducts(SecurityClass securityClass)
		{
			return FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(products => products.EnumerateByFilterAndLocalize(securityClass, string.Empty, false));
		}

		private static CompanyCollectionClass EnumerateCompanies(SecurityClass securityClass)
		{
			return FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(companies => companies.EnumerateByRoleGetIDCodeTypesIdentityGuidOnly(securityClass, null));
		}

		/// <summary>
		/// Return a dictionary of translation values that have been defined for the specified type of entity
		/// </summary>
		/// <param name="securityClass">Contains security information</param>
		/// <param name="translationType">The type of translations to retrieve, e.g. company or product</param>
		/// <returns>A dictionary of translation values that have been defined for the specified type of entity</returns>
		private static Dictionary<string, Guid> EnumerateTranslationValues(SecurityClass securityClass, FMAETranslationType translationType)
		{
			List<FMAETranslation> translations = FMChannelHelper.MakeCall<IFMAETranslations, List<FMAETranslation>>(translationsClient => translationsClient.Enumerate(securityClass, translationType));

			Dictionary<string, Guid> translationTable = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);

			foreach (FMAETranslation translation in translations)
			{
				if (!translationTable.ContainsKey(translation.ID))
				{
					translationTable.Add(translation.ID, translation.EntityGuid);
				}
			}

			return translationTable;
		}

		#endregion

		#region Methods to Look up values in Reference Data

		/// <summary>
		/// Is the field name provided one of the default user data field names like "UserData" + "1" through "24"?
		/// </summary>
		/// <param name="fieldName">The field name to check</param>
		/// <returns>True if the field name provided is one of the default user data field names</returns>
		public bool IsDefaultUserDataFieldName(string fieldName)
		{
			return this.defaultUserDataFieldNames.Contains(fieldName);
		}

		/// <summary>
		/// This method will return the station guid for the given site and
		/// station ID (loading location).
		/// </summary>
		/// <param name="siteID"></param>
		/// <param name="stationID"></param>
		/// <returns></returns>
		public Guid GetStationGuid(string siteID, string stationID)
		{
			if (this.stationTable != null && !string.IsNullOrEmpty(siteID) && !string.IsNullOrEmpty(stationID))
			{
				Dictionary<string, Guid> siteStations;
				Guid stationGuid;

				if (this.stationTable.TryGetValue(siteID, out siteStations) && siteStations.TryGetValue(stationID, out stationGuid))
				{
					return stationGuid;
				}
			}

			return Guid.Empty;
		}

		/// <summary>
		/// This method will return the person guid for the given site and
		/// person ID.
		/// </summary>
		/// <param name="siteID"></param>
		/// <param name="personID"></param>
		/// <returns></returns>
		public Guid GetPersonMasterRecordGuid(string siteID, string personID)
		{
			if (this.personnelTable != null && !string.IsNullOrEmpty(siteID) && !string.IsNullOrEmpty(personID))
			{
				Dictionary<string, Guid> sitePersonnel;
				Guid personnelMasterRecordGuid;

				if (this.personnelTable.TryGetValue(siteID, out sitePersonnel) && sitePersonnel.TryGetValue(personID, out personnelMasterRecordGuid))
				{
					return personnelMasterRecordGuid;
				}
			}

			return Guid.Empty;
		}

		/// <summary>
		/// Return the site guid for the provided site ID
		/// </summary>
		/// <param name="siteID">The siteID to get the SiteGuid for</param>
		/// <returns>The siteGuid corresponding to the provided site ID</returns>
		public Guid GetSiteGuid(string siteID)
		{
			if (this.siteTable != null && !string.IsNullOrEmpty(siteID))
			{
				Guid siteGuid;

				if (this.siteTable.TryGetValue(siteID, out siteGuid) && siteGuid != Guid.Empty)
				{
					return siteGuid;
				}
			}

			return Guid.Empty;
		}

		/// <summary>
		/// Return the transaction alias guid for the provided transaction alias ID
		/// </summary>
		/// <param name="siteID">The site that the transaction belongs to</param>
		/// <param name="aliasID">The transaction alias ID to look up</param>
		/// <returns>The TransactionAliasGuid corresponding to the provided transaction alias ID</returns>
		public Guid GetAliasGuid(string siteID, string aliasID)
		{
			if (this.transactionAliasTable != null && !string.IsNullOrEmpty(siteID) && !string.IsNullOrEmpty(aliasID))
			{
				Dictionary<string, Guid> siteTransactionAliasNames;
				Guid transactionAliasMasterRecordGuid;

				if (this.transactionAliasTable.TryGetValue(siteID, out siteTransactionAliasNames) && siteTransactionAliasNames.TryGetValue(aliasID, out transactionAliasMasterRecordGuid)
					&& transactionAliasMasterRecordGuid != Guid.Empty)
				{
					return transactionAliasMasterRecordGuid;
				}
			}

			return Guid.Empty;
		}

		/// <summary>
		/// Get transaction alias user data fields for the site provided
		/// </summary>
		/// <param name="siteID">The site to get user data fields for</param>
		/// <param name="transactionAliasGuid">The transaction alias to get user data fields for</param>
		/// <returns>User data fields corresponding to the site provided, or null if no user data fields are found</returns>
		public Dictionary<string, string> GetTransactionAliasUserDataFields(string siteID, Guid transactionAliasGuid)
		{
			Dictionary<Guid, Dictionary<string, string>> siteUserDataFields;
			Dictionary<string, string> userDataFields;

			if (!string.IsNullOrEmpty(siteID)
				&& this.transactionAliasUserFieldsTable != null
				&& transactionAliasGuid != Guid.Empty
				&& this.transactionAliasUserFieldsTable.TryGetValue(siteID, out siteUserDataFields)
				&& siteUserDataFields.TryGetValue(transactionAliasGuid, out userDataFields))
			{
				return userDataFields;
			}

			return null;
		}

		/// <summary>
		/// Retrieve closeouts for the given site that correspond to the provided manager
		/// </summary>
		/// <param name="siteID">The site to get closeouts for</param>
		/// <param name="managerID">The manager to get closeouts for</param>
		/// <returns>A list of closeouts for the given site and manager</returns>
		public List<CloseoutDO> GetSiteCloseoutsForManager(string siteID, string managerID)
		{
			List<CloseoutDO> siteCloseouts = new List<CloseoutDO>();

			if (!string.IsNullOrEmpty(siteID)
				&& this.closeoutTable != null
				&& this.closeoutTable.TryGetValue(siteID, out siteCloseouts))
			{
				return siteCloseouts.FindAll(closeout => string.Compare(closeout.ManagerName, managerID, StringComparison.OrdinalIgnoreCase) == 0);
			}

			return siteCloseouts;
		}

		/// <summary>
		/// Retrieve the accounting general configuration for the given site. We use this information
		/// when checking the inventory date
		/// </summary>
		/// <param name="siteID">The site to get the general configuration for</param>
		/// <returns>The accounting general configuration for the given site</returns>
		public GeneralConfigDO GetSiteAccountingConfiguration(string siteID)
		{
			GeneralConfigDO siteAccountingConfiguration;

			if (!string.IsNullOrEmpty(siteID)
				&& this.forceCloseoutTable != null
				&& this.forceCloseoutTable.TryGetValue(siteID, out siteAccountingConfiguration))
			{
				return siteAccountingConfiguration;
			}

			return null;
		}

		/// <summary>
		/// Get the company in the enterprise system identified by the provided guid
		/// </summary>
		/// <param name="siteID">The site to search for the company in</param>
		/// <param name="companyGuid">Identifies the company to retrieve</param>
		/// <returns>The company matching the provided guid, or null if none was found</returns>
		public CompanyClass GetCompanyByGuid(string siteID, Guid companyGuid)
		{
			if (this.companyTable != null && !string.IsNullOrEmpty(siteID) && companyGuid != Guid.Empty)
			{
				Dictionary<string, CompanyClass> siteCompanies;

				if (this.companyTable.TryGetValue(siteID, out siteCompanies))
				{
					return siteCompanies.Values.FirstOrDefault(company => company.MasterRecordGuid == companyGuid);
				}
			}

			return null;
		}

		/// <summary>
		/// Get the company in the enterprise system matching the provided code
		/// </summary>
		/// <param name="siteID">The site to search for the company in</param>
		/// <param name="companyCode">The company code to search for in the enterprise system</param>
		/// <returns>The company matching the provided code, or null if none was found</returns>
		public CompanyClass GetCompanyByCode(string siteID, string companyCode)
		{
			if (this.companyTable != null && !string.IsNullOrEmpty(siteID) && !string.IsNullOrEmpty(companyCode))
			{
				Dictionary<string, CompanyClass> siteCompanies;

				if (this.companyTable.TryGetValue(siteID, out siteCompanies))
				{
					List<CompanyClass> matchingCompanies = siteCompanies.Values.Where(company => company.Code == companyCode).ToList();

					// Only return a company if there is one and only one match on the code
					if (matchingCompanies.Count == 1)
					{
						return matchingCompanies[0];
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Get the company in the enterprise system matching the provided ID
		/// </summary>
		/// <param name="siteID">The site to search for the company in</param>
		/// <param name="companyID">The company ID to search for in the enterprise system</param>
		/// <returns>The company matching the provided ID, or null if none was found</returns>
		public CompanyClass GetCompanyByID(string siteID, string companyID)
		{
			if (this.companyTable != null && !string.IsNullOrEmpty(siteID) && !string.IsNullOrEmpty(companyID))
			{
				Dictionary<string, CompanyClass> siteCompanies;
				CompanyClass company;

				if (this.companyTable.TryGetValue(siteID, out siteCompanies) && siteCompanies.TryGetValue(companyID, out company))
				{
					return company;
				}
			}

			return null;
		}

		/// <summary>
		/// Get the product in the enterprise system identified by the provided guid
		/// </summary>
		/// <param name="siteID">The site to search for the product in</param>
		/// <param name="productGuid">Identifies the product to retrieve</param>
		/// <returns>The product matching the provided guid, or null if none was found</returns>
		public ProductClass GetProductByGuid(string siteID, Guid productGuid)
		{
			if (this.productTable != null && !string.IsNullOrEmpty(siteID) && productGuid != Guid.Empty)
			{
				Dictionary<string, ProductClass> siteProducts;

				if (this.productTable.TryGetValue(siteID, out siteProducts))
				{
					return siteProducts.Values.FirstOrDefault(product => product.MasterRecordGuid == productGuid);
				}
			}

			return null;
		}

		/// <summary>
		/// Get the product in the enterprise system matching the provided ID
		/// </summary>
		/// <param name="siteID">The site to search for the product in</param>
		/// <param name="productID">The product ID to search for a match for in the enterprise system</param>
		/// <returns>The product matching the provided ID, or null if none was found</returns>
		public ProductClass GetProductByID(string siteID, string productID)
		{
			if (this.productTable != null && !string.IsNullOrEmpty(siteID) && !string.IsNullOrEmpty(productID))
			{
				Dictionary<string, ProductClass> siteProducts;
				ProductClass product;

				if (this.productTable.TryGetValue(siteID, out siteProducts) && siteProducts.TryGetValue(productID, out product))
				{
					return product;
				}
			}

			return null;
		}

		/// <summary>
		/// Get the product in the enterprise system matching the provided Code
		/// </summary>
		/// <param name="siteID">The site to search for the product in</param>
		/// <param name="productCode">The product Code to search for a match for in the enterprise system</param>
		/// <returns>The product matching the provided Code, or null if none was found</returns>
		public ProductClass GetProductByCode(string siteID, string productCode)
		{
			if (this.productTable != null && !string.IsNullOrEmpty(siteID) && !string.IsNullOrEmpty(productCode))
			{
				Dictionary<string, ProductClass> siteProducts;

				if (this.productTable.TryGetValue(siteID, out siteProducts))
				{
					List<ProductClass> matchingProducts = siteProducts.Values.Where(product => product.Code == productCode).ToList();

					// Only return a product if there is one and only one match on the code
					if (matchingProducts.Count == 1)
					{
						return matchingProducts[0];
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Retrieve the equipment guid corresponding to the provided equipment ID.
		/// </summary>
		/// <param name="siteID">The site we're looking up equipment for</param>
		/// <param name="equipmentID">The ID of an equipment record to use to search for a match</param>
		/// <returns>The EquipmentGuid of the record matching the provided equipment ID</returns>
		public Guid GetEquipmentGuid(string siteID, string equipmentID)
		{
			if (this.equipmentTable != null && !string.IsNullOrEmpty(siteID) && !string.IsNullOrEmpty(equipmentID))
			{
				Dictionary<string, Guid> siteEquipment;
				Guid equipmentMasterRecordGuid;

				if (this.equipmentTable.TryGetValue(siteID, out siteEquipment) && siteEquipment.TryGetValue(equipmentID, out equipmentMasterRecordGuid))
				{
					return equipmentMasterRecordGuid;
				}
			}

			return Guid.Empty;
		}

		public Guid GetTankGuid(string siteID, string tankID)
		{
			if (this.tankTable != null && !string.IsNullOrEmpty(siteID) && !string.IsNullOrEmpty(tankID))
			{
				Dictionary<string, Guid> siteTanks;
				Guid tankGuid;

				if (this.tankTable.TryGetValue(siteID, out siteTanks) && siteTanks.TryGetValue(tankID, out tankGuid))
				{
					return tankGuid;
				}
			}

			return Guid.Empty;
		}

		/// <summary>
		/// Get the Enterprise (FuelsManager) record guid defined as a translation for the 
		/// legacy entity ID provided.
		/// </summary>
		/// <param name="legacyEntityID">The ID from the legacy transaction record</param>
		/// <param name="translationType">The type of translation, e.g. product or company</param>
		/// <returns>The Guid of the entity the legacy ID should translate to, or Guid.Empty if no translation was found</returns>
		public Guid GetTranslatedEntityGuid(string legacyEntityID, FMAETranslationType translationType)
		{
			Guid translatedEntityGuid;

			if (translationType == FMAETranslationType.Company)
			{
				this.companyTranslationTable.TryGetValue(legacyEntityID, out translatedEntityGuid);
			}
			else if (translationType == FMAETranslationType.Product)
			{
				this.productTranslationTable.TryGetValue(legacyEntityID, out translatedEntityGuid);
			}
			else
			{
				throw new Exception("Unknown entity translation type: " + translationType);
			}

			return translatedEntityGuid;
		}

		#endregion
	}
}
