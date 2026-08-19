CREATE FUNCTION [dbo].[udf_GetAssignedReportDetailListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblReportDetailList TABLE
(
	[ReportConfigurationSettingsToSiteGuid] [uniqueidentifier]
	,[ReportDetailGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	INSERT INTO @tblReportDetailList
			SELECT [map].[tblEntityReportConfigurationSettingsToSite].[ReportConfigurationSettingsToSiteGuid], [dbo].[tblReportDetails].[ReportDetailGuid],[dbo].[tblReportDetails].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityReportConfigurationSettingsToSite].[MapToSiteGuid] 'AssignedToSiteGuid'
				FROM [map].[tblEntityReportConfigurationSettingsToSite]
					INNER JOIN [dbo].[tblReportDetails]
						ON [map].[tblEntityReportConfigurationSettingsToSite].[SiteGuid] = [dbo].[tblReportDetails].[SiteGuid]
				WHERE ([map].[tblEntityReportConfigurationSettingsToSite].[MapToSiteGuid] = @sync_context_site_guid)

	RETURN;
END