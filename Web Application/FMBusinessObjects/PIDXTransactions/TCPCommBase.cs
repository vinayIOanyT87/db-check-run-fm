namespace FMBusinessObjects.PIDXTransactions
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;

    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Exceptions;

    public abstract class TcpCommBase
	{
		public const int TcpcommConnectRetry  = 2;
		public const int TcpcommConnectTimeDelay = 1000;
		public const int TcpcommNumRetry      = 3;
		public const int TcpcommTimeoutMilli  = 30000;

		#region Private attributes
		private TcpConfig tcpConfig;

        private string logfileNamePath;
		private TcpClient tcpClient;
		private NetworkStream tcpStream;

        private string responseString;
		private string exceptionString = String.Empty;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the TCPCommBase class.
		/// </summary>
		protected TcpCommBase()
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

		public int Port
		{
			get { return this.tcpConfig.Port; }
			set { this.tcpConfig.Port = value; }
		}

        public string LoginName { get; set; }

        public string LoginPassword { get; set; }

		public string LogFileNameandPath
		{
			get { return this.logfileNamePath; }
			set { this.logfileNamePath = value; }
		}

        public PIDXVersion Version { get; set; }

        public bool LoggingEnabled
		{
			get
			{
			    if (!string.IsNullOrEmpty(this.logfileNamePath))
			    {
			        return true;
			    }
			    
                return false;
			}
		}

		public NetworkStream TcpStream
		{
			get { return this.tcpStream; }
			set { this.tcpStream = value; }
		}

        public PIDXRecordBase PidxRecord { get; set; }

        public PIDXAuthorizationBase PidxAuth { get; set; }

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

				int retries = TcpcommConnectRetry;
				while (retries > 0 && false == result)
				{
					// Write open connection information to log file
					if (this.LoggingEnabled)
					{
						const string Headerstring = "********************************************************************************";
						
                        // vthompson - Added Date/Time to Log entry - CSI 5705
						string connectstring = "Connecting to " + this.tcpConfig.HostName + " on port " + this.tcpConfig.Port.ToString(CultureInfo.InvariantCulture);
						this.WriteToLogFile(Headerstring);
						this.WriteToLogFile(connectstring, true);
					}

					// Connect to the specified host 
					this.tcpClient = new TcpClient { ReceiveTimeout = TcpcommTimeoutMilli };
				    this.tcpClient.Connect(this.tcpConfig.HostName, this.tcpConfig.Port);

					// Get a client stream for reading and writing.
					this.tcpStream = this.tcpClient.GetStream();
				    if (null != this.tcpStream)
				    {
				        // Do derived specific connection / handshaking
				        if (this.WriteSpecificConnectionString())
				        {
				            // only return true if everything completes succesfully
				            result = true;
				        }
				        else
				        {
				            this.CloseConnection();
				            retries--;

				            // retry conection
				            if (retries > 0)
				            {
				                Thread.Sleep(TcpcommConnectTimeDelay);
				                this.ExceptionString = string.Empty;
				            }
				        }
				    }
				    else
				    {
				        throw new PIDXException(PIDXConstants.ERR_MSG_033);
				    }
				}
			}
			catch (SocketException ex)
			{
				this.exceptionString += ex.Message + "\r\n";
			}
         catch (IOException ex)
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
				string disconnectstring = "Disconnecting from " + this.tcpConfig.HostName + " on port "
				                          + this.tcpConfig.Port.ToString(CultureInfo.InvariantCulture);
			    this.WriteToLogFile(disconnectstring, true);

				this.tcpClient.Close();

				result = true;
			}
			catch (SocketException ex)
			{
				this.exceptionString += ex.Message + "\r\n";
			}
         catch (IOException ex)
			{
				this.exceptionString += ex.Message + "\r\n";
			}
			catch (Exception e)
			{
				this.exceptionString += e.Message + "\r\n";
			}
			return result;
		}

		protected bool WriteStream(string dataString)
		{
			bool result = false;

			try
			{
				// Translate the passed message into ASCII and store it as a Byte array.
				byte[] data = Encoding.ASCII.GetBytes(dataString);

				// Write the actual stream to the open connection.
				// vthompson - Added Date/Time to log entry.  CSI 5705
			    this.WriteToLogFile(dataString, true);
				this.tcpStream.Write(data, 0, data.Length);
                Thread.Sleep(1000);

				// Allocate response buffer byte array
				data = new byte[2048];

				// String to store the response ASCII representation.
				this.responseString = string.Empty;

                // Read the response bytes.i
                int retries = TcpcommNumRetry;
                while (retries > 0 && false == result)
                {
                    try
                    {
                        int bytes = this.tcpStream.Read(data, 0, data.Length);
                        if (0 != bytes)
                        {
                            this.ConvertReceivedBytesToAscii7Bit(data, bytes);
                            this.responseString = Encoding.ASCII.GetString(data, 0, bytes);

                            // vthompson - Added Date/Time to log entry per CSI 5705
                            this.WriteToLogFile(this.responseString, false);

                            result = true;
                        }
                        else
                        {
                            retries--;
                        }
                    }
                    catch (IOException ex)
                    {
                        this.exceptionString += ex.Message + "\r\n";
                        retries--;
                    }
                }

                // throw an exception if the result is false and the response and exception are empty
                if ((false == result) && (string.Empty == this.responseString) && (string.Empty == this.ExceptionString))
				{
					throw new PIDXException(PIDXConstants.ERR_MSG_030);
				}
			}
			catch (SocketException ex)
			{
				this.exceptionString += ex.Message + "\r\n";
			}
			catch (IOException ex)
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
		/// <param name="dataString">The data to be written to the log file.</param>
		/// <param name="isOutput">Determines whether or not the data written is input or output</param>
		/// <returns>True if the process is successful.  False if an exception is caught</returns>
		protected bool WriteToLogFile(string dataString, bool isOutput)
		{
			// Make sure logging is enabled.  If not, don't do anything more
		    if (!this.LoggingEnabled)
		    {
		        return true;
		    }

			// Prepend the date/time for the log entry
			string prepend =
				DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " - ";

			// If isOutput is true prepend O: to the DataString otherwise prepend I:
		    if (isOutput)
		    {
		        prepend += "O: ";
		    }
		    else
		    {
		        prepend += "I: ";
		    }

			// Now write to the log file
			return this.WriteToLogFile(prepend + dataString);
		}

		protected bool WriteToLogFile(string dataString)
		{
			bool result = false;

			var fileWriteMutex = new Mutex(false, "PIDXFileWriteMutex");
			fileWriteMutex.WaitOne();
			try
			{
			    if (0 != this.logfileNamePath.Length)
			    {
					// vthompson - Appending date to log file name
					// Assumptions: The log file name contains an extension if a '.' is in the file name
					string fileName;
					int indexOfExtension = this.logfileNamePath.LastIndexOf(".", StringComparison.Ordinal);
					
                    // If no '.' is found assume there is no extension
					if (indexOfExtension > -1)
					{
						// A '.' was found.  Parse off the file name
					    fileName = this.logfileNamePath.Substring(0, indexOfExtension);
					    string extension = this.logfileNamePath.Substring(
					        indexOfExtension,
					        this.logfileNamePath.Length - indexOfExtension);
					    fileName += DateTime.Now.Year.ToString(CultureInfo.InvariantCulture);

					    // Append the month
					    if (DateTime.Now.Month < 10)
					    {
					        fileName += "0";
					    }

					    fileName += DateTime.Now.Month.ToString(CultureInfo.InvariantCulture);

						// Append the day
					    if (DateTime.Now.Day < 10)
					    {
					        fileName += "0";
					    }

						fileName += DateTime.Now.Day.ToString(CultureInfo.InvariantCulture);

						// Add the extension back
						fileName += extension;
					}
					else
					{
						fileName = this.logfileNamePath;
						fileName += DateTime.Now.Year.ToString(CultureInfo.InvariantCulture);

						// Append the month
					    if (DateTime.Now.Month < 10)
					    {
					        fileName += "0";
					    }

						fileName += DateTime.Now.Month.ToString(CultureInfo.InvariantCulture);

						// Append the day
					    if (DateTime.Now.Day < 10)
					    {
					        fileName += "0";
					    }

						fileName += DateTime.Now.Day.ToString(CultureInfo.InvariantCulture);
					}

					StreamWriter sw = File.AppendText(fileName);
					sw.WriteLine(dataString);
					sw.Flush();
					sw.Close();
					result = true;
				}
				else
				{
					this.exceptionString += "Can't write data string.\r\n";
				}

				return result;
			}
			catch (IOException ex)
			{
				this.exceptionString += ex.Message + "\r\n";
			}
			finally
			{
				fileWriteMutex.ReleaseMutex();
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
				this.tcpConfig = new TcpConfig();
			}
			catch (PIDXException ex)
			{
				this.exceptionString += ex.Message + "\r\n";
			}
		}

		protected void ConvertReceivedBytesToAscii7Bit(byte[] data, int bytes)
		{
			// in .net 2.0 the characters were changed to 16 bit unsigned integers. The PIDX system is sending 7 bit signed integers
		    // inorder to map the characters correctly we need to convert them
		    for (int loop = 0; loop < bytes; loop++)
		    {
		        var temp = (byte)(data[loop] & 0x7f);
		        data[loop] = temp;
		    }
		}

		#endregion

		#region Abstract methods
		protected abstract bool WriteSpecificConnectionString();
		
        public abstract bool SendTransaction();
		#endregion
	}
}