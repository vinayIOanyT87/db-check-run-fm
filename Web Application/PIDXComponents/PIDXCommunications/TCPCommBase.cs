/// <summary>
/// File name:	TCPCommBase.cs
/// Purpose:	
///	Comments:	Copyright (C) Varec, Inc. Norcross, GA, USA, 
///				2008.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Varec, Inc.
///	Author(s):	Ivan Orndorff
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:		By:				Reason:
///		----------	-------------	-------------------------------------------
///		01-Feb-08	I.Orndorff		1.0.0 - Initial Revision.
///
///		06-Feb-08	I.Orndorff		1.0.1 - Added abstract "SendTransaction()" to 
///											TCPCommBase class.
///										  - Set the ReceiveTimeout on the tcpClient 
///										    in "Initialize()".
///										  - Throw an exception if the result is false 
///										    and the response and exception are empty.
///		
///		04-Mar-08	I.Orndorff		1.0.2 - Added LogFileNamePath and WriteToLogFile methods 
///											to support logging. This fixes CSI #5552.
///										  - Added logging to "OpenConnection()", "CloseConnection()" and
///										    "WriteStream()". This fixes CSI #5552.
///		02-Apr-08	V. Thompson		1.0.3 - Adding Date/Time to each log entry
///		
///		16-Apr-08	W.Gray			7.4.2.1 - Change to retry up to 60 times with 1 seconds
///		
///		09-Jul-08	I.Orndorff		7.4.5.0 - Modified "WriteToLogFile()" to use ToLongTimeString instead 
///											  of ToShortTimeString.
///											  
///		08-Dec-08	W.Gray			7.4.6.0 - Modified OpenConnection to not recreate TcpClinet 
///											during retries, added call to Close on retry, and added retry
///											even when no response.  Modified WriteStream do decrement retires if read
///											returns 0.  Changed ReceiveTimeout from 10000 to 30000
///											
/// </summary>
/// 
using System;
using System.Collections;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using PIDXTransactions;

namespace PIDXCommunications
{
	public abstract class TCPCommBase
   {
		
		public const int TCPCOMM_CONNECT_RETRY  = 60;
		public const int TCPCOMM_CONNECT_TIME_DELAY = 1000;
		public const int TCPCOMM_NUM_RETRY      = 3;
		public const int TCPCOMM_TIMEOUT_MILLI  = 30000;

		#region Private attributes
		private TCPConfig tcpConfig;
		private string loginName;
		private string loginPassword;
		private string logfileNamePath;
		private TcpClient tcpClient;
		private NetworkStream tcpStream;
		private PIDXRecordBase pidxRecord;
		private PIDXAuthorizationBase pidxAuth;
		private string responseString;
		private string exceptionString=String.Empty;
      #endregion
        
      #region Constructors
                /// <summary>
        /// This is the default constructor for the TCPCommBase class.
        /// </summary>
        public TCPCommBase()
        {
            this.Initialize();
        }
        #endregion

        #region Properties
        public string HostName
        {
            get { return this.tcpConfig.HostName; }
            set { this.tcpConfig.HostName = value; }
        }

        public Int32 Port
        {
            get { return this.tcpConfig.Port; }
            set { this.tcpConfig.Port = value; }
        }

        public string LoginName
        {
            get { return this.loginName; }
            set { this.loginName = value; }
        }

        public string LoginPassword
        {
            get { return this.loginPassword; }
            set { this.loginPassword = value; }
        }

		public string LogFileNameandPath
		{
			get { return this.logfileNamePath; }
			set { this.logfileNamePath = value; }
		}

		public bool LoggingEnabled
		{
			get 
			{
				if( this.logfileNamePath != null
				&& 0 != this.logfileNamePath.Length )
					return true;
				else
					return false;
			}
		}

      public NetworkStream TcpStream
      {
         get { return this.tcpStream; }
         set { this.tcpStream = value; }
      }

      public PIDXRecordBase PidxRecord
      {
         get { return this.pidxRecord; }
         set { this.pidxRecord = value; }
      }

      public PIDXAuthorizationBase PidxAuth
      {
         get { return this.pidxAuth; }
         set { this.pidxAuth = value; }
      }

      public string ResponseString
      {
         get { return this.responseString; }
         set { this.responseString = value; }
      }

