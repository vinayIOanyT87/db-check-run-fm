namespace FMBusinessObjects.BusinessInterfaces
{
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;

	[ServiceContract]
	public interface ICopyTransactionsProcessor
	{
		[OperationContract]
		[TransactionFlow( TransactionFlowOption.Allowed )]
		SaveTransactionsResultDO Process( CopyTransactionsSR sr );
	}
}
