
CREATE FUNCTION [dbo].[udf_GetAssignedTransactionAliasListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblTransactionAliasList TABLE
(
	[TransactionAliasToSiteGuid] [uniqueidentifier]
	,[TransactionAliasGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN
	DECLARE @entityAssignmentList AS TABLE
	(
		TransactionAliasGuid uniqueidentifier
		,MasterRecordGuid uniqueidentifier
		,AssignedFromSiteGuid uniqueidentifier
		,AssignedToSiteGuid uniqueidentifier
	)

	-- First we will leverage a record versioning function which will return a list of all self-owned entities 
	-- in addition to the actual entities that are applicable to @sync_context_site_guid.
	-- Remember, the TransactionAliasGuid in the mapping table always points to the MasterTransactionAliasGuid so we need to utilize this
	-- function in order to obtain the correct TransactionAliasGuid values that correspond to each entity assignment record.
	INSERT INTO @entityAssignmentList
		SELECT data.[TransactionAliasGuid], data.[MasterRecordGuid],data.[AssignedFromSiteGuid],data.[AssignedToSiteGuid]
		FROM [erv].[udf_GetTransactionAliasRecordVersions](@sync_context_site_guid) data

	-- (Below), the first part of the union is our standard entity assignment query which will only return entities that were 
	-- created by the site (owned) or the master entity associated with the site via record versioning.  Typically, this query
	-- would return owned entities and the correct assigned entity.  However; record versioning only records the master 
	-- entity's guid in the entity assignment table, not the actual entity that is applicable to the site (unless it's self owned).
	-- This first query is still needed, because we must synchronize the master entity in order to support record versioning 
	-- because of foreign key references.

	-- One item that is missing from the record versioning results above, and that synchronization needs, is the 
	-- EntityTransactionAliasToSiteGuid.  So, the second part of the union joins the @entityAssignmentList back to the entity mapping
	-- table so that we can obtain the missing piece of information.  Caution: the TransactionAliasGuid in the mapping table 
	-- always points to the MasterTransactionAliasGuid, so we will return the TransactionAliasGuid from the @entityAssignmentList table
	-- instead of the TransactionAliasGuid from the mapping table.
	INSERT INTO @tblTransactionAliasList 
		SELECT [map].[tblEntityTransactionAliasToSite].[TransactionAliasToSiteGuid], [dbo].[tblTransactionAliases].[TransactionAliasGuid],[dbo].[tblTransactionAliases].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityTransactionAliasToSite].[SiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityTransactionAliasToSite]
			INNER JOIN [dbo].[tblTransactionAliases]
				ON [map].[tblEntityTransactionAliasToSite].[TransactionAliasGuid] = [dbo].[tblTransactionAliases].[TransactionAliasGuid]
		WHERE ([map].[tblEntityTransactionAliasToSite].[SiteGuid] = @sync_context_site_guid)
		UNION
		SELECT [map].[tblEntityTransactionAliasToSite].[TransactionAliasToSiteGuid], data1.[TransactionAliasGuid],[dbo].[tblTransactionAliases].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityTransactionAliasToSite].[SiteGuid] 'AssignedToSiteGuid'
			FROM @entityAssignmentList data1
				INNER JOIN [map].[tblEntityTransactionAliasToSite]
					ON [map].[tblEntityTransactionAliasToSite].[TransactionAliasGuid] = data1.[MasterRecordGuid]
						AND [map].[tblEntityTransactionAliasToSite].[AssignedFromSiteGuid] = data1.[AssignedFromSiteGuid]
						AND [map].[tblEntityTransactionAliasToSite].[SiteGuid] = data1.[AssignedToSiteGuid]
				INNER JOIN [dbo].[tblTransactionAliases]
					ON [dbo].[tblTransactionAliases].[TransactionAliasGuid] = data1.[TransactionAliasGuid]
			WHERE [dbo].[tblTransactionAliases].[TransactionAliasGuid] <> data1.[MasterRecordGuid]

	RETURN;
END