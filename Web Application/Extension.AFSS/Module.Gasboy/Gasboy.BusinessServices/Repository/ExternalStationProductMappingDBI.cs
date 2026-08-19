// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GasboyStationProductMappingDBI.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// Implements database interaction for the external station product mapping functionality
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Afss.Module.Gasboy.BusinessServices.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    using FMBusinessObjects.DataObjects;

    using FMBusinessServices.DataAccessLayer;

    using FuelsManager.Afss.BusinessObjects.DataObjects;
    using FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects;

    /// <summary>
    /// Implements database interaction for the external station product mapping functionality
    /// </summary>
    public static class GasboyStationProductMappingDBI
    {
        /// <summary>
        /// Provides database access
        /// </summary>
        private static readonly ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

        /// <summary>
        /// Get an external station product mapping identified by the provided guid from the database
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="externalStationToProductGuid">Identifies the external station product mapping record to retrieve</param>
        /// <returns>The external station product mapping identified by the provided guid from the database, or null if it was not found</returns>
        public static GasboyStationProductMapping Get(SecurityClass security, Guid externalStationToProductGuid)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "map.usp_ExternalStationToProductGet";

                cmd.Parameters.Add("@ExternalStationToProductGuid", SqlDbType.UniqueIdentifier).Value = externalStationToProductGuid;

                DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

                if (set.Tables.Count == 0 || set.Tables[0].Rows.Count == 0)
                {
                    return null;
                }

                return LoadObjectFromDataRow(set.Tables[0].Rows[0]);
            }
        }

        /// <summary>
        /// Get an external station product mapping record owned by the provided external station and that has the provided ID
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="externalStationGuid">Identifies an external station to search for a product mapping for</param>
        /// <param name="id">The ID of the product mapping to search for - ID is the external station's product</param>
        /// <returns>The external station product mapping record identified by the provided parameters from the database, or null if it was not found</returns>
        public static GasboyStationProductMapping GetByExternalStationAndProductID(SecurityClass security, Guid externalStationGuid, string id)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "map.usp_ExternalStationToProductGet";

                cmd.Parameters.Add("@ExternalStationGuid", SqlDbType.UniqueIdentifier).Value = externalStationGuid;
                cmd.Parameters.Add("@ExternalStationProduct", SqlDbType.NVarChar, 50).Value = id;

                DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

                if (set.Tables.Count == 0 || set.Tables[0].Rows.Count == 0)
                {
                    return null;
                }

                return LoadObjectFromDataRow(set.Tables[0].Rows[0]);
            }
        }

        /// <summary>
        /// Get all the external station product mappings for the external station identified by the provided guid
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="externalStationGuid">Identifies the external station to retrieve product mappings for</param>
        /// <returns>All of the external station product mappings for the external station identified by the provided guid</returns>
        public static List<GasboyStationProductMapping> GetList(SecurityClass security, Guid externalStationGuid)
        {
            List<GasboyStationProductMapping> productMappings = new List<GasboyStationProductMapping>();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "map.usp_ExternalStationToProductEnumerate";

                cmd.Parameters.Add("@ExternalStationGuid", SqlDbType.UniqueIdentifier).Value = externalStationGuid;

                DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

                if (set.Tables.Count == 0 || set.Tables[0].Rows.Count == 0)
                {
                    return productMappings;
                }

                foreach (DataRow row in set.Tables[0].Rows)
                {
                    productMappings.Add(LoadObjectFromDataRow(row));
                }

                return productMappings;
            }
        }

        /// <summary>
        /// Add an external station product mapping to the database
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="productMapping">Product mappings to save to the database</param>
        public static void Insert(SecurityClass security, GasboyStationProductMapping productMapping)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "map.usp_ExternalStationToProductInsert";

                cmd.Parameters.Add("@ExternalStationToProductGuid", SqlDbType.UniqueIdentifier).Value = productMapping.IdentityGuid;
                cmd.Parameters.Add("@CreatedUpdatedBy", SqlDbType.NVarChar, 100).Value = productMapping.CreatedBy;

                AddCommonInsertUpdateParameters(cmd, productMapping);

                ConsolidatedDA.ExecuteQuery(security, cmd);
            }
        }

        /// <summary>
        /// Modify an external station product mapping record in the database
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="productMapping">Identifies the product mapping record to modify </param>
        public static void Modify(SecurityClass security, GasboyStationProductMapping productMapping)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "map.usp_ExternalStationToProductUpdate";

                cmd.Parameters.Add("@ExternalStationToProductGuid", SqlDbType.UniqueIdentifier).Value = productMapping.IdentityGuid;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100).Value = productMapping.UpdatedBy;

                AddCommonInsertUpdateParameters(cmd, productMapping);

                ConsolidatedDA.ExecuteQuery(security, cmd);
            }
        }

        /// <summary>
        /// Delete an external station product mapping record in the database identified by the provided guid from the database
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="externalStationToProductGuid">Identifies the external station product mapping record to delete</param>
        public static void Purge(SecurityClass security, Guid externalStationToProductGuid)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "map.usp_ExternalStationToProductDelete";

                cmd.Parameters.Add("@ExternalStationToProductGuid", SqlDbType.UniqueIdentifier).Value = externalStationToProductGuid;
                ConsolidatedDA.ExecuteQuery(security, cmd);
            }
        }

        /// <summary>
        /// Load an external station product mapping record from a dataRow read from the database
        /// </summary>
        /// <param name="row">The dataRow to read external station product mapping information from</param>
        /// <returns>A populated GasboyStationProductMapping object</returns>
        private static GasboyStationProductMapping LoadObjectFromDataRow(DataRow row)
        {
            GasboyStationProductMapping productMapping = new GasboyStationProductMapping();

            productMapping.IdentityGuid = DataObject.getValue(row["ExternalStationToProductGuid"], Guid.Empty);
            productMapping.ExternalStationGuid = DataObject.getValue(row["ExternalStationGuid"], Guid.Empty);
            productMapping.ID = DataObject.getValue(row["ExternalStationProduct"], string.Empty);
            productMapping.FuelsManagerProductID = DataObject.getValue(row["ProductID"], string.Empty);
            productMapping.FuelsManagerProductMasterRecordGuid = DataObject.getValue(row["ProductGuid"], Guid.Empty);
            productMapping.CreatedDate = DataObject.getValue(row["CreatedDate"], DateTimeOffset.Now);
            productMapping.CreatedBy = DataObject.getValue(row["CreatedBy"], BaseDataObject.ADMIN);
            productMapping.UpdatedDate = DataObject.getValue(row["UpdatedDate"], DateTimeOffset.Now);
            productMapping.UpdatedBy = DataObject.getValue(row["UpdatedBy"], BaseDataObject.ADMIN);

            return productMapping;
        }

        /// <summary>
        /// Add parameters that are used by both the insert and update stored procedures
        /// </summary>
        /// <param name="cmd">A SqlCommand to add parameters to</param>
        /// <param name="productMapping">Contains values for the insert and update stored procedures</param>
        private static void AddCommonInsertUpdateParameters(SqlCommand cmd, GasboyStationProductMapping productMapping)
        {
            cmd.Parameters.Add("@ExternalStationProduct", SqlDbType.NVarChar, 50).Value = productMapping.ID;
            cmd.Parameters.Add("@ExternalStationGuid", SqlDbType.UniqueIdentifier).Value = productMapping.ExternalStationGuid;
            cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier).Value = productMapping.FuelsManagerProductMasterRecordGuid;
        }
    }
}