namespace DataImportExportWizard.Interfaces
{
    using DataImportExportWizard.InternalClasses.LogClient;

    public interface ILoggerService
    {
        #region Public Methods and Operators

        int CreateLog(string appName);

        void Log(string appName, LogLevel level, string message);

        void Start();

        void Stop();

        #endregion
    }
}