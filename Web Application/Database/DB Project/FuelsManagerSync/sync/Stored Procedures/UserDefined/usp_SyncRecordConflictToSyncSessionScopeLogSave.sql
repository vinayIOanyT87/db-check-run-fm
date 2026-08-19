CREATE PROCEDURE [sync].[usp_SyncRecordConflictToSyncSessionScopeLogSave](
	@IdentityGuid uniqueidentifier = NULL
    ,@SyncRecordConflictGuid uniqueidentifier    
    ,@SyncSessionScopeLogGuid uniqueidentifier    
	,@CreatedBy udtUserID
	,@NewRowGuid uniqueidentifier out
)
AS
BEGIN
	SET @NewRowGuid = NULL;

	IF (@IdentityGuid IS NULL)
		SET @NewRowGuid = newid();
	ELSE
		SET @NewRowGuid = @IdentityGuid;
	
    ;   
    MERGE [sync].[tblSyncRecordConflictToSyncSessionScopeLog] AS existing
    USING (SELECT @NewRowGuid
                    ,@SyncRecordConflictGuid
                    ,@SyncSessionScopeLogGuid
					,@CreatedBy
            ) AS updates (SyncRecordConflictToSyncSessionScopeLogGuid
							,SyncRecordConflictGuid
                            ,SyncSessionScopeLogGuid
        					,CreatedBy)
    ON (existing.SyncRecordConflictToSyncSessionScopeLogGuid = updates.SyncRecordConflictToSyncSessionScopeLogGuid)
    WHEN Matched
    THEN
        UPDATE SET SyncRecordConflictGuid = updates.SyncRecordConflictGuid
					,SyncSessionScopeLogGuid = updates.SyncSessionScopeLogGuid
    WHEN Not Matched
    THEN
        INSERT (SyncRecordConflictToSyncSessionScopeLogGuid
				,SyncRecordConflictGuid
                ,SyncSessionScopeLogGuid
				,CreatedDate
				,CreatedBy)
            VALUES (@NewRowGuid
                    ,@SyncRecordConflictGuid
					,@SyncSessionScopeLogGuid
					,SYSDATETIMEOFFSET()
					,CreatedBy)
    ;
	
	RETURN;
END
