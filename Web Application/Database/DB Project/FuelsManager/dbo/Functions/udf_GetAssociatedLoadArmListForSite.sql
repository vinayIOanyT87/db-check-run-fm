CREATE FUNCTION [dbo].[udf_GetAssociatedLoadArmListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblLoadArmList TABLE
(
	[LoadArmGuid] [uniqueidentifier]
	,[BayAStationGuid] [uniqueidentifier]
	,[BayBStationGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[CreatedDate] [datetimeoffset](7)
	,[UpdatedDate] [datetimeoffset](7)

)
AS
BEGIN
	DECLARE @tblStationList TABLE
	(
		[StationGuid] [uniqueidentifier]
		,[OwnerSiteGuid] [uniqueidentifier]
	)

	INSERT INTO @tblStationList
		SELECT [StationGuid],[OwnerSiteGuid] FROM [dbo].[udf_GetAssociatedStationListForSite](@sync_context_site_guid)

	-- LoadArm belong to a site and are associated with a Station.  Stations are assigned to a Site indirectly 
	--
	INSERT INTO @tblLoadArmList
        SELECT [dbo].[tblLoadArms].[LoadArmGuid],[dbo].[tblLoadArms].[BayAStationGuid],[dbo].[tblLoadArms].[BayBStationGuid],data.[OwnerSiteGuid] 'OwnerSiteGuid',[dbo].[tblLoadArms].[CreatedDate],[dbo].[tblLoadArms].[UpdatedDate]
            FROM [dbo].[tblLoadArms]
                INNER JOIN (SELECT [StationGuid],[OwnerSiteGuid] FROM @tblStationList) data
                    ON [dbo].[tblLoadArms].[BayAStationGuid] = data.[StationGuid]
            WHERE ([dbo].[tblLoadArms].[BayAStationGuid] IS NOT NULL)
                    AND ([dbo].[tblLoadArms].[BayBStationGuid] IS NULL)
        UNION
        SELECT [dbo].[tblLoadArms].[LoadArmGuid],[dbo].[tblLoadArms].[BayAStationGuid],[dbo].[tblLoadArms].[BayBStationGuid],data1.[OwnerSiteGuid] 'OwnerSiteGuid',[dbo].[tblLoadArms].[CreatedDate],[dbo].[tblLoadArms].[UpdatedDate]
            FROM [dbo].[tblLoadArms]
                INNER JOIN (SELECT [StationGuid],[OwnerSiteGuid] FROM @tblStationList) data1
                    ON [dbo].[tblLoadArms].[BayBStationGuid] = data1.[StationGuid]
            WHERE ([dbo].[tblLoadArms].[BayAStationGuid] IS NULL) 
                    AND ([dbo].[tblLoadArms].[BayBStationGuid] IS NOT NULL)
        UNION
        SELECT [dbo].[tblLoadArms].[LoadArmGuid],[dbo].[tblLoadArms].[BayAStationGuid],[dbo].[tblLoadArms].[BayBStationGuid],data.[OwnerSiteGuid] 'OwnerSiteGuid',[dbo].[tblLoadArms].[CreatedDate],[dbo].[tblLoadArms].[UpdatedDate]
            FROM [dbo].[tblLoadArms]
                INNER JOIN (SELECT [StationGuid],[OwnerSiteGuid] FROM @tblStationList) data
                    ON [dbo].[tblLoadArms].[BayAStationGuid] = data.[StationGuid]
                INNER JOIN (SELECT [StationGuid],[OwnerSiteGuid] FROM @tblStationList) data1
                    ON [dbo].[tblLoadArms].[BayBStationGuid] = data1.[StationGuid]
            WHERE ([dbo].[tblLoadArms].[BayAStationGuid] IS NOT NULL)
                    AND ([dbo].[tblLoadArms].[BayBStationGuid] IS NOT NULL)
	RETURN;
END
