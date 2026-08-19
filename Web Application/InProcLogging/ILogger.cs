namespace InProcLogging
{
	using System;

	/// <summary>
    /// Represent severities of LogEntries.
    /// </summary>
    public enum LogSeverity
    {
        /// <summary>
        /// Represents a severity level of "Debug"
        /// </summary>
        Debug = 1,
        /// <summary>
        /// Represents a severity level of "Info"
        /// </summary>
        Info = 2,
        /// <summary>
        /// Represents a severity level of "Status"
        /// </summary>
        Status = 3,
        /// <summary>
        /// Represents a severity level of "Warning"
        /// </summary>
        Warning = 4,
        /// <summary>
        /// Represents a severity level of "System_Error"
        /// </summary>
        Error = 5,
        /// <summary>
        /// Represents a severity level of "Critical"
        /// </summary>
        Critical = 6,
        /// <summary>
        /// Represents a severity level of "Fatal"
        /// </summary>
        Fatal = 7,
    }

    public interface ILogger: IDisposable
    {
        /// <summary>
        /// Gets or sets the severity logged.
        /// </summary>
        /// <value>The severity logged.</value>
        LogSeverity SeverityLogged
        {
            get;
            set;
        }

        /// <summary>
        /// Logs the specified severity.
        /// </summary>
        /// <param name="severity">The severity.</param>
        /// <param name="message">The message.</param>
        void Log(LogSeverity severity, string message);

        /// <summary>
        /// Logs the specified severity.
        /// </summary>
        /// <param name="severity">The severity.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The args.</param>
        void Log(LogSeverity severity, string message, params object[] args);

        void Flush();
    }
}