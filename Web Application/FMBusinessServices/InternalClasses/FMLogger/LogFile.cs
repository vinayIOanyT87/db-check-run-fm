/******************************************************************************

	FILE NAME:		LogFile.cs


	PURPOSE:			LogFile Class


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------
		12/04/2008	W.Gray		7.4.6.0 - Revised to issue \r\n rather than \n\r
										so that output formats properly with NotePad.  (CSI 6323)
*******************************************************************************/
using System;
using System.IO;
using FMBusinessObjects.UtilityObjects;

namespace FMBusinessServices.InternalClasses.FMLogger
{
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
			const string logDirDefault = "C:\\Program Files\\FuelsManager\\Logs";
			string logDir = AppSettingsHelper.GetKeyValue<string>("LoggerLogDir", logDirDefault);

			//Delete any old logs
			Delete_NonAzure(logDir);

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
						if (filename.ToLower().IndexOf("wad") == -1)
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

        //#region Azure Methods
        ///// <summary>
        ///// Opens connection to Azure queue storage and creates a new log queue.  Used in Azure deployments
        ///// </summary>
        //private void Open_Azure()
        //{
        //    CloudStorageAccount storageAccount = null;

        //    string dataConnectionStringValue = RoleEnvironment.GetConfigurationSettingValue("DataConnectionString");

        //    // Retrieve storage account from connection-string
        //    // There is a bug with the new Azure Storage 2.0 SDK which causes use development storage to not be parsed correctly
        //    //http://stackoverflow.com/questions/13110488/azure-october-2012-sdk-broke-usedevelopmentstorage-true
        //    if(string.Compare(dataConnectionStringValue,"UseDevelopmentStorage=true", true) == 0)
        //    {
        //        storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
        //    }
        //    else
        //    {
        //        storageAccount = CloudStorageAccount.Parse(dataConnectionStringValue);
        //    }
			
        //    // Create the table client
        //    tableClientLog = storageAccount.CreateCloudTableClient();

        //    //Delete any old logs
        //    Delete_Azure();

        //    //determine table name
        //    DateTimeOffset now = DateTimeOffset.Now;
        //    string dateStamp = now.Year.ToString() + now.Month.ToString("00") + now.Day.ToString("00");
        //    tableName = appName.ToLower() + dateStamp;

        //    //create new table
        //    CloudTable table = tableClientLog.GetTableReference(tableName);
        //    table.CreateIfNotExists();
        //}

        ///// <summary>
        ///// Writes an entry to log in Azure Queue.  Used in Azure deployments
        ///// </summary>
        //private void Write_Azure(string buffer)
        //{
        //    // Create a new log entry
        //    DateTimeOffset now = DateTimeOffset.Now;
        //    string partitionKey = string.Format("{0:D4}_{1:D2}_{2:D2}", now.Year, now.Month, now.Day);
        //    string rowKey = string.Format("{0:D2}_{1:D2}_{2:D2}_{3:D4}_{4:D4}", now.Hour, now.Minute, now.Second, now.Millisecond, now.Ticks % 10000);

        //    AzureLogEntry LogEntry = new AzureLogEntry(partitionKey, rowKey);
        //    LogEntry.Message = buffer;

        //    // Added this code to temporarily write trace information in addition to writing to the log table
        //    // It seems that in the emulator, logging works fine but in the Cloud we write records but they contain no message
        //    if (string.IsNullOrEmpty(buffer))
        //    {
        //        System.Diagnostics.Trace.TraceError("Tried to log a blank message");
        //    }
        //    else
        //    {
        //        System.Diagnostics.Trace.TraceError(buffer);
        //    }

        //    TableOperation insertOperation = TableOperation.Insert(LogEntry);

        //    CloudTable table = tableClientLog.GetTableReference(tableName);

        //    // Submit the operation to the table service
        //    try
        //    {
        //        table.Execute(insertOperation);
        //    }
        //    catch (Exception error)
        //    {
        //        System.Diagnostics.Trace.TraceError(error.Message);
        //        string message = string.Format("{0}-{1}-{2}", partitionKey, rowKey, buffer);
        //        System.Diagnostics.Trace.TraceInformation(message);
        //    }
        //}

        ///// <summary>
        ///// Deletes all Azure table logs earlier than the specified date
        ///// </summary>
        //protected void Delete_Azure()
        //{
        //    DateTime LogDate;
        //    if (LogTable_DeleteOld)
        //    {
        //        foreach (var table in tableClientLog.ListTables())
        //        {
        //            try
        //            {
        //                if (table.Name.ToLower().IndexOf("wad") == -1)
        //                {
        //                    //parse date created from log name
        //                    LogDate = Convert.ToDateTime(table.Name.Substring(table.Name.Length - 4, 2) + "/" + table.Name.Substring(table.Name.Length - 2, 2) + "/" + table.Name.Substring(table.Name.Length - 6, 2));

        //                    if (LogDate.AddDays(LogTable_DaysOld) < DateTime.UtcNow)
        //                    {
        //                        table.DeleteIfExists();
        //                    }
        //                }
        //            }
        //            catch
        //            {
        //                //continue with next iteration
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// AzureLogEntry is a class used to define table structure in Azure Table Storage
        ///// Azure table storage requires that the class implements TableEntity
        ///// </summary>
        //public class AzureLogEntry : TableEntity
        //{
        //    /// <summary>
        //    /// The entity must expose a default constructor
        //    /// </summary>
        //    public AzureLogEntry(){}

        //    public AzureLogEntry(string LogDate, string LogTime)
        //    {
        //        this.PartitionKey = LogDate;
        //        this.RowKey = LogTime;
        //    }

        //    public string Message { get; set; }
        //}

		//#endregion
	}



}
