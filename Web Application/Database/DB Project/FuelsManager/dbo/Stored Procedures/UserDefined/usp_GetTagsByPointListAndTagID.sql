CREATE PROCEDURE [dbo].[usp_GetTagsByPointListAndTagID]
    @pointList GuidListType READONLY,
	@tagID nvarchar( 255 )
AS
BEGIN
    SELECT pt.*, ptt.WellKnownIdentityGuid
	FROM dbo.tblPointTag pt
	JOIN tblPointTemplateTag ptt ON pt.PointTemplateTagGuid = ptt.PointTemplateTagGuid
	JOIN @pointList pl
	ON pl.Guid = pt.PointGuid
	WHERE pt.ID = @tagID
END