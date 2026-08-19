CREATE PROCEDURE [dbo].[usp_GetNodeHealth](@SiteGuid UniqueIdentifier, @NodeHealth NVarchar(2), @OrderBy NVARCHAR(MAX), @SiteID NVarchar(120), @NodeName NVarchar(256)) 
AS
BEGIN

	DECLARE @CriticalThresholdHours int
	SET @CriticalThresholdHours = (SELECT NodeHealthCriticalThresholdHours from tblSyncServerConfiguration)

	DECLARE @CautionThresholdHours int
	SET @CautionThresholdHours = (SELECT NodeHealthCautionThresholdHours from tblSyncServerConfiguration)
	 
	select SiteID into #siteid from dbo.udf_GetSiteToSiteHierarchyListForSiteGuid(@SiteGuid,1,0,1,0,1,0)

	SELECT y.SyncSessionLogGuid AS SyncSessionGuid,* INTO #TMP FROM
	(SELECT  d.*,
			CASE
				WHEN d.conflicts > 0 OR d.lastSyncHours >= @CriticalThresholdHours THEN 2 
				WHEN d.lastSyncHours >= @CautionThresholdHours AND d.lastSyncHours < @CriticalThresholdHours THEN 1 
				ELSE 0 
			END AS [nodeHealthIndicator]
	FROM (SELECT sessionLog.RemoteNodeMachineName AS [nodeName]
			,sites.Number AS [siteName]
			,sites.ID AS [siteID]
			,COUNT(conflict.TargetNodeGuid) AS [conflicts]
			,MAX(sessionLog.EndDate) AS [lastSyncDate]
			,DATEDIFF(HOUR, CAST(MAX(sessionLog.EndDate) as datetime), CURRENT_TIMESTAMP) as [lastSyncHours]
			,SUM(scopeLog.TotalChangesCount) AS [syncCount]
			,MAX(DATEDIFF(MINUTE, sessionLog.StartDate, sessionLog.EndDate)) AS [syncTimeMinutes] 
			,'' AS [notes]
		FROM sync.tblSyncSessionLog sessionLog 
		INNER JOIN sync.tblSyncSessionScopeLog scopeLog ON scopeLog.SyncSessionLogGuid = sessionLog.SyncSessionLogGuid
		LEFT JOIN sync.tblSyncRecordConflictToSyncSessionScopeLog mapScopeLog ON mapScopeLog.SyncSessionScopeLogGuid = scopeLog.SyncSessionScopeLogGuid
		LEFT JOIN sync.tblSyncRecordConflict conflict ON  conflict.ResolvedBy IS NULL AND conflict.SyncRecordConflictGuid = mapScopeLog.SyncRecordConflictGuid 
		LEFT OUTER JOIN tblSites sites ON sites.SiteGuid = scopeLog.SiteGuid
		WHERE sites.ID IN (select SiteID from #siteid)
		
		GROUP BY sessionLog.RemoteNodeMachineName, sites.Number, sites.ID
		) AS d ) AS x JOIN sync.tblSyncSessionLog  y ON x.lastSyncDate = y.EndDate AND x.nodeName = y.RemoteNodeMachineName
	WHERE ISNULL(@NodeHealth,'') IN ('', nodeHealthIndicator) AND ISNULL(@SiteID,'') IN ('', [siteID]) AND ISNULL(@NodeName,'') IN ('', [nodeName])
	ORDER BY nodeHealthIndicator DESC, siteID ASC
	IF (LEN(@OrderBy) > 0)
		SET @OrderBY = ' ORDER BY ' + @OrderBy;
	ELSE
		SET @OrderBy = 'ORDER BY siteID'
	EXEC ('SELECT * FROM #TMP ' + @OrderBy)

END



