using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace ADOFMSImport
{
   public class LoggerManager
   {
      #region Attributes
      protected static LoggerManager m_instance = new LoggerManager();
      protected static object m_singleton = new object();
      protected Hashtable m_loggerTable = new Hashtable();
      #endregion // Attributes

      public static void LogError(string a_key, string a_message)
      {
         LoggerManager logger = LoggerManager.GetInstance();
         logger.WriteLog_Error(a_key, a_message);
      }

      public static void LogProgress(string a_key, string a_message)
      {
         LoggerManager logger = LoggerManager.GetInstance();
         logger.WriteLog_Progress(a_key, a_message);
      }

      #region Construction

      protected LoggerManager()
      {

      }

      public static LoggerManager GetInstance()
      {
         lock (m_singleton)
         {
            if (m_instance == null)
            {
               m_instance = new LoggerManager();
            }

            return m_instance;
         }
      }

      #endregion // Construction

      public void WriteLog_Error(string a_key, string a_message)
      {
         Logger logger = GetLoggerEx(a_key);
         if (logger != null)
         {
            logger.WriteLog(a_message, ref logger.m_errorFile);
         }
      }

      public void WriteLog_Progress(string a_key, string a_message)
      {
         Logger logger = GetLoggerEx(a_key);
         if (logger != null)
         {
            logger.WriteLog(a_message, ref logger.m_progressFile);
         }
      }

      public static Logger CreateLogger(string a_key)
      {
         return LoggerManager.GetInstance().CreateLoggerEx(a_key);
      }

      internal void DeleteLogger(string a_key)
      {
         if (m_loggerTable.Contains(a_key))
            m_loggerTable.Remove(a_key);
      }

      protected Logger CreateLoggerEx(string a_key)
      {
         m_loggerTable[a_key] = new Logger(this, a_key);

         return m_loggerTable[a_key] as Logger;
      }

      protected Logger GetLoggerEx(string a_key)
      {
         Logger result = null;

         if (m_loggerTable.Contains(a_key))
            result = m_loggerTable[a_key] as Logger;

         return result;
      }
   }
}
