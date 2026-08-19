using FMCore.Interfaces;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Json;
using System;
using System.IO;
using Serilog.Exceptions;

namespace FMWebAPI.Services
{
    public class FMWebAPISerilogEventlogLogger : IFMCustomLogger
    {
        private readonly Serilog.ILogger _log;
        private readonly string _settingsFilePath;
        private const int NumberOfStackTraceFramesToSkip = 6;
        public FMWebAPISerilogEventlogLogger()
        {
            _settingsFilePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _settingsFilePath = Path.Combine(_settingsFilePath, "FuelsManager");
            _settingsFilePath = Path.Combine(_settingsFilePath, "AviationAccounting.json");
            var logConfig = new LoggerConfiguration()
                .MinimumLevel.Error()
                .WriteTo.EventLog(new JsonFormatter(), "FMWebAPI", "Application", manageEventSource: false)
                //.Enrich.With<StackTraceEnricher>()
                .Enrich.With<ClassAndMethodEnricher>()
                .Enrich.WithExceptionDetails();
            _log = logConfig.CreateLogger();
        }
        public void Debug(string messageTemplate)
        {
            _log.Debug(messageTemplate);
        }

        public void Debug(string messageTemplate, params object[] propertyValues)
        {
            _log.Debug(messageTemplate, propertyValues);
        }

        public void Debug(Exception exception, string messageTemplate)
        {
            _log.Debug(exception, messageTemplate);
        }

        public void Debug(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            _log.Debug(exception, messageTemplate, propertyValues);
        }

        public void Error(string messageTemplate)
        {
            _log.Error(messageTemplate);
        }

        public void Error(string messageTemplate, params object[] propertyValues)
        {
            _log.Error(messageTemplate, propertyValues);
        }

        public void Error(Exception exception, string messageTemplate)
        {
            _log.Error(exception, messageTemplate);
        }

        public void Error(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            _log.Error(exception, messageTemplate, propertyValues);
        }

        public void Fatal(string messageTemplate)
        {
            _log.Fatal(messageTemplate);
        }

        public void Fatal(string messageTemplate, params object[] propertyValues)
        {
            _log.Fatal(messageTemplate, propertyValues);
        }

        public void Fatal(Exception exception, string messageTemplate)
        {
            _log.Fatal(exception, messageTemplate);
        }

        public void Fatal(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            _log.Fatal(exception, messageTemplate, propertyValues);
        }

        public void Information(string messageTemplate)
        {
            _log.Information(messageTemplate);
        }

        public void Information(string messageTemplate, params object[] propertyValues)
        {
            _log.Information(messageTemplate, propertyValues);
        }

        public void Information(Exception exception, string messageTemplate)
        {
            _log.Information(exception, messageTemplate);
        }

        public void Information(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            _log.Information(exception, messageTemplate, propertyValues);
        }

        public void Verbose(string messageTemplate)
        {
            _log.Verbose(messageTemplate);
        }

        public void Verbose(string messageTemplate, params object[] propertyValues)
        {
            _log.Verbose(messageTemplate, propertyValues);
        }

        public void Verbose(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            _log.Verbose(exception, messageTemplate, propertyValues);
        }

        public void Verbose(Exception exception, string messageTemplate)
        {
            _log.Verbose(exception, messageTemplate);
        }

        public void Warning(string messageTemplate)
        {
            _log.Warning(messageTemplate);
        }

        public void Warning(string messageTemplate, params object[] propertyValues)
        {
            _log.Warning(messageTemplate, propertyValues);
        }

        public void Warning(Exception exception, string messageTemplate)
        {
            _log.Warning(exception, messageTemplate);
        }

        public void Warning(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            _log.Warning(exception, messageTemplate, propertyValues);
        }

        class StackTraceEnricher : ILogEventEnricher
        {
            public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
            {
                var stackTraceForCallingEvent = new System.Diagnostics.StackTrace(NumberOfStackTraceFramesToSkip, true);
                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("Stack", stackTraceForCallingEvent));
            }
        }
        class ClassAndMethodEnricher : ILogEventEnricher
        {
            public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
            {
                var stackFrameForCallingEvent = new System.Diagnostics.StackFrame(NumberOfStackTraceFramesToSkip, true);
                var method = stackFrameForCallingEvent.GetMethod();
                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("Class", method.DeclaringType.FullName));
                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("Method", method.Name));
            }
        }
    }
}