      public string ExceptionString
      {
         get { return this.exceptionString; }
         set { this.exceptionString = value; }
      }
      #endregion

      #region Protected methods
      protected bool OpenConnection()
      {
         bool result = false;
         
         try
         {
               // validate the host name and port have been set before opening the port
               this.tcpConfig.Validate();
               
				int retries = TCPCOMM_CONNECT_RETRY;
				while (retries > 0 && false == result)
				{
					// Write open connection information to log file
					if( this.LoggingEnabled )
					{
						string headerstring		= "********************************************************************************";
						// vthompson - Added Date/Time to Log entry - CSI 5705
						string connectstring	= "Connecting to " + tcpConfig.HostName + " on port " + tcpConfig.Port.ToString();
						this.WriteToLogFile( headerstring );
						this.WriteToLogFile( connectstring, true );
					}

					// Connect to the specified host 
					this.tcpClient.Connect(tcpConfig.HostName, tcpConfig.Port);

					// Get a client stream for reading and writing.
					tcpStream = this.tcpClient.GetStream();
					if( null != tcpStream )
					{
						// Do derived specific connection / handshaking
						if (this.WriteSpecificConnectionString())
						{
							// only return true if everything completes succesfully
							result = true;
						}

						else
						{
							CloseConnection();
							retries--;

							// retry conection
							if(retries > 0)
							{
								Thread.Sleep(TCPCOMM_CONNECT_TIME_DELAY);
								this.ExceptionString=String.Empty;
							}
						}
					}
					else
						throw new PIDXException(PIDXConstants.ERR_MSG_033);

				}
         }
         catch (SocketException ex)
         {
               this.exceptionString += ex.Message + "\r\n";
         }
         catch (System.IO.IOException ex)
         {
               this.exceptionString += ex.Message + "\r\n";
         }
         catch (PIDXException ex)
         {
               this.ExceptionString += ex.ErrorMessage + "\r\n";
         }
         return result;
      }

      protected bool CloseConnection()
      {
         bool result = false;

         try
         {
				// Write open connection information to log file
				// vthompson - Added Date/Time to log entry.  CSI 5705
				string disconnectstring = "Disconnecting from " + tcpConfig.HostName + 
					" on port " + tcpConfig.Port.ToString();
				this.WriteToLogFile( disconnectstring, true );
				
				this.tcpClient.Close();

				result = true;
         }
         catch (SocketException ex)
         {
               this.exceptionString += ex.Message + "\r\n";
         }
         catch (System.IO.IOException ex)
         {
               this.exceptionString += ex.Message + "\r\n";
         }
			catch (Exception e)
			{
				this.exceptionString += e.Message + "\r\n";
			}
			return result;
      }

      protected bool WriteStream(string DataString)
      {
			bool result = false;

			try
			{
				// Translate the passed message into ASCII and store it as a Byte array.
				Byte[] data = System.Text.Encoding.ASCII.GetBytes(DataString);

				// Write the actual stream to the open connection.
				// vthompson - Added Date/Time to log entry.  CSI 5705
				this.WriteToLogFile(DataString, true);
				this.tcpStream.Write(data, 0, data.Length);

				// Allocate response buffer byte array
				data = new Byte[256];

				// String to store the response ASCII representation.
				this.responseString = String.Empty;

				// Read the response bytes.
				int retries = TCPCOMM_NUM_RETRY;
				while (retries > 0 && false == result )
				{
					try
					{
						Int32 bytes = this.tcpStream.Read(data, 0, data.Length);
						if( 0 != bytes )
						{
							ConvertReceivedBytesToASCII7Bit(data, bytes);
							this.responseString = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
							// vthompson - Added Date/Time to log entry per CSI 5705
							this.WriteToLogFile(responseString, false);

							result = true;
						}
						else
							retries--;
					}
					catch (System.IO.IOException ex)
					{
						this.exceptionString += ex.Message + "\r\n";
						retries--;
					}
				}

				// throw an exception if the result is false and the response and exception are empty
				if((false == result) && (String.Empty == responseString) && (String.Empty == this.ExceptionString))
				{
					throw new PIDXException(PIDXConstants.ERR_MSG_030);
				}
			}
			catch (SocketException ex)
			{
				this.exceptionString += ex.Message + "\r\n";
			}
			catch (System.IO.IOException ex)
			{
				this.exceptionString += ex.Message + "\r\n";
			}
			catch (PIDXException ex)
			{
				this.exceptionString += ex.ErrorMessage + "\r\n";
			}
			return result;
		}
		
