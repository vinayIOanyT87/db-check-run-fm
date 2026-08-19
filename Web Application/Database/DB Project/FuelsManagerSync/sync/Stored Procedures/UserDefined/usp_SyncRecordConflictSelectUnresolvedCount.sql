CREATE PROCEDURE [sync].[usp_SyncRecordConflictSelectUnresolvedCount] (
	@SyncNodeGuid uniqueidentifier
)
AS
BEGIN
	BEGIN TRY
		SELECT Count(*) AS [Count],MIN(ssl.StartDate) AS [OldestDate]
    		FROM [sync].[tblSyncRecordConflict] src WITH (NOLOCK)
			INNER JOIN [sync].[tblSyncRecordConflictToSyncSessionScopeLog] srctsssl ON srctsssl.SyncRecordConflictGuid = src.SyncRecordConflictGuid
			INNER JOIN [sync].[tblSyncSessionScopeLog] sssl ON sssl.SyncSessionScopeLogGuid = srctsssl.SyncSessionScopeLogGuid
			INNER JOIN [sync].[tblSyncSessionLog] ssl ON ssl.SyncSessionLogGuid = sssl.SyncSessionLogGuid
    		WHERE ((@SyncNodeGuid IS NULL) OR (@SyncNodeGuid IS NOT NULL AND src.TargetNodeGuid = @SyncNodeGuid))
					AND (src.SyncConflictResolutionStatusIndex = 0 OR src.SyncConflictResolutionStatusIndex = 3)
					AND ssl.StartDate IS NOT NULL
					AND ssl.EndDate IS NOT NULL
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
						+ 'Procedure Name: usp_SyncRecordConflictSelectUnresolvedCount' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END
