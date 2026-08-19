
CREATE PROCEDURE [dbo].[usp_SiteListReadyForPeriodicSyncSelect](
	@RootSiteID nvarchar(30)
)
AS
BEGIN
	DECLARE @hostedSiteList AS TABLE (
		[SiteGuid] uniqueidentifier
		,[SiteID] nvarchar(60)
		,[Level] int
		,[NodeType] nvarchar(25)
	);

	INSERT INTO @hostedSiteList SELECT [SiteGuid]
								,[SiteID]
								,[Level] 
								,[NodeType] 
							FROM [dbo].[udf_GetSiteToSiteHierarchyListForSiteID](@RootSiteID,0,0,0,0,0,0);

	SELECT list.[SiteGuid], list.[SiteID], list.[Level], s.[PeriodicSyncIntervalMinutes]
		FROM [dbo].[tblSites] s
			INNER JOIN (SELECT [SiteGuid],[SiteID],[Level],[NodeType] FROM @hostedSiteList) list
				ON s.SiteGuid = list.SiteGuid
		WHERE (list.[Level] >= 0)
				AND (s.[EnablePeriodicSyncFlag] = 1)
				AND (s.[DisableSyncTransferFlag] = 0)
		ORDER BY list.[Level];

	RETURN;
END