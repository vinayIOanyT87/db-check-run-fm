

CREATE FUNCTION [dbo].[udf_NoteListBySiteGuid](
	@SiteGuid uniqueidentifier
)
RETURNS TABLE 
AS
RETURN 
	SELECT NoteGuid FROM dbo.udf_SitesAncillaryDataNoteListBySiteGuid(@SiteGuid)