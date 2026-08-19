// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FuelCardLimits.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// Implements operations exposed by the Fuel Card Limits Functionality to support database operations 
// like adding, modifying, or deleting a record.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Exceptions;

    using FMBusinessServices.DataAccessLayer;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.ServiceModel;

    /// <summary>
    /// Implements operations exposed by the Fuel Card Limits Functionality to support database operations 
    /// like adding, modifying, or deleting a record.
    /// </summary>
    [ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
    public class FuelCardLimits : IFuelCardLimits
    {
        /// <summary>
        /// Allows database access.
        /// </summary>
        internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

        /// <summary>
        /// Add a new Fuel Card Limit and any associated line items or fuel card mappings to the database.
        /// </summary>
        /// <param name="security">Contains Security Information.</param>
        /// <param name="fuelCardLimit">The fuel card limit record to add.</param>
        /// <returns>The identity guid (primary key) of the new Fuel Card Limit record.</returns>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public Guid Add(SecurityClass security, FuelCardLimit fuelCardLimit)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.MODIFY_FUEL_CARD_LIMIT))
            {
                throw new FMInsufficientRightsException();
            }

            if (fuelCardLimit == null)
            {
                throw new ArgumentNullException("fuelCardLimit");
            }

            this.Validate(fuelCardLimit);

            // Make sure that there is not already a fuel card limit assigned to or owned by this site
            // with the same ID 
            if (this.GetIdentityGuid(security, fuelCardLimit.ID) != Guid.Empty)
            {
                throw new Exception("A Fuel Card Limit with the same ID exists");
            }

            fuelCardLimit.IdentityGuid = Guid.NewGuid();
            fuelCardLimit.SiteGuid = security.SiteGuid;
            fuelCardLimit.CreatedBy = security.UserID;

            using (SqlCommand cmd = new SqlCommand())
            {
                fuelCardLimit.InsertSQL(cmd);
                this.ConsolidatedDA.ExecuteQuery(security, cmd);

                // Create a record mapping the fuel card limit to the current site
                EntityToSiteMaps entityToSiteMaps = new EntityToSiteMaps();
                EntityToSiteMapClass entityToSiteMap = new EntityToSiteMapClass(fuelCardLimit);
                entityToSiteMaps.Add(security, entityToSiteMap, this.GetType().GUID);
            }

            // Add the line items associated with the fuel card limit
            this.UpdateLineItems(security, null, fuelCardLimit);

            // Add mappings for the fuel cards associated with the fuel card limit
            this.UpdateFuelCardMappings(security, null, fuelCardLimit);

            return fuelCardLimit.IdentityGuid;
        }

        /// <summary>
        /// Update an existing Fuel Card Limit and any associated line items and fuel card mappings in the database
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="fuelCardLimit">The fuel card limit record to update</param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Modify(SecurityClass security, FuelCardLimit fuelCardLimit)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.MODIFY_FUEL_CARD_LIMIT))
            {
                throw new FMInsufficientRightsException();
            }

            if (fuelCardLimit == null)
            {
                throw new ArgumentNullException("fuelCardLimit");
            }

            this.Validate(fuelCardLimit);

            // Modify the security objects's site guid in case the fuel card limit's site is changing from an entity ownership change.
            // We want to perform the check in the site the limit will be owned by, not the site it's currently owned by.
            Guid siteGuid = security.SiteGuid;
            security.SiteGuid = fuelCardLimit.SiteGuid;

            Guid existingFuelCardLimitGuid = this.GetIdentityGuid(security, fuelCardLimit.ID);

            // restore the siteguid to its original value
            security.SiteGuid = siteGuid;

            if (existingFuelCardLimitGuid != Guid.Empty && existingFuelCardLimitGuid != fuelCardLimit.IdentityGuid)
            {
                throw new Exception("A Fuel Card Limit with the same ID exists");
            }

            FuelCardLimit oldFuelCardLimit = this.Get(security, fuelCardLimit.IdentityGuid);

            if (oldFuelCardLimit == null || oldFuelCardLimit.IdentityGuid == Guid.Empty)
            {
                throw new Exception("The Fuel Card Limit was not found");
            }

            fuelCardLimit.UpdatedBy = security.UserID;

            using (SqlCommand cmd = new SqlCommand())
            {
                fuelCardLimit.UpdateSQL(cmd);
                this.ConsolidatedDA.ExecuteQuery(security, cmd);
            }

            // Add, modify, or delete the line items associated with the fuel card limit
            // The determination of whether to add, modify, or delete is based off the line items
            // on the current (existing) version of the limit and the line items present on the new version of the limit
            this.UpdateLineItems(security, oldFuelCardLimit, fuelCardLimit);
            this.UpdateFuelCardMappings(security, oldFuelCardLimit, fuelCardLimit);

            if (fuelCardLimit.SiteGuid != oldFuelCardLimit.SiteGuid)
            {
                EntityToSiteMaps entityToSiteMaps = new EntityToSiteMaps();
                EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(
                    security, fuelCardLimit.EntityType, fuelCardLimit.IdentityGuid);

                // If the site changed,
                // Purge any records mapping the fuel card limit to a site
                foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
                {
                    entityToSiteMap.ID = fuelCardLimit.ID;
                    entityToSiteMaps.Purge(security, entityToSiteMap);
                }

                // Create a new record mapping the fuel card limit to the new site
                EntityToSiteMapClass newEntityToSiteMap = new EntityToSiteMapClass(fuelCardLimit);
                entityToSiteMaps.Add(security, newEntityToSiteMap, this.GetType().GUID);
            }
        }

        /// <summary>
        /// Delete the Fuel Card Limit record identified by the provided Guid and any associated records
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="fuelCardLimitGuid">Identifies the fuel card limit record to delete.</param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Purge(SecurityClass security, Guid fuelCardLimitGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.MODIFY_FUEL_CARD_LIMIT))
            {
                throw new FMInsufficientRightsException();
            }

            FuelCardLimit fuelCardLimit = this.Get(security, fuelCardLimitGuid);

            if (fuelCardLimit.IdentityGuid != Guid.Empty)
            {
                // Delete any records mapping the fuel card limit to a site
                EntityToSiteMaps entityToSiteMaps = new EntityToSiteMaps();
                EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(
                    security, fuelCardLimit.EntityType, fuelCardLimit.IdentityGuid);

                foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
                {
                    entityToSiteMap.ID = fuelCardLimit.ID;
                    entityToSiteMaps.Purge(security, entityToSiteMap);
                }

                // Delete the fuel card limit and any associated records (line items, mappings to fuel cards, etc)
                using (SqlCommand cmd = new SqlCommand())
                {
                    fuelCardLimit.PurgeSQL(cmd);
                    this.ConsolidatedDA.ExecuteQuery(security, cmd);
                }
            }
            else
            {
                throw new Exception("The Fuel Card Limit to delete was not found");
            }
        }

        /// <summary>
        /// Get the Fuel Card Limit record identified by the provided Guid
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="fuelCardLimitGuid">Identifies the fuel card limit record to retrieve.</param>
        /// <returns>The Fuel Card Limit record identified by the provided Guid. Will return null if no match is found</returns>
        public FuelCardLimit Get(SecurityClass security, Guid fuelCardLimitGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.VIEW_FUEL_CARD_LIMIT) && !security.HasRight(RIGHT.MODIFY_FUEL_CARD_LIMIT))
            {
                throw new FMInsufficientRightsException();
            }

            if (fuelCardLimitGuid == Guid.Empty)
            {
                throw new ArgumentException("fuelCardLimitGuid");
            }

            FuelCardLimit fuelCardLimit = new FuelCardLimit { IdentityGuid = fuelCardLimitGuid };

            using (SqlCommand cmd = new SqlCommand())
            {
                fuelCardLimit.SelectSQL(cmd);

                if (!fuelCardLimit.Load(this.ConsolidatedDA.GetDataSet(cmd, security)))
                {
                    return null;
                }
            }

            FuelCardLimitLineItems lineItemsServiceClass = new FuelCardLimitLineItems();
            fuelCardLimit.LineItems = lineItemsServiceClass.Enumerate(security, fuelCardLimit.IdentityGuid);
            fuelCardLimit.AssignedFuelCards = this.EnumerateFuelCardMappings(security, fuelCardLimit.IdentityGuid);

            return fuelCardLimit;
        }

        /// <summary>
        /// Get the Identity Guid (Primary Key) of the Fuel Card Limit record identified by the provided ID
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="fuelCardLimitID">Identifies the fuel card limit record to retrieve.</param>
        /// <returns>The Identity Guid (Primary Key) Fuel Card Limit record identified by the provided ID. Will return an empty guid if no match is found</returns>
        public Guid GetIdentityGuid(SecurityClass security, string fuelCardLimitID)
        {
            FuelCardLimit matchingFuelCardLimit = this.GetByID(security, fuelCardLimitID);
            return matchingFuelCardLimit == null ? Guid.Empty : matchingFuelCardLimit.IdentityGuid;
        }

        /// <summary>
        /// Get the Fuel Card Limit record identified by the provided ID
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="fuelCardLimitID">Identifies the fuel card limit record to retrieve.</param>
        /// <returns>The Fuel Card Limit record identified by the provided ID. Will return null if no record is found</returns>
        public FuelCardLimit GetByID(SecurityClass security, string fuelCardLimitID)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.VIEW_FUEL_CARD_LIMIT) && !security.HasRight(RIGHT.MODIFY_FUEL_CARD_LIMIT))
            {
                throw new FMInsufficientRightsException();
            }

            if (string.IsNullOrEmpty(fuelCardLimitID))
            {
                throw new ArgumentException("fuelCardLimitID");
            }

            FuelCardLimit fuelCardLimit = new FuelCardLimit { ID = fuelCardLimitID, SiteGuid = security.SiteGuid };

            using (SqlCommand cmd = new SqlCommand())
            {
                fuelCardLimit.SelectByIdSQL(cmd);
                if (!fuelCardLimit.Load(this.ConsolidatedDA.GetDataSet(cmd, security)))
                {
                    return null;
                }
            }

            FuelCardLimitLineItems lineItemsServiceClass = new FuelCardLimitLineItems();
            fuelCardLimit.LineItems = lineItemsServiceClass.Enumerate(security, fuelCardLimit.IdentityGuid);
            fuelCardLimit.AssignedFuelCards = this.EnumerateFuelCardMappings(security, fuelCardLimit.IdentityGuid);

            return fuelCardLimit.IdentityGuid == Guid.Empty ? null : fuelCardLimit;
        }

        public DataTable EnumerateForMobile(SecurityClass security)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.VIEW_FUEL_CARD_LIMIT) && !security.HasRight(RIGHT.MODIFY_FUEL_CARD_LIMIT))
            {
                throw new FMInsufficientRightsException();
            }

            FuelCardLimit fuelCardLimit = new FuelCardLimit
            {
                SiteGuid = security.SiteGuid,
            };

            using (SqlCommand cmd = new SqlCommand())
            {
                fuelCardLimit.EnumerateForMobileSQL(cmd);

                DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);

                if (set.Tables.Count <= 0)
                {
                    throw new FMDataExchangeNullDataException("Error occurred while retrieving FuelCardLimit data");
                }

                return set.Tables[0];
            }
        }

        /// <summary>
        /// Get all Fuel Card Limits assigned or owned by the current site
        /// </summary>
        /// <param name="security">Contains security information, like the site we're currently accessing to retrieve fuel card limits for</param>
        /// <returns>All Fuel Card Limits assigned or owned by the current site</returns>
        public List<FuelCardLimit> Enumerate(SecurityClass security)
        {
            return this.EnumerateAndFilter(security, string.Empty);
        }

        /// <summary>
        /// Get all Fuel Card Limits assigned or owned by the current site that partially match the ID provided
        /// </summary>
        /// <param name="security">Contains security information, like the site we're currently accessing to retrieve fuel card limits for</param>
        /// <param name="searchFilter">The ID to search for matches on</param>
        /// <returns>All Fuel Card Limits assigned or owned by the current site that partially match the provided ID</returns>
        public List<FuelCardLimit> EnumerateAndFilter(SecurityClass security, string searchFilter)
        {
            List<FuelCardLimit> fuelCardLimitCollection = new List<FuelCardLimit>();

            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.VIEW_FUEL_CARD_LIMIT) && !security.HasRight(RIGHT.MODIFY_FUEL_CARD_LIMIT))
            {
                throw new FMInsufficientRightsException();
            }

            FuelCardLimit fuelCardLimit = new FuelCardLimit
                                              {
                                                  SiteGuid = security.SiteGuid,
                                                  ID = searchFilter
                                              };

            using (SqlCommand cmd = new SqlCommand())
            {
                if (!string.IsNullOrEmpty(searchFilter))
                {
                    fuelCardLimit.EnumerateAndFilterSQL(cmd);
                }
                else
                {
                    fuelCardLimit.EnumerateSQL(cmd);
                }

                DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);

                if (set.Tables.Count <= 0)
                {
                    return fuelCardLimitCollection;
                }

                DataTable table = set.Tables[0];

                while (table.Rows.Count != 0)
                {
                    fuelCardLimit = new FuelCardLimit();
                    fuelCardLimit.Load(set);
                    fuelCardLimitCollection.Add(fuelCardLimit);
                    table.Rows.RemoveAt(0);
                }
            }

            return fuelCardLimitCollection;
        }

		/// <summary>
		/// This method will return a list of fuel card limits for a given fuel card.
		/// Currently, there should only be one.
		/// </summary>
		/// <param name="security">Secuity object.</param>
		/// <param name="fuelCardGuid">Fuel card GUID</param>
		/// <returns>A list of fuel card limits.</returns>
	    public FuelCardLimit EnumerateFuelCardLimitMappingsByFuelCardGuid(SecurityClass security, Guid fuelCardGuid)
		{
			FuelCardLimit associatedFuelCardLimit;

			if (security.Equals(null))
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_FUEL_CARD_DATA) && !security.HasRight(RIGHT.MODIFY_FUEL_CARD_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			if (fuelCardGuid.Equals(Guid.Empty))
			{
				throw new ArgumentException("fuelCardGuid");
			}

			using (var sqlCommand = new SqlCommand())
			{
				FuelCardLimit.EnumerateFuelCardLimitMappingsByFuelCardGuidSql(sqlCommand, security.SiteGuid, fuelCardGuid);
				DataSet set = this.ConsolidatedDA.GetDataSet(sqlCommand, security);

				if (set.Tables.Count <= 0)
				{
					return null;
				}

				associatedFuelCardLimit = new FuelCardLimit();

				if (associatedFuelCardLimit.Load(set) == false)
				{
					return null;
				}
			}

			return associatedFuelCardLimit;
	    }

	    /// <summary>
        /// Get the fuel cards associated with a particular Fuel Card Limit
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="fuelCardLimitGuid">Identifies the fuel card limit to retrieve the associated fuel cards for</param>
        /// <returns>The fuel cards associated with a particular Fuel Card Limit.</returns>
        public List<FuelCardClass> EnumerateFuelCardMappings(SecurityClass security, Guid fuelCardLimitGuid)
        {
            List<FuelCardClass> associatedFuelCardCollection = new List<FuelCardClass>();

            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.VIEW_FUEL_CARD_LIMIT) && !security.HasRight(RIGHT.MODIFY_FUEL_CARD_LIMIT))
            {
                throw new FMInsufficientRightsException();
            }

            if (fuelCardLimitGuid == Guid.Empty)
            {
                throw new ArgumentException("fuelCardLimitGuid");
            }

            using (SqlCommand cmd = new SqlCommand())
            {
                FuelCardLimit.EnumerateFuelCardMappingsSQL(cmd, security.SiteGuid, fuelCardLimitGuid);

                DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);

                if (set.Tables.Count <= 0)
                {
                    return associatedFuelCardCollection;
                }

                DataTable table = set.Tables[0];

                while (table.Rows.Count != 0)
                {
                    DataRow row = table.Rows[0];
                    FuelCardClass associatedFuelCard = new FuelCardClass
                                                           {
                                                               IdentityGuid = DataObject.getValue(row["FuelCardGuid"], Guid.Empty),
                                                               ID = DataObject.getValue(row["ID"], string.Empty),
                                                               ManagerID = DataObject.getValue(row["ManagerID"], string.Empty),
                                                               BillToID = DataObject.getValue(row["BillToID"], string.Empty)
                                                           };

                    associatedFuelCardCollection.Add(associatedFuelCard);
                    table.Rows.RemoveAt(0);
                }
            }

            return associatedFuelCardCollection;
        }

		/// <summary>
		/// This method will add a new fuel card to fuel card limit mapping.
		/// </summary>
		/// <param name="security">Security object.</param>
		/// <param name="fuelCard">The fuel card object.</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void AddFuelCardToFuelCardLimitMapping(SecurityClass security, FuelCardClass fuelCard)
		{
			if (fuelCard.FuelCardLimit != null)
			{
				this.AddFuelCardMapping(security, fuelCard.FuelCardLimit.IdentityGuid, fuelCard.IdentityGuid);
			}
		}

	    /// <summary>
	    /// This method will modify an existing fuel card to fuel card limit mapping.
	    /// </summary>
	    /// <param name="security">Security object.</param>
	    /// <param name="oldFuelCardLimitGuid">The old fuel card limit GUID.</param>
	    /// <param name="fuelCard">The fuel card object.</param>
	    [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void ModifyFuelCardToFuelCardLimitMapping(SecurityClass security, Guid oldFuelCardLimitGuid, FuelCardClass fuelCard)
	    {
			this.DeleteFuelCardMapping(security, oldFuelCardLimitGuid, fuelCard.IdentityGuid);
			this.AddFuelCardToFuelCardLimitMapping(security, fuelCard);
	    }

	    /// <summary>
        /// Add a mapping between a fuel card limit and a fuel card to the database
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="fuelCardLimitGuid">Identifies the fuel card limit to add a mapping for</param>
        /// <param name="fuelCardGuid">Identifies the fuel card to add a mapping for</param>
        private void AddFuelCardMapping(SecurityClass security, Guid fuelCardLimitGuid, Guid fuelCardGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.MODIFY_FUEL_CARD_LIMIT))
            {
                throw new FMInsufficientRightsException();
            }

            if (fuelCardLimitGuid == Guid.Empty)
            {
                throw new ArgumentException("fuelCardLimitGuid");
            }

            if (fuelCardGuid == Guid.Empty)
            {
                throw new ArgumentException("fuelCardGuid");
            }

            // Make sure that there is not already a fuel card limit assigned to or owned by this site
            // with the same ID 
            if (this.GetFuelCardMappingIdentityGuid(security, fuelCardLimitGuid, fuelCardGuid) != Guid.Empty)
            {
                throw new Exception("A Fuel Card Limit mapping record already exists for the specified fuel card limit and fuel card");
            }

            Guid fuelCardLimitToFuelCardGuid = Guid.NewGuid();

            using (SqlCommand cmd = new SqlCommand())
            {
                FuelCardLimit.AddFuelCardMappingSQL(cmd, fuelCardLimitToFuelCardGuid, fuelCardLimitGuid, fuelCardGuid, security.UserID);
                this.ConsolidatedDA.ExecuteQuery(security, cmd);
            }
        }

        /// <summary>
        /// Delete a mapping between a fuel card limit and a fuel card in the database
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="fuelCardLimitGuid">Identifies the fuel card limit to delete a mapping for</param>
        /// <param name="fuelCardGuid">Identifies the fuel card to delete a mapping for</param>
        private void DeleteFuelCardMapping(SecurityClass security, Guid fuelCardLimitGuid, Guid fuelCardGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.MODIFY_FUEL_CARD_LIMIT))
            {
                throw new FMInsufficientRightsException();
            }

            if (fuelCardLimitGuid == Guid.Empty)
            {
                throw new ArgumentException("fuelCardLimitGuid");
            }

            if (fuelCardGuid == Guid.Empty)
            {
                throw new ArgumentException("fuelCardGuid");
            }

            using (SqlCommand cmd = new SqlCommand())
            {
                FuelCardLimit.DeleteFuelCardMappingSQL(cmd, fuelCardLimitGuid, fuelCardGuid);
                this.ConsolidatedDA.ExecuteQuery(security, cmd);
            }
        }

        /// <summary>
        /// Retrieve the mapping between the identified fuel card and fuel card limit
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="fuelCardLimitGuid">Identifies the fuel card limit to retrieve the mapping for</param>
        /// <param name="fuelCardGuid">Identifies the fuel card to retrieve the mapping for</param>
        /// <returns>The mapping between the identified fuel card and fuel card limit, or null if none exists</returns>
        private Guid GetFuelCardMappingIdentityGuid(SecurityClass security, Guid fuelCardLimitGuid, Guid fuelCardGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.VIEW_FUEL_CARD_LIMIT) && !security.HasRight(RIGHT.MODIFY_FUEL_CARD_LIMIT))
            {
                throw new FMInsufficientRightsException();
            }

            if (fuelCardLimitGuid == Guid.Empty)
            {
                throw new ArgumentException("fuelCardLimitGuid");
            }

            if (fuelCardGuid == Guid.Empty)
            {
                throw new ArgumentException("fuelCardGuid");
            }

            using (SqlCommand cmd = new SqlCommand())
            {
                FuelCardLimit.GetFuelCardMappingSQL(cmd, fuelCardLimitGuid, fuelCardGuid);
                DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);

                if (set != null)
                {
                    DataTable table = set.Tables[0];

                    // There should be at most one match since the combination for FuelCardLimitGuid and FuelCardGuid should be unique in map.tblFuelCardLimitToFuelCard
                    if (table.Rows.Count != 0)
                    {
                        DataRow row = table.Rows[0];

                        return DataObject.getValue(row["FuelCardLimitToFuelCardGuid"], Guid.Empty);
                    }
                }
            }

            return Guid.Empty;
        }

        /// <summary>
        /// Check to make sure the fuel card limit is valid
        /// </summary>
        /// <param name="fuelCardLimit">The fuel card limit to check</param>
        private void Validate(FuelCardLimit fuelCardLimit)
        {
            if (string.IsNullOrEmpty(fuelCardLimit.ID))
            {
                throw new Exception("ID must be provided for a Fuel Card Limit");
            }
        }

        /// <summary>
        /// Determine which line items need to be added, modified, or deleted by comparing
        /// the line items of the existing version of the fuel card limit and the new version of the fuel card limit
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="oldFuelCardLimit">The existing version of the fuel card limit in the database</param>
        /// <param name="newFuelCardLimit">The new version of the fuel card limit</param>
        private void UpdateLineItems(SecurityClass security, FuelCardLimit oldFuelCardLimit, FuelCardLimit newFuelCardLimit)
        {
            FuelCardLimitLineItems lineItemsServiceClass = new FuelCardLimitLineItems();

            // If the new fuel card limit parameter was null, that means we're deleting the fuel card limit.
            // If it's not null, that means we may have to add, update, or delete line items.
            if (newFuelCardLimit != null)
            {
                foreach (FuelCardLimitLineItem fuelCardLimitLineItem in newFuelCardLimit.LineItems)
                {
                    // If the line item belonging to the new fuel card limit has no identity guid, 
                    // it is new and needs to be added to the database
                    if (fuelCardLimitLineItem.IdentityGuid == Guid.Empty)
                    {
                        fuelCardLimitLineItem.FuelCardLimitGuid = newFuelCardLimit.IdentityGuid;
                        lineItemsServiceClass.Add(security, fuelCardLimitLineItem);
                    }
                    else if (oldFuelCardLimit != null)
                    {
                        // If we can find a line item with the same identity guid in the old fuel card limit's set of line item, 
                        // the line item needs to be updated. After the update we remove it from the old fuel card limit so we know not to delete it later
                        foreach (FuelCardLimitLineItem oldFuelCardLimitLineItem in oldFuelCardLimit.LineItems)
                        {
                            if (oldFuelCardLimitLineItem.IdentityGuid == fuelCardLimitLineItem.IdentityGuid)
                            {
                                lineItemsServiceClass.Modify(security, fuelCardLimitLineItem);
                                oldFuelCardLimit.LineItems.Remove(oldFuelCardLimitLineItem);
                                break;
                            }
                        }
                    }
                }
            }

            // Delete any line items that are still present on the old fuel card limit object, 
            // because they weren't found in the new fuel card limit.
            if (oldFuelCardLimit != null)
            {
                foreach (FuelCardLimitLineItem oldFuelCardLimitLineItem in oldFuelCardLimit.LineItems)
                {
                    lineItemsServiceClass.Purge(security, oldFuelCardLimitLineItem.IdentityGuid);
                }
            }
        }

        /// <summary>
        /// Determine which fuel card mappings need to be added, modified, or deleted by comparing
        /// the mappings of the existing version of the fuel card limit and the new version of the fuel card limit
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="oldFuelCardLimit">The existing version of the fuel card limit in the database</param>
        /// <param name="newFuelCardLimit">The new version of the fuel card limit</param>
        private void UpdateFuelCardMappings(SecurityClass security, FuelCardLimit oldFuelCardLimit, FuelCardLimit newFuelCardLimit)
        {
            // If the new fuel card limit parameter was null, that means we're deleting the fuel card limit.
            // If it's not null, that means we may have to add, update, or delete mappings.
            if (newFuelCardLimit != null)
            {
                foreach (FuelCardClass newMappedFuelCard in newFuelCardLimit.AssignedFuelCards)
                {
                    bool matchFound = false;

                    if (oldFuelCardLimit != null)
                    {
                        if (oldFuelCardLimit.AssignedFuelCards.Find(
                                matchingMap => matchingMap.IdentityGuid == newMappedFuelCard.IdentityGuid) != null)
                        {
                            matchFound = true;
                        }
                    }

                    if (!matchFound)
                    {
                        this.AddFuelCardMapping(security, newFuelCardLimit.IdentityGuid, newMappedFuelCard.IdentityGuid);
                    }
                }
            }

            // Delete any line items that are still present on the old fuel card limit object, 
            // because they weren't found in the new fuel card limit.
            if (oldFuelCardLimit != null)
            {
                foreach (FuelCardClass oldMappedFuelCard in oldFuelCardLimit.AssignedFuelCards)
                {
                    bool matchFound = false;
                    if (newFuelCardLimit != null)
                    {
                        if (newFuelCardLimit.AssignedFuelCards.Find(
                                matchingMap => matchingMap.IdentityGuid == oldMappedFuelCard.IdentityGuid) != null)
                        {
                            matchFound = true;
                        }
                    }

                    if (!matchFound)
                    {
                        this.DeleteFuelCardMapping(security, oldFuelCardLimit.IdentityGuid, oldMappedFuelCard.IdentityGuid);
                    }
                }
            }
        }


        public List<FuelCardLimit> EnumerateForMobile3(SecurityClass security)
        {
            throw new NotImplementedException();
        }
    }
}