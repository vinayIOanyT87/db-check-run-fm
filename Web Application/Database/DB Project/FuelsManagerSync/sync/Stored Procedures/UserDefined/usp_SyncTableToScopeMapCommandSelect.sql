
CREATE PROCEDURE [sync].[usp_SyncTableToScopeMapCommandSelect](
	@SyncTableToScopeMapGuid uniqueidentifier = NULL
	,@IdentityGuid uniqueidentifier = NULL
)
AS
BEGIN
	BEGIN TRY
		IF (@SyncTableToScopeMapGuid IS NULL AND @IdentityGuid IS NULL)
		BEGIN
			RAISERROR('A synchronization scope mapping value must be provided: @SyncTableToScopeMapGuid',16,1);
			RETURN;
		END

		SELECT SyncTableToScopeMapCommandGuid
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
				,UpdatedBy
				,_RowVersion
				FROM sync.tblSyncTableToScopeMapCommand  WITH (NOLOCK)
				WHERE (@SyncTableToScopeMapGuid IS NULL OR SyncTableToScopeMapGuid = @SyncTableToScopeMapGuid)
					AND (@IdentityGuid IS NULL OR SyncTableToScopeMapCommandGuid = @IdentityGuid)
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
						+ 'Procedure Name: usp_SyncTableToScopeMapCommandSelect' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END