		/// <summary>
		/// Writes an entry to the log file
		/// </summary>
		/// <param name="DataString">The data to be written to the log file.</param>
		/// <param name="isOutput">Determines whether or not the data written is input or output</param>
		/// <returns>True if the process is successful.  False if an exception is caught</returns>
		protected bool WriteToLogFile(string DataString, bool isOutput)
		{
			// Make sure logging is enabled.  If not, don't do anything more
			if (!LoggingEnabled)
				return true;

			// Prepend the date/time for the log entry
			string prepend = 
				DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " - ";
			
			// If isOutput is true prepend O: to the DataString otherwise prepend I:
			if (isOutput)
				prepend += "O: ";
			else
				prepend += "I: ";

			// Now write to the log file
			return WriteToLogFile(prepend + DataString);
		}

		protected bool WriteToLogFile(string DataString)
		{
			bool result = false;

			Mutex FileWriteMutex = new Mutex(false, "PIDXFileWriteMutex");
			try
			{
				if( 0 != this.logfileNamePath.Length)
				{
					// vthompson - Appending date to log file name
					// Assumptions: The log file name contains an extension if a '.' is in the file name
					string fileName;
					int indexOfExtension = this.logfileNamePath.LastIndexOf(".");
					// If no '.' is found assume there is no extension
					if (indexOfExtension > -1)
					{
						// A '.' was found.  Parse off the file name
						fileName = this.logfileNamePath.Substring(0, indexOfExtension);
						string extension = this.logfileNamePath.Substring(indexOfExtension, 
							this.logfileNamePath.Length - (indexOfExtension));
						fileName += DateTime.Now.Year.ToString();
						
						// Append the month
						if (DateTime.Now.Month < 10)
							fileName += "0";

						fileName += DateTime.Now.Month.ToString();

						// Append the day
						if (DateTime.Now.Day < 10)
							fileName += "0";

						fileName += DateTime.Now.Day.ToString();

						// Add the extension back
						fileName += extension;
						
					}
					else
					{
						fileName = this.logfileNamePath;
						fileName += DateTime.Now.Year.ToString();
						
						// Append the month
						if (DateTime.Now.Month < 10)
							fileName += "0";

						fileName += DateTime.Now.Month.ToString();

						// Append the day
						if (DateTime.Now.Day < 10)
							fileName += "0";

						fileName += DateTime.Now.Day.ToString();
					}

					System.IO.StreamWriter sw = System.IO.File.AppendText(fileName);
					// wait on the named mutex
					FileWriteMutex.WaitOne();
					sw.WriteLine(DataString);
					sw.Flush();
					sw.Close();
					FileWriteMutex.ReleaseMutex();
					result = true;
				}
				else
				{
					this.exceptionString += "Can't write data string.\r\n";
				}
				return result;
			}
			catch (System.IO.IOException ex)
			{
				FileWriteMutex.ReleaseMutex();
				this.exceptionString += ex.Message + "\r\n";
			}
			return result;
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Initialize()
		{
			try
			{
				this.tcpStream = null;
				this.tcpConfig = new TCPConfig();
				this.tcpClient = new TcpClient();

				// Set a timeout for reading
				tcpClient.ReceiveTimeout = TCPCOMM_TIMEOUT_MILLI;
			}
			catch(PIDXException ex)
			{
				this.exceptionString += ex.Message + "\r\n"; 
			}
		}

		protected void ConvertReceivedBytesToASCII7Bit(Byte[] data, Int32 bytes)
		{
			// in .net 2.0 the characters were changed to 16 bit unsigned integers. The PIDX system is sending 7 bit signed integers
			// inorder to map the characters correctly we need to convert them
			Byte bTemp = 0x00;
			for (int iLoop = 0; iLoop < bytes; iLoop++)
			{
				bTemp = 0x00;
				bTemp = (Byte)(data[iLoop] & 0x7f);	// cast to only get the bottom 7 bits
				data[iLoop] = bTemp;
			}
		}

		#endregion

		#region Abstract methods
		protected abstract bool WriteSpecificConnectionString();
		public abstract bool SendTransaction();
		#endregion

    }
}
