CREATE FUNCTION [dbo].[udf_GetAssignedReportGroupListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblReportGroupList TABLE
(
	[ReportConfigurationSettingsToSiteGuid] [uniqueidentifier]
	,[ReportGroupGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	INSERT INTO @tblReportGroupList
		SELECT [map].[tblEntityReportConfigurationSettingsToSite].[ReportConfigurationSettingsToSiteGuid], [dbo].[tblReportGroups].[ReportGroupGuid],[dbo].[tblReportGroups].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityReportConfigurationSettingsToSite].[MapToSiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityReportConfigurationSettingsToSite]
			INNER JOIN [dbo].[tblReportGroups]
				ON [map].[tblEntityReportConfigurationSettingsToSite].[SiteGuid] = [dbo].[tblReportGroups].[SiteGuid]
		WHERE ([map].[tblEntityReportConfigurationSettingsToSite].[MapToSiteGuid] = @sync_context_site_guid)

	RETURN;
END