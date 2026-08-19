using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using FMNotificationBusinessObjects.DataObjects;
using FMNotificationBusinessServices.DataAccess;
using FMNotificationBusinessServices.UtilityObjects;
using FMBusinessObjects.DataObjects;

namespace FMNotificationService
{
    public partial class DirectoryWatcher : ServiceBase
    {
        private Timer timer1 = new Timer();

        #region Private Fields

        private List<ErrorNotificationConfig> _errorNotificationConfigs = new List<ErrorNotificationConfig>();
        private string _baseDir = string.Empty;

        #endregion

        public DirectoryWatcher()
        {
            InitializeComponent();
            _baseDir = ReadAppSettings("BaseDirectory");
        }

        protected override void OnStart(string[] args)
        {
            //load configurations
            LogError(new Exception("Loading configurations"), EventLogEntryType.Information, 0);
            LoadErrorNotificationConfigurations();

            LogError(new Exception("Setting timer1 properties"), EventLogEntryType.Information, 0);
            timer1.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer1.Interval = 60000;
            timer1.AutoReset = true;
            timer1.Enabled = true;
            LogError(new Exception("Starting timer1."), EventLogEntryType.Information, 0);
            timer1.Start();
        }

        protected override void OnStop()
        {
            timer1.Stop();
            timer1.Enabled = false;
        }

        #region Public Methods

        public void ProxyStart()
        {
            this.OnStart(null);
        }

