
CREATE FUNCTION [dbo].[udf_GetAssignedEquipmentListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblEquipmentList TABLE
(
	[EquipmentToSiteGuid] [uniqueidentifier]
	,[EquipmentGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN
	DECLARE @entityAssignmentList AS TABLE
	(
		EquipmentGuid uniqueidentifier
		,MasterRecordGuid uniqueidentifier
		,AssignedFromSiteGuid uniqueidentifier
		,AssignedToSiteGuid uniqueidentifier
	)

	-- First we will leverage a record versioning function which will return a list of all self-owned entities 
	-- in addition to the actual entities that are applicable to @sync_context_site_guid.
	-- Remember, the EquipmentGuid in the mapping table always points to the MasterEquipmentGuid so we need to utilize this
	-- function in order to obtain the correct EquipmentGuid values that correspond to each entity assignment record.
	INSERT INTO @entityAssignmentList
		SELECT data.[EquipmentGuid], data.[MasterRecordGuid],data.[AssignedFromSiteGuid],data.[AssignedToSiteGuid]
		FROM [erv].[udf_GetEquipmentRecordVersions](@sync_context_site_guid) data

	-- (Below), the first part of the union is our standard entity assignment query which will only return entities that were 
	-- created by the site (owned) or the master entity associated with the site via record versioning.  Typically, this query
	-- would return owned entities and the correct assigned entity.  However; record versioning only records the master 
	-- entity's guid in the entity assignment table, not the actual entity that is applicable to the site (unless it's self owned).
	-- This first query is still needed, because we must synchronize the master entity in order to support record versioning 
	-- because of foreign key references.

	-- One item that is missing from the record versioning results above, and that synchronization needs, is the 
	-- EntityEquipmentToSiteGuid.  So, the second part of the union joins the @entityAssignmentList back to the entity mapping
	-- table so that we can obtain the missing piece of information.  Caution: the EquipmentGuid in the mapping table 
	-- always points to the MasterEquipmentGuid, so we will return the EquipmentGuid from the @entityAssignmentList table
	-- instead of the EquipmentGuid from the mapping table.
	INSERT INTO @tblEquipmentList
		SELECT [map].[tblEntityEquipmentToSite].[EquipmentToSiteGuid], [dbo].[tblEquipment].[EquipmentGuid],[dbo].[tblEquipment].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityEquipmentToSite].[SiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityEquipmentToSite]
			INNER JOIN [dbo].[tblEquipment]
				ON [map].[tblEntityEquipmentToSite].[EquipmentGuid] = [dbo].[tblEquipment].[EquipmentGuid]
		WHERE ([map].[tblEntityEquipmentToSite].[SiteGuid] = @sync_context_site_guid)
		UNION
		SELECT [map].[tblEntityEquipmentToSite].[EquipmentToSiteGuid], data1.[EquipmentGuid],[dbo].[tblEquipment].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityEquipmentToSite].[SiteGuid] 'AssignedToSiteGuid'
			FROM @entityAssignmentList data1
				INNER JOIN [map].[tblEntityEquipmentToSite]
					ON [map].[tblEntityEquipmentToSite].[EquipmentGuid] = data1.[MasterRecordGuid]
						AND [map].[tblEntityEquipmentToSite].[AssignedFromSiteGuid] = data1.[AssignedFromSiteGuid]
						AND [map].[tblEntityEquipmentToSite].[SiteGuid] = data1.[AssignedToSiteGuid]
				INNER JOIN [dbo].[tblEquipment]
					ON [dbo].[tblEquipment].[EquipmentGuid] = data1.[EquipmentGuid]
			WHERE [dbo].[tblEquipment].[EquipmentGuid] <> data1.[MasterRecordGuid]

	RETURN;
END
