

CREATE FUNCTION [dbo].[udf_CheckUniquenessProduct]
(@_MasterRecordGuid uniqueidentifier, @SiteGuid uniqueidentifier, @ProductID nvarchar(30))
RETURNS BIT
AS
BEGIN
	DECLARE @Exists bit
	SET @Exists = 1


	-- dbo.tblProduct
	IF 0 < (SELECT COUNT(*) FROM tblProducts e
	LEFT JOIN map.tblEntityProductToSite em1 ON em1.ProductGuid = e._MasterRecordGuid
	RIGHT JOIN map.tblEntityProductToSite em2 ON em2.ProductGuid = @_MasterRecordGuid AND em2.SiteGuid = em1.SiteGuid
	WHERE e._MasterRecordGuid <> @_MasterRecordGuid
	AND ProductID = @ProductID)
		SET @Exists = 0

	RETURN @Exists
END

