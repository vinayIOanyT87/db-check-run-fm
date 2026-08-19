

CREATE FUNCTION [map].[udf_CheckUniquenessProduct]
(@_MasterRecordGuid uniqueidentifier, @SiteGuid uniqueidentifier)
RETURNS BIT
AS
BEGIN
	DECLARE @ProductID nvarchar(30)
			, @Exists bit
	SET @Exists = 1

	SET @ProductID = (SELECT ProductID FROM tblProducts e WHERE e.ProductGuid = @_MasterRecordGuid)
	IF 0 < (SELECT COUNT(*) FROM tblProducts e 
	RIGHT JOIN map.tblEntityProductToSite em ON em.SiteGuid = @SiteGuid AND em.ProductGuid = e._MasterRecordGuid 
	WHERE e._MasterRecordGuid <> @_MasterRecordGuid
	AND e.ProductID = @ProductID)
		SET @Exists = 0

	RETURN @Exists
END