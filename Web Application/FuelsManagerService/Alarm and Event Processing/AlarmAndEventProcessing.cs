// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AlarmAndEventProcessing.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//    Provides the ability to process alarms and events in a separate thread.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManagerService
{
   using System;
   using System.Globalization;
   using System.Collections.Generic;
   using System.Net.Mail;
   using System.Runtime.InteropServices;
   using System.Threading;
   using FMBusinessObjects;
   using FMBusinessObjects.BusinessInterfaces;
   using FMBusinessObjects.ChannelFactories;
   using FMBusinessObjects.DataObjects;

   /// <summary>
   /// Provides the ability to process alarms and events in a separate thread.
   /// </summary>
   internal static class AlarmAndEventProcessing
   {
      #region Constants and Fields

      /// <summary>
      /// Specifies the maximum length to allow in email subject lines. Depending on the length of the additional data field,
      /// we may place the additional data in the body or the subject line of the email message.
      /// </summary>
      private const int EmailMaximumSubjectLineLength = 120;

      /// <summary>
      /// This event is signaled when a new alarm or event log record is created
      /// </summary>
      private static readonly AutoResetEvent EventOrAlarmEvent = new AutoResetEvent(false);

      /// <summary>
      /// Stops processing
      /// </summary>
      private static readonly ManualResetEvent KillEvent = new ManualResetEvent(false);

      /// <summary>
      /// The thread responsible for processing
      /// </summary>
      private static Thread processThread = null;

      private static uint rasConnectionHandle = 0;
      private static RasApi.RASDIALEXTENSIONS rasDialExtensions = new RasApi.RASDIALEXTENSIONS();
      private static RasApi.RASDIALPARAMS rasDialParams = new RasApi.RASDIALPARAMS();

      private static Dictionary<string, HashSet<string>> alarmLogsToConsolidate = new Dictionary<string, HashSet<string>>();

      private static bool Started { get {  return processThread != null && processThread.ThreadState == ThreadState.Running; } }  

      #endregion

      #region Methods

      /// <summary>
      ///     Sets the state of the EventOrAlarmEvent to signaled.  This signals the
      ///     AlarmAndEventProcessing thread to begin processing events and alarms.
      /// </summary>
      internal static void SetEventOrAlarmEvent()
      {
         EventOrAlarmEvent.Set();
      }

      /// <summary>
      /// Starts execution of the ProcessThread.  Upon starting this thread loads all sites
      /// from the database into the site cache.  Then it continually loops and calls the
      /// ProcessEventsAndAlarms() method to perform the alarm and event log email processing.
      /// </summary>
      /// <param name="security">
      /// Contains Security Information.
      /// </param>
      internal static void StartProcessThread(SecurityClass security, TimeSpan waitInterval)
      {

         processThread = new Thread(() => ProcessScan(security, waitInterval));
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
      /// Retrieves the set of outstanding alarm and event logs and determines whether to send an
      /// email notification for each log.  When specific conditions are met an email is sent to
      /// the designated recipients for a given log.  The email sequence number is updated to the
      /// log sequence number for each log processed.  This ensures that a log is processed only once.
      /// </summary>
      /// <param name="security">
      /// Contains Security Information
      /// </param>
      private static void ProcessEventsAndAlarms(SecurityClass security)
      {
         try
         {

            // Get the last SequenceNumber Processed
            SequenceClass sequence = new SequenceClass { Key = "AlarmAndEventEmailSequence" };

            sequence.Value = FMChannelHelper.MakeCall<ISequences, long>(sequences => sequences.Get(security, sequence.Key));

            AlarmAndEventLogCollectionClass alarmAndEventLogCollection =
            FMChannelHelper.MakeCall<IAlarmAndEventLogs, AlarmAndEventLogCollectionClass>(
            alarmAndEventLogs => alarmAndEventLogs.EnumerateBySequenceNumber(security, sequence.Value));

            List<SiteClass> allSites = FMChannelHelper.MakeCall<ISites, List<SiteClass>>(sites => sites.EnumerateMailInfo(security));
            Dictionary<Guid, List<EmailGroupClass>> existingEmailGroups = new Dictionary<Guid, List<EmailGroupClass>>();
            var sec = security.Clone();

            foreach (AlarmAndEventLogClass log in alarmAndEventLogCollection)
            {
               
               SiteClass site = allSites.Find(x => x.SiteGuid == log.SiteGuid);

               if (site != null)
               {
                  sec.SiteGuid = site.SiteGuid;
                  sec.SiteID = site.SiteID;
                  List<EmailGroupClass> emailGroupCollection = null;
                  if (existingEmailGroups.ContainsKey(log.SiteGuid) == false)
                  {
                     emailGroupCollection = FMChannelHelper.MakeCall<IEmailGroups, List<EmailGroupClass>>(
                                       emailGroups => emailGroups.EnumerateWithEmailCatAndPriorityInfo(sec));
                     existingEmailGroups.Add(log.SiteGuid, emailGroupCollection);
                  }
                  else
                  {
                     emailGroupCollection = existingEmailGroups[log.SiteGuid];
                  }

                  SendEmail(sec, log, site, emailGroupCollection);
               }
               else
               {
                  FuelsManagerServiceLogger.Instance.LogWarning(string.Format("Site {0} is missing mail server information. ", log.SiteID));
               }
               sequence.Value = log.SequenceNumber;

               FMChannelHelper.MakeCall<ISequences>(sequences => sequences.Save(security, sequence));

               if (KillEvent.WaitOne(0, true))
               {
                  break;
               }
            }

            if (rasConnectionHandle != 0)
            {
               RasApi.RasHangUp(rasConnectionHandle);
            }
         }
         catch (Exception ex)
         {
            FuelsManagerServiceLogger.Instance.LogError(ex);
         }
      }

      internal static void SendEmail(SecurityClass security, AlarmAndEventLogClass log, SiteClass site, List<EmailGroupClass> emailGroupCollection)
      {
         if (Started == false) 
         return;

         string to = "Not available";
         string subject = "Not available";
         string body = "Not available";
            
         try
         {

            security.SiteGuid = log.SiteGuid;


            if (site != null)
            {
                SmtpClient mailClient;

                Uri uri = new Uri("abcd://" + site.MailServer);

                if (uri.Port != -1)
                {
                    mailClient = new SmtpClient { Host = uri.Host, Port = uri.Port };

                    if (uri.Port == 587 || uri.Port == 465)
                            mailClient.EnableSsl = true;

                }
                else
                {
                    mailClient = new SmtpClient { Host = site.MailServer };
                }

                    if (mailClient.Host != site.MailServer || rasDialParams.EntryName != site.DialupName)
               {
                  if (rasConnectionHandle != 0)
                  {
                     RasApi.RasHangUp(rasConnectionHandle);
                     rasConnectionHandle = 0;
                  }

                  rasDialParams.EntryName = site.DialupName;
               }



               foreach (EmailGroupClass emailGroup in emailGroupCollection)
               {
                  if (emailGroup.AlwaysEnabled
                  || (log.CreatedDate.ToLocalTime().TimeOfDay >= emailGroup.StartTime.Value.TimeOfDay
                  && log.CreatedDate.ToLocalTime().TimeOfDay < emailGroup.EndTime.Value.TimeOfDay))
                  {
                     // If no email address is configure, then continue to the next email group.
                     if (emailGroup.EmailAddressCollection.Count == 0)
                     {
                        continue;
                     }

                     bool inCategory = (emailGroup.CategoryCollection.Count == 0 && log.CategoryID == "{None}")
                                 || emailGroup.CategoryCollection.Find(category => category.ID == log.CategoryID) != null;

                     bool inPriority = (emailGroup.PriorityCollection.Count == 0 && log.PriorityID == "{None}")
                                 || emailGroup.PriorityCollection.Find(priority => priority.ID == log.PriorityID) != null;

                     if ((emailGroup.CategoriesAndPriorities && inCategory && inPriority)
                     || (!emailGroup.CategoriesAndPriorities && (inCategory || inPriority)))
                     {
                        MailMessage mailMessage = new MailMessage();
                        mailMessage.BodyEncoding = System.Text.Encoding.UTF8;

                        string from = (string.IsNullOrWhiteSpace(site.MailFrom) ? string.Format("FuelsManager.Service.{0}@no_reply_address.net", site.ID).Replace(" ", "_") : site.MailFrom);
                        mailMessage.From = new MailAddress(from);
                        mailMessage.Subject = "Alarm & Event notification";
                        mailMessage.Body = "Please do not respond to this email, it is for notification only. Responses are not monitored.";

                        if (log.ID != TransactionAlarmEventDO.FMAEInterfaceImportErrorsKey)
                        {
                           // We may place any Associated Data in the subject line of the email we send.
                           // If the associated data is too long, we place it in the body of the email instead.
                           string subjectLineText = log.ID;
                           bool subjectLineProvided = false;
                           if (!string.IsNullOrEmpty(log.AssociatedData))
                           {
                              // Carriage returns and line feeds will cause an ArgumentException if placed in the subject line
                              subjectLineText += " : " + log.AssociatedData.Replace("\n", " ").Replace("\r", " ");
                              const string subjectTag = "${Subject=";
                              int p0 = log.AssociatedData.IndexOf(subjectTag);
                              if (p0 > -1) {
                                 int p1 = log.AssociatedData.IndexOf("}", p0+ subjectTag.Length);
                                 if (p1 > p0)
                                 {
                                    mailMessage.Body = log.AssociatedData.Left(p0);

                                    string txt = log.AssociatedData.Substring(p0 + subjectTag.Length, p1-p0- subjectTag.Length);
                                    mailMessage.Subject =txt;
                                    int len = p1 - p0;
                                    if (mailMessage.Subject.Length > AlarmAndEventProcessing.EmailMaximumSubjectLineLength)
                                    {
                                       mailMessage.Subject = mailMessage.Subject.Left(AlarmAndEventProcessing.EmailMaximumSubjectLineLength);
                                    }                                             
                                    subjectLineProvided = true;
                                 }
                              }
                           }
                           if (subjectLineProvided == false)
                           {
                              // Is our potential subject line text too long? If it is, just use the ID as the subject line
                              // and put the associated data in the body
                              if (subjectLineText.Length > AlarmAndEventProcessing.EmailMaximumSubjectLineLength)
                              {
                                 mailMessage.Subject = log.ID;
                                 mailMessage.Body += Environment.NewLine + Environment.NewLine + log.AssociatedData;
                              }
                              else
                              {
                                 mailMessage.Subject = subjectLineText;
                              }
                           }
                        }
                        else
                        {
                           // We handle FMAE Error alarm and event emails differently due to requirements of the Aviation system.
                           // The differences are: The site should be in the subject line and the AssociatedData should be attached as a file.
                           // Ideally the alarm and event system would be more robust to handle these requirements, and we wouldn't have 
                           // special logic like this in the alarm and event email processing thread									
                           string siteID = string.Empty;

                           if (!string.IsNullOrEmpty(log.AssociatedData) && log.AssociatedData.Length >= 30)
                           {
                              // The site is the first thirty characters of the associated data
                              siteID = log.AssociatedData.Substring(0, 30);
                              log.AssociatedData = log.AssociatedData.Remove(0, 30);
                              siteID = siteID.Trim();

                              // Add the associated data as an attachment named after the site
                              mailMessage.Attachments.Add(Attachment.CreateAttachmentFromString(log.AssociatedData, siteID + "-log.txt"));
                           }

                           mailMessage.Subject = log.ID + ": (Site: " + siteID + ")";
                        }
                        subject = mailMessage.Subject;
                        to = string.Empty;
                        body = mailMessage.Body;
                                
                        // only send one email for each Log id per cycle if id in Consolidate list
                        HashSet<string> alreadySent = null;
                        alarmLogsToConsolidate.TryGetValue(log.ID, out alreadySent);

                        foreach (ApplicationStringMapClass emailAddress in emailGroup.EmailAddressCollection)
                        {
                           if (alreadySent == null || !alreadySent.Contains(emailAddress.ID))
                           {
                              mailMessage.To.Add(emailAddress.ID);
                              to += emailAddress.ID + "; ";
                           }
                           if (alreadySent != null)
                           {
                              alreadySent.Add(emailAddress.ID);
                           }
                        }

                        if (string.IsNullOrEmpty(to))
                        {
                           to = "No e-mail addresses found.";
                           FuelsManagerServiceLogger.Instance.LogWarning(string.Format("No e-mail addresses found.Site ID={0}{1}{2}",site.SiteID, Environment.NewLine, mailMessage.Subject));
                           return;
                        }
                        if (site.MailConnectMode == MAIL_SERVER_CONNECT_MODE.DIALUP)
                        {
                           if (rasConnectionHandle == 0)
                           {
                              uint result;
                              bool passwordFlag;

                              if (0 != (result = RasApi.RasGetEntryDialParams(null, ref rasDialParams, out passwordFlag)))
                              {
                                 FuelsManagerServiceLogger.Instance.LogError("Error: RasGetEntryDialParams Result = " + result.ToString(CultureInfo.InvariantCulture));
                              }
                              else
                              {
                                 if (!passwordFlag)
                                 {
                                    rasDialParams.UserName = site.MailUserName;
                                    rasDialParams.Password = site.MailPassword;
                                 }

                                 if (0 != (result = RasApi.RasDial(ref rasDialExtensions, null, ref rasDialParams, 0, null, ref rasConnectionHandle)))
                                 {
                                    FuelsManagerServiceLogger.Instance.LogError("Error: RasDial Result = " + result.ToString(CultureInfo.InvariantCulture));
                                 }
                                 else
                                 {
                                    mailClient.Send(mailMessage);
                                    FuelsManagerServiceLogger.Instance.LogWarning(string.Format("An e-mail is sent to {0}.{1}Subject: {2}", mailMessage.To, Environment.NewLine,mailMessage.Subject));
                                 }
                              }
                           }
                           else
                           {
                              mailClient.Send(mailMessage);
                              FuelsManagerServiceLogger.Instance.LogWarning(string.Format("An e-mail is sent to {0}.{1}Subject: {2}", mailMessage.To, Environment.NewLine, mailMessage.Subject));
                           }
                        }
                        else
                        {
                           mailClient.Send(mailMessage);
                           FuelsManagerServiceLogger.Instance.LogWarning(string.Format("An e-mail is sent to {0}.{1}Subject: {2}", mailMessage.To, Environment.NewLine, mailMessage.Subject));
                        }
                     }
                  }
               }
            }
         }
         catch (Exception ex)
         {
            FuelsManagerServiceLogger.Instance.LogError(string.Format("Exception occurred while sending e-mail to {0}.{1}Subject: {2}", to, Environment.NewLine, subject));
            FuelsManagerServiceLogger.Instance.LogError(string.Format("Failed to send e-mail to {0} with following content:{1}{2}", to, Environment.NewLine, body));

            FuelsManagerServiceLogger.Instance.LogError(ex);
         }

      }
      /// <summary>
      /// This is the ProcessThread worker method and is executed within the context of
      /// the ProcessThread.  First it loads all sites from the database into the site cache.
      /// Then it continually loops and calls the ProcessEventsAndAlarms() method to perform
      /// the actual alarm and event log email processing.
      /// </summary>
      /// <param name="security">
      /// Contains security information.
      /// </param>
      private static void ProcessScan(SecurityClass security, TimeSpan waitInterval)
      {
         WaitHandle[] events = { KillEvent, EventOrAlarmEvent };

         rasDialExtensions.Size = (uint)Marshal.SizeOf(rasDialExtensions);
         rasDialParams.Size = (uint)Marshal.SizeOf(rasDialParams);

         // only send one email for each Log id per cycle
         foreach (var id in LicenseExpirationAlarmAndEventDescriptors.AlarmEventIds)
         {
            alarmLogsToConsolidate.Add(id, new HashSet<string>());
         }

         while (0 != WaitHandle.WaitAny(events, waitInterval, true))
         {
            try
            {
               // If we're running in the cloud, use a lease on a blob to prevent other instances of the service from processing alarms and events
               // simultaneously.
               //if (RoleEnvironment.IsAvailable)
               //{
               //    BlobLeaseLock.Execute(() => ProcessEventsAndAlarms(security), "AlarmAndEventProcessing");
               //}
               //else
               //{
               ProcessEventsAndAlarms(security);
               //}
            }
            catch (Exception ex)
            {
               FuelsManagerServiceLogger.Instance.LogError(ex);
            }
         }
      }

      #endregion
   }
}
