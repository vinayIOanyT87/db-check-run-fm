
CREATE PROCEDURE [dbo].[usp_TransactionLinksGetList]
(
	@TransactionGuids dbo.TransactionGuidListType READONLY
)
AS
BEGIN

	-- This functionality is here to support the logic in the LineItemAssociatedTxDBI class. 
	-- The previous associations must be known to detect which were deleted when a transaction is modified
	SELECT 
		tblTransactionLineItems.TransactionGuid,
		tl.LinkedTransactionLineItemGuid,
		tl.TransactionLineItemGuid,
		tl.LinkedTransID
	FROM tblTransactionLinks tl 
	INNER JOIN tblTransactionLineItems ON tl.TransactionLineItemGuid = tblTransactionLineItems.TransactionLineItemGuid 
	INNER JOIN @TransactionGuids transactionGuids ON tblTransactionLineItems.TransactionGuid = transactionGuids.TransactionGuid

END