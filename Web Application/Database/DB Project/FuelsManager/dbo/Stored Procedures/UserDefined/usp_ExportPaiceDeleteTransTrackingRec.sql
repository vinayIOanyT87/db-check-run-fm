CREATE PROCEDURE [dbo].[usp_ExportPaiceDeleteTransTrackingRec]
(
	@TransactionGuid UNIQUEIDENTIFIER
)
AS
BEGIN
	UPDATE tbltransactions
	SET
		DATE01 = null
		, NUMBER01 = null
	WHERE TransactionGuid = @TransactionGuid 


END
