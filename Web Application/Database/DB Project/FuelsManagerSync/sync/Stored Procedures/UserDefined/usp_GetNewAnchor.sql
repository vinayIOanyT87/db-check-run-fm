CREATE PROCEDURE [sync].[usp_GetNewAnchor] (
	@sync_new_received_anchor bigint output
) AS
BEGIN
	SET @sync_new_received_anchor = convert(bigint, min_active_rowversion())-1
END