using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Threading;
using Accounting;
using EngineeringUnitsLibrary;
using System.Configuration;
using ADOFMSImport;
using FMBusinessObjects.Exceptions;

namespace ADFWebApp
{
   public partial class ADOFMSImportForm : AccountingWebFormView
   {
      #region Constants
      protected const string PATH_BASEDIR = "~/App_Data/ADOFMSImport/";

      protected const int LOG_POLL_INTERVAL = 500; // ms
      protected bool m_threadYield = false;

      protected static string PARAM_THREAD_YIELD_FN = typeof(YieldDelegate).ToString();
      protected static string PARAM_ADOFMS_LOGGER = typeof(Logger).ToString();
      protected static string PARAM_ERRORBOX = typeof(TextBox).ToString() + "Error";
      protected static string PARAM_PROGRESSBOX = typeof(TextBox).ToString() + "Progress";
      protected static string PARAM_CONTEXT = typeof(Defaults).ToString();
      protected static string PARAM_UPDATEPANEL_ERROR = typeof(UpdatePanel).ToString() + "Error";
      protected static string PARAM_IMPORT_FILE = "IMPORT FILE";
      #endregion

      #region Yield handling
      protected delegate bool YieldDelegate(bool a_forceYield);
      protected bool ThreadYield(bool a_forceYield)
      {
         if (a_forceYield)
            m_threadYield = true;

         return m_threadYield;
      }
      #endregion // Yield handling

      protected override void OnInit(EventArgs e)
      {
         base.OnInit(e);

         Page.Load += new EventHandler(Page_Load);
         ImportButton.Click += new EventHandler(ImportButton_Click);
         //ImportButton.Attributes["onclick"] = "javascript:disableButtons()";
      }

      protected void Page_Load(object sender, EventArgs e)
      {
         if (Session["Security"] == null)
            base.ErrorHandler(new FMSessionInvalidException());

         m_threadYield = false;
      }

      protected void ThreadedLogWriter(object a_threadParams)
      {
         Hashtable paramTable = a_threadParams as Hashtable;
         if (paramTable != null)
         {
            YieldDelegate yield = paramTable[PARAM_THREAD_YIELD_FN] as YieldDelegate;
            Logger adofmsLogger = paramTable[PARAM_ADOFMS_LOGGER] as Logger;
            TextBox errorBox = paramTable[PARAM_ERRORBOX] as TextBox;
            TextBox progressBox = paramTable[PARAM_PROGRESSBOX] as TextBox;

            try
            {
               string pretext = "ThreadedLogWriter could not instantiate ";
               if (yield == null)
                  throw new Exception(pretext + "Yield function");
               else if (adofmsLogger == null)
                  throw new Exception(pretext + "ADOFMS logger");
               else if (errorBox == null)
                  throw new Exception(pretext + "Error message text box");
               else if (progressBox == null)
                  throw new Exception(pretext + "Progress message text box");
            }
            catch (Exception e)
            {
               base.ErrorHandler(e);
               return;
            }
            

            while (!yield(false) || 
               adofmsLogger.ProgressFile.Length > 0 || 
               adofmsLogger.ErrorFile.Length > 0)
            {
               lock (adofmsLogger.Singleton)
               {
                  // write any progress or errors to the message boxes
                  if (adofmsLogger.ProgressFile.Length > 0)
                  {
                     progressBox.Text += adofmsLogger.ProgressFile;
                     progressBox.DataBind();
                  }

                  if (adofmsLogger.ErrorFile.Length > 0)
                  {
                     errorBox.Text += adofmsLogger.ErrorFile;
                     errorBox.DataBind();
                  }

                  // flush the logger
                  adofmsLogger.Flush();
               }

               Thread.Sleep(LOG_POLL_INTERVAL);
            }
         }
      }

      protected void EnableDisableUpload(bool a_enable)
      {
         ImportButton.Enabled = a_enable;
         FileUpload.Enabled = a_enable;
         FileUpload.BackColor = a_enable ? Color.White : Color.Silver;

         ImportButton.DataBind();
         FileUpload.DataBind();
      }

