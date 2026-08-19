

CREATE FUNCTION [map].[udf_CheckUniquenessTransactionAlias]
(@_MasterRecordGuid uniqueidentifier, @SiteGuid uniqueidentifier)
RETURNS BIT
AS
BEGIN
	DECLARE @AliasName nvarchar(32)
			, @MeterCloseout bit
			, @Exists bit
	SET @Exists = 1

	SET @AliasName = (SELECT AliasName FROM tblTransactionAliases e WHERE e.TransactionAliasGuid = @_MasterRecordGuid)
	SET @MeterCloseout = (SELECT MeterCloseout FROM tblTransactionAliases e WHERE e.TransactionAliasGuid = @_MasterRecordGuid)
	IF 0 < (SELECT COUNT(*) FROM tblTransactionAliases e 
	RIGHT JOIN map.tblEntityTransactionAliasToSite em ON em.SiteGuid = @SiteGuid AND em.TransactionAliasGuid = e._MasterRecordGuid 
	WHERE e._MasterRecordGuid <> @_MasterRecordGuid
	AND (AliasName = @AliasName
	OR (MeterCloseout = CAST(1 as bit)
	AND @MeterCloseout = CAST(1 as bit))))
		SET @Exists = 0

	RETURN @Exists
END
