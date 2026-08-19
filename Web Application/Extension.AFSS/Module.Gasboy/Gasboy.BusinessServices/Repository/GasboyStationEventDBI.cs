// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GasboyStationEventDBI.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// Implements database interaction for the gasboy station event functionality
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
    using FMBusinessServices.InternalClasses;

    using FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects;

    using Microsoft.SqlServer.Server;

    /// <summary>
    /// Implements database interaction for the gasboy station event functionality
    /// </summary>
    public class GasboyStationEventDBI
    {
        /// <summary>
        /// Provides database access
        /// </summary>
        private static readonly ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

        /// <summary>
        /// Add one or more gasboy event records to the database
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="gasboyStationEvents">The event records to add to the database</param>
        public static void Insert(SecurityClass security, List<GasboyStationEvent> gasboyStationEvents)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_GasboyStationEventInsert";

                SqlParameter tableValuedParameter = cmd.Parameters.Add("@GasboyStationEvents", SqlDbType.Structured);
                tableValuedParameter.Value = CreateSqlDataRecords(gasboyStationEvents);
                tableValuedParameter.TypeName = "dbo.GasboyStationEventType";

                ConsolidatedDA.ExecuteQuery(security, cmd);
            }
        }

        /// <summary>
        /// Get a event record identified by the provided guid from the database
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="externalStationLogGuid">Identifies the event record to retrieve</param>
        /// <returns>The event record identified by the provided guid from the database, or null if it was not found</returns>
        public static GasboyStationEvent Get(SecurityClass security, Guid externalStationLogGuid)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_GasboyStationEventGet";

                cmd.Parameters.Add("@ExternalStationLogGuid", SqlDbType.UniqueIdentifier).Value = externalStationLogGuid;

                DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

                if (set.Tables.Count == 0 || set.Tables[0].Rows.Count == 0)
                {
                    return null;
                }

                return LoadObjectFromDataRow(set.Tables[0].Rows[0]);
            }
        }

        /// <summary>
        /// Create sqlDataRecords corresponding to the provided event records for use by the insert stored procedure
        /// </summary>
        /// <param name="gasboyStationEvents">event records to add to the database</param>
        /// <returns>sqlDataRecords corresponding to the provided event records for use by the insert stored procedure</returns>
        private static IEnumerable<SqlDataRecord> CreateSqlDataRecords(IEnumerable<GasboyStationEvent> gasboyStationEvents)
        {
            SqlMetaData[] metaData = new SqlMetaData[18];

            int i = 0;
            metaData[i++] = new SqlMetaData("GasboyStationEventGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("ExternalStationLogGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("EventID", SqlDbType.Int);
            metaData[i++] = new SqlMetaData("LookupGasboyEventErrorClassCodeIndex", SqlDbType.Int);
            metaData[i++] = new SqlMetaData("ErrorCode", SqlDbType.Int);
            metaData[i++] = new SqlMetaData("FleetID", SqlDbType.Int);
            metaData[i++] = new SqlMetaData("ObjectID", SqlDbType.Int);
            metaData[i++] = new SqlMetaData("LookupGasboyEventObjectTypeIndex", SqlDbType.Int);
            metaData[i++] = new SqlMetaData("DeviceName", SqlDbType.NVarChar, 100);
            metaData[i++] = new SqlMetaData("Field1", SqlDbType.NVarChar, 100);
            metaData[i++] = new SqlMetaData("Field2", SqlDbType.NVarChar, 100);
            metaData[i++] = new SqlMetaData("Field3", SqlDbType.NVarChar, 100);
            metaData[i++] = new SqlMetaData("Field4", SqlDbType.NVarChar, 100);
            metaData[i++] = new SqlMetaData("Field5", SqlDbType.NVarChar, 100);
            metaData[i++] = new SqlMetaData("Field6", SqlDbType.NVarChar, 100);
            metaData[i++] = new SqlMetaData("Field7", SqlDbType.NVarChar, 100);
            metaData[i++] = new SqlMetaData("Field8", SqlDbType.NVarChar, 100);
            metaData[i] = new SqlMetaData("CreatedUpdatedBy", SqlDbType.NVarChar, 100);

            SqlDataRecord record = new SqlDataRecord(metaData);

            foreach (GasboyStationEvent gasboyStationEvent in gasboyStationEvents)
            {
                int j = 0;

                record.SetGuid(j++, gasboyStationEvent.IdentityGuid);
                record.SetGuid(j++, gasboyStationEvent.ExternalStationLogGuid);
                record.SetNullableInt(j++, gasboyStationEvent.EventID);
                record.SetNullableInt(j++, (int?)gasboyStationEvent.ErrorClassCode);
                record.SetNullableInt(j++, (int?)gasboyStationEvent.ErrorCode);
                record.SetNullableInt(j++, gasboyStationEvent.FleetID);
                record.SetNullableInt(j++, gasboyStationEvent.ObjectID);
                record.SetNullableInt(j++, (int?)gasboyStationEvent.EventObjectType);
                record.SetNullableString(j++, gasboyStationEvent.DeviceName);
                record.SetNullableString(j++, gasboyStationEvent.Field1);
                record.SetNullableString(j++, gasboyStationEvent.Field2);
                record.SetNullableString(j++, gasboyStationEvent.Field3);
                record.SetNullableString(j++, gasboyStationEvent.Field4);
                record.SetNullableString(j++, gasboyStationEvent.Field5);
                record.SetNullableString(j++, gasboyStationEvent.Field6);
                record.SetNullableString(j++, gasboyStationEvent.Field7);
                record.SetNullableString(j++, gasboyStationEvent.Field8);
                record.SetString(j, gasboyStationEvent.CreatedBy);

                yield return record;
            }
        }

        /// <summary>
        /// Load a gasboy event record from a dataRow read from the database
        /// </summary>
        /// <param name="row">The dataRow to read log information from</param>
        /// <returns>A populated GasboyStationEvent object</returns>
        private static GasboyStationEvent LoadObjectFromDataRow(DataRow row)
        {
            GasboyStationEvent gasboyStationEvent = new GasboyStationEvent();

            gasboyStationEvent.IdentityGuid = DataObject.getValue(row["ExternalStationLogGuid"], Guid.Empty);
            gasboyStationEvent.SiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty);
            gasboyStationEvent.ID = DataObject.getValue(row["LogText"], string.Empty);
            gasboyStationEvent.ExternalStationGuid = DataObject.getValue(row["ExternalStationGuid"], Guid.Empty);
            gasboyStationEvent.ExternalStationID = DataObject.getValue(row["ExternalStationID"], string.Empty);
            gasboyStationEvent.LogType = DataObject.getValue(row["LookupExternalStationLogTypeIndex"], ExternalStationLogType.ConnectionFailure);
            gasboyStationEvent.IdentityGuid = DataObject.getValue(row["GasboyStationEventGuid"], Guid.Empty);
            gasboyStationEvent.ExternalStationLogGuid = DataObject.getValue(row["ExternalStationLogGuid"], Guid.Empty);
            gasboyStationEvent.EventID = DataObject.getValue<int?>(row["EventID"], null);
            gasboyStationEvent.ErrorClassCode = (GasboyEventErrorClassCode?)DataObject.getOptionalInt(row["LookupGasboyEventErrorClassCodeIndex"]);
            gasboyStationEvent.ErrorCode = (ErrorCode)DataObject.getOptionalInt(row["ErrorCode"]);
            gasboyStationEvent.FleetID = DataObject.getValue<int?>(row["FleetID"], null);
            gasboyStationEvent.ObjectID = DataObject.getValue<int?>(row["ObjectID"], null);
            gasboyStationEvent.EventObjectType = (GasboyEventObjectType?)DataObject.getOptionalInt(row["LookupGasboyEventObjectTypeIndex"]);
            gasboyStationEvent.DeviceName = DataObject.getValue(row["DeviceName"], string.Empty);
            gasboyStationEvent.Field1 = DataObject.getValue(row["Field1"], string.Empty);
            gasboyStationEvent.Field2 = DataObject.getValue(row["Field2"], string.Empty);
            gasboyStationEvent.Field3 = DataObject.getValue(row["Field3"], string.Empty);
            gasboyStationEvent.Field4 = DataObject.getValue(row["Field4"], string.Empty);
            gasboyStationEvent.Field5 = DataObject.getValue(row["Field5"], string.Empty);
            gasboyStationEvent.Field6 = DataObject.getValue(row["Field6"], string.Empty);
            gasboyStationEvent.Field7 = DataObject.getValue(row["Field7"], string.Empty);
            gasboyStationEvent.Field8 = DataObject.getValue(row["Field8"], string.Empty);
            gasboyStationEvent.CreatedDate = DataObject.getValue(row["CreatedDate"], DateTimeOffset.Now);
            gasboyStationEvent.CreatedBy = DataObject.getValue(row["CreatedBy"], BaseDataObject.ADMIN);
            gasboyStationEvent.UpdatedDate = DataObject.getValue(row["UpdatedDate"], DateTimeOffset.Now);
            gasboyStationEvent.UpdatedBy = DataObject.getValue(row["UpdatedBy"], BaseDataObject.ADMIN);

            return gasboyStationEvent;
        }

    }
}