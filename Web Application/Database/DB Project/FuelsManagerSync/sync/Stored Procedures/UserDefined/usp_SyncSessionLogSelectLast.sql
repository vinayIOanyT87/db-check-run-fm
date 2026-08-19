CREATE PROCEDURE [sync].[usp_SyncSessionLogSelectLast]
AS
BEGIN
	SELECT TOP 1 [SyncSessionLogGuid]
				  ,[SyncProfileID]
				  ,[SyncSessionStatusIndex]
				  ,[SyncSessionStateIndex]
				  ,[SyncTransferTypeIndex]
				  ,[SyncRequestTypeIndex]
				  ,[SyncDateRangeStart]
				  ,[SyncDateRangeEnd]
				  ,[StartDate]
				  ,[EndDate]
				  ,[RemoteNodeGuid]
				  ,[RemoteNodeMachineName]
				  ,CONVERT(bigint, [SyncAnchorMax]) 'SyncAnchorMax'
				  ,[CreatedDate]
				  ,[CreatedBy]
				  ,[UpdatedDate]
				  ,[UpdatedBy]
		FROM [sync].[tblSyncSessionLog] WITH (NOLOCK) 
			WHERE EndDate IS NOT NULL
		ORDER BY EndDate DESC
END
