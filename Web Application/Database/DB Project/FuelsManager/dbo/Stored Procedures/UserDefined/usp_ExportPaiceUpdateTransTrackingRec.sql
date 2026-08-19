CREATE PROCEDURE [dbo].[usp_ExportPaiceUpdateTransTrackingRec]
(
	@TransactionGuid UNIQUEIDENTIFIER
)
AS
BEGIN

	UPDATE tbltransactions
	SET Date01  = sysdatetimeoffset()
	WHERE TransactionGuid = @TransactionGuid 

END
