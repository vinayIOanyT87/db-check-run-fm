CREATE FUNCTION [dbo].[udf_GetAssignedTestDefinitionListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblTestDefinitionList TABLE
(
	[TestToSiteGuid] [uniqueidentifier]
	,[TestDefinitionGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	INSERT INTO @tblTestDefinitionList 
		SELECT [map].[tblEntityTestToSite].[TestToSiteGuid], [dbo].[tblTestDefinitions].[TestDefinitionGuid],[dbo].[tblTestDefinitions].[OwnerSiteGuid] 'OwnerSiteGuid',[map].[tblEntityTestToSite].[SiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityTestToSite]
			INNER JOIN [dbo].[tblTestDefinitions]
				ON [map].[tblEntityTestToSite].[TestDefinitionGuid] = [dbo].[tblTestDefinitions].[TestDefinitionGuid]
		WHERE ([map].[tblEntityTestToSite].[SiteGuid] = @sync_context_site_guid)

	RETURN;
END