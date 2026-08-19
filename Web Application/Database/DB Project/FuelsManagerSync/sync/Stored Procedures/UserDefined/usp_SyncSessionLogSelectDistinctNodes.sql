CREATE PROCEDURE [sync].[usp_SyncSessionLogSelectDistinctNodes] @SiteGuid UniqueIdentifier
AS
BEGIN
	SELECT DISTINCT ss.RemoteNodeGuid, ss.RemoteNodeMachineName
		FROM [sync].[tblSyncSessionLog] AS [ss] WITH (NOLOCK)
			INNER JOIN [sync].[tblSyncSessionScopeLog] AS [SL] with (NOLOCK) on [SS].SyncSessionLogGuid = SL.SyncSessionLogGuid
			INNER JOIN dbo.udf_GetSiteToSiteHierarchyListForSiteGuid(@SiteGuid,1,0,1,0,1,0) AS sh on sl.siteguid = sh.siteguid
			WHERE ss.EndDate IS NOT NULL
END