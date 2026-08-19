
CREATE FUNCTION [dbo].[udf_GetAssignedPersonnelListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblPersonnelList TABLE
(
	[PersonnelToSiteGuid] [uniqueidentifier]
	,[PersonnelGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN
	DECLARE @entityAssignmentList AS TABLE
	(
		PersonnelGuid uniqueidentifier
		,MasterRecordGuid uniqueidentifier
		,AssignedFromSiteGuid uniqueidentifier
		,AssignedToSiteGuid uniqueidentifier
	)

	-- First we will leverage a record versioning function which will return a list of all self-owned entities 
	-- in addition to the actual entities that are applicable to @sync_context_site_guid.
	-- Remember, the PersonnelGuid in the mapping table always points to the MasterPersonnelGuid so we need to utilize this
	-- function in order to obtain the correct PersonnelGuid values that correspond to each entity assignment record.
	INSERT INTO @entityAssignmentList
		SELECT data.[PersonnelGuid], data.[MasterRecordGuid],data.[AssignedFromSiteGuid],data.[AssignedToSiteGuid]
		FROM [erv].[udf_GetPersonnelRecordVersions](@sync_context_site_guid) data

	-- (Below), the first part of the union is our standard entity assignment query which will only return entities that were 
	-- created by the site (owned) or the master entity associated with the site via record versioning.  Typically, this query
	-- would return owned entities and the correct assigned entity.  However; record versioning only records the master 
	-- entity's guid in the entity assignment table, not the actual entity that is applicable to the site (unless it's self owned).
	-- This first query is still needed, because we must synchronize the master entity in order to support record versioning 
	-- because of foreign key references.

	-- One item that is missing from the record versioning results above, and that synchronization needs, is the 
	-- EntityPersonnelToSiteGuid.  So, the second part of the union joins the @entityAssignmentList back to the entity mapping
	-- table so that we can obtain the missing piece of information.  Caution: the PersonnelGuid in the mapping table 
	-- always points to the MasterPersonnelGuid, so we will return the PersonnelGuid from the @entityAssignmentList table
	-- instead of the PersonnelGuid from the mapping table.
	INSERT INTO @tblPersonnelList 
		SELECT [map].[tblEntityPersonnelToSite].[PersonnelToSiteGuid], [dbo].[tblPersonnel].[PersonnelGuid],[dbo].[tblPersonnel].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityPersonnelToSite].[SiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityPersonnelToSite]
			INNER JOIN [dbo].[tblPersonnel]
				ON [map].[tblEntityPersonnelToSite].[PersonnelGuid] = [dbo].[tblPersonnel].[PersonnelGuid]
		WHERE ([map].[tblEntityPersonnelToSite].[SiteGuid] = @sync_context_site_guid)
		UNION
		SELECT [map].[tblEntityPersonnelToSite].[PersonnelToSiteGuid], data1.[PersonnelGuid],[dbo].[tblPersonnel].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityPersonnelToSite].[SiteGuid] 'AssignedToSiteGuid'
			FROM @entityAssignmentList data1
				INNER JOIN [map].[tblEntityPersonnelToSite]
					ON [map].[tblEntityPersonnelToSite].[PersonnelGuid] = data1.[MasterRecordGuid]
						AND [map].[tblEntityPersonnelToSite].[AssignedFromSiteGuid] = data1.[AssignedFromSiteGuid]
						AND [map].[tblEntityPersonnelToSite].[SiteGuid] = data1.[AssignedToSiteGuid]
				INNER JOIN [dbo].[tblPersonnel]
					ON [dbo].[tblPersonnel].[PersonnelGuid] = data1.[PersonnelGuid]
			WHERE [dbo].[tblPersonnel].[PersonnelGuid] <> data1.[MasterRecordGuid]

	RETURN;
END