      protected void ImportButton_Click(object sender, EventArgs e)
      {
         if (Request.Files.AllKeys.Length == 0)
            return; // failsafe

         HttpPostedFile postedFile = FileUpload.PostedFile;
         // do nothing for blank field or file, error messages are un-necessary and interrupting
         if (postedFile.FileName == "" || postedFile.ContentLength == 0)
            return;

         ResultsTextBox_Error.Text = "";
         ResultsTextBox_Progress.Text = "";
         ResultsTextBox_Error.DataBind();
         ResultsTextBox_Progress.DataBind();

         // force create directory
         if (!Directory.Exists(HttpContext.Current.Server.MapPath(PATH_BASEDIR)))
            Directory.CreateDirectory(HttpContext.Current.Server.MapPath(PATH_BASEDIR));

         // save the file
         string guid = Guid.NewGuid().ToString();
         string tempFilePath = HttpContext.Current.Server.MapPath(PATH_BASEDIR) + guid + ".csv";
         postedFile.SaveAs(tempFilePath);

         try
         {
            // disable upload control and import buttons to prevent interruptions
            EnableDisableUpload(true);

            // ADOFMS Import process
            YieldDelegate threadYieldFn = new YieldDelegate(ThreadYield);
            try
            {

               // set up ADOFMS Logger
               using (Logger logger = LoggerManager.CreateLogger(tempFilePath))
               {
                  logger.WriteMode = Defaults.IOMode.UPLOAD;
                  logger.Flush();

                  // prepare the context
                  Defaults adofmsContext = new Defaults();
                  adofmsContext.InputFile = adofmsContext.LoggerKey = tempFilePath;

                  // start the log checker
                  Hashtable paramTable_Logger = new Hashtable();
                  paramTable_Logger[PARAM_ADOFMS_LOGGER] = logger;
                  paramTable_Logger[PARAM_THREAD_YIELD_FN] = threadYieldFn;
                  paramTable_Logger[PARAM_ERRORBOX] = ResultsTextBox_Error;
                  paramTable_Logger[PARAM_PROGRESSBOX] = ResultsTextBox_Progress;

                  ParameterizedThreadStart logStarter = new ParameterizedThreadStart(ThreadedLogWriter);
                  Thread logThread = new Thread(logStarter);
                  logThread.Start(paramTable_Logger);

                  Runner adofmsRunner = new Runner();
                  adofmsRunner.Run(adofmsContext);

                  // write the last of the logs
                  lock (logger.Singleton)
                  {
                     ResultsTextBox_Error.Text += logger.ErrorFile;
                     ResultsTextBox_Progress.Text += logger.ProgressFile;

                     logger.Flush();
                  }
               }
            }
            catch (Exception ex)
            {
               throw ex;
            }
            finally
            {
               EnableDisableUpload(true);

               // delete input file after run
               File.Delete(tempFilePath);

               // yield the logger
               threadYieldFn(true);
            }
         }
         catch (Exception ex)
         {
            base.ErrorHandler(ex);
         }

         // set up thread params
         /*Hashtable paramTable_Import = new Hashtable();
         paramTable_Import[PARAM_IMPORT_FILE] = tempFilePath;
         paramTable_Import[PARAM_ERRORBOX] = ResultsTextBox_Error;
         paramTable_Import[PARAM_PROGRESSBOX] = ResultsTextBox_Progress;
         paramTable_Import[PARAM_UPDATEPANEL_ERROR] = updatePanel_error;

         ParameterizedThreadStart importStarter = new ParameterizedThreadStart(ThreadedImportButton_Click);
         Thread importThread = new Thread(importStarter);
         importThread.Start(paramTable_Import);*/
      }

