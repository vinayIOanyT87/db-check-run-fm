

CREATE FUNCTION [dbo].[udf_CheckUniquenessLedgerAggregateColumn]
(@LedgerAggregateColumnGuid uniqueidentifier, @SiteGuid uniqueidentifier, @ID nvarchar(30))
RETURNS BIT
AS
BEGIN
	DECLARE @Exists bit
	SET @Exists = 1


	-- dbo.tblLedgerAggregateColumn
	IF 0 < (SELECT COUNT(*) FROM tblLedgerAggregateColumns e
	LEFT JOIN map.tblEntityLedgerAggregateColumnToSite em1 ON em1.LedgerAggregateColumnGuid = e.LedgerAggregateColumnGuid
	RIGHT JOIN map.tblEntityLedgerAggregateColumnToSite em2 ON em2.LedgerAggregateColumnGuid = @LedgerAggregateColumnGuid AND em2.SiteGuid = em1.SiteGuid
	WHERE e.LedgerAggregateColumnGuid <> @LedgerAggregateColumnGuid
	AND ID = @ID)
		SET @Exists = 0

	RETURN @Exists
END
