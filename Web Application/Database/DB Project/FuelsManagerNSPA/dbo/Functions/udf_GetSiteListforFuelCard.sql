CREATE FUNCTION [dbo].[udf_GetSiteListForFuelCard](
@fuelCardGuid uniqueidentifier
)
RETURNS @tblFuelCardList TABLE
(
	[AssignedToSiteGuid] [uniqueidentifier]
	,[SiteID] [nvarchar] (30)
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	INSERT INTO @tblFuelCardList 
		SELECT [map].[tblEntityFuelCardToSite].[SiteGuid] 'AssignedToSiteGuid', [dbo].[tblSites].[ID] 'SiteID'
		FROM [map].[tblEntityFuelCardToSite]
			INNER JOIN [dbo].[tblSites]
				ON [map].[tblEntityFuelCardToSite].[SiteGuid] = [dbo].[tblSites].[SiteGuid]
		WHERE ([map].[tblEntityFuelCardToSite].[FuelCardGuid] = @fuelCardGuid)

	RETURN;
END