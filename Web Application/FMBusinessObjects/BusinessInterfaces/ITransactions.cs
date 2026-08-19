namespace FMBusinessObjects.BusinessInterfaces
{
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface ITransactions
	{
		[OperationContract]
		void SaveFastEntryTransaction(SecurityClass security, TransactionDO transaction);
	}
}
