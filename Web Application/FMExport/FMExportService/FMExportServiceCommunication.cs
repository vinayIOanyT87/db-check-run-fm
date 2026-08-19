// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMExportServiceCommunication.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Exposes methods so that they can be called by the FMExportConfiguration utility
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMExportService
{
	using System;
	using System.Collections.Generic;
	using System.Data;
    using System.IO;
	using System.ServiceModel;
	using System.Reflection;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.ServiceRequests;
    using FMBusinessObjects.UtilityObjects;

	/// <summary>
	/// Exposes methods that are used by the FM Export Configuration utility. 
	/// Many of these methods are already available through FMBusinessServices, however, some are not.
	/// </summary>
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Single)]
	public class FMExportServiceCommunication : IFMExportService
	{
		/// <summary>
		/// Get a list of interfaces that are supported by the FMExportService
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		/// <returns>A list of interfaces that are supported by the FMExportService</returns>
		public List<string> GetSupportedInterfaceIDs(SecurityClass security) {
			if (!security.HasRight(RIGHT.CONFIGURE_AVIATION_EXPORT))
				throw new FMInsufficientRightsException();

			List<string> supportedInterfaceIDs = new List<string>();
			
			foreach (Type objType in FMExportService.SupportedInterfaceTypes) {
				IDataRetriever dataRetriever = Activator.CreateInstance(objType) as IDataRetriever;
				if (dataRetriever != null) {
					supportedInterfaceIDs.Add(dataRetriever.InterfaceId);
				}
			}

			return supportedInterfaceIDs;
		}

		/// <summary>
		/// Gets a list of ExportRequestClass objects from the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <returns>The specified list of ExportRequestClass objects</returns>
		public List<ExportRequestClass> GetRequests(SecurityClass security)
		{
			return FMChannelHelper.MakeCall<IExportRequests, List<ExportRequestClass>>(
					exportRequests => exportRequests.GetRequests(security));
		}

		/// <summary>
		/// Gets a table of in-memory data from the database.  Executes the
		/// specified SQL command and returns the resultant DataTable.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="cmd">The serializable SQL Command</param>
		/// <returns>The DataTable containing the results of the specified SQL command</returns>
		public DataTable GetDataTable(SecurityClass security, SerializableSqlCommand cmd)
		{
			return FMChannelHelper.MakeCall<IExportRequests, DataTable>(
				exportRequests => exportRequests.GetDataTable(security, cmd));
		}

		/// <summary>
		/// Modifies an existing ExportRequestClass object in the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="exportRequest">The object to modify in the database</param>
		public void Update(SecurityClass security, ExportRequestClass exportRequest)
		{
			FMChannelHelper.MakeCall<IExportRequests>(exportRequests => exportRequests.Update(security, exportRequest));
		}

		/// <summary>
		/// Adds an ExportRequestClass object to the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="exportRequest">The object to add to the database</param>
		public void Add(SecurityClass security, ExportRequestClass exportRequest)
		{
			FMChannelHelper.MakeCall<IExportRequests>(exportRequests => exportRequests.Add(security, exportRequest));
		}

		/// <summary>
		/// Deletes an existing ExportRequestClass object from the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="identityGuid">Identifies the object to delete in the database</param>
		public void Delete(SecurityClass security, Guid identityGuid)
		{
			FMChannelHelper.MakeCall<IExportRequests>(exportRequests => exportRequests.Delete(security, identityGuid));
		}

		/// <summary>
		/// Gets an existing ExportRequestClass object from the database given the ID.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="id">The ID of the ExportRequestClass object</param>
		/// <returns>The specified ExportRequestClass object</returns>
		public ExportRequestClass GetRequestById(SecurityClass security, string id)
		{
			return FMChannelHelper.MakeCall<IExportRequests, ExportRequestClass>(exportRequests => exportRequests.GetRequestById(security, id));
		}

		/// <summary>
		/// Gets an existing ExportRequestClass object from the database given the Identity Guid (ExportRequestGuid)
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="identityGuid">The identity guid identifying the ExportRequestClass record</param>
		/// <returns>The specified ExportRequestClass record</returns>
		public ExportRequestClass Get(SecurityClass security, Guid identityGuid)
		{
			return FMChannelHelper.MakeCall<IExportRequests, ExportRequestClass>(exportRequests => exportRequests.Get(security, identityGuid));
		}

		/// <summary>
		/// Login to FuelsManager with the provided credentials
		/// </summary>
		/// <param name="changePasswordParam">Whether or not the user has to change their password</param>
		/// <param name="daysUntilExpirationParam">The number of days until the password expires</param>
		/// <param name="securityParam">The security object used to interact with FuelsManager</param>
		/// <param name="securityLoginRequest">The login request with information like the user name and password</param>
		/// <returns>A string with any information about why the login might not have been successful</returns>
		public string Login(
			out bool changePasswordParam,
			out int daysUntilExpirationParam,
			out SecurityClass securityParam,
			SecurityLoginRequest securityLoginRequest)
		{
			bool changePassword = false;
			int daysUntilExpiration = 0;
			SecurityClass security = null;

			string token = FMChannelHelper.MakeCall<ISites, string>(sites => sites.Login(out changePassword, out daysUntilExpiration, out security, securityLoginRequest));

			changePasswordParam = changePassword;
			daysUntilExpirationParam = daysUntilExpiration;
			securityParam = security;
			return token;
		}

		/// <summary>
		/// Log the user out of FuelsManager
		/// </summary>
		/// <param name="security">Contains information identifying the user to logout</param>
		public void Logout(SecurityClass security)
		{
			FMChannelHelper.MakeCall<ISites>(sites => sites.Logout(security));
		}

        /// <summary>
        /// Gets the list of FMAE Translation objects
        /// </summary>
        /// <param name="security">>The security object used to interact with FuelsManager</param>
        /// /// <param name="translationType">>the type of translation to return</param>
        public List<FMAETranslation> EnumerateFMAETranslations(SecurityClass security, FMAETranslationType translationType)
        {
            List<FMAETranslation> translations = FMChannelHelper.MakeCall<IFMAETranslations, List<FMAETranslation>>(translationsClient => translationsClient.Enumerate(security, translationType));
            Dictionary<string,Guid> companyIDs = EnumerateCompaniesIDs(security);
            List<string> currentTranslations = new List<string>();
            foreach (FMAECompanyTranslation translation in translations)
            {
                if (!currentTranslations.Contains(translation.ID.ToUpper()))
                {
                    currentTranslations.Add(translation.ID.ToUpper());
                }
            }
            foreach (string companyID in companyIDs.Keys)
            {
                if (!currentTranslations.Contains(companyID.ToUpper()))
                {
                    FMAECompanyTranslation newTranslation = (FMAECompanyTranslation)FMAETranslation.CreateTranslationObject(FMAETranslationType.Company);
                    newTranslation.EntityID = companyID;
                    newTranslation.EntityGuid = companyIDs[companyID];
                    newTranslation.ID = companyID;
                    newTranslation.IdentityGuid = new Guid();
                    newTranslation.CreatedBy = "VarecTemp";
                    newTranslation.UpdatedBy = "VarecTemp";
                    translations.Add(newTranslation);
                }
            }
            return translations;
        }

        private Dictionary<string,Guid> EnumerateCompaniesIDs(SecurityClass security)
        {
            SerializableSqlCommand cmd = new SerializableSqlCommand("Select Distinct ID,_MasterRecordGuid from tblCompanies where _MasterRecordGuid = CompanyGuid");
            DataTable table = GetDataTable(security, cmd);
            Dictionary<string, Guid> list = new Dictionary<string,Guid>();
            string id = "";
            foreach(DataRow row in table.Rows)
            {
                id = FMConvert.ConvertCellToString(row[0],false);
                if(!list.ContainsKey(id))
                {
                    list.Add(id,(Guid)row[1]);
                }
            }
            return list;
            
        }

        private string[] LinkFMAECompanyNamesToEnterpriseNames(SecurityClass security, string[] fmaeCompanyNames)
        {
            if(fmaeCompanyNames == null || fmaeCompanyNames.Length == 0)
                return new string[1];

            SortedList<string, string> list = new SortedList<string, string>();
            List<FMAETranslation> translations = EnumerateFMAETranslations(security, FMAETranslationType.Company);
            Dictionary<string,Guid> companyIDs = EnumerateCompaniesIDs(security);
            foreach (FMAETranslation translation in translations)
            {
                if(!list.ContainsKey(translation.ID.ToUpper()))
                {
                    list.Add(translation.ID.ToUpper(), translation.EntityID);
                }
            }

            foreach (string id in companyIDs.Keys)
            {
                if (!list.ContainsKey(id.ToUpper()))
                {
                    list.Add(id.ToUpper(), id);
                }
            }
            string[] linkedCompanies = new string[fmaeCompanyNames.Length];
            for (int i = 0; i < fmaeCompanyNames.Length; i++ )
            {
                string s = fmaeCompanyNames[i];
                linkedCompanies[i] = "";
                string[] tokens = s.Split(",".ToCharArray());
                if (tokens.Length >= 1)
                {
                    string fmaeID = tokens[0].ToUpper().Trim();
                    if (list.ContainsKey(fmaeID))
                    {
                        linkedCompanies[i] = list[fmaeID] + "=" + s;
                    }
                    else
                    {
                        linkedCompanies[i] = Constants.NO_LEGACY_MAPPING_VALUE + "=" + s;
                    }
                }
            }
            return linkedCompanies;
        }

        public List<string> GetSupportedWebServicePluginIDs(SecurityClass Security)
        {
            if (!Security.HasRight(RIGHT.CONFIGURE_AVIATION_EXPORT))
                throw new FMInsufficientRightsException();

            List<string> objInterfaceNames;
            Type[] objServiceTypes;
            string strPath, strDir;
            IWebServicePlugin objPlugin;

            objInterfaceNames = new List<string>();
            strPath = Assembly.GetExecutingAssembly().Location;
            strDir = Path.GetDirectoryName(strPath);
            strDir = Path.Combine(strDir, Constants.WEBSERVICE_PLUGIN_FOLDER);

            if (Directory.Exists(strDir))
            {
                objServiceTypes = FMExportService.GetTypesImplementingInterface("IWebServicePlugin", strDir);

                foreach (Type objType in objServiceTypes)
                {
                    objPlugin = (IWebServicePlugin)Activator.CreateInstance(objType);
                    if (objPlugin != null)
                    {
                        objInterfaceNames.Add(objPlugin.WebServicePluginID);
                    }
                }
            }

            return objInterfaceNames;
        }

    }
}
