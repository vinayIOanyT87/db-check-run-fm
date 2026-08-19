CREATE PROCEDURE [sync].[usp_SyncSessionLogSave](
	@IdentityGuid uniqueidentifier = NULL
    ,@SyncProfileID nvarchar(80)
    ,@SyncRequestTypeIndex bigint
    ,@SyncTransferTypeIndex bigint
    ,@SyncSessionStatusIndex bigint
    ,@SyncSessionStateIndex bigint
    ,@SyncDateRangeStart datetimeoffset
    ,@SyncDateRangeEnd datetimeoffset
    ,@StartDate datetimeoffset
    ,@EndDate datetimeoffset
    ,@RemoteNodeGuid uniqueidentifier
    ,@RemoteNodeMachineName nvarchar(256)
    ,@SyncAnchorMax bigint
    ,@CreatedBy nvarchar(100)
    ,@UpdatedBy nvarchar(100)
    ,@NewRowGuid uniqueidentifier out
)
AS
BEGIN
    SET @NewRowGuid = NULL;

    IF (@IdentityGuid IS NULL)
        SET @NewRowGuid = newid();
    ELSE
        SET @NewRowGuid = @IdentityGuid;

	-- Initial Synchronization, delete rows from any prior sessions associated with the @RemoteNodeMachineName
	IF @SyncRequestTypeIndex = 4
	AND @EndDate IS NULL
	BEGIN
		DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncSessionScopeLogGuid IN
			(SELECT SyncSessionScopeLogGuid FROM [sync].[tblSyncSessionScopeLog] WHERE SyncSessionLogGuid IN
			(SELECT SyncSessionLogGuid FROM [sync].[tblSyncSessionLog] WHERE RemoteNodeGuid <> @RemoteNodeGuid AND RemoteNodeMachineName = @RemoteNodeMachineName)) 

		DELETE FROM [sync].[tblSyncSessionScopeLog] WHERE SyncSessionLogGuid IN
			(SELECT SyncSessionLogGuid FROM [sync].[tblSyncSessionLog] WHERE RemoteNodeGuid <> @RemoteNodeGuid AND RemoteNodeMachineName = @RemoteNodeMachineName) 

		DELETE FROM [sync].[tblSyncRecordConflict] WHERE TargetNodeGuid IN 
			(SELECT DISTINCT RemoteNodeGuid FROM [sync].[tblSyncSessionLog] WHERE RemoteNodeGuid <> @RemoteNodeGuid AND RemoteNodeMachineName = @RemoteNodeMachineName) 

		DELETE FROM [sync].[tblSyncSessionLog] WHERE RemoteNodeGuid <> @RemoteNodeGuid AND RemoteNodeMachineName = @RemoteNodeMachineName
	END

    
   ;MERGE [sync].[tblSyncSessionLog] AS existing
    USING (SELECT @NewRowGuid
                    ,@SyncProfileID
                    ,@SyncRequestTypeIndex
                    ,@SyncTransferTypeIndex
                    ,@SyncSessionStatusIndex
                    ,@SyncSessionStateIndex
                    ,@SyncDateRangeStart
                    ,@SyncDateRangeEnd
                    ,@StartDate
                    ,@EndDate
                    ,@RemoteNodeGuid
                    ,@RemoteNodeMachineName
                    ,CONVERT(binary(8), @SyncAnchorMax)
                    ,@CreatedBy
                    ,@UpdatedBy
            ) AS updates (SyncSessionLogGuid
                            ,SyncProfileID
                            ,SyncRequestTypeIndex
                            ,SyncTransferTypeIndex
                            ,SyncSessionStatusIndex
                            ,SyncSessionStateIndex
                            ,SyncDateRangeStart
                            ,SyncDateRangeEnd
                            ,StartDate
                            ,EndDate
                            ,RemoteNodeGuid
                            ,RemoteNodeMachineName
                            ,SyncAnchorMax
                            ,CreatedBy
                            ,UpdatedBy)
    ON (existing.SyncSessionLogGuid = updates.SyncSessionLogGuid)
    WHEN Matched
    THEN
        UPDATE SET SyncProfileID = updates.SyncProfileID
                    ,SyncRequestTypeIndex = updates.SyncRequestTypeIndex
                    ,SyncTransferTypeIndex = updates.SyncTransferTypeIndex
                    ,SyncSessionStatusIndex = updates.SyncSessionStatusIndex
                    ,SyncSessionStateIndex = updates.SyncSessionStateIndex
                    ,SyncDateRangeStart = updates.SyncDateRangeStart
                    ,SyncDateRangeEnd = updates.SyncDateRangeEnd
                    ,StartDate = updates.StartDate
                    ,EndDate = updates.EndDate
                    ,RemoteNodeGuid = updates.RemoteNodeGuid
                    ,RemoteNodeMachineName = updates.RemoteNodeMachineName
                    ,SyncAnchorMax = updates.SyncAnchorMax
                    ,UpdatedDate = SYSDATETIMEOFFSET()
                    ,UpdatedBy = updates.UpdatedBy
    WHEN Not Matched
    THEN
        INSERT (SyncSessionLogGuid
                ,SyncProfileID
                ,SyncRequestTypeIndex
                ,SyncTransferTypeIndex
                ,SyncSessionStatusIndex
                ,SyncSessionStateIndex
                ,SyncDateRangeStart
                ,SyncDateRangeEnd
                ,StartDate
                ,EndDate
                ,RemoteNodeGuid
                ,RemoteNodeMachineName
                ,SyncAnchorMax
                ,CreatedDate
                ,CreatedBy
                ,UpdatedDate
                ,UpdatedBy)
            VALUES (@NewRowGuid
                    ,@SyncProfileID
                    ,@SyncRequestTypeIndex
                    ,@SyncTransferTypeIndex
                    ,@SyncSessionStatusIndex
                    ,@SyncSessionStateIndex
                    ,@SyncDateRangeStart
                    ,@SyncDateRangeEnd
                    ,@StartDate
                    ,@EndDate
                    ,@RemoteNodeGuid
                    ,@RemoteNodeMachineName
                    ,CONVERT(binary(8), @SyncAnchorMax)
                    ,SYSDATETIMEOFFSET()
                    ,CreatedBy
                    ,SYSDATETIMEOFFSET()
                    ,UpdatedBy)
    ;

    RETURN;
END
