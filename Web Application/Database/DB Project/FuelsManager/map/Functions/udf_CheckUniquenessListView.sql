

CREATE FUNCTION [map].[udf_CheckUniquenessLedgerView]
(@ListViewGuid uniqueidentifier, @SiteGuid uniqueidentifier)
RETURNS BIT
AS
BEGIN
	DECLARE @ID nvarchar(50)
			, @Exists bit
	SET @Exists = 1

	SET @ID = (SELECT ID FROM tblListViews e WHERE e.ListViewGuid = @ListViewGuid)
	IF 0 < (SELECT COUNT(*) FROM tblListViews e 
	RIGHT JOIN map.tblEntityLedgerViewToSite em ON em.SiteGuid = @SiteGuid AND em.ListViewGuid = e.ListViewGuid 
	WHERE e.ListViewGuid <> @ListViewGuid
	AND e.ID = @ID)
		SET @Exists = 0

	RETURN @Exists
END
