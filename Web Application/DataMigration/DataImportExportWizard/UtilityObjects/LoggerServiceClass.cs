// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggerServiceClass.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the LoggerServiceClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataImportExportWizard.UtilityObjects
{
	using DataImportExportWizard.Interfaces;
	using DataImportExportWizard.InternalClasses.LogClient;

    using DataImportExportWizard.InternalClasses.Logger;

	/// <summary>
	/// Logger service implementation class
	/// </summary>
	public class LoggerServiceClass : ILoggerService
	{
		#region Public Methods and Operators

		/// <summary>
		/// Creates the log.
		/// </summary>
		/// <param name="appName">Name of the app.</param>
		/// <returns>Returns result from logger implementation class.</returns>
		public int CreateLog(string appName)
		{
			var loggerImpl = new LoggerImpl();
			return loggerImpl.CreateLog(appName);
		}

		/// <summary>
		/// Logs the specified app name.
		/// </summary>
		/// <param name="appName">Name of the app.</param>
		/// <param name="level">The level.</param>
		/// <param name="message">The message.</param>
		public void Log(string appName, LogLevel level, string message)
		{
			var loggerImpl = new LoggerImpl();
			loggerImpl.Log(appName, level, message);
		}

		/// <summary>
		/// Starts this instance.
		/// </summary>
		public void Start()
		{
			var loggerImpl = new LoggerImpl();
			loggerImpl.Start();
		}

		/// <summary>
		/// Stops this instance.
		/// </summary>
		public void Stop()
		{
			var loggerImpl = new LoggerImpl();
			loggerImpl.Stop();
		}

		#endregion
	}
}