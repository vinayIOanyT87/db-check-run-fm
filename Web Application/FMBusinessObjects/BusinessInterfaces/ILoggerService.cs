namespace FMBusinessObjects.BusinessInterfaces
{
	using System.ServiceModel;

	using FMBusinessObjects.LogClient;

	[ServiceContract]
	public interface ILoggerService
	{
		#region Public Methods and Operators

		[OperationContract]
		int CreateLog(string appName);

		[OperationContract]
		void Log(string appName, LogLevel level, string message);

		[OperationContract]
		void Start();

		[OperationContract]
		void Stop();

		#endregion
	}
}