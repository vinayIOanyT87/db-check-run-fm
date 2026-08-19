CREATE PROCEDURE [sync].[usp_SyncRecordConflictSelectByTableAndEntityKey](
    @TableName nvarchar(256)
	,@RecordKey nvarchar(512)
	,@OnlyUnresolved bit
)
AS
BEGIN
	BEGIN TRY
		IF (@OnlyUnresolved IS NOT NULL AND @OnlyUnresolved = 1)
		BEGIN
			SELECT SyncRecordConflictGuid
					,TargetNodeGuid
					,TargetNodeName
					,TableName
					,RecordKey
					,CONVERT(bigint, RecordRowVersion) 'RecordRowVersion'
					,CONVERT(bigint, ReSyncAnchorMin) 'ReSyncAnchorMin'
					,CONVERT(bigint, ReSyncAnchorMax) 'ReSyncAnchorMax'
					,SyncConflictTypeIndex
					,SyncConflictResolutionStatusIndex
					,ResolvedDate
					,ResolvedBy
					,CreatedDate
					,CreatedBy
					,UpdatedDate
					,UpdatedBy
					,ConflictDescription
					,_RowVersion
					,Retrys
    			FROM [sync].[tblSyncRecordConflict] WITH (NOLOCK)
    			WHERE (@TableName IS NOT NULL AND TableName = @TableName)
					AND (@RecordKey IS NOT NULL AND RecordKey = @RecordKey)
					AND (SyncConflictResolutionStatusIndex <> 2)
		END
		ELSE
		BEGIN
			SELECT SyncRecordConflictGuid
					,TargetNodeGuid
					,TargetNodeName
					,TableName
					,RecordKey
					,CONVERT(bigint, RecordRowVersion) 'RecordRowVersion'
					,CONVERT(bigint, ReSyncAnchorMin) 'ReSyncAnchorMin'
					,CONVERT(bigint, ReSyncAnchorMax) 'ReSyncAnchorMax'
					,SyncConflictTypeIndex
					,SyncConflictResolutionStatusIndex
					,ResolvedDate
					,ResolvedBy
					,CreatedDate
					,CreatedBy
					,UpdatedDate
					,UpdatedBy
					,ConflictDescription
					,_RowVersion
    			FROM [sync].[tblSyncRecordConflict] WITH (NOLOCK)
    			WHERE (@TableName IS NOT NULL AND TableName = @TableName)
					AND (@RecordKey IS NOT NULL AND RecordKey = @RecordKey)
		END
	END TRY
	BEGIN CATCH
		DECLARE	@_ErrMessage NVARCHAR(2048)      
				, @_ErrNumber INT           
				, @_ErrProcName NVARCHAR(126)           
				, @_ErrLineNumber INT;            
		SET @_ErrMessage = ERROR_MESSAGE();        
		SET @_ErrNumber = ERROR_NUMBER();        
		SET @_ErrProcName= ERROR_PROCEDURE();        
		SET @_ErrLineNumber = ERROR_LINE();            
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
						+ 'Procedure Name: usp_SyncRecordConflictSelectByTableAndEntityKey' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END
