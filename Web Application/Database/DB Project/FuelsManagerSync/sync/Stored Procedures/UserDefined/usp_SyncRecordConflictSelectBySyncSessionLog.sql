CREATE PROCEDURE [sync].[usp_SyncRecordConflictSelectBySyncSessionLog](
	@SyncSessionLogGuid uniqueidentifier,
	@MaxRecords bigint,
	@StartRowVersion bigint
)
AS
BEGIN
	BEGIN TRY
		DECLARE @StartRowVersionVarbinary varbinary(8)
		SET @StartRowVersionVarbinary = CONVERT(varbinary(8), @StartRowVersion);



		SELECT TOP(@MaxRecords) src.SyncRecordConflictGuid
				,src.TargetNodeGuid
				,src.TargetNodeName
				,src.TableName
				,src.RecordKey
				,CONVERT(bigint, src.RecordRowVersion) 'RecordRowVersion'
				,CONVERT(bigint, src.ReSyncAnchorMin) 'ReSyncAnchorMin'
				,CONVERT(bigint, src.ReSyncAnchorMax) 'ReSyncAnchorMax'
				,src.SyncConflictTypeIndex
				,src.SyncConflictResolutionStatusIndex
				,src.ResolvedDate
				,src.ResolvedBy
				,src.CreatedDate
				,src.CreatedBy
				,src.UpdatedDate
				,src.UpdatedBy
				,src.ConflictDescription
				,src._RowVersion
				,CommandText
				,CommandType
				,Retrys
			FROM [sync].tblSyncSessionScopeLog sssl WITH (NOLOCK)
				INNER JOIN [sync].[tblSyncRecordConflictToSyncSessionScopeLog] srdssd WITH (NOLOCK)
					ON sssl.[SyncSessionScopeLogGuid] = srdssd.[SyncSessionScopeLogGuid]
	    		INNER JOIN [sync].[tblSyncRecordConflict] src WITH (NOLOCK)
					ON src.[SyncRecordConflictGuid] = srdssd.[SyncRecordConflictGuid]
   			WHERE sssl.SyncSessionLogGuid = @SyncSessionLogGuid AND src.RecordRowVersion > @StartRowVersionVarbinary
			ORDER BY src.RecordRowVersion ASC
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
						+ 'Procedure Name: usp_SyncRecordConflictSelectBySyncSessionLog' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END
