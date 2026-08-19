CREATE FUNCTION [dbo].[udf_GetAssignedStandingOfferListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblStandingOfferList TABLE
(
	[StandingOfferToSiteGuid] [uniqueidentifier]
	,[StandingOfferGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	INSERT INTO @tblStandingOfferList 
		SELECT [map].[tblEntityStandingOfferToSite].[StandingOfferToSiteGuid], [dbo].[tblStandingOffers].[StandingOfferGuid],[dbo].[tblStandingOffers].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityStandingOfferToSite].[SiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityStandingOfferToSite]
			INNER JOIN [dbo].[tblStandingOffers]
				ON [map].[tblEntityStandingOfferToSite].[StandingOfferGuid] = [dbo].[tblStandingOffers].[StandingOfferGuid]
		WHERE ([map].[tblEntityStandingOfferToSite].[SiteGuid] = @sync_context_site_guid)

	RETURN;
END