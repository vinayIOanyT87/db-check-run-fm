using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ADOFMSImport
{
   public class Defaults
   {
      public enum IOMode : int
      {
         FILE,
         UPLOAD
      }

      #region Constants
      public const string FILE_PROGRESS = "progress.log";
      public const string FILE_ERRORS = "errors.log";
      public const string OWNER = "ADO";
      public const string MANAGER = "ADO";
      public const bool STOP_ON_ERROR = false;
      public const IOMode MODE = IOMode.FILE;
      public const string LOGGER_KEY = "ADOFMSImport Standalone";
      #endregion // Constants

      #region Properties
      public IOMode Mode { get; set; }
      public string ProgressFile { get; set; }
      public string ErrorFile { get; set; }
      public string InputFile { get; set; }
      public bool StopOnError { get; set; }
      public string LoggerKey { get; set; }
      #endregion // Properties

      #region Construction
      public Defaults()
      {
         Mode = MODE;

         ProgressFile = FILE_PROGRESS;
         ErrorFile = FILE_ERRORS;
         LoggerKey = LOGGER_KEY;

         // no constants, must be set by user
         InputFile = null;
         StopOnError = STOP_ON_ERROR;
      }
      #endregion // Construction
   }
}
