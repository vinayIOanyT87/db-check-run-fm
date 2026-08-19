CREATE FUNCTION [dbo].[udf_GetAssignedFuelCardLimitListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblFuelCardLimitList TABLE
(
	[FuelCardLimitToSiteGuid] [uniqueidentifier]
	,[FuelCardLimitGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	INSERT INTO @tblFuelCardLimitList 
		SELECT [map].[tblEntityFuelCardLimitToSite].[FuelCardLimitToSiteGuid]
                , [dbo].[tblFuelCardLimit].[FuelCardLimitGuid]
                , [dbo].[tblFuelCardLimit].[SiteGuid] 'OwnerSiteGuid'
                , [map].[tblEntityFuelCardLimitToSite].[SiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityFuelCardLimitToSite]
			INNER JOIN [dbo].[tblFuelCardLimit]
				ON [map].[tblEntityFuelCardLimitToSite].[FuelCardLimitGuid] = [dbo].[tblFuelCardLimit].[FuelCardLimitGuid]
		WHERE ([map].[tblEntityFuelCardLimitToSite].[SiteGuid] = @sync_context_site_guid)

	RETURN;
END