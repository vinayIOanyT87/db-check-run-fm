/******************************************************************************

	FILE NAME:		UserRecoveryServer.cs


	PURPOSE:			UserRecoveryServer Class


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	A. Coker


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------
*******************************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Data.SqlClient;

using ConsolidatedDAL;
using ConsolidatedBLL;
using ConsolidatedDataObjects;
using FMCommon;


namespace LogService
{
   class UserRecoveryServer
   {
      EventLog eventLog = new EventLog("Application", ".", "FuelsManager Service");
      System.Threading.Thread thread = null;


      ConsolidatedDAClass dal = null;
      protected void RecoverUsers()
      {
         try
         {
            SecurityClass security = new SecurityClass();
            security.UserID = DBAccess.ServiceLogin;
            security.RightCollection.Add(RIGHT.VIEW_USERS);
            ConsolidatedDAClass.ReadHardwareKey();
            HardwareKeyClass HardwareKey = new HardwareKeyClass();
            HardwareKey.ValidateKey();
            dal = new ConsolidatedDAClass();
            if (HardwareKey.DescKey)
            {
               UsersClass users = new UsersClass();
               UserCollectionClass userColl = EnumerateAllUsers(security);//users.EnumerateAndFilter(security, null);
               SqlConnectionStringBuilder csBuilder = new SqlConnectionStringBuilder(dal.ConnectionString);

               foreach (UserClass user in userColl)
               {
                  try
                  {
                      if (  SQLLoginExists(security, user)                  == false ||
                            SQLUserExists(security, user, "["+csBuilder.InitialCatalog+"]") == false ||
                            SQLUserExists(security, user, "master")         == false ||
                            SQLUserExists(security, user, "[" + csBuilder.InitialCatalog + "Archive]", true) == false ||
                            SIDsSame(security, user, "[" + csBuilder.InitialCatalog + "]") == false ||
                            SIDsSame(security, user, "master")              == false
                         )
                     
                     {
                        eventLog.WriteEntry("Creating user " + user.ID, EventLogEntryType.Information);
                        //SQL Login does not exist for user. Drop and create user.

                        try
                        {
                           dal.DeleteDBUser(security, user.ID, null);
                        }
                        catch (Exception e)
                        {
                           eventLog.WriteEntry("Exception in UserRecoveryServer:" + e.Message, EventLogEntryType.Warning);
                        }

                        dal.CreateDBUser(security, user.ID, null, user.Password);
                     }
                     else
                      {
                         eventLog.WriteEntry("Pass user " + user.ID, EventLogEntryType.Information);
                      }
                     
                     //Promote or Demote user based on security right.
                     RightsClass rightsClass = new RightsClass();
                     SecurityClass userSecurity = new SecurityClass();
                     userSecurity.UserIndex = user.Index;
                     userSecurity.UserID = user.ID;
                     userSecurity.RightCollection = rightsClass.EnumerateByUser(security, user.Index);
                     if (  userSecurity.HasRight(RIGHT.MODIFY_USER_GROUPS) ||
                           userSecurity.HasRight(RIGHT.MODIFY_USERS))
                     {
                        dal.PromoteToAdmin(security, user.ID, null);
                     }
                     else
                     {
                        dal.DemoteFromAdmin(security, user.ID, null);
                     }
                  }
                  catch (Exception e)
                  {
                     eventLog.WriteEntry("Exception in UserRecoveryServer:" + e.Message, EventLogEntryType.Warning);
                  }

                   //  System.Threading.Thread.Sleep(1000);//for testing

               }
            }
            else
            {
               eventLog.WriteEntry("UserRecoveryServer: DESC key not present. ", EventLogEntryType.Information);
            }

            // Make sure no user rights are assigned above the maximum RIGHT value
            // This can happen during calls to fm_PopulateDB or fm_CreateDefaultSingleSite
            eventLog.WriteEntry("Delete excess rights mappings", EventLogEntryType.Information);

            // JS20100831 WI-16178 will now use an array of existing rights, instead of the length of the rights enumeration
            StringBuilder builder = new StringBuilder();
            foreach (RIGHT right in Enum.GetValues(typeof(RIGHT)))
            {
					if ( SecurityClass.UndefinedRightText.NotEquals( SecurityClass.RightID( right ) ) )
					{
						builder.Append( ( (int)right ).ToString() + "," );
					}
            }
            // remove trailing comma
            builder.Remove(builder.Length - 1, 1);

            string SQL = "DELETE FROM tblGroupRightsMap WHERE [RightIndex] NOT IN (" + builder.ToString() + ")";
            dal.ExecuteQuery(security, SQL);

         }
         catch (Exception e)
         {
            eventLog.WriteEntry("Exception in UserRecoveryServer:" + e.Message, EventLogEntryType.Error);
         }
         eventLog.WriteEntry("User Recovery completed.", EventLogEntryType.Information);
         thread = null;
      }

      internal void Start()
      {
         eventLog.WriteEntry("User Recovery started.", EventLogEntryType.Information);
         System.Threading.ThreadStart start = new System.Threading.ThreadStart(RecoverUsers);
         thread = new System.Threading.Thread(start);
         thread.Start();
      }

      internal bool Stop()
      {
         if (thread != null && thread.IsAlive)
         {
            eventLog.WriteEntry("Cannot stop UserRecoveryServer. It is busy.", EventLogEntryType.Information);
            return false;
         }
         return true;

      }

      private bool SQLLoginExists(SecurityClass security, UserClass user)
      {
         try
         {
            string sql = "SELECT * FROM sys.server_principals WHERE type='S' AND name=N'" + user.ID + "'";
            SqlCommand command = new SqlCommand(sql);
            DataSet dataSet = dal.GetDataSet(sql, security);
            DataTable table = dataSet.Tables[0];
            DataRowCollection rows = table.Rows;
            return (rows.Count > 0);
         }
         catch (Exception e)
         {
            eventLog.WriteEntry("Exception in UserRecoveryServer.SQLLoginExists:" + e.Message, EventLogEntryType.Information);
         }
         return false;
      }

      private bool SQLUserExists(SecurityClass security, UserClass user, string dbname)
      {
         return SQLUserExists(security, user, dbname, false);
      }

      private bool SQLUserExists(SecurityClass security, UserClass user, string dbname, bool bIgnoreDBNotExist)
      {
         try
         {
            string sql = "SELECT * FROM " + dbname + ".sys.database_principals WHERE type='S' AND name=N'" + user.ID + "'";
            SqlCommand command = new SqlCommand(sql);
            DataSet dataSet = dal.GetDataSet(sql, security);
            DataTable table = dataSet.Tables[0];
            DataRowCollection rows = table.Rows;
            return (rows.Count > 0);
         }
         catch (SqlException se)
         {
            if (bIgnoreDBNotExist && (se.Number == 4060))
            {
               return true;
            }

            throw se;

         }
         catch (Exception e)
         {
            eventLog.WriteEntry("Exception in UserRecoveryServer.SQLUserExists:" + e.Message, EventLogEntryType.Information);
         }
         return false;
      }

      private bool SIDsSame(SecurityClass security, UserClass user, string dbname)
      {
         try
         {
            string sql = "SELECT sp.name AS 'Server Login', dp.name AS 'Database User' " +
               " FROM  " + dbname + ".sys.database_principals dp join sys.server_principals sp on dp.sid = sp.sid " +
               " WHERE sp.name = N'" + user.ID + "' AND dp.name = N'" + user.ID + "'";
            SqlCommand command = new SqlCommand(sql);
            DataSet dataSet = dal.GetDataSet(sql, security);
            DataTable table = dataSet.Tables[0];
            DataRowCollection rows = table.Rows;
            return (rows.Count > 0);
         }
         catch (Exception e)
         {
            eventLog.WriteEntry("Exception in UserRecoveryServer.SIDsSame:" + e.Message, EventLogEntryType.Information);
         }
         return false;
      }
      
      private UserCollectionClass EnumerateAllUsers(SecurityClass security)
      {
         UserCollectionClass UserCollection = new UserCollectionClass();
         string sql = "SELECT * FROM tblUsers ORDER BY userID";

         SqlCommand command = new SqlCommand(sql);
         DataSet dataSet = dal.GetDataSet(sql, security);
         DataTable Table = dataSet.Tables[0];
         UserClass User;
         while (Table.Rows.Count != 0)
         {
            User = new UserClass();
            User.Load(dataSet);
            UserCollection.Add(User);
            Table.Rows.RemoveAt(0);
         }
         return UserCollection;
      }


   }
}