      /*protected void ThreadedImportButton_Click(object a_param)
      {
         try
         {
            Hashtable paramTable_Import = a_param as Hashtable;
            if (paramTable_Import == null)
               return;

            string fileName = paramTable_Import[PARAM_IMPORT_FILE] as string;
            TextBox errorBox = paramTable_Import[PARAM_ERRORBOX] as TextBox;
            TextBox progressBox = paramTable_Import[PARAM_PROGRESSBOX] as TextBox;
            UpdatePanel errorPanel = paramTable_Import[PARAM_UPDATEPANEL_ERROR] as UpdatePanel;

            // disable upload control and import buttons to prevent interruptions
            FileUpload.Enabled = false;
            ImportButton.Enabled = false;

            // ADOFMS Import process
            YieldDelegate threadYieldFn = new YieldDelegate(ThreadYield);
            try
            {

               // set up ADOFMS Logger
               using (Logger logger = LoggerManager.CreateLogger(fileName))
               {
                  logger.WriteMode = Defaults.IOMode.UPLOAD;
                  logger.Flush();

                  // prepare the context
                  Defaults adofmsContext = new Defaults();
                  adofmsContext.InputFile = fileName;
                  adofmsContext.LoggerKey = fileName;

                  // start the log checker
                  Hashtable paramTable_Logger = new Hashtable();
                  paramTable_Logger[PARAM_ADOFMS_LOGGER] = logger;
                  paramTable_Logger[PARAM_THREAD_YIELD_FN] = threadYieldFn;
                  paramTable_Logger[PARAM_ERRORBOX] = errorBox;
                  paramTable_Logger[PARAM_PROGRESSBOX] = progressBox;
                  paramTable_Logger[PARAM_UPDATEPANEL_ERROR] = errorPanel;

                  ParameterizedThreadStart logStarter = new ParameterizedThreadStart(ThreadedLogWriter);
                  Thread logThread = new Thread(logStarter);
                  logThread.Start(paramTable_Logger);

                  // run the import process on a thread so that it can write results interactively
                  ////Hashtable paramTable_Runner = new Hashtable();
                  //paramTable_Runner[PARAM_THREAD_YIELD_FN] = threadYieldFn;
                  //paramTable_Runner[PARAM_CONTEXT] = adofmsContext;
                  //paramTable_Logger[PARAM_ADOFMS_LOGGER] = logger;

                  //ParameterizedThreadStart runnerStarter = new ParameterizedThreadStart(ThreadedRunImport);
                  //Thread runnerThread = new Thread(runnerStarter);
                  //runnerThread.Start(paramTable_Runner);

                  Runner adofmsRunner = new Runner();
                  adofmsRunner.Run(adofmsContext);
               }
            }
            catch (Exception ex)
            {
               throw ex;
            }
            finally
            {
               FileUpload.Enabled = true;
               ImportButton.Enabled = true;

               // delete input file after run
               File.Delete(fileName);

               // yield the logger
               threadYieldFn(true);
            }
         }
         catch (Exception ex)
         {
            base.ErrorHandler(ex);
         }
      }*/

      protected void ThreadedRunImport(object a_context)
      {
         Hashtable paramTable = a_context as Hashtable;
         if (null == paramTable)
            return; // failsafe

         try
         {
            /*Defaults context = paramTable[PARAM_CONTEXT] as Defaults;
            YieldDelegate yieldFn = paramTable[PARAM_THREAD_YIELD_FN] as YieldDelegate;
            Logger logger = paramTable[PARAM_ADOFMS_LOGGER] as Logger;

            if (context != null)
            {
               Hashtable paramTable_Logger = new Hashtable();
               paramTable_Logger[PARAM_ADOFMS_LOGGER] = logger;
               paramTable_Logger[PARAM_THREAD_YIELD_FN] = yieldFn;
               paramTable_Logger[PARAM_ERRORBOX] = ResultsTextBox_Error;
               paramTable_Logger[PARAM_PROGRESSBOX] = ResultsTextBox_Progress;
               paramTable_Logger[PARAM_UPDATEPANEL_ERROR] = updatePanel_error;

               ParameterizedThreadStart logStarter = new ParameterizedThreadStart(ThreadedLogWriter);
               Thread logThread = new Thread(logStarter);
               logThread.Start(paramTable_Logger);

               Runner adofmsRunner = new Runner();
               adofmsRunner.Run(context);

               // delete input file after run
               File.Delete(context.InputFile);

               // yield the logger
               yieldFn(true);
            }*/
         }
         catch (Exception ex)
         {
            base.ErrorHandler(ex);
         }
      }
   }
}
