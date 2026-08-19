

CREATE FUNCTION [dbo].[udf_CheckUniquenessTransactionAlias]
(@_MasterRecordGuid uniqueidentifier, @SiteGuid uniqueidentifier, @AliasName nvarchar(32), @MeterCloseout bit)
RETURNS BIT
AS
BEGIN
	DECLARE @Exists bit
	SET @Exists = 1


	-- dbo.tblTransactionAlias
	IF 0 < (SELECT COUNT(*) FROM tblTransactionAliases e
	LEFT JOIN map.tblEntityTransactionAliasToSite em1 ON em1.TransactionAliasGuid = e._MasterRecordGuid
	RIGHT JOIN map.tblEntityTransactionAliasToSite em2 ON em2.TransactionAliasGuid = @_MasterRecordGuid AND em2.SiteGuid = em1.SiteGuid
	WHERE e._MasterRecordGuid <> @_MasterRecordGuid
	AND (AliasName = @AliasName
	OR (MeterCloseout = CAST(1 as bit)
	AND @MeterCloseout = CAST(1 as bit))))
		SET @Exists = 0

	RETURN @Exists
END

