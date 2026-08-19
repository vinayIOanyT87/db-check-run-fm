CREATE FUNCTION [dbo].[udf_GetAssociatedCustomToolbarListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblCustomToolbarList TABLE
(
	[CustomToolbarGuid] [uniqueidentifier]
	,[DispatchConfigurationToSiteGuid] [uniqueidentifier]
	,[DispatchConfigurationGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN
	--
	--
	INSERT INTO @tblCustomToolbarList 
		SELECT data.[CustomToolbarGuid]
				,data.[DispatchConfigurationToSiteGuid]
				,data.[DispatchConfigurationGuid]
				,data.[OwnerSiteGuid] 
				,data.[AssignedToSiteGuid]
			FROM (SELECT [dbo].[tblCustomToolbar].[CustomToolbarGuid]
						,data.DispatchConfigurationToSiteGuid
						,data.DispatchConfigurationGuid
						,data.[OwnerSiteGuid]
						,data.[AssignedToSiteGuid]
					FROM [dbo].[tblCustomToolbar]
						INNER JOIN (SELECT [DispatchConfigurationToSiteGuid],[DispatchConfigurationGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedDispatchConfigurationListForSite](@sync_context_site_guid)) data
							ON [dbo].[tblCustomToolbar].[DispatchConfigurationGuid] = data.[DispatchConfigurationGuid]
				) data
	RETURN;
END