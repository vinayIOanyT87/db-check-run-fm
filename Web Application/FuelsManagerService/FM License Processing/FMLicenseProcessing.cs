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
    using System.Diagnostics;
    using System.Threading;
    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;

    class FMLicenseProcessing
    {
        #region Constants and Fields

        /// <summary>
        /// Stops processing
        /// </summary>
        private static readonly ManualResetEvent KillEvent = new ManualResetEvent(false);


        // Default email if copy in DB configuration settings is not found
        private static readonly string preExpiryEmail = "Hello!\r\n\r\n" +
            "We hope you have been enjoying your experience with Varec's FuelsManager solution. We wanted to send you a friendly reminder that your software license subscription is due to expire in {0} day{1} on {2}.\r\n\r\n" +
            "To ensure uninterrupted access to all the features and benefits you have come to rely on, please renew your subscription before the expiration date to avoid any loss of access.\r\n\r\n" +
            "To renew your subscription, simply contact Varec Sales:\r\n\r\n" +
            "   E-mail:  Sales@varec.com\r\n" +
            "   Web:  https://www.varec.com/contact/sales-support/\r\n" + "" +
            "   Phone:  +1 770-447-9202 (US) or +1 866-698-2732 (Internationally)\r\n" +
            "\r\n" +
            "If you have any questions or need assistance, please don't hesitate to contact our support team:\r\n" +
            "\r\n" +
            "   E-mail:  Support@varec.com\r\n" +
            "   Web:  https://www.varec.com/contact/technical-support/\r\n" +
            "   Phone:  +1 770-446-0818 (US) or +1 800-999-6708 (Internationally)\r\n" +
            "\r\n" +
            "Thank you for being a valued customer of FuelsManager,\r\n" +
            "Varec, Inc., a wholly owned subsidiary of Leidos";

        private static readonly string postExpiryEmail = "Hello!\r\n" +
            "\r\n" +
            "We hope you have been enjoying your experience with Varec's FuelsManager solution. We wanted to send you a friendly reminder that your software license subscription has expired on {0}.\r\n" +
            "\r\n" +
            "To regain access to all the features and benefits you have come to rely on, please renew your subscription now.\r\n" +
            "\r\n" +
            "To renew your subscription, simply contact Varec Sales:\r\n" +
            "\r\n" +
            "   E-mail:  Sales@varec.com\r\n" +
            "   Web:  https://www.varec.com/contact/sales-support/\r\n" +
            "   Phone:  +1 866-698-2732 (US) or +1 770-447-9202 (Internationally)\r\n" +
            "If you have any questions or need assistance, please don't hesitate to contact our support team:\r\n" +
            "\r\n" +
            "   E-mail:  Support@varec.com\r\n" +
            "   Web:  https://www.varec.com/contact/technical-support/\r\n" +
            "   Phone:  +1 800-999-6708 (US) or +1 770-446-0818 (Internationally)\r\n" +
            "Thank you for being a valued customer of FuelsManager,\r\n" +
            "Varec, Inc., a wholly owned subsidiary of Leidos";

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
            try
            {
            CheckLicenseExpirationAndNotify(security);
            }
            catch (Exception ex)
            {
                FuelsManagerServiceLogger.Instance.LogError(ex);
            }

            DateTime current = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,0,0,0);
            DateTime nextDay = current.AddDays(1);
            TimeSpan waitInterval = nextDay - current ;
            WaitHandle[] events = { KillEvent };
            DateTime start = DateTime.Now;

            while (0 != WaitHandle.WaitAny(events, waitInterval, true))
            {
                try
                {
                    CheckLicenseExpirationAndNotify(security);
                }
                catch (Exception ex)
                {
                    FuelsManagerServiceLogger.Instance.LogError(ex);
                }
                current = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                nextDay = current.AddDays(1);
                waitInterval = nextDay - current;
            }
        }

        private static void CheckLicenseExpirationAndNotify(SecurityClass security)
        {
            DateTime expiryDate = FMChannelHelper.MakeCall<IHardwareKey, DateTime>(x => x.GetLicenseExpirationDate());
            bool licenseExpired = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.GetLicenseExpired());
            if (licenseExpired)
            {

                string msg = string.Format(postExpiryEmail, expiryDate);
                eventLog.WriteEntry(msg, EventLogEntryType.Warning);

                // Can send via Alarm and Event logs once services are altered to run with expired license
                FMChannelHelper.MakeCall<IEmailClient>(x => x.SendExpiredLicenceEmail());

                eventLog.WriteEntry("Invalid or Expired Software License File Detected.", EventLogEntryType.Error);
                return;
            }

            SiteClass adminSite = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(security, security.LoginSiteGuid,false,false,false));
            var dateTimeFormat = adminSite.GetDateTimeFormatInfo();
            var formatedDate = expiryDate.ToString("d", dateTimeFormat);

            long daysLeft = FMChannelHelper.MakeCall<IHardwareKey, long>(x => x.GetLicenseDaysLeftToExpire());
            if (daysLeft <= 90)
            {
                AlarmAndEventLogCollectionClass alarmAndEventLogs = new AlarmAndEventLogCollectionClass();
                AlarmAndEventLogClass alarmAndEventLog = null;
                DateTimeOffset current = DateTimeOffset.Now;

                DateTimeOffset beginning = new DateTimeOffset(current.Year, current.Month, current.Day, 0, 0, 0, current.Offset);
                DateTimeOffset ending = new DateTimeOffset(current.Year, current.Month, current.Day, 23, 59, 59, current.Offset);

                string categoryID = String.Empty;
                string priorityID = "Notify";
                bool includeMemberSites = true;
                bool queryArchiveDb = false;
                bool includeGlobalSites = true;
                int days = 90;

                if(daysLeft <= LicenseExpirationAlarmAndEventDescriptors.NUMBER_DAYS_FOR_DAILY_REMINDER)
                {
                    days = (int)daysLeft;
                    alarmAndEventLog = new AlarmAndEventLogClass(new AlarmAndEventDescriptorClass(true, BaseObjectClass.License, string.Format(LicenseExpirationAlarmAndEventDescriptors.IDxDaysBeforeLicenseExpire, daysLeft, daysLeft > 1 ? "s" : string.Empty)));
                }
                else if (daysLeft <= 30)
                {
                    //check alarm and event log if user acknowledged 30 day license expiration alert
                    days = 30;
                    alarmAndEventLog = new AlarmAndEventLogClass(LicenseExpirationAlarmAndEventDescriptors.alarmEventDescriptorFor30DayBeforeLicenseExpire);

                }
                else if (daysLeft <= 60)
                {
                    //check alarm and event log if user acknowledged 60 day license expiration alert
                    days = 60;
                    alarmAndEventLog = new AlarmAndEventLogClass(LicenseExpirationAlarmAndEventDescriptors.alarmEventDescriptorFor60DayBeforeLicenseExpire);
                    
                }
                else
                {
                    // 90 days or less
                    days = 90;
                    alarmAndEventLog = new AlarmAndEventLogClass(LicenseExpirationAlarmAndEventDescriptors.alarmEventDescriptorFor90DayBeforeLicenseExpire);
                }

                beginning = beginning.AddDays(daysLeft - days);

                string source = alarmAndEventLog.Source;
                string type = alarmAndEventLog.Alarm ? "Alarm" : "Event";
                string id = alarmAndEventLog.ID;
                AlarmAndEventClass alarmAndEvent = null;
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




                alarmAndEventLogs = FMChannelHelper.MakeCall<IAlarmAndEventLogs, AlarmAndEventLogCollectionClass>(
                    x => x.Enumerate(security,
                            beginning,
                            ending,
                            source,
                            type,
                            id,
                            categoryID,
                            priorityID,
                            includeMemberSites,
                            queryArchiveDb,
                            includeGlobalSites)
                    );
                //string dbg = string.Format("alarmAndEventLogs.count={0}   beginning={1}    ending={2}   daysleft={3}  Guid={4}", alarmAndEventLogs.Count, beginning, ending, daysLeft, alarmAndEventLog.IdentityGuid);
                //eventLog.WriteEntry(dbg, EventLogEntryType.Warning);

                if (alarmAndEventLogs.Count == 0)
                {
                    string msg = string.Format("Notification generated because less than {0} day{1} left for license to expire on {2}.", daysLeft, daysLeft == 1 ? string.Empty : "s", formatedDate);
                    eventLog.WriteEntry(msg, EventLogEntryType.Warning);
                    var configSettingDo = FMChannelHelper.MakeCall<IConfigurationSettings, ConfigurationSettingDOClass>(x => x.GetByKey(security, "FMLicensePreExpiryEmail"));
                    var sites = FMChannelHelper.MakeCall<ISites, SiteCollectionClass>(s => s.Enumerate(security));
                    var emailBody = configSettingDo == null  || string.IsNullOrWhiteSpace(configSettingDo.SettingValue) ? preExpiryEmail : configSettingDo.SettingValue;
                    alarmAndEventLog.AssociatedData = string.Format(emailBody, daysLeft, daysLeft ==1 ? string.Empty:"s", formatedDate);
                    alarmAndEventLog.CategoryID = categoryID;
                    alarmAndEventLog.PriorityID = priorityID;
                    foreach(var site in sites)
                    {
                        var sec =security.Clone();
                        sec.SiteGuid = site.SiteGuid;
                        alarmAndEventLog.SiteGuid = site.SiteGuid;
                        FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(sec, alarmAndEventLog));
                    }
                    
                    
                }
            }
        }

        #endregion
    }
}
