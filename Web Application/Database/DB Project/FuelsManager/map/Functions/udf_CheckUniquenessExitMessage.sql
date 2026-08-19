

CREATE FUNCTION [map].[udf_CheckUniquenessExitMessage]
(@ApplicationStringGuid uniqueidentifier, @SiteGuid uniqueidentifier)
RETURNS BIT
AS
BEGIN
	DECLARE @ID nvarchar(250)
			, @Exists bit
	SET @Exists = 1

	SET @ID = (SELECT ID FROM [dbo].[tblApplicationString] e WHERE e.ApplicationStringGuid = @ApplicationStringGuid AND e.LookupApplicationStringTypeIndex = 10)
	IF 0 < (SELECT COUNT(*) FROM [dbo].[tblApplicationString] e 
	RIGHT JOIN map.tblEntityExitMessageToSite em ON em.SiteGuid = @SiteGuid AND em.ApplicationStringGuid = e.ApplicationStringGuid 
	WHERE e.ApplicationStringGuid <> @ApplicationStringGuid
	AND e.ID = @ID  AND e.LookupApplicationStringTypeIndex = 10)
		SET @Exists = 0

	RETURN @Exists
END
