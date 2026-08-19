// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILogger.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for ILogger.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataImportExportWizard.InternalClasses.LogClient
{
	using System;

	/// <summary>
	/// Specifies the logging level for messages.
	/// </summary>
	[Serializable]
	public enum LogLevel
	{
		/// <summary>
		/// Debug level messages
		/// </summary>
		DEBUG,

		/// <summary>
		/// Performance level messages
		/// </summary>
		PERFORM,

		/// <summary>
		/// For informational messages
		/// </summary>
		INFO,

		/// <summary>
		/// For warning messages.
		/// </summary>
		WARN,

		/// <summary>
		/// For error messages
		/// </summary>
		ERROR,

		/// <summary>
		/// For critical error messages
		/// </summary>
		CRITICAL
	}

	/// <summary>
	/// Interface for the FuelsManager logger.
	/// </summary>
	public interface ILogger
	{
		#region Public Methods and Operators

		/// <summary>
		/// Logs a critical message.
		/// </summary>
		/// <param name="message">The message.</param>
		void Critical(string message);

		/// <summary>
		/// Logs a debug message.
		/// </summary>
		/// <param name="message">The message.</param>
		void Debug(string message);

		/// <summary>
		/// Logs an error message.
		/// </summary>
		/// <param name="message">The message.</param>
		void Error(string message);

		/// <summary>
		/// Logs an info message.
		/// </summary>
		/// <param name="message">The message.</param>
		void Info(string message);

		/// <summary>
		/// Logs a message of the specified level.
		/// </summary>
		/// <param name="level">The level.</param>
		/// <param name="message">The message.</param>
		void Log(LogLevel level, string message);

		/// <summary>
		/// Logs a performance message.
		/// </summary>
		/// <param name="message">The message.</param>
		void Perform(string message);

		/// <summary>
		/// Logs a warning message.
		/// </summary>
		/// <param name="message">The message.</param>
		void Warn(string message);

        /// <summary>
        /// Dispose of the logging system since we're the only one using it.
        /// </summary>
	    void Dispose();

	    #endregion
	}
}