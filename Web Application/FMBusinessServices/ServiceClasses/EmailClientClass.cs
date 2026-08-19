
namespace FMBusinessServices.ServiceClasses
{
    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;
    using FMBusinessServices.DataAccessLayer;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Data;
    using System.Net.Mail;
    using System.Runtime.InteropServices;

    public class EmailClientClass : IEmailClient
    {
        private static DateTime LAST_LICENSE_EXPIRED_EMAIL_SENT = DateTime.MinValue;
        public bool EmailUserByGuid(SecurityClass security, string subjectText, string messageText, Guid toUserGuid)
        {
            return EmailUserByGuid(security, subjectText, messageText, security.UserGuid, toUserGuid);
        }

        public bool EmailUserByGuid(SecurityClass security, string subjectText, string messageText, Guid fromUserGuid, Guid toUserGuid)
        {
            UsersClass users = new UsersClass();
            UserClass toUser = users.Get(security, toUserGuid);

            if (toUser.IdentityGuid == null || toUser.IdentityGuid == Guid.Empty)
            {
                throw new Exception("Could not find user information for " + toUserGuid.ToString() + ".");
            }

            if (string.IsNullOrEmpty(toUser.EmailAddress))
            {
                throw new ApplicationException("No email address configured for: " + toUserGuid.ToString());
            }

            UserClass fromUser = users.Get(security, fromUserGuid);
            if (fromUser.IdentityGuid == null || fromUser.IdentityGuid == Guid.Empty)
            {
                throw new Exception("Could not find user information for " + fromUserGuid.ToString() + ".");
            }

            if (string.IsNullOrEmpty(fromUser.EmailAddress))
            {
                throw new ApplicationException("No email address configured for: " + fromUserGuid.ToString());
            }
            return EmailUser(security, subjectText, messageText, fromUser.EmailAddress, toUser.EmailAddress);
        }

        public bool EmailUserById(SecurityClass security, string subjectText, string messageText, string toUserId)
        {
            return EmailUserById(security, subjectText, messageText, security.UserID, toUserId);
        }

        public bool EmailUserById(SecurityClass security, string subjectText, string messageText, string fromUserId, string toUserId)
        {
            UsersClass users = new UsersClass();
            UserClass toUser = users.GetByID(security, toUserId);

            if (toUser.IdentityGuid == null || toUser.IdentityGuid == Guid.Empty)
            {
                throw new Exception("Could not find user information for " + toUserId + ".");
            }

            if (string.IsNullOrEmpty(toUser.EmailAddress))
            {
                throw new ApplicationException("No email address configured for: " + toUserId);
            }

            UserClass fromUser = users.GetByID(security, fromUserId);
            if (fromUser.IdentityGuid == null || fromUser.IdentityGuid == Guid.Empty)
            {
                throw new Exception("Could not find user information for " + fromUser + ".");
            }

            if (string.IsNullOrEmpty(fromUser.EmailAddress))
            {
                throw new ApplicationException("No email address configured for: " + fromUser);
            }
            return EmailUser(security, subjectText, messageText, fromUser.EmailAddress, toUser.EmailAddress);
        }

