
CREATE FUNCTION [dbo].[udf_GetAssignedProductListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblProductList TABLE
(
	[ProductToSiteGuid] [uniqueidentifier]
	,[ProductGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN
	DECLARE @entityAssignmentList AS TABLE
	(
		ProductGuid uniqueidentifier
		,MasterRecordGuid uniqueidentifier
		,AssignedFromSiteGuid uniqueidentifier
		,AssignedToSiteGuid uniqueidentifier
	)

	-- First we will leverage a record versioning function which will return a list of all self-owned entities 
	-- in addition to the actual entities that are applicable to @sync_context_site_guid.
	-- Remember, the ProductGuid in the mapping table always points to the MasterProductGuid so we need to utilize this
	-- function in order to obtain the correct ProductGuid values that correspond to each entity assignment record.
	INSERT INTO @entityAssignmentList
		SELECT data.[ProductGuid], data.[MasterRecordGuid],data.[AssignedFromSiteGuid],data.[AssignedToSiteGuid]
		FROM [erv].[udf_GetProductRecordVersions](@sync_context_site_guid) data

	-- (Below), the first part of the union is our standard entity assignment query which will only return entities that were 
	-- created by the site (owned) or the master entity associated with the site via record versioning.  Typically, this query
	-- would return owned entities and the correct assigned entity.  However; record versioning only records the master 
	-- entity's guid in the entity assignment table, not the actual entity that is applicable to the site (unless it's self owned).
	-- This first query is still needed, because we must synchronize the master entity in order to support record versioning 
	-- because of foreign key references.

	-- One item that is missing from the record versioning results above, and that synchronization needs, is the 
	-- EntityProductToSiteGuid.  So, the second part of the union joins the @entityAssignmentList back to the entity mapping
	-- table so that we can obtain the missing piece of information.  Caution: the ProductGuid in the mapping table 
	-- always points to the MasterProductGuid, so we will return the ProductGuid from the @entityAssignmentList table
	-- instead of the ProductGuid from the mapping table.
	INSERT INTO @tblProductList 
		SELECT [map].[tblEntityProductToSite].[ProductToSiteGuid], [dbo].[tblProducts].[ProductGuid],[dbo].[tblProducts].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityProductToSite].[SiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityProductToSite]
			INNER JOIN [dbo].[tblProducts]
				ON [map].[tblEntityProductToSite].[ProductGuid] = [dbo].[tblProducts].[ProductGuid]
		WHERE ([map].[tblEntityProductToSite].[SiteGuid] = @sync_context_site_guid)
		UNION
		SELECT [map].[tblEntityProductToSite].[ProductToSiteGuid], data1.[ProductGuid],[dbo].[tblProducts].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityProductToSite].[SiteGuid] 'AssignedToSiteGuid'
			FROM @entityAssignmentList data1
				INNER JOIN [map].[tblEntityProductToSite]
					ON [map].[tblEntityProductToSite].[ProductGuid] = data1.[MasterRecordGuid]
						AND [map].[tblEntityProductToSite].[AssignedFromSiteGuid] = data1.[AssignedFromSiteGuid]
						AND [map].[tblEntityProductToSite].[SiteGuid] = data1.[AssignedToSiteGuid]
				INNER JOIN [dbo].[tblProducts]
					ON [dbo].[tblProducts].[ProductGuid] = data1.[ProductGuid]
			WHERE [dbo].[tblProducts].[ProductGuid] <> data1.[MasterRecordGuid]

	RETURN;
END