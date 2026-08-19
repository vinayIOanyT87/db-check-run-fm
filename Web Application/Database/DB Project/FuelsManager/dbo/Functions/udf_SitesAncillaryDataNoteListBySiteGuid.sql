
CREATE FUNCTION [dbo].[udf_SitesAncillaryDataNoteListBySiteGuid](
	@SiteGuid uniqueidentifier 
)
RETURNS TABLE
AS
RETURN
	SELECT n.NoteGuid
		FROM [dbo].[tblSitesAncillaryData] s
			INNER JOIN [dbo].[tblNotes] n
				ON s.NoteGuid = n.NoteGuid 
		WHERE s.SiteGuid = @SiteGuid 
			AND s.NoteGuid IS NOT NULL