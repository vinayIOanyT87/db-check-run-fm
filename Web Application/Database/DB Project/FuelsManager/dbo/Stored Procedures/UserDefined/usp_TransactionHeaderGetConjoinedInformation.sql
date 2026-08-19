CREATE PROCEDURE [dbo].[usp_TransactionHeaderGetConjoinedInformation]
(
	@TransactionGuids dbo.GuidListType READONLY
)
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT
		tblTransactions.TransactionGuid,
		tblTransactions.SubType,	
		ConjoinedTransaction.TransactionGuid AS ConjoinTransactionGuid,
		ConjoinedTransaction.TransID AS ConjoinTransID,			
		ConjoinedTransactionUserData.TransactionUserDataGuid AS ConjoinTransactionUserDataGuid,
		ConjoinedTransactionNotes.TransactionNoteGuid AS ConjoinTransactionNoteGuid,
		ConjoinedTransactionSignature.TransactionSignatureGuid AS ConjoinTransactionSignatureGuid
	FROM tblTransactions 
	INNER JOIN @TransactionGuids transactionGuids ON transactionGuids.[Guid] = tblTransactions.TransactionGuid
	LEFT JOIN tblTransactions ConjoinedTransaction ON tblTransactions.ConjoinTransID = ConjoinedTransaction.TransID
	LEFT JOIN tblTransactionUserData ConjoinedTransactionUserData ON ConjoinedTransaction.TransactionGuid = ConjoinedTransactionUserData.TransactionGuid
	LEFT JOIN tblTransactionNotes ConjoinedTransactionNotes ON ConjoinedTransaction.TransactionGuid = ConjoinedTransactionNotes.TransactionGuid
	LEFT JOIN tblTransactionSignature ConjoinedTransactionSignature ON ConjoinedTransaction.TransactionGuid = ConjoinedTransactionSignature.TransactionGuid

END 