        public bool EmailUser(SecurityClass security, string subjectText, string messageText, string fromEmailAddress, string toEmailAddress)
        {
            if (string.IsNullOrEmpty(fromEmailAddress)
                || string.IsNullOrEmpty(toEmailAddress))
            {
                throw new ArgumentNullException("Error: One or both of the From and To Email Addresses are Null or Empty!");
            }
            SitesClass sites = new SitesClass();
            SiteClass site = sites.Get(security, security.SiteGuid, true, true, true);
            if (string.IsNullOrEmpty(site.MailServer))
            {
                return false;
            }
            uint RasConnectionHandle = 0;
            FMBusinessObjects.RasApi.RASDIALEXTENSIONS RasDialExtensions = new FMBusinessObjects.RasApi.RASDIALEXTENSIONS();
            RasDialExtensions.Size = (uint)Marshal.SizeOf(RasDialExtensions);
            FMBusinessObjects.RasApi.RASDIALPARAMS RasDialParams = new FMBusinessObjects.RasApi.RASDIALPARAMS();
            RasDialParams.Size = (uint)Marshal.SizeOf(RasDialParams);

            SmtpClient mailClient = new SmtpClient();
            mailClient.Host = site.MailServer;

            if (mailClient.Host != site.MailServer
                || RasDialParams.EntryName != site.DialupName)
            {
                if (RasConnectionHandle != 0)
                {
                    FMBusinessObjects.RasApi.RasHangUp(RasConnectionHandle);
                    RasConnectionHandle = 0;
                }

                mailClient.Host = site.MailServer;
                RasDialParams.EntryName = site.DialupName;
            }

            MailMessage Message = new MailMessage();
            Message.From = new MailAddress(fromEmailAddress);
            Message.To.Add(new MailAddress(toEmailAddress));
            Message.Subject = subjectText;
            Message.Body = messageText;

            if (site.MailConnectMode == MAIL_SERVER_CONNECT_MODE.DIALUP)
            {
                if (RasConnectionHandle == 0)
                {
                    uint Result;
                    bool PasswordFlag;
                    if (0 != (Result = FMBusinessObjects.RasApi.RasGetEntryDialParams(null, ref RasDialParams, out PasswordFlag)))
                    {
                        throw new ApplicationException("Error: RasGetEntryDialParams Result = " + Result.ToString());
                    }
                    else
                    {
                        if (PasswordFlag == false)
                        {
                            RasDialParams.UserName = site.MailUserName;
                            RasDialParams.Password = site.MailPassword;
                        }

                        if (0 != (Result = FMBusinessObjects.RasApi.RasDial(ref RasDialExtensions, null, ref RasDialParams, 0, null, ref RasConnectionHandle)))
                        {
                            throw new ApplicationException("Error: RasDial Result = " + Result.ToString());
                        }
                        else
                        {
                            mailClient.Send(Message);
                        }
                    }
                }
                else
                {
                    mailClient.Send(Message);
                }
            }
            else
            {
                mailClient.Send(Message);
            }
            return true;
        }


        public void SendExpiredLicenceEmail()
        {
            // Check if license has really expired
            var hardwareKey = new HardwareKeyClass();
            if (!hardwareKey.GetLicenseExpired())
            {
                return;
            }

            if(LAST_LICENSE_EXPIRED_EMAIL_SENT > DateTime.Now.AddDays(-1))
            {
                // Prevent sending multiple emails if FuelsManageService goes through automatic restarts
                return;
            }

            using (var command = new SqlCommand())
            {
                var consolidatedDA = new ConsolidatedDAClass();

                // Get SiteAdmin email server settings
                command.CommandText = "SELECT MailServer,MailPassword,MailUserName,MailFrom,DialupName,LookupMailConnectModeIndex,ShortDatePattern FROM tblSites WHERE SiteGuid = @siteGuid";
                command.Parameters.AddWithValue("@siteGuid", Guids.SiteAdminGuid);
                var set = consolidatedDA.GetDataSet(command, null);
                var row = set.Tables[0].Rows[0];

                string mailServer = DataObject.getString(row["MailServer"]);
                string mailPassword = DataObject.getString(row["MailPassword"]);
                string mailUserName = DataObject.getString(row["MailUserName"]);
                string mailFrom = DataObject.getString(row["MailFrom"]);
                string dialupName = DataObject.getString(row["DialupName"]);
                string shortDatePattern = DataObject.getString(row["ShortDatePattern"]);
                MAIL_SERVER_CONNECT_MODE mailConnectMode = (MAIL_SERVER_CONNECT_MODE)DataObject.getValue(row["LookupMailConnectModeIndex"], (byte)MAIL_SERVER_CONNECT_MODE.LAN);

                //  Get a list of email addresses to send expired license email to
                List<string> emailAdrresses = new List<string>();

                command.CommandText = " SELECT DISTINCT s.ID as email" +
                                        " FROM [dbo].[tblApplicationString] s " +
                                        " JOIN map.tblApplicationStringToEmailAddress m " +
                                        " ON m.ApplicationStringGuid = s.ApplicationStringGuid " +
                                        " JOIN tblEmailGroups g " +
                                        " ON g.EmailGroupGuid = m.EmailGroupGuid " +
                                        " WHERE s.ID <> '' AND " +
                                        "( g.ID = @GroupId OR g.EmailGroupGuid = @emailGroupGuid ) ";

                command.Parameters.AddWithValue("@GroupId", "License Expiration Notification");
                command.Parameters.AddWithValue("@emailGroupGuid", "A1D606A5-BF39-436D-9FC5-A9E7F62C5D0B");

                set = consolidatedDA.GetDataSet(command, null);
                if (set != null && set.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow emailRow in set.Tables[0].Rows)
                    {
                        emailAdrresses.Add(DataObject.getString(emailRow["email"]));
                    }
                }

                // Get the text for the email body
                command.CommandText = " SELECT TOP 1 SettingValue FROM tblConfigurationSetting " +
                                      " WHERE ConfigurationSettingGuid = @ConfigurationSettingGuid " +
                                      " OR SettingKey = @SettingKey " +
                                      " AND SettingValue <> ''";

                command.Parameters.AddWithValue("@SettingKey", "FMLicensePostExpiryEmail");
                command.Parameters.AddWithValue("@ConfigurationSettingGuid", "C2D3865E-B30F-4A45-B376-3A42BE5B57CE");
                set = consolidatedDA.GetDataSet(command, null);
                var messageText = "Please do not respond to this email, it is for notification only. Responses are not monitored.";
                messageText += Environment.NewLine + Environment.NewLine;

                if (set != null && set.Tables[0].Rows.Count >= 1)
                {
                    row = set.Tables[0].Rows[0];
                    messageText += DataObject.getString(row["SettingValue"]);
                }

                var expirationDate = hardwareKey.GetLicenseExpirationDate();
                messageText = string.Format(messageText, expirationDate.ToString(shortDatePattern));
                var subjectText = "Your FuelsManager license has expired";
                SendEmail(mailServer, dialupName, mailConnectMode, mailUserName, mailPassword, subjectText, messageText, mailFrom, emailAdrresses);

                LAST_LICENSE_EXPIRED_EMAIL_SENT =  DateTime.Now;
            }
        }


