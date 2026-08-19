namespace InProcLogging
{
	using System.Diagnostics;

	public class DebugLogger : ILogger
    {
        private LogSeverity severityLogged;

        public LogSeverity SeverityLogged
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

        public void Log(LogSeverity severity, string message)
        {
            if (severity >= this.severityLogged)
            {
                Debug.WriteLine(message, severity.ToString());
            }
        }

        public void Log(LogSeverity severity, string message, params object[] args)
        {
            if (severity >= this.severityLogged)
            {
                Debug.WriteLine(string.Format(message, args), severity.ToString());
            }
        }

        public void Flush()
        {
            Debug.Flush();
        }

        public void Dispose()
        {
           
        }
    }
}
