CREATE PROCEDURE [dbo].[usp_ExportPaiceGetTransType]
(
	@TransactionGuid UNIQUEIDENTIFIER
)
AS
BEGIN

	DECLARE @TransType nvarchar(2) = null;

	SELECT @TransType = NUMBER01 
	FROM tbltransactions
	WHERE TransactionGuid = @TransactionGuid 

	IF (@TransType IS NOT NULL) AND (len(@TransType) > 0)
		SELECT @TransType
	ELSE
		SELECT '**'

END
