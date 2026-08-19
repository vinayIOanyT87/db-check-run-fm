// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuditProcessing.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//    Provides the ability to process FM license expiration in a separate thread.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManagerService
{
   using System;
   using System.Collections.Generic;
   using System.Diagnostics;
   using System.Linq;
   using System.Security.Policy;
   using System.Threading;
   using FMBusinessObjects.BusinessInterfaces;
   using FMBusinessObjects.ChannelFactories;
   using FMBusinessObjects.DataObjects;

   class PointAlarmStatusProcessing
   {
      #region Constants and Fields

      /// <summary>
      /// Stops processing
      /// </summary>
      private static readonly ManualResetEvent KillEvent = new ManualResetEvent(false);
      
      private static string administratorUserID = "administrator";
      private static Guid administratorUserGuid = new Guid("00000000-0000-0000-0000-000000000002");
      private static Dictionary<string,DateTime> alarmStatusInAlarmAndEventLog = new Dictionary<string,DateTime>();
      private static int notificationInterval = 5;

      /// <summary>
      /// The thread responsible for processing
      /// </summary>
      private static Thread processThread = null;

      private static EventLog eventLog = null;
      #endregion

      #region Methods

      /// <summary>
      /// Starts execution of the ProcessThread.
      /// </summary>
      /// <param name="security">
      /// Contains Security Information.
      /// </param>
      internal static void StartProcessThread(SecurityClass security, EventLog el)
      {
         eventLog = el;
         processThread = new Thread(() => ProcessScan(security));
         processThread.Start();
      }

      /// <summary>
      ///     Stops the ProcessThread.
      /// </summary>
      internal static void StopProcessThread()
      {
         KillEvent.Set();

         if (processThread != null)
         {
            processThread.Join();
         }
      }

      /// <summary>
      /// Processes the scan.
      /// </summary>
      /// <param name="security">The security.</param>
      private static void ProcessScan(SecurityClass security)
      {


         int interval = 5;//in minutes
         TimeSpan waitInterval = new TimeSpan(0, 1, 0);
         WaitHandle[] events = { KillEvent };
         DateTime start = DateTime.Now;
         WaitHandle.WaitAny(events, waitInterval, true);

         try
         {
            var configSettingDo = FMChannelHelper.MakeCall<IConfigurationSettings, ConfigurationSettingDOClass>(x => x.GetByKey(security, "AlarmStatusCheckInterval"));//in minutes
            if (string.IsNullOrWhiteSpace(configSettingDo.SettingValue) == false)
            {
               if (int.TryParse(configSettingDo.SettingValue, out interval) == false)
               {
                  interval = 5;
               }
            }
            configSettingDo = FMChannelHelper.MakeCall<IConfigurationSettings, ConfigurationSettingDOClass>(x => x.GetByKey(security, "AlarmStatusNotNormalEmailNotificationInterval"));//in minutes
            if (string.IsNullOrWhiteSpace(configSettingDo.SettingValue) == false)
            {
               if (int.TryParse(configSettingDo.SettingValue, out notificationInterval) == false)
               {
                  notificationInterval = 5;
               }
            }
            //if (interval < FuelsManagerSettings.AlarmAndEventProcessingInterval)
            //{
            //    //Email notification interval cannot be less than alarm and event processing interval.
            //    interval = FuelsManagerSettings.AlarmAndEventProcessingInterval;
            //}
            waitInterval = new TimeSpan(0, interval, 0);
            CheckAlarmStatusNotNormalAndNotify(security);
         }
         catch (Exception ex)
         {
            waitInterval = new TimeSpan(0, 5, 0);
            FuelsManagerServiceLogger.Instance.LogError(ex);
         }

         while (0 != WaitHandle.WaitAny(events, waitInterval, true))
         {
            try
            {
               CheckAlarmStatusNotNormalAndNotify(security);
            }
            catch (Exception ex)
            {
               FuelsManagerServiceLogger.Instance.LogError(ex);
            }
         }
      }

      /// <summary>
      /// Retrieves active alarms that do not have a normal status.
      /// </summary>
      /// <param name="security"></param>
      private static void CheckAlarmStatusNotNormalAndNotify(SecurityClass security)
      {
         SiteCollectionClass sites = FMChannelHelper.MakeCall<ISites, SiteCollectionClass>(s => s.Enumerate(security));
         if (sites == null || sites.Count == 0)
         {
            return;
         }

         bool unacknowledged = true;
         var sec = security.Clone();
         sec.UserID = administratorUserID;
         sec.UserGuid = administratorUserGuid;

         string source = AlarmStatusClass.PointAlarmStatusNotNormalNotificationDescriptor.Source;
         string type = AlarmStatusClass.PointAlarmStatusNotNormalNotificationDescriptor.Alarm ? "Alarm" : "Event";
         string id = AlarmStatusClass.PointAlarmStatusNotNormalNotificationDescriptor.ID;
         AlarmAndEventClass alarmAndEvent = null;
         string categoryID = string.Empty;
         string priorityID = "Notify";
        
         Guid alarmAndEventGuid = FMChannelHelper.MakeCall<IAlarmAndEvents, Guid>(x => x.GetIdentityGuid(security, source, id));

         if (alarmAndEventGuid == Guid.Empty)
         {
            string msg = string.Format("AlarmAndEvent not found in database table (source={0} and ID={1})", source, id);
            eventLog.WriteEntry(msg, EventLogEntryType.Warning);
         }
         else
         {
            alarmAndEvent = FMChannelHelper.MakeCall<IAlarmAndEvents, AlarmAndEventClass>(x => x.Get(security, alarmAndEventGuid));
            if (alarmAndEvent == null || alarmAndEvent.IdentityGuid == Guid.Empty)
            {
               string msg = string.Format("AlarmAndEvent not found in database table (source={0} and ID={1})", source, id);
               eventLog.WriteEntry(msg, EventLogEntryType.Warning);
            }
            else
            {
               categoryID = alarmAndEvent.CategoryID;
               priorityID = alarmAndEvent.PriorityID;
            }
         }

         Dictionary<Guid, List<EmailGroupClass>> existingEmailGroups = new Dictionary<Guid, List<EmailGroupClass>>();

         foreach (var site in sites)
         {
            sec.SiteGuid = site.SiteGuid;
            sec.SiteID = site.SiteID;

            var alarmStatusList = FMChannelHelper.MakeCall<IAlarmStatus, List<AlarmStatusClass2>>(x => x.GetActiveAlarms(sec, unacknowledged, false, true));

            var alarmsNotNormal = alarmStatusList.Where(x => x.IsNormal == false).OrderBy(x => x.AlarmPriority).ThenByDescending(x => x.Timestamp);

            //eventLog.WriteEntry(string.Format("{0}: Number of alarm statuses not normal {1}.", sec.SiteID, alarmsNotNormal.Count()), EventLogEntryType.Error);
            string[] keys = alarmStatusInAlarmAndEventLog.Keys.ToArray();
            foreach (string k in keys)
            {
               DateTime d = alarmStatusInAlarmAndEventLog[k];
               //send once every notification interval
               TimeSpan dt = new TimeSpan(0, 0, notificationInterval, 0);
               if (DateTime.Now - d > dt)
               {
                  alarmStatusInAlarmAndEventLog.Remove(k);
               }
            }
            foreach (var alarmStatus in alarmsNotNormal)
            {
               string emailBody = string.Format("AlarmID={0}; PointID={1}; SiteID={2}; TagID={3}; AlarmPriorityID={4}; Description={5}", 
                                                alarmStatus.AlarmID,
                                                alarmStatus.PointID,
                                                alarmStatus.SiteID,
                                                alarmStatus.TagID,
                                                alarmStatus.AlarmPriorityID,
                                                alarmStatus.Description);

               if (alarmAndEvent.EmailTemplate.IdentityGuid != Guid.Empty)
               {
                     emailBody = alarmAndEvent.EmailTemplate.Body.Replace("${AlarmAndEvent.Data}", emailBody);
               }

               string subject = alarmAndEvent.EmailTemplate.Subject;
               if (subject.Length > 0)
               {
                     emailBody += string.Format("${{Subject={0}}}", subject);
               }

               emailBody = emailBody.Replace("${AlarmID}", alarmStatus.AlarmID)
                                    .Replace("${PointID}", alarmStatus.PointID)
                                    .Replace("${SiteID}", alarmStatus.SiteID)
                                    .Replace("${TagID}", alarmStatus.TagID)
                                    .Replace("${AlarmPriorityID}", alarmStatus.AlarmPriorityID)
                                    .Replace("${Description}", alarmStatus.Description)
                                    .Replace("${ServerName}", Environment.MachineName)
                                    .Replace("${NewLine}", Environment.NewLine);
               string key = string.Format("{0}/{1}/{2}/{3}", alarmStatus.AlarmGuid, alarmStatus.PointGuid, alarmStatus.SiteGuid, alarmStatus.TagGuid);

               if (alarmStatusInAlarmAndEventLog.ContainsKey(key) == false)
               {
                  AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(AlarmStatusClass.PointAlarmStatusNotNormalNotificationDescriptor)
                  {
                     SiteGuid = sec.SiteGuid,
                     SiteID = sec.SiteID,
                     PriorityID = priorityID,
                     CategoryID = categoryID,
                     AssociatedData = emailBody
                  };
                  alarmStatusInAlarmAndEventLog[key] = DateTime.Now;
                  FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(sec, alarmAndEventLog));
                  List<EmailGroupClass> emailGroupCollection = null;
                  if (existingEmailGroups.ContainsKey(sec.SiteGuid) == false)
                  {
                     emailGroupCollection = FMChannelHelper.MakeCall<IEmailGroups, List<EmailGroupClass>>(
                                       emailGroups => emailGroups.EnumerateWithEmailCatAndPriorityInfo(sec));
                     existingEmailGroups.Add(sec.SiteGuid, emailGroupCollection);
                  }
                  else
                  {
                     emailGroupCollection = existingEmailGroups[sec.SiteGuid];
                  }
                  AlarmAndEventProcessing.SendEmail(sec, alarmAndEventLog, site, emailGroupCollection);
               }
            }

         }
         eventLog.WriteEntry(string.Format("alarmStatusEmailQueueSize={0}", alarmStatusInAlarmAndEventLog.Count), EventLogEntryType.Warning);

      }

      #endregion
   }
}
