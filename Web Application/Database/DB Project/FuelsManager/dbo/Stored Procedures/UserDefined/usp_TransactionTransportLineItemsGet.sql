
CREATE PROCEDURE [dbo].[usp_TransactionTransportLineItemsGet]
(
	@TransactionGuid UNIQUEIDENTIFIER
)
AS
BEGIN
	SET NOCOUNT ON

	-- The use of the NOLOCK hint in this procedure is questionable at best. It should be removed and any deadlocks that result should be fixed.
	SELECT * 
	FROM tblTransactionTransportLineItems WITH(NOLOCK) 
	WHERE TransactionGuid = @TransactionGuid

END 

