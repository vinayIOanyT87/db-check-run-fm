namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.ServiceModel;

	using FMBusinessObjects.ServiceRequests;

	[ServiceContract]
	public interface ITransactionNoteProcessor
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		Guid Process ( TransactionNoteSR sr );
	}
}