        // This is intended only for sending expired license emails as we dont check security.
        private bool SendEmail(string mailServer, string dialupName, MAIL_SERVER_CONNECT_MODE mailConnectMode, string mailUserName, string mailPassword, string subjectText, string messageText, string fromEmailAddress, List<string> toEmailAddresses)
        {
            if (string.IsNullOrEmpty(fromEmailAddress)
                || toEmailAddresses.Count == 0)
            {
                throw new ArgumentNullException("Error: One or both of the From and To Email Addresses are Null or Empty!");
            }

            if (string.IsNullOrEmpty(mailServer))
            {
                return false;
            }
            uint RasConnectionHandle = 0;
            FMBusinessObjects.RasApi.RASDIALEXTENSIONS RasDialExtensions = new FMBusinessObjects.RasApi.RASDIALEXTENSIONS();
            RasDialExtensions.Size = (uint)Marshal.SizeOf(RasDialExtensions);
            FMBusinessObjects.RasApi.RASDIALPARAMS RasDialParams = new FMBusinessObjects.RasApi.RASDIALPARAMS();
            RasDialParams.Size = (uint)Marshal.SizeOf(RasDialParams);

            SmtpClient mailClient = new SmtpClient();
            mailClient.Host = mailServer;

            if (mailClient.Host != mailServer
                || RasDialParams.EntryName != dialupName)
            {
                if (RasConnectionHandle != 0)
                {
                    FMBusinessObjects.RasApi.RasHangUp(RasConnectionHandle);
                    RasConnectionHandle = 0;
                }

                mailClient.Host = mailServer;
                RasDialParams.EntryName = dialupName;
            }

            MailMessage Message = new MailMessage();
            Message.BodyEncoding = System.Text.Encoding.UTF8;
            Message.From = new MailAddress(fromEmailAddress);
            foreach(string address in toEmailAddresses)
            {
                Message.To.Add(new MailAddress(address));
            }
            
            Message.Subject = subjectText;
            Message.Body = messageText;

            if (mailConnectMode == MAIL_SERVER_CONNECT_MODE.DIALUP)
            {
                if (RasConnectionHandle == 0)
                {
                    uint Result;
                    bool PasswordFlag;
                    if (0 != (Result = FMBusinessObjects.RasApi.RasGetEntryDialParams(null, ref RasDialParams, out PasswordFlag)))
                    {
                        throw new ApplicationException("Error: RasGetEntryDialParams Result = " + Result.ToString());
                    }
                    else
                    {
                        if (PasswordFlag == false)
                        {
                            RasDialParams.UserName = mailUserName;
                            RasDialParams.Password = mailPassword;
                        }

                        if (0 != (Result = FMBusinessObjects.RasApi.RasDial(ref RasDialExtensions, null, ref RasDialParams, 0, null, ref RasConnectionHandle)))
                        {
                            throw new ApplicationException("Error: RasDial Result = " + Result.ToString());
                        }
                        else
                        {
                            mailClient.Send(Message);
                        }
                    }
                }
                else
                {
                    mailClient.Send(Message);
                }
            }
            else
            {
                mailClient.Send(Message);
            }
            return true;
        }

    }
}