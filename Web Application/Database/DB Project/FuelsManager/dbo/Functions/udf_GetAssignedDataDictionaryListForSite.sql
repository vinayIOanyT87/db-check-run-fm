CREATE FUNCTION [dbo].[udf_GetAssignedDataDictionaryListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblDataDictionaryList TABLE
(
	[DataDictionaryToSiteGuid] [uniqueidentifier]
	,[DataDictionaryGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	INSERT INTO @tblDataDictionaryList
		SELECT [map].[tblEntityDataDictionaryToSite].[DataDictionaryToSiteGuid], [dbo].[tblDataDictionaries].[DataDictionaryGuid],[dbo].[tblDataDictionaries].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityDataDictionaryToSite].[MapToSiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityDataDictionaryToSite]
			INNER JOIN [dbo].[tblDataDictionaries]
				ON [map].[tblEntityDataDictionaryToSite].[OwnerSiteGuid] = [dbo].[tblDataDictionaries].[SiteGuid]
		WHERE ([map].[tblEntityDataDictionaryToSite].[MapToSiteGuid] = @sync_context_site_guid)

	RETURN;
END