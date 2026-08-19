CREATE PROCEDURE [sync].[usp_SyncRecordConflictSelectUnresolved] (
	@SyncNodeGuid uniqueidentifier,
	@MaxRecords bigint,
	@StartRowVersion bigint
)
AS
BEGIN
	BEGIN TRY
		DECLARE @StartRowVersionVarbinary varbinary(8)
		SET @StartRowVersionVarbinary = CONVERT(varbinary(8), @StartRowVersion);

		SELECT TOP(@MaxRecords) SyncRecordConflictGuid
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
				,CommandText
				,CommandType
				,Retrys
    		FROM [sync].[tblSyncRecordConflict] WITH (NOLOCK)
    		WHERE ((@SyncNodeGuid IS NULL) OR (@SyncNodeGuid IS NOT NULL AND TargetNodeGuid = @SyncNodeGuid))
					AND (SyncConflictResolutionStatusIndex = 0
					OR SyncConflictResolutionStatusIndex = 3)
					AND RecordRowVersion > @StartRowVersionVarbinary
			ORDER BY  RecordRowVersion ASC
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
						+ 'Procedure Name: usp_SyncRecordConflictSelectUnresolved' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END
