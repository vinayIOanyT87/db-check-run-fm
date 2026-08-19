// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FuelCardLimitLineItems.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// Implements operations to manipulate Fuel Card Limit line items in the database
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Exceptions;

    using FMBusinessServices.DataAccessLayer;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.ServiceModel;

    /// <summary>
    /// Implements operations to manipulate Fuel Card Limit line items in the database
    /// </summary>
    [ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
    public class FuelCardLimitLineItems
    {
        /// <summary>
        /// Allows database access.
        /// </summary>
        internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

        /// <summary>
        /// Add a fuel card limit line item to the database
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="fuelCardLimitLineItem">The line item to add</param>
        /// <returns>The identity guid (Primary Key) of the new line item record</returns>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public Guid Add(SecurityClass security, FuelCardLimitLineItem fuelCardLimitLineItem)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.MODIFY_FUEL_CARD_LIMIT))
            {
                throw new FMInsufficientRightsException();
            }

            if (fuelCardLimitLineItem == null)
            {
                throw new ArgumentNullException("fuelCardLimitLineItem");
            }

            this.Validate(security, fuelCardLimitLineItem);

            fuelCardLimitLineItem.IdentityGuid = Guid.NewGuid();
            fuelCardLimitLineItem.CreatedBy = security.UserID;

            using (SqlCommand cmd = new SqlCommand())
            {
                fuelCardLimitLineItem.InsertSQL(cmd);
                this.ConsolidatedDA.ExecuteQuery(security, cmd);
            }

            return fuelCardLimitLineItem.IdentityGuid;
        }

        /// <summary>
        /// Update an existing fuel card limit line item in the database
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="fuelCardLimitLineItem">The line item to update</param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Modify(SecurityClass security, FuelCardLimitLineItem fuelCardLimitLineItem)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (fuelCardLimitLineItem == null)
            {
                throw new ArgumentNullException("fuelCardLimitLineItem");
            }

            if (!security.HasRight(RIGHT.MODIFY_FUEL_CARD_LIMIT))
            {
                throw new FMInsufficientRightsException();
            }

            this.Validate(security, fuelCardLimitLineItem);

            fuelCardLimitLineItem.UpdatedBy = security.UserID;

            using (SqlCommand cmd = new SqlCommand())
            {
                fuelCardLimitLineItem.UpdateSQL(cmd);
                this.ConsolidatedDA.ExecuteQuery(security, cmd);
            }
        }

        /// <summary>
        /// Remove an existing Fuel Card Limit Line item from the database
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="fuelCardLimitLineItemGuid">Identifies the fuel card limit line item to delete</param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Purge(SecurityClass security, Guid fuelCardLimitLineItemGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.MODIFY_FUEL_CARD_LIMIT))
            {
                throw new FMInsufficientRightsException();
            }

            FuelCardLimitLineItem fuelCardLimitLineItem = this.Get(security, fuelCardLimitLineItemGuid);

            if (fuelCardLimitLineItem.IdentityGuid != Guid.Empty)
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    fuelCardLimitLineItem.PurgeSQL(cmd);
                    this.ConsolidatedDA.ExecuteQuery(security, cmd);
                }
            }
        }

        /// <summary>
        /// Retrieve an existing fuel card limit line item from the database
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="fuelCardLimitLineItemGuid">Identifies the fuel card limit line item to retrieve</param>
        /// <returns>The fuel card limit line item identified by the provided guid, or null if it was not found</returns>
        public FuelCardLimitLineItem Get(SecurityClass security, Guid fuelCardLimitLineItemGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.VIEW_FUEL_CARD_LIMIT) && !security.HasRight(RIGHT.MODIFY_FUEL_CARD_LIMIT))
            {
                throw new FMInsufficientRightsException();
            }

            if (fuelCardLimitLineItemGuid == Guid.Empty)
            {
                throw new ArgumentException("fuelCardLimitLineItemGuid");
            }

            SitesClass sites = new SitesClass();
            SiteClass site = sites.Get(security, security.SiteGuid, false, false, false);
            FuelCardLimitLineItem fuelCardLimitLineItem = new FuelCardLimitLineItem(site) { IdentityGuid = fuelCardLimitLineItemGuid };

            using (SqlCommand cmd = new SqlCommand())
            {
                fuelCardLimitLineItem.SelectSQL(security, cmd);

                if (fuelCardLimitLineItem.Load(this.ConsolidatedDA.GetDataSet(cmd, security)))
                {
                    return fuelCardLimitLineItem;
                }
            }

            return null;
        }

        /// <summary>
        /// Read a fuel card limit line item record from the database by its natural key
        /// (the product or product group it is assigned to and the period)
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="fuelCardLimitGuid">Identifies the fuel card limit a line item is assigned to</param>
        /// <param name="productGuid">Identifies the product a line item is assigned to. Empty if the limit is associated with a product group or all products</param>
        /// <param name="productGroupApplicationStringGuid">Identifies the product group a line item is assigned to. Empty if the limit is associated with a product or all products.</param>
        /// <param name="period">The period associated with the fuel card limit line item</param>
        /// <returns>The fuel card limit line item identified by the provided paramters, or null if no match was found</returns>
        public FuelCardLimitLineItem GetByAssignmentAndPeriod(SecurityClass security, Guid fuelCardLimitGuid, Guid productGuid, Guid productGroupApplicationStringGuid, FuelCardLimitPeriod period)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.VIEW_FUEL_CARD_LIMIT) && !security.HasRight(RIGHT.MODIFY_FUEL_CARD_LIMIT))
            {
                throw new FMInsufficientRightsException();
            }

            SitesClass sites = new SitesClass();
            SiteClass site = sites.Get(security, security.SiteGuid, false, false, false);

            using (SqlCommand cmd = new SqlCommand())
            {
                FuelCardLimitLineItem fuelCardLimitLineItem = new FuelCardLimitLineItem(site)
                                                                  {
                                                                      FuelCardLimitGuid = fuelCardLimitGuid,
                                                                      ProductGuid = productGuid,
                                                                      ProductGroupApplicationStringGuid = productGroupApplicationStringGuid,
                                                                      Period = period
                                                                  };

                fuelCardLimitLineItem.SelectByAssignmentAndPeriodSQL(security, cmd);

                if (fuelCardLimitLineItem.Load(this.ConsolidatedDA.GetDataSet(cmd, security)))
                {
                    return fuelCardLimitLineItem;
                }
            }

            return null;
        }

        /// <summary>
        /// Retrieve all line items belonging to a particular fuel card limit
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="fuelCardLimitGuid">Identifies the fuel card limit to retrieve line items for</param>
        /// <returns>All line items belonging to a particular fuel card limit. The collection will be empty if none are found.</returns>
        public List<FuelCardLimitLineItem> Enumerate(SecurityClass security, Guid fuelCardLimitGuid)
        {
            List<FuelCardLimitLineItem> lineItems = new List<FuelCardLimitLineItem>();

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

            SitesClass sites = new SitesClass();
            SiteClass site = sites.Get(security, security.SiteGuid, false, false, false);

            FuelCardLimitLineItem lineItem = new FuelCardLimitLineItem(site) { FuelCardLimitGuid = fuelCardLimitGuid };

            using (SqlCommand cmd = new SqlCommand())
            {
                lineItem.EnumerateSQL(security, cmd);
                DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);

                if (set.Tables.Count <= 0)
                {
                    return lineItems;
                }

                DataTable table = set.Tables[0];

                while (table.Rows.Count != 0)
                {
                    lineItem = new FuelCardLimitLineItem(site);
                    lineItem.Load(set);
                    lineItems.Add(lineItem);
                    table.Rows.RemoveAt(0);
                }
            }

            return lineItems;
        }

        /// <summary>
        /// Make sure the data in the line item passes validation rules.
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="fuelCardLimitLineItem">The line item to check</param>
        private void Validate(SecurityClass security, FuelCardLimitLineItem fuelCardLimitLineItem)
        {
            if (fuelCardLimitLineItem.ProductGroupApplicationStringGuid != Guid.Empty
                && fuelCardLimitLineItem.ProductGuid != Guid.Empty)
            {
                throw new Exception("A Fuel Card Limit Line Item may not be assigned to both a product and a product group");
            }

            if (fuelCardLimitLineItem.Limit.Value <= 0)
            {
                throw new Exception("A Fuel Card Limit Line Item must have a Limit greater than zero");
            }

            FuelCardLimitLineItem matchingLineItem = this.GetByAssignmentAndPeriod(
                security,
                fuelCardLimitLineItem.FuelCardLimitGuid,
                fuelCardLimitLineItem.ProductGuid,
                fuelCardLimitLineItem.ProductGroupApplicationStringGuid,
                fuelCardLimitLineItem.Period);

            if (matchingLineItem != null && ((fuelCardLimitLineItem.IdentityGuid != Guid.Empty && matchingLineItem.IdentityGuid != fuelCardLimitLineItem.IdentityGuid) ||
                fuelCardLimitLineItem.IdentityGuid == Guid.Empty && matchingLineItem.IdentityGuid != Guid.Empty))
            {
                string errorMessage = string.Format("A duplicate line item was found. A line item's ID (a Product, a Product Group, or All Products) and Period must be unique. The duplicated value was ID: {0} Period: {1}", 
                    matchingLineItem.LineItemType == FuelCardLimitLineItemType.AllProducts ? matchingLineItem.UserFriendlyLineItemType : matchingLineItem.AssignedProductGroupOrProductID, matchingLineItem.Period);

                throw new Exception(errorMessage);
            }
        }
    }
}