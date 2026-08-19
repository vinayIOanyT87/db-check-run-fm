CREATE FUNCTION [dbo].[udf_GetAssignedTestSetDefinitionListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblTestSetDefinitionList TABLE
(
	[TestSetToSiteGuid] [uniqueidentifier]
	,[TestSetDefinitionGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	INSERT INTO @tblTestSetDefinitionList 
		SELECT [map].[tblEntityTestSetToSite].[TestSetToSiteGuid], [dbo].[tblTestSetDefinitions].[TestSetDefinitionGuid],[dbo].[tblTestSetDefinitions].[OwnerSiteGuid] 'OwnerSiteGuid',[map].[tblEntityTestSetToSite].[SiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityTestSetToSite]
			INNER JOIN [dbo].[tblTestSetDefinitions]
				ON [map].[tblEntityTestSetToSite].[TestSetDefinitionGuid] = [dbo].[tblTestSetDefinitions].[TestSetDefinitionGuid]
		WHERE ([map].[tblEntityTestSetToSite].[SiteGuid] = @sync_context_site_guid)

	RETURN;
END