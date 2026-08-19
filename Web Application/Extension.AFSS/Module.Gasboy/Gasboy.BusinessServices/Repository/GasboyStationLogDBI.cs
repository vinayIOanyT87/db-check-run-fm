// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GasboyStationLogDBI.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//  Implements methods for the Gasboy Station Log functionality that interact with the database 
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

    using FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects;

    using Microsoft.SqlServer.Server;

    /// <summary>
    /// Implements methods for the Gasboy Station Log functionality that interact with the database 
    /// </summary>
    public static class GasboyStationLogDBI
    {
        /// <summary>
        /// Provides database access
        /// </summary>
        private static readonly ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

        /// <summary>
        /// Get a log record identified by the provided guid from the database
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="externalStationLogGuid">Identifies the log record to retrieve</param>
        /// <returns>The log record identified by the provided guid from the database, or null if it was not found</returns>
        public static GasboyStationLog Get(SecurityClass security, Guid externalStationLogGuid)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_ExternalStationLogGet";

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
        /// Search the database for log records identified by the provided parameters
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="siteGuid">The site to retrieve log records for</param>
        /// <param name="externalStationGuid">The station to get logs for, or Guid.Empty for all stations</param>
        /// <param name="beginDate">The beginning date to get logs for</param>
        /// <param name="endDate">The end date to get logs for</param>
        /// <param name="logType">The type of logs to get, or null for all types</param>
        /// <returns>External station logs for the site matching the provided search parameters</returns>
        public static List<GasboyStationLog> GetList(
            SecurityClass security,
            Guid siteGuid,
            Guid externalStationGuid,
            DateTimeOffset beginDate,
            DateTimeOffset endDate,
            ExternalStationLogType? logType)
        {
            List<GasboyStationLog> logs = new List<GasboyStationLog>();

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_ExternalStationLogEnumerate";

                cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = siteGuid;

                if (externalStationGuid != Guid.Empty)
                {
                    cmd.Parameters.Add("@ExternalStationGuid", SqlDbType.UniqueIdentifier).Value = externalStationGuid;
                }

                cmd.Parameters.Add("@BeginDate", SqlDbType.DateTimeOffset).Value = beginDate;
                cmd.Parameters.Add("@EndDate", SqlDbType.DateTimeOffset).Value = endDate;

                if (logType.HasValue)
                {
                    cmd.Parameters.Add("@LogType", SqlDbType.Int).Value = logType;
                }

                DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

                if (set.Tables.Count == 0 || set.Tables[0].Rows.Count == 0)
                {
                    return logs;
                }

                foreach (DataRow row in set.Tables[0].Rows)
                {
                    GasboyStationLog gasboyStationLog = new GasboyStationLog
                    {
                        IdentityGuid = DataObject.getValue(row["ExternalStationLogGuid"], Guid.Empty),
                        SiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty),
                        ID = DataObject.getValue(row["LogText"], string.Empty),
                        ExternalStationGuid = DataObject.getValue(row["ExternalStationGuid"], Guid.Empty),
                        ExternalStationID = DataObject.getValue(row["ExternalStationID"], string.Empty),
                        LogType = DataObject.getValue(row["LookupExternalStationLogTypeIndex"], ExternalStationLogType.ConnectionFailure),
                        LogDate = DataObject.getValue(row["LogDate"], DateTimeOffset.Now)
                    };

							gasboyStationLog.ID = Convert.ToString((ErrorCode) Enum.Parse(typeof (ErrorCode), gasboyStationLog.ID)); //parse the description from the error code to display to the user

                    logs.Add(gasboyStationLog);
                }
            }

            return logs;
        }

        /// <summary>
        /// Add one or more gasboy log records to the database
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="logs">The log records to add to the database</param>
        public static void Insert(SecurityClass security, List<GasboyStationLog> logs)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_ExternalStationLogInsert";

                SqlParameter tableValuedParameter = cmd.Parameters.Add("@ExternalStationLogs", SqlDbType.Structured);
                tableValuedParameter.Value = CreateSqlDataRecords(logs);
                tableValuedParameter.TypeName = "dbo.ExternalStationLogType";

                ConsolidatedDA.ExecuteQuery(security, cmd);
            }
        }

        /// <summary>
        /// Remove gasboy log records older than the provided number of days from the database
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="maximumDaysToRetainLogs">Logs older than this number of days will be deleted</param>
        public static void Purge(SecurityClass security, int maximumDaysToRetainLogs)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_ExternalStationLogDelete";

                cmd.Parameters.Add("@MaximumDaysToRetainLogs", SqlDbType.Int).Value = maximumDaysToRetainLogs;
                ConsolidatedDA.ExecuteQuery(security, cmd);
            }        
        }

        /// <summary>
        /// Create sqlDataRecords corresponding to the provided log records for use by the insert stored procedure
        /// </summary>
        /// <param name="externalStationLogs">Log records to add to the database</param>
        /// <returns>sqlDataRecords corresponding to the provided log records for use by the insert stored procedure</returns>
        private static IEnumerable<SqlDataRecord> CreateSqlDataRecords(IEnumerable<GasboyStationLog> externalStationLogs)
        {
            SqlMetaData[] metaData = new SqlMetaData[7];

            int i = 0;
            metaData[i++] = new SqlMetaData("ExternalStationLogGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("SiteGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("ExternalStationGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("LogText", SqlDbType.NVarChar, -1);
            metaData[i++] = new SqlMetaData("LookupExternalStationLogTypeIndex", SqlDbType.Int);
            metaData[i++] = new SqlMetaData("LogDate", SqlDbType.DateTimeOffset);
            metaData[i] = new SqlMetaData("CreatedUpdatedBy", SqlDbType.NVarChar, 100);

            SqlDataRecord record = new SqlDataRecord(metaData);

            foreach (GasboyStationLog externalStationLog in externalStationLogs)
            {
                int j = 0;

                record.SetGuid(j++, externalStationLog.IdentityGuid);
                record.SetGuid(j++, externalStationLog.SiteGuid);
                record.SetGuid(j++, externalStationLog.ExternalStationGuid);
                record.SetString(j++, externalStationLog.ID);
                record.SetInt32(j++, (int)externalStationLog.LogType);
                record.SetDateTimeOffset(j++, externalStationLog.LogDate);
                record.SetString(j, externalStationLog.CreatedBy);

                yield return record;
            }
        }

        /// <summary>
        /// Load a gasboy log record from a dataRow read from the database
        /// </summary>
        /// <param name="row">The dataRow to read log information from</param>
        /// <returns>A populated GasboyStationLog object</returns>
        private static GasboyStationLog LoadObjectFromDataRow(DataRow row)
        {
            GasboyStationLog log = new GasboyStationLog();

            log.IdentityGuid = DataObject.getValue(row["ExternalStationLogGuid"], Guid.Empty);
            log.SiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty);
            log.ID = DataObject.getValue(row["LogText"], string.Empty);
            log.ExternalStationGuid = DataObject.getValue(row["ExternalStationGuid"], Guid.Empty);
            log.ExternalStationID = DataObject.getValue(row["ExternalStationID"], string.Empty);
            log.LogType = DataObject.getValue(row["LookupExternalStationLogTypeIndex"], ExternalStationLogType.ConnectionFailure);
            log.LogDate = DataObject.getValue(row["LogDate"], DateTimeOffset.Now);
            log.CreatedDate = DataObject.getValue(row["CreatedDate"], DateTimeOffset.Now);
            log.CreatedBy = DataObject.getValue(row["CreatedBy"], BaseDataObject.ADMIN);
            log.UpdatedDate = DataObject.getValue(row["UpdatedDate"], DateTimeOffset.Now);
            log.UpdatedBy = DataObject.getValue(row["UpdatedBy"], BaseDataObject.ADMIN);

            return log;
        }
    }
}