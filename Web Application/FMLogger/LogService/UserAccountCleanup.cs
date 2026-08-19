/******************************************************************************

	FILE NAME:		UserAccountCleanup.cs


	PURPOSE:			UserAccountCleanup Class


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	S. Jiang


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------
*******************************************************************************/
using System;
using System.Diagnostics;
using ConsolidatedDAL;
using ConsolidatedBLL;
using ConsolidatedDataObjects;
using FMCommon;
using Microsoft.Win32;

namespace LogService
{
    class UserAccountCleanup
    {
        EventLog eventLog = new EventLog("Application", ".", "FuelsManager Service");
        System.Threading.Thread DisableUserthread = null;
        System.Threading.Thread ArchiveUserthread = null;
        ConsolidatedDAClass ConsolidatedDA = null;
        SecurityClass security = null;
        int iInterval = 24;
        TimeSpan timespan; 
        DateTime dtLastDisableUser;
        DateTime dtLastArchieveUser; 

        public UserAccountCleanup()
        {
        }

        private void DisableUser()
        {
            try
            {
                string SQL = "UPDATE tblUsers SET tblUsers.InactivityLockout = 1, tblUsers.InactivityLockoutDate = GETUTCDATE() " +
                "FROM tblSites JOIN tblUsers ON tblUsers.SiteGuid = tblSites.SiteGuid " +
                "WHERE (UserID <> 'Administrator') AND (tblUsers.InactivityLockout = 0 OR tblUsers.InactivityLockout IS NULL) AND " +
                "( LastLoginDate + InactivityDisablePeriod < GETUTCDATE() )";

                ConsolidatedDA.ExecuteQuery(security, SQL);
                while ((DateTime.Now - dtLastDisableUser > timespan) && DisableUserthread.IsAlive)
                {
                    dtLastDisableUser = DateTime.Now;
                    ConsolidatedDA.ExecuteQuery(security, SQL);
                }
            }
            catch (Exception e)
            {
                eventLog.WriteEntry("Exception in UserAccountCleanup.SetDisabled:" + e.Message, EventLogEntryType.Information);
            }
         
        }

        private void ArchiveUser()
        {           
            string strSQLError = "SELECT @intErrorCode = @@ERROR IF (@intErrorCode <> 0) GOTO PROBLEM ";
            string strSQLFrom = "FROM tblUsers JOIN tblSites ON tblSites.SiteGuid = tblUsers.SiteGuid " +
                                "WHERE (UserID <> 'Administrator') AND (DisableArchivePeriod > 0) AND ((InactivityLockout = 1) AND " +
                                "(((InactivityLockoutDate IS NOT NULL) AND ((DisableArchivePeriod + InactivityLockoutDate) < GETDATE())) OR " +
                                "(((InactivityLockoutDate IS NULL) AND ((DisableArchivePeriod + PasswordTimeStamp) < GETDATE())))))	";
            string SQL = "SET XACT_ABORT ON " +
                         "DECLARE @intErrorCode INT " + 
                         "BEGIN TRAN " +
                        UserClass.ArchiveSQL +
	                    strSQLFrom + 
                        strSQLError +
                        "DELETE FROM tblUserGroupMap WHERE UserGuid IN ( SELECT tblUsers.UserGuid " +
                        strSQLFrom + 
                        ") " +
                        strSQLError +
		                "DELETE tblUsers " +
		                strSQLFrom +
		                strSQLError +
		                "COMMIT TRAN " + 
		                "PROBLEM: IF (@intErrorCode <> 0) BEGIN ROLLBACK TRAN END " ;

            try
            {
                ConsolidatedDA.ExecuteQuery(security, SQL);
                while ((DateTime.Now - dtLastArchieveUser > timespan) && ArchiveUserthread.IsAlive)
                {
                    dtLastArchieveUser = DateTime.Now;
                    ConsolidatedDA.ExecuteQuery(security, SQL);
                }
            }
            catch (Exception e)
            {
                eventLog.WriteEntry("Exception in UserAccountCleanup.ArchiveDisabledUser:" + e.Message, EventLogEntryType.Information);
            }
        }

        public void Start()
        {
            try
            {
                security = new SecurityClass();
                security.UserID = DBAccess.ServiceLogin;
                ConsolidatedDA = new ConsolidatedDAClass();
                ConsolidatedDAClass.ReadHardwareKey();
                RegistryKey RegKey = Registry.LocalMachine.OpenSubKey("Software\\Varec\\FuelsManager Service");
                if (RegKey == null)
                {
                    RegKey = Registry.LocalMachine.CreateSubKey("Software\\Varec\\FuelsManager Service");
                    RegKey.SetValue("AccountCleanUpInterval", 24, RegistryValueKind.DWord);
                }
                else
                    iInterval = (int)RegKey.GetValue("AccountCleanUpInterval");
                timespan = new TimeSpan((int)iInterval, 0, 0);
                dtLastDisableUser = DateTime.Now;
                dtLastArchieveUser = DateTime.Now;

                eventLog.WriteEntry("User Account Cleanup started.", EventLogEntryType.Information);
                System.Threading.ThreadStart startDisableUser = new System.Threading.ThreadStart(DisableUser);
                DisableUserthread = new System.Threading.Thread(startDisableUser);
                DisableUserthread.Start();

                HardwareKeyClass HardwareKey = new HardwareKeyClass();
                HardwareKey.ValidateKey();
                if (HardwareKey.DescKey)
                {
                    System.Threading.ThreadStart startArchiveUser = new System.Threading.ThreadStart(ArchiveUser);
                    ArchiveUserthread = new System.Threading.Thread(startArchiveUser);
                    ArchiveUserthread.Start();
                }
            }
            catch (Exception e)
            {
                eventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
            }
        }

        public bool Stop()
        {
            try
            {
                if (DisableUserthread != null)
                {
                    DisableUserthread.Abort();
                    if (DisableUserthread.IsAlive)
                    {
                        eventLog.WriteEntry("Cannot stop Disable User thread. It is busy.", EventLogEntryType.Information);
                        return false;
                    }
                }
                if (ArchiveUserthread != null)
                {
                    ArchiveUserthread.Abort();
                    if (ArchiveUserthread.IsAlive)
                    {
                        eventLog.WriteEntry("Cannot stop Archive User thread. It is busy.", EventLogEntryType.Information);
                        return false;
                    }
                }
            }

            catch (Exception e)
            {
                eventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
                return false;
            }
            return true;
        }
    }
}
