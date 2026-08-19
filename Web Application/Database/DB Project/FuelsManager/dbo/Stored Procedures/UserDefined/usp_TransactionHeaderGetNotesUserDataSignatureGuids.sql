CREATE PROCEDURE [dbo].[usp_TransactionHeaderGetNotesUserDataSignatureGuids]
(
	@TransactionGuids dbo.GuidListType READONLY
)
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT	
		tblTransactions.TransactionGuid,
		tblTransactionUserData.TransactionUserDataGuid,
		tblTransactionNotes.TransactionNoteGuid,
		tblTransactionSignature.TransactionSignatureGuid
	FROM tblTransactions 
	INNER JOIN @TransactionGuids transactionGuids ON transactionGuids.[Guid] = tblTransactions.TransactionGuid
	LEFT JOIN tblTransactionUserData ON tblTransactions.TransactionGuid = tblTransactionUserData.TransactionGuid
	LEFT JOIN tblTransactionNotes ON tblTransactions.TransactionGuid = tblTransactionNotes.TransactionGuid
	LEFT JOIN tblTransactionSignature ON tblTransactions.TransactionGuid = tblTransactionSignature.TransactionGuid

END 

