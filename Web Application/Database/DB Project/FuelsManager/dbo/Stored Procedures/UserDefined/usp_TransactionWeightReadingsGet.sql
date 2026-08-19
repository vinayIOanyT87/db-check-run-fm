
CREATE PROCEDURE [dbo].[usp_TransactionWeightReadingsGet]
(
	@TransactionGuid UNIQUEIDENTIFIER
)
AS
BEGIN
	SET NOCOUNT ON

	-- The use of the NOLOCK hint in this procedure is questionable at best. It should be removed and any deadlocks that result should be fixed.
	SELECT * 
	FROM tblTransactionWeightReadings WITH(NOLOCK) 
	WHERE TransactionGuid = @TransactionGuid AND HistoricalFlag = 0

END 

