

CREATE FUNCTION [map].[udf_CheckUniquenessLedgerAggregateColumn]
(@LedgerAggregateColumnGuid uniqueidentifier, @SiteGuid uniqueidentifier)
RETURNS BIT
AS
BEGIN
	DECLARE @ID nvarchar(50)
			, @Exists bit
	SET @Exists = 1

	SET @ID = (SELECT ID FROM tblLedgerAggregateColumns e WHERE e.LedgerAggregateColumnGuid = @LedgerAggregateColumnGuid)
	IF 0 < (SELECT COUNT(*) FROM tblLedgerAggregateColumns e 
	RIGHT JOIN map.tblEntityLedgerAggregateColumnToSite em ON em.SiteGuid = @SiteGuid AND em.LedgerAggregateColumnGuid = e.LedgerAggregateColumnGuid 
	WHERE e.LedgerAggregateColumnGuid <> @LedgerAggregateColumnGuid
	AND e.ID = @ID)
		SET @Exists = 0

	RETURN @Exists
END
