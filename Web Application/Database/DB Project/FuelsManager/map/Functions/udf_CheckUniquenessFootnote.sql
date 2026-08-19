
CREATE FUNCTION [map].[udf_CheckUniquenessFootNote]
(@ApplicationStringGuid uniqueidentifier, @SiteGuid uniqueidentifier)
RETURNS BIT
AS
BEGIN
	DECLARE @ID nvarchar(250)
			, @Exists bit
	SET @Exists = 1

	SET @ID = (SELECT ID FROM [dbo].[tblApplicationString] e WHERE e.ApplicationStringGuid = @ApplicationStringGuid AND e.LookupApplicationStringTypeIndex = 12)
	IF 0 < (SELECT COUNT(*) FROM [dbo].[tblApplicationString] e 
	RIGHT JOIN map.tblEntityFootNoteToSite em ON em.SiteGuid = @SiteGuid AND em.ApplicationStringGuid = e.ApplicationStringGuid 
	WHERE e.ApplicationStringGuid <> @ApplicationStringGuid
	AND e.ID = @ID AND e.LookupApplicationStringTypeIndex = 12)
		SET @Exists = 0

	RETURN @Exists
END
