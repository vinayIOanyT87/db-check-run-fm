CREATE FUNCTION [dbo].[udf_GetAssignedAnimationListPerSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblAnimationList TABLE
(
	[AnimationGuid] [uniqueidentifier]
)
AS
BEGIN
	INSERT INTO @tblAnimationList 
		SELECT DISTINCT a.[AnimationGuid] FROM [dbo].[tblAnimation] a
		WHERE a.SiteGuid = @sync_context_site_guid
	RETURN;
END