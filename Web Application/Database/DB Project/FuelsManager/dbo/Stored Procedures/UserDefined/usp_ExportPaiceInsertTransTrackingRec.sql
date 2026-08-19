CREATE PROCEDURE [dbo].[usp_ExportPaiceInsertTransTrackingRec]
(
	@TransactionGuid UNIQUEIDENTIFIER,
	@TransType nvarchar(2)
)
AS
BEGIN

	UPDATE tblTransactions 
	SET 
		Date01 = SYSDATETIMEOFFSET()
		, Number01 = CAST(@TransType AS float)
	WHERE TransactionGuid = @TransactionGuid

END
