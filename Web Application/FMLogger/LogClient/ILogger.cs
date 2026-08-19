using System;

namespace LogClient
{
	/// <summary>
	/// Summary description for ILogger.
	/// </summary>
	[System.Serializable]
	public enum LogLevel
	{
		DEBUG,
		PERFORM,
		INFO,
		WARN,
		ERROR,
		CRITICAL
	}
	public interface ILogger
	{
		void Log(LogLevel level, string message);

		void Debug(string message);
		void Perform(string message);
		void Info(string message);
		void Warn(string message);
		void Error(string message);
		void Critical(string message);
	}
}
