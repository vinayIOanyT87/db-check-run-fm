CREATE FUNCTION [dbo].[udf_GetAssignedDrawingListPerSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblDrawingList TABLE
(
	[DrawingGuid] [uniqueidentifier]
)
AS
BEGIN
	INSERT INTO @tblDrawingList 
		SELECT d.[DrawingGuid] FROM [dbo].[tblDrawings] d
		WHERE d.SiteGuid = @sync_context_site_guid AND d.PointTemplateGuid IS NULL
	RETURN;
END