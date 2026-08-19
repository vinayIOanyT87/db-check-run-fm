/// <summary>
/// File name:	TCPCommTDS.cs
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
///		02-Feb-08	I.Orndorff		1.0.0 - Initial Revision.
///		
///		05-Feb-08	I.Orndorff		1.0.1 - Added support for BB and CB transactions.
///		
///		06-Feb-08	I.Orndorff		1.0.2 - Added abstract "SendTransaction()" to 
///											TCPCommBase class.
///										  - Throw an exception if the result is false 
///										    and the response and exception are empty.
///										    
///		04-Mar-08	I.Orndorff		1.0.3 - Added logging to "WriteSpecificConnectionString()".
///											This fixes CSI #5552.
///		
///		02-Apr-08	V.Thompson		1.0.4 - Modifed calls to base.WriteToLogFile to meet
///											requirements of CSI's 5704 & 5705.
///											
///		09-Jul-08	I.Orndorff		1.0.5 - Modified "SendTransaction()" to use Monitor.Enter and 
///											Monitor.Exit. This will prevent reentrant collisions.
///											
///		22-Jun-09	W.Gray			7.4.6.0 - Modified Exception Handling (CSI 4168)
///		
///		
/// </summary>
/// 
using System;
using System.Collections;
using System.Threading;
using System.Text;
using System.Net.Sockets;
using PIDXTransactions;

namespace PIDXCommunications
{
    public class TCPCommTDS : TCPCommBase
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor for the TDS TCP Communication class.
        /// </summary>
        public TCPCommTDS()
        {
            this.Initialize();
        }
        #endregion

        #region Properties
        #endregion

        #region Abstract method implementations
        /// <summary>
        /// This method implement connection / handshaking with the TDS server.
        /// </summary>
        /// <returns></returns>
        protected override bool WriteSpecificConnectionString()
        {
            bool result = false;

			try
			{
				String message = "H";

				// Translate the passed message into ASCII and store it as a Byte array.
				Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

				// Send the message to the connected TcpServer. 
				// send first 'H' then wait 1/2 second between sending second 'H'
				// vthompson - Added Date/Time to log entry per CSI 5705
				base.WriteToLogFile(message, true);
				base.TcpStream.Write(data, 0, data.Length);
				System.Threading.Thread.Sleep(500);
				// vthompson - Added Date/Time to log entry per CSI 5705
				base.WriteToLogFile(message, true);
				base.TcpStream.Write(data, 0, data.Length);
                
				// Buffer to store the response bytes.
				data = new Byte[256];

				// String to store the response ASCII representation.
				String responseData = String.Empty;

				// Read the first batch of the TcpServer response bytes.
				Int32 bytes = base.TcpStream.Read(data, 0, data.Length);
				ConvertReceivedBytesToASCII7Bit(data, bytes);
				responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
				// vthompson - Added Date/Time to log entry per CSI 5705
				base.WriteToLogFile(responseData, false);

				// if the response containg "U#=" then user username,password
				if (-1 != responseData.IndexOf("U#="))
				{
					message = base.LoginName + "," + base.LoginPassword + "\r";
					data = System.Text.Encoding.ASCII.GetBytes(message);
					// vthompson - Added Date/Time to log entry per CSI 5705
					base.WriteToLogFile(message, true);
					base.TcpStream.Write(data, 0, data.Length);

					// Read the next batch of TcpServer response bytes.
					data = new Byte[256];
					responseData = String.Empty;
					bytes = base.TcpStream.Read(data, 0, data.Length);
					if( 0 != bytes )
					{
						ConvertReceivedBytesToASCII7Bit(data, bytes);
						responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
						// vthompson - Added Date/Time to log entry per CSI 5705
						base.WriteToLogFile(responseData, false);
						base.ResponseString = responseData;
						if (-1 != responseData.IndexOf("R?"))
						{
							result = true;
						}

						else if(-1 != responseData.IndexOf("VALIDATION FAULT"))
							throw new PIDXException(PIDXConstants.ERR_MSG_032);
					}
				}

				// throw an exception if the result is false and the response and exception are empty
				if((false == result) && (String.Empty == responseData) && (String.Empty == base.ExceptionString))
				{
					throw new PIDXException(PIDXConstants.ERR_MSG_030);
				}
			}
			catch (SocketException ex)
			{
				base.ExceptionString += ex.Message + "\r\n";
			}
			catch (System.IO.IOException ex)
			{
				base.ExceptionString += ex.Message + "\r\n";
			}
			catch (PIDXException ex)
			{
				base.ExceptionString += ex.ErrorMessage + "\r\n";
			}
         return result;
      }

