using FMCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMCore.Services
{
    /// <summary>
    /// DO NOT USE, this class just does nothing and is a placeholder for your own logger
    /// </summary>
    public class FakeLogger : IFMCustomLogger
    {
        public void Debug(string messageTemplate)
        {
        }

        public void Debug(string messageTemplate, params object[] propertyValues)
        {
        }

        public void Debug(Exception exception, string messageTemplate)
        {
        }

        public void Debug(Exception exception, string messageTemplate, params object[] propertyValues)
        {
        }

        public void Error(string messageTemplate)
        {
        }

        public void Error(string messageTemplate, params object[] propertyValues)
        {
        }

        public void Error(Exception exception, string messageTemplate)
        {
        }

        public void Error(Exception exception, string messageTemplate, params object[] propertyValues)
        {
        }

        public void Fatal(string messageTemplate)
        {
        }

        public void Fatal(string messageTemplate, params object[] propertyValues)
        {
        }

        public void Fatal(Exception exception, string messageTemplate)
        {
        }

        public void Fatal(Exception exception, string messageTemplate, params object[] propertyValues)
        {
        }

        public void Information(string messageTemplate)
        {
        }

        public void Information(string messageTemplate, params object[] propertyValues)
        {
        }

        public void Information(Exception exception, string messageTemplate)
        {
        }

        public void Information(Exception exception, string messageTemplate, params object[] propertyValues)
        {
        }

        public void Verbose(string messageTemplate)
        {
        }

        public void Verbose(string messageTemplate, params object[] propertyValues)
        {
        }

        public void Verbose(Exception exception, string messageTemplate, params object[] propertyValues)
        {
        }

        public void Verbose(Exception exception, string messageTemplate)
        {
        }

        public void Warning(string messageTemplate)
        {
        }

        public void Warning(string messageTemplate, params object[] propertyValues)
        {
        }

        public void Warning(Exception exception, string messageTemplate)
        {
        }

        public void Warning(Exception exception, string messageTemplate, params object[] propertyValues)
        {
        }
    }
}
