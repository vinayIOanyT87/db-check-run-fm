 #pragma warning disable 1587
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
#pragma warning restore 1587
namespace FMBusinessObjects.PIDXTransactions
{
    using System;
    using System.IO;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;

    using FMBusinessObjects.Constants;
    using FMBusinessObjects.Exceptions;

    public class TcpCommTds : TcpCommBase
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor for the TDS TCP Communication class.
		/// </summary>
		public TcpCommTds ( )
		{
			this.Initialize ( );
		}
		#endregion

		#region Properties
		#endregion

		#region Abstract method implementations
		/// <summary>
		/// This method implement connection / handshaking with the TDS server.
		/// </summary>
		/// <returns></returns>
		protected override bool WriteSpecificConnectionString ( )
		{
			bool result = false;

			try
			{
				string message = "H";

				// Translate the passed message into ASCII and store it as a Byte array.
				byte[] data = Encoding.ASCII.GetBytes ( message );

				// Send the message to the connected TcpServer. 
				// send first 'H' then wait 1/2 second between sending second 'H'
				// vthompson - Added Date/Time to log entry per CSI 5705
				this.WriteToLogFile ( message, true );
				this.TcpStream.Write ( data, 0, data.Length );
				Thread.Sleep ( 500 );
				// vthompson - Added Date/Time to log entry per CSI 5705
				this.WriteToLogFile ( message, true );
				this.TcpStream.Write ( data, 0, data.Length );

				// Buffer to store the response bytes.
				data = new byte[256];

				// String to store the response ASCII representation.

			    // Read the first batch of the TcpServer response bytes.
				int bytes = this.TcpStream.Read(data,0,data.Length);
				this.ConvertReceivedBytesToAscii7Bit(data,bytes);
				var responseData = Encoding.ASCII.GetString(data,0,bytes);
				// vthompson - Added Date/Time to log entry per CSI 5705
				this.WriteToLogFile ( responseData, false );

				// if the response containg "U#=" then user username,password
				if (-1 != responseData.IndexOf ( "U#=", StringComparison.Ordinal ))
				{
					message = this.LoginName + "," + this.LoginPassword + "\r";
					data = Encoding.ASCII.GetBytes ( message );
					// vthompson - Added Date/Time to log entry per CSI 5705
					this.WriteToLogFile ( message, true );
					this.TcpStream.Write ( data, 0, data.Length );

					// Read the next batch of TcpServer response bytes.
					data = new byte[256];
					responseData = string.Empty;
					bytes = this.TcpStream.Read ( data, 0, data.Length );
					if (0 != bytes)
					{
						this.ConvertReceivedBytesToAscii7Bit(data,bytes);
						responseData = Encoding.ASCII.GetString(data,0,bytes);
						// vthompson - Added Date/Time to log entry per CSI 5705
						this.WriteToLogFile ( responseData, false );
						this.ResponseString = responseData;
						if (-1 != responseData.IndexOf ( "R?", StringComparison.Ordinal ))
						{
							result = true;
						}

						else if (-1 != responseData.IndexOf ( "VALIDATION FAULT", StringComparison.Ordinal ))
							throw new PIDXException ( PIDXConstants.ERR_MSG_032 );
					}
				}

				// throw an exception if the result is false and the response and exception are empty
				if (( false == result ) && ( string.Empty == responseData ) && ( string.Empty == this.ExceptionString ))
				{
					throw new PIDXException ( PIDXConstants.ERR_MSG_030 );
				}
			}
			catch (SocketException ex)
			{
				this.ExceptionString += ex.Message + "\r\n";
			}
			catch (IOException ex)
			{
				this.ExceptionString += ex.Message + "\r\n";
			}
			catch (PIDXException ex)
			{
				this.ExceptionString += ex.ErrorMessage + "\r\n";
			}
			return result;
		}

