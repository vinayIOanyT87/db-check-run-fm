// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMAETranslations.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// Provides methods for retrieving, creating, updating, and deleting
// records that define translations between values in the legacy aviation application's transaction records
// and in FuelsManager when the transactions are imported through the FMAE interface
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Linq;
	using System.ServiceModel;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	using FMBusinessServices.DataAccessLayer;

	/// <summary>
	/// Provides methods for retrieving, creating, updating, and deleting
	/// records that define translations between values in the legacy aviation application's transaction records
	/// and in FuelsManager when the transactions are imported through the FMAE interface
	/// </summary>
    [ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class FMAETranslations : IFMAETranslations
	{
		/// <summary>
		/// Allows database access.
		/// </summary>
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		/// <summary>
		/// Retrieve a translation record of the specified type matching the provided identity guid  
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="identityGuid">Identifies an FMAE translation record</param>
		/// <param name="translationType">The type of translation to retrieve</param>
		/// <returns>The translation record matching the identity guid. Null if no record was found. </returns>
		private FMAETranslation Get(SecurityClass security, Guid identityGuid, FMAETranslationType translationType)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			if (identityGuid == Guid.Empty)
			{
				throw new Exception("FMAE Translation Identity Guid must be provided");
			}

			FMAETranslation fmaeTranslation = FMAETranslation.CreateTranslationObject(translationType);

			using (SqlCommand cmd = new SqlCommand())
			{
				fmaeTranslation.IdentityGuid = identityGuid;
				fmaeTranslation.SelectByIDSQL(cmd);

				if (fmaeTranslation.Load(ConsolidatedDA.GetDataSet(cmd, security)))
				{
					return fmaeTranslation;
				}
			}

			return null;
		}

		/// <summary>
		/// Get the translation record of the specified type for the specified legacy ID.
		/// There can only be one translation defined for a particular type and legacy ID.
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="fmaeID">The legacy ID to retrieve the translation for</param>
		/// <param name="translationType">The type of translation to retrieve</param>
		/// <returns>The identity guid of the translation record matching the provided legacy ID, or Guid.Empty if none was found</returns>
		private Guid GetIdentityGuid(SecurityClass security, string fmaeID, FMAETranslationType translationType)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			if (string.IsNullOrEmpty(fmaeID))
			{
				throw new Exception("FMAE ID must be provided");
			}

			FMAETranslation fmaeTranslation = FMAETranslation.CreateTranslationObject(translationType);
			fmaeTranslation.ID = fmaeID;

			using (SqlCommand cmd = new SqlCommand())
			{
				fmaeTranslation.SelectByIDSQL(cmd);

				if (fmaeTranslation.Load(ConsolidatedDA.GetDataSet(cmd, security)))
				{
					return fmaeTranslation.IdentityGuid;
				}
			}

			return Guid.Empty;
		}

		/// <summary>
		/// Enumerate all FMAE translation records of the specified type
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="translationType">The type of translations to retrieve</param>
		/// <returns>A list of all translations of the specified type</returns>
		[OperationBehavior(TransactionScopeRequired = false)]
		public List<FMAETranslation> Enumerate(SecurityClass security, FMAETranslationType translationType)
		{
		    return this.EnumerateAndFilter(security, translationType, string.Empty);
		}

        /// <summary>
        /// Enumerate all FMAE translation records of the specified type with an ID that partially matches the search filter
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="translationType">The type of translations to retrieve</param>
        /// <param name="searchFilter">The value to search FMAE translations on</param>
        /// <returns>A list of all translations of the specified type matching the search filter</returns>
	    [OperationBehavior(TransactionScopeRequired = false)]
	    public List<FMAETranslation> EnumerateAndFilter(SecurityClass security, FMAETranslationType translationType, string searchFilter)
	    {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.IMPORT_TRANSACTION))
            {
                throw new FMInsufficientRightsException();
            }

            List<FMAETranslation> fmaeTranslations = new List<FMAETranslation>();
            FMAETranslation fmaeTranslation = FMAETranslation.CreateTranslationObject(translationType);

            using (SqlCommand cmd = new SqlCommand())
            {
                fmaeTranslation.EnumerateSQL(cmd, searchFilter);

                DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

                DataTable table = set.Tables[0];

                while (table.Rows.Count != 0)
                {
                    fmaeTranslation = FMAETranslation.CreateTranslationObject(translationType);
                    fmaeTranslation.Load(set);
                    fmaeTranslations.Add(fmaeTranslation);
                    table.Rows.RemoveAt(0);
                }
            }

            return fmaeTranslations;
	    }

		/// <summary>
		/// Add an FMAE translation record to the database
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="fmaeTranslation">The record to add</param>
		/// <returns>The identity guid of the newly added record</returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, FMAETranslation fmaeTranslation)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (fmaeTranslation == null)
			{
				throw new ArgumentNullException("fmaeTranslation");
			}

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			Guid identityGuid = this.GetIdentityGuid(security, fmaeTranslation.ID, fmaeTranslation.TranslationType);

			if (identityGuid != Guid.Empty
				&& identityGuid != fmaeTranslation.IdentityGuid)
			{
				throw new Exception("A FMAE Translation Record for the same ID exists. You may only specify one translation for a particular ID");
			}

			fmaeTranslation.CreatedDate = DateTimeOffset.Now;
			fmaeTranslation.CreatedBy = security.UserID;
			fmaeTranslation.UpdatedDate = fmaeTranslation.CreatedDate;
			fmaeTranslation.UpdatedBy = security.UserID;

			using (SqlCommand cmd = new SqlCommand())
			{
				string identityGuidParameterName = fmaeTranslation.InsertSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
				fmaeTranslation.IdentityGuid = (Guid)cmd.Parameters[identityGuidParameterName].Value;
			}

			return fmaeTranslation.IdentityGuid;
		}

		/// <summary>
		/// Update an FMAE translation record in the database
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="fmaeTranslation">The translation record to update</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, FMAETranslation fmaeTranslation)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (fmaeTranslation == null)
			{
				throw new ArgumentNullException("fmaeTranslation");
			}

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			Guid identityGuid = this.GetIdentityGuid(security, fmaeTranslation.ID, fmaeTranslation.TranslationType);

			if (identityGuid != Guid.Empty
				&& identityGuid != fmaeTranslation.IdentityGuid)
			{
				throw new Exception("A FMAE Translation Record for the same ID exists. You may only specify one translation for a particular ID");
			}

			FMAETranslation oldTranslation = this.Get(security, fmaeTranslation.IdentityGuid, fmaeTranslation.TranslationType);

			if (oldTranslation == null || oldTranslation.IdentityGuid == Guid.Empty)
			{
				throw (new Exception("Could not locate the FMAE Translation Record to modify"));
			}

			fmaeTranslation.UpdatedDate = DateTimeOffset.Now;
			fmaeTranslation.UpdatedBy = security.UserID;

			using (SqlCommand cmd = new SqlCommand())
			{
				fmaeTranslation.UpdateSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// Delete an FMAE translation record in the database
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="fmaeTranslation">The translation record to delete</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, FMAETranslation fmaeTranslation)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (fmaeTranslation == null)
			{
				throw new ArgumentNullException("fmaeTranslation");
			}

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			FMAETranslation oldTranslation = this.Get(security, fmaeTranslation.IdentityGuid, fmaeTranslation.TranslationType);

			if (oldTranslation == null || oldTranslation.IdentityGuid == Guid.Empty)
			{
				throw new Exception("Could not locate the FMAE Translation Record to delete");
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				fmaeTranslation.PurgeSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

        /// <summary>
        /// Import translations into the system. 
        /// Translations which don't exist will be created, translations that already exist will be updated.
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="translations">Translations to import. Can be of mixed translation types.</param>
        /// <returns>A list of any errors encountered while importing.</returns>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
	    public List<string> Import(SecurityClass security, List<FMAETranslation> translations)
	    {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (translations == null)
            {
                throw new ArgumentNullException("translations");
            }

            if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
            {
                throw new FMInsufficientRightsException();
            }

	        List<string> errorMessages = new List<string>();
            
            // Get companies and products assigned to the site.
            CompaniesClass companiesServiceClass = new CompaniesClass();
			CompanyCollectionClass companies = companiesServiceClass.EnumerateByRoleGetIDCodeTypesIdentityGuidOnly(security, null);

            ProductsClass productsServiceClass = new ProductsClass();
            ProductCollectionClass products = productsServiceClass.EnumerateByFilterAndLocalize(security, string.Empty, false);

            List<FMAETranslation> companyTranslationsToAdd = new List<FMAETranslation>();
            List<FMAETranslation> productTranslationsToAdd = new List<FMAETranslation>();

            foreach (FMAETranslation translation in translations)
            {
                if (string.IsNullOrWhiteSpace(translation.ID))
                {
                    errorMessages.Add("FMAE ID must be provided.");
                }

                if (string.IsNullOrWhiteSpace(translation.EntityID))
                {
                    errorMessages.Add("Enterprise Entity ID must be provided.");
                }

                if (translation.TranslationType != FMAETranslationType.Company
                    && translation.TranslationType != FMAETranslationType.Product)
                {
                    errorMessages.Add("Unrecognized Entity Type: " + translation.TranslationType + ".");
                }

                // Use the EntityID provided on the translation to find a matching company or product record in the system
                // Retain the company or product guid for use when saving the translation if a match is found
                if (translation.TranslationType == FMAETranslationType.Company)
                {
                    CompanyClass company = companies.Find(matchingCompany => matchingCompany.ID.Equals(translation.EntityID, StringComparison.OrdinalIgnoreCase));

                    if (company != null)
                    {
                        translation.EntityGuid = company.MasterRecordGuid;
                        companyTranslationsToAdd.Add(translation);
                    }
                    else
                    {
                        errorMessages.Add("A company named " + translation.EntityID + " was not found.");
                    }
                }
                else if (translation.TranslationType == FMAETranslationType.Product)
                {
                    ProductClass product = products.Find(matchingProduct => matchingProduct.ID.Equals(translation.EntityID, StringComparison.OrdinalIgnoreCase));

                    if (product != null)
                    {
                        translation.EntityGuid = product.MasterRecordGuid;
                        productTranslationsToAdd.Add(translation);
                    }
                    else
                    {
                        errorMessages.Add("A product named " + translation.EntityID + " was not found.");
                    }
                }               
            }

            this.ImportTranslations(security, companyTranslationsToAdd, FMAETranslationType.Company);
            this.ImportTranslations(security, productTranslationsToAdd, FMAETranslationType.Product);

            return errorMessages.Distinct().ToList();
	    }

        /// <summary>
        /// Import translations of the specified type into the database.
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="translations">The translations to import.</param>
        /// <param name="translationType">The type of translation we're importing.</param>
        private void ImportTranslations(SecurityClass security, List<FMAETranslation> translations, FMAETranslationType translationType)
	    {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (translations == null)
            {
                throw new ArgumentNullException("translations");
            }

            if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
            {
                throw new FMInsufficientRightsException();
            }

            // If no translation records need to be saved we have no work to do.
            if (translations.Count == 0)
            {
                return;
            }

            // Guard against the possibility of a user providing translations that aren't of the type specified
	        if (translations.Find(translation => translation.TranslationType != translationType) != null)
	        {
	            throw new Exception(
	                "A translation that was not of the specified type " + translationType + " was found in the translations to import.");
	        }

            // Run the import stored procedure, passing in the translations to import as a table-valued parameter.
            using (SqlCommand cmd = new SqlCommand())
            {
                FMAETranslation fmaeTranslation = FMAETranslation.CreateTranslationObject(translationType);

                fmaeTranslation.ImportSql(cmd, security, translations);

                ConsolidatedDAClass consolidatedDa = new ConsolidatedDAClass();
                consolidatedDa.ExecuteQuery(security, cmd);
            }
	    }
	}
}