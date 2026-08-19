/******************************************************************************

	FILE NAME:		LogServer.cs


	PURPOSE:			LogServer Class


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------
		12/04/2008	W.Gray		7.4.6.0 - Revised instantiate LoggerImpl and then
										export it with RemotingServices.Marshal.  This
										allows LogClient to be loaded from the GAC (CSI 6323)
*******************************************************************************/
using System;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Lifetime;


using LogClient;

namespace LogService
{
	/// <summary>
	/// Summary description for LogServer.
	/// </summary>
    /// 

    //NOTE: The LogServer Windows Service is no longer hosting the LoggerImpl .NET Remoting service. The LoggerImpl functionality has been moved to the LoggerService WCF Service.
	public class LogServer
	{
		#region Attributes
		protected System.Runtime.Remoting.Channels.Tcp.TcpServerChannel channel;
		//protected LoggerImpl loggerImpl;
		protected EventLog eventLog=new EventLog("Application",".","FuelsManager Service");
		#endregion Attributes

		public LogServer()
		{
		}

		public void Start()
		{
			try
			{
				int port = 0;
				Microsoft.Win32.RegistryKey Key =
					Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\Varec\\Logger", false);
				if(Key != null)
				{
					port = (int) Key.GetValue("port",8086);
				}
				else
				{
					port = 8086;
				}

				channel = new TcpServerChannel(port);
				System.Runtime.Remoting.Channels.ChannelServices.RegisterChannel(channel,false);

                /*
				loggerImpl=new LoggerImpl();
				if(loggerImpl == null)
					eventLog.WriteEntry("Error: CreateInstance LoggerImpl",EventLogEntryType.Information);
				else
				{
					RemotingServices.Marshal(loggerImpl,"Logger");
					ILease Lease=(ILease) loggerImpl.GetLifetimeService();
					Lease.Renew(new TimeSpan(10000,0,0,0));

					loggerImpl.Start();
				}
                */
				eventLog.WriteEntry("Started",EventLogEntryType.Information);


			}
			catch(Exception e)
			{
				eventLog.WriteEntry(e.ToString(),EventLogEntryType.Error);
			}

		}

		public void Stop()
		{
			try
			{
                /*
				if(loggerImpl != null)
				{
					RemotingServices.Disconnect(loggerImpl);
					loggerImpl.Stop();
					loggerImpl=null;
				}
                */
				
				channel.StopListening(null);
				channel = null;
				ChannelServices.UnregisterChannel(channel);

				eventLog.WriteEntry("Stopped");

				GC.Collect();
			}
			catch(Exception e)
			{
				eventLog.WriteEntry(e.ToString(),EventLogEntryType.Error);
			}
		}
	}
}
