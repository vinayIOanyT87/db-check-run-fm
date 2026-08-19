using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMNotificationBusinessObjects.DataObjects;
using FMNotificationBusinessServices.DataAccess;
using FMBusinessServices.DataAccessLayer;
using FMBusinessObjects.DataObjects;

namespace FMNotificationBusinessServices.DataAccess
{
    public class ErrorNotificationConfigDAL
    {
        internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

        private string _connString = string.Empty;
        public ErrorNotificationConfigDAL()
        {
            _connString = ConfigurationManager.AppSettings["ConnectionString"];
        }

        public List<ErrorNotificationConfig> GetErrorNotificationConfigs(SecurityClass sc)
        {
            string sql = @"Select t2.ID as SiteID, t1.EmailAddresses, t1.ErrorFolder, t1.CreatedBy, t1.CreatedDate, 
                t1.UpdatedBy, t1.UpdatedDate 
                from tblErrorNotificationConfigurations t1 
                LEFT OUTER JOIN tblSites t2 ON t1.SiteGuid = t2.SiteGuid 
                WHERE t2.SiteGroupFlag = 0
                order by t2.ID";

            List<ErrorNotificationConfig> configs = new List<ErrorNotificationConfig>();
            try
            {
                //Use this for now since we do not have a login.  Update this once either the service uses a login or 
                //a right has been specifically created for reading tblErrorNotificationConfigurations  
                if (!sc.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS))
                {
                    return configs;
                }

                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    DataTable dtResults = this.ConsolidatedDA.GetDataTable(cmd, sc);

                    foreach(DataRow dr in dtResults.Rows)
                    {
                        ErrorNotificationConfig cfg = new ErrorNotificationConfig();
                        cfg.SiteId = dr["SiteID"] != DBNull.Value ? dr["SiteID"].ToString() : string.Empty;
                        cfg.EmailAddresses = dr["EmailAddresses"] != DBNull.Value ? dr["EmailAddresses"].ToString() : string.Empty;
                        cfg.ErrorFolder = dr["ErrorFolder"] != DBNull.Value ? dr["ErrorFolder"].ToString() : string.Empty;
                        cfg.CreatedBy = dr["CreatedBy"] != DBNull.Value ? dr["CreatedBy"].ToString() : string.Empty;
                        if (dr["CreatedDate"] != DBNull.Value)
                        {
                            cfg.CreatedDate = DateTimeOffset.Parse(dr["CreatedDate"].ToString());
                        }
                        else
                        {
                            cfg.CreatedDate = DateTimeOffset.MinValue;
                        }
                        cfg.UpdatedBy = dr["UpdatedBy"] != DBNull.Value ? dr["UpdatedBy"].ToString() : string.Empty;
                        if (dr["CreatedDate"] != DBNull.Value)
                        {
                            cfg.UpdatedDate = DateTimeOffset.Parse(dr["UpdatedDate"].ToString());
                        }
                        else
                        {
                            cfg.UpdatedDate = DateTimeOffset.MinValue;
                        }

                        configs.Add(cfg);
                    }

                }

                    return configs;
            }
            catch (Exception ex)
            {
                LogError(ex, EventLogEntryType.Error, 3000);
                return configs;
            }
        }

        public ErrorNotificationConfig GetErrorNotificationConfigBySite(string siteId, SecurityClass sc)
        {
            string sql = @"Select t2.ID as SiteID, t1.EmailAddresses, t1.ErrorFolder, t1.CreatedBy, t1.CreatedDate, 
                t1.UpdatedBy, t1.UpdatedDate 
                from tblErrorNotificationConfigurations t1 
                LEFT OUTER JOIN tblSites t2 ON t1.SiteGuid = t2.SiteGuid 
                WHERE t2.SiteGroupFlag = 0 AND t2.ID = @SiteId 
                order by t2.ID";

            ErrorNotificationConfig config = new ErrorNotificationConfig();

            try
            {
                //Use this for now since we do not have a login.  Update this once either the service uses a login or 
                //a right has been specifically created for reading tblErrorNotificationConfigurations  
                if (!sc.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS))
                {
                    return config;
                }
                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.Add(new SqlParameter("@SiteId", siteId));
                    DataTable dtResults = this.ConsolidatedDA.GetDataTable(cmd, sc);
                    foreach (DataRow dr in dtResults.Rows)
                    {
                        config.SiteId = dr["SiteID"] != DBNull.Value ? dr["SiteID"].ToString() : string.Empty;
                        config.EmailAddresses = dr["EmailAddresses"] != DBNull.Value ? dr["EmailAddresses"].ToString() : string.Empty;
                        config.ErrorFolder = dr["ErrorFolder"] != DBNull.Value ? dr["ErrorFolder"].ToString() : string.Empty;
                        config.CreatedBy = dr["CreatedBy"] != DBNull.Value ? dr["CreatedBy"].ToString() : string.Empty;
                        config.CreatedDate = dr["CreatedDate"] != DBNull.Value ? DateTimeOffset.Parse(dr["CreatedDate"].ToString()) : (DateTimeOffset?)null;
                        config.UpdatedBy = dr["UpdatedBy"] != DBNull.Value ? dr["UpdatedBy"].ToString() : string.Empty;
                        config.UpdatedDate = dr["UpdatedDate"] != DBNull.Value ? DateTimeOffset.Parse(dr["UpdatedDate"].ToString()) : (DateTimeOffset?)null;
                    }
                }
                
                return config;
            }
            catch (Exception ex)
            {
                LogError(ex, EventLogEntryType.Error, 3001);
                return config;
            }
        }


        #region Logs

        /// <summary>
        /// Simple method for adding error messages to the Windows Application Event Log
        /// </summary>
        /// <param name="ex">Exception thrown</param>
        /// <param name="logType">Type of event</param>
        /// <param name="eventId">Event Id</param>
        private void LogError(Exception ex, EventLogEntryType logType, int eventId)
        {
            try
            {
                string source = "NotificationServiceBusinessServices";
                string log = "Application";
                if (!EventLog.SourceExists(source))
                {
                    EventLog.CreateEventSource(source, log);
                }

                EventLog.WriteEntry(source, "Exception: " + ex.Message + Environment.NewLine + "Inner Exception: " + (ex.InnerException?.Message ?? String.Empty) + Environment.NewLine + "Stack Trace: " + ex.StackTrace, EventLogEntryType.Error, eventId);

                //Use this instead?
                //FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(errorMessage, FMEventLogEntryType.Error));
            }
            catch (Exception exp)
            {
                //error logging error?
            }
        }

        #endregion
    }
}
