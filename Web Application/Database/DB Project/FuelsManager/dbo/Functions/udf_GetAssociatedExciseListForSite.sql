CREATE FUNCTION [dbo].[udf_GetAssociatedExciseListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblExciseList TABLE
(
	[ExciseGuid] [uniqueidentifier]
	,[ProductToSiteGuid] [uniqueidentifier]
	,[ProductGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN
	--
	--
	INSERT INTO @tblExciseList 
		SELECT data.[ExciseGuid],data.[ProductToSiteGuid],data.[ProductGuid],data.[OwnerSiteGuid],data.[AssignedToSiteGuid]
			FROM (SELECT [dbo].[tblExcise].[ExciseGuid],data.[ProductToSiteGuid],data.[ProductGuid],data.[OwnerSiteGuid],data.[AssignedToSiteGuid]
					FROM [dbo].[tblExcise]
						INNER JOIN (SELECT [ProductToSiteGuid],[ProductGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedProductListForSite](@sync_context_site_guid)) data
							ON [dbo].[tblExcise].[ProductGuid] = data.[ProductGuid]
				) data
	RETURN;
END