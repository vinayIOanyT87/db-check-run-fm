
CREATE PROCEDURE [sync].[usp_SyncTableToScopeMapCommandSave](
	@IdentityGuid uniqueidentifier = NULL
	,@SyncTableToScopeMapGuid uniqueidentifier = NULL
	,@SelectIncrementalInserts nvarchar(512) = NULL
	,@ApplyIncrementalInserts nvarchar(512) = NULL
	,@SelectIncrementalUpdates nvarchar(512) = NULL
	,@ApplyIncrementalUpdates nvarchar(512) = NULL
	,@SelectIncrementalDeletes nvarchar(512) = NULL
	,@ApplyIncrementalDeletes nvarchar(512) = NULL
	,@SelectUpdateConflicts nvarchar(512) = NULL
	,@SelectDeleteConflicts nvarchar(512) = NULL
	,@CreatedBy udtUserID
	,@UpdatedBy udtUserID
	,@NewRowGuid uniqueidentifier out
)
AS
BEGIN
	SET @NewRowGuid = NULL;
	
	IF (@IdentityGuid IS NULL)
		SET @NewRowGuid = newid();
	ELSE
		SET @NewRowGuid = @IdentityGuid;
	
    ; MERGE [sync].[tblSyncTableToScopeMapCommand] AS existing
    USING (SELECT @NewRowGuid
					,@SyncTableToScopeMapGuid
					,@SelectIncrementalInserts
					,@ApplyIncrementalInserts
					,@SelectIncrementalUpdates
					,@ApplyIncrementalUpdates
					,@SelectIncrementalDeletes
					,@ApplyIncrementalDeletes
					,@SelectUpdateConflicts
					,@SelectDeleteConflicts
					,@CreatedBy
					,@UpdatedBy
            ) AS updates (SyncTableToScopeMapCommandGuid
							,SyncTableToScopeMapGuid
							,SelectIncrementalInserts
							,ApplyIncrementalInserts
							,SelectIncrementalUpdates
							,ApplyIncrementalUpdates
							,SelectIncrementalDeletes
							,ApplyIncrementalDeletes
							,SelectUpdateConflicts
							,SelectDeleteConflicts
							,CreatedBy
							,UpdatedBy)
    ON (existing.SyncTableToScopeMapCommandGuid = updates.SyncTableToScopeMapCommandGuid)
    WHEN Matched
    THEN
        UPDATE SET SyncTableToScopeMapGuid = updates.SyncTableToScopeMapGuid
					,SelectIncrementalInserts = updates.SelectIncrementalInserts
					,ApplyIncrementalInserts = updates.ApplyIncrementalInserts
					,SelectIncrementalUpdates = updates.SelectIncrementalUpdates
					,ApplyIncrementalUpdates = updates.ApplyIncrementalUpdates
					,SelectIncrementalDeletes = updates.SelectIncrementalDeletes
					,ApplyIncrementalDeletes = updates.ApplyIncrementalDeletes
					,SelectUpdateConflicts = updates.SelectUpdateConflicts
					,SelectDeleteConflicts = updates.SelectDeleteConflicts
					,UpdatedDate = SYSDATETIMEOFFSET()
					,UpdatedBy = updates.UpdatedBy
    WHEN Not Matched
    THEN
        INSERT (SyncTableToScopeMapCommandGuid
				,SyncTableToScopeMapGuid
				,SelectIncrementalInserts
				,ApplyIncrementalInserts
				,SelectIncrementalUpdates
				,ApplyIncrementalUpdates
				,SelectIncrementalDeletes
				,ApplyIncrementalDeletes
				,SelectUpdateConflicts
				,SelectDeleteConflicts
				,CreatedDate
				,CreatedBy
				,UpdatedDate
				,UpdatedBy)
            VALUES (@NewRowGuid
					,@SyncTableToScopeMapGuid
					,@SelectIncrementalInserts
					,@ApplyIncrementalInserts
					,@SelectIncrementalUpdates
					,@ApplyIncrementalUpdates
					,@SelectIncrementalDeletes
					,@ApplyIncrementalDeletes
					,@SelectUpdateConflicts
					,@SelectDeleteConflicts
					,SYSDATETIMEOFFSET()
					,CreatedBy
					,SYSDATETIMEOFFSET()
					,UpdatedBy)
    ;
	
	RETURN;
END