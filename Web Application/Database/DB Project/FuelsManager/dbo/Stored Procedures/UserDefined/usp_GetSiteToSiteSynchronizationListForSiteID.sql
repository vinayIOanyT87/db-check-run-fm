
-- ===========================================================================================
-- Author:		<Author,,Peters George C>
-- Create Date:	<Create Date,,09-17-2012>
-- Description:	<Description,,This stored procedure creates a Site Dependency Graph for the
-- SiteID passed in.  Ancestors represent the SiteGroups that the SiteID is a member of OR 
-- is a descendant of.  If the passed in SiteID is for a SiteGroup, then the descendants 
-- represent the SiteGroups or Sites that are members of the SiteGroup.  Typically there will
-- not be any Descendants if the passed in SiteID represents a Site.
--
-- The SiteID will always have a SiteTreeLevel of 0.  Any Ancestors will have a negative (-)
-- SiteTreeLevel and any Descendants will have a positive (+) SiteTreeLevel.
-- ===========================================================================================

/* {CheckPoint: CREATING STORED PROCEDURE: usp_GetSiteToSiteSynchronizationListForSiteID } */
CREATE PROCEDURE [dbo].[usp_GetSiteToSiteSynchronizationListForSiteID]
    @SiteID nvarchar(30) = NULL
AS
BEGIN
	SELECT [SiteGuid]
			,[SiteID]
			,[Level] 
			,[NodeType] 
			,[EnablePeriodicSyncFlag]
			,[PeriodicSyncIntervalMinutes]
			,[DisableSyncTransferFlag]
	FROM [dbo].[udf_GetSiteToSiteHierarchyListForSiteID](@SiteID, 0, 0, 0, 0, 0, 0)
	ORDER BY [Level];
END