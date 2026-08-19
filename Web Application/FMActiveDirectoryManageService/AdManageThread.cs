namespace FMActiveDirectoryManageService
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices.ComTypes;
    using System.Text;
    using System.Threading;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;

    public class AdManageThread : BaseThread
    {
        #region Data Memebers
        private SecurityClass security;
        private bool isSsoMode;
        private bool isTestMode;
        private int sleepIntervalTime;
        private string testModeFilePath;
        private bool stopFlag;
        private List<Tuple<string, Guid>> siteIdAndGuidList;

        private ProcessUserSiteAssociation processUserSiteAssociation;
        private ProcessUserUserGroupAssociation processUserUserGroupAssociation;

        private const string TestModeFlagKey        = "ActiveDirectoryManageSvr_TestModeFlag";
        private const string TestModeFilePathKey    = "ActiveDirectoryManageSvr_TestModeFilePath";
        private const string SleepIntervalTimeKey   = "ActiveDirectoryManageSvr_SleepIntervalTime";
        public const string MessagePrefixKey        = "FMADManSvr - ";
        #endregion

        #region Constructor
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public AdManageThread()
        {
            this.sleepIntervalTime = 3600000;  // 60 minutes in milliseconds.
        }
        #endregion

        #region Properties
        public bool StopFlag
        {
            get
            {
                return this.stopFlag;
            }
            set
            {
                this.stopFlag = value;
                if (this.processUserSiteAssociation != null)
                {
                    this.processUserSiteAssociation.StopFlag = this.stopFlag;
                }

                if (this.processUserUserGroupAssociation != null)
                {
                    this.processUserUserGroupAssociation.StopFlag = this.stopFlag;
                }
            }
        }
        #endregion

        #region Main thread methods
        /// <summary>
        /// This method implements the thread handler and starts
        /// the listening.
        /// </summary>
        protected override void ThreadHandler()
        {
            ReadApi readApi;

            try
            {
                // Stop the service if there is another FM Active Directory Manage Service process running.
                if (this.IsMoreThanOneProcessRunning())
                {
                    this.StopFlag = true;
                    this.StopService();
                    return;
                }

                // In debug mode, sleep for 30 seconds in order to attach the debugger.
                if(this.IsDebugFlagSet())
                {
                    Thread.Sleep(30000);
                }

                FMChannelHelper.MakeCall<IHardwareKey, ushort>(x => x.CheckActivatedLicenceVersion());

                // Build Security object. False means there was an error.
                var buildSecurityClass = new BuildSecurityClass();
                this.security = buildSecurityClass.BuildSecurity(Guids.SiteAdminGuid);

                // Read the service configuration from app.config
                this.ReadConfiguration();

                // Create a new Read API to read the test file or AD API.
                readApi = new ReadApi(this.security, this.FMEventLog);

                // Temporary in order to generate an example XML file.
                //readApi.WriteTempTestFile(this.testModeFilePath);

                // Create the processing objects.
                this.processUserSiteAssociation = new ProcessUserSiteAssociation(this.security, this.FMEventLog, this.StopFlag);
                this.processUserUserGroupAssociation = new ProcessUserUserGroupAssociation(this.security, this.FMEventLog, this.StopFlag);
            }
            catch (Exception ex)
            {
                this.FMEventLog.WriteEntry(MessagePrefixKey + "error: " + ex.Message, EventLogEntryType.Error);
                this.StopService();
                return;
            }

            while (this.StopFlag == false)
            {
                try
                {
                    // Get system settings from the database which contains the SSO mode flag. False means there was an error.
                    if (this.GetSsoSetting() == false)
                    {
                        this.StopService();
                        return;
                    }

                    // If not in SSO mode do nothing.
                    if (this.isSsoMode == false)
                    {
                        this.FMEventLog.WriteEntry(MessagePrefixKey + " SSO mode is disable, FM Active Directory Manage Service will not process.", EventLogEntryType.Information);
                        this.SleepDelay();
                        continue;
                    }

                    // Retrieve a list of all Site IDs and Guids (used for getting the Site ID on error messages)
                    this.RetrieveSiteIdAndGuids();
                    this.processUserSiteAssociation.SiteIdAndGuidList = this.siteIdAndGuidList;
                    this.processUserUserGroupAssociation.SiteIdAndGuidList = this.siteIdAndGuidList;

                    // Call the AD to refresh the AD Site Group and AD User Group tables.
                    // Only want to call this when not in test mode. Must happen prior to the call
                    // to get the site to AD site mappings and the user group to AD user group
                    // mappings below.
                    if(this.isTestMode == false) this.CallActiveDirectoryToRefresh();

                    List<ActiveDirectoryUserDTO> adUserCollection;

                    // Get site to AD Site mapping data from the database
                    List<SiteToAdSiteDO> siteToAdSiteList = this.GetSiteToAdSiteMapping();

                    // Get User to AD User mapping data from test file
                    List<UserGroupToAdUserGroupDO> userGroupToAdUserGroupList = this.GetUserGroupToAdUserGroupMapping();

                    if (this.isTestMode)
                    {
                        adUserCollection = readApi.ReadTestFile(this.testModeFilePath);

                        if (adUserCollection == null)
                        {
                            this.SleepDelay();
                            continue;
                        }

                        // Process user based on AD sites.  Note: process site association must execute before
                        // the call to process AD user group below.
                        this.processUserSiteAssociation.ProcessUsersBasedOnSites(siteToAdSiteList, adUserCollection, this.IsDebugFlagSet());

                        // Process users based on AD user groups
                        this.processUserUserGroupAssociation.ProcessUsersBasedOnUserGroups(userGroupToAdUserGroupList, adUserCollection, this.IsDebugFlagSet());

                        this.SleepDelay();
                        continue;
                    }

                    // Get the User to AD user group mapping data and the User to AD site mappings
                    // from the Active Directory Service API.
                    adUserCollection = readApi.ReadActiveDirectoryApi();

                    if (adUserCollection != null)
                    {
                        if (this.IsDebugFlagSet())
                        {
                            StringBuilder sbWriteOut = new StringBuilder();
                            sbWriteOut.AppendLine("==== User Name -> User Groups / Sites ====\n");
                            foreach (ActiveDirectoryUserDTO ad in adUserCollection)
                            {
                                sbWriteOut.AppendLine(ad.UserName);
                                sbWriteOut.Append("\tMember of User Groups: [");
                                foreach (string s in ad.UserGroups) sbWriteOut.Append(s + ", ");
                                sbWriteOut.AppendLine("]");
                                sbWriteOut.Append("\tMember of Sites: [");
                                foreach (string s in ad.Sites) sbWriteOut.Append(s + ", ");
                                sbWriteOut.AppendLine("]\n");
                            }
                            sbWriteOut.Replace(", ]", "]");

                            string mFileName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\ADUsers-Groups-Sites.txt";

                            using (TextWriter textWriter = new StreamWriter(new FileStream(mFileName, FileMode.Create, FileAccess.Write)))
                            {
                                textWriter.Write(sbWriteOut.ToString());
                            }
                        }

                        // Process user based on AD sites.Note: process site association must execute before
                        // the call to process AD user group below.
                        this.processUserSiteAssociation.ProcessUsersBasedOnSites(siteToAdSiteList, adUserCollection, this.IsDebugFlagSet());

                        // Process users based on AD user groups
                        this.processUserUserGroupAssociation.ProcessUsersBasedOnUserGroups(userGroupToAdUserGroupList, adUserCollection, this.IsDebugFlagSet());


                        try
                        {
                            FMChannelHelper.MakeCall<IUsers>(x => x.DeleteOrphanUserRecords(this.security));
                        }
                        catch (Exception ex)
                        {
                            this.FMEventLog.WriteEntry(MessagePrefixKey + "DeleteOrphanUserRecords: " + ex.Message, EventLogEntryType.Error);
                        }
                    }

                    this.SleepDelay();
                    continue;
                }
                catch (Exception ex)
                {
                    this.FMEventLog.WriteEntry(
                        MessagePrefixKey + " FM Active Directory Manage received an exception: " + ex.Message,
                        EventLogEntryType.Error);
                }

                this.SleepDelay();
            }

            this.StopService();
        }

        /// <summary>
        /// This method will place the thread in a wait pattern only if the stop flag
        /// is false.
        /// </summary>
        private void SleepDelay()
        {
            if (this.StopFlag == false)
            {
                const int checkStopTime = 1000;
                int loopCount = this.sleepIntervalTime / checkStopTime;

                if (loopCount == 0)
                {
                    loopCount = 60; // Set default to one minute
                }

                for(int count = 0; count < loopCount; count++)
                {
                    // Op out if the stop flag is set to stop (true)
                    if(this.stopFlag)
                    {
                        return;
                    }

                    Thread.Sleep(checkStopTime);
                }
            }
        }
        #endregion

        #region Retrieve mapping data between FM and AD.
        /// <summary>
        /// This method will return a list of site to AD site mappings.
        /// </summary>
        /// <returns>Returns a list of site to AD site objects.</returns>
        private List<SiteToAdSiteDO> GetSiteToAdSiteMapping()
        {
            var siteToAdSiteMappings = new List<SiteToAdSiteDO>();
            var dataSet = FMChannelHelper.MakeCall<IActiveDirectoryMappings, DataSet>(x => x.EnumerateSiteToActiveDirectorySiteMapping(this.security));

            if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
            {
                const string ErrMsg = MessagePrefixKey + " No Site to AD Site mappings. ";
                this.FMEventLog.WriteEntry(ErrMsg, EventLogEntryType.Error);
                return siteToAdSiteMappings;
            }
     
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                var siteToAdSite = new SiteToAdSiteDO
                                   {
                                       SiteId = row.IsNull("SiteID") ? string.Empty : (string)row["SiteID"],
                                       SiteGuid = row.IsNull("SiteGuid") ? Guid.Empty : (Guid)row["SiteGuid"],
                                       ActiveDirectorySiteId = row.IsNull("ActiveDirectorySiteName") ? string.Empty : (string)row["ActiveDirectorySiteName"]
                                   };


                siteToAdSiteMappings.Add(siteToAdSite);
            }

            return siteToAdSiteMappings;
        }

        /// <summary>
        /// This method will return a list of user group to AD user group mappings.
        /// </summary>
        /// <returns>Returns a list of user group to AD user group objects.</returns>
        private List<UserGroupToAdUserGroupDO> GetUserGroupToAdUserGroupMapping()
        {
            var userGroupToAdUserGroupMappings = new List<UserGroupToAdUserGroupDO>();
            var dataSet = FMChannelHelper.MakeCall<IActiveDirectoryMappings, DataSet>(x => x.EnumerateUserGroupToActiveDirectoryUserGroupMapping(this.security));

            if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
            {
                const string ErrMsg = MessagePrefixKey + " No User Group to AD User Group mappings. ";
                this.FMEventLog.WriteEntry(ErrMsg, EventLogEntryType.Error);
                return userGroupToAdUserGroupMappings;
            }

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                var userGroupToAdUserGroup = new UserGroupToAdUserGroupDO()
                {
                    UserGroupId = row.IsNull("GroupID") ? string.Empty : (string)row["GroupID"],
                    UserGroupGuid = row.IsNull("GroupGuid") ? Guid.Empty : (Guid)row["GroupGuid"],
                    ActiveDirectoryUserGroupID = row.IsNull("ActiveDirectoryUserGroupName") ? string.Empty : (string)row["ActiveDirectoryUserGroupName"]
                };

                userGroupToAdUserGroupMappings.Add(userGroupToAdUserGroup);
            }

            return userGroupToAdUserGroupMappings;
        }

        /// <summary>
        /// This method will retrieve and populate a list of site IDs and Guids.
        /// </summary>
        private void RetrieveSiteIdAndGuids()
        {
            this.siteIdAndGuidList = new List<Tuple<string, Guid>>();

            try
            {
                var dataSet = FMChannelHelper.MakeCall<IActiveDirectoryMappings, DataSet>(x => x.EnumerateAllSiteIdAndGuid(this.security));

                if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
                {
                    const string ErrMsg = MessagePrefixKey + " No Site ID and GUIDs. ";
                    this.FMEventLog.WriteEntry(ErrMsg, EventLogEntryType.Warning);
                    return;
                }

                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    string siteId = row.IsNull("SiteID") ? string.Empty : (string)row["SiteID"];
                    Guid siteGuid = row.IsNull("SiteGuid") ? Guid.Empty : (Guid)row["SiteGuid"];

                    if (string.IsNullOrEmpty(siteId) == false && siteGuid != Guid.Empty)
                    {
                        var siteIdAndGuid = new Tuple<string, Guid>(siteId, siteGuid);
                        this.siteIdAndGuidList.Add(siteIdAndGuid);
                    }
                }
            }
            catch (Exception ex)
            {
                string errMsg = MessagePrefixKey + " Error retrieving Site ID and GUIDs. " + ex.Message;
                this.FMEventLog.WriteEntry(errMsg, EventLogEntryType.Error);
            }
            
        }
        #endregion

        #region Get system settings and configuration
        /// <summary>
        /// This method will get the Configuration Settings which has the SSO mode flag.  
        /// It will set the local SSO Mode flag. The default is set to NOT in SSO mode (false).
        /// </summary>
        /// <returns>Return true if the call was successful, otherwise it returns false.</returns>
        private bool GetSsoSetting()
        {
            this.isSsoMode = false;

            try
            {
                var configSetting = FMChannelHelper.MakeCall<IConfigurationSettings, ConfigurationSettingDOClass>
                                                (x => x.GetByKey(this.security, ConfigurationSettingDOClass.Key_SingleSignOnMode));

                if (configSetting != null && string.IsNullOrEmpty(configSetting.SettingValue) == false)
                {
                    this.isSsoMode = configSetting.SettingValue == "1" ? true : false;
                }
            }
            catch (Exception ex)
            {
                const string ErrMsg = MessagePrefixKey + " Unable to retrieve Configuration Settings from FMBusinessServices. Check that the FMBusinessServices is running. ";
                this.FMEventLog.WriteEntry(ErrMsg + ex.Message, EventLogEntryType.Error);
                return false;
            }

            return true;
        }

        /// <summary>
        /// This method will read the configuration setting where the tracking list is
        /// located and the scan interval setting.
        /// </summary>
        private void ReadConfiguration()
        {
            string testModeFlagStr = string.Empty;
            string sleepIntervalStr = string.Empty;

            try
            {
                var configSettingDo = FMChannelHelper.MakeCall<IConfigurationSettings, ConfigurationSettingDOClass>(x => x.GetByKey(this.security, TestModeFlagKey));
                testModeFlagStr = configSettingDo.SettingValue;

                configSettingDo = FMChannelHelper.MakeCall<IConfigurationSettings, ConfigurationSettingDOClass>(x => x.GetByKey(this.security, TestModeFilePathKey));
                this.testModeFilePath = configSettingDo.SettingValue;

                configSettingDo = FMChannelHelper.MakeCall<IConfigurationSettings, ConfigurationSettingDOClass>(x => x.GetByKey(this.security, SleepIntervalTimeKey));
                sleepIntervalStr = configSettingDo.SettingValue;
            }
            catch (Exception ex)
            {
                const string ErrMsg = MessagePrefixKey + " Unable to retrieve Configuration Settings from FMBusinessServices. Check that the FMBusinessServices is running. ";
                this.FMEventLog.WriteEntry(ErrMsg + ex.Message, EventLogEntryType.Error);
            }

            // Set default sleep time to 60 minutes in milliseconds.
            this.sleepIntervalTime = 3600000;

            // Set the test mode false to disabled.
            this.isTestMode = false;

            if (string.IsNullOrEmpty(sleepIntervalStr) == false)
            {
                int interval;

                if (int.TryParse(sleepIntervalStr, out interval))
                {
                    // The configured sleep interval is set in minutes in the app.config file.
                    // Convert to milliseconds 1 min = 60,000 milliseconds.
                    if (interval >= 1 && interval <= 60)
                    {
                        this.sleepIntervalTime = interval * 60000;
                    }
                }
            }

            if (string.IsNullOrEmpty(testModeFlagStr) == false && testModeFlagStr.ToUpper().Equals("TRUE"))
            {
                this.isTestMode = true;
            }

            if (string.IsNullOrEmpty(this.testModeFilePath))
            {
                this.testModeFilePath = string.Empty;
                this.isTestMode = false;
            }
        }

        /// <summary>
        /// This method will read the app setting for a debug flag.
        /// </summary>
        /// <returns>Returns true if the debug flag is set, otherwise returns false.</returns>
        private bool IsDebugFlagSet()
        {
            string debugFlagStr = ConfigurationManager.AppSettings["DebugFlag"];

            if(string.IsNullOrEmpty(debugFlagStr))
            {
                return false;
            }

            if(debugFlagStr.ToUpper() != "TRUE")
            {
                return false;
            }

            return true;
        }
        #endregion

        #region Call AD to update the AD site and user group tables
        /// <summary>
        /// This method will call the AD service to update the site group and user group tables based
        /// on the AD.
        /// </summary>
        private void CallActiveDirectoryToRefresh()
        {
            try
            {
                // Refresh the AD site group table.
                FMChannelHelper.MakeCall<IActiveDirectoryService>(x => x.RefreshSites(this.security));

                // Refresh the AD user group table. 
                FMChannelHelper.MakeCall<IActiveDirectoryService>(x => x.RefreshUserGroups(this.security));
            }
            catch (Exception ex)
            {
                const string ErrMsg = MessagePrefixKey + " Unable to refresh the AD Sites/User Groups. Check that the FMBusinessServices is running. ";
                this.FMEventLog.WriteEntry(ErrMsg + ex.Message, EventLogEntryType.Error);
            }
        }
        #endregion
    }
}
