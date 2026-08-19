namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Data.SqlClient;
	using System.Diagnostics;
	using System.Globalization;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	using FMBusinessServices.DataAccessLayer;

	/// <summary>
	/// Class used to handle processing of FMFatalErrorException types.  Method ShutdownRequired() uses
	/// the ShutdownIfMaximumErrorCountExceededForLogs config value together with the exception error count
	/// to determine whether or not to shut down FuelsManager.  Method ProcessFatalError() determines whether
	/// a shutdown is required and if so writes a message to the event log and shuts down SQL Server.
	/// </summary>
	[SecuritySafeCritical]
	public class FMFatalErrorHandler : IFMFatalErrorHandler
	{
		/// <summary>
		/// Determines whether or not to shut down FuelsManager based on the exception error count
		/// and the ShutdownIfMaximumErrorCountExceededForLogs application configuration setting.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="fatalErrorEx">The FMFatalErrorException exception</param>
		/// <returns>true if shut down of FuelsManager is required</returns>
		public bool ShutdownRequired(SecurityClass security, FMFatalErrorException fatalErrorEx)
		{
			bool shutdownFuelsManager = true;
			// An error count of -1 indicates that previous processing has determined that a shutdown is required
			if (fatalErrorEx.ErrorCount > -1)
			{
				bool shutdownIfExceeded = false;
				var c = new ConfigurationSettingsClass();

				ConfigurationSettingDOClass shutdownIfExceededSetting = c.GetByKey(security, "ShutdownIfMaximumErrorCountExceededForLogs");
				if (shutdownIfExceededSetting != null && !string.IsNullOrEmpty(shutdownIfExceededSetting.SettingValue))
				{
					bool.TryParse(shutdownIfExceededSetting.SettingValue, out shutdownIfExceeded);
				}

				if (shutdownIfExceeded)
				{
					int maxErrorCount = -1;
					ConfigurationSettingDOClass maxErrorCountValueSetting = c.GetByKey(security, "MaximumConsecutiveErrorCountForLogs");
					if (maxErrorCountValueSetting != null && !string.IsNullOrEmpty(maxErrorCountValueSetting.SettingValue))
					{
						Int32.TryParse(maxErrorCountValueSetting.SettingValue, out maxErrorCount);
					}

					shutdownFuelsManager = fatalErrorEx.ErrorCount > maxErrorCount;
				}
				else
				{
					shutdownFuelsManager = false;
				}
			}

			return shutdownFuelsManager;
		}


		/// <summary>
		/// Determines whether or not to shut down FuelsManager.  If a shutdown is required then an
		/// entry is written to the event log and the FuelsManager SQL Server instance is shut down.
		/// </summary>
		/// <param name="fatalErrorEx">The FMFatalErrorException exception</param>
		/// <param name="security">The SecurityClass object</param>
		/// <returns>true if FuelsManager has been shut down</returns>
		public bool ProcessFatalError(SecurityClass security, FMFatalErrorException fatalErrorEx)
		{
			bool shutdownFuelsManager = ShutdownRequired(security, fatalErrorEx);
			if (shutdownFuelsManager)
			{
				try
				{
					var fmFatalExpectionHandlerClass = new FMFatalErrorHandlerClass();

					var eventLog = new EventLog("Application", ".", "FuelsManager");
					eventLog.WriteEntry(fatalErrorEx.Message + " " + FMFatalErrorHandlerClass.ShutdownMessage, EventLogEntryType.Error);

					var consolidatedDa = new ConsolidatedDAClass();

					using (var cmd = new SqlCommand())
					{
						fmFatalExpectionHandlerClass.ShutDownSQL(cmd);
						consolidatedDa.ExecuteQuery(security, cmd);
					}
				}
				catch
				{
				}
			}

			return shutdownFuelsManager;
		}
	}
}
