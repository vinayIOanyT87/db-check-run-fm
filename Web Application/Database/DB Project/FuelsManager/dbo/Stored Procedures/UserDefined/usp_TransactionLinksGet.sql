
CREATE PROCEDURE [dbo].[usp_TransactionLinksGet]
(
	@TransID NVARCHAR(64) = NULL,
	@TransactionLineItemGuid UNIQUEIDENTIFIER = NULL
)
AS
BEGIN
	SET NOCOUNT ON

	-- The use of the NOLOCK hint in this procedure is questionable at best. It should be removed and any deadlocks that result should be fixed.
	SELECT t.*, 
		li.TransactionLineItemGuid, 
		li.Product, 
		li.DeliveryLocation, 
		li.GrossQuantity, 
		li.Tax1, 
		li.Tax2, 
		li.Tax3 
	FROM tblTransactionLinks tl WITH(NOLOCK) 
	LEFT JOIN tblTransactionLineItems li WITH(NOLOCK) ON tl.LinkedTransactionLineItemGuid = li.TransactionLineItemGuid 
	LEFT JOIN tblTransactions t WITH(NOLOCK) ON tl.LinkedTransID = t.TransID 
	WHERE tl.OriginalTransID = @TransID AND tl.TransactionLineItemGuid = @TransactionLineItemGuid

END 
