
CREATE PROCEDURE [sync].[usp_SyncTableToScopeMapSave](
	@IdentityGuid uniqueidentifier = NULL
	,@ID nvarchar(80)
	,@SyncScopeGuid nvarchar(80)
	,@SyncTableGuid uniqueidentifier
	,@SyncOrder int = NULL
	,@SyncDirection int = NULL
	,@MaxBatchSegmentRowCount int = NULL
	,@MaxTransferSegmentKB int = NULL
	,@AdditionalFilterJoinClause nvarchar(1024) = NULL
	,@AdditionalFilterWhereClause nvarchar(512) = NULL
	,@ClientTableNameOverride nvarchar(1024) = NULL
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
	
	DECLARE @localSyncOrder int
	SET @localSyncOrder = @SyncOrder

	IF (@localSyncOrder IS NULL)
	BEGIN	
		-- Get the last item in the specified Synchronization Group
		SELECT @localSyncOrder = MAX(SyncOrder)
			FROM [sync].[tblSyncTableToScopeMap] WITH(NOLOCK)
				WHERE  SyncScopeGuid = @SyncScopeGuid

		IF (@localSyncOrder IS NULL)
			SET @localSyncOrder = 1;				
		ELSE
			SET @localSyncOrder = @localSyncOrder + 1;
	END
	
    ; MERGE [sync].[tblSyncTableToScopeMap] AS existing
    USING (SELECT @NewRowGuid
					,@ID
					,@SyncScopeGuid
					,@SyncTableGuid
					,@SyncOrder
					,@SyncDirection
					,@MaxBatchSegmentRowCount
					,@MaxTransferSegmentKB
					,@AdditionalFilterJoinClause
					,@AdditionalFilterWhereClause
					,@ClientTableNameOverride
					,@CreatedBy
					,@UpdatedBy
            ) AS updates (SyncTableToScopeMapGuid
							,ID
							,SyncScopeGuid
							,SyncTableGuid
							,SyncOrder
							,SyncDirection
							,MaxBatchSegmentRowCount
							,MaxTransferSegmentKB
							,AdditionalFilterJoinClause
							,AdditionalFilterWhereClause
							,ClientTableNameOverride
							,CreatedBy
							,UpdatedBy)
    ON (existing.SyncTableToScopeMapGuid = updates.SyncTableToScopeMapGuid)
    WHEN Matched
    THEN
        UPDATE SET ID = updates.ID
					,SyncScopeGuid = updates.SyncScopeGuid
					,SyncTableGuid = updates.SyncTableGuid
					,SyncOrder = updates.SyncOrder
					,SyncDirection = updates.SyncDirection
					,MaxBatchSegmentRowCount = updates.MaxBatchSegmentRowCount
					,MaxTransferSegmentKB = updates.MaxTransferSegmentKB
					,AdditionalFilterJoinClause = updates.AdditionalFilterJoinClause
					,AdditionalFilterWhereClause = updates.AdditionalFilterWhereClause
					,ClientTableNameOverride = updates.ClientTableNameOverride
					,UpdatedDate = SYSDATETIMEOFFSET()
					,UpdatedBy = updates.UpdatedBy
    WHEN Not Matched
    THEN
        INSERT (SyncTableToScopeMapGuid
				,ID
				,SyncScopeGuid
				,SyncTableGuid
				,SyncOrder
				,SyncDirection
				,MaxBatchSegmentRowCount
				,MaxTransferSegmentKB
				,AdditionalFilterJoinClause
				,AdditionalFilterWhereClause
				,ClientTableNameOverride
				,CreatedDate
				,CreatedBy
				,UpdatedDate
				,UpdatedBy)
            VALUES (@NewRowGuid
					,@ID
					,@SyncScopeGuid
					,@SyncTableGuid
					,@SyncOrder
					,@SyncDirection
					,@MaxBatchSegmentRowCount
					,@MaxTransferSegmentKB
					,@AdditionalFilterJoinClause
					,@AdditionalFilterWhereClause
					,@ClientTableNameOverride
					,SYSDATETIMEOFFSET()
					,CreatedBy
					,SYSDATETIMEOFFSET()
					,UpdatedBy)
    ;
	
	RETURN;
END