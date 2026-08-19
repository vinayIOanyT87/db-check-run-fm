using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Globalization;
namespace FuelsManagerEnterpriseExportWndSvs
{
	public partial class Service1 : ServiceBase
	{
		private ManualResetEvent KillEvent;
		private Thread DependencyThread;

		public Service1()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			try
			{
				string msg = String.Format(CultureInfo.CurrentCulture, "Start FuelsManager Enterprise Export Window Service at: {0}",
											DateTimeOffset.Now.ToString("u", CultureInfo.CurrentCulture));
				EventLog.WriteEntry(msg, EventLogEntryType.Information);
				ThreadStart DependencyStart = new ThreadStart(CloseoutInventoryDependency);
				DependencyThread = new Thread(DependencyStart);
				DependencyThread.Start();
			}
			catch (Exception e)
			{
				EventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
			}
		}

		protected override void OnStop()
		{
			if (KillEvent != null)
			{
				KillEvent.Set();
				KillEvent = null;
			}
			if (DependencyThread != null)
			{
				DependencyThread.Join();
				//DependencyThread.Abort(); // added for testing 
				DependencyThread = null;
			}
			string msg = String.Format(System.Globalization.CultureInfo.CurrentCulture, "Stop FuelsManager Enterprise Export Window Service at: {0}",
										DateTimeOffset.Now.ToString("u", CultureInfo.CurrentCulture));
			EventLog.WriteEntry(msg, EventLogEntryType.Information);
		}

		public void CloseoutInventoryDependency()
		{
			string strFunctionName = "CloseoutInventoryDependency()";
			bool bGotPermissions = false;
			CloseoutDependencyClass dep = new CloseoutDependencyClass();
			try
			{
				dep.InitializeCloseoutDependencyClass(EventLog);
				bGotPermissions = dep.EnoughPermission();
				if (bGotPermissions == false)
				{
					string msg = String.Format("Exception starting in object: {0}, function: {1}, Message: EnoughPermissions() returned false. Exiting.",
												this.ToString(), strFunctionName);
					EventLog.WriteEntry(msg, EventLogEntryType.Warning);

				}
			}
			catch (Exception ex)
			{
				string msg = String.Format("Exception starting in object: {0}, function: {1}, Exception: {2}",
											this.ToString(), strFunctionName, ex.Message);
				EventLog.WriteEntry(msg, EventLogEntryType.Warning);
			}

			KillEvent = new ManualResetEvent(false);
			WaitHandle[] Events = { KillEvent };
			int nWaitResults;
			TimeSpan thirtySeconds = new TimeSpan(0, 0, 30);

			while (0 != (nWaitResults = WaitHandle.WaitAny(Events, thirtySeconds, true)))
			{
				if (dep.DependsRun() == true)
				{
					break;
				}
			}

		}
	}
}
