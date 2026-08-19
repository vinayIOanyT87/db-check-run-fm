CREATE PROCEDURE [dbo].[usp_TransactionLineItemsGetConjoinedInformation]
(
	@TransactionGuids dbo.GuidListType READONLY
)
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT
		tblTransactionLineItems.TransactionGuid,
		tblTransactionLineItems.TransactionLineItemGuid,
		tblTransactionLineItems.SequenceID,
		tblTransactionLineItemUserData.TransactionLineItemUserDataGuid
	FROM tblTransactionLineItems 
	INNER JOIN @TransactionGuids transactionGuids ON transactionGuids.[Guid] = tblTransactionLineItems.TransactionGuid
	LEFT JOIN tblTransactionLineItemUserData ON tblTransactionLineItems.TransactionLineItemGuid = tblTransactionLineItemUserData.TransactionLineItemGuid

END 

