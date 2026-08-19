
/// <summary>
/// File name:	Emailer.cs
/// Purpose:	The purpose of this class is to handle sending email from the 
///             FuelsManager Web App.  This class will wrap the SmtpClient and  
///             MailMessage classes and will manage connecting tot he SMTP server, 
///             sending email, and error handling.
///	Comments:	Copyright (c) Varec, Inc.  All rights reserved.
///	Author(s):	Gregory Lybanon
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:			By:						Reason:
///		----------		--------------------	----------------------------------
///		
/// </summary>

using System;
using System.Configuration;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

//For FMChannelHelper.MakeCall in error logger below
//using FMBusinessObjects.BusinessInterfaces;
//using FMBusinessObjects.ChannelFactories;
//using FMBusinessObjects.Constants;

namespace FMNotificationBusinessServices.UtilityObjects
{
    public class Emailer
    {

        #region Private Properties

        //Properties to populate SMTPClient 
        private string smtpLogin = string.Empty;
        private string smtpPassword = string.Empty;
        private bool enableSsl = false;
        private string host = string.Empty;
        private int port = 25;
        private bool useDefaultCredentials = true;

        //Properties to populate MailMessage
        private string body = string.Empty;
        private string from = string.Empty;
        private bool isBodyHtml = false;
        private string subject = string.Empty;
        private string to = string.Empty;
        private List<Attachment> attachments = new List<Attachment>();

        #endregion

        #region Public Properties

        public string SmtpLogin
        {
            get { return smtpLogin; }
            set { smtpLogin = value; }
        }

        public string SmtpPassword
        {
            get { return smtpPassword; }
            set { smtpPassword = value; }
        }

        public bool EnableSsl
        {
            get { return enableSsl; }
            set { enableSsl = value; }
        }

        public string Host
        {
            get { return host; }
            set { host = value; }
        }

        public int Port
        {
            get { return port; }
            set { port = value; }
        }

        public bool UseDefaultCredentials
        {
            get { return useDefaultCredentials; }
            set { useDefaultCredentials = value; }
        }

        public string Body
        {
            get { return body; }
            set { body = value; }
        }

        public string From
        {
            get { return from; }
            set { from = value; }
        }

        public bool IsBodyHtml
        {
            get { return isBodyHtml; }
            set { isBodyHtml = value; }
        }

        public string Subject
        {
            get { return subject; }
            set { subject = value; }
        }

        public string To
        {
            get { return to; }
            set { to = value; }
        }

        public List<Attachment> Attachments
        {
            get { return attachments; }

        }

        #endregion

        #region Constructors

        public Emailer()
        {
            LoadConfigurations();
        }

        #endregion

        #region Public Methods

        public bool SendMessage()
        {
            try
            {
                if (ValidateProperties())
                {
                    using (SmtpClient client = new SmtpClient())
                    {
                        client.Host = host;
                        client.Port = port;
                        if (!useDefaultCredentials)
                        {
                            NetworkCredential credential = new NetworkCredential(smtpLogin, smtpPassword);
                            client.Credentials = credential;
                        }
                        client.EnableSsl = enableSsl;

                        using (MailMessage msg = new MailMessage())
                        {
                            string[] recipients = to.Split(';');
                            foreach (string recipeint in recipients)
                            {
                                MailAddress address = new MailAddress(recipeint);
                                msg.To.Add(address);
                            }
                            msg.From = new MailAddress(from);
                            msg.Subject = subject;
                            msg.Body = body;
                            msg.IsBodyHtml = isBodyHtml;

                            //add attachments, if any
                            if (attachments.Count > 0)
                            {
                                foreach (Attachment att in attachments)
                                {
                                    msg.Attachments.Add(att);
                                }
                            }

                            client.Send(msg);
                        }
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                //log and/or return error information
                LogError(new Exception(String.Format("Error sending email: {0}", ex.Message)), 4000);
                throw;
            }

        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Validates that the minimum set of properties have been set in order to send an email
        /// </summary>
        /// <returns></returns>
        private bool ValidateProperties()
        {
            try
            {
                //check that minimum information has been set: host, to, from, subject, body
                List<string> errors = new List<string>();

                if (String.IsNullOrEmpty(host))
                {
                    errors.Add("Host not set.");
                }
                if (String.IsNullOrEmpty(to))
                {
                    errors.Add("Destination email address not set.");
                }
                if (String.IsNullOrEmpty(from))
                {
                    errors.Add("Sender email address not set.");
                }
                if (String.IsNullOrEmpty(subject))
                {
                    errors.Add("Email subject not set.");
                }
                if (String.IsNullOrEmpty(body))
                {
                    errors.Add("Email body not set.");
                }

                if (errors.Count > 0)
                {
                    LogError(new Exception(string.Join(Environment.NewLine, errors)), 4101);
                    return false;
                }

                //If we are not using the default credentials, smtpLogin and smtpPassword must be set
                if (!useDefaultCredentials && (String.IsNullOrEmpty(smtpLogin) || String.IsNullOrEmpty(smtpPassword)))
                {
                    LogError(new Exception("SMTP login or password not set for site."), 4102);
                    return false;
                }


                return true;
            }
            catch (Exception ex)
            {
                //log and/or return error information
                LogError(new Exception(String.Format("Error validating email properties: {0}", ex.Message)), 4103);
                throw;
            }
        }

        private void LogError(Exception ex, int eventID)
        {
            try
            {
                string source = "NotificationServiceBusinessObjects";
                string log = "Application";
                if (!EventLog.SourceExists(source))
                {
                    EventLog.CreateEventSource(source, log);
                }

                EventLog.WriteEntry(source, "Exception: " + ex.Message + Environment.NewLine + "Inner Exception: " + (ex.InnerException?.Message ?? String.Empty) + Environment.NewLine + "Stack Trace: " + ex.StackTrace, EventLogEntryType.Error, eventID); ;

                //Use this instead?
                //FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(errorMessage, FMEventLogEntryType.Error));
            }
            catch (Exception exc)
            {
                //error logging error?
            }
        }

        /// <summary>
        /// Reads configuration information from the appSettings section of the application's config file.
        /// </summary>
        /// <param name="key">The key to read from in the appSettings.</param>
        /// <returns>The value of the key requested.</returns>
        private string ReadAppSettings(string key)
        {
            try
            {
                return ConfigurationManager.AppSettings[key].ToString();
            }
            catch (Exception ex)
            {
                //log and/or return error information
                LogError(new Exception(String.Format("Error reading settings from configuration for key {0}: {0}", key, ex.Message)), 4104);
                return String.Empty;
            }
        }

        private void LoadConfigurations()
        {
            try
            {
                Host = ReadAppSettings("mailerHost");
                Port = int.Parse(ReadAppSettings("mailerPort"));
                EnableSsl = Convert.ToBoolean(ReadAppSettings("mailerEnableSsl"));
                UseDefaultCredentials = Convert.ToBoolean(ReadAppSettings("mailerUseDefaultCredentials"));
                SmtpLogin = ReadAppSettings("mailerSmtpLogin");
                SmtpPassword = ReadAppSettings("mailerSmtpPassword");
            }
            catch (Exception ex)
            {
                //log and/or return error information
                LogError(new Exception(String.Format("Error configuring the emailer object: {0}", ex.Message)), 4105);
            }
            #endregion



        }
    }
}
