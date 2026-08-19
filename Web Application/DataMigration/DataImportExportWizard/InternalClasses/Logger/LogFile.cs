namespace DataImportExportWizard.InternalClasses.Logger
{
    using System;
    using System.IO;

    using DataImportExportWizard.UtilityObjects;

    /// <summary>
    /// Summary description for LogFile.
    /// </summary>
    internal class LogFile : BaseTarget
    {
        #region Attributes
        protected System.IO.FileStream writer;
        //		protected string appName;
        protected int index = 0;

        //tables older than this are deleted automatically
        protected int LogTable_DaysOld
        {
            get
            {
                return AppSettingsHelper.GetKeyValue<int>("LogTable_DaysOld", 30);
            }
        }

        //determines whether to delete old log files
        protected bool LogTable_DeleteOld
        {
            get
            {
                return AppSettingsHelper.GetKeyValue<bool>("LogTable_DeleteOld", false);
            }
        }

        #endregion Attributes

        public LogFile(string appName)
            : base(appName)
        {
            Open();
        }

        ~LogFile()
        {
            Close();
        }

        protected internal void StringToByteArray(string theString, ref byte[] data, int offset)
        {
            int realI = 0;
            for (int i = 0; i < theString.Length; ++i)
            {
                realI = (i * 2) + offset;
                data[realI] = (byte)(theString[i] & 0xFF);
                data[realI + 1] = (byte)((theString[i] & 0xFF00) >> 16);
            }
        }

        protected void Open()
        {
            Open_NonAzure();
        }

        #region Overrides
        override protected void Write(string buffer)
        {
            Write_NonAzure(buffer);
        }

        protected override string Format(LogMessage message)
        {
            return base.Format(message);
        }

        internal override void RollLog()
        {
            Close();
            Open();
        }

        internal override void Close()
        {
            Close_NonAzure();
        }

        #endregion Overrides

        #region NonAzure Methods

        /// <summary>
        /// Opens file stream to a log file.  The Open method above originally contained this functionality
        /// Used in Non-Azure deployments
        /// </summary>
        private void Open_NonAzure()
        {
            const string logDirDefault = ".";
            string logDir = "."; //AppSettingsHelper.GetKeyValue<string>("LoggerLogDir", logDirDefault);

            DateTimeOffset now = DateTimeOffset.Now;
            string dateStamp = now.Year.ToString() + "-" + now.Month.ToString("00") + "-" + now.Day.ToString("00");
            string fileName = logDir + "\\" + appName + "_" + dateStamp + ".log";

            writer = new System.IO.FileStream(fileName,
                                                System.IO.FileMode.Append,
                                                System.IO.FileAccess.Write,
                                                System.IO.FileShare.Read,
                                                1,
                                                false);
        }

        /// <summary>
        /// Closes a file steam to the log file.  The Close method above originally contained this functinality
        /// Used in Non-Azure deployments
        /// </summary>
        private void Close_NonAzure()
        {
            if (writer != null)
            {
                writer.Close();
            }
        }

        /// <summary>
        /// Writes an entry to the log file.  The Write method above originally contained this functinality
        /// Used in Non-Azure deployments
        /// </summary>
        private void Write_NonAzure(string buffer)
        {
            ++index;

            buffer += "\r\n";

            System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();

            byte[] byteArray = new Byte[buffer.Length * 2 + 8];
            encoder.GetBytes(buffer, 0, buffer.Length, byteArray, 0);

            writer.Write(byteArray, 0, buffer.Length);
            writer.Flush();
        }

        /// <summary>
        /// Deletes all Azure table logs earlier than the specified date
        /// </summary>
        protected void Delete_NonAzure(string dir)
        {
            if (Directory.Exists(dir))
            {
                foreach (string filename in Directory.GetFiles(dir))
                {
                    try
                    {
                        //do not delete WAD logs
                        if (filename.ToLower().IndexOf("upg_wiz") >= 0)
                        {
                            DateTime FileDate = File.GetCreationTime(filename);
                            if (FileDate.AddDays(LogTable_DaysOld) < DateTime.UtcNow)
                            {
                                File.Delete(filename);
                            }
                        }
                    }
                    catch
                    {
                        //continue with next iteration
                    }
                }
            }
        }


        #endregion
    }



}