		public override bool SendTransaction()
		{
			bool result = false;
            
			Monitor.Enter(this);

			try
			{
				// Process credit authorization record (CA)
				if (typeof(CreditAuthorizationRecord) == base.PidxRecord.GetType())
				{
					if (base.OpenConnection())
					{
						CreditAuthorizationRecord ca = (CreditAuthorizationRecord)base.PidxRecord;

						int retries = TCPCOMM_NUM_RETRY;
						while (retries > 0 && false == result)
						{
							if (base.WriteStream(ca.GetDataRecord() + '\r'))
							{
								if (-1 != base.ResponseString.IndexOf("AUTH"))
								{
									AuthorizationGrantedCA auth = new AuthorizationGrantedCA();

									auth.Parse(base.ResponseString);

									if (auth.ValidateCheckBit())
									{
										base.PidxAuth = auth;
										result = true;
									}
									else
									{
										// ******************** IGO **********************
										// need to write retransmit code when bad check bit
										// ******************** IGO **********************
									}
								}
								else if (-1 != base.ResponseString.IndexOf("DENY"))
								{
									AuthorizationDeny deny = new AuthorizationDeny();

									deny.Parse(base.ResponseString);
									base.PidxAuth = deny;
									result = true;
								}
								else if (-1 != base.ResponseString.IndexOf("E!"))
								{
									// resend last transaction
									retries--;
								}
							}
							else
							{
								// write stream failed try again
								retries--;
							}
						}
						base.CloseConnection();
					}
				}

				// Process BOL record (BB)
				else if (typeof(BOLBBRecord) == base.PidxRecord.GetType())
				{
					if (base.OpenConnection())
					{
						BOLBBRecord bb = (BOLBBRecord)base.PidxRecord;

						int retries = TCPCOMM_NUM_RETRY;
						while (retries > 0 && false == result)
						{
							if (base.WriteStream(bb.GetDataRecord() + '\r'))
							{
								if (-1 != base.ResponseString.IndexOf("R?"))
								{
									result = true;
								}
								else if (-1 != base.ResponseString.IndexOf("E!"))
								{
									// resend last transaction
									retries--;
								}
							}
							else
							{
								// write stream failed try again
								retries--;
							}
						}
						base.CloseConnection();
					}
				}

				// Process completed BOL record (CB)
				else if (typeof(BOLCBRecord) == base.PidxRecord.GetType())
				{
					if (base.OpenConnection())
					{
						BOLCBRecord cb = (BOLCBRecord)base.PidxRecord;

						int retries = TCPCOMM_NUM_RETRY;
						while (retries > 0 && false == result)
						{
							if (base.WriteStream(cb.GetDataRecord() + '\r'))
							{
								if (-1 != base.ResponseString.IndexOf("R?"))
								{
									result = true;
								}
								else if (-1 != base.ResponseString.IndexOf("E!"))
								{
									// resend last transaction
									retries--;
								}
							}
							else
							{
								// write stream failed try again
								retries--;
							}
						}
						base.CloseConnection();
					}
				}

				// Non supported record type
				else
				{
					throw new PIDXException(PIDXConstants.ERR_MSG_029);
				}
			}
			catch (PIDXException ex)
			{
				base.ExceptionString += ex.ErrorMessage + "\r\n";
			}
			catch (Exception ex)
			{
				base.ExceptionString += ex.Message + "\r\n";
			}
			finally
			{
				Monitor.Exit(this);
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
        }
        #endregion
    }
}
