
namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.ServiceRequests;

	[ServiceContract]
	public interface ITransactionImportProcessor
	{
		[OperationContract]
		[FaultContract(typeof(SaveTransactionsException))]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Process(TransactionImportSR transactionSR);

		/// <summary>
		/// Take a list of transactions and for each transaction in the list retrieve the primary keys associated with all child items of the transaction (e.g. line item, line item user data),
		/// if there is a matching existing record in our system.
        /// Also, handle Conjoined transaction information as it is vital for the import to function properly.
		/// Note that primary keys for weight reading records are not retrieved because we always insert weight readings. Existing records become historical. 
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		/// <param name="transactions">A list of transactions to get the primary keys for</param>
		/// <returns>A list of transactions updated with the primary keys</returns>
		[OperationContract]
		List<TransactionDO> PopulateKeyTransactionGuids(SecurityClass security, List<TransactionDO> transactions);

		/// <summary>
		/// Take a list of transactions and for each transaction in the list retrieve the primary keys associated with all records in the transaction, 
		/// and then save the transactions.
        /// Also, handle Conjoined transaction information as it is vital for the import to function properly.
		/// Note that primary keys for weight reading records are not retrieved because we always insert weight readings. Existing records become historical. 
		/// </summary>
		/// <param name="saveRequest">Information needed to save transactions, including the list of transactions themselves</param>
		/// <returns>The results from saving the transactions</returns>
		[OperationContract]
		[FaultContract(typeof(SaveTransactionsException))]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		SaveTransactionsResultDO PopulateKeyTransactionGuidsAndSave(SaveTransactionsSR saveRequest);

		/// <summary>
		/// Retrieve only the corresponding TransactionGuid for the transIDs provided
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="transIds">A list of transIds to retrieve the corresponding guids for</param>
		/// <returns>A dictionary mapping the transIds provided to the TransactionGuids</returns>
		[OperationContract]
		Dictionary<string, Guid> GetTransactionGuidsForTransIDs(SecurityClass security, List<string> transIds);
	}
}