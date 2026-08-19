CREATE FUNCTION [dbo].[udf_IsDocumentNumberUnique]
(
	@TransactionGuid UNIQUEIDENTIFIER,
	@SiteGuid UNIQUEIDENTIFIER,
	@TransactionAliasGuid UNIQUEIDENTIFIER,
	@ReversalType NVARCHAR(2), 
	@ConjoinTransID NVARCHAR(64),
	@TransID NVARCHAR(64),
	@DocumentNumber NVARCHAR(30),
	@DeleteFlag BIT
)
RETURNS BIT
AS
BEGIN
	-- If the transaction is deleted or not an original, duplicate document numbers are permitted. If the document number is not provided, there's no reason to check further.
	IF (@DeleteFlag = 1 OR ISNULL(@DocumentNumber, '') = '' OR (ISNULL(@ReversalType, 'O') <> 'O' AND ISNULL(@ReversalType, '') <> ''))
	BEGIN
		RETURN 1
	END

	-- Make sure that there are no other non-deleted transactions in the same site with the same transaction alias that have the same document number.
	-- Duplication of document numbers is still permitted if the transaction is not an original or if the transaction is conjoined to the transaction with the same document number.
	IF EXISTS (
		SELECT *
		FROM tblTransactions t 
		WHERE t.SiteGuid = @SiteGuid
		AND t.TransactionAliasGuid = @TransactionAliasGuid
		AND t.TransactionGuid <> @TransactionGuid
		AND ISNULL(t.DeleteFlag, 0) = 0
		AND (ISNULL(t.ReversalType, 'O') = 'O' OR ISNULL(t.ReversalType, '') = '') 
		AND t.TransID <> ISNULL(@ConjoinTransID, '')
		AND @TransID <> ISNULL(t.ConjoinTransID, '')
		AND @DocumentNumber = t.DocumentNumber
		)
	BEGIN
		RETURN 0
	END

	RETURN 1
END