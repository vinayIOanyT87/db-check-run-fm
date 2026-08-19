
namespace FMBusinessObjects.BusinessInterfaces
{

	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	[ServiceContract]
	public interface IFMFatalErrorHandler
	{

		[OperationContract]
		bool ShutdownRequired(SecurityClass security, FMFatalErrorException fatalErrorEx);

		[OperationContract]
		bool ProcessFatalError(SecurityClass security, FMFatalErrorException fatalErrorEx);
	}

}
