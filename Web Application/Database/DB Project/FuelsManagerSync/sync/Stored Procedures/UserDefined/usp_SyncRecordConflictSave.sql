CREATE PROCEDURE [sync].[usp_SyncRecordConflictSave](
	@IdentityGuid uniqueidentifier = NULL
	,@TargetNodeGuid uniqueidentifier
	,@TargetNodeName nvarchar(256)
	,@TableName nvarchar(256)    
	,@RecordKey nvarchar(512)
	,@RecordRowVersion bigint
	,@ReSyncAnchorMin bigint
	,@ReSyncAnchorMax bigint
	,@SyncConflictTypeIndex bigint
	,@SyncConflictResolutionStatusIndex bigint
	,@ResolvedDate DateTimeOffset(7)
	,@ResolvedBy nvarchar(100)
	,@SyncSessionScopeLogGuid uniqueidentifier = NULL
	,@CreatedBy nvarchar(100)
	,@UpdatedBy nvarchar(100)
	,@ConflictDescription nvarchar(4000)
	,@CommandText nvarchar(4000)
	,@CommandType bigint
	,@Parameters varbinary(max)
	,@Retrys int
	,@NewRowGuid uniqueidentifier out
)
AS
BEGIN
	DECLARE @newRowFlag bit;
	SET @newRowFlag = 0;

	SET @NewRowGuid = NULL;


	IF (@IdentityGuid IS NULL)
	BEGIN
		SET @NewRowGuid = newid();
		SET @newRowFlag = 1;
	END
	ELSE
		SET @NewRowGuid = @IdentityGuid;
	
    ;   
    MERGE [sync].[tblSyncRecordConflict] AS existing
    USING (SELECT @NewRowGuid
						,@TargetNodeGuid
						,@TargetNodeName
						,@TableName
						,@RecordKey
						,CONVERT(binary(8), @RecordRowVersion)
						,CONVERT(binary(8), @ReSyncAnchorMin)
						,CONVERT(binary(8), @ReSyncAnchorMax)
						,@SyncConflictTypeIndex
						,@ConflictDescription
						,@CommandText
						,@CommandType
						,@Parameters
						,@Retrys
						,@SyncConflictResolutionStatusIndex
						,@ResolvedDate
						,@ResolvedBy
						,@CreatedBy
						,@UpdatedBy
            ) AS updates (SyncRecordConflictGuid
					,TargetNodeGuid
					,TargetNodeName
					,TableName
					,RecordKey
					,RecordRowVersion
					,ReSyncAnchorMin
					,ReSyncAnchorMax
					,SyncConflictTypeIndex
					,ConflictDescription
					,CommandText
					,CommandType
					,Parameters
					,Retrys
					,SyncConflictResolutionStatusIndex
					,ResolvedDate
					,ResolvedBy
					,CreatedBy
					,UpdatedBy)
    ON (existing.SyncRecordConflictGuid = updates.SyncRecordConflictGuid)
    WHEN Matched
    THEN
        UPDATE SET TargetNodeGuid = updates.TargetNodeGuid
					,TargetNodeName = updates.TargetNodeName
					,TableName = updates.TableName
					,RecordKey = updates.RecordKey
					,RecordRowVersion = updates.RecordRowVersion
					,ReSyncAnchorMin = updates.ReSyncAnchorMin
					,ReSyncAnchorMax = updates.ReSyncAnchorMax
					,SyncConflictTypeIndex = updates.SyncConflictTypeIndex
					,ConflictDescription = updates.ConflictDescription
					,CommandText = updates.CommandText
					,CommandType = updates.CommandType
					,Parameters = updates.Parameters
					,Retrys = updates.Retrys
					,SyncConflictResolutionStatusIndex = updates.SyncConflictResolutionStatusIndex
					,ResolvedDate = updates.ResolvedDate
					,ResolvedBy = updates.ResolvedBy
					,UpdatedDate = SYSDATETIMEOFFSET()
					,UpdatedBy = updates.UpdatedBy
    WHEN Not Matched
    THEN
        INSERT (SyncRecordConflictGuid
				,TargetNodeGuid
				,TargetNodeName
				,TableName
				,RecordKey
				,RecordRowVersion
				,ReSyncAnchorMin
				,ReSyncAnchorMax
				,SyncConflictTypeIndex
				,ConflictDescription
				,CommandText
				,CommandType
				,Parameters
				,Retrys
				,SyncConflictResolutionStatusIndex
				,ResolvedDate
				,ResolvedBy
				,CreatedDate
				,CreatedBy
				,UpdatedDate
				,UpdatedBy)
            VALUES (@NewRowGuid
				,@TargetNodeGuid
				,@TargetNodeName
				,@TableName
				,@RecordKey
				,@@DBTS
				,CONVERT(binary(8), @ReSyncAnchorMin)
				,CONVERT(binary(8), @ReSyncAnchorMax)
				,@SyncConflictTypeIndex
				,@ConflictDescription
				,@CommandText
				,@CommandType
				,@Parameters
				,@Retrys
				,@SyncConflictResolutionStatusIndex
				,@ResolvedDate
				,@ResolvedBy
				,SYSDATETIMEOFFSET()
				,CreatedBy
				,SYSDATETIMEOFFSET()
				,UpdatedBy)
    ;


	IF (@newRowFlag = 1	AND @SyncSessionScopeLogGuid IS NOT NULL)
	BEGIN
		SET NOCOUNT ON
		IF NOT EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WITH (NOLOCK) WHERE SyncRecordConflictGuid = @NewRowGuid AND SyncSessionScopeLogGuid = @SyncSessionScopeLogGuid)
		BEGIN
			DECLARE @newId uniqueidentifier;
			DECLARE @outRowGuid uniqueidentifier;

			SET @newId = newid();

			EXEC sp_executesql N'EXEC [sync].[usp_SyncRecordConflictToSyncSessionScopeLogSave] @IdentityGuid, @SyncRecordConflictGuid, @SyncSessionScopeLogGuid, @CreatedBy, @NewRowGuid out'
								,N'@IdentityGuid uniqueidentifier,@SyncRecordConflictGuid uniqueidentifier,@SyncSessionScopeLogGuid uniqueidentifier,@CreatedBy udtUserID,@NewRowGuid uniqueidentifier out'
								,@IdentityGuid = @newId
								,@SyncRecordConflictGuid = @NewRowGuid
								,@SyncSessionScopeLogGuid = @SyncSessionScopeLogGuid
								,@CreatedBy = @CreatedBy
								,@NewRowGuid = @outRowGuid OUTPUT;
		END
 		SET NOCOUNT OFF
	END
		
	RETURN;
END
