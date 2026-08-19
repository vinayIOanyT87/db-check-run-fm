namespace FMBusinessObjects.BusinessInterfaces
{
    using System.ServiceModel;

    using FMBusinessObjects.ServiceRequests;

    [ServiceContract]
	public interface IPidxBolProcessor
	{
		[OperationContract]
		void Process ( TransactionBolPidxSR sr );
	}
}
