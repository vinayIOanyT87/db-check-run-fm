using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

// copied from DBPopulateComponents with some modifications,
// should one day consolidate this so that it is flexible for
// writing to stdout or writing to file.
namespace ADOFMSImport
{
   public class Logger : IDisposable
   {
      #region Attributes
      //protected static Logger m_instance = LoggerManager.CreateLogger("DEFAULT");
      public object Singleton = new object();
      protected Hashtable m_sources = new Hashtable();
      protected LoggerManager m_manager = null;
      protected string m_key;

      internal string m_progressFile = "";
      internal string m_errorFile = "";
      #endregion // Attributes

      #region Properties
      public string ErrorFile { get { return m_errorFile; } set { m_errorFile = value;  } }
      public string ProgressFile { get { return m_progressFile; } set { m_progressFile = value; } }
      public Defaults.IOMode WriteMode { get; set; }
      #endregion // Properties

      internal Logger(LoggerManager a_manager, string a_key)
      {
         m_manager = a_manager;
         m_key = a_key;

         ErrorFile = null;
         ProgressFile = null;
         WriteMode = Defaults.MODE;
      }

      /*public static Logger GetInstance()
      {
         lock (Singleton)
         {
            if (m_instance == null)
            {
               m_instance = new Logger();
            }

            return m_instance;
         }
      }*/

      public void WriteLog(string a_message, ref string a_file)
      {
         lock (Singleton)
         {
            DateTime logTime = DateTime.Now;
            string message = logTime.ToShortDateString() + " " + logTime.ToShortTimeString() + ": " + a_message;

            switch (WriteMode)
            {
               case Defaults.IOMode.FILE:
                  using (StreamWriter writer = new StreamWriter(a_file, true))
                  {
                     writer.WriteLine(message);
                  }
                  break;
               case Defaults.IOMode.UPLOAD:
                  a_file += message + "\n";
                  break;
            }
         }
      }

      /*public static void LogError(string a_message)
      {
         Logger logger = Logger.GetInstance();
         logger.WriteLog(a_message, logger.ErrorFile);
      }

      public static void LogProgress(string a_message)
      {
         Logger logger = Logger.GetInstance();
         logger.WriteLog(a_message, logger.ProgressFile);
      }*/

      public void Flush()
      {
         lock (Singleton)
         {
            switch (WriteMode)
            {
               case Defaults.IOMode.FILE:
                  using (StreamWriter writer = new StreamWriter(ErrorFile)) { }
                  using (StreamWriter writer = new StreamWriter(ProgressFile)) { }
                  break;
               case Defaults.IOMode.UPLOAD:
                  ErrorFile = "";
                  ProgressFile = "";
                  break;
            }
         }
      }

      #region IDisposable members
      public void Dispose()
      {
         m_manager.DeleteLogger(m_key);
      }
      #endregion // IDisposable members
   }
}