		public override bool SendTransaction ( )
		{
			bool result = false;

			Monitor.Enter ( this );

			try
			{
				// Process load authorization record (LA)
				if(typeof(LoadAuthorizationRecord) == this.PidxRecord.GetType())
				{
					if(this.OpenConnection())
					{
						LoadAuthorizationRecord la = (LoadAuthorizationRecord)this.PidxRecord;

						int retries = TcpcommNumRetry;
						while(retries > 0 && false == result)
						{
							if(this.WriteStream(la.GetDataRecord(this.Version) + '\r'))
							{
								if(-1 != this.ResponseString.IndexOf("AUTH", StringComparison.Ordinal))
								{
									AuthorizationGrantedLA auth = new AuthorizationGrantedLA();

									int responseindex = this.ResponseString.IndexOf("AUTH", StringComparison.Ordinal);
									string actualresponse = this.ResponseString.Substring(responseindex);
									auth.Parse(actualresponse);

									if(auth.ValidateCheckBit())
									{
										this.PidxAuth = auth;
										result = true;
									}
									else
									{
										// ******************** IGO **********************
										// need to write retransmit code when bad check bit
										// ******************** IGO **********************
									}
								}
								else if(-1 != this.ResponseString.IndexOf("DENY", StringComparison.Ordinal))
								{
									AuthorizationDenyLA deny = new AuthorizationDenyLA();

									int responseindex = this.ResponseString.IndexOf("DENY", StringComparison.Ordinal);
									string actualresponse = this.ResponseString.Substring(responseindex);
									deny.Parse(actualresponse);
									this.PidxAuth = deny;
									result = true;
								}
								else if(-1 != this.ResponseString.IndexOf("E!", StringComparison.Ordinal))
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
					}
				}


				// Process credit authorization record (CA)
				else if(typeof(CreditAuthorizationRecord) == this.PidxRecord.GetType())
				{
					if (this.OpenConnection ( ))
					{
						CreditAuthorizationRecord ca = (CreditAuthorizationRecord) this.PidxRecord;

						int retries = TcpcommNumRetry;
						while(retries > 0 && false == result)
						{
							if(this.WriteStream(ca.GetDataRecord(this.Version) + '\r'))
							{
								if(-1 != this.ResponseString.IndexOf("AUTH", StringComparison.Ordinal))
								{
									AuthorizationGrantedCA auth = new AuthorizationGrantedCA();

									auth.Parse(this.ResponseString);

									if(auth.ValidateCheckBit())
									{
										this.PidxAuth = auth;
										result = true;
									}
									else
									{
										// ******************** IGO **********************
										// need to write retransmit code when bad check bit
										// ******************** IGO **********************
									}
								}
								else if(-1 != this.ResponseString.IndexOf("DENY", StringComparison.Ordinal))
								{
									AuthorizationDenyCA deny = new AuthorizationDenyCA();

									deny.Parse(this.ResponseString);
									this.PidxAuth = deny;
									result = true;
								}
								else if(-1 != this.ResponseString.IndexOf("E!", StringComparison.Ordinal))
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
					}
				}

				// Process BOL record (BB)
				else if (typeof ( BOLBBRecord ) == this.PidxRecord.GetType ( ))
				{
					if (this.OpenConnection ( ))
					{
						BOLBBRecord bb = (BOLBBRecord) this.PidxRecord;

						int retries = TcpcommNumRetry;
						while(retries > 0 && false == result)
						{
							if(this.WriteStream(bb.GetDataRecord(this.Version) + '\r'))
							{
								if(-1 != this.ResponseString.IndexOf("R?", StringComparison.Ordinal))
								{
									result = true;
								}
								else if(-1 != this.ResponseString.IndexOf("E!", StringComparison.Ordinal))
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
					}
				}

				// Process completed BOL record (CB)
				else if (typeof ( BOLCBRecord ) == this.PidxRecord.GetType ( ))
				{
					if (this.OpenConnection ( ))
					{
						BOLCBRecord cb = (BOLCBRecord) this.PidxRecord;

						int retries = TcpcommNumRetry;
						while(retries > 0 && false == result)
						{
							if(this.WriteStream(cb.GetDataRecord(this.Version) + '\r'))
							{
								if(-1 != this.ResponseString.IndexOf("R?", StringComparison.Ordinal))
								{
									result = true;
								}
								else if(-1 != this.ResponseString.IndexOf("E!", StringComparison.Ordinal))
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
					}
				}

				// Process BOL record (BL)
				else if(typeof(BOLBLRecord) == this.PidxRecord.GetType())
				{
					if(this.OpenConnection())
					{
						BOLBLRecord bl = (BOLBLRecord)this.PidxRecord;

						int retries = TcpcommNumRetry;
						while(retries > 0 && false == result)
						{
							if(this.WriteStream(bl.GetDataRecord(this.Version) + '\r'))
							{
								if(-1 != this.ResponseString.IndexOf("R?", StringComparison.Ordinal))
								{
									result = true;
								}
								else if(-1 != this.ResponseString.IndexOf("E!", StringComparison.Ordinal))
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
					}
				}

				// Non supported record type
				else
				{
					throw new PIDXException ( PIDXConstants.ERR_MSG_029 );
				}
			}
			catch (PIDXException ex)
			{
				this.ExceptionString += ex.ErrorMessage + "\r\n";
			}
			catch (Exception ex)
			{
				this.ExceptionString += ex.Message + "\r\n";
			}
			finally
			{
				this.CloseConnection();
				Monitor.Exit(this);
			}

			return result;
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Initialize ( )
		{
		}
		#endregion
	}
}