        public void ProxyStop()
        {
            this.OnStop();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Main thread handler for processing enterprise export error emails.
        /// </summary>
        private void EnterpriseExportEmailThreadHandler()
        {
            try
            {
                //check for any configuration updates
                CheckForEmailConfigChanges();

                foreach (ErrorNotificationConfig config in _errorNotificationConfigs)
                {
                    SendEnterpriseErrorEmails(config);
                }
                   
            }
            catch (Exception ex)
            {
                LogError(ex,EventLogEntryType.Error,1000);
            }
        }

        /// <summary>
        /// Calls configuration validation routine, then sends the email.
        /// </summary>
        private void SendEnterpriseErrorEmails(ErrorNotificationConfig config)
        {
            if (ValidateEntErrorEmailConfig(config))
            {
                SendEmailWithAttachments(config);
            }
        }
        
        /// <summary>
        /// Validates that site email configuration is correct.
        /// </summary>
        /// <returns>Success or failure.</returns>
        private bool ValidateEntErrorEmailConfig(ErrorNotificationConfig config)
        {

            //Verify that Configuration Exists
            if (config == null)
            {
                return false;
            }
            //Verify that folder is not empty
            if (config.ErrorFolder == "")
            {
                return false;
            }
            else
            {
                //Verify that folder exists
                string path = Path.Combine(_baseDir,config.ErrorFolder);
                if (!Directory.Exists(path))
                {
                    return false;
                }
            }

            //Verify that emails are configured for this site
            if (config.EmailAddresses.Length == 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// If there are files in the site-specific folder, create an email message, attach the files
        /// then send them to the configured email(s).
        /// </summary>
        /// <param name="config">Site error notification configuration</param>
        private void SendEmailWithAttachments(ErrorNotificationConfig config)
        {
            string path = string.Empty;
            

            //If there is no site configuration
            if (config == null)
            {
                return;
            }

            //If the error folder is not defined in the site configuration
            if (config.ErrorFolder == "")
            {
                return;
            }

            //If no email addresses for this configuration exist
            if (config.EmailAddresses.Length == 0)
            {
                return;
            }

            //If directory does not exist or there are no files in the directory
            path = Path.Combine(_baseDir, config.ErrorFolder);
            if (!Directory.Exists(path) || Directory.GetFiles(path, "*.rtf").Length == 0)
            {
                return;
            }

            Emailer mailer = new Emailer();
            string[] files = null;

            SortedList<string, string> listOfFilesSent = new SortedList<string, string>();

            try
            {
                //Construct Mail Message
                mailer.From = "team-foundation@varec.com"; //"datacenter@varec.com";
                mailer.To = config.EmailAddresses;
                mailer.Subject = "FMAE Export Errors: (Enterprise System: " + Environment.MachineName + ") - (Site: " + config.SiteId + ")";
                mailer.Body = "Attached are error files for this site";
                mailer.IsBodyHtml = false;

                files = Directory.GetFiles(path, "*.rtf");

                foreach (string fileName in files)
                {
                    try
                    {
                        // Create  the file attachment for this e-mail message.
                        Attachment data = new Attachment(fileName, MediaTypeNames.Text.Plain);
                        // Add time stamp information for the file.
                        ContentDisposition disposition = data.ContentDisposition;
                        disposition.CreationDate = System.IO.File.GetCreationTime(fileName);
                        disposition.ModificationDate = System.IO.File.GetLastWriteTime(fileName);
                        disposition.ReadDate = System.IO.File.GetLastAccessTime(fileName);
                        mailer.Attachments.Add(data);
                        listOfFilesSent.Add(fileName, "");
                    }
                    catch(Exception ex)
                    {
                        //log sending error to Application Event Log
                        LogError(new Exception(String.Format("Unable to attach file {0}. Reason: {1}",fileName,ex.Message)), EventLogEntryType.Error, 1001);
                    }
                }
            }
            catch (Exception ex)
            {
                //log sending error to Application Event Log
                LogError(new Exception("Unable to create MailMessage Object for Site: " + config.SiteId + Environment.NewLine + "Reason:  " + ex.Message), EventLogEntryType.Error, 1002);
                return;
            }

            //Send Message
            try
            {
                if (mailer.Attachments.Count > 0)
                {
                    if(mailer.SendMessage())
                    { 
                        LogError(new Exception("Email with Export Errors Sent for Site: " + config.SiteId), EventLogEntryType.Information, 3000);
                    }
                    else
                    {
                        throw new Exception("Error validating email properties");
                    }
                }

                files = Directory.GetFiles(path);
                //Delete All Files from folder after sending
                if (files != null)
                {
                    foreach (string fileName in files)
                    {
                        try
                        {
                            if (listOfFilesSent.Keys.Contains(fileName))
                            {
                                File.Delete(fileName);
                            }
                        }
                        catch (Exception ex)
                        {
                            LogError(new Exception("Unable to delete file: " + fileName + Environment.NewLine + "Reason:  " + ex.Message), EventLogEntryType.Warning, 1003);
                        }
                    }
                }
            }
            catch (SmtpException ex)
            {
                LogError(new Exception("Unable to send MailMessage Object for Site: " + config.SiteId + Environment.NewLine +
                                  "SMTP System: " + mailer.Host + Environment.NewLine +
                                  "Reason:  " + ex.Message + Environment.NewLine +
                                  "Error Code: " + ex.StatusCode.ToString() + Environment.NewLine +
                                  "Inner Exception: " + ex.InnerException.ToString() + Environment.NewLine +
                                  "Stack Trace: " + ex.StackTrace), EventLogEntryType.Error, 1004);
                return;
            }
            catch (Exception ex)
            {
                LogError(new Exception("Unable to send MailMessage Object for Site: " + config.SiteId + Environment.NewLine + "Reason:  " + ex.Message), EventLogEntryType.Error, 5001);
                return;
            }
        }


   private void LoadErrorNotificationConfigurations()
        {
            try
            {
                //get data
                List<ErrorNotificationConfig> dataReturned = new List<ErrorNotificationConfig>();
                //mock security class
                SecurityClass sc = new SecurityClass();
                sc.AddRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS);
                //replace this once the project is moved from RICE into the main app
                ErrorNotificationConfigDAL configDAL = new ErrorNotificationConfigDAL();
                dataReturned = configDAL.GetErrorNotificationConfigs(sc);

                foreach (ErrorNotificationConfig config in dataReturned)
                {
                    _errorNotificationConfigs.Add(config);
                }
            }
            catch (Exception ex)
            {
                LogError(ex, EventLogEntryType.Error, 1005);
            }

        }

        private void CheckForEmailConfigChanges()
        {
            try
            {
                List<ErrorNotificationConfig> list = new List<ErrorNotificationConfig>();
                //mock security class
                SecurityClass sc = new SecurityClass();
                sc.AddRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS);
                // replace this once the project is moved from RICE into the main app
                ErrorNotificationConfigDAL configDAL = new ErrorNotificationConfigDAL();
                list = configDAL.GetErrorNotificationConfigs(sc);

                foreach (ErrorNotificationConfig config in list)
                {
                    ErrorNotificationConfig mainConfig = _errorNotificationConfigs.Where(x => x.SiteId == config.SiteId).FirstOrDefault();
                    if (mainConfig == null)
                    {
                        _errorNotificationConfigs.Add(config);
                        LogError(new Exception("Email Configuration data for SiteID: \"" + config.SiteId + "\" was added by \"" + config.CreatedBy + "\" on " + config.CreatedDate.Value.ToUniversalTime().ToString("r")), EventLogEntryType.Information, 1236);
                    }
                    else if(config.UpdatedDate != mainConfig.UpdatedDate) //check updated date to see if the data has changed
                    {
                        LogError(new Exception("Email Configuration data for SiteID: \"" + config.SiteId + "\" was changed by \"" + config.UpdatedBy + "\" on " + config.UpdatedDate.Value.ToUniversalTime().ToString("r")), EventLogEntryType.Information, 1237);
                        mainConfig.SiteId = config.SiteId;
                        mainConfig.CreatedBy = config.CreatedBy;
                        mainConfig.CreatedDate = config.CreatedDate;
                        mainConfig.EmailAddresses = config.EmailAddresses;
                        mainConfig.ErrorFolder = config.ErrorFolder;
                        mainConfig.SiteGuid = config.SiteGuid;
                        mainConfig.UpdatedBy = config.UpdatedBy;
                        mainConfig.UpdatedDate = config.UpdatedDate;
                    }
                }

                //determine list of configs to remove
                List<ErrorNotificationConfig> configs = new List<ErrorNotificationConfig>();
                foreach (ErrorNotificationConfig mainConfig in _errorNotificationConfigs)
                {
                    if (!list.Exists(x => x.SiteGuid == mainConfig.SiteGuid))
                    {
                        LogError(new Exception("Email Configuration data for SiteID: \"" + mainConfig.SiteId + "\" was deleted"), EventLogEntryType.Information, 1238);
                        configs.Add(mainConfig);
                    }
                }
                //remove configs
                foreach (ErrorNotificationConfig config in configs)
                {
                    _errorNotificationConfigs.Remove(config);
                }
            }
            catch(Exception ex)
            {
                LogError(ex, EventLogEntryType.Error, 1007);
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
                LogError(new Exception(String.Format("Error reading settings from configuration for key {0}: {1}", key, ex.Message)), EventLogEntryType.Warning,1006);
                return String.Empty;
            }
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            EnterpriseExportEmailThreadHandler();
        }

        #endregion


        #region Logs

        /// <summary>
        /// Simple method for adding error messages to the Windows Application Event Log
        /// </summary>
        /// <param name="ex">The exception thrown.</param>
        /// <param name="logType">Type of event</param>
        /// <param name="eventId">Event Id</param>
        private void LogError(Exception ex, EventLogEntryType logType, int eventId)
        {
            try
            {
                string source="NotificationService";
                string log = "Application";
                if(!EventLog.SourceExists(source))
                {
                    EventLog.CreateEventSource(source, log);
                }

                switch (logType)
                {
                    case EventLogEntryType.Error:
                        EventLog.WriteEntry(source, "Exception: " + ex.Message + Environment.NewLine + "Inner Exception: " + (ex.InnerException?.Message ?? String.Empty) + Environment.NewLine + "Stack Trace: " + ex.StackTrace, logType, eventId);
                        break;
                    case EventLogEntryType.Warning:
                        EventLog.WriteEntry(source, "Warning: " + ex.Message, logType, eventId);
                        break;
                    case EventLogEntryType.Information:
                        EventLog.WriteEntry(source, ex.Message, logType, eventId);
                        break;
                    case EventLogEntryType.SuccessAudit:
                        EventLog.WriteEntry(source, ex.Message, logType, eventId);
                        break;
                    case EventLogEntryType.FailureAudit:
                        EventLog.WriteEntry(source, ex.Message, logType, eventId);
                        break;
                    default:
                        EventLog.WriteEntry(source, ex.Message, logType, eventId);
                        break;
                }
                

                //Use this instead?
                //FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(errorMessage, FMEventLogEntryType.Error));
            }
            catch (Exception exc)
            {
                //error logging error?
            }
        }

        #endregion
    }
}
