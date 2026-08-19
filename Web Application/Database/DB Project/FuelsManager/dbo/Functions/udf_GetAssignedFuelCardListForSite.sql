CREATE FUNCTION [dbo].[udf_GetAssignedFuelCardListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblFuelCardList TABLE
(
	[FuelCardToSiteGuid] [uniqueidentifier]
	,[FuelCardGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	INSERT INTO @tblFuelCardList 
		SELECT [map].[tblEntityFuelCardToSite].[FuelCardToSiteGuid], [dbo].[tblFuelCards].[FuelCardGuid],[dbo].[tblFuelCards].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityFuelCardToSite].[SiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityFuelCardToSite]
			INNER JOIN [dbo].[tblFuelCards]
				ON [map].[tblEntityFuelCardToSite].[FuelCardGuid] = [dbo].[tblFuelCards].[FuelCardGuid]
		WHERE ([map].[tblEntityFuelCardToSite].[SiteGuid] = @sync_context_site_guid)

	RETURN;
END