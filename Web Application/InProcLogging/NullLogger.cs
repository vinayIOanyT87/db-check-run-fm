namespace InProcLogging
{
	/// <summary>
    /// A stubbed implementation of the ILogger interface
    /// </summary>
    public class NullLogger : ILogger
    {
        private LogSeverity severityLogged;

        /// <summary>
        /// Gets or sets the severity logged.
        /// </summary>
        /// <value>The severity logged.</value>
        LogSeverity ILogger.SeverityLogged
        {
            get
            {
                return this.severityLogged;
            }
            set
            {
                this.severityLogged = value;
            }
        }

        /// <summary>
        /// Logs the specified severity.
        /// </summary>
        /// <param name="severity">The severity.</param>
        /// <param name="message">The message.</param>
        public void Log(LogSeverity severity, string message)
        {
        }

        /// <summary>
        /// Logs the specified severity.
        /// </summary>
        /// <param name="severity">The severity.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The args.</param>
        public void Log(LogSeverity severity, string message, params object[] args)
        {            
        }

        public void Flush()
        {
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {            
        }
    }
}
